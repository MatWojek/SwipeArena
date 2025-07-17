using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwipeArena
{
    public partial class BaseForm : Form
    {

        protected static Stack<Form> formHistory = new Stack<Form>();
        
        public BaseForm()
        {
            // Favicon dla każdego okna
            Icon = new Icon("images/ico/SwipeArenaIcon.ico");

            // Ustawienia wspólne dla wszystkich formularzy 
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);
        }

        /// <summary>
        /// Wczytywanie obrazu tła asynchronicznie
        /// </summary>
        /// <param name="image"></param>
        protected void LoadBackgroundImage(string imagePath)
        {
            Task.Factory.StartNew(() =>
            {
                return Image.FromFile(imagePath);
            })
            .ContinueWith(t =>
            {
                if (t.Exception == null)
                {
                    BackgroundImage = t.Result;
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    MessageBox.Show("Nie udało się wczytać obrazu: " + t.Exception.InnerException?.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Wyświetla komunikat o błędzie.
        /// </summary>
        protected void ShowError(string message)
        {
            MessageBox.Show(message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Przejście do kolejnego okna
        /// </summary>
        /// <param name="nextForm"></param>
        protected void NavigateToForm(Form nextForm)
        {
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();
                Thread.Sleep(60); // Symulacja ładowania
            }

            nextForm.Show();
            Hide();
        }

        /// <summary>
        /// Powrót do poprzedniego okna.
        /// </summary>
        protected void NavigateBack()
        {
            if (formHistory.Count > 0)
            {
                var previousForm = formHistory.Pop(); // Pobranie poprzedniego okna z historii
                previousForm.Show();
                Hide(); // Zamknięcie obecnego okna
            }
            else
            {
                MessageBox.Show("Brak poprzedniego okna w historii.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Czyszczenie stosu po zamknięciu okna
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (formHistory.Count > 0)
            {
                formHistory.Clear(); // Czyszczenie historii przy zamknięciu aplikacji
            }
        }

        /// <summary>
        /// Ustawienie pozycji i rozmiaru dla listy przycisków
        /// </summary>
        /// <param name="controls">Lista kontrolek do ustawienia</param>
        protected void AdjustButtonPositions(List<Control> controls)
        {
            if (controls == null || controls.Count == 0)
                return;

            // Skalowanie przycisków względem rozmiaru okna
            int buttonWidth = Math.Max(150, ClientSize.Width / 4);
            int buttonHeight = Math.Max(40, ClientSize.Height / 15);

            // Pozycje pionowe (środek)
            int centerX = (ClientSize.Width - buttonWidth) / 2;
            int baseY = (ClientSize.Height - (controls.Count * (buttonHeight + 15)));

            // Ustawienie rozmiaru i pozycji dla każdego przycisku
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Size = new Size(buttonWidth, buttonHeight);
                controls[i].Location = new Point(centerX, baseY + i * (buttonHeight + 15));
            }
        }

        /// <summary>
        /// Automatyczne rozmieszczanie i skalowanie kontrolek w panelu ustawień
        /// </summary>
        /// <param name="controls">Lista kontrolek do rozmieszczenia</param>
        protected void AdjustControlLayoutForSettings(List<Control> controls)
        {
            if (controls == null || controls.Count == 0)
                return;

            int panelPadding = 20;

            // Rozmiar pojedynczego elementu w pionie (będzie zależny od typu)
            int spacing = 20;
            int currentY = panelPadding;

            foreach (Control ctrl in controls)
            {
                int width = ClientSize.Width / 2;
                int height = 40;

                // Specjalne rozmiary dla różnych typów
                if (ctrl is Label)
                {
                    width = (int)(ClientSize.Width * 0.6);
                    height = 30;
                    ctrl.Font = new Font(BasicSettings.FontFamily, BasicSettings.FontSize);
                }
                else if (ctrl is ComboBox || ctrl is TrackBar)
                {
                    width = (int)(ClientSize.Width * 0.5);
                }
                else if (ctrl is Button)
                {
                    width = Math.Max(150, ClientSize.Width / 3);
                }

                // Ustaw rozmiar i pozycję
                ctrl.Size = new Size(width, height);
                ctrl.Location = new Point((ClientSize.Width - width) / 2, currentY);

                currentY += height + spacing;
            }
        }

    }
}
