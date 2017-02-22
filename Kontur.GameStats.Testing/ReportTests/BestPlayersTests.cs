using NUnit.Framework;
using Kontur.GameStats.Server.Models.Reports;

namespace Kontur.GameStats.Testing.ReportTests
{
    [TestFixture, Description("Тесты GET /reports/best-players")]
    internal class BestPlayersTests : BaseTest
    {
        public override void SetUp()
        {
            base.SetUp();
            MakeAdvertiseRequest("serv-13", TestsData.FirstServer);
        }

        [Test, Description("count по-умолчанию == 5")]
        public void TestDefaultCount()
        {
            PutPlayersForTestingReports(10, 6);
            var players = GetModelFromApi<PlayerReport[]>("/reports/best-players");
            Assert.AreEqual(5, players.Length);
        }

        [Test, Description("Если count == 0 вернуть пустой массив")]
        public void TestZeroCount()
        {
            TestPlayerReports(10, 5, 0, 0);
        }

        [Test, Description("Если count < 0 вернуть пустой массив")]
        public void TestNegativeCount()
        {
            TestPlayerReports(10, 5, -1, 0);
        }

        [Test, Description("count не превосходит 50")]
        public void TestBiggerThan50Count()
        {
            TestPlayerReports(10, 51, 51, 50);
        }

        [Test, Description("Пропускать не умирающих игроков")]
        public void TestSkipChiters()
        {
            TestPlayerReports(10, 51, 51, 50, false);
        }

        [Test, Description("Пропускать игроков с меньше 10 игр")]
        public void TestSkipNewbies()
        {
            TestPlayerReports(9, 51, 51, 0);
        }
    }
}
