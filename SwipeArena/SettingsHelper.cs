using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena
{
    /// <summary>
    /// Klasa do ustawiania wielkości okna
    /// </summary>
    public static class SettingsHelper
    {
        public static void ApplySettings(Form form, string text)
        {
            form.Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);
            form.Text = text;
        }
    }
}
