using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwipeArena.Config;
using SwipeArena.Helpers;
using SwipeArena.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SwipeArena
{
    /// <summary>
    /// Okno wygranej
    /// </summary>
    public partial class LevelCompleteForm : BaseForm
    {

        Button _exitLevelButton, _restartLevelButton, _nextLevelButton, _menuButton;

        public LevelCompleteForm()
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
            _restartLevelButton = UIHelper.CreateButton(
                title: "Restart",
                text: "Restart",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            _restartLevelButton.Click += RestartLevelButton_Click;
            Controls.Add(_restartLevelButton);

            _menuButton = UIHelper.CreateButton( 
                title: "_menuButton", 
                text: "Wróć do Menu",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
            );
            _menuButton.Click += MenuButton_Click;
            Controls.Add(_menuButton);

            // Przycisk NextLevel
            _nextLevelButton = UIHelper.CreateButton(
                title: "_nextLevelButton",
                text: "Następny",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            _nextLevelButton.Click += NextLevelButton_Click;
            Controls.Add(_nextLevelButton);

            // Przycisk Wyjście
            _exitLevelButton = UIHelper.CreateButton(
                title: "Exit",
                text: "Wyjdź z Gry",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
                );
            _exitLevelButton.Click += (s, e) => Close();
            Controls.Add(_exitLevelButton);

            var buttons = new List<Control> {_nextLevelButton, _restartLevelButton, _menuButton, _exitLevelButton};
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
