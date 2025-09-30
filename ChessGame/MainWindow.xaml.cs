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
        List<(int,int)> listOfPlayableMoves = new List<(int,int)>();
        String boardRow;
        public class Tile
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
            //System.Diagnostics.Debug.WriteLine(((TextBlock)sender).Name);
            ((TextBlock)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");


            ColorTileByPosition(ruleset(sender, FindPosition(sender, board).Item2, FindPosition(sender, board).Item1));

            foreach (var moves in listOfPlayableMoves) {
                System.Diagnostics.Debug.WriteLine(moves);
            }
        }

        private List<(int,int)> ruleset(object sender, int ycords, int xcords)
        {
            listOfPlayableMoves.Clear();

            switch (((TextBlock)sender).Text){
                case "♟️":
                    listOfPlayableMoves.Add((xcords,ycords + 1));
                    listOfPlayableMoves.Add((xcords, ycords + 2));
                    return listOfPlayableMoves;
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
            return null;
        }

        public void ColorTileByPosition( List<(int, int)> position)
        {

            for (int j = 0; j < listOfPlayableMoves.Count; j++)
            {
                object item = chessboard.FindName(board[listOfPlayableMoves[j].Item1, listOfPlayableMoves[j].Item2].Name);
                ((TextBlock)item).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#2f7341");
            }
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

         public (int, int) FindPosition(object sender, Tile[,] board)
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    if(((TextBlock)sender).Name == board[i,j].Name)
                    {
                        return (i,j);
                    }
                        
                }
            }
            return (-1,-1);

        }


        public void placeholder(object sender, RoutedEventArgs e)
        {
            object item = chessboard.FindName("b10");
            if (item is TextBlock)
            {
                System.Diagnostics.Debug.WriteLine("I guess it works?");
            }
        }

    }
}

    

