define([
    'services/system-data-service',
], function (systemService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            messageOfTheDay: ko.observable(),
            myNaatiAvailable: ko.observable(),
            showMessageOfTheDay: ko.observable(),
            myNaatiAvailableMessage: ko.observable()
        };

        vm.activate = function () {
            systemService.getFluid("getMotdValues").then(
                function (data) {
                    vm.messageOfTheDay(data.MessageOfTheDay);
                    vm.showMessageOfTheDay(data.ShowMessageOfTheDay == 'true');
                    vm.myNaatiAvailable.subscribe(function (newValue) {
                        toggleActivate(newValue);
                    });
                    vm.myNaatiAvailable(data.MyNaatiAvailable == 'false');

                });

        };

        //vm.toggleActivate = function () {
        //    toggleActivate()
        //}

        vm.save = function () {
            systemService.post({ MessageOfTheDay: vm.messageOfTheDay(), MyNaatiAvailable: !vm.myNaatiAvailable(), ShowMessageOfTheDay: vm.showMessageOfTheDay() }, 'postMotdValues').then(function () {
                toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
            });
        };

        return vm;

        function toggleActivate(newValue) {
            if (newValue) {
                vm.myNaatiAvailableMessage("The myNAATI logon is disabled. myNaati users are unable to login");
            } else {
                vm.myNaatiAvailableMessage("The myNAATI logon is enabled. myNaati users are able to login as normal");
            }
        }

    }
});