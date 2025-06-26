using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoListProject.Data;
using TodoListProject.Models;

namespace TodoListProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly TodoRepository _todoRepository;

        public CategoryController(TodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = GetUserId();
            var categories = await _todoRepository.GetCategoriesByUserIdAsync(userId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            var userId = GetUserId();
            category.UserId = userId;
            category.CreatedDate = DateTime.UtcNow;
            var id = await _todoRepository.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategories), new { id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            var userId = GetUserId();
            if (id != category.Id) return BadRequest();
            category.UserId = userId;
            var updated = await _todoRepository.UpdateCategoryAsync(category);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = GetUserId();
            var deleted = await _todoRepository.DeleteCategoryAsync(id, userId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/order")]
        public async Task<IActionResult> UpdateCategoryOrder(int id, [FromBody] int orderIndex)
        {
            var userId = GetUserId();
            var updated = await _todoRepository.UpdateCategoryOrderAsync(id, orderIndex, userId);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderCategories([FromBody] List<CategoryOrderUpdateDto> updates)
        {
            var userId = GetUserId();
            foreach (var update in updates)
            {
                await _todoRepository.UpdateCategoryOrderAsync(update.Id, update.OrderIndex, userId);
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var userId = GetUserId();
            var categories = await _todoRepository.GetCategoriesByUserIdAsync(userId);
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        public class CategoryOrderUpdateDto
        {
            public int Id { get; set; }
            public int OrderIndex { get; set; }
        }
    }
} 