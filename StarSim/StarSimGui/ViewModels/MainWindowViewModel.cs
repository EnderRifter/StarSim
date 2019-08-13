using ReactiveUI;
using StarSimLib.Contexts;
using StarSimLib.Models;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the main view.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Backing field for the <see cref="CurrentUser"/> property.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            SimulatorContext = new SimulatorContext();

            DatabaseViewModel = new DatabaseViewModel(SimulatorContext);

            OverviewViewModel = new OverviewViewModel(SimulatorContext);

            SimulationViewModel = new SimulationViewModel(SimulatorContext);

            UserLoginViewModel = new UserLoginViewModel(SimulatorContext);

            UserLoginViewModel.LoggedIn += user => CurrentUser = user;
            UserLoginViewModel.LoggedIn += DatabaseViewModel.HandleLogin;
            UserLoginViewModel.LoggedIn += OverviewViewModel.HandleLogin;
            UserLoginViewModel.LoggedIn += SimulationViewModel.HandleLogin;

            UserLoginViewModel.LoggedOut += () => CurrentUser = null;
            UserLoginViewModel.LoggedOut += DatabaseViewModel.HandleLogout;
            UserLoginViewModel.LoggedOut += OverviewViewModel.HandleLogout;
            UserLoginViewModel.LoggedOut += SimulationViewModel.HandleLogout;

#if DEBUG
            // simulate a login to accelerate development of the application, as the implemented cryptographic features
            // of the login system make it slow to solve for the password hash. furthermore loading the database takes
            // a relatively long time as well - this wastes development time
            UserLoginViewModel.DebugSimulateLogin(new User(0, "Debug User", UserPrivileges.Admin, "debug@application.net"));
#endif
        }

        /// <summary>
        /// The database context in which the program should store data and which it should query to fetch data.
        /// </summary>
        private SimulatorContext SimulatorContext { get; }

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
        /// Represents the database view.
        /// </summary>
        public DatabaseViewModel DatabaseViewModel { get; set; }

        /// <summary>
        /// Represents the overview.
        /// </summary>
        public OverviewViewModel OverviewViewModel { get; set; }

        /// <summary>
        /// Represents the simulation view.
        /// </summary>
        public SimulationViewModel SimulationViewModel { get; set; }

        /// <summary>
        /// Represents the user login view.
        /// </summary>
        public UserLoginViewModel UserLoginViewModel { get; set; }
    }
}