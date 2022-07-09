using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    //Needed a model for student as you can have multiple pictures per one game
    public class Student
    {
        public int memberId { set; get; }
        public int groupId { set; get; }
        public String studentName { get; set; }
        public String studentRole { get; set; }
        public bool isHidden { get; set; }

        public Student() { }
        public Student(int mid, int gid, string name, string role)
        {
            memberId = mid;
            groupId = gid;
            studentName = name;
            studentRole = role;
        }
    }
}