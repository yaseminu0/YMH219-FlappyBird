
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public class Pipe
    {
        public const int PipeWidth = 70;
        public const int GroundY = 510;

        public static int Gap { get; private set; } = 170;

        private static readonly Random random = new Random();

        public int X { get; private set; }

        public int GapTop { get; private set; }

        public int GapBottom => GapTop + Gap;

        private bool scored;

        public Rectangle TopBounds =>
            new Rectangle(X, 0, PipeWidth, GapTop);

        public Rectangle BottomBounds =>
            new Rectangle(
                X,
                GapBottom,
                PipeWidth,
                Math.Max(0, GroundY - GapBottom)
            );

        public Pipe(int startX)
        {
            X = startX;
            scored = false;
            GenerateGap();
        }

        public static void SetDifficultyGap(int gapSize)
        {
            Gap = Math.Max(135, Math.Min(200, gapSize));
        }

        private void GenerateGap()
        {
            int minimumTop = 85;
            int maximumTop = GroundY - Gap - 85;

            if (maximumTop < minimumTop)
                maximumTop = minimumTop;

            GapTop = random.Next(
                minimumTop,
                maximumTop + 1
            );
        }

        public void Move(int speed)
        {
            X -= speed;
        }

        public bool TryScore(Rectangle birdBounds)
        {
            if (scored)
                return false;

            if (X + PipeWidth < birdBounds.Left)
            {
                scored = true;
                return true;
            }

            return false;
        }

        public bool CollidesWith(Rectangle birdBounds)
        {
            return TopBounds.IntersectsWith(birdBounds) ||
                   BottomBounds.IntersectsWith(birdBounds);
        }

        public bool IsOffScreen()
        {
            return X + PipeWidth < 0;
        }

        public void ResetPosition(int newX)
        {
            X = newX;
            scored = false;
            GenerateGap();
        }

        public void Draw(Graphics g)
        {
            DrawPipePart(g, TopBounds, true);
            DrawPipePart(g, BottomBounds, false);
        }

        private void DrawPipePart(Graphics g, Rectangle rect, bool isTop)
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
    }
}
