using Microsoft.AspNetCore.SignalR;

namespace Dwarf.Digger.Interaction;

internal sealed class ArduinoHub : Hub
{
	private readonly ILogger<ArduinoHub> logger;

	public ArduinoHub(ILogger<ArduinoHub> logger)
	{
		this.logger = logger;
	}
}