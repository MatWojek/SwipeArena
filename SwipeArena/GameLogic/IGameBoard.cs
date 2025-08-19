using SwipeArena.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.GameLogic
{
    /// <summary>
    /// Interfejs tworzący i manipulujący planszą gry
    /// </summary>
    public interface IGameBoard
    {
        int Rows { get; }
        int Cols { get; }
        IGameElement[,] Grid { get; }
        List<IGameElement> ElementTypes { get; set; }

        int GetCurrentLevel();
        IGameElement[,] GetGrid();
        void SetGrid(IGameElement[,] grid);
        IGameElement GetElement(int x, int y);
        void SetElement(int x, int y, IGameElement element);
        int GetRows();
        int GetCols();
        void RandomBoardSize(int levelNumber);
        int ElementSize();
        void GenerateLevel(List<IGameElement> availableElements);
        void ShuffleBoard(Func<List<Point>> findMatchesFunc, Func<bool> hasValidMoveFunc);
        void AddElement(int typeIndex, int x, int y);

    }
}
