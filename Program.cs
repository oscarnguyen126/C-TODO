using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDb")));

builder.Services.AddControllers();
var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();
