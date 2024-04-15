using EnviarEtiquetasImpressora.Comum;

namespace EnviarEtiquetasImpressora
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Serviço iniciado em: {time}", DateTimeOffset.Now);
                    }

                    Iniciar();
                }
                catch (Exception ex)
                {
                    Mensagem.Log("Erro ao executar serviço:", ex);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public static void Iniciar()
        {
            Etiquetas etiquetas = new Etiquetas();

            if (etiquetas.TemImpressaoPendente())
                etiquetas.EnviarImpressaoEtiqueta();
        }
    }
}
