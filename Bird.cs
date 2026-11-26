
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public enum BirdType
    {
        Yellow,
        Blue,
        Red,
        Green
    }

    public class Bird
    {
        private float velocity;
        private float wingPhase;
        private float rotation;

        private const float Gravity = 0.32f;
        private const float JumpForce = -7.2f;
        private const float MaximumFallSpeed = 7.2f;

        private const int BirdWidth = 48;
        private const int BirdHeight = 40;

        public Rectangle Bounds { get; private set; }

        public BirdType Type { get; private set; }

        public BirdType SelectedType => Type;

        public Bird()
        {
            Type = BirdType.Yellow;

            Bounds = new Rectangle(
                145, 230,
                BirdWidth, BirdHeight
            );

            velocity = 0;
            wingPhase = 0;
            rotation = 0;
        }

        public void SetType(BirdType type)
        {
            Type = type;
        }

        public void Jump()
        {
            velocity = JumpForce;
        }

        public void Dive()
        {
            velocity = Math.Min(
                velocity + 2.8f,
                MaximumFallSpeed
            );
        }

        public void Update()
        {
            velocity += Gravity;

            velocity = Math.Min(
                velocity,
                MaximumFallSpeed
            );

            int newY = Bounds.Y + (int)Math.Round(velocity);

            Bounds = new Rectangle(
                Bounds.X,
                newY,
                BirdWidth,
                BirdHeight
            );

            wingPhase += 0.28f;

            float targetRotation = velocity * 4.2f;

            targetRotation = Math.Max(
                -25,
                Math.Min(55, targetRotation)
            );

            rotation += (targetRotation - rotation) * 0.12f;
        }

        public void Reset()
        {
            Bounds = new Rectangle(
                145, 230,
                BirdWidth, BirdHeight
            );

            velocity = 0;
            wingPhase = 0;
            rotation = 0;
        }

        public void Draw(Graphics g)
        {
            DrawAt(
                g,
                Bounds,
                rotation
            );
        }

        public void DrawAt(Graphics g, Rectangle rect)
        {
            DrawAt(g, rect, 0);
        }

        public void DrawAt(
            Graphics g,
            Rectangle rect,
            float angle)
        {
            float scale = rect.Width / (float)BirdWidth;

            DrawAt(
                g,
                rect.X + rect.Width / 2,
                rect.Y + rect.Height / 2,
                scale,
                angle
            );
        }

        public void DrawAt(
            Graphics g,
            int centerX,
            int centerY,
            float scale)
        {
            DrawAt(
                g,
                centerX,
                centerY,
                scale,
                0
            );
        }

        private void DrawAt(
            Graphics g,
            int centerX,
            int centerY,
            float scale,
            float angle)
        {
            GraphicsState state = g.Save();

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TranslateTransform(centerX, centerY);
            g.ScaleTransform(scale, scale);
            g.RotateTransform(angle);

            Color mainColor;
            Color lightColor;
            Color darkColor;

            GetColors(
                out mainColor,
                out lightColor,
                out darkColor
            );

            DrawShadow(g);
            DrawTail(g, darkColor);
            DrawWing(g, mainColor, lightColor, darkColor);
            DrawBody(g, mainColor, lightColor, darkColor);
            DrawBeak(g);
            DrawEyes(g);
            DrawBodyHighlights(g, lightColor);

            g.Restore(state);
        }

        private void GetColors(
            out Color main,
            out Color light,
            out Color dark)
        {
            switch (Type)
            {
                case BirdType.Blue:
                    main = Color.FromArgb(40, 160, 245);
                    light = Color.FromArgb(125, 220, 255);
                    dark = Color.FromArgb(15, 75, 165);
                    break;

                case BirdType.Red:
                    main = Color.FromArgb(235, 75, 90);
                    light = Color.FromArgb(255, 155, 145);
                    dark = Color.FromArgb(155, 35, 65);
                    break;

                case BirdType.Green:
                    main = Color.FromArgb(55, 205, 130);
                    light = Color.FromArgb(150, 255, 190);
                    dark = Color.FromArgb(20, 115, 75);
                    break;

                default:
                    main = Color.FromArgb(255, 195, 45);
                    light = Color.FromArgb(255, 235, 125);
                    dark = Color.FromArgb(205, 115, 20);
                    break;
            }
        }

        private void DrawShadow(Graphics g)
        {
            using (SolidBrush shadow = new SolidBrush(
                Color.FromArgb(45, 0, 0, 0)))
            {
                g.FillEllipse(
                    shadow,
                    -18, 15,
                    39, 12
                );
            }

            using (SolidBrush glow = new SolidBrush(
                Color.FromArgb(28, 100, 220, 255)))
            {
                g.FillEllipse(
                    glow,
                    -24, -22,
                    48, 45
                );
            }
        }

        private void DrawTail(Graphics g, Color dark)
        {
            Point[] tail =
            {
                new Point(-17, 4),
                new Point(-27, -2),
                new Point(-24, 8),
                new Point(-30, 13),
                new Point(-15, 13)
            };

            using (SolidBrush brush = new SolidBrush(dark))
            using (Pen outline = new Pen(
                Color.FromArgb(100, 75, 35), 1.2f))
            {
                g.FillPolygon(brush, tail);
                g.DrawPolygon(outline, tail);
            }
        }

        private void DrawWing(
            Graphics g,
            Color main,
            Color light,
            Color dark)
        {
            float wingAngle =
                (float)Math.Sin(wingPhase) * 18f;

            GraphicsState state = g.Save();

            g.TranslateTransform(-5, 5);
            g.RotateTransform(wingAngle);

            Rectangle wingRect = new Rectangle(
                -17, -1,
                25, 19
            );

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(wingRect);

                using (PathGradientBrush wing =
                    new PathGradientBrush(path))
                {
                    wing.CenterColor = light;
                    wing.SurroundColors = new Color[] { dark };

                    g.FillPath(wing, path);
                }

                using (Pen outline = new Pen(dark, 1.4f))
                {
                    g.DrawPath(outline, path);
                }
            }

            using (Pen feather = new Pen(
                Color.FromArgb(160, light), 2))
            {
                g.DrawArc(
                    feather,
                    -13, 3,
                    17, 11,
                    190, 130
                );

                g.DrawArc(
                    feather,
                    -10, 6,
                    12, 8,
                    190, 120
                );
            }

            g.Restore(state);
        }

        private void DrawBody(
            Graphics g,
            Color main,
            Color light,
            Color dark)
        {
            Rectangle bodyRect = new Rectangle(
                -21, -17,
                43, 35
            );

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(bodyRect);

                using (PathGradientBrush body =
                    new PathGradientBrush(path))
                {
                    body.CenterColor = light;
                    body.SurroundColors = new Color[] { dark };

                    g.FillPath(body, path);
                }

                using (Pen outline = new Pen(
                    Color.FromArgb(130, 70, 45, 25), 1.5f))
                {
                    g.DrawPath(outline, path);
                }
            }

            using (SolidBrush belly = new SolidBrush(
                Color.FromArgb(115, 255, 250, 220)))
            {
                g.FillEllipse(
                    belly,
                    -13, 3,
                    26, 17
                );
            }
        }

        private void DrawBeak(Graphics g)
        {
            Point[] beak =
            {
                new Point(15, -2),
                new Point(29, 2),
                new Point(16, 9),
                new Point(11, 5)
            };

            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    new Rectangle(11, -2, 18, 12),
                    Color.FromArgb(255, 225, 95),
                    Color.FromArgb(230, 100, 15),
                    LinearGradientMode.Vertical))
            using (Pen outline = new Pen(
                Color.FromArgb(160, 85, 20), 1.2f))
            {
                g.FillPolygon(brush, beak);
                g.DrawPolygon(outline, beak);
            }

            using (Pen shine = new Pen(
                Color.FromArgb(180, 255, 245, 180), 1.5f))
            {
                g.DrawLine(
                    shine,
                    17, 0,
                    24, 2
                );
            }
        }

        private void DrawEyes(Graphics g)
        {
            // White eye backgrounds
            using (SolidBrush white = new SolidBrush(
                Color.White))
            using (Pen outline = new Pen(
                Color.FromArgb(70, 55, 40), 1.2f))
            {
                g.FillEllipse(
                    white,
                    -7, -12,
                    13, 18
                );

                g.FillEllipse(
                    white,
                    5, -12,
                    13, 18
                );

                g.DrawEllipse(
                    outline,
                    -7, -12,
                    13, 18
                );

                g.DrawEllipse(
                    outline,
                    5, -12,
                    13, 18
                );
            }

            // Pupils
            using (SolidBrush pupil = new SolidBrush(
                Color.FromArgb(20, 25, 40)))
            {
                g.FillEllipse(
                    pupil,
                    -3, -7,
                    7, 11
                );

                g.FillEllipse(
                    pupil,
                    9, -7,
                    7, 11
                );
            }

            // Eye reflections
            using (SolidBrush shine = new SolidBrush(
                Color.White))
            {
                g.FillEllipse(
                    shine,
                    -2, -6,
                    3, 4
                );

                g.FillEllipse(
                    shine,
                    10, -6,
                    3, 4
                );
            }
        }

        private void DrawBodyHighlights(
            Graphics g,
            Color light)
        {
            using (SolidBrush highlight = new SolidBrush(
                Color.FromArgb(125, light)))
            {
                g.FillEllipse(
                    highlight,
                    -15, -14,
                    13, 7
                );
            }

            using (Pen shine = new Pen(
                Color.FromArgb(100, Color.White), 1.5f))
            {
                g.DrawArc(
                    shine,
                    -16, -15,
                    29, 26,
                    205, 100
                );
            }
        }
    }
}
