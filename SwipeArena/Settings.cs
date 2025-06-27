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
        public double Volume { get; set; } = 0.5;
        public bool IsVolumeOn { get; set; } = true;


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
                if (File.Exists("images/icons/volume_up.svg") && File.Exists("images/icons/volume_off.svg"))
                {
                    volumeOnIcon = Image.FromFile("images/icons/volume_up.svg");
                    volumeOffIcon = Image.FromFile("images/icons/volume_off.svg");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono ikon głośnika");
                }

                // Ustawienia formularza
                Text = "Ustawienia";
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);
                AddButtons();
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

            // Suwak do ustawiania głośności muzyki
            var volumeMusic = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(settings.Volume * 100), // Ustawienie domyślnej wartości
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 1,
                Size = new Size(190, 40),
                Location = new Point(310, 300),
                BackColor = Color.White
            };

            volumeMusic.Scroll += (s, e) =>
            {
                settings.Volume = volumeMusic.Value / 100.0; // Aktualizacja głośności
            };
            Controls.Add(volumeMusic);

            // Przycisk Ustawień Muzyki
            volumeButton = new Button
            {
                Text = "",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Size = new Size(40, 40),
                Location = new Point(510, 300),
                FlatAppearance = { BorderSize = 0 },
                BackgroundImage = settings.IsVolumeOn ? volumeOnIcon : volumeOffIcon,
                BackgroundImageLayout = ImageLayout.Stretch
            };
            volumeButton.Click += (s, e) =>
            {
                settings.IsVolumeOn = !settings.IsVolumeOn;
                volumeButton.BackgroundImage = settings.IsVolumeOn ? volumeOnIcon : volumeOffIcon;
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
                Size = new Size(190, 40),
                Location = new Point(310, 425),
                Font = new Font("Arial", 12, FontStyle.Regular)
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
            Controls.Add(changeResolution);

            // Przycisk Pomocy
            helpButton = new Button
            {
                Text = "Pomoc",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(190, 40),
                Location = new Point(310, 500),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            helpButton.Click += HelpButton_Click;
            helpButton.Controls.Add(helpButton);


            // Przycisk Resetu 
            resetButton = new Button
            {
                Text = "Zresetuj",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(190, 40),
                Location = new Point(310, 500),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            resetButton.Click += ResetButton_Click;
            resetButton.Controls.Add(resetButton);

            // Przycisk Wyjście
            exitButton = new Button
            {
                Text = "Wyjście",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(67, 203, 107),
                ForeColor = Color.White,
                Size = new Size(190, 40),
                Location = new Point(310, 500),
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Arial", 15, FontStyle.Bold)
            };
            exitButton.Click += (s, e) => Close();
            Controls.Add(exitButton);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ResetButton_Click(object? sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void VolumeButton_Click(object? sender, EventArgs e)
        {

        }

        void HelpButton_Click(object? sender, EventArgs e)
        {

        }
    }     
}
