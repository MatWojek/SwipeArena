namespace SwipeArena
{
    public partial class StartWindowForm : Form
    {
        System.Windows.Forms.Timer transitionTimer;

        public StartWindowForm()
        {
            try
            {

                Icon = new Icon("images/ico/SwipeArenaIcon.ico");

                var settings = new SettingsData();

                InitializeComponent();

                // Asynchroniczne wczytanie ilustracji jako tła
                Task.Factory.StartNew(() =>
                {
                    // Wczytanie obrazu w tle
                    return Image.FromFile("images/background/StartWindow.png");
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
                // Ustawienia formularza
                Text = "Ekran Startowy";
                Size = new Size(settings.Resolution.X, settings.Resolution.Y);

                // Zablokowanie zmiany rozmiaru okna 
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;

                // Inicjalizacja timera
                transitionTimer = new System.Windows.Forms.Timer();
                transitionTimer.Interval = 3000;
                transitionTimer.Tick += TransitionTimer_Tick;
                transitionTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);
        }

        /// <summary>
        /// Płynne przechodzenie z ekranu startowego do Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TransitionTimer_Tick(object? sender, EventArgs e)
        {
            transitionTimer.Stop();

            var mainMenu = new MenuForm();
            mainMenu.Show();

            Hide();
        }


        /// <summary>
        /// Zamykanie całej aplikacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartWindow_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Zamknięcie całej aplikacji
            Application.Exit();
        }

        private void StartWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
