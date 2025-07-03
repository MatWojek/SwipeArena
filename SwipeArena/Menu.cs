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

                // Asynchroniczne wczytanie ilustracji jako t�a
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
                        MessageBox.Show("Nie uda�o si� wczyta� obrazu: " + t.Exception.InnerException?.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                // Ustawienia formularza
                Text = "Menu G��wne";
                Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);
                AddButtons();

                AdjustButtonPositions();
                Resize += (s, e) => AdjustButtonPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            // Rejestracja obs�ugi zamkni�cia okna
            FormUtils.RegisterFormClosingHandler(this);

        }

        /// <summary>
        /// Tworzenie nowych przycisk�w
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

            // Przycisk Wyj�cie
            exitButton = new Button
            {
                Text = "Wyj�cie",
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
        /// Ustawienie pozycji przycisk�w
        /// </summary>
        void AdjustButtonPositions()
        {
            // Pozycje pionowe (�rodek)
            int centerX = (ClientSize.Width - startButton.Width) / 2 - 45;
            int baseY = ClientSize.Height / 2 + 75;

            // Skalowanie przycisku wzgl�dem rozmiaru okna
            int buttonWidth = Math.Max(150, ClientSize.Width / 4);
            int buttonHeight = Math.Max(40, ClientSize.Height / 15);

            // Przypisz nowy rozmiar i pozycj�
            startButton.Size = new Size(buttonWidth, buttonHeight);
            settingsButton.Size = new Size(buttonWidth, buttonHeight);
            exitButton.Size = new Size(buttonWidth, buttonHeight);

            startButton.Location = new Point(centerX, baseY);
            settingsButton.Location = new Point(centerX, baseY + 75);
            exitButton.Location = new Point(centerX, baseY + 150);
        }

        /// <summary>
        /// U�ycie przycisku Startu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartButton_Click(object? sender, EventArgs e)
        {
            // Wy�wietlenie okna �adowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu �adowania
                Thread.Sleep(2000);
            }

            // Przej�cie do formularza SelectedLevel
            var selectedLevelForm = new SelectLevel();
            selectedLevelForm.Show();

            // Zamkni�cie bie��cego formularza
            Hide();
        }

        /// <summary>
        /// U�ycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            // Wy�wietlenie okna �adowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu �adowania 
                Thread.Sleep(2000);
            }

            // Przej�cie do formularza Settings
            var settingsForm = new Settings();
            settingsForm.Show();

            // Zamkni�cie bie��cego formularza
            Hide();
        }

    }
}
