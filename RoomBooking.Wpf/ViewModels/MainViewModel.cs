﻿using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Booking _currentBooking;
        private Room _currentRoom;
        private ObservableCollection<Room> _rooms;
        private ObservableCollection<Booking> _bookings;
        private ICommand _cmdEditCustomerCommand;

        public Booking CurrentBooking
        {
            get => _currentBooking;
            set
            {
                _currentBooking = value;
                OnPropertyChanged(nameof(CurrentBooking));
            }
        }

        public ObservableCollection<Room> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }


        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                _currentRoom = value;
                OnPropertyChanged(nameof(CurrentRoom));
                LoadDataAsync();
            }
        }


        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        private async Task LoadDataAsync()
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                if (Rooms == null)
                {
                    var rooms = (await uow.Rooms
                                         .GetAllAsync())
                                         .OrderBy(r => r.RoomNumber)
                                         .ToList();
                    CurrentRoom = rooms.First();
                    Rooms = new ObservableCollection<Room>(rooms);
                }
                var bookings = (await uow.Bookings
                                         .GetByRoomWithCustomerAsync(CurrentRoom.Id))
                                         .OrderBy(b => b.From)
                                         .ThenBy(b => b.To)
                                         .ToList();
                Bookings = new ObservableCollection<Booking>(bookings);
            }
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        public ICommand CmdEditCustomerCommand
        {
            get
            {
                if (_cmdEditCustomerCommand == null)
                {
                    _cmdEditCustomerCommand = new RelayCommand(
                        execute: _ =>
                        {
                            Controller.ShowWindow(new EditCustomerModel(Controller, CurrentBooking.Customer),true);
                        },
                        canExecute: _ => CurrentBooking != null
                    );
                }
                return _cmdEditCustomerCommand;
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
