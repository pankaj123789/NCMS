define([
	'services/util',
	'services/person-data-service',
	'services/application-data-service',
	'services/screen/date-service',
	'modules/enums',
], function (util, personService, applicationService, dateService, enums) {
	return {
		getInstance: getInstance
	};

	function getInstance(params) {
		var serverModel = {
			CredentialId: ko.observable(),
			CertificationPeriodId: ko.observable().extend({
				required: {
					param: true,
					message: ko.Localization('Naati.Resources.Application.resources.SelectAPeriod')
				}
			}),
			Notes: ko.observable('').extend({ required: true })
		};

		var emptyModel = ko.toJS(serverModel);

		var validation = ko.validatedObservable(serverModel);

		var vm = {
			person: params.person,
			modalId: util.guid(),
			periods: ko.observableArray(),
			credential: serverModel,
			visible: ko.observable(false),
			windowTitle: ko.observable(),
		};

		vm.tableDefinition = {
			id: 'moveCredentialTable',
			headerTemplate: 'move-credentials-header-template',
			rowTemplate: 'move-credentials-row-template',
			dataTable: {
				source: vm.periods,
				dom: "<'row'<'col-sm-12'tr>>" +
					"<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
				searching: false,
				paging: false,
				oLanguage: { sInfoEmpty: '', sInfo: '' },
				columnDefs: [
					{
						orderable: false,
						className: 'select-checkbox',
						targets: 0
					},
					{
						targets: [1, 2, 3],
						render: $.fn.dataTable.render.moment(moment.ISO_8601, CONST.settings.shortDateDisplayFormat)
					}
				],
                select: {
					style: 'single',
					info: false
				},
				events: {
					select: select,
					deselect: select,
				}
			}
		};

		var defer;
		vm.show = function (credential) {
			ko.viewmodel.updateFromModel(serverModel, emptyModel);
			serverModel.CredentialId(credential.Id);
			vm.windowTitle(ko.Localization('Naati.Resources.Application.resources.MoveCredential') + ' - ' + credential.CredentialTypeInternalName + ' ' + credential.Direction);

			if (validation.errors) {
				validation.errors.showAllMessages(false);
			}

			var periodsRequest = {
				PersonId: vm.person.PersonId(),
				CertificationPeriodStatus: [
					enums.CertificationPeriodStatus.Expired,
					enums.CertificationPeriodStatus.Current,
					enums.CertificationPeriodStatus.Future
				]
			};

			personService.getFluid('certificationperiods', periodsRequest).then(function (data) {
				vm.periods(ko.utils.arrayFilter(data, function (d) {
					return credential.CertificationPeriodId !== d.Id;
				}));
			});

			defer = Q.defer();

			$('#' + vm.modalId).modal('show');
			vm.visible(true);
			return defer.promise;
		};

		vm.close = function () {
			vm.visible(false);
			$('#' + vm.modalId).modal('hide');
		};

		vm.save = function () {
			if (!validation.isValid()) {
				validation.errors.showAllMessages();
				return;
			}

			var request = ko.toJS(serverModel);

			applicationService.post(request, 'movecredential').then(function (data) {
				toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
				vm.close();
				defer.resolve(request);
			});
		};

		vm.status = function (startDate, endDate) {
			if (moment(endDate).toDate() < new Date()) {
				return ko.Localization('Naati.Resources.Person.resources.Expired');
			}
			if (moment(startDate).toDate() > new Date()) {
				return ko.Localization('Naati.Resources.Person.resources.Future');
			}

			return ko.Localization('Naati.Resources.Person.resources.Current');
		};

		function select(e, dt) {
			var indexes = dt.rows('.selected').indexes();

			if (!indexes.length) {
				return serverModel.CertificationPeriodId(null);
			}

			var period = vm.periods()[indexes[0]];
			serverModel.CertificationPeriodId(period.Id);
		}

		return vm;
	}
});