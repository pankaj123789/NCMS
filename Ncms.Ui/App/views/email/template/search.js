define([
	'plugins/router',
	'views/shell',
	'modules/common',
	'modules/enums',
	'services/application-data-service',
	'services/emailTemplate-data-service',
],
	function (router, shell, common, enums, applicationService, emailTemplateService) {
		var vm = {
			title: shell.titleWithSmall,
			filters: [
				{ id: 'ApplicationType' },
                { id: 'SystemActionType' }
			],
			tableDefinition: {
				id: 'emailTemplateTable',
				headerTemplate: 'email-template-header-template',
				rowTemplate: 'email-template-row-template'
			},
			searchType: enums.SearchTypes.EmailTemplate,
			searchTerm: ko.observable({}),
			emailTemplates: ko.observableArray([]),
			computedEmailTemplates: ko.observableArray([]),
            showInactive: ko.observable(false),
            idSearchOptions: {
                search: function (value) {
                    var filter = JSON.stringify({ EmailTemplateIntList: [value] });
                    emailTemplateService.getFluid('search', { request: { Skip: null, Take: 2, Filter: filter }, supressResponseMessages: true })
                        .then(function (data) {
                            if (!data || !data.length) {
                                return toastr.success(ko.Localization('Naati.Resources.Shared.resources.NotFound').format('Email Template', value));
                            }
                            router.navigate('system/email-template/' + value);
                        });
                }
            }
		};

		ko.computed(function () {
			vm.computedEmailTemplates([]);
			var computedEmailTemplates = ko.utils.arrayFilter(vm.emailTemplates(), function (e) {
				return vm.showInactive() || e.Active;
			});
			vm.computedEmailTemplates(computedEmailTemplates);
		});

		vm.parsedSearchTerm = ko.pureComputed(function () {
			var searchTerm = vm.searchTerm();

			var parsedSearchTerm = {};

			if (searchTerm.EmailTemplateName) {
				$.extend(parsedSearchTerm, {
					NaatiNumber: searchTerm.EmailTemplateName.Data.EmailTemplateName
				});
			}

			return parsedSearchTerm;
		});

		vm.tableDefinition.dataTable = {
			source: vm.computedEmailTemplates,
			order: [
				[1, "asc"]
			],
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
					text: '<input type="checkbox" /><i></i><span>' + ko.Localization('Naati.Resources.SystemResources.resources.ShowInactive') + '</span>',
					className: 'i-switch i-switch-md bg-info m-l',
					init: function (dt, node, config) {
						node.children('input[type="checkbox"]').prop('checked', vm.showInactive());
					},
					action: function (e, dt, node, config) {
						var checked = !node.children('input[type="checkbox"]').prop('checked');
						node.children('input[type="checkbox"]').prop('checked', checked);
						vm.showInactive(checked);
					}
				}]
			}
		};

		function parseSearchTerm(searchTerm) {
			var json = {
				Name: searchTerm.EmailTemplateName ? searchTerm.EmailTemplateName.Data.value : null,
				ApplicationTypeIntList: searchTerm.ApplicationType ? searchTerm.ApplicationType.Data.selectedOptions : null,
                SystemActionTypeIntList: searchTerm.SystemActionType ? searchTerm.SystemActionType.Data.selectedOptions : null,
			};

			return JSON.stringify(json);
		}

		vm.activate = function () {
			vm.showInactive(false);
		};

		vm.searchCallback = function () {
			emailTemplateService.getFluid('search', { request: { Skip: null, Take: 500, Filter: parseSearchTerm(vm.searchTerm()) } })
                .then(vm.emailTemplates);
		};

		return vm;
	});
