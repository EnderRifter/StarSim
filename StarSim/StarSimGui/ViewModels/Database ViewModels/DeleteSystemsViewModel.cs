using System;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using StarSimLib.Contexts;
using StarSimLib.Models;

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
        /// Backing field for the <see cref="Bodies"/> property.
        /// </summary>
        private IObservableCollection<Body> bodies;

        /// <summary>
        /// Backing field for the <see cref="Bodies"/> property.
        /// </summary>
        private IObservableCollection<PublishedSystem> publishedSystems;

        /// <summary>
        /// Backing field for the <see cref="SelectedBody"/> property.
        /// </summary>
        private Body selectedBody;

        /// <summary>
        /// Backing field for the <see cref="SelectedPublishedSystem"/> property.
        /// </summary>
        private PublishedSystem selectedPublishedSystem;

        /// <summary>
        /// Backing field for the <see cref="SelectedSystem"/> property.
        /// </summary>
        private StarSimLib.Models.System selectedSystem;

        /// <summary>
        /// Backing field for the <see cref="Bodies"/> property.
        /// </summary>
        private IObservableCollection<StarSimLib.Models.System> systems;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        public DeleteSystemsViewModel()
        {
            bodies = new ObservableCollectionExtended<Body>();

            systems = new ObservableCollectionExtended<StarSimLib.Models.System>();

            publishedSystems = new ObservableCollectionExtended<PublishedSystem>();

            IObservable<bool> canRemoveBody = this.WhenAnyValue(x => x.SelectedBody, selector: user => user != null);

            RemoveBodyCommand = ReactiveCommand.Create(RemoveBodyCommandImpl, canRemoveBody);

            IObservable<bool> canRemoveSystem =
                this.WhenAnyValue(x => x.SelectedSystem, selector: system => system != null);

            RemoveSystemCommand = ReactiveCommand.Create(RemoveSystemCommandImpl, canRemoveSystem);

            IObservable<bool> canRemovePublishedSystem = this.WhenAnyValue(x => x.SelectedPublishedSystem,
                selector: publishedSystem => publishedSystem != null);

            RemovePublishedSystemCommand = ReactiveCommand.Create(RemovePublishedSystemCommandImpl, canRemovePublishedSystem);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DeleteSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            Bodies.Load(dbContext.Bodies);

            Systems.Load(dbContext.Systems);

            PublishedSystems.Load(dbContext.PublishedSystems);
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<Body> Bodies
        {
            get
            {
                return bodies;
            }
            set
            {
                bodies = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Provides feedback about the changes made to the body database.
        /// </summary>
        public string BodyFeedback { get; private set; }

        /// <summary>
        /// Provides feedback about the changes made to the published system database.
        /// </summary>
        public string PublishedSystemFeedback { get; private set; }

        /// <summary>
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<PublishedSystem> PublishedSystems
        {
            get
            {
                return publishedSystems;
            }
            set
            {
                publishedSystems = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user wants to remove the currently selected body from the database.
        /// </summary>
        public ICommand RemoveBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to remove the currently selected published system from the database.
        /// </summary>
        public ICommand RemovePublishedSystemCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to remove the currently selected system from the database.
        /// </summary>
        public ICommand RemoveSystemCommand { get; }

        /// <summary>
        /// The currently selected body.
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
            }
        }

        /// <summary>
        /// The currently selected published system.
        /// </summary>
        public PublishedSystem SelectedPublishedSystem
        {
            get
            {
                return selectedPublishedSystem;
            }
            set
            {
                selectedPublishedSystem = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The currently selected system.
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
            }
        }

        /// <summary>
        /// Provides feedback about the changes made to the system database.
        /// </summary>
        public string SystemFeedback { get; private set; }

        /// <summary>
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<StarSimLib.Models.System> Systems
        {
            get
            {
                return systems;
            }
            set
            {
                systems = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Invokes the <see cref="DatabaseEdited"/> event.
        /// </summary>
        private void OnDatabaseEdited()
        {
            DatabaseEdited?.Invoke();
        }

        /// <summary>
        /// Invoked whenever the user wants to remove the currently selected body from the database.
        /// </summary>
        private void RemoveBodyCommandImpl()
        {
            try
            {
                dbContext.Bodies.Remove(SelectedBody);

                BodyFeedback = "Body was successfully removed from the database.";
                this.RaisePropertyChanged(nameof(BodyFeedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                BodyFeedback = "Body could not be removed from the database.";
                this.RaisePropertyChanged(nameof(BodyFeedback));
            }
        }

        /// <summary>
        /// Invoked whenever the user wants to remove the currently selected published system from the database.
        /// </summary>
        private void RemovePublishedSystemCommandImpl()
        {
            try
            {
                dbContext.PublishedSystems.Remove(SelectedPublishedSystem);

                PublishedSystemFeedback = "Published system was successfully removed from the database.";
                this.RaisePropertyChanged(nameof(PublishedSystemFeedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                PublishedSystemFeedback = "Published system could not be removed from the database.";
                this.RaisePropertyChanged(nameof(PublishedSystemFeedback));
            }
        }

        /// <summary>
        /// Invoked whenever the user wants to remove the currently selected system from the database.
        /// </summary>
        private void RemoveSystemCommandImpl()
        {
            try
            {
                dbContext.Systems.Remove(SelectedSystem);

                SystemFeedback = "System was successfully removed from the database.";
                this.RaisePropertyChanged(nameof(SystemFeedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                SystemFeedback = "System could not be removed from the database.";
                this.RaisePropertyChanged(nameof(SystemFeedback));
            }
        }

        /// <summary>
        /// Refreshed the database sources for this view model.
        /// </summary>
        internal void HandleDatabaseRefresh()
        {
            Bodies.Load(dbContext.Bodies);
            Systems.Load(dbContext.Systems);
            PublishedSystems.Load(dbContext.PublishedSystems);
        }
    }
}