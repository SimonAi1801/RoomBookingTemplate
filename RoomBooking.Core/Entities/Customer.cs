using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace RoomBooking.Core.Entities
{
    public class Customer : EntityObject, INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FirstName
        {
            get => _firstName;
            set 
            { 
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        [Required]
        public string LastName
        {
            get => _lastName;
            set 
            { 
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }


        private string _iban;

        public string Iban
        {
            get => _iban;
            set 
            { 
                _iban = value;
                OnPropertyChanged(nameof(Iban));
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Booking> Bookings { get; set; }

        public Customer()
        {
            Bookings = new List<Booking>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
