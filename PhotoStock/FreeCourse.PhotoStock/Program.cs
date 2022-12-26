<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

>>>>>>> 9ec69e8db6b5c5aa767d48090463f294838d0d26
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

<<<<<<< HEAD
builder.Services.AddControllers();
=======
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
>>>>>>> 9ec69e8db6b5c5aa767d48090463f294838d0d26
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
=======

//Microservisi koruma altýna aldým 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token daðýtmaktan sorumlu arkadasý veriyoruz. Býzým Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanmýþ bir token geldiðinde public key ile doðrulama yapýcak.
    //Public keyi de belirttiðimiz URL üzerinden alýcak.
    options.Authority = builder.Configuration["IdentityServerURL"];

    //Gelen token aud içerisinde resource_catalog olmasý gerekiyor.
    options.Audience = "resource_photo_stock";

    options.RequireHttpsMetadata = false;

});

>>>>>>> 9ec69e8db6b5c5aa767d48090463f294838d0d26
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

//wwwroot içerisindeki dosyalara static olarak dýþarýdan eriþebilme.
app.UseStaticFiles();

<<<<<<< HEAD
=======
app.UseAuthentication();

>>>>>>> 9ec69e8db6b5c5aa767d48090463f294838d0d26
app.Run();
