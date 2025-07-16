using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal
{
   
    public class MaterialRequestQueryService : IMaterialRequestQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public MaterialRequestQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        private void UpsertMaterialRequestActionData(MaterialRequestActionDto actionData, List<object> itemsToUpsert, List<object> itemsToDelete)
        {
            var materialRequestData = GetMaterialRequestToUpsert(actionData.MaterialRequestInfo, actionData.Documents);

            itemsToUpsert.AddRange(materialRequestData.itemToUpsert);
            itemsToDelete.AddRange(materialRequestData.itemsToRemove);

            var personNotesData = GetPersonNotesToUpsert(actionData.PersonNotes);
            itemsToUpsert.AddRange(personNotesData.itemToUpsert);
            itemsToDelete.AddRange(personNotesData.itemsToRemove);

            var materialRequestNotes = GetMaterialRequestNotesToUpsert(materialRequestData.materialRequest, actionData.Notes);
            itemsToUpsert.AddRange(materialRequestNotes.itemToUpsert);
            itemsToDelete.AddRange(materialRequestNotes.itemsToRemove);

            var materialRequestPublicNotes = GetMaterialRequestPublicNotesToUpsert(materialRequestData.materialRequest, actionData.PublicNotes);
            itemsToUpsert.AddRange(materialRequestPublicNotes.itemToUpsert);
            itemsToDelete.AddRange(materialRequestPublicNotes.itemsToRemove);

            foreach (var round in actionData.Rounds)
            {
                var data = GetItemsToUpsert(materialRequestData.materialRequest, round);
                itemsToUpsert.AddRange(data.itemToUpsert);
                itemsToDelete.AddRange(data.itemsToRemove);
            }
        }

        public UpsertMaterialRequestResultResponse UpsertMaterialRequestActionData(MaterialRequestActionDto actionData)
        {
            return UpsertMaterialRequestsActionData(new[] { actionData });
        }

        public UpsertMaterialRequestResultResponse UpsertMaterialRequestsActionData(IList<MaterialRequestActionDto> actions)
        {
            var itemsToUpsert = new List<object>();
            var itemsToDelete = new List<object>();

            try
            {
                foreach (var action in actions)
                {
                    UpsertMaterialRequestActionData(action, itemsToUpsert, itemsToDelete);
                }

                using (var transaction = NHibernateSession.Current.BeginTransaction())
                {
                    itemsToDelete.Reverse();
                    foreach (var item in itemsToDelete)
                    {
                        NHibernateSession.Current.Delete(item);
                    }

                    foreach (var item in itemsToUpsert)
                    {
                        NHibernateSession.Current.SaveOrUpdate(item);
                    }
                    transaction.Commit();
                }

                var roundIds = itemsToUpsert.OfType<MaterialRequestRound>().Select(x => x.Id).ToList();


                var preExistingMaterialDocuments = actions.SelectMany(a => a.Documents.Where(x => x.StoredFileId > 0))
                    .Select(y => y.StoredFileId);

                var preExistingRoundDocuments = actions.SelectMany(a => a.Rounds.SelectMany(x => x.Documents))
                    .Where(x => x.StoredFileId > 0)
                    .Select(y => y.StoredFileId);
                var fileService = new FileSystemFileStorageService(_autoMapperHelper);
                var preExistingDocuments = preExistingMaterialDocuments.Concat(preExistingRoundDocuments).ToList();
                var testMaterialAttachments = itemsToUpsert.OfType<TestMaterialAttachment>();

                var storeFileIds = new List<int>();
                var moveFileRequest = new MoveFileRequest();

                foreach (var attachment in testMaterialAttachments)
                {
                    if (preExistingDocuments.All(x => x != attachment.StoredFile.Id))
                    {
                        storeFileIds.Add(attachment.StoredFile.Id);
                        moveFileRequest.StoredFileId = attachment.StoredFile.Id;
                        moveFileRequest.StotoragePath =
                            $"{(StoredFileType)attachment.StoredFile.DocumentType.Id}\\{attachment.TestMaterial.Id}\\{attachment.StoredFile.FileName}";
                        fileService.MoveFile(moveFileRequest);
                    }
                }

                var roudnAttachments = itemsToUpsert.OfType<MaterialRequestRoundAttachment>();
                foreach (var attachment in roudnAttachments)
                {
                    if (preExistingDocuments.All(x => x != attachment.StoredFile.Id))
                    {
                        storeFileIds.Add(attachment.StoredFile.Id);
                        moveFileRequest.StoredFileId = attachment.StoredFile.Id;
                        moveFileRequest.StotoragePath =
                            $"{(StoredFileType)attachment.StoredFile.DocumentType.Id}\\{attachment.MaterialRequestRound.Id}\\{attachment.StoredFile.FileName}";
                        fileService.MoveFile(moveFileRequest);
                    }
                }

                var response = new UpsertMaterialRequestResultResponse
                {
                    MaterialRequestIds = itemsToUpsert.OfType<Domain.MaterialRequest>().Select(y => y.Id),
                    MaterialRequestRoundIds = roundIds,
                    TestMaterialIds = itemsToUpsert.OfType<TestMaterial>().Select(y => y.Id),
                    StoredFileIds = storeFileIds
                };

                return response;
            }
            catch
            {
                var fileService = new FileSystemFileStorageService(_autoMapperHelper);

                var preExistingMaterialDocuments = actions.SelectMany(a => a.Documents.Where(x => x.StoredFileId > 0))
                    .Select(y => y.StoredFileId);

                var preExistingRoundDocuments = actions.SelectMany(a => a.Rounds).SelectMany(x => x.Documents)
                    .Where(x => x.StoredFileId > 0)
                    .Select(y => y.StoredFileId);

                var preExistingDocuments = preExistingMaterialDocuments.Concat(preExistingRoundDocuments).ToList();

                var materialAttachments = itemsToUpsert.OfType<TestMaterialAttachment>();
                foreach (var attachment in materialAttachments)
                {
                    if (attachment.StoredFile?.Id > 0 && preExistingDocuments.All(x => x != attachment.StoredFile.Id))
                    {
                        fileService.DeleteFile(new DeleteFileRequest() { StoredFileId = attachment.StoredFile.Id });
                    }
                }
                var materialRequestAttachments = itemsToUpsert.OfType<MaterialRequestRoundAttachment>();
                foreach (var attachment in materialRequestAttachments)
                {
                    if (attachment.StoredFile?.Id > 0 && preExistingDocuments.All(x => x != attachment.StoredFile.Id))
                    {
                        fileService.DeleteFile(new DeleteFileRequest() { StoredFileId = attachment.StoredFile.Id });
                    }
                }
                throw;
            }
        }

        public MaterialRequestActionDto GetMaterialRequestActionData(int materialRequestId)
        {
            var materialRequestData = new MaterialRequestActionDto
            {
                Notes = new List<MaterialRequestActionNoteDto>(),
                PublicNotes = new List<MaterialRequestActionPublicNoteDto>(),
                PersonNotes = new List<MaterialRequestPersonNoteDto>(),
                Documents = new List<OutputTestMaterialDocumentInfoDto>()
            };

            materialRequestData.MaterialRequestInfo = GetMaterialRequestInfo(materialRequestId);
            materialRequestData.Rounds = GetMaterialRounds(materialRequestId);

            return materialRequestData;
        }

        public MaterialRequestInfoDto GetMaterialRequest(int materialRequestId)
        {
            return GetMaterialRequestInfo(materialRequestId);
        }

        public GetMaterialRequestPayrollResponse GetPendingToApproveMaterialRequestPayRolls(MaterialRequestPayRollRequest request)
        {
            var filters = new List<TestMaterialRequestSearchCriteria>
            {
                new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.HasMembersToApprove,
                    Values = new[] {true.ToString()}
                }
            };

            if (request.MaterialRequetsId.HasValue)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.MaterialRequestIdIntList,
                    Values = new[] { request.MaterialRequetsId.Value.ToString() }
                });
            }
            var searchRequest = new TestMaterialRequestSearchRequest()
            {
                Filters = filters,
                Take = request.Take
            };

            var results = SearchTestMaterialRequests(searchRequest).Results.Select(x => x.Id).ToList();

            var materialRequest = NHibernateSession.Current.Query<Domain.MaterialRequest>().Where(x => results.Contains(x.Id)).ToList();

            var dtos = new List<MaterialRequestPayRollInfoDto>();
            foreach (var mr in materialRequest)
            {
                var dto = MapMaterialRequestPayRollInfoDto(mr);
                dtos.Add(dto);
            }

            return new GetMaterialRequestPayrollResponse() { Results = dtos };
        }

        public GetMaterialRequestPayrollResponse GetPendingToPayMaterialRequestPayRolls(MaterialRequestPayRollRequest request)
        {
            var filters = new List<TestMaterialRequestSearchCriteria>
            {
                new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.HasMembersToPay,
                    Values = new[] {true.ToString()}
                }
            };

            if (request.MaterialRequetsId.HasValue)
            {
                filters.Add(new TestMaterialRequestSearchCriteria
                {
                    Filter = TestMaterialRequestFilterType.MaterialRequestIdIntList,
                    Values = new[] { request.MaterialRequetsId.Value.ToString() }
                });
            }
            var searchRequest = new TestMaterialRequestSearchRequest()
            {
                Filters = filters,
                Take = request.Take
            };

            var results = SearchTestMaterialRequests(searchRequest).Results.Select(x => x.Id).ToList();

            var materialRequests = NHibernateSession.Current.Query<Domain.MaterialRequest>()
                .Where(x => results.Contains(x.Id));

            var dtos = new List<MaterialRequestPayRollInfoDto>();
            foreach (var materialRequest in materialRequests)
            {
                var dto = MapMaterialRequestPayRollInfoDto(materialRequest);
                dtos.Add(dto);
            }

            return new GetMaterialRequestPayrollResponse() { Results = dtos };
        }

        private MaterialRequestPayRollInfoDto MapMaterialRequestPayRollInfoDto(MaterialRequest materialRequest)
        {
            var dto = new MaterialRequestPayRollInfoDto
            {
                MaterialRequestId = materialRequest.Id,
                CredentialTypeId = materialRequest.OutputMaterial.TestComponentType.TestSpecification.CredentialType.Id,
                ProductSpecificationId = materialRequest.ProductSpecification.Id,
                CostPerUnit = materialRequest.ProductSpecification.CostPerUnit,
                GlCode = materialRequest.ProductSpecification.GLCode.Code,
                SpecificationCode = materialRequest.ProductSpecification.Code,
                OwnerUserId = materialRequest.OwnedByUser?.Id ?? materialRequest.StatusChangeUser.Id,
                Members = GetMaterialRequestMembers(materialRequest),
                ModifiedDate = materialRequest.StatusChangeDate
            };

            return dto;
        }

        private IList<MaterialRequestRoundDto> GetMaterialRounds(int materialRequestId)
        {
            var rounds = NHibernateSession.Current.Query<MaterialRequestRound>()
                .Where(x => x.MaterialRequest.Id == materialRequestId)
                .Select(
                    y => new MaterialRequestRoundDto()
                    {
                        MaterialRoundId = y.Id,
                        RoundNumber = y.RoundNumber,
                        StatusTypeId = y.MaterialRequestRoundStatusType.Id,
                        StatusTypeDisplayName = y.MaterialRequestRoundStatusType.DisplayName,
                        SubmittedDate = y.SubmittedDate,
                        RequestedDate = y.RequestedDate,
                        StatusChangeDate = y.StatusChangeDate,
                        ModifiedUserId = y.ModifiedUser.Id,

                        DueDate = y.DueDate
                    }).ToList();

            foreach (var round in rounds)
            {
                round.Documents = new List<MaterialRequestDocumentInfoDto>();
                round.Links = NHibernateSession.Current.Query<MaterialRequestRoundLink>()
                    .Where(x => x.MaterialRequestRound.Id == round.MaterialRoundId)
                    .Select(y => new MaterialRequestRoundLinkDto() { Id = y.Id, Link = y.Link, NcmsAvailable = y.NcmsAvailable, UserId = y.User.Id, PersonId = y.Person.Id }).ToList();
            }

            return rounds;
        }


        public ServiceResponse<MaterialRequestRoundDto> GetMaterialRequestRound(int materialRequestRoundId)
        {
            var round = NHibernateSession.Current.Get<MaterialRequestRound>(materialRequestRoundId);
            var result = new MaterialRequestRoundDto()
            {
                MaterialRoundId = round.Id,
                RoundNumber = round.RoundNumber,
                StatusTypeId = round.MaterialRequestRoundStatusType.Id,
                SubmittedDate = round.SubmittedDate,
                RequestedDate = round.RequestedDate,
                StatusChangeDate = round.StatusChangeDate,
                ModifiedUserId = round.ModifiedUser.Id,
                DueDate = round.DueDate
            };


            result.Documents = new List<MaterialRequestDocumentInfoDto>();
            result.Links = new List<MaterialRequestRoundLinkDto>();
            return new ServiceResponse<MaterialRequestRoundDto>() { Data = result };
        }


        private IEnumerable<MaterialRequestPanelMembershipDto> GetMaterialRequestMembers(MaterialRequest materialRequest)
        {
            var members = materialRequest.MaterialRequestRoundPanelMemberships
                .Select(y =>
                    {
                        var email = y.PanelMembership.Person.Entity.Emails
                            .FirstOrDefault(x => !x.Invalid && x.IsPreferredEmail)
                            ?.EmailAddress;

                        var dto = new MaterialRequestPanelMembershipDto
                        {
                            Id = y.Id,
                            GivenName = y.PanelMembership.Person.GivenName + " " + y.PanelMembership.Person.Surname,
                            Tasks = GetMaterialRequestTasks(y),
                            MemberTypeId = y.MaterialRequestPanelMembershipType.Id,
                            NaatiNumber = y.PanelMembership.Person.Entity.NaatiNumber,
                            PanelMemberShipId = y.PanelMembership.Id,
                            PrimaryEmail = email,
                            EntityId = y.PanelMembership.Person.Entity.Id,
                            PayRoll = GetMemberPayroll(y)
                        };

                        return dto;
                    }
                );

            return members;
        }


        private MaterialRequestPayrollDto GetMemberPayroll(MaterialRequestPanelMembership member)
        {
            var result = NHibernateSession.Current.Query<MaterialRequestPayroll>()
                .FirstOrDefault(x => x.MaterialRequestPanelMembership.Id == member.Id);

            if (result != null)
            {
                return new MaterialRequestPayrollDto
                {
                    Id = result.Id,
                    Amount = result.Amount,
                    ApprovedByUserId = result.ApprovedByUser?.Id,
                    ApprovedDate = result.ApprovedDate,
                    PaidByUserId = result.PaidByUser?.Id,
                    PaymentDate = result.PaymentDate,
                    PaymentReference = result.PaymentReference,
                    RoundPanelMemberId = result.MaterialRequestPanelMembership.Id
                };
            }

            return null;
        }

        private IList<MaterialRequestTaskDto> GetMaterialRequestTasks(MaterialRequestPanelMembership member)
        {
            var tasks = member.MaterialRequestPanelMembershipTasks
                .Select(y => new MaterialRequestTaskDto()
                {
                    Id = y.Id,
                    HoursSpent = y.HoursSpent,
                    MaterialRequestTaskTypeId = y.MaterialRequestTaskType.Id,
                    MaterialRequestTaskTypeDisplayName = y.MaterialRequestTaskType.DisplayName
                })
                .ToList();

            return tasks;
        }
        private MaterialRequestInfoDto GetMaterialRequestInfo(int materialRequestId)
        {
            var materialRequest = NHibernateSession.Current.Load<Domain.MaterialRequest>(materialRequestId);
            var materialRequestDto = MapMaterialRequest(materialRequest);
            materialRequestDto.Members = GetMaterialRequestMembers(materialRequest);
            return materialRequestDto;
        }
        private MaterialRequestInfoDto MapMaterialRequest(Domain.MaterialRequest materialRequest)
        {
            var rounds = GetMaterialRounds(materialRequest.Id);
            var lastRound = rounds.OrderByDescending(r => r.RoundNumber).FirstOrDefault();

            return new MaterialRequestInfoDto
            {
                MaterialRequestId = materialRequest.Id,
                CreatedByUserId = materialRequest.CreatedByUser.Id,
                OutputTestMaterial = new TestMaterialQueryService(_autoMapperHelper).GetTestMaterials(materialRequest.OutputMaterial.Id),
                SourceTestMaterial = materialRequest.SourceMaterial != null ? new TestMaterialQueryService(_autoMapperHelper).GetTestMaterials(materialRequest.SourceMaterial.Id) : null,
                PanelId = materialRequest.Panel.Id,
                RequestTitle = materialRequest.OutputMaterial.Title,
                StatusChangeUserId = materialRequest.StatusChangeUser.Id,
                StatusTypeId = materialRequest.MaterialRequestStatusType.Id,
                RequestTypeDisplayName = materialRequest.OutputMaterial.TestMaterialType.DisplayName,
                StatusTypeDisplayName = materialRequest.MaterialRequestStatusType.DisplayName,
                StatusChangeDate = materialRequest.StatusChangeDate,
                OwnedByUserEmail = materialRequest.OwnedByUser?.Email,
                CreatedDate = materialRequest.CreatedDate,
                EnteredOffice = materialRequest.CreatedByUser.Office.Institution.LatestInstitutionName.InstitutionName.Name,
                OwnedByUserId = materialRequest.OwnedByUser?.Id,
                ProductSpecificationId = materialRequest.ProductSpecification.Id,
                ProductSpecificationCostPerUnit = materialRequest.ProductSpecification.CostPerUnit,
                MaxBillableHours = materialRequest.MaxBillableHours,
                TestMaterialDomainId = materialRequest.OutputMaterial.TestMaterialDomain.Id,
                LastRound = lastRound
            };
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetItemsToUpsert(Domain.MaterialRequest materialRequest, MaterialRequestRoundDto roundData)
        {
            var requestRound = NHibernateSession.Current.Get<MaterialRequestRound>(roundData.MaterialRoundId) ?? new MaterialRequestRound
            {
                MaterialRequest = materialRequest,
                RoundNumber = roundData.RoundNumber,
                RequestedDate = roundData.RequestedDate,
            };

            requestRound.DueDate = roundData.DueDate;
            requestRound.StatusChangeDate = roundData.StatusChangeDate;
            requestRound.MaterialRequestRoundStatusType = NHibernateSession.Current.Load<MaterialRequestRoundStatusType>(roundData.StatusTypeId);
            requestRound.SubmittedDate = roundData.SubmittedDate;
            requestRound.ModifiedUser = NHibernateSession.Current.Load<User>(roundData.ModifiedUserId);


            var documents = UpsertRoundDocuments(requestRound, roundData.Documents);
            var links = GetRoundLinksToUpsert(requestRound, roundData.Links);

            return (documents.itemsToRemove.Concat(links.itemsToRemove).ToList(), new object[] { requestRound }.Concat(documents.itemToUpsert).Concat(links.itemToUpsert).ToArray());

        }
        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetRoundLinksToUpsert(MaterialRequestRound round, IList<MaterialRequestRoundLinkDto> links)
        {
            var dataToUpsert = new List<object>();
            var linkIds = links.Select(m => m.Id).ToList();

            var existingLinks = NHibernateSession.Current.Query<MaterialRequestRoundLink>()
                .Where(x => x.MaterialRequestRound.Id == round.Id).ToList();

            var deletedLinks = existingLinks.Where(x => linkIds.All(y => y != x.Id)).ToList<object>();


            foreach (var link in links)
            {
                var roundLinksToUpsert = existingLinks.FirstOrDefault(x => x.Id == link.Id);
                roundLinksToUpsert = GetRoundLinkToUpsert(round, roundLinksToUpsert, link);
                dataToUpsert.Add(roundLinksToUpsert);
            }

            return (deletedLinks, dataToUpsert);
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetItemsToUpsert(Domain.MaterialRequest materialRequest, IEnumerable<MaterialRequestPanelMembershipDto> members)
        {
            var dataToUpsert = new List<object>();
            var dataToRemove = new List<object>();
            var memberIds = members.Select(m => m.Id).ToList();

            var existingMembers = NHibernateSession.Current.Query<MaterialRequestPanelMembership>()
                .Where(x => x.MaterialRequest.Id == materialRequest.Id).ToList();

            var deletedMembers = existingMembers.Where(x => memberIds.All(y => y != x.Id)).ToList();
            dataToRemove.AddRange(deletedMembers);

            var deletedMembersIds = deletedMembers.Select(x => x.Id).Concat(new[] { 0 }).ToList();

            var taskToDelete = NHibernateSession.Current.Query<MaterialRequestPanelMembershipTask>()
                .Where(x => deletedMembersIds.Contains(x.MaterialRequestPanelMembership.Id)).ToList();

            dataToRemove.AddRange(taskToDelete);


            foreach (var member in members)
            {
                var memberToUpsert = existingMembers.FirstOrDefault(x => x.Id == member.Id);
                var memberDataToUpsert = GetMaterialRequestPanelMembershipToUpsert(materialRequest, memberToUpsert, member);
                dataToUpsert.AddRange(memberDataToUpsert.itemToUpsert);
                dataToRemove.AddRange(memberDataToUpsert.itemsToRemove);
            }

            return (dataToRemove, dataToUpsert);
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetMaterialLinksToUpsert(TestMaterial testMaterial, IList<TestMaterialLinkDto> links)
        {
            var dataToUpsert = new List<object>();
            var linkIds = links.Select(m => m.Id).ToList();

            var existingLinks = NHibernateSession.Current.Query<TestMaterialLink>()
                .Where(x => x.FromTestMaterial.Id == testMaterial.Id).ToList();

            var deletedLinks = existingLinks.Where(x => linkIds.All(y => y != x.Id)).ToList<object>();


            foreach (var link in links)
            {
                var materialLinkToUpsert = existingLinks.FirstOrDefault(x => x.Id == link.Id);
                materialLinkToUpsert = GetMaterialLinkToUpsert(testMaterial, materialLinkToUpsert, link);
                dataToUpsert.Add(materialLinkToUpsert);
            }

            return (deletedLinks, dataToUpsert);
        }


        private TestMaterialLink GetMaterialLinkToUpsert(TestMaterial testMaterial,
            TestMaterialLink existingLink,
            TestMaterialLinkDto linkData)
        {
            var link = existingLink ?? new TestMaterialLink()
            {
                FromTestMaterial = testMaterial
            };

            link.ToTestMaterial = NHibernateSession.Current.Load<TestMaterial>(linkData.ToTestMaterialId);
            link.TestMaterialLinkType = NHibernateSession.Current.Load<TestMaterialLinkType>(linkData.TypeId);
            return link;
        }

        private MaterialRequestRoundLink GetRoundLinkToUpsert(MaterialRequestRound round,
            MaterialRequestRoundLink existingLink,
            MaterialRequestRoundLinkDto linkData)
        {
            var link = existingLink ?? new MaterialRequestRoundLink()
            {
                MaterialRequestRound = round
            };

            link.User = NHibernateSession.Current.Get<User>(linkData.UserId.GetValueOrDefault());
            link.Person = NHibernateSession.Current.Get<Person>(linkData.PersonId.GetValueOrDefault());
            link.Link = linkData.Link;
            link.NcmsAvailable = linkData.NcmsAvailable;
            return link;
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetMaterialRequestPanelMembershipToUpsert(Domain.MaterialRequest materialRequest,
            MaterialRequestPanelMembership exitingMember,
            MaterialRequestPanelMembershipDto memberData)
        {
            var itemsToUpsert = new List<object>();
            var itemsToDelete = new List<object>();

            var selectedTaskIds = memberData.Tasks.Select(x => x.Id).ToList();
            var member = exitingMember ?? new MaterialRequestPanelMembership()
            {
                MaterialRequest = materialRequest,
            };

            member.PanelMembership = NHibernateSession.Current.Get<PanelMembership>(memberData.PanelMemberShipId);
            member.MaterialRequestPanelMembershipType = NHibernateSession.Current.Get<MaterialRequestPanelMembershipType>(memberData.MemberTypeId);

            var existingTasks = NHibernateSession.Current.Query<MaterialRequestPanelMembershipTask>()
                .Where(x => x.MaterialRequestPanelMembership.Id == member.Id).ToList();

            var deletedTasks = existingTasks.Where(x => selectedTaskIds.All(y => y != x.Id)).ToList<object>();
            itemsToDelete.AddRange(deletedTasks);

            itemsToUpsert.Add(member);

            foreach (var memberTask in memberData.Tasks)
            {
                var existingTask = existingTasks.FirstOrDefault(x => x.Id == memberTask.Id);
                existingTask = GetMaterialRequestTaskToUpsert(member, existingTask, memberTask);
                itemsToUpsert.Add(existingTask);
            }

            var payrollData = GetPayRollToUpsert(member, memberData.PayRoll);
            itemsToDelete.AddRange(payrollData.itemsToRemove);
            itemsToUpsert.AddRange(payrollData.itemToUpsert);
            return (itemsToDelete, itemsToUpsert);
        }


        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetPayRollToUpsert(MaterialRequestPanelMembership exitingMember, MaterialRequestPayrollDto payrollDto)
        {
            var itemsToUpsert = new List<object>();
            var itemsToDelete = new List<object>();

            MaterialRequestPayroll materialRequestPayroll = null;
            if (payrollDto != null)
            {
                materialRequestPayroll = NHibernateSession.Current.Get<MaterialRequestPayroll>(payrollDto.Id) ?? new MaterialRequestPayroll();
                materialRequestPayroll.MaterialRequestPanelMembership = exitingMember;
            }

            if (materialRequestPayroll == null)
            {
                return (itemsToDelete, itemsToUpsert);
            }

            itemsToUpsert.Add(materialRequestPayroll);

            materialRequestPayroll.Amount = payrollDto.Amount;
            materialRequestPayroll.ApprovedByUser = NHibernateSession.Current.Get<User>(payrollDto.ApprovedByUserId.GetValueOrDefault());
            materialRequestPayroll.PaidByUser = NHibernateSession.Current.Get<User>(payrollDto.PaidByUserId.GetValueOrDefault());
            materialRequestPayroll.ApprovedDate = payrollDto.ApprovedDate;
            materialRequestPayroll.PaymentDate = payrollDto.PaymentDate;
            materialRequestPayroll.PaymentReference = payrollDto.PaymentReference;

            return (itemsToDelete, itemsToUpsert);
        }

        private MaterialRequestPanelMembershipTask GetMaterialRequestTaskToUpsert(
            MaterialRequestPanelMembership member,
            MaterialRequestPanelMembershipTask existingTask,
            MaterialRequestTaskDto taskData)
        {
            var task = existingTask ?? new MaterialRequestPanelMembershipTask()
            {
                MaterialRequestTaskType =
                    NHibernateSession.Current.Get<MaterialRequestTaskType>(taskData.MaterialRequestTaskTypeId),
                MaterialRequestPanelMembership = member,
            };

            task.HoursSpent = taskData.HoursSpent;

            return task;
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert, Domain.MaterialRequest materialRequest) GetMaterialRequestToUpsert(MaterialRequestInfoDto data, IList<OutputTestMaterialDocumentInfoDto> documents)
        {
            var materialRequest = NHibernateSession.Current.Get<Domain.MaterialRequest>(data.MaterialRequestId);
            if (materialRequest == null)
            {
                materialRequest = new Domain.MaterialRequest
                {
                    CreatedDate = data.CreatedDate,
                    CreatedByUser = NHibernateSession.Current.Load<User>(data.CreatedByUserId),
                    Panel = NHibernateSession.Current.Load<Panel>(data.PanelId),
                };

            }
            materialRequest.OutputMaterial = GetTestMaterialToUpsert(data.OutputTestMaterial);
            materialRequest.SourceMaterial = data.SourceTestMaterial != null ? NHibernateSession.Current.Load<TestMaterial>(data.SourceTestMaterial.Id)
                : null;

            materialRequest.OwnedByUser = NHibernateSession.Current.Get<User>(data.OwnedByUserId.GetValueOrDefault());
            materialRequest.ProductSpecification = NHibernateSession.Current.Load<ProductSpecification>(data.ProductSpecificationId);
            materialRequest.StatusChangeUser = NHibernateSession.Current.Load<User>(data.StatusChangeUserId);
            materialRequest.StatusChangeDate = data.StatusChangeDate;
            materialRequest.MaxBillableHours = data.MaxBillableHours;

            materialRequest.MaterialRequestStatusType = NHibernateSession.Current.Load<MaterialRequestStatusType>(data.StatusTypeId);

            var linksData = GetMaterialLinksToUpsert(materialRequest.OutputMaterial, data.OutputTestMaterial.Links);

            var membersData = GetItemsToUpsert(materialRequest, data.Members);
            var testMaterialDocuments = UpsertTestMaterialDocuments(materialRequest.OutputMaterial, documents);

            return (linksData.itemsToRemove.Concat(testMaterialDocuments.itemsToRemove).Concat(membersData.itemsToRemove).ToArray(),
                new object[] { materialRequest.OutputMaterial, materialRequest }.Concat(linksData.itemToUpsert).Concat(testMaterialDocuments.itemToUpsert).Concat(membersData.itemToUpsert).ToArray(), materialRequest);
        }


        private TestMaterial GetTestMaterialToUpsert(TestMaterialDto testMaterial)
        {
            var material = NHibernateSession.Current.Get<TestMaterial>(testMaterial.Id) ?? new TestMaterial()
            {
                Language = NHibernateSession.Current.Get<Language>(testMaterial.LanguageId.GetValueOrDefault()),
                Skill = NHibernateSession.Current.Get<Skill>(testMaterial.SkillId.GetValueOrDefault()),
                TestMaterialType = NHibernateSession.Current.Load<TestMaterialType>(testMaterial.TestMaterialTypeId),
                TestComponentType = NHibernateSession.Current.Load<TestComponentType>(testMaterial.TestComponentTypeId),
            };

            material.Title = testMaterial.Title;
            material.TestMaterialDomain = NHibernateSession.Current.Load<TestMaterialDomain>(testMaterial.TestMaterialDomainId);
            material.Available = testMaterial.Available;

            return material;
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) UpsertRoundDocuments(MaterialRequestRound round, IEnumerable<MaterialRequestDocumentInfoDto> documentsData)
        {
            var itemsToUpsert = new List<object>();
            var service = new FileSystemFileStorageService(_autoMapperHelper);
            foreach (var document in documentsData)
            {
                var storedFiledId = document.StoredFileId;
                if (storedFiledId <= 0)
                {
                    var request = new CreateOrUpdateFileRequest
                    {
                        UpdateStoredFileId = null,
                        UpdateFileName = null,
                        Type = (StoredFileType)document.DocumentTypeId,
                        StoragePath = $"{(StoredFileType)document.DocumentTypeId}\\{document.FileName}",
                        UploadedByUserId = document.UserId,
                        UploadedDateTime = DateTime.Now,
                        FilePath = document.FilePath
                    };

                    var response = service.CreateOrUpdateFile(request);
                    storedFiledId = response.StoredFileId;
                }

                var storeFile = NHibernateSession.Current.Load<StoredFile>(storedFiledId);

                var roundDocument = NHibernateSession.Current.Get<MaterialRequestRoundAttachment>(document.AttachmentId) ??
                    new MaterialRequestRoundAttachment() { MaterialRequestRound = round };

                roundDocument.Description = Path.GetFileNameWithoutExtension(document.Description);
                roundDocument.StoredFile = storeFile;
                roundDocument.ExaminersAvailable = document.ExaminersAvailable;
                roundDocument.NcmsAvailable = document.NcmsAvailable;

                itemsToUpsert.Add(roundDocument);
            }

            return (new object[] { }, itemsToUpsert);
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) UpsertTestMaterialDocuments(TestMaterial testMaterial, IEnumerable<OutputTestMaterialDocumentInfoDto> documentsData)
        {
            var itemsToUpsert = new List<object>();
            var service = new FileSystemFileStorageService(_autoMapperHelper);
            foreach (var document in documentsData)
            {
                var storedFiledId = document.StoredFileId;

                if (storedFiledId == 0)
                {
                    var request = new CreateOrUpdateFileRequest
                    {
                        UpdateStoredFileId = null,
                        UpdateFileName = null,
                        Type = (StoredFileType)document.DocumentTypeId,
                        StoragePath = $"{(StoredFileType)document.DocumentTypeId}\\{document.FileName}",
                        UploadedByUserId = document.UserId,
                        UploadedByPersonId = document.PersonId,
                        UploadedDateTime = DateTime.Now,
                        FilePath = document.FilePath,
                    };

                    var response = service.CreateOrUpdateFile(request);
                    storedFiledId = response.StoredFileId;
                }

                var storeFile = NHibernateSession.Current.Load<StoredFile>(storedFiledId);


                var materialDocument = NHibernateSession.Current.Get<TestMaterialAttachment>(document.AttachmentId) ??
                                       new TestMaterialAttachment() { TestMaterial = testMaterial };

                materialDocument.Title = document.Description;
                materialDocument.StoredFile = storeFile;
                materialDocument.ExaminerToolsDownload = document.ExaminersAvailable;
                materialDocument.MergeDocument = document.MergeDocument;
                materialDocument.Deleted = false;

                itemsToUpsert.Add(materialDocument);
            }

            return (new object[] { }, itemsToUpsert);
        }
        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetPersonNotesToUpsert(IEnumerable<MaterialRequestPersonNoteDto> notesData)
        {
            var itemsToUpsert = new List<object>();

            foreach (var note in notesData)
            {
                var baseNote = new Note()
                {
                    CreatedDate = DateTime.Now,
                    Description = note.Description,
                    Highlight = note.Highlight,
                    ReadOnly = note.ReadOnly,
                    User = NHibernateSession.Current.Load<User>(note.UserId)
                };

                var personNote = new NaatiEntityNote()
                {
                    Entity = NHibernateSession.Current.Query<NaatiEntity>()
                        .FirstOrDefault(x => x.NaatiNumber == note.NaatiNumber),
                    Note = baseNote
                };

                itemsToUpsert.Add(baseNote);
                itemsToUpsert.Add(personNote);
            }

            return (new object[] { }, itemsToUpsert);
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetMaterialRequestNotesToUpsert(Domain.MaterialRequest materialRequest, IEnumerable<MaterialRequestActionNoteDto> notesData)
        {
            var itemsToUpsert = new List<object>();

            foreach (var note in notesData)
            {
                var baseNote = new Note()
                {
                    CreatedDate = DateTime.Now,
                    Description = note.Description,
                    Highlight = note.Highlight,
                    ReadOnly = note.ReadOnly,
                    User = NHibernateSession.Current.Load<User>(note.UserId)
                };

                var materialRequestNote = new MaterialRequestNote()
                {
                    MaterialRequest = materialRequest,
                    Note = baseNote
                };

                itemsToUpsert.Add(baseNote);
                itemsToUpsert.Add(materialRequestNote);
            }

            return (new object[] { }, itemsToUpsert);
        }

        private (IList<object> itemsToRemove, IList<object> itemToUpsert) GetMaterialRequestPublicNotesToUpsert(Domain.MaterialRequest materialRequest, IEnumerable<MaterialRequestActionPublicNoteDto> notesData)
        {
            var itemsToUpsert = new List<object>();

            foreach (var note in notesData)
            {
                var baseNote = new Note()
                {
                    CreatedDate = DateTime.Now,
                    Description = note.Description,
                    Highlight = note.Highlight,
                    ReadOnly = note.ReadOnly,
                    User = NHibernateSession.Current.Load<User>(note.UserId)
                };

                var materialRequestNote = new MaterialRequestPublicNote()
                {
                    MaterialRequest = materialRequest,
                    Note = baseNote
                };

                itemsToUpsert.Add(baseNote);
                itemsToUpsert.Add(materialRequestNote);
            }

            return (new object[] { }, itemsToUpsert);
        }




        public LookupTypeResponse GetRoundsLookup(int materialRequestId)
        {
            var response = NHibernateSession.Current.Query<MaterialRequestRound>()
                .Where(x => x.MaterialRequest.Id == materialRequestId).ToList().Select(y => new LookupTypeDto() { Id = y.Id, DisplayName = y.RoundNumber.ToString() });
            return new LookupTypeResponse { Results = response.ToList() };
        }

        public ServiceResponse<IEnumerable<RoundDocumentLookupTypeDto>> GetExistingDocuments(int materialRequestId)
        {
            var result = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>()
                .Where(x => x.MaterialRequestRound.MaterialRequest.Id == materialRequestId).ToList().Select(y => new RoundDocumentLookupTypeDto()
                {
                    Id = y.StoredFile.Id,
                    DisplayName = y.Description,
                    DocumentTypeId = y.StoredFile.DocumentType.Id,
                    RoundNumber = y.MaterialRequestRound.RoundNumber,
                    FileType = Path.GetExtension(y.StoredFile.FileName),
                    Size = y.StoredFile.FileSize,
                    UploadedBy = y.StoredFile.UploadedByUser != null ? y.StoredFile.UploadedByUser.FullName : y.StoredFile.UploadedByPerson.FullName,
                    ExaminersAvailable = y.ExaminersAvailable,
                    MergeDocument = false

                }).ToList();

            return new ServiceResponse<IEnumerable<RoundDocumentLookupTypeDto>>() { Data = result };
        }

        public GetMaterialRequestAttachmentsResponse GetAttachments(GetMaterialRequestRoundAttachmentsRequest request)
        {
            var personId = request.PersonId.GetValueOrDefault();

            if (request.NAATINumber.HasValue)
            {
                var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(p => p.Entity.NaatiNumber == request.NAATINumber);
                personId = person?.Id ?? 0;
            }

            var query = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>();

            if (request.NcmsAvailable.HasValue)
            {
                query = query.Where(x => x.NcmsAvailable == request.NcmsAvailable.GetValueOrDefault());
            }

            if (request.ExaminerAvailable.HasValue)
            {
                query = query.Where(x => x.ExaminersAvailable == request.ExaminerAvailable.GetValueOrDefault());
            }

            var attachments = query.Where(n => n.MaterialRequestRound.Id == request.MaterialRequestRoundId).ToList().Select(n => new MaterialRequestRoundAttachmentDto()
            {
                MaterialRequestRoundAttachmentId = n.Id,
                MaterialRequestRoundId = n.MaterialRequestRound.Id,
                StoredFileId = n.StoredFile.Id,
                FileName = n.StoredFile.FileName,
                Description = n.Description,
                DocumentType = n.StoredFile.DocumentType.DisplayName,
                UploadedByName = n.StoredFile.UploadedByUser != null ? n.StoredFile.UploadedByUser.FullName : n.StoredFile.UploadedByPerson.FullName,
                UploadedDateTime = n.StoredFile.UploadedDateTime,
                FileSize = n.StoredFile.FileSize,
                Type = (StoredFileType)n.StoredFile.DocumentType.Id,
                UploadedByUserId = n.StoredFile.UploadedByUser?.Id,
                UploadedByPersonId = n.StoredFile.UploadedByPerson?.Id,
                EportalDownload = n.ExaminersAvailable,
                IsOwner = personId == n.StoredFile.UploadedByPerson?.Id,
                NcmsAvailable = n.NcmsAvailable,
                FileType = Path.GetExtension(n.StoredFile.FileName),
                SoftDeleteDate = n.StoredFile.StoredFileStatusType.Id != 1? n.StoredFile.StoredFileStatusChangeDate:null
            });

            return new GetMaterialRequestAttachmentsResponse
            {
                Attachments = attachments.ToArray()
            };
        }

        public CreateOrReplaceMaterialRequestRoundAttachmentResponse CreateOrReplaceAttachment(CreateOrReplaceMaterialRequestRoundAttachmentRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var personId = request.UploadedByPersonId;

            if (!personId.HasValue && request.NAATINumber.HasValue)
            {
                var person = NHibernateSession.Current.Query<Person>().Single(x => x.Entity.NaatiNumber == request.NAATINumber);
                personId = person.Id;
            }

            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = request.StoredFileId != 0 ? (int?)request.StoredFileId : null,
                UpdateFileName = request.StoredFileId != 0 ? request.FileName : null,
                Type = request.Type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedByPersonId = personId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
                TokenToRemoveFromFilename = request.TokenToRemoveFromFilename,
            });

            var storeFile = NHibernateSession.Current.Load<StoredFile>(response.StoredFileId);
            var materialRequestRound = NHibernateSession.Current.Load<MaterialRequestRound>(request.MaterialRequestRoundId);
            var materialRequestAttachment = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>().SingleOrDefault(n => n.StoredFile.Id == response.StoredFileId);
            if (materialRequestAttachment == null)
            {
                materialRequestAttachment = new MaterialRequestRoundAttachment()
                {
                    StoredFile = storeFile,
                    MaterialRequestRound = materialRequestRound,
                };
            }

            materialRequestAttachment.Description = Path.GetFileNameWithoutExtension(request.Title);
            materialRequestAttachment.ExaminersAvailable = request.EportalDownload;
            materialRequestAttachment.NcmsAvailable = request.NcmsAvailable;

            NHibernateSession.Current.Save(materialRequestAttachment);
            NHibernateSession.Current.Flush();

            return new CreateOrReplaceMaterialRequestRoundAttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public DeleteAttachmentResponse DeleteAttachment(DeleteMaterialRequestRoundAttachmentRequest request)
        {
            var attachment = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>().First(n => n.Id == request.MaterialRequestRoundAttachmentId);

            var storedFileId = attachment.StoredFile.Id;

            NHibernateSession.Current.Delete(attachment);
            NHibernateSession.Current.Flush();

            var response = new DeleteAttachmentResponse();

            var existingEmailAttachment = NHibernateSession.Current.Query<EmailMessageAttachment>().Any(x => x.StoredFile.Id == storedFileId);

            if (existingEmailAttachment)
            {
                return response;
            }

            var existingRoundAttachment = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>().Any(x => x.StoredFile.Id == storedFileId);

            if (existingRoundAttachment)
            {
                return response;
            }

            var existingTestMaterial = NHibernateSession.Current.Query<TestMaterialAttachment>().Any(x => x.StoredFile.Id == storedFileId);

            if (existingTestMaterial)
            {
                return response;
            }

            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            fileService.DeleteFile(new DeleteFileRequest
            {
                StoredFileId = storedFileId
            });

            return response;
        }

        public GetSkillsForCredentialTypeResponse GetMaterialRequestSkills(MaterialRequestSkillRequest request)
        {

            var skillTypeId = NHibernateSession.Current.Load<CredentialType>(request.CredentialTypeId).SkillType.Id;
            var languageId = NHibernateSession.Current.Load<Panel>(request.PanelId).Language.Id;

            var skills = NHibernateSession.Current.Query<SkillApplicationType>()
                .Where(ca => ca.Skill.SkillType.Id == skillTypeId && (ca.Skill.Language1.Id == languageId || ca.Skill.Language2.Id == languageId))
                .Select(y => y.Skill).ToList().GroupBy(x => x.Id)
                .Select(v => new SkillLookupDto
                {
                    Id = v.Key,
                    DisplayName = v.First().DisplayName,
                    CredentialTypeId = request.CredentialTypeId,
                    Language1Id = v.First().Language1.Id,
                    Language2Id = v.First().Language2.Id,
                    DirectionTypeId = v.First().DirectionType.Id,
                });


            return new GetSkillsForCredentialTypeResponse
            {
                Results = skills
            };
        }

        public TestComponentTypeDto GetTestComponentType(int testComponentTypeId)
        {
            var response = NHibernateSession.Current.Load<TestComponentType>(testComponentTypeId);

            return new TestComponentTypeDto
            {
                DefaultMaterialRequestDueDays = response.DefaultMaterialRequestDueDays,
                DefaultMaterialRequestHours = response.DefaultMaterialRequestHours,
                Id = response.Id,
                CredentialType = response.TestSpecification.CredentialType.ExternalName,
                Description = response.Description,
                CredentialTypeId = response.TestSpecification.CredentialType.Id
            };
        }

        public TestMaterialRequestSearchResultResponse SearchTestMaterialRequests(TestMaterialRequestSearchRequest request)
        {
            var queryHelper = new MaterialRequestQueryHelper();

            var testMaterialRequestSummaryResponse = new TestMaterialRequestSearchResultResponse { Results = queryHelper.Search(request) };
            return testMaterialRequestSummaryResponse;
        }

        public ServiceResponse<int> CountTestMaterialRequests(TestMaterialRequestSearchRequest request)
        {
            var queryHelper = new MaterialRequestQueryHelper();
            var result = queryHelper.SearchCount(request);
            return new ServiceResponse<int>() { Data = result };
        }

        public GetMaterialRequestAttachmentResponse GetAttachment(GetMaterialRequestRoundAttachmentRequest request)
        {
            var attachment = NHibernateSession.Current.Query<MaterialRequestRoundAttachment>().Single(x => x.Id == request.MaterialRequestRoundAttachmentId);

            var mFileStorageService = new FileSystemFileStorageService(_autoMapperHelper);
            var storedFile = mFileStorageService.GetFile(new GetFileRequest { StoredFileId = attachment.StoredFile.Id, TempFileStorePath = request.TempFileStorePath });

            return new GetMaterialRequestAttachmentResponse
            {
                FileName = attachment.StoredFile.FileName,
                FilePaths = storedFile.FilePaths
            };
        }

        public UpdateMaterialRequestMembersResponse UpdateMaterialRequestMembers(UpdateMaterialRequestMembersRequest request)
        {
            var requestIds = request.Members.Select(m => m.PanelMemberShipId).ToList();
            var materialMembershipQuery = NHibernateSession.Current.Query<MaterialRequestPanelMembership>();
            var panelMembershipQuery = NHibernateSession.Current.Query<PanelMembership>();
            var materialRequest = NHibernateSession.Current.Query<Domain.MaterialRequest>().FirstOrDefault(m => m.Id == request.MaterialRequestId);
            var membersUpdated = materialMembershipQuery.Where(m => requestIds.Contains(m.PanelMembership.Id) && m.MaterialRequest.Id == request.MaterialRequestId).ToList();
            var membersToDelete = materialMembershipQuery.Where(m => !requestIds.Contains(m.PanelMembership.Id) && m.MaterialRequest.Id == request.MaterialRequestId).ToList();

            var existIds = membersUpdated.Select(m => m.PanelMembership.Id).ToList();
            var newMembers = panelMembershipQuery.Where(m => requestIds.Contains(m.Id) && !existIds.Contains(m.Id));

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                AddMembers(newMembers, request, materialRequest);
                UpdateMembers(membersUpdated, request);
                DeleteMembers(membersToDelete);
                transaction.Commit();
            }

            return new UpdateMaterialRequestMembersResponse();
        }

        private void DeleteMembers(List<MaterialRequestPanelMembership> membersToDelete)
        {
            foreach (var m in membersToDelete)
            {
                NHibernateSession.Current.Delete(m);
            }
        }

        private void UpdateMembers(List<MaterialRequestPanelMembership> membersUpdated, UpdateMaterialRequestMembersRequest request)
        {
            foreach (var m in membersUpdated)
            {
                var member = request.Members.FirstOrDefault(me => me.PanelMemberShipId == m.PanelMembership.Id);
                foreach (var t in m.MaterialRequestPanelMembershipTasks)
                {
                    NHibernateSession.Current.Delete(t);
                }

                AddTasks(m, member.Tasks);
            }
        }

        private void AddMembers(IQueryable<PanelMembership> newMembers, UpdateMaterialRequestMembersRequest request, Domain.MaterialRequest materialRequest)
        {
            var materialRequestPanelMembershipType = NHibernateSession.Current.Load<MaterialRequestPanelMembershipType>((int)MaterialRequestPanelMembershipTypeName.PanelCollaborator);

            foreach (var m in newMembers)
            {
                var member = request.Members.FirstOrDefault(me => me.PanelMemberShipId == m.Id);
                var newMember = new MaterialRequestPanelMembership
                {
                    MaterialRequest = materialRequest,
                    PanelMembership = m,
                    MaterialRequestPanelMembershipType = materialRequestPanelMembershipType
                };

                NHibernateSession.Current.Save(newMember);
                AddTasks(newMember, member.Tasks);
            }
        }

        private void AddTasks(MaterialRequestPanelMembership member, IList<MaterialRequestTaskDto> tasks)
        {
            var taskTypes = NHibernateSession.Current.Query<MaterialRequestTaskType>().ToList();
            foreach (var t in tasks)
            {
                var task = new MaterialRequestPanelMembershipTask
                {
                    MaterialRequestPanelMembership = member,
                    HoursSpent = t.HoursSpent,
                    MaterialRequestTaskType = taskTypes.FirstOrDefault(tt => tt.Id == t.MaterialRequestTaskTypeId)
                };
                NHibernateSession.Current.Save(task);
            }
        }

        public LookupTypeResponse GetAvailableMaterialRequestCredentialTypes(int naatiNumber)
        {
            var credentialTypeIds = NHibernateSession.Current.Query<MaterialRequestPanelMembership>()
                .Where(x => x.PanelMembership.Person.Entity.NaatiNumber == naatiNumber)
                .Select(x => x.MaterialRequest.OutputMaterial.TestComponentType.TestSpecification.CredentialType.Id)
                .Distinct().ToList();

            credentialTypeIds.Add(0);

            var lookups = NHibernateSession.Current.Query<CredentialType>().Where(x => credentialTypeIds.Contains(x.Id))
                .Select(y => new LookupTypeDto()
                {
                    Id = y.Id,
                    DisplayName = y.ExternalName
                }).ToList();

            return new LookupTypeResponse()
            {
                Results = lookups
            };

        }

        public SaveMaterialRequestRoundLinkResponse SaveMaterialRequestRoundLink(SaveMaterialRequestRoundLinkRequest request)
        {
            var round = NHibernateSession.Current.Load<MaterialRequestRound>(request.MaterialRequestRoundId);
            var link = new MaterialRequestRoundLink
            {
                Link = request.Link,
                MaterialRequestRound = round,
                NcmsAvailable = request.NcmsAvailable,
                Person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == request.NaatiNumber)
            };
            NHibernateSession.Current.Save(link);
            NHibernateSession.Current.Flush();
            return new SaveMaterialRequestRoundLinkResponse { MaterialRequestRoundLinkId = link.Id };
        }

        public GetMaterialRequestRoundLinkResponse GetMaterialRequestRoundLink(GetMaterialRequestRoundLinkRequest request)
        {
            var links = NHibernateSession.Current.Query<MaterialRequestRoundLink>().Where(l => l.MaterialRequestRound.Id == request.MaterialRequestRoundId);
            return new GetMaterialRequestRoundLinkResponse
            {
                Results = links.Select(l => new MaterialRequestRoundLinkDto
                {
                    Id = l.Id,
                    Link = l.Link,
                    NcmsAvailable = l.NcmsAvailable,
                    PersonNaatiNumber = l.Person.Entity.NaatiNumber
                }).ToList()
            };
        }

        public void DeleteMaterialRequestLink(DeleteMaterialRequestLinkRequest request)
        {
            var link = NHibernateSession.Current.Load<MaterialRequestRoundLink>(request.MaterialRequestRoundLinkId);
            NHibernateSession.Current.Delete(link);
            NHibernateSession.Current.Flush();
        }

   
    }
}
