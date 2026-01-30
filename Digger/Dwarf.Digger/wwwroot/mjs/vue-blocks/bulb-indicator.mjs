import { ref, reactive } from 'vue'
import { templateLoader as ttLoader } from 'mbase/vue-helper.mjs'

export default ttLoader.buildComponent("bulb-indicator.html", templ => ({
	template: templ,
	props: {
		title: String,
		on: Number
	},
	setup(props) {
		return {
		}
	},
	//	components: {
	//		fsNavigator,
	//		fsViewer,
	//	}
}));