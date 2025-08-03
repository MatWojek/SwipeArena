using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwipeArena
{
    public partial class GameOverForm : BaseForm
    {

        Button exitLevelButton;
        Button restartLevelButton;
        Button settingsButton;
        Button menuButton;

        Random random = new Random(); 

        public GameOverForm()
        {
            try
            {
                InitializeComponent();

                // Ustawienia formularza
                LoadBackgroundImage("images/background/game_over.png");
                SettingsHelper.ApplySettings(this, "Przegrana");
                AddButtons();

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

            var buttons = new List<Control> { restartLevelButton, settingsButton, menuButton, exitLevelButton };
            AdjustButtonPositions(buttons);
        }


        /// <summary>
        /// Użycie przycisku Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuButton_Click(object sender, EventArgs e)
        {
            var menuForm = new MenuForm();
            NavigateToForm(this, menuForm);
        }

        /// <summary>
        /// Użycie przycisku Restartu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RestartButton_Click(object sender, EventArgs e)
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
