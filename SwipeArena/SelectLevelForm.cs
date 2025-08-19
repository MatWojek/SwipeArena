using System.Drawing.Drawing2D;
using SwipeArena.Config;
using SwipeArena.Helpers;
using SwipeArena.UI;


namespace SwipeArena
{
    /// <summary>
    /// Okno wybierania poziomu
    /// </summary>
    public partial class SelectLevelForm : BaseForm
    {

        Button _settingsButton, _levelButton;

        int _levels = BasicSettings.Levels;
        const int MARGIN = 100;

        Random _random = new Random();

        List<Button> _levelButtons = new List<Button>();

        public SelectLevelForm()
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

            _settingsButton = UIHelper.CreateButton(
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
            _settingsButton.Click += (s, e) =>
            {
                var settingsForm = new SettingsForm();
                NavigateToForm(this, settingsForm);
            };
            panel1.Controls.Add(_settingsButton);

            Create_levelButtons();

            // Zmiana rozmiaru okna
            panel1.Resize += SelectLevel_Resize;

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);

        }

        /// <summary>
        /// Ustawienia wyboru leveli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Create_levelButtons()
        {
            SaveLoad save = new SaveLoad();
            save.Load();

            if (save.GetLevelCompleted() < _levels)
            {
                for (int i = 1; i <= save.GetLevelCompleted() + 1; i++)
                {
                    int buttonWidth = 100;
                    int buttonHeight = 100;

                    // Losowa pozycja przycisków w granicach okna
                    int randomX = _random.Next(0, Math.Max(1, panel1.Width - buttonWidth));

                    _levelButton = UIHelper.CreateButton(
                        title: "LevelButton",
                        text: "Poziom " + i,
                        backColor: Color.FromArgb(169, 169, 169),
                        foreColor: Color.White,
                        size: new Size(buttonWidth, buttonHeight),
                        location: new Point(randomX, MARGIN * i),
                        flatStyle: FlatStyle.Flat,
                        fontStyle: FontStyle.Bold,
                        fontSize: 10
                        );

                    if (save.GetLevelCompleted() + 1 == i)
                    {
                        _levelButton.BackColor = Color.FromArgb(66, 197, 230);
                    }

                    else
                    {
                        _levelButton.BackColor = Color.FromArgb(169, 169, 169);
                    }

                    // Ustawienie okrągłego kształtu przycisku
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, _levelButton.Width, _levelButton.Height);
                    _levelButton.Region = new Region(path);

                    // Styl przycisku
                    _levelButton.FlatAppearance.BorderSize = 0;

                    // Przypisanie zdarzenia kliknięcia
                    _levelButton.Tag = i;
                    _levelButton.Click += LevelButton_Click;

                    // Dodanie przycisku do panelu
                    panel1.Controls.Add(_levelButton);
                    _levelButtons.Add(_levelButton);
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
        void SelectLevel_Resize(object sender, EventArgs e)
        {
            int buttonSize = Math.Min(panel1.Width / 10, 100);
            int currentMargin = panel1.Height / (_levels + 2) + MARGIN;

            for (int i = 0; i < _levelButtons.Count; i++)
            {
                Button levelButton = _levelButtons[i];

                // Przeliczenie rozmiaru przycisku
                levelButton.Size = new Size(buttonSize, buttonSize);


                // Losowe przesunięcie w granicach 10 pikseli
                int randomOffsetX = _random.Next(-50, 50); 
                int randomOffsetY = _random.Next(-50, 50);

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
