using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    //Needed a model for pictures as you can have multiple pictures per one game
    public class PictureFiles
    {
        public int pictureId { get; set; }
        public String pictureFileName { get; set; }
        public Nullable<int> pictureFileSize { get; set; }
        public String pictureFilePath { get; set; }
        public int gameID { get; set; }

        public PictureFiles() { }

        public PictureFiles(int id, string pName, string pPath, int gID)
        {
            pictureId = id;
            pictureFileName = pName;
            pictureFilePath = pPath;
            gameID = gID;
        }
    }
}