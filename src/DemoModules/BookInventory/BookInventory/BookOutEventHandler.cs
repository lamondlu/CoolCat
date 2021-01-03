using BookInventory.DAL;
using CoolCat.Core.Contracts;
using Newtonsoft.Json;
using System;

namespace BookInventory
{
    public class BookOutEventHandler : INotificationHandler
    {
        private BookDAL _bookDAL = null;

        public BookOutEventHandler(IDbConnectionFactory connectionFactory)
        {
            _bookDAL = new BookDAL(connectionFactory);
        }

        public void Handle(string data)
        {
            var obj = JsonConvert.DeserializeObject<BookOutEvent>(data);
            _bookDAL.UpdateBookStatus(obj.BookId, true);
        }
    }

    public class BookOutEvent
    {
        public Guid BookId { get; set; }

        public DateTime OutDate { get; set; }
    }

}
