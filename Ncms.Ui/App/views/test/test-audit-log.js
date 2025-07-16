define([
    'services/test-data-service',
    'services/person-data-service',
    'plugins/router',
], function (testService, personService, router) {
    var personDetailsHeadingFormat = ko.Localization('Naati.Resources.Test.resources.PersonDetailsHeadingFormat');

    var testServerModel = {
        TestAttendanceId: ko.observable(),
        Supplementary: ko.observable(),
        NaatiNumber: ko.observable(),
        ResultId: ko.observable()
    };

    var personServerModel = {
        NaatiNumber: ko.observable(),
        GivenName: ko.observable(),
        SurName: ko.observable(),
        BirthDate: ko.observable()
    };

    var request = ko.computed(function () {
        return { RecordName: 'Test Result', RecordId: testServerModel.ResultId() };
    });

    var vm = {
        auditLogOptions: { showInModal: false, request: request },
        testSittingId: ko.observable(),
        person: personServerModel,
        test: testServerModel,
    };

    vm.personDetailsHeading = ko.pureComputed(function () {
        var personNaatiHearLink = '<a href="#/person/' + vm.person.NaatiNumber() + '?tab=contactDetails" target="_blank" class="text-info">' + vm.person.NaatiNumber() + '</a>&nbsp;';

        var title = personDetailsHeadingFormat.format(vm.person.GivenName() + ' ' + vm.person.SurName(),
            personNaatiHearLink + ' ',
            moment(vm.person.BirthDate()).format(CONST.settings.shortDateDisplayFormat));

        if (testServerModel.Supplementary()) {
            title += ' - ' + supplementaryTestText;
        }

        return title;
    });

    vm.canActivate = function (id) {
        vm.testSittingId(id);

        return testService.getFluid(vm.testSittingId() + '/summary').then(function (data) {
            ko.viewmodel.updateFromModel(testServerModel, data);
            testServerModel.ResultId.valueHasMutated();  

            return personService.getFluid(testServerModel.NaatiNumber() + '/summary').then(function (data) {
                ko.viewmodel.updateFromModel(personServerModel, data);
                return true;
            });
        });
    };

    return vm;
});
