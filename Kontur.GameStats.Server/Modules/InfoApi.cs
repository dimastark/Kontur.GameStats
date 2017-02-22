using System;
using Nancy;
using Nancy.ModelBinding;
using Kontur.GameStats.Server.Data;
using Kontur.GameStats.Server.Models.Info;
using Kontur.GameStats.Server.Validators;

namespace Kontur.GameStats.Server.Modules
{
    public class InfoApi : BaseNancyModule
    {
        public InfoApi(IDatabaseProvider databaseProvider) : base(databaseProvider, "/servers")
        {
            Get[$"/{QueryFormats.Endpoint}/info"] = GetServerInfo;
            Put[$"/{QueryFormats.Endpoint}/info"] = PutServerInfo;

            Get[$"/{QueryFormats.Endpoint}/matches/{QueryFormats.Timestamp}"] = GetServerMatches;
            Put[$"/{QueryFormats.Endpoint}/matches/{QueryFormats.Timestamp}"] = PutServerMatches;

            Get["/info"] = GetServersInfo;
        }

        private dynamic GetServerInfo(dynamic parameters)
        {
            string endpoint = GetCannonicalEndpoint(parameters);
            var serverModel = Db.FindById<GameServer>(endpoint);
            return JsonOrErrorCode(serverModel?.Info);
        }

        private dynamic PutServerInfo(dynamic parameters)
        {
            string endpoint = GetCannonicalEndpoint(parameters);
            var serverInfoModel = this.BindAndValidate<ServerInfo>();
            if (!ModelValidationResult.IsValid)
                return HttpStatusCode.BadRequest;
            Db.InsertOrUpdateById(endpoint, new GameServer {Info = serverInfoModel});
            return HttpStatusCode.OK;
        }

        private dynamic GetServerMatches(dynamic parameters)
        {
            string endpoint = GetCannonicalEndpoint(parameters);
            var matchInfoModel = DbProcessor.FindMatchInfo(endpoint, parameters.timestamp);
            return JsonOrErrorCode<MatchInfo>(matchInfoModel);
        }

        private dynamic PutServerMatches(dynamic parameters)
        {
            string endpoint = GetCannonicalEndpoint(parameters);
            DateTime timestamp = parameters.timestamp;
            var server = Db.FindById<GameServer>(endpoint);
            var matchInfoModel = this.BindAndValidate<MatchInfo>();
            if (!ModelValidationResult.IsValid || server == null)
                return HttpStatusCode.BadRequest;
            Db.Insert(new GameMatch
            {
                Server = endpoint,
                Results = matchInfoModel,
                Timestamp = timestamp
            });
            return HttpStatusCode.OK;
        }

        private dynamic GetServersInfo(dynamic parameters)
        {
            return Response.AsJson(Db.FindAll<GameServer>());
        }
    }
}
