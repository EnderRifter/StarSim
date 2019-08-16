using StarSimLib.Contexts;

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
        /// Initialises a new instance of the <see cref="UpdateUsersViewModel"/> class.
        /// </summary>
        public UpdateUsersViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public UpdateUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}