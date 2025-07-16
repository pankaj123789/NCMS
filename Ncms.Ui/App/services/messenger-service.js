define([
    'modules/enums',
    'services/util',
    'services/test-material-data-service',
    'services/messenger-data-service'
], function (enums, util, testMaterialService, messenger) {
    var vm = {
        connect: connect,
        click: click,
        deleteClick: deleteClick,
        checkMessages: checkMessages,
        messages: ko.observableArray([])
    };

    return vm;

    function setNotificationSettings(message) {
        message.type = 'success';
        message.clickPromise = function () {
            return Promise.resolve();
        };

        switch (message.NotificationTypeId) {
            case enums.NotificationType.DownloadTestMaterial:
                message.Text = ko.Localization('Naati.Resources.Shared.resources.YourTestMaterialIsReadyToDownload').format(message.Data.TestSessionName);
                message.clickPromise = function () {
                    return testMaterialService.getFluid('downloadFromNotification', { notificationId: message.Id }).then(function (data) {
                        util.downloadFile(data);
                    });
                }
                break;
            case enums.NotificationType.ErrorMessage:
                message.Text = message.Data.Content;
                message.type = 'error';
                break;
        }
    }

    function setNotificationSettingsAndNotify(message, preventNotify) {
        setNotificationSettings(message);
        if (!message.Text) {
            return;
        }

        var messages = vm.messages();
        messages.unshift(message);
        vm.messages(messages);

        if (preventNotify) {
            return;
        }

        toastr[message.type](message.Text, null, {
            onclick: function () {
                click(message);
            },
            closeButton: true,
            timeOut: 0,
            extendedTimeOut: 0
        });
    }

    function click(message) {
        Promise.resolve(message.clickPromise()).then(function () {
            deleteClick(message);
        });
    }

    function deleteClick(message) {
        messenger.post(null, 'markAsRead/' + message.Id).then(function (messages) {
            refresh(messages, true);
        });
    }

    function refresh(messages, preventNotify) {
        vm.messages([]);
        var messages = messages;
        ko.utils.arrayForEach(messages, function (m) {
            setNotificationSettingsAndNotify(m, preventNotify);
        });
    }

    function checkMessages() {
        localStorage.setItem('checkMessages', true);
        startTimer();
    }

    function startTimer() {
        var interval = setInterval(function () {
            messenger.get().then(function (messages) {
                if (messages.length) {
                    localStorage.setItem('checkMessages', false);
                    refresh(messages);
                    clearInterval(interval);
                }
            });
        }, 15000);
    }

    function connect() {
        var checkMessages = localStorage.getItem('checkMessages');
        if (checkMessages == "true") {
            startTimer();
        }
        else {
            messenger.get().then(refresh);
        }
    }
});