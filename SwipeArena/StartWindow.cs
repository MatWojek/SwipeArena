namespace SwipeArena
{
    public partial class StartWindow : Form
    {
        Bitmap? backgroundImage;
        System.Windows.Forms.Timer transitionTimer;

        public StartWindow()
        {
            try
            {
                InitializeComponent();

                // Wczytanie ilustracji jako tła
                if (File.Exists("images/StartWindow.png"))
                {
                    backgroundImage = new Bitmap("images/StartWindow.png");
                    BackgroundImage = backgroundImage;
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    MessageBox.Show("Image file not found");
                }

                // Ustawienia formularza
                Text = "Ekran Startowy";
                Size = new Size(800, 600);

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

            var mainMenu = new Menu();
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
    }
}
