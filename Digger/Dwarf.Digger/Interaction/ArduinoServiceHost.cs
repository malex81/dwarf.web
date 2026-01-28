
using Dwarf.Digger.Interaction.Models;
using Microsoft.AspNetCore.SignalR;

namespace Dwarf.Digger.Interaction;

internal sealed class ArduinoServiceHost : IHostedService
{
	private readonly CancellationTokenSource serviceLifetimeCTS = new();
	private readonly IHubContext<ArduinoHub> ardHubContext;
	private readonly ILogger<ArduinoServiceHost> logger;
	private Task? serviceTask;

	public ArduinoServiceHost(IHubContext<ArduinoHub> ardHubContext, ILogger<ArduinoServiceHost> logger)
	{
		this.ardHubContext = ardHubContext;
		this.logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, serviceLifetimeCTS.Token);
		var ct = cts.Token;
		serviceTask = new Task(async () => await ServiceTaskLoop(ct), ct, TaskCreationOptions.LongRunning);
		serviceTask.Start();
		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		serviceLifetimeCTS.Cancel();
		if (serviceTask != null)
			await serviceTask;
	}

	async Task ServiceTaskLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(1000, ct);
			await ardHubContext.Clients.All.SendAsync("ArduinoState", new ArduinoState([new("D1", 1), new("D2", 0)]), cancellationToken: ct);
			//logger.LogInformation("123");
		}
	}
}