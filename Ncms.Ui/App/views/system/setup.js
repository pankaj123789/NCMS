define([
    'views/shell'
], function (shell) {
    var vm = {
        title: shell.titleWithSmall,
        parts: [
            'azure-sign-in',
            'wiise-sign-in',
            'motd'
       ],
        processedParts: ko.observableArray(),
    };

    vm.activate = function (message) {
        vm.processedParts([]);
        var parts = [];

        $.each(vm.parts, function (i, p) {
            parts.push('views/system/setup-parts/' + p);
        });

        require(parts, function () {
            $.each(arguments, function (i, part) {
                vm.processedParts.push({
                    view: parts[i],
                    model: part.getInstance()
                });
            });
        });

        if (message) {
            toastr.info(ko.Localization('Naati.Resources.SystemResources.resources.' + message));
        }
    }

    return vm;
});