using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatAppServer1._0.Models
{
    public class Chat
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ChatId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
