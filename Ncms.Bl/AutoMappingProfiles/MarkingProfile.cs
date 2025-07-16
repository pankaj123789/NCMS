using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Rubrics.Rules;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Test;
using TestComponentModel = Ncms.Contracts.Models.Test.TestComponentModel;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class MarkingProfile : Profile
    {
        public MarkingProfile()
        {
            CreateMap<RubricMarkingBandDto, RubricMarkingBandModel>();

            CreateMap<RubricMarkingBandModel, RubricMarkingBandDto>();

            CreateMap<GetRubricMarkingBandResponse, GetRubricMarkingBandResponseModel>();

            CreateMap<RubricQuestionPassRuleDto, RubricQuestionPassRuleModel>();

            CreateMap<RubricQuestionPassRuleModel, RubricQuestionPassRuleDto>();

            CreateMap<GetQuestionPassRulesResponse, GetQuestionPassRulesResponseModel>()
                .ForMember(x => x.TestSpecificationId, y => y.MapFrom(z => z.TestSpecificationId))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.Configurations, y => y.MapFrom(z => z.Configurations));

            CreateMap<RubricTestBandRuleDto, RubricTestBandRuleModel>();

            CreateMap<RubricTestBandRuleModel, RubricTestBandRuleDto>();

            CreateMap<GetTestBandRulesResponse, GetTestBandRulesResponseModel>()
                .ForMember(x => x.TestSpecificationId, y => y.MapFrom(z => z.TestSpecificationId))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.Configurations, y => y.MapFrom(z => z.Configurations));

            CreateMap<GetRubricConfigurationResponse, GetRubricConfigurationResponseModel>()
                .ForMember(x => x.TestSpecificationId, y => y.MapFrom(z => z.TestSpecificationId))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents));


            CreateMap<RubricTestQuestionRuleDto, RubricTestQuestionRuleModel>();

            CreateMap<RubricTestQuestionRuleModel, RubricTestQuestionRuleDto>();

            CreateMap<GetTestQuestionRulesResponse, GetTestQuestionRulesResponseModel>()
                .ForMember(x => x.TestSpecificationId, y => y.MapFrom(z => z.TestSpecificationId))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.Configurations, y => y.MapFrom(z => z.Configurations));

            CreateMap<TestResultMarkingDto, TestRubricModel>()
                .ForMember(x => x.DateSubmitted, y => y.MapFrom(z => z.ModifiedDate))
                .ForMember(x => x.OriginalTestResultStatusTypeId, y => y.MapFrom(z => z.TestResultStatusTypeId))
                .ForMember(x => x.AttendanceId, y => y.MapFrom(z => z.AttendanceId))
                .ForMember(x => x.Id, y => y.MapFrom(z => z.TestResultId))
                .ForMember(x => x.MinCommentsLength, y => y.MapFrom(z => 1))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.ResultAutoCalculation, y => y.MapFrom(z => z.ResultAutoCalculation))
                .ForMember(x => x.ExaminerName, y => y.Ignore())
                .ForMember(x => x.ReceivedDate, y => y.Ignore())
                .ForMember(x => x.CredentialType, y => y.Ignore())
                .ForMember(x => x.Skill, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.Ignore())
                .ForMember(x => x.ApplicationReference, y => y.Ignore())
                .ForMember(x => x.ApplicationId, y => y.Ignore())
                .ForMember(x => x.ApplicationType, y => y.Ignore())
                .ForMember(x => x.TestDate, y => y.Ignore())
                .ForMember(x => x.TestStatus, y => y.Ignore())
                .ForMember(x => x.TestStatusTypeId, y => y.Ignore())
                .ForMember(x => x.TestResultStatus, y => y.Ignore())
                .ForMember(x => x.TestLocation, y => y.Ignore())
                .ForMember(x => x.Venue, y => y.Ignore())
                .ForMember(x => x.TestMaterialIds, y => y.Ignore())
                .ForMember(x => x.Supplementary, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForPass, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForConcededPass, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForSupplementary, y => y.Ignore())
                .ForMember(x => x.Feedback, y => y.Ignore());

            CreateMap<TestSummaryDto, TestRubricModel>()
                .ForMember(x => x.CredentialType, y => y.MapFrom(z => z.CredentialTypeInternalName))
                .ForMember(x => x.ApplicationId, y => y.MapFrom(z => z.ApplicationId))
                .ForMember(x => x.ApplicationReference, y => y.MapFrom(z => z.ApplicationReference))
                .ForMember(x => x.ApplicationType, y => y.MapFrom(z => z.ApplicationType))
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.NaatiNumber))
                .ForMember(x => x.Skill, y => y.MapFrom(z => z.Skill))
                .ForMember(x => x.TestDate, y => y.MapFrom(z => z.TestDate))
                .ForMember(x => x.TestLocation, y => y.MapFrom(z => z.TestLocation))
                .ForMember(x => x.TestMaterialIds, y => y.MapFrom(z => z.TestMaterialIds))
                .ForMember(x => x.TestStatus, y => y.MapFrom(z => z.TestStatus))
                .ForMember(x => x.TestStatusTypeId, y => y.MapFrom(z => z.TestStatusTypeId))
                .ForMember(x => x.TestResultStatusTypeId, y => y.MapFrom(z => z.LastTestResultStatusTypeId))
                .ForMember(x => x.TestResultStatus, y => y.MapFrom(z => z.LastTestResultStatus))
                .ForMember(x => x.Venue, y => y.MapFrom(z => z.Venue))
                .ForMember(x => x.AttendanceId, y => y.MapFrom(z => z.TestAttendanceId))
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.ExaminerName, y => y.Ignore())
                .ForMember(x => x.DateSubmitted, y => y.Ignore())
                .ForMember(x => x.ReceivedDate, y => y.Ignore())
                .ForMember(x => x.TestComponents, y => y.Ignore())
                .ForMember(x => x.OriginalTestResultStatusTypeId, y => y.Ignore())
                .ForMember(x => x.MinCommentsLength, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForPass, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForConcededPass, y => y.MapFrom(z => z.EligibleForConcededPass))
                .ForMember(x => x.ComputedEligibleForSupplementary, y => y.MapFrom(z => z.EligibleForSupplementary))
                .ForMember(x => x.ResultAutoCalculation, y => y.Ignore())
                .ForMember(x => x.Feedback, y => y.Ignore());

            CreateMap<TestMarkingComponentDto, TestComponentModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.TestComponentId))
                .ForMember(x => x.TotalMarks, y => y.MapFrom(z => z.TotalMarks))
                .ForMember(x => x.PassMark, y => y.MapFrom(z => z.PassMark))
                .ForMember(x => x.Mark, y => y.MapFrom(z => z.Mark))
                .ForMember(x => x.ComponentNumber, y => y.MapFrom(z => z.ComponentNumber))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Competencies, y => y.MapFrom(z => z.RubricMarkingCompentencies))
                .ForMember(x => x.Successful, y => y.MapFrom(z => z.Successful))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted))
                .ForMember(x => x.MarkingResultTypeId, y => y.MapFrom(z => z.MarkingResultTypeId))
                .ForMember(x => x.RubricTestComponentResultId, y => y.MapFrom(z => z.RubricTestComponentResultId))
                .ForMember(x => x.MaxCommentLength, y => y.MapFrom(z => TestResultService.MaxRubricCommentLength))
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly));

            CreateMap<RubricMarkingCompetencyDto, TestCompetenceModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.CompetencyId))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Assessments, y => y.MapFrom(z => z.RubricMarkingAssessmentCriteria));

            CreateMap<RubricMarkingAssessmentCriterionDto, TestAssessmentModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.AssessmentCriterionId))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comments))
                .ForMember(x => x.SelectedBand, y => y.MapFrom(z => z.SelectedBandId))
                .ForMember(x => x.Bands, y => y.MapFrom(z => z.Bands))
                .ForMember(x => x.ExaminerResults, y => y.MapFrom(z => z.ExaminerResults));

            CreateMap<RubricMarkingBandDto, TestBandModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.BandId))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description));

            CreateMap<ExaminerResultDto, ExaminerResultModel>()
                .ForMember(x => x.JobExaminerId, y => y.MapFrom(z => z.JobExaminerId))
                .ForMember(x => x.BandLevel, y => y.MapFrom(z => z.BandLevel))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comment))
                .ForMember(x => x.Initials, y => y.MapFrom(z => z.Initials));

            CreateMap<JobExaminerMarkingDto, TestRubricModel>()
                .ForMember(x => x.DateSubmitted, y => y.MapFrom(z => z.SubmittedDate))
                .ForMember(x => x.ReceivedDate, y => y.MapFrom(z => z.ReceivedDate))
                .ForMember(x => x.AttendanceId, y => y.MapFrom(z => z.AttendanceId))
                .ForMember(x => x.OriginalTestResultStatusTypeId, y => y.MapFrom(z => z.TestResultStatusTypeId))
                .ForMember(x => x.Id, y => y.MapFrom(z => z.JobExaminerId))
                .ForMember(x => x.ExaminerName, y => y.MapFrom(z => z.ExaminerName))
                .ForMember(x => x.MinCommentsLength, y => y.MapFrom(z => 10))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.ResultAutoCalculation, y => y.MapFrom(z => z.ResultAutoCalculation))
                .ForMember(x => x.CredentialType, y => y.Ignore())
                .ForMember(x => x.Skill, y => y.Ignore())
                .ForMember(x => x.ApplicationReference, y => y.Ignore())
                .ForMember(x => x.ApplicationId, y => y.Ignore())
                .ForMember(x => x.ApplicationType, y => y.Ignore())
                .ForMember(x => x.TestDate, y => y.Ignore())
                .ForMember(x => x.TestStatus, y => y.Ignore())
                .ForMember(x => x.TestStatusTypeId, y => y.Ignore())
                .ForMember(x => x.TestResultStatus, y => y.Ignore())
                .ForMember(x => x.TestLocation, y => y.Ignore())
                .ForMember(x => x.Venue, y => y.Ignore())
                .ForMember(x => x.TestMaterialIds, y => y.Ignore())
                .ForMember(x => x.Supplementary, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForPass, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForConcededPass, y => y.Ignore())
                .ForMember(x => x.ComputedEligibleForSupplementary, y => y.Ignore())
                .ForMember(x => x.Feedback, y => y.MapFrom(z => z.Feedback));

            CreateMap<TestRubricModel, JobExaminerMarkingDto>()
                .ForMember(x => x.SubmittedDate, y => y.MapFrom(z => z.DateSubmitted))
                .ForMember(x => x.ReceivedDate, y => y.MapFrom(z => z.ReceivedDate))
                .ForMember(x => x.JobExaminerId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.TestSpecificationId, y => y.Ignore())
                .ForMember(x => x.TestResultId, y => y.Ignore())
                .ForMember(x => x.Feedback, y => y.MapFrom(z => z.Feedback));

            CreateMap<TestComponentModel, TestMarkingComponentDto>()
                .ForMember(x => x.TestComponentId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TotalMarks, y => y.MapFrom(z => z.TotalMarks))
                .ForMember(x => x.PassMark, y => y.MapFrom(z => z.PassMark))
                .ForMember(x => x.Mark, y => y.MapFrom(z => z.Mark))
                .ForMember(x => x.ComponentNumber, y => y.MapFrom(z => z.ComponentNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted))
                .ForMember(x => x.Successful, y => y.MapFrom(z => z.Successful))
                .ForMember(x => x.RubricMarkingCompentencies, y => y.MapFrom(z => z.Competencies))
                .ForMember(x => x.MarkingResultTypeId, y => y.MapFrom(z => z.MarkingResultTypeId))
                .ForMember(x => x.RubricTestComponentResultId, y => y.MapFrom(z => z.RubricTestComponentResultId))
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly));

            CreateMap<TestCompetenceModel, RubricMarkingCompetencyDto>()
                .ForMember(x => x.CompetencyId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.RubricMarkingAssessmentCriteria, y => y.MapFrom(z => z.Assessments))
                .ForMember(x => x.Label, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

            CreateMap<TestBandModel, RubricMarkingBandDto>()
                .ForMember(x => x.BandId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.CriterionName, y => y.Ignore())
                .ForMember(x => x.CriterionLabel, y => y.Ignore());

            CreateMap<TestAssessmentModel, RubricMarkingAssessmentCriterionDto>()
                .ForMember(x => x.AssessmentCriterionId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comments, y => y.MapFrom(z => z.Comment ?? string.Empty))
                .ForMember(x => x.SelectedBandId, y => y.MapFrom(z => z.SelectedBand))
                .ForMember(x => x.Bands, y => y.MapFrom(z => z.Bands))
                .ForMember(x => x.ExaminerResults, y => y.MapFrom(z => Enumerable.Empty<ExaminerResultDto>()))
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

            CreateMap<TestRubricModel, TestResultMarkingDto>()
                .ForMember(x => x.ModifiedDate, y => y.MapFrom(z => z.DateSubmitted))
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.TestSpecificationId, y => y.Ignore());

            CreateMap<TestResultQuestionRuleDto, RubricTestResultQuestionRule>();

            CreateMap<TestResultBandRuleDto, RubricTestResultBandRule>();

            CreateMap<QuestionPassRuleDto, RubricQuestionPassRule>()
                .ForMember(x => x.TestResultEligibilityType, y => y.Ignore());

            CreateMap<MarkingForPayrollDto, MarkingForPayrollModel>();
        }
    }
}
