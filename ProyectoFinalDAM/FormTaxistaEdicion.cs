using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public partial class FormTaxistaEdicion : Form
    {
        private Label lblMensaje;

        public Taxista Taxista { get; private set; }

        public FormTaxistaEdicion(Taxista taxistaExistente = null)
        {
            InitializeComponent();
            Taxista = taxistaExistente ?? new Taxista();
            // Crear y configurar el Label para mensajes
            lblMensaje = new Label();
            lblMensaje.Name = "lblMensaje";
            lblMensaje.AutoSize = true;
            lblMensaje.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblMensaje.ForeColor = Color.Red;
            lblMensaje.Visible = false;
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;

            // Ubicarlo en la parte superior central del formulario
            lblMensaje.Location = new Point((this.ClientSize.Width - lblMensaje.Width) / 2, 10);
            lblMensaje.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Agregar al formulario
            this.Controls.Add(lblMensaje);



            // Si se está editando un taxista existente, cargar sus datos en los campos
            if (taxistaExistente != null)
            {
                txtNombre.Text = Taxista.Nombre;
                txtDNI.Text = Taxista.DNI;
                txtDireccion.Text = Taxista.Direccion;
                txtPoblacion.Text = Taxista.Poblacion;
                txtProvincia.Text = Taxista.Provincia;
                txtTelefono.Text = Taxista.NumeroTelefono.ToString();
                txtEmail.Text = Taxista.Email;
                txtCCC.Text = Taxista.CCC;
            }
        }

        private async void MostrarMensaje(string mensaje, Color color)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = color;
            lblMensaje.Visible = true;

            // Recentrar después de cambiar el texto
            lblMensaje.Location = new Point((this.ClientSize.Width - lblMensaje.PreferredWidth) / 2, 10);

            await Task.Delay(3000);
            lblMensaje.Visible = false;
        }


        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("El nombre es obligatorio.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MostrarMensaje("El DNI es obligatorio.", Color.Red);
                return false;
            }

            if (!EsDNIValido(txtDNI.Text))
            {
                MostrarMensaje("El DNI no es válido (debe tener 8 números y una letra).", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MostrarMensaje("La dirección es obligatoria.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPoblacion.Text))
            {
                MostrarMensaje("La población es obligatoria.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtProvincia.Text))
            {
                MostrarMensaje("La provincia es obligatoria.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTelefono.Text) ||
            !Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{9,}$"))
            {
                MostrarMensaje("El teléfono no es válido (mínimo 9 dígitos).", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !EsEmailValido(txtEmail.Text))
            {
                MostrarMensaje("El email no es válido.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCCC.Text) || !EsCCCValido(txtCCC.Text))
            {
                MostrarMensaje("El CCC no es válido (deben ser 20 dígitos).", Color.Red);
                return false;
            }

            MostrarMensaje("Todos los campos son válidos.", Color.Green);
            return true;
        }


        private bool EsDNIValido(string dni)
        {
            // Expresión regular para validar un DNI español: 8 dígitos seguidos de una letra
            var regex = new Regex(@"^\d{8}[A-Za-z]$");
            return regex.IsMatch(dni);
        }

        private bool EsEmailValido(string email)
        {
            // Expresión regular para validar un email
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        private bool EsCCCValido(string ccc)
        {
            // Expresión regular para validar un CCC: exactamente 20 dígitos
            var regex = new Regex(@"^\d{20}$");
            return regex.IsMatch(ccc);
        }

        private void buttonAceptar_Click(object sender, EventArgs e)
        {
            // Validar los campos antes de guardar
            if (!ValidarCampos())
                return;

            // Crear un nuevo objeto Taxista con los datos ingresados
            Taxista = new Taxista
            {
                Nombre = txtNombre.Text.Trim(),
                DNI = txtDNI.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Poblacion = txtPoblacion.Text.Trim(),
                Provincia = txtProvincia.Text.Trim(),
                NumeroTelefono = int.Parse(txtTelefono.Text.Trim()),
                Email = txtEmail.Text.Trim(),
                CCC = txtCCC.Text.Trim()
            };

            // Cerrar el formulario con resultado OK
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            // Cerrar el formulario con resultado Cancel
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}