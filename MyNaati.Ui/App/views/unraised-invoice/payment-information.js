define(['services/requester'],
	function (requester, fees) {

		var unraisedinvoices = new requester('unraisedinvoices');

		return {
			getInstance: getInstance
		};

		function getInstance(options) {

			var vm = {
				canContinue: ko.observable(false),
				credentialApplicationId: ko.observable(),
				preferredTestLocation: ko.observable(),
				applicationReference: ko.observable(),
				credentialRequestsDisplayNames: ko.observable()
			};


			vm.load = function () {
				vm.canContinue(true);
			}



			vm.activate = function () {
				vm.credentialApplicationId(options.credentialApplicationId);
				unraisedinvoices.getFluid('raiseandpayinvoice/information', { credentialApplicationId: vm.credentialApplicationId() }).then(function (data) {
					vm.preferredTestLocation(data.PreferredTestLocation);
					vm.applicationReference(data.ApplicationReference);
					vm.credentialRequestsDisplayNames(data.CredentialRequestsDisplayNames);
					ko.applyBindings(vm);
				})
			}



			return vm;

		};
	});




