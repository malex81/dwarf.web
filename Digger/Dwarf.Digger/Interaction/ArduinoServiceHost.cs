
using Dwarf.Digger.Interaction.Models;
using Microsoft.AspNetCore.SignalR;

namespace Dwarf.Digger.Interaction;

internal sealed class ArduinoServiceHost : IHostedService
{
	private readonly CancellationTokenSource serviceLifetimeCTS = new();
	private readonly IHubContext<ArduinoHub> ardHubContext;
	private readonly ILogger<ArduinoServiceHost> logger;
	private Task? serviceTask;
	private ArduinoClient? arduinoClient;

	public ArduinoServiceHost(IHubContext<ArduinoHub> ardHubContext, ILogger<ArduinoServiceHost> logger)
	{
		this.ardHubContext = ardHubContext;
		this.logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, serviceLifetimeCTS.Token);
		var ct = cts.Token;
		serviceTask = Task.WhenAll(
			Task.Run(async () => await EmulatorTaskLoop(ct), ct),
			Task.Run(async () => await ServiceTaskLoop(ct), ct));
		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		arduinoClient?.Dispose();
		serviceLifetimeCTS.Cancel();
		if (serviceTask != null)
			await Task.WhenAny(serviceTask, Task.Delay(5000, cancellationToken));
	}

	async Task ServiceTaskLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(200, ct);
		}
	}

	async Task EmulatorTaskLoop(CancellationToken ct)
	{
		var a1 = 200;
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(200, ct);
			/* Эмуляция активной деятельности */
			a1 = Math.Clamp((a1 + (int)(DateTime.Now.Ticks % 61) - 30), 0, 1024);
			await ardHubContext.Clients.All.SendAsync("ArduinoState", new ArduinoState(true, [
				new("D1", a1 < 300 ? 1 : 0),
				new("D2", a1 > 800 ? 1 : 0),
				new("A1", a1, IsAnalog: true),
				new("A2", (int)(DateTime.Now.Ticks % 1024), IsAnalog: true)]), cancellationToken: ct);
			//logger.LogInformation("123");
		}
	}
}