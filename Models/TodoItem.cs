using System.ComponentModel.DataAnnotations;

namespace TodoListProject.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedDate { get; set; }
        
        public int OrderIndex { get; set; } = 0;
        
        public int? CategoryId { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual User? User { get; set; }

        // Dapper projection için
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
    }
} 