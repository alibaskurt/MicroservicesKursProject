using FreeCourse.Services.Catalog.Services.Abstract;
using FreeCourse.Services.Catalog.Services.Concreate;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//T�m class seviyesinde Authorize atturibute eklendi.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
    
    
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
builder.Services.AddScoped<ICourseService, CourseService>();


//Microservisi koruma alt�na ald�m 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token da��tmaktan sorumlu arkadas� veriyoruz. B�z�m Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanm�� bir token geldi�inde public key ile do�rulama yap�cak.
    //Public keyi de belirtti�imiz URL �zerinden al�cak.
    options.Authority = builder.Configuration["IdentityServerURL"];

    //Gelen token aud i�erisinde resource_catalog olmas� gerekiyor.
    options.Audience = "resource_catalog";

    options.RequireHttpsMetadata = false;

});

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
