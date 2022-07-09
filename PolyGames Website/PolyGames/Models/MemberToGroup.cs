using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class MemberToGroup
    {
        public int MemberId { get; set; }
        public int GroupId { get; set; }
        public string StudentRole { get; set; }

        public List<MemberToGroup> items { get; set; }

        public MemberToGroup(int memberId, int groupId, string studentRole)
        {
            MemberId = memberId;
            GroupId = groupId;
            StudentRole = studentRole;
        }

        public MemberToGroup(int memberId, int groupId)
        {
            MemberId = memberId;
            GroupId = groupId;
        }

        public MemberToGroup()
        {
            //Default Constructor
        }
    }
}