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
    public partial class LevelComplete : BaseForm
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
                // Ustawienia formularza
                LoadBackgroundImage("images/background/level_complete.png");
                SettingsHelper.ApplySettings(this, "Wygrana!!!");

                AddButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Tworzenie nowych przycisków
        /// </summary>
        void AddButtons()
        {
            // Przycisk Restart
            restartLevelButton = UIHelper.CreateButton(
                title: "Restart",
                text: "Restart",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            restartLevelButton.Click += RestartButton_Click;
            Controls.Add(restartLevelButton);

            menuButton = UIHelper.CreateButton( 
                title: "MenuButton", 
                text: "Wyjdź do Menu",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
            );
            menuButton.Click += MenuButton_Click;
            Controls.Add(menuButton);

            // Przycisk NextLevel
            nextLevelButton = UIHelper.CreateButton(
                title: "NextLevelButton",
                text: "Następny",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            nextLevelButton.Click += NextButton_Click;
            Controls.Add(nextLevelButton);

            // Przycisk Ustawienia
            settingsButton = UIHelper.CreateButton(
                title: "Settings",
                text: "Ustawienia",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            settingsButton.Click += SettingsButton_Click;
            Controls.Add(settingsButton);

            // Przycisk Wyjście
            exitLevelButton = UIHelper.CreateButton(
                title: "Exit",
                text: "Wyjście z Gry",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            exitLevelButton.Click += (s, e) => Close();
            Controls.Add(exitLevelButton);

            var buttons = new List<Control> {restartLevelButton, menuButton, settingsButton, exitLevelButton};
            AdjustButtonPositions(buttons);

        }

        /// <summary>
        /// Użycie przycisku Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuButton_Click(object sender, EventArgs e)
        {
            // Przejście do formularza Menu
            var menuForm = new Menu();
            NavigateToForm(menuForm);
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
            var settingsForm = new Settings();
            NavigateToForm(settingsForm);
        }

    }
}
