using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class Games
    {
        //Holds a list of Games for queries that return multiple games
        public List<Game> Items { get; set; }
        public Games() { }
        public int EditIndex { get; set; }
    }
}