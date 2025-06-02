using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;

namespace ProyectoFinalDAM
{
    public partial class FormLugares : Form
    {
        private Timer rotarTimer;
        private float angulo = 0f;
        private float totalAnguloGirado = 0f;
        private Image imagenOriginalBotonRefrescar;
        private bool estaRefrescando = false;
        private bool tareaTerminada = false;
        private List<Lugar> lugaresCargados = new List<Lugar>();
        private readonly string placeholderTexto = "Buscar lugar...";
        private readonly Color placeholderColor = Color.Gray;
        private readonly Color textoNormalColor = Color.Black;

        private Panel panelEdicion;
        private TextBox txtNombreEdicion;
        private Button btnAceptarEdicion, btnCancelarEdicion, btnSeleccionarEnMapaEdicion;
        private double? latitudSeleccionada = null;
        private double? longitudSeleccionada = null;
        private Lugar lugarEnEdicion = null;
        private readonly string rolUsuario;

        public FormLugares(string rol)
        {
            InitializeComponent();
            rolUsuario = rol;
            InitializeWebView();
            AplicarSeguridad();
            InitializeRefreshAnimation(); // Animación del botón refrescar
            InicializarPlaceholder();     // Placeholder en txtBuscar
            InitializePanelEdicion();     // Panel emergente para edición

            dataGridViewLugares.SelectionChanged += dataGridViewLugares_SelectionChanged;
            dataGridViewLugares.CellContentDoubleClick += dataGridViewLugares_CellContentDoubleClick;
            txtBuscar.TextChanged += txtBuscar_TextChanged;
        }

        #region Métodos UI

        private void AplicarSeguridad()
        {
            if (rolUsuario != "admin") // o cualquier lógica que uses
            {
                // Quita el evento de doble clic
                dataGridViewLugares.CellContentDoubleClick -= dataGridViewLugares_CellContentDoubleClick;

                // Desactiva el botón Eliminar
                btnEliminar.Enabled = false;
            }
        }

        private void InicializarPlaceholder()
        {
            txtBuscar.Text = placeholderTexto;
            txtBuscar.ForeColor = placeholderColor;
            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == placeholderTexto)
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = textoNormalColor;
                }
            };
            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = placeholderTexto;
                    txtBuscar.ForeColor = placeholderColor;
                }
            };
        }

        private void InitializeRefreshAnimation()
        {
            imagenOriginalBotonRefrescar = buttonRefrescar.Image;
            rotarTimer = new Timer { Interval = 50 };
            rotarTimer.Tick += RotarTimer_Tick;
        }

        private void RotarTimer_Tick(object sender, EventArgs e)
        {
            angulo -= 10f;
            totalAnguloGirado += 10f;
            if (angulo <= -360f) angulo = 0f;
            buttonRefrescar.Image = RotarImagen(imagenOriginalBotonRefrescar, angulo);
            if (tareaTerminada && totalAnguloGirado >= 360f)
            {
                rotarTimer.Stop();
                buttonRefrescar.Image = imagenOriginalBotonRefrescar;
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

        #endregion

        #region WebView - Mapa

        async void InitializeWebView()
        {
            if (webViewMapaLugares == null)
            {
                webViewMapaLugares = new WebView2();
                this.Controls.Add(webViewMapaLugares);
                webViewMapaLugares.Dock = DockStyle.Fill;
            }

            await webViewMapaLugares.EnsureCoreWebView2Async(null);

            string htmlFilePath = Path.Combine(Application.StartupPath, "Resources", "place.html");
            if (File.Exists(htmlFilePath))
            {
                webViewMapaLugares.Source = new Uri("file:///" + htmlFilePath.Replace("\\", "/"));
            }
            else
            {
                MessageBox.Show("No se encontró el archivo place.html.");
            }

            webViewMapaLugares.CoreWebView2.WebMessageReceived += async (sender, args) =>
            {
                string message = args.TryGetWebMessageAsString();
                if (message.Contains("lugar_seleccionado"))
                {
                    try
                    {
                        JObject lugarJson = JObject.Parse(message);
                        string descripcion = lugarJson["nombre"]?.ToString() ?? "Sin nombre";
                        double latitud = TryParseDouble(lugarJson["lat"]);
                        double longitud = TryParseDouble(lugarJson["lng"]);

                        int nuevoId;
                        var lugaresExistentes = await FirebaseHelper.ObtenerTodosLosLugares();
                        nuevoId = lugaresExistentes.Count == 0 ? 1 : lugaresExistentes.Max(l => l.Object.ID) + 1;

                        var lugar = new Lugar
                        {
                            ID = nuevoId,
                            Descripcion = descripcion,
                            Latitud = latitud,
                            Longitud = longitud
                        };

                        if (!lugar.CoordenadasValidas())
                        {
                            MessageBox.Show("Las coordenadas no son válidas.");
                            return;
                        }

                        string firebaseId = await FirebaseHelper.AñadirLugar(lugar);
                        lugar.FirebaseID = firebaseId;
                        await CargarLugaresDesdeFirebase();
                        MostrarMensajeTemporal("Lugar guardado correctamente.", Color.Green);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al procesar el lugar: " + ex.Message);
                    }
                }
                else if (message.Contains("ubicacion_seleccionada"))
                {
                    try
                    {
                        JObject json = JObject.Parse(message);
                        latitudSeleccionada = (double)json["lat"];
                        longitudSeleccionada = (double)json["lng"];
                        MessageBox.Show("✅ Ubicación seleccionada.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("❌ Error al obtener coordenadas: " + ex.Message);
                    }
                }
            };

            await CargarLugaresDesdeFirebase();
        }

        private double TryParseDouble(JToken token)
        {
            if (token == null) return double.NaN;
            string valueStr = token.ToString().Trim();
            return double.TryParse(valueStr, out double result) ? result : double.NaN;
        }

        #endregion

        #region Panel de Edición

        private void InitializePanelEdicion()
        {
            panelEdicion = new Panel
            {
                Size = new Size(300, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 200) / 2)
            };

            Label lblTitulo = new Label
            {
                Text = "Editar lugar",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 30,
                ForeColor = Color.Black
            };

            txtNombreEdicion = new TextBox
            {
                Width = 250,
                Location = new Point(25, 40)
            };
            txtNombreEdicion.Text = "Nombre del lugar";
            txtNombreEdicion.ForeColor = Color.Gray;

            txtNombreEdicion.GotFocus += (s, ev) =>
            {
                if (txtNombreEdicion.Text == "Nombre del lugar")
                {
                    txtNombreEdicion.Text = "";
                    txtNombreEdicion.ForeColor = Color.Black;
                }
            };

            txtNombreEdicion.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtNombreEdicion.Text))
                {
                    txtNombreEdicion.Text = "Nombre del lugar";
                    txtNombreEdicion.ForeColor = Color.Gray;
                }
            };

            btnAceptarEdicion = new Button
            {
                Text = "Aceptar",
                Location = new Point(25, 125),
                Width = 115
            };
            btnAceptarEdicion.Click += BtnAceptarEdicion_Click;

            btnCancelarEdicion = new Button
            {
                Text = "Cancelar",
                Location = new Point(160, 125),
                Width = 115
            };
            btnCancelarEdicion.Click += BtnCancelarEdicion_Click;

            panelEdicion.Controls.Add(lblTitulo);
            panelEdicion.Controls.Add(txtNombreEdicion);
            panelEdicion.Controls.Add(btnSeleccionarEnMapaEdicion);
            panelEdicion.Controls.Add(btnAceptarEdicion);
            panelEdicion.Controls.Add(btnCancelarEdicion);

            this.Controls.Add(panelEdicion);
        }

        private void SeleccionarUbicacion_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string message = args.TryGetWebMessageAsString();
            if (message.Contains("ubicacion_seleccionada"))
            {
                try
                {
                    JObject json = JObject.Parse(message);
                    latitudSeleccionada = (double)json["lat"];
                    longitudSeleccionada = (double)json["lng"];
                    MessageBox.Show("✅ Ubicación seleccionada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error al obtener coordenadas: " + ex.Message);
                }
                finally
                {
                    // Desuscribirse tras recibir el mensaje
                    webViewMapaLugares.CoreWebView2.WebMessageReceived -= SeleccionarUbicacion_WebMessageReceived;
                }
            }
        }

        private async void BtnAceptarEdicion_Click(object sender, EventArgs e)
        {
            if (lugarEnEdicion == null) return;

            string nuevoNombre = txtNombreEdicion.Text.Trim();
            if (nuevoNombre == "Nombre del lugar" || string.IsNullOrEmpty(nuevoNombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lugarEnEdicion.Descripcion = nuevoNombre;

            if (latitudSeleccionada.HasValue && longitudSeleccionada.HasValue)
            {
                lugarEnEdicion.Latitud = latitudSeleccionada.Value;
                lugarEnEdicion.Longitud = longitudSeleccionada.Value;
            }

            try
            {
                await FirebaseHelper.ActualizarLugar(lugarEnEdicion.FirebaseID, lugarEnEdicion);
                await CargarLugaresDesdeFirebase();

                string script = $"mostrarLugarDesdeApp('{lugarEnEdicion.Descripcion.Replace("'", "\\'")}', {lugarEnEdicion.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lugarEnEdicion.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture)});";
                await webViewMapaLugares.CoreWebView2.ExecuteScriptAsync(script);

                MostrarMensajeTemporal("✅ Lugar actualizado correctamente.", Color.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al actualizar lugar: " + ex.Message);
            }

            lugarEnEdicion = null;
            panelEdicion.Visible = false;
        }

        private void BtnCancelarEdicion_Click(object sender, EventArgs e)
        {
            lugarEnEdicion = null;
            panelEdicion.Visible = false;
        }

        #endregion

        #region Eventos DataGridView

        private void dataGridViewLugares_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var lugar = dataGridViewLugares.Rows[e.RowIndex].DataBoundItem as Lugar;
            if (lugar == null || string.IsNullOrEmpty(lugar.FirebaseID)) return;

            lugarEnEdicion = lugar;
            txtNombreEdicion.Text = lugar.Descripcion;
            latitudSeleccionada = lugar.Latitud;
            longitudSeleccionada = lugar.Longitud;

            panelEdicion.Visible = true;
            panelEdicion.BringToFront();
        }

        private async void dataGridViewLugares_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewLugares.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewLugares.SelectedRows[0];
                var lugar = selectedRow.DataBoundItem as Lugar;
                if (lugar != null && lugar.CoordenadasValidas())
                {
                    string script = $"mostrarLugarDesdeApp('{lugar.Descripcion.Replace("'", "\\'")}', {lugar.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {lugar.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture)});";
                    await webViewMapaLugares.CoreWebView2.ExecuteScriptAsync(script);
                }
            }
        }

        #endregion

        #region Otros métodos

        private void ConfigurarDataGridView()
        {
            dataGridViewLugares.AutoGenerateColumns = false;
            dataGridViewLugares.Columns.Clear();
            dataGridViewLugares.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descripcion",
                HeaderText = "Lugar",
                Name = "Descripcion",
                Width = 200
            });
            dataGridViewLugares.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FirebaseID",
                HeaderText = "Clave Firebase",
                Name = "FirebaseID",
                Width = 250,
                ReadOnly = true,
                Visible = false
            });
            foreach (var col in new[] { "ID", "Latitud", "Longitud" })
            {
                dataGridViewLugares.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = col,
                    Visible = false,
                    Name = col
                });
            }
        }

        private async Task CargarLugaresDesdeFirebase()
        {
            try
            {
                var lugaresFirebase = await FirebaseHelper.ObtenerTodosLosLugares();
                lugaresCargados = lugaresFirebase
                .Select(f => new Lugar
                {
                    ID = f.Object.ID,
                    Descripcion = f.Object.Descripcion,
                    Latitud = f.Object.Latitud,
                    Longitud = f.Object.Longitud,
                    FirebaseID = f.Key
                })
                .OrderBy(l => l.Descripcion) // Orden alfabético
                .ToList();

                ConfigurarDataGridView();
                dataGridViewLugares.DataSource = lugaresCargados;
                dataGridViewLugares.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los lugares desde Firebase: " + ex.Message);
            }
        }

        private async void buttonRefrescar_Click(object sender, EventArgs e)
        {
            if (estaRefrescando) return;
            estaRefrescando = true;
            tareaTerminada = false;
            angulo = 0f;
            totalAnguloGirado = 0f;
            txtBuscar.Text = placeholderTexto;
            txtBuscar.ForeColor = placeholderColor;
            rotarTimer.Start();
            await CargarLugaresDesdeFirebase();
            tareaTerminada = true;
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewLugares.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un lugar para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedRow = dataGridViewLugares.SelectedRows[0];
            var lugar = selectedRow.DataBoundItem as Lugar;
            if (lugar == null || string.IsNullOrEmpty(lugar.FirebaseID))
            {
                MessageBox.Show("No se pudo obtener el lugar seleccionado o falta la clave de Firebase.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult confirm = MessageBox.Show($"¿Está seguro de eliminar el lugar '{lugar.Descripcion}'?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    await FirebaseHelper.EliminarLugar(lugar.FirebaseID);
                    await CargarLugaresDesdeFirebase();
                    MostrarMensajeTemporal("Lugar eliminado correctamente.", Color.Red);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar lugar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscar.Text == placeholderTexto) return;
            string textoFiltro = txtBuscar.Text.Trim().ToLower();
            var resultados = lugaresCargados
                .Where(l => l.Descripcion.ToLower().Contains(textoFiltro))
                .ToList();
            dataGridViewLugares.DataSource = resultados;
            dataGridViewLugares.ClearSelection();
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
            
        #endregion
    }
}