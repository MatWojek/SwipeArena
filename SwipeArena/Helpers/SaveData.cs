using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Klasa do automatycznego wypełaniania plików JSON wartościami domyślnymi (uniknięcie wartości null)
    /// </summary>
    internal class SaveData
    {   
        public int CurrentWinStreak { get; set; } = 0;
        public int BestWinStreak { get; set; } = 0;
        public int LastLevelPlayed { get; set; } = 0;
        public int LevelCompleted { get; set; } = 0;
        public int MaxPoints { get; set; } = 0;
        public int TotalPoints { get; set; } = 0;
        public double TimeGame { get; set; } = 0.0;
    }
}
