using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class DocumentTypeCategory: EntityBase, IDynamicLookupType
    {
        private IList<DocumentType> mDocumentTypes = new List<DocumentType>();
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual IEnumerable<DocumentType> DocumentTypes => mDocumentTypes;
    }
}
