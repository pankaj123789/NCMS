using F1Solutions.Naati.Common.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.FileDeletion
{
    public class FileDeletionException : Exception
    {
        public FileDeletionException()
        {

        }

        public FileDeletionException(string message)
            : base(message)
        {

        }
         
        public FileDeletionException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
