using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Klasa do obsługi zamykania okna gry lub wychodzenia z gry (zamykanie wszystkich okien)
    /// </summary>
    public class FormUtils
    {
        static bool _isExiting = false;

        /// <summary>
        /// Rejestruje zdarzenie FormClosing, aby zamknąć całą aplikację.
        /// </summary>
        /// <param name="form">Formularz, dla którego rejestrowane jest zdarzenie.</param>
        public static void RegisterFormClosingHandler(Form form)
        {
            form.FormClosing += (sender, e) =>
            {
                if (!_isExiting)
                {
                    Application.Exit();
                }
            };
        }

        /// <summary>
        /// Ustawia flagę, aby aplikacja mogła się zamknąć.
        /// </summary>
        public static void ExitApplication()
        {
            _isExiting = true;
            Application.Exit();
        }
    }
}
