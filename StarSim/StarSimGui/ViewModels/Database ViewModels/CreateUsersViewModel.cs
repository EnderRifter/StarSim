using StarSimLib.Contexts;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the create users view.
    /// </summary>
    public class CreateUsersViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateUsersViewModel"/> class.
        /// </summary>
        public CreateUsersViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public CreateUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}