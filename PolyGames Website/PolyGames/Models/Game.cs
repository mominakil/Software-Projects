using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class Game
    {
        //Allows changing the view on the Game.cshtml page to the editable view
        public bool IsEditable { get; set; }
        public bool IsHidden { get; set; }
        //public int MemberId { get; set; }

        //Game Attributes
        public int Id { get; set; }
        [Required]
        public string GameName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Year { get; set; }


        //Video Attributes
        public List<VideoFiles> GameVideos { get; set; }
        public HttpPostedFileBase videoUpload { get; set; }


        //Picture Attributes
        public int pictureIdOne { get; set; }
        public String pictureFileNameOne { get; set; }
        public Nullable<int> pictureFileSizeOne { get; set; }
        public String pictureFilePathOne { get; set; }
        public IEnumerable<HttpPostedFileBase> picturesUpload { get; set; }
        public List<PictureFiles> GamePictures { get; set; }


        //Executable File Attributes
        public int executableId { get; set; }
        public String executableFileName { get; set; }
        public Nullable<int> executableFileSize { get; set; }
        public String executableFilePath { get; set; }
        public HttpPostedFileBase executableUpload { get; set; }


        //Group Attributes
        public int GroupId { get; set; }
        public string GroupName { get; set; }


        //Group Member Attributes
        public int studentIdOne { set; get; }
        public String studentNameOne { get; set; }
        public String studentRoleOne { get; set; }
        public int studentIdTwo { set; get; }
        public String studentNameTwo { get; set; }
        public String studentRoleTwo { get; set; }
        public int studentIdThree { set; get; }
        public String studentNameThree { get; set; }
        public String studentRoleThree { get; set; }
        public int studentIdFour { set; get; }
        public String studentNameFour { get; set; }
        public String studentRoleFour { get; set; }
        public int studentIdFive { set; get; }
        public String studentNameFive { get; set; }
        public String studentRoleFive { get; set; }
        public int studentIdSix { set; get; }
        public String studentNameSix { get; set; }
        public String studentRoleSix { get; set; }
        public List<Student> groupMembers { get; set; }
        public int originalNumberOfMembers { get; set; }


        //Constructors
        public Game() { }

        //Constructor for get games by year query
        public Game(int id, string title, string description, string pFilePath, int year)
        {
            Id = id;
            GameName = title;
            Description = description;
            pictureFilePathOne = pFilePath;
            Year = year;
        }

        //Constructor for most recent game query
        public Game(int id, string title, string description, int year, string vFilePath)
        {
            Id = id;
            GameName = title;
            Description = description;
            Year = year;
            //videoFilePath = vFilePath;
        }

        //Constructor for getGamesOrderedByMostRecentlyAdded query
        public Game(int id, string title, string description, string picPath, string vidPath)
        {
            Id = id;
            GameName = title;
            Description = description;
            pictureFilePathOne = picPath;
            //videoFilePath = vidPath;
        }

        //Constructor for getGamesOrderedByMostRecentlyAdded query
        //public Game(int id, string title, string description)
        //{
        //    Id = id;
        //    GameName = title;
        //    Description = description;
        //    //pictureFilePathOne = picPath;
        //    //videoFilePath = vidPath;
        //}

        //Constructor for getAllYears query
        public Game(int year)
        {
            Year = year;
        }

        //Constructor for getAllGames query
        public Game(int id, string title, string description, string pFilePath)
        {
            Id = id;
            GameName = title;
            Description = description;
            pictureFilePathOne = pFilePath;
        }

        //Constructor for getAllGames query
        public Game(int id, string title, string description)
        {
            Id = id;
            GameName = title;
            Description = description;
        }

        //Constructor for getGameById query
        public Game(int id, string title, int groupId, string description, int year, string groupName, int vId, string videoPath,
            int exId, string executablePath)
        {
            Id = id;
            GameName = title;
            GroupId = groupId;
            Description = description;
            Year = year;
            GroupName = groupName;
            //videoId = vId;
            //videoFilePath = videoPath;
            executableId = exId;
            executableFilePath = executablePath;
        }

        public Game(int gameId, int groupId, int exeId, String groupName, String gameName, int year, String exeFileName, int exeFileSize)
        {
            Id = gameId;
            GroupId = groupId;
            executableId = exeId;
            GroupName = groupName;
            GameName = gameName;
            Year = year;
            executableFileName = exeFileName;
            executableFileSize = exeFileSize;
        }


        //Constructor for just Group Name and Id used inside getUserDataById 
        public Game(string groupName, int groupId, Boolean isHidden)
        {
            GroupName = groupName;
            GroupId = groupId;
            IsHidden = isHidden;
        }

        //Constructor for getUserDataById 
        public Game(int id, string groupName, string title, int groupId, Boolean isHidden)
        {
            Id = id;
            GroupName = groupName;
            GameName = title;
            GroupId = groupId;
            IsHidden = isHidden;
        }
    }
}