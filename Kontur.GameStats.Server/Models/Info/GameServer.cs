using LiteDB;

namespace Kontur.GameStats.Server.Models.Info
{
    public class GameServer
    {
        [BsonId]
        public string Endpoint { get; set; }
        public ServerInfo Info { get; set; }
    }
}
