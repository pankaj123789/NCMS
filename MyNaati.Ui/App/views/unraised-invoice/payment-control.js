define(['services/requester', 'components/views/wizard-parts/fees-embedded'],
	function (requester, fees) {
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var applications = requester('unraisedinvoices');			
			var feesInstance = fees.getInstance(options);

			var serverModel = {
				OrganisationName: ko.observable(),
				Contact: ko.observable(moment().format()),
				Trusted: ko.observable()
			};

            var vm = {
                testAttendanceId: options.testAttendanceId || ko.observable(),
                testSessionId: options.testSessionId || ko.observable(),
                credentialRequestId: options.credentialRequestId || ko.observable(),
				credentialApplicationId: options.credentialApplicationId || ko.observable(),
				serviceName: options.serviceName || ko.observable(),
				question: options.question,
				sponsor: serverModel,
				validation: feesInstance.validation,
				canContinue: ko.observable(false),
			};

			vm.feesOptions = {
				model: feesInstance,
				view: 'components/views/wizard-parts/fees-embedded',
			};

			vm.load = function () {
                vm.canContinue(false);
                var req = {
                    TestAttendanceId: vm.testAttendanceId(),
                    TestSessionId: vm.testSessionId(),
                    CredentialRequestId: vm.credentialRequestId(),
                    CredentialApplicationId: vm.credentialApplicationId()
                };
				applications.getFluid(vm.serviceName + '/paymentcontrol', req).then(function (data) {
					ko.viewmodel.updateFromModel(serverModel, data.Sponsor);
					feesInstance.fees(data.Fees);
					const SCHOLARSHIP_ORG_NAME = 'Scholarship - NAATI funded';
					const isScholarship =serverModel.OrganisationName() && serverModel.OrganisationName().toLowerCase() === SCHOLARSHIP_ORG_NAME.toLowerCase();

					if (isScholarship) {
						const base = ko.toJS(feesInstance.fees()[0]);
						feesInstance.fees.push({
							Code: 'SCHOLARSHIPNAATIFUND',
							Description: 'Scholarship – NAATI funded',
							ExGST: -base.ExGST,
							GST: -base.GST,
							Total: -base.Total
						});

						feesInstance.paymentMethod.PaymentMethodType(ENUMS.PaymentMethods.DirectDeposit);
						vm.canContinue(true);
					}
					feesInstance.resetValidation();
					if (feesInstance.zeroTotal()) {
						feesInstance.paymentMethod.PaymentMethodType(ENUMS.PaymentMethods.DirectDeposit);
						vm.canContinue(true);
						return;
					}
				    if (vm.sponsor.OrganisationName()) {
                        feesInstance.paymentMethod.PaymentMethodType(ENUMS.PaymentMethods.DirectDeposit);
				    }
				});
			};

			init();

			function init() {
                for (var propertyName in feesInstance.paymentMethod) {
                    var property = feesInstance.paymentMethod[propertyName];
					if (ko.isObservable(property)) {
						property.subscribe(updateResponse);
					}
				}
			}

			function updateResponse() {
                var request = ko.toJS(feesInstance.paymentMethod);
				vm.question.Response(JSON.stringify(request));
			}

			return vm;
		}
	});