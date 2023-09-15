using Microsoft.Extensions.DependencyModel;
using RPA.ConsultaProcessosPrimeiroGrau.eSAJ;
using RPA.ConsultaProcessosPrimeiroGrau.eSAJ.Handlers;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        //Environment Variable Configuration for creating the Log and Serilog folder
        Environment.SetEnvironmentVariable("BASEDIR", AppContext.BaseDirectory);
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();

        //Dependency Injection
        services.AddScoped<ISeleniumHandler, SeleniumHandler>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
