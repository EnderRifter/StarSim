using System;
using ReactiveUI;
using StarSimLib.Configuration;
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
        /// The database context in which the program should store data and which it should query to fetch data.
        /// </summary>
        private readonly SimulatorContext simulatorContext;

        /// <summary>
        /// Backing field for the <see cref="CurrentUser"/> property.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            simulatorContext = new SimulatorContext();

            simulatorContext.ChangeTracker.AutoDetectChangesEnabled = true;

            OverviewViewModel = new OverviewViewModel(in simulatorContext);

            UserLoginViewModel = new UserLoginViewModel(in simulatorContext);

            UserLoginViewModel.LoggedIn += user => CurrentUser = user;
            UserLoginViewModel.LoggedIn += OverviewViewModel.HandleLogin;

            UserLoginViewModel.LoggedOut += () => CurrentUser = null;
            UserLoginViewModel.LoggedOut += OverviewViewModel.HandleLogout;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="configuration">The current configuration of the application.</param>
        public MainWindowViewModel(Config configuration) : this()
        {
            DatabaseViewModel = new DatabaseViewModel(in simulatorContext, in configuration);

            SimulationViewModel = new SimulationViewModel(in simulatorContext, in configuration);

            UserLoginViewModel.LoggedIn += DatabaseViewModel.HandleLogin;
            UserLoginViewModel.LoggedIn += SimulationViewModel.HandleLogin;

            UserLoginViewModel.LoggedOut += DatabaseViewModel.HandleLogout;
            UserLoginViewModel.LoggedOut += SimulationViewModel.HandleLogout;

            SimulationViewModel.DatabaseUpdated += HandleDatabaseUpdated;
            DatabaseViewModel.DatabaseUpdated += HandleDatabaseUpdated;

#if DEBUG
            // simulate a login to accelerate development of the application, as the implemented cryptographic features
            // of the login system make it slow to solve for the password hash. furthermore loading the database takes
            // a relatively long time as well - this wastes development time
            UserLoginViewModel.DebugSimulateLogin(new User(0, "Debug User", UserPrivileges.Admin, "debug@application.net"));
#endif
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

        /// <summary>
        /// Handles the DatabaseEdited event.
        /// </summary>
        private void HandleDatabaseUpdated()
        {
            Console.WriteLine($"Database has tracked changes?: {simulatorContext.ChangeTracker.HasChanges()}");
            Console.WriteLine($"Saved {simulatorContext.SaveChanges()} changed entities");
            DatabaseViewModel.RefreshDBSources();
        }
    }
}