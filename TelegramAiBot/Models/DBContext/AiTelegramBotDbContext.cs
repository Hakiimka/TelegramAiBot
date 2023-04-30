using Microsoft.EntityFrameworkCore;

namespace TelegramAiBot.Models.DBContext;

public partial class AiTelegramBotDbContext : DbContext
{
    private readonly string _connectionString;
    public AiTelegramBotDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AiTelegramBotDbContext(DbContextOptions<AiTelegramBotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MessageSequence> MessageSequences { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MessageSequence>(entity =>
        {

            entity.HasKey(e => e.Id);
            entity
            
                .ToTable("message_sequences");

            


            entity.Property(e => e.Id)

                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.MessageText)
                .HasColumnType("text")
                .HasColumnName("message_text");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {

            entity.HasKey(e => e.Id);
            entity
               
                .ToTable("users");

            entity.Property(e => e.FirstName)
                .HasColumnType("text")
                .HasColumnName("first_name");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.LanguageCode)
                .HasColumnType("text")
                .HasColumnName("language_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasColumnType("text")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
