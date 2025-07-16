define(['services/person-data-service'],
	function (personService) {
		return {
			getInstance: getInstance
		};

		function getInstance() {
			var vm = {
				certificationPeriods: ko.observableArray([]),
				defer: null,
				tableDefinition: {
					searching: false,
					paging: false,
					oLanguage: { sInfoEmpty: '', sInfo: '' },
					columnDefs: [
						{
							targets: -1,
							orderable: false
						}, {
							targets: 0,
							render: $.fn.dataTable.render.moment('', CONST.settings.shortDateDisplayFormat)
						}
					],
					order: [[0, "desc"]]
				},
			};

			vm.show = function (certificationPeriods, naatiNumber) {
				vm.certificationPeriods([]);
				vm.defer = Q.defer();
				$('#certificationPeriodsModal').modal('show');

				personService.getFluid(naatiNumber + '/certificationPeriodsRequests').then(function (data) {

					var periods = [];
					ko.utils.arrayForEach(certificationPeriods,
						function (period) {


							if (!period.SubmittedRecertificationApplicationId) {
								return;
							}
							period.RecertificationApplication = 'APP' + period.SubmittedRecertificationApplicationId;
							period.Credentials = '';

							var foundRequest = data.find(function (element) {
								return element.CertificationPeriodId === period.Id;
							});
							if (foundRequest) {

								var reducer = function (acumulator, value) {
									return (acumulator ? acumulator + ',<br/>' : acumulator) + value.ExternalName + ' - ' + value.Skill;
								}

								period.Credentials = foundRequest.Requests.reduce(reducer, '');
							}

							periods.push(period);

						});

					vm.certificationPeriods(periods);
				});

				return vm.defer.promise;

			}

			vm.viewPeriod = function (period) {
				$('#certificationPeriodsModal').modal('hide');
				vm.defer.resolve(period.Id);
			}

			return vm;
		}
	}
);