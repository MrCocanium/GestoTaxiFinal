using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoFinalDAM; // Asegúrate de tener acceso a FirebaseHelper y Paciente

namespace ProyectoFinalDAM
{
    public partial class FormPacienteEdicion : Form
    {
        public Paciente Paciente { get; private set; }

        private Label lblMensaje;

        public FormPacienteEdicion(Paciente pacienteExistente = null)
        {
            InitializeComponent();
            Paciente = pacienteExistente ?? new Paciente();

            lblMensaje = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                Visible = false
            };

            Controls.Add(lblMensaje);
            Controls.SetChildIndex(lblMensaje, 0);

            if (pacienteExistente != null)
            {
                txtNombre.Text = Paciente.Nombre;
                txtDNI.Text = Paciente.DNI;
            }
        }

        private async void MostrarMensaje(string mensaje, Color color, int duracion = 3000)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = color;
            lblMensaje.Visible = true;

            await Task.Delay(duracion);

            lblMensaje.Visible = false;
        }

        private bool EsDNIValido(string dni)
        {
            var regex = new Regex(@"^\d{8}[A-Za-z]$");
            return regex.IsMatch(dni);
        }

        private async Task<bool> ValidarCamposAsync()
        {
            string nombre = txtNombre.Text.Trim();
            string dni = txtDNI.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarMensaje("El nombre es obligatorio.", Color.Red);
                return false;
            }

            if (string.IsNullOrWhiteSpace(dni) || !EsDNIValido(dni))
            {
                MostrarMensaje("El DNI no es válido. Debe tener 8 dígitos y una letra.", Color.Red);
                return false;
            }

            // Verificar si el DNI ya existe en Firebase
            var pacientes = await FirebaseHelper.ObtenerTodosLosPacientes();
            bool dniDuplicado = pacientes.Any(p =>
                p.Object.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase) &&
                (Paciente == null || p.Object.DNI != Paciente.DNI));

            if (dniDuplicado)
            {
                MostrarMensaje("Este DNI ya está registrado.", Color.Red);
                return false;
            }

            return true;
        }

        private async void buttonAceptar_Click(object sender, EventArgs e)
        {
            if (!await ValidarCamposAsync())
                return;

            Paciente = new Paciente
            {
                Nombre = txtNombre.Text.Trim(),
                DNI = txtDNI.Text.Trim()
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    public class Paciente
    {
        public string Nombre { get; set; }
        public string DNI { get; set; }
    }
}
