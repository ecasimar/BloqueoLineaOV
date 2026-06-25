using System;

namespace BloqueoLineaOV
{
    internal class Program
    {
        // El atributo STAThread es OBLIGATORIO para que las APIs de comunicación COM 
        // de SAP Business One funcionen correctamente y sin problemas de hilos (Threads).
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // 1. Inicializamos tu clase del Add-on
                BlockLine blockLine = new BlockLine();

                // 2. Iniciamos el ciclo de vida oficial de la aplicación.
                // Esto mantiene el Add-on vivo en segundo plano escuchando los eventos de SAP,
                // consumiendo el mínimo de memoria y sin necesidad de abrir ventanas de consola negras.
                System.Windows.Forms.Application.Run();
            }
            catch (Exception ex)
            {
                // En producción, evita usar MessageBox flotantes si es posible, 
                // pero para errores fatales de inicio está perfecto.
                System.Windows.Forms.MessageBox.Show(
                    "Error fatal al iniciar el Add-on de SAP: " + ex.Message,
                    "Error de Sistema",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
            }
        }
    }
}