using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public partial class frmLogin : Form
    {
        private bool contrasenaVisible = false;
        public bool IsAuthenticated { get; private set; } = false;
        public string RolUsuario { get; private set; } = string.Empty;

        public frmLogin()
        {
            InitializeComponent();
            this.AcceptButton = buttonLogin;
            AplicarEstilos();
        }

        private void AplicarEstilos()
        {
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Font fuente = new Font("Segoe UI", 12);

            textBoxUsuario.Font = fuente;
            textBoxUsuario.Height = 35;
            textBoxUsuario.Margin = new Padding(5);
            textBoxUsuario.BorderStyle = BorderStyle.FixedSingle;

            textBoxContraseña.Font = fuente;
            textBoxContraseña.Height = 35;
            textBoxContraseña.Margin = new Padding(5);
            textBoxContraseña.BorderStyle = BorderStyle.FixedSingle;
            textBoxContraseña.UseSystemPasswordChar = true;

            // Solo carga la imagen, sin aplicar estilos visuales al botón
            string rutaBase = Path.Combine(Application.StartupPath, "Resources");
            string rutaCerrado = Path.Combine(rutaBase, "cerrado.png");

            if (File.Exists(rutaCerrado))
            {
                buttonMostrarContraseña.Image = Image.FromFile(rutaCerrado);
                buttonMostrarContraseña.ImageAlign = ContentAlignment.MiddleCenter;
            }
            else
            {
                MessageBox.Show("No se encontró la imagen cerrado.png.",
                                "Error de recursos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            string usuario = textBoxUsuario.Text.Trim();
            string contraseña = textBoxContraseña.Text;

            // Validación básica en el cliente
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Por favor ingrese nombre de usuario y contraseña.");
                return;
            }

            try
            {
                buttonLogin.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var (success, rol) = await FirebaseHelper.LoginConNombreYPassword(usuario, contraseña);

                if (success)
                {
                    IsAuthenticated = true;
                    RolUsuario = rol;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            finally
            {
                buttonLogin.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void buttonMostrarContraseña_Click(object sender, EventArgs e)
        {
            contrasenaVisible = !contrasenaVisible;
            textBoxContraseña.UseSystemPasswordChar = !contrasenaVisible;

            string rutaBase = Path.Combine(Application.StartupPath, "Resources");
            string rutaAbierto = Path.Combine(rutaBase, "abierto.png");
            string rutaCerrado = Path.Combine(rutaBase, "cerrado.png");

            if (File.Exists(rutaAbierto) && File.Exists(rutaCerrado))
            {
                buttonMostrarContraseña.Image = contrasenaVisible
                    ? Image.FromFile(rutaAbierto)
                    : Image.FromFile(rutaCerrado);
            }
            else
            {
                MessageBox.Show("No se encontraron las imágenes abierto.png o cerrado.png.",
                                "Error de recursos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}