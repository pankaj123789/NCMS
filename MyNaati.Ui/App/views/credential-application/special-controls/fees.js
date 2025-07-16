define(['components/views/wizard-parts/fees-embedded'],
    function (fees) {
        return {
            getInstance: getInstance
        };

        function getInstance(options) {
            var credentialApplication = options.credentialApplication;

			var feesInstance = fees.getInstance();
			var serverModel = feesInstance.paymentMethod;

			var vm = {
                applicationId: options.applicationId,
                applicationFormId: options.applicationFormId,
                question: options.question,
                token: options.token,
				validation: feesInstance.validation,
				canContinue: ko.observable(false),
			};

			vm.feesOptions = {
				model: feesInstance,
                view: 'components/views/wizard-parts/fees-embedded',
			};

            vm.load = function () {
                vm.canContinue(false);
                credentialApplication.fees({ ApplicationFormId: vm.applicationFormId(), Token: vm.token() }).then(function (data) {
                    feesInstance.fees(data);
					ko.viewmodel.updateFromModel(serverModel, emptyModel);
                    vm.canContinue(true);
                    feesInstance.resetValidation();
                });
            };

            init();

            function init() {
                for (var propertyName in serverModel) {
                    var property = serverModel[propertyName];
                    if (ko.isObservable(property)) {
                        property.subscribe(updateResponse);
                    }
                }
            }

            function updateResponse() {
                if (!serverModel.PaymentMethodType()) {
                    return;
                }
                var request = ko.toJS(serverModel);
                vm.question.Response(JSON.stringify(request));
            }

            return vm;
        }
    });