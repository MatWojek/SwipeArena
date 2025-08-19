using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Klasa do zapisywania i wczytywania danych gry (zwycięstwa, seria, najlepszy wynik, itp.)
    /// </summary>
    internal class SaveLoad : ISaveLoad
    {
        static readonly string SaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "saveData.json");

        /// <summary>
        /// Liczba zwycięstw z rzędu w bieżącej sesji gry.
        /// </summary>
        int CurrentWinStreak { get; set; } = 0;

        /// <summary>
        /// Najlepsza seria zwycięstw osiągnięta w historii gry.
        /// </summary>
        int BestWinStreak { get; set; } = 0;

        /// <summary>
        /// Numer ostatniego poziomu, który został rozpoczęty.
        /// </summary>
        int LastLevelPlayed { get; set; } = 0;

        /// <summary>
        /// Liczba ukończonych poziomów.
        /// </summary>
        int LevelCompleted { get; set; } = 0;

        /// <summary>
        /// Najwyższa liczba punktów zdobyta w jednej sesji gry.
        /// </summary>
        int MaxPoints { get; set; } = 0;

        /// <summary>
        /// Całkowita liczba punktów zdobyta przez gracza w całej grze.
        /// </summary>
        int TotalPoints { get; set; } = 0;

        /// <summary>
        /// Łączny czas gry w sekundach.
        /// </summary>
        double TimeGame { get; set; } = 0.0;

        /// <summary>
        /// Ustawianie obecnej serii zwycięstw
        /// </summary>
        /// <param name="checkWin"></param>
        public void SetCurrentWinStreak(bool checkWin)
        {
            if (checkWin)
            {
                CurrentWinStreak += 1;
            }
            else
            {
                CurrentWinStreak = 0;
            }
        }

        /// <summary>
        /// Pobieranie obecnej serii zwycięstw
        /// </summary>
        /// <returns></returns>
        public int GetCurrentWinStreak() 
        { 
            return CurrentWinStreak; 
        }

        /// <summary>
        /// Ustawianie najlepszej serii zwycięstw
        /// </summary>
        /// <param name="bestWinStreak"></param>
        public void SetBestWinStreak(int bestWinStreak)
        {
            BestWinStreak = bestWinStreak;
        }

        /// <summary>
        /// Pobieranie najlepszej serii zwycięstw
        /// </summary>
        /// <returns></returns>
        public int GetBestWinStreak()
        {
            return BestWinStreak;
        }

        /// <summary>
        /// Ustawianie ostatniego granego poziomu
        /// </summary>
        /// <param name="lastLevelPlayed"></param>
        public void SetLastLevelPlayed(int lastLevelPlayed)
        {
            LastLevelPlayed = lastLevelPlayed;
        }

        /// <summary>
        /// Pobieranie ostatniego granego poziomu
        /// </summary>
        /// <returns></returns>
        public int GetLastLevelPlayed()
        {
            return LastLevelPlayed;
        }

        /// <summary>
        /// Ustawianie najdalszego wygranego poziomu
        /// </summary>
        /// <param name="levelCompleted"></param>
        public void SetLevelCompleted(int levelCompleted)
        {
            LevelCompleted = levelCompleted;
        }

        /// <summary>
        /// Pobieranie najdalszego wygranego poziomu
        /// </summary>
        /// <returns></returns>
        public int GetLevelCompleted()
        {
            return LevelCompleted;
        }

        /// <summary>
        /// Ustawianie maksymalnej zdobytej ilości punktów 
        /// </summary>
        /// <param name="maxPoints"></param>
        public void SetMaxPoints(int maxPoints)
        {
            MaxPoints = maxPoints;
        }

        /// <summary>
        /// Pobieranie maksymalnej zdobytej ilości punktów
        /// </summary>
        /// <returns></returns>
        public int GetMaxPoints()
        {
            return MaxPoints;
        }

        /// <summary>
        /// Ustawianie całkowitej zdobytej ilości punktów
        /// </summary>
        /// <param name="totalPoints"></param>
        public void SetTotalPoints(int totalPoints)
        {
            TotalPoints += totalPoints;
        }

        /// <summary>
        /// Pobieranie całkowitej zdobytej ilości punktów
        /// </summary>
        /// <returns></returns>
        public int GetTotalPoints()
        {
            return TotalPoints;
        }

        /// <summary>
        /// Ustawianie całkowitego czasu w grze
        /// </summary>
        /// <param name="timeGame"></param>
        public void SetTimeGame(double timeGame)
        {
            TimeGame += timeGame;
        }

        /// <summary>
        /// Pobieranie całkowitego czasu w grze
        /// </summary>
        /// <returns></returns>
        public double GetTimeGame()
        {
            return TimeGame;
        }

        /// <summary>
        /// Zapisuje postępy do pliku JSON.
        /// </summary>
        public void Save()
        {
            var saveData = new SaveData
            {
                CurrentWinStreak = CurrentWinStreak,
                BestWinStreak = BestWinStreak,
                LevelCompleted = LevelCompleted,
                LastLevelPlayed = LastLevelPlayed,
                MaxPoints = MaxPoints,
                TotalPoints = TotalPoints,
                TimeGame = TimeGame
            };

            var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath)!);
            File.WriteAllText(SaveFilePath, json);
        }


        /// <summary>
        /// Wczytuje postępy z pliku JSON.
        /// </summary>
        public void Load()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    var json = File.ReadAllText(SaveFilePath);
                    var saveData = JsonConvert.DeserializeObject<SaveData>(json);

                    if (saveData != null)
                    {
                        CurrentWinStreak = saveData.CurrentWinStreak;
                        BestWinStreak = saveData.BestWinStreak;
                        LastLevelPlayed = saveData.LastLevelPlayed;
                        LevelCompleted = saveData.LevelCompleted;
                        MaxPoints = saveData.MaxPoints;
                        TotalPoints = saveData.TotalPoints;
                        TimeGame = saveData.TimeGame;
                    }
                }
                catch (Exception ex)
                {
                    // Jeśli plik jest uszkodzony → reset zamiast crasha
                    Reset();
                    Console.WriteLine($"Błąd przy wczytywaniu zapisu: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Resetuje wszystkie postępy.
        /// </summary>
        public void Reset()
        {
            CurrentWinStreak = 0;
            BestWinStreak = 0;
            LastLevelPlayed = 1;
            LevelCompleted = 0;
            MaxPoints = 0;
            TotalPoints = 0;
            TimeGame = 0;

            // Usunięcie pliku zapisu
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
        }

        /// <summary>
        /// Wczytuje postępy z wybranego pliku JSON.
        /// </summary>
        public void LoadFromSelectedFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.Title = "Wybierz plik zapisu";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(openFileDialog.FileName);
                        var saveData = JsonConvert.DeserializeObject<SaveData>(json);

                        // Aktualizacja właściwości na podstawie wczytanych danych
                        CurrentWinStreak = saveData.CurrentWinStreak;
                        BestWinStreak = saveData.BestWinStreak;
                        LastLevelPlayed = saveData.LastLevelPlayed;
                        LevelCompleted = saveData.LevelCompleted;
                        MaxPoints = saveData.MaxPoints;
                        TotalPoints = saveData.TotalPoints;
                        TimeGame = saveData.TimeGame;

                        Save();

                        MessageBox.Show("Postępy zostały pomyślnie wczytane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Wystąpił błąd podczas wczytywania pliku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
