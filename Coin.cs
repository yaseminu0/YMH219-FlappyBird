
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public class Coin
    {
        public const int Size = 28;

        private static readonly Random random = new Random();

        private int x;
        private int y;

        private float rotation;
        private float pulse;

        private bool collected;

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    x,
                    y,
                    Size,
                    Size
                );
            }
        }

        public bool IsCollected => collected;

        public Coin(Pipe pipe)
        {
            ResetForPipe(pipe);
        }

        public void Move(int speed)
        {
            if (collected)
                return;

            x -= speed;

            rotation += 0.13f;
            if (rotation >= Math.PI * 2)
                rotation -= (float)(Math.PI * 2);

            pulse += 0.09f;
            if (pulse >= Math.PI * 2)
                pulse -= (float)(Math.PI * 2);
        }

        public bool TryCollect(Rectangle birdBounds)
        {
            if (collected)
                return false;

            Rectangle expandedCoin = Rectangle.Inflate(
                Bounds,
                4,
                4
            );

            if (expandedCoin.IntersectsWith(birdBounds))
            {
                collected = true;
                return true;
            }

            return false;
        }

        public void ResetForPipe(Pipe pipe)
        {
            x = pipe.X + Pipe.PipeWidth / 2 - Size / 2;

            int minimumY = pipe.GapTop + 25;
            int maximumY = pipe.GapBottom - Size - 25;

            if (maximumY < minimumY)
                maximumY = minimumY;

            y = random.Next(
                minimumY,
                maximumY + 1
            );

            rotation = 0;
            pulse = 0;
            collected = false;
        }

        public void Draw(Graphics g)
        {
            if (collected)
                return;

            GraphicsState state = g.Save();

            g.SmoothingMode = SmoothingMode.AntiAlias;

            float centerX = x + Size / 2f;
            float centerY = y + Size / 2f;

            float glowPulse =
                0.5f + 0.5f * (float)Math.Sin(pulse);

            // Glow around the coin
            int glowAlpha = 35 + (int)(35 * glowPulse);

            using (SolidBrush glowBrush =
                new SolidBrush(
                    Color.FromArgb(
                        glowAlpha,
                        255,
                        190,
                        35
                    )))
            {
                float glowSize = Size + 16 + glowPulse * 5;

                g.FillEllipse(
                    glowBrush,
                    centerX - glowSize / 2,
                    centerY - glowSize / 2,
                    glowSize,
                    glowSize
                );
            }

            g.TranslateTransform(centerX, centerY);

            // Simulated 3D rotation
            float horizontalScale =
                Math.Abs((float)Math.Cos(rotation));

            horizontalScale = Math.Max(0.12f, horizontalScale);

            g.ScaleTransform(horizontalScale, 1f);

            // Coin shadow
            using (SolidBrush shadow =
                new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
            {
                g.FillEllipse(
                    shadow,
                    -Size / 2f + 2,
                    -Size / 2f + 4,
                    Size,
                    Size
                );
            }

            // Main golden body
            RectangleF coinRect = new RectangleF(
                -Size / 2f,
                -Size / 2f,
                Size,
                Size
            );

            using (LinearGradientBrush gold =
                new LinearGradientBrush(
                    coinRect,
                    Color.FromArgb(255, 245, 125),
                    Color.FromArgb(190, 105, 12),
                    LinearGradientMode.ForwardDiagonal))
            {
                gold.InterpolationColors =
                    new ColorBlend
                    {
                        Colors = new[]
                        {
                            Color.FromArgb(255, 250, 180),
                            Color.FromArgb(255, 205, 55),
                            Color.FromArgb(225, 145, 20),
                            Color.FromArgb(255, 225, 95)
                        },
                        Positions = new[]
                        {
                            0f,
                            0.35f,
                            0.7f,
                            1f
                        }
                    };

                g.FillEllipse(gold, coinRect);
            }

            // Outer edge
            using (Pen outerBorder =
                new Pen(
                    Color.FromArgb(255, 255, 225, 130),
                    2.5f))
            using (Pen innerBorder =
                new Pen(
                    Color.FromArgb(210, 145, 80, 10),
                    1.2f))
            {
                g.DrawEllipse(outerBorder, coinRect);

                RectangleF innerRect =
                    new RectangleF(
                        -Size / 2f + 4,
                        -Size / 2f + 4,
                        Size - 8,
                        Size - 8
                    );

                g.DrawEllipse(innerBorder, innerRect);
            }

            // Center star
            using (SolidBrush starBrush =
                new SolidBrush(
                    Color.FromArgb(255, 255, 250, 205)))
            using (Pen starOutline =
                new Pen(
                    Color.FromArgb(255, 180, 105, 15),
                    1f))
            {
                PointF[] star = CreateStar(
                    0,
                    0,
                    8,
                    3.5f,
                    5
                );

                g.FillPolygon(starBrush, star);
                g.DrawPolygon(starOutline, star);
            }

            // Small shine
            using (SolidBrush shine =
                new SolidBrush(
                    Color.FromArgb(190, 255, 255, 235)))
            {
                g.FillEllipse(
                    shine,
                    -8,
                    -9,
                    5,
                    3
                );
            }

            g.Restore(state);
        }

        private PointF[] CreateStar(
            float centerX,
            float centerY,
            float outerRadius,
            float innerRadius,
            int points)
        {
            PointF[] result = new PointF[points * 2];

            for (int i = 0; i < points * 2; i++)
            {
                float angle =
                    (float)(i * Math.PI / points)
                    - (float)(Math.PI / 2);

                float radius =
                    i % 2 == 0
                    ? outerRadius
                    : innerRadius;

                result[i] = new PointF(
                    centerX + (float)Math.Cos(angle) * radius,
                    centerY + (float)Math.Sin(angle) * radius
                );
            }

            return result;
        }
    }
}
