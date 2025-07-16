using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts.Models.Examiner;

namespace Ncms.Bl.Mappers
{
    public class ExaminerDtoMapper : BaseMapper<ExaminerDto, ExaminerModel>
    {
        public override ExaminerModel Map(ExaminerDto source)
        {
            return new ExaminerModel
            {
                EntityId = source.EntityId,
                Name = source.Name,
                FirstName = source.FirstName,
                LastName = source.LastName,
                NAATINumber = source.NaatiNumber,
                IsChair = source.IsChair,
                PanelId = source.PanelId,
                PanelMembershipId = source.PanelMembershipId,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                PersonId = source.PersonId,
                PersonName = source.PersonName,
                DoNotSendCorrespondence = source.DoNotSendCorrespondence
            };
        }

        public override ExaminerDto MapInverse(ExaminerModel source)
        {
            throw new NotImplementedException();
        }
    }

    public class ExtendedExaminerDtoMapper : BaseMapper<ExaminerDto, ExtendedExaminerModel>
    {
        public override ExtendedExaminerModel Map(ExaminerDto source)
        {
            return new ExtendedExaminerModel
            {
                EntityId = source.EntityId,
                Name = source.Name,
                FirstName = source.FirstName,
                LastName = source.LastName,
                NAATINumber = source.NaatiNumber,
                IsChair = source.IsChair,
                TestResultId = source.TestResultId,
                DateAllocated = source.DateAllocated,
                DueDate = source.DueDate,
                EndDate = source.EndDate,
                ExaminerCost = source.ExaminerCost,
                ExaminerPaperLost = source.ExaminerPaperLost,
                ExaminerPaperReceivedDate = source.ExaminerPaperReceivedDate,
                ExaminerReceivedDate = source.ExaminerReceivedDate,
                ExaminerReceivedUser = source.ExaminerReceivedUser,
                ExaminerReceivedUserID = source.ExaminerReceivedUserId,
                ExaminerSentDate = source.ExaminerSentDate,
                ExaminerSentUser = source.ExaminerSentUser,
                ExaminerSentUserID = source.ExaminerSentUserId,
                ExaminerToPayrollDate = source.ExaminerToPayrollDate,
                ExaminerToPayrollUser = source.ExaminerToPayrollUser,
                ExaminerToPayrollUserID = source.ExaminerToPayrollUserId,
                JobExaminerID = source.JobExaminerId,
                JobId = source.JobId,
                LetterRecipient = source.LetterRecipient,
                MarkerStatus = source.MarkerStatus,
                NaatiNumberDisplay = source.NaatiNumberDisplay,
                PaidReviewer = source.PaidReviewer,
                PanelMembershipId = source.PanelMembershipId,
                PersonName = source.PersonName,
                ThirdExaminer = source.ThirdExaminer,
                PanelRoleId = source.PanelRoleId,
                ProductSpecificationId = source.ProductSpecificationId,
                ProductSpecificationCode = source.ProductSpecificationCode,
                PayrollStatusDisplayName = source.PayrollStatusName,
                ProductSpecificationChangedDate = source.ProductSpecificationChangedDate,
                ProductSpecificationChangedUserId = source.ProductSpecificationChangedUserId
            };
        }

        public override ExaminerDto MapInverse(ExtendedExaminerModel source)
        {
            throw new NotImplementedException();
        }
    }
}
