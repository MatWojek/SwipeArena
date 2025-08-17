using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Config
{
    /// <summary>
    /// Klasa z podstawowymi ustawieniami
    /// </summary>
    public static class BasicSettings
    {
        public static string FontFamily { get; } = "Arial";
        public static int FontSize { get; } = 15;
        public static int DefaultX { get; } = 1024;
        public static int DefaultY { get; } = 768;
        public static double DefaultMusicVolume { get; } = 0.5;
        public static bool DefaultIsVolumeOn { get; } = true;
        public static bool DefualtIsAIEnabled { get; } = false;
        public static int Levels { get; } = 15;

    }

}
