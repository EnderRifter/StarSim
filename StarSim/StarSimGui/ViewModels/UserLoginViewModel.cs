using ReactiveUI;

using StarSimGui.Source;

using StarSimLib.Contexts;
using StarSimLib.Cryptography;
using StarSimLib.Models;

using System;
using System.Linq;
using System.Windows.Input;

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

            LoginCommand = ReactiveCommand.Create(LoginCommandImpl, canLogin);

            IObservable<bool> canLogout = this.WhenAnyValue(x => x.IsLoggedIn);

            LogoutCommand = ReactiveCommand.Create(LogoutCommandImpl, canLogout);
        }

        /// <summary>
        /// Invoked whenever a successful login is made.
        /// </summary>
        public event Action<User> LoggedIn;

        /// <summary>
        /// Invoked whenever the currently logged in user logs out.
        /// </summary>
        public event Action LoggedOut;

        /// <summary>
        /// Whether the user is logged into the service.
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return isLoggedIn;
            }

            private set
            {
                isLoggedIn = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user tries to login.
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Feedback about the last login attempt.
        /// </summary>
        public LoginAttemptResult LoginFeedback
        {
            get
            {
                return loginFeedback;
            }

            private set
            {
                loginFeedback = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user tries to logout.
        /// </summary>
        public ICommand LogoutCommand { get; }

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
        private void LoginCommandImpl()
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
                LoginFeedback = new LoginAttemptResult(LoginResult.Success, "Logged in successfully");
                OnLoggedIn(tempUser);
            }
            catch (Exception)
            {
                // if any exception occurs during the login attempt, inform the user
                LoginFeedback = new LoginAttemptResult(LoginResult.Error, "Unexpected error occurred. Please try again later");
            }
        }

        /// <summary>
        /// Invoked whenever the user attempts to log out.
        /// </summary>
        private void LogoutCommandImpl()
        {
            IsLoggedIn = false;
            LoginFeedback = new LoginAttemptResult();
            OnLoggedOut();
        }

        /// <summary>
        /// Invokes the <see cref="LoggedIn"/> event.
        /// </summary>
        /// <param name="user">The <see cref="User"/> who successfully logged in.</param>
        protected void OnLoggedIn(User user)
        {
            LoggedIn?.Invoke(user);
        }

        /// <summary>
        /// Invokes the <see cref="LoggedOut"/> event.
        /// </summary>
        protected void OnLoggedOut()
        {
            LoggedOut?.Invoke();
        }

#if DEBUG

        /// <summary>
        /// Simulates a user logging in irrespective of the view.
        /// </summary>
        /// <param name="debugUser">The debug user to substitute a real user.</param>
        public void DebugSimulateLogin(User debugUser)
        {
            Username = debugUser.Username;
            Password = debugUser.Username;

            IsLoggedIn = true;
            LoginFeedback = new LoginAttemptResult(LoginResult.Success, "Successful login");
            OnLoggedIn(debugUser);
        }

#endif
    }
}