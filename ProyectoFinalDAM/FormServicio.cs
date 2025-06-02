using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Globalization;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ProyectoFinalDAM
{
    public partial class FormServicios : Form
    {
        private DataTable dataTable = new DataTable();
        private bool isFirstLoad = true;
        private Timer rotarTimer;
        private float angulo = 0f;
        private float totalAnguloGirado = 0f;
        private Image imagenOriginal;
        private bool mapaListo = false;
        private bool estaRefrescando = false;
        private bool tareaTerminada = false;
        private bool datosCargados = false;
        private readonly string rolUsuario;
        public FormServicios(string rol)
        {
            InitializeComponent();
            rolUsuario = rol;
            ConfigureDataGridView();
            InitializeRefreshAnimation();
            ConfigureSearchPlaceholder();
            AplicarSeguridad();

            this.Load += async (sender, e) =>
            {
                await CargarServiciosConReintento(); // Primero cargar datos
                InicializarWebView(); // Luego inicializar el mapa
            };
        }
        private void AplicarSeguridad()
        {
            Seguridad.DesactivarAccionesPorRol(rolUsuario, btnAñadir, btnModificar, btnEliminar);
        }

        private void ConfigureSearchPlaceholder()
        {
            txtBuscar.Text = "Buscar servicio...";
            txtBuscar.ForeColor = Color.Gray;
            txtBuscar.Enter += (sender, e) =>
            {
                if (txtBuscar.Text == "Buscar servicio...")
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = Color.Black;
                }
            };
            txtBuscar.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = "Buscar servicio...";
                    txtBuscar.ForeColor = Color.Gray;
                }
            };
        }

        private void ConfigureDataGridView()
        {
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.DataSource = dataTable;
            dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Limpiar columnas previas
            dgvServicios.Columns.Clear();
            dataTable.Columns.Clear();

            // Definir columnas del DataTable
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Fecha", typeof(string));
            dataTable.Columns.Add("Kilometros", typeof(int));
            dataTable.Columns.Add("HorasEspera", typeof(int));
            dataTable.Columns.Add("Taxista", typeof(string));
            dataTable.Columns.Add("Paciente", typeof(string));
            dataTable.Columns.Add("Origen", typeof(string));
            dataTable.Columns.Add("Destino", typeof(string));
            dataTable.Columns.Add("LatOrigen", typeof(double));
            dataTable.Columns.Add("LngOrigen", typeof(double));
            dataTable.Columns.Add("LatDestino", typeof(double));
            dataTable.Columns.Add("LngDestino", typeof(double));
            dataTable.Columns.Add("FirebaseID", typeof(string));

            // Añadir columnas al DataGridView
            AddColumn("ID", "ID", visible: false);
            AddColumn("Fecha", "Fecha", width: 100);
            AddColumn("Kilometros", "Kilómetros", width: 80);
            AddColumn("HorasEspera", "Horas Espera", width: 80);
            AddColumn("Taxista", "Taxista", width: 150);
            AddColumn("Paciente", "Paciente", width: 150);
            AddColumn("Origen", "Origen", width: 150);
            AddColumn("Destino", "Destino", width: 150);
            AddColumn("LatOrigen", "LatOrigen", visible: false);
            AddColumn("LngOrigen", "LngOrigen", visible: false);
            AddColumn("LatDestino", "LatDestino", visible: false);
            AddColumn("LngDestino", "LngDestino", visible: false);
            AddColumn("FirebaseID", "FirebaseID", visible: false);
        }

        private void AddColumn(string dataPropertyName, string headerText, int width = 100, bool visible = true)
        {
            var column = new DataGridViewTextBoxColumn()
            {
                Name = dataPropertyName, // Nombre clave para buscar después
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Width = width,
                Visible = visible
            };
            dgvServicios.Columns.Add(column);
        }

        private async Task CargarServicios(string filtro = "")
        {
            await CargarServiciosBase(filtro, isFirstLoad);
            if (isFirstLoad) isFirstLoad = false;
        }

        public async Task CargarServiciosConReintento(string filtro = "")
        {
            int reintentos = 0;
            const int maxReintentos = 3;
            while (reintentos < maxReintentos)
            {
                try
                {
                    progressBarCarga.Visible = true;
                    lblEstado.Text = "Cargando servicios...";
                    Application.DoEvents();
                    await CargarServiciosBase(filtro, true);
                    datosCargados = true;
                    progressBarCarga.Visible = false;
                    lblEstado.Text = "";
                    return;
                }
                catch (Exception ex)
                {
                    reintentos++;
                    if (reintentos >= maxReintentos)
                    {
                        MessageBox.Show($"Error al cargar servicios después de {maxReintentos} intentos: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressBarCarga.Visible = false;
                        lblEstado.Text = "Error al cargar datos";
                        return;
                    }
                    await Task.Delay(1000 * reintentos); // Espera progresiva
                }
            }
        }

        private async Task CargarServiciosBase(string filtro, bool mostrarProgreso)
        {
            try
            {
                if (mostrarProgreso)
                {
                    progressBarCarga.Visible = true;
                    lblEstado.Text = "Cargando servicios...";
                    Application.DoEvents();
                }

                var servicios = await FirebaseHelper.ObtenerTodosLosServicios();
                var taxistas = await FirebaseHelper.ObtenerTodosLosTaxistas();
                var pacientes = await FirebaseHelper.ObtenerTodosLosPacientes();
                var lugares = await FirebaseHelper.ObtenerTodosLosLugares();

                dataTable.Rows.Clear();

                foreach (var servicio in servicios)
                {
                    var datos = servicio.Object;
                    if (string.IsNullOrEmpty(servicio.Key))
                    {
                        MessageBox.Show("Se encontró un servicio sin FirebaseID. Este servicio será ignorado.", "Advertencia",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    var taxista = taxistas.FirstOrDefault(t => t.Key == datos.Taxista);
                    string nombreTaxista = taxista?.Object?.Nombre ?? "Desconocido";

                    var paciente = pacientes.FirstOrDefault(p => p.Key == datos.Paciente);
                    string nombrePaciente = paciente?.Object?.Nombre ?? "Desconocido";

                    var lugarOrigen = lugares.FirstOrDefault(l => l.Key == datos.LugarOrigen);
                    string descripcionOrigen = lugarOrigen?.Object?.Descripcion ?? "Sin ubicación";

                    double latOrigen = 0;
                    double lngOrigen = 0;
                    if (lugarOrigen?.Object != null)
                    {
                        latOrigen = lugarOrigen.Object.Latitud;
                        lngOrigen = lugarOrigen.Object.Longitud;
                    }

                    var lugarDestino = lugares.FirstOrDefault(l => l.Key == datos.LugarDestino);
                    string descripcionDestino = lugarDestino?.Object?.Descripcion ?? "Sin ubicación";

                    double latDestino = 0;
                    double lngDestino = 0;
                    if (lugarDestino?.Object != null)
                    {
                        latDestino = lugarDestino.Object.Latitud;
                        lngDestino = lugarDestino.Object.Longitud;
                    }

                    if (string.IsNullOrEmpty(filtro) ||
                        nombreTaxista.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        nombrePaciente.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        descripcionOrigen.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        descripcionDestino.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dataTable.Rows.Add(
                            datos.ID,
                            datos.Fecha,
                            datos.Kilometros,
                            datos.HorasEspera,
                            nombreTaxista,
                            nombrePaciente,
                            descripcionOrigen,
                            descripcionDestino,
                            latOrigen,
                            lngOrigen,
                            latDestino,
                            lngDestino,
                            servicio.Key
                        );
                    }
                }

                if (mostrarProgreso)
                {
                    progressBarCarga.Visible = false;
                    lblEstado.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar servicios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (mostrarProgreso)
                {
                    progressBarCarga.Visible = false;
                    lblEstado.Text = "Error al cargar datos";
                }
            }
        }

        private void MostrarRutaEnMapa(double latOrigen, double lngOrigen, double latDestino, double lngDestino, string origenDesc, string destinoDesc)
        {
            try
            {
                string htmlPath = Path.Combine(Application.StartupPath, "Resources", "map.html");
                string htmlContent = File.ReadAllText(htmlPath);

                origenDesc = origenDesc ?? "Origen desconocido";
                destinoDesc = destinoDesc ?? "Destino desconocido";

                htmlContent = htmlContent.Replace(
                    "// El mapa se inicializa sin ninguna ruta predefinida",
                    $"drawRoute({latOrigen.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{lngOrigen.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{latDestino.ToString(CultureInfo.InvariantCulture)}, " +
                    $"{lngDestino.ToString(CultureInfo.InvariantCulture)});"
                );

                webViewMapita.NavigateToString(htmlContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar la ruta: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerFirebaseIDSeleccionado()
        {
            if (dgvServicios.SelectedRows.Count == 0)
                return null;

            int indiceColumnaFirebaseID = dgvServicios.Columns["FirebaseID"].Index;
            var firebaseIDCell = dgvServicios.SelectedRows[0].Cells[indiceColumnaFirebaseID];
            return firebaseIDCell.Value?.ToString();
        }

        public async void btnAñadir_Click(object sender, EventArgs e)
        {
            using (var formEdicion = new FormServicioEdicion())
            {
                if (formEdicion.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var nuevoServicio = formEdicion.ObtenerServicio();
                        await FirebaseHelper.AñadirServicio(nuevoServicio);
                        await CargarServicios();
                        MostrarMensajeTemporal("Servicio añadido correctamente.", Color.Green);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al añadir servicio: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnModificar_Click(object sender, EventArgs e)
        {
            var firebaseID = ObtenerFirebaseIDSeleccionado();
            if (string.IsNullOrEmpty(firebaseID))
            {
                MessageBox.Show("Seleccione un servicio para modificar.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var servicioActual = await FirebaseHelper.ObtenerServicio(firebaseID);
                if (servicioActual == null)
                {
                    MessageBox.Show("El servicio seleccionado no existe en Firebase.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var form = new FormServicioEdicion(servicioActual))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var servicioModificado = form.ObtenerServicio();
                        await FirebaseHelper.ActualizarServicio(firebaseID, servicioModificado);
                        await CargarServicios();
                        MostrarMensajeTemporal("Servicio actualizado correctamente.", Color.FromArgb(70, 180, 220));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar servicio: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            var firebaseID = ObtenerFirebaseIDSeleccionado();
            if (string.IsNullOrEmpty(firebaseID))
            {
                MessageBox.Show("Seleccione un servicio para eliminar.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("¿Está seguro de eliminar este servicio?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                await FirebaseHelper.EliminarServicio(firebaseID);
                await CargarServicios();
                MostrarMensajeTemporal("Servicio eliminado correctamente.", Color.Red);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar servicio: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarMensajeTemporal(string mensaje, Color colorFondo)
        {
            Panel panelMensaje = new Panel
            {
                BackColor = colorFondo,
                Size = new Size(300, 50),
                Location = new Point(this.ClientSize.Width / 2 - 150, 20),
                Visible = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblMensaje = new Label
            {
                Text = mensaje,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            panelMensaje.Controls.Add(lblMensaje);
            this.Controls.Add(panelMensaje);
            panelMensaje.BringToFront();

            var mensajeTimer = new Timer { Interval = 3000 };
            mensajeTimer.Tick += (s, ev) =>
            {
                panelMensaje.Dispose();
                mensajeTimer.Stop();
            };
            mensajeTimer.Start();
        }

        private void InitializeRefreshAnimation()
        {
            imagenOriginal = buttonRefrescar.Image;
            rotarTimer = new Timer();
            rotarTimer.Interval = 50;
            rotarTimer.Tick += RotarTimer_Tick;
        }

        private void RotarTimer_Tick(object sender, EventArgs e)
        {
            angulo -= 10f;
            totalAnguloGirado += 10f;
            if (angulo <= -360f) angulo = 0f;
            buttonRefrescar.Image = RotarImagen(imagenOriginal, angulo);
            if (tareaTerminada && totalAnguloGirado >= 360f)
            {
                rotarTimer.Stop();
                buttonRefrescar.Image = imagenOriginal;
                estaRefrescando = false;
            }
        }

        private Image RotarImagen(Image img, float angle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform(img.Width / 2f, img.Height / 2f);
                g.RotateTransform(angle);
                g.TranslateTransform(-img.Width / 2f, -img.Height / 2f);
                g.DrawImage(img, new Point(0, 0));
            }
            return bmp;
        }

        private async void buttonRefrescar_Click(object sender, EventArgs e)
        {
            if (estaRefrescando) return;
            txtBuscar.Text = "Buscar servicio...";
            txtBuscar.ForeColor = Color.Gray;

            estaRefrescando = true;
            tareaTerminada = false;
            angulo = 0f;
            totalAnguloGirado = 0f;
            rotarTimer.Start();
            await CargarServicios();
            tareaTerminada = true;
        }


        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            _ = CargarServicios(txtBuscar.Text);
        }

        private async void InicializarWebView()
        {
            try
            {
                if (!datosCargados) return;

                var env = await CoreWebView2Environment.CreateAsync(
                    null,
                    Path.Combine(Application.StartupPath, "WebView2Cache"));

                await webViewMapita.EnsureCoreWebView2Async(env);

                webViewMapita.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(e.WebMessageAsJson);
                        if (data.tipo == "mapa_listo")
                        {
                            mapaListo = true;
                            this.BeginInvoke(new Action(async () =>
                            {
                                await Task.Delay(500);
                                await IntentarCalcularRutaDesdeDgv();
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error en mensaje WebView: {ex.Message}");
                    }
                };

                string htmlPath = Path.Combine(Application.StartupPath, "Resources", "map.html");
                webViewMapita.Source = new Uri(htmlPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar WebView2: {ex.Message}");
            }
        }

        private async Task IntentarCalcularRutaDesdeDgv()
        {
            if (!mapaListo || dgvServicios.CurrentRow == null) return;

            try
            {
                var fila = dgvServicios.CurrentRow;

                // Validar que las columnas existan
                if (dgvServicios.Columns["LatOrigen"] == null ||
                    dgvServicios.Columns["LngOrigen"] == null ||
                    dgvServicios.Columns["LatDestino"] == null ||
                    dgvServicios.Columns["LngDestino"] == null)
                {
                    MessageBox.Show("Algunas columnas necesarias no están disponibles.");
                    return;
                }

                int idxLatOrigen = dgvServicios.Columns["LatOrigen"].Index;
                int idxLngOrigen = dgvServicios.Columns["LngOrigen"].Index;
                int idxLatDestino = dgvServicios.Columns["LatDestino"].Index;
                int idxLngDestino = dgvServicios.Columns["LngDestino"].Index;

                // Validar valores antes de convertir
                if (!double.TryParse(fila.Cells[idxLatOrigen].Value?.ToString(), out double lat1))
                {
                    MessageBox.Show("Latitud de origen inválida.");
                    return;
                }
                if (!double.TryParse(fila.Cells[idxLngOrigen].Value?.ToString(), out double lng1))
                {
                    MessageBox.Show("Longitud de origen inválida.");
                    return;
                }
                if (!double.TryParse(fila.Cells[idxLatDestino].Value?.ToString(), out double lat2))
                {
                    MessageBox.Show("Latitud de destino inválida.");
                    return;
                }
                if (!double.TryParse(fila.Cells[idxLngDestino].Value?.ToString(), out double lng2))
                {
                    MessageBox.Show("Longitud de destino inválida.");
                    return;
                }

                string script = $@"
try {{
    drawRoute(
        {lat1.ToString(CultureInfo.InvariantCulture)},
        {lng1.ToString(CultureInfo.InvariantCulture)},
        {lat2.ToString(CultureInfo.InvariantCulture)},
        {lng2.ToString(CultureInfo.InvariantCulture)}
    );
}} catch (error) {{
    console.error('Error al dibujar ruta:', error.message);
}}";

                await webViewMapita.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MostrarErrorEnMapa($"Error al calcular ruta: {ex.Message}");
                Debug.WriteLine($"Error completo: {ex}");
            }
        }

        private void MostrarErrorEnMapa(string mensaje)
        {
            MessageBox.Show(mensaje, "Error en el mapa", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WebViewMapita_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                dynamic data = JsonConvert.DeserializeObject(e.WebMessageAsJson);
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    switch (data.tipo.ToString())
                    {
                        case "error":
                            MostrarErrorEnMapa(data.mensaje.ToString());
                            break;
                        default:
                            Debug.WriteLine($"Mensaje no reconocido: {e.WebMessageAsJson}");
                            break;
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error procesando mensaje WebView: {ex}");
            }
        }

        private async void dgvServicios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServicios.SelectedRows.Count > 0 && mapaListo)
            {
                await IntentarCalcularRutaDesdeDgv();
            }
        }

        private async void dgvServicios_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvServicios.SelectedRows.Count > 0 && mapaListo)
            {
                await IntentarCalcularRutaDesdeDgv();
            }
        }
    }
}