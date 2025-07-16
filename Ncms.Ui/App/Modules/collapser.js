define([], function () {
	return {
		create: create
	};

	function create(model) {
		return new collapser(model);
	}

	function collapser(model) {
		var array = [];

		if (ko.isObservable(model)) {
			array = model();

			model.subscribe(function () {
				array = model();
				init();
			});
		}

		if (!$.isArray(array)) {
			throw 'Model is not an array';
		}

		var self = this;
		var openControl = {};

		init();

		self.changeState = function () { };

		self.expandAll = function () {
			expandCollapse(true);
		};

		self.collapseAll = function () {
			expandCollapse(false);
		};

		self.toggleCollapse = function (data) {
			var index = array.indexOf(data);
			openControl[index](!openControl[index]());
			self.changeState({ data: data, opened: openControl[index]() });
		};

		self.isOpened = function (data) {
			var index = array.indexOf(data);
			return openControl[index]();
		};

		function expandCollapse(expand) {
			$.each(array, function (i, s) {
				openControl[i](expand);
				self.changeState({ data: s, opened: expand });
			});
		}

		function init() {
			$.each(array, function (i, s) {
				openControl[i] = ko.observable(array.length === 1);
			});
		}
	}
});