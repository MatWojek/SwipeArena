using System.Drawing.Drawing2D;


namespace SwipeArena
{
    public partial class SelectLevel : BaseForm
    {

        Button settingsButton; 

        public SelectLevel()
        {

            InitializeComponent();

            // Asynchroniczne wczytanie ilustracji jako tła
            Task.Factory.StartNew(() =>
            {
                // Wczytanie obrazu w tle
                return Image.FromFile("images/background/select_level.png");
            })
            .ContinueWith(t =>
            {
                if (t.Exception == null) 
                {
                    panel1.BackgroundImage = t.Result;
                    panel1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    MessageBox.Show("Nie udało się wczytać obrazu: " + t.Exception.InnerException?.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            // Ustawienia formularza
            SettingsHelper.ApplySettings(this, "Wybierz poziom");

            settingsButton = new Button
            {
                Text = "Ustawienia",
                Size = new Size(100, 30),
                Location = new Point(10, 100)
            };
            settingsButton.Click += (s, e) => { new Settings().ShowDialog(); SettingsHelper.ApplySettings(this, "Ustawienia"); };
            Controls.Add(settingsButton);

            CreateLevelButtons();

            // Zmiana rozmiaru okna
            panel1.Resize += SelectLevel_Resize;

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);
        }


        const int levels = 10;
        const int margin = 100;
        Random random = new Random();
        List<Button> levelButtons = new List<Button>();

        /// <summary>
        /// Ustawienia wyboru leveli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateLevelButtons()
        {

            for (int i = 1; i <= levels; i++)
            {
                Button levelButton = new Button();
                levelButton.Text = "Poziom " + i;
                levelButton.Size = new Size(100, 100);

                // Losowa pozycja przycisków w granicach okna
                int randomX = random.Next(0, panel1.Width - levelButton.Width);
                levelButton.Location = new Point(randomX, i * margin);

                // Ustawienie okrągłego kształtu przycisku
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, levelButton.Width, levelButton.Height);
                levelButton.Region = new Region(path);

                // Styl przycisku
                levelButton.FlatStyle = FlatStyle.Flat;
                levelButton.FlatAppearance.BorderSize = 0;
                levelButton.ForeColor = Color.White;
                levelButton.BackColor = Color.FromArgb(230, 191, 70);

                // Przypisanie zdarzenia kliknięcia
                levelButton.Tag = i;
                levelButton.Click += LevelButton_Click;

                // Dodanie przycisku do panelu
                panel1.Controls.Add(levelButton);
                levelButtons.Add(levelButton);
            }
        }

        /// <summary>
        /// Wybranie levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LevelButton_Click(object sender, EventArgs e)
        {

            // Wyświetlenie okna ładowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ładowania 
                Thread.Sleep(2000);
            }

            var button = sender as Button;
            int levelNumber = (int)(button?.Tag ?? 1);

            int rows;
            int cols;

            // Określenie wielkości planszy na podstawie poziomu
            if (levelNumber >= 6)
            {
                rows = random.Next(4, 8);
                cols = random.Next(4, 8);
            }
            else
            {
                rows = random.Next(3, 3 + levelNumber);
                cols = random.Next(3, 3 + levelNumber);
            }

            // Przejście do Levelu
            var selectedLevel = new Level(levelNumber, rows, cols);
            selectedLevel.Show();

            // Zamknięcie bieżącego formularza
            Hide();
        }


        /// <summary>
        /// Obsługa zmiany rozmiaru okna
        /// </summary>
        private void SelectLevel_Resize(object sender, EventArgs e)
        {
            int buttonSize = Math.Min(panel1.Width / 10, 100);
            int currentMargin = panel1.Height / (levels + 1) + margin;

            for (int i = 0; i < levelButtons.Count; i++)
            {
                Button levelButton = levelButtons[i];

                // Przeliczenie rozmiaru przycisku
                levelButton.Size = new Size(buttonSize, buttonSize);

                // Losowe przesunięcie w granicach 10 pikseli
                int randomOffsetX = random.Next(-50, 50); 
                int randomOffsetY = random.Next(-50, 50);

                // Przeliczenie pozycji
                int x = (panel1.Width - buttonSize) / 2 + randomOffsetX;
                int y = (i + 1) * currentMargin + randomOffsetY;
                levelButton.Location = new Point(x, y);

                // Aktualizacja kształtu na okrągły
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(0, 0, levelButton.Width, levelButton.Height);
                levelButton.Region = new Region(path);
            }
        }
    }
}
