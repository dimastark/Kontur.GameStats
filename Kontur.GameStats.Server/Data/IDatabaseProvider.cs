using LiteDB;

namespace Kontur.GameStats.Server.Data
{
    public interface IDatabaseProvider
    {
        LiteDatabase GetDatabaseConnection();
    }
}
