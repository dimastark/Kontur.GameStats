using System;

namespace Kontur.GameStats.Server.Modules.Logging
{
    public interface ILogger
    {
        void LogError(Exception exception);
    }
}
