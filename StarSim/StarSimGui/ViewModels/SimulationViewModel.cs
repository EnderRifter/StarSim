using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Cache.Internal;
using ReactiveUI;
using SFML.Graphics;
using SFML.Window;
using StarSimGui.Source;
using StarSimLib;
using StarSimLib.Contexts;
using StarSimLib.Models;
using StarSimLib.Physics;
using StarSimLib.UI;
using Body = StarSimLib.Data_Structures.Body;

namespace StarSimGui.ViewModels
{
    /// <summary>
    /// Represents the simulation view.
    /// </summary>
    public class SimulationViewModel : ViewModelBase
    {
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
        private BodyDummy currentBodyDummy;

        /// <summary>
        /// The current simulation.
        /// </summary>
        private SimulationScreen currentSimulation;

        /// <summary>
        /// The backing field for the <see cref="CurrentUser"/> property.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// The backing field for the <see cref="SelectedItemIndex"/> property.
        /// </summary>
        private int selectedItemIndex = 0;

        /// <summary>
        /// The bodies that will participate in the simulation.
        /// </summary>
        private List<Body> simulatedBodies;

        /// <summary>
        /// The backing field for the <see cref="SimulatedBodies"/> property.
        /// </summary>
        private IObservableCollection<Body> simulatedBodiesObservable;

        /// <summary>
        /// The backing field for the <see cref="SimulatedBodyCount"/> property.
        /// </summary>
        private int simulatedBodyCount = Constants.BodyCount;

        /// <summary>
        /// Maps the bodies participating in the simulation to their shapes that will be rendered to represent them.
        /// </summary>
        private Dictionary<Body, CircleShape> simulatedBodyShapes;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationViewModel"/> class.
        /// </summary>
        public SimulationViewModel() : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public SimulationViewModel(SimulatorContext context)
        {
            dbContext = context;

            simulatedBodies = new List<Body>();

            simulatedBodiesObservable = new ObservableCollectionExtended<Body>(simulatedBodies);

            IObservable<bool> canAddBody =
                this.WhenAnyValue(x => x.CurrentBody, selector: body => !body?.Equals(new BodyDummy()) ?? false);

            AddBodyCommand = ReactiveCommand.Create(AddBodyCommandImpl, canAddBody);

            IObservable<bool> canDeleteBody =
                this.WhenAnyValue(x => x.CurrentBody, selector: body => !body?.Equals(new BodyDummy()) ?? false);

            DeleteBodyCommand = ReactiveCommand.Create(DeleteBodyCommandImpl, canDeleteBody);

            GenerateBodiesCommand = ReactiveCommand.Create(GenerateBodiesCommandImpl);

            IObservable<bool> currentSimulationInactive =
                this.WhenAnyValue(x => x.currentSimulation, selector: screen => screen == null);

            StartSimulationCommand =
                ReactiveCommand.CreateFromTask(StartSimulationCommandImpl, currentSimulationInactive);

            IObservable<bool> canUpdateBody = this.WhenAnyValue(x => x.CurrentBody, x => x.SelectedItemIndex,
                (body, selectedIndex) => (!body?.Equals(new BodyDummy()) ?? false) && selectedIndex >= 0 && selectedIndex < 10_000);

            UpdateBodyCommand = ReactiveCommand.Create(UpdateBodyCommandImpl, canUpdateBody);
        }

        /// <summary>
        /// Command invoked whenever the currently inspected body should be added to the <see cref="simulatedBodies"/> collection.
        /// </summary>
        public ICommand AddBodyCommand { get; }

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
        /// Command invoked whenever the currently inspected body should be deleted from the <see cref="simulatedBodies"/> collection.
        /// </summary>
        public ICommand DeleteBodyCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to generate a new collection.
        /// </summary>
        public ICommand GenerateBodiesCommand { get; }

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
                    currentBodyDummy = new BodyDummy(simulatedBodiesObservable?[selectedItemIndex]);
                    this.RaisePropertyChanged(nameof(CurrentBody));
                }
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
        /// Command invoked whenever a simulation should be started.
        /// </summary>
        public ICommand StartSimulationCommand { get; }

        /// <summary>
        /// Command invoked whenever the user wants to update the currently edited body.
        /// </summary>
        public ICommand UpdateBodyCommand { get; }

        /// <summary>
        /// Invoked whenever the user wants to add the currently edited body to the simulation.
        /// </summary>
        private void AddBodyCommandImpl()
        {
            CurrentBody.Generation = BodyGenerator.CurrentGeneration;
            CurrentBody.Id = simulatedBodiesObservable[simulatedBodiesObservable.Count - 1].Id + 1;

            Body currentBody = (Body)CurrentBody;

            simulatedBodies.Add(currentBody);
            simulatedBodiesObservable.Add(currentBody);
        }

        /// <summary>
        /// Invoked whenever the user wants to delete the currently edited body from the simulation.
        /// </summary>
        private void DeleteBodyCommandImpl()
        {
            simulatedBodies.RemoveAt(selectedItemIndex);
            simulatedBodiesObservable.RemoveAt(selectedItemIndex);

            CurrentBody = new BodyDummy();
        }

        /// <summary>
        /// Invoked whenever the user wants to generate a new set of bodies to simulate.
        /// </summary>
        private void GenerateBodiesCommandImpl()
        {
            if (SimulatedBodyCount < 0 || SimulatedBodyCount > 10_000)
            {
                // we cannot generate a negative number of bodies, and we cannot reliably simulate more than 10 000 bodies
                return;
            }

            simulatedBodies = new List<Body>(BodyGenerator.GenerateBodies(SimulatedBodyCount, SimulateCentralAttractor));

            simulatedBodiesObservable.Load(simulatedBodies);

            simulatedBodyShapes =
                BodyGenerator.GenerateShapes(simulatedBodies, bodyMassToRadiusDelegate, bodyMassToColourDelegate);
        }

        /// <summary>
        /// Invoked whenever the user wants to start a new simulation.
        /// </summary>
        private async Task StartSimulationCommandImpl()
        {
            Body[] simulatedBodiesArr = simulatedBodies.ToArray();

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
        /// Handles the <see cref="UserLoginViewModel.LoggedIn"/> event.
        /// </summary>
        /// <param name="newUser">The user which logged in.</param>
        public void HandleLogin(User newUser)
        {
            CurrentUser = newUser;
        }

        /// <summary>
        /// Handles the <see cref="UserLoginViewModel.LoggedOut"/> event.
        /// </summary>
        public void HandleLogout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Invoked whenever the user wants to update the currently edited body.
        /// </summary>
        public void UpdateBodyCommandImpl()
        {
            simulatedBodies[selectedItemIndex] = (Body)CurrentBody;
            simulatedBodiesObservable[selectedItemIndex] = (Body)CurrentBody;
        }
    }
}