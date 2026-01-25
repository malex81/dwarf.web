using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Dwarf.Web.Configuration
{
	public static class WebHostHelper
	{
		public static IWebHostBuilder CreateDefaultBuilder(string[] args)
		{
			var sl = SettingsLocation.DetermineSettingsLocation(args);

			var hostingConfig = new ConfigurationBuilder()
				.SetBasePath(sl.ConfigurationBasePath)
				.AddJsonFile("hosting.json", optional: true)
				.AddCommandLine(args)
				.Build();

			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, confBuilder) =>
				{
					confBuilder.SetBasePath(sl.ConfigurationBasePath);
				})
				.UseConfiguration(hostingConfig)
				.ConfigureServices(services =>
				{
					services.AddSingleton(sl);
				});
		}
	}
}
