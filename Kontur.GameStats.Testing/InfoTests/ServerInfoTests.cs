using Nancy;
using NUnit.Framework;
using Kontur.GameStats.Server.Models.Info;

namespace Kontur.GameStats.Testing.InfoTests
{
    [TestFixture, Description("Тесты GET/PUT для /server/<endpoint>/info")]
    internal class ServerInfoTests : BaseTest
    {
        [Test, Description("Код ответа на advertise запрос [200 OK]")]
        public void TestAdvertiseRequestStatusCode()
        {
            var response = MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test, Description("Если не было advertise запроса с таким endpoint то [404 Not Found]")]
        public void TestGetWithoutAdvertising()
        {
            var response = Browser.Get("/servers/my-13/info");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("Если нет каких то полей в advertise запросе то [400 Bad Request]")]
        public void TestAdvertiseWithoutFields()
        {
            var response = MakeAdvertiseRequest("my-13", new ServerInfo());
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Description("Пустое имя - плохо [400 Bad Request]")]
        public void TestEmptyServerName()
        {
            var response = MakeAdvertiseRequest("my-13", new ServerInfo
            {
                Name = "", GameModes = TestsData.FirstServer.GameModes
            });
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Description("<endpoint> без порта - [404 Not Found]")]
        public void TestBadEndpointWithoutPort()
        {
            var response = MakeAdvertiseRequest("bad_endpoint", TestsData.FirstServer);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("<endpoint> с слишком большим портом - [404 Not Found]")]
        public void TestBadEndpointBigPort()
        {
            var response = MakeAdvertiseRequest("my-65536", TestsData.FirstServer);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test, Description("порт == 65535 включителен")]
        public void TestNotBigEnoughtPort()
        {
            var response = MakeAdvertiseRequest("my-65535", TestsData.FirstServer);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test, Description("Второй advertise запрос переписывает первый")]
        public void TestRewritingAdvertise()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            MakeAdvertiseRequest("my-13", TestsData.SecondServer);
            var serverInfoModelFromApi = GetModelFromApi<ServerInfo>("/servers/my-13/info");
            Assert.AreEqual(TestsData.SecondServer.Name, serverInfoModelFromApi.Name);
        }

        [Test, Description("Информация о всех серверах должна выводить всё и про всех")]
        public void TestFullInfo()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            MakeAdvertiseRequest("my-14", TestsData.SecondServer);
            var allServers = GetModelFromApi<GameServer[]>("/servers/info");
            Assert.AreEqual(2, allServers.Length);
        }

        [Test, Description("Полная информация должна совпадать с частичной")]
        public void TestFullInfoConsistanse()
        {
            MakeAdvertiseRequest("my-13", TestsData.FirstServer);
            MakeAdvertiseRequest("my-14", TestsData.SecondServer);
            var allServers = GetModelFromApi<GameServer[]>("/servers/info");
            var aboutFirst = GetModelFromApi<ServerInfo>("/servers/my-13/info");
            var aboutSecond = GetModelFromApi<ServerInfo>("/servers/my-14/info");
            Assert.AreEqual(2, allServers.Length);
            Assert.AreEqual(allServers[0].Info.Name, aboutFirst.Name);
            Assert.AreEqual(allServers[1].Info.Name, aboutSecond.Name);
        }
    }
}
