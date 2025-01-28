using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using Shared;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
    .EntityFrameworkRepository(opt =>
    {
        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, bld) =>
        {
            bld.UseSqlServer(builder.Configuration.GetConnectionString("SqlConn"),
                migration =>
                {
                    migration.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                });
        });
    });

    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        rabbitCfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        rabbitCfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderSaga, e =>
        {
            e.ConfigureSaga<OrderStateInstance>(context);
        });        
    });
});



builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
