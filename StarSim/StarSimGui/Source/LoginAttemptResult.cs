using System;

namespace StarSimGui.Source
{
    /// <summary>
    /// Enumerates the possible results of a login attempt.
    /// </summary>
    [Flags]
    public enum LoginResult : byte
    {
        /// <summary>
        /// The user logged in successfully.
        /// </summary>
        Success = 1 << 0,

        /// <summary>
        /// There was an unexpected error, and the user was not logged in.
        /// </summary>
        Error = 1 << 1,

        /// <summary>
        /// The user did not give a valid username, and was not logged in.
        /// </summary>
        BadUsername = Error | 1 << 2,

        /// <summary>
        /// The user did not give a valid password, and was not logged in.
        /// </summary>
        BadPassword = Error | 1 << 3,
    }

    /// <summary>
    /// Represents the result of a login attempt, and holds data about the error (if any) encountered during the login.
    /// </summary>
    public struct LoginAttemptResult
    {
        /// <summary>
        /// The result of the attempt.
        /// </summary>
        public LoginResult Result;

        /// <summary>
        /// Initialises a new instance of the <see cref="LoginAttemptResult"/> struct.
        /// </summary>
        /// <param name="result">The <see cref="LoginResult"/> of this attempt.</param>
        /// <param name="message">The message about the error that occured, if any.</param>
        public LoginAttemptResult(LoginResult result, string message = null)
        {
            Result = result;
            Message = message;
        }

        /// <summary>
        /// Whether this attempt resulted in an error.
        /// </summary>
        public bool IsError
        {
            get { return (Result & LoginResult.Error) == LoginResult.Error; }
        }

        /// <summary>
        /// Whether this attempt resulted in a success.
        /// </summary>
        public bool IsSuccess
        {
            get { return (Result & LoginResult.Success) == LoginResult.Success; }
        }

        /// <summary>
        /// A description of the error that occured, if any.
        /// </summary>
        public string Message { get; set; }
    }
}