
using System;
using System.Drawing;

namespace YMH219_FlappyBird
{
    public class Pipe
    {
        public int X { get; private set; }

        public Rectangle TopBounds { get; private set; }
        public Rectangle BottomBounds { get; private set; }
        public Rectangle GapBounds { get; private set; }

        public int Width => PipeWidth;

        private const int PipeWidth = 70;
        private const int GapSize = 170;
        private const int GroundY = 510;

        private bool scored;

        private static readonly Random random = new Random();

        public Pipe(int startX)
        {
            X = startX;
            scored = false;
            GeneratePosition();
        }

        private void GeneratePosition()
        {
            int gapTop = random.Next(90, 250);
            int gapBottom = gapTop + GapSize;

            TopBounds = new Rectangle(
                X, 0, PipeWidth, gapTop);

            BottomBounds = new Rectangle(
                X,
                gapBottom,
                PipeWidth,
                GroundY - gapBottom);

            GapBounds = new Rectangle(
                X,
                gapTop,
                PipeWidth,
                GapSize);
        }

        public void Move(int speed)
        {
            X -= speed;

            TopBounds = new Rectangle(
                X,
                TopBounds.Y,
                TopBounds.Width,
                TopBounds.Height);

            BottomBounds = new Rectangle(
                X,
                BottomBounds.Y,
                BottomBounds.Width,
                BottomBounds.Height);

            GapBounds = new Rectangle(
                X,
                GapBounds.Y,
                GapBounds.Width,
                GapBounds.Height);
        }

        public bool CollidesWith(Rectangle bird)
        {
            return TopBounds.IntersectsWith(bird) ||
                   BottomBounds.IntersectsWith(bird);
        }

        public bool TryScore(Rectangle bird)
        {
            if (!scored && bird.Left > X + PipeWidth)
            {
                scored = true;
                return true;
            }

            return false;
        }

        public bool IsOffScreen()
        {
            return X + PipeWidth < 0;
        }

        public void ResetPosition(int newX)
        {
            X = newX;
            scored = false;
            GeneratePosition();
        }
    }
}
