define([
    'modules/collapser',
    'modules/common',
    'modules/enums',
], function (collapser, common, enums) {
    function ViewModel(params) {
        var self = this;

        var defaultParams = {
            credentialRequests: ko.observableArray(),
            showActions: false,
            hideApplicationType: false,
            selectAction: function () { },
        };

        $.extend(defaultParams, params);
        
        self.credentialRequests = defaultParams.credentialRequests;
        self.showActions = defaultParams.showActions;
        self.hideApplicationType = defaultParams.hideApplicationType;
        self.selectAction = defaultParams.selectAction;

        self.collapser = collapser.create(self.credentialRequests);

        self.humanizeDate = function (date) {
            return common.functions().humanizeDate(moment(date).toDate());
        };

        self.testSessionLinkText = function (testSession) {
            var location = testSession.TestLocation() + ', ' + testSession.TestLocationState();
            return ko.Localization('Naati.Resources.Application.resources.TestSessionLinkText').format(testSession.TestSessionId(), moment(testSession.TestDate()).format(CONST.settings.shortDateTimeDisplayFormat), location);
        };

        self.attendanceLinkText = function (testSession) {
            var text = ko.Localization('Naati.Resources.Application.resources.AttendanceLinkText').format(testSession.CredentialTestSessionId());
            if (ko.toJS(testSession.Supplementary)) {
                text += ', ' + ko.Localization('Naati.Resources.Test.resources.SupplementaryTest');
            }
            if (!ko.toJS(testSession.HasDefaultSpecification)) {
                text += ', ' + ko.Localization('Naati.Resources.Test.resources.NotDefaultSpecification');
            }
            return text;
        };

        self.CredentialRequestIsAutoCreated = function (credentialRequest) {
            var autoCreated = credentialRequest.AutoCreated();

            return autoCreated;
        }

        self.credentialStatusClass = function (credentialRequest) {
            var statuses = enums.CredentialRequestStatusTypes;
            var status = credentialRequest.StatusTypeId();

            if (status === statuses.Draft) {
                return 'gray';
            }

            if (status === statuses.ProcessingSubmission) {
                return 'dark-gray';
            }

            if (status === statuses.RequestEntered) {
                return 'info';
            }
            if (status === statuses.ReadyForAssessment) {
                return 'info';
            }
            if (status === statuses.BeingAssessed || status === statuses.CertificationOnHold) {
                return 'orange';
            }
            if (status === statuses.Pending) {
                return 'dark-yellow';
            }
            if (status === statuses.AssessmentFailed) {
                return 'danger';
            }
            if (status === statuses.AssessmentPaidReview) {
                return 'orange';
            }
            if (status === statuses.EligibleForTesting) {
                return 'info';
            }
            if (status === statuses.AssessmentSuccessful) {
                return 'orange';
            }
            if (status === statuses.Rejected) {
                return 'danger';
            }
            if (status === statuses.TestFailed) {
                return 'danger';
            }
            if (status === statuses.CredentialIssued) {
                return 'success';
            }
            if (status === statuses.Withdrawn) {
                return 'purple';
            }
            if (status === statuses.Canceled) {
                return 'purple';
            }
            if (status === statuses.Deleted) {
                return 'purple';
            }
            if (status === statuses.AwaitingTestPayment || status === statuses.AwaitingApplicationPayment || status === statuses.AwaitingSupplementaryTestPayment) {
                return 'dark-yellow';
            }
            if (status === statuses.TestAccepted) {
                return 'info';
            }
            if (status === statuses.TestSat) {
                return 'orange';
            }
            if (status === statuses.CheckIn) {
                return 'orange';
            }
            if (status === statuses.UnderPaidTestReview) {
                return 'orange';
            }
            if (status === statuses.IssuedPassResult) {
                return 'orange';
            }
            if (status === statuses.ProcessingPaidReviewInvoice) {
                return 'dark-gray';
            }
            if (status === statuses.AwaitingPaidReviewPayment) {
                return 'dark-yellow';
            }
            if (status === statuses.SupplementaryTestInvoiceProcessed) {
                return 'dark-gray';
            }
            if (status === statuses.TestInvalidated) {
                return 'purple';
            }
            if (status === statuses.ToBeIssued) {
                return 'dark-gray';
            }
            if (status === statuses.ProcessingTestInvoice) {
                return 'dark-gray';
            }
            if (status === statuses.ProcessingRequest) {
                return 'dark-gray';
            }
            if (status === statuses.RefundRequested) {
                return 'orange';
            }
            if (status === statuses.ProcessingCreditNote) {
                return 'dark-gray';
            }
            if (status === statuses.AwaitingCreditNotePayment) {
                return 'orange';
            }
            if (status === statuses.RefundRequestApproved) {
                return 'dark-gray';
            }
            if (status === statuses.RefundFailed) {
                return 'orange';
            }
            if (status === statuses.OnHoldToBeIssued)
            {
                return 'orange';
            }
            else {
                return 'success';
            }
        };
    };

    return ViewModel;
});
