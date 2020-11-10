using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Core.Validations;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _iban;
        private Customer _customer;
        private Customer _undoCustomer;
        private ICommand _cmdUndoCommand;
        private ICommand _cmdSaveCommand;


        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
                Validate();
            }
        }
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
                Validate();
            }
        }

        [Required]
        [MinLength(2, ErrorMessage = "Der Nachname erfordert mindestens 2 Buchstaben")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                Validate();
            }
        }

        public string Iban
        {
            get => _iban;
            set
            {
                _iban = value;
                OnPropertyChanged(nameof(Iban));
                Validate();
            }
        }


        public EditCustomerModel(IWindowController controller, Customer customer) : base(controller)
        {
            _customer = customer;
            _firstName = customer.FirstName;
            _lastName = customer.LastName;
            _iban = customer.Iban;

            _undoCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Iban = customer.Iban
            };
        }

        public ICommand CmdUndoCommand
        {
            get
            {
                if (_cmdUndoCommand == null)
                {
                    _cmdUndoCommand = new RelayCommand(
                        execute: _ =>
                        {
                            _firstName = _undoCustomer.FirstName;
                            _lastName = _undoCustomer.LastName;
                            _iban = _undoCustomer.Iban;
                        },
                        canExecute: _ => Customer != _undoCustomer
                        );
                }
                return _cmdUndoCommand;
            }
            set => _cmdUndoCommand = value;
        }


        public ICommand CmdSaveCommand
        {
            get
            {
                if (_cmdSaveCommand == null)
                {
                    _cmdSaveCommand = new RelayCommand(
                        execute: async _ =>
                        {
                            Validate();

                            try
                            {
                                using IUnitOfWork uow = new UnitOfWork();
                                var customerInDb = await uow.Customers.GetByIdAsync(_customer.Id);
                                customerInDb.FirstName = _firstName;
                                customerInDb.LastName = _lastName;
                                customerInDb.Iban = _iban;
                                uow.Customers.Update(customerInDb);
                                await uow.SaveAsync();
                                Controller.CloseWindow(this);
                            }
                            catch (ValidationException ex)
                            {
                                if (ex.Value is IEnumerable<string> properties)
                                {
                                    foreach (var property in properties)
                                    {
                                        Errors.Add(property, new List<string> { ex.ValidationResult.ErrorMessage });
                                    }
                                }
                                else
                                {
                                    DbError = ex.ValidationResult.ToString();
                                }
                            }
                        },
                        canExecute: _ => Customer != null);
                }
                return _cmdSaveCommand;
            }
            set => _cmdSaveCommand = value;
        }


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IbanChecker.CheckIban(Iban))
            {
                yield return new ValidationResult($"Iban nicht gültig!", new string[] { nameof(Iban) });
            }
        }
    }
}
