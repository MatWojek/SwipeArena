using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwipeArena
{
    public partial class StatsForm : BaseForm
    {
        Panel panelSettings;

        Button exitButton;

        Label infoLabel;

        SaveLoad saveLoad = new SaveLoad();

        public StatsForm()
        {
            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/settingsImage.png");
                SettingsHelper.ApplySettings(this, "Ustawienia");

                // Tworzenie panelu ustawień 
                panelSettings = UIHelper.CreatePanel(
                    "PanelSettings",
                    new Size(ClientSize.Width - 100, ClientSize.Height - 50),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                );
                Controls.Add(panelSettings);

                saveLoad.Load();

                // Wyświetlenie statystyk 
                infoLabel = UIHelper.CreateLabel(
                title: "GameInfoLabel",
                text: $"🎮 Swipe Arena Statystyki\n\n" +
                       $"Ostatnia ilość wygranych pod rząd: {saveLoad.CurrentWinStreak}\n" +
                       $"Najlepsza ilość wygranych pod rząd: {saveLoad.BestWinStreak}\n" +
                       $"Ostatni grany poziom: {saveLoad.LastLevelPlayed}\n" +
                       $"Ilość ukończonych poziomów: {saveLoad.LevelCompleted}\n" +
                       $"Najwyższy wynik zdobyty w pojedynczej grze: {saveLoad.MaxPoints}\n" +
                       $"Łącznie zdobyte punkty: {saveLoad.TotalPoints}\n" +
                       $"Łączny czas gry: {saveLoad.TimeGame}\n\n",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width - 100, ClientSize.Height / 2),
                fontStyle: FontStyle.Bold
                );
                panelSettings.Controls.Add(infoLabel);

                // Przycisk Wyjście
                exitButton = UIHelper.CreateButton(
                    title: "ExitButton",
                    text: "Wróć do Ustawień",
                    backColor: Color.FromArgb(255, 102, 102),
                    foreColor: Color.White,
                    //size: new Size(ClientSize.Width / 3, 40),
                    location: new Point(ClientSize.Width - 100, ClientSize.Height),
                    font: BasicSettings.FontFamily,
                    fontSize: BasicSettings.FontSize,
                    fontStyle: FontStyle.Bold
                    );
                exitButton.Click += (s, e) => NavigateBack();
                panelSettings.Controls.Add(exitButton);

                var allControls = panelSettings.Controls.Cast<Control>().ToList();

                AdjustControlLayoutForSettingsSmall(allControls);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
    }
}
