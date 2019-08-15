using System.Linq;
using DynamicData.Binding;
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
        private IObservableCollection<PublishedSystem> publishedSystems;

        /// <summary>
        /// Initialises a new instance of the <see cref="OverviewViewModel"/> class.
        /// </summary>
        public OverviewViewModel()
        {
            publishedSystems = new ObservableCollectionExtended<PublishedSystem>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="OverviewViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public OverviewViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
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
            }
        }

        /// <summary>
        /// The <see cref="PublishedSystem"/> instance that the current user has published.
        /// </summary>
        public IObservableCollection<PublishedSystem> PublishedSystems
        {
            get
            {
                return publishedSystems;
            }
            set
            {
                publishedSystems = value;
                this.RaisePropertyChanged();
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