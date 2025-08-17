using SwipeArena.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwipeArena.Animations;

namespace SwipeArena.GameLogic
{
    public class MoveValidator
    {
        readonly GameBoard _board;

        public MoveValidator(GameBoard board)
        {
            _board = board;
        }

        /// <summary>
        /// Sprawdzenie czy przesunięcie elementu o jedno pole skutkuje matchem
        /// </summary>
        /// <returns></returns>
        public bool HasValidMove()
        {
            for (int y = 0; y < _board.Rows; y++)
            {
                for (int x = 0; x < _board.Cols; x++)
                {
                    if (x < _board.Cols - 1 && CanFormMatch(x, y, x + 1, y))
                        return true;
                    if (y < _board.Rows - 1 && CanFormMatch(x, y, x, y + 1))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Czy jest połączenie na planszy
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        bool CanFormMatch(int x1, int y1, int x2, int y2)
        {
            var e1 = _board.GetElement(x1, y1);
            var e2 = _board.GetElement(x2, y2);

            if (e1 == null || e2 == null)
                return false;

            // Zamiana tymczasowa
            _board.SetElement(x1, y1, e2.Clone());
            _board.SetElement(x2, y2, e1.Clone());

            bool hasMatch = FindMatches().Count > 0;

            // Przywrócenie oryginalnych wartości
            _board.SetElement(x1, y1, e1);
            _board.SetElement(x2, y2, e2);

            return hasMatch;
        }


        /// <summary>
        ///  Czy pola sąsiadują ze sobą
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool AreAdjacent(Point a, Point b) 
            => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) == 1;

        public List<Point> FindMatches()
        {
            HashSet<Point> matches = new HashSet<Point>();
            int rows = _board.Rows;
            int cols = _board.Cols;

            // Sprawdzanie poziomych połączeń
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols - 2; x++)
                {
                    var a = _board.GetElement(x, y);      
                    var b = _board.GetElement(x + 1, y);
                    var c = _board.GetElement(x + 2, y);

                    if (a != null && b != null && c != null && a.Name == b.Name && b.Name == c.Name)
                    {
                        matches.Add(new Point(x, y));
                        matches.Add(new Point(x + 1, y));
                        matches.Add(new Point(x + 2, y));
                    }
                }
            }

            // Sprawdzanie pionowych połączeń
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows - 2; y++)
                {
                    var a = _board.GetElement(x, y);
                    var b = _board.GetElement(x, y + 1);
                    var c = _board.GetElement(x, y + 2);

                    if (a != null && b != null && c != null && a.Name == b.Name && b.Name == c.Name)
                    {
                        matches.Add(new Point(x, y));
                        matches.Add(new Point(x, y + 1));
                        matches.Add(new Point(x, y + 2));
                    }
                }
            }

            return matches.ToList();
        }



        /// <summary>
        /// Zamienia elementy w siatce
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Swap(Point a, Point b)
        {
            if (a.X < 0 || a.X >= _board.Cols || a.Y < 0 || a.Y >= _board.Rows)
                return;
            if (b.X < 0 || b.X >= _board.Cols || b.Y < 0 || b.Y >= _board.Rows)
                return;

            var temp = _board.GetElement(a.X, a.Y);
            _board.SetElement(a.X, a.Y, _board.GetElement(b.X, b.Y));
            _board.SetElement(b.X, b.Y, temp);
        }
    }
}
