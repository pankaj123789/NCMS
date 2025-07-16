define([
    'modules/common',
    'services/screen/date-service',
    'services/panel-member-data-service',
    'services/entity-data-service',
    'services/screen/message-service',
    'modules/enums'
], function (common, dateService, panelMemberDataService, entityService, messageService, enums) {
    function getMember() {
        var member = {
            PanelMembershipId: ko.observable(),
            PanelId: ko.observable().extend({ required: true }),
            PersonId: ko.observable().extend({ required: true }),
            PanelRoleId: ko.observable().extend({ required: true }),
            StartDate: ko.observable().extend({ required: true }),
            EndDate: ko.observable().extend({ required: true }),
            CredentialTypeIds: ko.observableArray([]),
            MaterialCredentialTypeIds: ko.observableArray([]),
            CoordinatorCredentialTypeIds: ko.observableArray([]),
            NaatiNumber: ko.observable()
        };

		return member;
	}

	function getInstance() {
		var defer = null;

        var vm = {
            editMode: ko.observable(false),
            title: ko.pureComputed(function() {
                return vm.editMode()
                    ? 'Naati.Resources.Panel.resources.EditMember'
                    : 'Naati.Resources.Panel.resources.AddMember';
            }),
            currentMember: getMember(),
            naatiNumber: ko.observable(''),
            personName: ko.observable(''),
            setDetails: function(member) {
                var startDateFormatted = moment(member.StartDate).format(CONST.settings.shortDateDisplayFormat);
                var endDateFormatted = moment(member.EndDate).format(CONST.settings.shortDateDisplayFormat);
        
                vm.currentMember.PanelMembershipId(member.PanelMembershipId);
                vm.currentMember.PanelId(member.PanelId);
                vm.currentMember.PersonId(member.PersonId);
                vm.currentMember.NaatiNumber(member.NaatiNumber);
                vm.naatiNumber(member.NaatiNumber);
                vm.personName(member.PersonName);
                vm.currentMember.PanelRoleId(member.PanelRoleId);
                vm.currentMember.StartDate(startDateFormatted);
                vm.currentMember.EndDate(endDateFormatted);
                setCredentialTypes(member.CredentialTypeIds, member.MaterialCredentialTypeIds, member.CoordinatorCredentialTypeIds);
            },
            clearDetails: function() {
                vm.currentMember.PanelMembershipId(null);
                vm.currentMember.PanelId(null);
                vm.currentMember.PersonId(null);
                vm.naatiNumber(null);
                vm.personName(null);
                vm.currentMember.PanelRoleId(null);
                vm.currentMember.StartDate(null);
                vm.currentMember.EndDate(null);
                vm.currentMember.CredentialTypeIds([]);
                vm.clearValidation();
                vm.currentMember.NaatiNumber(null);
            },
            setNewDetails: function(panelId) {
        
                vm.clearDetails();
                vm.currentMember.PanelId(panelId);
                setCredentialTypes([]);
        
            },
            createMember: function(panelId) {
                defer = Q.defer();
        
                vm.editMode(false);
                vm.setNewDetails(panelId);
                $('#memberModal').modal('show');
        
                return defer.promise;
            },
            editMember: function(member) {
                defer = Q.defer();
        
                vm.editMode(true);
                vm.setDetails(member);
                $('#memberModal').modal('show');
        
                return defer.promise;
            },
            validate: function() {
                var isValid = vm.validation.isValid();
        
                vm.validation.errors.showAllMessages(!isValid);
        
                return isValid;
            },
            clearValidation: function() {
                vm.validation.errors.showAllMessages(false);
            },
            parseMember: function() {
                var parsedMember = ko.toJS(vm.currentMember);
        
                parsedMember.StartDate = dateService.format(parsedMember.StartDate);
                parsedMember.EndDate = dateService.format(parsedMember.EndDate);

                return parsedMember;
            },
            save: function() {
                if (vm.validate()) {
                    var request = new Array();
                    request[0] = {
                        PersonId: vm.parseMember().PersonId,
                        PanelId: vm.parseMember().PanelId,
                        PanelRoleId: vm.parseMember().PanelRoleId,
                        StartDate: vm.parseMember().StartDate,
                        EndDate: vm.parseMember().EndDate,
                        PanelMembershipId: vm.parseMember().PanelMembershipId
                    };
                    //returns true when an overlapping membership is detected.
                    panelMemberDataService.post({ Items: request }, 'HasOverlappingMembership').then(
                        function(data) {
                            if (data === true) {
                                toastr.error(
                                    ko.Localization('Naati.Resources.Panel.resources.OverlappingPanelMembership'));
                            } else {
                                panelMemberDataService.getFluid('hasPersonEmailAddress/' + vm.parseMember().PersonId)
                                    .then(
                                        function(data) {
                                            if (data === true) {
                                                panelMemberDataService.post(vm.parseMember()).then(function() {
                                                    $('#memberModal').modal('hide');
                                                    toastr.success(
                                                        ko.Localization(
                                                            'Naati.Resources.Shared.resources.SavedSuccessfully'));
                                                    if (vm.parseMember().PanelRoleId === enums.PanelRole.RolePlayer) {
                                                        checkRolePlayerRules();
                                                    }
                                                    defer.resolve(vm.parseMember());
                                                });
                                            } else {
                                                $('#memberModal').modal('hide');
                                                toastr.error(
                                                    ko.Localization('Naati.Resources.Shared.resources.EmailNotFound'));
                                                defer.resolve(vm.parseMember());
                                            }
                                        });
                            }
                        });
                }
            }
        
        };

		$.extend(true, vm, {
			validation: ko.validatedObservable(vm.currentMember),
			naatiNumberOptions: {
				value: vm.naatiNumber,
				source: common.functions().naatiNumberSearch,
				minLength: 1,
				select: function (event, ui) {
					var person = ui.item.data;

                    vm.currentMember.NaatiNumber(person.NaatiNumber);
					vm.currentMember.PersonId(person.PersonId);
					vm.personName(person.Name);

					event.preventDefault();
				},
				valueProp: 'NAATINumber',
				labelProp: 'Name',
				resattr: {
					placeholder: 'Naati.Resources.Shared.resources.NAATINumber'
				}
			},
			personNameOptions: {
				value: vm.personName,
				disable: true,
				resattr: {
					placeholder: 'Naati.Resources.Shared.resources.Name'
				}
			},
			panelRoleOptions: {
				value: vm.currentMember.PanelRoleId,
				multiple: false,
				options: ko.observableArray(),
				optionsValue: 'PanelRoleId',
				optionsText: 'Name',
				multiselect: { enableFiltering: false }

			},
			startDateOptions: {
				value: vm.currentMember.StartDate,
				resattr: {
					placeholder: 'Naati.Resources.Shared.resources.StartDate'
				},
				disable: ko.pureComputed(function () {
					return !vm.currentMember.StartDate();
				})
			},
			endDateOptions: {
				value: vm.currentMember.EndDate,
				resattr: {
					placeholder: 'Naati.Resources.Shared.resources.EndDate'
				},
				disable: ko.pureComputed(function () {
					return !vm.currentMember.EndDate();
				})

			},
			credentialTypeOptions: {
				selectedOptions: vm.currentMember.CredentialTypeIds,
				multiple: true,
				options: ko.observableArray(),
				optionsValue: 'Id',
				optionsText: 'DisplayName',
				//afterRender: function () { panelMemberDataService.getFluid('AvailableCredentialTypes/' + vm.currentMember.PanelId()  + '/' + (vm.currentMember.PanelMembershipId() || 0)) },
				multiselect: { enableFiltering: true }
			},
            materialCredentialTypeOptions: {
                selectedOptions: vm.currentMember.MaterialCredentialTypeIds,
                multiple: true,
                options: ko.observableArray(),
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                multiselect: { enableFiltering: true }
            },
            coordinatorCredentialTypeOptions: {
                selectedOptions: vm.currentMember.CoordinatorCredentialTypeIds,
                multiple: true,
                options: ko.observableArray(),
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                multiselect: { enableFiltering: true }
            },

		});
		vm.credentialTypeOptions.disable = ko.pureComputed(function () {
			return !vm.credentialTypeOptions.options().length;
		});

		vm.currentMember.PanelRoleId.subscribe(setMembershipEndDate);

		vm.activate = function () {

			var defer = Q.defer();
			var deferArray = [];
			deferArray.push(entityService.getFluid('panelRoleCategory'));
			deferArray.push(entityService.getFluid('panelRole'));
			Promise.all(deferArray).then(function (results) {
				var categories = results[0];
				var roles = results[1];

				ko.utils.arrayForEach(roles,
					function (role) {

						var filteredCategory = ko.utils.arrayFilter(categories,
							function (category) {
								return category.PanelRoleCategoryId === role.PanelRoleCategoryId;
							})[0];

						role.MembershipDurationMonths = filteredCategory.MembershipDurationMonths;

					});

				vm.panelRoleOptions.options(roles);
				defer.resolve(true);
			});

			return defer.promise;
		}

        function setCredentialTypes(credentialTypesIds, materialCredentialTypeIds, coordinatorCredentialTypeIds) {
			panelMemberDataService.getFluid('AvailableCredentialTypes/' + vm.currentMember.PanelId() + '/' + (vm.currentMember.PanelMembershipId() || 0)).then(function (data) {
                vm.credentialTypeOptions.options(data);
                vm.materialCredentialTypeOptions.options(data);
                vm.coordinatorCredentialTypeOptions.options(data);

                vm.ma.options(data);
                vm.currentMember.CredentialTypeIds(credentialTypesIds);
                vm.currentMember.MaterialCredentialTypeIds(materialCredentialTypeIds);
                vm.currentMember.CoordinatorCredentialTypeIds(coordinatorCredentialTypeIds);
			});
		}

		function setMembershipEndDate(panelRoleId) {
			if (vm.currentMember.PanelMembershipId()) {
				return;
			}
			if (!panelRoleId) {
				vm.currentMember.StartDate(null);
				vm.currentMember.EndDate(null);
				return;
			}
			var panelRole = ko.utils.arrayFilter(vm.panelRoleOptions.options(),
				function (r) {
					return r.PanelRoleId === panelRoleId;
				})[0];

			var membershipDuration = panelRole.MembershipDurationMonths;
            var financialYear = getFinancialYear(moment());
            var endDate = financialYear.endDate.add(membershipDuration, 'M');
		    var startDateFormatted = financialYear.startDate.format(CONST.settings.shortDateDisplayFormat);
		    var endDateFormatted = endDate.format(CONST.settings.shortDateDisplayFormat);
			vm.currentMember.StartDate(startDateFormatted);
			vm.currentMember.EndDate(endDateFormatted);
        }

        function getFinancialYear(date) {

	        var endOfYear = date.month() > 5; // moments months are 0 based, i.e. 5 is June
	        var startOfFinancialYear = endOfYear ? date.year() : date.year() - 1;
	        var endOfFinancialYear = endOfYear ? date.year() + 1 : date.year();

	        return {
	            startDate: moment({ year: startOfFinancialYear, month: 6, day: 1 }),
	            endDate: moment({ year: endOfFinancialYear - 1, month: 5, day: 30 })
	        };
	    }

	    function checkRolePlayerRules() {

            panelMemberDataService.getFluid('hasRolePlayerRatingLocation/' + vm.parseMember().PersonId).then(function (data) {
                if (data === true) {
                    messageService.custom({
                        title: ko.Localization('Naati.Resources.Shared.resources.Confirm'),
                        content: ko.Localization('Naati.Resources.Shared.resources.SetRolePlayerDetails'),
                        buttons: [
                            {
                                text: ko.Localization('Naati.Resources.Shared.resources.Yes'),
                                className: 'btn btn-primary',
                                argument: 'yes',
                                click: function () {
                                    var url = '/#person/' + vm.currentMember.NaatiNumber() + '/settings';
                                    window.open(url);
                                }
                            },
                            {
                                text: ko.Localization('Naati.Resources.Shared.resources.No'),
                                className: 'btn btn-default',
                                argument: 'no',
                            }
                        ]
                    });
                }
            });
        }
        
        return vm;
    }


	return {
		getInstance: getInstance
	};
});
