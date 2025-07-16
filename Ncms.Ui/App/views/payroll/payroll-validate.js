define([
    'views/shell',
    'services/payroll-data-service',
    'services/util',
    'modules/common'
], function (shell, payrollService, util, common) {
    var vm = {
        users: ko.observableArray([]),
        title: shell.titleWithSmall,
    };

    var pageSize = 50;
    var recursiveFlag = common.functions().recursiveFlag({ flagName: 'Verified' });

    vm.activate = function () {
        vm.users([]);
        return payrollService.getFluid('getMarkingsForValidation').then(
            function (data) {

                $.each(data,
                    function (i, user) {
                        user.Verified = ko.observable(false);
                        if (!user.NoCode) {
                            recursiveFlag.addFlag(user);
                        }
                        $.each(user.Groups,
                            function (i2, group) {
                                group.expanded = ko.observable(false);
                                group.headId = util.guid();
                                group.childId = util.guid();
                                group.Verified = ko.observable(false);
                                if (!user.NoCode) {
                                    recursiveFlag.addFlag(group, user);
                                }

                                $.each(group.Markings, function (i3, marking) {
                                    marking.Verified = ko.observable(false);
                                    if (!user.NoCode) {
                                        recursiveFlag.addFlag(marking, group);
                                    }
                                    marking.Group = group;
                                });

                                group.Markings = ko.observableArray(group.Markings);
                                group.TotalSize = ko.observable(pageSize);
                                group.VisibleMarkings = ko.computed(function () {
                                    var i3 = -1;
                                    return ko.utils.arrayFilter(group.Markings(), function () {
                                        i3++;
                                        return i3 < group.TotalSize();
                                    });
                                });
                                group.increaseTotalSize = function (g) {
                                    g.TotalSize(g.TotalSize() + pageSize);
                                };

                                group.User = user;
                            });

                        user.headId = util.guid();
                        user.childId = util.guid();
                        user.expanded = ko.observable(false);

                        user.Groups = ko.observableArray(user.Groups);

                        vm.users.push(user);
                    });
                return true;
            });
    }

    vm.getFirstReceivedDateInfo = function (group) {
        return ko.Localization('Naati.Resources.Payroll.resources.OldestReceivedDay').format('<span class="font-grey-bold text-md">' + group.OldestReceivedDays + '</span>');
    };

    vm.counterInfo = function (group) {
        var resource;
        if (group.User.NoCode) {
            resource = 'Naati.Resources.Payroll.resources.NoCodeCounterInfo';
        } else {
            resource = 'Naati.Resources.Payroll.resources.CounterInfo';
        }
        return ko.Localization(resource).format(group.Markings().length);
    };

    vm.toggleExpanded = function (spec) {
        spec.expanded(!spec.expanded());
    };

    vm.toggleVerified = function (item) {
        item.Verified(!item.Verified());
    };

    vm.enteredInfo = function (user) {
        if (user.NoCode) {
            return ko.Localization('Naati.Resources.Payroll.resources.NoCodeValidationGroupTitle').format(user.User);
        }
        return ko.Localization('Naati.Resources.Payroll.resources.EnteredBy').format(user.User);
    };

    vm.parseCost = function (cost) {
        return "$ " + parseFloat(cost).toFixed(2);
    };

    vm.validate = function (marking, e) {
        var examiners = [];
        var verified = getVerified();

        ko.utils.arrayForEach(verified, function (marking) {
            examiners.push(marking.JobExaminerId);
        });

        payrollService.post(examiners, 'validate').then(function () {
            ko.utils.arrayForEach(verified, function (marking) {
                var group = marking.Group;
                var user = group.User;

                group.Markings.remove(marking);

                if (!group.Markings().length)
                    user.Groups.remove(group);

                if (!user.Groups().length)
                    vm.users.remove(user);
            });

            recursiveFlag.reset();
        });
    };

    vm.expandAll = function () {
        $.each(vm.users(), function (i, user) {
            if (!user.expanded()) {
                vm.toggleGroups(user);
            }

            $.each(user.Groups(), function (j, group) {
                group.expanded(true);
            });
        });
    };

    vm.collapseAll = function () {
        $.each(vm.users(), function (i, user) {
            if (user.expanded()) {
                vm.toggleGroups(user);
            }

            $.each(user.Groups(), function (j, group) {
                group.expanded(false);
            });
        });
    };

    vm.toggleGroups = function (user) {
        var $head = $('#' + user.headId)
        var $icon = $("i", $head);
        $icon.toggleClass("fa-plus-circle");
        $icon.toggleClass("fa-minus-circle");
        $('#' + user.childId).slideToggle();
        user.expanded(!user.expanded());
    };

    vm.openTest = function (marking) {
        window.open('#test/' + marking.TestAttendanceId, '_blank');
    };

    function getVerified() {
        var markings = [];
        ko.utils.arrayForEach(vm.users(), function (user) {
            ko.utils.arrayForEach(user.Groups(), function (group) {
                ko.utils.arrayForEach(group.Markings(), function (marking) {
                    if (marking.Verified()) {
                        markings.push(marking);
                    }
                });
            });
        });
        return markings;
    }

    return vm;
});