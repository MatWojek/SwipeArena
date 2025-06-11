namespace SwipeArena
{
    public partial class Menu : Form
    {
        private Bitmap backgroundImage; 
        public Menu()
        {
            InitializeComponent();
            // Wczytanie ilustracji jako t�a
            backgroundImage = new Bitmap("Menu.webp"); 

            // Ustawienia formularza
            this.Text = "Menu G��wne";
            this.Size = new Size(800, 600); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Rysowanie t�a na ca�ym obszarze formularza
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, this.ClientRectangle);
            }
        }
        
        private void Menu_Load(object sender, EventArgs e)
        {

        }
    }
}
