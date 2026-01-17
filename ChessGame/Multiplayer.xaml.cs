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
    /// Interaction logic for Multiplayer.xaml
    /// </summary>
    public partial class Multiplayer : UserControl
    {
        public Tile[,] board = new Tile[8, 8];
        public List<(int, int)> listOfPlayableMoves = new List<(int, int)>();
        public List<(int, int)> listOfDangerousMoves = new List<(int, int)>();
        public TaskCompletionSource<string> _inputTaskSource;
        public turn savedTurn;
        private turn myTurn;
        public bool didKingMove = false;
        public bool didLeftRookMove = false;
        public bool didRightRookMove = false;
        bool isPlayerBlack = false;
        object currentSender;

        CGUtil util;
        MovementLogic movement;
        ChessPos chessPos = new ChessPos();
        TCPServer tcpServer;
        TCPClient tcpClient;


        enum selectTileState
        {
            select,
            move
        }
        selectTileState phase = selectTileState.select;

        public enum turn
        {
            whitesTurn,
            blacksTurn
        }
        public turn playerTurn = turn.whitesTurn;

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

        public Multiplayer()
        {
            InitializeComponent();
            this.Loaded += Multiplayer_Loaded;
            this.KeyDown += SwitchTurnsManually;
            bindText();
        }

        public void Multiplayer_Loaded(object sender, RoutedEventArgs e)
        {
            tcpClient = new TCPClient();
            tcpServer = new TCPServer();
            util = new CGUtil(this);
            movement = new MovementLogic(util, this);
            util.SetMovement(movement);
            util.AssignNamesToBoardTiles();
            util.isKingChecked();
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
        private void SwitchTurnsManually(object sender, KeyEventArgs e)
        {
            if (Key.NumPad1 == e.Key)
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

            if (Key.NumPad2 == e.Key)
            {
                TCPServer tcpServer = new TCPServer();
            }

            if (Key.NumPad3 == e.Key)
            {
                TCPClient tcpClient = new TCPClient();
            }
        }


        private async Task WaitForOtherPlayersResponse()
        {
            // simulating oponents respond time, also debug purposes

            System.Diagnostics.Debug.WriteLine("Waiting for other player's response...");

            await Task.Delay(5000); //instead of this will be a network call to get the other player's 

            System.Diagnostics.Debug.WriteLine("Player has played their move...");

            playerTurn = myTurn;

            //return Task.CompletedTask;
        }

        private void SendMoveDataToOpponent(String message)
        {
            tcpServer.SendMessage(message);
        }

        private void MoveChessPiece(object sender, List<(int, int)> listOfPlayableMoves, object previousSender)
        {
            string desiredPlay = ((TextBlock)sender).Name;
            foreach (var play in listOfPlayableMoves)
            {
                if (desiredPlay == board[play.Item1, play.Item2].Name)
                {
                    if (((TextBlock)previousSender).Text == "♔" && util.FindPosition(sender, board).Item1 == util.FindPosition(previousSender, board).Item1 - 2)
                    {
                        ((TextBlock)sender).Text = ((TextBlock)previousSender).Text;
                        ((TextBlock)sender).Foreground = ((TextBlock)previousSender).Foreground;
                        ((TextBlock)previousSender).Text = "";
                        ((TextBlock)FindName(board[0, 0].Name)).Text = "";
                        ((TextBlock)FindName(board[3, 0].Name)).Text = "♖";
                        ((TextBlock)FindName(board[3, 0].Name)).Foreground = Brushes.White;
                        util.switchTurns();
                    }

                    else if (((TextBlock)previousSender).Text == "♔" && util.FindPosition(sender, board).Item1 == util.FindPosition(previousSender, board).Item1 + 2)
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
                        util.switchTurns();
                    }
                }
            }

            if (((TextBlock)sender).Text == "♟️" && util.FindPosition(sender, board).Item2 != 1)
            {
                board[util.FindPosition(previousSender, board).Item1, util.FindPosition(previousSender, board).Item2].didTheFirstMove = false;
                board[util.FindPosition(sender, board).Item1, util.FindPosition(sender, board).Item2].didTheFirstMove = true;
            }

            if (((TextBlock)sender).Text == "♔" && util.FindPosition(sender, board).Item1 != 4 || ((TextBlock)sender).Text == "♔" && util.FindPosition(sender, board).Item2 != 0)
            {
                didKingMove = true;
            }

            if (((TextBlock)sender).Text == "♖" && util.FindPosition(previousSender, board).Item1 == 0 && board[util.FindPosition(previousSender, board).Item1, util.FindPosition(previousSender, board).Item2] != board[util.FindPosition(sender, board).Item1, util.FindPosition(sender, board).Item2] && !didLeftRookMove)
            {
                didLeftRookMove = true;
            }

            if (((TextBlock)sender).Text == "♖" && util.FindPosition(previousSender, board).Item1 == 7 && board[util.FindPosition(previousSender, board).Item1, util.FindPosition(previousSender, board).Item2] != board[util.FindPosition(sender, board).Item1, util.FindPosition(sender, board).Item2] && !didRightRookMove)
            {
                didRightRookMove = true;
            }
        }
        private async void selectTile(object sender, MouseButtonEventArgs e)
        {
            if (((TextBlock)sender).Text == "" && phase == selectTileState.select) { return; }

            if (myTurn == playerTurn)
            {
                switch (phase)
                {
                    case selectTileState.select:
                        if (((TextBlock)sender).Foreground == Brushes.White && playerTurn == turn.whitesTurn || ((TextBlock)sender).Foreground == Brushes.Black && playerTurn == turn.blacksTurn)
                        {
                            currentSender = sender;
                            ((TextBlock)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");
                            ColorTileByPosition(movement.ruleset(sender, util.FindPosition(sender, board).Item2, util.FindPosition(sender, board).Item1));
                            phase = selectTileState.move;
                        }
                        break;


                    case selectTileState.move:
                        ClearTileColorByPosition(listOfPlayableMoves, currentSender);
                        MoveChessPiece(sender, listOfPlayableMoves, currentSender);
                        phase = selectTileState.select;
                        if (CheckPromotions() != -1)
                        {
                            Promote();
                        }
                        if (util.isKingChecked() && !util.doesKingHaveMoves(util.findKingPos().Item1, util.findKingPos().Item2))
                        {
                            if (util.isTheGameOver())
                            {
                                System.Diagnostics.Debug.WriteLine("You won!");
                                System.Windows.Application.Current.Shutdown();
                            }
                        }
                        if (!util.isKingChecked() && !util.doesKingHaveMoves(util.findKingPos().Item1, util.findKingPos().Item2))
                        {
                            if (util.isTheGameTied())
                            {
                                System.Diagnostics.Debug.WriteLine("You tied");
                                System.Windows.Application.Current.Shutdown();
                            }
                        }
                        if (((TextBlock)currentSender).Name != ((TextBlock)sender).Name)
                        {
                            SendMoveDataToOpponent(((TextBlock)currentSender).Name + ((TextBlock)sender).Name);
                            await WaitForOtherPlayersResponse();
                        }
                        break;
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

            if (util.isKingChecked() && !util.doesKingHaveMoves(util.findKingPos().Item1, util.findKingPos().Item2))
            {
                if (util.isTheGameOver())
                {
                    System.Diagnostics.Debug.WriteLine("You lost");
                    System.Windows.Application.Current.Shutdown();
                }
            }

        }
        public async void Promotion(object sender, MouseButtonEventArgs e)
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


        public void ClearTileColorByPosition(List<(int, int)> position, object sender)
        {
            foreach (var moves in listOfPlayableMoves)
            {
                ((TextBlock)chessboard.FindName(board[moves.Item1, moves.Item2].Name)).Background = board[moves.Item1, moves.Item2].DefaultColor;
            }

            ((TextBlock)sender).Background = board[util.FindPosition(sender, board).Item1, util.FindPosition(sender, board).Item2].DefaultColor;

        }
        public void ColorTileByPosition(List<(int, int)> position)
        {

            for (int j = 0; j < listOfPlayableMoves.Count; j++)
            {
                object item = chessboard.FindName(board[listOfPlayableMoves[j].Item1, listOfPlayableMoves[j].Item2].Name);
                ((TextBlock)item).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#2f7341");
            }
        }
        public Brush getTileColorByName(string name)
        {
            return ((TextBlock)chessboard.FindName(name)).Background;
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
    }
}
