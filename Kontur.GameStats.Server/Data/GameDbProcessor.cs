using System;
using System.Linq;
using System.Collections.Generic;
using LiteDB;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Models.Info;
using Kontur.GameStats.Server.Models.Reports;

//TODO: Отрефакторить этот кривущий класс
namespace Kontur.GameStats.Server.Data
{
    public class GameDbProcessor
    {
        public readonly LiteDatabase Db;

        public GameDbProcessor(IDatabaseProvider databaseProvider)
        {
            Db = databaseProvider.GetDatabaseConnection();
        }

        public MatchInfo FindMatchInfo(string endpoint, DateTime timestamp)
        {
            return Db.Find<GameMatch>(x => x.Server == endpoint)
                .FirstOrDefault(x => x.Timestamp.Equals(timestamp))?.Results;
        }

        private IEnumerable<GameMatch> GetMatchesForServer(string endpoint)
        {
            return Db.Find<GameMatch>(x => x.Server == endpoint);
        }

        private IEnumerable<int> GetCountMatchesPerDay(string endpoint)
        {
            var matches = GetMatchesForServer(endpoint);
            return matches.GroupBy(x => x.MatchEnd.Date).Select(g => g.Count());
        }

        private IEnumerable<int> GetCountPlayerMatchesPerDay(string playerName)
        {
            var matches = GetPlayerMatches(playerName);
            return matches.GroupBy(x => x.MatchEnd.Date).Select(g => g.Count());
        }

        public static string[] GetTop5GameModes(IEnumerable<GameMatch> matches, string endpoint)
        {
            var gameModes = matches.Select(x => x.Results.GameMode).ToArray();
            return gameModes.Distinct()
                .OrderByDescending(x => gameModes.Count(y => y == x))
                .ToArray();
        }

        public static string[] GetTop5Maps(IEnumerable<GameMatch> matches, string endpoint)
        {
            var maps = matches.Select(x => x.Results.Map).ToArray();
            return maps.Distinct()
                .OrderByDescending(x => maps.Count(y => y == x))
                .ToArray();
        }

        public static string GetFavouriteServer(IEnumerable<GameMatch> matches)
        {
            var servers = matches.Select(x => x.Server).ToArray();
            return servers.Distinct()
                .OrderByDescending(x => servers.Count(y => y == x))
                .First();
        }

        public static string GetFavouriteGameMode(IEnumerable<GameMatch> matches)
        {
            var servers = matches.Select(x => x.Results.GameMode).ToArray();
            return servers.Distinct()
                .OrderByDescending(x => servers.Count(y => y == x))
                .First();
        }

        public static int GetUniqueServersCount(IEnumerable<GameMatch> matches)
        {
            return matches.Select(x => x.Server).Distinct().Count();
        }

        private static IEnumerable<int> GetCountMatchesPerDay(IEnumerable<GameMatch> matches, string endpoint)
        {
            return matches.Where(m => m.Server == endpoint)
                .GroupBy(x => x.MatchEnd.Date).Select(g => g.Count());
        }

        public IEnumerable<ServerReport> GetServerRates()
        {
            var servers = Db.FindAll<GameServer>();
            return servers.Select(server => new ServerReport
            {
                Name = server.Info.Name,
                Endpoint = server.Endpoint,
                AverageMatchesPerDay = GetCountMatchesPerDay(server.Endpoint).Average()
            });
        }

        public IEnumerable<PlayerScore> GetAllScores()
        {
            return Db.FindAll<GameMatch>().SelectMany(match => match.Results.Scoreboard);
        }

        public IEnumerable<GameMatch> GetPlayerMatches(string playerName)
        {
            return Db.FindAll<GameMatch>()
                .Where(m => m.Results.Scoreboard
                .Any(s => s.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase)));
        }

        public double GetKillToDeathRatio(string playerName)
        {
            var playerScores = GetAllScores()
                .Where(score => score.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase)).ToArray();
            var deathCount = playerScores.Select(s => s.Deaths).Sum();
            var killsCount = playerScores.Select(s => s.Kills).Sum();
            return (double) killsCount / deathCount;
        }
        
        public double GetScoreboardPercent(PlayerScore[] scores, string name)
        {
            var totalPlayers = scores.Length;
            var place = scores.Select((s, i) => new {Index = i, Score = s})
                .First(s => s.Score.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Index;
            var playersBelowCurrent = totalPlayers - place - 1;
            return totalPlayers == 1 ? 100 : (double)playersBelowCurrent / (totalPlayers - 1) * 100;
        }

        private IEnumerable<double> GetScoreboardPercents(IEnumerable<GameMatch> matches, string name)
        {
            return matches.Select(m => GetScoreboardPercent(m.Results.Scoreboard, name));
        }

        public IEnumerable<PlayerReport> GetPlayerRates()
        {
            var allScores = GetAllScores().ToArray();
            var players = allScores.Select(score => score.Name).Distinct();
            foreach (var playerName in players)
            {
                var playerScores = allScores
                    .Where(score => score.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase)).ToArray();
                var deathCount = playerScores.Select(s => s.Deaths).Sum();
                var killsCount = playerScores.Select(s => s.Kills).Sum();
                if (deathCount > 0 && playerScores.Length >= 10)
                    yield return new PlayerReport
                    {
                        Name = playerName,
                        KillToDeathRatio = (double)killsCount / deathCount
                    };
            }
        }

        public ServerStats GetServerStats(string endpoint)
        {
            var matches = Db.Find<GameMatch>(x => x.Server == endpoint).ToArray();
            var countMatchesPerDay = GetCountMatchesPerDay(matches, endpoint).ToArray();
            var population = matches.Select(x => x.Results.Scoreboard.Length).ToArray();
            return new ServerStats
            {
                TotalMatchesPlayed = matches.Length,
                MaximumMatchesPerDay = countMatchesPerDay.Max(),
                AverageMatchesPerDay = countMatchesPerDay.Average(),
                MaximumPopulation = population.Max(),
                AveragePopulation = population.Average(),
                Top5GameModes = GetTop5GameModes(matches, endpoint),
                Top5Maps = GetTop5Maps(matches, endpoint)
            };
        }

        public PlayerStats GetPlayerStats(string name)
        {
            var matches = GetPlayerMatches(name).ToArray();
            var countsPerDay = GetCountPlayerMatchesPerDay(name).ToArray();
            var lastMatch = Db.FindAll<GameMatch>()
                .OrderByDescending(match => match.Timestamp).First();
            return new PlayerStats
            {
                TotalMatchesPlayed = matches.Length,
                TotalMatchesWon = matches.Count(m => m.WinnerScore.Name.Equals(name, StringComparison.OrdinalIgnoreCase)),
                FavoriteServer = GetFavouriteServer(matches),
                UniqueServers = GetUniqueServersCount(matches),
                FavoriteGameMode = GetFavouriteGameMode(matches),
                AverageScoreboardPercent = GetScoreboardPercents(matches, name).Average(),
                MaximumMatchesPerDay = countsPerDay.Max(),
                AverageMatchesPerDay = countsPerDay.Average(),
                LastMatchPlayed = lastMatch.Timestamp.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
                KillToDeathRatio = GetKillToDeathRatio(name)
            };
        }
    }
}
