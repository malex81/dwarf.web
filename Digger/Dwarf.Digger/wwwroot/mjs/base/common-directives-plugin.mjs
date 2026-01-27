import * as bs from 'bootstrap'
import { nextTick } from 'vue'

export default {
	install(app, options) {
		app.directive('focus', {
			mounted(el, binding) {
				el.focus();
				if(binding.modifiers.select)
					nextTick(() => { el.select(); });
			}
		})
		app.directive('tooltip', {
			mounted(el, binding, vnode) {
				// https://getbootstrap.com/docs/5.3/components/tooltips/#enable-tooltips
				// debugger;
				const tooltip = new bs.Tooltip(el, {
					title: binding.value,
					trigger: 'hover',
					fallbackPlacements: ['top', 'bottom', 'right', 'left']
				})
			}
		})
	}
}