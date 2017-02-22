using LiteDB;
using Nancy;
using Nancy.Testing;
using NUnit.Framework;
using Kontur.GameStats.Testing.Bootstrap;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Testing
{
    internal abstract class BaseTest
    {
        protected Browser Browser;

        [SetUp]
        public virtual void SetUp()
        {
            Browser = new Browser(new TestsBootstrapper());
        }

        [TearDown]
        public virtual void TearDown()
        {
            using (var db = new LiteDatabase("tests.db"))
            {
                db.DropCollection(typeof(GameServer).Name);
                db.DropCollection(typeof(GameMatch).Name);
            }
        }

        public T GetModelFromApi<T>(string query)
        {
            var response = Browser.Get(query);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            return response.Body.DeserializeJson<T>();
        }

        public void PutModelToApi<T>(string query, T model)
        {
            var response = Browser.Put(query, with => with.JsonBody(model));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        public BrowserResponse MakeAdvertiseRequest(string endpoint, ServerInfo info)
        {
            var query = $"/servers/{endpoint}/info";
            return Browser.Put(query, with => with.JsonBody(info));
        }

        protected void PutMatchesForTestingReports(int count)
        {
            for (var i = 0; i < count; i++)
                PutModelToApi($"/servers/serv-13/matches/2017-01-22T15:{i:00}:00Z", TestsData.FirstMatch);
        }

        protected void TestMatchReports(int putCount, int getCount, int exceptedCount)
        {
            PutMatchesForTestingReports(putCount);
            var matches = GetModelFromApi<GameMatch[]>($"/reports/recent-matches/{getCount}");
            Assert.AreEqual(exceptedCount, matches.Length);
        }

        protected void PutPlayersForTestingReports(int matchesCount, int count, bool isDead = true)
        {
            for (var i = 0; i < matchesCount; i++)
                PutModelToApi("/servers/serv-13/matches/2017-01-22T15:00:00Z", TestsData.GenerateMatchInfo(count));
        }

        protected void TestPlayerReports(int matchesCount, int putCount, int getCount, int exceptedCount, bool isDead = true)
        {
            PutPlayersForTestingReports(matchesCount, putCount, isDead);
            var players = GetModelFromApi<GameMatch[]>($"/reports/best-players/{getCount}");
            Assert.AreEqual(exceptedCount, players.Length);
        }
    }
}
