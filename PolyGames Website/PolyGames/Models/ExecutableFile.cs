namespace PolyGames.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ExecutableFile
    {
        [Key]
        public int executableId { get; set; }

        [StringLength(100)]
        public string executableFileName { get; set; }

        public int? executableFileSize { get; set; }

        [StringLength(100)]
        public string executableFilePath { get; set; }

        public int gameID { get; set; }

        public virtual Game Game { get; set; }
    }
}
