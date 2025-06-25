using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// TODO:
// Dodać podświetlnie tego co wybraliśmy (lub takie przybliżenie) 
// Dodać żeby przesunięcie też było jakoś zasygnalizowane, co ze sobą zamieniliśmy 
// Dodać, że jak nie ma ruchu to mieszamy poziom

namespace SwipeArena
{
    public partial class Level : Form
    {

        List<IGameElement> elementTypes = new List<IGameElement>();
        IGameElement[,] grid;
        List<IGameElement> gameElements = new List<IGameElement>();
        PictureBox dragged;

        Bitmap? backgroundImage;

        int xSize = 128;
        int ySize = 128;

        int rows = 3;
        int cols = 3;

        public Level()
        {
            try
            {
                InitializeComponent();

                // Wczytanie ilustracji jako tła
                if (File.Exists("images/background/background.png"))
                {
                    backgroundImage = new Bitmap("images/background/background.png");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono obrazu");
                }

                // Ustawienia formularza
                Text = "Level";
                Size = new Size(800, 600);
                BackgroundImage = backgroundImage;

                GenerateLevel();
                CenterElements();

                // Obsługa zmiany rozmiaru okna
                Resize += Level_Resize;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            // Rejestracja obsługi zamknięcia okna
            FormUtils.RegisterFormClosingHandler(this);

        }

        /// <summary>
        /// Losowe generowanie planszy
        /// </summary>
        void GenerateLevel()
        {

            elementTypes = new List<IGameElement>
            {
                new GameElement("Helmet", Image.FromFile("images/level/helmet.png"), new Point(0, 0)),
                new GameElement("GoldShield", Image.FromFile("images/level/gold_shield.png"), new Point(0, 0)),
                new GameElement("BlueShield", Image.FromFile("images/level/blue_shield.png"), new Point(0, 0)),
                new GameElement("Axe", Image.FromFile("images/level/axe.png"), new Point(0, 0)),
                new GameElement("Sword", Image.FromFile("images/level/sword.png"), new Point(0, 0)),

            };

            grid = new IGameElement[rows, cols];

            Random random = new Random();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    IGameElement element = elementTypes[random.Next(elementTypes.Count)];
                    grid[y, x] = element;
                    gameElements.Add(element);

                    PictureBox pic = new PictureBox
                    {
                        Image = element.Icon,
                        Size = new Size(xSize, ySize),
                        Location = new Point(x * xSize, y * ySize),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Tag = new Point(x, y),
                        BackColor = Color.Transparent
                    };

                    // Tworzenie zaokrąglonego regionu
                    GraphicsPath roundedPath = CreateRoundedRectanglePath(new Rectangle(0, 0, pic.Width, pic.Height), 16);
                    pic.Region = new Region(roundedPath);

                    pic.MouseDown += Pic_MouseDown;
                    pic.AllowDrop = true;
                    pic.DragEnter += Pic_DragEnter;
                    pic.DragDrop += Pic_DragDrop;

                    pic.MouseEnter += Pic_MouseEnter;
                    pic.MouseLeave += Pic_MouseLeave;

                    Controls.Add(pic);

                }
            }

            // Sprawdzanie połączeń
            ProcessMatches();
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // Górny lewy róg
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Górny prawy róg
            path.AddArc(rect.X + rect.Width - diameter, rect.Y, diameter, diameter, 270, 90);
            // Dolny prawy róg
            path.AddArc(rect.X + rect.Width - diameter, rect.Y + rect.Height - diameter, diameter, diameter, 0, 90);
            // Dolny lewy róg
            path.AddArc(rect.X, rect.Y + rect.Height - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Obsługa zmiany rozmiaru okna
        /// </summary>
        void Level_Resize(object sender, EventArgs e)
        {
            CenterElements();
        }

        /// <summary>
        /// Wyśrodkowanie elementów na ekranie
        /// </summary>
        void CenterElements()
        {
            int elementSize = xSize;
            int gridWidth = cols * elementSize;
            int gridHeight = rows * elementSize;

            int startX = (ClientSize.Width - gridWidth) / 2;
            int startY = (ClientSize.Height - gridHeight) / 2;

            foreach (Control control in Controls)
            {
                if (control is PictureBox pic && pic.Tag is Point position)
                {
                    int x = position.X;
                    int y = position.Y;
                    pic.Location = new Point(startX + x * elementSize, startY + y * elementSize);
                }
            }
        }

        /// <summary>
        /// Zmiana stylu obramowania na hover
        /// </summary>
        void Pic_MouseEnter(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic != null)
            {
                pic.BorderStyle = BorderStyle.Fixed3D;
            }
        }

        /// <summary>
        /// Przywrócenie stylu obramowania po opuszczeniu kursora
        /// </summary>
        void Pic_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic != null)
            {
                pic.BorderStyle = BorderStyle.None;
            }
        }

        /// <summary>
        /// Obsługa przeciągania elementów
        /// </summary>
        void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            dragged = sender as PictureBox;
            if (dragged != null)
                dragged.DoDragDrop(dragged, DragDropEffects.Move);
        }

        void Pic_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Obsługa upuszczania elementów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pic_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox target = sender as PictureBox;
            if (dragged != null && target != null && dragged != target)
            {
                Point pos1 = (Point)dragged.Tag;
                Point pos2 = (Point)target.Tag;

                // Sprawdzenie, czy ruch jest o jedno pole
                int deltaX = Math.Abs(pos1.X - pos2.X);
                int deltaY = Math.Abs(pos1.Y - pos2.Y);

                // Ruch tylko w pionie lub poziomie
                if ((deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1))
                {
                    IGameElement temp = grid[pos1.Y, pos1.X];
                    grid[pos1.Y, pos1.X] = grid[pos2.Y, pos2.X];
                    grid[pos2.Y, pos2.X] = temp;

                    Image tempImg = dragged.Image;
                    dragged.Image = target.Image;
                    target.Image = tempImg;

                    // Sprawdzanie połączeń
                    ProcessMatches();
                }

            }
        }

        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        void ProcessMatches()
        {
            bool hasMatches;
            do
            {
                hasMatches = false;
                List<Point> matches = FindMatches();

                if (matches.Count > 0)
                {
                    RemoveMatches(matches);
                    hasMatches = true;
                }
            } while (hasMatches);
        }

        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        /// <returns></returns>
        List<Point> FindMatches()
        {
            List<Point> matches = new List<Point>();

            // Sprawdzanie poziomych połączeń
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols - 2; x++)
                {
                    string name1 = grid[y, x].Name;
                    string name2 = grid[y, x + 1].Name;
                    string name3 = grid[y, x + 2].Name;

                    if (name1 == name2 && name2 == name3)
                    {
                        matches.Add(new Point(x, y));
                        matches.Add(new Point(x + 1, y));
                        matches.Add(new Point(x + 2, y));
                    }
                }
            }

            // Sprawdzanie pionowych połączeń
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows - 2; y++)
                {
                    string name1 = grid[y, x].Name;
                    string name2 = grid[y + 1, x].Name;
                    string name3 = grid[y + 2, x].Name;

                    if (name1 == name2 && name2 == name3)
                    {
                        matches.Add(new Point(x, y));
                        matches.Add(new Point(x, y + 1));
                        matches.Add(new Point(x, y + 2));
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Usuwanie połączonych elementów
        /// </summary>
        /// <param name="matches"></param>
        void RemoveMatches(List<Point> matches)
        {
            Random random = new Random();

            foreach (Point match in matches)
            {
                int x = match.X;
                int y = match.Y;

                // Usuwanie elementu z siatki
                grid[y, x] = null;

                foreach (Control control in Controls)
                {
                    if (control is PictureBox pic && pic.Tag is Point position && position.X == x && position.Y == y)
                    {
                        Controls.Remove(pic);
                        pic.Dispose();
                        break;
                    }
                }
            }

            // Zastępowanie usuniętych elementów nowymi
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (grid[y, x] == null)
                    {
                        IGameElement newElement = elementTypes[random.Next(elementTypes.Count)];
                        grid[y, x] = newElement;

                        PictureBox pic = new PictureBox
                        {
                            Image = newElement.Icon,
                            Size = new Size(xSize, ySize),
                            Location = new Point((ClientSize.Width - cols * xSize) / 2 + x * xSize, (ClientSize.Height - rows * ySize) / 2 + y * ySize),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Tag = new Point(x, y),
                        };

                        pic.MouseDown += Pic_MouseDown;
                        pic.AllowDrop = true;
                        pic.DragEnter += Pic_DragEnter;
                        pic.DragDrop += Pic_DragDrop;

                        pic.MouseEnter += Pic_MouseEnter;
                        pic.MouseLeave += Pic_MouseLeave;

                        Controls.Add(pic);
                    }
                }
            }
        }

        /// <summary>
        /// Rysowanie tła
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Rysowanie tła na całym obszarze formularza
            if (backgroundImage != null)
            {
                // Skalowanie obrazu
                e.Graphics.DrawImage(backgroundImage, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            }
        }
    }
}
