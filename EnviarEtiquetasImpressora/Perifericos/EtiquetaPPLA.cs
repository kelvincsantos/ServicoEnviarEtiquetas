using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EnviarEtiquetasImpressora.Perifericos
{
    public  class EtiquetaPPLA
    {
        public string IP { get; set; }
        public int Porta { get; set; }
        public string Temperatura { get; set; }

        public List<string> comandos { get; set; }

        public EtiquetaPPLA(string IP, int Porta, string Temperatura)
        {
            this.IP = IP;
            this.Temperatura = Temperatura;
            this.Porta = Porta;

            this.comandos = new List<string>();
        }

        public bool GerarImpressao(Nucleo.Data.Etiqueta etiqueta)
        {
            try
            {
                this.comandos = new List<string>();
                comandos.Add(etiqueta.NumeroIdentificacao);
                comandos.Add(etiqueta.DiretorioLaudo);
                comandos.Add(etiqueta.DataCalibracao.GetValueOrDefault().ToShortDateString());
                comandos.Add(etiqueta.ProximaCalibracao.GetValueOrDefault().ToShortDateString());

                return true;
            }
            catch (Exception ex)
            {
                Comum.Mensagem.Log("Erro ao gerar comando de impressão", ex);
                return false;
            }

        }

        public bool Imprimir()
        {
            using (var client = new System.Net.Sockets.TcpClient())
            {
                var serverEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(IP), Porta);
                client.Connect(serverEndPoint);

                using (var clientStream = client.GetStream())
                {
                    var encoder = new System.Text.ASCIIEncoding();
                    
                    foreach (string comando in comandos)
                    {
                        byte[] buffer = encoder.GetBytes(comando);
                        clientStream.Write(buffer, 0, buffer.Length);
                    }
                    
                    
                    clientStream.Flush();
                }
            }

            return true;
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
