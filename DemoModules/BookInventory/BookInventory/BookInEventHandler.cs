using Mystique.Core.Contracts;
using Newtonsoft.Json;
using System;

namespace BookInventory
{
    public class BookInEventHandler : INotificationHandler
    {
        public void Handle(string data)
        {
            var obj = JsonConvert.DeserializeObject<BookInEvent>(data);
        }
    }

    public class BookInEvent
    {
        public Guid BookId { get; set; }

        public DateTime InDate { get; set; }
    }

}
