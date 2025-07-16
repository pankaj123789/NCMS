using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Ui.ViewModels.Certificate;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.Products;
using MyNaati.Ui.ViewModels.Stamp;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {

            CreateMap<ApplicationFormSectionContract, ApplicationFormSectionModel>();
            CreateMap<ApplicationFormQuestionContract, ApplicationFormQuestionModel>()
                .ForMember(x => x.Question, y => y.MapFrom(z => z.Text))
                .ForMember(x => x.Type, y => y.MapFrom(z => z.AnswerTypeId))
                .ForMember(x => x.Answers, y => y.MapFrom(z => z.AnswerOptions))
                .ForMember(x => x.Logics, y => y.MapFrom(z => z.QuestionLogics));

            CreateMap<AnswerOptionContract, ApplicationFormAnswerModel>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Option))
                .ForMember(x => x.MandatoryAttachments, y => y.MapFrom(z => z.Documents.Any()))
                .ForMember(x => x.Documents, y => y.MapFrom(z => z.Documents));

            CreateMap<AnswerCredentialOptionContract, ApplicationFormCredentialAnswerModel>()
                .IncludeBase<AnswerOptionContract, ApplicationFormAnswerModel>();


            CreateMap<AnswerDocumentContract, ApplicationFormAnswerDocumentModel>();

            CreateMap<OptionActionContract, ApplicationFormAnswerActionModel>()
                .ForMember(x => x.Type, y => y.MapFrom(z => z.ActionTypeId));

            CreateMap<QuestionLogicContract, ApplicationFormQuestionLogicModel>();
            
            CreateMap<SaveApplicationFormRequestModel, SaveApplicationFormRequestContract>()
                .ForMember(x => x.UserName, y => y.MapFrom(z => GetUserName()));

            CreateMap<ApplicationFormSectionModel, ApplicationFormSectionContract>()
                .ForMember(x => x.HasTokens, y => y.Ignore());

            CreateMap<ApplicationFormQuestionModel, ApplicationFormQuestionContract>()
                .ForMember(x => x.Text, y => y.MapFrom(z => z.Question))
                .ForMember(x => x.AnswerTypeId, y => y.MapFrom(z => z.Type))
                .ForMember(x => x.AnswerOptions, y => y.MapFrom(z => z.Answers))
                .ForMember(x => x.QuestionLogics, y => y.Ignore());

            CreateMap<ApplicationFormAnswerModel, AnswerOptionContract>()
                .ForMember(x => x.Option, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Function, y => y.Ignore())
                .ForMember(x => x.Documents, y => y.Ignore());
        }


        private string GetUserName()
        {
            return ServiceLocator.Resolve<ISecretsCacheQueryService>().Get(SecuritySettings.MyNaatiDefaultIdentityKey);
        }
    }
}