using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwipeArena.Config;
using SwipeArena.Helpers;
using static System.Net.Mime.MediaTypeNames;

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
                return System.Drawing.Image.FromFile(imagePath);
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
        protected void NavigateToForm(Form thisForm, Form nextForm)
        {
            using (var loadingForm = new LoadingForm())
            {
                if (nextForm == new MenuForm())
                {
                    formHistory.Clear();
                }
                formHistory.Push(thisForm);
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
                Hide(); // Zamknięcie obecnego okna}
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
        /// Automatyczne rozmieszczanie i skalowanie kontrolek w panelu ustawień przy rozdzielczości od 1280x720
        /// </summary>
        /// <param name="controls">Lista kontrolek do rozmieszczenia</param>
        protected void AdjustControlLayoutForSettingsMedium(List<Control> controls)
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
                // Specjalne rozmiary dla różnych typów
                if (ctrl is System.Windows.Forms.Label label)
                {
                    int maxWidth = (ctrl.Parent?.ClientSize.Width ?? ClientSize.Width) - 40;
                    width = Math.Min((int)(ClientSize.Width * 0.6), maxWidth);

                    label.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize, FontStyle.Bold);
                    label.AutoSize = false;
                    label.MaximumSize = new Size(width, 0); 
                    label.Size = new Size(width, 0);
                    label.TextAlign = ContentAlignment.MiddleLeft;

                    // Oblicz nową wysokość na podstawie tekstu
                    Size proposedSize = new Size(width, int.MaxValue);
                    TextFormatFlags flags = TextFormatFlags.WordBreak;
                    Size measuredSize = TextRenderer.MeasureText(label.Text, label.Font, proposedSize, flags);
                    height = measuredSize.Height;
                }
                else if (ctrl is TrackBar trackBar)
                {
                    width = (int)(ClientSize.Width * 0.5);

                    // Tworzymy Label dla TrackBara
                    System.Windows.Forms.Label trackBarLabel = new System.Windows.Forms.Label();
                    trackBarLabel.Text = $"{trackBar.Name}: {trackBar.Value}";
                    trackBarLabel.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize - 1);
                    trackBarLabel.AutoSize = true;
                    trackBarLabel.Location = new Point((ClientSize.Width - width) / 2, currentY);

                    // Dodajemy Label do formularza (lub panelu jeśli inny kontener)
                    this.Controls.Add(trackBarLabel);

                    // Przesuwamy TrackBar niżej
                    currentY += trackBarLabel.Height + 5;
                }
                else if (ctrl is ComboBox || ctrl is TrackBar)
                {
                    width = (int)(ClientSize.Width * 0.5);
                }
                else if (ctrl is Button button)
                {
                    width = Math.Max(120, ClientSize.Width / 4);
                    height = 35;

                    button.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize - 2, FontStyle.Bold);
                }

                // Ustaw rozmiar i pozycję
                ctrl.Size = new Size(width, height);
                ctrl.Location = new Point((ClientSize.Width - width) / 2, currentY);

                currentY += height + spacing;
            }
        }

        /// <summary>
        /// Automatyczne rozmieszczanie i skalowanie kontrolek w panelu ustawień od rozdzielczości 800x600
        /// </summary>
        /// <param name="controls">Lista kontrolek do rozmieszczenia</param>
        protected void AdjustControlLayoutForSettingsSmall(List<Control> controls)
        {
            if (controls == null || controls.Count == 0)
                return;

            int panelPadding = 20;

            // Rozmiar pojedynczego elementu w pionie (będzie zależny od typu)
            int spacing = 20;
            int currentY = panelPadding;

            int buttonWidth = 200;
            int buttonHeight = 40;
            int buttonSpacing = 10;
            int buttonsPerRow = 2;
            int buttonRowXStart = (ClientSize.Width - (buttonWidth * buttonsPerRow + buttonSpacing * (buttonsPerRow - 1))) / 2;

            int buttonIndex = 0; 

            foreach (Control ctrl in controls)
            {
                int width = ClientSize.Width / 2;
                int height = 40;

                // Specjalne rozmiary dla różnych typów
                if (ctrl is System.Windows.Forms.Label label)
                {
                    int maxWidth = (ctrl.Parent?.ClientSize.Width ?? ClientSize.Width) - 40; 
                    width = Math.Min((int)(ClientSize.Width * 0.6), maxWidth);

                    label.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize - 1, FontStyle.Bold);
                    label.AutoSize = false;
                    label.MaximumSize = new Size(width, 0); 
                    label.Size = new Size(width, 0);
                    label.TextAlign = ContentAlignment.MiddleLeft;

                    // Oblicz nową wysokość na podstawie tekstu
                    Size proposedSize = new Size(width, int.MaxValue);
                    TextFormatFlags flags = TextFormatFlags.WordBreak;
                    Size measuredSize = TextRenderer.MeasureText(label.Text, label.Font, proposedSize, flags);
                    height = measuredSize.Height;
                }
                else if (ctrl is ComboBox || ctrl is TrackBar)
                {
                    width = (int)(ClientSize.Width * 0.5);
                }
                else if (ctrl is TrackBar trackBar)
                {
                    width = (int)(ClientSize.Width * 0.5);

                    // Tworzymy Label dla TrackBara
                    System.Windows.Forms.Label trackBarLabel = new System.Windows.Forms.Label();
                    trackBarLabel.Text = $"{trackBar.Name}: {trackBar.Value}";
                    trackBarLabel.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize - 2);
                    trackBarLabel.AutoSize = true;
                    trackBarLabel.Location = new Point((ClientSize.Width - width) / 2, currentY);

                    // Dodajemy Label do formularza (lub panelu jeśli inny kontener)
                    this.Controls.Add(trackBarLabel);

                    // Przesuwamy TrackBar niżej
                    currentY += trackBarLabel.Height + 5;
                }

                else if (ctrl is Button button)
                {
                    // Ustaw rozmiar przycisków
                    width = buttonWidth;
                    height = buttonHeight;

                    button.Font = new System.Drawing.Font(BasicSettings.FontFamily, BasicSettings.FontSize - 4, FontStyle.Bold);

                    // Oblicz pozycję przycisku w rzędzie
                    int rowX = buttonRowXStart + (buttonIndex % buttonsPerRow) * (buttonWidth + buttonSpacing);
                    int rowY = currentY + (buttonIndex / buttonsPerRow) * (buttonHeight + spacing);

                    ctrl.Size = new Size(width, height);
                    ctrl.Location = new Point(rowX, rowY);

                    buttonIndex++;
                    continue; // Przejdź do następnej kontrolki
                }

                // Ustaw rozmiar i pozycję
                ctrl.Size = new Size(width, height);
                ctrl.Location = new Point((ClientSize.Width - width) / 2, currentY);

                currentY += height + spacing;
            }
        }

    }
}
