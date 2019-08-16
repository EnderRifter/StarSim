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
        /// Represents the create systems view.
        /// </summary>
        private readonly CreateSystemsViewModel createSystemsViewModel;

        /// <summary>
        /// Represents the create users view.
        /// </summary>
        private readonly CreateUsersViewModel createUsersViewModel;

        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Represents the delete systems view.
        /// </summary>
        private readonly DeleteSystemsViewModel deleteSystemsViewModel;

        /// <summary>
        /// Represents the delete users view.
        /// </summary>
        private readonly DeleteUsersViewModel deleteUsersViewModel;

        /// <summary>
        /// Represents the read systems view.
        /// </summary>
        private readonly ReadSystemsViewModel readSystemsViewModel;

        /// <summary>
        /// Represents the read users view.
        /// </summary>
        private readonly ReadUsersViewModel readUsersViewModel;

        /// <summary>
        /// Represents the update systems view.
        /// </summary>
        private readonly UpdateSystemsViewModel updateSystemsViewModel;

        /// <summary>
        /// Represents the update users view.
        /// </summary>
        private readonly UpdateUsersViewModel updateUsersViewModel;

        /// <summary>
        /// The currently logged in user.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        public DatabaseViewModel()
        {
            createSystemsViewModel = new CreateSystemsViewModel();

            createUsersViewModel = new CreateUsersViewModel();

            deleteSystemsViewModel = new DeleteSystemsViewModel();

            deleteUsersViewModel = new DeleteUsersViewModel();

            readSystemsViewModel = new ReadSystemsViewModel();

            readUsersViewModel = new ReadUsersViewModel();

            updateSystemsViewModel = new UpdateSystemsViewModel();

            updateUsersViewModel = new UpdateUsersViewModel();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DatabaseViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            createSystemsViewModel = new CreateSystemsViewModel(in context);

            createUsersViewModel = new CreateUsersViewModel(in context);

            deleteSystemsViewModel = new DeleteSystemsViewModel(in context);

            deleteUsersViewModel = new DeleteUsersViewModel(in context);

            readSystemsViewModel = new ReadSystemsViewModel(in context);

            readUsersViewModel = new ReadUsersViewModel(in context);

            updateSystemsViewModel = new UpdateSystemsViewModel(in context);

            updateUsersViewModel = new UpdateUsersViewModel(in context);
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
            get { return (currentUser?.Privileges & UserPrivileges.Admin) == UserPrivileges.Admin; }
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
        /// Exposes the <see cref="readSystemsViewModel"/> field to the view.
        /// </summary>
        public ReadSystemsViewModel ReadSystemsViewModel
        {
            get { return readSystemsViewModel; }
        }

        /// <summary>
        /// Exposes the <see cref="readUsersViewModel"/> field to the view.
        /// </summary>
        public ReadUsersViewModel ReadUsersViewModel
        {
            get { return readUsersViewModel; }
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