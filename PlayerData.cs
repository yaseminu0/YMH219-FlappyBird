
using System;
using System.IO;

namespace YMH219_FlappyBird
{
    public static class PlayerData
    {
        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                ),
                "YMH219_FlappyBird"
            );

        private static readonly string FilePath =
            Path.Combine(FolderPath, "playerdata.txt");

        public static void Load(out int bestScore, out int totalCoins)
        {
            bestScore = 0;
            totalCoins = 0;

            try
            {
                if (!File.Exists(FilePath))
                    return;

                string[] lines = File.ReadAllLines(FilePath);

                if (lines.Length >= 1)
                    int.TryParse(lines[0], out bestScore);

                if (lines.Length >= 2)
                    int.TryParse(lines[1], out totalCoins);

                bestScore = Math.Max(0, bestScore);
                totalCoins = Math.Max(0, totalCoins);
            }
            catch (IOException)
            {
                bestScore = 0;
                totalCoins = 0;
            }
            catch (UnauthorizedAccessException)
            {
                bestScore = 0;
                totalCoins = 0;
            }
        }

        public static void Save(int bestScore, int totalCoins)
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                string[] lines =
                {
                    Math.Max(0, bestScore).ToString(),
                    Math.Max(0, totalCoins).ToString()
                };

                string temporaryFile = FilePath + ".tmp";

                File.WriteAllLines(temporaryFile, lines);

                File.Move(temporaryFile, FilePath, true);
            }
            catch (IOException)
            {
                // Keep the game running if saving fails.
            }
            catch (UnauthorizedAccessException)
            {
                // Keep the game running if access is denied.
            }
        }
    }
}
