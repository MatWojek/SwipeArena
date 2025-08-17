using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using SwipeArena.Config;
using SwipeArena.GameLogic;
using SwipeArena.Helpers;
using SwipeArena.Models;
using SwipeArena.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using SwipeArena.Animations;

namespace SwipeArena
{
    public partial class LevelForm : BaseForm
    {
        Stopwatch _gameStopwatch = new Stopwatch();

        AIHelper _ai = new AIHelper();
        List<IGameElement> _elementTypes = new();
        PictureBox firstClicked = null, dragged = null;

        int _elementSize;

        Label movesLabel, pointsLabel;
        Button settingsButton, hintButton;

        bool isDragging = false;
        Point mouseDownPos;

        public static int currentLevel;

        static GameBoard _board = new GameBoard();
        static MoveValidator _move;
        static GameRules _rules;

        IAnimation _matchAnimation = new MatchAnimation();
        IAnimation _swapAnimation = new SwapAnimation();

        public LevelForm(int level)
        {
            InitializeComponent();
            LoadBackgroundImage("images/background/background.png");

            // Obsługa AI dla poziomu
            //if (settings.IsAIEnabled)
            //{
            //    var aiHelper = new AIHelper();
            //    aiHelper.PlayAutomatically(board, () => settings.IsAIEnabled);
            //}

            currentLevel = level;

            _board.RandomBoardSize(currentLevel);
            _elementSize = _board.ElementSize();

            _move = new MoveValidator(board: _board);
            _rules = new GameRules(board: _board, validator: _move);

            SettingsHelper.ApplySettings(this, $"Level {currentLevel}");

            CreateUI();
            GenerateLevel();

            CenterElements();
            Resize += (s, e) => CenterElements();
        }

        /// <summary>
        /// Tworzenie interfejsu (liczniki, przycisku) 
        /// </summary>
        void CreateUI()
        {
            movesLabel = UIHelper.CreateLabel(
                title: "MovesLabel",
                text: $"Ruchy do końca: {_rules.GetMovesLeft()}",
                font: BasicSettings.FontFamily,
                fontSize: BasicSettings.FontSize,
                foreColor: Color.White,
                backColor: Color.FromArgb(66, 197, 230),
                fontStyle: FontStyle.Bold
                );

            pointsLabel = UIHelper.CreateLabel(
                title: "PointsLabel",
                text: $"Punkty: {_rules.GetPointsCollected()}/{_rules.GetPointsToWin()}",
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
            settingsButton.Click += (s, e) =>
            {
                var settingsForm = new SettingsForm();
                NavigateToForm(this, settingsForm);
            };

            Controls.AddRange(new Control[] { movesLabel, pointsLabel, settingsButton, hintButton, });

            var allControls = Controls.Cast<Control>().ToList();

            AdjustControlLayoutForSettingsSmall(allControls);

        }

        /// <summary>
        /// Zmiana stylu obramowania na hover
        /// </summary>
        public void Pic_MouseEnter(object sender, EventArgs e)
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
        public void Pic_MouseLeave(object sender, EventArgs e)
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
        public void Pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragged = sender as PictureBox;
                mouseDownPos = e.Location;
                isDragging = false;
            }
        }

        public void Pic_MouseMove(object sender, MouseEventArgs e)
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
        public void Pic_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Zamiana elementów w siatce
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        void HandleSwap(Point pos1, Point pos2, PictureBox box1, PictureBox box2)
        {
            // Jeśli trwa animacja, nie wykonujemy ruchu
            if (isAnimating) return;

            // Sprawdzenie, czy pola sąsiadują
            if (!_move.AreAdjacent(pos1, pos2)) return;

            isAnimating = true;

            var context = new AnimationContext
            {
                Pos1 = pos1,
                Pos2 = pos2,
                Box1 = box1,
                Box2 = box2
            };

            _swapAnimation.Animation(context, () =>
            {
                // Zamiana elementów w logice gry
                _move.Swap(pos1, pos2);

                // Zamiana tagów PictureBox
                SwapTags(box1, box2); 

                // Aktualizacja obrazków na planszy
                RefreshBoardUI();

                // Sprawdzenie, czy ruch tworzy match
                if (_move.FindMatches().Count > 0)
                {
                    // Jeśli ruch tworzy match, przetwarzamy dopasowania
                    ProcessMatches(decrementMoves: true);
                    isAnimating = false;
                }
                else
                {
                    // Jeśli ruch nie tworzy matcha, cofamy zamianę z animacją
                    var reverseContext = new AnimationContext
                    {
                        Pos1 = pos2,
                        Pos2 = pos1,
                        Box1 = box1,
                        Box2 = box2
                    };

                    _swapAnimation.Animation(reverseContext, () =>
                    {
                        _move.Swap(pos1, pos2); 

                        // Cofnięcie tagów PictureBox
                        SwapTags(box1, box2);

                        RefreshBoardUI(); 

                        isAnimating = false;
                    });
                }
            });
        }


        void SwapTags(PictureBox box1, PictureBox box2)
        {
            var temp = box1.Tag;
            box1.Tag = box2.Tag;
            box2.Tag = temp;
        }

        void RefreshBoardUI()
        {
            foreach (PictureBox pic in Controls.OfType<PictureBox>())
                if (pic.Tag is Point pos)
                    pic.Image = _board.GetElement(pos.X, pos.Y)?.Icon;
        }

        /// <summary>
        /// Obsługa upuszczania elementów
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Pic_DragDrop(object sender, DragEventArgs e)
        {
            if (dragged == null || dragged.Tag is not Point draggedPos || sender is not PictureBox target || target.Tag is not Point targetPos)
                return;

            HandleSwap(draggedPos, targetPos, dragged, target);

            // Resetowanie stanu przeciągania
            dragged = null;
            isDragging = false;
        }

        void HintButton_Click(object sender, EventArgs e)
        {
            var bestMove = _ai.SuggestBestMove(_board.GetGrid());
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
            _elementTypes = new()
            {
                new GameElement("Helmet", Image.FromFile("images/level/helmet.png"), Point.Empty),
                new GameElement("GoldShield", Image.FromFile("images/level/gold_shield.png"), Point.Empty),
                new GameElement("BlueShield", Image.FromFile("images/level/blue_shield.png"), Point.Empty),
                new GameElement("Axe", Image.FromFile("images/level/axe.png"), Point.Empty),
                new GameElement("Sword", Image.FromFile("images/level/sword.png"), Point.Empty),
            };

            _board.GenerateLevel(_elementTypes);

            Controls.OfType<PictureBox>().ToList().ForEach(p => { Controls.Remove(p); p.Dispose(); });

            for (int y = 0; y < _board.GetRows(); y++)
            {
                for (int x = 0; x < _board.GetCols(); x++)
                {
                    var element = _board.GetElement(x, y);
                    var pic = CreatePictureBoxForElement(element, x, y);
                    Controls.Add(pic);
                }
            }


            if (!_move.HasValidMove())
            {
                _board.ShuffleBoard(findMatchesFunc: _move.FindMatches, hasValidMoveFunc: _move.HasValidMove);

                // odśwież GUI
                RefreshBoardUI();
            }

            ProcessMatches();
        }

        /// <summary>
        /// Obsługa kliknięcia (pierwszy/drugi klik)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Pic_Click(object sender, EventArgs e)
        {
            if (sender is not PictureBox clicked || clicked.Tag is not Point pos2) return;

            if (isAnimating) return;


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

            HandleSwap(pos1, pos2, firstClicked, clicked);
            ResetClick();
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
        /// Wyśrodkowuje elementy na planszy
        /// </summary>
        void CenterElements()
        {
            int _gridW = _board.Cols * _elementSize, _gridH = _board.GetRows() * _elementSize;
            int startX = (ClientSize.Width - _gridW) / 2;

            // Rzutujemy Controls na IEnumerable<Control>
            int topUIBottom = Controls
                .Cast<Control>()
                .Where(c => !(c is PictureBox))
                .DefaultIfEmpty()
                .Max(c => c?.Bottom ?? 0);

            int startY = topUIBottom + 40;

            foreach (PictureBox pic in Controls.OfType<PictureBox>())
                if (pic.Tag is Point pos)
                    pic.Location = new Point(startX + pos.X * _elementSize, startY + pos.Y * _elementSize);
        }

        /// <summary>
        /// Sprawdzanie czy istnieją połączenia
        /// </summary>
        void ProcessMatches(bool decrementMoves = false)
        {
            bool lastMoveCreatedMatch = false;
            int safetyCounter = 0;

            do
            {
                List<Point> matches = _move.FindMatches();

                if (matches.Count > 0)
                {
                    lastMoveCreatedMatch = true;

                    _rules.AddPoints(matches.Count);
                    pointsLabel.Text = $"Punkty: {_rules.GetPointsCollected()}/{_rules.GetPointsToWin()}";

                    if (CheckGameOver())
                    {
                        RemoveMatches(matches);
                    }

                }

                if (!_move.HasValidMove())
                {
                    _board.ShuffleBoard(findMatchesFunc: _move.FindMatches, hasValidMoveFunc: _move.HasValidMove);

                    // odśwież GUI
                    RefreshBoardUI();
                }

                else break;

                safetyCounter++;

            } while (safetyCounter < 10);

            if (decrementMoves && lastMoveCreatedMatch)
            {
                _rules.DecrementMoves();
                movesLabel.Text = $"Ruchy do końca: {_rules.GetMovesLeft()}";
            }

            if (!_move.HasValidMove())
            {
                _board.ShuffleBoard(findMatchesFunc: _move.FindMatches, hasValidMoveFunc: _move.HasValidMove);

                // odśwież GUI
               RefreshBoardUI();
            }

            _rules.SaveData();

        }

        /// <summary>
        /// Sprawdzenie końca gry
        /// </summary>
        bool CheckGameOver()
        {
            int result = _rules.CheckGameOver();
            if (result == 1)
            {
                var levelComplete = new LevelCompleteForm();
                NavigateToForm(this, levelComplete);

                return false; 
            }
            else if (result == 2)
            {
                var gameOver = new GameOverForm();
                NavigateToForm(this, gameOver);

                return false; 
            }

            return true; 
        }

        /// <summary>
        /// Usuwanie połączonych elementów
        /// </summary>
        /// <param name="matches"></param>
        bool isAnimating = false;

        void RemoveMatches(List<Point> matches)
        {
            if (matches.Count == 0) return;

            isAnimating = true;

            var context = new AnimationContext
            {
                ParentControl = this,
                Matches = matches
            };

            _matchAnimation.Animation(context, () =>
            {
                RemoveMatchedElements(matches);
                ReplaceRemovedElements();
                CenterElements();
                isAnimating = false;

                // dopiero teraz sprawdzamy kolejne dopasowania
                List<Point> newMatches = _move.FindMatches();
                if (newMatches.Count > 0)
                    RemoveMatches(newMatches);  
            });
        }

        /// <summary>
        /// / Usuwa wskazane elementy z planszy logicznie (Board) oraz graficznie (PictureBoxy).
        /// </summary>
        void RemoveMatchedElements(List<Point> matches)
        {
            foreach (Point match in matches)
            {
                int x = match.X;
                int y = match.Y;

                // Usuń element z logiki gry
                _board.SetElement(x, y, null);

                // Usuń odpowiadający PictureBox
                var pic = Controls.OfType<PictureBox>()
                                  .FirstOrDefault(p => p.Tag is Point pos && pos.X == x && pos.Y == y);

                if (pic != null)
                {
                    Controls.Remove(pic);
                    pic.Dispose();
                }
            }
        }


        /// <summary>
        /// Tworzy nowe losowe elementy dla pustych miejsc na planszy i dodaje odpowiadające PictureBoxy do interfejsu.
        /// </summary>
        void ReplaceRemovedElements()
        {
            Random random = new Random();

            for (int y = 0; y < _board.GetRows(); y++)
            {
                for (int x = 0; x < _board.GetCols(); x++)
                {
                    if (_board.GetElement(x, y) == null)
                    {
                        IGameElement newElement = _elementTypes[random.Next(_elementTypes.Count)].Clone();
                        _board.SetElement(x, y, newElement); ;

                        var pic = CreatePictureBoxForElement(newElement, x, y);
                        pic.Tag = new Point(x, y);
                        pic.Region = new Region(UIHelper.CreateRoundedRectanglePath(
                                     new Rectangle(0, 0, _elementSize, _elementSize), 16));
                        Controls.Add(pic);
                    }
                }
            }
        }

        /// <summary>
        ///  Tworzy nowy PictureBox dla podanego elementu gry, ustawia jego pozycję, rozmiar, obrazek, tag i eventy.
        /// </summary>
        PictureBox CreatePictureBoxForElement(IGameElement element, int x, int y)
        {
            return new PictureBox
            {
                Image = element.Icon,
                Size = new Size(_elementSize, _elementSize),
                Location = new Point(
                    (ClientSize.Width - _board.GetCols() * _elementSize) / 2 + x * _elementSize,
                    (ClientSize.Height - _board.GetRows() * _elementSize) / 2 + y * _elementSize
                ),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Tag = new Point(x, y),
                BackColor = Color.Transparent,
                AllowDrop = true
            }.WithEventHandlers(this);
        }
    }
}