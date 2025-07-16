using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Ui.ViewModels.Certificate;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.ExaminerTools;
using MyNaati.Ui.ViewModels.Products;
using MyNaati.Ui.ViewModels.Stamp;
using RolePlayerSettingsDto = MyNaati.Contracts.BackOffice.RolePlayerSettingsDto;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestComponentModel, TestMarkingComponentContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.TotalMarks, y => y.MapFrom(z => z.TotalMarks))
                .ForMember(x => x.PassMark, y => y.MapFrom(z => z.PassMark))
                .ForMember(x => x.Mark, y => y.MapFrom(z => z.Mark))
                .ForMember(x => x.ComponentNumber, y => y.MapFrom(z => z.ComponentNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.TypeName, y => y.MapFrom(z => z.TypeName))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TypeLabel))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted))
                .ForMember(x => x.Competencies, y => y.MapFrom(z => z.Competencies))
                .ForMember(x => x.Successful, y => y.Ignore())
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly));

            CreateMap<TestCompetenceModel, TestCompetenceContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Assessments, y => y.MapFrom(z => z.Assessments));

            CreateMap<TestBandModel, TestBandContract>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.Level, y => y.MapFrom(z => z.Level))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description));

            CreateMap<TestBandContract, TestBandModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.Level, y => y.MapFrom(z => z.Level))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description));

            CreateMap<TestAssessmentModel, TestAssessmentContract>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comment))
                .ForMember(x => x.SelectedBand, y => y.MapFrom(z => z.SelectedBand))
                .ForMember(x => x.ExaminerResults, y => y.MapFrom(z => Enumerable.Empty<ExaminerResultContract>()))
                .ForMember(x => x.Label, y => y.Ignore());

            CreateMap<TestMarkingComponentContract, TestComponentModel>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.TypeName, y => y.MapFrom(z => z.TypeName))
                .ForMember(x => x.TypeLabel, y => y.MapFrom(z => z.TypeLabel))
                .ForMember(x => x.Competencies, y => y.MapFrom(z => z.Competencies))
                .ForMember(x => x.WasAttempted, y => y.MapFrom(z => z.WasAttempted))
                .ForMember(x => x.ReadOnly, y => y.MapFrom(z => z.ReadOnly));

            CreateMap<TestCompetenceContract, TestCompetenceModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Assessments, y => y.MapFrom(z => z.Assessments));

            CreateMap<TestAssessmentContract, TestAssessmentModel>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Comment, y => y.MapFrom(z => z.Comment))
                .ForMember(x => x.SelectedBand, y => y.MapFrom(z => z.SelectedBand));
                
            CreateMap<TestComponentContract, TestComponentModel>()
                .ForMember(x => x.WasAttempted, y => y.Ignore())
                .ForMember(x => x.MinExaminerCommentLength, y => y.Ignore())
                .ForMember(x => x.MaxCommentLength, y => y.Ignore())
                .ForMember(x => x.Competencies, y => y.Ignore());

            CreateMap<TestComponentModel, TestComponentContract>();

            CreateMap<TestComponentModel, MyNaati.Contracts.Portal.TestComponentContract>();

            CreateMap<RolePlayerSettingsDto, RolePlayerSettingsModel>();

            CreateMap<TestContract, TestModel>()
                .ForMember(x => x.JobExaminerID, y => y.Ignore())
                .ForMember(x => x.TestMaterialID, y => y.Ignore())
                .ForMember(x => x.Materials, y => y.Ignore());
        }
    }
}
 