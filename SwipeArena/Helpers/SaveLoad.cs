using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Zapis będzie polegał na tym że będzie pokazywał serię zwycięstw 
// Obecną i najlepszą 
// Oraz który level był robiony jako ostatni

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Zapisywanie i wczytywanie gry
    /// </summary>
    internal class SaveLoad : ISaveLoad
    {
        static readonly string SaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "saveData.json");

        /// <summary>
        /// Liczba zwycięstw z rzędu w bieżącej sesji gry.
        /// </summary>
        public int CurrentWinStreak { get; set; } = 0;

        /// <summary>
        /// Najlepsza seria zwycięstw osiągnięta w historii gry.
        /// </summary>
        public int BestWinStreak { get; set; } = 0;

        /// <summary>
        /// Numer ostatniego poziomu, który został rozpoczęty.
        /// </summary>
        public int LastLevelPlayed { get; set; } = 0;

        /// <summary>
        /// Liczba ukończonych poziomów.
        /// </summary>
        public int LevelCompleted { get; set; } = 0;

        /// <summary>
        /// Najwyższa liczba punktów zdobyta w jednej sesji gry.
        /// </summary>
        public int MaxPoints { get; set; } = 0;

        /// <summary>
        /// Całkowita liczba punktów zdobyta przez gracza w całej grze.
        /// </summary>
        public int TotalPoints { get; set; } = 0;

        /// <summary>
        /// Łączny czas gry w sekundach.
        /// </summary>
        public double TimeGame { get; set; } = 0.0;


        /// <summary>
        /// Zapisuje postępy do pliku JSON.
        /// </summary>
        public void Save()
        {
            var saveData = new
            {
                CurrentWinStreak,
                BestWinStreak,
                LevelCompleted,
                LastLevelPlayed,
                MaxPoints,
                TotalPoints,
                TimeGame
            };

            var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }

        /// <summary>
        /// Wczytuje postępy z pliku JSON.
        /// </summary>
        public void Load()
        {
            if (File.Exists(SaveFilePath))
            {
                var json = File.ReadAllText(SaveFilePath);
                var saveData = JsonConvert.DeserializeObject<dynamic>(json);

                CurrentWinStreak = saveData.CurrentWinStreak;
                BestWinStreak = saveData.BestWinStreak;
                LastLevelPlayed = saveData.LastLevelPlayed;
                LevelCompleted = saveData.LevelCompleted;
                MaxPoints = saveData.MaxPoints;
                TotalPoints = saveData.TotalPoints;
                TimeGame = saveData.TimeGame;
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
                        var saveData = JsonConvert.DeserializeObject<dynamic>(json);

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
