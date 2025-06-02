namespace ProyectoFinalDAM
{
    partial class FormFacturas
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
            this.txtBuscarFactura = new System.Windows.Forms.TextBox();
            this.dgvFacturas = new System.Windows.Forms.DataGridView();
            this.webViewFacturas = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.dgvServichio = new System.Windows.Forms.DataGridView();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.buttonRefrescar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.txtBuscarServichio = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webViewFacturas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServichio)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBuscarFactura
            // 
            this.txtBuscarFactura.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarFactura.Location = new System.Drawing.Point(12, 109);
            this.txtBuscarFactura.Name = "txtBuscarFactura";
            this.txtBuscarFactura.Size = new System.Drawing.Size(280, 30);
            this.txtBuscarFactura.TabIndex = 6;
            this.txtBuscarFactura.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFacturas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacturas.Location = new System.Drawing.Point(12, 148);
            this.dgvFacturas.MultiSelect = false;
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.ReadOnly = true;
            this.dgvFacturas.RowHeadersWidth = 51;
            this.dgvFacturas.RowTemplate.Height = 24;
            this.dgvFacturas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFacturas.Size = new System.Drawing.Size(280, 620);
            this.dgvFacturas.TabIndex = 12;
            this.dgvFacturas.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFacturas_CellContentClick);
            this.dgvFacturas.SelectionChanged += new System.EventHandler(this.dgvFacturas_SelectionChanged);
            // 
            // webViewFacturas
            // 
            this.webViewFacturas.AllowExternalDrop = true;
            this.webViewFacturas.CreationProperties = null;
            this.webViewFacturas.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewFacturas.Location = new System.Drawing.Point(414, 148);
            this.webViewFacturas.Name = "webViewFacturas";
            this.webViewFacturas.Size = new System.Drawing.Size(482, 620);
            this.webViewFacturas.TabIndex = 13;
            this.webViewFacturas.ZoomFactor = 1D;
            this.webViewFacturas.Click += new System.EventHandler(this.webViewFacturas_Click);
            // 
            // dgvServichio
            // 
            this.dgvServichio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServichio.Location = new System.Drawing.Point(984, 148);
            this.dgvServichio.Name = "dgvServichio";
            this.dgvServichio.ReadOnly = true;
            this.dgvServichio.RowHeadersWidth = 51;
            this.dgvServichio.RowTemplate.Height = 24;
            this.dgvServichio.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServichio.Size = new System.Drawing.Size(416, 620);
            this.dgvServichio.TabIndex = 14;
            // 
            // btnImprimir
            // 
            this.btnImprimir.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnImprimir.Image = global::ProyectoFinalDAM.Properties.Resources.imprimirPDF;
            this.btnImprimir.Location = new System.Drawing.Point(1464, 148);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(128, 123);
            this.btnImprimir.TabIndex = 30;
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // buttonRefrescar
            // 
            this.buttonRefrescar.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRefrescar.Image = global::ProyectoFinalDAM.Properties.Resources.refresss1;
            this.buttonRefrescar.Location = new System.Drawing.Point(322, 148);
            this.buttonRefrescar.Name = "buttonRefrescar";
            this.buttonRefrescar.Size = new System.Drawing.Size(65, 62);
            this.buttonRefrescar.TabIndex = 29;
            this.buttonRefrescar.UseVisualStyleBackColor = false;
            this.buttonRefrescar.Click += new System.EventHandler(this.buttonRefrescar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = global::ProyectoFinalDAM.Properties.Resources.bin;
            this.btnEliminar.Location = new System.Drawing.Point(322, 244);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(65, 54);
            this.btnEliminar.TabIndex = 28;
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // txtBuscarServichio
            // 
            this.txtBuscarServichio.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarServichio.Location = new System.Drawing.Point(984, 109);
            this.txtBuscarServichio.Name = "txtBuscarServichio";
            this.txtBuscarServichio.Size = new System.Drawing.Size(416, 30);
            this.txtBuscarServichio.TabIndex = 31;
            this.txtBuscarServichio.TextChanged += new System.EventHandler(this.txtBuscarServichio_TextChanged);
            // 
            // FormFacturas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1725, 792);
            this.Controls.Add(this.txtBuscarServichio);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.buttonRefrescar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.dgvServichio);
            this.Controls.Add(this.webViewFacturas);
            this.Controls.Add(this.dgvFacturas);
            this.Controls.Add(this.txtBuscarFactura);
            this.Name = "FormFacturas";
            this.Text = "FormFacturas";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webViewFacturas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServichio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBuscarFactura;
        private System.Windows.Forms.DataGridView dgvFacturas;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewFacturas;
        private System.Windows.Forms.DataGridView dgvServichio;
        private System.Windows.Forms.Button buttonRefrescar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.TextBox txtBuscarServichio;
    }
}