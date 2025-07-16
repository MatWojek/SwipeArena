using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SwipeArena
{
    public partial class LevelComplete : Form
    {

        Button exitLevelButton;
        Button restartLevelButton;
        Button nextLevelButton;
        Button settingsButton;
        Button menuButton;

        Random random = new Random();

        public LevelComplete()
        {
            try
            {
                Icon = new Icon("images/ico/SwipeArenaIcon.ico");

                InitializeComponent();

                // Zablokowanie zmiany rozmiaru okna
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;

                // Asynchroniczne wczytanie ilustracji jako tła
                Task.Factory.StartNew(() =>
                {
                    // Wczytanie obrazu w tle
                    return Image.FromFile("images/background/game_over.png");
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

                // Ustawienia formularza
                Text = "Menu Główne";
                Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);
                AddButtons();

                AdjustButtonPositions();
                Resize += (s, e) => AdjustButtonPositions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);
        }

        /// <summary>
        /// Tworzenie nowych przycisków
        /// </summary>
        void AddButtons()
        {
            // Przycisk Restart
            restartLevelButton = new Button
            {
                Text = "Restart",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(66, 197, 230),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            restartLevelButton.Click += RestartButton_Click;
            Controls.Add(restartLevelButton);

            menuButton = new Button
            {
                Text = "Restart",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(66, 197, 230),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            menuButton.Click += MenuButton_Click;
            Controls.Add(menuButton);

            // Przycisk NextLevel
            nextLevelButton = new Button
            {
                Text = "Ustawienia",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            nextLevelButton.Click += NextButton_Click;
            Controls.Add(nextLevelButton);

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

            // Przycisk Wyjście
            exitLevelButton = new Button
            {
                Text = "Wyjście",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            exitLevelButton.Click += (s, e) => Close();

            Controls.Add(exitLevelButton);
        }


        /// <summary>
        /// Użycie przycisku Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuButton_Click(object sender, EventArgs e)
        {
            // Wyświetlenie okna ładowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ładowania
                Thread.Sleep(2000);
            }

            // Przejście do formularza Menu
            var menuForm = new Menu();
            menuForm.Show();

            // Zamknięcie bieżącego formularza
            Hide();
        }


        void NextButton_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Użycie przycisku Restartu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RestartButton_Click(object sender, EventArgs e)
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
        /// Użycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            // Wyświetlenie okna ładowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ładowania 
                Thread.Sleep(2000);
            }

            // Przejście do formularza Settings
            var settingsForm = new Settings();
            settingsForm.Show();

            // Zamknięcie bieżącego formularza
            Hide();
        }



        /// <summary>
        /// Ustawienie pozycji przycisków
        /// </summary>
        void AdjustButtonPositions()
        {
            // Pozycje pionowe (środek)
            int centerX = (ClientSize.Width - restartLevelButton.Width) / 2 - restartLevelButton.Width;
            int baseY = ClientSize.Height / 2 + 75;

            // Skalowanie przycisku względem rozmiaru okna
            int buttonWidth = Math.Max(150, ClientSize.Width / 4);
            int buttonHeight = Math.Max(40, ClientSize.Height / 15);

            // Przypisz nowy rozmiar i pozycję
            restartLevelButton.Size = new Size(buttonWidth, buttonHeight);
            settingsButton.Size = new Size(buttonWidth, buttonHeight);
            exitLevelButton.Size = new Size(buttonWidth, buttonHeight);

            restartLevelButton.Location = new Point(centerX, baseY);
            settingsButton.Location = new Point(centerX, baseY + 75);
            exitLevelButton.Location = new Point(centerX, baseY + 150);
        }

    }
}
