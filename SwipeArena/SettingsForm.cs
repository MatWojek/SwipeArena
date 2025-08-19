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
using SwipeArena.Helpers;
using SwipeArena.UI;
using SwipeArena.Config;

namespace SwipeArena
{
    /// <summary>
    /// Okno ustawień gry
    /// </summary>
    public partial class SettingsForm : BaseForm
    {
        Button _resetButton, _saveButton, _helpButton, _exitButton, _returnToMenuButton, _statsButton, _loadSaveButton, _loadSettingsButton, _returnButton;

        TrackBar _volumeMusic;

        Label _titleLabel, _volumeLabel, _aiLabel;

        ComboBox _changeResolution;

        System.Windows.Forms.CheckBox _aiCheckBox; 

        Panel _panelSettings;

        Image? _volumeOnIcon, _volumeOffIcon;

        SettingsData _settings = SettingsData.Instance;

        SaveLoad _saveLoad = new SaveLoad();

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
                    _volumeOnIcon = Image.FromFile("images/icons/dark_volume_up.png");
                    _volumeOffIcon = Image.FromFile("images/icons/dark_volume_off.png");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono ikon głośnika");
                }

                // Tworzenie panelu ustawień 
                _panelSettings = UIHelper.CreatePanel(
                    "PanelSettings",
                    new Size(ClientSize.Width - 100, ClientSize.Height - 50),
                    new Point(20, 20),
                    Color.FromArgb(240, 240, 240)
                ); 
                Controls.Add(_panelSettings);

                AddButtons();

                //Resize += (s, e) => ArrangeSettingsLayout();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        /// <summary>
        /// Tworzenie interfejsu
        /// </summary>
        void AddButtons()
        {

            // Dodanie napisu "Ustawienia"
            _titleLabel = UIHelper.CreateLabel(
                title: "TitleLabel",
                text: "Ustawienia",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 100, 20),
                fontStyle: FontStyle.Bold
                );
            _panelSettings.Controls.Add(_titleLabel);

            // Dodanie napisu "Zmiana głośności"
            _volumeLabel = UIHelper.CreateLabel(
                title: "VolumeLabel",
                text: $"Zmiana głośności [{Math.Round(_settings.Volume * 100)}%]",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 3 - 50)
                );
            _panelSettings.Controls.Add(_volumeLabel);

            // Suwak do ustawiania głośności muzyki
            _volumeMusic = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(_settings.Volume * 100),
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 1,
                Size = new Size(ClientSize.Width / 3, 40),
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 3)
            };
            _volumeMusic.Scroll += (s, e) =>
            {
                _settings.Volume = _volumeMusic.Value / 100.0;
                _volumeLabel.Text = $"Zmiana głośności [{Math.Round(_settings.Volume * 100)}%]";

            };

            _panelSettings.Controls.Add(_volumeMusic);

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

            _changeResolution = UIHelper.CreateComboBox(
                 title: "ChangeResolution",
                 dropDownStyle: ComboBoxStyle.DropDownList,
                 backColor: Color.White,
                 foreColor: Color.Black,
                 size: new Size(ClientSize.Width / 3, 40),
                 location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2)
            );


            // Dodanie opcji rozdzielczości do listy rozwijanej
            foreach (var resolution in resolutions)
            {
                _changeResolution.Items.Add($"{resolution.X}x{resolution.Y}");
            }

            // Ustawienie domyślnej wartości
            _changeResolution.SelectedIndex = resolutions.FindIndex(r => r == _settings.Resolution);

            // Obsługa zmiany rozdzielczości
            _changeResolution.SelectedIndexChanged += (s, e) =>
            {
                var selectedResolution = resolutions[_changeResolution.SelectedIndex];
                _settings.Resolution = selectedResolution;
                Size = new Size(_settings.Resolution.X, _settings.Resolution.Y);
            };
            _panelSettings.Controls.Add(_changeResolution);

            // ComboBox Funkcja AI
            _aiLabel = UIHelper.CreateLabel(
                title: "AiLabel",
                text: "Funkcja AI",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.Black,
                backColor: Color.Transparent,
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2 + 70)
            ); 
            _panelSettings.Controls.Add(_aiLabel);

            _aiCheckBox = UIHelper.CreateCheckBox(
                title: "AiCheckBox",
                text: "Włącz funkcję AI",
                backColor: Color.Transparent,
                foreColor: Color.Black,
                size: new Size(ClientSize.Width / 4, 40),
                location: new Point(ClientSize.Width / 2, ClientSize.Height / 2 + 70),
                isChecked: _settings.IsAIEnabled,
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize
            );

            _aiCheckBox.CheckedChanged += (s, e) =>
            {
                _settings.IsAIEnabled = _aiCheckBox.Checked; // Aktualizacja ustawienia na podstawie stanu CheckBoxa
            };
            _panelSettings.Controls.Add(_aiCheckBox);

            // Przycisk Wczytaj Ustawienia
            _loadSettingsButton = UIHelper.CreateButton(
                title: "LoadSettings",
                text: "Wczytaj Ustawienia",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 8, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 200),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _loadSettingsButton.Click += (s, e) =>
            {
                _settings.LoadFromSelectedFile();
                UpdateSettingsUI();
            };
            _panelSettings.Controls.Add(_loadSettingsButton);

            // Przycisk Wczytaj Ustawienia
            _loadSaveButton = UIHelper.CreateButton(
                title: "LoadSave",
                text: "Wczytaj Zapis",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 200),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _loadSaveButton.Click += (s, e) => _saveLoad.LoadFromSelectedFile();
            _panelSettings.Controls.Add(_loadSaveButton);

            // Przycisk Zresetuj
            _resetButton = UIHelper.CreateButton(
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
            _resetButton.Click += ResetButton_Click;
            _panelSettings.Controls.Add(_resetButton);

            // Przycisk Zapisz
            _saveButton = UIHelper.CreateButton(
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
            _saveButton.Click += SaveButton_Click;
            _panelSettings.Controls.Add(_saveButton);

            // Przycisk Statystyki
            _statsButton = UIHelper.CreateButton(
                title: "StatsButton",
                text: "Statystyki",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 200),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _statsButton.Click += StatsButton_Click;
            _panelSettings.Controls.Add(_statsButton);

            // Przycisk Pomocy
            _helpButton = UIHelper.CreateButton(
                title: "HelpButton",
                text: "Pomoc",
                backColor: Color.FromArgb(66, 197, 230),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 100),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            _helpButton.Click += HelpButton_Click;
            _panelSettings.Controls.Add(_helpButton);

            // Przycisk Wróć do Menu
            _returnToMenuButton = UIHelper.CreateButton(
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
            _returnToMenuButton.Click += ReturnToMenuButton_Click;
            _panelSettings.Controls.Add(_returnToMenuButton);

            // Przycisk Wyjście
            _exitButton = UIHelper.CreateButton(
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
            _exitButton.Click += ExitButton_Click;
            _panelSettings.Controls.Add(_exitButton);

            // Przycisk Powrotu
            _returnButton = UIHelper.CreateButton(
                title: "Return",
                text: "Powrót",
                backColor: Color.FromArgb(255, 102, 102),
                foreColor: Color.White,
                size: new Size(ClientSize.Width / 3, 40),
                location: new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 250),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            _returnButton.Click += ExitSettingsButton_Click;
            _panelSettings.Controls.Add(_returnButton);

            var allControls = _panelSettings.Controls.Cast<Control>().ToList();

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
        /// Aktualizacja ustawień po zmianie
        /// </summary>
        void UpdateSettingsUI()
        {
            // Aktualizacja suwaka głośności
            _volumeMusic.Value = (int)(_settings.Volume * 100);
            _volumeLabel.Text = $"Zmiana głośności [{Math.Round(_settings.Volume * 100)}%]";

            // Aktualizacja listy rozwijanej rozdzielczości
            var resolutionIndex = _changeResolution.Items.IndexOf($"{_settings.Resolution.X}x{_settings.Resolution.Y}");
            if (resolutionIndex >= 0)
            {
                _changeResolution.SelectedIndex = resolutionIndex;
            }

            // Aktualizacja stanu CheckBoxa AI
            _aiCheckBox.Checked = _settings.IsAIEnabled;

            // Aktualizacja rozmiaru okna
            Size = new Size(_settings.Resolution.X, _settings.Resolution.Y);
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
                _settings.SaveToFile();
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
                _settings.SaveToFile();
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
                _settings.SaveToFile();
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

                _settings.SaveToFile();

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
            _panelSettings.Size = new Size(ClientSize.Width - 40, ClientSize.Height - 40);
            _panelSettings.Location = new Point(20, 20);
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
                var result = MessageBox.Show(
                "Czy chcesz zresetować ustawienia?",
                "Zapisywanie ustawień",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    
                    _settings.Reset();

                    var result2 = MessageBox.Show(
                    "Czy chcesz zresetować zapis gry?",
                    "Zapisywanie ustawień",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        _saveLoad.Reset();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // Jeśli użytkownik wybierze "Cancel", zakończ metodę
                    return;
                }

                UpdateSettingsUI();

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
