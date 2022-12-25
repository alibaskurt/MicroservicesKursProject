using FreeCourse.Services.Catalog.Services.Abstract;
using FreeCourse.Services.Catalog.Services.Concreate;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Tüm class seviyesinde Authorize atturibute eklendi.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
    
    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Program.cs altýndaki Profile clasýn'ý inherit alan mapping class'larýný bulup IOC ye ekledim.
builder.Services.AddAutoMapper(typeof(Program));

//appsettings klasorundeki databasesetting property'si ile DatabaseSetting sýnýfýný doldurdum.
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

//Herhangi bir sýnýf yapýcý metodunda IDatabaseSettings geçersek DatabaseSettings sýnýftan bir nesne gelicek.
builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICourseService, CourseService>();


//Microservisi koruma altýna aldým 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token daðýtmaktan sorumlu arkadasý veriyoruz. Býzým Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanmýþ bir token geldiðinde public key ile doðrulama yapýcak.
    //Public keyi de belirttiðimiz URL üzerinden alýcak.
    options.Authority = builder.Configuration["IdentityServerURL"];

    //Gelen token aud içerisinde resource_catalog olmasý gerekiyor.
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
