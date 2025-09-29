using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tile[,] board = new Tile[8, 8];
        String boardRow;
        class Tile
        {
            public string Name { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AssignNamesToBoardTiles();

        }

        private void selectTile(object sender, MouseButtonEventArgs e)
        {
            //placeholder(this, sender);
            System.Diagnostics.Debug.WriteLine(((TextBlock)sender).Name);
            ((TextBlock)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");
            ruleset(sender);
            //(TextBlock)sender).Name
        }

        private string ruleset(object sender)
        {
            switch (((TextBlock)sender).Text){
                case "♟️":

                break;


                case "♘":

                break;


                case "♗":

                break;


                case "♕":

                break;


                case "♔":

                break;
            }
            return "zero";
        }

        public void AssignNamesToBoardTiles()
        {


            for (int i = 0; i <= 7; i++)
            {
                switch (i)
                {
                    case 0:
                        boardRow = "a";
                        break;

                    case 1:
                        boardRow = "b";
                        break;

                    case 2:
                        boardRow = "c";
                        break;

                    case 3:
                        boardRow = "d";
                        break;

                    case 4:
                        boardRow = "e";
                        break;

                    case 5:
                        boardRow = "f";
                        break;

                    case 6:
                        boardRow = "g";
                        break;

                    case 7:
                        boardRow = "h";
                        break;
                }

                for (int j = 0; j <= 7; j++)
                {
                    board[i, j] = new Tile();
                    board[i, j].Name = boardRow + (j + 1);
                }

            }
        }

        public Tile FindPosition()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {

                }
            }
        }


        public static void placeholder(Visual myvisual, object sender)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myvisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myvisual, i);



                
                    System.Diagnostics.Debug.WriteLine((String)childVisual.GetValue(NameProperty));
                


                //(String)childVisual.GetValue(NameProperty)








                //placeholder(childVisual);
            }
        }
    }
}

    

