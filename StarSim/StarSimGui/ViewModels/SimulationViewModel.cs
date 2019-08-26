using DynamicData.Binding;

using Microsoft.EntityFrameworkCore;

using ReactiveUI;

using SFML.Graphics;
using SFML.Window;

using StarSimGui.Source;

using StarSimLib.Configuration;
using StarSimLib.Contexts;
using StarSimLib.Data_Structures;
using StarSimLib.Models;
using StarSimLib.Physics;
using StarSimLib.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Body = StarSimLib.Data_Structures.Body;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the simulation view.
    /// </summary>
    public class SimulationViewModel : ViewModelBase
    {
        /// <summary>
        /// The maximum number of bodies that can be generated and that can participate in a simulation.
        /// </summary>
        private const int MaxGeneratedBodies = 10_000;

        /// <summary>
        /// The minimum number of bodies that can be generated and that can participate in a simulation.
        /// </summary>
        private const int MinGeneratedBodies = 0;

        /// <summary>
        /// The delegate method to use to derive the colour of a body's shape from the body's mass.
        /// </summary>
        private readonly MassToColourDelegate bodyMassToColourDelegate = BodyGenerator.DefaultColourDelegate;

        /// <summary>
        /// The delegate method to use to derive the radius of a body's shape from the body's mass.
        /// </summary>
        private readonly MassToRadiusDelegate bodyMassToRadiusDelegate = BodyGenerator.DefaultRadiusDelegate;

        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// The body position update delegate to use in the simulation.
        /// </summary>
        private readonly UpdateDelegate simulationBodyUpdateDelegate = BodyUpdater.UpdateBodiesBarnesHut;

        /// <summary>
        /// The dummy body holding the field values for the generated <see cref="Body"/> instance.
        /// </summary>
        private BodyDummy currentBodyDummy = new BodyDummy();

        /// <summary>
        /// The current simulation.
        /// </summary>
        private SimulationScreen currentSimulation;

        /// <summary>
        /// The backing field for the <see cref="CurrentUser"/> property.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Backing field for the <see cref="PublishedSystems"/> property.
        /// </summary>
        private IObservableCollection<PublishedSystem> publishedSystemsObservable;

        /// <summary>
        /// The backing field for the <see cref="SelectedItemIndex"/> property.
        /// </summary>
        private int selectedItemIndex;

        /// <summary>
        /// The backing field for the <see cref="SimulatedBodies"/> property.
        /// </summary>
        private IObservableCollection<Body> simulatedBodiesObservable;

        /// <summary>
        /// The backing field for the <see cref="SimulatedBodyCount"/> property.
        /// </summary>
        private int simulatedBodyCount;

        /// <summary>
        /// Holds the circle shapes for the bodies participating in the simulation.
        /// </summary>
        private Dictionary<Body, CircleShape> simulatedBodyShapes;

        /// <summary>
        /// Backing field for the <see cref="SystemName"/> property.
        /// </summary>
        private string systemName;

        /// <summary>
        /// Backing field for the <see cref="SelectedPublishedSystem"/> property.
        /// </summary>
        public PublishedSystem selectedPublishedSystem;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationViewModel"/> class.
        /// </summary>
        public SimulationViewModel()
        {
            simulatedBodiesObservable = new ObservableCollectionExtended<Body>();

            #region Main Menu Commands

            IObservable<bool> canAddBody =
                this.WhenAnyValue(x => x.CurrentBody, selector: body => !body?.Equals(new BodyDummy()) ?? false);

            AddBodyCommand = ReactiveCommand.Create(AddBodyCommandImpl, canAddBody);

            IObservable<bool> canClearBodies =
                this.WhenAnyValue(x => x.SimulatedBodies, selector: bodies => bodies != null);

            ClearBodiesCommand = ReactiveCommand.Create(ClearBodiesCommandImpl, canClearBodies);

            IObservable<bool> validBodySelected =
                this.WhenAnyValue(x => x.SelectedItemIndex, x => x.SimulatedBodies, x => x.CurrentBody,
                    (index, simulatedBodies, body) => index > -1 && simulatedBodies.Count > 0 && (!body?.Equals(new BodyDummy()) ?? false));

            CopyBodyCommand = ReactiveCommand.Create(CopyBodyCommandImpl, validBodySelected);

            DeleteBodyCommand = ReactiveCommand.Create(DeleteBodyCommandImpl, validBodySelected);

            DeselectBodyCommand = ReactiveCommand.Create(DeselectBodyCommandImpl, validBodySelected);

            GenerateBodiesCommand = ReactiveCommand.Create(GenerateBodiesCommandImpl);

            IObservable<bool> currentSimulationInactive =
                this.WhenAnyValue(x => x.currentSimulation, selector: screen => screen == null);

            StartSimulationCommand =
                ReactiveCommand.CreateFromTask(StartSimulationCommandImpl, currentSimulationInactive);

            #endregion Main Menu Commands

            #region Body Editor Commands

            IObservable<bool> canUpdateBody =
                this.WhenAnyValue(x => x.CurrentBody, x => x.SelectedItemIndex,
                    (body, selectedIndex) => (!body?.Equals(new BodyDummy()) ?? false) && selectedIndex > -1 &&
                                             selectedIndex < MaxGeneratedBodies - MinGeneratedBodies);

            UpdateBodyCommand = ReactiveCommand.Create(UpdateBodyCommandImpl, canUpdateBody);

            #endregion Body Editor Commands

            #region System Publishing Commands

            IObservable<bool> canPublish = this.WhenAnyValue(x => x.SystemName,
                name => !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name));

            PublishCurrentSystemCommand = ReactiveCommand.Create(PublishCurrentSystemCommandImpl, canPublish);

            #endregion System Publishing Commands

            #region Published System Import Commands

            IObservable<bool> canImportPublishedSystem =
                this.WhenAnyValue(x => x.SelectedPublishedSystem, selector: system => system != null);

            ImportPublishedSystemCommand = ReactiveCommand.Create(ImportPublishedSystemCommandImpl, canImportPublishedSystem);

            #endregion Published System Import Commands

            publishedSystemsObservable = new ObservableCollectionExtended<PublishedSystem>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        /// <param name="configuration">The current configuration of the application.</param>
        public SimulationViewModel(in SimulatorContext context, in Config configuration) : this()
        {
            dbContext = context;
            Configuration = configuration;

            simulatedBodyCount = Configuration.BodyCount;
            PublishedSystems.Load(dbContext.PublishedSystems.Include(system => system.System));
        }

        #region Commands

        /// <summary>
        /// Command invoked whenever the currently inspected body should be added to the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        public ICommand AddBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to clear the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        public ICommand ClearBodiesCommand { get; }

        /// <summary>
        /// Command invoked whenever the currently selected body should be added again to the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        public ICommand CopyBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the currently inspected body should be deleted from the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        public ICommand DeleteBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to deselect the currently selected body.
        /// </summary>
        public ICommand DeselectBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to generate a new collection.
        /// </summary>
        public ICommand GenerateBodiesCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to import the currently selected published system.
        /// </summary>
        public ICommand ImportPublishedSystemCommand { get; }

        /// <summary>
        /// Command invoked whenever the user want to publish the currently generated system.
        /// </summary>
        public ICommand PublishCurrentSystemCommand { get; }

        /// <summary>
        /// Command invoked whenever a simulation should be started.
        /// </summary>
        public ICommand StartSimulationCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to update the currently edited body.
        /// </summary>
        public ICommand UpdateBodyCommand { get; }

        #endregion Commands

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseUpdated;

        /// <summary>
        /// The current configuration of the application.
        /// </summary>
        public Config Configuration { get; internal set; }

        /// <summary>
        /// The <see cref="Body"/> instance currently selected for editing or viewing.
        /// </summary>
        public BodyDummy CurrentBody
        {
            get
            {
                return currentBodyDummy;
            }
            set
            {
                currentBodyDummy = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The currently logged in user.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Whether the current user can publish systems.
        /// </summary>
        public bool IsPublisher
        {
            get { return (CurrentUser.Privileges & UserPrivileges.Publisher) == UserPrivileges.Publisher; }
        }

        /// <summary>
        /// Whether the currently selected published system is null.
        /// </summary>
        public bool IsSelectedPublishedSystemNull
        {
            get { return SelectedPublishedSystem == null; }
        }

        /// <summary>
        /// Feedback regarding the publication attempt.
        /// </summary>
        public string PublicationFeedback { get; private set; }

        /// <summary>
        /// The published systems held in the database.
        /// </summary>
        public IObservableCollection<PublishedSystem> PublishedSystems
        {
            get
            {
                return publishedSystemsObservable;
            }
            set
            {
                publishedSystemsObservable = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The index of the currently selected item in the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        public int SelectedItemIndex
        {
            get
            {
                return selectedItemIndex;
            }
            set
            {
                selectedItemIndex = value;
                this.RaisePropertyChanged();

                if (selectedItemIndex >= 0)
                {
                    CurrentBody = new BodyDummy(SimulatedBodies?[value], Configuration);
                }
            }
        }

        /// <summary>
        /// The currently selected published system from the <see cref="PublishedSystems"/> collection.
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

                this.RaisePropertyChanged(nameof(IsSelectedPublishedSystemNull));
            }
        }

        /// <summary>
        /// Whether to simulate a central attractor.
        /// </summary>
        public bool SimulateCentralAttractor { get; set; } = true;

        /// <summary>
        /// The bodies that will participate in the simulation.
        /// </summary>
        public IObservableCollection<Body> SimulatedBodies
        {
            get
            {
                return simulatedBodiesObservable;
            }
            set
            {
                simulatedBodiesObservable = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The number of bodies to generate.
        /// </summary>
        public int SimulatedBodyCount
        {
            get
            {
                return simulatedBodyCount;
            }
            set
            {
                simulatedBodyCount = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The name of the system when it will be published.
        /// </summary>
        public string SystemName
        {
            get
            {
                return systemName;
            }
            set
            {
                systemName = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Invokes the <see cref="DatabaseUpdated"/> event.
        /// </summary>
        private void OnDatabaseUpdated()
        {
            DatabaseUpdated?.Invoke();
        }

        #region Command Implementations

        /// <summary>
        /// Invoked whenever the user wants to add the currently edited body to the simulation.
        /// </summary>
        private void AddBodyCommandImpl()
        {
            CurrentBody.Generation = BodyGenerator.CurrentGeneration;

            if (SimulatedBodies.Count > 0)
            {
                CurrentBody.Id = SimulatedBodies[SimulatedBodies.Count - 1].Id + 1;
            }
            else
            {
                CurrentBody.Id = 0;
            }

            Body currentBody = CurrentBody.AsBody();

            SimulatedBodies.Add(currentBody);

            CurrentBody = new BodyDummy();
        }

        /// <summary>
        /// Invoked whenever the user wants to clear the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        private void ClearBodiesCommandImpl()
        {
            SimulatedBodies.Clear();
        }

        /// <summary>
        /// Invoked whenever the user wants to re-add the currently selected body to the <see cref="SimulatedBodies"/> collection.
        /// </summary>
        private void CopyBodyCommandImpl()
        {
            CurrentBody.Generation = BodyGenerator.CurrentGeneration;
            CurrentBody.Id = SimulatedBodies[SimulatedBodies.Count - 1].Id + 1;

            Body currentBody = CurrentBody.AsBody();

            SimulatedBodies.Add(currentBody);
        }

        /// <summary>
        /// Invoked whenever the user wants to delete the currently edited body from the simulation.
        /// </summary>
        private void DeleteBodyCommandImpl()
        {
            SimulatedBodies.RemoveAt(SelectedItemIndex);

            CurrentBody = new BodyDummy();
        }

        /// <summary>
        /// Invoked whenever the user wants to deselect the currently selected body.
        /// </summary>
        private void DeselectBodyCommandImpl()
        {
            SelectedItemIndex = -1;
            CurrentBody = new BodyDummy();
        }

        /// <summary>
        /// Invoked whenever the user wants to generate a new set of bodies to simulate.
        /// </summary>
        private void GenerateBodiesCommandImpl()
        {
            if (SimulatedBodyCount < MinGeneratedBodies || SimulatedBodyCount > MaxGeneratedBodies)
            {
                // we cannot generate a negative number of bodies, and we cannot reliably simulate more than 10 000 bodies
                return;
            }

            SimulatedBodies.Load(BodyGenerator.GenerateBodies(SimulatedBodyCount, SimulateCentralAttractor));

            simulatedBodyShapes =
                BodyGenerator.GenerateShapes(SimulatedBodies, bodyMassToRadiusDelegate, bodyMassToColourDelegate);
        }

        /// <summary>
        /// Invoked whenever the user wants to import the currently selected system into the simulation.
        /// </summary>
        private void ImportPublishedSystemCommandImpl()
        {
            using (SimulatedBodies.SuspendNotifications())
            {
                SimulatedBodies.Clear();

                StarSimLib.Models.System system = SelectedPublishedSystem.System;
                IReadOnlyDictionary<ulong, (Vector4 pos, Vector4 vel)> bodyPositionData = system.BodyPositionData;

                uint bodyId = 0;

                foreach (BodyToSystemJoin join in system.BodyToSystemJoins)
                {
                    (Vector4 position, Vector4 velocity) = bodyPositionData[join.BodyId];

                    SimulatedBodies.Add(new Body(position, velocity, join.Body.Mass, BodyGenerator.CurrentGeneration, bodyId++));
                }
            }
        }

        /// <summary>
        /// Invoked whenever the user wants to publish the currently generated system.
        /// </summary>
        private void PublishCurrentSystemCommandImpl()
        {
            if (dbContext.Systems.Any(system => system.Name.Equals(SystemName)))
            {
                PublicationFeedback = "System with same name already exists in the database.";
                this.RaisePropertyChanged(nameof(PublicationFeedback));
                return;
            }

            try
            {
                StarSimLib.Models.Body lastBody = dbContext.Bodies.OrderBy(body => body.Id).Last();
                StarSimLib.Models.System lastSystem = dbContext.Systems.OrderBy(system => system.Id).Last();
                BodyToSystemJoin lastJoin = dbContext.BodyToSystemJoins.OrderBy(join => join.Id).Last();
                PublishedSystem lastPublishedSystem = dbContext.PublishedSystems.OrderBy(system => system.Id).Last();

                ulong bodyId = lastBody.Id,
                    systemId = lastSystem.Id + 1,
                    joinId = lastJoin.Id,
                    publishedSystemId = lastPublishedSystem.Id + 1;

                PublishedSystem newPublishedSystem =
                    new PublishedSystem(publishedSystemId, CurrentUser.Id, systemId, DateTime.Now);
                List<StarSimLib.Models.Body> bodies = new List<StarSimLib.Models.Body>(SimulatedBodies.Count);
                List<BodyToSystemJoin> joins = new List<BodyToSystemJoin>(SimulatedBodies.Count);

                Dictionary<ulong, (Vector4 pos, Vector4 vel)> bodyPositionData =
                    new Dictionary<ulong, (Vector4 pos, Vector4 vel)>();

                foreach (Body body in SimulatedBodies)
                {
                    //todo implement naming functionality for bodies, perhaps in update bodies screen?
                    bodies.Add(new StarSimLib.Models.Body(++bodyId, null, body.Mass));

                    bodyPositionData[bodyId] = (body.Position, body.Velocity);

                    joins.Add(new BodyToSystemJoin(++joinId, bodyId, systemId));
                }

                StarSimLib.Models.System newSystem = new StarSimLib.Models.System(systemId, SystemName, bodyPositionData);

                dbContext.Bodies.AddRange(bodies);
                dbContext.Systems.Add(newSystem);
                dbContext.BodyToSystemJoins.AddRange(joins);
                dbContext.PublishedSystems.Add(newPublishedSystem);

                OnDatabaseUpdated();

                PublishedSystems.Load(dbContext.PublishedSystems);

                PublicationFeedback = "System was successfully published to the database.";
                this.RaisePropertyChanged(nameof(PublicationFeedback));
            }
            catch (InvalidOperationException)
            {
                PublicationFeedback = "System was already published and exists in the database.";
                this.RaisePropertyChanged(nameof(PublicationFeedback));
            }
            catch (DbUpdateException)
            {
                PublicationFeedback = "Error while saving system to the database.";
                this.RaisePropertyChanged(nameof(PublicationFeedback));
            }
        }

        /// <summary>
        /// Invoked whenever the user wants to start a new simulation.
        /// </summary>
        private async Task StartSimulationCommandImpl()
        {
            Body[] simulatedBodiesArr = SimulatedBodies.ToArray();

            simulatedBodyShapes =
                BodyGenerator.GenerateShapes(simulatedBodiesArr, bodyMassToRadiusDelegate, bodyMassToColourDelegate);

            IInputHandler simulationInputHandler = new SimulationInputHandler(ref simulatedBodiesArr);

            ContextSettings customContextSettings = new ContextSettings
            {
                AntialiasingLevel = 8,
                DepthBits = 24,
                StencilBits = 8
            };

            await Task.Factory.StartNew(contextSettingsObj =>
            {
                using (RenderWindow window = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulation: FPS ",
                    Styles.Default, (ContextSettings)contextSettingsObj))
                {
                    currentSimulation = new SimulationScreen(window, simulationInputHandler,
                        ref simulatedBodiesArr, ref simulatedBodyShapes, simulationBodyUpdateDelegate);

                    currentSimulation.Run();
                }
            }, customContextSettings);

            currentSimulation = null;
        }

        /// <summary>
        /// Invoked whenever the user wants to update the currently edited body.
        /// </summary>
        private void UpdateBodyCommandImpl()
        {
            SimulatedBodies[SelectedItemIndex] = CurrentBody.AsBody();
        }

        #endregion Command Implementations

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedIn"/> event.
        /// </summary>
        /// <param name="newUser">The user which logged in.</param>
        public void HandleLogin(User newUser)
        {
            CurrentUser = newUser;

            this.RaisePropertyChanged(nameof(IsPublisher));
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedOut"/> event.
        /// </summary>
        public void HandleLogout()
        {
            CurrentUser = null;
        }
    }
}