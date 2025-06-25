using System.Threading;

namespace SwipeArena
{
    public partial class Menu : Form
    {
        Bitmap? backgroundImage;

        Button startButton;
        Button settingsButton;
        Button exitButton;

        public Menu()
        {
            try
            {

                var settings = new SettingsData();

                InitializeComponent();

                // Wczytanie ilustracji jako t�a
                if (File.Exists("images/background/menu.png"))
                {
                    backgroundImage = new Bitmap("images/background/menu.png");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono obrazu");
                }

                // Ustawienia formularza
                Text = "Menu G��wne";
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);
                BackgroundImage = null;
                AddButtons();

                // Zablokowanie zmiany rozmiaru okna 
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;
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
                Size = new Size(190, 40),
                Location = new Point(310, 350),
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
                Size = new Size(190, 40),
                Location = new Point(310, 425),
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
                Size = new Size(190, 40),
                Location = new Point(310, 500),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            exitButton.Click += (s, e) => Close();
            Controls.Add(exitButton);
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

        /// <summary>
        /// Rysowanie menu
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Rysowanie t�a na ca�ym obszarze formularza
            if (backgroundImage != null)
            {
                // Skalowanie obrazu
                e.Graphics.DrawImage(backgroundImage, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            }
        }

    }
}
