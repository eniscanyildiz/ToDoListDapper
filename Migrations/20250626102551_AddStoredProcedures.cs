using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListProject.Migrations
{
    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryColor",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetTodoItemsByUserId
                @UserId NVARCHAR(450)
            AS
            BEGIN
                SELECT t.*, c.Name as CategoryName, c.Color as CategoryColor
                FROM TodoItems t
                LEFT JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.UserId = @UserId
                ORDER BY t.OrderIndex
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetTodoItemById
                @Id INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                SELECT t.*, c.Name as CategoryName, c.Color as CategoryColor
                FROM TodoItems t
                LEFT JOIN Categories c ON t.CategoryId = c.Id
                WHERE t.Id = @Id AND t.UserId = @UserId
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CreateTodoItem
                @Title NVARCHAR(200),
                @Description NVARCHAR(1000),
                @IsCompleted BIT,
                @CreatedDate DATETIME,
                @OrderIndex INT,
                @CategoryId INT = NULL,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                INSERT INTO TodoItems (Title, Description, IsCompleted, CreatedDate, OrderIndex, CategoryId, UserId)
                VALUES (@Title, @Description, @IsCompleted, @CreatedDate, @OrderIndex, @CategoryId, @UserId);
                SELECT SCOPE_IDENTITY() AS Id;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_UpdateTodoItem
                @Id INT,
                @Title NVARCHAR(200),
                @Description NVARCHAR(1000),
                @IsCompleted BIT,
                @CompletedDate DATETIME = NULL,
                @OrderIndex INT,
                @CategoryId INT = NULL,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                UPDATE TodoItems
                SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted,
                    CompletedDate = @CompletedDate, OrderIndex = @OrderIndex, CategoryId = @CategoryId
                WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_DeleteTodoItem
                @Id INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                DELETE FROM TodoItems WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_UpdateTodoItemOrder
                @Id INT,
                @OrderIndex INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                UPDATE TodoItems SET OrderIndex = @OrderIndex WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_ReorderTodos
                @UserId NVARCHAR(450),
                @OrderList NVARCHAR(MAX)
            AS
            BEGIN
                -- OrderList: JSON array [{Id,OrderIndex}]
                DECLARE @json NVARCHAR(MAX) = @OrderList;
                UPDATE t
                SET t.OrderIndex = j.OrderIndex
                FROM TodoItems t
                JOIN OPENJSON(@json)
                WITH (Id INT, OrderIndex INT) j ON t.Id = j.Id
                WHERE t.UserId = @UserId;
            END
            ");

            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetCategoriesByUserId
                @UserId NVARCHAR(450)
            AS
            BEGIN
                SELECT * FROM Categories WHERE UserId = @UserId ORDER BY OrderIndex
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetCategoryById
                @Id INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                SELECT * FROM Categories WHERE Id = @Id AND UserId = @UserId
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CreateCategory
                @Name NVARCHAR(100),
                @Color NVARCHAR(7),
                @OrderIndex INT,
                @UserId NVARCHAR(450),
                @CreatedDate DATETIME
            AS
            BEGIN
                INSERT INTO Categories (Name, Color, OrderIndex, UserId, CreatedDate)
                VALUES (@Name, @Color, @OrderIndex, @UserId, @CreatedDate);
                SELECT SCOPE_IDENTITY() AS Id;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_UpdateCategory
                @Id INT,
                @Name NVARCHAR(100),
                @Color NVARCHAR(7),
                @OrderIndex INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                UPDATE Categories SET Name = @Name, Color = @Color, OrderIndex = @OrderIndex
                WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_DeleteCategory
                @Id INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                DELETE FROM Categories WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_UpdateCategoryOrder
                @Id INT,
                @OrderIndex INT,
                @UserId NVARCHAR(450)
            AS
            BEGIN
                UPDATE Categories SET OrderIndex = @OrderIndex WHERE Id = @Id AND UserId = @UserId;
                SELECT @@ROWCOUNT;
            END
            ");
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_ReorderCategories
                @UserId NVARCHAR(450),
                @OrderList NVARCHAR(MAX)
            AS
            BEGIN
                -- OrderList: JSON array [{Id,OrderIndex}]
                DECLARE @json NVARCHAR(MAX) = @OrderList;
                UPDATE c
                SET c.OrderIndex = j.OrderIndex
                FROM Categories c
                JOIN OPENJSON(@json)
                WITH (Id INT, OrderIndex INT) j ON c.Id = j.Id
                WHERE c.UserId = @UserId;
            END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryColor",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "TodoItems");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTodoItemsByUserId");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTodoItemById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateTodoItem");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateTodoItem");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteTodoItem");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateTodoItemOrder");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ReorderTodos");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetCategoriesByUserId");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetCategoryById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CreateCategory");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateCategory");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteCategory");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateCategoryOrder");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ReorderCategories");
        }
    }
}
