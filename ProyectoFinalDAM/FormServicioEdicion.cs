using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace ProyectoFinalDAM
{
    public partial class FormServicioEdicion : Form
    {
        public Servicio Servicio { get; private set; }
        private Servicio ServicioInicial;
        private bool esModoEdicion = false;

        // Clase auxiliar para mostrar lugar + coordenadas en ComboBox
        private class LugarComboItem
        {
            public string ID { get; set; }
            public string Descripcion { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }

            public override string ToString()
            {
                return Descripcion;
            }
        }

        public FormServicioEdicion(Servicio servicioExistente = null)
        {
            InitializeComponent();
            Servicio = servicioExistente ?? new Servicio { ID = GenerarNuevoID() };
            ServicioInicial = servicioExistente;
            esModoEdicion = servicioExistente != null;

            // Configurar controles iniciales
            dtpFecha.MinDate = DateTime.Now.AddYears(-100);

            numKilometros.Minimum = 0;
            numKilometros.Maximum = 2000;
            numHorasEspera.Minimum = 0;
            numHorasEspera.Maximum = 48;


            // Eventos
            cmbOrigen.SelectedIndexChanged += Combo_OrigenDestino_Changed;
            cmbDestino.SelectedIndexChanged += Combo_OrigenDestino_Changed;

            // Inicialización asíncrona
            CargarCombos();
            InitializeWebView2();
        }

        private async void InitializeWebView2()
        {
            try
            {
                await webViewMapa.EnsureCoreWebView2Async(null);

                // Manejar mensajes desde JS
                webViewMapa.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(e.WebMessageAsJson);

                        if (data.tipo == "error")
                        {
                            MessageBox.Show((string)data.mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (data.tipo == "ruta_calculada")
                        {
                            // Convertir metros a kilómetros
                            if (double.TryParse((string)data.distancia, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double distanciaMetros))
                            {
                                int kilometros = (int)Math.Round(distanciaMetros / 1000);
                                if (kilometros > 0)
                                {
                                    // Asegurarse de que la actualización se hace en el hilo de la UI
                                    if (InvokeRequired)
                                    {
                                        Invoke(new Action(() => numKilometros.Value = kilometros));
                                    }
                                    else
                                    {
                                        numKilometros.Value = kilometros;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al recibir mensaje del mapa: " + ex.Message);
                    }
                };


                string htmlPath = Path.Combine(Application.StartupPath, "Resources", "map.html");
                if (File.Exists(htmlPath))
                {
                    webViewMapa.Source = new Uri(htmlPath);
                }
                else
                {
                    MessageBox.Show($"No se encontró el archivo map.html en la ruta: {htmlPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Dibujar ruta si estamos en modo edición
                if (ServicioInicial != null)
                {
                    webViewMapa.CoreWebView2.NavigationCompleted += async (sender, args) =>
                    {
                        if (args.IsSuccess)
                        {
                            await Task.Delay(500); // Pequeño retardo para asegurar carga
                            ComprobarComboBox();
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar WebView2: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CargarCombos()
        {
            try
            {
                var taxistas = await FirebaseHelper.ObtenerTodosLosTaxistas();
                cmbTaxista.DataSource = taxistas.Select(t => new { ID = t.Key, Nombre = t.Object.Nombre }).ToList();
                cmbTaxista.DisplayMember = "Nombre";
                cmbTaxista.ValueMember = "ID";
                cmbTaxista.SelectedIndex = -1;

                var pacientes = await FirebaseHelper.ObtenerTodosLosPacientes();
                cmbPaciente.DataSource = pacientes.Select(p => new { ID = p.Key, Nombre = p.Object.Nombre }).ToList();
                cmbPaciente.DisplayMember = "Nombre";
                cmbPaciente.ValueMember = "ID";
                cmbPaciente.SelectedIndex = -1;

                var lugaresFirebase = await FirebaseHelper.ObtenerTodosLosLugares();

                // Crear listas independientes para evitar conflictos entre combos
                var listaLugaresOrigen = lugaresFirebase
                    .Select(l => new LugarComboItem
                    {
                        ID = l.Key,
                        Descripcion = l.Object.Descripcion,
                        Latitud = l.Object.Latitud,
                        Longitud = l.Object.Longitud
                    })
                    .ToList();

                var listaLugaresDestino = lugaresFirebase
                    .Select(l => new LugarComboItem
                    {
                        ID = l.Key,
                        Descripcion = l.Object.Descripcion,
                        Latitud = l.Object.Latitud,
                        Longitud = l.Object.Longitud
                    })
                    .ToList();

                // Asignar DataSource independiente a cada combo
                cmbOrigen.DataSource = listaLugaresOrigen;
                cmbOrigen.DisplayMember = "Descripcion";
                cmbOrigen.ValueMember = "ID";
                cmbOrigen.SelectedIndex = -1;

                cmbDestino.DataSource = listaLugaresDestino;
                cmbDestino.DisplayMember = "Descripcion";
                cmbDestino.ValueMember = "ID";
                cmbDestino.SelectedIndex = -1;

                if (ServicioInicial != null)
                    CargarDatosExistente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void CargarDatosExistente()
        {
            if (Servicio == null) return;

            dtpFecha.Value = DateTime.TryParse(Servicio.Fecha, out var fecha) ? fecha : DateTime.Now;
            numKilometros.Value = Math.Max(0, Servicio.Kilometros);
            numHorasEspera.Value = Math.Max(0, Servicio.HorasEspera);
            cmbTaxista.SelectedValue = Servicio.Taxista;
            cmbPaciente.SelectedValue = Servicio.Paciente;

            var lugarOrigen = (LugarComboItem)cmbOrigen.Items
                .Cast<LugarComboItem>()
                .FirstOrDefault(l => l.ID == Servicio.LugarOrigen);
            if (lugarOrigen != null) cmbOrigen.SelectedItem = lugarOrigen;

            var lugarDestino = (LugarComboItem)cmbDestino.Items
                .Cast<LugarComboItem>()
                .FirstOrDefault(l => l.ID == Servicio.LugarDestino);
            if (lugarDestino != null) cmbDestino.SelectedItem = lugarDestino;

            // Forzar comprobación después de cargar los datos
            ComprobarComboBox();
        }

        private int GenerarNuevoID() => new Random().Next(1000, 9999);

        private void Combo_OrigenDestino_Changed(object sender, EventArgs e)
        {
            ComprobarComboBox();
        }

        private bool ValidarDatos()
        {
            if (cmbTaxista.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un taxista", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbPaciente.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un paciente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbOrigen.SelectedItem == null || cmbDestino.SelectedItem == null)
            {
                MessageBox.Show("Seleccione origen y destino", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var lugarOrigen = (LugarComboItem)cmbOrigen.SelectedItem;
            var lugarDestino = (LugarComboItem)cmbDestino.SelectedItem;

            if (lugarOrigen.ID == lugarDestino.ID)
            {
                MessageBox.Show("El origen y destino no pueden ser iguales", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (numKilometros.Value <= 0)
            {
                MessageBox.Show("Los kilómetros deben ser mayores a 0", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            Servicio.Fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            Servicio.Kilometros = (int)numKilometros.Value;
            Servicio.HorasEspera = (int)numHorasEspera.Value;
            Servicio.Taxista = cmbTaxista.SelectedValue?.ToString();
            Servicio.Paciente = cmbPaciente.SelectedValue?.ToString();

            var lugarOrigen = (LugarComboItem)cmbOrigen.SelectedItem;
            var lugarDestino = (LugarComboItem)cmbDestino.SelectedItem;

            Servicio.LugarOrigen = lugarOrigen?.ID;
            Servicio.LugarDestino = lugarDestino?.ID;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public Servicio ObtenerServicio() => Servicio;

        public void CargarServicio(Servicio servicioExistente)
        {
            Servicio = servicioExistente;
            ServicioInicial = servicioExistente;
            CargarDatosExistente();
        }

        private async void ComprobarComboBox()
        {
            // Verifica si ambos ComboBox tienen una selección válida
            if (cmbOrigen.SelectedItem == null || cmbDestino.SelectedItem == null)
            {
                webViewMapa.Visible = false;
                return;
            }

            var lugarOrigen = (LugarComboItem)cmbOrigen.SelectedItem;
            var lugarDestino = (LugarComboItem)cmbDestino.SelectedItem;

            // Verifica que no sean el mismo lugar
            if (lugarOrigen.ID == lugarDestino.ID)
            {
                MessageBox.Show("El origen y el destino no pueden ser iguales", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                webViewMapa.Visible = false;
                return;
            }

            // Calcula la ruta solo si ambos lugares son válidos
            await CalcularRutaAsync(lugarOrigen, lugarDestino);
        }

        private async Task CalcularRutaAsync(LugarComboItem lugarOrigen, LugarComboItem lugarDestino)
        {
            if (webViewMapa.CoreWebView2 == null || lugarOrigen == null || lugarDestino == null)
            {
                webViewMapa.Visible = false;
                return;
            }

            string script = $@"drawRoute(
                {lugarOrigen.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                {lugarOrigen.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                {lugarDestino.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                {lugarDestino.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture)}
            );";

            await webViewMapa.ExecuteScriptAsync(script);
            webViewMapa.Visible = true;
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}