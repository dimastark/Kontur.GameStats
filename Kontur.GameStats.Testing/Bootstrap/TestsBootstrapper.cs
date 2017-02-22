using Nancy;
using Nancy.TinyIoc;
using Kontur.GameStats.Server.Data;

namespace Kontur.GameStats.Testing.Bootstrap
{
    public class TestsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IDatabaseProvider, TestDatabase>();
        }
    }
}
