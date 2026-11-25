
using System;
using System.IO;

namespace YMH219_FlappyBird
{
    public static class PlayerData
    {
        private static readonly string Folder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "YMH219_FlappyBird");

        private static readonly string FilePath =
            Path.Combine(Folder, "playerdata.txt");

        public static void Load(
            out int bestScore,
            out int totalCoins)
        {
            bestScore = 0;
            totalCoins = 0;

            try
            {
                if (!File.Exists(FilePath))
                    return;

                string[] lines = File.ReadAllLines(FilePath);

                if (lines.Length > 0)
                    int.TryParse(lines[0], out bestScore);

                if (lines.Length > 1)
                    int.TryParse(lines[1], out totalCoins);
            }
            catch
            {
                bestScore = 0;
                totalCoins = 0;
            }
        }

        public static void Save(
            int bestScore,
            int totalCoins)
        {
            try
            {
                Directory.CreateDirectory(Folder);

                File.WriteAllLines(FilePath, new[]
                {
                    bestScore.ToString(),
                    totalCoins.ToString()
                });
            }
            catch
            {
                // اللعبة تستمر حتى لو تعذر الحفظ.
            }
        }
    }
}
