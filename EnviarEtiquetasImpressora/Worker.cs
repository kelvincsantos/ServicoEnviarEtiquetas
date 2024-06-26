using EnviarEtiquetasImpressora.Comum;

namespace EnviarEtiquetasImpressora
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public static Nucleo.Base.SQL.SQL? banco;
        public static Arquivos? Arquivos;
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

            if(LogarBanco())
            {
                Etiquetas etiquetas = new Etiquetas(banco);

                if (etiquetas.TemImpressaoPendente())
                    etiquetas.EnviarImpressaoEtiqueta();
            }
 
        }

        public static bool LogarBanco()
        {
            Arquivos = new();
            string dados = Arquivos.LerBanco();
            string? Servidor = string.Empty;
            string? NomeBanco = string.Empty;
            string? Senha = string.Empty;


            if (string.IsNullOrEmpty(dados))
            {
                Mensagem.Log("Não foi encontrado o arquivo de banco de dados do sistema.");

                Servidor = Mensagem.Request("Informe o endereço do servidor:");
                NomeBanco = Mensagem.Request("Informe o nome do banco de dados:");
                Senha = Mensagem.Request("Informe a senha do servidor:");

                if (string.IsNullOrEmpty(Servidor))
                {
                    Mensagem.Log("servidor do banco de dados inconsistente, o sistema será fechado.");
                    return false;
                }

                if (string.IsNullOrEmpty(NomeBanco))
                {
                    Mensagem.Log("Nome do banco de dados inconsistente, o sistema será fechado.");
                    return false;
                }

                if (string.IsNullOrEmpty(Senha))
                {
                    Mensagem.Log("Senha do banco de dados inconsistente, o sistema será fechado.");
                    return false;
                }

                dados = string.Concat(Servidor, "|", NomeBanco, "|", Senha);
                Arquivos.GravarBanco(dados);
            }

            try
            {
                banco = new Nucleo.Base.SQL.SQL(new Nucleo.Base.SQL.SQL.Conexao(dados));
            }
            catch (Exception ex)
            {
                Mensagem.Log("Erro ao verificar banco de dados, o sistema será fechado.", ex);
                return false;
            }
            return true;
        }
    }
}
