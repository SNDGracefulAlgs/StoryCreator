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
        static Canvas canvasOnHover;
        static Canvas curManipulatingCanvas;
        static Border curManipCanvBorder;
        static Polyline curDrawingPoyline;

        static Canvas workspaceCanvas;

        static PointF nodeCanvOffsVector;
        static PointF mouseLastPos;

        static Dictionary<Canvas, NodeCanvasData> curNodesOnLayout;

        class NodeCanvasData
        {
            public bool isCanvasChanged;
            public Dictionary<Canvas,TextBlock> heirsTextBlocks;
            public List<Polyline> relations;
            public Ellipse parentPivot;
            public System.Windows.Shapes.Rectangle heirsPivot;
            public Node node;

            public NodeCanvasData(Node node)
            {
                this.node = node;
                relations = new List<Polyline>();
                heirsTextBlocks = new Dictionary<Canvas, TextBlock>();
            }
        }

        public static void Init(MainWindow mWInst, Canvas workspaceCanvas, Node root)
        {
            curNodesOnLayout = new Dictionary<Canvas, NodeCanvasData>();
            VisualWorkspaceHandler.mWInst = mWInst;
            VisualWorkspaceHandler.workspaceCanvas = workspaceCanvas;
            workspaceCanvas.MouseMove += workspaceCanvas_MouseMove;
            workspaceCanvas.MouseRightButtonUp += workspaceCanvas_RightMouseUp;
        } 

        public static void RepaintLayout()
        {

        }

        public static void DrawFrame()
        {
            CreateNewNode(new PointF((float)(workspaceCanvas.ActualWidth * .5f - 100), 50), "branch text",true);
        }

        //создание визуальных элементов узла
        static Canvas CreateNodeCanvas(Node node, PointF nodeCanvasPos)
        {
            //the border, that embrace node canvas
            Border nodeBorder = new Border() {CornerRadius=new CornerRadius(5),BorderThickness=new Thickness(1),BorderBrush = System.Windows.Media.Brushes.Black };

            //canvas, that visually represent corresponding node 
            Canvas nodeCanvas = new Canvas();
            nodeCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(nodeCanvas_LeftMouseDown);
            nodeCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(nodeCanvas_LeftMouseUp);
            nodeCanvas.MouseEnter += new MouseEventHandler(nodeCanvas_MouseEnter);
            nodeCanvas.MouseLeave += new MouseEventHandler(nodeCanvas_MouseLeave);
            nodeCanvas.MouseRightButtonUp += new MouseButtonEventHandler(nodeCanvas_RightMouseUp);
            nodeCanvas.Width = 200;
            nodeCanvas.Height = 100;
            nodeCanvas.Background = System.Windows.Media.Brushes.LightGray;
            workspaceCanvas.Children.Add(nodeBorder);
            nodeBorder.Child = nodeCanvas;
            Canvas.SetTop(nodeBorder, nodeCanvasPos.Y);
            Canvas.SetLeft(nodeBorder, nodeCanvasPos.X);

            //text block, containing the text of the node
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

            //visual separation block
            nodeCanvas.Children.Add(new Line() { X1 = 0, Y1 = 50, X2 = 200, Y2 = 50, Stroke = System.Windows.Media.Brushes.Black });

            //defenition of the button, deleting canvas
            Button deleteNodeButton = new Button();
            deleteNodeButton.Width = 35;
            deleteNodeButton.Height = 35;
            nodeCanvas.Children.Add(deleteNodeButton);
            deleteNodeButton.Click += new RoutedEventHandler(nodeDelButton_Click);
            Canvas.SetRight(deleteNodeButton, 5);
            Canvas.SetTop(deleteNodeButton, 5);
            //del button content
            Canvas delButContCanvas = new Canvas();
            delButContCanvas.Width = deleteNodeButton.Width * .8f;
            delButContCanvas.Height = deleteNodeButton.Height * .8f;
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("C:/Users/Nikita/Source/Repos/StoryCreator/StoryCreator/Images/delIcon.png", UriKind.Absolute));
            delButContCanvas.Background = ib;
            deleteNodeButton.Content = delButContCanvas;

            //defenition of the button, adding new node
            Button addHeirButton = new Button();
            addHeirButton.Width = 35;
            addHeirButton.Height = 35;
            nodeCanvas.Children.Add(addHeirButton);
            addHeirButton.Click += new RoutedEventHandler(nodeAddHeirButton_Click);
            Canvas.SetLeft(addHeirButton, 5);
            Canvas.SetBottom(addHeirButton, 5);
            //add button content
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
            //Canvas.SetZIndex(nodeCanvas,0);
            

            return nodeCanvas;
        }

        #region Events handeling

        private static void canvasRedRect_MouseEnter(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Rectangle redRect = (System.Windows.Shapes.Rectangle)sender;
            Border redRectBorder = (Border)redRect.Parent;
            redRectBorder.BorderBrush = System.Windows.Media.Brushes.Blue;
            redRectBorder.BorderThickness = new Thickness(2);
        }
        private static void canvasRedRect_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Rectangle redRect = (System.Windows.Shapes.Rectangle)sender;
            Border redRectBorder = (Border)redRect.Parent;
            redRectBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            redRectBorder.BorderThickness = new Thickness(1);
        }
        private static void canvasRedRect_LeftMouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Rectangle redRect = (System.Windows.Shapes.Rectangle)sender;
            Canvas createdNodecanvas = CreateNodeHeir(GetCanvasNodeData(GetRectCanvas(redRect)).node, "branch text");
            Border nodeCanvasBorder = (Border)createdNodecanvas.Parent;
            Canvas.SetLeft(nodeCanvasBorder, e.GetPosition(workspaceCanvas).X-createdNodecanvas.Width*.5f);
            Canvas.SetTop(nodeCanvasBorder, e.GetPosition(workspaceCanvas).Y- createdNodecanvas.Height*.75f);

            curManipulatingCanvas = createdNodecanvas;
            curManipCanvBorder = (Border)curManipulatingCanvas.Parent;
            nodeCanvOffsVector = new PointF((float)(e.GetPosition(mWInst.workspaceCanvas).X - Canvas.GetLeft(curManipCanvBorder)), (float)(e.GetPosition(mWInst.workspaceCanvas).Y - Canvas.GetTop(curManipCanvBorder)));
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nodeCanvPositioning;

        }
        private static void canvasRedRect_RightMouseDown(object sender, MouseEventArgs e)
        {
            curManipulatingCanvas = GetRectCanvas((System.Windows.Shapes.Rectangle)sender);
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.fromParentToHeirRelationPositioniong;
        }
        private static void nodeCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Canvas nodeCanvas = (Canvas)sender;
            canvasOnHover = nodeCanvas;
            Border nodeCanvasBorder = (Border)nodeCanvas.Parent;
            nodeCanvasBorder.BorderBrush = System.Windows.Media.Brushes.Blue;
            nodeCanvasBorder.BorderThickness = new Thickness(2);
            switch (mWInst.workspaceCondition)
            {
                case MainWindow.WorkspaceConditions.fromParentToHeirRelationPositioniong:
                    break;
                default:
                    break;
            }
        }
        private static void nodeCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            canvasOnHover = null;
            Canvas nodeCanvas = (Canvas)sender;
            Border nodeCanvasBorder = (Border)nodeCanvas.Parent;
            nodeCanvasBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            nodeCanvasBorder.BorderThickness = new Thickness(1);
        }
        private static void nodeCanvas_LeftMouseDown(object sender, MouseEventArgs e)
        {
            curManipulatingCanvas = (Canvas)sender;
            curManipCanvBorder = (Border)curManipulatingCanvas.Parent;
            Canvas.SetZIndex(curManipulatingCanvas, 0);
            nodeCanvOffsVector = new PointF((float)(e.GetPosition(mWInst.workspaceCanvas).X - Canvas.GetLeft(curManipCanvBorder)), (float)(e.GetPosition(mWInst.workspaceCanvas).Y - Canvas.GetTop(curManipCanvBorder)));
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nodeCanvPositioning;
        }
        private static void nodeCanvas_LeftMouseUp(object sender, MouseEventArgs e)
        {
            curManipCanvBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            curManipCanvBorder.BorderThickness = new Thickness(1);

            workspaceCanvas.Children.Remove(curManipulatingCanvas);
            curManipCanvBorder.Child = curManipulatingCanvas;
            mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nothingHappens;
        }
        private static void nodeCanvas_RightMouseUp(object sender, MouseEventArgs e)
        {
            Canvas nodeCanvas = (Canvas)sender;
            switch (mWInst.workspaceCondition)
            {
                case MainWindow.WorkspaceConditions.fromParentToHeirRelationPositioniong:

                    TextBlock branchTextBlock = InstantiateBranchTextBlock("branch text");

                    NodeCanvasData parentNCDInst = GetCanvasNodeData(curManipulatingCanvas);
                    parentNCDInst.heirsTextBlocks.Add(nodeCanvas, branchTextBlock);

                    GetCanvasNodeData(nodeCanvas).node.AddParent(parentNCDInst.node);
                    parentNCDInst.node.AddHeir("branch text", GetCanvasNodeData(nodeCanvas).node);
                    mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nothingHappens;
                    GetCanvasNodeData(curManipulatingCanvas).isCanvasChanged = true;
                    workspaceCanvas.Children.Remove(curDrawingPoyline);
                    break;
                default:
                    break;
            }
        }
        private static void workspaceCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            mouseLastPos = new PointF((float)(e.GetPosition(workspaceCanvas).X), (float)(e.GetPosition(workspaceCanvas).Y));
            switch (mWInst.workspaceCondition)
            {
                case MainWindow.WorkspaceConditions.fromParentToHeirRelationPositioniong:
                    workspaceCanvas.Children.Remove(curDrawingPoyline);
                    Border curManipCanvBorder = (Border)curManipulatingCanvas.Parent;
                    PointF p0 = new PointF((float)(Canvas.GetLeft(curManipCanvBorder) + .5f * curManipCanvBorder.ActualWidth), (float)(Canvas.GetTop(curManipCanvBorder)));
                    PointF p1 = new PointF((float)(Canvas.GetLeft(curManipCanvBorder) + .5f * curManipCanvBorder.ActualWidth), (float)(Canvas.GetTop(curManipCanvBorder) + curManipCanvBorder.ActualHeight));
                    PointF p2 = mouseLastPos;
                    if (canvasOnHover != null) {
                        Ellipse pivotParentEllipse = GetCanvasNodeData(canvasOnHover).parentPivot;
                        p2 = new PointF((float)(Canvas.GetLeft(pivotParentEllipse)+pivotParentEllipse.ActualWidth*.5f),(float)(Canvas.GetTop(pivotParentEllipse)+pivotParentEllipse.ActualHeight*.5f));
                    }
                    curDrawingPoyline = RelationsDrawer.ConstructPolyline(p0,p1,p2,new PointF {X=p2.X,Y=p2.Y+50 });
                    workspaceCanvas.Children.Add(curDrawingPoyline);
                    break;
                default:
                    break;
            }
        }
        private static void workspaceCanvas_RightMouseUp(object sender, MouseEventArgs e)
        {
            switch (mWInst.workspaceCondition)
            {
                case MainWindow.WorkspaceConditions.fromParentToHeirRelationPositioniong:
                    workspaceCanvas.Children.Remove(curDrawingPoyline);
                    mWInst.workspaceCondition = MainWindow.WorkspaceConditions.nothingHappens;
                    break;
                default:
                    break;
            }
        }
        private static void nodeTextBlock_LeftMouseDown(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("LMB click was handeled at "+ ((TextBlock)sender).Name);
        }
        private static void nodeDelButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas nodeCanvasToDel = (Canvas)((Button)sender).Parent;
            DelNodeFromHeirs(nodeCanvasToDel);
            DelNodeFromParents(nodeCanvasToDel);
            Border nodecanvasBorder = (Border)nodeCanvasToDel.Parent;
            workspaceCanvas.Children.Remove(nodecanvasBorder);
            RemoveNodeShapes(nodeCanvasToDel);
            curNodesOnLayout.Remove(nodeCanvasToDel);
        }
        private static void nodeAddHeirButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNodeHeir(curNodesOnLayout[((Canvas)((Button)sender).Parent)].node, "branch text");
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
            NodeCanvasData nCDinst = GetCanvasNodeData(curManipulatingCanvas);
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
                foreach (Heir heir in node.heirs)
                {
                    heirsCanvases.Add(curNodesOnLayout.Where(el => el.Value.node.Equals(heir.heirNode)).ToArray()[0].Key);
                }
                for (int i = 0; i < heirsCanvases.Count; i++)
                {
                    PointF p0 = new PointF((float)(Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvasBorder.ActualWidth), (float)Canvas.GetTop(nodeCanvasBorder));
                    PointF p1 = new PointF((float)(Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvasBorder.ActualWidth), (float)(Canvas.GetTop(nodeCanvasBorder) + nodeCanvasBorder.ActualHeight + ncDInst.heirsPivot.ActualHeight*.5f));
                    PointF p2 = new PointF((float)(Canvas.GetLeft(((Border)heirsCanvases[i].Parent)) + .5f * ((Border)heirsCanvases[i].Parent).ActualWidth), (float)(Canvas.GetTop(((Border)heirsCanvases[i].Parent))- GetCanvasNodeData(heirsCanvases[i]).parentPivot.ActualHeight*.5f));
                    PointF p3 = new PointF((float)(Canvas.GetLeft(((Border)heirsCanvases[i].Parent)) + .5f * ((Border)heirsCanvases[i].Parent).ActualWidth), (float)(Canvas.GetTop(((Border)heirsCanvases[i].Parent)) + ((Border)heirsCanvases[i].Parent).ActualHeight));
                    Polyline newPLine = RelationsDrawer.ConstructPolyline(p0, p1, p2, p3);
                    
                    int midIndex = (int)(newPLine.Points.Count * .5f);

                    AddArrowsToPolyline(newPLine);

                    System.Windows.Point textBlockPos = newPLine.Points.ToArray()[midIndex];

                    Border tmpBorder = (Border)GetBranchTextBlock(nodeCanvas,heirsCanvases[i]).Parent;
                    Canvas.SetLeft(tmpBorder, textBlockPos.X- tmpBorder.ActualWidth * .5f);
                    Canvas.SetTop(tmpBorder, textBlockPos.Y);

                    ncDInst.relations.Add(newPLine);
                    workspaceCanvas.Children.Add(newPLine);
                }
            }

            Border rectBorder = new Border() { CornerRadius = new CornerRadius(2), BorderThickness = new Thickness(1), BorderBrush = System.Windows.Media.Brushes.Black };

            System.Windows.Shapes.Rectangle heirsPivotRect = new System.Windows.Shapes.Rectangle() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.IndianRed), Width = 25, Height = 25 };
            Canvas.SetLeft(rectBorder, Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvasBorder.ActualWidth - 12.5f);
            Canvas.SetTop(rectBorder, Canvas.GetTop(nodeCanvasBorder) + nodeCanvasBorder.ActualHeight);
            heirsPivotRect.MouseEnter += new MouseEventHandler(canvasRedRect_MouseEnter);
            heirsPivotRect.MouseLeave += new MouseEventHandler(canvasRedRect_MouseLeave);
            heirsPivotRect.MouseLeftButtonDown += new MouseButtonEventHandler(canvasRedRect_LeftMouseDown);
            heirsPivotRect.MouseRightButtonDown += new MouseButtonEventHandler(canvasRedRect_RightMouseDown);
            rectBorder.Child = heirsPivotRect;
            workspaceCanvas.Children.Add(rectBorder);
            ncDInst.heirsPivot = heirsPivotRect;

            Ellipse parentPivotEllipse = new Ellipse() { Stroke = new SolidColorBrush(Colors.Black), Fill = new SolidColorBrush(Colors.CadetBlue), Width = 25, Height = 25 };
            Canvas.SetLeft(parentPivotEllipse, Canvas.GetLeft(nodeCanvasBorder) + .5f * nodeCanvas.Width - 12.5f);
            Canvas.SetTop(parentPivotEllipse, Canvas.GetTop(nodeCanvasBorder) - 25);
            workspaceCanvas.Children.Add(parentPivotEllipse);
            ncDInst.parentPivot = parentPivotEllipse;
        }
        static void RemoveNodeShapes(Canvas nodeCanvas)
        {
            NodeCanvasData ncDInst = curNodesOnLayout[nodeCanvas];
            if (ncDInst.heirsPivot != null)
            {
                Border borderToDel = (Border)ncDInst.heirsPivot.Parent;
                workspaceCanvas.Children.Remove(borderToDel);
                borderToDel = null;
            }
            if (ncDInst.parentPivot != null)
            {
                workspaceCanvas.Children.Remove(ncDInst.parentPivot);
                ncDInst.parentPivot = null;
            }
            List<Polyline> relations = ncDInst.relations;
            if (relations != null && relations.Count > 0)
                for (int i = 0; i < relations.Count; i++)
                {
                    if(relations[i]!=null && relations[i].Points!=null)
                    while(relations[i].Points.Count>0)
                    {
                        relations[i].Points.Remove(relations[i].Points.Take(1).ToArray()[0]);
                    }
                    workspaceCanvas.Children.Remove(relations[i]);
                    relations[i] = null;
                }

        }
        #endregion

        #region executive fucntions
        //return instantiated node
        public static Node CreateNewNode(PointF nodeCanvasPos, string branchText, bool isNodeRoot=false)
        {
            Node newNode = new Node();
            Canvas createdCanvas = CreateNodeCanvas(newNode, nodeCanvasPos);
            newNode.canvas = createdCanvas;
            return newNode;
        }

        public static Canvas CreateNodeHeir(Node parentNode,string branchText)
        {
            Node heirNode = parentNode.AddHeir("new heir's branch text","new heir content");
            heirNode.AddParent(parentNode);
            
            Canvas parentCanvas = parentNode.canvas;
            Border nodeCanvasBorder = (Border)parentCanvas.Parent;
            PointF heirCanvasPos = new PointF((float)Canvas.GetLeft(nodeCanvasBorder),(float)(Canvas.GetTop(nodeCanvasBorder) +parentCanvas.Height+50));
            Canvas heirCanvas = CreateNodeCanvas(heirNode, heirCanvasPos);

            TextBlock branchTextBlock = InstantiateBranchTextBlock(branchText);

            NodeCanvasData parentNCDInst = GetCanvasNodeData(parentCanvas);
            parentNCDInst.heirsTextBlocks.Add(heirCanvas, branchTextBlock);

            return heirCanvas;
        }

        static TextBlock InstantiateBranchTextBlock (string text)
        {
            Border tBBorder = new Border() { CornerRadius = new CornerRadius(5), BorderThickness = new Thickness(1), BorderBrush = System.Windows.Media.Brushes.Black,ClipToBounds=true };
            TextBlock newTextBlock = new TextBlock();
            tBBorder.Child = newTextBlock;
            workspaceCanvas.Children.Add(tBBorder);
            newTextBlock.Background = System.Windows.Media.Brushes.White;
            newTextBlock.Text = text;
            newTextBlock.Width = 100;
            newTextBlock.Height = 25;
            return newTextBlock;
        }

        static NodeCanvasData GetCanvasNodeData(Canvas canvas)
        {
            return curNodesOnLayout[canvas];
        }
        static Canvas GetRectCanvas(System.Windows.Shapes.Rectangle redRect)
        {
            return curNodesOnLayout.Where(el => el.Value.heirsPivot.Equals(redRect)).ToArray()[0].Key;
        }
        static TextBlock GetBranchTextBlock(Canvas parent, Canvas child)
        {
           return curNodesOnLayout[parent].heirsTextBlocks[child];
        }
        static void DelNodeFromParents(Canvas canvNodeToDel)
        {
            HashSet<Node> parentNodes = GetCanvasNodeData(canvNodeToDel).node.parents;
            if (parentNodes.Count > 0)
                foreach (Node parent in parentNodes)
                    if (parent.heirs.Count > 0)
                        foreach (Heir heir in parent.heirs)
                            if (heir.heirNode.canvas.Equals(canvNodeToDel))
                            {
                                parent.heirs.Remove(heir);
                                GetCanvasNodeData(parent.canvas).isCanvasChanged = true;
                                DelNodeFromParents(canvNodeToDel);
                                return;
                            }
        }
        static void DelNodeFromHeirs(Canvas canvNodeToDel)
        {
            Node nodeToDel = GetCanvasNodeData(canvNodeToDel).node;
            if(nodeToDel.heirs!=null && nodeToDel.heirs.Count>0)
            foreach(Heir heir in nodeToDel.heirs)
                heir.heirNode.parents.Remove(nodeToDel);
        }
        #endregion

        #region accesory funcs
        static void AddArrowsToPolyline(Polyline sourceLine)
        {
            int quartIndex = (int)(sourceLine.Points.Count * .25f);
            int threeQuartIndex = (int)(sourceLine.Points.Count * .75f) + 4;

            System.Windows.Point forwPoint = sourceLine.Points.ToArray()[quartIndex];
            System.Windows.Point backPoint = sourceLine.Points.ToArray()[Math.Max(quartIndex - 3, 0)];
            float vLength = (float)Math.Sqrt(Math.Pow(backPoint.X - forwPoint.X, 2) + Math.Pow(backPoint.Y - forwPoint.Y, 2));
            PointF backPointF = new PointF()
            {
                X = (float)(forwPoint.X + 10 * (backPoint.X - forwPoint.X) / vLength),
                Y = (float)(forwPoint.Y + 10 * (backPoint.Y - forwPoint.Y) / vLength)
            };
            PointF backPointL = RotatePointRelative(new PointF { X = (float)forwPoint.X, Y = (float)forwPoint.Y }, backPointF, 30);
            PointF backPointR = RotatePointRelative(new PointF { X = (float)forwPoint.X, Y = (float)forwPoint.Y }, backPointF, -30);

            sourceLine.Points.Insert(quartIndex + 1, new System.Windows.Point { X = backPointL.X, Y = backPointL.Y });
            sourceLine.Points.Insert(quartIndex + 2, forwPoint);
            sourceLine.Points.Insert(quartIndex + 3, new System.Windows.Point { X = backPointR.X, Y = backPointR.Y });
            sourceLine.Points.Insert(quartIndex + 4, forwPoint);

            forwPoint = sourceLine.Points.ToArray()[threeQuartIndex];
            backPoint = sourceLine.Points.ToArray()[Math.Max(threeQuartIndex - 3, 0)];
            backPointF = new PointF()
            {
                X = (float)(forwPoint.X + 10 * (backPoint.X - forwPoint.X) / vLength),
                Y = (float)(forwPoint.Y + 10 * (backPoint.Y - forwPoint.Y) / vLength)
            };
            backPointL = RotatePointRelative(new PointF { X = (float)forwPoint.X, Y = (float)forwPoint.Y }, backPointF, 30);
            backPointR = RotatePointRelative(new PointF { X = (float)forwPoint.X, Y = (float)forwPoint.Y }, backPointF, -30);
            sourceLine.Points.Insert(threeQuartIndex + 1, new System.Windows.Point { X = backPointL.X, Y = backPointL.Y });
            sourceLine.Points.Insert(threeQuartIndex + 2, forwPoint);
            sourceLine.Points.Insert(threeQuartIndex + 3, new System.Windows.Point { X = backPointR.X, Y = backPointR.Y });
            sourceLine.Points.Insert(threeQuartIndex + 4, forwPoint);
        }
        static PointF RotatePointRelative(PointF rotCenter, PointF pointStartCoord, float angle)
        {
            PointF pointResultCoord = new PointF();
            float radAngle = (float)(angle * Math.PI / 180f);
            pointStartCoord.X -= rotCenter.X;
            pointStartCoord.Y -= rotCenter.Y;

            pointResultCoord.X = (float)(pointStartCoord.X * Math.Cos(radAngle) - pointStartCoord.Y * Math.Sin(radAngle));
            pointResultCoord.Y = (float)(pointStartCoord.X * Math.Sin(radAngle) + pointStartCoord.Y * Math.Cos(radAngle));

            pointResultCoord.X += rotCenter.X;
            pointResultCoord.Y += rotCenter.Y;
            return pointResultCoord;
        }
        #endregion

    }
}
