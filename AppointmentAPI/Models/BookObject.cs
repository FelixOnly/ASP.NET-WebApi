using System;

namespace AppointmentAPI.Objects
{
    public class BookObject
    {
        private BookingContext context;

        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Author { get; set; }
        
        public string Comment { get; set; }
        
        public bool IsReserved { get; set; }
        
        public DateTime Date { get; set; }

    }
}
