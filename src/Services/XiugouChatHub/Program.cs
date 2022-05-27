using StackExchange.Redis;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Implementations;
using XiugouChatHub;
using XiugouChatHub.Operations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConnection = Environment.GetEnvironmentVariable("RedisConnection") ?? "127.0.0.1:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddScoped<IXiugouRepository, XiugouRepository>();

builder.Services.AddSingleton<OperationExecutor>();
builder.Services.AddScoped<StoreChatInformationOperation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
