using System;
using System.Collections.Generic;
using System.Text;
using StarSimLib.Contexts;

namespace StarSimGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SimulatorContext = new SimulatorContext();

            DatabaseViewModel = new DatabaseViewModel();

            OverviewViewModel = new OverviewViewModel();

            SimulationViewModel = new SimulationViewModel();

            UserLoginViewModel = new UserLoginViewModel(SimulatorContext);
        }

        /// <summary>
        /// The database context in which the program should store data and which it should query to fetch data.
        /// </summary>
        private SimulatorContext SimulatorContext { get; }

        public DatabaseViewModel DatabaseViewModel { get; set; }
        public OverviewViewModel OverviewViewModel { get; set; }
        public SimulationViewModel SimulationViewModel { get; set; }
        public UserLoginViewModel UserLoginViewModel { get; set; }
    }
}