using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using static ChessGame.MainWindow;

namespace ChessGame
{
    internal class MovementLogic
    {
        public readonly CGUtil _util;
        public readonly MainWindow _main;
        public MovementLogic(CGUtil util, MainWindow main)
        {
            _util = util;
            _main = main;
        }


        // General movesets and rules
        public List<(int, int)> ruleset(object sender, int ycords, int xcords)
        {
            _main.listOfPlayableMoves.Clear();

            switch (((TextBlock)sender).Text)
            {

                // pawn moveset
                case "♟️":
                    switch (_main.playerTurn)
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
                    switch (_main.playerTurn)
                    {
                        case turn.whitesTurn:
                            whKnightMoves(xcords, ycords);
                            whKnightMoves(xcords, ycords);
                            break;

                        case turn.blacksTurn:
                            blKnightMoves(xcords, ycords);
                            break;
                    }
                    break;

                // bishop moveset
                case "♗":
                    switch (_main.playerTurn)
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
                    switch (_main.playerTurn)
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
                    switch (_main.playerTurn)
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
                    switch (_main.playerTurn)
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


        // Pawn moveset
        public List<(int, int)> whPawnMoveswSender(object sender, int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (_util.isInBoundsX(xcords + 1) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords + 1)), xcords, ycords, xcords + 1, ycords + 1); }

            // diagonal left chess piece take 
            if (_util.isInBoundsX(xcords - 1) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords + 1)), xcords, ycords, xcords - 1, ycords + 1); }

            // move up 1 tile 
            if (_util.doesTileHaveAChessPiece(xcords, ycords + 1)) { return _main.listOfPlayableMoves; }
            _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + 1)), xcords, ycords, xcords, ycords + 1);

            // move up 2 tiles 
            if (_util.doesTileHaveAChessPiece(xcords, ycords + 2)) { return _main.listOfPlayableMoves; }

            if (_main.board[_util.FindPosition(sender, _main.board).Item1, _util.FindPosition(sender, _main.board).Item2].didTheFirstMove == false) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + 2)), xcords, ycords, xcords, ycords + 2); }
            return _main.listOfPlayableMoves;
        }
        public void whPawnMoves(int xcords, int ycords)
        {
            // diagonal right chess piece take 
            if (_util.isInBoundsX(xcords + 1) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == "black") { _main.listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }

            // diagonal left chess piece take 
            if (_util.isInBoundsX(xcords - 1) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == "black") { _main.listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

            // move up 1 tile 
            if (_util.doesTileHaveAChessPiece(xcords, ycords + 1)) { return; }
            _main.listOfPlayableMoves.Add((xcords, ycords + 1));

            // move up 2 tiles 
            if (_util.doesTileHaveAChessPiece(xcords, ycords + 2)) { return; }
        }
        public List<(int, int)> blPawnMoves(object sender, int xcords, int ycords)

        {
            // diagonal right chess piece take 
            if (_util.isInBoundsX(xcords + 1) && _util.doesTileHaveAChessPiece(xcords + 1, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords - 1)), xcords, ycords, xcords + 1, ycords - 1); }

            // diagonal left chess piece take 
            if (_util.isInBoundsX(xcords - 1) && _util.doesTileHaveAChessPiece(xcords - 1, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords - 1)), xcords, ycords, xcords - 1, ycords - 1); }

            // move up 1 tile 
            if (_util.doesTileHaveAChessPiece(xcords, ycords - 1)) { return _main.listOfPlayableMoves; }
            _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - 1)), xcords, ycords, xcords, ycords - 1);

            // move up 2 tiles 
            if (_util.doesTileHaveAChessPiece(xcords, ycords - 2)) { return _main.listOfPlayableMoves; }

            if (_main.board[_util.FindPosition(sender, _main.board).Item1, _util.FindPosition(sender, _main.board).Item2].didTheFirstMove == false) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - 2)), xcords, ycords, xcords, ycords - 2); }
            return _main.listOfPlayableMoves;
        }

        // Bishop moveset
        public void whBishopMoves(int xcords, int ycords)
        {
            whDiagonalTopLeftMoves(xcords, ycords);
            whDiagonalTopRightMoves(xcords, ycords);
            whDiagonalBottomLeftMoves(xcords, ycords);
            whDiagonalBottomRightMoves(xcords, ycords);
        }
        public void blBishopMoves(int xcords, int ycords)
        {
            blDiagonalTopLeftMoves(xcords, ycords);
            blDiagonalTopRightMoves(xcords, ycords);
            blDiagonalBottomLeftMoves(xcords, ycords);
            blDiagonalBottomRightMoves(xcords, ycords);
        }

        // Knight moveset
        public void blKnightMoves(int xcords, int ycords)
        {
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 2) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }

            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords - 2) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }


            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 2) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }

            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords - 2) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }

        }
        public void whKnightMoves(int xcords, int ycords)
        {
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 2) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords + 2) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords + 2)), xcords, ycords, xcords + 1, ycords + 2); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords + 2, ycords + 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords + 1)), xcords, ycords, xcords + 2, ycords + 1); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords + 2, ycords - 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 2, ycords - 1)), xcords, ycords, xcords + 2, ycords - 1); }

            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords - 2) && _util.isAChessPieceBlackOrWhite(xcords + 1, ycords - 2) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + 1, ycords - 2)), xcords, ycords, xcords + 1, ycords - 2); }


            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 2) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords + 2) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords + 2)), xcords, ycords, xcords - 1, ycords + 2); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords + 1) && _util.isAChessPieceBlackOrWhite(xcords - 2, ycords + 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords + 1)), xcords, ycords, xcords - 2, ycords + 1); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords - 1) && _util.isAChessPieceBlackOrWhite(xcords - 2, ycords - 1) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 2, ycords - 1)), xcords, ycords, xcords - 2, ycords - 1); }

            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords - 2) && _util.isAChessPieceBlackOrWhite(xcords - 1, ycords - 2) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - 1, ycords - 2)), xcords, ycords, xcords - 1, ycords - 2); }
        }

        // Queen moveset
        public void whQueenMoves(int xcords, int ycords)
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
        public void blQueenMoves(int xcords, int ycords)
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

        // Rook moveset
        public void whRookMoves(int xcords, int ycords)
        {
            whVerticalBottomMoves(xcords, ycords);
            whVerticalTopMoves(xcords, ycords);
            whHorizontalLeftMoves(xcords, ycords);
            whHorizontalRightMoves(xcords, ycords);
        }
        public void blRookMoves(int xcords, int ycords)
        {
            blVerticalBottomMoves(xcords, ycords);
            blVerticalTopMoves(xcords, ycords);
            blHorizontalLeftMoves(xcords, ycords);
            blHorizontalRightMoves(xcords, ycords);
        }

        // King moveset
        public void whKingMoves(int xcords, int ycords)
        {
            string color = null;
            switch (_main.playerTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }

            if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords, ycords + 1) && _util.isTileSafe(_main.board[xcords, ycords + 1].Name)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + 1)), xcords, ycords, xcords, ycords + 1); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords + 1) && _util.isTileSafe(_main.board[xcords + 1, ycords + 1].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords) && _util.isTileSafe(_main.board[xcords + 1, ycords].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords - 1) && _util.isTileSafe(_main.board[xcords + 1, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords, ycords - 1) && _util.isTileSafe(_main.board[xcords, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords - 1) && _util.isTileSafe(_main.board[xcords - 1, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords) && _util.isTileSafe(_main.board[xcords - 1, ycords].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords + 1) && _util.isTileSafe(_main.board[xcords - 1, ycords + 1].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }

            if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords) && !_main.didKingMove)
            {
                if (!_main.didLeftRookMove && !_util.doesTileHaveAChessPiece(xcords - 1, ycords) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords) && !_util.doesTileHaveAChessPiece(xcords - 3, ycords) && _util.isTileSafe(_main.board[xcords - 2, ycords].Name))
                {
                    _main.listOfPlayableMoves.Add((xcords - 2, ycords));
                }
                if (!_main.didRightRookMove && !_util.doesTileHaveAChessPiece(xcords + 1, ycords) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords) && _util.isTileSafe(_main.board[xcords + 2, ycords].Name))
                {
                    _main.listOfPlayableMoves.Add((xcords + 2, ycords));
                }

            }
        }
        public void blKingMoves(int xcords, int ycords)
        {
            string color = null;
            switch (_main.playerTurn)
            {
                case turn.whitesTurn:
                    color = "black";
                    break;
                case turn.blacksTurn:
                    color = "white";
                    break;
            }

            if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + 1) && (!_util.doesTileHaveAChessPiece(xcords, ycords + 1) || _util.isAChessPieceBlackOrWhite(xcords, ycords + 1) == color) && _util.isTileSafe(_main.board[xcords, ycords + 1].Name)) { _main.listOfPlayableMoves.Add((xcords, ycords + 1)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 1) && (!_util.doesTileHaveAChessPiece(xcords + 1, ycords + 1) || _util.isAChessPieceBlackOrWhite(xcords + 1, ycords + 1) == color) && _util.isTileSafe(_main.board[xcords + 1, ycords + 1].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords + 1)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords) && (!_util.doesTileHaveAChessPiece(xcords + 1, ycords) || _util.isAChessPieceBlackOrWhite(xcords + 1, ycords) == color) && _util.isTileSafe(_main.board[xcords + 1, ycords].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 1) && (!_util.doesTileHaveAChessPiece(xcords + 1, ycords - 1) || _util.isAChessPieceBlackOrWhite(xcords + 1, ycords - 1) == color) && _util.isTileSafe(_main.board[xcords + 1, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords + 1, ycords - 1)); }
            if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - 1) && (!_util.doesTileHaveAChessPiece(xcords, ycords - 1) || _util.isAChessPieceBlackOrWhite(xcords, ycords - 1) == color) && _util.isTileSafe(_main.board[xcords, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 1) && (!_util.doesTileHaveAChessPiece(xcords - 1, ycords - 1) || _util.isAChessPieceBlackOrWhite(xcords - 1, ycords - 1) == color) && _util.isTileSafe(_main.board[xcords - 1, ycords - 1].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords) && (!_util.doesTileHaveAChessPiece(xcords - 1, ycords) || _util.isAChessPieceBlackOrWhite(xcords - 1, ycords) == color) && _util.isTileSafe(_main.board[xcords - 1, ycords].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 1) && (!_util.doesTileHaveAChessPiece(xcords - 1, ycords + 1) || _util.isAChessPieceBlackOrWhite(xcords - 1, ycords + 1) == color) && _util.isTileSafe(_main.board[xcords - 1, ycords + 1].Name)) { _main.listOfPlayableMoves.Add((xcords - 1, ycords + 1)); }
        }


        // Moves in a specific direction
        public void whDiagonalTopLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { break; }
                else { break; }
            }
        }
        public void whDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { break; }
                else { break; }
            }
        }
        public void whDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        public void whDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        public void whHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { break; }
                else { break; }
            }
        }
        public void whHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { break; }
                else { break; }
            }
        }
        public void whVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); break; }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { break; }
                else { break; }
            }
        }
        public void whVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); break; }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { break; }
                else { break; }
            }
        }
        public void blDiagonalTopLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords + i)), xcords, ycords, xcords - i, ycords + i); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        public void blDiagonalTopRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords + i)), xcords, ycords, xcords + i, ycords + i); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        public void blDiagonalBottomLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords - i)), xcords, ycords, xcords - i, ycords - i); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords - i) == "black") { break; }
                else { break; }
            }
        }
        public void blDiagonalBottomRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords - i)), xcords, ycords, xcords + i, ycords - i); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords - i) == "black") { break; }
                else { break; }
            }
        }
        public void blHorizontalRightMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords + i, ycords)), xcords, ycords, xcords + i, ycords); break; }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords) && _util.isAChessPieceBlackOrWhite(xcords + i, ycords) == "black") { break; }
                else { break; }
            }
        }
        public void blHorizontalLeftMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords - i, ycords)), xcords, ycords, xcords - i, ycords); break; }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords) && _util.isAChessPieceBlackOrWhite(xcords - i, ycords) == "black") { break; }
                else { break; }
            }
        }
        public void blVerticalTopMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords, ycords + i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords, ycords + i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords + i)), xcords, ycords, xcords, ycords + i); break; }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i) && _util.isAChessPieceBlackOrWhite(xcords, ycords + i) == "black") { break; }
                else { break; }
            }
        }
        public void blVerticalBottomMoves(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords, ycords - i)) { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords, ycords - i) == "white") { _util.CheckIfMoveIsLegal(() => _main.listOfPlayableMoves.Add((xcords, ycords - i)), xcords, ycords, xcords, ycords - i); break; }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i) && _util.isAChessPieceBlackOrWhite(xcords, ycords - i) == "black") { break; }
                else { break; }
            }
        }


        // Moves in a specific direction that indicate dangerous tiles
        public void whDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else { break; }
            }
        }
        public void whDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else { break; }
            }
        }
        public void whDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else { break; }
            }
        }
        public void whDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else { break; }
            }
        }
        public void whHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords)) { _main.listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords)) { _main.listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else { break; }
            }
        }
        public void whHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords)) { _main.listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords)) { _main.listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else { break; }
            }
        }
        public void whVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords, ycords + i)) { _main.listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i)) { _main.listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else { break; }
            }
        }
        public void whVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords, ycords - i)) { _main.listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i)) { _main.listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else { break; }
            }
        }
        public void whKnightMovesDANGER(int xcords, int ycords)
        {
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }
        public void blDiagonalTopLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords + i)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords - i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords + i)); break; }
                else { break; }
            }
        }
        public void blDiagonalTopRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords + i)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords + i, ycords + i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords + i)); break; }
                else { break; }
            }
        }
        public void blDiagonalBottomLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords - i)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords - i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords - i, ycords - i)); break; }
                else { break; }
            }
        }
        public void blDiagonalBottomRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords - i)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords + i, ycords - i)) { _main.listOfDangerousMoves.Add((xcords + i, ycords - i)); break; }
                else { break; }
            }
        }
        public void blHorizontalRightMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords + i) && !_util.doesTileHaveAChessPiece(xcords + i, ycords)) { _main.listOfDangerousMoves.Add((xcords + i, ycords)); }
                else if (_util.isInBoundsX(xcords + i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords + i, ycords)) { _main.listOfDangerousMoves.Add((xcords + i, ycords)); break; }
                else { break; }
            }
        }
        public void blHorizontalLeftMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsX(xcords - i) && !_util.doesTileHaveAChessPiece(xcords - i, ycords)) { _main.listOfDangerousMoves.Add((xcords - i, ycords)); }
                else if (_util.isInBoundsX(xcords - i) && _util.isInBoundsY(ycords) && _util.doesTileHaveAChessPiece(xcords - i, ycords)) { _main.listOfDangerousMoves.Add((xcords - i, ycords)); break; }
                else { break; }
            }
        }
        public void blVerticalTopMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords + i) && !_util.doesTileHaveAChessPiece(xcords, ycords + i)) { _main.listOfDangerousMoves.Add((xcords, ycords + i)); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords + i) && _util.doesTileHaveAChessPiece(xcords, ycords + i)) { _main.listOfDangerousMoves.Add((xcords, ycords + i)); break; }
                else { break; }
            }
        }
        public void blVerticalBottomMovesDANGER(int xcords, int ycords)
        {
            for (int i = 1; i < 8; i++)
            {
                if (_util.isInBoundsY(ycords - i) && !_util.doesTileHaveAChessPiece(xcords, ycords - i)) { _main.listOfDangerousMoves.Add((xcords, ycords - i)); }
                else if (_util.isInBoundsX(xcords) && _util.isInBoundsY(ycords - i) && _util.doesTileHaveAChessPiece(xcords, ycords - i)) { _main.listOfDangerousMoves.Add((xcords, ycords - i)); break; }
                else { break; }
            }
        }
        public void blKnightMovesDANGER(int xcords, int ycords)
        {
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords + 2)); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords + 1)); }

            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }
            if (_util.isInBoundsX(xcords + 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords + 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords + 2, ycords - 1)); }

            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }
            if (_util.isInBoundsX(xcords + 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords + 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords + 1, ycords - 2)); }


            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords + 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords + 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords + 2)); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords + 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords + 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords + 1)); }

            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && !_util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }
            if (_util.isInBoundsX(xcords - 2) && _util.isInBoundsY(ycords - 1) && _util.doesTileHaveAChessPiece(xcords - 2, ycords - 1)) { _main.listOfDangerousMoves.Add((xcords - 2, ycords - 1)); }

            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && !_util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }
            if (_util.isInBoundsX(xcords - 1) && _util.isInBoundsY(ycords - 2) && _util.doesTileHaveAChessPiece(xcords - 1, ycords - 2)) { _main.listOfDangerousMoves.Add((xcords - 1, ycords - 2)); }

        }










    }
}
