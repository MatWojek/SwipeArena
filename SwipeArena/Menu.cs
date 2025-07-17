using Microsoft.VisualBasic.ApplicationServices;
using System.Threading;

namespace SwipeArena
{
    public partial class Menu : BaseForm
    { 

        Button startButton;
        Button settingsButton;
        Button exitButton;

        public Menu()
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
            startButton = UIHelper.CreateButton(
                title: "Start",
                text: "Start",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            startButton.Click += StartButton_Click;
            Controls.Add(startButton);

            // Przycisk Ustawienia
            Button settingsButton = UIHelper.CreateButton(
                title: "Settings",
                text: "Ustawienia",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            settingsButton.Click += SettingsButton_Click;
            Controls.Add(settingsButton);

            // Przycisk Wyjœcie
            exitButton = UIHelper.CreateButton (
                title: "Exit",
                text: "Wyjœcie",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            exitButton.Click += (s, e) => Close();
            Controls.Add(exitButton);

            var buttons = new List<Control> { startButton, settingsButton, exitButton };
            AdjustButtonPositions(buttons);
        }

        /// <summary>
        /// U¿ycie przycisku Startu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartButton_Click(object? sender, EventArgs e)
        {
            // Zapisanie bie¿¹cego formularza do historii 
            formHistory.Push(this);

            // Przejœcie do formularza SelectedLevel
            var selectedLevelForm = new SelectLevel();
            NavigateToForm(selectedLevelForm);  
        }

        /// <summary>
        /// U¿ycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            // Zapisanie bie¿¹cego formularza do historii 
            formHistory.Push(this);

            // Przejœcie do formularza Ustawienia
            var settingsForm = new Settings();
            NavigateToForm(settingsForm);
           
        }

    }
}
