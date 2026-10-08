using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectFourLogic.Board;

namespace ConnectFourLogic.Strategies
{
    class MediumLevelStrategy : IGameStrategy
    {
        private readonly IGameBoard _board;

        public MediumLevelStrategy(IGameBoard board)
        {
            _board = board;
        }

        public GameStrategyLevel GetLevel()
        {
            return GameStrategyLevel.Medium;
        }

        public (int, int) Play(Player currentPlayer, Player opponentPlayer)
        {
            // 自分が勝てるなら勝つ
            var column = _board.GetColumnToWin(currentPlayer);

            if (column >= 0)
            {
                var row = _board.DropDisc(column, currentPlayer);
                return (column, row);
            }

            // 相手が勝つなら防ぐ
            column = _board.GetColumnToWin(opponentPlayer);

            if (column >= 0)
            {
                var row = _board.DropDisc(column, currentPlayer);
                return (column, row);
            }

            // 中央優先
            column = GetCenterPriorityColumn();

            var droppedRow = _board.DropDisc(column, currentPlayer);

            return (column, droppedRow);
        }

        private int GetCenterPriorityColumn()
        {
            int center = _board.GetColumnLength() / 2;

            if (!_board.IsColumnFull(center))
                return center;

            for (int offset = 1; offset < _board.GetColumnLength(); offset++)
            {
                int left = center - offset;
                int right = center + offset;

                if (left >= 0 && !_board.IsColumnFull(left))
                    return left;

                if (right < _board.GetColumnLength() &&
                    !_board.IsColumnFull(right))
                    return right;
            }

            return GetRandomColumn();
        }

        private int GetRandomColumn()
        {
            var availableColumns = _board.ColumnIndices()
                .Where(c => !_board.IsColumnFull(c))
                .ToList();

            var random = new Random();

            return availableColumns[random.Next(availableColumns.Count)];
        }
    }
}


