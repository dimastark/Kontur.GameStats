using LiteDB;

namespace Kontur.GameStats.Server.Data
{
    public sealed class LiteDatabaseProvider : IDatabaseProvider
    {
        private const string ConnectionString = "filename=gamestats.db; journal=false";
        private static readonly LiteDatabase Instance = new LiteDatabase(ConnectionString);

        public LiteDatabase GetDatabaseConnection()
        {
            return Instance;
        }
    }
}
