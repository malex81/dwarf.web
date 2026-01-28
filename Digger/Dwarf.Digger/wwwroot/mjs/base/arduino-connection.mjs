import { ref } from 'vue'

export default {
	pinStates: ref([]),

	connect() {
		const self = this;
		const connection = new signalR.HubConnectionBuilder()
			.withUrl("/arduino")
			.build();

		connection.on("ArduinoState", (state) => {
			self.pinStates.value = state.pinStates;
			//console.log(state);
		});

		connection.start().catch(err => console.error(err));
	}
}