using System;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using iTextSharp.text.pdf;
using SD = System.Drawing;
using SD2D = System.Drawing.Drawing2D;
using iTextSharp.text;

namespace ProyectoFinalDAM
{
    public partial class FormFacturas : Form
    {
        private DataTable dtFacturas = new DataTable("Facturas");
        private DataTable dtServicios = new DataTable("Servicios");
        private Label lblNoPDF;
        private Timer rotationTimer;
        private float rotationAngle = 0;
        private bool isRotating = false;
        private string rolUsuario;


        public FormFacturas(string rol)
        {
            InitializeComponent();
            this.rolUsuario = rol; // Guardamos el rol
            InicializarDataGridViews();
            CargarDatosAsync();
            dgvFacturas.SelectionChanged += dgvFacturas_SelectionChanged;
            InicializarLabelNoPDF();
            ConfigurarPermisosPorRol(); // Aplicamos los permisos según el rol
        }

        private void ConfigurarPermisosPorRol()
        {
            if (rolUsuario != "admin")
            {
                btnImprimir.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void IniciarRotacion()
        {
            if (rotationTimer == null)
            {
                rotationTimer = new Timer();
                rotationTimer.Interval = 50;
                rotationTimer.Tick += (s, e) =>
                {
                    rotationAngle -= 5;
                    if (rotationAngle < 0) rotationAngle += 360;
                    buttonRefrescar.Invalidate();
                };
            }
            isRotating = true;
            rotationTimer.Start();
        }

        private void DetenerRotacion()
        {
            isRotating = false;
            rotationTimer.Stop();
            rotationAngle = 0;
            buttonRefrescar.Invalidate();
        }

        private void InicializarDataGridViews()
        {
            // Configurar dgvFacturas
            dgvFacturas.AutoGenerateColumns = false;
            dgvFacturas.Columns.Clear();

            var colFacturaKey = new DataGridViewTextBoxColumn();
            colFacturaKey.HeaderText = "ID Factura";
            colFacturaKey.DataPropertyName = "FacturaKey";
            colFacturaKey.Name = "FacturaKey";
            dgvFacturas.Columns.Add(colFacturaKey);

            dtFacturas.Clear();
            dtFacturas.Columns.Add("FacturaKey", typeof(string));
            dgvFacturas.DataSource = dtFacturas;

            // Configurar dgvServichio
            dgvServichio.AutoGenerateColumns = false;
            dgvServichio.Columns.Clear();
            dtServicios.Clear();
            dtServicios.Columns.Clear();

            dtServicios.Columns.Add("ID", typeof(string));
            dtServicios.Columns.Add("Fecha", typeof(string));
            dtServicios.Columns.Add("Kilometros", typeof(int));
            dtServicios.Columns.Add("HorasEspera", typeof(int));
            dtServicios.Columns.Add("Taxista", typeof(string));
            dtServicios.Columns.Add("Paciente", typeof(string));
            dtServicios.Columns.Add("LugarOrigen", typeof(string));
            dtServicios.Columns.Add("LugarDestino", typeof(string));

            foreach (DataColumn column in dtServicios.Columns)
            {
                var col = new DataGridViewTextBoxColumn();
                col.HeaderText = column.ColumnName;
                col.DataPropertyName = column.ColumnName;
                col.Name = column.ColumnName;
                dgvServichio.Columns.Add(col);
            }

            dgvServichio.DataSource = dtServicios;
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                var firebaseFacturas = await FirebaseHelper.ObtenerTodasLasFacturas();
                var firebaseServicios = await FirebaseHelper.ObtenerTodosLosServicios();
                var firebaseTaxistas = await FirebaseHelper.ObtenerTodosLosTaxistas();
                var firebasePacientes = await FirebaseHelper.ObtenerTodosLosPacientes();
                var firebaseLugares = await FirebaseHelper.ObtenerTodosLosLugares();

                dtFacturas.Clear();
                dtServicios.Clear();

                foreach (var factura in firebaseFacturas)
                {
                    dtFacturas.Rows.Add(factura.Key); // factura.Key: "factura1", etc.
                }

                foreach (var servicio in firebaseServicios)
                {
                    var datos = servicio.Object;
                    string nombreTaxista = firebaseTaxistas.FirstOrDefault(t => t.Key == datos.Taxista)?.Object?.Nombre ?? "Desconocido";
                    string nombrePaciente = firebasePacientes.FirstOrDefault(p => p.Key == datos.Paciente)?.Object?.Nombre ?? "Desconocido";
                    string lugarOrigen = firebaseLugares.FirstOrDefault(l => l.Key == datos.LugarOrigen)?.Object?.Descripcion ?? "Sin ubicación";
                    string lugarDestino = firebaseLugares.FirstOrDefault(l => l.Key == datos.LugarDestino)?.Object?.Descripcion ?? "Sin ubicación";

                    dtServicios.Rows.Add(
                        datos.ID,
                        datos.Fecha,
                        datos.Kilometros,
                        datos.HorasEspera,
                        nombreTaxista,
                        nombrePaciente,
                        lugarOrigen,
                        lugarDestino
                    );
                }

                dgvFacturas.Refresh();
                dgvServichio.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
        }

        private async void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvServichio.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un servicio para generar la factura.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var taxistasSeleccionados = dgvServichio.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(r => r.Cells["Taxista"].Value?.ToString())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .ToList();

                if (taxistasSeleccionados.Count > 1)
                {
                    MessageBox.Show("Todos los servicios seleccionados deben pertenecer al mismo taxista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string taxista = taxistasSeleccionados.First();
                var fechas = dgvServichio.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(r =>
                    {
                        string fechaStr = r.Cells["Fecha"].Value?.ToString();
                        return DateTime.TryParse(fechaStr, out DateTime dt) ? dt : DateTime.Now;
                    })
                    .OrderBy(d => d)
                    .ToList();

                string fechaFacturaStr = fechas.FirstOrDefault().ToString("yyyyMMdd");
                string nombreBase = $"FAC-{fechaFacturaStr}-{taxista.Replace(" ", "")}";

                int numeroSecuencial = await ObtenerNumeroSecuencialFactura(nombreBase);
                string nombreArchivo = $"{nombreBase}-{numeroSecuencial:D4}.pdf";

                string carpetaFacturas = GetFacturasFolderPath();
                if (!Directory.Exists(carpetaFacturas))
                    Directory.CreateDirectory(carpetaFacturas);

                string rutaArchivoFactura = Path.Combine(carpetaFacturas, nombreArchivo);

                // Calcular totales
                decimal totalKilometros = 0;
                decimal totalHoras = 0;

                foreach (DataGridViewRow fila in dgvServichio.SelectedRows)
                {
                    string kmStr = fila.Cells["Kilometros"].Value?.ToString() ?? "0";
                    string horasStr = fila.Cells["HorasEspera"].Value?.ToString() ?? "0";

                    decimal km = decimal.TryParse(kmStr, out decimal kmVal) ? kmVal : 0;
                    decimal horas = decimal.TryParse(horasStr, out decimal hrVal) ? hrVal : 0;

                    totalKilometros += km;
                    totalHoras += horas;
                }

                // Calcular total
                decimal precioPorKm = 0.49m;
                decimal precioPorHora = 9m;
                decimal subtotal = (totalKilometros * precioPorKm) + (totalHoras * precioPorHora);
                decimal iva = subtotal * 0.21m;
                decimal totalConIVA = subtotal + iva;

                // Crear objeto que solo contiene los campos a guardar en Firebase
                var facturaFirebase = new
                {
                    FechaEmision = DateTime.Now.ToString("o"),
                    Kilometros = (int)totalKilometros,
                    HorasEspera = (int)totalHoras,
                    Total = Math.Round(totalConIVA, 2)
                };

                // Guardar en Firebase usando como clave el nombre de la factura
                string firebaseKey = nombreArchivo.Replace(".pdf", "");
                await FirebaseHelper.GuardarFactura(firebaseKey, facturaFirebase);

                // ===== GENERAR PDF =====
                using (FileStream fs = new FileStream(rutaArchivoFactura, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 20, 20, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // === CABECERA ===
                    PdfPTable headerTable = new PdfPTable(1);
                    headerTable.WidthPercentage = 100;
                    headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfPCell headerCell = new PdfPCell();
                    headerCell.BackgroundColor = new BaseColor(30, 60, 114);
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.FixedHeight = 80f;
                    headerCell.Padding = 20f;

                    if (File.Exists("logo.png"))
                    {
                        Image logo = Image.GetInstance("logo.png");
                        logo.ScaleToFit(150f, 60f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        headerCell.AddElement(logo);
                    }
                    else
                    {
                        Paragraph logoText = new Paragraph("GESTO TAXI",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.WHITE));
                        logoText.Alignment = Element.ALIGN_CENTER;
                        headerCell.AddElement(logoText);
                    }

                    headerTable.AddCell(headerCell);
                    document.Add(headerTable);

                    // === TÍTULO ===
                    Paragraph titulo = new Paragraph("FACTURA DE SERVICIOS",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(30, 60, 114)));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingAfter = 25f;
                    document.Add(titulo);

                    // === INFORMACIÓN ===
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SpacingAfter = 20f;

                    AddStyledInfoCell(infoTable, "Taxista:", taxista);
                    AddStyledInfoCell(infoTable, "Fecha:", DateTime.Now.ToString("dd/MM/yyyy"));
                    AddStyledInfoCell(infoTable, "Código:", nombreArchivo.Replace(".pdf", ""));

                    document.Add(infoTable);

                    // === TABLA DE DATOS ===
                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 10f;
                    table.SpacingAfter = 15f;
                    table.DefaultCell.Padding = 8f;
                    table.DefaultCell.BorderWidth = 0.5f;
                    table.DefaultCell.BorderColor = new BaseColor(224, 224, 224);

                    string[] headers = { "Taxista", "Paciente", "Origen", "Destino", "Kilómetros", "Horas Espera", "Subtotal (€)" };
                    foreach (string header in headers)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(header,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE)));
                        cell.BackgroundColor = new BaseColor(30, 60, 114);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 8f;
                        cell.BorderWidth = 0;
                        table.AddCell(cell);
                    }

                    foreach (DataGridViewRow fila in dgvServichio.SelectedRows)
                    {
                        string paciente = fila.Cells["Paciente"].Value?.ToString() ?? "";
                        string origen = fila.Cells["LugarOrigen"].Value?.ToString() ?? "";
                        string destino = fila.Cells["LugarDestino"].Value?.ToString() ?? "";
                        string kmStr = fila.Cells["Kilometros"].Value?.ToString() ?? "0";
                        string horasStr = fila.Cells["HorasEspera"].Value?.ToString() ?? "0";

                        decimal km = decimal.TryParse(kmStr, out decimal kmVal) ? kmVal : 0;
                        decimal horas = decimal.TryParse(horasStr, out decimal hrVal) ? hrVal : 0;
                        decimal sub = (km * 0.49m) + (horas * 9.00m);

                        BaseColor rowColor = (table.Rows.Count % 2 == 1)
                            ? new BaseColor(248, 249, 250)
                            : BaseColor.WHITE;

                        AddStyledCell(table, taxista, rowColor);
                        AddStyledCell(table, paciente, rowColor);
                        AddStyledCell(table, origen, rowColor);
                        AddStyledCell(table, destino, rowColor);
                        AddStyledCell(table, km.ToString("F2"), rowColor, Element.ALIGN_RIGHT);
                        AddStyledCell(table, horas.ToString("F2"), rowColor, Element.ALIGN_RIGHT);
                        AddStyledCell(table, sub.ToString("F2") + " €", rowColor, Element.ALIGN_RIGHT);
                    }

                    document.Add(table);

                    // === TOTALES ===
                    PdfPTable footerTable = new PdfPTable(2);
                    footerTable.WidthPercentage = 50;
                    footerTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                    footerTable.SpacingBefore = 10f;
                    footerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                    AddStyledFooterCell(footerTable, "Total:", subtotal.ToString("F2") + " €");
                    AddStyledFooterCell(footerTable, "IVA (21%):", iva.ToString("F2") + " €");
                    AddStyledFooterCell(footerTable, "Total con IVA:", totalConIVA.ToString("F2") + " €");

                    document.Add(footerTable);
                    document.Close();
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = rutaArchivoFactura,
                    UseShellExecute = true
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddStyledInfoCell(PdfPTable table, string label, string value)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
            cellLabel.Border = Rectangle.NO_BORDER;
            table.AddCell(cellLabel);

            PdfPCell cellValue = new PdfPCell(new Phrase(value,
                FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            cellValue.Border = Rectangle.NO_BORDER;
            table.AddCell(cellValue);
        }

        private void AddStyledFooterCell(PdfPTable table, string label, string value)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(labelCell);

            PdfPCell valueCell = new PdfPCell(new Phrase(value,
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, new BaseColor(30, 60, 114))));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(valueCell);
        }

        private void AddStyledCell(PdfPTable table, string text, BaseColor backgroundColor, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA, 9)));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = alignment;
            cell.Padding = 6f;
            cell.BorderWidth = 0.5f;
            cell.BorderColor = new BaseColor(224, 224, 224);
            table.AddCell(cell);
        }

        private async Task<int> ObtenerNumeroSecuencialFactura(string nombreBase)
        {
            var todasLasFacturas = await FirebaseHelper.ObtenerTodasLasFacturas();
            var facturasMismoPrefijo = todasLasFacturas
                .Where(f => f.Key.StartsWith(nombreBase))
                .ToList();

            if (facturasMismoPrefijo.Count == 0)
                return 1;

            var numeros = facturasMismoPrefijo
                .Select(f =>
                {
                    string key = f.Key;
                    if (key.EndsWith(".pdf")) key = key.Substring(0, key.Length - 4);
                    string[] partes = key.Split('-');
                    return int.TryParse(partes.Last(), out int num) ? num : 0;
                })
                .Where(n => n > 0);

            int maxNum = numeros.DefaultIfEmpty(0).Max();
            return maxNum + 1;
        }

        private void dgvFacturas_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvFacturas.SelectedRows.Count > 0)
                {
                    var selectedRow = dgvFacturas.SelectedRows[0];
                    var facturaId = selectedRow.Cells["FacturaKey"].Value?.ToString();
                    if (!string.IsNullOrEmpty(facturaId))
                    {
                        MostrarPDFEnWebView(facturaId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar factura: {ex.Message}");
            }
        }

        private void InicializarLabelNoPDF()
        {
            lblNoPDF = new Label
            {
                Text = "No se encontró el archivo PDF.",
                ForeColor = System.Drawing.Color.Red,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Visible = false
            };
            lblNoPDF.Location = new System.Drawing.Point(
                (this.ClientSize.Width - lblNoPDF.Width) / 2,
                10
            );
            this.Controls.Add(lblNoPDF);
            this.Controls.SetChildIndex(lblNoPDF, 0);
        }

        private string GetFacturasFolderPath()
        {
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..");
            string fullPath = Path.GetFullPath(baseDir);
            return Path.Combine(fullPath, "Facturas");
        }

        private async void MostrarPDFEnWebView(string facturaId)
        {
            try
            {
                var folderPath = GetFacturasFolderPath();
                var pdfFilePath = Path.Combine(folderPath, $"{facturaId}.pdf");

                await webViewFacturas.EnsureCoreWebView2Async();
                await webViewFacturas.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML = '';");
                if (File.Exists(pdfFilePath))
                {
                    lblNoPDF.Visible = false;
                    var uri = new Uri($"file:///{pdfFilePath.Replace("\\", "/")}");
                    webViewFacturas.Source = new Uri("about:blank");
                    await Task.Delay(100);
                    webViewFacturas.Source = uri;
                }
                else
                {
                    lblNoPDF.Text = $"No se encontró el PDF para la factura: {facturaId}";
                    lblNoPDF.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblNoPDF.Text = "Error al cargar el PDF.";
                lblNoPDF.Visible = true;
                try
                {
                    await webViewFacturas.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML = '';");
                }
                catch
                {
                    // Ignorar errores si no está listo
                }
                Console.WriteLine($"Excepción: {ex.Message}");
            }
        }

        private void FiltrarFacturas(string filtro)
        {
            DataView dv = dtFacturas.DefaultView;
            if (!string.IsNullOrEmpty(filtro))
            {
                dv.RowFilter = $"FacturaKey LIKE '%{filtro}%'";
            }
            else
            {
                dv.RowFilter = "";
            }
            dgvFacturas.DataSource = dv.ToTable();
        }

        private void FiltrarServiciosPorTaxista(string filtro)
        {
            DataView dv = dtServicios.DefaultView;
            if (!string.IsNullOrEmpty(filtro))
            {
                dv.RowFilter = $"Taxista LIKE '%{filtro}%'";
            }
            else
            {
                dv.RowFilter = "";
            }
            dgvServichio.DataSource = dv.ToTable();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarFacturas(txtBuscarFactura.Text.Trim());
        }

        private void dgvFacturas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // TODO: Implementar acción al hacer clic en celda
        }

        private void webViewFacturas_Click(object sender, EventArgs e)
        {
            // TODO: Implementar acción al hacer clic en WebView2
        }

        private async void buttonRefrescar_Click(object sender, EventArgs e)
        {
            // Limpiar los textbox de búsqueda
            txtBuscarFactura.Text = string.Empty;
            txtBuscarServichio.Text = string.Empty;

            IniciarRotacion(); // Comenzar animación

            await CargarDatosAsync();

            DetenerRotacion(); // Detener animación
        }


        private void buttonRefrescar_Paint(object sender, PaintEventArgs e)
        {
            if (buttonRefrescar.Image == null) return;

            var g = e.Graphics;
            g.SmoothingMode = SD2D.SmoothingMode.AntiAlias;

            SD.Image originalImage = buttonRefrescar.Image;
            int w = originalImage.Width;
            int h = originalImage.Height;
            var center = new SD.Point(buttonRefrescar.Width / 2, buttonRefrescar.Height / 2);

            g.TranslateTransform(center.X, center.Y);
            if (isRotating)
                g.RotateTransform(rotationAngle);
            g.TranslateTransform(-w / 2, -h / 2);
            g.DrawImage(originalImage, 0, 0, w, h);
            g.ResetTransform();
        }



        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvFacturas.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar una factura para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = dgvFacturas.SelectedRows[0];
                string facturaKey = selectedRow.Cells["FacturaKey"].Value?.ToString();

                if (string.IsNullOrEmpty(facturaKey))
                {
                    MessageBox.Show("No se pudo obtener el identificador de la factura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"¿Está seguro de que desea eliminar la factura {facturaKey}?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Eliminar del DataGridView
                    var dataBoundItem = selectedRow.DataBoundItem as DataRowView;
                    if (dataBoundItem != null)
                    {
                        dtFacturas.Rows.Remove(dataBoundItem.Row);
                    }

                    // Eliminar del Firebase
                    await FirebaseHelper.EliminarFactura(facturaKey);

                    // Eliminar el archivo PDF asociado
                    string carpetaFacturas = GetFacturasFolderPath();
                    string rutaArchivoPdf = Path.Combine(carpetaFacturas, $"{facturaKey}.pdf");

                    if (File.Exists(rutaArchivoPdf))
                    {
                        File.Delete(rutaArchivoPdf);
                    }

                    // Limpiar WebView si muestra esta factura
                    if (webViewFacturas.Source != null && webViewFacturas.Source.ToString().Contains(facturaKey))
                    {
                        lblNoPDF.Visible = true;
                        lblNoPDF.Text = "El PDF ha sido eliminado.";
                        await webViewFacturas.CoreWebView2.ExecuteScriptAsync("document.body.innerHTML = '';");
                    }

                    MessageBox.Show("Factura y archivo asociado eliminados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscarServichio_TextChanged(object sender, EventArgs e)
        {
            FiltrarServiciosPorTaxista(txtBuscarServichio.Text.Trim());
        }
    }
}