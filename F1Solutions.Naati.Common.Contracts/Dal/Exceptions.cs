using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class SamException : Exception
    {
        public SamException(string message)
            : base(message) { }
        public SamException(string message, Exception ex)
            : base(message, ex) { }

        public string LogComment { get; set; }
    }

    // use this for exceptions with a message that is suitable to be seen by users
    public class UserFriendlySamException : SamException
    {
        /// <summary>
        /// The message will be seen by the user. Nothing will be logged.
        /// </summary>
        /// <param name="message">Error message to show the user. Ensure this is a user-friendly message.</param>
        public UserFriendlySamException(string message)
            : base(message) { }

        /// <summary>
        /// The message will be seen by the user. The InnerException will be logged.
        /// </summary>
        /// <param name="message">Error message to show the user. Ensure this is a user-friendly message.</param>
        /// <param name="ex">Exception to be logged.</param>
        public UserFriendlySamException(string message, Exception ex)
            : base(message, new HandledException(message, ex.Message, ex.StackTrace, "NCMS")) { }
    }

    public class WiiseRateExceededException : UserFriendlySamException
    {
        public WiiseRateExceededException(string message) : base(message)
        {
        }

        public WiiseRateExceededException(string message, Exception ex) : base(message, ex)
        {
        }
    }

    /// <summary>
    /// This exists for logging purposes. 
    /// </summary>
    public class HandledException : SamException
    {
        private readonly string _message;
        public HandledException(string message, string stacktrace, string source)
            : base(message)
        {
            _message = message;
            StackTrace = stacktrace;
            Source = source;
            LogComment = "This exception was handled but logged for potential debugging/support purposes.";
        }

        public HandledException(string userMessage, string errorMessage, string stacktrace, string source)
            : this(errorMessage, stacktrace, source)
        {
            LogComment = $"Message shown to user: \"{userMessage}\"";
        }

        public override string StackTrace { get; }

        public override string Message => $"A handled error occurred in {Source}: {_message}";

        public override string ToString()
        {
            return Message + Environment.NewLine + StackTrace;
        }
    }
}
