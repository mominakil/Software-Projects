using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class Groups
    {
        public string GroupName { get; set; }

        public int GroupId { get; set; }

        public List<Groups> GroupList;

        public List<int> ids { get; set; }


        public Groups() { }
        public Groups(int groupId, string groupName) {
            GroupId = groupId;
            GroupName = groupName;
        }
    }
}