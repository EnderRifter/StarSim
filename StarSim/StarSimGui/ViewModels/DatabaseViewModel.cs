using System.Collections.Generic;
using System.Linq;
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using StarSimGui.ViewModels.Database_ViewModels;
using StarSimLib.Contexts;
using StarSimLib.Models;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the database view.
    /// </summary>
    public class DatabaseViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Represents the system read view.
        /// </summary>
        private readonly SystemReadViewModel systemReadViewModel;

        /// <summary>
        /// Represents the user read view.
        /// </summary>
        private readonly UserReadViewModel userReadViewModel;

        /// <summary>
        /// The currently logged in user.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        public DatabaseViewModel()
        {
            userReadViewModel = new UserReadViewModel();

            systemReadViewModel = new SystemReadViewModel();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DatabaseViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            userReadViewModel = new UserReadViewModel(in context);

            systemReadViewModel = new SystemReadViewModel(in context);
        }

        /// <summary>
        /// The currently logged in user.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
                this.RaisePropertyChanged();

                /*
                if (value != null)
                {
                    PublishedSystems.Load(dbContext.PublishedSystems.Where(system => system.PublisherId == value.Id));
                }
                */
            }
        }

        /// <summary>
        /// Whether the currently logged in user has administrator privileges.
        /// </summary>
        public bool IsAdmin
        {
            get { return (currentUser.Privileges & UserPrivileges.Admin) == UserPrivileges.Admin; }
        }

        /// <summary>
        /// Whether the currently logged in user had the default privileges.
        /// </summary>
        public bool IsDefault
        {
            get { return (currentUser.Privileges & UserPrivileges.Default) == UserPrivileges.Default; }
        }

        /// <summary>
        /// Whether the currently logged in user has publisher privileges.
        /// </summary>
        public bool IsPublisher
        {
            get { return (currentUser.Privileges & UserPrivileges.Publisher) == UserPrivileges.Publisher; }
        }

        /// <summary>
        /// Exposes the <see cref="systemReadViewModel"/> field to the view.
        /// </summary>
        public SystemReadViewModel SystemReadViewModel
        {
            get { return systemReadViewModel; }
        }

        /// <summary>
        /// Exposes the <see cref="userReadViewModel"/> field to the view.
        /// </summary>
        public UserReadViewModel UserReadViewModel
        {
            get { return userReadViewModel; }
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedIn"/> event.
        /// </summary>
        /// <param name="newUser">The user which logged in.</param>
        public void HandleLogin(User newUser)
        {
            CurrentUser = newUser;

            this.RaisePropertyChanged(nameof(IsAdmin));
            this.RaisePropertyChanged(nameof(IsDefault));
            this.RaisePropertyChanged(nameof(IsPublisher));
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedOut"/> event.
        /// </summary>
        public void HandleLogout()
        {
            CurrentUser = null;

            this.RaisePropertyChanged(nameof(IsAdmin));
            this.RaisePropertyChanged(nameof(IsDefault));
            this.RaisePropertyChanged(nameof(IsPublisher));
        }
    }
}