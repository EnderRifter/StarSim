using System;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using StarSimGui.Source;
using StarSimLib.Contexts;
using StarSimLib.Cryptography;
using StarSimLib.Models;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the user login view.
    /// </summary>
    public class UserLoginViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="IsLoggedIn"/> property.
        /// </summary>
        private bool isLoggedIn;

        /// <summary>
        /// Backing field for the <see cref="LoginFeedback"/> property.
        /// </summary>
        private LoginAttemptResult loginFeedback;

        /// <summary>
        /// Backing field for the <see cref="Password"/> property.
        /// </summary>
        private string password;

        /// <summary>
        /// Backing field for the <see cref="Username"/> property.
        /// </summary>
        private string username;

        /// <summary>
        /// Initialises a new instance of the<see cref="UserLoginViewModel"/> class.
        /// </summary>
        public UserLoginViewModel() : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UserLoginViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public UserLoginViewModel(SimulatorContext context)
        {
            dbContext = context;

            IObservable<bool> canLogin = this.WhenAnyValue(x => x.Username, x => x.Password, x => x.IsLoggedIn,
                (username, password, isLoggedIn) =>
                    !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !isLoggedIn);

            OnLoginCommand = ReactiveCommand.Create(OnLoginImpl, canLogin);

            IObservable<bool> canLogout = this.WhenAnyValue(x => x.IsLoggedIn);

            OnLogoutCommand = ReactiveCommand.Create(OnLogoutImpl, canLogout);
        }

        /// <summary>
        /// Whether the user is logged into the service.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }
            set
            {
                isLoggedIn = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Feedback about the last login attempt.
        /// </summary>
        public LoginAttemptResult LoginFeedback
        {
            get
            {
                return loginFeedback;
            }

            set
            {
                loginFeedback = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user tries to login.
        /// </summary>
        public ICommand OnLoginCommand { get; }

        /// <summary>
        /// Command invoked whenever the user tries to logout.
        /// </summary>
        public ICommand OnLogoutCommand { get; }

        /// <summary>
        /// The password to attempt to login with.
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The username to attempt to login with.
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Invoked whenever the user attempts to login with a <see cref="Username"/> and <see cref="Password"/>.
        /// </summary>
        private void OnLoginImpl()
        {
            try
            {
                // we convert the password to a byte array, so that it may be hashed
                byte[] passwordAsBytes = CryptographyHelper.StringToBytes(Password);

                // we attempt to find a user with the requested username, to check the given password against
                User tempUser = dbContext.Users.FirstOrDefault(user => user.Username == Username);

                // if no user with the given username was found, we skip any further processing and inform the user
                if (tempUser == null)
                {
                    LoginFeedback = new LoginAttemptResult(LoginResult.BadUsername, "User with the given username does not exist");
                    return;
                }

                // a user with the given username exists, so we get their password salt and compare the password hashes
                byte[] userSalt = tempUser.PasswordSalt;
                byte[] givenPasswordHash = CryptographyHelper.GenerateHash(passwordAsBytes, userSalt);

                // if the passwords do not match, we skip any further processing and inform the user
                if (!CryptographyHelper.HashesEqual(givenPasswordHash, tempUser.PasswordHash))
                {
                    LoginFeedback = new LoginAttemptResult(LoginResult.BadPassword, "Given password is invalid");
                    return;
                }

                // if both the given username and password match, the user is logged in
                IsLoggedIn = true;
                LoginFeedback = new LoginAttemptResult(LoginResult.Success, "Logged in successfully.");
            }
            catch (Exception)
            {
                LoginFeedback = new LoginAttemptResult(LoginResult.Error, "Unexpected error occurred");
            }
        }

        /// <summary>
        /// Invoked whenever the user attempts to log out.
        /// </summary>
        private void OnLogoutImpl()
        {
            IsLoggedIn = false;
            LoginFeedback = new LoginAttemptResult();
        }
    }
}