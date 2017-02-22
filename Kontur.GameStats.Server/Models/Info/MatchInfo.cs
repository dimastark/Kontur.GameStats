namespace Kontur.GameStats.Server.Models.Info
{
    public class MatchInfo
    {
        public string Map { get; set; }
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public PlayerScore[] Scoreboard { get; set; }
    }
}
