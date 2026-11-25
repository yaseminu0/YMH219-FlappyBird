
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YMH219_FlappyBird
{
    public class Coin
    {
        public Rectangle Bounds { get; private set; }
        public bool Active { get; private set; }

        private const int Size = 24;

        public Coin(Pipe pipe)
        {
            ResetForPipe(pipe);
        }

        public void ResetForPipe(Pipe pipe)
        {
            Bounds = new Rectangle(
                pipe.X + pipe.Width / 2 - Size / 2,
                pipe.GapBounds.Y + pipe.GapBounds.Height / 2 - Size / 2,
                Size,
                Size);

            Active = true;
        }

        public void Move(int speed)
        {
            if (!Active)
                return;

            Bounds = new Rectangle(
                Bounds.X - speed,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height);
        }

        public bool TryCollect(Rectangle bird)
        {
            if (!Active || !Bounds.IntersectsWith(bird))
                return false;

            Active = false;
            return true;
        }

        public void Draw(Graphics g)
        {
            if (!Active)
                return;

            using (SolidBrush glow = new SolidBrush(
                Color.FromArgb(55, 255, 205, 50)))
            using (SolidBrush outer = new SolidBrush(
                Color.FromArgb(220, 155, 25)))
            using (SolidBrush inner = new SolidBrush(
                Color.FromArgb(255, 210, 65)))
            using (SolidBrush shine = new SolidBrush(
                Color.FromArgb(255, 245, 170)))
            using (Pen outline = new Pen(
                Color.FromArgb(255, 240, 150), 2))
            {
                g.FillEllipse(
                    glow,
                    Bounds.X - 7,
                    Bounds.Y - 7,
                    Bounds.Width + 14,
                    Bounds.Height + 14);

                g.FillEllipse(outer, Bounds);
                g.DrawEllipse(outline, Bounds);

                Rectangle innerRect = new Rectangle(
                    Bounds.X + 4,
                    Bounds.Y + 4,
                    Bounds.Width - 8,
                    Bounds.Height - 8);

                g.FillEllipse(inner, innerRect);

                g.FillEllipse(
                    shine,
                    Bounds.X + 6,
                    Bounds.Y + 4,
                    5,
                    5);
            }

            using (Font font = new Font(
                "Segoe UI", 12, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(
                Color.FromArgb(145, 85, 15)))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                g.DrawString(
                    "$",
                    font,
                    brush,
                    Bounds,
                    format);
            }
        }
    }
}
