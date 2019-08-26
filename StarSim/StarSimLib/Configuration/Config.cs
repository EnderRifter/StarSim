using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using StarSimLib.Data_Structures;

namespace StarSimLib.Configuration
{
    /// <summary>
    /// Stores constants used to configure the simulation.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
            #region Copy over values from Constants class

            AcceptedEmailProviders = Constants.AcceptedEmailProviders;

            AstronomicalUnit = Constants.AstronomicalUnit;

            BodyCount = Constants.BodyCount;

            CentralBodyMass = Constants.CentralBodyMass;

            EulerRotationStep = Constants.EulerRotationStep;

            FrameRate = Constants.FrameRate;

            G = Constants.G;

            MinimumTreeWidth = Constants.MinimumTreeWidth;

            OutputFileDirectory = Constants.OutputFileDirectory;

            SecondsPerTick = Constants.SecondsPerTick;

            SimulationRate = Constants.SimulationRate;

            SofteningFactor = Constants.SofteningFactor;

            SofteningFactor2 = Constants.SofteningFactor2;

            SolarMass = Constants.SolarMass;

            StoredPreviousPositionCount = Constants.StoredPreviousPositionCount;

            TimeStep = Constants.TimeStep;

            TreeTheta = Constants.TreeTheta;

            UniverseOctant = Constants.UniverseOctant;

            UniverseSize = Constants.UniverseSize;

            ZoomStep = Constants.ZoomStep;

            #endregion Copy over values from Constants class
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="acceptedEmailProviders">The value to use for the <see cref="AcceptedEmailProviders"/> property.</param>
        /// <param name="astronomicalUnit">The value to use for the <see cref="AstronomicalUnit"/> property.</param>
        /// <param name="bodyCount">The value to use for the <see cref="BodyCount"/> property.</param>
        /// <param name="centralBodyMass">The value to use for the <see cref="CentralBodyMass"/> property.</param>
        /// <param name="eulerRotationStep">The value to use for the <see cref="EulerRotationStep"/> property.</param>
        /// <param name="frameRate">The value to use for the <see cref="FrameRate"/> property.</param>
        /// <param name="g">The value to use for the <see cref="G"/> property.</param>
        /// <param name="minimumTreeWidth">The value to use for the <see cref="MinimumTreeWidth"/> property.</param>
        /// <param name="outputFileDirectory">The value to use for the <see cref="OutputFileDirectory"/> property.</param>
        /// <param name="secondsPerTick">The value to use for the <see cref="SecondsPerTick"/> property.</param>
        /// <param name="simulationRate">The value to use for the <see cref="SimulationRate"/> property.</param>
        /// <param name="softeningFactor">The value to use for the <see cref="SofteningFactor"/> property.</param>
        /// <param name="softeningFactor2">The value to use for the <see cref="SofteningFactor2"/> property.</param>
        /// <param name="solarMass">The value to use for the <see cref="SolarMass"/> property.</param>
        /// <param name="storedPreviousPositionCount">The value to use for the <see cref="StoredPreviousPositionCount"/> property.</param>
        /// <param name="timeStep">The value to use for the <see cref="TimeStep"/> property.</param>
        /// <param name="treeTheta">The value to use for the <see cref="TreeTheta"/> property.</param>
        /// <param name="universeOctant">The value to use for the <see cref="UniverseOctant"/> property.</param>
        /// <param name="universeSize">The value to use for the <see cref="UniverseSize"/> property.</param>
        /// <param name="zoomStep">The value to use for the <see cref="ZoomStep"/> property.</param>
        /// <remarks>
        /// Intended to only be used with serialisers when setting the read-only properties on this instance.
        /// </remarks>
        public Config(string[] acceptedEmailProviders, double astronomicalUnit, int bodyCount,
            double centralBodyMass, double eulerRotationStep, uint frameRate, double g,
            double minimumTreeWidth, string outputFileDirectory, double secondsPerTick,
            uint simulationRate, double softeningFactor, double softeningFactor2, double solarMass,
            int storedPreviousPositionCount, double timeStep, double treeTheta, Octant universeOctant,
            double universeSize, double zoomStep)
        {
            #region Setting values from parameters

            AcceptedEmailProviders = acceptedEmailProviders;

            AstronomicalUnit = astronomicalUnit;

            BodyCount = bodyCount;

            CentralBodyMass = centralBodyMass;

            EulerRotationStep = eulerRotationStep;

            FrameRate = frameRate;

            G = g;

            MinimumTreeWidth = minimumTreeWidth;

            OutputFileDirectory = outputFileDirectory;

            SecondsPerTick = secondsPerTick;

            SimulationRate = simulationRate;

            SofteningFactor = softeningFactor;

            SofteningFactor2 = softeningFactor2;

            SolarMass = solarMass;

            StoredPreviousPositionCount = storedPreviousPositionCount;

            TimeStep = timeStep;

            TreeTheta = treeTheta;

            UniverseOctant = universeOctant;

            UniverseSize = universeSize;

            ZoomStep = zoomStep;

            #endregion Setting values from parameters
        }

        /// <summary>
        /// The list of accepted email providers.
        /// </summary>
        [JsonProperty]
        public string[] AcceptedEmailProviders { get; private set; }

        /// <summary>
        /// Definition of an astronomical unit.
        /// </summary>
        [JsonProperty]
        public double AstronomicalUnit { get; private set; }

        /// <summary>
        /// The amount of bodies that are rendered by default.
        /// </summary>
        [JsonProperty]
        public int BodyCount { get; private set; }

        /// <summary>
        /// The mass of the central body, if it is included.
        /// </summary>
        [JsonProperty]
        public double CentralBodyMass { get; private set; }

        /// <summary>
        /// The amount of degrees by which the view will be rotated in any given direction.
        /// </summary>
        [JsonProperty]
        public double EulerRotationStep { get; private set; }

        /// <summary>
        /// The frame rate limit for the program.
        /// </summary>
        [JsonProperty]
        public uint FrameRate { get; private set; }

        /// <summary>
        /// The gravitational constant (m^3 kg^-1 s^-2).
        /// </summary>
        [JsonProperty]
        public double G { get; private set; }

        /// <summary>
        /// The minimum width of a tree. Subtrees are not created when if their width would be smaller than this value,
        /// to prevent widths of NaN as a result of division errors.
        /// </summary>
        [JsonProperty]
        public double MinimumTreeWidth { get; private set; }

        /// <summary>
        /// The directory path at which all the simulation output files are stored.
        /// </summary>
        [JsonProperty]
        public string OutputFileDirectory { get; private set; }

        /// <summary>
        /// The number of seconds that each simulation tick represents.
        /// </summary>
        [JsonProperty]
        public double SecondsPerTick { get; private set; }

        /// <summary>
        /// The tick rate limit for the simulation.
        /// </summary>
        [JsonProperty]
        public uint SimulationRate { get; private set; }

        /// <summary>
        /// Softens the force between <see cref="Body"/>s to avoid infinities.
        /// </summary>
        [JsonProperty]
        public double SofteningFactor { get; private set; }

        /// <summary>
        /// The square of the <see cref="SofteningFactor"/>.
        /// </summary>
        [JsonProperty]
        public double SofteningFactor2 { get; private set; }

        /// <summary>
        /// The mass of the sun (1.98892e30f).
        /// </summary>
        [JsonProperty]
        public double SolarMass { get; private set; }

        /// <summary>
        /// The number of previous positions that will be stored by a body
        /// </summary>
        [JsonProperty]
        public int StoredPreviousPositionCount { get; private set; }

        /// <summary>
        /// The default time step for the simulation.
        /// </summary>
        [JsonProperty]
        public double TimeStep { get; private set; }

        /// <summary>
        /// The tolerance of the mass grouping approximation in the simulation. A body is only accelerated when the
        /// ratio of the tree's width to the distance (from the tree's center of mass to the body) is less than this.
        /// </summary>
        [JsonProperty]
        public double TreeTheta { get; private set; }

        /// <summary>
        /// The <see cref="Octant"/> instance representing the rendered universe.
        /// </summary>
        [JsonProperty]
        public Octant UniverseOctant { get; private set; }

        /// <summary>
        /// The maximum radius within which <see cref="Body"/>s will be placed.
        /// </summary>
        [JsonProperty]
        public double UniverseSize { get; private set; }

        /// <summary>
        /// The amount by which the zoom level will be increased or decreased.
        /// </summary>
        [JsonProperty]
        public double ZoomStep { get; private set; }

        /// <summary>
        /// Loads the serialised instance from the given file contents.
        /// </summary>
        /// <param name="serialisedConfigurationFileContents">
        /// The file contents from which a serialised instance should be deserialised.
        /// </param>
        /// <returns>
        /// The deserialised instance.
        /// </returns>
        public static Config Load(string serialisedConfigurationFileContents)
        {
            return JsonConvert.DeserializeObject<Config>(serialisedConfigurationFileContents) ?? new Config();
        }
    }
}