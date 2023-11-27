using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppServer.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
