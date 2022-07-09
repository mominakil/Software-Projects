namespace PolyGames.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PictureFile
    {
        [Key]
        public int pictureID { get; set; }

        [StringLength(100)]
        public string pictureFileName { get; set; }

        public int? pictureFileSize { get; set; }

        [StringLength(100)]
        public string pictureFilePath { get; set; }

        public int gameID { get; set; }

        public virtual Game Game { get; set; }
    }
}
