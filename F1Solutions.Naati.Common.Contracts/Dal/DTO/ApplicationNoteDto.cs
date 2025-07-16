using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApplicationNoteDto
    {
        public NoteDto Note { get; set; }
        public int CredentialApplicationId { get; set; }
    }
}
