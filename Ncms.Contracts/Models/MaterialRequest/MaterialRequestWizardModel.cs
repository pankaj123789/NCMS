using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest.Wizard;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestWizardModel : SystemActionWizardModel
    {
        public int MaterialRequestId { get; set; }
        public int MaterialRequestRoundId { get; set; }

        public int TestMaterialDomainId { get; set; }
        private SystemActionSource? _source;
        public override SystemActionSource Source => _source ?? SystemActionSource.Ncms;
        protected MembersStepModel MembersModel => GetStepModel<MembersStepModel>(MaterialRequestWizardStep.Members);

        protected RoundLinksStepModel RoundLinksModel => GetStepModel<RoundLinksStepModel>(MaterialRequestWizardStep.RoundLinks);
        public string[] RoundLinks => RoundLinksModel?.Links ?? new string[0];
        public CoordinatorStepModel Coordinator => GetStepModel<CoordinatorStepModel>(MaterialRequestWizardStep.Coordinator);
        public MaterialRequestMember[] Members => MembersModel?.SelectedMembers ?? new MaterialRequestMember[0];
     
        public RoundDetailsStepModel RoundDetails => GetStepModel<RoundDetailsStepModel>(MaterialRequestWizardStep.RoundDetails);
        public TestMaterialStepModel OutputTestMaterial => GetStepModel<TestMaterialStepModel>(MaterialRequestWizardStep.TestMaterial);
        public TestMaterialSourceStepModel TestMaterialSource => GetStepModel<TestMaterialSourceStepModel>(MaterialRequestWizardStep.TestMaterialSource);
        public NotesStepModel Notes => GetStepModel<NotesStepModel>(MaterialRequestWizardStep.Notes);
        public ExistingDocumentsStepsModel ExistingDocumentsModel => GetStepModel<ExistingDocumentsStepsModel>(MaterialRequestWizardStep.ExistingDocuments);

        public DocumentLookupTypeModel[] ExistingDocuments => ExistingDocumentsModel?.Documents ?? new DocumentLookupTypeModel[0];
        public DocumentUploadModel[] NewDocuments => NewDocumentsModel?.Documents ?? new DocumentUploadModel[0];
        public DocumentsUploadStepModel NewDocumentsModel => GetStepModel<DocumentsUploadStepModel>(MaterialRequestWizardStep.DocumentsUpload);

        public SendEmailCheckOptionStepModel SendEmailCheckOptions => GetStepModel<SendEmailCheckOptionStepModel>(MaterialRequestWizardStep.SendEmailCheckOption);
        public override bool SendEmail => (SendEmailCheckOptions == null || (bool)(SendEmailCheckOptions?.Checked ?? false));

        public MaterialRequestGroupingModel Approval { get; set; }
        public MaterialRequestMemberGroupingModel ExaminerPayment { get; set; }

        public override string PublicNotes => Notes?.PublicNote;
        public override string PrivateNotes => Notes?.PrivateNote;

        public void SetSource(SystemActionSource source)
        {
            _source = source;
        }
        private T GetStepModel<T>(MaterialRequestWizardStep step)
        {
            var stepData = Steps.FirstOrDefault(x => x.Id == (int) step);
            return (T)stepData?.Data.ToObject<T>();
        }
 
    }
}
