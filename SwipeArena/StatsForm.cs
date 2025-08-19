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
using SwipeArena.Config;
using SwipeArena.Helpers;
using SwipeArena.UI;

namespace SwipeArena
{
    /// <summary>
    /// Okno Statystyk
    /// </summary>
    public partial class StatsForm : BaseForm
    {
        Panel _panelSettings;

        Button _exitButton;

        Label _infoLabel;

        SaveLoad _saveLoad = new SaveLoad();

        public StatsForm()
        {
            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/settingsImage.png");
                SettingsHelper.ApplySettings(this, "Ustawienia");

                // Tworzenie panelu ustawień 
                _panelSettings = UIHelper.CreatePanel(
                    "PanelSettings",
                    new Size(ClientSize.Width - 100, ClientSize.Height - 50),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                );
                Controls.Add(_panelSettings);

                _saveLoad.Load();

                // Wyświetlenie statystyk 
                _infoLabel = UIHelper.CreateLabel(
                title: "GameInfoLabel",
                text: $"🎮 Swipe Arena Statystyki\n\n" +
                       $"Ostatnia ilość wygranych pod rząd: {_saveLoad.GetCurrentWinStreak()}\n" +
                       $"Najlepsza ilość wygranych pod rząd: {_saveLoad.GetBestWinStreak()}\n" +
                       $"Ostatni grany poziom: {_saveLoad.GetLastLevelPlayed()}\n" +
                       $"Ilość ukończonych poziomów: {_saveLoad.GetLevelCompleted()}\n" +
                       $"Najwyższy wynik zdobyty w pojedynczej grze: {_saveLoad.GetMaxPoints()}\n" +
                       $"Łącznie zdobyte punkty: {_saveLoad.GetTotalPoints()}\n" +
                       $"Łączny czas gry: {Math.Round(_saveLoad.GetTimeGame())} min\n\n",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width - 100, ClientSize.Height / 2),
                fontStyle: FontStyle.Bold
                );
                _panelSettings.Controls.Add(_infoLabel);

                // Przycisk Wyjście
                _exitButton = UIHelper.CreateButton(
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
                _exitButton.Click += (s, e) => NavigateBack();
                _panelSettings.Controls.Add(_exitButton);

                var allControls = _panelSettings.Controls.Cast<Control>().ToList();

                if (ClientSize.Width < 1336)
                {
                    AdjustControlLayoutForSettingsSmall(allControls);
                }
                else
                {
                    AdjustControlLayoutForSettingsMedium(allControls);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
    }
}
