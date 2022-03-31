using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
   public class Action
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserType { get; set; }

        public DateTime Time { get; set; }

        public string ActionText { get; set; }
    }
}
