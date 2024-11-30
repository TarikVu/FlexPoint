using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class FlexPointDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserExercises> UserExercises { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string solutionRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                                          .Parent!
                                          .Parent!
                                          .Parent!
                                          .Parent!
                                          .FullName;

        string dbPath = Path.Combine(solutionRoot, "Backend", "users.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}")
                      .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserExercises>().ToTable("UserExercises");

        modelBuilder.Entity<UserExercises>()
            .HasKey(ue => new { ue.UserId, ue.ExerciseId });  

        modelBuilder.Entity<UserExercises>()
            .HasOne(ue => ue.User)
            .WithMany(u => u.UserExercises)
            .HasForeignKey(ue => ue.UserId);

        modelBuilder.Entity<UserExercises>()
            .HasOne(ue => ue.Exercise)
            .WithMany(e => e.UserExercises)
            .HasForeignKey(ue => ue.ExerciseId);
 
    }
}
