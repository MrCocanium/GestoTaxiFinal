using Firebase.Database.Query;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public partial class FormTaxistas : Form
    {
        private DataTable dataTable = new DataTable();
        private bool isFirstLoad = true;
        private readonly string rolUsuario;

        public FormTaxistas(string rol)
        {
            InitializeComponent();
            rolUsuario = rol;
            ConfigureDataGridView();
            AplicarSeguridad();

            // Configurar el placeholder para txtBuscar
            txtBuscar.Text = "Buscar taxista..."; // Texto placeholder
            txtBuscar.ForeColor = Color.Gray; // Color del placeholder

            // Asociar eventos Enter y Leave
            txtBuscar.Enter += TxtBuscar_Enter;
            txtBuscar.Leave += TxtBuscar_Leave;

            // Cargar los datos al cargar el formulario
            this.Load += async (sender, e) => await CargarTaxistas();
        }
        private void AplicarSeguridad()
        {
            Seguridad.DesactivarAccionesPorRol(rolUsuario, btnAñadir, btnModificar, btnEliminar);
        }

        private void ConfigureDataGridView()
        {
            dgvTaxistas.DataSource = dataTable;
            dgvTaxistas.AutoGenerateColumns = false;
            dgvTaxistas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaxistas.AllowUserToAddRows = false;

            // Configuración manual de columnas
            dataTable.Columns.Add("Nombre", typeof(string));
            dataTable.Columns.Add("DNI", typeof(string));
            dataTable.Columns.Add("Teléfono", typeof(string)); // Cambiado a string para evitar problemas con números grandes
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Dirección", typeof(string));
            dataTable.Columns.Add("Población", typeof(string));
            dataTable.Columns.Add("Provincia", typeof(string));
            dataTable.Columns.Add("CCC", typeof(string));
            dataTable.Columns.Add("ID", typeof(string)); // Nueva columna para el identificador

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 150
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DNI",
                HeaderText = "DNI",
                Width = 100
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Teléfono",
                HeaderText = "Teléfono",
                Width = 100
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 150
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Dirección",
                HeaderText = "Dirección",
                Width = 200
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Población",
                HeaderText = "Población",
                Width = 100
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Provincia",
                HeaderText = "Provincia",
                Width = 100
            });

            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "CCC",
                HeaderText = "CCC",
                Width = 200
            });

            // Columna oculta para el ID
            dgvTaxistas.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ID",
                Visible = false // Ocultar esta columna
            });
        }

        private async Task CargarTaxistas(string filtro = "")
        {
            if (filtro == "Buscar taxista...")
            {
                filtro = "";
            }

            if (isFirstLoad) // Mostrar ProgressBar y mensaje solo en la primera carga
            {
                progressBarCarga.Visible = true;
                lblEstado.Visible = true;
                lblEstado.Text = "Cargando datos...";
            }

            try
            {
                var taxistas = await FirebaseHelper.ObtenerTodosLosTaxistas(); // Devuelve FirebaseObject<Taxista>
                Console.WriteLine($"Datos cargados desde Firebase: {taxistas.Count} taxistas encontrados.");

                if (taxistas == null || taxistas.Count == 0)
                {
                    lblEstado.Text = "No se encontraron datos.";
                    return;
                }

                dataTable.Rows.Clear();
                foreach (var taxista in taxistas)
                {
                    string id = taxista.Key; // Obtener el identificador (ID)
                    var datosTaxista = taxista.Object; // Obtener los datos del taxista

                    if (string.IsNullOrEmpty(filtro) ||
                        datosTaxista.Nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        datosTaxista.DNI.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dataTable.Rows.Add(
                            datosTaxista.Nombre,
                            datosTaxista.DNI,
                            datosTaxista.NumeroTelefono.ToString(), // Convertir a string
                            datosTaxista.Email,
                            datosTaxista.Direccion,
                            datosTaxista.Poblacion,
                            datosTaxista.Provincia,
                            datosTaxista.CCC,
                            id // Añadir el identificador (ID)
                        );
                    }
                }

                dgvTaxistas.DataSource = null;
                dgvTaxistas.DataSource = dataTable;

                if (isFirstLoad) // Mostrar mensaje de confirmación solo en la primera carga
                {
                    lblEstado.Text = "Datos cargados correctamente.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar taxistas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (isFirstLoad) // Ocultar ProgressBar y mensaje solo en la primera carga
                {
                    progressBarCarga.Visible = false;
                    lblEstado.Visible = false;
                    lblEstado.Text = "";
                    isFirstLoad = false; // Marcar como cargado
                }
            }
        }

        private void btnRecargar_Click(object sender, EventArgs e)
        {
            _ = CargarTaxistas(txtBuscar.Text);
        }

        private async void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            await CargarTaxistas(txtBuscar.Text);
        }

        private async void txtBuscar_TextChanged_1(object sender, EventArgs e)
        {
            await CargarTaxistas(txtBuscar.Text);
        }

        private void TxtBuscar_Enter(object sender, EventArgs e)
        {
            // Si el texto es el placeholder, limpiarlo y cambiar el color
            if (txtBuscar.Text == "Buscar taxista...")
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = Color.Black; // Color normal
            }
        }

        private void TxtBuscar_Leave(object sender, EventArgs e)
        {
            // Si el TextBox está vacío, restaurar el placeholder
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar taxista...";
                txtBuscar.ForeColor = Color.Gray; // Color del placeholder
            }
        }

        private System.Windows.Forms.Timer mensajeTimer;

        private void MostrarMensajeTemporal(string mensaje, Color colorFondo)
        {
            // Crear el Panel si no existe
            Panel panelMensaje = new Panel
            {
                BackColor = colorFondo,
                Size = new System.Drawing.Size(300, 50),
                Location = new System.Drawing.Point(this.ClientSize.Width / 2 - 150, 20), // Centrado horizontalmente
                Visible = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Crear el Label dentro del Panel
            Label lblMensaje = new Label
            {
                Text = mensaje,
                ForeColor = Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            // Agregar el Label al Panel
            panelMensaje.Controls.Add(lblMensaje);

            // Agregar el Panel al formulario
            this.Controls.Add(panelMensaje);
            panelMensaje.BringToFront();

            // Iniciar el temporizador
            mensajeTimer?.Dispose(); // Detener el temporizador anterior si existe
            mensajeTimer = new System.Windows.Forms.Timer { Interval = 3000 }; // 3 segundos
            mensajeTimer.Tick += (s, ev) =>
            {
                panelMensaje.Dispose(); // Eliminar el Panel después de 3 segundos
                mensajeTimer.Stop();
            };
            mensajeTimer.Start();
        }

        private async void btnAñadir_Click_1(object sender, EventArgs e)
        {
            using (var formEdicion = new FormTaxistaEdicion())
            {
                if (formEdicion.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await FirebaseHelper.AñadirTaxista(formEdicion.Taxista);
                        await CargarTaxistas(txtBuscar.Text); // Recargar los datos en el DataGridView
                        MostrarMensajeTemporal("Taxista añadido correctamente.", Color.Green);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al añadir taxista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnModificar_Click_1(object sender, EventArgs e)
        {
            // 1. Obtener el ID del taxista seleccionado
            string idSeleccionado = ObtenerIDDeFilaSeleccionada();
            if (string.IsNullOrEmpty(idSeleccionado))
            {
                MessageBox.Show("Seleccione un taxista para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Obtener los datos actuales del taxista desde Firebase
            try
            {
                var todosLosTaxistas = await FirebaseHelper.ObtenerTodosLosTaxistas();
                var taxistaActual = todosLosTaxistas.FirstOrDefault(t => t.Key == idSeleccionado)?.Object;

                if (taxistaActual == null)
                {
                    MessageBox.Show("No se encontró el taxista seleccionado en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Abrir el formulario de edición con los datos actuales
                using (var formEdicion = new FormTaxistaEdicion(taxistaActual))
                {
                    if (formEdicion.ShowDialog() == DialogResult.OK)
                    {
                        // 4. Actualizar en Firebase
                        await FirebaseHelper.ActualizarTaxista(idSeleccionado, formEdicion.Taxista);

                        // 5. Recargar la lista y mostrar mensaje
                        await CargarTaxistas(txtBuscar.Text);
                        MostrarMensajeTemporal("Taxista actualizado correctamente.", Color.FromArgb(70, 180, 220));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar taxista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEliminar_Click_1(object sender, EventArgs e)
        {
            // 1. Obtener el ID del taxista seleccionado
            string idSeleccionado = ObtenerIDDeFilaSeleccionada();
            if (string.IsNullOrEmpty(idSeleccionado))
            {
                MessageBox.Show("Seleccione un taxista para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Confirmar eliminación
            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este taxista? Esta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta != DialogResult.Yes)
                return;

            // 3. Eliminar de Firebase
            try
            {
                await FirebaseHelper.EliminarTaxista(idSeleccionado);

                // 4. Recargar la lista y mostrar mensaje
                await CargarTaxistas(txtBuscar.Text);
                MostrarMensajeTemporal("Taxista eliminado correctamente.", Color.FromArgb(200, 0, 0)); // Rojo oscuro
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar taxista: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string ObtenerIDDeFilaSeleccionada()
        {
            if (dgvTaxistas.SelectedRows.Count == 0)
                return null;

            // La columna "ID" es la última (índice = TotalColumnas - 1)
            int indiceColumnaID = dgvTaxistas.Columns.Count - 1;
            var idCell = dgvTaxistas.SelectedRows[0].Cells[indiceColumnaID];

            return idCell.Value?.ToString();
        }

        private void FormTaxistas_Load(object sender, EventArgs e)
        {
            imagenOriginal = buttonRefrescar.Image;

            rotarTimer = new Timer();
            rotarTimer.Interval = 50;
            rotarTimer.Tick += RotarTimer_Tick;

        }

        private void RotarTimer_Tick(object sender, EventArgs e)
        {
            angulo -= 10f; // sentido antihorario
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

            txtBuscar.Text = "";

            // Inicializa estados
            estaRefrescando = true;
            tareaTerminada = false;
            angulo = 0f;
            totalAnguloGirado = 0f;

            rotarTimer.Start();

            // Ejecuta la tarea
            await CargarTaxistas();

            // Marca que terminó, pero no detenemos aún hasta mínimo 1 vuelta
            tareaTerminada = true;
        }


        private Timer rotarTimer;
        private float angulo = 0f;
        private float totalAnguloGirado = 0f;
        private Image imagenOriginal;
        private bool estaRefrescando = false;
        private bool tareaTerminada = false;


    }
}