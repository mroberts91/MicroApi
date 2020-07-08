using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroApi;
using Microsoft.AspNetCore.Http.Json;
using MicroApi.Data;
using System.Net;

var Repo = new TodoRepository();
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
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
        });
    })
    .Build()
    .Run();