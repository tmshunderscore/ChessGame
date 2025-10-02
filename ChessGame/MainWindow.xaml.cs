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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        selectTileState phase = selectTileState.select;
        object currentSender;
        enum selectTileState
        {
            select,
            move
        }



        public class Tile
        {
            public string Name { get; set; }
            public Brush DefaultColor { get; set; }
            public bool didTheFirstMove {  get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }
        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AssignNamesToBoardTiles();
            
        }






        private void selectTile(object sender, MouseButtonEventArgs e)
        {
            if (((TextBlock)sender).Text == "" && phase == selectTileState.select) { return; }

            switch (phase)
            {
                
                case selectTileState.select:
                    currentSender = sender;

                    ((TextBlock)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");


                    ColorTileByPosition(ruleset(sender, FindPosition(sender, board).Item2, FindPosition(sender, board).Item1));

                    foreach (var moves in listOfPlayableMoves)
                    {
                        System.Diagnostics.Debug.WriteLine(moves);
                    }

                    phase = selectTileState.move;
                    break;

                case selectTileState.move:
                    ClearTileColorByPosition(listOfPlayableMoves, currentSender);
                    MoveChessPiece(sender, listOfPlayableMoves, currentSender);



                    phase = selectTileState.select;
                    break;
            }

            
        }


        private void MoveChessPiece(object sender, List<(int, int)> listOfPlayableMoves, object previousSender)
        {
            string desiredPlay = ((TextBlock)sender).Name;
            foreach (var play in listOfPlayableMoves) { 
                if (desiredPlay == board[play.Item1,play.Item2].Name)
                {
                    ((TextBlock)sender).Text = ((TextBlock)previousSender).Text;
                    ((TextBlock)sender).Foreground = ((TextBlock)previousSender).Foreground;
                    ((TextBlock)previousSender).Text = "";
                }
            }
            if(((TextBlock)sender).Text == "♟️")
            {
                board[FindPosition(previousSender, board).Item1, FindPosition(previousSender, board).Item2].didTheFirstMove = false;
                board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove = true;
            }
            
        }



        private bool isInBoundsX(int xcords)
        {
            if (xcords < 8 && xcords > 0) { return true; }  
                    return false;
        }

        private bool doesTileHaveAChessPiece(int xcords, int ycords)
        {
            if (((TextBlock)chessboard.FindName(board[xcords, ycords].Name)).Text != "") { return true; }
            return false;
        }

        private string isAChessPieceBlackOrWhite(int xcords, int ycords) {
        if  (((TextBlock) chessboard.FindName(board[xcords, ycords].Name)).Foreground == Brushes.Black)
            {
                return "black";
            }
        if (((TextBlock)chessboard.FindName(board[xcords, ycords].Name)).Foreground == Brushes.White)
            {
                return "white";
            }
            return "N/A";

        }

        private List<(int,int)> ruleset(object sender, int ycords, int xcords)
        {
            listOfPlayableMoves.Clear();

            switch (((TextBlock)sender).Text){

                // pawn moveset
                case "♟️":
                    // diagonal right chess piece take 
                    if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords+1,ycords+1) == "black") { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }

                    // diagonal left chess piece take 
                    if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

                    // move up 1 tile 
                    if (doesTileHaveAChessPiece(xcords,ycords+1)) {return listOfPlayableMoves; }
                    listOfPlayableMoves.Add((xcords,ycords + 1));

                    // move up 2 tiles 
                    if (doesTileHaveAChessPiece(xcords, ycords + 2)) { return listOfPlayableMoves; }

                    if (board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove == false) { listOfPlayableMoves.Add((xcords, ycords + 2)); }

                    return listOfPlayableMoves;
                break;

                
                // knight moveset
                case "♘":

                break;

                // bishop moveset
                case "♗":

                break;

                // rook moveset
                case "♖":

                break;

                // queen moveset
                case "♕":

                break;

                // king moveset
                case "♔":

                break;

            }
            return null;
        }




        public void ClearTileColorByPosition(List<(int,int)> position, object sender)
        {
            foreach (var moves in listOfPlayableMoves)
            {
                //((TextBlock)chessboard.FindName(board[moves.Item1,moves.Item2].Name)).Background = getTileColorByName(board[moves.Item1, moves.Item2].Name);
                
                ((TextBlock)chessboard.FindName(board[moves.Item1, moves.Item2].Name)).Background = board[moves.Item1, moves.Item2].DefaultColor;
            }
            ((TextBlock)sender).Background = board[FindPosition(sender, board).Item1,FindPosition(sender,board).Item2].DefaultColor;

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
                    board[i, j].DefaultColor = getTileColorByName(board[i, j].Name);
                    board[i, j].didTheFirstMove = false;
                }

            }
        }




        private Brush getTileColorByName(string name)
        {
            return ((TextBlock)chessboard.FindName(name)).Background;
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






        

    }
}

    

