import { ref, reactive, watch } from 'vue'
import { templateLoader as ttLoader } from 'mbase/vue-helper.mjs'
import ardConnection from 'mbase/arduino-connection.mjs'

export default ttLoader.buildComponent("ard-state.html", templ => ({
	template: templ,
	setup() {
		//watch(ardConnection.pinStates, st => {
		//	console.log(st);
		//})
		return {
			test: [1,2,3],
			pinStates: ardConnection.pinStates
		}
	},
	//	components: {
	//		fsNavigator,
	//		fsViewer,
	//	}
}));