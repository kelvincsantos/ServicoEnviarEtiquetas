using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnviarEtiquetasImpressora.Comum
{
    public static class Mensagem
    {
        public static void Log(String source, Exception ex)
        {
            string log = DateTime.Now.ToString() + ": " + source + " # " + ex.Message;

            if (ex.InnerException != null)
            {
                Exception? exception = ex.InnerException;

                while (exception != null)
                {
                    log = string.Concat(log, System.Environment.NewLine, " # ", exception.Message);

                    exception = exception.InnerException;
                }
            }

            Console.WriteLine(log);

            try
            {
                System.IO.File.AppendAllText("ControleDeEstoquePDV5.err", log + "\n\r");
            }
            catch (Exception) { }
        }

        public static void Log(String e)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + e);
        }
    }
}
