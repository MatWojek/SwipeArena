namespace SwipeArena
{
    public partial class Menu : Form
    {
        private Bitmap backgroundImage; 
        public Menu()
        {
            InitializeComponent();
            // Wczytanie ilustracji jako t³a
            backgroundImage = new Bitmap("Menu.webp"); 

            // Ustawienia formularza
            this.Text = "Menu G³ówne";
            this.Size = new Size(800, 600); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Rysowanie t³a na ca³ym obszarze formularza
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
