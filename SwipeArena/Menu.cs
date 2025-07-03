using System.Threading;

namespace SwipeArena
{
    public partial class Menu : Form
    { 

        Button startButton;
        Button settingsButton;
        Button exitButton;

        public Menu()
        {
            try
            {
                Icon = new Icon("images/ico/SwipeArenaIcon.ico");

                InitializeComponent();

                // Zablokowanie zmiany rozmiaru okna
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;

                // Asynchroniczne wczytanie ilustracji jako t³a
                Task.Factory.StartNew(() =>
                {
                    // Wczytanie obrazu w tle
                    return Image.FromFile("images/background/menu.png");
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
                        MessageBox.Show("Nie uda³o siê wczytaæ obrazu: " + t.Exception.InnerException?.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                // Ustawienia formularza
                Text = "Menu G³ówne";
                Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);
                AddButtons();

                AdjustButtonPositions();
                Resize += (s, e) => AdjustButtonPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            // Rejestracja obs³ugi zamkniêcia okna
            FormUtils.RegisterFormClosingHandler(this);

        }

        /// <summary>
        /// Tworzenie nowych przycisków
        /// </summary>
        void AddButtons()
        {

            // Przycisk Start 
            startButton = new Button
            {
                Text = "Start",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(66, 197, 230),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            startButton.Click += StartButton_Click;
            Controls.Add(startButton);

            // Przycisk Ustawienia
            settingsButton = new Button
            {
                Text = "Ustawienia",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            settingsButton.Click += SettingsButton_Click;
            Controls.Add(settingsButton);

            // Przycisk Wyjœcie
            exitButton = new Button
            {
                Text = "Wyjœcie",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            exitButton.Click += (s, e) => Close();

            Controls.Add(exitButton);
        }

        /// <summary>
        /// Ustawienie pozycji przycisków
        /// </summary>
        void AdjustButtonPositions()
        {
            // Pozycje pionowe (œrodek)
            int centerX = (ClientSize.Width - startButton.Width) / 2 - 45;
            int baseY = ClientSize.Height / 2 + 75;

            // Skalowanie przycisku wzglêdem rozmiaru okna
            int buttonWidth = Math.Max(150, ClientSize.Width / 4);
            int buttonHeight = Math.Max(40, ClientSize.Height / 15);

            // Przypisz nowy rozmiar i pozycjê
            startButton.Size = new Size(buttonWidth, buttonHeight);
            settingsButton.Size = new Size(buttonWidth, buttonHeight);
            exitButton.Size = new Size(buttonWidth, buttonHeight);

            startButton.Location = new Point(centerX, baseY);
            settingsButton.Location = new Point(centerX, baseY + 75);
            exitButton.Location = new Point(centerX, baseY + 150);
        }

        /// <summary>
        /// U¿ycie przycisku Startu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartButton_Click(object? sender, EventArgs e)
        {
            // Wyœwietlenie okna ³adowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ³adowania
                Thread.Sleep(2000);
            }

            // Przejœcie do formularza SelectedLevel
            var selectedLevelForm = new SelectLevel();
            selectedLevelForm.Show();

            // Zamkniêcie bie¿¹cego formularza
            Hide();
        }

        /// <summary>
        /// U¿ycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            // Wyœwietlenie okna ³adowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ³adowania 
                Thread.Sleep(2000);
            }

            // Przejœcie do formularza Settings
            var settingsForm = new Settings();
            settingsForm.Show();

            // Zamkniêcie bie¿¹cego formularza
            Hide();
        }

    }
}
