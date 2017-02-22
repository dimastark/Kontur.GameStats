using System;
using System.IO;

namespace Kontur.GameStats.Server.Modules.Logging
{
    class Logger : ILogger
    {
        public void LogError(Exception exception)
        {
            using (var file = new StreamWriter("log.txt"))
                file.WriteLine(exception.ToString());
        }
    }
}
