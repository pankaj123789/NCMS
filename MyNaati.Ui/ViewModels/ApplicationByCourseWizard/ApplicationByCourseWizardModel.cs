using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.ViewModels.Shared;
using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class WizardModel : ApplicationWizardModel
    {
        private const string LEVEL_3 = "Professional";
        public WizardModel(IAutoMapperHelper autoMapperHelper) : base(autoMapperHelper)
        {
            Steps = new List<WizardStep>(new[]
            {
                new WizardStep() { Name = "Course Details", Action = "CourseDetails" },
                new WizardStep() {Name = "Credential Details", Action = "CredentialDetails"},
                new WizardStep() {Name = "Personal Details", Action = "PersonalDetails"},
                new WizardStep() {Name = "Address", Action = "Address"},
                new WizardStep() {Name = "Phone", Action = "Phone"},
                new WizardStep() {Name = "Email", Action = "Email"},
                new WizardStep() {Name = "Residency Status", Action = "ResidencyStatus"},
                new WizardStep() {Name = "Purpose of Credentials", Action = "PurposeOfCredentials"},
                new WizardStep() {Name = "Download Form", Action = "DownloadForm"},
            });
            Steps[0].AllowDirectNavigation = true;
        }

        public CourseDetailsModel CourseDetails { get; set; }
        public CredentialDetailsModel CredentialDetails { get; set; }
        public DownloadFormModel DownloadForm { get; set; }

        public void UpdateCourseDetails(CourseDetailsModel courseDetails)
        {
            if (courseDetails != null)
            {
                if (CourseDetails != null && CourseDetails.CourseId != courseDetails.CourseId)
                {
                    ClearCredentialDetails();
                }

                CourseDetails = courseDetails;
            }
        }
        
        public void BuildCourseDetailsLists()
        {
            if (CourseDetails == null)
                throw new NullReferenceException("CourseDetails is null");
            CourseDetails.BuildLists();
        }

        public void ClearCredentialDetails()
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            CredentialDetails.RequestedCredentials = new List<CredentialDetailsRow>();
        }

        // Returns an error that should be added to the modelstate (or nothing)
        public string AddCredential(CredentialDetailsRow credential)
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            if (CredentialDetails.RequestedCredentials.Any(e => CredentialDetailsRow.Equals(credential, e)))
                return string.Format("There is already a request for {0}, {1}.", credential.LevelName, credential.Direction);
            CredentialDetails.RequestedCredentials.Add(credential);
            return null;
        }

        public void DeleteCredential(CredentialDetailsRow credential)
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            CredentialDetails.RequestedCredentials.RemoveAll(e => CredentialDetailsRow.Equals(credential, e));
        }

        public void DeleteCredentialAt(int credentialIndex)
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            if (CredentialDetails.RequestedCredentials.Count > credentialIndex && credentialIndex >= 0)
                CredentialDetails.RequestedCredentials.RemoveAt(credentialIndex);
        }

        public void BuildCredentialDetailsLists()
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            CredentialDetails.BuildLists(CourseDetails.CourseId, CourseDetails.DateCompleted);
        }

        public void CalculateApplicationFees(ApplicationFee[] applicationFees)
        {
            if (DownloadForm == null)
                throw new NullReferenceException("DownloadForm is null");
            DownloadForm.CalculateApplicationFees(this, applicationFees);
        }

        internal void UpdateForSkillsAssessment()
        {
            if (DownloadForm == null)
                throw new NullReferenceException("DownloadForm is null");
            if (PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOfCredentials is null");

            DownloadForm.SkillsAssessmentRequested = PurposeOfCredentials.SkillsAssessment;
        }

        public override void UpdateForCredentialRequests()
        {
            if (CredentialDetails == null)
                throw new NullReferenceException("CredentialDetails is null");
            if (this.PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOdCredentials is null");
            PurposeOfCredentials.AtLeastLevel3Professional = CredentialDetails.RequestedCredentials.Any(x => x.LevelName.Contains(LEVEL_3));            
        }
    }
}