using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Klasa do ustawiania wielkości okna
    /// </summary>
    public static class SettingsHelper
    {
        static SettingsData settings = SettingsData.Instance;

        public static void ApplySettings(Form form, string text)
        {
            form.Size = new Size(settings.Resolution.X, settings.Resolution.Y);
            form.Text = text;
        }
    }
}
