using ReactiveUI;

using StarSimLib.Contexts;
using StarSimLib.Models;

using System;
using System.Linq;
using System.Windows.Input;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the create systems view.
    /// </summary>
    public class CreateSystemsViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="BodyMass"/> property.
        /// </summary>
        private double bodyMass;

        /// <summary>
        /// Backing field for the <see cref="BodyName"/> property.
        /// </summary>
        private string bodyName;

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateSystemsViewModel"/> class.
        /// </summary>
        public CreateSystemsViewModel()
        {
            IObservable<bool> canCreate = this.WhenAnyValue(x => x.BodyName, x => x.BodyMass,
                (name, mass) => !string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name) && mass > 0);

            CreateBodyCommand = ReactiveCommand.Create(CreateBodyCommandImpl, canCreate);

            ResetBodyCommand = ReactiveCommand.Create(ResetBodyCommandImpl);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CreateSystemsViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public CreateSystemsViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// The mass of the body to create.
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
        /// The name of the body to create.
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
        /// Command invoked whenever the user wants to add the currently edited body to the database.
        /// </summary>
        public ICommand CreateBodyCommand { get; }

        /// <summary>
        /// Provides feedback about the changes made to the database.
        /// </summary>
        public string Feedback { get; private set; }

        /// <summary>
        /// Command invoked whenever the user wants to reset the currently edited body.
        /// </summary>
        public ICommand ResetBodyCommand { get; }

        /// <summary>
        /// Invoked whenever the user wants to add the currently edited body to the database.
        /// </summary>
        private void CreateBodyCommandImpl()
        {
            if (dbContext.Bodies.Any(body => body.Name.Equals(BodyName)))
            {
                Feedback = "Body with the same name already exists in the database.";
                this.RaisePropertyChanged(nameof(Feedback));
            }

            try
            {
                Body lastBody = dbContext.Bodies.OrderBy(body => body.Id).Last();

                Body newBody = new Body(lastBody.Id + 1, BodyName, BodyMass);

                dbContext.Bodies.Add(newBody);

                Feedback = "Body was successfully added to the database.";
                this.RaisePropertyChanged(nameof(Feedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                Feedback = "Body could not be added to the database.";
                this.RaisePropertyChanged(nameof(Feedback));
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
        /// Invoked whenever the user wants to reset the currently edited body.
        /// </summary>
        private void ResetBodyCommandImpl()
        {
            BodyName = string.Empty;
            BodyMass = 0;
            Feedback = string.Empty;
            this.RaisePropertyChanged(nameof(Feedback));
        }

        /// <summary>
        /// Refreshed the database sources for this view model.
        /// </summary>
        internal void HandleDatabaseRefresh()
        {
        }
    }
}