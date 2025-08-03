using System.Drawing.Drawing2D;


namespace SwipeArena
{
    public partial class SelectLevelForm : BaseForm
    {

        Button settingsButton, levelButton; 

        public SelectLevelForm()
        {

            InitializeComponent();

            // Włącz przewijanie w panelu
            panel1.AutoScroll = true;

            // Obsługa zdarzenia Scroll
            panel1.Scroll += Panel1_Scroll;

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

            settingsButton = UIHelper.CreateButton(
                title: "Settings",
                text: "Ustawienia",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(170, 50),
                location: new Point(ClientSize.Width / 3, 10),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            settingsButton.Click += (s, e) => { new SettingsForm().ShowDialog(); SettingsHelper.ApplySettings(this, "Ustawienia"); };
            panel1.Controls.Add(settingsButton);

            CreateLevelButtons();

            // Zmiana rozmiaru okna
            panel1.Resize += SelectLevel_Resize;

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);
        }

        void Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            // Aktualizacja pozycji przycisku "Ustawienia" w zależności od przesunięcia scrolla
            settingsButton.Location = new Point(
                settingsButton.Location.X,
                10 - panel1.AutoScrollPosition.Y
            );
        }

        const int levels = 15;
        const int margin = 100;
        Random random = new Random();
        List<Button> levelButtons = new List<Button>();

        /// <summary>
        /// Ustawienia wyboru leveli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CreateLevelButtons()
        {

            SaveLoad save = new SaveLoad();
            save.Load();

            if (save.LevelCompleted < levels)
            {
                for (int i = 1; i <= save.LevelCompleted + 1; i++)
                {
                    int buttonWidth = 100;
                    int buttonHeight = 100;

                    // Losowa pozycja przycisków w granicach okna
                    int randomX = random.Next(0, Math.Max(1, panel1.Width - buttonWidth));

                    levelButton = UIHelper.CreateButton(
                        title: "LevelButton",
                        text: "Poziom " + i,
                        backColor: Color.FromArgb(169, 169, 169),
                        foreColor: Color.White,
                        size: new Size(buttonWidth, buttonHeight),
                        location: new Point(randomX, margin * i),
                        flatStyle: FlatStyle.Flat,
                        fontStyle: FontStyle.Bold,
                        fontSize: 10
                        );

                    if (save.LevelCompleted + 1 == i)
                    {
                        levelButton.BackColor = Color.Red;
                    }

                    else
                    {
                        levelButton.BackColor = Color.FromArgb(169, 169, 169);
                    }

                    // Ustawienie okrągłego kształtu przycisku
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, levelButton.Width, levelButton.Height);
                    levelButton.Region = new Region(path);

                    // Styl przycisku
                    levelButton.FlatAppearance.BorderSize = 0;

                    // Przypisanie zdarzenia kliknięcia
                    levelButton.Tag = i;
                    levelButton.Click += LevelButton_Click;

                    // Dodanie przycisku do panelu
                    panel1.Controls.Add(levelButton);
                    levelButtons.Add(levelButton);
                }
            }
        }

        /// <summary>
        /// Wybranie levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LevelButton_Click(object sender, EventArgs e)
        {

            var button = sender as Button;
            int levelNumber = (int)(button?.Tag ?? 1);

            // Przejście do Levelu
            var selectedLevel = new LevelForm(levelNumber);
            NavigateToForm(this, selectedLevel);

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
