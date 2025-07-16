define([
    'views/shell',
], function(shell) {
  
        shell.hideHeader(true);
        shell.hideMenu(true);
        
        var vm = {
            email: ko.observable(),
            emailsPreviewOptions: {
                loaded: ko.observable(true),
                emails: ko.pureComputed(function() {
                    if (vm.email()) {
                        return [ko.viewmodel.fromModel(vm.email())];
                    }

                    return [];
                })
            }
        };

        return vm;
   
});