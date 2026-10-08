using ConnectFourLogic.Board;
using System.Collections.Generic;

namespace ConnectFourLogic
{
    public interface IGame
    {
        IGameBoard GetBoard();

        Player GetCurrentPlayer();

        Player GetWinner();

        bool IsOver();

        void DropDisc(int column);
        
        Player GetPlayerOne();
        
        Player GetPlayerTwo();

        IReadOnlyList<int> GetMoveHistory();

    }
}