using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.GameLogic
{
    /// <summary>
    /// Interfejs do poprawnego działania ruchów w grze
    /// </summary>
    public interface IMoveValidator
    {
        bool HasValidMove();
        bool AreAdjacent(Point a, Point b);
        List<Point> FindMatches();
        void Swap(Point a, Point b);
        void SwapTags(PictureBox box1, PictureBox box2);
       
    }
}
