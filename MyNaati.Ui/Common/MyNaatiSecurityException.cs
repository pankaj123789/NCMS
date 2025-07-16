using System;

namespace MyNaati.Ui.Common
{
    public class MyNaatiSecurityException : Exception
    {
        /// <summary>
        /// The message will be seen by the user. Nothing will be logged.
        /// </summary>
        private new const string Message = "User is not authorised to access this item.";

        public MyNaatiSecurityException()

            : base(Message)
        {

        }
    
    }
}