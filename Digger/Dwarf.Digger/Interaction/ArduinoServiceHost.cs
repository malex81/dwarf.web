
using Dwarf.Digger.Interaction.Models;
using Microsoft.AspNetCore.SignalR;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Dwarf.Digger.Interaction;

internal sealed partial class ArduinoServiceHost : IHostedService
{
	private readonly CancellationTokenSource serviceLifetimeCTS = new();
	private readonly IHubContext<ArduinoHub> ardHubContext;
	private readonly ArduinoClientaccessor arduinoClientaccessor;
	private readonly ILogger<ArduinoServiceHost> logger;
	private Task? serviceTask;

	public ArduinoServiceHost(IHubContext<ArduinoHub> ardHubContext, ArduinoClientaccessor arduinoClientaccessor, ILogger<ArduinoServiceHost> logger)
	{
		this.ardHubContext = ardHubContext;
		this.arduinoClientaccessor = arduinoClientaccessor;
		this.logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, serviceLifetimeCTS.Token);
		var ct = cts.Token;

		/*		arduinoClientaccessor.Connect();
				arduinoClientaccessor.Current.OnMessageReceived += ArduinoMessageReceived;
				arduinoClientaccessor.Current.OnConnectionChanged += ArduinoConnectionChanged;
				serviceTask = Task.Run(async () => await ServiceTaskLoop(ct), ct);
		*/

		serviceTask = Task.Run(async () => await EmulatorTaskLoop(ct), ct);

		/*
			serviceTask = Task.WhenAll(
				Task.Run(async () => await EmulatorTaskLoop(ct), ct),
				Task.Run(async () => await ServiceTaskLoop(ct), ct));
		*/
		return Task.CompletedTask;
	}

	private void ArduinoConnectionChanged(bool connection)
	{
		logger.LogInformation("Arduino connection state: {connection}", connection ? "Connected" : "Disconnected");
		if (!connection)
			ardHubContext.Clients.All.SendAsync("ArduinoState", new ArduinoState(false, []));
	}

	private void ArduinoMessageReceived(string msg)
	{
		//logger.LogInformation("Arduino send: {message}", msg);
		if (!msg.StartsWith("STATE:"))
			return;
		var regex = StateRegex();
		var matches = regex.Matches(msg);
		var pins = matches.Select(m => new { name = m.Groups["name"].Value, value = m.Groups["value"].Value })
			.Select(r => new ArduinoPinState(r.name, int.Parse(r.value), r.name.StartsWith('A')));
		ardHubContext.Clients.All.SendAsync("ArduinoState", new ArduinoState(true, pins.ToArray()));
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		arduinoClientaccessor.Free();
		serviceLifetimeCTS.Cancel();
		if (serviceTask != null)
			await Task.WhenAny(serviceTask, Task.Delay(5000, cancellationToken));
	}

	async Task ServiceTaskLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(2000, ct);
			if (arduinoClientaccessor.Current.Connected)
				await arduinoClientaccessor.Current.SendCommand("PING", ct);
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
				new("D3", 1),
				new("D4", 0),
				new("A1", a1, IsAnalog: true),
				new("A2", (int)(DateTime.Now.Ticks % 1024), IsAnalog: true)]), cancellationToken: ct);
			//logger.LogInformation("123");
		}
	}

	[GeneratedRegex(@"(?<name>\w+):(?<value>\d+);")]
	private static partial Regex StateRegex();
}