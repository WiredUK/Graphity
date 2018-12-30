namespace Graphity.Authorisation
{
    /// <summary>
    /// The result of a custom authorisation requirement.
    /// </summary>
    public class AuthorisationResult
    {
        /// <summary>
        /// Gets a successful authorisation result.
        /// </summary>
        /// <returns></returns>
        public static AuthorisationResult Success() => new AuthorisationResult(true, string.Empty);

        /// <summary>
        /// Gets a failed authorisation result.
        /// </summary>
        /// <returns></returns>
        public static AuthorisationResult Fail(string errorMessage) => new AuthorisationResult(false, errorMessage);

        private AuthorisationResult(bool success, string errorMessage)
        {
            Successful = success;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Was authorisation successful
        /// </summary>
        public bool Successful { get;  }

        /// <summary>
        /// If authorisation was not successful, this is the error message.
        /// </summary>
        public string ErrorMessage { get; }
    }
}