define([
    'durandal/system',
    'durandal/app',
    'modules/enums'
], function (system, app, enums) {
    var stringEncodings;
    var isPageLoading = ko.observable(false);

    function init() {
        return urlEncodings.done(function (data) {
            stringEncodings = data;
        });
    }

    function updateFromModel(observable) {
        return function (data) {
            ko.viewmodel.updateFromModel(observable, data);
        };
    }

    function showLoadingIndicator() {
        var $body = $('body');

        if ($body.hasClass('modal-open')) {
            var $modal = $('.modal:visible .modal-body');
            if (!$('.butterbar', $modal).length) {
                $modal.prepend($('<div ui-butterbar class="butterbar hide"><span class="bar"></span></div>'));
            }
        }

        var $loadingBar = $('.butterbar');

        $loadingBar.removeClass('hide');
        $loadingBar.addClass('active');

        isPageLoading(true);

        app.trigger(CONST.eventNames.loadingIndicatorStatusChanged, true);
    }

    function hideLoadingIndicator() {
        var $loadingBar = $('.butterbar');

        $loadingBar.removeClass('active');
        $loadingBar.addClass('hide');

        isPageLoading(false);

        app.trigger(CONST.eventNames.loadingIndicatorStatusChanged, false);
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    function downloadFile(url, successMessage, errorMessage, callback) {
        if (jQuery.type(url) === 'function') {
            url = url();
        }

        if (!url) {
            return;
        }

        if (url.indexOf(window.baseUrl) == -1) {
            return $.fileDownload(url);
        }

        successMessage = successMessage || ko.Localization('Naati.Resources.Shared.resources.DownloadSuccessful');
        errorMessage = errorMessage || ko.Localization('Naati.Resources.Shared.resources.DownloadFailed');

        app.trigger(CONST.eventNames.showLoadingIndicator);

        $.fileDownload(url, {
            successCallback: function (url, message) {
                if (successMessage) {
                    toastr.success(successMessage);
                } else {
                    if (message)
                        toastr.success(message);
                }

                app.trigger(CONST.eventNames.cancelLoadingIndicator);

                if (callback)
                    callback('Ok');
            },
            failCallback: function (error, url) {
                var message = errorMessage;
                var obj = JSON.parse(error.replace(/(<[^>]+>)+/g, ""));

                if (obj && obj.error) {
                    message = obj.error;
                }

                toastr.error(message || 'Download failed');

                app.trigger(CONST.eventNames.cancelLoadingIndicator);

                if (callback)
                    callback('Error', error);
            }
        });
    }

    function sortBy() {
        var sort = arguments;
        return function (a, b) {
            var result = 0;
            for (var i = 0; i < sort.length; i++) {
                var property = sort[i];
                var factor = Math.pow(10, sort.length - i);
                var aProperty = ko.toJS(a[property]);
                var bProperty = ko.toJS(b[property]);
                if (aProperty < bProperty) {
                    result += -1 * factor;
                } else if (aProperty > bProperty) {
                    result += factor;
                }
            }
            return result;
        };
    }

    app.on(CONST.eventNames.showLoadingIndicator).then(showLoadingIndicator);
    app.on(CONST.eventNames.cancelLoadingIndicator).then(hideLoadingIndicator);

    $(document).on({
        ajaxStart: function () {
            app.trigger(CONST.eventNames.showLoadingIndicator);
            $('body').addClass('ajax-start');
        },
        ajaxStop: function () {
            app.trigger(CONST.eventNames.cancelLoadingIndicator);
            $('body').removeClass('ajax-start');
        }
    });

    function distinctBy(array, prop) {
        var flags = [], output = [], l = array.length, i;
        for (i = 0; i < l; i++) {
            if (flags[array[i][prop]]) continue;
            flags[array[i][prop]] = true;
            output.push(array[i]);
        }
        return output;
    }

    function resetModel(model) {
        for (var prop in model) {
            if (model.hasOwnProperty(prop)) {
                if (ko.isObservable(model[prop]) && !ko.isComputed(model[prop])) {
                    if ($.isArray(model[prop]())) {
                        model[prop]([]);
                    }
                    else {
                        if (typeof model[prop]() === 'boolean') {
                            model[prop](false);
                        } else {
                            model[prop]('');
                        }
                    }
                }
            }
        }
    }

    function addressToString(address) {
        var string = address.StreetDetails();
        if (address.Suburb()) {
            string += ', ' + address.Suburb();
        }
        if (address.CountryName() !== 'Australia') {
            string += ', ' + address.CountryName();
        }
        return string;
    }

    function windowOpen(options, content, removeElement) {
        var settings = {
            url: '',
            target: '',
            features: {
                width: 1024,
                height: 800,
                menubar: 0,
                status: 0,
                titlebar: 0,
                toolbar: 0,
                scrollbars: 1
            },
            replace: null
        };

        $.extend(settings, options);

        var features = [];

        for (var f in settings.features) {
            features.push(f + '=' + settings.features[f]);
        }
        var popUp = window.open(settings.url, settings.target, features.join(','), settings.replace);
        if (content) {
            popUp.document.write(content);
        }
        if (removeElement) {
            for (var i in removeElement) {
                if (removeElement.hasOwnProperty(i)) {
                    $(popUp.document.body).find(removeElement[i]).remove();
                }
            }
        }
        return popUp;
    }

    function getTestMaterialStatusColor(statusId) {

        if (statusId === enums.TestMaterialStatusType.New) {
            return 'success';
        }
        else if (statusId === enums.TestMaterialStatusType.ToBeUsed) {
            return 'info';
        }
        else if (statusId === enums.TestMaterialStatusType.PreviouslyUsed) {
            return 'orange';
        }
        else if (statusId === enums.TestMaterialStatusType.UsedByApplicants) {
            return 'danger';
        }
        return 'gray';
    }

    function getTestMaterialStatusText(statusId) {

        if (statusId === enums.TestMaterialStatusType.New) {

            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.New');
        }
        else if (statusId === enums.TestMaterialStatusType.ToBeUsed) {

            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.ToBeUsed');
        }
        else if (statusId === enums.TestMaterialStatusType.PreviouslyUsed) {
            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.PreviouslyUsed');
        }
        else if (statusId === enums.TestMaterialStatusType.UsedByApplicants) {
            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.UsedByApplicants');
        }
        return 'Undefined';
    }

    function getTestMaterialStatusToolTip(statusId, lastDateUsed) {

        if (statusId === enums.TestMaterialStatusType.New) {
            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.NewTooltip');
        }
        else if (statusId === enums.TestMaterialStatusType.ToBeUsed) {
            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.ToBeUsedTooltip');
        }
        else if (statusId === enums.TestMaterialStatusType.PreviouslyUsed) {
            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.PreviouslyUsedTooltip').format(moment(lastDateUsed).format(CONST.settings.shortDateDisplayFormat));
        }
        else if (statusId === enums.TestMaterialStatusType.UsedByApplicants) {

            return ko.Localization('Naati.Resources.TestMaterialStatusType.resources.UsedByApplicantsTooltip');
        }
        return 'Undefined';
    }

    function getTestMaterialDomainColor(domainId, lastDateUsed) {

        if (domainId === enums.TestMaterialDomain.Undefined) {
            return 'light-gray';
        }
        return 'info';
    }

    function updateCredentialStatus(credential) {
        switch (credential.StatusId) {
            case enums.CredentialStatus.Active:
                credential.credentialStatusClass = 'bg-success';
                credential.credentialStatusDisplay = ko.Localization('Naati.Resources.Application.resources.CredentialStatusActive');
                break;
            case enums.CredentialStatus.Future:
                credential.credentialStatusClass = 'bg-darkblue';
                credential.credentialStatusDisplay = ko.Localization('Naati.Resources.Application.resources.CredentialStatusFuture');
                break;
            case enums.CredentialStatus.Expired:
                credential.credentialStatusClass = 'bg-danger';
                credential.credentialStatusDisplay = ko.Localization('Naati.Resources.Application.resources.CredentialStatusExpired');
                break;
            case enums.CredentialStatus.Teminated:
                credential.credentialStatusClass = 'bg-danger';
                credential.credentialStatusDisplay = ko.Localization('Naati.Resources.Application.resources.CredentialStatusTerminated');
                break;
            default:
                credential.credentialStatusClass = '';
                credential.credentialStatusDisplay = '';
                break;
        }
        return credential;
    }

    function getTestSittingStatus(applicant) {
        var statusModifiedDate = applicant.StatusModifiedDate;
        var status = applicant.Status;
        var rejected = applicant.Rejected;
        var supplementary = applicant.Supplementary;
        var supplementaryRequest = applicant.SupplementaryCredentialRequest;

        var strStatus = applicant.StatusDisplayName;
        var statuses = enums.CredentialRequestStatusTypes;

        if (status === statuses.TestSessionAccepted) {
            strStatus = "Confirmed on " + moment(statusModifiedDate).format(CONST.settings.shortDateDisplayFormat);
        }


        if (status === statuses.CheckIn || status === statuses.TestSat || status === statuses.TestFailed || status === statuses.IssuedPassResult || status === statuses.CertificationIssued || status === statuses.UnderPaidTestReview) {
            strStatus = applicant.StatusDisplayName;
        }

        if (!supplementary && supplementaryRequest) {
            strStatus = "Failed - Pending Supplementary";
        }

        if (rejected) {
            strStatus = "Session Rejected";
        }

        return strStatus;
    }

    function getRolePlayerStatusCss(status) {
        var strStatusCss = 'gray';
        var statuses = enums.RolePlayerStatusType;

        if (status === statuses.Pending) {
            strStatusCss = 'dark-yellow';
        }

        if (status === statuses.Accepted) {
            strStatusCss = 'info';
        }

        if (status === statuses.Rejected) {
            strStatusCss = 'gray';
        }

        if (status === statuses.Rehearsed) {
            strStatusCss = 'orange';
        }

        if (status === statuses.Attended) {
            strStatusCss = 'success';
        }

        if (status === statuses.NoShow) {
            strStatusCss = 'danger';
        }

        return strStatusCss;
    }

    function getMaterialRequestStatusCss(status) {
        var strStatusCss = "info";

        if (status === enums.MaterialRequestStatusType.InProgress) {
            strStatusCss = 'orange';
        } else if (status === enums.MaterialRequestStatusType.AwaitingFinalisation) {
            strStatusCss = 'info';
        } else if (status === enums.MaterialRequestStatusType.Finalised) {
            strStatusCss = 'success';
        } else if (status === enums.MaterialRequestStatusType.Cancelled) {
            strStatusCss = 'purple';
        }
        return strStatusCss;
    }

    function getMaterialRequestRoundStatusCss(status) {
        if (status == enums.MaterialRequestRoundStatusType.SentForDevelopment) {
            return 'dark-yellow';
        } else if (status == enums.MaterialRequestRoundStatusType.AwaitingAproval) {
            return 'orange';
        } else if (status == enums.MaterialRequestRoundStatusType.Rejected) {
            return 'danger';
        } else if (status == enums.MaterialRequestRoundStatusType.Approved) {
            return 'success';
        } else if (status == enums.MaterialRequestRoundStatusType.Cancelled) {
            return 'purple';
        }
        return "info";
        }

    function getIssueCredentialMatchStatusCss(status) {
        var strStatusCss = 'info';

        if (status === enums.IssueCredentialStatusType.Match) {
            strStatusCss = 'info';
        } else if (status === enums.IssueCredentialStatusType.Unmatch) {
            strStatusCss = 'orange';
        }

        return strStatusCss;
    }

    function getTestSittingStatusCss(applicant) {
        var status = applicant.Status;
        var rejected = applicant.Rejected;
        var sat = applicant.Sat;
        var supplementary = applicant.Supplementary;
        var supplementaryRequest = applicant.SupplementaryCredentialRequest;

        var strStatusCss = 'gray';
        var statuses = enums.CredentialRequestStatusTypes;

        if (status === statuses.TestSessionAccepted) {
            strStatusCss = 'info';
        }

        if (status === statuses.ProcessingTestInvoice) {
            strStatusCss = 'dark-gray';
        }

        if (status === statuses.AwaitingTestPayment) {
            strStatusCss = 'dark-yellow';
        }

        if (status === statuses.CheckIn ||
            status === statuses.CertificationOnHold) {
            strStatusCss = 'orange';
        }

        if (status === statuses.TestSat ||
            status === statuses.TestFailed ||
            status === statuses.IssuedPassResult ||
            status === statuses.CertificationIssued ||
            status === statuses.UnderPaidTestReview ||
            status === statuses.AwaitingPaidReviewPayment ||
            status === statuses.ProcessingPaidReviewInvoice ||
            status === statuses.AwaitingSupplementaryTestPayment) {
            strStatusCss = 'success';
        }

        if (!supplementary && supplementaryRequest) {
            strStatusCss = 'success';
        }

        if (rejected) {
            strStatusCss = "gray";
        }
        return strStatusCss;
    }

    function getPersonSubScreenTitle(naatiNumber, givenName, surname, practitionerNumber, screenTitle) {
        if (practitionerNumber) {
            return '{0} - {1} {2} - {3} - {4}'.format(naatiNumber, givenName, surname, practitionerNumber, screenTitle);
        } else {
            return '{0} - {1} {2} - {3}'.format(naatiNumber, givenName, surname, screenTitle);
        }
    }

    function callerIn(instances) {
        try {
            var caller = arguments.callee;
            for (var i = 0; i < 20 && caller; i++) {
                for (var j = 0; j < instances.length; j++) {
                    var instance = instances[j];
                    for (var k = 0; k < instance.functions.length; k++) {
                        var f = instance.functions[k];
                        if (f == caller) {
                            return true;
                        }
                    }
                }
                caller = caller.caller;
            }
        }
        catch (err) { }

        return false;
    }

    return {
        isPageLoading: function () { return isPageLoading() },
        showLoadingIndicator: showLoadingIndicator,
        hideLoadingIndicator: hideLoadingIndicator,
        init: init,
        downloadFile: downloadFile,
        guid: guid,
        resetModel: resetModel,
        addressToString: addressToString,
        windowOpen: windowOpen,
        getTestSittingStatus: getTestSittingStatus,
        getTestSittingStatusCss: getTestSittingStatusCss,
        getRolePlayerStatusCss: getRolePlayerStatusCss,
        getMaterialRequestStatusCss: getMaterialRequestStatusCss,
        getMaterialRequestRoundStatusCss: getMaterialRequestRoundStatusCss,
        getIssueCredentialMatchStatusCss: getIssueCredentialMatchStatusCss,
        getTestMaterialStatusToolTip: getTestMaterialStatusToolTip,
        getTestMaterialStatusText: getTestMaterialStatusText,
        getTestMaterialStatusColor: getTestMaterialStatusColor,
        sortBy: sortBy,
        getPersonSubScreenTitle: getPersonSubScreenTitle,
        getTestMaterialDomainColor: getTestMaterialDomainColor,
        updateFromModel: updateFromModel,
        distinctBy: distinctBy,
        callerIn: callerIn,
        updateCredentialStatus: updateCredentialStatus
    };
});
