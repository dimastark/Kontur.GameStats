using Nancy;
using NUnit.Framework;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Testing.StatTests
{
    [TestFixture, Description("Тесты GET статистики для сервера")]
    internal class ServerStatTests : BaseTest
    {
        [Test, Description("Нельзя спрашивать stats сервера, для которого не было advertise [404 Not Found]")]
        public void TestStatsWithoutAdvertise()
        {
            var response = Browser.Get("/servers/stranger-13/stats");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("Если не было матчей, то нет и статистики [404 Not Found]")]
        public void TestNoStatsWithoutMatches()
        {
            MakeAdvertiseRequest("my-3", TestsData.FirstServer);
            var response = Browser.Get("/servers/stranger-13/stats");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("Матч на границе дней относится к тому дню, где он кончился")]
        public void TestTimeEllapsedOfMatch()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.FirstMatch);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.SecondMatch);
            var stats = GetModelFromApi<ServerStats>("/servers/my-13/stats");
            Assert.AreEqual(1, stats.MaximumMatchesPerDay);
        }

        [Test, Description("Проверка корректности статистики для сервера")]
        public void TestServerStatsCorrect()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.FirstMatch);
            PutModelToApi("/servers/my-13/matches/2017-01-22T15:17:00Z", TestsData.SecondMatch);
            PutModelToApi("/servers/my-13/matches/2017-01-23T15:17:00Z", TestsData.FirstMatch);
            var stats = GetModelFromApi<ServerStats>("/servers/my-13/stats");
            Assert.AreEqual((2 + 1) / 2d, stats.AverageMatchesPerDay);
            Assert.AreEqual(2, stats.MaximumMatchesPerDay);
            Assert.AreEqual(3, stats.MaximumPopulation);
            Assert.AreEqual((3 + 3 + 1) / 3d, stats.AveragePopulation);
            Assert.AreEqual(3, stats.TotalMatchesPlayed);
            Assert.AreEqual("DM", stats.Top5GameModes[0]);
            Assert.AreEqual("DM-HelloWorld", stats.Top5Maps[0]);
        }
    }
}
