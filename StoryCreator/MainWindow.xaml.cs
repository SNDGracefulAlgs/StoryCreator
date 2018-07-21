using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public MainWindow()
        {
            InitializeComponent();
            CurSolution.FillTestTree();
            CurSolution.CalcTreeLayout();
        }

        private void addHeirSecButton_Click(object sender, RoutedEventArgs e)
        {
            Button newButton = new Button();
            newButton.Height = 250;
            newButton.Width = 250;
            heirsStackPanel.Children.Add(newButton);
        }
    }
}
