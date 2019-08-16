using StarSimLib.Contexts;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the delete systems view.
    /// </summary>
    public class DeleteSystemsViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteSystemsViewModel"/> class.
        /// </summary>
        public DeleteSystemsViewModel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteSystemsViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DeleteSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }
    }
}