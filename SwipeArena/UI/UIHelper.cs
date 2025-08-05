using System.Drawing;
using System.Drawing.Drawing2D;
using static System.Net.Mime.MediaTypeNames;

namespace SwipeArena.UI
{
    public static class UIHelper
    {

        /// <summary>
        /// Tworzenie Button
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="backColor"></param>
        /// <param name="foreColor"></param>
        /// <param name="size"></param>
        /// <param name="location"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontStyle"></param>
        /// <param name="flatStyle"></param>
        /// <param name="borderSize"></param>
        /// <returns></returns>
        public static Button CreateButton(
            string title,
            string text,
            Color backColor,
            Color foreColor,
            Size? size = null,
            Point? location = null,
            string font = "Arial",
            int fontSize = 15,
            FontStyle fontStyle = FontStyle.Bold,
            FlatStyle flatStyle = FlatStyle.Flat,
            int borderSize = 0,
            ImageLayout imageLayout = ImageLayout.None,
            System.Drawing.Image? backgroundImage = null
            )
        {
            try
            {
                return new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = backColor,
                    ForeColor = foreColor,
                    Size = size ?? new Size(10, 10),
                    Location = location ?? new Point(0, 0),
                    FlatAppearance = { BorderSize = borderSize },
                    Font = new System.Drawing.Font(font, fontSize, fontStyle),
                    BackgroundImage = backgroundImage,
                    BackgroundImageLayout = imageLayout
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");

                return new Button
                {
                    Text = "Error",
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    Size = new Size(100, 50),
                    Location = new Point(0, 0),
                };
            }

        }


        /// <summary>
        /// Tworzenie Label
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <param name="location"></param>
        /// <param name="fontStyle"></param>
        /// <param name="autostart"></param>
        /// <returns></returns>
        public static Label CreateLabel(
            string title,
            string text,
            string font,
            int fontSize,
            Color foreColor,
            Color backColor,
            Point? location = null,
            FontStyle fontStyle = FontStyle.Regular,
            bool autostart = true

        )
        {
            return new Label
            {
                Text = text,
                Font = new System.Drawing.Font(font, fontSize),
                ForeColor = foreColor,
                BackColor = backColor,
                Location = location ?? new Point(10, 10),
                AutoSize = autostart
            };
        }

        /// <summary>
        /// Tworzenie Panel
        /// </summary>
        /// <param name="title"></param>
        /// <param name="size"></param>
        /// <param name="Location"></param>
        /// <param name="backColor"></param>
        /// <param name="borderStyle"></param>
        /// <returns></returns>
        public static Panel CreatePanel(
            string title,
            Size size,
            Point Location,
            Color backColor,
            BorderStyle borderStyle = BorderStyle.None
        )
        {
            return new Panel
            {
                Name = title,
                Size = size,
                Location = Location,
                BackColor = backColor,
                BorderStyle = borderStyle
            };
        }

        /// <summary>
        /// Tworzy ścieżkę zaokrąglonego prostokąta
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        //public static ComboBox CreateComboBox()
        //{
        //    ComboBox box = new ComboBox(); 
        //}

        //public static TrackBar CreateTrackBar()
        //{
        //    ComboBox box = new ComboBox(); 
        //}

        //public static PictureBox CreatePictureBox()
        //{
        //    PictureBox pic = new PictureBox();
        //}
    }
}
