using RPA.ConsultaProcessosPrimeiroGrau.eSAJ.Handlers;
using System.Reflection;

namespace RPA.ConsultaProcessosPrimeiroGrau.eSAJ
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço Iniciado");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Factory.StartNew(() => DoWorkAsync(stoppingToken));
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async void DoWorkAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ISeleniumHandler handler = scope.ServiceProvider.GetRequiredService<ISeleniumHandler>();

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!handler.InitChromeDriver() || !handler.Navigation())
                    {
                        handler.CloseChromeDriver();
                        _logger.LogError("Finalizando ChromeDriver");
                        continue;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name);
            }
            finally
            {
                handler.CloseChromeDriver();
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}