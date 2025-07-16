define(['services/requester'],
	function (requester) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var applications = requester('applications');

			var serverModel = {
				TestSessionId: ko.observable(),
				TestLocationName: ko.observable(),
                TestDate: ko.observable(moment().format()),
                TestDateString: ko.observable(),
				CredentialTypeDisplayName: ko.observable(),
				SkillDisplayName: ko.observable(),
				State: ko.observable(),
			};

			var vm = {
                testAttendanceId: options.testAttendanceId || ko.observable(),
                testSessionId: options.testSessionId || ko.observable(),
                credentialRequestId: options.credentialRequestId || ko.observable(),
                credentialApplicationId: options.credentialApplicationId || ko.observable(),
				serviceName: options.serviceName,
				testSession: serverModel,
				canContinue: ko.observable(false),
			};

			vm.load = function () {
                vm.canContinue(false);
                var req = {
                    TestAttendanceId: vm.testAttendanceId(),
                    TestSessionId: vm.testSessionId(),
                    CredentialRequestId: vm.credentialRequestId(),
                    CredentialApplicationId: vm.credentialApplicationId()
                };
                applications.getFluid(vm.serviceName + '/testdetail', req).then(function (data) {
                    var testDate = getDate(data.TestDate);
                    ko.viewmodel.updateFromModel(serverModel, data);
                    serverModel.TestDate(testDate);
					vm.canContinue(true);
				});
            };

		    function getDate(date) {

		        date = date.replace('Z', '');
		        date = moment(date).format('YYYY-MM-DDTHH:mm:ss');
		        return moment(date + moment().format("Z")).format();
		    }

			return vm;
		}
	});