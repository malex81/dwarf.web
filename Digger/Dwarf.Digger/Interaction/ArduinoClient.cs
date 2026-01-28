using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Dwarf.Digger.Interaction;

internal sealed class ArduinoClient : IDisposable
{
	private TcpClient? client;
	private NetworkStream? stream;
	private readonly string ip;
	private readonly int port;
	private CancellationTokenSource cts = new();

	public event Action<string>? OnMessageReceived;
	public event Action<bool>? OnConnectionChanged;

	public ArduinoClient(string ip, int port)
	{
		this.ip = ip;
		this.port = port;
	}

	[MemberNotNullWhen(true, nameof(client), nameof(stream))]
	public bool Connected => stream != null && client != null && client.Connected;

	public void Start()
	{
		cts = new CancellationTokenSource();
		_ = ConnectionLoop(cts.Token); // Запуск фонового цикла
	}

	private async Task ConnectionLoop(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				if (client == null || !client.Connected)
				{
					client?.Dispose();
					client = new TcpClient();
					await client.ConnectAsync(ip, port);
					stream = client.GetStream();
					OnConnectionChanged?.Invoke(true);

					// Запускаем чтение в отдельном потоке
					_ = ReceiveLoop(token);
				}
			}
			catch { OnConnectionChanged?.Invoke(false); }

			await Task.Delay(5000, token); // Пауза перед проверкой/переподключением
		}
	}

	private async Task ReceiveLoop(CancellationToken token)
	{
		byte[] buffer = new byte[1024];
		try
		{
			while (Connected && !token.IsCancellationRequested)
			{
				int bytesRead = await stream.ReadAsync(buffer, token);
				if (bytesRead == 0) break; // Соединение закрыто со стороны Arduino

				string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead);
				OnMessageReceived?.Invoke(msg);
			}
		}
		catch { }
		finally { OnConnectionChanged?.Invoke(false); }
	}

	public async Task SendCommand(string command, CancellationToken cancellationToken = default)
	{
		if (Connected)
		{
			byte[] data = Encoding.ASCII.GetBytes(command + "\n");
			await stream.WriteAsync(data, cancellationToken);
		}
	}

	public void Dispose()
	{
		cts?.Cancel();
		client?.Dispose();
	}
}