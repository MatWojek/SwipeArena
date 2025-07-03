using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Zapis będzie polegał na tym że będzie pokazywał serię zwycięstw 
// Obecną i najlepszą 
// Oraz który level był robiony jako ostatni

namespace SwipeArena
{
    /// <summary>
    /// Zapisywanie i wczytywanie gry
    /// </summary>
    internal class SaveLoad
    {

        const string SaveFilePath = "saveData.json";

        public int CurrentWinStreak { get; set; } = 0;
        public int BestWinStreak { get; set; } = 0;
        public int LastLevelPlayed { get; set; } = 1;
        public int MaxPoints { get; set; } = 0;

        /// <summary>
        /// Zapisuje postępy do pliku JSON.
        /// </summary>
        public void Save()
        {
            var saveData = new
            {
                CurrentWinStreak,
                BestWinStreak,
                LastLevelPlayed,
                MaxPoints
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
                MaxPoints = saveData.MaxPoints;
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
            MaxPoints = 0;

            // Usunięcie pliku zapisu
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
        }
    }

}
