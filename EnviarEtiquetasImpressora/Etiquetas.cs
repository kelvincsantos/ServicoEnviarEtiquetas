using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using EnviarEtiquetasImpressora.Perifericos;

namespace EnviarEtiquetasImpressora
{
    public class Etiquetas
    {
        public List<Nucleo.Data.Etiqueta> Impressoes { get; set; }
        public Nucleo.Base.SQL.SQL? Banco;

        public Etiquetas() 
        {   
            Iniciar();
        }
        public Etiquetas(Nucleo.Base.SQL.SQL? banco)
        {
            Banco = banco;
            Iniciar();
        }

        private void Iniciar()
        {
            Impressoes = new List<Nucleo.Data.Etiqueta>();
            BuscarImpressoes();
        }

        private void BuscarImpressoes()
        {
            Nucleo.Operacoes.BO.Etiquetas BO = new Nucleo.Operacoes.BO.Etiquetas(Banco);
            Impressoes = BO.BuscarPendentesDeImpressao();
        }

        public bool TemImpressaoPendente()
        {
            return Impressoes.Count > 0;
        }

        public void EnviarImpressaoEtiqueta()
        {
            Nucleo.Data.Configuracao conf = new Nucleo.Operacoes.BO.Configuracao(Banco).BuscarConfiguracao();

            foreach (Nucleo.Data.Etiqueta item in Impressoes) 
            {
                EtiquetaPPLA PPLA = new EtiquetaPPLA(conf.IP_Impressora, Convert.ToInt32(conf.Porta_Impressora)); //ENVIAR O IP DA IMPRESSORA

                Nucleo.Operacoes.BO.FilaImpressao BO = new Nucleo.Operacoes.BO.FilaImpressao(Banco);
                bool impressao = PPLA.GerarImpressao(item);

                Nucleo.Data.FilaImpressao fila = BO.BuscarPorEtiqueta(item.ID);

                if (PPLA.Imprimir())
                {
                    fila.Impressao = DateTime.Now;
                    fila.Concluido = true;
                    fila.Erro = string.Empty;
                }
                else
                {
                    fila.Concluido = false;
                    fila.Impressao = null;
                    fila.Erro = PPLA.erro.Message;
                }

                BO.Alterar(fila);
                
            }
        }
    }
}