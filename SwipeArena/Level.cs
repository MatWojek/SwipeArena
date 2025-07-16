using System.Drawing.Drawing2D;
using System.Windows.Forms;

// TODO:
// Dodać podświetlnie tego co wybraliśmy (lub takie przybliżenie) (zrobione)  
// Dodać żeby przesunięcie też było jakoś zasygnalizowane, co ze sobą zamieniliśmy 
// Dodać, że jak nie ma ruchu to mieszamy poziom (do naprawienia)
// Licznik ruchów i licznik połączeń które należy zdobyć zależy od poziomu
// Dodać przycisk do ustawień i dodać nowe ustawienia gry, 
// Przycisk cofania 
// Przycisk powrotu do menu 
// Progres jest zapisywany, i wyświetlany na ekranie, 
// Progres to ile poziomów udało się zdobyć
// Przy przyciśnięciu lub zmianie, zmienia się kolor na czerwony, żeby w 100% informować że te elementy są zmieniane lub wybrane


namespace SwipeArena
{
    public partial class Level : Form
    {

        List<IGameElement> elementTypes = new List<IGameElement>();
        IGameElement[,] grid;
        List<IGameElement> gameElements = new List<IGameElement>();
        PictureBox dragged;

        int xSize = 0;
        int ySize = 0;

        int rows = 0;
        int cols = 0;

        int movesLeft;
        int pointsCollected;
        int pointsToWin;
        Label movesLabel;
        Label pointsLabel;

        PictureBox firstClicked = null;

        SettingsData settings = new SettingsData();

        DateTime lastMouseDownTime;

        Point mouseDownPos;
        bool isDragging = false;

        Button settingsButton;

        public Level(int level, int rows, int cols)
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
                    return Image.FromFile("images/background/background.png");
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

                if (rows == 0 || cols == 0)
                {
                    MessageBox.Show("Nie można wczytać rozmiaru planszy;");
                }

                this.rows = rows;
                this.cols = cols;

                if (rows > 0 && rows < 4)
                {
                    xSize = 128;
                    ySize = 128;
                }
                else
                {
                    xSize = 64;
                    ySize = 64;
                }

                // Ustawienia formularza
                Text = $"Level {level}";
                Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);

                // Ustawienia liczników w zależności od poziomu
                pointsToWin = level * 10;
                movesLeft = Math.Max(30 - level, 3);
                pointsCollected = 0;

                // Dodanie etykiet do wyświetlania liczników
                movesLabel = new Label
                {
                    Text = $"Ruchy do końca: {movesLeft}",
                    Font = new Font("Arial", 20),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                pointsLabel = new Label
                {
                    Text = $"Punkty: {pointsCollected}/{pointsToWin}",
                    Font = new Font("Arial", 20),
                    ForeColor = Color.Red,
                    BackColor = Color.Transparent,
                    Location = new Point(10, 50),
                    AutoSize = true
                };

                // Dodanie guzika do Ustawień 
                settingsButton = new Button
                {
                    Text = "Ustawienia",
                    Size = new Size(100, 30),
                    Location = new Point(10, 100)
                };
                settingsButton.Click += SettingsButton_Click;

                Controls.Add(settingsButton);
                Controls.Add(movesLabel);
                Controls.Add(pointsLabel);

                movesLabel.BringToFront();
                pointsLabel.BringToFront();

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
            gameElements.Clear();
            foreach (Control ctrl in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(ctrl);
                ctrl.Dispose();
            }

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
                    pic.Click += Pic_Click;
                    pic.MouseDown += Pic_MouseDown;
                    pic.MouseMove += Pic_MouseMove;

                    Controls.Add(pic);

                    movesLabel.BringToFront();
                    pointsLabel.BringToFront();

                }
            }

            // Sprawdzanie połączeń
            ProcessMatches();

            if (!HasValidMove())
            {
                ShuffleBoard();
            }

            // Sprawdzanie połączeń
            ProcessMatches();
        }

        /// <summary>
        /// Kliknięcie guzika przechodzącego do Ustawień
        /// </summary>
        void SettingsButton_Click(object sender, EventArgs e)
        {
            using (Settings settingsForm = new Settings())
            {
                settingsForm.ShowDialog();
                
                // Zmiana ustawień w ustawieniach wpływa na ustawienia Level
                ApplySettings();
            }
        }

        /// <summary>
        /// Aktualizacja zmiany w ustawieniach
        /// </summary>
        void ApplySettings()
        {
            
            Size = new Size(SettingsData.Instance.Resolution.X, SettingsData.Instance.Resolution.Y);

            CenterElements();
        }

        /// <summary>
        /// Animacja po połączeniu 3 elementów 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="onAnimationComplete"></param>
        void AnimateMatches(List<Point> matches, Action onAnimationComplete)
        {
            // Przykład animacji: miganie elementów
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            int blinkCount = 0;

            timer.Interval = 100;
            timer.Tick += (s, e) =>
            {
                foreach (Point match in matches)
                {
                    PictureBox pic = Controls.OfType<PictureBox>()
                        .FirstOrDefault(p => p.Tag is Point position && position == match);
                    if (pic != null)
                    {
                        pic.Visible = !pic.Visible;
                    }
                }

                blinkCount++;
                if (blinkCount >= 6)
                {
                    timer.Stop();
                    foreach (Point match in matches)
                    {
                        PictureBox pic = Controls.OfType<PictureBox>()
                            .FirstOrDefault(p => p.Tag is Point position && position == match);
                        if (pic != null)
                        {
                            pic.Visible = true;
                        }
                    }

                    onAnimationComplete?.Invoke();
                }
            };

            timer.Start();
        }


        /// <summary>
        /// Kliknięcie jako przesuwanie elementów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pic_Click(object sender, EventArgs e)
        {
            if (sender is not PictureBox clicked || clicked.Tag is not Point pos2)
                return;

            if (firstClicked == null)
            {
                // Pierwszy klik
                firstClicked = clicked;
                firstClicked.BackColor = Color.FromArgb(128, Color.Cyan);
            }
            else
            {
                // Drugi klik
                if (firstClicked.Tag is not Point pos1)
                {
                    firstClicked.BackColor = Color.Transparent;
                    firstClicked = null;
                    return;
                }

                // Sprawdzenie odległości – czy sąsiadujące
                int deltaX = Math.Abs(pos1.X - pos2.X);
                int deltaY = Math.Abs(pos1.Y - pos2.Y);

                if ((deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1))
                {
                    // Zamiana elementów w gridzie
                    IGameElement temp = grid[pos1.Y, pos1.X];
                    grid[pos1.Y, pos1.X] = grid[pos2.Y, pos2.X];
                    grid[pos2.Y, pos2.X] = temp;

                    // Sprawdzenie, czy ruch tworzy match
                    if (FindMatches().Count > 0)
                    {
                        // Zamiana obrazków
                        Image tempImg = firstClicked.Image;
                        firstClicked.Image = clicked.Image;
                        clicked.Image = tempImg;

                        // Wywołanie logiki dropa
                        ProcessMatches();
                    }
                    else
                    {
                        // Przywrócenie oryginalnego stanu, jeśli nie ma matcha
                        grid[pos2.Y, pos2.X] = grid[pos1.Y, pos1.X];
                        grid[pos1.Y, pos1.X] = temp;
                    }
                }

                // Resetowanie stanu
                firstClicked.BackColor = Color.Transparent;
                firstClicked = null;
            }
        }

        /// <summary>
        /// Sprawdzenie czy przesunięcie elementu o jedno pole skutkuje matchem
        /// </summary>
        /// <returns></returns>
        bool HasValidMove()
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    // Sprawdzenie ruchu w prawo
                    if (x < cols - 1 && CanFormMatch(x, y, x + 1, y))
                        return true;

                    // Sprawdzenie ruchu w dół
                    if (y < rows - 1 && CanFormMatch(x, y, x, y + 1))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Czy jest połączenie na planszy
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        bool CanFormMatch(int x1, int y1, int x2, int y2)
        {
            // Zamiana elementów
            IGameElement temp = grid[y1, x1];
            grid[y1, x1] = grid[y2, x2];
            grid[y2, x2] = temp;

            // Sprawdzenie, czy powstało połączenie
            bool hasMatch = FindMatches().Count > 0;

            // Przywrócenie oryginalnego stanu
            grid[y2, x2] = grid[y1, x1];
            grid[y1, x1] = temp;

            return hasMatch;
        }


        /// <summary>
        /// Tasowanie planszy
        /// </summary>
        void ShuffleBoard()
        {
            Random random = new Random();
            List<IGameElement> elements = new List<IGameElement>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    elements.Add(grid[y, x]);
                }
            }

            int maxAttempts = 1000;
            int attempts = 0;

            do
            {
                elements = elements.OrderBy(e => random.Next()).ToList();

                int index = 0;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        grid[y, x] = elements[index++];
                    }
                }

                attempts++;

            } while ((!HasValidMove() || FindMatches().Count > 0) && attempts < maxAttempts);

            foreach (Control control in Controls)
            {
                if (control is PictureBox pic && pic.Tag is Point position)
                {
                    int x = position.X;
                    int y = position.Y;
                    pic.Image = grid[y, x].Icon;
                }
            }

            CenterElements();
        }


        /// <summary>
        /// Zaokrąglenie elementów
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
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

        void Pic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Pic_Click(sender, EventArgs.Empty);
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
                pic.BackColor = Color.FromArgb(128, Color.Cyan);
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
                pic.BackColor = Color.Transparent;
            }
        }

        /// <summary>
        /// Obsługa przeciągania elementów
        /// </summary>
        void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragged = sender as PictureBox;
                mouseDownPos = e.Location;
                isDragging = false;
            }
        }


        void Pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dragged != null)
            {
                int dx = Math.Abs(e.X - mouseDownPos.X);
                int dy = Math.Abs(e.Y - mouseDownPos.Y);

                if ((dx >= 5 || dy >= 5) && !isDragging)
                {
                    isDragging = true;
                    dragged.BackColor = Color.White;
                    dragged.DoDragDrop(dragged, DragDropEffects.Move);
                }
            }
        }


        /// <summary>
        /// Obsługa podnoszenia elementu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            if (dragged == null || dragged.Tag is not Point draggedPos || sender is not PictureBox target || target.Tag is not Point targetPos)
                return;

            // Sprawdzenie, czy ruch jest sąsiadujący
            int deltaX = Math.Abs(draggedPos.X - targetPos.X);
            int deltaY = Math.Abs(draggedPos.Y - targetPos.Y);

            if ((deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1))
            {
                // Zamiana elementów w gridzie
                IGameElement temp = grid[draggedPos.Y, draggedPos.X];
                grid[draggedPos.Y, draggedPos.X] = grid[targetPos.Y, targetPos.X];
                grid[targetPos.Y, targetPos.X] = temp;

                // Sprawdzenie, czy ruch tworzy match
                if (FindMatches().Count > 0)
                {
                    // Jeśli ruch tworzy match, aktualizuj obrazki
                    Image tempImg = dragged.Image;
                    dragged.Image = target.Image;
                    target.Image = tempImg;

                    // Wywołanie logiki przetwarzania dopasowań
                    ProcessMatches();
                }

                else
                {
                    // Jeśli ruch nie tworzy matcha, przywróć oryginalny stan
                    grid[targetPos.Y, targetPos.X] = grid[draggedPos.Y, draggedPos.X];
                    grid[draggedPos.Y, draggedPos.X] = temp;

                    // Przywrócenie obrazków
                    dragged.Image = grid[draggedPos.Y, draggedPos.X].Icon;
                    target.Image = grid[targetPos.Y, targetPos.X].Icon;
                }
            }

            // Resetowanie stanu przeciągania
            dragged = null;
            isDragging = false;
        }


        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        void ProcessMatches()
        {
            bool lastMoveCreatedMatch = false;

            do
            {
                List<Point> matches = FindMatches();

                if (matches.Count > 0)
                {
                    lastMoveCreatedMatch = true;

                    pointsCollected += matches.Count;
                    pointsLabel.Text = $"Punkty: {pointsCollected}/{pointsToWin}";

                    RemoveMatches(matches);
                }

                if (!HasValidMove())
                {
                    ShuffleBoard();
                }
                else
                {
                    break;
                }

            } while (true);

            // Zmniejszenie liczby ruchów tylko, jeśli był match
            if (lastMoveCreatedMatch)
            {
                movesLeft--;
                movesLabel.Text = $"Ruchy do końca: {movesLeft}";
            }

            // Sprawdzenie warunków końca gry
            CheckGameOver();
        }


        /// <summary>
        /// Sprawdzenie zwycięstwa
        /// </summary>
        void CheckGameOver()
        {
            // Zwycięstwo w poziomie
            if (pointsCollected >= pointsToWin)
            { 
                // Przejście do formularza LevelComplete
                var levelComplete = new LevelComplete();
                levelComplete.Show();

                // Zamknięcie bieżącego formularza
                Hide();
            }

            // Przegranie w poziomie 
            else if (movesLeft <= 0)
            {
                // Przejście do formularza GameOver 
                var gameOver = new GameOver();
                gameOver.Show();

                // Zamknięcie bieżącego formularza
                Hide();
            }
        }

        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        /// <returns></returns>
        List<Point> FindMatches()
        {
            HashSet<Point> matches = new HashSet<Point>();

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

            return matches.ToList();
        }

        /// <summary>
        /// Usuwanie połączonych elementów
        /// </summary>
        /// <param name="matches"></param>
        void RemoveMatches(List<Point> matches)
        {
            AnimateMatches(matches, () =>
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
                                BackColor = Color.Transparent,
                            };

                            pic.MouseDown += Pic_MouseDown;
                            pic.AllowDrop = true;
                            pic.DragEnter += Pic_DragEnter;
                            pic.DragDrop += Pic_DragDrop;

                            pic.MouseEnter += Pic_MouseEnter;
                            pic.MouseLeave += Pic_MouseLeave;
                            pic.MouseMove += Pic_MouseMove;
                            pic.Click += Pic_Click;

                            Controls.Add(pic);
                        }
                    }
                }
            });
        }
    }
}