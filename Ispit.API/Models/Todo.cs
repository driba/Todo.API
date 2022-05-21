using System.ComponentModel.DataAnnotations;

namespace Ispit.API.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Exceeded maximum Title length of 100 characters")]
        public string Title { get; set; }

        [StringLength(3000, ErrorMessage = "Exceeded maximum description length")]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;

    }
}
