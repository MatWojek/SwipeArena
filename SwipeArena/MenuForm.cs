using Microsoft.VisualBasic.ApplicationServices;
using System.Threading;

namespace SwipeArena
{
    public partial class MenuForm : BaseForm
    { 

        Button startButton;
        Button settingsButton;
        Button exitButton;

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
        /// Tworzenie nowych przycisk�w
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

            // Przycisk Wyj�cie
            exitButton = UIHelper.CreateButton (
                title: "Exit",
                text: "Wyj�cie",
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
        /// U�ycie przycisku Startu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartButton_Click(object? sender, EventArgs e)
        {
            // Przej�cie do formularza SelectedLevel
            var selectedLevelForm = new SelectLevelForm();
            NavigateToForm(this, selectedLevelForm);  
        }

        /// <summary>
        /// U�ycie przycisku Ustawienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SettingsButton_Click(object? sender, EventArgs e)
        {
            // Przej�cie do formularza Ustawienia
            var settingsForm = new SettingsForm();
            NavigateToForm(this, settingsForm);
           
        }

    }
}
