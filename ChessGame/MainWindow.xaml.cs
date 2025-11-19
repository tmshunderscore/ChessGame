using System.ComponentModel;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tile[,] board = new Tile[8, 8];
        Save[,] safeBoard = new Save[8, 8];
        List<(int, int)> listOfPlayableMoves = new List<(int, int)>();
        List<(int, int)> listOfDangerousMoves = new List<(int, int)>();
        TaskCompletionSource<string> _inputTaskSource;
        String boardRow;
        selectTileState phase = selectTileState.select;
        turn playerTurn = turn.whitesTurn;
        turn myTurn;
        turn savedTurn;
        object currentSender;
        ChessPos chessPos = new ChessPos();

        bool isKingInCheck = false;
        bool didKingMove = false;
        bool didLeftRookMove = false;
        bool didRightRookMove = false;
        bool isPlayerBlack = false;

        public class ChessPos
        {
            public string hor1 { get; set; }
            public string hor2 { get; set; }
            public string hor3 { get; set; }
            public string hor4 { get; set; }
            public string hor5 { get; set; }
            public string hor6 { get; set; }
            public string hor7 { get; set; }
            public string hor8 { get; set; }

            public string ver1 { get; set; }
            public string ver2 { get; set; }
            public string ver3 { get; set; }
            public string ver4 { get; set; }
            public string ver5 { get; set; }
            public string ver6 { get; set; }
            public string ver7 { get; set; }
            public string ver8 { get; set; }
        }

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
            public string value { get; set; }
            public Brush color { get; set; }
        }
        public class Tile
        {
            public string Name { get; set; }
            public Brush DefaultColor { get; set; }
            public bool didTheFirstMove { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.KeyDown += SwitchTurnsManually;

            bindText();

            // TODO: Implement en passant (later)
        }

        private void bindText()
        {
            DataContext = chessPos;

            switch (isPlayerBlack)
            {
                case true:
                    chessPos.hor1 = "h";
                    chessPos.hor2 = "g";
                    chessPos.hor3 = "f";
                    chessPos.hor4 = "e";
                    chessPos.hor5 = "d";
                    chessPos.hor6 = "c";
                    chessPos.hor7 = "b";
                    chessPos.hor8 = "a";

                    chessPos.ver1 = "8";
                    chessPos.ver2 = "7";
                    chessPos.ver3 = "6";
                    chessPos.ver4 = "5";
                    chessPos.ver5 = "4";
                    chessPos.ver6 = "3";
                    chessPos.ver7 = "2";
                    chessPos.ver8 = "1";
                    break;
                case false:
                    chessPos.hor1 = "a";
                    chessPos.hor2 = "b";
                    chessPos.hor3 = "c";
                    chessPos.hor4 = "d";
                    chessPos.hor5 = "e";
                    chessPos.hor6 = "f";
                    chessPos.hor7 = "g";
                    chessPos.hor8 = "h";

                    chessPos.ver1 = "1";
                    chessPos.ver2 = "2";
                    chessPos.ver3 = "3";
                    chessPos.ver4 = "4";
                    chessPos.ver5 = "5";
                    chessPos.ver6 = "6";
                    chessPos.ver7 = "7";
                    chessPos.ver8 = "8";
                    break;
            }

        }
        private async Task WaitForOtherPlayersResponse()
        {
            // simulating oponents respond time, also debug purposes
            System.Diagnostics.Debug.WriteLine("Waiting for other player's response...");
            await Task.Delay(5000); //instead of this will be a network call to get the other player's move
            System.Diagnostics.Debug.WriteLine("Player has played their move...");
            playerTurn = myTurn;
            //return Task.CompletedTask;
        }

        private void SendMoveDataToOpponent() {
            // here will be the code to send the move data to the opponent over the network
        }

        private async void selectTile(object sender, MouseButtonEventArgs e)
        {
            if (myTurn == playerTurn)
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

                        if (CheckPromotions() != -1)
                        {
                            Promote();

                        }

                        phase = selectTileState.select;
                        if (isKingChecked() && !doesKingHaveMoves(findKingPos().Item1, findKingPos().Item2))
                        {
                            if (isTheGameOver())
                            {
                                System.Diagnostics.Debug.WriteLine("You won!");
                                System.Windows.Application.Current.Shutdown();
                            }
                        }

                        if (!isKingChecked() && !doesKingHaveMoves(findKingPos().Item1, findKingPos().Item2))
                        {
                            if (isTheGameTied())
                            {
                                System.Diagnostics.Debug.WriteLine("You tied");
                                System.Windows.Application.Current.Shutdown();
                            }
                        }
                        SendMoveDataToOpponent();
                        await WaitForOtherPlayersResponse();
                        break;

                }

                

            }

        }
        private async Task Promote()
        {
            PromotionSelection.Visibility = Visibility.Visible;
            _inputTaskSource = new TaskCompletionSource<string>();

            string selectedPiece = await _inputTaskSource.Task;
            System.Diagnostics.Debug.WriteLine($"Player selected: {selectedPiece}");

            switch (selectedPiece)
            {
                case "queen":
                    ((TextBlock)chessboard.FindName(board[CheckPromotions(), 7].Name)).Text = "♕";
                    break;
                case "rook":
                    ((TextBlock)chessboard.FindName(board[CheckPromotions(), 7].Name)).Text = "♖";
                    break;
                case "bishop":
                    ((TextBlock)chessboard.FindName(board[CheckPromotions(), 7].Name)).Text = "♗";
                    break;
                case "knight":
                    ((TextBlock)chessboard.FindName(board[CheckPromotions(), 7].Name)).Text = "♘";
                    break;
            }

            if (isKingChecked() && !doesKingHaveMoves(findKingPos().Item1, findKingPos().Item2))
            {
                if (isTheGameOver())
                {
                    System.Diagnostics.Debug.WriteLine("You lost");
                    System.Windows.Application.Current.Shutdown();
                }
            }

        }

        private int CheckPromotions()
        {
            for (int i = 0; i < 8; i++)
            {
                SolidColorBrush temp;

                if (((TextBlock)chessboard.FindName(board[i, 7].Name)).Text == "♟️" && ((TextBlock)chessboard.FindName(board[i, 7].Name)).Foreground == Brushes.White)
                {
                    return i;
                }
            }
            return -1;
        }

        private void CheckIfMoveIsLegal(Action addMove, int srcxcords, int srcycords, int destxcords, int destycords)
        {
            // check if move puts king in check => move illegal
            saveTheBoard();
            ((TextBlock)chessboard.FindName(board[destxcords, destycords].Name)).Text = ((TextBlock)chessboard.FindName(board[srcxcords, srcycords].Name)).Text;
            ((TextBlock)chessboard.FindName(board[srcxcords, srcycords].Name)).Text = "";
            ((TextBlock)chessboard.FindName(board[destxcords, destycords].Name)).Foreground = ((TextBlock)chessboard.FindName(board[srcxcords, srcycords].Name)).Foreground;
            if (!isKingChecked()) { addMove(); loadTheBoard(); return; }
            loadTheBoard();
            return;


        }


        private void SwitchTurnsManually(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.K)
            {
                switch (myTurn)
                {
                    case turn.whitesTurn:
                        myTurn = turn.blacksTurn;
                        break;

                    case turn.blacksTurn:
                        myTurn = turn.whitesTurn;
                        break;
                }
            }

        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AssignNamesToBoardTiles();
            isKingChecked();
            DecideWhoHasWhichColor();
        }

        private void DecideWhoHasWhichColor()
        {
            switch (isPlayerBlack)
            {
                case false:
                    myTurn = turn.whitesTurn;
                    break;

                case true:
                    myTurn = turn.blacksTurn;
                    break;
            }
            return;
        }

        private void saveTheBoard()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    safeBoard[i, j] = new Save();
                    safeBoard[i, j].value = ((TextBlock)chessboard.FindName(board[i, j].Name)).Text;
                    safeBoard[i, j].color = ((TextBlock)chessboard.FindName(board[i, j].Name)).Foreground;
                    savedTurn = playerTurn;
                }
            }
        }



        private void loadTheBoard()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    ((TextBlock)chessboard.FindName(board[i, j].Name)).Text = safeBoard[i, j].value;
                    ((TextBlock)chessboard.FindName(board[i, j].Name)).Foreground = safeBoard[i, j].color;
                    playerTurn = savedTurn;
                }
            }
        }

        private bool CalculateAmountOfMoves(Action specialcommand, int i, int j)
        {
            saveTheBoard();
            listOfPlayableMoves.Clear();
            specialcommand();


            if (listOfPlayableMoves.Count == 0)
            {
                return false;
            }

            return true;
        }


        private bool CalculateMoves(Action specialcommand, int i, int j)
        {
            saveTheBoard();
            listOfPlayableMoves.Clear();
            specialcommand();

            foreach (var pospla in listOfPlayableMoves)
            {
                System.Diagnostics.Debug.WriteLine(board[pospla.Item1, pospla.Item2].Name);
            }
            saveTheBoard();

            foreach (var possiblePlay in listOfPlayableMoves)
            {
                ((TextBlock)chessboard.FindName(board[possiblePlay.Item1, possiblePlay.Item2].Name)).Text = ((TextBlock)chessboard.FindName(board[i, j].Name)).Text;
                ((TextBlock)chessboard.FindName(board[possiblePlay.Item1, possiblePlay.Item2].Name)).Foreground = ((TextBlock)chessboard.FindName(board[i, j].Name)).Foreground;
                ((TextBlock)chessboard.FindName(board[i, j].Name)).Text = "";
                if (!isKingChecked())
                {
                    System.Diagnostics.Debug.WriteLine($"from {board[i, j].Name} to {board[possiblePlay.Item1, possiblePlay.Item2].Name}");
                    playerTurn = turn.whitesTurn;
                    loadTheBoard();
                    return true;
                }
                loadTheBoard();
            }
            return false;
            System.Diagnostics.Debug.WriteLine("KING IS CHECKMATED");
        }

        private bool isTheGameTied()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)chessboard.FindName(board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whPawnMoves(i, j), i, j)) { return false; }
                                    }
                                    break;

                                case turn.blacksTurn:

                                    break;
                            }
                            break;

                        case "♘":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => blKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => blBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => blRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => blQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                        case "♔":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => whKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => blKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return true;
        }

        private bool isTheGameOver()
        {

            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)chessboard.FindName(board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whPawnMoves(i, j), i, j)) { return false; }
                                    }
                                    break;

                                case turn.blacksTurn:

                                    break;
                            }
                            break;

                        case "♘":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => blKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => blBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => blRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => blQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                        case "♔":
                            switch (myTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => whKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => blKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return true;
        }










        private void MoveChessPiece(object sender, List<(int, int)> listOfPlayableMoves, object previousSender)
        {
            string desiredPlay = ((TextBlock)sender).Name;
            foreach (var play in listOfPlayableMoves)
            {
                if (desiredPlay == board[play.Item1, play.Item2].Name)
                {
                    if (((TextBlock)previousSender).Text == "♔" && FindPosition(sender, board).Item1 == FindPosition(previousSender, board).Item1 - 2)
                    {
                        ((TextBlock)sender).Text = ((TextBlock)previousSender).Text;
                        ((TextBlock)sender).Foreground = ((TextBlock)previousSender).Foreground;
                        ((TextBlock)previousSender).Text = "";
                        ((TextBlock)FindName(board[0, 0].Name)).Text = "";
                        ((TextBlock)FindName(board[3, 0].Name)).Text = "♖";
                        ((TextBlock)FindName(board[3, 0].Name)).Foreground = Brushes.White;

                        switchTurns();
                    }
                    else if (((TextBlock)previousSender).Text == "♔" && FindPosition(sender, board).Item1 == FindPosition(previousSender, board).Item1 + 2)
                    {
                        ((TextBlock)sender).Text = ((TextBlock)previousSender).Text;
                        ((TextBlock)sender).Foreground = ((TextBlock)previousSender).Foreground;
                        ((TextBlock)previousSender).Text = "";
                        ((TextBlock)FindName(board[7, 0].Name)).Text = "";
                        ((TextBlock)FindName(board[5, 0].Name)).Text = "♖";
                        ((TextBlock)FindName(board[5, 0].Name)).Foreground = Brushes.White;
                    }
                    else
                    {
                        ((TextBlock)sender).Text = ((TextBlock)previousSender).Text;
                        ((TextBlock)sender).Foreground = ((TextBlock)previousSender).Foreground;
                        ((TextBlock)previousSender).Text = "";
                        switchTurns();
                    }
                }
            }
            if (((TextBlock)sender).Text == "♟️" && FindPosition(sender, board).Item2 != 1)
            {
                board[FindPosition(previousSender, board).Item1, FindPosition(previousSender, board).Item2].didTheFirstMove = false;
                board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove = true;
            }
            if (((TextBlock)sender).Text == "♔" && FindPosition(sender, board).Item1 != 4 || ((TextBlock)sender).Text == "♔" && FindPosition(sender, board).Item2 != 0)
            {
                didKingMove = true;
            }
            if (((TextBlock)sender).Text == "♖" && FindPosition(previousSender, board).Item1 == 0 && board[FindPosition(previousSender, board).Item1, FindPosition(previousSender, board).Item2] != board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2] && !didLeftRookMove)
            {
                didLeftRookMove = true;
            }
            if (((TextBlock)sender).Text == "♖" && FindPosition(previousSender, board).Item1 == 7 && board[FindPosition(previousSender, board).Item1, FindPosition(previousSender, board).Item2] != board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2] && !didRightRookMove)
            {
                didRightRookMove = true;
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
            //if (!isInBoundsY(ycords+1)) {  return false; }
            if (!isInBoundsY(ycords)) { return false; }
            if (((TextBlock)chessboard.FindName(board[xcords, ycords].Name)).Text != "") { return true; }
            return false;
        }

        private string isAChessPieceBlackOrWhite(int xcords, int ycords)
        {
            if (((TextBlock)chessboard.FindName(board[xcords, ycords].Name)).Foreground == Brushes.Black)
            {
                return "black";
            }
            if (((TextBlock)chessboard.FindName(board[xcords, ycords].Name)).Foreground == Brushes.White)
            {
                return "white";
            }
            return "N/A";

        }

        private void whDiagonalTopLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { break; }
                else { break; }
            }
        }
        private void whDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        private void whDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { break; }
                else { break; }
            }
        }

        private void whVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { break; }
                else { break; }
            }
        }

        private void whKnightMoves(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }

        }

        private void blDiagonalTopLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i) && isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i) && isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i) && isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { break; }
                else { break; }
            }
        }
        private void blDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i) && isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); break; }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords) && isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); break; }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords) && isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i) && isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { break; }
                else { break; }
            }
        }

        private void blVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); break; }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i) && isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { break; }
                else { break; }
            }
        }

        private void blKnightMoves(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }

        }

        private List<(int, int)> whPawnMoveswSender(object sender, int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords + 1)), xcords, ycords, xcords + 1, ycords + 1); }

            // diagonal left chess piece take 
            if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords + 1)), xcords, ycords, xcords - 1, ycords + 1); }

            // move up 1 tile 
            if (doesTileHaveAChessPiece(xcords, ycords + 1)) { return listOfPlayableMoves; }
            CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + 1)), xcords, ycords, xcords, ycords + 1);

            // move up 2 tiles 
            if (doesTileHaveAChessPiece(xcords, ycords + 2)) { return listOfPlayableMoves; }

            if (board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove == false) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + 2)), xcords, ycords, xcords, ycords + 2); }
            return listOfPlayableMoves;
        }
        private List<(int, int)> blPawnMoves(object sender, int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords - 1) && isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords + 1, ycords - 1)), xcords, ycords, xcords + 1, ycords - 1); }

            // diagonal left chess piece take 
            if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords - 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == "white") { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords - 1, ycords - 1)), xcords, ycords, xcords - 1, ycords - 1); }

            // move up 1 tile 
            if (doesTileHaveAChessPiece(xcords, ycords - 1)) { return listOfPlayableMoves; }
            CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - 1)), xcords, ycords, xcords, ycords - 1);

            // move up 2 tiles 
            if (doesTileHaveAChessPiece(xcords, ycords - 2)) { return listOfPlayableMoves; }

            if (board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].didTheFirstMove == false) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords - 2)), xcords, ycords, xcords, ycords - 2); }
            return listOfPlayableMoves;
        }

        private List<(int, int)> ruleset(object sender, int ycords, int xcords)
        {
            listOfPlayableMoves.Clear();

            switch (((TextBlock)sender).Text)
            {

                // pawn moveset
                case "♟️":
                    switch (myTurn)
                    {
                        case turn.whitesTurn:
                            return whPawnMoveswSender(sender, xcords, ycords);
                            break;

                        case turn.blacksTurn:
                            return blPawnMoves(sender, xcords, ycords);
                            break;
                    }
                    break;


                // knight moveset
                case "♘":
                    switch (myTurn)
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
                    switch (myTurn)
                    {
                        case turn.whitesTurn:
                            whBishopMoves(xcords, ycords);
                            break;

                        case turn.blacksTurn:
                            blBishopMoves(xcords, ycords);
                            break;
                    }

                    break;

                // rook moveset
                case "♖":
                    switch (myTurn)
                    {
                        case turn.whitesTurn:
                            whRookMoves(xcords, ycords);
                            break;

                        case turn.blacksTurn:
                            blRookMoves(xcords, ycords);
                            break;
                    }
                    break;

                // queen moveset
                case "♕":
                    switch (myTurn)
                    {
                        case turn.whitesTurn:
                            whQueenMoves(xcords, ycords);
                            break;

                        case turn.blacksTurn:
                            blQueenMoves(xcords, ycords);
                            break;
                    }

                    break;

                // king moveset
                case "♔":
                    switch (myTurn)
                    {
                        case turn.whitesTurn:
                            whKingMoves(xcords, ycords);
                            break;
                        case turn.blacksTurn:
                            blKingMoves(xcords, ycords);
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
                if (board[tile.Item1, tile.Item2].Name == selectedTile)
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
            var temp = findKingPos();
            ((TextBlock)chessboard.FindName(board[temp.Item1, temp.Item2].Name)).Text = "";
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)chessboard.FindName(board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (myTurn)
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
                            switch (myTurn)
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
                            switch (myTurn)
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
                            switch (myTurn)
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
                            switch (myTurn)
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
            ((TextBlock)chessboard.FindName(board[temp.Item1, temp.Item2].Name)).Text = "♔";
        }

        private string findKing()
        {
            string color = null;
            switch (myTurn)
            {
                case turn.whitesTurn:
                    color = "white";
                    break;
                case turn.blacksTurn:
                    color = "black";
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((TextBlock)chessboard.FindName(board[i, j].Name)).Text == "♔" && isAChessPieceBlackOrWhite(i, j) == color)
                    {
                        return board[i, j].Name;
                    }
                }
            }
            return null;
        }

        private (int, int) findKingPos()
        {
            string color = null;
            switch (myTurn)
            {
                case turn.whitesTurn:
                    color = "white";
                    break;
                case turn.blacksTurn:
                    color = "black";
                    break;
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((TextBlock)chessboard.FindName(board[i, j].Name)).Text == "♔" && isAChessPieceBlackOrWhite(i, j) == color)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }

        private bool doesKingHaveMoves(int xcords, int ycords)
        {
            string color = null;
            switch (myTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }
            listOfPlayableMoves.Clear();
            if (isInBoundsX(xcords) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords, ycords + 1) && isTileSafe(board[xcords, ycords + 1].Name))) { listOfPlayableMoves.Add((xcords, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == color) && isTileSafe(board[xcords + 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords + 1, ycords) || isAChessPieceBlackOrWhite(xcords + 1, ycords) == color) && isTileSafe(board[xcords + 1, ycords].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == color) && isTileSafe(board[xcords + 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords, ycords - 1) || isAChessPieceBlackOrWhite(xcords, ycords - 1) == color) && isTileSafe(board[xcords, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == color) && isTileSafe(board[xcords - 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords - 1, ycords) || isAChessPieceBlackOrWhite(xcords - 1, ycords) == color) && isTileSafe(board[xcords - 1, ycords].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == color) && isTileSafe(board[xcords - 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }
            if (!listOfPlayableMoves.Any()) { return false; }
            return true;
        }


        private bool isKingChecked()
        {
            findAllDangerousTiles();

            foreach (var danger in listOfDangerousMoves)
            {
                isKingInCheck = false;
                if (board[danger.Item1, danger.Item2].Name == findKing())
                {
                    isKingInCheck = true;
                    return isKingInCheck;
                }

            }
            return isKingInCheck;

        }

        public void ClearTileColorByPosition(List<(int, int)> position, object sender)
        {
            foreach (var moves in listOfPlayableMoves)
            {
                //((TextBlock)chessboard.FindName(board[moves.Item1,moves.Item2].Name)).Background = getTileColorByName(board[moves.Item1, moves.Item2].Name);

                ((TextBlock)chessboard.FindName(board[moves.Item1, moves.Item2].Name)).Background = board[moves.Item1, moves.Item2].DefaultColor;
            }
            ((TextBlock)sender).Background = board[FindPosition(sender, board).Item1, FindPosition(sender, board).Item2].DefaultColor;

        }

        public void ColorTileByPosition(List<(int, int)> position)
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
                    if (((TextBlock)sender).Name == board[i, j].Name)
                    {
                        return (i, j);
                    }

                }
            }
            return (-1, -1);

        }


        private void whDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else { break; }
            }
        }
        private void whDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else { break; }
            }
        }

        private void whDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else { break; }
            }
        }
        private void whDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else { break; }
            }
        }

        private void whHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else { break; }
            }
        }

        private void whHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else { break; }
            }
        }

        private void whVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else { break; }
            }
        }

        private void whVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else { break; }
            }
        }

        private void whKnightMovesDANGER(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }

        private void blDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords - i, ycords + i)) { listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else { break; }
            }
        }
        private void blDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords + i, ycords + i)) { listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else { break; }
            }
        }

        private void blDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords - i, ycords - i)) { listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else { break; }
            }
        }
        private void blDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords + i, ycords - i)) { listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else { break; }
            }
        }

        private void blHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords + i) && !doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (isInBoundsX(xcords + i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords + i, ycords)) { listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else { break; }
            }
        }

        private void blHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsX(xcords - i) && !doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (isInBoundsX(xcords - i) && isInBoundsY(ycords) && doesTileHaveAChessPiece(xcords - i, ycords)) { listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else { break; }
            }
        }

        private void blVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords + i) && !doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords + i) && doesTileHaveAChessPiece(xcords, ycords + i)) { listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else { break; }
            }
        }

        private void blVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (isInBoundsY(ycords - i) && !doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (isInBoundsX(xcords) && isInBoundsY(ycords - i) && doesTileHaveAChessPiece(xcords, ycords - i)) { listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else { break; }
            }
        }

        private void blKnightMovesDANGER(int xcords, int ycords)
        {
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (isInBoundsX(xcords + 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && !doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 2) && doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords + 1) && doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (isInBoundsX(xcords - 2) && isInBoundsY(ycords - 1) && doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && !doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 2) && doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }

        private void whBishopMoves(int xcords, int ycords)
        {
            whDiagonalTopLeftMoves(xcords, ycords);
            whDiagonalTopRightMoves(xcords, ycords);
            whDiagonalBottomLeftMoves(xcords, ycords);
            whDiagonalBottomRightMoves(xcords, ycords);
        }

        private void whQueenMoves(int xcords, int ycords)
        {
            whDiagonalTopLeftMoves(xcords, ycords);
            whDiagonalTopRightMoves(xcords, ycords);
            whDiagonalBottomLeftMoves(xcords, ycords);
            whDiagonalBottomRightMoves(xcords, ycords);
            whVerticalBottomMoves(xcords, ycords);
            whVerticalTopMoves(xcords, ycords);
            whHorizontalLeftMoves(xcords, ycords);
            whHorizontalRightMoves(xcords, ycords);
        }

        private void whRookMoves(int xcords, int ycords)
        {
            whVerticalBottomMoves(xcords, ycords);
            whVerticalTopMoves(xcords, ycords);
            whHorizontalLeftMoves(xcords, ycords);
            whHorizontalRightMoves(xcords, ycords);
        }

        private void whPawnMoves(int xcords, int ycords)
        {
            // diagonal right chess piece take 
            if (isInBoundsX(xcords + 1) && doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }

            // diagonal left chess piece take 
            if (isInBoundsX(xcords - 1) && doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

            // move up 1 tile 
            if (doesTileHaveAChessPiece(xcords, ycords + 1)) { return; }
            listOfPlayableMoves.Add((xcords, ycords + 1));

            // move up 2 tiles 
            if (doesTileHaveAChessPiece(xcords, ycords + 2)) { return; }
        }

        private void blQueenMoves(int xcords, int ycords)
        {
            blDiagonalTopLeftMoves(xcords, ycords);
            blDiagonalTopRightMoves(xcords, ycords);
            blDiagonalBottomLeftMoves(xcords, ycords);
            blDiagonalBottomRightMoves(xcords, ycords);
            blVerticalBottomMoves(xcords, ycords);
            blVerticalTopMoves(xcords, ycords);
            blHorizontalLeftMoves(xcords, ycords);
            blHorizontalRightMoves(xcords, ycords);
        }

        private void blRookMoves(int xcords, int ycords)
        {
            blVerticalBottomMoves(xcords, ycords);
            blVerticalTopMoves(xcords, ycords);
            blHorizontalLeftMoves(xcords, ycords);
            blHorizontalRightMoves(xcords, ycords);
        }

        private void blBishopMoves(int xcords, int ycords)
        {
            blDiagonalTopLeftMoves(xcords, ycords);
            blDiagonalTopRightMoves(xcords, ycords);
            blDiagonalBottomLeftMoves(xcords, ycords);
            blDiagonalBottomRightMoves(xcords, ycords);
        }

        public void whKingMoves(int xcords, int ycords)
        {
            string color = null;
            switch (myTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }

            if (isInBoundsX(xcords) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords, ycords + 1) && isTileSafe(board[xcords, ycords + 1].Name)) { CheckIfMoveIsLegal(() => listOfPlayableMoves.Add((xcords, ycords + 1)), xcords, ycords, xcords, ycords + 1); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords + 1, ycords + 1) && isTileSafe(board[xcords + 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords + 1, ycords) && isTileSafe(board[xcords + 1, ycords].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords + 1, ycords - 1) && isTileSafe(board[xcords + 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords, ycords - 1) && isTileSafe(board[xcords, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 1) && !doesTileHaveAChessPiece(xcords - 1, ycords - 1) && isTileSafe(board[xcords - 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords) && !doesTileHaveAChessPiece(xcords - 1, ycords) && isTileSafe(board[xcords - 1, ycords].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 1) && !doesTileHaveAChessPiece(xcords - 1, ycords + 1) && isTileSafe(board[xcords - 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

            if (isInBoundsX(xcords) && isInBoundsY(ycords) && !didKingMove)
            {
                if (!didLeftRookMove && !doesTileHaveAChessPiece(xcords - 1, ycords) && !doesTileHaveAChessPiece(xcords - 2, ycords) && !doesTileHaveAChessPiece(xcords - 3, ycords) && isTileSafe(board[xcords - 2, ycords].Name))
                {
                    listOfPlayableMoves.Add((xcords - 2, ycords));
                }
                if (!didRightRookMove && !doesTileHaveAChessPiece(xcords + 1, ycords) && !doesTileHaveAChessPiece(xcords + 2, ycords) && isTileSafe(board[xcords + 2, ycords].Name))
                {
                    listOfPlayableMoves.Add((xcords + 2, ycords));
                }

            }
        }

        private void blKingMoves(int xcords, int ycords)
        {
            string color = null;
            switch (myTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }

            if (isInBoundsX(xcords) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords, ycords + 1) || isAChessPieceBlackOrWhite(xcords, ycords + 1) == color) && isTileSafe(board[xcords, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == color) && isTileSafe(board[xcords + 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords + 1, ycords) || isAChessPieceBlackOrWhite(xcords + 1, ycords) == color) && isTileSafe(board[xcords + 1, ycords].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == color) && isTileSafe(board[xcords + 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords, ycords - 1) || isAChessPieceBlackOrWhite(xcords, ycords - 1) == color) && isTileSafe(board[xcords, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == color) && isTileSafe(board[xcords - 1, ycords - 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords - 1, ycords) || isAChessPieceBlackOrWhite(xcords - 1, ycords) == color) && isTileSafe(board[xcords - 1, ycords].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == color) && isTileSafe(board[xcords - 1, ycords + 1].Name)) { listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }
        }

        private async void Promotion(object sender, MouseButtonEventArgs e)
        {
            string var = null;
            switch (((TextBlock)sender).Text)
            {
                case "♕":
                    var = "queen";
                    break;

                case "♖":
                    var = "rook";
                    break;

                case "♗":
                    var = "bishop";
                    break;

                case "♘":
                    var = "knight";
                    break;
            }
            _inputTaskSource?.TrySetResult(var);
            PromotionSelection.Visibility = Visibility.Hidden;
        }



    }
}



