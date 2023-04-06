//using Domain.Models.Customers;
using Domain.Models.Polls;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { } 
        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countryes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RegistrationCaruselItem> RegistrationCaruselItems { get; set; }
        public DbSet<ItemColor> ItemColors { get; set; }
        public DbSet<Story> Storyes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; } // Alex
        public DbSet<Topic> Topics { get; set; }
        public DbSet<StoryTopic> StoryTopics { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LikedUserStory> LikedUserStories { get; set; }
        public DbSet<ViewedUserStory> ViewedUserStories { get; set; }
        public DbSet<MessageUser> MessageUsers { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<ReportedUsers> ReportedUsers { get; set; }
        public DbSet<HidenUser> HidenUsers { get; set; }
        public DbSet<TailedUsers> TailedUsers { get; set; }
        public DbSet<TailedStoryes> TailedStoryes { get; set; }
        //public DbSet<Report> Reports { set; get; }
        public DbSet<Poll> Poll { get; set; } 
        public DbSet<Vote> Votes { set; get; } 
         
        public DbSet<AnsweredQuestion> AnsweredQuestion { get; set; }
        public DbSet<Word> Words { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportedUsers>()
             .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<User>()
                .HasMany(p => p.ReportedUsers)
                .WithOne(p => p.IUser)
                .HasForeignKey(p => p.LeftId);
                
            modelBuilder.Entity<BlockedUser>()
             .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<User>()
                .HasMany(p => p.BlockedUser)
                .WithOne(p => p.IUser)
                .HasForeignKey(p => p.LeftId);

            modelBuilder.Entity<HidenUser>()
              .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<User>()
                .HasMany(p => p.HidenUser)
                .WithOne(p => p.IUser)
                .HasForeignKey(p => p.LeftId);

            modelBuilder.Entity<TailedUsers>()
              .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<User>()
                .HasMany(p => p.TailedUsers)
                .WithOne(p => p.IUser)
                .HasForeignKey(p => p.LeftId);

            modelBuilder.Entity<TailedStoryes>()
              .HasKey(bc => new { bc.uId, bc.sId });

            modelBuilder.Entity<User>()
                .HasMany(p => p.TailedStoryes)
                .WithOne(p => p.IUser)
                .HasForeignKey(p => p.uId);

            //modelBuilder.Entity<HidenUser>()
            //   .HasKey(bc => new { bc.LeftId, bc.RightId });

            //modelBuilder.Entity<HidenUser>()
            //    .HasOne(bc => bc.IUser)
            //    .WithMany(b => b.HidenUser)
            //    .HasForeignKey(bc => bc.LeftId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<HidenUser>()
            //    .HasOne(bc => bc.OUser)
            //    .WithMany(c => c.HidenUser)
            //    .HasForeignKey(bc => bc.RightId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BlockedUser>()
            //   .HasKey(bc => new { bc.LeftId, bc.RightId });

            //modelBuilder.Entity<BlockedUser>()
            //    .HasOne(bc => bc.IUser)
            //    .WithMany(b => b.BlockedUser)
            //    .HasForeignKey(bc => bc.LeftId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BlockedUser>()
            //    .HasOne(bc => bc.OUser)
            //    .WithMany(c => c.BlockedUser)
            //    .HasForeignKey(bc => bc.RightId).OnDelete(DeleteBehavior.Restrict);


            //modelBuilder.Entity<MessageUser>()
            //   .HasKey(bc => new { bc.LeftId, bc.RightId });

            //modelBuilder.Entity<MessageUser>()
            //    .HasOne(bc => bc.IUser)
            //    .WithMany(b => b.MessageUser)
            //    .HasForeignKey(bc => bc.LeftId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<MessageUser>()
            //    .HasOne(bc => bc.OUser)
            //    .WithMany(c => c.MessageUser)
            //    .HasForeignKey(bc => bc.RightId).OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Comment>().HasOne(p => p.User).WithMany(p => p.Comments).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comment>().HasOne(p => p.Story).WithMany(p => p.Comments).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>().HasOne(p => p.User).WithMany(p => p.Notifications).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LikedUserStory>()
               .HasKey(bc => new { bc.LeftId, bc.RightId });        

            modelBuilder.Entity<LikedUserStory>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.LikedUserStory)
                .HasForeignKey(bc => bc.LeftId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LikedUserStory>()
                .HasOne(bc => bc.Story)
                .WithMany(c => c.LikedUserStory)
                .HasForeignKey(bc => bc.RightId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViewedUserStory>()
               .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<ViewedUserStory>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.ViewedUserStory)
                .HasForeignKey(bc => bc.LeftId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViewedUserStory>()
                .HasOne(bc => bc.Story)
                .WithMany(c => c.ViewedUserStory)
                .HasForeignKey(bc => bc.RightId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoryTopic>()
               .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<Poll>()
              .HasKey(bc => bc.ID);

            modelBuilder.Entity<AnsweredQuestion>()
             .HasKey(bc => bc.ID);           

            modelBuilder.Entity<AnsweredQuestion>()
              .HasOne(bc => bc.Poll)
                .WithMany(b => b.AnsweredQuestion)
                .HasForeignKey(bc => bc.QuestionId);

            modelBuilder.Entity<AnsweredQuestion>()
               .HasKey(bc => new { bc.ID, bc.QuestionId });

            modelBuilder.Entity<StoryTopic>()
                .HasOne(bc => bc.Story)
                .WithMany(b => b.StoryTopic)
                .HasForeignKey(bc => bc.LeftId);

            modelBuilder.Entity<StoryTopic>()
                .HasOne(bc => bc.Topic)
                .WithMany(c => c.StoryTopic)
                .HasForeignKey(bc => bc.RightId);
            modelBuilder.Entity<UserRole>()
               .HasKey(bc => new { bc.LeftId, bc.RightId });

            modelBuilder.Entity<UserRole>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserRole)
                .HasForeignKey(bc => bc.LeftId);

            modelBuilder.Entity<UserRole>()
                .HasOne(bc => bc.Role)
                .WithMany(c => c.UserRole)
                .HasForeignKey(bc => bc.RightId);
        }
    }
}
