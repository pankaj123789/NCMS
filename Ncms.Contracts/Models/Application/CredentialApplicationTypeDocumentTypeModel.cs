using Ncms.Contracts.Models.File;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationTypeDocumentTypeModel
    {
        public DocumentTypeModel DocumentType { get; set; }

        public bool Mandatory { get; set; }
    }
}