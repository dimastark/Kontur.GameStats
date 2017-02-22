using Nancy.Bootstrapper;

namespace Kontur.GameStats.Server.Modules.Logging
{
    public class ExceptionLogger : IApplicationStartup
    {
        private readonly ILogger _log;

        public ExceptionLogger(ILogger log)
        {
            _log = log;
        }

        public void Initialize(IPipelines pipelines)
        {
            pipelines.OnError.AddItemToEndOfPipeline((ctx, exception) =>
            {
                _log.LogError(exception);
                return null;
            });
        }
    }
}
