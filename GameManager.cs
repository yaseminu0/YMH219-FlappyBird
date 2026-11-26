
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

        private const int InitialPipeSpeed = 4;
        private const int MaximumPipeSpeed = 7;

        private const int InitialPipeSpacing = 400;
        private const int MinimumPipeSpacing = 330;

        private const int ScoresPerLevel = 5;
        private const int MaximumLevel = 8;

        private int currentPipeSpeed;
        private int currentPipeSpacing;

        public int CurrentLevel =>
            Math.Min((Score / ScoresPerLevel) + 1, MaximumLevel);

        public int CurrentPipeSpeed => currentPipeSpeed;

        public string DifficultyName
        {
            get
            {
                if (CurrentLevel <= 1)
                    return "EASY";

                if (CurrentLevel <= 3)
                    return "NORMAL";

                if (CurrentLevel <= 5)
                    return "HARD";

                return "EXPERT";
            }
        }

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

            currentPipeSpeed = InitialPipeSpeed;
            currentPipeSpacing = InitialPipeSpacing;

            Started = false;
            GameOver = false;
            Paused = false;
        }

        public void Update()
        {
            if (!Started || GameOver || Paused)
                return;

            UpdateDifficulty();

            Bird.Update();

            Pipe1.Move(currentPipeSpeed);
            Pipe2.Move(currentPipeSpeed);

            Coin1.Move(currentPipeSpeed);
            Coin2.Move(currentPipeSpeed);

            if (Pipe1.TryScore(Bird.Bounds))
            {
                Score++;
                UpdateBestScore();
                UpdateDifficulty();
            }

            if (Pipe2.TryScore(Bird.Bounds))
            {
                Score++;
                UpdateBestScore();
                UpdateDifficulty();
            }

            if (Coin1.TryCollect(Bird.Bounds))
                CollectCoin();

            if (Coin2.TryCollect(Bird.Bounds))
                CollectCoin();

            if (Pipe1.IsOffScreen())
            {
                int newX = Math.Max(Pipe1.X, Pipe2.X)
                           + currentPipeSpacing;

                Pipe1.ResetPosition(newX);
                Coin1.ResetForPipe(Pipe1);
            }

            if (Pipe2.IsOffScreen())
            {
                int newX = Math.Max(Pipe1.X, Pipe2.X)
                           + currentPipeSpacing;

                Pipe2.ResetPosition(newX);
                Coin2.ResetForPipe(Pipe2);
            }

            CheckCollisions();
        }

        private void UpdateDifficulty()
        {
            int level = CurrentLevel - 1;

            currentPipeSpeed = Math.Min(
                InitialPipeSpeed + level / 2,
                MaximumPipeSpeed
            );

            currentPipeSpacing = Math.Max(
                InitialPipeSpacing - level * 10,
                MinimumPipeSpacing
            );

            int gapSize = Math.Max(
                170 - level * 5,
                135
            );

            Pipe.SetDifficultyGap(gapSize);
        }

        private void CheckCollisions()
        {
            bool hitCeiling = Bird.Bounds.Y < 0;
            bool hitGround = Bird.Bounds.Bottom > Pipe.GroundY;

            bool hitPipe =
                Pipe1.CollidesWith(Bird.Bounds) ||
                Pipe2.CollidesWith(Bird.Bounds);

            if (hitCeiling || hitGround || hitPipe)
            {
                GameOver = true;
                Paused = false;
                SaveData();
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
            if (GameOver || Paused)
                return;

            Started = true;
            Bird.Jump();
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
            if (Paused && !GameOver)
                Paused = false;
        }

        public void Restart()
        {
            Bird.Reset();

            Pipe.SetDifficultyGap(170);

            Pipe1 = new Pipe(800);
            Pipe2 = new Pipe(1200);

            Coin1 = new Coin(Pipe1);
            Coin2 = new Coin(Pipe2);

            Score = 0;
            RunCoins = 0;

            currentPipeSpeed = InitialPipeSpeed;
            currentPipeSpacing = InitialPipeSpacing;

            GameOver = false;
            Paused = false;
            Started = false;
        }
    }
}
