using ConnectFourLogic.Board;
using ConnectFourLogic.Helpers;
using ConnectFourLogic.Strategies;
using System.Collections.Generic;

namespace ConnectFourLogic
{
    public class BattleRunner
    {
        public (Player Winner, IGame Game)
            Run(GameStrategyLevel level)
        {
            var moveHistory = new List<int>();

            var board = new GameBoard();

            string targetColor =
                ColorHelper.GetRandomColor();

            string easyColor =
                ColorHelper.GetComplementaryColor(
                    targetColor);

            var easyPlayer =
                new Player(
                    "Computer-Easy",
                    easyColor);

            var targetPlayer =
                new Player(
                    $"Computer-{level}",
                    targetColor);

            var easyStrategy =
                GameStrategyFactory.Create(
                    GameStrategyLevel.Easy,
                    board);

            var targetStrategy =
                GameStrategyFactory.Create(
                    level,
                    board);

            Player current = easyPlayer;
            Player winner = null;

            while (true)
            {
                var strategy =
                    current == easyPlayer
                        ? easyStrategy
                        : targetStrategy;

                var opponent =
                    current == easyPlayer
                        ? targetPlayer
                        : easyPlayer;

                var move =
                    strategy.Play(
                        current,
                        opponent);

                moveHistory.Add(move.Item1);

                if (board.HasPlayerWon(
                    current,
                    new BoardCell(
                        move.Item1,
                        move.Item2)))
                {
                    winner = current;
                    break;
                }

                if (board.IsFull())
                {
                    break;
                }

                current =
                    current == easyPlayer
                        ? targetPlayer
                        : easyPlayer;
            }

            var game =
                new Game(
                    easyPlayer,
                    targetPlayer,
                    level,
                    board,
                    winner,
                    moveHistory);

            return (winner, game);
        }
    }
}
