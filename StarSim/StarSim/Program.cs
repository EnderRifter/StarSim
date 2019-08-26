using Newtonsoft.Json;

using SFML.Graphics;
using SFML.Window;

using StarSimLib.Configuration;
using StarSimLib.Data_Structures;
using StarSimLib.Physics;
using StarSimLib.UI;

using System;
using System.Collections.Generic;
using System.IO;

namespace StarSim
{
    /// <summary>
    /// Main program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private static readonly Body[] bodies;

        /// <summary>
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private static readonly Dictionary<Body, CircleShape> bodyShapeMap;

        /// <summary>
        /// The simulation which we will render, once the user sets it up.
        /// </summary>
        private static readonly SimulationScreen simulationScreen;

        /// <summary>
        /// The current configuration of the application.
        /// </summary>
        public static Config configuration;

        /// <summary>
        /// Initialises a new instance of the <see cref="Program"/> class,
        /// </summary>
        static Program()
        {
            ReadConfigFile();

            bodies = BodyGenerator.GenerateBodies(configuration.BodyCount, true);
            bodyShapeMap = BodyGenerator.GenerateShapes(bodies);

#if DEBUG
            UpdateDelegate bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#else
            UpdateDelegate bodyPositionUpdater = BodyUpdater.UpdateBodiesBarnesHut;
#endif

            ContextSettings customContextSettings = new ContextSettings { AntialiasingLevel = 8, DepthBits = 24, StencilBits = 8 };

            RenderWindow simulationWindow =
                new RenderWindow(VideoMode.DesktopMode, "N-Body Simulation: FPS ", Styles.Default, customContextSettings);

            PrintContextSettings(customContextSettings);

            IInputHandler simulationInputHandler = new SimulationInputHandler(ref bodies);

            simulationScreen = new SimulationScreen(simulationWindow, simulationInputHandler, ref bodies, ref bodyShapeMap, bodyPositionUpdater)
            {
                Configuration = configuration,
            };
        }

        /// <summary>
        /// Entry point for our application.
        /// </summary>
        private static void Main()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Press 'enter' to continue...");
            Console.ReadLine();

            // run simulation
            simulationScreen.Run();

            Console.WriteLine("Goodbye, World!");
            Console.WriteLine("Press 'enter' to quit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints details about the given <see cref="ContextSettings"/> instance.
        /// </summary>
        /// <param name="settings">The settings to print.</param>
        private static void PrintContextSettings(ContextSettings settings)
        {
            Console.WriteLine($"AA Level: {settings.AntialiasingLevel}, " +
                              $"Depth Bits: {settings.DepthBits}, " +
                              $"Stencil Bits: {settings.StencilBits}, " +
                              $"SRGB Capable: {settings.SRgbCapable}, " +
                              $"Attributes: {settings.AttributeFlags}, " +
                              $"Version: {settings.MajorVersion}.{settings.MinorVersion}");
        }

        /// <summary>
        /// Reads in and deserialises the configuration file at the given path.
        /// </summary>
        /// <param name="path">The path at which the configuration is located.</param>
        private static void ReadConfigFile(string path = @"./config.txt")
        {
            try
            {
                string fullPath = Path.GetFullPath(path);

                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                }

                using (FileStream fs = File.OpenRead(fullPath))
                {
                    using (StreamReader fileReader = new StreamReader(fs))
                    {
                        configuration = Config.Load(fileReader.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                configuration = new Config();
            }
        }
    }
}