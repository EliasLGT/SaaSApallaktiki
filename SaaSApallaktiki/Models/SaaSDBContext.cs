using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SaaSApallaktiki.Models;

public partial class SaaSDBContext : DbContext
{
    public SaaSDBContext()
    {
    }

    public SaaSDBContext(DbContextOptions<SaaSDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<Hobby> Hobbies { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-IMF8D9T\\MSSQLSERVER03;Database=SaaSDB;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D718719B5474D");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__FriendRe__33A8519AE2863896");

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendRequestReceivers).HasConstraintName("FK__FriendReq__Recei__0C85DE4D");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequestSenders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__FriendReq__Sende__0B91BA14");
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.HasKey(e => e.HobbyId).HasName("PK__Hobbies__0ABE0BEF7DB8F404");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037CEFF4FD1D");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Messages__Sender__151B244E");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32DAFE8E1C");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Notificat__UserI__18EBB532");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA1260383D802CA4");

            entity.HasOne(d => d.Course).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Posts__CourseID__08B54D69");

            entity.HasOne(d => d.Hobby).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Posts__HobbyID__07C12930");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Posts__UserID__06CD04F7");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Teams__123AE7B9CE2A6721");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => new { e.TeamId, e.UserId }).HasName("PK__TeamMemb__C3426B73E98A1535");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers).HasConstraintName("FK__TeamMembe__TeamI__114A936A");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers).HasConstraintName("FK__TeamMembe__UserI__123EB7A3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC30F425C2");

            entity.HasMany(d => d.Courses).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .HasConstraintName("FK__UserCours__Cours__00200768"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserCours__UserI__7F2BE32F"),
                    j =>
                    {
                        j.HasKey("UserId", "CourseId").HasName("PK__UserCour__7B1A1BB475898DF4");
                        j.ToTable("UserCourses");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("CourseId").HasColumnName("CourseID");
                    });

            entity.HasMany(d => d.Hobbies).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserHobby",
                    r => r.HasOne<Hobby>().WithMany()
                        .HasForeignKey("HobbyId")
                        .HasConstraintName("FK__UserHobbi__Hobby__03F0984C"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserHobbi__UserI__02FC7413"),
                    j =>
                    {
                        j.HasKey("UserId", "HobbyId").HasName("PK__UserHobb__F7232C12EF49B090");
                        j.ToTable("UserHobbies");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("HobbyId").HasColumnName("HobbyID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
