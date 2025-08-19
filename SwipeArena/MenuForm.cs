using Microsoft.VisualBasic.ApplicationServices;
using SwipeArena.Config;
using SwipeArena.Helpers;
using SwipeArena.UI;
using System.Threading;

namespace SwipeArena
{
    /// <summary>
    /// Okno Menu 
    /// </summary>
    public partial class MenuForm : BaseForm
    { 

        Button _startButton, _settingsButton, _exitButton;

        public MenuForm()
        {
            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/menuImage.png");
                SettingsHelper.ApplySettings(this, "Menu");

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
            // Przycisk Start 
            _startButton = UIHelper.CreateButton(
                title: "Start",
                text: "Start",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            _startButton.Click += _startButton_Click;
            Controls.Add(_startButton);

            // Przycisk Ustawienia
            _settingsButton = UIHelper.CreateButton(
                title: "Settings",
                text: "Ustawienia",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _settingsButton.Click += _settingsButton_Click;
            Controls.Add(_settingsButton);

            // Przycisk Wyjœcie
            _exitButton = UIHelper.CreateButton (
                title: "Exit",
                text: "WyjdŸ z Gry",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _exitButton.Click += (s, e) => Close();
            Controls.Add(_exitButton);

            var buttons = new List<Control> { _startButton, _settingsButton, _exitButton };
            AdjustButtonPositions(buttons);
        }

        /// <summary>
        /// U¿ycie przycisku Startu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _startButton_Click(object? sender, EventArgs e)
        {
            // Przejœcie do formularza SelectedLevel
            var selectedLevelForm = new SelectLevelForm();
            NavigateToForm(this, selectedLevelForm);  
        }

        /// <summary>
        /// U¿ycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _settingsButton_Click(object? sender, EventArgs e)
        {
            // Przejœcie do formularza Ustawienia
            var settingsForm = new SettingsForm();
            NavigateToForm(this, settingsForm);
           
        }

    }
}
