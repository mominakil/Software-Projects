namespace PolyGames.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VideoFile
    {
        [Key]
        public int videoId { get; set; }

        [StringLength(100)]
        public string videoFileName { get; set; }

        public int? videoFileSize { get; set; }

        [StringLength(100)]
        public string videoFilePath { get; set; }

        public int gameID { get; set; }

        public virtual Game Game { get; set; }
    }
}
