using DynamicData.Binding;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using StarSimLib.Contexts;
using StarSimLib.Models;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the system read view.
    /// </summary>
    public class SystemReadViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="Bodies"/> property.
        /// </summary>
        private IObservableCollection<Body> bodiesObservable;

        /// <summary>
        /// Backing field for the <see cref="Systems"/> property.
        /// </summary>
        private IObservableCollection<StarSimLib.Models.System> systemsObservable;

        /// <summary>
        /// Backing field for the <see cref="SelectedBody"/> property.
        /// </summary>
        public Body selectedBody;

        /// <summary>
        /// Backing field for the <see cref="SelectedSystem"/> property.
        /// </summary>
        public StarSimLib.Models.System selectedSystem;

        /// <summary>
        /// Initialises a new instance of the <see cref="SystemReadViewModel"/> class.
        /// </summary>
        public SystemReadViewModel()
        {
            bodiesObservable = new ObservableCollectionExtended<Body>();

            systemsObservable = new ObservableCollectionExtended<StarSimLib.Models.System>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SystemReadViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public SystemReadViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            Bodies.Load(dbContext.Bodies.Include(body => body.BodyToSystemJoins).ThenInclude(join => join.System));

            Systems.Load(dbContext.Systems.Include(system => system.BodyToSystemJoins).ThenInclude(join => join.Body));
        }

        /// <summary>
        /// The bodies held in the database.
        /// </summary>
        public IObservableCollection<Body> Bodies
        {
            get
            {
                return bodiesObservable;
            }
            set
            {
                bodiesObservable = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Whether the currently selected body is null.
        /// </summary>
        public bool IsSelectedBodyNull
        {
            get { return SelectedBody == null; }
        }

        /// <summary>
        /// Whether the currently selected system is null.
        /// </summary>
        public bool IsSelectedSystemNull
        {
            get { return SelectedSystem == null; }
        }

        /// <summary>
        /// The currently selected body from the <see cref="Bodies"/> collection.
        /// </summary>
        public Body SelectedBody
        {
            get
            {
                return selectedBody;
            }
            set
            {
                selectedBody = value;
                this.RaisePropertyChanged();

                this.RaisePropertyChanged(nameof(IsSelectedBodyNull));
            }
        }

        /// <summary>
        /// The currently selected system from the <see cref="Systems"/> collection.
        /// </summary>
        public StarSimLib.Models.System SelectedSystem
        {
            get
            {
                return selectedSystem;
            }
            set
            {
                selectedSystem = value;
                this.RaisePropertyChanged();

                this.RaisePropertyChanged(nameof(IsSelectedSystemNull));
            }
        }

        /// <summary>
        /// The systems held in the database.
        /// </summary>
        public IObservableCollection<StarSimLib.Models.System> Systems
        {
            get
            {
                return systemsObservable;
            }
            set
            {
                systemsObservable = value;
                this.RaisePropertyChanged();
            }
        }
    }
}