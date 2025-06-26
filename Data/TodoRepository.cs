using Dapper;
using TodoListProject.Models;
using System.Data;

namespace TodoListProject.Data
{
    public class TodoRepository
    {
        private readonly DapperContext _context;

        public TodoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItemsByUserIdAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_GetTodoItemsByUserId";
            return await connection.QueryAsync<TodoItem>(sql, new { UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<TodoItem?> GetTodoItemByIdAsync(int id, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_GetTodoItemById";
            return await connection.QueryFirstOrDefaultAsync<TodoItem>(sql, new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateTodoItemAsync(TodoItem todoItem)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_CreateTodoItem";
            return await connection.QuerySingleAsync<int>(sql, todoItem, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateTodoItemAsync(TodoItem todoItem)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_UpdateTodoItem";
            var rowsAffected = await connection.ExecuteAsync(sql, todoItem, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteTodoItemAsync(int id, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_DeleteTodoItem";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateTodoItemOrderAsync(int id, int orderIndex, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_UpdateTodoItemOrder";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, OrderIndex = orderIndex, UserId = userId }, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_GetCategoriesByUserId";
            return await connection.QueryAsync<Category>(sql, new { UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_GetCategoryById";
            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateCategoryAsync(Category category)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_CreateCategory";
            return await connection.QuerySingleAsync<int>(sql, category, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_UpdateCategory";
            var rowsAffected = await connection.ExecuteAsync(sql, category, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_DeleteCategory";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId }, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCategoryOrderAsync(int id, int orderIndex, string userId)
        {
            using var connection = _context.CreateConnection();
            const string sql = "sp_UpdateCategoryOrder";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, OrderIndex = orderIndex, UserId = userId }, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
    }
} 