using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    // basic negamax
    public class DefaultEvilBot : IChessBot
    {
        //piece values         .  P    K    B    R    Q     K
        int[] pieceValues = { 0, 100, 300, 310, 500, 900, 10000 };
        int inf = 99999999;
        int maxDepth = 3;
        Move bestMove;

        public Move Think(Board board, Timer timer) {
            EvaluateBoardNegaMax(board, maxDepth, -inf, inf, board.IsWhiteToMove ? 1 : -1);
            return bestMove;
        }

        int EvaluateBoardNegaMax(Board board, int depth, int alpha, int beta, int color) {
            Move[] legalMoves;

            if (board.IsDraw())
                return 0;

            if (depth == 0 || (legalMoves = board.GetLegalMoves()).Length == 0) {
                // EVALUATE
                int sum = 0;

                if (board.IsInCheckmate())
                    return -9999999;

                foreach (PieceList pieces in board.GetAllPieceLists()) {
                    int value = 0;
                    foreach (Piece piece in pieces) {
                        value += pieceValues[(int) pieces.TypeOfPieceInList];
                    }
                    sum += pieces.IsWhitePieceList ? value : -value;
                }
                /*for (int i = 0; ++i < 7;)
                    sum += (board.GetPieceList((PieceType)i, true).Count - board.GetPieceList((PieceType)i, false).Count) * pieceValues[i];*/
                
                //sum += board.GetLegalMoves().Length;
                // EVALUATE

                return color * sum;
            }

            // TREE SEARCH
            int recordEval = int.MinValue;
            foreach (Move move in legalMoves) {
                board.MakeMove(move);
                int evaluation = -EvaluateBoardNegaMax(board, depth - 1, -beta, -alpha, -color);
                if (move.IsCastles) {
                    evaluation += 100;
                }
                board.UndoMove(move);

                if (recordEval < evaluation) {
                    recordEval = evaluation;
                    if (depth == maxDepth)
                        bestMove = move;
                }
                alpha = Math.Max(alpha, recordEval);
                if (alpha >= beta) break;
            }
            // TREE SEARCH

            return recordEval;
        }
    }
}

