using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class ALLInOneModel
    {
        public User user { get; set; }
        public Groups groups { get; set; }
        public Student student { get; set; }
        public Filters filters { get; set; }
        public Games games { get; set; }

        public List<string> passwordCheck { get; set; }

    }
}