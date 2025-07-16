using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl
{
    public class MaterialRequestWizardLogicService : IMaterialRequestWizardLogicService
    {
        private readonly IEmailMessageQueryService _emailmessageQueryService;
        private readonly ITokenReplacementService _tokenReplacementService;
        private readonly IEnumerable<SystemActionTypeName> RequiresPublicNote = new SystemActionTypeName[] { };
        private readonly IEnumerable<SystemActionTypeName> RequiresPrivateNote = new SystemActionTypeName[] { };


        public MaterialRequestWizardLogicService(IEmailMessageQueryService emailmessageQueryService, ITokenReplacementService tokenReplacementService)
        {
            _emailmessageQueryService = emailmessageQueryService;
            _tokenReplacementService = tokenReplacementService;
        }
        public GenericResponse<NoteFieldRules> GetSystemNoteFieldRules(int actionId)
        {

            var actions = new[] { (SystemActionTypeName)actionId };
            var rules = new NoteFieldRules
            {
                ShowPrivateNote = true,
                RequirePublicNote = actions.Any(RequiresPublicNote.Contains),
                RequirePrivateNote = actions.Any(RequiresPrivateNote.Contains)
            };

            var emailTemplateReponse = _emailmessageQueryService.GetSystemEmailTemplate(new GetSystemEmailTemplateRequest
            {
                Actions = actions
            });

            var actionPublicNoteName = _tokenReplacementService.GetTokenNameFor(TokenReplacementField.ActionPublicNote);
            rules.ShowPublicNote =
                emailTemplateReponse.Data != null &&
                emailTemplateReponse.Data.Any(x => x.Active &&
                                                   (
                                                       x.Content.IndexOf($"[[{actionPublicNoteName}]]") != -1 ||
                                                       x.Subject.IndexOf($"[[{actionPublicNoteName}]]") != -1
                                                   ));

            return rules;
        }

        public GenericResponse<IEnumerable<MaterialRequestWizardStep>> GetMaterialRequestSteps(int actionId, int materialRequestId, int panelId)
        {
            var action = (SystemActionTypeName)actionId;
            var steps = new List<MaterialRequestWizardStep>();

            switch (action)
            {
                case SystemActionTypeName.CreateMaterialRequest:
                case SystemActionTypeName.CloneMaterialRequest:
                    if (panelId <= 0)
                    {
                        steps.Add(MaterialRequestWizardStep.Panel);
                    }
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.TestMaterial,
                        MaterialRequestWizardStep.Coordinator,
                        MaterialRequestWizardStep.TestMaterialSource,
                        MaterialRequestWizardStep.RoundDetails,
                        MaterialRequestWizardStep.ExistingDocuments,
                        MaterialRequestWizardStep.DocumentsUpload,
                        MaterialRequestWizardStep.RoundLinks,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });

                    break;
                case SystemActionTypeName.CreateMaterialRequestRound:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.RoundDetails,
                        MaterialRequestWizardStep.ExistingDocuments,
                        MaterialRequestWizardStep.DocumentsUpload,
                        MaterialRequestWizardStep.RoundLinks,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;

                case SystemActionTypeName.UploadFinalMaterialDocuments:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.ExistingDocuments,
                        MaterialRequestWizardStep.DocumentsUpload,
                        MaterialRequestWizardStep.TestMaterial,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;


                case SystemActionTypeName.RevertMaterialRequest:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;

                case SystemActionTypeName.UpdateMaterialRequest:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.TestMaterial,
                        MaterialRequestWizardStep.Coordinator,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;

                case SystemActionTypeName.CancelMaterialRequest:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;
                case SystemActionTypeName.UpdateMaterialRequestMembers:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Members,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;
                default:
                    throw new Exception($"Unexpected {nameof(SystemActionTypeName)}: {action}.");
            }

            return new GenericResponse<IEnumerable<MaterialRequestWizardStep>>(steps);
        }

        public GenericResponse<IEnumerable<MaterialRequestWizardStep>> GetMaterialRequestRoundSteps(int actionId, int materialRequestsRoundId)
        {
            var action = (SystemActionTypeName)actionId;
            var steps = new List<MaterialRequestWizardStep>();

            switch (action)
            {
                case SystemActionTypeName.SubmitRoundForApproval:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.DocumentsUpload,
                        MaterialRequestWizardStep.RoundLinks,
                        MaterialRequestWizardStep.Members,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;
                case SystemActionTypeName.ApproveMaterialRequestRound:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;

                case SystemActionTypeName.RejectMaterialRequestRound:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });

                    break;
                case SystemActionTypeName.RevertMaterialRequestRound:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;
                case SystemActionTypeName.UpdateMaterialRequestRound:
                    steps.AddRange(new[]
                    {
                        MaterialRequestWizardStep.RoundDetails,
                        MaterialRequestWizardStep.Notes,
                        MaterialRequestWizardStep.SendEmailCheckOption,
                        MaterialRequestWizardStep.EmailPreview,
                    });
                    break;
                default:
                    throw new Exception($"Unexpected {nameof(SystemActionTypeName)}: {action}.");
            }

            return new GenericResponse<IEnumerable<MaterialRequestWizardStep>>(steps);
        }

        public GenericResponse<IEnumerable<SystemActionNameModel>> GetValidMaterialRequestActions(int materialRequestStatusTypeId)
        {
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string>
            {
                [SystemActionTypeName.CloneMaterialRequest] = Naati.Resources.MaterialRequest.CloneMaterialRequestLabel,
                [SystemActionTypeName.CreateMaterialRequestRound] = Naati.Resources.MaterialRequest.CreateMaterialRequestRoundLabel,
                [SystemActionTypeName.RevertMaterialRequest] = Naati.Resources.MaterialRequest.RevertMaterialRequestLabel,
                [SystemActionTypeName.UploadFinalMaterialDocuments] = Naati.Resources.MaterialRequest.UploadFinalMaterialDocumentsLabel,
                [SystemActionTypeName.UpdateMaterialRequest] = Naati.Resources.MaterialRequest.UpdateMaterialRequestLabel,
                [SystemActionTypeName.CancelMaterialRequest] = Naati.Resources.MaterialRequest.CancelMaterialRequestLabel,
                [SystemActionTypeName.UpdateMaterialRequestMembers] = Naati.Resources.MaterialRequest.UpdateMaterialRequestMembersLabel,
            };
            var status = (MaterialRequestStatusTypeName)materialRequestStatusTypeId;

            switch (status)
            {

                case MaterialRequestStatusTypeName.InProgress:
                    actions = new[] { SystemActionTypeName.CloneMaterialRequest, SystemActionTypeName.CreateMaterialRequestRound, SystemActionTypeName.UpdateMaterialRequest, SystemActionTypeName.CancelMaterialRequest, SystemActionTypeName.UpdateMaterialRequestMembers };
                    break;
                case MaterialRequestStatusTypeName.AwaitingFinalisation:
                    actions = new[] { SystemActionTypeName.UploadFinalMaterialDocuments, SystemActionTypeName.CloneMaterialRequest, SystemActionTypeName.UpdateMaterialRequest, };
                    break;
                case MaterialRequestStatusTypeName.Finalised:
                    actions = new[] { SystemActionTypeName.CloneMaterialRequest, SystemActionTypeName.RevertMaterialRequest };
                    break;
                case MaterialRequestStatusTypeName.Cancelled:
                    actions = new[] { SystemActionTypeName.RevertMaterialRequest, SystemActionTypeName.CloneMaterialRequest, };
                    break;

                default:
                    throw new Exception($"Unexpected {nameof(MaterialRequestStatusTypeName)}: {status}.");
            }

            return new GenericResponse<IEnumerable<SystemActionNameModel>>(
                actions.Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }

        public GenericResponse<IEnumerable<SystemActionNameModel>> GetValidMaterialRequestRoundActions(int materialRequestRoundStatusTypeId)
        {
            var actions = new SystemActionTypeName[] { };
            var actionDisplayNames = new Dictionary<SystemActionTypeName, string>
            {
                [SystemActionTypeName.SubmitRoundForApproval] = Naati.Resources.MaterialRequest.SubmitMaterialRequestRoundLabel,
                [SystemActionTypeName.ApproveMaterialRequestRound] = Naati.Resources.MaterialRequest.ApproveMaterialRequestRoundLabel,
                [SystemActionTypeName.RejectMaterialRequestRound] = Naati.Resources.MaterialRequest.RejectMaterialRequestRoundLabel,
                [SystemActionTypeName.RevertMaterialRequestRound] = Naati.Resources.MaterialRequest.RevertMaterialRequestRoundLabel,
                [SystemActionTypeName.UpdateMaterialRequestRound] = Naati.Resources.MaterialRequest.UpdateMaterialRequestRoundLabel,
            };

            var status = (MaterialRequestRoundStatusTypeName)materialRequestRoundStatusTypeId;

            switch (status)
            {
                case MaterialRequestRoundStatusTypeName.SentForDevelopment:
                    actions = new[] { SystemActionTypeName.SubmitRoundForApproval, SystemActionTypeName.RejectMaterialRequestRound, SystemActionTypeName.UpdateMaterialRequestRound, };
                    break;
                case MaterialRequestRoundStatusTypeName.AwaitingApproval:
                    actions = new[] { SystemActionTypeName.ApproveMaterialRequestRound, SystemActionTypeName.RejectMaterialRequestRound, };
                    break;
                case MaterialRequestRoundStatusTypeName.Approved:
                    actions = new[] { SystemActionTypeName.RevertMaterialRequestRound };
                    break;
                case MaterialRequestRoundStatusTypeName.Rejected:
                    actions = new[] { SystemActionTypeName.RevertMaterialRequestRound };
                    break;

                case MaterialRequestRoundStatusTypeName.Cancelled:
                    actions = new SystemActionTypeName[] { };
                    break;

                default:
                    throw new Exception($"Unexpected {nameof(MaterialRequestRoundStatusTypeName)}: {status}.");
            }

            return new GenericResponse<IEnumerable<SystemActionNameModel>>(
                actions.Select(x => new SystemActionNameModel
                {
                    Id = (int)x,
                    Name = actionDisplayNames.ContainsKey(x) ? actionDisplayNames[x] : x.ToString()
                }));
        }
    }
}
