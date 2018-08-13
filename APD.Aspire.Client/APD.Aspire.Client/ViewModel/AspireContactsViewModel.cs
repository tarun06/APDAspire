using APDAspire.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using APD.Aspire.Logger;

namespace APD.Aspire.Client
{
    public enum APICommand
    {
        None,
        Get,
        Add,
        Update,
        Delete
    }

    internal class AspireContactsViewModel : ViewBase
    {      
        #region Constructor
        public AspireContactsViewModel()
        {
            _commandExecute = new Dictionary<APICommand, Action>()
            {
                { APICommand.Add, Add},
                { APICommand.Update, Update},
                { APICommand.Delete, Delete},
                { APICommand.Get, GetAllContacts}
            };
        }
        #endregion

        #region Private Member

        private Dictionary<APICommand, Action> _commandExecute = new Dictionary<APICommand, Action>();

        private ObservableCollection<ContactModel> _contacts = new ObservableCollection<ContactModel>();

        private string _statusMessage = string.Empty;

        private string _searchText;

        private bool _selectAll;

        #endregion

        #region Command(s)

        private DelegateCommand _executeCommand;
        public DelegateCommand ExecuteCommand
        {

            get
            {
                if (_executeCommand == null)
                    _executeCommand = new DelegateCommand(Execute, CaExecuteDo);
                return _executeCommand;
            }
        }

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand
        {

            get
            {
                if (_searchCommand == null)
                    _searchCommand = new DelegateCommand(execute => SearchContact(),
                        canExecute => !string.IsNullOrEmpty(SearchText));
                return _searchCommand;
            }
        }

        private DelegateCommand _selectAllCommand;
        public DelegateCommand SelectAllCommand
        {

            get
            {
                if (_selectAllCommand == null)
                    _selectAllCommand = new DelegateCommand(execute => SelectAllContact(),
                        canExecute => true);
                return _selectAllCommand;
            }
        }


        #endregion

        #region Properties
        
        public bool SelectAll
        {

            get { return _selectAll; }
            set
            {
                _selectAll = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ContactModel> Contacts
        {

            get { return _contacts; }
            set
            {
                _contacts = value;
                RaisePropertyChanged();
            }
        }

        public string SearchText
        {

            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged();
            }
        }

        public string StatusMessage
        {

            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Private Method(s)

        /// <summary>
        /// Select all contacts
        /// </summary>
        private void SelectAllContact()
        {
            if (Contacts == null || !Contacts.Any()) return;

            Contacts.All(x => x.IsChecked = SelectAll);
        }


        /// <summary>
        /// Can Execute for Command(s)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CaExecuteDo(object obj)
        {
            if (obj == null) return false;

            if (string.IsNullOrEmpty(obj?.ToString())) return false;

            Enum.TryParse(obj.ToString(), out APICommand apiCommand);
            if (apiCommand == APICommand.None)
                return false;
            return true;
        }

        /// <summary>
        /// Search Contact by 'First Name'
        /// </summary>
        /// <param name="obj"></param>
        private async void SearchContact()
        {
            try
            {
                using (var client = ServerInstance.GetServerInstance())
                {
                    if (client == null) return;

                    HttpResponseMessage httpRequestMessage = await client.GetAsync("api/Contact/firstName/" + SearchText + "");

                    if (!httpRequestMessage.IsSuccessStatusCode)
                    {
                        Contacts.Clear();
                        StatusMessage = string.Format("No Contact Found.");
                        return;
                    }

                    var result = await httpRequestMessage.Content.ReadAsAsync<List<ContactModel>>();

                    if (result != null)
                        Contacts = new ObservableCollection<ContactModel>(result);

                    StatusMessage = string.Format("Find Command, {0} result found.", Contacts.Count());
                }
            } 
            catch(Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(SearchContact), ex.ToString());
            }
        }

        /// <summary>
        /// Execute CRUD Operation
        /// </summary>
        /// <param name="obj"></param>
        private void Execute(object obj)
        {
            try
            {
                if (obj == null) return;

                if (string.IsNullOrEmpty(obj?.ToString())) return;

                Enum.TryParse(obj.ToString(), out APICommand commandToExecute);

                if (!_commandExecute.ContainsKey(commandToExecute)) return;

                _commandExecute[commandToExecute]?.Invoke();
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(Execute), ex.ToString());
            }
        }

        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <param name="obj"></param>
        private async void GetAllContacts()
        {
            try
            {
                using (var client = ServerInstance.GetServerInstance())
                {
                    if (client == null) return;

                    HttpResponseMessage httpRequestMessage = await client.GetAsync("api/Contact/all");

                    if (!httpRequestMessage.IsSuccessStatusCode)
                    {
                        Contacts.Clear();
                        StatusMessage = string.Format("GET command, Failed or No Contacts Found .");
                        return;
                    }

                    var result = await httpRequestMessage.Content.ReadAsAsync<List<ContactModel>>();

                    if (result == null || !result.Any()) return;

                    Contacts = new ObservableCollection<ContactModel>(result);

                    StatusMessage = string.Format("GET command, {0} result found.", Contacts.Count());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(GetAllContacts), ex.ToString());
            }

        }

        /// <summary>
        /// Delete selcted contacts
        /// </summary>
        private async void Delete()
        {
            try
            {
                using (var client = ServerInstance.GetServerInstance())
                {
                    if (client == null) return;

                    var contactsToBeRemoved = Contacts.Where(x => x.IsChecked).ToList();

                    if (!contactsToBeRemoved.Any()) return;

                    for (int index = 0; index < contactsToBeRemoved.Count();)
                    {
                        if (contactsToBeRemoved.Count <= index) continue;

                        var item = contactsToBeRemoved[index];

                        if (item == null) continue;

                        HttpResponseMessage httpRequestMessage = await client.DeleteAsync("api/Contact/" + item.Contact_Id);

                        if (!httpRequestMessage.IsSuccessStatusCode)
                        {
                            StatusMessage = string.Format("Delete command Failed.");
                            return;
                        }

                        var result = await httpRequestMessage.Content.ReadAsAsync<bool>();
                        if (!result) continue;

                        Contacts.Remove(item);
                        StatusMessage = string.Format("Delete Command, {0} deleted.", item.Contact_Id);
                        index++;

                    }
                }
            } 
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(Delete), ex.ToString());
            }            
        }

        /// <summary>
        /// Add Contact(s)
        /// </summary>
        private async void Add()
        {
            try
            {
                using (var client = ServerInstance.GetServerInstance())
                {
                    if (client == null) return;

                    foreach (var contactTobeAdd in Contacts.Where(x => x.IsChecked).ToList())
                    {
                        if (client == null) return;

                        HttpResponseMessage httpRequestMessage = await client.PutAsJsonAsync("api/Contact/add", contactTobeAdd);

                        if (!httpRequestMessage.IsSuccessStatusCode)
                        {
                            StatusMessage = string.Format("Add command Failed.");
                            return;
                        }

                        var result = await httpRequestMessage.Content.ReadAsAsync<Guid>();

                        if (result == null || result == Guid.Empty) return;

                        contactTobeAdd.Contact_Id = result;
                        StatusMessage = string.Format("Add Command, {0} added.", contactTobeAdd.Contact_Id);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(Add), ex.ToString());
            }
        }

        /// <summary>
        /// Update Selected Contact(s)
        /// </summary>
        private async void Update()
        {
            try
            {
                using (var client = ServerInstance.GetServerInstance())
                {
                    if (client == null) return;

                    foreach (var contactTobeUpdated in Contacts.Where(x => x.IsChecked).ToList())
                    {
                        HttpResponseMessage httpRequestMessage = await client.PostAsJsonAsync("api/Contact/update", contactTobeUpdated);

                        if (!httpRequestMessage.IsSuccessStatusCode)
                        {
                            StatusMessage = string.Format("Update command Failed.");
                            return;
                        }

                        var result = await httpRequestMessage.Content.ReadAsAsync<Guid>();

                        if (result == null || result == Guid.Empty) return;

                        contactTobeUpdated.Contact_Id = result;

                        StatusMessage = string.Format("Update Command, {0} updated.", contactTobeUpdated.Contact_Id);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(LogLevel.Exception, nameof(Update), ex.ToString());
            }
        }
        #endregion
    }
}