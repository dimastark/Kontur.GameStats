using System;
using LiteDB;
using Nancy.Json;

namespace Kontur.GameStats.Server.Models.Info
{
    public class GameMatch
    {
        [BsonIndex] public string Server { get; set; }
        [BsonIndex] public virtual DateTime Timestamp { get; set; }
        public MatchInfo Results { get; set; }

        [BsonIgnore]
        [ScriptIgnore]
        public DateTime MatchEnd => Timestamp.AddSeconds(Results.TimeElapsed);

        [BsonIgnore]
        [ScriptIgnore]
        public PlayerScore WinnerScore => Results.Scoreboard[0];
    }
}
