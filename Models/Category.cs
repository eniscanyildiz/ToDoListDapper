using System.ComponentModel.DataAnnotations;

namespace TodoListProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(7)] // Hex color code (#FFFFFF)
        public string Color { get; set; } = "#007bff";
        
        public int OrderIndex { get; set; } = 0;
        
        public string UserId { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<TodoItem>? TodoItems { get; set; }
    }
} 