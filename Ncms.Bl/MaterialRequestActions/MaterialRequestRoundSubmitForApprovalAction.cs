using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRoundSubmitForApprovalAction : MaterialRequestRoundAction
    {
        protected override MaterialRequestRoundStatusTypeName[] MaterialRequestRoundEntryStates => new[] { MaterialRequestRoundStatusTypeName.SentForDevelopment };

        protected override MaterialRequestRoundStatusTypeName MaterialRequestRoundExitState => MaterialRequestRoundStatusTypeName.AwaitingApproval;

        private IEnumerable<MaterialRequestRoundAttachmentModel> _roundDocuments = new List<MaterialRequestRoundAttachmentModel>();

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                };

                if (WizardModel.Source == SystemActionSource.MyNaati)
                {
                    //actions.Add(ValidateParticipants);
                    //actions.Add(ValidateDocumentsAndLinks);
                    actions.Add(ValidateSubmittedHours);
                    actions.Add(ValidateTestMaterialDomain);
                }
                return actions;
            }
        }

        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>();

                if (WizardModel.Source == SystemActionSource.MyNaati)
                {
                    actions.Add(EnableDocuments);
                    actions.Add(EnableRoundLinks);
                    actions.Add(SetTestMaterialDomain);
                }
                else
                {
                    actions.Add(AddDocuments);
                    actions.Add(AddRoundLinks);
                    actions.Add(AddMembers);
                }
                actions.Add(SetSubmittedDate);
                actions.Add(CreateNote);
                actions.Add(CreatePublicNote);
                actions.Add(SetExitState);
                return actions;
            }
        }

        protected void ValidateTestMaterialDomain()
        {
            if (WizardModel.TestMaterialDomainId == 0 || WizardModel.TestMaterialDomainId == (int)TestMaterialDomainTypeName.Undefined)
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.TestMaterialDomainRequired);
            }
        }

        protected void EnableDocuments()
        {

            if ((_roundDocuments?.Count() ?? 0) == 0)
            {
                _roundDocuments = MaterialRequestService.ListAttachments(new ListAttachmentsRequestModel()
                {
                    MaterialRequestRoundId = RoundModel.MaterialRoundId,
                    ExaminersAvailable = true
                });
            }

            foreach (var document in _roundDocuments)
            {
                RoundModel.Documents.Add(new MaterialRequestDocumentInfoModel()
                {
                    AttachmentId = document.MaterialRequestRoundAttachmentId,
                    StoredFileId = document.StoredFileId,
                    Description = document.Description,
                    ExaminersAvailable = document.EportalDownload,
                    NcmsAvailable = true
                });
            }
        }
        protected void EnableRoundLinks()
        {
            foreach (var link in RoundModel.Links)
            {
                link.NcmsAvailable = true;
            }
        }

        //protected void ValidateParticipants()
        //{

        //}


        public void ValidateSubmittedHours()
        {
            var totalHours = ActionModel.MaterialRequestInfo.Members.Sum(x => x.Tasks.Sum(y => y.HoursSpent));
            if (totalHours > ActionModel.MaterialRequestInfo.MaxBillableHours)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MaxBillableHoursRequired, ActionModel.MaterialRequestInfo.MaxBillableHours));
            }
        }

        //protected void ValidateDocumentsAndLinks()
        //{
        //    _roundDocuments = MaterialRequestService.ListAttachments(new ListAttachmentsRequestModel()
        //    {
        //        MaterialRequestRoundId = RoundModel.MaterialRoundId,
        //        ExaminersAvailable = true
        //    });

        //    if (!_roundDocuments.Any())
        //    {
        //        if (!RoundModel.Links.Any(x => x.PersonId > 0))
        //        {
        //            throw new UserFriendlySamException(Resources.MaterialRequest.DocumentsRequired);
        //        }
        //    }
        //}

        protected virtual void SetSubmittedDate()
        {
            RoundModel.SubmittedDate = DateTime.Now;
        }

        protected override void AddMembers()
        {
            CheckMembers();
        }

    }
}
