using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
    public partial class LevelForm : BaseForm
    {
        Stopwatch gameStopwatch = new Stopwatch();

        AIHelper ai = new AIHelper();
        List<IGameElement> elementTypes = new();
        IGameElement[,] grid;
        PictureBox firstClicked = null;
        PictureBox dragged = null;

        int rows, cols, xSize, ySize;
        int movesLeft, pointsCollected, pointsToWin;

        Label movesLabel, pointsLabel;
        Button settingsButton, hintButton;

        bool isDragging = false;
        Point mouseDownPos;

        public static int currentLevel;

        public LevelForm(int level)
        {
            InitializeComponent();
            LoadBackgroundImage("images/background/background.png");

            currentLevel = level;
            RandomBoardSize(level);
            xSize = rows < 8 ? 128 : 96;
            ySize = xSize;

            SettingsHelper.ApplySettings(this, $"Level {level}");

            pointsToWin = level * 10;
            movesLeft = Math.Max(30 - level, 3);

            CreateUI();
            GenerateLevel();

            CenterElements();
            Resize += (s, e) => CenterElements();
        }

        /// <summary>
        /// Określenie wielkości planszy na podstawie poziomu
        /// </summary>
        /// <param name="levelNumber"></param>
        void RandomBoardSize(int levelNumber)
        {
            Random random = new Random();

            if (levelNumber >= 6)
            {
                rows = random.Next(4, 8);
                cols = random.Next(4, 8);
            }
            else
            {
                rows = random.Next(3, 3 + levelNumber);
                cols = random.Next(3, 3 + levelNumber);
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
                    ProcessMatches(decrementMoves: true);
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
        /// Tworzenie interfejsu (liczniki, przycisku) 
        /// </summary>
        void CreateUI()
        {
            movesLabel = UIHelper.CreateLabel(
                title: "MovesLabel",
                text: $"Ruchy do końca: {movesLeft}",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.White,
                backColor: Color.FromArgb(66, 197, 230),
                fontStyle: FontStyle.Bold
                ); 

            pointsLabel = UIHelper.CreateLabel(
                title: "PointsLabel",
                text: $"Punkty: {pointsCollected}/{pointsToWin}", 
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.White, 
                backColor: Color.FromArgb(66, 197, 230),
                fontStyle: FontStyle.Bold  
                );

            hintButton = UIHelper.CreateButton(
                title: "HintButton",
                text: "Podpowiedź",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(170, 50),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
            );
            hintButton.Click += HintButton_Click;

            settingsButton = UIHelper.CreateButton(
                title: "Settings",
                text: "Ustawienia",
                backColor: Color.FromArgb(67, 203, 107),
                foreColor: Color.White,
                size: new Size(170, 50),
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                fontStyle: FontStyle.Bold
                );
            settingsButton.Click += (s, e) => { new SettingsForm().ShowDialog(); SettingsHelper.ApplySettings(this, "Ustawienia"); };
            Controls.AddRange(new Control[] { movesLabel, pointsLabel, settingsButton, hintButton, });

            var allControls = Controls.Cast<Control>().ToList();

            AdjustControlLayoutForSettingsSmall(allControls);

        }

        void HintButton_Click(object sender, EventArgs e)
        {
            var bestMove = ai.SuggestBestMove(grid);
            if (bestMove.startX != -1)
            {
                HighlightHint(bestMove);
            }
        }

        void HighlightHint((int startX, int startY, int endX, int endY) move)
        {
            foreach (PictureBox pic in Controls.OfType<PictureBox>())
            {
                if (pic.Tag is Point pos &&
                    (pos == new Point(move.startX, move.startY) || pos == new Point(move.endX, move.endY)))
                {
                    pic.BackColor = Color.Yellow;
                }
            }

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                foreach (PictureBox pic in Controls.OfType<PictureBox>())
                {
                    if (pic.BackColor == Color.Yellow)
                        pic.BackColor = Color.Transparent;
                }
                timer.Stop();
            };
            timer.Start();
        }

        /// <summary>
        /// Generowanie losowo elementów gry
        /// </summary>
        void GenerateLevel()
        {
            elementTypes = new()
            {
                new GameElement("Helmet", Image.FromFile("images/level/helmet.png"), Point.Empty),
                new GameElement("GoldShield", Image.FromFile("images/level/gold_shield.png"), Point.Empty),
                new GameElement("BlueShield", Image.FromFile("images/level/blue_shield.png"), Point.Empty),
                new GameElement("Axe", Image.FromFile("images/level/axe.png"), Point.Empty),
                new GameElement("Sword", Image.FromFile("images/level/sword.png"), Point.Empty),
            };

            grid = new IGameElement[rows, cols];
            Controls.OfType<PictureBox>().ToList().ForEach(p => { Controls.Remove(p); p.Dispose(); });

            Random random = new();
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    AddElement(random.Next(elementTypes.Count), x, y);

            if (!HasValidMove()) ShuffleBoard();
            ProcessMatches();
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
        /// Dodawanie pojedynczego elementu do planszy i interfejsu
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void AddElement(int typeIndex, int x, int y)
        {
            var element = elementTypes[typeIndex];
            grid[y, x] = element;

            var pic = new PictureBox
            {
                Image = element.Icon,
                Size = new Size(xSize, ySize),
                Tag = new Point(x, y),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Region = new Region(CreateRoundedRectanglePath(new Rectangle(0, 0, xSize, ySize), 16))
            };

            pic.MouseDown += Pic_MouseDown;
            pic.MouseMove += Pic_MouseMove;
            pic.MouseEnter += (s, e) => { pic.BorderStyle = BorderStyle.Fixed3D; pic.BackColor = Color.FromArgb(128, Color.Cyan); };
            pic.MouseLeave += (s, e) => { pic.BorderStyle = BorderStyle.None; pic.BackColor = Color.Transparent; };
            pic.Click += Pic_Click;
            pic.AllowDrop = true;
            pic.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
            pic.DragDrop += Pic_DragDrop;

            Controls.Add(pic);
        }

        /// <summary>
        /// Obsługa kliknięcia (pierwszy/drugi klik)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pic_Click(object sender, EventArgs e)
        {
            if (sender is not PictureBox clicked || clicked.Tag is not Point pos2) return;

            if (firstClicked == null)
            {
                firstClicked = clicked;
                firstClicked.BackColor = Color.FromArgb(128, Color.Cyan);
                return;
            }

            if (firstClicked.Tag is not Point pos1)
            {
                ResetClick();
                return;
            }

            if (AreAdjacent(pos1, pos2))
            {
                Swap(pos1, pos2);
                if (FindMatches().Count > 0)
                {
                    SwapImages(firstClicked, clicked);
                    ProcessMatches(decrementMoves: true);
                }
                else Swap(pos1, pos2);
            }

            ResetClick();
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
        /// Resetuje zaznaczenie kliknięcia
        /// </summary>
        void ResetClick()
        {
            if (firstClicked != null) firstClicked.BackColor = Color.Transparent;
            firstClicked = null;
        }

        /// <summary>
        ///  Czy pola sąsiadują ze sobą
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        bool AreAdjacent(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) == 1;

        /// <summary>
        /// Zamienia elementy w siatce
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void Swap(Point a, Point b)
        {
            (grid[a.Y, a.X], grid[b.Y, b.X]) = (grid[b.Y, b.X], grid[a.Y, a.X]);
        }


        /// <summary>
        /// Zamienia obrazki PictureBoxów
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void SwapImages(PictureBox a, PictureBox b)
        {
            (a.Image, b.Image) = (b.Image, a.Image);
        }

        /// <summary>
        /// Wyśrodkowuje elementy na planszy
        /// </summary>
        void CenterElements()
        {
            int gridW = cols * xSize, gridH = rows * ySize;
            int startX = (ClientSize.Width - gridW) / 2;

            // Rzutujemy Controls na IEnumerable<Control>
            int topUIBottom = Controls
                .Cast<Control>()
                .Where(c => !(c is PictureBox))
                .DefaultIfEmpty()
                .Max(c => c?.Bottom ?? 0);

            int startY = topUIBottom + 40; // Przesunięcie planszy poniżej UI

            foreach (PictureBox pic in Controls.OfType<PictureBox>())
                if (pic.Tag is Point pos)
                    pic.Location = new Point(startX + pos.X * xSize, startY + pos.Y * ySize);
        }

        /// <summary>
        /// Tworzy ścieżkę zaokrąglonego prostokąta
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
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

        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        void ProcessMatches(bool decrementMoves = false)
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

                    if (CheckGameOver())
                    {
                        RemoveMatches(matches);
                    }
                    
                }

                if (!HasValidMove())
                {
                    ShuffleBoard();
                }
                else break;

            } while (true);

            if (decrementMoves && lastMoveCreatedMatch)
            {
                movesLeft--;
                movesLabel.Text = $"Ruchy do końca: {movesLeft}";
            }

            if (!HasValidMove())
            {
                ShuffleBoard();
            }


            SaveData();

        }

        /// <summary>
        /// Zapis wyników punktowych
        /// </summary>
        void SaveData()
        {
            SaveLoad save = new SaveLoad();

            save.Load();

            save.BestWinStreak = Math.Max(save.BestWinStreak, save.CurrentWinStreak);
            save.MaxPoints = Math.Max(save.MaxPoints, pointsCollected);
            save.LastLevelPlayed = currentLevel;
            save.TotalPoints += pointsCollected;

            save.Save();
        }

        /// <summary>
        /// Zapis po wygranej lub przegranej
        /// </summary>
        void SaveAfterWin()
        {
            SaveLoad save = new SaveLoad();

            save.Load();

            if (movesLeft >= 0 && pointsCollected >= pointsToWin)
            {
                save.CurrentWinStreak++;

                if (currentLevel > save.LevelCompleted)
                {
                    save.LevelCompleted = currentLevel;
                }

            }
            else if (movesLeft <= 0 && pointsCollected <= pointsToWin)
            {
                save.CurrentWinStreak = 0;
            }

            save.BestWinStreak = Math.Max(save.BestWinStreak, save.CurrentWinStreak);

            if (gameStopwatch != null)
            {
                save.TimeGame += gameStopwatch.Elapsed.TotalSeconds;
                gameStopwatch.Reset();
            }

            save.Save();
        }


        /// <summary>
        /// Sprawdzenie zwycięstwa
        /// </summary>
        bool CheckGameOver()
        {
            // Zwycięstwo w poziomie
            if (movesLeft >= 0 && pointsCollected >= pointsToWin)
            {
                SaveAfterWin();

                // Przejście do formularza LevelComplete
                var levelComplete = new LevelCompleteForm();
                NavigateToForm(this, levelComplete);

                return false;
            }

            // Przegranie w poziomie 
            else if (movesLeft <= 0 && pointsCollected <= pointsToWin)
            {
                SaveAfterWin();

                // Przejście do formularza GameOver 
                var gameOver = new GameOverForm();
                NavigateToForm(this, gameOver);

                return false;
            }

            return true; 
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
                CenterElements();

                ProcessMatches();
            });
        }
    }
}