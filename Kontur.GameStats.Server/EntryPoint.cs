using System;
using Fclp;
using Nancy.Hosting.Self;

namespace Kontur.GameStats.Server
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            var commandLineParser = new FluentCommandLineParser<Options>();

            commandLineParser
                .Setup(options => options.Prefix)
                .As("prefix")
                .SetDefault("http://+:8080/")
                .WithDescription("HTTP prefix to listen on");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader($"{AppDomain.CurrentDomain.FriendlyName} [--prefix <prefix>]")
                .Callback(text => Console.WriteLine(text));

            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunServer(commandLineParser.Object);
        }

        private static void RunServer(Options options)
        {
            var uri = new Uri(options.Prefix.Replace("+", "localhost"));

            using (var host = new NancyHost(uri))
            {
                host.Start();

                Console.WriteLine("GameStats Server is running on " + uri);
                Console.WriteLine("Press [Enter] to close the host.");
                Console.ReadLine();
            }
        }

        private class Options
        {
            public string Prefix { get; set; }
        }
    }
}
