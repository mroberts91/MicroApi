using MicroApi.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroApi
{
    public static class TodoRequestExtensions
    {
        public static long GetListId(this HttpRequest req)
        {
            if (!long.TryParse((string)req.RouteValues["id"], out var id))
                throw new InvalidCastException("Parameter id was not present in the request.");
            return id;
        }

        public static async Task<TodoList> GetTodoRequestBody(this HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            return JsonSerializer.Deserialize<TodoList>(await reader.ReadToEndAsync())
                ?? throw new NullReferenceException($"{nameof(TodoList)} object was not present in the request body.");
        }
    }
}
