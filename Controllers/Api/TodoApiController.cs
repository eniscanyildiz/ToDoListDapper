using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoListProject.Data;
using TodoListProject.Models;

namespace TodoListProject.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoApiController : ControllerBase
    {
        private readonly TodoRepository _todoRepository;

        public TodoApiController(TodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var userId = GetUserId();
            var todos = await _todoRepository.GetTodoItemsByUserIdAsync(userId);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            var userId = GetUserId();
            var todo = await _todoRepository.GetTodoItemByIdAsync(id, userId);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] TodoItem todo)
        {
            var userId = GetUserId();
            todo.UserId = userId;
            todo.CreatedDate = DateTime.UtcNow;
            var id = await _todoRepository.CreateTodoItemAsync(todo);
            return CreatedAtAction(nameof(GetTodo), new { id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoItem todo)
        {
            var userId = GetUserId();
            if (id != todo.Id) return BadRequest();
            todo.UserId = userId;
            var updated = await _todoRepository.UpdateTodoItemAsync(todo);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = GetUserId();
            var deleted = await _todoRepository.DeleteTodoItemAsync(id, userId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/order")]
        public async Task<IActionResult> UpdateTodoOrder(int id, [FromBody] int orderIndex)
        {
            var userId = GetUserId();
            var updated = await _todoRepository.UpdateTodoItemOrderAsync(id, orderIndex, userId);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderTodos([FromBody] List<TodoOrderUpdateDto> updates)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            foreach (var update in updates)
            {
                await _todoRepository.UpdateTodoItemOrderAsync(update.Id, update.OrderIndex, userId);
            }
            return NoContent();
        }

        public class TodoOrderUpdateDto
        {
            public int Id { get; set; }
            public int OrderIndex { get; set; }
        }
    }
} 