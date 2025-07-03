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

namespace SwipeArena
{
    public class SettingsData
    {
        private static readonly string SettingsFile = "settings.json";
        private static SettingsData? _instance;

        // Właściwości ustawień
        public Point Resolution { get; set; } = new Point(800, 600);
        public bool IsVolumeOn { get; set; } = true;

        double _volume = 0.5;
        public double Volume
        {
            get => IsVolumeOn ? _volume : 0; 
            set => _volume = value;
        }

        // Singleton - jedna instancja klasy
        public static SettingsData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadFromFile();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Wczytywanie ustawień z pliku
        /// </summary>
        /// <returns></returns>
        private static SettingsData LoadFromFile()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFile);
                    var settings = JsonConvert.DeserializeObject<SettingsData>(json);
                    if (settings != null)
                    {
                        return settings;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas wczytywania ustawień: {ex.Message}");
                }
            }

            // Jeśli plik nie istnieje lub wystąpił błąd, zwróć domyślne ustawienia
            return new SettingsData();
        }

        /// <summary>
        /// Zapisywanie ustawień do pliku
        /// </summary>
        public void SaveToFile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania ustawień: {ex.Message}");
            }
        }
    }


    public partial class Settings : Form
    {
        Button volumeMusic;
        Button volumeButton;

        ComboBox changeResolution;
        Button resetButton;
        Button saveButton;
        Button helpButton;

        Button exitButton;

        Image? volumeOnIcon;
        Image? volumeOffIcon;
           
        SettingsData settings = new SettingsData();

        public Settings()
        { 

            try
            { 
                Icon = new Icon("images/ico/SwipeArenaIcon.ico");

                InitializeComponent();

                // Zablokowanie zmiany rozmiaru okna
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;

                // Asynchroniczne wczytanie ilustracji jako tła
                Task.Factory.StartNew(() =>
                {
                    // Wczytanie obrazu w tle
                    return Image.FromFile("images/background/settings.png");
                })
                .ContinueWith(t =>
                {
                    if (t.Exception == null)
                    {
                        BackgroundImage = t.Result;
                        BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się wczytać obrazu: " + t.Exception.InnerException?.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

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

                // Ustawienia formularza
                Text = "Ustawienia";
                Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);

                AddButtons();
                UpdateButtonPositions();
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

            // Dodanie napisu "Ustawienia" na górze
            var titleLabel = new Label
            {
                Text = "Ustawienia",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(ClientSize.Width / 2 - 100, 20) // Wyśrodkowanie
            };
            Controls.Add(titleLabel);

            // Dodanie napisu "Zmiana głośności"
            var volumeLabel = new Label
            {
                Text = "Zmiana głośności",
                Font = new Font("Arial", 12, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(ClientSize.Width / 2 - 100, ClientSize.Height / 3 - 50)
            };
            Controls.Add(volumeLabel);

            // Suwak do ustawiania głośności muzyki
            var volumeMusic = new TrackBar
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
            Controls.Add(volumeMusic);

            // Przycisk Ustawień Muzyki
            volumeButton = new Button
            {
                Text = "",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Size = new Size(40, 40),
                Location = new Point(ClientSize.Width / 2 + 150, ClientSize.Height / 3),
                FlatAppearance = { BorderSize = 0 },
                BackgroundImage = settings.IsVolumeOn ? volumeOnIcon : volumeOffIcon,
                BackgroundImageLayout = ImageLayout.Stretch
            };
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
            Controls.Add(volumeButton);

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
                UpdateButtonPositions();
            };
            Controls.Add(changeResolution);

            // Przycisk Zresetuj
            resetButton = new Button
            {
                Text = "Zresetuj",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(ClientSize.Width / 4, 40),
                Location = new Point(ClientSize.Width / 2 - 200, ClientSize.Height - 150),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            resetButton.Click += ResetButton_Click;
            Controls.Add(resetButton);

            // Przycisk Zapisz
            var saveButton = new Button
            {
                Text = "Zapisz",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(ClientSize.Width / 4, 40),
                Location = new Point(ClientSize.Width / 2 + 50, ClientSize.Height - 150),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            saveButton.Click += SaveButton_Click;
            Controls.Add(saveButton);

            // Przycisk Pomocy
            helpButton = new Button
            {
                Text = "Pomoc",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(ClientSize.Width / 3, 40),
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 100),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            helpButton.Click += HelpButton_Click;
            Controls.Add(helpButton);

            // Przycisk Wyjście
            exitButton = new Button
            {
                Text = "Wyjście",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(ClientSize.Width / 3, 40),
                Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 50),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            exitButton.Click += ExitButton_Click;
            Controls.Add(exitButton);

        }

        /// <summary>
        /// Aktualizuje pozycje przycisków na podstawie rozmiaru okna.
        /// </summary>
        private void UpdateButtonPositions()
        {
            // Wyśrodkowanie przycisków i elementów na podstawie rozmiaru okna
            volumeButton.Location = new Point(ClientSize.Width / 2 + 150, ClientSize.Height / 3);
            changeResolution.Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height / 2);
            resetButton.Location = new Point(ClientSize.Width / 2 - 200, ClientSize.Height - 150);
            saveButton.Location = new Point(ClientSize.Width / 2 + 50, ClientSize.Height - 150);
            helpButton.Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 100);
            exitButton.Location = new Point(ClientSize.Width / 2 - 150, ClientSize.Height - 50);
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

            // Wyświetlenie okna ładowania
            using (var loadingForm = new Loading())
            {
                loadingForm.Show();
                loadingForm.Refresh();

                // Symulacja czasu ładowania
                Thread.Sleep(2000);
            }

            // Przejście do formularza Menu
            var menuForm = new Menu();
            menuForm.Show();

            // Zamknięcie bieżącego formularza
            Hide();
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
        /// Otworzenie okna Pomocy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HelpButton_Click(object? sender, EventArgs e)
        {

        }
    }     
}
