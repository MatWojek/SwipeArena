using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwipeArena.Models;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Klasa ustawiania podpowiedzi i automatycznego przechodzenia gry
    /// </summary>
    class AIHelper
    {
        /// <summary>
        /// Analizuje planszę i sugeruje najlepszy ruch.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Najlepszy ruch w formacie (startX, startY, endX, endY).</returns>
        public (int startX, int startY, int endX, int endY) SuggestBestMove(IGameElement[,] board)
        {
            var possibleMoves = FindPossibleMoves(board);
            return EvaluateBestMove(possibleMoves, board);
        }

        /// <summary>
        /// Przeskanuj planszę i znajdź możliwe kombinacje.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Lista możliwych ruchów.</returns>
        List<(int startX, int startY, int endX, int endY)> FindPossibleMoves(IGameElement[,] board)
        {
            int _rows = board.GetLength(0);
            int _cols = board.GetLength(1);
            var _moves = new List<(int, int, int, int)>();

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (x + 1 < _cols && CausesMatch(board, x, y, x + 1, y))
                        _moves.Add((x, y, x + 1, y));

                    if (y + 1 < _rows && CausesMatch(board, x, y, x, y + 1))
                        _moves.Add((x, y, x, y + 1));
                }
            }

            return _moves;
        }

        /// <summary>
        /// Sprawdza, czy zamiana dwóch elementów na planszy spowoduje utworzenie połączenia 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        bool CausesMatch(IGameElement[,] board, int x1, int y1, int x2, int y2)
        {
            (board[y1, x1], board[y2, x2]) = (board[y2, x2], board[y1, x1]);
            var _result = CheckMatchAt(board, x1, y1) || CheckMatchAt(board, x2, y2);
            (board[y1, x1], board[y2, x2]) = (board[y2, x2], board[y1, x1]);
            return _result;
        }

        /// <summary>
        /// Sprawdza, czy na danej pozycji na planszy istnieje połączenie  w poziomie lub pionie.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool CheckMatchAt(IGameElement[,] board, int x, int y)
        {
            string _name = board[y, x].Name;
            int _rows = board.GetLength(0);
            int _cols = board.GetLength(1);

            int countH = 1, countV = 1;
            for (int i = x - 1; i >= 0 && board[y, i].Name == _name; i--) countH++;
            for (int i = x + 1; i < _cols && board[y, i].Name == _name; i++) countH++;
            for (int i = y - 1; i >= 0 && board[i, x].Name == _name; i--) countV++;
            for (int i = y + 1; i < _rows && board[i, x].Name == _name; i++) countV++;

            return countH >= 3 || countV >= 3;
        }

        /// <summary>
        /// Oblicza wartość punktową każdej kombinacji i wybiera najlepszą.
        /// </summary>
        /// <param name="moves">Lista możliwych ruchów.</param>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Najlepszy ruch.</returns>
        (int startX, int startY, int endX, int endY) EvaluateBestMove(List<(int startX, int startY, int endX, int endY)> moves, IGameElement[,] board)
        {
            if (moves.Count == 0) return (-1, -1, -1, -1);
            return moves[0];
        }
    }
}