export default {
	connect() {
		const connection = new signalR.HubConnectionBuilder()
			.withUrl("/arduino")
			.build();

		connection.on("ArduinoState", (state) => {
			console.log(state);
		});

		connection.start().catch(err => console.error(err));
	}
}