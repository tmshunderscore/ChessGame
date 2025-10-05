using System.Reflection;
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
        turn playerTurn = turn.whitesTurn;
        object currentSender;
        enum selectTileState
        {
            select,
            move
        }

        enum turn
        {
            whitesTurn,
            blacksTurn
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
                    if (((TextBlock)sender).Foreground == Brushes.White && playerTurn == turn.whitesTurn || ((TextBlock)sender).Foreground == Brushes.Black && playerTurn == turn.blacksTurn)
                    {


                        currentSender = sender;

                        ((TextBlock)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");


                        ColorTileByPosition(ruleset(sender, FindPosition(sender, board).Item2, FindPosition(sender, board).Item1));

                        foreach (var moves in listOfPlayableMoves)
                        {
                            System.Diagnostics.Debug.WriteLine(moves);
                        }

                        phase = selectTileState.move;
                    }
                        break;

                case selectTileState.move:
                    ClearTileColorByPosition(listOfPlayableMoves, currentSender);
                    MoveChessPiece(sender, listOfPlayableMoves, currentSender);



                    phase = selectTileState.select;

                    if (playerTurn == turn.whitesTurn)
                    {
                        playerTurn = turn.blacksTurn;
                    }
                    else if (playerTurn == turn.blacksTurn)
                    {
                        playerTurn = turn.whitesTurn;
                    }
                    System.Diagnostics.Debug.WriteLine(playerTurn);

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
            if (xcords < 8 && xcords >= 0) { return true; }  
                    return false;
        }

        private bool isInBoundsY(int ycords)
        {
            if (ycords < 8 && ycords >= 0) { return true; }
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

        private void whDiagonalTopLeftMoves(int xcords, int ycords) {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfPlayableMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { listOfPlayableMoves.Add((xcords - i, ycords + i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { break; }
                else {  break; }
            }
        }
        private void whDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfPlayableMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { listOfPlayableMoves.Add((xcords + i, ycords + i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfPlayableMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { listOfPlayableMoves.Add((xcords - i, ycords - i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        private void whDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfPlayableMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { listOfPlayableMoves.Add((xcords + i, ycords - i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfPlayableMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { listOfPlayableMoves.Add((xcords + i, ycords)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfPlayableMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { listOfPlayableMoves.Add((xcords - i, ycords)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfPlayableMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { listOfPlayableMoves.Add((xcords, ycords + i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfPlayableMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { listOfPlayableMoves.Add((xcords, ycords - i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whKnightMoves(int xcords, int ycords)
        {
                if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfPlayableMoves.Add((xcords + 1, ycords + 2)); }
                if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "black") { listOfPlayableMoves.Add((xcords + 1, ycords + 2)); }

                if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfPlayableMoves.Add((xcords + 2, ycords + 1)); }
                if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords + 2, ycords + 1)); }

                if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfPlayableMoves.Add((xcords + 2, ycords - 1)); }
                if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "black") { listOfPlayableMoves.Add((xcords + 2, ycords - 1)); }

                if (isInBoundsX(xcords + 1) && isInBoundsY(ycords -2) && !doesTileHaveAChessPiece(xcords + 1, ycords -2)) { listOfPlayableMoves.Add((xcords + 1, ycords -2)); }
                if (isInBoundsX(xcords + 1) && isInBoundsY(ycords -2) && doesTileHaveAChessPiece(xcords + 1, ycords -2) && isAChessPieceBlackOrWhite(xcords + 1, ycords -2) == "black") { listOfPlayableMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfPlayableMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "black") { listOfPlayableMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfPlayableMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfPlayableMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "black") { listOfPlayableMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfPlayableMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "black") { listOfPlayableMoves.Add((xcords - 1, ycords - 2)); }

        }

        private List<(int,int)> whPawnMoves(object sender,int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }

            // diagonal left chess piece take 
            if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

            // move up 1 tile 
            if (doesTileHaveAChessPiece(xcords, ycords + 1)) { return listOfPlayableMoves; }
            listOfPlayableMoves.Add((xcords, ycords + 1));

            // move up 2 tiles 
            if (doesTileHaveAChessPiece(xcords, ycords + 2)) { return listOfPlayableMoves; }

            if (board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove == false) { listOfPlayableMoves.Add((xcords, ycords + 2)); }
            return listOfPlayableMoves;
        }

        private void blDiagonalTopLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfPlayableMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { listOfPlayableMoves.Add((xcords - i, ycords + i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfPlayableMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { listOfPlayableMoves.Add((xcords + i, ycords + i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfPlayableMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { listOfPlayableMoves.Add((xcords - i, ycords - i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfPlayableMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { listOfPlayableMoves.Add((xcords + i, ycords - i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfPlayableMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { listOfPlayableMoves.Add((xcords + i, ycords)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfPlayableMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { listOfPlayableMoves.Add((xcords - i, ycords)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfPlayableMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { listOfPlayableMoves.Add((xcords, ycords + i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfPlayableMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { listOfPlayableMoves.Add((xcords, ycords - i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blKnightMoves(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfPlayableMoves.Add((xcords + 1, ycords + 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "white") { listOfPlayableMoves.Add((xcords + 1, ycords + 2)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfPlayableMoves.Add((xcords + 2, ycords + 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "white") { listOfPlayableMoves.Add((xcords + 2, ycords + 1)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfPlayableMoves.Add((xcords + 2, ycords - 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "white") { listOfPlayableMoves.Add((xcords + 2, ycords - 1)); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfPlayableMoves.Add((xcords + 1, ycords - 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "white") { listOfPlayableMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfPlayableMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "white") { listOfPlayableMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfPlayableMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "white") { listOfPlayableMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfPlayableMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "white") { listOfPlayableMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfPlayableMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "white") { listOfPlayableMoves.Add((xcords - 1, ycords - 2)); }

        }

        private List<(int, int)> blPawnMoves(object sender, int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == "white") { listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }

            // diagonal left chess piece take 
            if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == "white") { listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }

            // move up 1 tile 
            if (doesTileHaveAChessPiece(xcords, ycords - 1)) { return listOfPlayableMoves; }
            listOfPlayableMoves.Add((xcords, ycords - 1));

            // move up 2 tiles 
            if (doesTileHaveAChessPiece(xcords, ycords - 2)) { return listOfPlayableMoves; }

            if (board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove == false) { listOfPlayableMoves.Add((xcords, ycords - 2)); }
            return listOfPlayableMoves;
        }


        private List<(int,int)> ruleset(object sender, int ycords, int xcords)
        {
            listOfPlayableMoves.Clear();

            switch (((TextBlock)sender).Text){

                // pawn moveset
                case "♟️":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            return whPawnMoves(sender,xcords,ycords);
                            break;

                        case turn.blacksTurn:
                            return blPawnMoves(sender, xcords, ycords);
                            break;
                    }
                break;

                
                // knight moveset
                case "♘":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            whKnightMoves(xcords, ycords);
                        break;

                        case turn.blacksTurn:
                            blKnightMoves(xcords, ycords);
                        break;
                    }
                break;

                // bishop moveset
                case "♗":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            whDiagonalTopLeftMoves(xcords, ycords);
                            whDiagonalTopRightMoves(xcords, ycords);
                            whDiagonalBottomLeftMoves(xcords, ycords);
                            whDiagonalBottomRightMoves(xcords, ycords);
                        break;

                        case turn.blacksTurn:
                            blDiagonalTopLeftMoves(xcords, ycords);
                            blDiagonalTopRightMoves(xcords, ycords);
                            blDiagonalBottomLeftMoves(xcords, ycords);
                            blDiagonalBottomRightMoves(xcords, ycords);
                        break;
                    }

                    break;

                // rook moveset
                case "♖":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            whVerticalBottomMoves(xcords, ycords);
                            whVerticalTopMoves(xcords, ycords);
                            whHorizontalLeftMoves(xcords, ycords);
                            whHorizontalRightMoves(xcords, ycords);
                        break;

                        case turn.blacksTurn:
                            blVerticalBottomMoves(xcords, ycords);
                            blVerticalTopMoves(xcords, ycords);
                            blHorizontalLeftMoves(xcords, ycords);
                            blHorizontalRightMoves(xcords, ycords);
                        break;
                    }
                break;

                // queen moveset
                case "♕":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            whDiagonalTopLeftMoves(xcords, ycords);
                            whDiagonalTopRightMoves(xcords, ycords);
                            whDiagonalBottomLeftMoves(xcords, ycords);
                            whDiagonalBottomRightMoves(xcords, ycords);
                            whVerticalBottomMoves(xcords, ycords);
                            whVerticalTopMoves(xcords, ycords);
                            whHorizontalLeftMoves(xcords, ycords);
                            whHorizontalRightMoves(xcords, ycords);
                        break;

                        case turn.blacksTurn:
                            blDiagonalTopLeftMoves(xcords, ycords);
                            blDiagonalTopRightMoves(xcords, ycords);
                            blDiagonalBottomLeftMoves(xcords, ycords);
                            blDiagonalBottomRightMoves(xcords, ycords);
                            blVerticalBottomMoves(xcords, ycords);
                            blVerticalTopMoves(xcords, ycords);
                            blHorizontalLeftMoves(xcords, ycords);
                            blHorizontalRightMoves(xcords, ycords);
                        break;
                    }

                    break;

                // king moveset
                case "♔":
                    switch (playerTurn)
                    {
                        case turn.whitesTurn:
                            break;
                        case turn.blacksTurn:
                            break;
                    }
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

    

