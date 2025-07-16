define(['services/screen/date-service', 'services/panel-data-service', 'views/panels/details'], function (dateService, panelDataService, panelDetailsModel) {
    function getInstance() {
        function getpanelDetailsInstance() {
            var panelDetailsInstance = panelDetailsModel.getInstance();

            panelDetailsInstance.css('form-group col-md-12');
            panelDetailsInstance.visibleInEportalCss('hide');

            return panelDetailsInstance;
        }

        var defer = null;

        var vm = {
            panelDetailsInstance: ko.observable(null),
            createPanel: function () {
                defer = Q.defer();

                vm.panelDetailsInstance(getpanelDetailsInstance());
                vm.panelDetailsInstance().clearDetails();
                vm.panelDetailsInstance().panel.CommissionedDate(dateService.today(CONST.settings.shortDateDisplayFormat));
                vm.panelDetailsInstance().panel.VisibleInEportal(true);
                $('#createPanelModal').modal('show');

                return defer.promise;
            },
            bindDetails: ko.pureComputed(function () {
                return vm.panelDetailsInstance() !== null;
            }),
            parsePanel: function () {
                var parsedPanel = ko.toJS(vm.panelDetailsInstance().panel);

                parsedPanel.CommissionedDate = dateService.format(parsedPanel.CommissionedDate);

                return parsedPanel;
            },
            save: function () {
                if (vm.panelDetailsInstance().validate()) {
                    panelDataService.post(vm.parsePanel()).then(function (panelId) {
                        $('#createPanelModal').modal('hide');
                        toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));

                        vm.panelDetailsInstance().panel.PanelId(panelId);
                        defer.resolve(vm.parsePanel());
                    });
                }
            }
        };

        return vm;
    }

    return {
        getInstance: getInstance
    };
});
