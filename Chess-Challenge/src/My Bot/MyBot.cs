using ChessChallenge.API;
using System;

//originally based on basic negamax by AugsEU https://github.com/AugsEU/Chess-Challenge/blob/main/Chess-Challenge/src/OtherBots/NegamaxBasic.cs
public class MyBot : IChessBot {
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
		Move[] legalMoves = board.GetLegalMoves();
        Move[] oppoMoves;
        board.MakeMove(Move.NullMove);
        oppoMoves = board.GetLegalMoves();
        board.UndoMove(Move.NullMove);

		if (board.IsDraw())
			return 0;

		if (depth == 0 || legalMoves.Length == 0) {
			// EVALUATE
			int sum = 0;

			if (board.IsInCheckmate())
				return -9999999;

			foreach (PieceList pieces in board.GetAllPieceLists()) {
                int value = 0;
                foreach (Piece piece in pieces) {
                    value += pieceValues[(int) pieces.TypeOfPieceInList];
                    if (piece.IsPawn) {
                        value += piece.IsWhite ? piece.Square.Rank : 7-piece.Square.Rank;
                    }
                }
                sum += pieces.IsWhitePieceList ? value : -value;
            }
            /*for (int i = 0; ++i < 7;)
				sum += (board.GetPieceList((PieceType)i, true).Count - board.GetPieceList((PieceType)i, false).Count) * pieceValues[i];*/
			
            sum += legalMoves.Length*2-oppoMoves.Length*2;
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