namespace ProyectoFinalDAM
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelBotones = new System.Windows.Forms.Panel();
            this.tableLayoutBotones = new System.Windows.Forms.TableLayoutPanel();
            this.buttonFacturas = new System.Windows.Forms.Button();
            this.buttonLugares = new System.Windows.Forms.Button();
            this.buttonServicios = new System.Windows.Forms.Button();
            this.buttonPacientes = new System.Windows.Forms.Button();
            this.buttonTaxistas = new System.Windows.Forms.Button();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.tableLayoutMain.SuspendLayout();
            this.panelBotones.SuspendLayout();
            this.tableLayoutBotones.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 1;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Controls.Add(this.panelBotones, 0, 0);
            this.tableLayoutMain.Controls.Add(this.panelContenedor, 0, 1);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 2;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1192, 838);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // panelBotones
            // 
            this.panelBotones.BackColor = System.Drawing.Color.LightGray;
            this.panelBotones.Controls.Add(this.tableLayoutBotones);
            this.panelBotones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBotones.Location = new System.Drawing.Point(3, 3);
            this.panelBotones.Name = "panelBotones";
            this.panelBotones.Size = new System.Drawing.Size(1186, 93);
            this.panelBotones.TabIndex = 0;
            // 
            // tableLayoutBotones
            // 
            this.tableLayoutBotones.ColumnCount = 5;
            this.tableLayoutBotones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutBotones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutBotones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutBotones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutBotones.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutBotones.Controls.Add(this.buttonFacturas, 4, 0);
            this.tableLayoutBotones.Controls.Add(this.buttonLugares, 3, 0);
            this.tableLayoutBotones.Controls.Add(this.buttonServicios, 2, 0);
            this.tableLayoutBotones.Controls.Add(this.buttonPacientes, 1, 0);
            this.tableLayoutBotones.Controls.Add(this.buttonTaxistas, 0, 0);
            this.tableLayoutBotones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutBotones.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutBotones.Name = "tableLayoutBotones";
            this.tableLayoutBotones.RowCount = 1;
            this.tableLayoutBotones.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutBotones.Size = new System.Drawing.Size(1186, 93);
            this.tableLayoutBotones.TabIndex = 0;
            // 
            // buttonFacturas
            // 
            this.buttonFacturas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonFacturas.Image = global::ProyectoFinalDAM.Properties.Resources.Factura;
            this.buttonFacturas.Location = new System.Drawing.Point(951, 3);
            this.buttonFacturas.Name = "buttonFacturas";
            this.buttonFacturas.Size = new System.Drawing.Size(231, 90);
            this.buttonFacturas.TabIndex = 4;
            this.buttonFacturas.UseVisualStyleBackColor = true;
            this.buttonFacturas.Click += new System.EventHandler(this.buttonFacturas_Click);
            // 
            // buttonLugares
            // 
            this.buttonLugares.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonLugares.Image = global::ProyectoFinalDAM.Properties.Resources.Lugar;
            this.buttonLugares.Location = new System.Drawing.Point(714, 3);
            this.buttonLugares.Name = "buttonLugares";
            this.buttonLugares.Size = new System.Drawing.Size(231, 90);
            this.buttonLugares.TabIndex = 3;
            this.buttonLugares.UseVisualStyleBackColor = true;
            this.buttonLugares.Click += new System.EventHandler(this.buttonLugares_Click);
            // 
            // buttonServicios
            // 
            this.buttonServicios.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonServicios.Image = global::ProyectoFinalDAM.Properties.Resources.Servicio;
            this.buttonServicios.Location = new System.Drawing.Point(477, 3);
            this.buttonServicios.Name = "buttonServicios";
            this.buttonServicios.Size = new System.Drawing.Size(231, 90);
            this.buttonServicios.TabIndex = 2;
            this.buttonServicios.UseVisualStyleBackColor = true;
            this.buttonServicios.Click += new System.EventHandler(this.buttonServicios_Click);
            // 
            // buttonPacientes
            // 
            this.buttonPacientes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonPacientes.Image = global::ProyectoFinalDAM.Properties.Resources.Paciente;
            this.buttonPacientes.Location = new System.Drawing.Point(240, 3);
            this.buttonPacientes.Name = "buttonPacientes";
            this.buttonPacientes.Size = new System.Drawing.Size(231, 90);
            this.buttonPacientes.TabIndex = 1;
            this.buttonPacientes.UseVisualStyleBackColor = true;
            this.buttonPacientes.Click += new System.EventHandler(this.buttonPacientes_Click);
            // 
            // buttonTaxistas
            // 
            this.buttonTaxistas.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonTaxistas.Image = global::ProyectoFinalDAM.Properties.Resources.Taxista;
            this.buttonTaxistas.Location = new System.Drawing.Point(3, 3);
            this.buttonTaxistas.Name = "buttonTaxistas";
            this.buttonTaxistas.Size = new System.Drawing.Size(231, 90);
            this.buttonTaxistas.TabIndex = 0;
            this.buttonTaxistas.UseVisualStyleBackColor = true;
            this.buttonTaxistas.Click += new System.EventHandler(this.buttonTaxistas_Click);
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.Color.White;
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(3, 102);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1186, 733);
            this.panelContenedor.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 838);
            this.Controls.Add(this.tableLayoutMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GestoTaxi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutMain.ResumeLayout(false);
            this.panelBotones.ResumeLayout(false);
            this.tableLayoutBotones.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Panel panelBotones;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.TableLayoutPanel tableLayoutBotones;
        private System.Windows.Forms.Button buttonPacientes;
        private System.Windows.Forms.Button buttonTaxistas;
        private System.Windows.Forms.Button buttonFacturas;
        private System.Windows.Forms.Button buttonLugares;
        private System.Windows.Forms.Button buttonServicios;
    }
}

