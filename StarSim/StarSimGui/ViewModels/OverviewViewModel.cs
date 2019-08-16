using System.Linq;
using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using StarSimLib.Contexts;
using StarSimLib.Models;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the overview.
    /// </summary>
    public class OverviewViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// The backing field for the <see cref="CurrentUser"/> property.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// The backing field for the <see cref="PublishedSystems"/> property.
        /// </summary>
        private IObservableCollection<PublishedSystem> publishedSystemsObservable;

        /// <summary>
        /// The backing field for the <see cref="SelectedPublishedSystem"/> property.
        /// </summary>
        private PublishedSystem selectedPublishedSystem;

        /// <summary>
        /// Initialises a new instance of the <see cref="OverviewViewModel"/> class.
        /// </summary>
        public OverviewViewModel()
        {
            publishedSystemsObservable = new ObservableCollectionExtended<PublishedSystem>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="OverviewViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public OverviewViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            PublishedSystems.Load(dbContext.PublishedSystems.Include(system => system.System));
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
                this.RaisePropertyChanged(nameof(IsSelectedPublishedSystemNull));
            }
        }

        /// <summary>
        /// Whether the currently selected published system is null.
        /// </summary>
        public bool IsSelectedPublishedSystemNull
        {
            get { return SelectedPublishedSystem == null; }
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
        /// Handles the <see cref="UserLoginViewModel.LoggedIn"/> event.
        /// </summary>
        /// <param name="newUser">The user which logged in.</param>
        public void HandleLogin(User newUser)
        {
            CurrentUser = newUser;

            IQueryable<PublishedSystem> usersPublishedSystems =
                dbContext.PublishedSystems.Where(system => system.Publisher.Id == CurrentUser.Id);

            PublishedSystems.Load(usersPublishedSystems);

            SelectedPublishedSystem = null;
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedOut"/> event.
        /// </summary>
        public void HandleLogout()
        {
            CurrentUser = null;

            PublishedSystems.Clear();
        }
    }
}