const templateLoader = {
	getUrl(templateName) { return `/templates/${templateName}`; },
	async load(templateUrl) {
		const resp = await fetch(templateUrl);
		if (!resp.ok)
			throw new Error(`An error on load template '${templateUrl}'`);
		return await resp.text();
	}
}

export { templateLoader };