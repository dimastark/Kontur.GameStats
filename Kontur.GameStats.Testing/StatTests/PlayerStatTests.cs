using Nancy;
using NUnit.Framework;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Testing.StatTests
{
    [TestFixture, Description("Тесты GET статистики для игрока")]
    internal class PlayerStatTests : BaseTest
    {
        [Test, Description("Если нет такого игрока [404 Not Found]")]
        public void TestNotFoundPlayer()
        {
            var response = Browser.Get("/players/stranger/stats");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("Если в матче один игрок, то scoreboardPercent = 100%")]
        public void TestOnePlayer()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.SecondMatch);
            var stats = GetModelFromApi<PlayerStats>("/players/Player1/stats");
            Assert.AreEqual(100, stats.AverageScoreboardPercent);
        }

        [Test, Description("Имена игроков должны сравниваться без учета регистра")]
        public void TestPlayerNameCompare()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.SecondMatch);
            var response = Browser.Get("/players/player1/stats");
            Assert.AreNotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("Проверка корректности статистики для игрока")]
        public void TestPlayerStatsCorrect()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            MakeAdvertiseRequest("my-14", TestsData.SecondServer);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.FirstMatch);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.FirstMatch);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.FirstMatch);
            PutModelToApi("/servers/my-14/matches/2017-01-24T15:18:00Z", TestsData.FirstMatch);
            var stats = GetModelFromApi<PlayerStats>("/players/Player1/stats");
            Assert.AreEqual(2, stats.AverageMatchesPerDay);
            Assert.AreEqual(100, stats.AverageScoreboardPercent);
            Assert.AreEqual("DM", stats.FavoriteGameMode);
            Assert.AreEqual("my-13", stats.FavoriteServer);
            Assert.AreEqual(21 * 4f / 12, stats.KillToDeathRatio);
            Assert.AreEqual("2017-01-24T15:18:00Z", stats.LastMatchPlayed);
            Assert.AreEqual(3, stats.MaximumMatchesPerDay);
            Assert.AreEqual(4, stats.TotalMatchesPlayed);
            Assert.AreEqual(4, stats.TotalMatchesWon);
            Assert.AreEqual(2, stats.UniqueServers);
        }
    }
}
