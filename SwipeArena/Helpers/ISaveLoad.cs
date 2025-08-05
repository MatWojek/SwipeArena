using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    public interface ISaveLoad
    {
        public int CurrentWinStreak { get; set; }
        public int BestWinStreak { get; set; }
        public int LastLevelPlayed { get; set; }
        public int LevelCompleted { get; set; }
        public int MaxPoints { get; set; }
        public int TotalPoints { get; set; }
        public double TimeGame { get; set; }
        void Load();
        void Save();
    }
}
