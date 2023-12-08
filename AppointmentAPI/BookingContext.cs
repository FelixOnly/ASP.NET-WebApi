using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using AppointmentAPI.Objects;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;

namespace AppointmentAPI
{
    public class BookingContext
    {
        #region Connection

        public string ConnectionString { get; set; }

        public BookingContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        #endregion

        public ContentResult Insert(BookObject book)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    if (!conn.Ping())
                    {
                        return new ContentResult { StatusCode = 500, ContentType = "Server is not responce" };
                    }

                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO Books (id, title, author, is_reserved, date) VALUES ('{book.Id}', '{book.Title}', '{book.Author}', {Convert.ToInt16(book.IsReserved)}, '{book.Date.ToString("yyyy-MM-dd HH:mm:ss")}');", conn);
                    cmd.ExecuteReader();

                    return new ContentResult { StatusCode = 200, ContentType = "Successful" };
                }
            }
            catch(Exception ex) 
            {
                return new ContentResult { StatusCode = 500, ContentType = "Server error response", Content = ex.Message };
            }
            
            
        }

        public ContentResult Update(BookObject book)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    if (!conn.Ping())
                    {
                        return new ContentResult { StatusCode = 500, ContentType = "Server is not responce" };
                    }

                    MySqlCommand cmd = new MySqlCommand(
                        $"UPDATE Books SET title = '{book.Title}', author = '{book.Author}', comment = '{book.Comment}', is_reserved = '{Convert.ToInt16(book.IsReserved)}', date = '{book.Date.ToString("yyyy-MM-dd HH:mm:ss")}'  WHERE id = {book.Id};", conn);
                    cmd.ExecuteReader();

                    return new ContentResult { StatusCode = 200, ContentType = "Successful" };
                }
            }
            catch(Exception ex)
            {
                return new ContentResult { StatusCode = 500, ContentType = "Server error response", Content = ex.Message };
            }


        }

        public ContentResult StatusChange(HistoryObject history)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    
                    if (!conn.Ping())
                    {
                        return new ContentResult { StatusCode = 500, ContentType = "Server is not responce" };
                    }

                    
                    MySqlCommand cmd = new MySqlCommand($"INSERT INTO History (id, status_change, date) VALUES ('{history.Id}', '{Convert.ToInt16(history.StatusChange)}', '{history.Date.ToString("yyyy-MM-dd HH:mm:ss")}');", conn);
                    cmd.ExecuteReader();

                    return new ContentResult { StatusCode = 200, ContentType = "Successful" };
                }
            }
            catch (Exception ex)
            {
                return new ContentResult { StatusCode = 500, ContentType = "Server error response", Content = ex.Message };
            }
        }

        public (List<HistoryObject>, ContentResult) GetAllHistory()
        {
            try
            {
                List<HistoryObject> list = new List<HistoryObject>();

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    if (!conn.Ping())
                    {
                        return (null, new ContentResult { StatusCode = 500, ContentType = "Server is not responce" });
                    }

                    MySqlCommand cmd = new MySqlCommand("select * from History", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HistoryObject()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                StatusChange = Convert.ToBoolean(reader["status_change"]),
                                Date = DateTime.Parse(reader["date"].ToString())
                            }); ;
                        }
                    }
                }
                return (list, new ContentResult { StatusCode = 200, ContentType = "Successful" });
            }
            catch (Exception ex)
            {
                return (null, new ContentResult { StatusCode = 500, ContentType = "Server error response", Content = ex.Message });
            }


        }

        public (List<BookObject>, ContentResult) GetAllAppointments()
        {
            try
            {
                List<BookObject> list = new List<BookObject>();

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    if (!conn.Ping())
                    {
                        return (null, new ContentResult { StatusCode = 500, ContentType = "Server is not responce" });
                    }

                    MySqlCommand cmd = new MySqlCommand("select * from Books", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new BookObject()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Title = reader["title"].ToString(),
                                Author = reader["author"].ToString(),
                                Comment = reader["comment"].ToString(),
                                IsReserved = Convert.ToBoolean(reader["is_reserved"]),
                                Date = DateTime.Parse(reader["date"].ToString())
                            }); ;
                        }
                    }
                }
                return (list, new ContentResult { StatusCode = 200, ContentType = "Successful" });
            }
            catch (Exception ex)
            {
                return (null, new ContentResult { StatusCode = 500, ContentType = "Server error response", Content = ex.Message });
            }
        }
    }
}
