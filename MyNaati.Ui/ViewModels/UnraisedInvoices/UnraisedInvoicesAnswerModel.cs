using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace MyNaati.Ui.ViewModels.UnraisedInvoices
{
    public class UnraisedInvoicesAnswerModel
    {
        public int Id { get; set; }

        public int FormAnswerOptionId { get; set; }

        public bool DefaultAnswer { get; set; }

        public int? CredentialApplicationFieldId { get; set; }
     
        public int DisplayOrder { get; set; }

        public string Name { get; set; }

        public bool MandatoryAttachments { get; set; }

        [LegalCharacters('\n', '\r', '\t')]
        public string Description { get; set; }

        public int? FieldOptionId { get; set; }

        public UnraisedInvoicesAnswerActionModel Function { get; set; }

        public IEnumerable<UnraisedInvoicesAnswerDocumentModel> Documents { get; set; }

        public string FieldData { get; set; }

        public bool HasTokens { get; set; }
    }

    public class UnraisedInvoicesCredentialAnswerModel 
    {
        public int CredentialTypeId { get; set; }
        public int Id { get; set; }

        public int FormAnswerOptionId { get; set; }

        public bool DefaultAnswer { get; set; }

        public int? CredentialApplicationFieldId { get; set; }

        public int DisplayOrder { get; set; }

        public string Name { get; set; }

        public bool MandatoryAttachments { get; set; }

        [LegalCharacters('\n', '\r', '\t')]
        public string Description { get; set; }

        public int? FieldOptionId { get; set; }

        public UnraisedInvoicesAnswerActionModel Function { get; set; }

        public IEnumerable<UnraisedInvoicesAnswerDocumentModel> Documents { get; set; }

        public string FieldData { get; set; }

        public bool HasTokens { get; set; }
    }
}