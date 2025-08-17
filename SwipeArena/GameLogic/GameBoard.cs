using SwipeArena.Models;
using SwipeArena.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace SwipeArena.GameLogic
{
    public class GameBoard
    {

        private static readonly Random random = new Random();

        IGameElement[,] _grid;
        List<IGameElement> _elementTypes = new();
        int rows, cols;

        public int Rows => rows;
        public int Cols => cols;
        public IGameElement[,] Grid
        {
            get => _grid;
            private set => _grid = value;
        }
        static int CurrentLevel { get; set; }

        public List<IGameElement> ElementTypes
        {
            get => _elementTypes;
            set => _elementTypes = value;
        }

        public GameBoard() { }

        public int GetCurrentLevel()
        {
            return CurrentLevel;
        }

        /// <summary>
        /// Zwrócenie planszy
        /// </summary>
        /// <returns></returns>
        public IGameElement[,] GetGrid()
        {
            return _grid;
        }

        /// <summary>
        /// Ustawienie planszy
        /// </summary>
        /// <param name="grid"></param>
        public void SetGrid(IGameElement[,] grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Zwrócenie elementu na planszy
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public IGameElement GetElement(int x, int y)
        {
            if (x < 0 || y < 0 || x >= GetCols() || y >= GetRows())
                return null;
            return _grid[y, x];
        }

        /// <summary>
        /// Ustawienie elementu na planszy
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="element"></param>
        public void SetElement(int x, int y, IGameElement element)
        {
            _grid[y, x] = element;
        }

        public int GetRows() => rows;

        public int GetCols() => cols; 

        /// <summary>
        /// Określenie wielkości planszy na podstawie poziomu
        /// </summary>
        /// <param name="levelNumber"></param>
        public void RandomBoardSize(int levelNumber)
        {
            CurrentLevel = levelNumber;

            if (CurrentLevel >= 6)
            {
                rows = random.Next(4, 8);
                cols = random.Next(4, 8);
            }
            else
            {
                rows = random.Next(3, 3 + CurrentLevel);
                cols = random.Next(3, 3 + CurrentLevel);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ElementSize()
        {
            if (rows <= 4 && cols <= 5)
            {
                return 128;
            }
            else if (rows > 4 && cols < 6 && rows < 5)
            {
                return 96;
            }
            else
            {
                return 72;
            }
        }

        /// <summary>
        /// Generowanie losowo elementów gry
        /// </summary>
        public void GenerateLevel(List<IGameElement> availableElements)
        {
            if (rows == 0 || cols == 0)
                throw new InvalidOperationException("Rozmiar planszy nie został ustawiony.");
            if (availableElements == null || availableElements.Count == 0)
                throw new ArgumentException("Brak dostępnych elementów do generowania planszy.");

            _elementTypes = availableElements;
            _grid = new IGameElement[rows, cols];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var element = _elementTypes[random.Next(_elementTypes.Count)].Clone();
                    _grid[y, x] = element;
                }
            }
        }


        /// <summary>
        /// Tasowanie planszy
        /// </summary>
        public void ShuffleBoard(Func<List<Point>> findMatchesFunc, Func<bool> hasValidMoveFunc)
        {
            List<IGameElement> elements = new List<IGameElement>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    elements.Add(_grid[y, x]);
                }
            }

            int maxAttempts = 1000;
            int attempts = 0;

            do
            {
                elements = elements.OrderBy(e => random.Next()).ToList();

                int index = 0;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        _grid[y, x] = elements[index++];
                    }
                }

                attempts++;

            } while ((!hasValidMoveFunc() || findMatchesFunc().Count > 0) && attempts < maxAttempts);
        }

        /// <summary>
        /// Dodawanie pojedynczego elementu do planszy i interfejsu
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddElement(int typeIndex, int x, int y)
        {
            if (typeIndex < 0 || typeIndex >= _elementTypes.Count)
                throw new ArgumentOutOfRangeException(nameof(typeIndex));
            if (x < 0 || x >= cols || y < 0 || y >= rows)
                throw new ArgumentOutOfRangeException("Pozycja poza planszą.");

            _grid[y, x] = _elementTypes[typeIndex].Clone();
        }

    }
}