define(['services/person-data-service'], function (personService) {
    var vm = {
        query: ko.observable(),
        naatiTable: {
            source: ko.observableArray()
        },
        activate: activate
    };

    return vm;

    function activate(activationData) {
        vm.query(activationData);
        personService.get({term:activationData}).then(function(data){
            $.each(data, function (i, d) {
                d.BirthDateFormatted = moment(d.BirthDate).format(CONST.settings.shortDateDisplayFormat);
            });

            vm.naatiTable.source(data);
        });
    }
});