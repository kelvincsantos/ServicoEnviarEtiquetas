using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text;
using PDF = ceTe.DynamicPDF;
using PDF_Page = ceTe.DynamicPDF.PageElements;
using System.Threading.Tasks;
using IO = System.IO;

namespace EnviarEtiquetasImpressora.Comum
{
    public class Arquivos
    {
        private const string Diretorio = "Config";

        private const string Banco = "Caixa.conf";
        private const string Assinatura = "APIS.conf";

        private readonly string dirBanco = IO.Path.Combine(Diretorio, Banco);
        private readonly string dirAssinatura = IO.Path.Combine(Diretorio, Assinatura);

        private Nucleo.Base.Seguranca.Criptografia Criptografia;

        public Arquivos()
        {
            Criptografia = new();
            if (!Directory.Exists(Diretorio))
                Directory.CreateDirectory(Diretorio);
        }

        public bool GravarBanco(string dados)
        {
            string s = Criptografia.Codificar(dados);

            return Gravar(dirBanco, s);
        }
        public string LerBanco()
        {
            return Criptografia.Decodificar(Ler(dirBanco));
        }
        public bool GravarAssinatura(string assinatura)
        {
            return Gravar(dirAssinatura, Criptografia.Codificar(assinatura));
        }
        public void DeletarAssinatura()
        {
            Deletar(dirAssinatura);
        }

        public string LerAssinatura()
        {
            return Criptografia.Decodificar(Ler(dirAssinatura));
        }

        private string Ler(string arquivo, bool mensagem = false)
        {
            try
            {
                if (!File.Exists(arquivo))
                {
                    if (mensagem)
                        Mensagem.Log(string.Concat("Arquivo inexistente: ", arquivo));

                    return string.Empty;
                }

                string conteudo = File.ReadAllText(arquivo);

                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    if (mensagem)
                        Mensagem.Log(string.Concat("Arquivo vazio: ", arquivo));

                    return string.Empty;
                }

                return conteudo;
            }
            catch (Exception ex)
            {
                Mensagem.Log(string.Concat("Erro ao ler arquivo: ", arquivo), ex);
                return string.Empty;
            }
        }

        private List<string>? LerLinhas(string arquivo)
        {
            try
            {
                if (!File.Exists(arquivo))
                    return null;

                return File.ReadAllLines(arquivo).ToList();
            }
            catch (Exception ex)
            {
                Mensagem.Log(string.Concat("Erro ao ler arquivo: ", arquivo), ex);
                return null;
            }
        }

        private bool Gravar(string arquivo, string conteudo)
        {
            try
            {
                if (File.Exists(arquivo))
                    File.Delete(arquivo);

                if (string.IsNullOrWhiteSpace(conteudo))
                    return true;

                using (StreamWriter escritor = new StreamWriter(arquivo))
                {
                    escritor.Write(conteudo);
                }

                return true;
            }
            catch (Exception ex)
            {
                Mensagem.Log(string.Concat("Erro ao ler arquivo: ", arquivo), ex);
                return false;
            }
        }

        private bool Gravar(string arquivo, List<string> conteudo)
        {
            try
            {
                Deletar(arquivo);
                
                if (conteudo == null)
                    return true;

                using (StreamWriter escritor = new StreamWriter(arquivo))
                {
                    foreach (string item in conteudo)
                    {
                        escritor.WriteLine(item);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Mensagem.Log(string.Concat("Erro ao ler arquivo: ", arquivo), ex);
                return false;
            }
        }

        private void Deletar(string arquivo)
        {
            if (File.Exists(arquivo))
                File.Delete(arquivo);
        }


        public static void CriarArquivoPDFVazio(string nomeArquivo)
        {
            PDF.Document document = new PDF.Document();

            PDF.Page page = new PDF.Page(PDF.PageSize.Letter, PDF.PageOrientation.Portrait, 54.0f);
            document.Pages.Add(page);

            string labelText = "Arquivo em processo de geração...";
            PDF_Page.Label label = new PDF_Page.Label(labelText, 0, 0, 504, 100, PDF.Font.Helvetica, 18, PDF.TextAlign.Center);
            page.Elements.Add(label);

            document.Draw(nomeArquivo);
        }

        public static string PegarNomeArquivo(string Diretorio)
        {
            return Diretorio.Split('\\').Last();
        }
    }
}
