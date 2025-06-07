using AtualizarBaseEstrutural.Comum;
using System;
using System.Diagnostics;
namespace AtualizarBaseEstrutural
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public static Nucleo.Base.SQL.SQL? banco;

        public static Nucleo.Base.SQL.SQL? bancoLocal;
        public static Arquivos? Arquivos;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                Console.WriteLine($"rodou {DateTime.Now}");


                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Modifiers == ConsoleModifiers.Alt && cki.Key == ConsoleKey.C)
                {
                    // Ação a ser executada quando a tecla Alt+C for pressionada
                    Console.WriteLine("Tecla Alt+C pressionada!");

                    Process.Start("ConfigDataBaseManager.exe");

                }

                //Iniciar();

                await Task.Delay(1000, stoppingToken);
            }
        }

        public static void Iniciar()
        {
            if (LogarBancos())
            {
                try
                {
                    //new Nucleo.Operacoes.BO.EstruturaDatabase(banco).Atualizar(bancoLocal);
                    new Nucleo.Operacoes.BO.EstruturaDatabase(banco).AtualizarPorScript(bancoLocal);
                }
                catch (Exception ex)
                {
                    Mensagem.Log("Erro ao validar a estrutura do banco de dados, o sistema será fechado.", ex);
                }
            }

        }

        public static bool LogarBancos()
        {
            bool ret = false;

            ret = LogarBanco();
            if (ret)
                ret = LogarBancoLocal();
            return ret;
        }

        private static bool LogarBanco()
        {
            banco = Instanciar(new Arquivos().LerBanco(), true);

            return banco != null;
        }

        private static bool LogarBancoLocal()
        {
            bancoLocal = Instanciar(new Arquivos().LerBancoLocal(), false);

            return bancoLocal != null;
        }

        private static Nucleo.Base.SQL.SQL? Instanciar(string dados, bool LoginAzure)
        {
            Arquivos = new();
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
                    return null;
                }

                if (string.IsNullOrEmpty(NomeBanco))
                {
                    Mensagem.Log("Nome do banco de dados inconsistente, o sistema será fechado.");
                    return null;
                }

                if (string.IsNullOrEmpty(Senha))
                {
                    Mensagem.Log("Senha do banco de dados inconsistente, o sistema será fechado.");
                    return null;
                }

                dados = string.Concat(Servidor, "|", NomeBanco, "|", Senha);
                Arquivos.GravarBanco(dados);
            }

            try
            {
                return new Nucleo.Base.SQL.SQL(new Nucleo.Base.SQL.SQL.Conexao(dados, LoginAzure));
            }
            catch (Exception ex)
            {
                Mensagem.Log("Erro ao verificar banco de dados, o sistema será fechado.", ex);
                return null;
            }
        }
    }
}
