using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class MarkingProfile : Profile
    {
        private static int MaxRubrickCommentsLength = 2000;

        public MarkingProfile()
        {
            CreateMap<MarkingForPayrollDto, MarkingPayrollItemContract>()
                .ForMember(x => x.InvoiceTotal, y => y.Ignore());

            CreateMap<TestMarkingComponentDto, TestMarkingComponentContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.TestComponentId))
                .ForMember(x => x.TotalMarks, y => y.MapFrom(z => z.TotalMarks))
                .ForMember(x => x.PassMark, y => y.MapFrom(z => z.PassMark))
                .ForMember(x => x.Mark, y => y.MapFrom(z => z.Mark))
                .ForMember(x => x.ComponentNumber, y => y.MapFrom(z => z.ComponentNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.TypeName, y => y.MapFrom(z => z.TypeName))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TypeLabel))
                .ForMember(x => x.Competencies, y => y.MapFrom(z => z.RubricMarkingCompentencies))
                .ForMember(x => x.Successful, y => y.MapFrom(z => z.Successful))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted.GetValueOrDefault()))
                .ForMember(x => x.MaxCommentLength, y => y.MapFrom(z => MaxRubrickCommentsLength))
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly));

            CreateMap<RubricMarkingCompetencyDto, TestCompetenceContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.CompetencyId))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Assessments, y => y.MapFrom(z => z.RubricMarkingAssessmentCriteria));


            CreateMap<RubricMarkingAssessmentCriterionDto, TestAssessmentContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.AssessmentCriterionId))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comments))
                .ForMember(x => x.SelectedBand, y => y.MapFrom(z => z.SelectedBandId))
                .ForMember(x => x.Bands, y => y.MapFrom(z => z.Bands))
                .ForMember(x => x.ExaminerResults, y => y.MapFrom(z => z.ExaminerResults));

            CreateMap<RubricMarkingBandDto, TestBandContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.BandId))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.Level, y => y.MapFrom(z => z.Level))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description));

            CreateMap<ExaminerResultDto, ExaminerResultContract>()
                .ForMember(x => x.JobExaminerId, y => y.MapFrom(z => z.JobExaminerId))
                .ForMember(x => x.Band, y => y.MapFrom(z => z.BandLevel))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comment))
                .ForMember(x => x.Initials, y => y.MapFrom(z => "I"));

            CreateMap<JobExaminerMarkingDto, TestRubricMarkingContract>()
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.NaatiNumber))
                .ForMember(x => x.SubmittedDate, y => y.MapFrom(z => z.SubmittedDate))
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => z.TestResultId))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents));

            CreateMap<TestRubricMarkingContract, JobExaminerMarkingDto>()
                .ForMember(x => x.SubmittedDate, y => y.MapFrom(z => z.SubmittedDate))
                .ForMember(x => x.JobExaminerId, y => y.MapFrom(z => 0))
                .ForMember(x => x.TestComponents, y => y.MapFrom(z => z.TestComponents))
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => z.TestResultId))
                .ForMember(x => x.NaatiNumber, y => y.MapFrom(z => z.NaatiNumber))
                .ForMember(x => x.TestSpecificationId, y => y.Ignore())
                .ForMember(x => x.AttendanceId, y => y.Ignore())
                .ForMember(x => x.TestResultStatusTypeId, y => y.Ignore())
                .ForMember(x => x.ExaminerName, y => y.Ignore())
                .ForMember(x => x.ResultAutoCalculation, y => y.Ignore())
                .ForMember(x => x.ReceivedDate, y => y.Ignore());

            CreateMap<TestMarkingComponentContract, TestMarkingComponentDto>()
                .ForMember(x => x.TestComponentId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TotalMarks, y => y.MapFrom(z => z.TotalMarks))
                .ForMember(x => x.PassMark, y => y.MapFrom(z => z.PassMark))
                .ForMember(x => x.Mark, y => y.MapFrom(z => z.Mark))
                .ForMember(x => x.ComponentNumber, y => y.MapFrom(z => z.ComponentNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.TypeName, y => y.MapFrom(z => z.TypeName))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TypeLabel))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted))
                .ForMember(x => x.Successful, y => y.MapFrom(z => z.Successful))
                .ForMember(x => x.RubricMarkingCompentencies, y => y.MapFrom(z => z.Competencies))
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly))
                .ForMember(x => x.TestComponentTypeId, y => y.Ignore())
                .ForMember(x => x.TypeDescription, y => y.Ignore())
                .ForMember(x => x.MarkingResultTypeId, y => y.Ignore())
                .ForMember(x => x.RubricTestComponentResultId, y => y.Ignore())
                .ForMember(x => x.MinNaatiCommentLength, y => y.Ignore());

            CreateMap<TestCompetenceContract, RubricMarkingCompetencyDto>()
                .ForMember(x => x.CompetencyId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.RubricMarkingAssessmentCriteria, y => y.MapFrom(z => z.Assessments))
                .ForMember(x => x.Label, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

            CreateMap<TestBandContract, RubricMarkingBandDto>()
                .ForMember(x => x.BandId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.CriterionName, y => y.Ignore())
                .ForMember(x => x.CriterionLabel, y => y.Ignore());

            CreateMap<TestAssessmentContract, RubricMarkingAssessmentCriterionDto>()
                .ForMember(x => x.AssessmentCriterionId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comments, y => y.MapFrom(z => z.Comment ?? string.Empty))
                .ForMember(x => x.SelectedBandId, y => y.MapFrom(z => z.SelectedBand))
                .ForMember(x => x.Bands, y => y.MapFrom(z => z.Bands))
                .ForMember(x => x.ExaminerResults, y => y.MapFrom(z => Enumerable.Empty<ExaminerResultDto>()))
                .ForMember(x => x.DisplayOrder, y => y.Ignore());
        }
    }
}
