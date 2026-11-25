
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
        public Rectangle Bounds { get; private set; }

        public BirdType SelectedType { get; private set; }

        private int velocity;

        private const int Gravity = 1;
        private const int JumpPower = -12;

        public Bird()
        {
            SelectedType = BirdType.Yellow;
            Bounds = new Rectangle(100, 200, 46, 38);
        }

        public void SetType(BirdType type)
        {
            SelectedType = type;
        }

        public void Jump()
        {
            velocity = JumpPower;
        }

        public void Dive()
        {
            velocity = 8;
        }

        public void Update()
        {
            velocity += Gravity;

            Bounds = new Rectangle(
                Bounds.X,
                Bounds.Y + velocity,
                Bounds.Width,
                Bounds.Height);
        }

        public void Reset()
        {
            velocity = 0;

            Bounds = new Rectangle(
                100, 200, 46, 38);
        }

        public void Draw(Graphics g)
        {
            DrawAt(g, Bounds.X, Bounds.Y, 1f);
        }

        public void DrawAt(
            Graphics g,
            int x,
            int y,
            float scale = 1f)
        {
            int w = (int)(46 * scale);
            int h = (int)(38 * scale);

            Color bodyColor = Color.Gold;

            switch (SelectedType)
            {
                case BirdType.Yellow:
                    bodyColor = Color.Gold;
                    break;

                case BirdType.Blue:
                    bodyColor = Color.DeepSkyBlue;
                    break;

                case BirdType.Red:
                    bodyColor = Color.OrangeRed;
                    break;

                case BirdType.Green:
                    bodyColor = Color.LimeGreen;
                    break;
            }

            using (SolidBrush bodyBrush =
                   new SolidBrush(bodyColor))
            using (SolidBrush wingBrush =
                   new SolidBrush(ControlPaint.Dark(bodyColor)))
            using (SolidBrush whiteBrush =
                   new SolidBrush(Color.White))
            using (SolidBrush blackBrush =
                   new SolidBrush(Color.Black))
            using (SolidBrush beakBrush =
                   new SolidBrush(Color.Orange))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.FillEllipse(
                    bodyBrush,
                    x,
                    y,
                    w,
                    h);

                g.FillEllipse(
                    wingBrush,
                    x + (int)(5 * scale),
                    y + (int)(19 * scale),
                    (int)(21 * scale),
                    (int)(12 * scale));

                g.FillEllipse(
                    whiteBrush,
                    x + (int)(29 * scale),
                    y + (int)(6 * scale),
                    (int)(12 * scale),
                    (int)(12 * scale));

                g.FillEllipse(
                    blackBrush,
                    x + (int)(35 * scale),
                    y + (int)(10 * scale),
                    (int)(5 * scale),
                    (int)(5 * scale));

                Point[] beak =
                {
                    new Point(
                        x + (int)(w - 2 * scale),
                        y + (int)(18 * scale)),

                    new Point(
                        x + (int)(w + 9 * scale),
                        y + (int)(22 * scale)),

                    new Point(
                        x + (int)(w - 2 * scale),
                        y + (int)(27 * scale))
                };

                g.FillPolygon(beakBrush, beak);
            }
        }
    }
}
