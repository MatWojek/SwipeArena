//using SwipeArena.Models;
//using SwipeArena.UI;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Windows.Forms;

//namespace SwipeArena.GameLogic
//{
//    internal class GameBoard
//    {
//        private readonly Control.ControlCollection _controls;
//        private readonly Label _pointsLabel;
//        private readonly Label _movesLabel;
//        private readonly int _xSize;
//        private readonly int _ySize;
//        private readonly int _cols;
//        private readonly int _rows;
//        private readonly int _pointsToWin;

//        private int _pointsCollected;
//        private int _movesLeft;

//        private IGameElement[,] _grid;
//        private List<IGameElement> _elementTypes;

//        public GameBoard(Control.ControlCollection controls, Label pointsLabel, Label movesLabel,
//                         int cols, int rows, int xSize, int ySize, int pointsToWin, int initialMoves)
//        {
//            _controls = controls;
//            _pointsLabel = pointsLabel;
//            _movesLabel = movesLabel;
//            _cols = cols;
//            _rows = rows;
//            _xSize = xSize;
//            _ySize = ySize;
//            _pointsToWin = pointsToWin;
//            _movesLeft = initialMoves;

//            GenerateLevel();
//        }

//        private void AddElement(int typeIndex, int x, int y)
//        {
//            var element = _elementTypes[typeIndex];
//            _grid[y, x] = element;

//            var pic = new PictureBox
//            {
//                Image = element.Icon,
//                Size = new Size(_xSize, _ySize),
//                Tag = new Point(x, y),
//                SizeMode = PictureBoxSizeMode.StretchImage,
//                BackColor = Color.Transparent,
//                Region = new Region(UIHelper.CreateRoundedRectanglePath(new Rectangle(0, 0, _xSize, _ySize), 16))
//            };

//            pic.MouseDown += Pic_MouseDown;
//            pic.MouseMove += Pic_MouseMove;
//            pic.MouseEnter += (s, e) => { pic.BorderStyle = BorderStyle.Fixed3D; pic.BackColor = Color.FromArgb(128, Color.Cyan); };
//            pic.MouseLeave += (s, e) => { pic.BorderStyle = BorderStyle.None; pic.BackColor = Color.Transparent; };
//            pic.Click += Pic_Click;
//            pic.AllowDrop = true;
//            pic.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
//            pic.DragDrop += Pic_DragDrop;

//            _controls.Add(pic);
//        }

//        private void Swap(Point a, Point b)
//        {
//            (_grid[a.Y, a.X], _grid[b.Y, b.X]) = (_grid[b.Y, b.X], _grid[a.Y, a.X]);
//        }

//        private List<Point> FindMatches()
//        {
//            var matches = new HashSet<Point>();

//            for (int y = 0; y < _rows; y++)
//                for (int x = 0; x < _cols - 2; x++)
//                    if (_grid[y, x].Name == _grid[y, x + 1].Name && _grid[y, x + 1].Name == _grid[y, x + 2].Name)
//                    {
//                        matches.Add(new Point(x, y));
//                        matches.Add(new Point(x + 1, y));
//                        matches.Add(new Point(x + 2, y));
//                    }

//            for (int x = 0; x < _cols; x++)
//                for (int y = 0; y < _rows - 2; y++)
//                    if (_grid[y, x].Name == _grid[y + 1, x].Name && _grid[y + 1, x].Name == _grid[y + 2, x].Name)
//                    {
//                        matches.Add(new Point(x, y));
//                        matches.Add(new Point(x, y + 1));
//                        matches.Add(new Point(x, y + 2));
//                    }

//            return matches.ToList();
//        }

//        private void SwapImages(PictureBox a, PictureBox b)
//        {
//            var tempImage = a.Image;
//            a.Image = b.Image;
//            b.Image = tempImage;
//        }

//        private void ProcessMatches(bool decrementMoves = false)
//        {
//            bool lastMoveCreatedMatch = false;

//            do
//            {
//                var matches = FindMatches();

//                if (matches.Count > 0)
//                {
//                    lastMoveCreatedMatch = true;
//                    _pointsCollected += matches.Count;
//                    _pointsLabel.Text = $"Punkty: {_pointsCollected}/{_pointsToWin}";

//                    if (CheckGameOver())
//                        RemoveMatches(matches);
//                }

//                if (!HasValidMove())
//                    ShuffleBoard();
//                else
//                    break;

//            } while (true);

//            if (decrementMoves && lastMoveCreatedMatch)
//            {
//                _movesLeft--;
//                _movesLabel.Text = $"Ruchy do końca: {_movesLeft}";
//            }

//            if (!HasValidMove())
//                ShuffleBoard();

//            SaveData();
//        }

//        private void RemoveMatches(List<Point> matches)
//        {
//            AnimateMatches(matches, () =>
//            {
//                var random = new Random();

//                foreach (var match in matches)
//                {
//                    int x = match.X, y = match.Y;
//                    _grid[y, x] = null;

//                    foreach (Control control in _controls)
//                    {
//                        if (control is PictureBox pic && pic.Tag is Point pos && pos.X == x && pos.Y == y)
//                        {
//                            _controls.Remove(pic);
//                            pic.Dispose();
//                            break;
//                        }
//                    }
//                }

//                for (int y = 0; y < _rows; y++)
//                    for (int x = 0; x < _cols; x++)
//                        if (_grid[y, x] == null)
//                        {
//                            var newElement = _elementTypes[random.Next(_elementTypes.Count)];
//                            _grid[y, x] = newElement;

//                            var pic = new PictureBox
//                            {
//                                Image = newElement.Icon,
//                                Size = new Size(_xSize, _ySize),
//                                Location = UIHelper.CalculateLocation(x, y, _cols, _rows, _xSize, _ySize),
//                                SizeMode = PictureBoxSizeMode.StretchImage,
//                                Tag = new Point(x, y),
//                                BackColor = Color.Transparent
//                            };

//                            pic.MouseDown += Pic_MouseDown;
//                            pic.AllowDrop = true;
//                            pic.DragEnter += Pic_DragEnter;
//                            pic.DragDrop += Pic_DragDrop;
//                            pic.MouseEnter += Pic_MouseEnter;
//                            pic.MouseLeave += Pic_MouseLeave;
//                            pic.MouseMove += Pic_MouseMove;
//                            pic.Click += Pic_Click;

//                            _controls.Add(pic);
//                        }

//                CenterElements();
//                ProcessMatches();
//            });
//        }

//        private bool CanFormMatch(int x1, int y1, int x2, int y2)
//        {
//            var temp = _grid[y1, x1];
//            _grid[y1, x1] = _grid[y2, x2];
//            _grid[y2, x2] = temp;

//            bool hasMatch = FindMatches().Count > 0;

//            _grid[y2, x2] = _grid[y1, x1];
//            _grid[y1, x1] = temp;

//            return hasMatch;
//        }

//        private bool HasValidMove()
//        {
//            for (int y = 0; y < _rows; y++)
//                for (int x = 0; x < _cols; x++)
//                {
//                    if (x < _cols - 1 && CanFormMatch(x, y, x + 1, y)) return true;
//                    if (y < _rows - 1 && CanFormMatch(x, y, x, y + 1)) return true;
//                }
//            return false;
//        }

//        private void ShuffleBoard()
//        {
//            var random = new Random();
//            var elements = new List<IGameElement>();

//            for (int y = 0; y < _rows; y++)
//                for (int x = 0; x < _cols; x++)
//                    elements.Add(_grid[y, x]);

//            int attempts = 0, maxAttempts = 1000;

//            do
//            {
//                elements = elements.OrderBy(e => random.Next()).ToList();

//                int index = 0;
//                for (int y = 0; y < _rows; y++)
//                    for (int x = 0; x < _cols; x++)
//                        _grid[y, x] = elements[index++];

//                attempts++;

//            } while ((!HasValidMove() || FindMatches().Count > 0) && attempts < maxAttempts);

//            foreach (Control control in _controls)
//            {
//                if (control is PictureBox pic && pic.Tag is Point position)
//                {
//                    int x = position.X, y = position.Y;
//                    pic.Image = _grid[y, x].Icon;
//                }
//            }

//            CenterElements();
//        }

//        private void GenerateLevel()
//        {
//            _elementTypes = new List<IGameElement>
//            {
//                new GameElement("Helmet", Image.FromFile("images/level/helmet.png"), Point.Empty),
//                new GameElement("GoldShield", Image.FromFile("images/level/gold_shield.png"), Point.Empty),
//                new GameElement("BlueShield", Image.FromFile("images/level/blue_shield.png"), Point.Empty),
//                new GameElement("Axe", Image.FromFile("images/level/axe.png"), Point.Empty),
//                new GameElement("Sword", Image.FromFile("images/level/sword.png"), Point.Empty)
//            };

//            _grid = new IGameElement[_rows, _cols];
//            _controls.OfType<PictureBox>().ToList().ForEach(p => { _controls.Remove(p); p.Dispose(); });

//            var random = new Random();
//            for (int y = 0; y < _rows; y++)
//                for (int x = 0; x < _cols; x++)
//                    AddElement(random.Next(_elementTypes.Count), x, y);

//            if (!HasValidMove()) ShuffleBoard();
//            ProcessMatches();
//        }

//        // Placeholder stubs
//        private void AnimateMatches(List<Point> matches, Action callback) => callback();
//        private void CenterElements() { }
//        private bool CheckGameOver() => true;
//        private void SaveData() { }
//        private void Pic_MouseDown(object sender, MouseEventArgs e) { }
//        private void Pic_MouseMove(object sender, MouseEventArgs e) { }
//        private void Pic_Click(object sender, EventArgs e) { }
//        private void Pic_DragDrop(object sender, DragEventArgs e) { }
//        private void Pic_DragEnter(object sender, DragEventArgs e) { }
//        private void Pic_MouseEnter(object sender, EventArgs e) { }
//        private void Pic_MouseLeave(object sender, EventArgs e) { }
//    }
//}