using System.Linq;
using Nancy;
using Kontur.GameStats.Server.Data;
using Kontur.GameStats.Server.Models.Info;
using Kontur.GameStats.Server.Validators;

namespace Kontur.GameStats.Server.Modules
{
    public class ReportsApi : BaseNancyModule
    {
        public ReportsApi(IDatabaseProvider databaseProvider) : base(databaseProvider, "/reports")
        {
            Get[$"/recent-matches/{QueryFormats.Count}"] = GetRecentMatches;
            Get[$"/best-players/{QueryFormats.Count}"] = GetBestPlayers;
            Get[$"/popular-servers/{QueryFormats.Count}"] = GetPopularServers;
        }

        private dynamic GetRecentMatches(dynamic parameters)
        {
            int count = GetCannonicalCount(parameters.count);
            return Response.AsJson(Db.FindAll<GameMatch>()
                .OrderByDescending(match => match.Timestamp)
                .Take(count));
        }

        private dynamic GetBestPlayers(dynamic parameters)
        {
            int count = GetCannonicalCount(parameters.count);
            return Response.AsJson(DbProcessor.GetPlayerRates()
                .OrderByDescending(playerRate => playerRate.KillToDeathRatio)
                .Take(count).ToArray());
        }

        private dynamic GetPopularServers(dynamic parameters)
        {
            int count = GetCannonicalCount(parameters.count);
            return Response.AsJson(DbProcessor.GetServerRates()
                .OrderByDescending(r => r.AverageMatchesPerDay)
                .Take(count));
        }
    }
}
