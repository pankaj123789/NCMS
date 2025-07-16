define(['plugins/router'], function (router) {
    var vm = {
        formAttr: ko.observable(),
        query: ko.observable(),
        submit: submit,
        activate: activate
    };

    return vm;

    function activate(activationData) {
        var formAttr = $.extend({ 'class': "navbar-form navbar-left" }, (activationData || {}).formAttr);
        vm.formAttr(formAttr);
    }

    function submit() {
        if (!vm.query())
            toastr.warning("You must enter the term to be searched");
        else
            router.navigate('search/result/' + vm.query() || null);
    }
});