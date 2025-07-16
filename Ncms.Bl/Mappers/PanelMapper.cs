using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts.Models.Panel;

namespace Ncms.Bl.Mappers
{
    public class PanelDtoMapper : BaseMapper<PanelDto, PanelModel>
    {
        public override PanelModel Map(PanelDto source)
        {
            return new PanelModel
            {
                PanelId = source.PanelId,
                LanguageId = source.LanguageId,
                Language = source.Language,
                Name = source.Name,
                Note = source.Note,
                PanelTypeId = source.PanelTypeId,
                PanelType = source.PanelType,
                CommissionedDate = source.ComissionedDate,
                HasCurrentMembers = source.HasCurrentMembers,
                HasExaminersAllocated = source.HasExaminersAllocated,
                VisibleInEportal = source.VisibleInEportal,
            };
        }

        public override PanelDto MapInverse(PanelModel source)
        {
            throw new NotImplementedException();
        }
    }

    public class ExaminerUnavailabilityDtoMapper : BaseMapper<ExaminerUnavailabilityDto, ExaminerUnavailability>
    {
        public override ExaminerUnavailability Map(ExaminerUnavailabilityDto source)
        {
            return new ExaminerUnavailability
            {
                StartDate = source.StartDate,
                EndDate = source.EndDate
            };
        }

        public override ExaminerUnavailabilityDto MapInverse(ExaminerUnavailability source)
        {
            throw new NotImplementedException();
        }
    }

    public class MarkingRequestDtoMapper : BaseMapper<MarkingRequestDto, MarkingRequest>
    {
        public override MarkingRequest Map(MarkingRequestDto source)
        {
            return new MarkingRequest
            {
                AttendanceId = source.AttendanceId,
                NaatiNumber = source.NaatiNumber,
                ApplicantName = source.ApplicantName,
                Type = source.Type,
                Language = source.Language,
                Status = source.Status,
                Category = source.Category,
                Direction = source.Direction,
                DueDate = source.DueDate,
				TestDate = source.TestDate
            };
        }

        public override MarkingRequestDto MapInverse(MarkingRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class MaterialRequestDtoMapper : BaseMapper<MaterialRequestDto, MaterialRequest>
    {
        public override MaterialRequest Map(MaterialRequestDto source)
        {
            return new MaterialRequest
            {
                TestMaterialID = source.TestMaterialID,
                JobExaminerID = source.JobExaminerID,
                JobID = source.JobID,
                Language = source.Language,
                Category = source.Category,
                Direction = source.Direction,
                Level = source.Level,
                DueDate = source.DueDate,
                DateReceived = source.DateReceived,
                Cost = source.Cost,
                Approved = source.Approved
            };
        }

        public override MaterialRequestDto MapInverse(MaterialRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class PanelMembershipDtoMapper : BaseMapper<MembershipDto, PanelMembershipModel>
    {
        public override PanelMembershipModel Map(MembershipDto source)
        {
            var unAvailableExaminers = source.UnAvailableExaminers?.ToList();
            var currentDate = DateTime.Now;
            var futureDate = DateTime.Now.AddMonths(3);
            unAvailableExaminers = unAvailableExaminers?.Where(x => x.StartDate <= futureDate && x.EndDate.Date >= currentDate.Date).OrderBy(y=>y.StartDate).ToList();
            
            return new PanelMembershipModel
            {
                PanelMembershipId = source.PanelMembershipId,
                PersonId = source.PersonId,
                NaatiNumber = source.NAATINumber,
                PersonName = source.Name,
                PanelId = source.PanelId,
                PanelName = source.PanelName,
                PanelRoleId = source.PanelRoleId,
                PanelRole = source.PanelRole,
                IsExaminerRole = source.IsExaminerRole,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Phone = source.Phone,
                Email = source.Email,
                HasUnavailability = source.HasUnavailability,
                HasMarkingRequests = source.HasMarkingRequests,
                HasMaterialRequests = source.HasMaterialRequests,
                CredentialTypeIds = source.CredentialTypeIds.ToArray(),
                MaterialCredentialTypeIds = source.MaterialCredentialTypeIds.ToArray(),
                CoordinatorCredentialTypeIds = source.CoordinatorCredentialTypeIds.ToArray(),
                InProgress = source.InProgress,
                Overdue = source.Overdue,
                UnAvailableExaminers = unAvailableExaminers
            };
        }

        public override MembershipDto MapInverse(PanelMembershipModel source)
        {
            throw new NotImplementedException();
        }
    }

    public class CreatePanelMapper : BaseMapper<PanelModel, CreateOrUpdatePanelRequest>
    {
        public override CreateOrUpdatePanelRequest Map(PanelModel source)
        {
            return new CreateOrUpdatePanelRequest
            {
                PanelId = source.PanelId,
                Name = source.Name,
                PanelTypeId = source.PanelTypeId,
                LanguageId = source.LanguageId,
                ComissionedDate = source.CommissionedDate,
                Note = source.Note ?? string.Empty,
                VisibleInEportal = source.VisibleInEportal
            };
        }

        public override PanelModel MapInverse(CreateOrUpdatePanelRequest source)
        {
            throw new NotImplementedException();
        }
    }

    public class AddPanelMembershipMapper : BaseMapper<PanelMembershipModel, AddOrUpdateMembershipRequest>
    {
        public override AddOrUpdateMembershipRequest Map(PanelMembershipModel source)
        {
            var request = new AddOrUpdateMembershipRequest
            {
                PanelMembershipId = source.PanelMembershipId,
                PersonId = source.PersonId,
                PanelId = source.PanelId,
                PanelRoleId = source.PanelRoleId,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                CredentialTypeIds = source.CredentialTypeIds,
                MaterailCredentialTypesIds = source.MaterialCredentialTypeIds,
                CoordinatorCredentialTypesIds = source.CoordinatorCredentialTypeIds
            };

            return request; 
        }

        public override PanelMembershipModel MapInverse(AddOrUpdateMembershipRequest source)
        {
            throw new NotImplementedException();
        }
    }
}
