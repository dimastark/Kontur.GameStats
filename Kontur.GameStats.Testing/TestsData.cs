using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Testing
{
    public static class TestsData
    {
        public static ServerInfo FirstServer = new ServerInfo
        {
            Name = "] My P3rfect Server [",
            GameModes = new[] {"DM", "TDM"}
        };

        public static ServerInfo SecondServer = new ServerInfo
        {
            Name = "SUCH PERFECT SERVER",
            GameModes = new[] { "DM", "TDM", "PVP ILI ZASSAL" }
        };

        public static MatchInfo FirstMatch = new MatchInfo
        {
            Map = "DM-HelloWorld",
            GameMode = "DM",
            FragLimit = 20,
            TimeLimit = 20,
            TimeElapsed = 13.345678,
            Scoreboard = new[]
            {
                new PlayerScore {Name = "Player1", Deaths = 3, Frags = 20, Kills = 21},
                new PlayerScore {Name = "Player2", Deaths = 21, Frags = 2, Kills = 2},
                new PlayerScore {Name = "Player3", Deaths = 10, Frags = 2, Kills = 2}
            }
        };

        public static MatchInfo SecondMatch = new MatchInfo
        {
            Map = "Sayonara-Cruel-World",
            GameMode = "TDM",
            FragLimit = 20,
            TimeLimit = 60 * 60 * 24,
            TimeElapsed = 60 * 60 * 24,
            Scoreboard = new[]
            {
                new PlayerScore {Name = "Player1", Deaths = 3, Frags = 20, Kills = 21}
            }
        };

        public static IEnumerable<PlayerScore> GeneratePlayerScores(int count, bool isDead = true)
        {
            for (var i = 0; i < count; i++)
                yield return new PlayerScore {Deaths = isDead ? 1 : 0, Frags = 0, Kills = 0, Name = $"{i}"};
        }

        public static MatchInfo GenerateMatchInfo(int playersCount, bool isDead = true)
        {
            return new MatchInfo
            {
                Map = "Some", GameMode = "Match",
                FragLimit = 20, TimeLimit = 20, TimeElapsed = 20,
                Scoreboard = GeneratePlayerScores(playersCount, isDead).ToArray()
            };
        }
    }
}
