using Microsoft.AspNetCore.Identity;

namespace TodoListProject.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
} 