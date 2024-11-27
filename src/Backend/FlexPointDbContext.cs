using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class FlexPointDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string solutionRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)! 
                                          .Parent!
                                          .Parent! 
                                          .Parent!
                                          .Parent! 
                                          .FullName;

        string dbPath = Path.Combine(solutionRoot, "src", "Backend", "users.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}")
                      .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserExercise>().ToTable("UserExercises");


        modelBuilder.Entity<UserExercise>()
            .HasKey(ue => new { ue.UserId, ue.ExerciseId }); // Composite Key

        modelBuilder.Entity<UserExercise>()
            .HasOne(ue => ue.User)
            .WithMany(u => u.UserExercises)
            .HasForeignKey(ue => ue.UserId);

        modelBuilder.Entity<UserExercise>()
            .HasOne(ue => ue.Exercise)
            .WithMany(e => e.UserExercises)
            .HasForeignKey(ue => ue.ExerciseId);

        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "John Doe" }
        );

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise { ExerciseId = "PushUp", Name = "Push-Up", GifUrl = "https://example.com/pushup.gif", Instructions = new List<string> { "Do a push-up." } }
        );

    }
}
