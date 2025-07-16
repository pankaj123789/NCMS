define([
], function () {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultLink = 'https://';
        var httpRegex = new RegExp("^(http|https)://", "i");

        var defaultParams = {
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null,
            wizardService: null,
            allowEdit: ko.observable(true),
            links: ko.observableArray(),
        };

        $.extend(defaultParams, params);

        var vm = {
            link: ko.observable().extend({ url: true }),
            links: defaultParams.links,
            materialRequest: defaultParams.materialRequest,
            wizardService: defaultParams.wizardService,
            action: defaultParams.action,
            allowEdit: defaultParams.allowEdit
        };

        var validation = ko.validatedObservable([vm.link]);

        vm.load = function () {
            defaultParams.wizardService.getFluid('wizard/roundlinks/' + vm.materialRequest.MaterialRequestId() + '/' + vm.action().Id).then(function (data) {
                vm.links(data.Links);
            });
            clearValidation();
        };

        vm.linkFocus = function () {
            if (!vm.link()) {
                vm.link(defaultLink);
                clearValidation();
            }
        };

        vm.linkFocusOut = function () {
            if (vm.link() == defaultLink) {
                vm.link(null);
                clearValidation();
            }
        };

        vm.addLink = function () {
            if (!validation.isValid()) {
                validation.errors.showAllMessages();
                return;
            }

            if (!httpRegex.test(vm.link())) {
                vm.link('http://' + vm.link());
            }

            vm.links.push(vm.link());
            vm.link(null);
            clearValidation();
        };

        vm.postData = function () {
            return { Links: ko.toJS(vm.links) };
        };

        vm.removeLink = function (link) {
            vm.links.remove(link);
        };

        function clearValidation() {
            if (validation.errors) {
                validation.errors.showAllMessages(false);
            }
        }

        return vm;
    }
});