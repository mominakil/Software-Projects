using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace PolyGames.Models
{
    public partial class AssignTeamForDll : DbContext
    {
        public AssignTeamForDll()
            : base("name=AssignTeamForDll")
        {
        }

        public virtual DbSet<ExecutableFile> ExecutableFiles { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<MemberToGroup> MemberToGroups { get; set; }
        public virtual DbSet<PictureFile> PictureFiles { get; set; }
        public virtual DbSet<PolyGamesGroup> PolyGamesGroups { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<VideoFile> VideoFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(e => e.GameName)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.GameDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.ExecutableFiles)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.PictureFiles)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasMany(e => e.VideoFiles)
                .WithRequired(e => e.Game)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MemberToGroup>()
                .Property(e => e.studentRole)
                .IsFixedLength();

            modelBuilder.Entity<PolyGamesGroup>()
                .Property(e => e.GroupName)
                .IsUnicode(false);

            modelBuilder.Entity<PolyGamesGroup>()
                .HasMany(e => e.MemberToGroups)
                .WithRequired(e => e.PolyGamesGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<UserLogin>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<UserLogin>()
                .HasMany(e => e.MemberToGroups)
                .WithRequired(e => e.UserLogin)
                .WillCascadeOnDelete(false);
        }
    }
}
