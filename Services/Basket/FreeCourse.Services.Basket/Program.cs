using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


//Bu micrsoservisi kullan�cak client mutlaka authentice olmu� userId g�ndermek zorunda bunun i�in policy ekleyip controllera filter olarak bu policy'i ekliyoruz.
var requireAuthenticationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

//Sub yerine ba�ka bir url basmas�n token i�erisine.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

//appsettings klasorundeki redissettings property'si ile redissettings s�n�f�n� doldurdum.
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

//Herhangi bir s�n�f yap�c� metodunda IDatabaseSettings ge�ersek DatabaseSettings s�n�ftan bir nesne gelicek.
builder.Services.AddSingleton<IRedisSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<RedisSettings>>().Value;
});

builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;

    var redis = new RedisService(redisSettings.Host, redisSettings.Port);

    redis.Connect();

    return redis;
});
//Microservisi koruma alt�na ald�m 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token da��tmaktan sorumlu arkadas� veriyoruz. B�z�m Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanm�� bir token geldi�inde public key ile do�rulama yap�cak.
    //Public keyi de belirtti�imiz URL �zerinden al�cak.
    options.Authority = builder.Configuration["IdentityServerURL"];

    options.Audience = "resource_basket";

    options.RequireHttpsMetadata = false;

    

});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();


builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthenticationPolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
