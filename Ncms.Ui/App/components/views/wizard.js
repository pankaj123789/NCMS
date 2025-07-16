define([
	'services/util',
], function (util) {
	function ViewModel(params) {
		var self = this;

		params.component = self;

		self.id = util.guid();
		self.timelineOptions = {
			items: params.steps
		};
		self.header = params.header;
		self.showFinish = params.showFinish;
		self.next = params.next;
		self.finish = params.finish;
		self.cancel = params.cancel;
		self.setStepState = function (step, isSuccess) {
			step.success(isSuccess);
			step.cancel(!isSuccess);
		};
		self.scrollToStep = function (step) {
			var container = $('#' + self.id);
			var scrollTo = $('#' + step.id);

			container.animate({
				scrollTop: scrollTo.offset().top - container.offset().top + container.scrollTop(),
			}, 'slow');
		};
		self.validateStep = function (step) {
			var defer = Q.defer();

			if (!step.compose.model.isValid) {
				defer.resolve(true);
				return defer.promise;
			}

			var isValidPromise = Promise.resolve(step.compose.model.isValid());
			isValidPromise.then(defer.resolve);

			return defer.promise;
		};

		self.ajaxError = ko.observable(false);

		ko.computed(function () {
			var items = ko.unwrap(params.steps);
			ko.utils.arrayForEach(items,
				function (i) {
					if (!i.keepEditable) {
						ko.computed(function () {
							var success = ko.unwrap(i.success);
							var id = ko.unwrap(i.id);
							var $parent = $('#' + id);
							$parent.find(':input').attr('disabled', success ? 'disabled' : null);
							$parent.find('.dataTable').DataTable().on('user-select',
								function (e) {
									e.preventDefault();
								});

							if (success) {
								$parent.find(':button.multiselect').addClass('disabled');
							} else {
								$parent.find(':button.multiselect').removeClass('disabled');
							}
						});
					}
				});
		});

		var ajaxEventName = 'ajaxError';

		self.activate = function () {
			$(document).on(ajaxEventName + '.wizard', function () {
				self.ajaxError(true);
			});
		};

		self.deactivate = function () {
			self.ajaxError(false);
			$(document).off(ajaxEventName + '.wizard');
		};

		self.activate();
	}

	return ViewModel;
});