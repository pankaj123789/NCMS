using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using MyNaati.Contracts.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace MyNaati.Bl.BackOffice
{
    public class UnraisedInvoiceService : IUnraisedInvoiceService
    {
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly IApplicationBusinessLogicService mApplicationBusinessLogicService;
        private readonly ISystemQueryService mSystemQueryService;

        public UnraisedInvoiceService(IApplicationQueryService applicationQueryService,
            IApplicationBusinessLogicService applicationBusinessLogicService,
            ISystemQueryService systemQueryService)
        {
            mApplicationQueryService = applicationQueryService;
            mApplicationBusinessLogicService = applicationBusinessLogicService;
            mSystemQueryService = systemQueryService;
        }

        public GetUnraisedInvoiceSectionsResponse GetUnraisedInvoiceSections(int credentialApplicationId)
        {
            var sections = GetUnraisedInvoiceSections();
            return new GetUnraisedInvoiceSectionsResponse { Results = sections };
        }

        public CredentialApplicationDto GetApplication(int credentialApplicationId)
        {
            return mApplicationQueryService.GetApplication(credentialApplicationId).Result;
        }

        private IList<UnraisedInvoicesSectionContract> GetUnraisedInvoiceSections()
        {
            var termsAndConditions = mSystemQueryService
               .GetSystemValue(new GetSystemValueRequest() { ValueKey = "SelectTestTermsAndConditionsUrl" })
               .Value;

            var sections = new List<UnraisedInvoicesSectionContract>();

            sections.Add(new UnraisedInvoicesSectionContract()
            {
                Id = 1,
                Name = "Application Information",
                Questions = new List<UnraisedInvoicesQuestionContract>
                {
                    new UnraisedInvoicesQuestionContract()
                    {
                        Id = 1,
                        AnswerTypeId =  (int)CredentialApplicationFormAnswerTypeName.Information,
                        AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                        QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                    }
                }
            });

            sections.Add(new UnraisedInvoicesSectionContract()
            {
                Id = 2,
                Name = "Payment",
                Questions = new List<UnraisedInvoicesQuestionContract>
                {
                    new UnraisedInvoicesQuestionContract()
                    {
                        Id = 2,
                        AnswerTypeId =  (int)CredentialApplicationFormAnswerTypeName.PaymentControl,
                        AnswerOptions = Enumerable.Empty<AnswerOptionContract>(),
                        QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                    }
                }
            });


            sections.Add(new UnraisedInvoicesSectionContract()
            {
                Id = 3,
                Name = "Terms and Conditions",
                Questions = new List<UnraisedInvoicesQuestionContract>
                {
                    new UnraisedInvoicesQuestionContract()
                    {
                        Id = 3,
                        Text = $"Do you agree to the <a href=\"{termsAndConditions}\" target=\"_blank\">Terms and Conditions</a>?",
                        AnswerTypeId = (int) CredentialApplicationFormAnswerTypeName.RadioOptions,
                        AnswerOptions = new[]
                        {
                            new AnswerOptionContract
                            {
                                Id = 1,
                                Option = "Yes",
                                Description = "Click <b>Finish</b> to pay this invoice.",
                                Documents = Enumerable.Empty<AnswerDocumentContract>()
                            },
                            new AnswerOptionContract
                            {
                                Id = 2,
                                Option = "No",
                                Description = "Click <b>Finish</b> to cancel your selection.",
                                Documents = Enumerable.Empty<AnswerDocumentContract>()
                            },
                        },
                        QuestionLogics = Enumerable.Empty<QuestionLogicContract>()
                    }
                }
            });


            return sections;
        }
    }
}
