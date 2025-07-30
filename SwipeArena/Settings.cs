using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    public partial class Settings : BaseForm
    {
        Button volumeButton, resetButton, saveButton, helpButton, exitButton, returnToMenuButton, statsButton, closeSettingsButton;

        TrackBar volumeMusic;

        Label titleLabel, volumeLabel, aiLabel;

        ComboBox changeResolution, aiComboBox;

        Panel panelSettings;

        Image? volumeOnIcon, volumeOffIcon;
           
        SettingsData settings = new SettingsData();

        public Settings()
        {

            try
            {
                InitializeComponent();

                // Ustawienia Formularza 
                LoadBackgroundImage("images/background/settings.png");
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
                    new Size(ClientSize.Width - 40, ClientSize.Height - 40),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                ); 
                Controls.Add(panelSettings);

                AddButtons();

                Resize += (s, e) => ArrangeSettingsLayout();

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

            closeSettingsButton = UIHelper.CreateButton(
                title: "CloseSettingsButton",
                text: "",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                size: new Size(0, 10),
                location: new Point(ClientSize.Width / 3 + 300, 20),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold,
                backgroundImage: Image.FromFile("images/icons/light_close.png")
                );
            panelSettings.Controls.Add(closeSettingsButton);

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

                // Zmiana ikony w zależności od poziomu głośności
                if (settings.Volume == 0)
                {
                    volumeButton.BackgroundImage = volumeOffIcon;
                }
                else if (settings.Volume <= 0.6)
                {
                    volumeButton.BackgroundImage = Image.FromFile("images/icons/dark_volume_mute.png");
                }
                else
                {
                    volumeButton.BackgroundImage = Image.FromFile("images/icons/dark_volume_up.png");
                }
            };
            panelSettings.Controls.Add(volumeMusic);

            // Przycisk Ustawień Muzyki
            volumeButton = UIHelper.CreateButton(
                title: "VolumeButton",
                text: "",
                backColor: Color.Transparent,
                foreColor: Color.Black,
                size: new Size(40, 40),
                location: new Point(ClientSize.Width / 2 + 150, ClientSize.Height / 3), 
                backgroundImage: settings.IsVolumeOn ? volumeOnIcon : volumeOffIcon,
                imageLayout: ImageLayout.Stretch
                );
            volumeButton.Click += (s, e) =>
            {
                settings.IsVolumeOn = !settings.IsVolumeOn;

                if (!settings.IsVolumeOn)
                {
                    settings.Volume = 0;
                    volumeMusic.Value = 0;
                    volumeButton.BackgroundImage = volumeOffIcon;
                }
                else
                {
                    settings.Volume = 0.5; // Domyślna wartość po włączeniu
                    volumeMusic.Value = (int)(settings.Volume * 100);
                    volumeButton.BackgroundImage = Image.FromFile("images/icons/dark_volume_mute.png");
                }
            };
            panelSettings.Controls.Add(volumeButton);

            // Lista rozwijana zmiany rozdzielczości 

            var resolutions = new List<Point>
            {
                new Point(800, 600),
                new Point(1024, 768),
                new Point(1280, 720),
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

            aiComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Size = new Size(ClientSize.Width / 4, 40),
                Location = new Point(ClientSize.Width / 2, ClientSize.Height / 2 + 70)
            };
            aiComboBox.Items.AddRange(new string[] { "Wyłączona", "Włączona" });
            aiComboBox.SelectedIndex = 0; // domyślnie wyłączona
            panelSettings.Controls.Add(aiComboBox);

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
            statsButton.Click += (s, e) => MessageBox.Show("Statystyki gracza – TODO");
            panelSettings.Controls.Add(statsButton);

            // Przycisk Wyjdź do menu
            returnToMenuButton = UIHelper.CreateButton(
                title: "ReturnMenu",
                text: "Wyjdź do Menu",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 250),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            returnToMenuButton.Click += (s, e) => { /* TODO: przejście do menu */ };
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
            AdjustControlLayoutForSettings(allControls);

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
                SettingsData.Instance.SaveToFile();
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
                saveLoad.Save();
                MessageBox.Show("Postęp został zapisany.", "Zapis postępu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisywania postępu: {ex.Message}", "Błąd zapisu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                settings.Resolution = new Point(800, 600);
                settings.IsVolumeOn = true;
                settings.Volume = 0.5;

                // Aktualizacja UI
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);
                volumeButton.BackgroundImage = volumeOnIcon;

                MessageBox.Show("Ustawienia zostały zresetowane do domyślnych.", "Reset ustawień", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas resetowania ustawień: {ex.Message}", "Błąd resetu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dynamiczne rozmieszczanie kontrolek po zmianie rozdzielczości
        /// </summary>
        void ArrangeSettingsLayout()
        {
            foreach (Control ctrl in panelSettings.Controls)
            {
                if (ctrl is Button || ctrl is ComboBox || ctrl is TrackBar)
                {
                    ctrl.Width = ClientSize.Width / 3;
                    ctrl.Height = 40;
                    ctrl.Left = (ClientSize.Width - ctrl.Width) / 2;
                }
            }

            // Panel główny
            panelSettings.Size = new Size(ClientSize.Width - 40, ClientSize.Height - 40);
            panelSettings.Location = new Point(20, 20);
        }

        /// <summary>
        /// Otworzenie okna Pomocy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HelpButton_Click(object? sender, EventArgs e)
        {
            // Zapisanie bieżącego formularza do historii 
            formHistory.Push(this);

            var helpButton = new Help();
            NavigateToForm(helpButton);
        }
    }     
}
