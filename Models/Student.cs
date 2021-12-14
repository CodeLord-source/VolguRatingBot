using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RatingBot.Models
{
    [Table("students")]
    public class Student
    {
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("id")]
        [Key]
        public int Id { get; private set; }

        [Column("chat_id")]
        [Required]
        public long ChatId { get; set; }

        [Column("login")]
        public string? Log { get; set; }

        [Column("password")]
        public string? Pass { get; set; }

        [Column("semester")]
        public int? Semester { get; set; }
    }
}
