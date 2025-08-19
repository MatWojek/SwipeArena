using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.GameLogic
{
    /// <summary>
    /// Interfejs zapisujący wyniki gry (podczas gry)
    /// </summary>
    public interface IGameRules
    {
        void AddPoints(int points);
        int GetPointsCollected();
        int GetPointsToWin();
        void SetMovesLeft(int movesLeft);
        void DecrementMoves();
        int GetMovesLeft();
        int CheckGameOver();  
    }
}
