
using System;

namespace YMH219_FlappyBird
{
    public class GameManager
    {
        public Bird Bird { get; private set; }

        public Pipe Pipe1 { get; private set; }
        public Pipe Pipe2 { get; private set; }

        public Coin Coin1 { get; private set; }
        public Coin Coin2 { get; private set; }

        public int Score { get; private set; }
        public int BestScore { get; private set; }

        public int TotalCoins { get; private set; }
        public int RunCoins { get; private set; }

        public bool GameOver { get; private set; }
        public bool Started { get; private set; }
        public bool Paused { get; private set; }

        private const int PipeSpeed = 5;
        private const int PipeSpacing = 400;

        public GameManager()
        {
            Bird = new Bird();

            Pipe1 = new Pipe(800);
            Pipe2 = new Pipe(1200);

            Coin1 = new Coin(Pipe1);
            Coin2 = new Coin(Pipe2);

            PlayerData.Load(out int best, out int coins);

            BestScore = best;
            TotalCoins = coins;

            Started = false;
        }

        public void Update()
        {
            if (!Started || GameOver || Paused)
                return;

            Bird.Update();

            Pipe1.Move(PipeSpeed);
            Pipe2.Move(PipeSpeed);

            Coin1.Move(PipeSpeed);
            Coin2.Move(PipeSpeed);

            if (Pipe1.TryScore(Bird.Bounds))
            {
                Score++;
                UpdateBestScore();
            }

            if (Pipe2.TryScore(Bird.Bounds))
            {
                Score++;
                UpdateBestScore();
            }

            if (Coin1.TryCollect(Bird.Bounds))
                CollectCoin();

            if (Coin2.TryCollect(Bird.Bounds))
                CollectCoin();

            if (Pipe1.IsOffScreen())
            {
                int newX = Math.Max(Pipe1.X, Pipe2.X) + PipeSpacing;

                Pipe1.ResetPosition(newX);
                Coin1.ResetForPipe(Pipe1);
            }

            if (Pipe2.IsOffScreen())
            {
                int newX = Math.Max(Pipe1.X, Pipe2.X) + PipeSpacing;

                Pipe2.ResetPosition(newX);
                Coin2.ResetForPipe(Pipe2);
            }

            if (Bird.Bounds.Y < 0 ||
                Bird.Bounds.Bottom > 510 ||
                Pipe1.CollidesWith(Bird.Bounds) ||
                Pipe2.CollidesWith(Bird.Bounds))
            {
                GameOver = true;
                Paused = false;
            }
        }

        private void UpdateBestScore()
        {
            if (Score > BestScore)
            {
                BestScore = Score;
                SaveData();
            }
        }

        private void CollectCoin()
        {
            RunCoins++;
            TotalCoins++;

            SaveData();
        }

        private void SaveData()
        {
            PlayerData.Save(BestScore, TotalCoins);
        }

        public void Jump()
        {
            if (!GameOver && !Paused)
            {
                Started = true;
                Bird.Jump();
            }
        }

        public void Dive()
        {
            if (Started && !GameOver && !Paused)
                Bird.Dive();
        }

        public void TogglePause()
        {
            if (!Started || GameOver)
                return;

            Paused = !Paused;
        }

        public void Resume()
        {
            if (Paused)
                Paused = false;
        }

        public void Restart()
        {
            Bird.Reset();

            Pipe1 = new Pipe(800);
            Pipe2 = new Pipe(1200);

            Coin1 = new Coin(Pipe1);
            Coin2 = new Coin(Pipe2);

            Score = 0;
            RunCoins = 0;

            GameOver = false;
            Paused = false;
            Started = false;
        }
    }
}
