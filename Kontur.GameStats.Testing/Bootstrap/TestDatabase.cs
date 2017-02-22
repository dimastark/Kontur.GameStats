using LiteDB;
using Kontur.GameStats.Server.Data;

namespace Kontur.GameStats.Testing.Bootstrap
{
    public sealed class TestDatabase : IDatabaseProvider
    {
        private const string ConnectionString = "filename=tests.db; journal=false";
        private static readonly LiteDatabase Instance = new LiteDatabase(ConnectionString);

        public LiteDatabase GetDatabaseConnection()
        {
            return Instance;
        }
    }
}
