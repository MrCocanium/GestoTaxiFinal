using System;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public static class Seguridad
    {
        /// <summary>
        /// Desactiva los botones si el rol del usuario no es "admin".
        /// </summary>
        /// <param name="rol">Rol del usuario</param>
        /// <param name="botones">Lista de botones a desactivar</param>
        public static void DesactivarAccionesPorRol(string rol, params Button[] botones)
        {
            if (!string.Equals(rol, "admin", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var boton in botones)
                {
                    boton.Enabled = false;
                }
            }
        }
    }
}
