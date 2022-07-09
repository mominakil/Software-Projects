using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class Users
    {

        public List<User> Items { get; set; }
        public int EditIndex { get; set; }

        public Users()
        {
            //Default Constructor
        }
    }
}