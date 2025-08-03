using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

// TODO: 
// Przed wyjściem wyskoczy MessageBox z pytaniem czy zapisać zmiany 
// Będzie można kliknąć ikonę głośnika wtedy dźwięki zostaną wyciszone 
// Będzie również pasek do ustawienia głośności 
// Będzie zmiana rozdzielczości przez którą użytkownik będzie mógł zmienić rozmiar okna i automatycznie będzie się wszystko ustawiać
// W pliku settings, będą wszystkie ustawienia związane z rozmiarem okna, z ustawieniami głośności, będą one również zapisywane do pliku
// Zapis gry i ustawień w jsonie
// Zapisz powoduje przejście do ostatniego otwartego okna

namespace SwipeArena
{
    public partial class SettingsForm : BaseForm
    {
        Button resetButton, saveButton, helpButton, exitButton, returnToMenuButton, statsButton;

        TrackBar volumeMusic;

        Label titleLabel, volumeLabel, aiLabel;

        ComboBox changeResolution;

        System.Windows.Forms.CheckBox aiCheckBox; 

        Panel panelSettings;

        Image? volumeOnIcon, volumeOffIcon;

        SettingsData settings = SettingsData.Instance;

        public SettingsForm()
        {

            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/settingsImage.png");
                SettingsHelper.ApplySettings(this, "Ustawienia");

                // Wczytanie ikon głośnika
                if (File.Exists("images/icons/dark_volume_up.png") && File.Exists("images/icons/dark_volume_off.png"))
                {
                    volumeOnIcon = Image.FromFile("images/icons/dark_volume_up.png");
                    volumeOffIcon = Image.FromFile("images/icons/dark_volume_off.png");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono ikon głośnika");
                }

                // Tworzenie panelu ustawień 
                panelSettings = UIHelper.CreatePanel(
                    "PanelSettings",
                    new Size(ClientSize.Width - 100, ClientSize.Height - 50),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                ); 
                Controls.Add(panelSettings);

                AddButtons();

                //Resize += (s, e) => ArrangeSettingsLayout();

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

            // Dodanie napisu "Ustawienia"
            titleLabel = UIHelper.CreateLabel(
                title: "TitleLabel",
                text: "Ustawienia",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 100, 20),
                fontStyle: FontStyle.Bold
                );
            panelSettings.Controls.Add(titleLabel);

            // Dodanie napisu "Zmiana głośności"
            volumeLabel = UIHelper.CreateLabel(
                title: "VolumeLabel",
                text: $"Zmiana głośności [{Math.Round(settings.Volume * 100)}%]",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 3 - 50)
                );
            panelSettings.Controls.Add(volumeLabel);

            // Suwak do ustawiania głośności muzyki
            volumeMusic = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(settings.Volume * 100),
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 1,
                Size = new Size(ClientSize.Width / 3, 40),
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 3)
            };
            volumeMusic.Scroll += (s, e) =>
            {
                settings.Volume = volumeMusic.Value / 100.0;
                volumeLabel.Text = $"Zmiana głośności [{Math.Round(settings.Volume * 100)}%]";

            };

            panelSettings.Controls.Add(volumeMusic);

            // Lista rozwijana zmiany rozdzielczości 
            var resolutions = new List<Point>
            {
                new Point(1024, 768),
                new Point(1174, 778),
                new Point(1280, 826),
                new Point(1336, 864), 
                new Point(1600, 900),
                new Point(1920, 1080)
            };

            changeResolution = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Size = new Size(ClientSize.Width / 3, 40),
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2)
            };


            // Dodanie opcji rozdzielczości do listy rozwijanej
            foreach (var resolution in resolutions)
            {
                changeResolution.Items.Add($"{resolution.X}x{resolution.Y}");
            }

            // Ustawienie domyślnej wartości
            changeResolution.SelectedIndex = resolutions.FindIndex(r => r == settings.Resolution);

            // Obsługa zmiany rozdzielczości
            changeResolution.SelectedIndexChanged += (s, e) =>
            {
                var selectedResolution = resolutions[changeResolution.SelectedIndex];
                settings.Resolution = selectedResolution;
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);
            };
            panelSettings.Controls.Add(changeResolution);

            // ComboBox Funkcja AI
            aiLabel = UIHelper.CreateLabel(
                title: "AiLabel",
                text: "Funkcja AI",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2 + 70)
            ); 
            panelSettings.Controls.Add(aiLabel);

            aiCheckBox = new System.Windows.Forms.CheckBox
            {
                Text = "Włącz funkcję AI", // Tekst obok CheckBoxa
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                Size = new Size(ClientSize.Width / 4, 40),
                Location = new Point(ClientSize.Width / 2, ClientSize.Height / 2 + 70),
                Checked = settings.IsAIEnabled // Ustawienie domyślnego stanu
            };
            aiCheckBox.CheckedChanged += (s, e) =>
            {
                settings.IsAIEnabled = aiCheckBox.Checked; // Aktualizacja ustawienia na podstawie stanu CheckBoxa
            };
            panelSettings.Controls.Add(aiCheckBox);


            // Przycisk Statystyki
            statsButton = UIHelper.CreateButton(
                title: "StatsButton",
                text: "Statystyki",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 200),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            statsButton.Click += StatsButton_Click;
            panelSettings.Controls.Add(statsButton);

            // Przycisk Wróć do Menu
            returnToMenuButton = UIHelper.CreateButton(
                title: "ReturnMenu",
                text: "Wróć do Menu",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 250),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            returnToMenuButton.Click += ReturnToMenuButton_Click;
            panelSettings.Controls.Add(returnToMenuButton);

            // Przycisk Zresetuj
            resetButton = UIHelper.CreateButton(
                title: "ResetButton",
                text: "Zresetuj",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 4, 40),
                location: new Point(ClientSize.Width / 2 - 200, ClientSize.Height - 150),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            resetButton.Click += ResetButton_Click;
            panelSettings.Controls.Add(resetButton);

            // Przycisk Zapisz
            saveButton = UIHelper.CreateButton(
                title: "SaveButton",
                text: "Zapisz",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 4, 40),
                location: new Point(ClientSize.Width / 2 + 50, ClientSize.Height - 150),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            saveButton.Click += SaveButton_Click;
            panelSettings.Controls.Add(saveButton);

            // Przycisk Pomocy
            helpButton = UIHelper.CreateButton(
                title: "HelpButton",
                text: "Pomoc",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 100),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            helpButton.Click += HelpButton_Click;
            panelSettings.Controls.Add(helpButton);

            // Przycisk Wyjście
            exitButton = UIHelper.CreateButton(
                title: "ExitButton",
                text: "Wyjdź z Gry",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 50),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            exitButton.Click += ExitButton_Click;
            panelSettings.Controls.Add(exitButton);

            var allControls = panelSettings.Controls.Cast<Control>().ToList();

            if (ClientSize.Width < 1336)
            {
                AdjustControlLayoutForSettingsSmall(allControls);
            }
            else
            {
                AdjustControlLayoutForSettingsMedium(allControls);
            }

            Resize += (s, e) =>
            {
                if (ClientSize.Width < 1336)
                {
                    AdjustControlLayoutForSettingsSmall(allControls);
                }
                else
                {
                    AdjustControlLayoutForSettingsMedium(allControls);
                }
                ArrangeSettingsLayout();
            };
        }

        /// <summary>
        /// Przejście do formularza Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ReturnToMenuButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Czy chcesz zapisać zmiany przed wyjściem?",
                "Zapisywanie ustawień",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                settings.SaveToFile();
            }
            else if (result == DialogResult.Cancel)
            {
                // Jeśli użytkownik wybierze "Cancel", zakończ metodę
                return;
            }

            var menuForm = new MenuForm();
            NavigateToForm(this, menuForm);
        }

        /// <summary>
        /// Przejście do formularza statystyk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StatsButton_Click(object sender, EventArgs e)
        {
            var stats = new StatsForm();
            NavigateToForm(this, stats);
        }

        /// <summary>
        /// Obsługa zdarzeń po wciśniętu przycisku Wyjścia z Ustawień
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExitButton_Click(object? sender, EventArgs e)
        {

            var result = MessageBox.Show(
                "Czy chcesz zapisać zmiany przed wyjściem?",
                "Zapisywanie ustawień",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                settings.SaveToFile();
            }
            else if (result == DialogResult.Cancel)
            {
                // Jeśli użytkownik wybierze "Cancel", zakończ metodę
                return;
            }

            Close();
        }

        /// <summary>
        /// Wyjście z ustawień
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExitSettingsButton_Click(object? sender, EventArgs e)
        {

            var result = MessageBox.Show(
                "Czy chcesz zapisać zmiany przed wyjściem?",
                "Zapisywanie ustawień",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                settings.SaveToFile();
            }
            else if (result == DialogResult.Cancel)
            {
                // Jeśli użytkownik wybierze "Cancel", zakończ metodę
                return;
            }

            NavigateBack();
        }

        /// <summary>
        /// Zapisywanie postępu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {

                var saveLoad = new SaveLoad();

                saveLoad.Load();
                saveLoad.Save();

                settings.SaveToFile();

                MessageBox.Show("Postęp i ustawienia zostały zapisane.", "Zapis postępu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisywania postępu i ustawień: {ex.Message}", "Błąd zapisu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dynamiczne rozmieszczanie kontrolek po zmianie rozdzielczości
        /// </summary>
        void ArrangeSettingsLayout()
        {
            // Panel główny
            panelSettings.Size = new Size(ClientSize.Width - 40, ClientSize.Height - 40);
            panelSettings.Location = new Point(20, 20);
        }


        /// <summary>
        /// Resetowanie postępu oraz ustawień do domyślnych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ResetButton_Click(object? sender, EventArgs e)
        {
            try
            {
                settings.Resolution = new Point(BasicSettings.DefaultX, BasicSettings.DefaultY);
                settings.IsVolumeOn = BasicSettings.DefaultIsVolumeOn;
                settings.Volume = BasicSettings.DefaultMusicVolume;
                settings.IsAIEnabled = BasicSettings.DefualtIsAIEnabled;

                // Aktualizacja UI
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);

                MessageBox.Show("Ustawienia zostały zresetowane do domyślnych.", "Reset ustawień", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas resetowania ustawień: {ex.Message}", "Błąd resetu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Otworzenie okna Pomocy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HelpButton_Click(object? sender, EventArgs e)
        {
            var helpButton = new HelpForm();
            NavigateToForm(this, helpButton);
        }
    }     
}
