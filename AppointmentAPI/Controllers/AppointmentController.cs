using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using AppointmentAPI.Objects;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;
using Microsoft.AspNetCore.Http;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private BookingContext context 
        { 
            get
            {
                return HttpContext.RequestServices.GetService(typeof(BookingContext)) as BookingContext;
            } 
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<BookObject>> GetFreeBooks()
        {
            var responce = context.GetAllAppointments();

            return responce.Item2.StatusCode == 200 ? Ok(responce.Item1
                .Where(book => book.IsReserved == false)) : responce.Item2;
        }

        [HttpGet("reservered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<BookObject>> GetReserveredBooks()
        {
            var responce = context.GetAllAppointments();

            return responce.Item2.StatusCode == 200 ? Ok(responce.Item1
                .Where(book => book.IsReserved == true)) : responce.Item2;
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<BookObject>> GetHistory()
        {
            var responce = context.GetAllHistory();
            return responce.Item2.StatusCode == 200 ? Ok(responce.Item1) : responce.Item2;
        }



        [HttpPost("{id}/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult NewAppointment(int id, string title, string author, DateTime date)
        {
            if (id < 0 || title == string.Empty)
            {
                return new ContentResult() { StatusCode = 400, ContentType = "Client error", Content = "One of the parameters has a wrong value." };
            }

            var responce = context.GetAllAppointments();

            if (responce.Item2.StatusCode != 200)
            {
                return responce.Item2;
            }

            if (responce.Item1.Find(book => book.Id == id) != null)
            {
                return new ContentResult() { StatusCode = 400, ContentType = "Client error", Content = $"Appointment with this ID-{id} already exist."};
            }

            BookObject book = new BookObject()
            {
                Id = id,
                Title = title,
                Author = author,
                IsReserved = false,
                Date = date
            };

            return context.Insert(book); ;
        }

        [HttpPost("{appointmentID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ReserveTime(int appointmentID, string reservetionComment, bool isReserved)
        {
            var responce = context.GetAllAppointments();

            if (responce.Item2.StatusCode != 200)
            {
                return responce.Item2;
            }

            if (responce.Item1.Find(book => book.Id == appointmentID) == null)
            {
                return new ContentResult() { StatusCode = 400, ContentType = "Client error", Content = $"Appointment with this ID-{appointmentID} does not exist." };
            }

            BookObject book = responce.Item1.Single(book => book.Id == appointmentID);
            book.IsReserved = isReserved;
            book.Comment = reservetionComment;

            context.StatusChange(new HistoryObject()
            {
                Id = appointmentID,
                StatusChange = isReserved,
                Date = book.Date
            });

            return context.Update(book);
        }
    }
}
