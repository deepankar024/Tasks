using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackFormWebApp.Models
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // Auto-increment
        public int Id { get; set; }

        [Required]  // NOT NULL constraint
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Submitted On")]
        public DateTime SubmittedAt { get; set; }

        public Feedback()
        {
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);

            // Create DateTime with Unspecified kind so EF stores exactly this time as-is
            SubmittedAt = DateTime.SpecifyKind(istTime, DateTimeKind.Unspecified);
        }
    }
}

//A simple C# class (a POCO - Plain Old CLR Object) that represents a single feedback entry.
//Each property (Id, Name, Email, etc.) corresponds to a column in the database table.