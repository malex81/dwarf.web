import { defineAsyncComponent, ref, reactive } from 'vue'
import { templateLoader as tl} from 'mbase/vue-helper.mjs'

export default defineAsyncComponent(async () => {
	const templ = await tl.load(tl.getUrl("indicator.html"));
	return {
		template: templ,
		setup() {
			return {
				st: "1232"
			}
		},
	//	components: {
	//		fsNavigator,
	//		fsViewer,
	//	}
	}
})