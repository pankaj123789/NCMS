define([
    'modules/common',
    'modules/enums',
    'services/person-data-service',
    'services/screen/message-service',
    'services/screen/date-service',
    'services/util',
    'plugins/router',
    'services/institution-data-service',
], function (common, enums, personDataService, message, dateService, util, router, institutionService) {

    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            load: load,
            populate: populate,
            mode: ko.observable(),
            addressList: ko.observableArray([]),
            phoneList: ko.observableArray([]),
            emailList: ko.observableArray([]),
            websiteList: ko.observableArray([]),
            editAddress: editAddress,
            addAddress: addAddress,
            tryDeleteAddress: tryDeleteAddress,
            editPhone: editPhone,
            addPhone: addPhone,
            tryDeletePhone: tryDeletePhone,
            editEmail: editEmail,
            addEmail: addEmail,
            tryDeleteEmail: tryDeleteEmail,
            editWebsite: editWebsite,
            addWebsite: addWebsite,
            tryDeleteWebsite: tryDeleteWebsite,
            showAddAddress: showAddAddress,
            showAddPhone: showAddPhone,
            showAddEmail: showAddEmail,
            entityId: null,
            isInstitution: ko.observable(),
            showWebsite: ko.observable(false)
        };

        vm.formatPhone = function (phone) {
            return common.functions().formatPhone(phone.LocalNumber());
        };

        vm.hasPrimaryAddress = ko.computed(function () {
            var addresses = vm.addressList();
            if (!addresses.length) return false;
            return addresses[0].PrimaryContact();
        });

        vm.hasPrimaryEmail = ko.computed(function () {
            var emails = vm.emailList();
            if (!emails.length) return false;
            return emails[0].IsPreferredEmail();
        });

        return vm;

        function load(entityId) {
            (vm.isInstitution() ? institutionService : personDataService).getFluid(entityId + '/contactdetails')
                .then(function (data) {
                    populate(entityId, data);
                });
        }

        function populate(entityId, data, isInstitution) {
            vm.entityId = entityId;
            loadAddresses(data.Addresses);
            loadPhones(data.Phones);
            loadEmails(data.Emails);
            loadWebsites(data.Websites);
            vm.isInstitution(isInstitution);
            vm.showWebsite(data.ShowWebsite);
        }

        function loadAddresses(data) {
            var odAddressVisibilityType = enums.OdAddressVisibilityType;
            data = data.sort(function (a, b) {
                var o1 = a.PrimaryContact ? -1 : 1;
                var o2 = b.PrimaryContact ? -1 : 1;

                if (o1 < o2) return -1;
                if (o1 > o2) return 1;

                return 0;
            });

            var list = ko.viewmodel.fromModel(data);
            list()
                .forEach(
                    function (item) {
                        item.Address = ko.pureComputed(function () { return util.addressToString(item); });
                        item.IsOdAddressVisibility = ko.pureComputed(function () {
                            return (item.OdAddressVisibilityTypeId() !== odAddressVisibilityType.DoNotShow);
                        });
                      }
                    );
            vm.addressList.removeAll();
            ko.utils.arrayPushAll(vm.addressList, list());
        }

        function loadPhones(data) {
            var list = ko.viewmodel.fromModel(data);
            vm.phoneList.removeAll();
            ko.utils.arrayPushAll(vm.phoneList, list());
        }

        function loadEmails(data) {
            data = data.sort(function (a, b) {
                var o1 = a.IsPreferredEmail ? -1 : 1;
                var o2 = b.IsPreferredEmail ? -1 : 1;

                if (o1 < o2) return -1;
                if (o1 > o2) return 1;

                return 0;
            });
            var list = ko.viewmodel.fromModel(data);
            vm.emailList.removeAll();
            ko.utils.arrayPushAll(vm.emailList, list());
        }

        function loadWebsites(data) {
            var list = ko.viewmodel.fromModel(data);
            vm.websiteList.removeAll();
            ko.utils.arrayPushAll(vm.websiteList, list());
        }

        function editAddress(address) {
            router.navigate('address/' + vm.entityId + '/' + address.AddressId());
        }

        function addAddress() {
            if (vm.isInstitution()) {
                router.navigate('#organisation/' + vm.entityId + '/address/');
            } else {
                router.navigate('#person/' + vm.entityId + '/address/');
            }
        }

        function tryDeleteAddress(address) {
            message.remove()
                .then(
                    function (answer) {
                        if (answer === 'yes') {
                            (vm.isInstitution() ? institutionService : personDataService).post(address.AddressId(), 'deleteAddress')
                                .then(
                                    function () {
                                        toastr.success('Address deleted.');
                                        load(vm.entityId);
                                    });
                        }
                    });
        }

        function editPhone(phone) {
            router.navigate('phone/' + vm.entityId + '/' + phone.PhoneId());
        }

        function addPhone() {
            if (vm.isInstitution()) {
                router.navigate('#organisation/' + vm.entityId + '/phone/');
            } else {
                router.navigate('#person/' + vm.entityId + '/phone/');
            }
        }

        function tryDeletePhone(phone) {
            message.remove()
                .then(
                    function(answer) {
                        if (answer === 'yes') {
                            var postData = {
                                ObjectId: phone.PhoneId(),
                                FlowAnswers: []
                            };
                            (vm.isInstitution() ? institutionService : personDataService).post(postData, 'deletePhone')
                                .then(
                                    function() {
                                        toastr.success('Phone number deleted.');
                                        load(vm.entityId);
                                    });
                        }
                    });
        }

        function editEmail(email) {
            router.navigate('email/' + vm.entityId + '/' + email.EmailId());
        }

        function addEmail() {
            if (vm.isInstitution()) {
                router.navigate('#organisation/' + vm.entityId + '/email/');
            } else {
                router.navigate('#person/' + vm.entityId + '/email/');
            }
        }

        function tryDeleteEmail(email) {
            message.remove()
                .then(
                    function(answer) {
                        if (answer === 'yes') {
                            var postData = {
                                ObjectId: email.EmailId(),
                                FlowAnswers: []
                            };
                            (vm.isInstitution() ? institutionService : personDataService).post(postData, 'deleteEmail')
                                .then(
                                    function() {
                                        toastr.success('Email address deleted.');
                                        load(vm.entityId);
                                    });
                        }
                    });
        }

        function editWebsite() {
            router.navigate('website/' + vm.entityId);
        }

        function addWebsite() {
            router.navigate('#website/' + vm.entityId);
        }

        function tryDeleteWebsite(website) {
            message.remove()
                            .then(
                                function (answer) {
                                    if (answer === 'yes') {
                                        (vm.isInstitution() ? institutionService : personDataService).post(vm.entityId, 'deleteWebsite')
                                            .then(
                                                function () {
                                                    toastr.success('Website address deleted.');
                                                    load(vm.entityId);
                                                });
                                    }
                                });
        }

        function showAddAddress() {
            return vm.addressList().length < 2 && currentUser.hasPermission(enums.SecNoun.Contact,enums.SecVerb.Create);
        }

        function showAddPhone() {
            return vm.phoneList().length < 2 && currentUser.hasPermission(enums.SecNoun.Contact, enums.SecVerb.Create);
        }

        function showAddEmail() {
            return vm.emailList().length < 2 && currentUser.hasPermission(enums.SecNoun.Contact, enums.SecVerb.Create);
        }

    }
});
