using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoFinalDAM
{
    public partial class Form1 : Form
    {
        private Form currentForm = null;
        private string rol;
        private ToolTip toolTip;
        private Label labelAdvertencia;
        private Button buttonUsuario;

        public Form1(string rolUsuario)
        {
            InitializeComponent();
            rol = rolUsuario;
            ConfigureMainLayout();
            AddButtonsToLayout();
            ConfigureButtonStyles();
            AddAdvertenciaLabelIfNeeded();
        }

        private void ConfigureMainLayout()
        {
            tableLayoutMain.Dock = DockStyle.Fill;
            panelContenedor.Dock = DockStyle.Fill;

            tableLayoutBotones.Dock = DockStyle.Fill;
            tableLayoutBotones.RowCount = 1;
            tableLayoutBotones.ColumnCount = 6;
        }

        private void AddButtonsToLayout()
        {
            tableLayoutBotones.Controls.Add(buttonTaxistas, 0, 0);
            tableLayoutBotones.Controls.Add(buttonPacientes, 1, 0);
            tableLayoutBotones.Controls.Add(buttonServicios, 2, 0);
            tableLayoutBotones.Controls.Add(buttonLugares, 3, 0);
            tableLayoutBotones.Controls.Add(buttonFacturas, 4, 0);

            // Botón de usuario
            buttonUsuario = new Button
            {
                Name = "buttonUsuario",
                BackgroundImage = Properties.Resources.usuario,
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 3, 5, 3)
            };
            buttonUsuario.FlatAppearance.BorderSize = 0;
            buttonUsuario.Click += buttonUsuario_Click;

            toolTip = new ToolTip();
            toolTip.SetToolTip(buttonUsuario, "Cerrar sesión");

            tableLayoutBotones.Controls.Add(buttonUsuario, 5, 0);
        }

        private void ConfigureButtonStyles()
        {
            tableLayoutBotones.ColumnStyles.Clear();
            for (int i = 0; i < tableLayoutBotones.ColumnCount; i++)
            {
                tableLayoutBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / tableLayoutBotones.ColumnCount));
            }

            foreach (Button btn in tableLayoutBotones.Controls.OfType<Button>())
            {
                btn.Dock = DockStyle.Fill;
                btn.Padding = new Padding(5);
                btn.Margin = new Padding(5, 3, 5, 3);
                btn.BackColor = SystemColors.Control;
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;

                if (btn != buttonUsuario)
                {
                    switch (btn.Name)
                    {
                        case "buttonTaxistas":
                            toolTip.SetToolTip(btn, "Gestión de taxistas");
                            break;
                        case "buttonPacientes":
                            toolTip.SetToolTip(btn, "Gestión de pacientes");
                            break;
                        case "buttonServicios":
                            toolTip.SetToolTip(btn, "Gestión de servicios");
                            break;
                        case "buttonLugares":
                            toolTip.SetToolTip(btn, "Gestión de lugares");
                            break;
                        case "buttonFacturas":
                            toolTip.SetToolTip(btn, "Gestión de facturas");
                            break;
                    }
                }
            }
        }

        private void AddAdvertenciaLabelIfNeeded()
        {
            if (rol == "usuario")
            {
                labelAdvertencia = new Label
                {
                    Text = "!",
                    ForeColor = Color.Red,
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    AutoSize = true,
                    Cursor = Cursors.Hand
                };

                toolTip.SetToolTip(labelAdvertencia, "El usuario no tiene permisos para gestionar datos.");
                this.Controls.Add(labelAdvertencia);
                labelAdvertencia.BringToFront();

                // Posicionar inicialmente y actualizar en cada resize
                PositionAdvertenciaLabel();
                this.Resize += (s, e) => PositionAdvertenciaLabel();
            }
        }

        private void PositionAdvertenciaLabel()
        {
            if (labelAdvertencia != null)
            {
                int paddingRight = 20;  // Ajusta este valor según cuán a la derecha lo quieres
                labelAdvertencia.Location = new Point(this.ClientSize.Width - labelAdvertencia.Width - paddingRight, 10);
            }
        }


        private void OpenChildForm(Form childForm)
        {
            if (currentForm != null)
                currentForm.Close();

            currentForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelContenedor.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }

        private void HighlightButton(Button btn)
        {
            foreach (var control in tableLayoutBotones.Controls.OfType<Button>())
            {
                if (control != buttonUsuario)
                {
                    control.BackColor = SystemColors.Control;
                    control.Font = new Font(control.Font, FontStyle.Regular);
                }
            }

            btn.BackColor = Color.LightBlue;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
        }

        private void buttonTaxistas_Click(object sender, EventArgs e)
        {
            HighlightButton(buttonTaxistas);
            OpenChildForm(new FormTaxistas(rol));
        }

        private void buttonPacientes_Click(object sender, EventArgs e)
        {
            HighlightButton(buttonPacientes);
            OpenChildForm(new FormPacientes(rol));
        }

        private void buttonServicios_Click(object sender, EventArgs e)
        {
            HighlightButton(buttonServicios);
            OpenChildForm(new FormServicios(rol));
        }

        private void buttonLugares_Click(object sender, EventArgs e)
        {
            HighlightButton(buttonLugares);
            OpenChildForm(new FormLugares(rol));
        }

        private void buttonFacturas_Click(object sender, EventArgs e)
        {
            HighlightButton(buttonFacturas);
            OpenChildForm(new FormFacturas(rol));
        }

        private void buttonUsuario_Click(object sender, EventArgs e)
        {
            ShowTemporaryMessage("Cerrando sesión...", 1000);

            Timer timer = new Timer { Interval = 1200 };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Application.Restart();
            };
            timer.Start();
        }

        private async void ShowTemporaryMessage(string message, int durationMilliseconds)
        {
            Label tempMessage = new Label
            {
                Text = message,
                AutoSize = true,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Centrar en la parte superior del formulario
            tempMessage.Location = new Point((this.ClientSize.Width - tempMessage.PreferredWidth) / 2, 10);
            tempMessage.Anchor = AnchorStyles.Top;

            this.Controls.Add(tempMessage);
            tempMessage.BringToFront();

            await Task.Delay(durationMilliseconds);
            this.Controls.Remove(tempMessage);
        }
    }
}
