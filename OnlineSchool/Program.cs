using Microsoft.EntityFrameworkCore;
using OnlineSchool.Models;

var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения
string? connectionString = builder.Configuration.GetConnectionString("SchoolConnection");
// Добавляем сервис SchoolContext
builder.Services.AddDbContext<SchoolContext>(options => options.UseSqlServer(connectionString));


// Add services to the container.
builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
