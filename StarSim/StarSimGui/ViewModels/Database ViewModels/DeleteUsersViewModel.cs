using StarSimLib.Contexts;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the delete users view.
    /// </summary>
    public class DeleteUsersViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        public DeleteUsersViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DeleteUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}