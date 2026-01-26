namespace Dwarf.Digger.AppConfig;

internal static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddConfigs(this WebApplicationBuilder builder)
	{
		builder.Configuration
			.SetBasePath(Path.Combine(builder.Environment.ContentRootPath, "config"))
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			//.AddJsonFile($"config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();
		return builder;
	}

}