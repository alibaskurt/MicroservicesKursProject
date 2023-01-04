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


//Bu micrsoservisi kullanýcak client mutlaka authentice olmuþ userId göndermek zorunda bunun için policy ekleyip controllera filter olarak bu policy'i ekliyoruz.
var requireAuthenticationPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

//Sub yerine baþka bir url basmasýn token içerisine.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

//appsettings klasorundeki redissettings property'si ile redissettings sýnýfýný doldurdum.
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

//Herhangi bir sýnýf yapýcý metodunda IDatabaseSettings geçersek DatabaseSettings sýnýftan bir nesne gelicek.
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
//Microservisi koruma altýna aldým 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token daðýtmaktan sorumlu arkadasý veriyoruz. Býzým Porjemizde IdentityServer Microservisi.
    //Bu microservise private key ile imzalanmýþ bir token geldiðinde public key ile doðrulama yapýcak.
    //Public keyi de belirttiðimiz URL üzerinden alýcak.
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
