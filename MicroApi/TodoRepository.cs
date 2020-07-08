using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace MicroApi.Data
{
    public class TodoRepository
    {
        private readonly List<TodoList> Database;
        private long nextId = 0;
        private long GetNextId() => ++nextId;


        public TodoRepository()
        {
            Database = new List<TodoList>();
            Seed();
        }

        public async Task<List<TodoList>> GetTodoListsAsync()
        {
            return await Task.FromResult(Database);
        }

        public TodoList? GetTodoList(long listId)
        {
            return  Database.FirstOrDefault(t => t.Id == listId);
        }

        public async Task<TodoList?> GetTodoListAsync(long listId)
        {
            return await Task.FromResult(Database.FirstOrDefault(t => t.Id == listId));
        }

        public TodoList? CreateTodoList(TodoList todoList)
        {
            todoList.Id = GetNextId();
            todoList.Items ??= new List<TodoItem>();
            Database.Add(todoList);
            return GetTodoList(todoList.Id)
                ?? throw new DataException($"Unable to find {nameof(TodoList)} with ID {todoList.Id}");
        }

        public async Task<TodoList> CreateTodoListAsync(TodoList todoList)
        {
            todoList.Id = GetNextId();
            todoList.Items ??= new List<TodoItem>();
            Database.Add(todoList);
            return await GetTodoListAsync(todoList.Id)
                ?? throw new DataException($"Unable to find {nameof(TodoList)} with ID {todoList.Id}");
        }

        public async Task<TodoList> UpdateTodoListAsync(TodoList todoList)
        {
            var list = await GetTodoListAsync(todoList.Id) 
                ?? throw new DataException($"Unable to find {nameof(TodoList)} with ID {todoList.Id}");

            list.Items = todoList.Items;

            var rtn = await GetTodoListAsync(todoList.Id)
                ?? throw new DataException($"Unable to find {nameof(TodoList)} with ID {todoList.Id}"); ;
            return rtn;
        }

        public async Task DeleteTodoListAsync(long listId)
        {
            await Task.Run(() =>
            {
                var list = Database.FirstOrDefault(t => t.Id == listId)
                    ?? throw new DataException($"Unable to find {nameof(TodoList)} with ID {listId}");
                Database.Remove(list);
            });
        }

        public void Seed()
        {
            var items = new List<TodoItem>
            {
                new TodoItem { Title = "Pay Electric Bill", Description = "Need to pay the bill." },
                new TodoItem { Title = "Car Wash", Description = "Need to wash the car." }
            };
            var list = new TodoList
            {
                Items = items
            };

            CreateTodoList(list);
        }
    }

    public record TodoList
    {
        public long Id { get; set; }
        public List<TodoItem>? Items { get; set; }
    }

    public record TodoItem
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
    }
}

// Current bug in .NET 5 preview. Required for init only properties.
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
