using DynamicData.Binding;

using ReactiveUI;

using StarSimGui.Source.Comparers;

using StarSimLib.Contexts;
using StarSimLib.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the update systems view.
    /// </summary>
    public class UpdateSystemsViewModel : ViewModelBase
    {
        /// <summary>
        /// Caches an <see cref="IEqualityComparer{T}"/> for <see cref="BodyToSystemJoin"/> instances.
        /// </summary>
        private static readonly BodyToSystemJoinComparer joinComparer = new BodyToSystemJoinComparer();

        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="Bodies"/> property.
        /// </summary>
        private IObservableCollection<Body> bodies;

        /// <summary>
        /// Backing field for the <see cref="BodyMass"/> property.
        /// </summary>
        private double bodyMass;

        /// <summary>
        /// Backing field for the <see cref="BodyName"/> property;
        /// </summary>
        private string bodyName;

        /// <summary>
        /// Backing field for the <see cref="BodyToAdd"/> property.
        /// </summary>
        private Body bodyToAdd;

        /// <summary>
        /// Backing field for the <see cref="BodyToRemove"/> property.
        /// </summary>
        private BodyToSystemJoin bodyToRemove;

        /// <summary>
        /// Backing field for the <see cref="SelectedBody"/> property.
        /// </summary>
        private Body selectedBody;

        /// <summary>
        /// Backing field for the <see cref="SelectedSystem"/> property.
        /// </summary>
        private StarSimLib.Models.System selectedSystem;

        /// <summary>
        /// Backing field for the <see cref="SelectedSystemBodies"/> property.
        /// </summary>
        private IObservableCollection<BodyToSystemJoin> selectedSystemBodies;

        /// <summary>
        /// Backing field for the <see cref="Systems"/> property.
        /// </summary>
        private IObservableCollection<StarSimLib.Models.System> systems;

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateSystemsViewModel"/> class.
        /// </summary>
        public UpdateSystemsViewModel()
        {
            bodies = new ObservableCollectionExtended<Body>();

            systems = new ObservableCollectionExtended<StarSimLib.Models.System>();

            selectedSystemBodies = new ObservableCollectionExtended<BodyToSystemJoin>();

            IObservable<bool> canUpdateBody = this.WhenAnyValue(x => x.SelectedBody, x => x.BodyName, x => x.BodyMass,
                (body, name, mass) =>
                {
                    if (body == null)
                    {
                        return false;
                    }

                    if (body.Name == name && Math.Abs(body.Mass - mass) < 1)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                    {
                        return true;
                    }

                    return false;
                });

            UpdateBodyCommand = ReactiveCommand.Create(UpdateBodyCommandImpl, canUpdateBody);

            IObservable<bool> canUpdateSystem =
                this.WhenAnyValue(x => x.SelectedSystem, x => x.SelectedSystemBodies,
                (system, bodies) =>
                {
                    if (system == null || bodies == null)
                    {
                        return false;
                    }

                    if (!system.BodyToSystemJoins.SequenceEqual(SelectedSystemBodies, joinComparer))
                    {
                        return true;
                    }

                    return false;
                });

            UpdateSystemCommand = ReactiveCommand.Create(UpdateSystemCommandImpl, canUpdateSystem);

            IObservable<bool> canAddBody = this.WhenAnyValue(x => x.SelectedSystem, x => x.BodyToAdd,
                (system, body) =>
                {
                    if (system == null || body == null)
                    {
                        return false;
                    }

                    return system.BodyToSystemJoins.All(join => join.BodyId != body.Id);
                });

            AddBodyToSystemCommand = ReactiveCommand.Create(AddBodyToSystemCommandImpl, canAddBody);

            IObservable<bool> canRemoveBody = this.WhenAnyValue(x => x.SelectedSystem, x => x.BodyToRemove,
                (system, body) =>
                {
                    if (system == null || body == null)
                    {
                        return false;
                    }

                    return true;
                });

            RemoveBodyFromSystemCommand = ReactiveCommand.Create(RemoveBodyFromSystemCommandImpl, canRemoveBody);

            ResetBodyCommand = ReactiveCommand.Create(ResetBodyCommandImpl);

            ResetSystemCommand = ReactiveCommand.Create(ResetSystemCommandImpl);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateSystemsViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public UpdateSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            Bodies.Load(dbContext.Bodies);

            Systems.Load(dbContext.Systems);
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// Command invoked whenever the user wants to add a body to the currently edited system.
        /// </summary>
        public ICommand AddBodyToSystemCommand { get; }

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
        /// The mass of the body to update.
        /// </summary>
        public double BodyMass
        {
            get
            {
                return bodyMass;
            }
            set
            {
                bodyMass = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The name of the body to update.
        /// </summary>
        public string BodyName
        {
            get
            {
                return bodyName;
            }
            set
            {
                bodyName = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The body to add to the currently edited system.
        /// </summary>
        public Body BodyToAdd
        {
            get
            {
                return bodyToAdd;
            }
            set
            {
                bodyToAdd = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The body to remove from the currently edited system.
        /// </summary>
        public BodyToSystemJoin BodyToRemove
        {
            get
            {
                return bodyToRemove;
            }
            set
            {
                bodyToRemove = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Command invoked whenever the user wants to remove a body from the currently edited system.
        /// </summary>
        public ICommand RemoveBodyFromSystemCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to deselect the currently selected body.
        /// </summary>
        public ICommand ResetBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to deselect the currently selected system.
        /// </summary>
        public ICommand ResetSystemCommand { get; }

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

                if (value != null)
                {
                    BodyName = value.Name;
                    BodyMass = value.Mass;
                }
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

                if (value != null)
                {
                    SelectedSystemBodies.Load(value.BodyToSystemJoins);
                }
            }
        }

        /// <summary>
        /// The body to system joins of the selected system's bodies.
        /// </summary>
        public IObservableCollection<BodyToSystemJoin> SelectedSystemBodies
        {
            get
            {
                return selectedSystemBodies;
            }
            set
            {
                selectedSystemBodies = value;
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
        /// Command invoked whenever the user wants to update the currently selected body in the database.
        /// </summary>
        public ICommand UpdateBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to update the currently selected system in the database.
        /// </summary>
        public ICommand UpdateSystemCommand { get; }

        /// <summary>
        /// Invoked whenever the user wants to add a body to the currently edited system.
        /// </summary>
        private void AddBodyToSystemCommandImpl()
        {
            if (SelectedSystemBodies.All(join => join.BodyId != BodyToAdd.Id))
            {
                BodyToSystemJoin lastJoin = dbContext.BodyToSystemJoins.OrderBy(join => join.Id).Last();

                BodyToSystemJoin addedBodyJoin = new BodyToSystemJoin(lastJoin.Id + 1, BodyToAdd.Id, SelectedSystem.Id);

                SelectedSystemBodies.Add(addedBodyJoin);
                this.RaisePropertyChanged(nameof(SelectedSystemBodies));
            }
            else
            {
                SystemFeedback = "Could not add selected body to system, as it already exists there.";
                this.RaisePropertyChanged(nameof(SystemFeedback));
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
        /// Invoked whenever the user wants to remove a body from the currently edited system.
        /// </summary>
        private void RemoveBodyFromSystemCommandImpl()
        {
            SelectedSystemBodies.Remove(BodyToRemove);
        }

        /// <summary>
        /// Invoked whenever the user wants to deselect the currently selected body.
        /// </summary>
        private void ResetBodyCommandImpl()
        {
            SelectedBody = null;
        }

        /// <summary>
        /// Invoked whenever the user wants to deselect the currently selected system.
        /// </summary>
        private void ResetSystemCommandImpl()
        {
            SelectedSystem = null;
            SelectedSystemBodies.Clear();
        }

        /// <summary>
        /// Invoked whenever the user wants to update the currently selected body in the database.
        /// </summary>
        private void UpdateBodyCommandImpl()
        {
            try
            {
                Body editedBody = dbContext.Bodies.First(body => body.Id == SelectedBody.Id);

                editedBody.Name = BodyName;
                editedBody.Mass = BodyMass;

                BodyFeedback = "Body was successfully updated.";
                this.RaisePropertyChanged(nameof(BodyFeedback));

                SelectedBody = null;

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                BodyFeedback = "Body could not be updated.";
                this.RaisePropertyChanged(nameof(BodyFeedback));
            }
        }

        /// <summary>
        /// Invoked whenever the user wants to update the currently selected system in the database.
        /// </summary>
        private void UpdateSystemCommandImpl()
        {
            try
            {
                dbContext.Systems.First(system => system.Id == SelectedSystem.Id).BodyToSystemJoins =
                    SelectedSystemBodies.ToList();

                SystemFeedback = "System was successfully updated.";
                this.RaisePropertyChanged(nameof(SystemFeedback));

                SelectedSystem = null;
                SelectedSystemBodies.Clear();

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                SystemFeedback = "System could not be updated.";
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

            if (SelectedSystem?.BodyToSystemJoins != null)
            {
                SelectedSystemBodies.Load(SelectedSystem.BodyToSystemJoins);
            }
        }
    }
}