using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectFourLogic.Board;

namespace ConnectFourLogic.Strategies
{
    class HardLevelStrategy : IGameStrategy
    {
        private readonly IGameBoard _board;

        private Player _aiPlayer;
        private Player _humanPlayer;

        public HardLevelStrategy(IGameBoard board)
        {
            _board = board;
        }

        public GameStrategyLevel GetLevel()
        {
            return GameStrategyLevel.Hard;
        }

        public (int, int) Play(
            Player currentPlayer,
            Player opponentPlayer)
        {
            _aiPlayer = currentPlayer;
            _humanPlayer = opponentPlayer;

            int bestScore = int.MinValue;
            int bestColumn = -1;

            foreach (var column in _board.ColumnIndices())
            {
                if (_board.IsColumnFull(column))
                    continue;

                int row = _board.DropDisc(column, currentPlayer);

                int score = MiniMax(
                    depth: 4,
                    maximizing: false,
                    lastColumn: column,
                    lastRow: row);

                _board.RemoveDisc(column, row);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestColumn = column;
                }
            }

            int playedRow =
                _board.DropDisc(bestColumn, currentPlayer);

            return (bestColumn, playedRow);
        }

        /// <summary>
        /// MiniMax本体
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="maximizing"></param>
        /// <param name="lastColumn"></param>
        /// <param name="lastRow"></param>
        /// <returns></returns>
        private int MiniMax(
            int depth,
            bool maximizing,
            int lastColumn,
            int lastRow)
        {
            var lastCell =
                new BoardCell(lastColumn, lastRow);

            if (_board.HasPlayerWon(
                _aiPlayer,
                lastCell))
            {
                return 1000 + depth;
            }

            if (_board.HasPlayerWon(
                _humanPlayer,
                lastCell))
            {
                return -1000 - depth;
            }

            if (depth == 0 || _board.IsFull())
            {
                return EvaluateBoard();
            }

            if (maximizing)
            {
                int best = int.MinValue;

                foreach (var col in _board.ColumnIndices())
                {
                    if (_board.IsColumnFull(col))
                        continue;

                    int row =
                        _board.DropDisc(col, _aiPlayer);

                    int score =
                        MiniMax(depth - 1,
                            false,
                            col,
                            row);

                    _board.RemoveDisc(col, row);

                    best = Math.Max(best, score);
                }

                return best;
            }
            else
            {
                int best = int.MaxValue;

                foreach (var col in _board.ColumnIndices())
                {
                    if (_board.IsColumnFull(col))
                        continue;

                    int row =
                        _board.DropDisc(col, _humanPlayer);

                    int score =
                        MiniMax(depth - 1,
                            true,
                            col,
                            row);

                    _board.RemoveDisc(col, row);

                    best = Math.Min(best, score);
                }

                return best;
            }
        }

        ///Minimax の末端評価として、
        private int EvaluateBoard()
        {
            int score = 0;

            score += EvaluateCenter();

            score += EvaluateHorizontal();
            score += EvaluateVertical();
            score += EvaluatePositiveDiagonal();
            score += EvaluateNegativeDiagonal();

            return score;
        }

        ///中央列評価
        private int EvaluateCenter()
        {
            int score = 0;

            int centerColumn =
                _board.GetColumnLength() / 2;

            foreach (var row in _board.RowIndices())
            {
                var player =
                    _board.GetPlayerAtCell(
                        centerColumn,
                        row);

                if (player == null)
                    continue;

                if (player.Color == _aiPlayer.Color)
                    score += 6;
                else
                    score -= 6;
            }

            return score;
        }

        ///横評価
        private int EvaluateHorizontal()
        {
            int score = 0;

            for (int row = 0;
                 row < _board.GetRowLength();
                 row++)
            {
                for (int col = 0;
                     col < _board.GetColumnLength() - 3;
                     col++)
                {
                    score += EvaluateWindow(col, row, 1, 0);
                }
            }

            return score;
        }

        ///縦評価
        private int EvaluateVertical()
        {
            int score = 0;

            for (int col = 0;
                 col < _board.GetColumnLength();
                 col++)
            {
                for (int row = 0;
                     row < _board.GetRowLength() - 3;
                     row++)
                {
                    score += EvaluateWindow(col, row, 0, 1);
                }
            }

            return score;
        }

        ///右下方向
        private int EvaluatePositiveDiagonal()
        {
            int score = 0;

            for (int col = 0;
                 col < _board.GetColumnLength() - 3;
                 col++)
            {
                for (int row = 0;
                     row < _board.GetRowLength() - 3;
                     row++)
                {
                    score += EvaluateWindow(col, row, 1, 1);
                }
            }

            return score;
        }

        ///右上方向
        private int EvaluateNegativeDiagonal()
        {
            int score = 0;

            for (int col = 0;
                 col < _board.GetColumnLength() - 3;
                 col++)
            {
                for (int row = 3;
                     row < _board.GetRowLength();
                     row++)
                {
                    score += EvaluateWindow(col, row, 1, -1);
                }
            }

            return score;
        }

        ///一番重要な評価
        private int EvaluateWindow(
            int startCol,
            int startRow,
            int colStep,
            int rowStep)
        {
            int aiCount = 0;
            int opponentCount = 0;
            int emptyCount = 0;

            for (int i = 0; i < 4; i++)
            {
                var player =
                    _board.GetPlayerAtCell(
                        startCol + i * colStep,
                        startRow + i * rowStep);

                if (player == null)
                {
                    emptyCount++;
                }
                else if (
                    player.Color ==
                    _aiPlayer.Color)
                {
                    aiCount++;
                }
                else
                {
                    opponentCount++;
                }
            }

            return ScoreWindow(
                aiCount,
                opponentCount,
                emptyCount);
        }

        ///点数ルール///ここがAIの頭脳です。
        private int ScoreWindow(
            int aiCount,
            int opponentCount,
            int emptyCount)
        {
            if (aiCount == 4)
                return 100000;

            if (opponentCount == 4)
                return -100000;

            if (aiCount == 3 &&
                emptyCount == 1)
                return 100;

            if (aiCount == 2 &&
                emptyCount == 2)
                return 10;

            if (opponentCount == 3 &&
                emptyCount == 1)
                return -120;

            if (opponentCount == 2 &&
                emptyCount == 2)
                return -10;

            return 0;
        }


    }
}