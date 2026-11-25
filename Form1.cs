
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace YMH219_FlappyBird
{
    public partial class Form1 : Form
    {
        private readonly System.Windows.Forms.Timer gameTimer;
        private GameManager game;

        private bool showInstructions = false;
        private bool showSelection = false;

        private float animationTime = 0;

        private Rectangle playRect = new Rectangle(290, 315, 220, 55);
        private Rectangle selectRect = new Rectangle(290, 385, 220, 48);
        private Rectangle howRect = new Rectangle(290, 447, 220, 42);

        private Rectangle backRect = new Rectangle(320, 445, 160, 42);

        private Rectangle[] birdRects =
        {
            new Rectangle(105, 205, 135, 175),
            new Rectangle(255, 205, 135, 175),
            new Rectangle(405, 205, 135, 175),
            new Rectangle(555, 205, 135, 175)
        };

        private Rectangle flapRect = new Rectangle(650, 435, 120, 55);
        private Rectangle retryRect = new Rectangle(290, 345, 220, 48);
        private Rectangle menuRect = new Rectangle(290, 405, 220, 45);

        private Point mousePosition = Point.Empty;

        public Form1()
        {
            InitializeComponent();

            Text = "FLAPPY BIRD - NIGHT EDITION";
            ClientSize = new Size(800, 550);
            StartPosition = FormStartPosition.CenterScreen;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            KeyPreview = true;
            DoubleBuffered = true;

            game = new GameManager();

            MouseClick += Form1_MouseClick;
            MouseMove += (s, e) =>
            {
                mousePosition = e.Location;
                Invalidate();
            };

            Paint += DrawGame;

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        // =====================================
        // KEYBOARD
        // =====================================

        protected override bool ProcessCmdKey(
            ref Message msg, Keys keyData)
        {
            if (showInstructions || showSelection)
            {
                if (keyData == Keys.Escape)
                {
                    showInstructions = false;
                    showSelection = false;
                    Invalidate();
                }

                return true;
            }

            if (keyData == Keys.Space || keyData == Keys.Up)
            {
                if (!game.GameOver)
                    game.Jump();

                Invalidate();
                return true;
            }

            if (keyData == Keys.Down)
            {
                if (game.Started && !game.GameOver)
                    game.Dive();

                return true;
            }

            if (keyData == Keys.Enter && game.GameOver)
            {
                game.Restart();
                game.Jump();
                Invalidate();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // =====================================
        // MOUSE CONTROLS
        // =====================================

        private void Form1_MouseClick(object? sender, MouseEventArgs e)
        {
            Point p = e.Location;

            if (showInstructions)
            {
                if (backRect.Contains(p))
                {
                    showInstructions = false;
                    Invalidate();
                }
                return;
            }

            if (showSelection)
            {
                if (backRect.Contains(p))
                {
                    showSelection = false;
                    Invalidate();
                    return;
                }

                if (birdRects[0].Contains(p))
                    SelectBird(BirdType.Yellow);
                else if (birdRects[1].Contains(p))
                    SelectBird(BirdType.Blue);
                else if (birdRects[2].Contains(p))
                    SelectBird(BirdType.Red);
                else if (birdRects[3].Contains(p))
                    SelectBird(BirdType.Green);

                return;
            }

            if (game.GameOver)
            {
                if (retryRect.Contains(p))
                {
                    game.Restart();
                    game.Jump();
                }
                else if (menuRect.Contains(p))
                {
                    game.Restart();
                }

                Invalidate();
                return;
            }

            if (!game.Started)
            {
                if (playRect.Contains(p))
                {
                    game.Jump();
                }
                else if (selectRect.Contains(p))
                {
                    showSelection = true;
                }
                else if (howRect.Contains(p))
                {
                    showInstructions = true;
                }
            }
            else
            {
                if (flapRect.Contains(p))
                    game.Jump();
                else
                    game.Jump();
            }

            Invalidate();
        }

        private void SelectBird(BirdType type)
        {
            game.Bird.SetType(type);
            showSelection = false;
            Invalidate();
        }

        // =====================================
        // GAME LOOP
        // =====================================

        private void GameLoop(object? sender, EventArgs e)
        {
            if (game.Started &&
                !game.GameOver &&
                !showInstructions &&
                !showSelection)
            {
                game.Update();
            }

            animationTime += 0.04f;
            Invalidate();
        }

        // =====================================
        // DRAW GAME
        // =====================================

        private void DrawGame(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(g);

            if (game.Started || game.GameOver)
            {
                DrawPipe(g, game.Pipe1.TopBounds, true);
                DrawPipe(g, game.Pipe1.BottomBounds, false);

                DrawPipe(g, game.Pipe2.TopBounds, true);
                DrawPipe(g, game.Pipe2.BottomBounds, false);

                game.Bird.Draw(g);
                DrawScore(g);
            }

            if (showInstructions)
            {
                DrawInstructions(g);
                return;
            }

            if (showSelection)
            {
                DrawSelection(g);
                return;
            }

            if (!game.Started && !game.GameOver)
                DrawMainMenu(g);

            if (game.GameOver)
                DrawGameOver(g);

            if (game.Started && !game.GameOver)
                DrawFlapButton(g);
        }

        // =====================================
        // BACKGROUND
        // =====================================

        private void DrawBackground(Graphics g)
        {
            Rectangle screen = new Rectangle(0, 0, 800, 550);

            using (LinearGradientBrush sky =
                new LinearGradientBrush(
                    screen,
                    Color.FromArgb(10, 18, 40),
                    Color.FromArgb(47, 72, 100),
                    90f))
            {
                g.FillRectangle(sky, screen);
            }

            DrawStars(g);
            DrawMoon(g);

            int move = (int)(animationTime * 10);

            DrawCloud(g, WrapX(60 - move), 95);
            DrawCloud(g, WrapX(360 - move), 160);
            DrawCloud(g, WrapX(700 - move), 85);
            DrawCloud(g, WrapX(180 - move), 245);

            DrawCity(g);

            Point[] hills =
            {
                new Point(0, 420),
                new Point(150, 350),
                new Point(310, 430),
                new Point(480, 350),
                new Point(650, 420),
                new Point(800, 355),
                new Point(800, 510),
                new Point(0, 510)
            };

            using (SolidBrush b = new SolidBrush(
                Color.FromArgb(30, 75, 68)))
            {
                g.FillPolygon(b, hills);
            }

            Point[] front =
            {
                new Point(0, 465),
                new Point(180, 400),
                new Point(360, 455),
                new Point(560, 390),
                new Point(800, 450),
                new Point(800, 510),
                new Point(0, 510)
            };

            using (SolidBrush b = new SolidBrush(
                Color.FromArgb(24, 91, 70)))
            {
                g.FillPolygon(b, front);
            }

            using (SolidBrush soil = new SolidBrush(
                Color.FromArgb(61, 53, 64)))
            {
                g.FillRectangle(soil, 0, 510, 800, 40);
            }

            using (SolidBrush grass = new SolidBrush(
                Color.FromArgb(45, 125, 77)))
            {
                g.FillRectangle(grass, 0, 510, 800, 10);
            }

            using (Pen p = new Pen(
                Color.FromArgb(105, 170, 110), 2))
            {
                g.DrawLine(p, 0, 510, 800, 510);
            }
        }

        private int WrapX(int x)
        {
            return ((x % 1000) + 1000) % 1000 - 130;
        }

        private void DrawStars(Graphics g)
        {
            Random r = new Random(10);

            for (int i = 0; i < 65; i++)
            {
                int x = r.Next(0, 800);
                int y = r.Next(15, 300);

                int alpha = 130 +
                    (int)((Math.Sin(animationTime * 2 + i) + 1) * 50);

                using (SolidBrush b = new SolidBrush(
                    Color.FromArgb(alpha, 230, 240, 255)))
                {
                    int size = i % 5 == 0 ? 4 : 2;
                    g.FillEllipse(b, x, y, size, size);
                }
            }
        }

        private void DrawMoon(Graphics g)
        {
            using (SolidBrush glow = new SolidBrush(
                Color.FromArgb(25, 230, 225, 190)))
            using (SolidBrush moon = new SolidBrush(
                Color.FromArgb(245, 235, 200)))
            {
                g.FillEllipse(glow, 610, 25, 125, 125);
                g.FillEllipse(moon, 635, 45, 75, 75);
            }

            using (SolidBrush crater = new SolidBrush(
                Color.FromArgb(210, 205, 175)))
            {
                g.FillEllipse(crater, 650, 65, 12, 9);
                g.FillEllipse(crater, 680, 90, 9, 8);
            }
        }

        private void DrawCloud(Graphics g, int x, int y)
        {
            if (x < -150 || x > 850)
                return;

            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(100, 15, 25, 45)))
            using (SolidBrush cloud = new SolidBrush(
                Color.FromArgb(185, 105, 130, 155)))
            {
                g.FillEllipse(shadow, x + 5, y + 18, 100, 30);

                g.FillEllipse(cloud, x, y + 15, 55, 30);
                g.FillEllipse(cloud, x + 20, y, 55, 45);
                g.FillEllipse(cloud, x + 50, y + 13, 55, 32);
            }
        }

        private void DrawCity(Graphics g)
        {
            using (SolidBrush buildings = new SolidBrush(
                Color.FromArgb(23, 36, 55)))
            using (SolidBrush windows = new SolidBrush(
                Color.FromArgb(190, 220, 175, 110)))
            {
                for (int i = 0; i < 10; i++)
                {
                    int x = i * 90;
                    int h = 55 + (i % 4) * 20;
                    int y = 410 - h;

                    g.FillRectangle(buildings, x, y, 55, h);

                    for (int row = 0; row < 3; row++)
                    {
                        for (int col = 0; col < 2; col++)
                        {
                            g.FillRectangle(
                                windows,
                                x + 10 + col * 22,
                                y + 12 + row * 18,
                                7, 7);
                        }
                    }
                }
            }
        }

        // =====================================
        // PIPE
        // =====================================

        private void DrawPipe(Graphics g, Rectangle rect, bool isTop)
        {
            if (rect.Height <= 0)
                return;

            using (SolidBrush b = new SolidBrush(
                Color.FromArgb(35, 130, 78)))
            using (Pen p = new Pen(
                Color.FromArgb(15, 65, 45), 3))
            {
                g.FillRectangle(b, rect);
                g.DrawRectangle(p, rect);
            }

            using (SolidBrush light = new SolidBrush(
                Color.FromArgb(100, 180, 115)))
            {
                g.FillRectangle(
                    light,
                    rect.X + 9,
                    rect.Y + 2,
                    9,
                    Math.Max(0, rect.Height - 4));
            }

            int capHeight = 20;

            Rectangle cap = isTop
                ? new Rectangle(rect.X - 7, rect.Bottom - capHeight,
                    rect.Width + 14, capHeight)
                : new Rectangle(rect.X - 7, rect.Y,
                    rect.Width + 14, capHeight);

            using (SolidBrush b = new SolidBrush(
                Color.FromArgb(45, 150, 85)))
            using (Pen p = new Pen(
                Color.FromArgb(15, 65, 45), 3))
            {
                g.FillRectangle(b, cap);
                g.DrawRectangle(p, cap);
            }
        }

        // =====================================
        // MAIN MENU
        // =====================================

        private void DrawMainMenu(Graphics g)
        {
            DrawPanel(g, new Rectangle(170, 65, 460, 440));

            // Small subtitle
            DrawText(g, "WELCOME TO", 12, Color.LightSkyBlue,
                new Rectangle(200, 85, 400, 25), true);

            // Main title
            DrawText(g, "FLAPPY BIRD", 39,
                Color.FromArgb(255, 225, 155),
                new Rectangle(185, 112, 430, 65), true);

            // Bird animation
            float bob = (float)Math.Sin(animationTime * 3) * 7;

            game.Bird.DrawAt(
                g, 375, 205 + (int)bob, 1.7f);

            DrawText(g, "THE NIGHT FLIGHT", 13,
                Color.FromArgb(175, 210, 225),
                new Rectangle(200, 270, 400, 25), true);

            DrawGameButton(g, playRect, "PLAY GAME",
                Color.FromArgb(55, 145, 110));

            DrawGameButton(g, selectRect, "CHOOSE YOUR BIRD",
                Color.FromArgb(55, 100, 145));

            DrawGameButton(g, howRect, "HOW TO PLAY",
                Color.FromArgb(70, 80, 110));
        }

        // =====================================
        // BIRD SELECTION
        // =====================================

        private void DrawSelection(Graphics g)
        {
            DrawPanel(g, new Rectangle(75, 75, 650, 420));

            DrawText(g, "CHOOSE YOUR BIRD", 30,
                Color.FromArgb(255, 225, 155),
                new Rectangle(100, 100, 600, 55), true);

            BirdType[] types =
            {
                BirdType.Yellow,
                BirdType.Blue,
                BirdType.Red,
                BirdType.Green
            };

            string[] names =
            {
                "SUNNY",
                "SKY",
                "BLAZE",
                "LEAF"
            };

            Color[] colors =
            {
                Color.FromArgb(210, 170, 70),
                Color.FromArgb(80, 160, 220),
                Color.FromArgb(210, 95, 90),
                Color.FromArgb(80, 175, 110)
            };

            for (int i = 0; i < 4; i++)
            {
                bool selected = game.Bird.SelectedType == types[i];

                DrawBirdCard(
                    g, birdRects[i], types[i],
                    names[i], colors[i], selected);
            }

            DrawText(g, "SELECT A CHARACTER FOR YOUR FLIGHT",
                11, Color.LightSkyBlue,
                new Rectangle(120, 400, 560, 25), true);

            DrawGameButton(g, backRect, "BACK",
                Color.FromArgb(65, 85, 110));
        }

        private void DrawBirdCard(
            Graphics g,
            Rectangle rect,
            BirdType type,
            string name,
            Color color,
            bool selected)
        {
            Color cardColor = selected
                ? Color.FromArgb(65, 100, 105)
                : Color.FromArgb(32, 48, 68);

            using (SolidBrush b = new SolidBrush(cardColor))
            using (Pen p = new Pen(
                selected ? Color.Gold : color, selected ? 4 : 2))
            {
                FillRoundRect(g, b, rect, 15);
                DrawRoundRect(g, p, rect, 15);
            }

            Bird preview = new Bird();
            preview.SetType(type);

            preview.DrawAt(g, rect.X + 40, rect.Y + 40, 1.15f);

            DrawText(g, name, 14, Color.White,
                new Rectangle(rect.X + 5, rect.Y + 105,
                    rect.Width - 10, 30), true);

            DrawText(g, selected ? "SELECTED" : "CHOOSE",
                9, selected ? Color.Gold : Color.LightSkyBlue,
                new Rectangle(rect.X + 5, rect.Y + 137,
                    rect.Width - 10, 20), true);
        }

        // =====================================
        // INSTRUCTIONS
        // =====================================

        private void DrawInstructions(Graphics g)
        {
            DrawPanel(g, new Rectangle(140, 80, 520, 420));

            DrawText(g, "HOW TO PLAY", 32,
                Color.FromArgb(255, 225, 155),
                new Rectangle(170, 105, 460, 55), true);

            DrawInstruction(g, 190, "SPACE / UP",
                "Make your bird fly upward.");

            DrawInstruction(g, 255, "DOWN ARROW",
                "Make your bird dive downward.");

            DrawInstruction(g, 320, "FLAP BUTTON",
                "Click the button during the game.");

            DrawInstruction(g, 385, "YOUR MISSION",
                "Avoid the pipes and collect points.");

            DrawGameButton(g, backRect, "BACK",
                Color.FromArgb(65, 85, 110));
        }

        private void DrawInstruction(
            Graphics g, int y, string title, string desc)
        {
            DrawText(g, title, 13,
                Color.LightSkyBlue,
                new Rectangle(205, y, 390, 25), false);

            DrawText(g, desc, 11,
                Color.WhiteSmoke,
                new Rectangle(205, y + 27, 390, 25), false);
        }

        // =====================================
        // GAME OVER
        // =====================================

        private void DrawGameOver(Graphics g)
        {
            DrawPanel(g, new Rectangle(165, 100, 470, 365));

            DrawText(g, "GAME OVER", 38,
                Color.FromArgb(245, 125, 120),
                new Rectangle(190, 130, 420, 65), true);

            DrawText(g, "YOUR SCORE", 15,
                Color.LightSkyBlue,
                new Rectangle(200, 225, 400, 25), true);

            DrawText(g, game.Score.ToString(), 35,
                Color.FromArgb(255, 225, 155),
                new Rectangle(200, 255, 400, 55), true);

            DrawGameButton(g, retryRect, "TRY AGAIN",
                Color.FromArgb(55, 145, 110));

            DrawGameButton(g, menuRect, "MAIN MENU",
                Color.FromArgb(65, 85, 110));
        }

        // =====================================
        // FLAP BUTTON
        // =====================================

        private void DrawFlapButton(Graphics g)
        {
            DrawGameButton(g, flapRect, "FLAP!",
                Color.FromArgb(55, 145, 110));
        }

        // =====================================
        // SCORE
        // =====================================

        private void DrawScore(Graphics g)
        {
            DrawText(g, game.Score.ToString(), 32,
                Color.White,
                new Rectangle(320, 15, 160, 55), true);
        }

        // =====================================
        // UI DRAWING HELPERS
        // =====================================

        private void DrawPanel(Graphics g, Rectangle rect)
        {
            using (SolidBrush b = new SolidBrush(
                Color.FromArgb(225, 17, 29, 49)))
            using (Pen p = new Pen(
                Color.FromArgb(90, 155, 180), 3))
            {
                FillRoundRect(g, b, rect, 22);
                DrawRoundRect(g, p, rect, 22);
            }
        }

        private void DrawGameButton(
            Graphics g,
            Rectangle rect,
            string text,
            Color color)
        {
            bool hover = rect.Contains(mousePosition);

            Color buttonColor = hover
                ? ControlPaint.Light(color, 0.25f)
                : color;

            Rectangle drawRect = rect;

            if (hover)
                drawRect = new Rectangle(
                    rect.X - 2, rect.Y - 2,
                    rect.Width + 4, rect.Height + 4);

            using (SolidBrush b = new SolidBrush(buttonColor))
            using (Pen p = new Pen(
                hover ? Color.FromArgb(255, 225, 155)
                      : Color.FromArgb(110, 170, 190),
                hover ? 3 : 2))
            {
                FillRoundRect(g, b, drawRect, 14);
                DrawRoundRect(g, p, drawRect, 14);
            }

            DrawText(g, text, 13,
                Color.White,
                drawRect, true);
        }

        private void DrawText(
            Graphics g,
            string text,
            float size,
            Color color,
            Rectangle rect,
            bool centered)
        {
            using (Font font = new Font(
                "Segoe UI", size, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(color))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = centered
                    ? StringAlignment.Center
                    : StringAlignment.Near;

                format.LineAlignment = StringAlignment.Center;

                g.DrawString(text, font, brush, rect, format);
            }
        }

        private void FillRoundRect(
            Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = MakeRoundPath(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawRoundRect(
            Graphics g, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath path = MakeRoundPath(rect, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath MakeRoundPath(
            Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d,
                d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d,
                d, d, 90, 90);

            path.CloseFigure();

            return path;
        }
    }
}
