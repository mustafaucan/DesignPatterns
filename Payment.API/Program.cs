using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddMassTransit(opt =>
//{
//    opt.AddConsumer<OrderCreatedEventConsumer>();

//    opt.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
//        cfg.ReceiveEndpoint(RabbitMqSettingsConst.StockOrderCreatedEventQueueName, e =>
//        {
//            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
//        });
//    });
//});

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
