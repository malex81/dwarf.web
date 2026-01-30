import { ref, reactive, watch, computed } from 'vue'
import { templateLoader as ttLoader } from 'mbase/vue-helper.mjs'
import ardConnection from 'mbase/arduino-connection.mjs'
import bulbIndicator from './bulb-indicator.mjs'

export default ttLoader.buildComponent("ard-state.html", templ => ({
	template: templ,
	setup() {
		//watch(ardConnection.pinStates, st => {
		//	console.log(st);
		//})
		return {
			test: [1, 2, 3],
			digPins: computed(() => ardConnection.pinStates.value.filter(p => !p.isAnalog)),
			analPins: computed(() => ardConnection.pinStates.value.filter(p => p.isAnalog)),
		}
	},
	components: {
		bulbIndicator,
	}
}));