using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dwarf.Web.Configuration
{
	class SettingsLocation : ISettingsLocation
	{
		public SettingsLocation(RedirectSettings redirectSettings)
		{
			var curDir = Directory.GetCurrentDirectory();
			var configPath = redirectSettings?.ConfigPath;
			if (!string.IsNullOrWhiteSpace(configPath))
			{
				ConfigurationBasePath = Path.IsPathRooted(configPath) ? configPath : Path.Combine(curDir, configPath);
				if (!Directory.Exists(ConfigurationBasePath))
					throw new Exception($"Configuration folder '{ConfigurationBasePath}' not exists ('ConfigPath: {configPath}')");
			}
			else
				ConfigurationBasePath = curDir;
		}

		public string ConfigurationBasePath { get; }

		public static ISettingsLocation DetermineSettingsLocation(string[] args)
		{
			var curDir = Directory.GetCurrentDirectory();
			var redirectConfig = new ConfigurationBuilder()
				.SetBasePath(curDir)
				.AddJsonFile("appsettings.json", optional: true)
				.AddJsonFile("redirectsettings.json", optional: true)
				.AddJsonFile("redirectsettings.dev.json", optional: true)
				.AddCommandLine(args ?? new string[] { })
				.Build();
			return new SettingsLocation(redirectConfig.Get<RedirectSettings>());
		}

	}
}
