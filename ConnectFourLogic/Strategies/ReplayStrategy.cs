namespace ConnectFourLogic.Strategies
{
    public class ReplayStrategy : IGameStrategy
    {
        public GameStrategyLevel GetLevel()
        {
            return GameStrategyLevel.Replay;
        }

        public (int, int) Play(
            Player currentPlayer,
            Player opponentPlayer)
        {
            return (-1, -1);
        }
    }
}