define(['services/language-data-service',
    'services/panel-data-service',
    'modules/common',
    'services/util',], function (languageDataService, panelService, common, util) {


        function getPanel() {
            var panel = {
                PanelId: ko.observable(),
                Name: ko.observable().extend({ required: true, maxLength: 100 }),
                PanelTypeId: ko.observable().extend({ required: true }),
                LanguageId: ko.observable(),
                CommissionedDate: ko.observable().extend({ required: true }),
                Note: ko.observable().extend({ maxLength: 500 }),
                VisibleInEportal: ko.observable(),
            };

            return panel;
        }

        function getInstance() {
            var vm = {
                panel: getPanel(),
                panelNameCss: ko.observable('form-group col-md-3'),
                panelTypeCss: ko.observable('form-group col-md-3'),
                languageCss: ko.observable('form-group col-md-3'),
                commissionedDateCss: ko.observable('form-group col-md-3'),
                noteCss: ko.observable('form-group col-md-3'),
                visibleInEportalCss: ko.observable('form-group col-md-3'),
                css: function (value) {
                    vm.panelNameCss(value);
                    vm.panelTypeCss(value);
                    vm.languageCss(value);
                    vm.commissionedDateCss(value);
                    vm.noteCss(value);
                    vm.visibleInEportalCss(value);
                },
                setDetails: function (panel) {
                    var commissionedDateFormatted = moment(panel.CommissionedDate).toDate();
                    vm.panel.PanelId(panel.PanelId);
                    vm.panel.Name(panel.Name);
                    vm.panel.PanelTypeId(panel.PanelTypeId);
                    vm.panel.LanguageId(panel.LanguageId);
                    vm.panel.CommissionedDate(commissionedDateFormatted);
                    vm.panel.Note(panel.Note);
                    vm.panel.VisibleInEportal(panel.VisibleInEportal);
                }
            };

            vm.validation = ko.validatedObservable(vm.panel);
            vm.selectedPanelType = ko.pureComputed(function() {
                var panelType = ko.utils.arrayFilter(vm.panelTypeOptions.options(),
                    function(item) {
                        return item.Id === vm.panel.PanelTypeId();
                    });
                return (panelType || [null])[0];
            });

            vm.panelNameOptions = {
                value: vm.panel.Name,
                resattr: {
                    placeholder: 'Naati.Resources.Panel.resources.PanelName'
                }
            };

            vm.panelTypeOptions = {
                value: vm.panel.PanelTypeId,
                multiple: false,
                options: ko.observableArray(),
                optionsValue: 'Id',
                optionsText: 'DisplayName',
                afterRender: function () {
                    panelService.getFluid('PanelTypes').then(this.options);
                },
                disable: ko.pureComputed(function () {
                    return  vm.panel.PanelId();
                }),
            };

            vm.languageOptions = {
                options: ko.observableArray(),
                value: vm.panel.LanguageId,
                entity: 'language',
                valueField: ko.observable('LanguageId'),
                textField: ko.observable('Name'),
                multiple: false,
                dataCallback: function (options) {
                    return options.sort(util.sortBy('Name'));
                },
                disable: ko.pureComputed(function () {
                    var panelType = vm.selectedPanelType();
                    return !panelType || !panelType.AllowCredentialTypeLink;
                }),
            };
            vm.panel.LanguageId.extend({
                required: ko.pureComputed(function () {
                    return !vm.languageOptions.disable();
                })
            });

            vm.commissionedDateOptions = {
                value: vm.panel.CommissionedDate,
                resattr: {
                    placeholder: 'Naati.Resources.Panel.resources.CommissionedDate'
                }
            };

            vm.noteOptions = {
                value: vm.panel.Note,
                resattr: {
                    placeholder: 'Naati.Resources.Shared.resources.Note'
                }
            };

            vm.validate = function () {
                var isValid = vm.validation.isValid();

                vm.validation.errors.showAllMessages(!isValid);

                return isValid;
            };

            vm.clearValidation = function () {
                vm.validation.errors.showAllMessages(false);
            };

            vm.clearDetails = function () {
                vm.panel.PanelId(null);
                vm.panel.Name(null);
                vm.panel.PanelTypeId(null);
                vm.panel.LanguageId(null);
                vm.panel.CommissionedDate(null);
                vm.panel.Note(null);
                vm.panel.VisibleInEportal(null);
                vm.clearValidation();
            };

            return vm;
        }

        return {
            getInstance: getInstance
        };
    });
