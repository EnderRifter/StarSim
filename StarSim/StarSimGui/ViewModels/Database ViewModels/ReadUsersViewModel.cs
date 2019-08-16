using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using StarSimLib.Contexts;
using StarSimLib.Models;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the read users view.
    /// </summary>
    public class ReadUsersViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="PublishedSystems"/> property.
        /// </summary>
        private IObservableCollection<PublishedSystem> publishedSystemsObservable;

        /// <summary>
        /// Backing field for the <see cref="Users"/> property.
        /// </summary>
        private IObservableCollection<User> usersObservable;

        /// <summary>
        /// Backing field for the <see cref="SelectedPublishedSystem"/> property.
        /// </summary>
        public PublishedSystem selectedPublishedSystem;

        /// <summary>
        /// Backing field for the <see cref="SelectedUser"/> property.
        /// </summary>
        public User selectedUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="ReadUsersViewModel"/> class.
        /// </summary>
        public ReadUsersViewModel()
        {
            publishedSystemsObservable = new ObservableCollectionExtended<PublishedSystem>();

            usersObservable = new ObservableCollectionExtended<User>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ReadUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public ReadUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            PublishedSystems.Load(dbContext.PublishedSystems.Include(system => system.System));

            Users.Load(dbContext.Users.Include(user => user.PublishedSystems).ThenInclude(system => system.System));
        }

        /// <summary>
        /// Whether the currently selected published system is null.
        /// </summary>
        public bool IsSelectedPublishedSystemNull
        {
            get { return SelectedPublishedSystem == null; }
        }

        /// <summary>
        /// Whether the currently selected user is null.
        /// </summary>
        public bool IsSelectedUserNull
        {
            get { return SelectedUser == null; }
        }

        /// <summary>
        /// The published systems held in the database.
        /// </summary>
        public IObservableCollection<PublishedSystem> PublishedSystems
        {
            get
            {
                return publishedSystemsObservable;
            }
            set
            {
                publishedSystemsObservable = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The currently selected published system from the <see cref="PublishedSystems"/> collection.
        /// </summary>
        public PublishedSystem SelectedPublishedSystem
        {
            get
            {
                return selectedPublishedSystem;
            }
            set
            {
                selectedPublishedSystem = value;
                this.RaisePropertyChanged();

                this.RaisePropertyChanged(nameof(IsSelectedPublishedSystemNull));
            }
        }

        /// <summary>
        /// The currently selected user from the <see cref="Users"/> collection.
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
                this.RaisePropertyChanged();

                this.RaisePropertyChanged(nameof(IsSelectedUserNull));
            }
        }

        /// <summary>
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<User> Users
        {
            get
            {
                return usersObservable;
            }
            set
            {
                usersObservable = value;
                this.RaisePropertyChanged();
            }
        }
    }
}