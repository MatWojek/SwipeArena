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
    public partial class HelpForm : BaseForm
    {

        Panel panelSettings;

        Button exitButton;

        Label infoLabel;

        public HelpForm()
        {

            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/settings.png");
                SettingsHelper.ApplySettings(this, "Ustawienia");

                // Tworzenie panelu ustawień 
                panelSettings = UIHelper.CreatePanel(
                    "PanelSettings",
                    new Size(ClientSize.Width - 100, ClientSize.Height - 50),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                );
                Controls.Add(panelSettings);
                infoLabel = UIHelper.CreateLabel(
                    title: "GameInfoLabel",
                    text: "🎮 Swipe Arena\n\n" +
                          "Swipe Arena to logiczno-przygodowa gra polegająca na przeciąganiu elementów na planszy w celu wykonywania dopasowań," +
                          "zdobywania punktów oraz pokonywania przeciwników.\n\n" +
                          "🧠 Cel gry:\nZdobądź określoną ilość punktów, w odpowiedniej liczbie ruchów.\n\n" +
                          "🕹️ Jak grać:\n- Kliknij i przeciągnij element, by zamienić go z sąsiednim." +
                          "\n- Dopasuj 3 lub więcej identycznych elementów w rzędzie lub kolumnie.\n\n" +
                          "👨‍💻 Autor: Mateusz Wojciechowski\n" +
                          "Rok wydania: 2025\n" +
                          "Dziękuję za zagranie w Swipe Arena! 💙",
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
                    size: new Size(ClientSize.Width / 3, 40),
                    location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 200),
                    font: BasicSettings.FontFamily,
                    fontSize: BasicSettings.FontSize,
                    fontStyle: FontStyle.Bold
                    );
                exitButton.Click += (s, e) => NavigateBack();
                panelSettings.Controls.Add(exitButton);

                var allControls = panelSettings.Controls.Cast<Control>().ToList();

                AdjustControlLayoutForSettingsMedium(allControls);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

    }
}
