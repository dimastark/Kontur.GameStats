using LiteDB;

namespace Kontur.GameStats.Server.Models.Info
{
    public class PlayerScore
    {
        [BsonIndex]
        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
