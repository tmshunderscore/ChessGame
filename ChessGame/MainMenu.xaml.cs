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

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        MainWindow mainwindow;
        public MainMenu()
        {
            InitializeComponent();
            mainwindow = (MainWindow)Application.Current.MainWindow;
        }

        private void Singleplayer_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Multiplayer_Click(object sender, RoutedEventArgs e)
        {
            mainwindow.MainContent.Content = new Multiplayer();
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
