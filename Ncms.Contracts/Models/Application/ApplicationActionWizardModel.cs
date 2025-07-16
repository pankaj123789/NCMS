using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Application.Wizard;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.System;
using Ncms.Contracts.Models.Test;

namespace Ncms.Contracts.Models.Application
{
    public class ApplicationActionWizardModel : SystemActionWizardModel
    {
        private Guid? _invoiceId;
        private string _invoiceNumber;
        private bool _processInBatch;
        private bool _backgroundAction;
        private IssueCredentialDataModel _issueCredentialData;
        private double? _refundPercentage;
        private int? _refundMethodTypeId;
        private int? _credentialWorkflowFeeId;
        private decimal? _refundAmount;
        private string _refundComments;
        private string _refundBankDetails;

        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public int? CredentialId { get; set; }
        public string TransactionId { get; protected set; }
        public string OrderNumber { get; protected set; }
        public virtual StepDataModel NotesStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.Notes);
        public virtual StepDataModel SupplementaryTestStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.SupplementaryTest);
        public virtual int[] SupplementaryTestComponents => (int[])SupplementaryTestStep?.Data?.Components?.ToObject<int[]>() ?? new int[0];
        public virtual string InvoiceReference => Data?[1].Data.InvoiceReference?.Value;
        public override string PublicNotes => NotesStep?.Data?.PublicNote?.Value;
        public override string PrivateNotes => NotesStep?.Data?.PrivateNote?.Value;
        public string PaymentReference { get; protected set; }
        public decimal? PaymentAmount { get; protected set; }

        public virtual StepDataModel ConfigureRefundStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.ConfigureRefund);
        public virtual StepDataModel RefundApprovalStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.ApproveRefund);
        public virtual RefundStepModel RefundStepData => ConfigureRefundStep?.Data?.ToObject<RefundStepModel>() ?? RefundApprovalStep?.Data?.ToObject<RefundStepModel>();
       
        public override bool SendEmail => (CheckOptionStep == null || (bool)(CheckOptionStep?.Data.Checked ?? false));

        public virtual StepDataModel CheckOptionStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.CheckOption);
        public virtual StepDataModel DocumentsPreviewStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.DocumentsPreview);

        public virtual StepDataModel ViewInvoiceStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.ViewInvoice);
        public bool DoNotInvoice => ViewInvoiceStep?.Data.DoNotInvoice?.Value ?? false;
        public virtual IssueCredentialDataModel IssueCredentialData => _issueCredentialData ?? (IssueCredentialDataModel)IssueCredentialStep?.Data
            .ToObject<IssueCredentialDataModel>();
        public virtual StepDataModel IssueCredentialStep => Steps.FirstOrDefault(x => x.Id == (int)ApplicationWizardSteps.IssueCredential);
        public DocumentData[] CredentialPreviewFiles => DocumentsPreviewStep?.Data.ToObject<DocumentData[]>() ?? new DocumentData[0];
        public virtual int? TestSessionId { get; set; }
        public virtual Guid? InvoiceId => _invoiceId;
        public virtual string InvoiceNumber => _invoiceNumber;
        public virtual DateTime DueDate => DateTime.Today.AddDays(3);
        public virtual StepDataModel TestSessionStep => null;
        public virtual int? TestSittingId { get; private set; }

        public virtual int? CredentialTypeId
        {
            get { return (int?)Data?[0]?.Data?.CredentialTypeId?.Value; }
            set { }
        }

        public virtual int? SkillId
        {
            get { return (int?)Data?[0]?.Data?.SkillId?.Value; }
            set { }
        }

        public virtual int? CategoryId
        {
            get { return (int?)Data?[0]?.Data?.CategoryId?.Value; }
            set { }
        }

        public double? RefundPercentage => _refundPercentage;
        public int? RefundMethodTypeId => _refundMethodTypeId;
        public int? CredentialWorkflowFeeId => _credentialWorkflowFeeId;
        public decimal? RefundAmount => _refundAmount;
        public string RefundComments => _refundComments;
        public string RefundBankDetails => _refundBankDetails;
        public bool ProcessInBatch => _processInBatch;
        public bool IsBackgroundAction => _backgroundAction;
        public virtual TestSessionRequestModel TestSessionRequestModel => null;
        public virtual TestMaterialAssignmentModel[] TestMaterialAssignments { get; } = new TestMaterialAssignmentModel[0];

        public virtual void SetNewTestSessionId(int testSessionId)
        {
            TestSessionId = testSessionId;
        }

        public void SetProcessInBatch(bool processInBatch)
        {
            _processInBatch = processInBatch;
        }
        public void SetBackGroundAction(bool backGroundAction)
        {
            _backgroundAction = backGroundAction;
        }

        public void SetIssueCredentialData(IssueCredentialDataModel data)
        {
            _issueCredentialData = data;
        }

        public virtual void SetInvoiceDetails(Guid? invoiceId, string invoiceNumber)
        {
            _invoiceId = invoiceId;
            _invoiceNumber = invoiceNumber;
        }

        public void SetRefundDetails(
            double? refundPercentage, 
            int? refundMethodTypeId, 
            int? credentialWorkflowFeeId,
            decimal? refundAmount,
            string comments,
            string refundBankDetails)
        {
            _refundPercentage = refundPercentage;
            _refundMethodTypeId = refundMethodTypeId;
            _credentialWorkflowFeeId = credentialWorkflowFeeId;
            _refundAmount = refundAmount;
            _refundComments = comments;
            _refundBankDetails = refundBankDetails;
        }

        public void SetPaymentReference(string paymentReference, decimal? paymentAmount, string transactionId, string orderNumber)
        {
            PaymentReference = paymentReference;
            PaymentAmount = paymentAmount;
            TransactionId = transactionId;
            OrderNumber = orderNumber;
        }
    }






    public class MyNaatiApplicationActionWizardModel : ApplicationActionWizardModel
    {
        private int? _testSessionId;
        public override int? TestSessionId => _testSessionId;
        private string _invoiceReference;

        private DateTime? _dueDate;
        public override SystemActionSource Source => SystemActionSource.MyNaati;

        public override string InvoiceReference => _invoiceReference;



        public override DateTime DueDate => _dueDate ?? DateTime.Today.AddDays(3);

        public void SetTestessionId(int testSessionId)
        {
            _testSessionId = testSessionId;
        }

        public void SetInvoiceReference(string invoiceReference)
        {
            _invoiceReference = invoiceReference;
        }

        public void SetDueDate(DateTime? dueDate)
        {
            _dueDate = dueDate;
        }


    }

    public class TestMaterialAssignmentBulkModel : ApplicationActionWizardModel
    {

        private int _testSittingId;
        public override int ActionType => (int)SystemActionTypeName.AssignTestMaterial;
        public override StepDataModel[] Steps => (StepDataModel[])Data?.Steps?.ToObject<StepDataModel[]>() ?? new StepDataModel[0];
        public virtual int[] TestSessionIds => (int[])Data.TestSessionIds?.ToObject<int[]>() ?? new int[] { };
        public virtual int TestSpecificationId => (int)Data.TestSpecificationId;
        public override int? SkillId => (int)Data.SkillId;
        public override TestMaterialAssignmentModel[] TestMaterialAssignments => (TestMaterialAssignmentModel[])Data.TestMaterialAssignments?.ToObject<TestMaterialAssignmentModel[]>() ?? new TestMaterialAssignmentModel[] { };

        public void SetTestSittingId(int testSittingId)
        {
            _testSittingId = testSittingId;
        }

        public override int? TestSittingId => _testSittingId;
    }

    public class CredentialRequestsBulkActionWizardModel : ApplicationActionWizardModel
    {
        private int? _newTestSessionId;
        public override int ActionType => (int)Data.Action;

        public virtual int CredentialApplicationTypeId => (int)Data.CredentialApplicationTypeId;
        public override int? CredentialTypeId => (int)Data.CredentialTypeId;
        public override int? SkillId => (int)Data.SkillId;

        public override StepDataModel ViewInvoiceStep => Steps.FirstOrDefault(x => x.Id == (int) CredentialRequestWizardSteps.ViewInvoice);
       
        public override DateTime DueDate => ViewInvoiceStep?.Data.DueDate?.Value ?? DateTime.Today.AddDays(3);
        public override string  InvoiceReference => ViewInvoiceStep?.Data.InvoiceReference.Value;
        public int CredentialRequestStatusId => (int)Data.CredentialRequestStatusTypeId;

        public override StepDataModel CheckOptionStep => Steps.FirstOrDefault(x => x.Id == (int)CredentialRequestWizardSteps.CheckOption);

        public override StepDataModel NotesStep => Steps.FirstOrDefault(x => x.Id == (int)CredentialRequestWizardSteps.Notes);

        public override StepDataModel TestSessionStep => Steps.FirstOrDefault(x => x.Id == (int)CredentialRequestWizardSteps.TestSession);

        public virtual StepDataModel NewApplicantsStep => Steps.FirstOrDefault(x => x.Id == (int)CredentialRequestWizardSteps.NewApplicants);

        public int TestLocationId => (int)Data.TestLocationId;

        public override StepDataModel[] Steps => (StepDataModel[])Data.Steps.ToObject<StepDataModel[]>();

        public override int? TestSessionId => _newTestSessionId ?? ((int?)TestSessionStep?.Data?.Id?.Value ?? 0);

        public virtual int[] CredentialRequestIds => (int[])Data.CredentialRequestIds?.ToObject<int[]>() ?? new int[] { };

        public override TestSessionRequestModel TestSessionRequestModel
        {
            get
            {
                var session = (TestSessionRequestModel)TestSessionStep.Data.ToObject<TestSessionRequestModel>();
                session.Skills = new TestSessionSkillModel[] { };
                return session;
            }
        }

        public override void SetNewTestSessionId(int testSessionId)
        {
            _newTestSessionId = testSessionId;
        }

    }


    public class TestSessionBulkActionWizardModel : CredentialRequestsBulkActionWizardModel
    {
        public int _actionType;
        public bool _sendEmail;

        public int[] _credentialRequestIds;
        public override int ActionType => _actionType;
        public override StepDataModel NewApplicantsStep => Steps.FirstOrDefault(x => x.Id == (int)TestSessionWizardSteps.MatchingApplicants);
        public virtual StepDataModel SkillStep => Steps.FirstOrDefault(x => x.Id == (int)TestSessionWizardSteps.Skills);
        public override int[] CredentialRequestIds => _credentialRequestIds ?? (int[])NewApplicantsStep?.Data.Applicants?.ToObject<int[]>() ?? new int[] { };
        public override int CredentialApplicationTypeId => (int)Data.ApplicationTypeId;

        public override bool SendEmail => _sendEmail;

        public virtual bool SendEmailToApplicants => (bool)(CheckOptionStep?.Data.NotifyApplicantsChecked ?? false);
        public virtual bool SendEmailToRolePlayers => (bool)(CheckOptionStep?.Data.NotifyRolePlayersChecked ?? false);

        public override StepDataModel CheckOptionStep => Steps.FirstOrDefault(x => x.Id == (int)TestSessionWizardSteps.CheckOption);
        public void SetAction(SystemActionTypeName action)
        {
            _actionType = (int)action;
        }

        public void SetCredentialRequests(int[] credentialRequestIds)
        {
            _credentialRequestIds = credentialRequestIds;
        }
        public void SetSendEmailFlag(bool sendEmail)
        {
            _sendEmail = sendEmail;
        }
        //Todo: Improve this
        public override StepDataModel TestSessionStep
        {
            get
            {
                var stepModel = new StepDataModel
                {
                    Id = (int)TestSessionWizardSteps.Details,
                    Data = Data
                };

                return stepModel;
            }
        }
        public TestSessionDetails TestSessionDetails => (TestSessionDetails)TestSessionStep.Data.ToObject<TestSessionDetails>();
        public override TestSessionRequestModel TestSessionRequestModel
        {
            get
            {
                var details = TestSessionDetails;
                var request = new TestSessionRequestModel
                {
                    Id = details.Id,
                    SessionName = details.Name,
                    TestDate = details.TestDate,
                    TestTime = details.TestTime,
                    CredentialApplicationTypeId = details.ApplicationTypeId,
                    Notes = details.PublicNote,
                    PreparationTime = details.PreparationTime,
                    SessionDuration = details.SessionDuration,
                    CredentialTypeId = details.CredentialTypeId,
                    VenueId = details.VenueId,
                    AllowSelfAssign = details.AllowSelfAssign,
                    OverrideVenueCapacity = details.OverrideVenueCapacity,
                    NewCandidatesOnly = details.NewCandidatesOnly,
                    Capacity = details.Capacity,
                    RehearsalDate = details.RehearsalDate,
                    RehearsalTime = details.RehearsalTime,
                    RehearsalNotes = details.RehearsalNotes,
                    DefaultTestSpecificationId = details.DefaultTestSpecificationId,
                    IsActive = details.IsActive,
                };
                var sessionSkills = (TestSessionSkillModel[])SkillStep?.Data.Skills?.ToObject<TestSessionSkillModel[]>() ??
                             new TestSessionSkillModel[] { };

                var skillToSave = new List<TestSessionSkillModel>();
                foreach (var testSessionSkillModel in sessionSkills)
                {
                    if (!testSessionSkillModel.Selected)
                    {
                        testSessionSkillModel.MaximumCapacity = 0;
                    }
                    if (testSessionSkillModel.Selected)
                    {
                        skillToSave.Add(testSessionSkillModel);
                    }
                }

                request.Skills = skillToSave.ToArray();

                return request;
            }
        }
    }

    public class AutoCreateCredentialRequestNonWizardModel : ApplicationActionWizardModel
    {
        public override int ActionType { get; set; }
        public override int? CredentialTypeId { get; set; }
        public override int? SkillId { get; set; }
        public override int? CategoryId { get; set; }
    }


}