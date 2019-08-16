using StarSimLib.Contexts;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the create systems view.
    /// </summary>
    public class CreateSystemsViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateSystemsViewModel"/> class.
        /// </summary>
        public CreateSystemsViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateSystemsViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public CreateSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}