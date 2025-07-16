define(['services/requester'],
	function (requester) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
            var credentialApplication = requester('credentialapplication');
            var year = new Date().getFullYear();
			var serverModel = {
				PaymentMethodType: ko.observable(),
				PayPalSurcharge: ko.observable(),
				CreditCardNumber: ko.observable().extend({
					minLength: {
						params: 14,
						message: 'Please enter a valid Card Number with 14 min digits and max 16 digits.'
					},
					maxLength: {
						params: 16,
						message: 'Please enter a valid Card Number with 14 min digits and max 16 digits.'
					}
				}),
				CreditCardName: ko.observable().extend({
					maxLength: {
						params: 30,
						message: 'Please enter no more than 30 characters in the Name on Card.'
					}
				}),
				CreditCardExpiryMonth: ko.observable().extend({
					min: {
						params: 1,
						message: 'Please enter an Expiry Month greater than or equal to {0}.'
					},
					max: {
						params: 12,
						message: 'Please enter an Expiry Month less than or equal to {0}.'
					}
				}),
				CreditCardExpiryYear: ko.observable().extend({
					min: {
						params: year,
						message: 'Please enter an Expiry Year greater than or equal to {0}.'
					},
					max: {
						params: year + 10,
						message: 'Please enter an Expiry Year less than or equal to {0}.'
					}
				}),
				CreditCardCCV: ko.observable().extend({
					min: {
						params: 1,
						message: 'Please enter a CCV greater than or equal to {0}.'
					},
					max: {
						params: 999,
						message: 'Please enter a CCV less than or equal to {0}.'
					},
				}),
				CreditCardFailMessageLine1: ko.observable(),
				CreditCardFailMessageLine2: ko.observable()

			};

			var creditCardRequired = {
				required: {
					onlyIf: ko.pureComputed(function () {
						return serverModel.PaymentMethodType() === ENUMS.PaymentMethods.CreditCard;
					})
				}
			};

			
			serverModel.CreditCardNumber.extend($.extend(true, { required: { message: 'The Card Number is required.' } }, creditCardRequired));
			serverModel.CreditCardName.extend($.extend(true, { required: { message: 'The Name on Card is required.' } }, creditCardRequired));
			serverModel.CreditCardExpiryMonth.extend($.extend(true, { required: { message: 'The Expiry Month is required.' } }, creditCardRequired));
			serverModel.CreditCardExpiryYear.extend($.extend(true, { required: { message: 'The Expiry Year is required.' } }, creditCardRequired));
			serverModel.CreditCardCCV.extend($.extend(true, { required: { message: 'The CCV is required.' } }, creditCardRequired));

			var emptyModel = ko.toJS(serverModel);

			serverModel.PaymentMethodType.subscribe(changePaymentMethodType);

			serverModel.CreditCardExpiry = ko.computed(function () {
				if (serverModel.CreditCardExpiryMonth() && serverModel.CreditCardExpiryYear()) {
					return serverModel.CreditCardExpiryMonth() + '/' + serverModel.CreditCardExpiryYear();
				}

				return null;
			}).extend(creditCardRequired);

			var validation = ko.validatedObservable(serverModel);

			var vm = {
				paymentMethod: serverModel,
				fees: ko.observableArray(),
                validation: validation,
                creditCardOptions: ko.observableArray()
            };

            serverModel.CreditCardOptions = ko.computed({
                read: function () {
                    return vm.creditCardOptions();
                },
                write: function () {}
            });

			serverModel.PaymentMethodType.extend({
				required: {
					onlyIf: ko.computed(function () {
						return vm.fees().length;
					})
				}
			});

			serverModel.Total = ko.computed(function () {
				return sum('Total');
			});

			vm.totalExGST = ko.computed(function () {
				return sum('ExGST');
			});

			vm.totalGST = ko.computed(function () {
				return sum('GST');
            });

			vm.paymentMethods = [
				{
					id: ENUMS.PaymentMethods.CreditCard,
					text: 'Pay with Credit Card (quickest option)',
					template: {
						name: 'creditCard',
						data: serverModel
					}
				},
				{
					id: ENUMS.PaymentMethods.PayPal,
					text: 'Pay with PayPal',
					template: {
						name: 'payPal',
						data: serverModel
					}
				},
				//{ id: ENUMS.PaymentMethods.DirectDeposit, text: 'Pay with Direct Deposit', template: { name: 'directDeposit' } }
			];

            vm.activate = function () {
                credentialApplication.getFluid('availablecreditcards').then(vm.creditCardOptions);
            };

			function sum(property) {
				var total = 0;
				ko.utils.arrayForEach(vm.fees(), function (f) {
					total += f[property];
				});
				return total;
			}

			function resetValidation() {
				if (validation.errors) {
					validation.errors.showAllMessages(false);
				}
			}

			function changePaymentMethodType() {
				resetValidation();
			}

		    vm.resetValidation = resetValidation;
			return vm;
		}
	});