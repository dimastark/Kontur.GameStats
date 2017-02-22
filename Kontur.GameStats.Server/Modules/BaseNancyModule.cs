using Nancy;
using LiteDB;
using Kontur.GameStats.Server.Data;

namespace Kontur.GameStats.Server.Modules
{
    public abstract class BaseNancyModule : NancyModule
    {
        protected readonly GameDbProcessor DbProcessor;
        public LiteDatabase Db => DbProcessor.Db;

        protected BaseNancyModule(IDatabaseProvider databaseProvider, string prefix = "") : base(prefix)
        {
            DbProcessor = new GameDbProcessor(databaseProvider);
        }

        protected Response JsonOrErrorCode<TModel>(TModel obj, HttpStatusCode code = HttpStatusCode.NotFound)
        {
            return obj == null ? code : Response.AsJson(obj);
        }

        protected static string GetCannonicalEndpoint(dynamic parameters)
        {
            return parameters.ipOrHostname + "-" + parameters.port;
        }

        protected static int GetCannonicalCount(int badCount)
        {
            if (badCount < 0)
                return 0;
            return badCount > 50 ? 50 : badCount;
        }
    }
}
