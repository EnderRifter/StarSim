using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using StarSimLib;
using StarSimLib.Contexts;
using StarSimLib.Cryptography;
using StarSimLib.Data_Structures;
using StarSimLib.Physics;
using StarSimLib.UI;

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
        /// The database context to use for the lifetime of the program.
        /// </summary>
        private static readonly SimulatorContext databaseContext;

        /// <summary>
        /// The simulation which we will render, once the user sets it up.
        /// </summary>
        private static readonly SimulationScreen simulationScreen;

        /// <summary>
        /// Initialises a new instance of the <see cref="Program"/> class,
        /// </summary>
        static Program()
        {
            // set up the database context for the program
            databaseContext = new SimulatorContext();

            bodies = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
            bodyShapeMap = BodyGenerator.GenerateShapes(bodies);

#if DEBUG
            UpdateDelegate bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#else
            UpdateDelegate bodyPositionUpdater = BodyUpdater.UpdateBodiesBarnesHut;
#endif

            RenderWindow simulationWindow = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulation: FPS ");
            IInputHandler simulationInputHandler = new SimulationInputHandler(ref bodies);

            simulationScreen = new SimulationScreen(simulationWindow, simulationInputHandler, ref bodies, ref bodyShapeMap, bodyPositionUpdater);
        }

        /// <summary>
        /// Converts the given enumerable to a string representation of its contents.
        /// </summary>
        /// <typeparam name="T">The type of object held in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to convert.</param>
        /// <param name="itemConverter">The custom function to use to convert a single enumerable item to its string form.</param>
        /// <returns>The contents of the enumerable as a string.</returns>
        private static string EnumerableToString<T>(IEnumerable<T> enumerable, Func<T, string> itemConverter = null)
        {
            StringBuilder enumerableStringBuilder = new StringBuilder("[");

            if (itemConverter == null)
            {
                // the default converter function is just calling the Object.ToString() function
                itemConverter = item => item.ToString();
            }

            foreach (T item in enumerable)
            {
                enumerableStringBuilder.Append($"{itemConverter(item) ?? ""},");
            }

            enumerableStringBuilder.Append("]");

            return enumerableStringBuilder.ToString();
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

        private static void TestHashing(string password)
        {
            // set a shorthand for nicety reasons
            string BytesToString(byte[] contents) => CryptographyHelper.BytesToString(contents);

            Console.WriteLine($"Password to hash: {password}");

            byte[] passwordBytes = CryptographyHelper.StringToBytes(password);
            Console.WriteLine($"Password bytes:\n{BytesToString(passwordBytes)}");

            // generates a salt of the default length
            byte[] saltBytes = CryptographyHelper.GenerateSalt();
            Console.WriteLine($"Generated salt:\n{BytesToString(saltBytes)}");

            // generates a hash of the default length
            byte[] passwordHash = CryptographyHelper.GenerateHash(passwordBytes, saltBytes);
            Console.WriteLine($"Generated valid hash:\n{BytesToString(passwordHash)}");

            byte[] invalidPasswordBytes = CryptographyHelper.StringToBytes("password ");
            Console.WriteLine($"Invalid password bytes:\n{BytesToString(invalidPasswordBytes)}");

            byte[] invalidPasswordHash = CryptographyHelper.GenerateHash(invalidPasswordBytes, saltBytes);
            Console.WriteLine($"Generated invalid hash:\n{BytesToString(invalidPasswordHash)}");

            Console.WriteLine($"Valid hash == invalid hash: {CryptographyHelper.HashesEqual(passwordHash, invalidPasswordHash)}");
        }
    }
}