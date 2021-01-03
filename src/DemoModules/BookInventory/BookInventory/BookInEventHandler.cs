using BookInventory.DAL;
using CoolCat.Core.Contracts;
using Newtonsoft.Json;
using System;

namespace BookInventory
{
    public class BookInEventHandler : INotificationHandler
    {
        private BookDAL _bookDAL = null;

        public BookInEventHandler(IDbConnectionFactory dbConnectionFactory)
        {
            _bookDAL = new BookDAL(dbConnectionFactory);
        }

        public void Handle(string data)
        {
            var obj = JsonConvert.DeserializeObject<BookInEvent>(data);
            _bookDAL.UpdateBookStatus(obj.BookId, true);
        }
    }

    public class BookInEvent
    {
        public Guid BookId { get; set; }

        public DateTime InDate { get; set; }
    }

}
