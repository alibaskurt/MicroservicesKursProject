using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


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

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();


builder.Services.AddControllers();
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
