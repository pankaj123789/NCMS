using AutoMapper;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.ViewModels.CredentialApplication;
using MyNaati.Ui.ViewModels.UnraisedInvoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class UnraisedInvoiceProfile : Profile
    {
        public UnraisedInvoiceProfile()
        {
            CreateMap<UnraisedInvoicesSectionContract, UnraisedInvoicesSectionModel>();
            CreateMap<UnraisedInvoicesQuestionContract, UnraisedInvoicesQuestionModel>()
                .ForMember(x => x.Question, y => y.MapFrom(z => z.Text))
                .ForMember(x => x.Type, y => y.MapFrom(z => z.AnswerTypeId))
                .ForMember(x => x.Answers, y => y.MapFrom(z => z.AnswerOptions))
                .ForMember(x => x.Logics, y => y.MapFrom(z => z.QuestionLogics));

            //CreateMap<AnswerOptionContract, UnraisedInvoicesAnswerModel>()
            //    .ForMember(x => x.Name, y => y.MapFrom(z => z.Option))
            //    .ForMember(x => x.MandatoryAttachments, y => y.MapFrom(z => z.Documents.Any()))
            //    .ForMember(x => x.Documents, y => y.Ignore());

            CreateMap<UnraisedInvoicesAnswerModel, AnswerOptionContract>()
                .ForMember(x => x.Option, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Function, y => y.Ignore())
                .ForMember(x => x.Documents, y => y.Ignore());

            CreateMap<UnraisedInvoicesQuestionModel, ApplicationFormQuestionContract>()
                .ForMember(x => x.Text, y => y.MapFrom(z => z.Question))
                .ForMember(x => x.AnswerTypeId, y => y.MapFrom(z => z.Type))
                .ForMember(x => x.AnswerOptions, y => y.MapFrom(z => z.Answers))
                .ForMember(x => x.QuestionLogics, y => y.Ignore());

            CreateMap<QuestionLogicContract, UnraisedInvoicesQuestionLogicModel>();

            CreateMap<AnswerDocumentContract, UnraisedInvoicesAnswerDocumentModel>();
        }
    }
}