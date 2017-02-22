using Nancy;
using Nancy.Testing;
using NUnit.Framework;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Testing.InfoTests
{
    [TestFixture, Description("Тесты GET/PUT для /server/<endpoint>/match")]
    internal class MatchInfoTests : BaseTest
    {
        [Test, Description("Отвечаем тем, от кого не было advertise запроса [400 Bad Request]")]
        public void TestNotAdvertisedServerPutMatchResult()
        {
            const string url = "http://localhost:8080/servers/stranger-7/matches/2017-01-22T15:17:00Z";
            var response = Browser.Put(url, with => with.JsonBody(TestsData.FirstMatch));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Description("Если был advertise запрос, то можно посылать результаты матчей")]
        public void TestPutMatchResults()
        {
            const string url = "http://localhost:8080/servers/my-13/matches/2017-01-22T15:17:00Z";
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            var response = Browser.Put(url, with => with.JsonBody(TestsData.FirstMatch));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test, Description("Если PUT-запроса по этому адресу не было, то [404 Not Found]")]
        public void TestGetWithoutPut()
        {
            const string url = "http://localhost:8080/servers/my-13/matches/2017-01-22T15:17:00Z";
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            var response = Browser.Get(url);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Test, Description("Пытаемся получить результаты матча после его отправки")]
        public void TestGetMatchResults()
        {
            const string url = "http://localhost:8080/servers/my-13/matches/2017-01-22T15:17:00Z";
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            Browser.Put(url, with => with.JsonBody(TestsData.FirstMatch));
            var matchInfoFromApi = GetModelFromApi<MatchInfo>(url);
            Assert.AreEqual(TestsData.FirstMatch.Map, matchInfoFromApi.Map);
        }

        [Test, Description("Если нет каких то полей в информации о матче то [400 Bad Request]")]
        public void TestSendMatchWithoutFields()
        {
            const string url = "http://localhost:8080/servers/my-13/matches/2017-01-22T15:17:00Z";
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            var response = Browser.Put(url, with => with.JsonBody(new MatchInfo()));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Description("Если плохой timestamp то [404 Not Found]")]
        public void TestSendBadTimestamp()
        {
            const string url = "http://localhost:8080/servers/my-13/matches/2017-02-31T15:17:00Z";
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            var response = Browser.Put(url, with => with.JsonBody(TestsData.FirstMatch));
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
