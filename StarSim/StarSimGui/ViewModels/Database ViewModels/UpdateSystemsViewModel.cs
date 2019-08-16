using StarSimLib.Contexts;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the update systems view.
    /// </summary>
    public class UpdateSystemsViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateSystemsViewModel"/> class.
        /// </summary>
        public UpdateSystemsViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateSystemsViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public UpdateSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}