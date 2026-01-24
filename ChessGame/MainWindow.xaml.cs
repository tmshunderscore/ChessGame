using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Reflection;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ChessGame
{
    public partial class MainWindow : Window
    {
        


        public MainWindow()
        {
            InitializeComponent();
            Multiplayer multi = new Multiplayer();
            MainMenu menu = new MainMenu();
            MainContent.Content = menu;
        }
        
    }
}



