using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static ChessGame.Multiplayer;

namespace ChessGame
{
    internal class CGUtil
    {
        public readonly Multiplayer _multiplayer;
        public MovementLogic _movement;
        public Save[,] safeBoard = new Save[8, 8];
        public String boardRow;
        public bool isKingInCheck = false;

        // Constructor and setup
        public CGUtil(Multiplayer main)
        {
            _multiplayer = main;
        }
        public void SetMovement(MovementLogic movement)
        {
            _movement = movement;
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
                    _multiplayer.board[i, j] = new Tile();
                    _multiplayer.board[i, j].Name = boardRow + (j + 1);
                    _multiplayer.board[i, j].DefaultColor = _multiplayer.getTileColorByName(_multiplayer.board[i, j].Name);
                    _multiplayer.board[i, j].didTheFirstMove = false;
                }

            }
        }


        // Check for endgame conditions
        public bool isTheGameTied()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {

                                        if (CalculateAmountOfMoves(() => _movement.whPawnMoves(i, j), i, j)) { return false; }
                                    }
                                    break;

                                case turn.blacksTurn:

                                    break;
                            }
                            break;

                        case "♘":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.whKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.blKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.whBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.blBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.whRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.blRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.whQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.blQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                        case "♔":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.whKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateAmountOfMoves(() => _movement.blKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return true;
        }
        public bool isTheGameOver()
        {

            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whPawnMoves(i, j), i, j)) { return false; }
                                    }
                                    break;

                                case turn.blacksTurn:

                                    break;
                            }
                            break;

                        case "♘":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => _movement.blKnightMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => _movement.blBishopMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => _movement.blRookMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => _movement.blQueenMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                        case "♔":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (CalculateMoves(() => _movement.whKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (CalculateMoves(() => _movement.blKingMoves(i, j), i, j)) { return false; }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return true;
        }

        // Boundary checks
        public bool isInBoundsX(int xcords)
        {
            if (xcords < 8 && xcords >= 0) { return true; }
            return false;
        }
        public bool isInBoundsY(int ycords)
        {
            if (ycords < 8 && ycords >= 0) { return true; }
            return false;
        }

        // Tile and piece checks
        public bool doesTileHaveAChessPiece(int xcords, int ycords)
        {
            if (!isInBoundsY(ycords)) { return false; }
            if (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[xcords, ycords].Name)).Text != "") { return true; }
            return false;
        }
        public string isAChessPieceBlackOrWhite(int xcords, int ycords)
        {
            if (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[xcords, ycords].Name)).Foreground == Brushes.Black)
            {
                return "black";
            }
            if (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[xcords, ycords].Name)).Foreground == Brushes.White)
            {
                return "white";
            }
            return "N/A";

        }
        public void CheckIfMoveIsLegal(Action addMove, int srcxcords, int srcycords, int destxcords, int destycords)
        {
            // check if move puts king in check => move illegal
            saveTheBoard();
            ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[destxcords, destycords].Name)).Text = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[srcxcords, srcycords].Name)).Text;
            ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[srcxcords, srcycords].Name)).Text = "";
            ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[destxcords, destycords].Name)).Foreground = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[srcxcords, srcycords].Name)).Foreground;
            if (!isKingChecked()) { addMove(); loadTheBoard(); return; }
            loadTheBoard();
            return;


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

        // Control over the board state
        public void saveTheBoard()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    safeBoard[i, j] = new Save();
                    safeBoard[i, j].value = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text;
                    safeBoard[i, j].color = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Foreground;
                    _multiplayer.savedTurn = _multiplayer.playerTurn;
                }
            }
        }
        public void loadTheBoard()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text = safeBoard[i, j].value;
                    ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Foreground = safeBoard[i, j].color;
                    _multiplayer.playerTurn = _multiplayer.savedTurn;
                }
            }
        }
        public void switchTurns()
        {
            if (_multiplayer.playerTurn == turn.whitesTurn)
            {
                _multiplayer.playerTurn = turn.blacksTurn;
            }
            else if (_multiplayer.playerTurn == turn.blacksTurn)
            {
                _multiplayer.playerTurn = turn.whitesTurn;
            }
            System.Diagnostics.Debug.WriteLine(_multiplayer.playerTurn);
        }

        // Dangerous tiles and check detection
        public void findAllDangerousTiles()
        {
            _multiplayer.listOfDangerousMoves.Clear();
            var temp = findKingPos();
            ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[temp.Item1, temp.Item2].Name)).Text = "";

            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    switch (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text)
                    {
                        case "♟️":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        if (isInBoundsX(i + 1) && isInBoundsY(j - 1)) { _multiplayer.listOfDangerousMoves.Add((i + 1, j - 1)); }
                                        if (isInBoundsX(i - 1) && isInBoundsY(j - 1)) { _multiplayer.listOfDangerousMoves.Add((i - 1, j - 1)); }
                                    }
                                    break;

                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        if (isInBoundsX(i + 1) && isInBoundsY(j + 1)) { _multiplayer.listOfDangerousMoves.Add((i + 1, j + 1)); }
                                        if (isInBoundsX(i - 1) && isInBoundsY(j + 1)) { _multiplayer.listOfDangerousMoves.Add((i - 1, j + 1)); }
                                    }
                                    break;
                            }
                            break;

                        case "♘":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        _movement.blKnightMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        _movement.whKnightMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;

                        case "♗":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        _movement.blDiagonalBottomLeftMovesDANGER(i, j);
                                        _movement.blDiagonalBottomRightMovesDANGER(i, j);
                                        _movement.blDiagonalTopLeftMovesDANGER(i, j);
                                        _movement.blDiagonalTopRightMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        _movement.whDiagonalBottomLeftMovesDANGER(i, j);
                                        _movement.whDiagonalBottomRightMovesDANGER(i, j);
                                        _movement.whDiagonalTopLeftMovesDANGER(i, j);
                                        _movement.whDiagonalTopRightMovesDANGER(i, j);
                                    }
                                    break;
                            }

                            break;

                        case "♖":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        _movement.blHorizontalLeftMovesDANGER(i, j);
                                        _movement.blHorizontalRightMovesDANGER(i, j);
                                        _movement.blVerticalBottomMovesDANGER(i, j);
                                        _movement.blVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        _movement.whHorizontalLeftMovesDANGER(i, j);
                                        _movement.whHorizontalRightMovesDANGER(i, j);
                                        _movement.whVerticalBottomMovesDANGER(i, j);
                                        _movement.whVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;

                        case "♕":
                            switch (_multiplayer.playerTurn)
                            {
                                case turn.whitesTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "black")
                                    {
                                        _movement.blDiagonalBottomLeftMovesDANGER(i, j);
                                        _movement.blDiagonalBottomRightMovesDANGER(i, j);
                                        _movement.blDiagonalTopLeftMovesDANGER(i, j);
                                        _movement.blDiagonalTopRightMovesDANGER(i, j);
                                        _movement.blHorizontalLeftMovesDANGER(i, j);
                                        _movement.blHorizontalRightMovesDANGER(i, j);
                                        _movement.blVerticalBottomMovesDANGER(i, j);
                                        _movement.blVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                                case turn.blacksTurn:
                                    if (isAChessPieceBlackOrWhite(i, j) == "white")
                                    {
                                        _movement.whDiagonalBottomLeftMovesDANGER(i, j);
                                        _movement.whDiagonalBottomRightMovesDANGER(i, j);
                                        _movement.whDiagonalTopLeftMovesDANGER(i, j);
                                        _movement.whDiagonalTopRightMovesDANGER(i, j);
                                        _movement.whHorizontalLeftMovesDANGER(i, j);
                                        _movement.whHorizontalRightMovesDANGER(i, j);
                                        _movement.whVerticalBottomMovesDANGER(i, j);
                                        _movement.whVerticalTopMovesDANGER(i, j);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[temp.Item1, temp.Item2].Name)).Text = "♔";
        }
        public bool isTileSafe(string selectedTile)
        {
            foreach (var tile in _multiplayer.listOfDangerousMoves)
            {
                if (_multiplayer.board[tile.Item1, tile.Item2].Name == selectedTile)
                {
                    return false;
                }
            }
            return true;
        }

        // Move calculations and validations
        public bool CalculateAmountOfMoves(Action specialcommand, int i, int j)
        {
            saveTheBoard();
            _multiplayer.listOfPlayableMoves.Clear();
            specialcommand();


            if (_multiplayer.listOfPlayableMoves.Count == 0)
            {
                return false;
            }

            return true;
        }
        public bool CalculateMoves(Action specialcommand, int i, int j)
        {
            saveTheBoard();
            _multiplayer.listOfPlayableMoves.Clear();
            specialcommand();

            foreach (var pospla in _multiplayer.listOfPlayableMoves)
            {
                System.Diagnostics.Debug.WriteLine(_multiplayer.board[pospla.Item1, pospla.Item2].Name);
            }
            saveTheBoard();

            foreach (var possiblePlay in _multiplayer.listOfPlayableMoves)
            {
                ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[possiblePlay.Item1, possiblePlay.Item2].Name)).Text = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text;
                ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[possiblePlay.Item1, possiblePlay.Item2].Name)).Foreground = ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Foreground;
                ((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text = "";
                if (!isKingChecked())
                {
                    System.Diagnostics.Debug.WriteLine($"from {_multiplayer.board[i, j].Name} to {_multiplayer.board[possiblePlay.Item1, possiblePlay.Item2].Name}");
                    _multiplayer.playerTurn = turn.whitesTurn;
                    loadTheBoard();
                    return true;
                }
                loadTheBoard();
            }
            return false;
            System.Diagnostics.Debug.WriteLine("KING IS CHECKMATED");
        }

        // King-specific methods
        public string findKing()
        {
            string color = null;
            switch (_multiplayer.playerTurn)
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
                    if (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text == "♔" && isAChessPieceBlackOrWhite(i, j) == color)
                    {
                        return _multiplayer.board[i, j].Name;
                    }
                }
            }
            return null;
        }
        public (int, int) findKingPos()
        {
            string color = null;
            switch (_multiplayer.playerTurn)
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
                    if (((TextBlock)_multiplayer.chessboard.FindName(_multiplayer.board[i, j].Name)).Text == "♔" && isAChessPieceBlackOrWhite(i, j) == color)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }
        public bool isKingChecked()
        {
            findAllDangerousTiles();

            foreach (var danger in _multiplayer.listOfDangerousMoves)
            {
                isKingInCheck = false;
                if (_multiplayer.board[danger.Item1, danger.Item2].Name == findKing())
                {
                    isKingInCheck = true;
                    return isKingInCheck;
                }

            }
            return isKingInCheck;

        }
        public bool doesKingHaveMoves(int xcords, int ycords)
        {
            string color = null;
            switch (_multiplayer.playerTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }
            _multiplayer.listOfPlayableMoves.Clear();
            if (isInBoundsX(xcords) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords, ycords + 1) && isTileSafe(_multiplayer.board[xcords, ycords + 1].Name))) { _multiplayer.listOfPlayableMoves.Add((xcords, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == color) && isTileSafe(_multiplayer.board[xcords + 1, ycords + 1].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords + 1, ycords) || isAChessPieceBlackOrWhite(xcords + 1, ycords) == color) && isTileSafe(_multiplayer.board[xcords + 1, ycords].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (isInBoundsX(xcords + 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords + 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == color) && isTileSafe(_multiplayer.board[xcords + 1, ycords - 1].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (isInBoundsX(xcords) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords, ycords - 1) || isAChessPieceBlackOrWhite(xcords, ycords - 1) == color) && isTileSafe(_multiplayer.board[xcords, ycords - 1].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords - 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords - 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == color) && isTileSafe(_multiplayer.board[xcords - 1, ycords - 1].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords) && (!doesTileHaveAChessPiece(xcords - 1, ycords) || isAChessPieceBlackOrWhite(xcords - 1, ycords) == color) && isTileSafe(_multiplayer.board[xcords - 1, ycords].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (isInBoundsX(xcords - 1) && isInBoundsY(ycords + 1) && (!doesTileHaveAChessPiece(xcords - 1, ycords + 1) || isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == color) && isTileSafe(_multiplayer.board[xcords - 1, ycords + 1].Name)) { _multiplayer.listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }
            if (!_multiplayer.listOfPlayableMoves.Any()) { return false; }
            return true;
        }




    }
}
