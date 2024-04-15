using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EnviarEtiquetasImpressora.Perifericos;

namespace EnviarEtiquetasImpressora
{
    public class Etiquetas
    {
        public List<Nucleo.Data.Etiqueta> Impressoes { get; set; }


        public Etiquetas() 
        {
            Impressoes = new List<Nucleo.Data.Etiqueta>();
            BuscarImpressoes();
        }

        private void BuscarImpressoes()
        {
            
        }

        public bool TemImpressaoPendente()
        {
            return Impressoes.Count > 0;
        }

        public void EnviarImpressaoEtiqueta()
        {
            foreach (Nucleo.Data.Etiqueta item in Impressoes) 
            {
                EtiquetaPPLA PPLA = new EtiquetaPPLA(""); //ENVIAR O IP DA IMPRESSORA

                string impressao = PPLA.GerarImpressao(item);

                PPLA.Imprimir();
            }
        }
    }
}


using System.Data.Common;
using LATROMI.Extensions;

var ip = (string)Variables["ip"].Value;
var porta = (int)Variables["porta"].Value;
var temperatura = (string)Variables["temperatura"].Value;
var comando = (string)Fields["txtComando"].Value;

if (string.IsNullOrEmpty(ip))
    throw new InvalidOperationException("Impressora não informada.");

if (string.IsNullOrEmpty(temperatura))
    throw new InvalidOperationException("Temperatura não informada.");

if (string.IsNullOrEmpty(comando))
    throw new InvalidOperationException("Informe um comando para enviar para a impressora.");

using (var client = new System.Net.Sockets.TcpClient())
{
    var serverEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), porta);
    client.Connect(serverEndPoint);

    using (var clientStream = client.GetStream())
    {
        var encoder = new System.Text.ASCIIEncoding();
        byte[] buffer = encoder.GetBytes(comando);
        clientStream.Write(buffer, 0, buffer.Length);
        clientStream.Flush();
    }
}