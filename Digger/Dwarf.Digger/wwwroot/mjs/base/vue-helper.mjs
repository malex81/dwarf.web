import { defineAsyncComponent } from 'vue'

const templateLoader = {
	getUrl(templateName) { return `/templates/${templateName}`; },
	async load(templateUrl) {
		const resp = await fetch(templateUrl);
		if (!resp.ok)
			throw new Error(`An error on load template '${templateUrl}'`);
		return await resp.text();
	},
	buildComponent(templateName, build) {
		const self = this;
		return defineAsyncComponent(async () => {
			const templ = await self.load(self.getUrl(templateName));
			return build(templ);
		})
	}
}

export { templateLoader };