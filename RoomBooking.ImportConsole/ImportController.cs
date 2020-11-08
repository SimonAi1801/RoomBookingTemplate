using RoomBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace RoomBooking.ImportConsole
{
    public static class ImportController
    {
        private const string _fileName = "Bookings.csv";
        /// <summary>
        /// Liest die Buchungen mit ihren Räumen und Kunden aus der
        /// csv-Datei ein.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Booking>> ReadBookingsFromCsvAsync()
        {
            string[][] matrix = await MyFile.ReadStringMatrixFromCsvAsync("bookings.csv", true);

            var customers = matrix
                .GroupBy(line => line[0] + ',' + line[1] + ',' + line[2])
                .Select(grp => new Customer()
                {
                    LastName = grp.Key.Split(',')[0],
                    FirstName = grp.Key.Split(',')[1],
                    Iban = grp.Key.Split(',')[2]
                })
                .ToList();

            var rooms = matrix
                .GroupBy(line => line[3])
                .Select(grp => new Room()
                {
                    RoomNumber = grp.Key
                })
                .ToList();

            Booking[] bookings = matrix
                .Select(line => new Booking()
                {
                    Customer = customers.Single(cus => cus.LastName.Equals(line[0]) && cus.FirstName.Equals(line[1])),
                    Room = rooms.Single(ro => ro.RoomNumber.Equals(line[3])),
                    From = line[4],
                    To = line[5]
                })
                .ToArray();

            return bookings;
        }
    }
}
