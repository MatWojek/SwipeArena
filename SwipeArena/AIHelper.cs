using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: 
// Tryb podpowiedzi: analiza planszy i sugestia najlepszego ruchu
// Tryb auto-gracza: bot automatycznie wykonuje ruchy za gracza 

// Przeskanuj planszę
// Wyszukaj możliwe kombinacje (min. 3 w rzędzie)
// Oblicz wartość punktową każdej kombinacji 
// Wybierz najlepszą 

namespace SwipeArena
{
    class AIHelper
    {
        /// <summary>
        /// Analizuje planszę i sugeruje najlepszy ruch.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Najlepszy ruch w formacie (startX, startY, endX, endY).</returns>
        public (int startX, int startY, int endX, int endY) SuggestBestMove(int[,] board)
        {
            var possibleMoves = FindPossibleMoves(board);
            var bestMove = EvaluateBestMove(possibleMoves, board);
            return bestMove;
        }

        /// <summary>
        /// Tryb auto-gracza: wykonuje ruchy za gracza.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        public void AutoPlay(int[,] board)
        {
            while (true)
            {
                var bestMove = SuggestBestMove(board);
                if (bestMove == (-1, -1, -1, -1)) break; // Brak możliwych ruchów
                ExecuteMove(board, bestMove);
            }
        }

        /// <summary>
        /// Przeskanuj planszę i znajdź możliwe kombinacje.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Lista możliwych ruchów.</returns>
        private List<(int startX, int startY, int endX, int endY)> FindPossibleMoves(int[,] board)
        {
            var moves = new List<(int startX, int startY, int endX, int endY)>();
            // Implementacja logiki wyszukiwania możliwych ruchów
            return moves;
        }

        /// <summary>
        /// Oblicza wartość punktową każdej kombinacji i wybiera najlepszą.
        /// </summary>
        /// <param name="moves">Lista możliwych ruchów.</param>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Najlepszy ruch.</returns>
        private (int startX, int startY, int endX, int endY) EvaluateBestMove(List<(int startX, int startY, int endX, int endY)> moves, int[,] board)
        {
            (int startX, int startY, int endX, int endY) bestMove = (-1, -1, -1, -1);
            int bestScore = int.MinValue;

            foreach (var move in moves)
            {
                int score = CalculateMoveScore(move, board);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        /// <summary>
        /// Oblicza wartość punktową dla danego ruchu.
        /// </summary>
        /// <param name="move">Ruch do oceny.</param>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <returns>Wartość punktowa ruchu.</returns>
        private int CalculateMoveScore((int startX, int startY, int endX, int endY) move, int[,] board)
        {
            int score = 0;
            // Implementacja logiki obliczania punktów
            return score;
        }

        /// <summary>
        /// Wykonuje ruch na planszy.
        /// </summary>
        /// <param name="board">Dwuwymiarowa tablica reprezentująca planszę.</param>
        /// <param name="move">Ruch do wykonania.</param>
        private void ExecuteMove(int[,] board, (int startX, int startY, int endX, int endY) move)
        {
            // Implementacja logiki wykonania ruchu
        }
    }
}
