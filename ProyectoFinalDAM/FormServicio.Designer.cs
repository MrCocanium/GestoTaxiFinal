namespace ProyectoFinalDAM
{
    partial class FormServicios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvServicios = new System.Windows.Forms.DataGridView();
            this.lblEstado = new System.Windows.Forms.Label();
            this.progressBarCarga = new System.Windows.Forms.ProgressBar();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.webViewMapita = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.btnAñadir = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.buttonRefrescar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webViewMapita)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvServicios
            // 
            this.dgvServicios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServicios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServicios.Location = new System.Drawing.Point(75, 105);
            this.dgvServicios.MultiSelect = false;
            this.dgvServicios.Name = "dgvServicios";
            this.dgvServicios.ReadOnly = true;
            this.dgvServicios.RowHeadersWidth = 51;
            this.dgvServicios.RowTemplate.Height = 24;
            this.dgvServicios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServicios.Size = new System.Drawing.Size(885, 816);
            this.dgvServicios.TabIndex = 19;
            this.dgvServicios.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvServicios_CellContentDoubleClick);
            this.dgvServicios.SelectionChanged += new System.EventHandler(this.dgvServicios_SelectionChanged);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(442, 28);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 16);
            this.lblEstado.TabIndex = 15;
            this.lblEstado.Visible = false;
            // 
            // progressBarCarga
            // 
            this.progressBarCarga.Location = new System.Drawing.Point(543, 48);
            this.progressBarCarga.Name = "progressBarCarga";
            this.progressBarCarga.Size = new System.Drawing.Size(167, 23);
            this.progressBarCarga.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarCarga.TabIndex = 14;
            this.progressBarCarga.Visible = false;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscar.Location = new System.Drawing.Point(75, 41);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(280, 30);
            this.txtBuscar.TabIndex = 13;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // webViewMapita
            // 
            this.webViewMapita.AllowExternalDrop = true;
            this.webViewMapita.CreationProperties = null;
            this.webViewMapita.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewMapita.Location = new System.Drawing.Point(1172, 105);
            this.webViewMapita.Name = "webViewMapita";
            this.webViewMapita.Size = new System.Drawing.Size(704, 816);
            this.webViewMapita.TabIndex = 21;
            this.webViewMapita.ZoomFactor = 1D;
            // 
            // btnAñadir
            // 
            this.btnAñadir.Image = global::ProyectoFinalDAM.Properties.Resources.agregar;
            this.btnAñadir.Location = new System.Drawing.Point(1016, 310);
            this.btnAñadir.Name = "btnAñadir";
            this.btnAñadir.Size = new System.Drawing.Size(122, 96);
            this.btnAñadir.TabIndex = 16;
            this.btnAñadir.UseVisualStyleBackColor = true;
            this.btnAñadir.Click += new System.EventHandler(this.btnAñadir_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Image = global::ProyectoFinalDAM.Properties.Resources.editar;
            this.btnModificar.Location = new System.Drawing.Point(1016, 524);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(122, 96);
            this.btnModificar.TabIndex = 17;
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = global::ProyectoFinalDAM.Properties.Resources.eliminar;
            this.btnEliminar.Location = new System.Drawing.Point(1016, 767);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(122, 96);
            this.btnEliminar.TabIndex = 18;
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // buttonRefrescar
            // 
            this.buttonRefrescar.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRefrescar.Image = global::ProyectoFinalDAM.Properties.Resources.actualizar;
            this.buttonRefrescar.Location = new System.Drawing.Point(1016, 105);
            this.buttonRefrescar.Name = "buttonRefrescar";
            this.buttonRefrescar.Size = new System.Drawing.Size(122, 80);
            this.buttonRefrescar.TabIndex = 20;
            this.buttonRefrescar.UseVisualStyleBackColor = false;
            this.buttonRefrescar.Click += new System.EventHandler(this.buttonRefrescar_Click);
            // 
            // FormServicios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 953);
            this.Controls.Add(this.webViewMapita);
            this.Controls.Add(this.buttonRefrescar);
            this.Controls.Add(this.dgvServicios);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnAñadir);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.progressBarCarga);
            this.Controls.Add(this.txtBuscar);
            this.Name = "FormServicios";
            this.Text = "FormServicio";
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webViewMapita)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvServicios;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ProgressBar progressBarCarga;
        private System.Windows.Forms.TextBox txtBuscar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewMapita;
        private System.Windows.Forms.Button btnAñadir;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button buttonRefrescar;
    }
}