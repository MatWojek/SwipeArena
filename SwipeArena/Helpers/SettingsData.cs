using Newtonsoft.Json;
using SwipeArena.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Zapisywanie i wczytywanie ustawień aplikacji (rozdzielczości, głośności, funkcji AI itp.) 
    /// </summary>
    public class SettingsData
    {
        static SettingsData? _instance;

        // Właściwości ustawień
        public Point Resolution { get; set; } = new Point(BasicSettings.DefaultX, BasicSettings.DefaultY);
        public bool IsVolumeOn { get; set; } = BasicSettings.DefaultIsVolumeOn;

        double _volume = BasicSettings.DefaultMusicVolume;
        public double Volume
        {
            get => IsVolumeOn ? _volume : 0;
            set => _volume = value;
        }
        public bool IsAIEnabled { get; set; } = BasicSettings.DefualtIsAIEnabled;

        static readonly string SettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "settings.json");

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
        static SettingsData LoadFromFile()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    if (File.Exists(SettingsFile))
                    {
                        var json = File.ReadAllText(SettingsFile);
                        return JsonConvert.DeserializeObject<SettingsData>(json) ?? new SettingsData();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas ładowania ustawień: {ex.Message}");
                }
                return new SettingsData();
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
                var directory = Path.GetDirectoryName(SettingsFile);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisywania ustawień: {ex.Message}");
            }
        }

        /// <summary>
        /// Wczytywanie ustawień z wybranego pliku .json
        /// </summary>
        public void LoadFromSelectedFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.Title = "Wybierz plik ustawień";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(openFileDialog.FileName);
                        var loadedSettings = JsonConvert.DeserializeObject<SettingsData>(json);

                        if (loadedSettings != null)
                        {
                            // Aktualizacja właściwości instancji
                            Resolution = loadedSettings.Resolution;
                            IsVolumeOn = loadedSettings.IsVolumeOn;
                            Volume = loadedSettings.Volume;
                            IsAIEnabled = loadedSettings.IsAIEnabled;

                            // Zapisz nowe ustawienia do domyślnego pliku
                            SaveToFile();

                            MessageBox.Show("Ustawienia zostały pomyślnie załadowane i zapisane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się załadować ustawień z pliku.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Wystąpił błąd podczas wczytywania pliku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        /// <summary>
        /// Reset Ustawień
        /// </summary>
        public void Reset()
        {
            Resolution = new Point(BasicSettings.DefaultX, BasicSettings.DefaultY);
            IsVolumeOn = BasicSettings.DefaultIsVolumeOn;
            Volume = BasicSettings.DefaultMusicVolume;
            IsAIEnabled = BasicSettings.DefualtIsAIEnabled;

            // Usunięcie pliku ustawień
            if (File.Exists(SettingsFile))
            {
                File.Delete(SettingsFile);
            }
        }
    }
}
