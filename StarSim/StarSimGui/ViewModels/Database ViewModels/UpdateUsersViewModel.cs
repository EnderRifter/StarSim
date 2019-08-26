using DynamicData.Binding;

using ReactiveUI;

using StarSimLib.Configuration;
using StarSimLib.Contexts;
using StarSimLib.Cryptography;
using StarSimLib.Models;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the update users view.
    /// </summary>
    public class UpdateUsersViewModel : ViewModelBase
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
        /// Backing field for the <see cref="SelectedUser"/> property.
        /// </summary>
        private User selectedUser;

        /// <summary>
        /// Backing field for the <see cref="Username"/> property.
        /// </summary>
        private string username;

        /// <summary>
        /// Backing field for the <see cref="Users"/> property.
        /// </summary>
        private IObservableCollection<User> users;

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateUsersViewModel"/> class.
        /// </summary>
        public UpdateUsersViewModel()
        {
            users = new ObservableCollectionExtended<User>();

            ResetUserCommand = ReactiveCommand.Create(ResetUserCommandImpl);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        /// <param name="configuration">The current configuration of the application.</param>
        public UpdateUsersViewModel(in SimulatorContext context, in Config configuration) : this()
        {
            dbContext = context;
            Configuration = configuration;

            #region Regex Pattern Builder

            StringBuilder regexBuilder = new StringBuilder(@"\w[^@]@(");

            for (int i = 0; i < Configuration.AcceptedEmailProviders.Length; i++)
            {
                regexBuilder.Append(i == 0
                    ? $"{Configuration.AcceptedEmailProviders[i].Replace(".", @"\.")}"
                    : $"|{Configuration.AcceptedEmailProviders[i].Replace(".", @"\.")}");
            }

            regexBuilder.Append(")$");

            #endregion Regex Pattern Builder

            Regex emailValidationRegex = new Regex(regexBuilder.ToString(), RegexOptions.Compiled);

            IObservable<bool> canUpdate = this.WhenAnyValue(x => x.SelectedUser, x => x.Username, x => x.Email, x => x.Password, x => x.Privileges,
                (user, username, email, password, privileges) =>
                {
                    if (user == null)
                    {
                        // we cannot edit a null user
                        return false;
                    }

                    if (user.Username == username && user.Email == email && user.Privileges == privileges && password.Equals(string.Empty))
                    {
                        // no changes were made, so cannot update user
                        return false;
                    }

                    // we must have a valid password and username to create a user
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) ||
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

            UpdateUserCommand = ReactiveCommand.Create(UpdateUserCommandImpl, canUpdate);

            Users.Load(dbContext.Users);
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// The current configuration of the application.
        /// </summary>
        public Config Configuration { get; internal set; }

        /// <summary>
        /// The email of the user to update.
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
        /// The password of the user to update.
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
        /// The privileges of the user to update.
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
        /// The currently selected user.
        /// </summary>
        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;

                Username = value?.Username ?? string.Empty;
                Email = value?.Email ?? string.Empty;
                Password = string.Empty;
                Privileges = value?.Privileges ?? 0;

                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user wants to update the currently selected user in the database.
        /// </summary>
        public ICommand UpdateUserCommand { get; }

        /// <summary>
        /// The username of the user to update.
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
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                this.RaisePropertyChanged();
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
            SelectedUser = null;
            Username = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            Feedback = string.Empty;
            this.RaisePropertyChanged(nameof(Feedback));
        }

        /// <summary>
        /// Invoked whenever the user wants to update the currently selected user in the database.
        /// </summary>
        private void UpdateUserCommandImpl()
        {
            if (dbContext.Users.Any(user => user.Username.Equals(Username)))
            {
                Feedback = "User with the same username already exists in the database.";
                this.RaisePropertyChanged(nameof(Feedback));
            }

            try
            {
                byte[] passwordBytes = CryptographyHelper.StringToBytes(Password);
                byte[] passwordSalt = SelectedUser.PasswordSalt;
                byte[] passwordHash = CryptographyHelper.GenerateHash(passwordBytes, passwordSalt);

                SelectedUser.Username = (Username == SelectedUser.Username) ? SelectedUser.Username : Username;
                SelectedUser.Email = (Email == SelectedUser.Email) ? SelectedUser.Email : Email;
                SelectedUser.PasswordHash = (passwordHash == SelectedUser.PasswordHash) ? SelectedUser.PasswordHash : passwordHash;
                SelectedUser.Privileges = (Privileges == SelectedUser.Privileges) ? SelectedUser.Privileges : Privileges;

                Feedback = "User was successfully updated.";
                this.RaisePropertyChanged(nameof(Feedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                Feedback = "User could not be updated.";
                this.RaisePropertyChanged(nameof(Feedback));
            }
        }

        /// <summary>
        /// Refreshed the database sources for this view model.
        /// </summary>
        internal void HandleDatabaseRefresh()
        {
            Users.Load(dbContext.Users);
        }
    }
}