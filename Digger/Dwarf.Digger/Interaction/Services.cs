using Dwarf.Toolkit.Basic.DiHelpers;

namespace Dwarf.Digger.Interaction;

internal sealed class Services : IServicesBatch
{
	public void Configure(IServiceCollection services)
	{
		services.AddHostedService<ArduinoServiceHost>();
		services.AddSingleton<ArduinoClientaccessor>();
	}
}