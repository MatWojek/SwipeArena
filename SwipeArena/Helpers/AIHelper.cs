using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwipeArena.Models;

// TODO: 
// Tryb podpowiedzi: analiza planszy i sugestia najlepszego ruchu
// Tryb auto-gracza: bot automatycznie wykonuje ruchy za gracza 

// Przeskanuj planszę
// Wyszukaj możliwe kombinacje (min. 3 w rzędzie)
// Oblicz wartość punktową każdej kombinacji 
// Wybierz najlepszą 

namespace SwipeArena.Helpers
{
    class AIHelper
    {

        public void PlayAutomatically(IGameElement[,] board, Func<bool> isAIEnabled)
        {
            while (isAIEnabled())
            {
                var bestMove = SuggestBestMove(board);

                // Jeśli nie ma możliwych ruchów, zakończ
                if (bestMove.startX == -1) break;

                // Wykonaj ruch
                ExecuteMove(board, bestMove);

                // Odśwież planszę (jeśli wymagane)
                RefreshBoard(board);
            }
        }

        private void ExecuteMove(IGameElement[,] board, (int startX, int startY, int endX, int endY) move)
        {
            // Zamień elementy na planszy
            (board[move.startY, move.startX], board[move.endY, move.endX]) =
                (board[move.endY, move.endX], board[move.startY, move.startX]);

            // Możesz dodać logikę do obsługi punktacji lub innych efektów
        }

        private void RefreshBoard(IGameElement[,] board)
        {
            // Logika odświeżania planszy, np. usuwanie dopasowań, przesuwanie elementów itp.
            // To zależy od implementacji Twojej gry.
        }

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
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            var moves = new List<(int, int, int, int)>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (x + 1 < cols && CausesMatch(board, x, y, x + 1, y))
                        moves.Add((x, y, x + 1, y));

                    if (y + 1 < rows && CausesMatch(board, x, y, x, y + 1))
                        moves.Add((x, y, x, y + 1));
                }
            }

            return moves;
        }

        bool CausesMatch(IGameElement[,] board, int x1, int y1, int x2, int y2)
        {
            (board[y1, x1], board[y2, x2]) = (board[y2, x2], board[y1, x1]);
            var result = CheckMatchAt(board, x1, y1) || CheckMatchAt(board, x2, y2);
            (board[y1, x1], board[y2, x2]) = (board[y2, x2], board[y1, x1]);
            return result;
        }

        bool CheckMatchAt(IGameElement[,] board, int x, int y)
        {
            string name = board[y, x].Name;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            int countH = 1, countV = 1;
            for (int i = x - 1; i >= 0 && board[y, i].Name == name; i--) countH++;
            for (int i = x + 1; i < cols && board[y, i].Name == name; i++) countH++;
            for (int i = y - 1; i >= 0 && board[i, x].Name == name; i--) countV++;
            for (int i = y + 1; i < rows && board[i, x].Name == name; i++) countV++;

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