namespace ProyectoFinalDAM
{
    partial class FormLugares
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
            this.lblEstado = new System.Windows.Forms.Label();
            this.progressBarCarga = new System.Windows.Forms.ProgressBar();
            this.webViewMapaLugares = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.buttonRefrescar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.dataGridViewLugares = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.webViewMapaLugares)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLugares)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(400, 17);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 16);
            this.lblEstado.TabIndex = 22;
            this.lblEstado.Visible = false;
            // 
            // progressBarCarga
            // 
            this.progressBarCarga.Location = new System.Drawing.Point(501, 37);
            this.progressBarCarga.Name = "progressBarCarga";
            this.progressBarCarga.Size = new System.Drawing.Size(167, 23);
            this.progressBarCarga.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarCarga.TabIndex = 21;
            this.progressBarCarga.Visible = false;
            // 
            // webViewMapaLugares
            // 
            this.webViewMapaLugares.AllowExternalDrop = true;
            this.webViewMapaLugares.CreationProperties = null;
            this.webViewMapaLugares.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewMapaLugares.Location = new System.Drawing.Point(33, 81);
            this.webViewMapaLugares.Name = "webViewMapaLugares";
            this.webViewMapaLugares.Size = new System.Drawing.Size(1129, 586);
            this.webViewMapaLugares.TabIndex = 28;
            this.webViewMapaLugares.ZoomFactor = 1D;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscar.Location = new System.Drawing.Point(1213, 81);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(280, 30);
            this.txtBuscar.TabIndex = 29;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // buttonRefrescar
            // 
            this.buttonRefrescar.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRefrescar.Image = global::ProyectoFinalDAM.Properties.Resources.refresss1;
            this.buttonRefrescar.Location = new System.Drawing.Point(1511, 81);
            this.buttonRefrescar.Name = "buttonRefrescar";
            this.buttonRefrescar.Size = new System.Drawing.Size(65, 62);
            this.buttonRefrescar.TabIndex = 27;
            this.buttonRefrescar.UseVisualStyleBackColor = false;
            this.buttonRefrescar.Click += new System.EventHandler(this.buttonRefrescar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = global::ProyectoFinalDAM.Properties.Resources.bin;
            this.btnEliminar.Location = new System.Drawing.Point(1511, 165);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(65, 54);
            this.btnEliminar.TabIndex = 26;
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // dataGridViewLugares
            // 
            this.dataGridViewLugares.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewLugares.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLugares.Location = new System.Drawing.Point(1213, 125);
            this.dataGridViewLugares.MultiSelect = false;
            this.dataGridViewLugares.Name = "dataGridViewLugares";
            this.dataGridViewLugares.ReadOnly = true;
            this.dataGridViewLugares.RowHeadersWidth = 51;
            this.dataGridViewLugares.RowTemplate.Height = 24;
            this.dataGridViewLugares.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewLugares.Size = new System.Drawing.Size(280, 542);
            this.dataGridViewLugares.TabIndex = 30;
            this.dataGridViewLugares.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewLugares_CellContentDoubleClick);
            this.dataGridViewLugares.SelectionChanged += new System.EventHandler(this.dataGridViewLugares_SelectionChanged);
            // 
            // FormLugares
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1602, 725);
            this.Controls.Add(this.dataGridViewLugares);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.webViewMapaLugares);
            this.Controls.Add(this.buttonRefrescar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.progressBarCarga);
            this.Name = "FormLugares";
            this.Text = "FormLugares";
            ((System.ComponentModel.ISupportInitialize)(this.webViewMapaLugares)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLugares)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ProgressBar progressBarCarga;
        private System.Windows.Forms.Button buttonRefrescar;
        private System.Windows.Forms.Button btnEliminar;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewMapaLugares;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.DataGridView dataGridViewLugares;
    }
}