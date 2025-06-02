namespace ProyectoFinalDAM
{
    partial class FormTaxistas
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
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.dgvTaxistas = new System.Windows.Forms.DataGridView();
            this.buttonRefrescar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnAñadir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxistas)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(413, 36);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 16);
            this.lblEstado.TabIndex = 7;
            this.lblEstado.Visible = false;
            // 
            // progressBarCarga
            // 
            this.progressBarCarga.Location = new System.Drawing.Point(514, 56);
            this.progressBarCarga.Name = "progressBarCarga";
            this.progressBarCarga.Size = new System.Drawing.Size(167, 23);
            this.progressBarCarga.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarCarga.TabIndex = 6;
            this.progressBarCarga.Visible = false;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscar.Location = new System.Drawing.Point(46, 49);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(280, 30);
            this.txtBuscar.TabIndex = 5;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged_1);
            this.txtBuscar.Enter += new System.EventHandler(this.TxtBuscar_Enter);
            this.txtBuscar.Leave += new System.EventHandler(this.TxtBuscar_Leave);
            // 
            // dgvTaxistas
            // 
            this.dgvTaxistas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTaxistas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTaxistas.Location = new System.Drawing.Point(46, 141);
            this.dgvTaxistas.MultiSelect = false;
            this.dgvTaxistas.Name = "dgvTaxistas";
            this.dgvTaxistas.ReadOnly = true;
            this.dgvTaxistas.RowHeadersWidth = 51;
            this.dgvTaxistas.RowTemplate.Height = 24;
            this.dgvTaxistas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTaxistas.Size = new System.Drawing.Size(1264, 586);
            this.dgvTaxistas.TabIndex = 11;
            // 
            // buttonRefrescar
            // 
            this.buttonRefrescar.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRefrescar.Image = global::ProyectoFinalDAM.Properties.Resources.actualizar;
            this.buttonRefrescar.Location = new System.Drawing.Point(1051, 14);
            this.buttonRefrescar.Name = "buttonRefrescar";
            this.buttonRefrescar.Size = new System.Drawing.Size(127, 93);
            this.buttonRefrescar.TabIndex = 12;
            this.buttonRefrescar.UseVisualStyleBackColor = false;
            this.buttonRefrescar.Click += new System.EventHandler(this.buttonRefrescar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Image = global::ProyectoFinalDAM.Properties.Resources.eliminar;
            this.btnEliminar.Location = new System.Drawing.Point(1448, 542);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(122, 96);
            this.btnEliminar.TabIndex = 10;
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click_1);
            // 
            // btnModificar
            // 
            this.btnModificar.Image = global::ProyectoFinalDAM.Properties.Resources.editar;
            this.btnModificar.Location = new System.Drawing.Point(1448, 350);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(122, 96);
            this.btnModificar.TabIndex = 9;
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click_1);
            // 
            // btnAñadir
            // 
            this.btnAñadir.Image = global::ProyectoFinalDAM.Properties.Resources.agregar;
            this.btnAñadir.Location = new System.Drawing.Point(1448, 141);
            this.btnAñadir.Name = "btnAñadir";
            this.btnAñadir.Size = new System.Drawing.Size(122, 96);
            this.btnAñadir.TabIndex = 8;
            this.btnAñadir.UseVisualStyleBackColor = true;
            this.btnAñadir.Click += new System.EventHandler(this.btnAñadir_Click_1);
            // 
            // FormTaxistas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1725, 725);
            this.Controls.Add(this.buttonRefrescar);
            this.Controls.Add(this.dgvTaxistas);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnAñadir);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.progressBarCarga);
            this.Controls.Add(this.txtBuscar);
            this.Name = "FormTaxistas";
            this.Text = "Taxistas";
            this.Load += new System.EventHandler(this.FormTaxistas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaxistas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ProgressBar progressBarCarga;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.DataGridView dgvTaxistas;
        private System.Windows.Forms.Button btnAñadir;
        private System.Windows.Forms.Button buttonRefrescar;
    }
}