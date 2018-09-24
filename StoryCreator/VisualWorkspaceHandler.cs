using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Input;
using System.Threading.Tasks;

namespace StoryCreator
{
    public static class VisualWorkspaceHandler
    {
        static MainWindow mWInst;
        static Canvas curManipulatingCanvas;
        static Border curManipCanvBorder;
        static Canvas workspaceCanvas;

        static PointF nodeCanvOffsVector;
        static PointF mouseLastPos;

        static Dictionary<Canvas, NodeCanvasData> curNodesOnLayout;

        class NodeCanvasData
        {
            public bool isCanvasChanged;
            public List<Polyline> relations;
            public Ellipse parentPivot;
            public System.Windows.Shapes.Rectangle heirsPivot;
            public Node node;

            public NodeCanvasData(Node node)
            {
                this.node = node;
                relations = new List<Polyline>();
            }
        }

        public static void Init(MainWindow mWInst, Canvas workspaceCanvas, Node root)
        {
            curNodesOnLayout = new Dictionary<Canvas, NodeCanvasData>();
            VisualWorkspaceHandler.mWInst = mWInst;
            VisualWorkspaceHandler.workspaceCanvas = workspaceCanvas;
            workspaceCanvas.MouseMove += workspaceCanvas_MouseMove;
        } 

        public static void RepaintLayout()
        {

        }

        public static void DrawFrame()
        {
            CreateNewNode(new PointF((float)(workspaceCanvas.ActualWidth * .5f - 100), 50));
        }

        static void CreateNodeCanvas(Node node, PointF nodeCanvasPos)
        {
            Border nodeBorder = new Border() {CornerRadius=new CornerRadius(5),BorderThickness=new Thickness(1),BorderBrush = System.Windows.Media.Brushes.Black };
            Canvas nodeCanvas = new Canvas();
            nodeCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(nodeCanvas_LeftMouseDown);
            nodeCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(nodeCanvas_LeftMouseUp);

            nodeCanvas.MouseEnter += new MouseEventHandler(nodeCanvas_MouseEnter);
            nodeCanvas.MouseLeave += new MouseEventHandler(nodeCanvas_MouseLeave);

            nodeCanvas.Width = 200;
            nodeCanvas.Height = 100;
            nodeCanvas.Background = System.Windows.Media.Brushes.LightGray;

            workspaceCanvas.Children.Add(nodeBorder);
            nodeBorder.Child = nodeCanvas;

            Canvas.SetTop(nodeBorder, nodeCanvasPos.Y);
            Canvas.SetLeft(nodeBorder, nodeCanvasPos.X);

            TextBlock demoTextBlock = new TextBlock();
            demoTextBlock.Text = "The first few lines of the node content";
            demoTextBlock.TextWrapping = TextWrapping.Wrap;
            nodeCanvas.Children.Add(demoTextBlock);
            demoTextBlock.Width = 150;
            demoTextBlock.Height = 35;
            demoTextBlock.Background = System.Windows.Media.Brushes.White;
            demoTextBlock.MouseLeftButtonDown += new MouseButtonEventHandler(nodeTextBlock_LeftMouseDown);
            Canvas.SetLeft(demoTextBlock, 5);
            Canvas.SetTop(demoTextBlock, 5);

            nodeCanvas.Children.Add(new Line() { X1 = 0, Y1 = 50, X2 = 200, Y2 = 50, Stroke = System.Windows.Media.Brushes.Black });

            Button deleteNodeButton = new Button();
            deleteNodeButton.Width = 35;
            deleteNodeButton.Height = 35;
            nodeCanvas.Children.Add(deleteNodeButton);
            deleteNodeButton.Click += new RoutedEventHandler(nodeDelButton_Click);
            Canvas.SetRight(deleteNodeButton, 5);
            Canvas.SetTop(deleteNodeButton, 5);

            Canvas delButContCanvas = new Canvas();
            delButContCanvas.Width = deleteNodeButton.Width * .8f;
            delButContCanvas.Height = deleteNodeButton.Height * .8f;
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("C:/Users/Nikita/Source/Repos/StoryCreator/StoryCreator/Images/delIcon.png", UriKind.Absolute));
            delButContCanvas.Background = ib;
            deleteNodeButton.Content = delButContCanvas;

            Button addHeirButton = new Button();
            addHeirButton.Width = 35;
            addHeirButton.Height = 35;
            nodeCanvas.Children.Add(addHeirButton);
            addHeirButton.Click += new RoutedEventHandler(nodeAddHeirButton_Click);
            Canvas.SetLeft(addHeirButton, 5);
            Canvas.SetBottom(addHeirButton, 5);

            Canvas addHeirButContCanvas = new Canvas();
            addHeirButContCanvas.Width = addHeirButton.Width * .8f;
            addHeirButContCanvas.Height = addHeirButton.Height * .8f;
            ImageBrush ib1 = new ImageBrush();
            ib1.ImageSource = new BitmapImage(new Uri("C:/Users/Nikita/Source/Repos/StoryCreator/StoryCreator/Images/addIcon.png", UriKind.Absolute));
            addHeirButContCanvas.Background = ib1;
            addHeirButton.Content = addHeirButContCanvas;

            NodeCanvasData nCDInst = new NodeCanvasData(node);
            node.canvas = nodeCanvas;

            nCDInst.isCanvasChanged = true;
            curNodesOnLayout.Add(nodeCanvas, nCDInst);
        }

        #region Events handler
        private static void nodeCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            curManipulatingCanvas = (Canvas)sender;
            curManipCanvBorder = (Border)curManipulatingCanvas.Parent;
            curManipCanvBorder.BorderBrush = System.Windows.Media.Brushes.Aquamarine;
            curManipCanvBorder.BorderThickness = new Thickness(2);
        }
        private static void nodeCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            curManipulatingCanvas = (Canvas)sender;
            curManipCanvBorder = (Border)curManipulatingCanvas.Parent;
            curManipCanvBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            curManipCanvBorder.BorderThickness = new Thickness(1);
        }
        private static void workspaceCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            mouseLastPos = new PointF((float)(e.GetPosition(workspaceCanvas).X), (float)(e.GetPosition(workspaceCanvas).Y));
        }
        private static void nodeCanvas_LeftMouseDown(object sender, MouseEventArgs e)
        {
            curManipulatingCanvas = (Canvas)sender;
            curManipCanvBorder = (Border)curManipulatingCanvas.Parent;

            curManipCanvBorder.BorderBrush = System.Windows.Media.Brushes.Blue;
            curManipCanvBorder.BorderThickness = new Thickness(2);
            nodeCanvOffsVector = new PointF((float)(e.GetPosition(mWInst.workspaceCanvas).X - Canvas.GetLeft(curManipCanvBorder)), (float)(e.GetPosition(mWInst.workspaceCanvas).Y - Canvas.GetTop(curManipCanvBorder)));
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nodeCanvPositioning;
            /*
                        curManipCanvBorder.Child = null;
                        workspaceCanvas.Children.Add(curManipulatingCanvas);



                        workspaceCanvas.Children.Remove(curManipulatingCanvas);
                        curManipCanvBorder.Child = curManipulatingCanvas;
            */
        }
        private static void nodeCanvas_LeftMouseUp(object sender, MouseEventArgs e)
        {
            curManipCanvBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            curManipCanvBorder.BorderThickness = new Thickness(1);

            workspaceCanvas.Children.Remove(curManipulatingCanvas);
            curManipCanvBorder.Child = curManipulatingCanvas;
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nothingHappens;
        }
        private static void nodeTextBlock_LeftMouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("LMB click was handeled at "+ ((TextBlock)sender).Name);
        }
        private static void nodeDelButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas nodeCanvasToDel = (Canvas)((Button)sender).Parent;
            Border nodecanvasBorder = (Border)nodeCanvasToDel.Parent;
            workspaceCanvas.Children.Remove(nodecanvasBorder);
            RemoveNodeShapes(nodeCanvasToDel);
            curNodesOnLayout.Remove(nodeCanvasToDel);
        }
        private static void nodeAddHeirButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNodeHeir(curNodesOnLayout[((Canvas)((Button)sender).Parent)].node);
        }
        #endregion

        #region controls functions
        //>>>
        //calls from MainWindow
        public static void ToPositionTheNodeCanvas()
        {
            CheckNodes();
            float x= mouseLastPos.X - nodeCanvOffsVector.X;
            float y= mouseLastPos.Y - nodeCanvOffsVector.Y;
            NodeCanvasData nCDinst = curNodesOnLayout[curManipulatingCanvas];
            nCDinst.isCanvasChanged = IsNodeCanvasPosChanged(curManipulatingCanvas, new PointF(x, y));
            Canvas.SetLeft(curManipCanvBorder, x);
            Canvas.SetTop(curManipCanvBorder, y);

            foreach (Node parentNode in nCDinst.node.parents)
                GetCanvasNodeData(parentNode.canvas).isCanvasChanged=true;
            

        }
        public static void CheckNodes()
        {
            foreach( KeyValuePair<Canvas,NodeCanvasData> pair in curNodesOnLayout)
            
                if (pair.Value.isCanvasChanged)
                {
                    RepaintNodeShapes(pair.Key);
                    pair.Value.isCanvasChanged = false;
                }
        }
        //<<<
        static bool IsNodeCanvasPosChanged(Canvas nodeCanvas,PointF newPoint)
        {
            if (Canvas.GetLeft(nodeCanvas) == newPoint.X && Canvas.GetTop(nodeCanvas) == newPoint.Y) return false;
            return true;
        }
        static void RepaintNodeShapes(Canvas nodeCanvas)
        {
            Border nodeCanvasBorder = (Border)nodeCanvas.Parent;
            
            RemoveNodeShapes(nodeCanvas);
            NodeCanvasData ncDInst = GetCanvasNodeData(nodeCanvas);
            Node node = ncDInst.node;

            if (node.heirs != null && node.heirs.Count > 0)
            {
                List<Canvas> heirsCanvases = new List<Canvas>();
                //здесь должна быть прорисовка связей
                foreach (Heir heir in node.heirs)
                {
                    heirsCanvases.Add(curNodesOnLayout.Where(el => el.Value.node.Equals(heir.heirNode)).ToArray()[0].Key);
                }
                for (int i = 0; i < heirsCanvases.Count; i++)
                {
                    PointF p0 = new PointF((float)(Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvasBorder.ActualWidth), (float)Canvas.GetTop(nodeCanvasBorder));
                    PointF p1 = new PointF((float)(Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvasBorder.ActualWidth), (float)(Canvas.GetTop(nodeCanvasBorder) + nodeCanvasBorder.ActualHeight));
                    PointF p2 = new PointF((float)(Canvas.GetLeft(((Border)heirsCanvases[i].Parent)) + .5f * ((Border)heirsCanvases[i].Parent).ActualWidth), (float)Canvas.GetTop(((Border)heirsCanvases[i].Parent)));
                    PointF p3 = new PointF((float)(Canvas.GetLeft(((Border)heirsCanvases[i].Parent)) + .5f * ((Border)heirsCanvases[i].Parent).ActualWidth), (float)(Canvas.GetTop(((Border)heirsCanvases[i].Parent)) + ((Border)heirsCanvases[i].Parent).ActualHeight));
                    Polyline newPLine = RelationsDrawer.ConstructPolyline(p0, p1, p2, p3);
                    ncDInst.relations.Add(newPLine);
                    workspaceCanvas.Children.Add(newPLine);
                }
            }

            System.Windows.Shapes.Rectangle heirsPivotRect = new System.Windows.Shapes.Rectangle() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Red), Width = 25, Height = 25 };
            Canvas.SetLeft(heirsPivotRect, Canvas.GetLeft(nodeCanvasBorder) + .5f *nodeCanvasBorder.ActualWidth - 12.5f);
            Canvas.SetTop(heirsPivotRect, Canvas.GetTop(nodeCanvasBorder) + nodeCanvasBorder.ActualHeight);
            workspaceCanvas.Children.Add(heirsPivotRect);
            ncDInst.heirsPivot = heirsPivotRect;

            Ellipse parentPivotEllipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.Blue), Width = 25, Height = 25 };
            Canvas.SetLeft(parentPivotEllipse, Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvas.Width - 12.5f);
            Canvas.SetTop(parentPivotEllipse, Canvas.GetTop(nodeCanvasBorder) - 25);
            workspaceCanvas.Children.Add(parentPivotEllipse);
            ncDInst.parentPivot = parentPivotEllipse;
        }
        static void RemoveNodeShapes(Canvas nodeCanvas)
        {
            NodeCanvasData ncDInst = curNodesOnLayout[nodeCanvas];
            if (ncDInst.heirsPivot != null) workspaceCanvas.Children.Remove(ncDInst.heirsPivot);
            if (ncDInst.parentPivot != null) workspaceCanvas.Children.Remove(ncDInst.parentPivot);
            List<Polyline> relations = ncDInst.relations;
            if (relations != null && relations.Count > 0)
                for (int i = 0; i < relations.Count; i++)
                    workspaceCanvas.Children.Remove(relations[i]);
        }
        #endregion

        #region executive fucntions
        //return instantiated node
        public static Node CreateNewNode(PointF nodeCanvasPos)
        {
            Node newNode = new Node();
            CreateNodeCanvas(newNode, nodeCanvasPos);
            return newNode;
        }

        public static void CreateNodeHeir(Node parentNode)
        {            Node heirNode = parentNode.AddHeir("new heir's branch text","new heir content");
            heirNode.AddParent(parentNode);
            Canvas parentCanvas = curNodesOnLayout.Where(el => el.Value.node.Equals(parentNode)).ToArray()[0].Key;
            Border nodeCanvasBorder = (Border)parentCanvas.Parent;
            PointF heirCanvasPos = new PointF((float)Canvas.GetLeft(nodeCanvasBorder),(float)(Canvas.GetTop(nodeCanvasBorder) +parentCanvas.Height+50));
            CreateNodeCanvas(heirNode, heirCanvasPos);
        }

        static NodeCanvasData GetCanvasNodeData(Canvas canvas)
        {
            return curNodesOnLayout[canvas];
        }
        #endregion

    }
}
