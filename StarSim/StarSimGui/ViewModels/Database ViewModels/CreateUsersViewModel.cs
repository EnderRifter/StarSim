using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ReactiveUI;
using StarSimLib;
using StarSimLib.Contexts;
using StarSimLib.Cryptography;
using StarSimLib.Models;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the create users view.
    /// </summary>
    public class CreateUsersViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="Email"/> property.
        /// </summary>
        private string email;

        /// <summary>
        /// Backing field for the <see cref="Password"/> property.
        /// </summary>
        private string password;

        /// <summary>
        /// Backing field for the <see cref="Privileges"/> property.
        /// </summary>
        private UserPrivileges privileges;

        /// <summary>
        /// Backing field for the <see cref="Username"/> property.
        /// </summary>
        private string username;

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateUsersViewModel"/> class.
        /// </summary>
        public CreateUsersViewModel()
        {
            privileges = UserPrivileges.Default;

            #region Regex Pattern Builder

            StringBuilder regexBuilder = new StringBuilder(@"\w[^@]@(");

            for (int i = 0; i < Constants.AcceptedEmailProviders.Length; i++)
            {
                regexBuilder.Append(i == 0
                    ? $"{Constants.AcceptedEmailProviders[i].Replace(".", @"\.")}"
                    : $"|{Constants.AcceptedEmailProviders[i].Replace(".", @"\.")}");
            }

            regexBuilder.Append(")$");

            #endregion Regex Pattern Builder

            Regex emailValidationRegex = new Regex(regexBuilder.ToString(), RegexOptions.Compiled);

            IObservable<bool> canCreate = this.WhenAnyValue(x => x.Username, x => x.Email, x => x.Password,
                (username, email, password) =>
                {
                    // we must have a valid password and username to create a user
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) &&
                        !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                    {
                        // if the optional email is given, then it must match the email validation regex
                        if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email))
                        {
                            return emailValidationRegex.IsMatch(email);
                        }

                        return true;
                    }

                    return false;
                });

            CreateUserCommand = ReactiveCommand.Create(CreateUserCommandImpl, canCreate);

            ResetUserCommand = ReactiveCommand.Create(ResetUserCommandImpl);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public CreateUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// Command invoked whenever the user wants to add the currently edited user to the database.
        /// </summary>
        public ICommand CreateUserCommand { get; }

        /// <summary>
        /// The email of the user to create.
        /// </summary>
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Provides feedback about the changes made to the database.
        /// </summary>
        public string Feedback { get; private set; }

        /// <summary>
        /// The password of the user to create.
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
        /// The possible <see cref="UserPrivileges"/> that a user can have.
        /// </summary>
        public UserPrivileges[] PossiblePrivileges
        {
            get { return new[] { UserPrivileges.Default, UserPrivileges.Publisher, UserPrivileges.Admin }; }
        }

        /// <summary>
        /// The privileges of the user to create.
        /// </summary>
        public UserPrivileges Privileges
        {
            get
            {
                return privileges;
            }
            set
            {
                privileges = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user wants to reset the currently edited user.
        /// </summary>
        public ICommand ResetUserCommand { get; }

        /// <summary>
        /// The username of the user to create.
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
        /// Invoked whenever the user wants to add the currently edited user to the database.
        /// </summary>
        private void CreateUserCommandImpl()
        {
            if (dbContext.Users.Any(user => user.Username.Equals(Username)))
            {
                Feedback = "User with the same username already exists in the database.";
                this.RaisePropertyChanged(nameof(Feedback));
            }

            try
            {
                User lastUser = dbContext.Users.OrderBy(user => user.Id).Last();

                byte[] passwordBytes = CryptographyHelper.StringToBytes(Password);
                byte[] passwordSalt = CryptographyHelper.GenerateSalt();
                byte[] passwordHash = CryptographyHelper.GenerateHash(passwordBytes, passwordSalt);

                User newUser = new User(lastUser.Id + 1, Username, Privileges, passwordHash, passwordSalt, Email);

                dbContext.Users.Add(newUser);

                Feedback = "User was successfully added to the database.";
                this.RaisePropertyChanged(nameof(Feedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                Feedback = "User could not be added to the database.";
                this.RaisePropertyChanged(nameof(Feedback));
            }
        }

        /// <summary>
        /// Invokes the <see cref="DatabaseEdited"/> event.
        /// </summary>
        private void OnDatabaseEdited()
        {
            DatabaseEdited?.Invoke();
        }

        /// <summary>
        /// Invoked whenever the user wants to reset the currently edited user.
        /// </summary>
        private void ResetUserCommandImpl()
        {
            Username = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            Feedback = string.Empty;
            this.RaisePropertyChanged(nameof(Feedback));
        }

        /// <summary>
        /// Refreshed the database sources for this view model.
        /// </summary>
        internal void HandleDatabaseRefresh()
        {
        }
    }
}