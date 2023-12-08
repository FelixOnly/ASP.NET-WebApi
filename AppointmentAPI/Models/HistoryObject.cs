using System;

namespace AppointmentAPI.Objects
{
    public class HistoryObject
    {
        private BookingContext context;

        public int Id { get; set; }
        
        public bool StatusChange { get; set; }

        public DateTime Date { get; set; }
    }
}
