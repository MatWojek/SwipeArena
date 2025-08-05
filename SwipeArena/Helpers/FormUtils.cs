using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    public class FormUtils
    {
        static bool isExiting = false;

        /// <summary>
        /// Rejestruje zdarzenie FormClosing, aby zamknąć całą aplikację.
        /// </summary>
        /// <param name="form">Formularz, dla którego rejestrowane jest zdarzenie.</param>
        public static void RegisterFormClosingHandler(Form form)
        {
            form.FormClosing += (sender, e) =>
            {
                if (!isExiting)
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
            isExiting = true;
            Application.Exit();
        }
    }
}
