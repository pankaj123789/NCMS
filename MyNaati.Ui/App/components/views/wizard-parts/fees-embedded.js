define(['services/requester'],
	function (requester) {
		var applications = requester('applications');
		return {
			getInstance: getInstance
		};

		function getInstance(options) {
			var credentialApplication = requester('credentialapplication');
			var year = new Date().getFullYear();
			var embeddedSecurePayUI;
			var payPalUiIsVisible;
			var containerId = 'securepay-ui-container';

			var serverModel = {
				PaymentMethodType: ko.observable(),
				CreditCardFailMessageLine1: ko.observable(),
				CreditCardFailMessageLine2: ko.observable(),
				CreditCardToken: ko.observable(),
				PayPalModel: ko.observable(),
				PayPalMessageLine: ko.observable(),
				PayerId: ko.observable()
			};

			serverModel.PayPalModel({
				NaatiReference: ko.observable(),
				NaatiUnitType: ko.observable(),
				OrderId: ko.observable(),
				PayerId: ko.observable(),
				PaymentId: ko.observable(),
				Actions: ko.observable(),
				Complete: ko.observable()
			});

			serverModel.PayPalModel().NaatiReference(options.naatiReference);
			serverModel.PayPalModel().NaatiUnitType(options.naatiUnitType);
			serverModel.PaymentMethodType.subscribe(changePaymentMethodType);
			serverModel.PayPalModel().Complete(false);

			var vm = {
				paymentMethod: serverModel,
				fees: ko.observableArray(),
				creditCardOptions: ko.observableArray(),
				paymentGatewayConfig: ko.observable(),
				readOnly: ko.observable(true)
			};

			serverModel.CreditCardToken.extend({
				validation: {
					validator: function (val) {
						return val != 'fail';
					},
					message: 'Invalid credit card.',
				}
			});

			serverModel.PaymentMethodType.extend({
				required: {
					onlyIf: ko.pureComputed(function () {
						return vm.fees().length;
					})
				}
			});

			var validation = ko.validatedObservable(serverModel);
			vm.validation = {
				isValid: function () {
					//serverModel.CreditCardToken('');
					var valid = validation.isValid();

					if (!valid) {
						return false;
					}

					if ((serverModel.PaymentMethodType() === ENUMS.PaymentMethods.PayPal) && !serverModel.PayPalModel().Complete()) {

						return false;
					}

					if ((serverModel.PaymentMethodType() === ENUMS.PaymentMethods.CreditCard) && !embeddedSecurePayUI) {
						return false;
					}

					if (serverModel.PaymentMethodType() === ENUMS.PaymentMethods.CreditCard)
					{
						embeddedSecurePayUI.tokenise();
					}

					return true;

					//below is effectively disabled. Enabling causes the next step to not appear.
					//doesnt appear to be any adverse effect in it not being called.
					var validationDefer = Q.defer();
					tokenDefer = Q.defer();

					tokenDefer.promise
						.then(function () {
							valid = validation.isValid();
							valid ? markAsReadOnly() : null;
							validationDefer.resolve(valid);
						});

					return validationDefer.promise;
				},
				errors: validation.errors
			};

			var tokenDefer;

			serverModel.CreditCardOptions = ko.computed({
				read: function () {
					return vm.creditCardOptions();
				},
				write: function () { }
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

			vm.zeroTotal = ko.computed(function () {
				return serverModel.Total() === 0;
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
					text: 'Pay with PayPal (PayPal Loading applies)',
					template: {
						name: 'payPal',
						data: serverModel
					}
				},
				//{ id: ENUMS.PaymentMethods.DirectDeposit, text: 'Pay with Direct Deposit', template: { name: 'directDeposit' } }
			];

			if (options.disablePayPal) {
				var payPalElement = vm.paymentMethods.find(x => { return x.id === ENUMS.PaymentMethods.PayPal })
				var payPalElementIndex = vm.paymentMethods.indexOf(payPalElement);
				if (payPalElementIndex > -1) {
					vm.paymentMethods.splice(payPalElementIndex,1);
					}
			}

			vm.activate = function () {
				loadSettings();
			};

			function markAsReadOnly() {
				vm.readOnly(true);

				var $container = $('#' + containerId);
				var div = '\
<div style="background-color: #fff;\
	opacity: .5;\
	position: absolute;\
	top: 0px;\
	bottom: 0px;\
	left: 0px;\
	right: 0px">\
</div>'

				$container.css('position', 'relative');
				$(div).appendTo($container);
			}

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

				if (serverModel.PaymentMethodType() === ENUMS.PaymentMethods.DirectDeposit) {
					serverModel.PayPalModel().Complete(false);
					if (vm.fees().length > 1) //remove the surcharge to the items
					{
						var last = vm.fees()[vm.fees().length - 1];
						if (last.Code !== 'SCHOLARSHIPNAATIFUND') { //to stop the removal of the scholarship offset (in case it is funded by NAATI)
							vm.fees.pop();
						}
					}
				}

				if (serverModel.PaymentMethodType() === ENUMS.PaymentMethods.CreditCard) {
					//embed credit card
					if (vm.fees().length > 1) //remove the surcharge to the items
					{
						vm.fees.pop();
					}

					if (!embeddedSecurePayUI) {
						serverModel.PayPalModel().Complete(false);
						embeddedSecurePayUI = new securePayUI.init({
							containerId: containerId,
							scriptId: 'securepay-ui-js',
							clientId: vm.paymentGatewayConfig().PaymentGatewayClientId,
							merchantCode: vm.paymentGatewayConfig().PaymentGatewayMerchantCode,
							card: { // card specific config and callbacks
								allowedCardTypes: vm.creditCardOptions(),
								showCardIcons: true,
								onTokeniseSuccess: function (tokenisedCard) {
									// card was successfully tokenised
									serverModel.CreditCardToken(tokenisedCard.token);
									tokenDefer.resolve();
								},
								onTokeniseError: function (error) {
									// card was successfully tokenised
									serverModel.CreditCardToken('fail');
									tokenDefer.resolve();
								}
							},
							onLoadComplete: function () {
								// the SecurePay UI Component has successfully loaded and is ready to be interacted with
								$(".securepay-ui-iframe").css("border-width", "0px");
							}
						});
					}
				}

				if (serverModel.PaymentMethodType() === ENUMS.PaymentMethods.PayPal) {

					if (vm.fees().length == 1) //add the surcharge to the items
					{
						var surchargePercentage = vm.fees()[0].PayPalSurcharge;
						var surchargeAmount = Math.round((vm.fees()[0].PayPalSurcharge * vm.fees()[0].Total) * 100)/100;
						vm.fees.push({ Code: 'BANKCHARGES', Description: 'PayPal Loading', ExGST: surchargeAmount, GST: 0, Total: surchargeAmount });
					}

					if (!payPalUiIsVisible) {
						if (typeof (paypal) == 'undefined') {
							serverModel.PayPalModel().Complete(true);
							serverModel.PayPalMessageLine("<p>PayPal seems not reachable. Please select another payment option</p>");
						}
						else {
							paypal.Buttons({
								style: {
									color: 'blue',
								},
								createOrder: function () {
									var request = {
										amount: serverModel.Total(),
										reference: serverModel.PayPalModel().NaatiReference(),
										unitType: serverModel.PayPalModel().NaatiUnitType()
									}
									return applications.post(request,'selecttestsession/authorisePayPal')
										.then(function (res) {
											return res.token;
										});
								},
								onApprove: function (data, actions) {
									recordPayPalAuthorisation(data, actions);
									return actions.order.get().then(function (orderDetails) {
										$('#paypal-button-container').hide();
										markAsReadOnly();
										serverModel.PayPalMessageLine("<p>Your PayPal payment has been authorised, click <b>Next</b> and complete to finalise payment</p>");
										serverModel.PayPalModel().Complete(true);
									});
								},
								onCancel: function (data, actions) {
									alert('An error has occurred processing the PayPal payment');
								},

								onError: function (data, actions) {
									alert('An error has occurred processing the PayPal payment');
								},
								//TODO log this
								onError: function (err) {
									alert('An error has occurred processing the PayPal payment');
								}
							}).render('#paypal-button-container');
							payPalUiIsVisible = true;
						}
					}

				}
			}

			function recordPayPalAuthorisation(data,actions)
			{
				serverModel.PayPalModel().OrderId(data.orderID);
				serverModel.PayPalModel().PayerId(data.payerID);
				serverModel.PayPalModel().PaymentId(data.paymentID);
				serverModel.PayerId(data.payerID);
			}

			function loadSettings() {
				var promises = [];

				promises.push(credentialApplication.getFluid('availablecreditcards')
					.then(function (data) {
						vm.creditCardOptions(data);
						return true;
					})
				);

				promises.push(credentialApplication.getFluid('getpaymentconfiguration')
					.then(function (data) {
						vm.paymentGatewayConfig(data);
						return true;
					})
				);

				Promise.all(promises).then(function (values) {
					for (var i = 0; i < values.length; i++) {
						var value = values[i];
						if (!value) {
							return vm.readOnly(true);
                        }
					}

					vm.readOnly(false);
				});
			}

			vm.resetValidation = resetValidation;
			return vm;
		}
	});