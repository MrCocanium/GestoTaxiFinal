using Firebase.Database.Query;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ProyectoFinalDAM
{
    public partial class FormPacientes : Form
    {
        private DataTable dataTable = new DataTable();
        private bool isFirstLoad = true;
        private readonly string rolUsuario;

        public FormPacientes(string rol)
        {
            InitializeComponent();
            rolUsuario = rol;
            ConfigureDataGridView();
            AplicarSeguridad();
            // Configurar el placeholder para txtBuscar
            txtBuscar.Text = "Buscar paciente..."; // Texto placeholder
            txtBuscar.ForeColor = Color.Gray; // Color del placeholder
            // Asociar eventos Enter y Leave
            txtBuscar.Enter += TxtBuscar_Enter;
            txtBuscar.Leave += TxtBuscar_Leave;
            // Cargar los datos al cargar el formulario
            this.Load += async (sender, e) => await CargarPacientes();
        }
        private void AplicarSeguridad()
        {
            Seguridad.DesactivarAccionesPorRol(rolUsuario, btnAñadir, btnModificar, btnEliminar);
        }

        private void ConfigureDataGridView()
        {
            dgvPacientes.DataSource = dataTable;
            dgvPacientes.AutoGenerateColumns = false;
            dgvPacientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPacientes.AllowUserToAddRows = false;

            // Configuración manual de columnas
            dataTable.Columns.Add("Nombre", typeof(string));
            dataTable.Columns.Add("DNI", typeof(string));
            dataTable.Columns.Add("ID", typeof(string)); // Nueva columna para el identificador

            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 150
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DNI",
                HeaderText = "DNI",
                Width = 100
            });

            // Columna oculta para el ID
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "ID",
                Visible = false // Ocultar esta columna
            });
        }

        private async Task CargarPacientes(string filtro = "")
        {
            if (filtro == "Buscar paciente...")
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
                var pacientes = await FirebaseHelper.ObtenerTodosLosPacientes(); // Devuelve FirebaseObject<Paciente>
                Console.WriteLine($"Datos cargados desde Firebase: {pacientes.Count} pacientes encontrados.");
                if (pacientes == null || pacientes.Count == 0)
                {
                    lblEstado.Text = "No se encontraron datos.";
                    return;
                }
                dataTable.Rows.Clear();
                foreach (var paciente in pacientes)
                {
                    string id = paciente.Key; // Obtener el identificador (ID)
                    var datosPaciente = paciente.Object; // Obtener los datos del paciente
                    if (string.IsNullOrEmpty(filtro) ||
                        datosPaciente.Nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        datosPaciente.DNI.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dataTable.Rows.Add(
                            datosPaciente.Nombre,
                            datosPaciente.DNI,
                            id // Añadir el identificador (ID)
                        );
                    }
                }
                dgvPacientes.DataSource = null;
                dgvPacientes.DataSource = dataTable;
                if (isFirstLoad) // Mostrar mensaje de confirmación solo en la primera carga
                {
                    lblEstado.Text = "Datos cargados correctamente.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void buttonRefrescar_Click(object sender, EventArgs e)
        {
            // Evitar múltiples clics mientras se está refrescando
            if (estaRefrescando) return;

            // Limpiar el campo de búsqueda
            txtBuscar.Text = "";

            // Inicializar estados
            estaRefrescando = true;
            tareaTerminada = false;
            angulo = 0f;
            totalAnguloGirado = 0f;

            // Iniciar la animación de rotación
            rotarTimer.Start();

            try
            {
                // Recargar los datos en el DataGridView
                await CargarPacientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al recargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Marcar que la tarea ha terminado
                tareaTerminada = true;
            }
        }

        private async void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            await CargarPacientes(txtBuscar.Text);
        }

        private void TxtBuscar_Enter(object sender, EventArgs e)
        {
            // Si el texto es el placeholder, limpiarlo y cambiar el color
            if (txtBuscar.Text == "Buscar paciente...")
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
                txtBuscar.Text = "Buscar paciente...";
                txtBuscar.ForeColor = Color.Gray; // Color del placeholder
            }
        }

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

        private async void btnAñadir_Click(object sender, EventArgs e)
        {
            using (var formEdicion = new FormPacienteEdicion())
            {
                if (formEdicion.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await FirebaseHelper.AñadirPaciente(formEdicion.Paciente);
                        await CargarPacientes(txtBuscar.Text); // Recargar los datos en el DataGridView
                        MostrarMensajeTemporal("Paciente añadido correctamente.", Color.Green);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al añadir paciente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnModificar_Click(object sender, EventArgs e)
        {
            // 1. Obtener el ID del paciente seleccionado
            string idSeleccionado = ObtenerIDDeFilaSeleccionada();
            if (string.IsNullOrEmpty(idSeleccionado))
            {
                MessageBox.Show("Seleccione un paciente para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2. Obtener los datos actuales del paciente desde Firebase
            try
            {
                var todosLosPacientes = await FirebaseHelper.ObtenerTodosLosPacientes();
                var pacienteActual = todosLosPacientes.FirstOrDefault(t => t.Key == idSeleccionado)?.Object;
                if (pacienteActual == null)
                {
                    MessageBox.Show("No se encontró el paciente seleccionado en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 3. Abrir el formulario de edición con los datos actuales
                using (var formEdicion = new FormPacienteEdicion(pacienteActual))
                {
                    if (formEdicion.ShowDialog() == DialogResult.OK)
                    {
                        // 4. Actualizar en Firebase
                        await FirebaseHelper.ActualizarPaciente(idSeleccionado, formEdicion.Paciente);
                        // 5. Recargar la lista y mostrar mensaje
                        await CargarPacientes(txtBuscar.Text);
                        MostrarMensajeTemporal("Paciente actualizado correctamente.", Color.FromArgb(70, 180, 220));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar paciente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            // 1. Obtener el ID del paciente seleccionado
            string idSeleccionado = ObtenerIDDeFilaSeleccionada();
            if (string.IsNullOrEmpty(idSeleccionado))
            {
                MessageBox.Show("Seleccione un paciente para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2. Confirmar eliminación
            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este paciente? Esta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (respuesta != DialogResult.Yes)
                return;
            // 3. Eliminar de Firebase
            try
            {
                await FirebaseHelper.EliminarPaciente(idSeleccionado);
                // 4. Recargar la lista y mostrar mensaje
                await CargarPacientes(txtBuscar.Text);
                MostrarMensajeTemporal("Paciente eliminado correctamente.", Color.FromArgb(200, 0, 0)); // Rojo oscuro
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar paciente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerIDDeFilaSeleccionada()
        {
            if (dgvPacientes.SelectedRows.Count == 0)
                return null;
            // La columna "ID" es la última (índice = TotalColumnas - 1)
            int indiceColumnaID = dgvPacientes.Columns.Count - 1;
            var idCell = dgvPacientes.SelectedRows[0].Cells[indiceColumnaID];
            return idCell.Value?.ToString();
        }

        private System.Windows.Forms.Timer mensajeTimer;
        private Timer rotarTimer;
        private float angulo = 0f;
        private float totalAnguloGirado = 0f;
        private Image imagenOriginal;
        private bool estaRefrescando = false;
        private bool tareaTerminada = false;
        private void RotarTimer_Tick(object sender, EventArgs e)
        {
            // Girar la imagen en sentido antihorario
            angulo -= 10f;
            totalAnguloGirado += 10f;

            // Si completa una vuelta completa, reiniciar el ángulo
            if (angulo <= -360f) angulo = 0f;

            // Aplicar la rotación a la imagen del botón
            buttonRefrescar.Image = RotarImagen(imagenOriginal, angulo);

            // Detener la animación si la tarea ha terminado y se ha girado al menos una vuelta
            if (tareaTerminada && totalAnguloGirado >= 360f)
            {
                rotarTimer.Stop();
                buttonRefrescar.Image = imagenOriginal; // Restaurar la imagen original
                estaRefrescando = false; // Marcar como no refrescando
            }
        }

        private Image RotarImagen(Image img, float angle)
        {
            // Crear un bitmap del mismo tamaño que la imagen original
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // Dibujar la imagen rotada
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform(img.Width / 2f, img.Height / 2f); // Mover al centro
                g.RotateTransform(angle); // Rotar
                g.TranslateTransform(-img.Width / 2f, -img.Height / 2f); // Volver al origen
                g.DrawImage(img, new Point(0, 0)); // Dibujar la imagen
            }

            return bmp;
        }

        private void FormPacientes_Load(object sender, EventArgs e)
        {
            // Guardar la imagen original del botón
            imagenOriginal = buttonRefrescar.Image;

            // Configurar el temporizador para la animación de rotación
            rotarTimer = new Timer();
            rotarTimer.Interval = 50; // Intervalo de 50ms para animación suave
            rotarTimer.Tick += RotarTimer_Tick;
        }

    }
}