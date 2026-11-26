
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

        private void DrawPipePart(
            Graphics g,
            Rectangle rect,
            bool isTop)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            Color dark = Color.FromArgb(5, 14, 38);
            Color blue = Color.FromArgb(12, 55, 105);
            Color cyan = Color.FromArgb(45, 225, 255);
            Color purple = Color.FromArgb(135, 85, 255);

            using (LinearGradientBrush body =
                new LinearGradientBrush(
                    rect,
                    blue,
                    dark,
                    LinearGradientMode.Horizontal))
            {
                g.FillRectangle(body, rect);
            }

            // Outer neon glow
            using (Pen glow = new Pen(
                Color.FromArgb(35, cyan), 10))
            using (Pen border = new Pen(
                Color.FromArgb(220, cyan), 2))
            {
                g.DrawRectangle(glow, rect);
                g.DrawRectangle(border, rect);
            }

            // Metallic side panels
            DrawSidePanels(g, rect, cyan, purple);

            // Futuristic details
            DrawTechDetails(g, rect);

            // Energy gate cap
            DrawCap(g, rect, isTop);
        }

        private void DrawSidePanels(
            Graphics g,
            Rectangle rect,
            Color cyan,
            Color purple)
        {
            if (rect.Height < 35)
                return;

            using (Pen leftLine = new Pen(
                Color.FromArgb(190, cyan), 2))
            using (Pen rightLine = new Pen(
                Color.FromArgb(170, purple), 2))
            {
                g.DrawLine(
                    leftLine,
                    rect.X + 10,
                    rect.Y + 8,
                    rect.X + 10,
                    rect.Bottom - 8
                );

                g.DrawLine(
                    rightLine,
                    rect.Right - 10,
                    rect.Y + 8,
                    rect.Right - 10,
                    rect.Bottom - 8
                );
            }
        }

        private void DrawTechDetails(
            Graphics g,
            Rectangle rect)
        {
            if (rect.Height < 100)
                return;

            for (int y = rect.Y + 55;
                 y + 25 < rect.Bottom - 25;
                 y += 90)
            {
                Rectangle panel = new Rectangle(
                    rect.X + 19,
                    y,
                    32,
                    28
                );

                using (SolidBrush panelBrush =
                    new SolidBrush(Color.FromArgb(225, 4, 12, 34)))
                using (Pen border =
                    new Pen(Color.FromArgb(180, 90, 190, 255), 1))
                using (Pen light =
                    new Pen(Color.FromArgb(220, 50, 230, 255), 2))
                {
                    g.FillRectangle(panelBrush, panel);
                    g.DrawRectangle(border, panel);

                    g.DrawLine(
                        light,
                        panel.X + 6,
                        panel.Y + 8,
                        panel.Right - 6,
                        panel.Y + 8
                    );

                    g.DrawLine(
                        light,
                        panel.X + 6,
                        panel.Y + 19,
                        panel.Right - 6,
                        panel.Y + 19
                    );
                }
            }
        }

        private void DrawCap(
            Graphics g,
            Rectangle rect,
            bool isTop)
        {
            const int capHeight = 20;

            Rectangle cap = isTop
                ? new Rectangle(
                    rect.X - 8,
                    rect.Bottom - capHeight,
                    rect.Width + 16,
                    capHeight
                )
                : new Rectangle(
                    rect.X - 8,
                    rect.Y,
                    rect.Width + 16,
                    capHeight
                );

            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    cap,
                    Color.FromArgb(20, 75, 145),
                    Color.FromArgb(45, 205, 235),
                    LinearGradientMode.Vertical))
            using (Pen glow =
                new Pen(Color.FromArgb(65, 35, 220, 255), 8))
            using (Pen outline =
                new Pen(Color.FromArgb(245, 125, 245, 255), 2))
            {
                g.FillRectangle(brush, cap);
                g.DrawRectangle(glow, cap);
                g.DrawRectangle(outline, cap);
            }

            using (Pen energy =
                new Pen(Color.FromArgb(235, 225, 255, 255), 2))
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
        }
    }
}
