using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuevedoPlay
{
    public partial class FrmActividadesTuristicas : Form
    {
        private const int TOP_MARGIN = 95;
        private const int SIDE_MARGIN = 45;
        private const int GAP = 35;

        public FrmActividadesTuristicas()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Load += FrmActividadesTuristicas_Load;
            ResizeEnd += (s, e) => ApplyLayout();
            Shown += (s, e) =>
            {
                CenterToScreen();   // fuerza centrado real al mostrar
                ApplyLayout();
            };
        }



        private void FrmActividadesTuristicas_Load(object sender, EventArgs e)
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(1100, 650);
            this.ClientSize = new Size(1100, 650); // tamaño inicial estable





            // Tamaño fijo recomendado para tarjetas (se ve pro)
            panel1.Size = new Size(310, 430);
            panel2.Size = new Size(310, 430);
            panel3.Size = new Size(310, 430);


            // Estilizar cards
            EstilizarPanel(panel1, label3, label6, ptBoxTalleres, button1);
            EstilizarPanel(panel2, label4, label7, ptBoxCaminatas, button2);
            EstilizarPanel(panel3, label5, label8, ptBoxVisitasGuiadas, button3);

            // Botón regresar (gris pro)
            btnRegresar.Size = new Size(170, 44);
            btnRegresar.BackColor = Color.FromArgb(230, 230, 230);
            btnRegresar.ForeColor = Color.Black;
            btnRegresar.FlatStyle = FlatStyle.Flat;
            btnRegresar.FlatAppearance.BorderSize = 0;

            // Botón “Juego Interactivo” (asumo button4 = verde)
            if (button4 != null)
            {
                button4.Height = 44;
                button4.Width = 260;
                button4.BackColor = Color.FromArgb(22, 160, 80);
                button4.ForeColor = Color.White;
                button4.FlatStyle = FlatStyle.Flat;
                button4.FlatAppearance.BorderSize = 0;
                button4.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                button4.Width = 240;
                button4.Height = 44;
                Redondear(button4, 22);

            }
            


            ApplyLayout();
            this.PerformLayout();
            this.Refresh();
        }

        // =========================
        // LAYOUT GLOBAL (centrado + zona inferior)
        // =========================

        private void ApplyLayout()
        {
            if (!IsHandleCreated) return;

            SuspendLayout();

            // 0) Seguridad: si la ventana es muy pequeña, no intentes “meter todo”
            int minW = SIDE_MARGIN * 2 + panel1.Width + panel2.Width + panel3.Width + GAP * 2;
            int minH = TOP_MARGIN + panel1.Height + 120; // espacio para barra inferior
            if (ClientSize.Width < minW || ClientSize.Height < minH)
            {
                // centra lo que puedas sin romperte
                panel1.Top = TOP_MARGIN;
                panel2.Top = TOP_MARGIN;
                panel3.Top = TOP_MARGIN;
            }

            // 1) Centrar cards
            int cardsTotalWidth = panel1.Width + panel2.Width + panel3.Width + GAP * 2;
            int startX = Math.Max(SIDE_MARGIN, (ClientSize.Width - cardsTotalWidth) / 2);

            int yCardsTop = TOP_MARGIN;

            panel1.Left = startX;
            panel2.Left = panel1.Right + GAP;
            panel3.Left = panel2.Right + GAP;

            panel1.Top = yCardsTop;
            panel2.Top = yCardsTop;
            panel3.Top = yCardsTop;

            // 2) Barra inferior fija (solo calcula por botones)
            int bottomPadding = 22;
            int barHeight = Math.Max(btnRegresar.Height, button4?.Height ?? 0) + (bottomPadding * 2);

            // 3) Evitar choque con barra inferior
            int maxCardsBottom = ClientSize.Height - barHeight - 10;
            int cardsBottom = panel1.Bottom;

            if (cardsBottom > maxCardsBottom)
            {
                int delta = cardsBottom - maxCardsBottom;
                panel1.Top -= delta;
                panel2.Top -= delta;
                panel3.Top -= delta;
            }

            // 4) Botones inferiores
            btnRegresar.Left = SIDE_MARGIN;
            btnRegresar.Top = ClientSize.Height - bottomPadding - btnRegresar.Height;

            if (button4 != null)
            {
                button4.Left = ClientSize.Width - SIDE_MARGIN - button4.Width;
                button4.Top = ClientSize.Height - bottomPadding - button4.Height;
                button4.BringToFront();
            }

            // 5) Re-layout interno de cards EN CADA ApplyLayout (clave 🔥)
            EstilizarPanel(panel1, label3, label6, ptBoxTalleres, button1);
            EstilizarPanel(panel2, label4, label7, ptBoxCaminatas, button2);
            EstilizarPanel(panel3, label5, label8, ptBoxVisitasGuiadas, button3);

            // 6) Redondeos
            RedondearPanel(panel1, 28);
            RedondearPanel(panel2, 28);
            RedondearPanel(panel3, 28);

            Redondear(btnRegresar, 22);
            if (button4 != null) Redondear(button4, 22);

            Redondear(button1, 22);
            Redondear(button2, 22);
            Redondear(button3, 22);

            ResumeLayout(true);

            // Botón X fijo en esquina superior derecha
            int xPad = 18;
            int yPad = 18;

            btnSalirApp.Top = yPad;
            btnSalirApp.Left = ClientSize.Width - xPad - btnSalirApp.Width;
            btnSalirApp.BringToFront();

        }

        // =========================
        // CARD STYLING
        // =========================
        private void EstilizarPanel(
            Panel panel,
            Label titulo,
            Label texto,
            PictureBox img,
            Button btn)
        {
            panel.BackColor = Color.White;
            panel.Padding = new Padding(30);

            int contentWidth = panel.Width - panel.Padding.Left - panel.Padding.Right;
            int y = panel.Padding.Top;

            // Imagen
            img.SizeMode = PictureBoxSizeMode.Zoom;
            img.Width = 230;
            img.Height = 160; // más ancha tipo banner
            img.Left = panel.Padding.Left + (contentWidth - img.Width) / 2;
            img.Top = y;
            y = img.Bottom + 16;


            // Título
            titulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titulo.ForeColor = Color.Black;
            titulo.TextAlign = ContentAlignment.MiddleCenter;
            titulo.AutoSize = false;
            titulo.Width = contentWidth;
            titulo.Height = 34;
            titulo.Left = panel.Padding.Left;
            titulo.Top = y;
            y = titulo.Bottom + 10;

            // Texto (más alto para que no se corte)
            texto.Font = new Font("Segoe UI", 11F);
            texto.ForeColor = Color.DimGray;
            texto.TextAlign = ContentAlignment.TopCenter;
            texto.AutoSize = false;
            texto.Width = contentWidth;
            texto.Height = 80;
            texto.Left = panel.Padding.Left;
            texto.Top = y;

            // Botón
            btn.Width = 170;
            btn.Height = 44;
            btn.Left = panel.Padding.Left + (contentWidth - btn.Width) / 2;
            btn.Top = panel.Height - btn.Height - 30;
            btn.BackColor = Color.FromArgb(52, 120, 198);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);

            Redondear(btn, 22);
        }

        // =========================
        // REDONDEO
        // =========================
        private void Redondear(Control control, int radio)
        {
            if (control == null) return;
            if (control.Width <= radio || control.Height <= radio) return;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(0, 0, radio, radio, 180, 90);
                path.AddArc(control.Width - radio, 0, radio, radio, 270, 90);
                path.AddArc(control.Width - radio, control.Height - radio, radio, radio, 0, 90);
                path.AddArc(0, control.Height - radio, radio, radio, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }

        private void RedondearPanel(Panel panel, int radio)
        {
            if (panel == null) return;
            if (panel.Width <= radio || panel.Height <= radio) return;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(0, 0, radio, radio, 180, 90);
                path.AddArc(panel.Width - radio, 0, radio, radio, 270, 90);
                path.AddArc(panel.Width - radio, panel.Height - radio, radio, radio, 0, 90);
                path.AddArc(0, panel.Height - radio, radio, radio, 90, 90);
                path.CloseFigure();
                panel.Region = new Region(path);
            }
        }

        // =========================
        // EVENTOS QUE YA TENÍAS
        // =========================
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            FrmOpciones opciones = new FrmOpciones();
            opciones.Show();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmActividadDetalle talleres = new FrmActividadDetalle();
            talleres.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new FrmActividadDetalle(ActividadBuilders.BuildCaminatas()).Show();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new FrmActividadDetalle(ActividadBuilders.BuildVisitas()).Show();
            Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClasificaActividadForm frm = new ClasificaActividadForm();
            frm.Show();
            Hide();
        }

        private void btnSalirApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
