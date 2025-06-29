﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtualizarBaseEstrutural.Comum
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
                System.IO.File.AppendAllText("ArqErro.err", log + "\n\r");
            }
            catch (Exception) { }
        }

        public static void Log(String e)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + e);
        }

        public static string? Request(String e)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + e);
            return Console.ReadLine();
        }
    }
}
