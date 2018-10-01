using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Drawing;
using System.Windows.Threading;
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
        public enum WorkspaceConditions {onAwakeActions,nodeCanvPositioning,fromParentToHeirRelationPositioniong, fromHeirToParentRelationPositioniong, nothingHappens };
        public WorkspaceConditions workspaceCondition;
        DispatcherTimer actionPumpingTimer;
        
        public MainWindow()
        {
            InitializeComponent();
            workspaceCondition = WorkspaceConditions.onAwakeActions;
            actionPumpingTimer = new DispatcherTimer();
            actionPumpingTimer.Tick += new EventHandler(TimerTickAction);
            actionPumpingTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            actionPumpingTimer.Start();
            
        }

        void TimerTickAction(object sender, EventArgs e)
        {
            switch (workspaceCondition)
            {
                case WorkspaceConditions.nodeCanvPositioning:
                    {
                        VisualWorkspaceHandler.ToPositionTheNodeCanvas();
                        break;
                    }
                case WorkspaceConditions.onAwakeActions:
                    {
                        VisualWorkspaceHandler.Init(this, workspaceCanvas, null);
                        VisualWorkspaceHandler.DrawFrame();
                        workspaceCondition = WorkspaceConditions.nothingHappens;
                        break;
                    }
                case WorkspaceConditions.nothingHappens:
                    {
                        VisualWorkspaceHandler.CheckNodes();
                        break;
                    }
                default: break;
            }
        }

        private void addHeirSecButton_Click(object sender, RoutedEventArgs e)
        {
            Button newButton = new Button();
            newButton.Height = 250;
            newButton.Width = 250;
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
