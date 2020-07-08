using MicroApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroApi
{
    public static class TodoEndpoints
    {
        private static readonly TodoRepository Repo = new TodoRepository();
        public static IApplicationBuilder DeclareTodoEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsJsonAsync("Micro CRUD API");
                });

                endpoints.MapGet("/todos", async context =>
                {
                    var lists = await Repo.GetTodoListsAsync();
                    context.Response.StatusCode = lists is null ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;
                    await context.Response.WriteAsJsonAsync(lists);
                });

                endpoints.MapGet("/todos/{id:int}", async context =>
                {
                    var id = context.Request.GetListId();
                    var list = await Repo.GetTodoListAsync(id);

                    context.Response.StatusCode = list is null ? (int)HttpStatusCode.NotFound : (int)HttpStatusCode.OK;
                    await context.Response.WriteAsJsonAsync(list);
                });

                endpoints.MapPost("/todos", async context =>
                {
                    var result = await Repo.CreateTodoListAsync(await context.Request.GetTodoRequestBody());

                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    await context.Response.WriteAsJsonAsync(result);
                });

                endpoints.MapPut("/todos", async context =>
                {
                    var result = await Repo.UpdateTodoListAsync(await context.Request.GetTodoRequestBody());

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsJsonAsync(result);
                });

                endpoints.MapDelete("/todos/{id:int}", async context =>
                {
                    var id = context.Request.GetListId();
                    await Repo.DeleteTodoListAsync(id);

                    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                });
            });
            return app;
        }

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
