using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Bl.MaterialRequestActions;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using SearchRequest = Ncms.Contracts.SearchRequest;

namespace Ncms.Bl
{
    public class MaterialRequestService : IMaterialRequestService
    {
        private readonly IMaterialRequestQueryService _materialRequestQueryService;
        private readonly IUtilityQueryService _utilityQueryService;
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly ISystemQueryService _systemQueryService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEmailMessageQueryService _emailMessageQueryService;
        private readonly IMaterialRequestPayRollHelper _materialRequestPayRollHelper;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public MaterialRequestService(IMaterialRequestQueryService materialRequestQueryService, IUtilityQueryService utilityQueryService, ITestMaterialQueryService testMaterialQueryService, ISystemQueryService systemQueryService, IFileStorageService fileStorageService, IEmailMessageQueryService emailMessageQueryService, IMaterialRequestPayRollHelper materialRequestPayRollHelper, IAutoMapperHelper autoMapperHelper)
        {
            _materialRequestQueryService = materialRequestQueryService;
            _utilityQueryService = utilityQueryService;
            _testMaterialQueryService = testMaterialQueryService;
            _systemQueryService = systemQueryService;
            _fileStorageService = fileStorageService;
            _emailMessageQueryService = emailMessageQueryService;
            _materialRequestPayRollHelper = materialRequestPayRollHelper;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<IEnumerable<EmailMessageModel>> GetEmailPreview(MaterialRequestWizardModel wizardModel)
        {
            MaterialRequestActionModel actionModel = GetMaterialRequestActionModel(wizardModel.MaterialRequestId);
            var action = MaterialRequestAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, actionModel, wizardModel);
            var emails = action.GetEmailPreviews();

            return new GenericResponse<IEnumerable<EmailMessageModel>>(emails);
        }


        public MaterialRequestActionModel GetEmptyActionModel()
        {
            return new MaterialRequestActionModel
            {
                MaterialRequestInfo = new MaterialRequestInfoModel() { },
                Rounds = new List<MaterialRequestRoundModel>(),
                Notes = new List<MaterialRequestActionNoteModel>(),
                PublicNotes = new List<MaterialRequestActionPublicNoteModel>(),
                PersonNotes = new List<MaterialRequestPersonNoteModel>(),
                Documents = new List<OutputTestMaterialDocumentInfoModel>()
            };
        }

        public GenericResponse<IEnumerable<ValidationResultModel>> ValidateActionPreconditions(MaterialRequestWizardModel wizardModel)
        {
            LoggingHelper.LogVerbose("Validating Action {Action}, {@WizardModel}", (SystemActionTypeName)wizardModel.ActionType, wizardModel);

            var model = GetMaterialRequestActionModel(wizardModel.MaterialRequestId);

            var action = MaterialRequestAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, model, wizardModel);
            try
            {
                action.ValidatePreconditions();
            }
            catch (UserFriendlySamException ex)
            {
                action.ValidationErrors.Add(new ValidationResultModel { Field = string.Empty, Message = ex.Message });
            }
            catch (Exception ex)
            {
                action.ValidationErrors.Add(new ValidationResultModel { Field = string.Empty, Message = "Error Validating Application" });
                LoggingHelper.LogException(ex, "Error validating material request MR{MaterialRequestId}", wizardModel.MaterialRequestId);
            }

            return new GenericResponse<IEnumerable<ValidationResultModel>>(action.ValidationErrors);
        }


        private MaterialRequestActionModel GetMaterialRequestActionModel(int materialRequestId)
        {
            if (materialRequestId == 0)
            {
                return GetEmptyActionModel();
            }

            var response = _materialRequestQueryService.GetMaterialRequestActionData(materialRequestId);

            var result = _autoMapperHelper.Mapper.Map<MaterialRequestActionModel>(response);
            return result;
        }

        public GenericResponse<UpsertMaterialRequestResultModel> PerformAction(MaterialRequestWizardModel wizardModel)
        {
            var actionModel = GetMaterialRequestActionModel(wizardModel.MaterialRequestId);
            var action = MaterialRequestAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, actionModel, wizardModel);

            if (!action.ArePreconditionsMet())
            {
                var response = new GenericResponse<UpsertMaterialRequestResultModel>();
                response.Errors.AddRange(action.ValidationErrors.Select(x => x.Message));
                response.Success = false;
                return response;
            }

            action.Perform();
            action.SaveChanges();
            var output = action.GetOutput();
            return output.UpsertResults;
        }

        public GenericResponse<UpsertMaterialRequestResultModel> UpsertMaterialRequestActionData(MaterialRequestActionModel model)
        {
            var request = _autoMapperHelper.Mapper.Map<MaterialRequestActionDto>(model);
            var response = _materialRequestQueryService.UpsertMaterialRequestActionData(request);

            var result = _autoMapperHelper.Mapper.Map<UpsertMaterialRequestResultModel>(response);
            return result;
        }

        public GenericResponse<IEnumerable<TestMaterialSearchModel>> SearchTestMaterials(string searchFilter, bool availableOnly)
        {
            var testMaterialId = 0;

            var filters = new List<TestMaterialSearchCriteria>();
            if (availableOnly)
            {
                filters.Add(new TestMaterialSearchCriteria()
                {
                    Filter = TestMaterialFilterType.AvailabilityBoolean,
                    Values = new[] { true.ToString() }
                });
            }

            var request = new TestMaterialSearchRequest() { Take = 5, Filter = filters };

            if (int.TryParse(searchFilter, out testMaterialId))
            {
                filters.Add(new TestMaterialSearchCriteria()
                {
                    Filter = TestMaterialFilterType.MaterialIdIntList,
                    Values = new[] { searchFilter }
                });
            }
            else if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                filters.Add(new TestMaterialSearchCriteria()
                {
                    Filter = TestMaterialFilterType.TitleString,
                    Values = new[] { searchFilter }
                });
            }

            var response = _testMaterialQueryService.SearchTestMaterials(request);

            var result = response.Results.Select(_autoMapperHelper.Mapper.Map<TestMaterialSearchModel>);
            return new GenericResponse<IEnumerable<TestMaterialSearchModel>>(result);
        }

        public GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> SearchTestMaterialRequests(SearchRequest request)
        {
            var filters = request.Filter.ToFilterList<TestMaterialRequestSearchCriteria, TestMaterialRequestFilterType>();
            var getRequest = _autoMapperHelper.Mapper.Map<TestMaterialRequestSearchRequest>(request);
            getRequest.Filters = filters;
            return Search(getRequest);
        }

        public GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetTestMaterialRelations(int materialId)
        {
            var testMaterialFilters = new List<TestMaterialRequestSearchCriteria>()
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.OutputTestMaterialIdIntList,
                    Values = new[] { materialId.ToString()}
                }
            };

            var sourceMaterialFilters = new List<TestMaterialRequestSearchCriteria>()
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.SourceTestMaterialIdIntList,
                    Values = new[] { materialId.ToString()}
                }
            };

            var testMaterialRequest = new TestMaterialRequestSearchRequest()
            {
                Filters = testMaterialFilters
            };

            var sourceMaterialRequest = new TestMaterialRequestSearchRequest()
            {
                Filters = sourceMaterialFilters
            };

            var testData = Search(testMaterialRequest).Data.ToList();
            var sourceData = Search(sourceMaterialRequest).Data.ToList();

            foreach (var t in testData)
            {
                t.RelationType = Naati.Resources.MaterialRequest.TestMaterial;
            }

            foreach (var s in sourceData)
            {
                s.RelationType = Naati.Resources.MaterialRequest.UsedAsSourceTestMaterial;
            }

            return new GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>>
            {
                Data = testData.Concat(sourceData)
            };
        }

        public GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetPanelMaterialRequests(int panelId)
        {
            var filters = new List<TestMaterialRequestSearchCriteria>()
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.PanelIntList,
                    Values = new[] { panelId.ToString()}
                },


                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.CreatedDateFromString,
                    Values = new[] { DateTime.Now.AddYears(-1).ToFilterString()}
                }
            };

            var request = new TestMaterialRequestSearchRequest()
            {
                Filters = filters
            };

            return Search(request);
        }

        public GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> GetActiveCoordinatorRequests(int coordinatorNaatiNumber)
        {
            var excludedStatuses = new[]
            {
                MaterialRequestStatusTypeName.Cancelled,
                MaterialRequestStatusTypeName.Finalised
            };

            var includedStatuses = Enum.GetValues(typeof(MaterialRequestStatusTypeName))
                .Cast<MaterialRequestStatusTypeName>()
                .Where(s => !excludedStatuses.Contains(s)).Select(x => ((int)x).ToString());

            var filters = new List<TestMaterialRequestSearchCriteria>()
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.CoordinatorNaatiNumberIntList,
                    Values = new[] { coordinatorNaatiNumber.ToString()}
                },

                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.TestMaterialRequestStatusIntList,
                    Values = includedStatuses
                }
            };

            var request = new TestMaterialRequestSearchRequest()
            {
                Filters = filters
            };

            return Search(request);
        }

        public GenericResponse<bool> HasActiveMaterialRequest(int outputTestMaterialId)
        {

            var filters = new List<TestMaterialRequestSearchCriteria>
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.OutputTestMaterialIdIntList,
                    Values = new []{ outputTestMaterialId.ToString() }
                },
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.TestMaterialRequestStatusIntList,
                    Values = new []
                    {
                        ((int)MaterialRequestStatusTypeName.InProgress).ToString(),
                        ((int)MaterialRequestStatusTypeName.AwaitingFinalisation).ToString()
                    }
                }
            };
            var request = new TestMaterialRequestSearchRequest
            {
                Filters = filters,
                Take = 1
            };

            return Search(request).Data.Any();
        }

        public GenericResponse<bool> IsUsedAsSourceMaterial(int testMaterialId)
        {

            var filters = new List<TestMaterialRequestSearchCriteria>
            {
                new TestMaterialRequestSearchCriteria()
                {
                    Filter = TestMaterialRequestFilterType.SourceTestMaterialIdIntList,
                    Values = new []{ testMaterialId.ToString() }
                }
            };
            var request = new TestMaterialRequestSearchRequest
            {
                Filters = filters,
                Take = 1
            };

            return Search(request).Data.Any();
        }


        public GenericResponse<bool> UnApproveRequests(IEnumerable<MaterialRequestMemberGroupingModel> items)
        {
            var materialRequestIds = items.SelectMany(x => x.Items.SelectMany(y => y.Claims.Where(c => c.Removed).Select(i => i.MaterialRequestId)
                    .Concat(y.Loadings.Where(l => l.Removed).Select(i => i.MaterialRequestId))))
                .Distinct();

            var response = new GenericResponse<bool>(true);

            foreach (var materialRequestId in materialRequestIds)
            {
                try
                {
                    var wizardModel = new MaterialRequestWizardModel()
                    {
                        ActionType = (int)SystemActionTypeName.UnApproveMaterialRequestPayment,
                        MaterialRequestId = materialRequestId
                    };

                    PerformAction(wizardModel);
                }
                catch (UserFriendlySamException ex)
                {
                    response.Messages.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    response.Messages.Add(string.Format(Naati.Resources.MaterialRequest.ErrorProcessingRequest, materialRequestId));
                }
            }

            return response;
        }
        public GenericResponse<bool> ApproveRequests(IEnumerable<MaterialRequestPayrollUserGroupingModel> items)
        {
            var approvedRequests = items.SelectMany(x => x.Items.SelectMany(y => y.Items)).Where(r => r.Approved);

            var response = new GenericResponse<bool>(true);

            foreach (var request in approvedRequests)
            {
                try
                {
                    var wizardModel = new MaterialRequestWizardModel()
                    {
                        ActionType = (int)SystemActionTypeName.ApproveMaterialRequestPayment,
                        MaterialRequestId = request.MaterialRequestId,
                        Approval = request
                    };

                    PerformAction(wizardModel);
                }
                catch (UserFriendlySamException ex)
                {
                    response.Messages.Add(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    response.Messages.Add(string.Format(Naati.Resources.MaterialRequest.ErrorProcessingRequest, request.MaterialRequestId));
                }
            }

            return response;
        }

        public GenericResponse<bool> PayMaterialRequestMembers(IEnumerable<MaterialRequestMemberGroupingModel> items)
        {
            var acceptedPayemnts = items.Where(x => !string.IsNullOrWhiteSpace(x.PaymentReference));
            var response = new GenericResponse<bool>(true);

            foreach (var payment in acceptedPayemnts)
            {
                try
                {
                    var materialRequestIds = payment.Items.SelectMany(i => i.Claims.Select(w => w.MaterialRequestId))
                        .Concat(payment.Items.SelectMany(i => i.Loadings.Select(w => w.MaterialRequestId)))
                        .Distinct();

                    var actions = new List<MaterialRequestAction>();
                    foreach (var materialRequestId in materialRequestIds)
                    {
                        var wizardModel = new MaterialRequestWizardModel()
                        {
                            ActionType = (int)SystemActionTypeName.MarkMaterialRequestMemberAsPaid,
                            MaterialRequestId = materialRequestId,
                            ExaminerPayment = payment
                        };
                        var actionModel = GetMaterialRequestActionModel(wizardModel.MaterialRequestId);
                        var action = MaterialRequestAction.CreateAction((SystemActionTypeName)wizardModel.ActionType, actionModel, wizardModel);
                        if (!action.ArePreconditionsMet())
                        {
                            response.Errors.AddRange(action.ValidationErrors.Select(x => x.Message));
                            actions.Clear();
                            break;
                        }

                        action.Perform();

                        actions.Add(action);
                    }

                    if (actions.Any())
                    {
                        var data = actions.Select(x => _autoMapperHelper.Mapper.Map<MaterialRequestActionDto>(x.ActionModel)).ToList();
                        _materialRequestQueryService.UpsertMaterialRequestsActionData(data);
                    }
                }
                catch (UserFriendlySamException ex)
                {
                    response.Messages.Add(string.Format(Naati.Resources.MaterialRequest.ErrorProcessingExaminerPayment, payment.DisplayName, ex.Message));
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex);
                    response.Messages.Add(string.Format(Naati.Resources.MaterialRequest.ErrorProcessingExaminerPayment, payment.DisplayName, string.Empty));
                }
            }

            return response;
        }

        private GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>> Search(TestMaterialRequestSearchRequest request)
        {
            TestMaterialRequestSearchResultResponse serviceReponse = null;
            serviceReponse = _materialRequestQueryService.SearchTestMaterialRequests(request);

            var models = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<TestMaterialRequestSearchResultModel>).ToList();
            var response = new GenericResponse<IEnumerable<TestMaterialRequestSearchResultModel>>(models);

            if (request.Take.HasValue && models.Count == request.Take.Value)
            {
                response.Warnings.Add($"Search result were limited to {request.Take.Value} records.");
            }

            if (request.Skip.HasValue)
            {
                response.Warnings.Add($"First {request.Skip.Value} records were skipped.");
            }

            return response;
        }

        public GenericResponse<MaterialRequestInfoModel> GetMaterialRequest(int materialRequestId)
        {
            var response = _materialRequestQueryService.GetMaterialRequest(materialRequestId);
            
            var result = _autoMapperHelper.Mapper.Map<MaterialRequestInfoModel>(response);

            return result;
        }

        public GenericResponse<object> GetNewRoundDetails(int materialRequestId, int actionId, int testComponentTypeId)
        {

            if (actionId == (int)SystemActionTypeName.UpdateMaterialRequestRound)
            {
                var latestRoundId = _materialRequestQueryService.GetRoundsLookup(materialRequestId).Results.OrderByDescending(x => x.Id).First().Id;

                var round = _materialRequestQueryService.GetMaterialRequestRound(latestRoundId).Data;
                return new
                {
                    Round = round.RoundNumber,
                    DueDate = round.DueDate,
                };

            }

            var testComponentType = _materialRequestQueryService.GetTestComponentType(testComponentTypeId);
            var dueDate = DateTime.Now.AddDays(testComponentType.DefaultMaterialRequestDueDays);
            if (actionId == (int)SystemActionTypeName.CloneMaterialRequest)
            {
                return new
                {
                    Round = 1,
                    DueDate = dueDate,
                };
            }

            var roundNumber = 1;
            if (materialRequestId > 0)
            {
                var maxValue = _materialRequestQueryService.GetRoundsLookup(materialRequestId)
                   .Results.Max(x => x.DisplayName);

                roundNumber = Convert.ToInt32(maxValue) + 1;
            }

            return new
            {
                Round = roundNumber,
                DueDate = dueDate,
            };
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetRoundLookup(int materialRequestId)
        {
            var response = _materialRequestQueryService.GetRoundsLookup(materialRequestId).Results.Select(y => new LookupTypeModel()
            {
                Id = y.Id,
                DisplayName = string.Format(Naati.Resources.MaterialRequest.RoundDisplayName, y.DisplayName)
            });

            return new GenericResponse<IEnumerable<LookupTypeModel>>(response);
        }

        public GenericResponse<TestComponentTypeModel> GetTestComponentType(int testComponenTypeId)
        {
            var responsse = _materialRequestQueryService.GetTestComponentType(testComponenTypeId);
            var result = _autoMapperHelper.Mapper.Map<TestComponentTypeModel>(responsse);
            return result;
        }

        public GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>> GetPendingItemsToApprove()
        {
            var result = _materialRequestPayRollHelper.GetPendingItemsToApprove();
            return new GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>>(result);
        }

        public GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>> GetPendingItemsToPay()
        {
            var result = _materialRequestPayRollHelper.GetPendingItemsToPay();
            return new GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>>(result);
        }

        public GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>> GetPendingItemsToPay(int materialRequestId)
        {
            var result = _materialRequestPayRollHelper.GetPendingItemsToPay(materialRequestId);
            return new GenericResponse<IEnumerable<MaterialRequestMemberGroupingModel>>(result);
        }

        public GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>> GetPendingItemsToApprove(int materialRequestId)
        {
            var result = _materialRequestPayRollHelper.GetPendingItemsToApprove(materialRequestId);
            return new GenericResponse<IEnumerable<MaterialRequestPayrollUserGroupingModel>>(result);
        }

        public GenericResponse<IEnumerable<LookupTypeDetailedModel>> GetAvailableDocumentTypes(int actionId, int testMaterialTypeId)
        {
            var availableCategories = new[]
            {   DocumentTypeCategoryTypeName.TestMaterialRequest,
                DocumentTypeCategoryTypeName.MaterialRequestSubmission,
                DocumentTypeCategoryTypeName.TestMaterial
            }.Where(x => CanAddDocumentCategory(x, actionId, testMaterialTypeId));


            var documentTypes = new List<DocumentTypeDto>();
            foreach (var category in availableCategories)
            {
                var types = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest() { Category = category }).Types;
                documentTypes.AddRange(types);

            }

            if (!documentTypes.Any())
            {
                var types = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest() { Category = DocumentTypeCategoryTypeName.General }).Types;
                documentTypes.AddRange(types);
            }


            var results = documentTypes.Select(x => new LookupTypeDetailedModel() { Id = x.Id, DisplayName = x.DisplayName, Name = x.Name });

            return new GenericResponse<IEnumerable<LookupTypeDetailedModel>>(results);
        }

        public GenericResponse<IEnumerable<LookupTypeDetailedModel>> GetAllAvailableDocumentTypes()
        {
            var availableCategories = new[]
            {   DocumentTypeCategoryTypeName.TestMaterialRequest,
                DocumentTypeCategoryTypeName.MaterialRequestSubmission,
                DocumentTypeCategoryTypeName.TestMaterial
            };

            var documentTypes = new List<DocumentTypeDto>();
            foreach (var category in availableCategories)
            {
                var types = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest() { Category = category }).Types;
                documentTypes.AddRange(types);

            }

            if (!documentTypes.Any())
            {
                var types = _fileStorageService.ListDocumentTypes(new ListDocumentTypesRequest() { Category = DocumentTypeCategoryTypeName.General }).Types;
                documentTypes.AddRange(types);
            }


            var results = documentTypes.Select(x => new LookupTypeDetailedModel() { Id = x.Id, DisplayName = x.DisplayName, Name = x.Name });

            return new GenericResponse<IEnumerable<LookupTypeDetailedModel>>(results);
        }

        private bool CanAddDocumentCategory(DocumentTypeCategoryTypeName category, int actionId, int testMaterialTypeId)
        {
            var action = (SystemActionTypeName)actionId;
            var materialType = (TestMaterialTypeName)testMaterialTypeId;
            switch (category)
            {
                case DocumentTypeCategoryTypeName.TestMaterialRequest:
                    ;
                    return new[]
                    {
                        SystemActionTypeName.CreateMaterialRequest,
                        SystemActionTypeName.CloneMaterialRequest,
                        SystemActionTypeName.CreateMaterialRequestRound
                    }.Any(x => x == action) || (action == SystemActionTypeName.UploadFinalMaterialDocuments && materialType == TestMaterialTypeName.Source);

                case DocumentTypeCategoryTypeName.MaterialRequestSubmission:
                    return (new[]
                           {
                               SystemActionTypeName.CreateMaterialRequest,
                               SystemActionTypeName.CloneMaterialRequest,
                               SystemActionTypeName.CreateMaterialRequestRound,
                           }.Any(x => x == action) && materialType == TestMaterialTypeName.Test) ||
                           new[] { SystemActionTypeName.SubmitRoundForApproval }.Any(x => x == action) ||
                           (new[] { SystemActionTypeName.UploadFinalMaterialDocuments }.Any(x => x == action) &&
                           materialType == TestMaterialTypeName.Source);

                case DocumentTypeCategoryTypeName.TestMaterial:
                    return new[]
                    {
                        SystemActionTypeName.CreateMaterialRequest,
                        SystemActionTypeName.CloneMaterialRequest,
                        SystemActionTypeName.CreateMaterialRequestRound,
                        SystemActionTypeName.UploadFinalMaterialDocuments
                    }.Any(x => x == action) && materialType == TestMaterialTypeName.Test;
            }

            return false;
        }

        public GenericResponse<IEnumerable<object>> GetExistingDocuments(int materialRequestId, int sourceTestMaterialId, int actionId, int testMaterialTypeId)
        {
            var validDocumentTypes = GetAvailableDocumentTypes(actionId, testMaterialTypeId).Data.Select(x => x.Id).ToList();
            var data = new List<object>();

            var roundDocuments = _materialRequestQueryService.GetExistingDocuments(materialRequestId);
            if (roundDocuments.Data.Any())
            {
                var maxRoundNumber = roundDocuments.Data.Max(x => x.RoundNumber);
                var lastRoundDocuments = roundDocuments.Data.Where(x => x.RoundNumber == maxRoundNumber && validDocumentTypes.Any(y => y == x.DocumentTypeId));

                var lastRoundDocumentsSection = new
                {
                    SectionName = string.Format(Naati.Resources.MaterialRequest.RoundSectionName, maxRoundNumber),
                    Documents = lastRoundDocuments.Select(w => new DocumentLookupTypeModel
                    {
                        Id = w.Id,
                        DisplayName = w.DisplayName,
                        DocumentTypeId = w.DocumentTypeId,
                        Size = w.Size,
                        FileType = w.FileType,
                        UploadedBy = w.UploadedBy,
                        ExaminersAvailable = w.ExaminersAvailable,
                        MergeDocument = w.MergeDocument
                    }).ToList()
                };

                if (lastRoundDocumentsSection.Documents.Any())
                {
                    data.Add(lastRoundDocumentsSection);
                }
            }

            var testMaterialDocuments = _testMaterialQueryService.GetTestMaterialDocuments(sourceTestMaterialId);
            var materialDocumentsSection = new
            {
                SectionName = string.Format(Naati.Resources.MaterialRequest.TestMaterialSectionName, sourceTestMaterialId),
                Documents = testMaterialDocuments.Data.Where(x => validDocumentTypes.Any(y => y == x.DocumentTypeId)).Select(w => new DocumentLookupTypeModel
                {
                    Id = w.Id,
                    DisplayName = w.DisplayName,
                    DocumentTypeId = w.DocumentTypeId,
                    Size = w.Size,
                    FileType = w.FileType,
                    UploadedBy = w.UploadedBy,
                    ExaminersAvailable = w.ExaminersAvailable,
                    MergeDocument = w.MergeDocument

                }).ToList()
            };
            if (materialDocumentsSection.Documents.Any())
            {
                data.Add(materialDocumentsSection);
            }
            var response = new GenericResponse<IEnumerable<object>>(data);
            return response;
        }

        public GenericResponse<bool> CanShowTotalCost(int materialRequestId)
        {

            var request = new TestMaterialRequestSearchRequest
            {
                Filters = new[] {
                    new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.MaterialRequestIdIntList,
                    Values = new []{ materialRequestId.ToString() }

                },
                    new TestMaterialRequestSearchCriteria
                    {
                        Filter = TestMaterialRequestFilterType.RoundNumberIntList,
                        Values = new []{ 1.ToString() }

                    },
                    new TestMaterialRequestSearchCriteria
                    {
                        Filter = TestMaterialRequestFilterType.RoundStatusIntList,
                        Values = new []{ ((int) MaterialRequestRoundStatusTypeName.SentForDevelopment).ToString() }

                    }
                }
            };

            var result = !_materialRequestQueryService.SearchTestMaterialRequests(request).Results.Any();

            return result;
        }

        public GenericResponse<bool> IsMaterialDomainRequired(int materialRequestId)
        {
            var request = new TestMaterialRequestSearchRequest
            {
                Filters = new[] {
                    new TestMaterialRequestSearchCriteria
                    {
                        Filter = TestMaterialRequestFilterType.MaterialRequestIdIntList,
                        Values = new []{ materialRequestId.ToString() }
                    },
                    new TestMaterialRequestSearchCriteria
                    {
                        Filter = TestMaterialRequestFilterType.RoundStatusIntList,
                        Values = new []
                        {
                            ((int) MaterialRequestRoundStatusTypeName.AwaitingApproval).ToString(),
                            ((int) MaterialRequestRoundStatusTypeName.Approved).ToString()
                        }
                    },
                    new TestMaterialRequestSearchCriteria
                    {
                        Filter = TestMaterialRequestFilterType.TestMaterialRequestStatusIntList,
                        Values = new []
                        {
                            ((int) MaterialRequestStatusTypeName.InProgress).ToString(),
                            ((int) MaterialRequestStatusTypeName.AwaitingFinalisation).ToString()
                        }
                    }
                }       
            };

            var result = _materialRequestQueryService.SearchTestMaterialRequests(request).Results.Any();

            return new GenericResponse<bool>(result);
        }

        public IEnumerable<MaterialRequestRoundAttachmentModel> ListAttachments(ListAttachmentsRequestModel request)
        {
            var queryRequest = new GetMaterialRequestRoundAttachmentsRequest
            {
                MaterialRequestRoundId = request.MaterialRequestRoundId,
                NcmsAvailable = request.NcmsAvailable,
                ExaminerAvailable = request.ExaminersAvailable
            };

            GetMaterialRequestAttachmentsResponse response = null;

            try
            {
                response = _materialRequestQueryService.GetAttachments(queryRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response?.Attachments.Select((Func<MaterialRequestRoundAttachmentDto, MaterialRequestRoundAttachmentModel>)(a =>
            {
                var model = _autoMapperHelper.Mapper.Map<MaterialRequestRoundAttachmentModel>(a);
                model.FileType = Path.GetExtension(a.FileName)?.Trim('.');
                model.Title = a.Description;
                return model;
            })).ToArray() ?? new MaterialRequestRoundAttachmentModel[0];
        }

        public int CreateOrReplaceAttachment(MaterialRequestRoundAttachmentModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<CreateOrReplaceMaterialRequestRoundAttachmentRequest>(request);

            CreateOrReplaceMaterialRequestRoundAttachmentResponse response = null;

            try
            {
                response = _materialRequestQueryService.CreateOrReplaceAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            return response.StoredFileId;
        }

        public void DeleteAttachment(int materialRequestRoundAttachmentId)
        {
            var serviceRequest = new DeleteMaterialRequestRoundAttachmentRequest
            {
                MaterialRequestRoundAttachmentId = materialRequestRoundAttachmentId
            };

            DeleteAttachmentResponse response = null;

            try
            {
                response = _materialRequestQueryService.DeleteAttachment(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<MaterialRequestRoundModel> GetMaterialRequestRound(int materialRequestRoundId)
        {
            var response = _materialRequestQueryService.GetMaterialRequestRound(materialRequestRoundId);

            var result = _autoMapperHelper.Mapper.Map<MaterialRequestRoundModel>(response.Data);

            var links = _materialRequestQueryService.GetMaterialRequestRoundLink(new GetMaterialRequestRoundLinkRequest
            {
                MaterialRequestRoundId = materialRequestRoundId
            });

            result.Links = links.Results.Select(l => _autoMapperHelper.Mapper.Map<MaterialRequestRoundLinkModel>(l)).ToList();
            return result;
        }

        public GenericResponse<IEnumerable<SkillLookupTypeModel>> GetSkillsForCredentialType(int credentialTypeId, int panelId)
        {
            GetSkillsForCredentialTypeResponse serviceReponse = null;
            serviceReponse = _materialRequestQueryService.GetMaterialRequestSkills(new MaterialRequestSkillRequest() { CredentialTypeId = credentialTypeId, PanelId = panelId });
            var data = serviceReponse.Results.Select(_autoMapperHelper.Mapper.Map<SkillLookupTypeModel>).ToList();
            return data;
        }

        public GenericResponse<IEnumerable<LookupTypeModel>> GetTestMaterialDomains(int credentialTypeId)
        {
            var response = _testMaterialQueryService.GetTestMaterialDomains(credentialTypeId).Results.Select(_autoMapperHelper.Mapper.Map<LookupTypeModel>).ToList();

            var results = new GenericResponse<IEnumerable<LookupTypeModel>>()
            {
                Data = response
            };

            return results;
        }

        public GenericResponse<int> AddLink(int materialRequestRoundId, MaterialRequestRoundLinkModel link)
        {
            var request = _autoMapperHelper.Mapper.Map<SaveMaterialRequestRoundLinkRequest>(link);
            request.MaterialRequestRoundId = materialRequestRoundId;
            request.NcmsAvailable = true;
            var response = _materialRequestQueryService.SaveMaterialRequestRoundLink(request);
            return response.MaterialRequestRoundLinkId;
        }

        public GenericResponse<bool> DeleteLink(int materialRequestRoundLinkId)
        {
            _materialRequestQueryService.DeleteMaterialRequestLink(new DeleteMaterialRequestLinkRequest
            {
                MaterialRequestRoundLinkId = materialRequestRoundLinkId
            });
            return true;
        }
    }
}
