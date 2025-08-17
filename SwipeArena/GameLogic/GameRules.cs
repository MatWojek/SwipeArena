using SwipeArena.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwipeArena.Animations;
using System.Diagnostics;
using System.Data;

namespace SwipeArena.GameLogic
{
    public class GameRules
    {
        readonly GameBoard _board;
        readonly MoveValidator _validator;

        int _currentLevel;

        int PointsCollected { get; set; } = 0;
        int MovesLeft { get; set; }
        int PointsToWin { get; set; }

    Stopwatch _gameStopwatch;

        public GameRules(GameBoard board, MoveValidator validator)
        {
            _board = board;
            _validator = validator;
            _currentLevel = _board.GetCurrentLevel();

            if (_currentLevel <= 0)
            {
                throw new InvalidOperationException("Obecny poziom musi być większy od 0");
            }

            MovesLeft = Math.Max(_currentLevel > 0 ? 30 - _currentLevel : 3, 3);
            PointsToWin = _currentLevel * 10;

            _gameStopwatch = new Stopwatch();
            _gameStopwatch.Start();
        }



        /// <summary>
        /// Ustawienie punktów zdobytych
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(int points)
        {
            PointsCollected += points;
        }

        /// <summary>
        /// Zwracanie zdobytych punktów
        /// </summary>
        /// <returns>Liczba zdobytych punktów</returns>
        public int GetPointsCollected()
        {
            return PointsCollected;
        }

        /// <summary>
        /// Zwracanie punktów potrzebnych do zwycięstwa
        /// </summary>
        /// <returns></returns>
        public int GetPointsToWin()
        {
            return PointsToWin;
        }

        /// <summary>
        /// Ustawienie możliwych punktów do wykonania
        /// </summary>
        /// <param name="movesLeft"></param>
        /// <returns></returns>
        public void SetMovesLeft(int movesLeft)
        {
            MovesLeft = movesLeft;
        }

        /// <summary>
        /// Zmniejszenie liczby ruchów
        /// </summary>
        public void DecrementMoves()
        {
            MovesLeft--;
        }

        /// <summary>
        /// Zwracanie ruchów możliwych do wykonania
        /// </summary>
        /// <returns></returns>
        public int GetMovesLeft()
        {
            return MovesLeft;
        }

        /// <summary>
        /// Zapis wyników punktowych
        /// </summary>
        public void SaveData()
        {
            SaveLoad save = new SaveLoad();

            save.Load();

            save.SetBestWinStreak(Math.Max(save.GetBestWinStreak(), save.GetCurrentWinStreak()));
            save.SetMaxPoints(Math.Max(save.GetMaxPoints(), PointsCollected));
            save.SetLastLevelPlayed(_board.GetCurrentLevel());
            save.SetTotalPoints(PointsCollected);

            save.Save();
        }

        /// <summary>
        /// Zapis po wygranej lub przegranej
        /// </summary>
        public void SaveAfterWin()
        {
            SaveLoad save = new SaveLoad();

            save.Load();

            if (MovesLeft >= 0 && PointsCollected >= PointsToWin)
            {
                save.SetCurrentWinStreak(true);

                if (_currentLevel > save.GetLevelCompleted())
                {
                    save.SetLevelCompleted(_currentLevel);
                }

            }
            else if (MovesLeft <= 0 && PointsCollected <= PointsToWin)
            {
                save.SetCurrentWinStreak(false);
            }

            save.SetBestWinStreak(Math.Max(save.GetBestWinStreak(), save.GetCurrentWinStreak()));

            if (_gameStopwatch != null)
            {
                save.SetTimeGame(_gameStopwatch.Elapsed.TotalSeconds);
                _gameStopwatch.Reset();
            }

            save.Save();
        }

        /// <summary>
        /// Sprawdzanie końca gry
        /// </summary>
        /// <returns></returns>
        public int CheckGameOver()
        {
            if (PointsCollected >= PointsToWin)
            {
                SaveAfterWin();
                return 1; // wygrana
            }
            else if (MovesLeft <= 0)
            {
                SaveAfterWin();
                return 2; // przegrana
            }
            return 0; // gra trwa
        }

    }
}
