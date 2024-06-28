using System.Net.Sockets;
using System.Text;

namespace EnviarEtiquetasImpressora.Perifericos
{
    public class EtiquetaPPLA
    {
        private string inicio = "<STX>";
        private string finalLinha = "<CR>";

        public string IP { get; set; }
        public int Porta { get; set; }

        public List<string> comandos { get; set; }

        public Exception erro { get; set; }

        public EtiquetaPPLA(string IP, int Porta)
        {

            this.IP = IP;
            this.Porta = Porta;

            this.comandos = new List<string>();
        }

        public bool GerarImpressao(Nucleo.Data.Etiqueta etiqueta)
        {
            try
            {
                this.comandos = new List<string>();
                comandos.Add(inicio + "L" + finalLinha);
                comandos.Add("111100000820065" + etiqueta.DataCalibracao.GetValueOrDefault().ToShortDateString() + finalLinha);
                comandos.Add("101100000620065" + etiqueta.NumeroCertificado + finalLinha);
                comandos.Add("101100000400065" + etiqueta.NumeroIdentificacao + finalLinha);
                comandos.Add("111100000200065" + etiqueta.ProximaCalibracao.GetValueOrDefault().ToShortDateString() + finalLinha);

                //codigo de barras
                //1v000000500040010200025QA
                //1v1100000250150
                comandos.Add("1W1D22000002201630,LA," + etiqueta.DiretorioLaudo + finalLinha);

                comandos.Add("E" + finalLinha);
                return true;
            }
            catch (Exception ex)
            {
                Comum.Mensagem.Log("Erro ao gerar comando de impressão", ex);
                erro = ex;
                return false;
            }

        }

        public bool Imprimir()
        {
            try
            {
                using (Comunicacao TCP = new Comunicacao(IP, Porta))
                {
                    string enviar = string.Empty;
                    foreach (string comando in comandos)
                    {
                        enviar += comando;
                    }

                    TCP.EnviarComandoPPLA(enviar);
                }


                return true;
            }
            catch (Exception ex)
            {
                Comum.Mensagem.Log("Erro ao enviar comandos para a impressora", ex);
                erro = ex;
                return false;
            }           
        }

        class Comunicacao : IDisposable
        {
            private TcpClient _tcpClient;
            private NetworkStream _networkStream;

            public Comunicacao(string enderecoIP, int porta)
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(enderecoIP, porta);
                _networkStream = _tcpClient.GetStream();
            }

            public void EnviarComandoPPLA(string comandoPPLA)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(ReplaceControlCharacters(comandoPPLA));
                _networkStream.Write(bytes, 0, bytes.Length);
            }

            private string ReplaceControlCharacters(string input)
            {
                return input
                    .Replace("<STX>", ((char)2).ToString())  // Start of Text
                    .Replace("<CR>", ((char)13).ToString()); // Carriage Return
            }

            public void Dispose()
            {
                _networkStream.Close();
                _tcpClient.Close();
            }
        }



        public class Comandos
        {
            public Fonte fonte { get; set; }
            public Orientacao orientacao { get; set; }
            public Multiplicador multiplicadorHorizontal { get; set; }
            public Multiplicador multiplicadorVertical { get; set; }
            public SubFonte fonteVariacao { get; set; }


            public Comandos(Fonte fonte, Orientacao sentido, Multiplicador multH = Multiplicador.Padrao_1, Multiplicador multV = Multiplicador.Padrao_1, SubFonte subFonte = SubFonte.Padrao)
            {
                this.fonte = fonte;
                this.fonteVariacao = subFonte;
                this.orientacao = sentido;
                this.multiplicadorVertical = multV;
                this.multiplicadorHorizontal = multH;
            }

            public enum Orientacao
            {
                Normal = 1,
                Anti_Horario_270 = 2,
                Anti_Horario_180 = 3,
                Anti_Horario_90 = 3,
            }

            public enum Fonte
            {
                Fonte_0 = 0,
                Fonte_1 = 1,
                Fonte_2 = 2,
                Fonte_3 = 3,
                Fonte_4 = 4,
                Fonte_5 = 5,
                Fonte_6 = 6,
                Fonte_7 = 7,
                Fonte_8 = 8,
            }

            public enum SubFonte
            {
                Padrao = 000,
            }

            public enum Multiplicador
            {
                Multiplicador_0 = 0,
                Padrao_1 = 1,
                Multiplicador_2 = 2,
                Multiplicador_3 = 3,
                Multiplicador_4 = 4,
                Multiplicador_5 = 5,
                Multiplicador_6 = 6,
                Multiplicador_7 = 7,
                Multiplicador_8 = 8,
                Multiplicador_9 = 9,

            }

            public enum LarguraCodigoBarras
            {
                ZERO = 0,
                UM = 1,
                DOIS = 2,
                TRES = 3,
                QUATRO = 4,
                CINCO = 5,
                SEIS = 6,
                SETE = 7,
                OITO = 8,
                NOVE = 9,
                A = 10,
                B = 11,
                C = 12,
                D = 13,
                E = 14,
                F = 15,
                G = 16,
                H = 17,
                I = 18,
                J = 19,
                K = 20,
                L = 21,
                M = 22,
                N = 23,
                O = 24,
            }

            public string TextoNormal(string comando, int x, int y)
            {
                return string.Concat(this.orientacao
                    , this.fonte
                    , this.multiplicadorHorizontal
                    , ((int)this.multiplicadorVertical).ToString()
                    , ((int)this.fonteVariacao).ToString().PadLeft(3, '0')
                    , y.ToString().PadLeft(4, '0')
                    , x.ToString().PadLeft(4, '0')
                    , comando);
            }

            public string CodigoBarras(string conteudo, int x, int y)
            {
                return string.Concat(this.orientacao
                    , "A"   //TIPO DE CODIGO DE BARRAS, CONSULTAR TABELA INTERNACIONAL
                    );
            }
        }
    }
}
