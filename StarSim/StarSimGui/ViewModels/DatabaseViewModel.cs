using ReactiveUI;
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
        /// The currently logged in user.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        public DatabaseViewModel() : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DatabaseViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DatabaseViewModel(SimulatorContext context)
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
        /// Handles the <see cref="UserLoginViewModel.LoggedIn"/> event.
        /// </summary>
        /// <param name="newUser">The user which logged in.</param>
        public void HandleLogin(User newUser)
        {
            CurrentUser = newUser;
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedOut"/> event.
        /// </summary>
        public void HandleLogout()
        {
            CurrentUser = null;
        }
    }
}