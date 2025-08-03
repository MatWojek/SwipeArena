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
            restartLevelButton.Click += RestartLevelButton_Click;
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
            nextLevelButton.Click += NextLevelButton_Click;
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

            var buttons = new List<Control> {nextLevelButton, restartLevelButton, menuButton, settingsButton, exitLevelButton};
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
            var menuForm = new MenuForm();
            NavigateToForm(this, menuForm);
        }

        /// <summary>
        /// Przejście do kolejnego Levelu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NextLevelButton_Click(object sender, EventArgs e)
        {
            // Przejście do Levelu
            var selectedLevel = new LevelForm(LevelForm.currentLevel + 1);
            NavigateToForm(this, selectedLevel);
        }

        /// <summary>
        /// Użycie przycisku Restartu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RestartLevelButton_Click(object sender, EventArgs e)
        {
            // Przejście do Levelu
            var selectedLevel = new LevelForm(LevelForm.currentLevel);
            NavigateToForm(this, selectedLevel);

        }

        /// <summary>
        /// Użycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            NavigateToForm(this, settingsForm);
        }

    }
}
