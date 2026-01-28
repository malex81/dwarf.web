import { ref, reactive } from 'vue'
import { templateLoader as ttLoader } from 'mbase/vue-helper.mjs'

export default ttLoader.buildComponent("indicator.html", templ => ({
	template: templ,
	setup() {
		return {
			st: "1232-eee"
		}
	},
	//	components: {
	//		fsNavigator,
	//		fsViewer,
	//	}
}));