define([
        'services/util',
        'services/screen/date-service',
        'services/audit-data-service',
        'services/person-data-service',
        'plugins/router',
        'services/screen/message-service'
],
    function (util, dateService, auditService, personService, router, message) {

        var serverModel = {
            GivenName: ko.observable(),
            Surname: ko.observable(),
            BirthDate: ko.observable(),
            NaatiNumber: ko.observable(),
            PractitionerNumber: ko.observable(),
            PersonId: ko.observable(),
            EntityId: ko.observable(),
            AuditHistory: ko.observableArray(),
        };

        var vm = {
            dateService: dateService,
            audithistory: serverModel,
            canActivate: canActivate,
            close: close,
            dateFormatted: dateFormatted,
            detail: detail
        };

        $.extend(vm,
        {
            windowTitle: ko.pureComputed(
                function() {
                    return util.getPersonSubScreenTitle(serverModel.NaatiNumber(),
                        serverModel.GivenName(), serverModel.Surname(),
                        serverModel.PractitionerNumber(),
                        'Audit History');
                })
        });

        function dateFormatted(date) {
            return moment(date).format("h:mm a DD/MM/YYYY");
        }

        function canActivate(params) {
            return loadAuditHistory(params);
        }

        function loadAuditHistory(params) {

            var defer = Q.defer();
            var paramsList = params.split('-');
            var entityId = paramsList[0];
            var naatiNumber = paramsList[1];
            var personId = paramsList[2];

            loadPerson(naatiNumber);

            var promises = [
                getDataWithParentId(entityId, 'Entity'),
                getDataWithParentId(personId, 'Person')
            ];

            Q.all(promises).done(function (result) {
                var data = [];
                ko.utils.arrayForEach(result,
                    function (value) {
                        data = data.concat(value);
                    });

                ko.viewmodel.updateFromModel(vm.audithistory.AuditHistory, data.sort(function (a, b) {
                    var dateA = new Date(b.DateTime);
                    var dateB = new Date(a.DateTime);
                    return dateA - dateB;
                }));
                defer.resolve(true);
            });

            return defer.promise;
        }

        function loadPerson(naatiNumber) {
            return personService.getFluid(naatiNumber).then(
                function(data) {
                    ko.viewmodel.updateFromModel(serverModel, data);
                });
        }

        function getDataWithParentId(parentId, parentName) {
            return auditService.get({ ParentId: parentId, PageSize: 1000, PageNumber: 1, ParentName: parentName })
                .then(function (data) { return data; }, function () { return []; });
        }

        function close() {
            router.navigateBack();
        } 

        vm.auditOptions = {
            name: 'audit-log',
            params: {}
        };

        function detail(item) {
            var d = ko.toJS(item);
            message.alert({
                title: vm.auditOptions.params.component.getHeader(d),
                content: vm.auditOptions.params.component.formatLog(d.ChangeDetail),
                css: 'modal-lg'
            });
        }
        return vm;
    });