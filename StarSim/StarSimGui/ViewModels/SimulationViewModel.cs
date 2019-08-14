using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData.Binding;
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
        private int simulatedBodyCount = Constants.BodyCount;

        /// <summary>
        /// Holds the circle shapes for the bodies participating in the simulation.
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

            simulatedBodiesObservable = new ObservableCollectionExtended<Body>();

            IObservable<bool> canAddBody =
                this.WhenAnyValue(x => x.CurrentBody, selector: body => !body?.Equals(new BodyDummy()) ?? false);

            AddBodyCommand = ReactiveCommand.Create(AddBodyCommandImpl, canAddBody);

            IObservable<bool> canClearBodies =
                this.WhenAnyValue(x => x.SimulatedBodies, selector: bodies => bodies != null);

            ClearBodiesCommand = ReactiveCommand.Create(ClearBodiesCommandImpl, canClearBodies);

            IObservable<bool> canCopyBody =
                this.WhenAnyValue(x => x.SelectedItemIndex, x => x.CurrentBody,
                    (index, dummy) => index > -1 && (!dummy?.Equals(new BodyDummy()) ?? false));

            CopyBodyCommand = ReactiveCommand.Create(CopyBodyCommandImpl, canCopyBody);

            IObservable<bool> canDeleteBody =
                this.WhenAnyValue(x => x.CurrentBody, selector: body => !body?.Equals(new BodyDummy()) ?? false);

            DeleteBodyCommand = ReactiveCommand.Create(DeleteBodyCommandImpl, canDeleteBody);

            IObservable<bool> canDeselectBody =
                this.WhenAnyValue(x => x.SelectedItemIndex, x => x.CurrentBody,
                    (index, dummy) => index > -1 && (!dummy?.Equals(new BodyDummy()) ?? false));

            DeselectBodyCommand = ReactiveCommand.Create(DeselectBodyCommandImpl, canDeselectBody);

            GenerateBodiesCommand = ReactiveCommand.Create(GenerateBodiesCommandImpl);

            IObservable<bool> currentSimulationInactive =
                this.WhenAnyValue(x => x.currentSimulation, selector: screen => screen == null);

            StartSimulationCommand =
                ReactiveCommand.CreateFromTask(StartSimulationCommandImpl, currentSimulationInactive);

            IObservable<bool> canUpdateBody =
                this.WhenAnyValue(x => x.CurrentBody, x => x.SelectedItemIndex,
                    (body, selectedIndex) => (!body?.Equals(new BodyDummy()) ?? false) && selectedIndex > -1 &&
                                             selectedIndex < MaxGeneratedBodies - MinGeneratedBodies);

            UpdateBodyCommand = ReactiveCommand.Create(UpdateBodyCommandImpl, canUpdateBody);
        }

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
                    CurrentBody = new BodyDummy(SimulatedBodies?[value]);
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

            if (SimulatedBodies.Count > 0)
            {
                CurrentBody.Id = SimulatedBodies[SimulatedBodies.Count - 1].Id + 1;
            }
            else
            {
                CurrentBody.Id = 0;
            }

            Body currentBody = (Body)CurrentBody;

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

            Body currentBody = (Body)CurrentBody;

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
        /// Invoked whenever the user wants to start a new simulation.
        /// </summary>
        private async Task StartSimulationCommandImpl()
        {
            Body[] simulatedBodiesArr = SimulatedBodies.ToArray();

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
            SimulatedBodies[SelectedItemIndex] = (Body)CurrentBody;
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
    }
}