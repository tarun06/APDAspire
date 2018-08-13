using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace APDAspire.Model
{
    public class ViewBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ContactModel : ViewBase
    {
        private Guid _contact_Id;
        public Guid Contact_Id
        {
            get { return _contact_Id; }
            set
            {
                _contact_Id = value;
                RaisePropertyChanged();
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }




        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged();
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _dob;
        public DateTime DOB
        {
            get { return _dob; }
            set
            {
                _dob = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> _emailID;
        public ObservableCollection<string> EmailId
        {
            get { return _emailID; }
            set
            {
                _emailID = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> _phoneNumber;
        public ObservableCollection<string> PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                RaisePropertyChanged();
            }
        }
    }
}
