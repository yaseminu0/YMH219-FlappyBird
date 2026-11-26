
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

        private bool showInstructions;
        private bool showSelection;
        private bool showSettings;
        private bool effectsEnabled = true;

        private float animationTime;
        private float coinEffectTime;
        private float levelEffectTime;

        private Point mousePosition = Point.Empty;

        private readonly Rectangle playRect = new Rectangle(290, 325, 220, 52);
        private readonly Rectangle selectRect = new Rectangle(290, 390, 220, 45);
        private readonly Rectangle howRect = new Rectangle(290, 448, 105, 38);
        private readonly Rectangle settingsRect = new Rectangle(405, 448, 105, 38);

        private readonly Rectangle backRect = new Rectangle(320, 445, 160, 42);

        private readonly Rectangle[] birdRects =
        {
            new Rectangle(105, 205, 135, 175),
            new Rectangle(255, 205, 135, 175),
            new Rectangle(405, 205, 135, 175),
            new Rectangle(555, 205, 135, 175)
        };

        private readonly Rectangle pauseRect = new Rectangle(690, 18, 90, 38);
        private readonly Rectangle resumeRect = new Rectangle(290, 315, 220, 48);
        private readonly Rectangle pauseMenuRect = new Rectangle(290, 375, 220, 45);

        private readonly Rectangle retryRect = new Rectangle(290, 390, 220, 45);
        private readonly Rectangle menuRect = new Rectangle(290, 450, 220, 42);

        private readonly Rectangle effectsRect = new Rectangle(300, 300, 200, 45);

        private readonly Color NeonCyan = Color.FromArgb(40, 235, 255);
        private readonly Color NeonPurple = Color.FromArgb(165, 80, 255);
        private readonly Color NeonPink = Color.FromArgb(255, 65, 170);
        private readonly Color NeonBlue = Color.FromArgb(65, 125, 255);

        public Form1()
        {
            InitializeComponent();

            Text = "NEON FLAPPY - Premium Arcade";
            ClientSize = new Size(800, 550);
            StartPosition = FormStartPosition.CenterScreen;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (showInstructions || showSelection || showSettings)
            {
                if (keyData == Keys.Escape)
                {
                    showInstructions = false;
                    showSelection = false;
                    showSettings = false;
                    Invalidate();
                }

                return true;
            }

            if (keyData == Keys.P)
            {
                if (game.Started && !game.GameOver)
                    game.TogglePause();

                Invalidate();
                return true;
            }

            if (keyData == Keys.Space || keyData == Keys.Up)
            {
                if (!game.GameOver && !game.Paused)
                    game.Jump();

                Invalidate();
                return true;
            }

            if (keyData == Keys.Down)
            {
                if (game.Started && !game.GameOver && !game.Paused)
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

            if (keyData == Keys.Escape && game.Paused)
            {
                game.Resume();
                Invalidate();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // =====================================
        // MOUSE CONTROLS
        // =====================================

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;

            if (showInstructions)
            {
                if (backRect.Contains(p))
                    showInstructions = false;

                Invalidate();
                return;
            }

            if (showSettings)
            {
                if (backRect.Contains(p))
                {
                    showSettings = false;
                }
                else if (effectsRect.Contains(p))
                {
                    effectsEnabled = !effectsEnabled;
                }

                Invalidate();
                return;
            }

            if (showSelection)
            {
                if (backRect.Contains(p))
                {
                    showSelection = false;
                }
                else if (birdRects[0].Contains(p))
                {
                    SelectBird(BirdType.Yellow);
                }
                else if (birdRects[1].Contains(p))
                {
                    SelectBird(BirdType.Blue);
                }
                else if (birdRects[2].Contains(p))
                {
                    SelectBird(BirdType.Red);
                }
                else if (birdRects[3].Contains(p))
                {
                    SelectBird(BirdType.Green);
                }

                Invalidate();
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

            if (game.Paused)
            {
                if (resumeRect.Contains(p))
                {
                    game.Resume();
                }
                else if (pauseMenuRect.Contains(p))
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
                else if (settingsRect.Contains(p))
                {
                    showSettings = true;
                }
            }
            else
            {
                if (pauseRect.Contains(p))
                    game.TogglePause();
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

        private void GameLoop(object sender, EventArgs e)
        {
            if (game.Started &&
                !game.GameOver &&
                !game.Paused &&
                !showInstructions &&
                !showSelection &&
                !showSettings)
            {
                int previousCoins = game.TotalCoins;
                int previousLevel = game.CurrentLevel;

                game.Update();

                if (game.TotalCoins > previousCoins && effectsEnabled)
                    coinEffectTime = 0.8f;

                if (game.CurrentLevel > previousLevel)
                    levelEffectTime = 1.3f;
            }

            if (coinEffectTime > 0)
                coinEffectTime -= 0.04f;

            if (levelEffectTime > 0)
                levelEffectTime -= 0.04f;

            animationTime += 0.04f;

            Invalidate();
        }

        // =====================================
        // DRAW GAME
        // =====================================

        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            DrawBackground(g);

            if (game.Started || game.GameOver)
            {
                DrawPipe(g, game.Pipe1.TopBounds, true);
                DrawPipe(g, game.Pipe1.BottomBounds, false);

                DrawPipe(g, game.Pipe2.TopBounds, true);
                DrawPipe(g, game.Pipe2.BottomBounds, false);

                game.Coin1.Draw(g);
                game.Coin2.Draw(g);

                game.Bird.Draw(g);

                DrawScore(g);
                DrawCoinEffect(g);
                DrawLevelEffect(g);
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

            if (showSettings)
            {
                DrawSettings(g);
                return;
            }

            if (!game.Started && !game.GameOver)
                DrawMainMenu(g);

            if (game.GameOver)
                DrawGameOver(g);

            if (game.Paused)
                DrawPauseMenu(g);

            if (game.Started && !game.GameOver && !game.Paused)
                DrawPauseButton(g);
        }

        // =====================================
        // BACKGROUND
        // =====================================

        private void DrawBackground(Graphics g)
        {
            using (LinearGradientBrush sky = new LinearGradientBrush(
                new Rectangle(0, 0, 800, 550),
                Color.FromArgb(3, 5, 24),
                Color.FromArgb(30, 12, 65),
                90f))
            {
                g.FillRectangle(sky, 0, 0, 800, 550);
            }

            DrawNebula(g);
            DrawStars(g);
            DrawMoon(g);
            DrawDistantBuildings(g);
            DrawNeonCity(g);
            DrawFlyingLights(g);
            DrawGround(g);
        }

        private void DrawNebula(Graphics g)
        {
            using (SolidBrush purpleGlow = new SolidBrush(
                Color.FromArgb(18, 130, 50, 220)))
            using (SolidBrush blueGlow = new SolidBrush(
                Color.FromArgb(15, 30, 130, 255)))
            {
                g.FillEllipse(purpleGlow, 60, 70, 360, 230);
                g.FillEllipse(blueGlow, 400, 100, 350, 200);
            }
        }

        private void DrawStars(Graphics g)
        {
            Random random = new Random(15);

            for (int i = 0; i < 85; i++)
            {
                int x = random.Next(0, 800);
                int y = random.Next(10, 340);

                int size = i % 7 == 0 ? 3 : 2;

                int alpha = 100 +
                    (int)((Math.Sin(animationTime * 2 + i) + 1) * 75);

                alpha = Math.Max(0, Math.Min(255, alpha));

                Color starColor = i % 3 == 0
                    ? Color.FromArgb(alpha, 100, 220, 255)
                    : Color.FromArgb(alpha, 220, 225, 255);

                using (SolidBrush brush = new SolidBrush(starColor))
                {
                    g.FillEllipse(brush, x, y, size, size);
                }
            }
        }

        private void DrawMoon(Graphics g)
        {
            using (SolidBrush outerGlow = new SolidBrush(
                Color.FromArgb(15, 100, 190, 255)))
            using (SolidBrush innerGlow = new SolidBrush(
                Color.FromArgb(35, 90, 160, 255)))
            using (SolidBrush moon = new SolidBrush(
                Color.FromArgb(245, 190, 255)))
            using (Pen moonBorder = new Pen(
                Color.FromArgb(170, 100, 220, 255), 2))
            {
                g.FillEllipse(outerGlow, 610, 22, 150, 150);
                g.FillEllipse(innerGlow, 625, 37, 120, 120);
                g.FillEllipse(moon, 650, 60, 72, 72);
                g.DrawEllipse(moonBorder, 650, 60, 72, 72);
            }

            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(220, 160, 160, 225)))
            {
                g.FillEllipse(shadow, 680, 75, 13, 13);
                g.FillEllipse(shadow, 660, 100, 8, 8);
            }
        }

        private void DrawDistantBuildings(Graphics g)
        {
            Random random = new Random(27);

            using (SolidBrush buildingBrush = new SolidBrush(
                Color.FromArgb(12, 20, 55)))
            using (Pen outline = new Pen(
                Color.FromArgb(65, 55, 120, 200), 1))
            {
                for (int i = 0; i < 18; i++)
                {
                    int x = i * 48;
                    int width = 42;
                    int height = 70 + random.Next(0, 100);
                    int y = 445 - height;

                    Rectangle building = new Rectangle(
                        x, y, width, height);

                    g.FillRectangle(buildingBrush, building);
                    g.DrawRectangle(outline, building);
                }
            }
        }

        private void DrawNeonCity(Graphics g)
        {
            for (int i = 0; i < 11; i++)
            {
                int x = i * 78 - 10;
                int height = 75 + (i * 37 % 100);
                int y = 465 - height;
                int width = 65;

                Rectangle building = new Rectangle(
                    x, y, width, height);

                Color buildingColor = i % 2 == 0
                    ? Color.FromArgb(13, 22, 55)
                    : Color.FromArgb(20, 15, 58);

                using (SolidBrush body = new SolidBrush(buildingColor))
                using (Pen border = new Pen(
                    i % 2 == 0 ? NeonCyan : NeonPurple, 1.2f))
                {
                    g.FillRectangle(body, building);

                    using (Pen glow = new Pen(
                        Color.FromArgb(45, border.Color), 5))
                    {
                        g.DrawLine(
                            glow,
                            x + width / 2,
                            y,
                            x + width / 2,
                            y + height
                        );
                    }

                    g.DrawRectangle(border, building);
                }

                DrawBuildingWindows(g, building, i);
                DrawBuildingRoof(g, building, i);
            }
        }

        private void DrawBuildingWindows(
            Graphics g,
            Rectangle building,
            int index)
        {
            Color windowColor = index % 2 == 0
                ? Color.FromArgb(210, 45, 210, 255)
                : Color.FromArgb(210, 195, 70, 255);

            using (SolidBrush windows = new SolidBrush(windowColor))
            using (SolidBrush darkWindows = new SolidBrush(
                Color.FromArgb(160, 8, 15, 45)))
            {
                for (int y = building.Y + 17;
                     y < building.Bottom - 12;
                     y += 22)
                {
                    for (int x = building.X + 10;
                         x < building.Right - 7;
                         x += 20)
                    {
                        g.FillRectangle(darkWindows, x, y, 8, 12);

                        if ((x + y + index) % 3 != 0)
                            g.FillRectangle(windows, x + 2, y + 2, 4, 7);
                    }
                }
            }
        }

        private void DrawBuildingRoof(
            Graphics g,
            Rectangle building,
            int index)
        {
            Color roofColor = index % 2 == 0
                ? NeonCyan
                : NeonPink;

            using (Pen glow = new Pen(
                Color.FromArgb(45, roofColor), 8))
            using (Pen line = new Pen(roofColor, 2))
            {
                g.DrawLine(
                    glow,
                    building.X,
                    building.Y,
                    building.Right,
                    building.Y
                );

                g.DrawLine(
                    line,
                    building.X,
                    building.Y,
                    building.Right,
                    building.Y
                );
            }
        }

        private void DrawFlyingLights(Graphics g)
        {
            for (int i = 0; i < 8; i++)
            {
                int x = WrapX((int)(i * 130 - animationTime * 35));
                int y = 180 + (i * 31 % 150);

                using (Pen glow = new Pen(
                    Color.FromArgb(35, 60, 220, 255), 5))
                using (Pen line = new Pen(
                    Color.FromArgb(180, 80, 230, 255), 1))
                {
                    g.DrawLine(glow, x, y, x + 17, y);
                    g.DrawLine(line, x, y, x + 17, y);
                }
            }
        }

        private int WrapX(int x)
        {
            return ((x % 950) + 950) % 950 - 80;
        }

        private void DrawGround(Graphics g)
        {
            using (LinearGradientBrush ground = new LinearGradientBrush(
                new Rectangle(0, 510, 800, 40),
                Color.FromArgb(12, 15, 42),
                Color.FromArgb(4, 8, 25),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(ground, 0, 510, 800, 40);
            }

            using (Pen outerGlow = new Pen(
                Color.FromArgb(45, 30, 220, 255), 9))
            using (Pen neonLine = new Pen(NeonCyan, 2))
            {
                g.DrawLine(outerGlow, 0, 511, 800, 511);
                g.DrawLine(neonLine, 0, 511, 800, 511);
            }

            int offset = (int)(animationTime * 35) % 50;

            using (Pen floorLines = new Pen(
                Color.FromArgb(65, 90, 65, 200), 1))
            {
                for (int x = -offset; x < 850; x += 50)
                {
                    g.DrawLine(floorLines, x, 520, x + 25, 550);
                    g.DrawLine(floorLines, x + 25, 550, x + 50, 520);
                }
            }
        }

        // =====================================
        // FUTURISTIC BUILDING OBSTACLES
        // =====================================

        private void DrawPipe(Graphics g, Rectangle rect, bool isTop)
        {
            if (rect.Height <= 0 || rect.Width <= 0)
                return;

            Color dark = Color.FromArgb(5, 13, 35);
            Color blue = Color.FromArgb(13, 55, 105);
            Color cyan = Color.FromArgb(45, 225, 255);

            // Building shadow
            Rectangle shadowRect = new Rectangle(
                rect.X + 5,
                rect.Y,
                rect.Width,
                rect.Height
            );

            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(80, 0, 0, 0)))
            {
                g.FillRectangle(shadow, shadowRect);
            }

            // Main building body
            using (LinearGradientBrush body = new LinearGradientBrush(
                rect,
                blue,
                dark,
                LinearGradientMode.Horizontal))
            {
                g.FillRectangle(body, rect);
            }

            // Architectural side panels
            DrawObstacleSidePanels(g, rect);

            // Repeated illuminated windows
            DrawObstacleWindows(g, rect, isTop);

            // Structural horizontal floors
            DrawObstacleFloors(g, rect);

            // Neon outline
            using (Pen outerGlow = new Pen(
                Color.FromArgb(35, cyan), 11))
            using (Pen middleGlow = new Pen(
                Color.FromArgb(75, cyan), 5))
            using (Pen outline = new Pen(
                Color.FromArgb(235, cyan), 2))
            {
                g.DrawRectangle(outerGlow, rect);
                g.DrawRectangle(middleGlow, rect);
                g.DrawRectangle(outline, rect);
            }

            // Energy platform at the opening
            DrawEnergyGate(g, rect, isTop);

            // Decorative corner lights
            DrawObstacleCornerLights(g, rect);
        }

        private void DrawObstacleSidePanels(Graphics g, Rectangle rect)
        {
            if (rect.Height < 35)
                return;

            using (LinearGradientBrush leftPanel =
                new LinearGradientBrush(
                    new Rectangle(rect.X + 3, rect.Y, 12, rect.Height),
                    Color.FromArgb(60, 75, 145, 205),
                    Color.FromArgb(10, 15, 35),
                    LinearGradientMode.Horizontal))
            using (LinearGradientBrush rightPanel =
                new LinearGradientBrush(
                    new Rectangle(rect.Right - 15, rect.Y, 12, rect.Height),
                    Color.FromArgb(10, 15, 35),
                    Color.FromArgb(60, 75, 145, 205),
                    LinearGradientMode.Horizontal))
            {
                g.FillRectangle(
                    leftPanel,
                    rect.X + 3,
                    rect.Y,
                    12,
                    rect.Height
                );

                g.FillRectangle(
                    rightPanel,
                    rect.Right - 15,
                    rect.Y,
                    12,
                    rect.Height
                );
            }

            using (Pen cyanLine = new Pen(
                Color.FromArgb(190, 65, 220, 255), 2))
            using (Pen purpleLine = new Pen(
                Color.FromArgb(170, 155, 85, 255), 1))
            {
                g.DrawLine(
                    cyanLine,
                    rect.X + 15,
                    rect.Y + 5,
                    rect.X + 15,
                    rect.Bottom - 5
                );

                g.DrawLine(
                    purpleLine,
                    rect.Right - 15,
                    rect.Y + 5,
                    rect.Right - 15,
                    rect.Bottom - 5
                );
            }
        }

        private void DrawObstacleWindows(
            Graphics g,
            Rectangle rect,
            bool isTop)
        {
            if (rect.Height < 75)
                return;

            using (SolidBrush darkWindow = new SolidBrush(
                Color.FromArgb(230, 3, 12, 32)))
            using (SolidBrush blueWindow = new SolidBrush(
                Color.FromArgb(210, 35, 170, 225)))
            using (SolidBrush purpleWindow = new SolidBrush(
                Color.FromArgb(200, 135, 75, 235)))
            {
                int startY = rect.Y + 32;
                int endY = rect.Bottom - 18;

                for (int y = startY; y + 17 < endY; y += 48)
                {
                    for (int column = 0; column < 2; column++)
                    {
                        int windowX = rect.X + 21 + column * 17;

                        Rectangle window = new Rectangle(
                            windowX,
                            y,
                            9,
                            18
                        );

                        g.FillRectangle(darkWindow, window);

                        if ((y + column + (isTop ? 1 : 0)) % 3 == 0)
                            g.FillRectangle(purpleWindow, window.X + 2,
                                window.Y + 3, 5, 11);
                        else
                            g.FillRectangle(blueWindow, window.X + 2,
                                window.Y + 3, 5, 11);
                    }
                }
            }
        }

        private void DrawObstacleFloors(Graphics g, Rectangle rect)
        {
            if (rect.Height < 65)
                return;

            using (Pen floorLine = new Pen(
                Color.FromArgb(100, 90, 150, 210), 1))
            using (Pen floorGlow = new Pen(
                Color.FromArgb(40, 40, 210, 255), 3))
            {
                for (int y = rect.Y + 25;
                     y < rect.Bottom - 10;
                     y += 48)
                {
                    g.DrawLine(
                        floorGlow,
                        rect.X + 16,
                        y,
                        rect.Right - 16,
                        y
                    );

                    g.DrawLine(
                        floorLine,
                        rect.X + 16,
                        y,
                        rect.Right - 16,
                        y
                    );
                }
            }
        }

        private void DrawEnergyGate(
            Graphics g,
            Rectangle rect,
            bool isTop)
        {
            int capHeight = 20;

            Rectangle cap = isTop
                ? new Rectangle(
                    rect.X - 9,
                    rect.Bottom - capHeight,
                    rect.Width + 18,
                    capHeight)
                : new Rectangle(
                    rect.X - 9,
                    rect.Y,
                    rect.Width + 18,
                    capHeight);

            using (LinearGradientBrush capBrush =
                new LinearGradientBrush(
                    cap,
                    Color.FromArgb(18, 75, 145),
                    Color.FromArgb(45, 205, 235),
                    LinearGradientMode.Vertical))
            using (Pen glow = new Pen(
                Color.FromArgb(75, 35, 220, 255), 9))
            using (Pen outline = new Pen(
                Color.FromArgb(245, 130, 245, 255), 2))
            {
                g.FillRectangle(capBrush, cap);
                g.DrawRectangle(glow, cap);
                g.DrawRectangle(outline, cap);
            }

            // Energy strip
            using (Pen energy = new Pen(
                Color.FromArgb(240, 225, 255, 255), 2))
            {
                int centerY = cap.Y + cap.Height / 2;

                g.DrawLine(
                    energy,
                    cap.X + 12,
                    centerY,
                    cap.Right - 12,
                    centerY
                );
            }

            // Small illuminated bolts
            using (SolidBrush bolt = new SolidBrush(Color.White))
            {
                g.FillEllipse(bolt, cap.X + 7, cap.Y + 7, 4, 4);
                g.FillEllipse(bolt, cap.Right - 11, cap.Y + 7, 4, 4);
            }
        }

        private void DrawObstacleCornerLights(Graphics g, Rectangle rect)
        {
            if (rect.Height < 30)
                return;

            using (SolidBrush light = new SolidBrush(
                Color.FromArgb(230, 100, 240, 255)))
            {
                g.FillEllipse(
                    light,
                    rect.X + 4,
                    rect.Y + 5,
                    4,
                    4
                );

                g.FillEllipse(
                    light,
                    rect.Right - 8,
                    rect.Y + 5,
                    4,
                    4
                );
            }
        }

        // =====================================
        // MAIN MENU
        // =====================================

        private void DrawMainMenu(Graphics g)
        {
            DrawPanel(g, new Rectangle(165, 45, 470, 475));

            DrawText(g, "WELCOME TO", 14,
                Color.FromArgb(135, 220, 255),
                new Rectangle(200, 65, 400, 28), true);

            DrawNeonTitle(
                g,
                "NEON FLAPPY",
                new Rectangle(185, 95, 430, 60)
            );

            float bob = (float)Math.Sin(animationTime * 3) * 7;

            game.Bird.DrawAt(
                g,
                400,
                210 + (int)bob,
                1.8f
            );

            DrawText(g, "FLY THROUGH THE NEON CITY",
                12, Color.FromArgb(145, 210, 245),
                new Rectangle(200, 265, 400, 28), true);

            DrawButton(g, playRect, "START GAME",
                Color.FromArgb(15, 125, 135));

            DrawButton(g, selectRect, "CHOOSE YOUR BIRD",
                Color.FromArgb(45, 65, 145));

            DrawButton(g, howRect, "HOW TO PLAY",
                Color.FromArgb(70, 45, 125));

            DrawButton(g, settingsRect, "SETTINGS",
                Color.FromArgb(100, 40, 120));

            DrawText(g,
                "BEST: " + game.BestScore +
                "      COINS: " + game.TotalCoins,
                12, Color.FromArgb(255, 215, 110),
                new Rectangle(200, 490, 400, 22), true);
        }

        private void DrawNeonTitle(
            Graphics g,
            string text,
            Rectangle rect)
        {
            using (Font font = new Font(
                "Arial",
                37,
                FontStyle.Bold))
            using (SolidBrush glow = new SolidBrush(
                Color.FromArgb(70, 35, 210, 255)))
            using (SolidBrush main = new SolidBrush(
                Color.FromArgb(245, 225, 255)))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                for (int i = 3; i >= 1; i--)
                {
                    Rectangle glowRect = new Rectangle(
                        rect.X - i,
                        rect.Y - i,
                        rect.Width + i * 2,
                        rect.Height + i * 2
                    );

                    g.DrawString(text, font, glow, glowRect, format);
                }

                g.DrawString(text, font, main, rect, format);
            }
        }

        // =====================================
        // BIRD SELECTION
        // =====================================

        private void DrawSelection(Graphics g)
        {
            DrawPanel(g, new Rectangle(75, 75, 650, 420));

            DrawNeonTitle(
                g,
                "CHOOSE YOUR BIRD",
                new Rectangle(100, 100, 600, 55)
            );

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
                "CYBER",
                "BLAZE",
                "NOVA"
            };

            Color[] colors =
            {
                Color.FromArgb(255, 195, 65),
                Color.FromArgb(50, 195, 255),
                Color.FromArgb(255, 80, 125),
                Color.FromArgb(60, 240, 165)
            };

            for (int i = 0; i < 4; i++)
            {
                bool selected = game.Bird.SelectedType == types[i];

                DrawBirdCard(
                    g,
                    birdRects[i],
                    types[i],
                    names[i],
                    colors[i],
                    selected
                );
            }

            DrawText(g, "SELECT YOUR FAVORITE CHARACTER",
                11, Color.FromArgb(160, 210, 245),
                new Rectangle(120, 400, 560, 25), true);

            DrawButton(g, backRect, "BACK",
                Color.FromArgb(50, 65, 130));
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
                ? Color.FromArgb(35, 65, 100)
                : Color.FromArgb(12, 22, 55);

            using (SolidBrush brush = new SolidBrush(cardColor))
            using (Pen glow = new Pen(
                Color.FromArgb(selected ? 150 : 65, color), 7))
            using (Pen border = new Pen(
                selected ? Color.White : color,
                selected ? 3 : 1.5f))
            {
                g.FillRectangle(brush, rect);
                g.DrawRectangle(glow, rect);
                g.DrawRectangle(border, rect);
            }

            Bird preview = new Bird();
            preview.SetType(type);

            preview.DrawAt(
                g,
                rect.X + 67,
                rect.Y + 65,
                1.35f
            );

            DrawText(g, name, 15, Color.White,
                new Rectangle(rect.X + 5, rect.Y + 112,
                    rect.Width - 10, 28), true);

            DrawText(g, selected ? "SELECTED" : "CHOOSE",
                10,
                selected ? Color.Gold : Color.LightSkyBlue,
                new Rectangle(rect.X + 5, rect.Y + 143,
                    rect.Width - 10, 20), true);
        }

        // =====================================
        // INSTRUCTIONS
        // =====================================

        private void DrawInstructions(Graphics g)
        {
            DrawPanel(g, new Rectangle(140, 70, 520, 420));

            DrawNeonTitle(
                g,
                "HOW TO PLAY",
                new Rectangle(170, 95, 460, 55)
            );

            DrawInstruction(g, 175, "SPACE / UP",
                "Make your bird fly upward.");

            DrawInstruction(g, 235, "DOWN ARROW",
                "Make your bird dive downward.");

            DrawInstruction(g, 295, "P KEY",
                "Pause or resume your game.");

            DrawInstruction(g, 355, "YOUR MISSION",
                "Avoid energy gates and collect coins.");

            DrawButton(g, backRect, "BACK",
                Color.FromArgb(50, 65, 130));
        }

        private void DrawInstruction(
            Graphics g,
            int y,
            string title,
            string desc)
        {
            DrawText(g, title, 13, Color.LightSkyBlue,
                new Rectangle(205, y, 390, 25), false);

            DrawText(g, desc, 11, Color.WhiteSmoke,
                new Rectangle(205, y + 27, 390, 25), false);
        }

        // =====================================
        // SETTINGS
        // =====================================

        private void DrawSettings(Graphics g)
        {
            DrawPanel(g, new Rectangle(160, 90, 480, 380));

            DrawNeonTitle(
                g,
                "GAME SETTINGS",
                new Rectangle(180, 115, 440, 55)
            );

            DrawText(g, "VISUAL EFFECTS", 14,
                Color.LightSkyBlue,
                new Rectangle(220, 225, 360, 30), true);

            DrawButton(
                g,
                effectsRect,
                effectsEnabled ? "EFFECTS: ON" : "EFFECTS: OFF",
                effectsEnabled
                    ? Color.FromArgb(15, 130, 125)
                    : Color.FromArgb(100, 45, 85)
            );

            DrawText(g, "DIFFICULTY: PROGRESSIVE",
                12, Color.WhiteSmoke,
                new Rectangle(220, 365, 360, 25), true);

            DrawButton(g, backRect, "BACK",
                Color.FromArgb(50, 65, 130));
        }

        // =====================================
        // GAME OVER
        // =====================================

        private void DrawGameOver(Graphics g)
        {
            using (SolidBrush overlay = new SolidBrush(
                Color.FromArgb(185, 3, 5, 25)))
            {
                g.FillRectangle(overlay, 0, 0, 800, 550);
            }

            Rectangle panel = new Rectangle(205, 65, 390, 425);

            DrawPanel(g, panel);

            DrawNeonTitle(
                g,
                "GAME OVER",
                new Rectangle(225, 95, 350, 55)
            );

            DrawText(g, "NICE TRY! KEEP FLYING",
                11, Color.FromArgb(155, 200, 235),
                new Rectangle(230, 148, 340, 25), true);

            DrawPanel(g, new Rectangle(255, 190, 290, 75));

            DrawText(g, "YOUR SCORE", 11,
                Color.FromArgb(160, 190, 220),
                new Rectangle(265, 197, 270, 20), true);

            DrawText(g, game.Score.ToString(), 29,
                Color.White,
                new Rectangle(265, 215, 270, 42), true);

            DrawText(g, "BEST SCORE", 11,
                Color.FromArgb(145, 180, 215),
                new Rectangle(245, 280, 145, 23), true);

            DrawText(g, game.BestScore.ToString(), 20,
                Color.White,
                new Rectangle(245, 303, 145, 30), true);

            DrawText(g, "COINS THIS RUN", 11,
                Color.FromArgb(255, 205, 105),
                new Rectangle(410, 280, 145, 23), true);

            DrawText(g, game.RunCoins.ToString(), 20,
                Color.FromArgb(255, 215, 100),
                new Rectangle(410, 303, 145, 30), true);

            DrawText(g, "TOTAL COINS: " + game.TotalCoins,
                12, Color.FromArgb(255, 215, 120),
                new Rectangle(240, 343, 320, 25), true);

            DrawButton(g, retryRect, "PLAY AGAIN",
                Color.FromArgb(15, 135, 115));

            DrawButton(g, menuRect, "MAIN MENU",
                Color.FromArgb(45, 70, 145));
        }

        // =====================================
        // PAUSE
        // =====================================

        private void DrawPauseButton(Graphics g)
        {
            DrawButton(g, pauseRect, "PAUSE",
                Color.FromArgb(50, 65, 130));
        }

        private void DrawPauseMenu(Graphics g)
        {
            using (SolidBrush overlay = new SolidBrush(
                Color.FromArgb(185, 3, 5, 25)))
            {
                g.FillRectangle(overlay, 0, 0, 800, 550);
            }

            DrawPanel(g, new Rectangle(190, 155, 420, 270));

            DrawNeonTitle(
                g,
                "GAME PAUSED",
                new Rectangle(210, 180, 380, 55)
            );

            DrawText(g, "Your bird is waiting for you!",
                12, Color.LightSkyBlue,
                new Rectangle(210, 245, 380, 30), true);

            DrawButton(g, resumeRect, "RESUME",
                Color.FromArgb(15, 135, 115));

            DrawButton(g, pauseMenuRect, "MAIN MENU",
                Color.FromArgb(45, 70, 145));
        }

        // =====================================
        // SCORE HUD
        // =====================================

        private void DrawScore(Graphics g)
        {
            DrawPanel(g, new Rectangle(12, 12, 220, 85));

            DrawText(g, "BEST: " + game.BestScore, 12,
                Color.LightSkyBlue,
                new Rectangle(25, 18, 190, 25), false);

            DrawText(g, "RUN COINS: " + game.RunCoins, 12,
                Color.Gold,
                new Rectangle(25, 48, 190, 22), false);

            DrawPanel(g, new Rectangle(300, 12, 200, 65));

            DrawText(g, game.Score.ToString(), 32,
                Color.White,
                new Rectangle(315, 15, 170, 55), true);

            DrawPanel(g, new Rectangle(565, 12, 115, 65));

            DrawText(g, game.DifficultyName, 10,
                Color.LightSkyBlue,
                new Rectangle(570, 17, 105, 18), true);

            DrawText(g, "LV " + game.CurrentLevel, 20,
                Color.Gold,
                new Rectangle(570, 35, 105, 30), true);
        }

        // =====================================
        // EFFECTS
        // =====================================

        private void DrawCoinEffect(Graphics g)
        {
            if (coinEffectTime <= 0 || !effectsEnabled)
                return;

            int alpha = (int)(255 * coinEffectTime / 0.8f);
            alpha = Math.Max(0, Math.Min(255, alpha));

            DrawText(g, "+1 COIN!", 18,
                Color.FromArgb(alpha, 255, 220, 80),
                new Rectangle(280, 105, 240, 40), true);
        }

        private void DrawLevelEffect(Graphics g)
        {
            if (levelEffectTime <= 0)
                return;

            int alpha = (int)(255 * levelEffectTime / 1.3f);
            alpha = Math.Max(0, Math.Min(255, alpha));

            DrawText(g, "LEVEL UP!", 22,
                Color.FromArgb(alpha, 100, 230, 255),
                new Rectangle(250, 145, 300, 45), true);
        }

        // =====================================
        // UI HELPERS
        // =====================================

        private void DrawPanel(Graphics g, Rectangle rect)
        {
            Rectangle shadowRect = new Rectangle(
                rect.X + 5,
                rect.Y + 7,
                rect.Width,
                rect.Height
            );

            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(100, 0, 0, 0)))
            using (SolidBrush panel = new SolidBrush(
                Color.FromArgb(240, 9, 17, 43)))
            using (Pen outerGlow = new Pen(
                Color.FromArgb(25, 70, 190, 255), 7))
            using (Pen border = new Pen(
                Color.FromArgb(150, 65, 180, 235), 1.5f))
            {
                g.FillRectangle(shadow, shadowRect);
                g.FillRectangle(panel, rect);
                g.DrawRectangle(outerGlow, rect);
                g.DrawRectangle(border, rect);
            }

            using (Pen topLine = new Pen(
                Color.FromArgb(130, 50, 220, 255), 2))
            {
                g.DrawLine(
                    topLine,
                    rect.X + 12,
                    rect.Y + 2,
                    rect.Right - 12,
                    rect.Y + 2
                );
            }
        }

        private void DrawButton(
            Graphics g,
            Rectangle rect,
            string text,
            Color color)
        {
            bool hover = rect.Contains(mousePosition);

            Color buttonColor = hover
                ? ControlPaint.Light(color, 0.25f)
                : color;

            Rectangle drawRect = hover
                ? new Rectangle(
                    rect.X - 2,
                    rect.Y - 2,
                    rect.Width + 4,
                    rect.Height + 4)
                : rect;

            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(100, 0, 0, 0)))
            using (SolidBrush brush = new SolidBrush(buttonColor))
            using (Pen glow = new Pen(
                Color.FromArgb(hover ? 100 : 35, NeonCyan),
                hover ? 7 : 4))
            using (Pen border = new Pen(
                hover ? Color.White : Color.FromArgb(130, 100, 200, 240),
                hover ? 2 : 1))
            {
                g.FillRectangle(
                    shadow,
                    rect.X + 3,
                    rect.Y + 4,
                    rect.Width,
                    rect.Height
                );

                g.FillRectangle(brush, drawRect);
                g.DrawRectangle(glow, drawRect);
                g.DrawRectangle(border, drawRect);
            }

            DrawText(g, text, 13, Color.White, drawRect, true);
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
                "Arial",
                size,
                FontStyle.Bold))
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

        // =====================================
        // FORM CLOSING
        // =====================================

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            gameTimer.Stop();
            gameTimer.Dispose();

            base.OnFormClosed(e);
        }
    }
}
