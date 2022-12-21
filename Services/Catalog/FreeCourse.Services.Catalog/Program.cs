using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Program.cs alt�ndaki Profile clas�n'� inherit alan mapping class'lar�n� bulup IOC ye ekledim.
builder.Services.AddAutoMapper(typeof(Program));

//appsettings klasorundeki databasesetting property'si ile DatabaseSetting s�n�f�n� doldurdum.
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

//Herhangi bir s�n�f yap�c� metodunda IDatabaseSettings ge�ersek DatabaseSettings s�n�ftan bir nesne gelicek.
builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

builder.Services.AddScoped<ICategoryService, CategoryService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
