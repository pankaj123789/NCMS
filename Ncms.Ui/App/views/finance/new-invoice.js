define([
	'plugins/router',
	'views/shell',
	'modules/common',
	'services/screen/message-service',
	'views/finance/new-invoice-parts/add-invoice-detail',
	'views/finance/new-invoice-parts/invoice-creation-result',
    'services/finance-data-service',
    'modules/enums'
],
	function (router, shell, common, message, addDetail, invoiceCreationResult, financeService, enums) {
		var commonFunctions = common.functions();

		var serverModel = {
			EntityId: ko.observable().extend({ required: true }),
			NaatiNumber: ko.observable().extend({ required: true }),
			Name: ko.observable(),
			Office: ko.observable().extend({ required: true }),
			DueDate: ko.observable(moment().add(3, 'day').toDate()).extend({ required: true }),
			BrandingThemeId: ko.observable().extend({ required: true }),
			LineItems: ko.observableArray([]),
			ReferenceText: ko.observable()
		};

		var cleanServerModel = ko.toJS(serverModel);

		serverModel.EntityId.subscribe(enableDisableDataTableButtons);

		var vm = {
			invoice: serverModel,
			defaultBranding: ko.observable(),
			title: shell.titleWithSmall,
			addDetailInstance: addDetail.getInstance(),
			invoiceCreationResultInstance: invoiceCreationResult.getInstance(),
			tableDefinition: {
				headerTemplate: 'new-invoice-details-header-template',
				rowTemplate: 'new-invoice-details-row-template'
			}
		};

		var validation = ko.validatedObservable(serverModel);

        var disableAdvancedOptions = ko.observable(true);
        currentUser.hasPermission(enums.SecNoun.Invoice, enums.SecVerb.Create).then(function(hasPermission) {
	        disableAdvancedOptions(!hasPermission);
	    });

		vm.invoiceCreationResultInstance.event.progress(processResultModalEvent);
		vm.defaultBranding.subscribe(serverModel.BrandingThemeId);

		vm.invoiceToOptions = {
			source: function (query, callback) {
				financeService.getFluid('customer', { term: query }).then(callback);
			},
			event: {
				keydown: function (data, event) {
					// Allow: backspace, delete, tab, escape, and enter
					if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
						// Allow: Ctrl+A
						(event.keyCode === 65 && event.ctrlKey === true) ||
						// Allow: . ,
						(event.keyCode === 188 || event.keyCode === 190 || event.keyCode === 110) ||
						// Allow: home, end, left, right
						(event.keyCode >= 35 && event.keyCode <= 39)) {
						// let it happen, don't do anything
						return true;
					}
					else {
						// Ensure that it is a number and stop the keypress
						if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
							event.preventDefault();

							return false;
						}
					}

					return true;
				}
			},
			multiple: false,
			valueProp: 'EntityId',
			labelProp: 'Name',
			template: 'invoiceto-template',
			textValue: serverModel.Name,
			value: serverModel.EntityId,
			resattr: {
				placeholder: 'Naati.Resources.Finance.resources.InvoiceTo'
			},
			select: function (e, data) {
				if (data.item.data) {
					serverModel.NaatiNumber(data.item.data.NaatiNumber);
				}
			}
		};

		vm.officeOptions = {
			options: ko.observableArray(),
			value: serverModel.Office,
			multiple: false,
			optionsValue: 'Abbreviation',
			optionsText: 'Name',
			enableWithPermission: 'FinanceOther.Update'
		};

		vm.brandingOptions = {
			options: ko.observableArray(),
			value: serverModel.BrandingThemeId,
			multiple: false,
			optionsValue: 'BrandingThemeId',
			optionsText: 'Name',
			disable: true
		};

		vm.dueDateOptions = {
			value: serverModel.DueDate,
            disable: disableAdvancedOptions,    
            resattr: {
				placeholder: 'Naati.Resources.Finance.resources.DueDate'
			}
		};

		vm.tableDefinition.dataTable = {
			source: serverModel.LineItems,
			buttons: {
				dom: {
					button: {
						tag: 'label',
						className: ''
					},
					buttonLiner: {
						tag: null
					}
				},
				buttons: [{
					enabled: false,
					text: '<span class="glyphicon glyphicon-plus"></span><span>' +
						ko.Localization('Naati.Resources.Finance.resources.AddInvoiceDetail') +
						'</span>',
					className: 'btn btn-success',
					action: function () {
						vm.addDetailInstance.add(ko.toJS(serverModel)).then(function (data) {
							var lineItems = serverModel.LineItems();
							serverModel.LineItems([]);
							lineItems.push(data);
							serverModel.LineItems(lineItems);
						});
					}
				}]
			},
			initComplete: function () {
				enableDisableDataTableButtons(serverModel.EntityId());
			}
		};

		vm.invoice.Reference = ko.computed(function () {
			var office = selectedOffice();
			if (!office) {
				return '-';
			}

			var referenceText = serverModel.ReferenceText();
			return '{0}{1}{2}'.format(office.Abbreviation, referenceText ? '-' : '', referenceText);
		});

		vm.officeOptions.options.subscribe(selectOfficeIfSingle);

		vm.activate = function (naatiNumber) {
			if (naatiNumber) {
				financeService.getFluid('customer', { term: naatiNumber }).then(
                    function (data) {
                        var entity = ko.utils.arrayFirst(data, function (d) {
                            return d.NaatiNumber == naatiNumber;
                        }) || {};
						serverModel.EntityId(entity.EntityId);
                        serverModel.Name(entity.Name);
                        serverModel.NaatiNumber(entity.NaatiNumber);
					});
			}

			financeService.getFluid('offices').then(function (data) {
				vm.officeOptions.options(data);
				setCurrentUserOffice();
			});

			financeService.getFluid('invoiceoptions').then(function (data) {
				vm.defaultBranding(data.DefaultBranding.toString());
				cleanServerModel.BrandingThemeId = vm.defaultBranding();
				vm.brandingOptions.options(data.SalesInvoiceBrandings);
			});

			clearValidation();
		};

		vm.edit = function (item) {
			vm.addDetailInstance.edit(item, ko.toJS(serverModel)).then(function (newValue) {
				var index = serverModel.LineItems.indexOf(item);

				if (index !== -1) {
					var lineItems = serverModel.LineItems();

					lineItems[index] = newValue;

					serverModel.LineItems(lineItems); // Notify subcribers
				}
			});
		};

		vm.remove = function (item) {
			message.remove({
				yes: '<span class="glyphicon glyphicon-trash"></span><span>' +
					ko.Localization('Naati.Resources.Shared.resources.Yes') +
					'</span>',
				no: ko.Localization('Naati.Resources.Shared.resources.No'),
				content: ko.Localization('Naati.Resources.Finance.resources.DeleteInvoiceDetailConfirmation')
			}).then(function (answer) {
				if (answer === 'yes') {
					var lineItems = serverModel.LineItems();
					serverModel.LineItems([]);
					var index = lineItems.indexOf(item);
					if (index !== -1) {
						lineItems.splice(index, 1);
						serverModel.LineItems(lineItems);
					}
				}
			});
		};

		vm.createAndMakePayment = function () {
			var saveDefer = save();
			if (saveDefer) {
				saveDefer.then(function (data) {
					forceClear();
					router.navigate('finance/new-payment/' + data.InvoiceNumber);
				});
			}
		};

		vm.createWithoutPayment = function () {
			var saveDefer = save();
			if (saveDefer) {
				saveDefer.then(function (data) {
					vm.invoiceCreationResultInstance.show(data.InvoiceNumber);
				});
			}
		};

		function save() {
			if (!validation.isValid()) {
				validation.errors.showAllMessages();
				return false;
			}

			if (!serverModel.LineItems().length) {
				message.alert({
					title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
					content: ko.Localization('Naati.Resources.Finance.resources.NoInvoiceLines')
				});

				return false;
			}

			return commonFunctions.performOperation({
				service: financeService,
				data: ko.toJS(vm.invoice),
				action: 'invoice',
				retryAction: 'retryInvoice',
				cancelAction: 'cancel',
				errorMessage: ko.Localization('Naati.Resources.Finance.resources.ErrorOnInvoiceCreation')
			});
		};

		vm.clear = function () {
			message.confirm({
				title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
				content: ko.Localization('Naati.Resources.Finance.resources.ClearInvoiceConfirmation')
			}).then(function (answer) {
				if (answer === 'yes') {
					forceClear();
				}
			});
		};

		function forceClear() {
			ko.viewmodel.updateFromModel(vm.invoice, cleanServerModel);
			serverModel.LineItems([]);
			setCurrentUserOffice();
			clearValidation();
		}

		function selectedOffice() {
			return ko.utils.arrayFirst(vm.officeOptions.options(),
				function (p) {
					return p.Abbreviation === vm.invoice.Office();
				});
		}

		function clearValidation() {
			if (validation.errors) {
				validation.errors.showAllMessages(false);
			}
		}

		function retryEnableDisableDataTableButtons(newValue) {
			setTimeout(function () {
				enableDisableDataTableButtons(newValue);
			}, 100);
		}

		function enableDisableDataTableButtons(newValue) {
			if (!$.fn.DataTable.isDataTable($('#' + vm.tableDefinition.component.id))) {
				retryEnableDisableDataTableButtons(newValue);
				return;
			}

			var buttons = $('#' + vm.tableDefinition.component.id).DataTable().buttons('*');
			if (!buttons.length) {
				retryEnableDisableDataTableButtons(newValue);
				return;
			}

			if (newValue) {
				buttons.enable();
			} else {
				buttons.disable();
			}
		}

		function processResultModalEvent(event) {
			if (event.name === 'NewInvoice') {
				forceClear();
			}
		}

		function selectOfficeIfSingle() {
			var offices = vm.officeOptions.options();
			if (offices.length === 1) {
				serverModel.Office(offices[0].Abbreviation);
			}
		}

		function setCurrentUserOffice() {
			var office = ko.utils.arrayFirst(vm.officeOptions.options(),
				function (p) {
					return p.Id === currentUser.OfficeId;
				});
			if (office) {
				serverModel.Office(office.Abbreviation);
			}
		}

		return vm;
	});
