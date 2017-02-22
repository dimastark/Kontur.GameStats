using NUnit.Framework;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Testing.ReportTests
{
    [TestFixture, Description("Тесты GET /reports/recent-matches")]
    internal class RecentMatchesTests : BaseTest
    {
        public override void SetUp()
        {
            base.SetUp();
            MakeAdvertiseRequest("serv-13", TestsData.FirstServer);
        }

        [Test, Description("count по-умолчанию == 5")]
        public void TestDefaultCount()
        {
            PutMatchesForTestingReports(6);
            var matches = GetModelFromApi<GameMatch[]>("/reports/recent-matches");
            Assert.AreEqual(5, matches.Length);
        }

        [Test, Description("Если count == 0 вернуть пустой массив")]
        public void TestZeroCount()
        {
            TestMatchReports(5, 0, 0);
        }

        [Test, Description("Если count < 0 вернуть пустой массив")]
        public void TestNegativeCount()
        {
            TestMatchReports(5, -1, 0);
        }

        [Test, Description("count не превосходит 50")]
        public void TestBiggerThan50Count()
        {
            TestMatchReports(51, 51, 50);
        }
    }
}
