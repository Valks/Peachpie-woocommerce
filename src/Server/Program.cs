using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using PeachPied.WordPress.AspNetCore;

namespace Server
{
    public class Program
    {
        private const string WORDPRESS_EXTENSIONS_PREFIX = "Plugin.Wordpress";
        private const string WORDPRESS_EXTENSIONS_CONFIGURATION = "Wordpress:LegacyPluginAssemblies";
      
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(Program).Assembly.Location));

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Assemblies aren't loaded yet so have to scan the files in the folder
                    var wordpressExtensions = new Dictionary<string, string>();
                    var index = 0;
                    foreach (var wordpressConfigFile in 
                        Directory.GetFiles(
                            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                            "*.dll",
                            SearchOption.TopDirectoryOnly)
                        .Where(f => Path.GetFileName(f).StartsWith(WORDPRESS_EXTENSIONS_PREFIX))
                        .Select(f => Path.GetFileNameWithoutExtension(f)))
                    {
                        Console.WriteLine($"Trying to load extension: {wordpressConfigFile} | {ConfigurationPath.Combine(WORDPRESS_EXTENSIONS_CONFIGURATION, index.ToString())}");
                        wordpressExtensions.Add(
                            ConfigurationPath.Combine(WORDPRESS_EXTENSIONS_CONFIGURATION, index.ToString()),
                            wordpressConfigFile);
                        index++;
                    }
                    config.AddInMemoryCollection(wordpressExtensions);
                })
                .UseStartup<Startup>();
    }
}