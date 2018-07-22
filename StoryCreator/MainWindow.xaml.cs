using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoryCreator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage curTreeBitmapImage;
        public MainWindow()
        {
            InitializeComponent();
            CurSolution.FillTestTree();
            CurSolution.CalcTreeLayout();

            Bitmap treeBitMap = new Bitmap(CurSolution.width * 180, CurSolution.height * 100);
            Graphics g = Graphics.FromImage(treeBitMap);
            g.FillRegion(new SolidBrush(System.Drawing.Color.AntiqueWhite), new Region(new System.Drawing.Rectangle(0, 0, CurSolution.width * 180, CurSolution.height * 100)));
            //tree drawing
            DrawTree(g,CurSolution.levelDict);
            g.Dispose();
            curTreeBitmapImage = BitmapToImageSource(treeBitMap);
            treeImage.Source = curTreeBitmapImage;
        }

        private void addHeirSecButton_Click(object sender, RoutedEventArgs e)
        {
            Button newButton = new Button();
            newButton.Height = 250;
            newButton.Width = 250;
        }

        void DrawTree(Graphics g, Dictionary<UInt16, HashSet<Node>> levelDict)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Black,5);
            foreach (HashSet<Node> nodes in levelDict.Values)
                foreach(Node node in nodes)
                g.DrawRectangle(myPen, new System.Drawing.Rectangle((int)(node.coord.X*40),(int)(node.coord.Y*40), 20, 20));
            myPen.Dispose();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
