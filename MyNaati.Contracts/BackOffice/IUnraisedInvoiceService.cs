using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNaati.Contracts.BackOffice
{
    public interface IUnraisedInvoiceService
    {
        GetUnraisedInvoiceSectionsResponse GetUnraisedInvoiceSections(int credentialApplicationId);
    }

    public class GetUnraisedInvoiceSectionsResponse
    {
        public IEnumerable<UnraisedInvoicesSectionContract> Results { get; set; }
    }

    public class UnraisedInvoicesSectionContract
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public string Description { get; set; }

        public IEnumerable<UnraisedInvoicesQuestionContract> Questions { get; set; }

        public bool HasTokens { get; set; }
    }

    public class UnraisedInvoicesQuestionContract
    {

        public int Id { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public int AnswerTypeId { get; set; }

        public string AnswerTypeName { get; set; }

        public int? CredentialApplicationFieldId { get; set; }

        public IEnumerable<AnswerOptionContract> AnswerOptions { get; set; }

        public IEnumerable<QuestionLogicContract> QuestionLogics { get; set; }

        public IList<object> Responses { get; set; }

        public object Response { get; set; }

        public bool HasTokens { get; set; }
    }


}
