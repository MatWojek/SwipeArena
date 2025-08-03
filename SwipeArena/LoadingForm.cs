using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwipeArena
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();

            // Ustawienia formularza
            Text = "Ładowanie...";
            Size = new Size(400, 200);
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            StartPosition = FormStartPosition.CenterScreen;

            // Dodanie etykiety z tekstem "Ładowanie..."
            var loadingLabel = new Label
            {
                Text = "Ładowanie...",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(150, 80)
            };
            Controls.Add(loadingLabel);

        }

        private void Loading_Load(object sender, EventArgs e)
        {

        }
    }
}
