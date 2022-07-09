using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class User
    {
        public List<User> Items { get; set; }
        public int EditIndex { get; set; }
        public bool IsEditable { get; set; }
        public string Email { set; get; }
        public string Password { set; get; }
        public Boolean IsActive { set; get; }
        public string Name { set; get; }
        public int MemberID { set; get; }
        public Boolean IsAdmin { set; get; }
        public Boolean IsComp214 { set; get; }

        [DataType(DataType.Date)]
        public DateTime? RegistrationDate { get; set; }
        public int RegistrationYear { get; set; }
        public int regYear{get; set;}
        public Groups CurrentTeamName { set; get; }

        public User(string em, string pw)
        {
            Email = em;
            Password = pw;

        }
        
        public User(string em, string pw, Boolean admin)
        {
            Email = em;
            Password = pw;
            IsAdmin = admin;
        }

        public User(string em, string pw, string name, Boolean isAdmin, int id)
        {
            Email = em;
            Password = pw;
            IsAdmin = isAdmin;
            MemberID = id;
            Name = name;
        }

        public User(int id, string name, string email, string password, Boolean isActive, Boolean isAdmin)
        {   
            MemberID = id;
            Name = name;
            Email = email;
            Password = password;
            IsAdmin = isAdmin;
            IsActive = isActive;
        }

        public User(int id, string name, string email, string password, Boolean isActive, Boolean isAdmin, DateTime registrationDate)
        {
            MemberID = id;
            Name = name;
            Email = email;
            Password = password;
            IsAdmin = isAdmin;
            IsActive = isActive;
            RegistrationDate = registrationDate;
        }

        public User(string em, string pw, string name, Boolean isAdmin, int id, Boolean isActive)
        {
            Email = em;
            Password = pw;
            IsAdmin = isAdmin;
            MemberID = id;
            Name = name;
            IsActive = isActive;
        }

        public User() { }



    }
}