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

//Microservisi koruma alt�na ald�m 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token da��tmaktan sorumlu arkadas� veriyoruz. B�z�m Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanm�� bir token geldi�inde public key ile do�rulama yap�cak.
    //Public keyi de belirtti�imiz URL �zerinden al�cak.
    options.Authority = builder.Configuration["IdentityServerURL"];

    //Gelen token aud i�erisinde resource_catalog olmas� gerekiyor.
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

//wwwroot i�erisindeki dosyalara static olarak d��ar�dan eri�ebilme.
app.UseStaticFiles();

<<<<<<< HEAD
=======
app.UseAuthentication();

>>>>>>> 9ec69e8db6b5c5aa767d48090463f294838d0d26
app.Run();
