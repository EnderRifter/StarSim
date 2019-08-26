using DynamicData.Binding;

using ReactiveUI;

using StarSimLib.Contexts;
using StarSimLib.Models;

using System;
using System.Windows.Input;

namespace StarSimGui.ViewModels.Database_ViewModels
{
    /// <summary>
    /// Represents the delete users view.
    /// </summary>
    public class DeleteUsersViewModel : ViewModelBase
    {
        /// <summary>
        /// The program database.
        /// </summary>
        private readonly SimulatorContext dbContext;

        /// <summary>
        /// Backing field for the <see cref="SelectedUser"/> property.
        /// </summary>
        private User selectedUser;

        /// <summary>
        /// Backing field for the <see cref="Users"/> property.
        /// </summary>
        private IObservableCollection<User> users;

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        public DeleteUsersViewModel()
        {
            users = new ObservableCollectionExtended<User>();

            IObservable<bool> canRemove = this.WhenAnyValue(x => x.SelectedUser, selector: user => user != null);

            RemoveUserCommand = ReactiveCommand.Create(RemoveUserCommandImpl, canRemove);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteUsersViewModel"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SimulatorContext"/> instance in which program data is stored.</param>
        public DeleteUsersViewModel(in SimulatorContext context) : this()
        {
            dbContext = context;

            Users.Load(dbContext.Users);
        }

        /// <summary>
        /// Signifies that the database should be updated.
        /// </summary>
        public event Action DatabaseEdited;

        /// <summary>
        /// Provides feedback about the changes made to the database.
        /// </summary>
        public string Feedback { get; private set; }

        /// <summary>
        /// Command invoked whenever the user wants to remove the currently selected user from the database.
        /// </summary>
        public ICommand RemoveUserCommand { get; }

        /// <summary>
        /// The currently selected user.
        /// </summary>
        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The users held in the database.
        /// </summary>
        public IObservableCollection<User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
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
        /// Invoked whenever the user wants to remove the currently selected user from the database.
        /// </summary>
        private void RemoveUserCommandImpl()
        {
            try
            {
                dbContext.Users.Remove(SelectedUser);

                Feedback = "User was successfully removed from the database.";
                this.RaisePropertyChanged(nameof(Feedback));

                OnDatabaseEdited();
            }
            catch (Exception)
            {
                Feedback = "User could not be removed from the database.";
                this.RaisePropertyChanged(nameof(Feedback));
            }
        }

        /// <summary>
        /// Refreshed the database sources for this view model.
        /// </summary>
        internal void HandleDatabaseRefresh()
        {
            Users.Load(dbContext.Users);
        }
    }
}