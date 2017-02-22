using System;
using System.Linq;
using Nancy;
using Fclp.Internals.Extensions;
using Kontur.GameStats.Server.Data;
using Kontur.GameStats.Server.Models.Info;
using Kontur.GameStats.Server.Validators;

namespace Kontur.GameStats.Server.Modules
{
    public class StatsApi : BaseNancyModule
    {
        public StatsApi(IDatabaseProvider databaseProvider) : base(databaseProvider)
        {
            Get[$"/servers/{QueryFormats.Endpoint}/stats"] = GetServerStats;
            Get[$"/players/{QueryFormats.PlayerName}/stats"] = GetPlayerStats;
        }

        private dynamic GetServerStats(dynamic parameters)
        {
            string endpoint = parameters.ipOrHostname + "-" + parameters.port;
            var serverModel = Db.FindById<GameServer>(endpoint);
            if (serverModel == null)
                return HttpStatusCode.NotFound;
            return Response.AsJson(DbProcessor.GetServerStats(endpoint));
        }

        private dynamic GetPlayerStats(dynamic parameters)
        {
            string playerName = parameters.name;
            var isNullOrEmpty = DbProcessor.GetAllScores()
                .Where(s => s.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                .IsNullOrEmpty();
            if (isNullOrEmpty)
                return HttpStatusCode.NotFound;
            return Response.AsJson(DbProcessor.GetPlayerStats(playerName));
        }
    }
}
