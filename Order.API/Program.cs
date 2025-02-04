using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(opt =>
{
    opt.AddConsumer<OrderRequestCompletedEventConsumer>();
    opt.AddConsumer<OrderRequestFailedEventConsumer>();
    //opt.AddConsumer<StockNotReservedEventConsumer>();

    opt.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderRequestCompletedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderRequestCompletedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderRequestFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderRequestFailedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();