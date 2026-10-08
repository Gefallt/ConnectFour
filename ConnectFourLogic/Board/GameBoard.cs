using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConnectFourLogic.Board
{
    public class GameBoard : IGameBoard
    {
        private readonly Player[,] _cells = new Player[7, 6];
        private const int TotalDiscsInRowToWin = 4;

        public static readonly int InvalidRowColumn = -1;

        public int GetColumnLength()
        {
            return _cells.GetLength(0);
        }

        public int GetRowLength()
        {
            return _cells.GetLength(1);
        }

        public IEnumerable<int> ColumnIndices()
        {
            for (int c = 0; c < GetColumnLength(); c++)
            {
                yield return c;
            }
        }

        public IEnumerable<int> RowIndices()
        {
            for (int r = 0; r < GetRowLength(); r++)
            {
                yield return r;
            }
        }

        public string GetDiscColorAtCell(int column, int row)
        {
            var player = _cells[column, row];
            return player?.Color;
        }

        public bool IsFull()
        {
            return ColumnIndices()
                .All(column => _cells[column, 0] != null);
        }

        public bool IsColumnFull(int column)
        {
            return _cells[column, 0] != null;
        }

        public int DropDisc(int column, Player player)
        {
            int nextRow = GetNextAvailableRow(column);

            if (nextRow >= 0)
            {
                _cells[column, nextRow] = player;
            }

            return nextRow;
        }

        public int GetColumnToWin(Player player)
        {
            foreach (var column in ColumnIndices())
            {
                if (CanPlayerWin(player, column))
                {
                    return column;
                }
            }

            return InvalidRowColumn;
        }

        public bool CanPlayerWin(Player player, int column)
        {
            int nextRow = GetNextAvailableRow(column);
            var currentCell = new BoardCell(column, nextRow);

            return CheckWinningPossibility(player, currentCell);
        }

        public bool HasPlayerWon(Player player, BoardCell lastPlayedCell)
        {
            return CheckWinningPossibility(player, lastPlayedCell);
        }

        private bool CheckWinningPossibility(Player player, BoardCell cellToCheckFrom)
        {
            if (!IsValidCell(cellToCheckFrom))
            {
                return false;
            }

            return CheckHorizontally(player, cellToCheckFrom)
                   || CheckVertically(player, cellToCheckFrom)
                   || CheckPositiveDiagonal(player, cellToCheckFrom)
                   || CheckNegativeDiagonal(player, cellToCheckFrom);
        }

        private bool CheckHorizontally(Player player, BoardCell currentCell)
        {
            for (var startColumn = currentCell.Column - 3; startColumn <= currentCell.Column; startColumn++)
            {
                var startCell = new BoardCell(startColumn, currentCell.Row);
                var canConnect = CanConnectFour(player, currentCell, startCell, BoardNavigationHelper.GetNextCellOnRight);

                if (canConnect)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckVertically(Player player, BoardCell currentCell)
        {
            var canConnect = CanConnectFour(player, currentCell, currentCell, BoardNavigationHelper.GetNextCellBelow);

            return canConnect;
        }

        private bool CheckPositiveDiagonal(Player player, BoardCell currentCell)
        {
            for (int startDistance = 3; startDistance >= 0; startDistance--)
            {
                var startCell = new BoardCell(currentCell.Column - startDistance, currentCell.Row + startDistance);
                var canConnect = CanConnectFour(player, currentCell, startCell, BoardNavigationHelper.GetNextCellOnRightCornerAbove);

                if (canConnect)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckNegativeDiagonal(Player player, BoardCell currentCell)
        {
            for (int startDistance = 3; startDistance >= 0; startDistance--)
            {
                var startCell = new BoardCell(currentCell.Column - startDistance, currentCell.Row - startDistance);
                var canConnect = CanConnectFour(player, currentCell, startCell, BoardNavigationHelper.GetNextCellOnRightCornerBelow);

                if (canConnect)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanConnectFour(Player player, BoardCell currentCell, BoardCell startCell, Func<BoardCell, BoardCell> getNextCellFunc)
        {
            BoardCell cellToCheck = startCell;
            var cellToBePlayedNext = IsAlreadyPlayed(currentCell) ? null : currentCell;

            int count = 0;

            for (int i = 0; i < TotalDiscsInRowToWin; i++)
            {
                if (!IsValidCell(cellToCheck.Column, cellToCheck.Row))
                {
                    return false;
                }

                if (!cellToCheck.Equals(cellToBePlayedNext) &&
                    GetDiscColorAtCell(cellToCheck.Column, cellToCheck.Row) != player.Color)
                {
                    return false;
                }

                count++;
                cellToCheck = getNextCellFunc(cellToCheck);
            }

            return count == TotalDiscsInRowToWin;
        }

        private bool IsAlreadyPlayed(BoardCell cell)
        {
            return IsValidCell(cell) && _cells[cell.Column, cell.Row] != null;
        }

        private bool IsValidCell(BoardCell cell)
        {
            return IsValidCell(cell.Column, cell.Row);
        }

        private bool IsValidCell(int column, int row)
        {
            return column >= 0 && column < GetColumnLength()
                               && row >= 0 && row < GetRowLength();
        }

        private int GetNextAvailableRow(int column)
        {
            for (int row = GetRowLength() - 1; row >= 0; row--)
            {
                if (_cells[column, row] == null)
                {
                    return row;
                }
            }

            return InvalidRowColumn;
        }

        public void RemoveDisc(int column, int row)
        {
            _cells[column, row] = null;
        }

        public Player GetPlayerAtCell(int column, int row)
        {
            return _cells[column, row];
        }

    }
}
