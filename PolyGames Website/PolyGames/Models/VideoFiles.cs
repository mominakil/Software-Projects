using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    //Needed a model for video as you can have multiple videos per one game
    public class VideoFiles
    {
        public int videoId { get; set; }
        public String videoFileName { get; set; }
        public Nullable<int> videoFileSize { get; set; }
        public String videoFilePath { get; set; }
        public int gameID { get; set; }

        public VideoFiles() { }

        public VideoFiles(int id, string pName, string pPath, int gID)
        {
            videoId = id;
            videoFileName = pName;
            videoFilePath = pPath;
            gameID = gID;
        }
    }
}