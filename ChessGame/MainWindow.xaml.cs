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
        Save[,] safeBoard = new Save[8, 8];
        List<(int,int)> listOfPlayableMoves = new List<(int,int)>();
        List<(int,int)> listOfDangerousMoves = new List<(int,int)>();
        List<Action> listOfCommands = new List<Action>();
        String boardRow;
        selectTileState phase = selectTileState.select;
        turn playerTurn = turn.whitesTurn;
        object currentSender;
        bool isKingInCheck = false;
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

        public class Save
        {
            public string value {  get; set; }
            public Brush color {  get; set; }
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
            //this.KeyDown += new KeyEventHandler(DebugShow);

        }



        /*private void DebugShow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                foreach (var dangerousmoves in listOfDangerousMoves) {
                    ((TextBlock)chessboard.FindName(board[dangerousmoves.Item1, dangerousmoves.Item2].Name)).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFF0000");
                    System.Diagnostics.Debug.WriteLine(playerTurn);
                }
            }

        }*/

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AssignNamesToBoardTiles();
            isKingChecked();
        }

        private void saveTheBoard()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    ((TextBlock)chessboard.FindName(board[i, j].Name)).Text = safeBoard[i, j].value;
                    ((TextBlock)chessboard.FindName(board[i, j].Name)).Foreground = safeBoard[i, j].color;
                }
            }

        }

        private void isTheGameOver()
        {




            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)chessboard.FindName(board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    
                                    break;

                                case turn.blacksTurn:
                                    
                                    break;
                            }
                            break;

                        case "♘":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    whKnightMoves(i,j);
                                    foreach(var possiblePlay in listOfPlayableMoves)
                                    {
                                        ((TextBlock)chessboard.FindName(board[possiblePlay.Item1, possiblePlay.Item2].Name)).Text = ((TextBlock)chessboard.FindName(board[i, j].Name)).Text;
                                        isKingChecked();
                                    }
                                    break;
                                case turn.blacksTurn:
                                    
                                    break;
                            }
                            break;

                        case "♗":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                     
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
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

                        


                        phase = selectTileState.move;
                    }
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
                    switchTurns();
                    isKingChecked();
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
                            if (isInBoundsX(xcords) && isInBoundsY(ycords+1) && !doesTileHaveAChessPiece(xcords, ycords+1) && isTileSafe(board[xcords,ycords+1].Name)){ listOfPlayableMoves.Add((xcords, ycords + 1)); }
                            if (isInBoundsX(xcords+1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords+1, ycords + 1) && isTileSafe(board[xcords+1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords+1, ycords + 1)); }
                            if (isInBoundsX(xcords+1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords+1, ycords) && isTileSafe(board[xcords+1, ycords].Name)) { listOfPlayableMoves.Add((xcords+1, ycords)); }
                            if (isInBoundsX(xcords+1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords+1, ycords - 1) && isTileSafe(board[xcords+1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords+1, ycords - 1)); }
                            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords, ycords - 1) && isTileSafe(board[xcords, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords, ycords - 1)); }
                            if (isInBoundsX(xcords-1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords-1, ycords - 1) && isTileSafe(board[xcords-1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords-1, ycords - 1)); }
                            if (isInBoundsX(xcords-1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords-1, ycords) && isTileSafe(board[xcords-1, ycords].Name)) { listOfPlayableMoves.Add((xcords-1, ycords)); }
                            if (isInBoundsX(xcords-1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords-1, ycords + 1) && isTileSafe(board[xcords-1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords-1, ycords + 1)); }

                            break;
                        case turn.blacksTurn:
                            if (isInBoundsX(xcords) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords, ycords + 1) && isTileSafe(board[xcords, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords, ycords + 1)); }
                            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isTileSafe(board[xcords + 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
                            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords + 1, ycords) && isTileSafe(board[xcords + 1, ycords].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords)); }
                            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 1, ycords - 1) && isTileSafe(board[xcords + 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
                            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords, ycords - 1) && isTileSafe(board[xcords, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords, ycords - 1)); }
                            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 1, ycords - 1) && isTileSafe(board[xcords - 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
                            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords - 1, ycords) && isTileSafe(board[xcords - 1, ycords].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords)); }
                            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isTileSafe(board[xcords - 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }
                            break;
                    }
                    break;

            }
            return null;
        }

        private bool isTileSafe(string selectedTile)
        {
            foreach (var tile in listOfDangerousMoves)
            {
                if (board[tile.Item1,tile.Item2].Name == selectedTile)
                {
                    return false;
                }
            }
            return true;
        }

        private void switchTurns()
        {
            if (playerTurn == turn.whitesTurn)
            {
                playerTurn = turn.blacksTurn;
            }
            else if (playerTurn == turn.blacksTurn)
            {
                playerTurn = turn.whitesTurn;
            }
            System.Diagnostics.Debug.WriteLine(playerTurn);
        }

        private void findAllDangerousTiles()
        {
            listOfDangerousMoves.Clear();
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)chessboard.FindName(board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (isInBoundsX(i + 1) && isInBoundsY(j - 1)) { listOfDangerousMoves.Add((i + 1, j - 1)); }

                                        if (isInBoundsX(i - 1) && isInBoundsY(j - 1)) { listOfDangerousMoves.Add((i - 1, j - 1)); }
                                    }
                                    break;

                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (isInBoundsX(i + 1) && isInBoundsY(j + 1)) { listOfDangerousMoves.Add((i + 1, j + 1)); }

                                        if (isInBoundsX(i - 1) && isInBoundsY(j + 1)) { listOfDangerousMoves.Add((i - 1, j + 1)); }
                                    }
                                    break;
                            }
                            break;

                        case "♘":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        blKnightMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        whKnightMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        blDiagonalBottomLeftMovesDANGER(i, j);
                                        blDiagonalBottomRightMovesDANGER(i, j);
                                        blDiagonalTopLeftMovesDANGER(i, j);
                                        blDiagonalTopRightMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        whDiagonalBottomLeftMovesDANGER(i, j);
                                        whDiagonalBottomRightMovesDANGER(i, j);
                                        whDiagonalTopLeftMovesDANGER(i, j);
                                        whDiagonalTopRightMovesDANGER(i, j);
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        blHorizontalLeftMovesDANGER(i, j);
                                        blHorizontalRightMovesDANGER(i, j);
                                        blVerticalBottomMovesDANGER(i, j);
                                        blVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        whHorizontalLeftMovesDANGER(i, j);
                                        whHorizontalRightMovesDANGER(i, j);
                                        whVerticalBottomMovesDANGER(i, j);
                                        whVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        blDiagonalBottomLeftMovesDANGER(i, j);
                                        blDiagonalBottomRightMovesDANGER(i, j);
                                        blDiagonalTopLeftMovesDANGER(i, j);
                                        blDiagonalTopRightMovesDANGER(i, j);
                                        blHorizontalLeftMovesDANGER(i, j);
                                        blHorizontalRightMovesDANGER(i, j);
                                        blVerticalBottomMovesDANGER(i, j);
                                        blVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        whDiagonalBottomLeftMovesDANGER(i, j);
                                        whDiagonalBottomRightMovesDANGER(i, j);
                                        whDiagonalTopLeftMovesDANGER(i, j);
                                        whDiagonalTopRightMovesDANGER(i, j);
                                        whHorizontalLeftMovesDANGER(i, j);
                                        whHorizontalRightMovesDANGER(i, j);
                                        whVerticalBottomMovesDANGER(i, j);
                                        whVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private string findKing()
        {
            for(int i = 0; i<8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((TextBlock)chessboard.FindName(board[i, j].Name)).Text == "♔")
                    {
                        return board[i, j].Name;
                    }
                }
            }
            return null;
        }

        private void isKingChecked()
        {
            findAllDangerousTiles();

            foreach (var danger in listOfDangerousMoves)
            {
                if (board[danger.Item1,danger.Item2].Name == findKing())
                {
                    isKingInCheck = true;
                    break;
                }
                else
                {
                    isKingInCheck = false;
                }
            }
            

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


        private void whDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { break; }
                else { break; }
            }
        }
        private void whDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        private void whDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whKnightMovesDANGER(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "black") { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "black") { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "black") { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "black") { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "black") { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "black") { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "black") { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "black") { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }

        private void blDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blKnightMovesDANGER(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "white") { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "white") { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "white") { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "white") { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "white") { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "white") { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "white") { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "white") { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }





    }
}

    

