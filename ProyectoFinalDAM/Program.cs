using System;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (frmLogin loginForm = new frmLogin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK && loginForm.IsAuthenticated)
                {
                    string rolUsuario = loginForm.RolUsuario;

                    // Puedes cambiar aquí el formulario principal que se abre
                    Application.Run(new Form1(rolUsuario));
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}
