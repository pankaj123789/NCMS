define([
    'services/application-data-service',
    'services/util',
    'modules/enums',
    'modules/custom-validator'
], function (applicationService, util, enums, customValidator) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var defaultParams = {
            application: null,
            credentialRequest: null,
            action: null,
            credentialRequest: null,
        };

        $.extend(defaultParams, params);

        var serverModel = {

        };

        var vm = {
            session: params.preRequisiteSession,
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            credentialPrerequisiteRequest: params.credentialPrerequisiteRequest,
            visibleSteps: defaultParams.visibleSteps,
            option: serverModel,
            optionPromise: null,
            prerequisiteExemptions: ko.observableArray(),
            allExempted: ko.observable(false),
            hasLoaded: ko.observable(false),
        };

        vm.stepDescriptionStep = ko.pureComputed(function () {
            if (vm.action().Id == 1063) { // Create Prerequisite Application
                return ko.Localization('Naati.Resources.Application.resources.PrerequisiteExemptionStep').format('Check the box to select if you want to exempt any of the prerequisite credentials from this table.')
            }
            else {
                return ko.Localization('Naati.Resources.Application.resources.PrerequisiteExemptionStep').format('');
            }
        });

        vm.tableDefinitionNonSelectable = {
            dom: "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
            searching: false,
            paging: false,
            oLanguage: { sInfoEmpty: '', sInfo: '' },
            columnDefs: [
                {
                    orderable: false,
                    targets: 0
                }
            ],
        };

        vm.tableDefinition = {
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
                }
            ],
            select: {
                style: 'multi+shift'
            },
            drawCallback: drawTable,
            events: {
                select: selectTable,
                deselect: selectTable
            }
        };

        // we only want to be able to select / deselect the table to create/ update exemptions if the action is to create prerequisite applications
        vm.isCreatePrereqAction = ko.pureComputed(function () {
            if (vm.action().Id == 1063) { // Create Prerequisite Application
                return true;
            }
            else {
                return false;
            }
        });

        // if all exemptions are checked then set all exempted to true
        vm.allExempted = ko.pureComputed(function () {
            return vm.prerequisiteExemptions().filter(exemption => {
                return exemption.Checked == true;
            }).length == vm.prerequisiteExemptions().length;
        })

        // this will execute first on step load
        vm.load = function () {
            // get prerequisite exemptions for the current person
            applicationService.getFluid('prerequisiteExemptions/{0}'.format(vm.credentialRequest.Id())).then(function (data) {
                //format the date
                ko.utils.arrayForEach(data, function (item) {
                    if (item.ExemptionStartDate) {
                        item.ExemptionStartDate = moment(item.ExemptionStartDate).format("DD/MM/YYYY");
                    }
                });
                // store the exemptions in an observable array
                vm.prerequisiteExemptions(data);
                // set has loaded after we get the data so the table renders properly
                vm.hasLoaded(true);
            });
        };

        vm.alterSteps = function (steps, allSteps, currentStep) {
            // if we are in the create prereq wizard and all exemptions are ticked then we want to remove all steps
            // and insert the final step saying no application required to be created
            if (vm.isCreatePrereqAction()) {
                if (vm.allExempted()) {
                    steps(steps.slice(0, steps.indexOf(currentStep) + 1));
                    var finalStepId = enums.ApplicationWizardSteps.NoNeedToContinue;
                    var finalStep = allSteps[finalStepId];
                    var finalStepType = 'NoSelectedNewApplications'
                    var model = finalStep.model;

                    var modelParams = {
                        preRequisiteSession: vm.session,
                        credentialPrerequisiteRequest: vm.credentialPrerequisiteRequest,
                        finalStepType: finalStepType
                    };

                    steps.push($.extend(finalStepId, finalStep, {
                        current: ko.observable(false),
                        success: ko.observable(false),
                        cancel: ko.observable(false),
                        label: ko.Localization('Naati.Resources.Application.resources.NoNewApplications'),
                        css: 'animated fadeIn',
                        compose: {
                            view: finalStep.compose.view,
                            model: model.getInstance(modelParams)
                        }
                    }));

                    return steps;
                }

                return steps;
            }
            // if we are not in the create prereq step and all are exempted then we want to remove the 
            // validation step for asking if a credential should get put on hold
            if (vm.allExempted()) {
                var stepsToSplice = [enums.ApplicationWizardSteps.IncompletePrerequisiteCredentials];

                ko.utils.arrayForEach(stepsToSplice, function (s) {
                    var stepToSpliceIndex = -1;
                    var stepToSplice = ko.utils.arrayFirst(steps(), function (step) {
                        return step.id == allSteps[s].id;
                    });

                    stepToSpliceIndex = steps.indexOf(stepToSplice);

                    if (stepToSpliceIndex > -1) {
                        steps.splice(stepToSpliceIndex, 1);
                    }
                });
            }

            return steps;
        }

        vm.postData = function () {
            // we only want to return data to the action if we are in the create prereq application wizard
            if (vm.isCreatePrereqAction()) {
                ko.utils.arrayForEach(vm.prerequisiteExemptions(), function (item) {
                    if (item.ExemptionStartDate) {
                        var [day, month, year] = item.ExemptionStartDate.split('/');
                        item.ExemptionStartDate = new Date(+year, month - 1, +day);
                    }
                });

                var exemptions =
                {
                    PrerequisiteExemptions: vm.prerequisiteExemptions()
                };

                return exemptions;
            }

            return;
        }

        vm.isValid = function () {
            return true;
        };

        var bypassSelect = false;

        function selectTable(e, dt) {
            if (bypassSelect || !dt) {
                return;
            }

            // get the indexes of all selected rows
            var indexes = dt.rows('.selected')[0];

            // set all exemption checks to false so we can only set the selected ones to true on the next foreach
            ko.utils.arrayForEach(vm.prerequisiteExemptions(), function (exemption) {
                exemption.Checked = false;
            });

            // set only the selected exemptions to be checked
            ko.utils.arrayForEach(indexes, function (index) {
                vm.prerequisiteExemptions()[index].Checked = true;
            });
        }

        function drawTable() {
            bypassSelect = true;

            // get DataTable by id
            var $table = $("#exemptionTable").DataTable();

            // if the exemption is checked then show row selected in UI
            for (var i = 0; i < vm.prerequisiteExemptions().length; i++) {
                if (vm.prerequisiteExemptions()[i].Checked) {
                    $table.row(i).select();
                }
            }

            bypassSelect = false;
        }

        return vm;
    };
});