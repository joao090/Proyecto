using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuevedoPlay
{
    public partial class ClasificaActividadForm : Form
    {
        // ====== Tema / Colores (sobrios, tipo app real) ======
        private readonly Color colorFondo = Color.FromArgb(245, 248, 252);
        private readonly Color colorCard = Color.White;
        private readonly Color colorCardHover = Color.FromArgb(242, 248, 255);
        private readonly Color colorBorde = Color.FromArgb(220, 230, 240);

        private readonly Color colorCorrecto = Color.FromArgb(190, 240, 200);
        private readonly Color colorIncorrecto = Color.FromArgb(255, 200, 200);

        private readonly Color colorAccent = Color.FromArgb(0, 120, 215);
        private readonly Color colorAccent2 = Color.FromArgb(22, 160, 80);
        private readonly Color colorGrisBoton = Color.FromArgb(120, 120, 120);

        private readonly Font fontTitle = new Font("Segoe UI", 16, FontStyle.Bold);
        private readonly Font fontStats = new Font("Segoe UI", 11, FontStyle.Bold);
        private readonly Font fontCard = new Font("Segoe UI", 11, FontStyle.Bold);
        private readonly Font fontBody = new Font("Segoe UI", 10, FontStyle.Regular);

        // ====== Estado del juego ======
        private readonly Random rnd = new Random();
        private int totalTarjetas = 0;
        private int tarjetasCorrectas = 0;
        private int puntaje = 0;
        private int errores = 0;

        private int tiempoRestante = 30;
        private const int bonusPorSegundo = 2;
        private int nivelActual = 1;
        private const int maxErrores = 5;
        private bool juegoTerminado = false;

        // Record
        private int puntajeMaximo = 0;
        private readonly string rutaArchivoRecord = "record.txt";

        // ====== UI principal ======
        private Panel pnlHeader;
        private Panel pnlMain;
        private Panel pnlLeft;
        private Panel pnlRight;
        private Panel pnlRightContent;  // <- IMPORTANTE (contenedor interno para scroll)
        private Panel pnlFooter;

        private Panel panelTarjetas;
        private Panel panelNaturaleza;
        private Panel panelCultura;
        private Panel panelGastronomia;

        private Label lblTitulo;
        private Label lblNivel;
        private Label lblPuntaje;
        private Label lblErrores;
        private Label lblTiempo;

        private ProgressBar barraProgreso;
        private Timer timerNivel;

        private Button btnRegresar;

        // Panel final
        private Panel panelFinal;
        private Label lblMensajeFinal;
        private Label lblPuntajeFinal;
        private Button btnFinalizar;
        private Button btnReintentar;


        public ClasificaActividadForm()
        {
            InitializeComponent();
            BuildUI();
            ResetAndStartGame();
        }

        // =========================================================
        // UI: construir una vez, luego solo actualizamos contenido
        // =========================================================

        private void BuildUI()
        {
            // =========================
            // Ventana principal
            // =========================
            Text = "Clasifica la Actividad";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ClientSize = new Size(1100, 700);
            BackColor = colorFondo;
            DoubleBuffered = true;

            Shown += (s, e) => ApplyLayout();
            ResizeEnd += (s, e) => ApplyLayout();

            // =========================
            // HEADER
            // =========================
            pnlHeader = new Panel
            {
                Height = 78,
                Dock = DockStyle.Top,
                BackColor = Color.White
            };
            pnlHeader.Paint += (s, e) => DrawBottomBorder(e.Graphics, pnlHeader);

            lblTitulo = new Label
            {
                Text = "Clasifica la Actividad",
                Font = fontTitle,
                ForeColor = Color.FromArgb(30, 50, 80),
                AutoSize = true,
                Location = new Point(22, 22)
            };

            lblPuntaje = MakeStatLabel("Puntaje: 0");
            lblErrores = MakeStatLabel("Errores: 0/5");
            lblTiempo = MakeStatLabel("Tiempo: 30s");
            lblNivel = MakeStatLabel("Nivel: 1");

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblPuntaje);
            pnlHeader.Controls.Add(lblErrores);
            pnlHeader.Controls.Add(lblTiempo);
            pnlHeader.Controls.Add(lblNivel);
            Controls.Add(pnlHeader);

            // =========================
            // FOOTER
            // =========================
            pnlFooter = new Panel
            {
                Height = 70,
                Dock = DockStyle.Bottom,
                BackColor = colorFondo
            };
            pnlFooter.Paint += (s, e) => DrawTopBorder(e.Graphics, pnlFooter);
            Controls.Add(pnlFooter);

            btnRegresar = MakeButton("Regresar", Color.Red, Color.White);
            btnRegresar.Size = new Size(150, 44);
            btnRegresar.Click += (s, e) =>
            {
                var f = new FrmActividadesTuristicas();
                f.Show();
                Hide();
            };
            pnlFooter.Controls.Add(btnRegresar);

            // =========================
            // MAIN
            // =========================
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                BackColor = colorFondo
            };
            Controls.Add(pnlMain);

            // =========================
            // COLUMNAS
            // =========================
            pnlLeft = new Panel { BackColor = Color.Transparent };

            pnlRight = new Panel
            {
                BackColor = Color.Transparent,
                AutoScroll = true // ✅ el scroll va aquí
            };

            pnlMain.Controls.Add(pnlLeft);
            pnlMain.Controls.Add(pnlRight);

            // =========================
            // CONTENIDO DERECHO (CLAVE)
            // =========================
            pnlRightContent = new Panel
            {
                BackColor = Color.Transparent,
                AutoSize = true,                 // ✅ ahora crece con el contenido
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(0, 0)
            };

            pnlRight.Controls.Add(pnlRightContent);



            // =========================
            // IZQUIERDA: TARJETAS
            // =========================
            barraProgreso = new ProgressBar
            {
                Style = ProgressBarStyle.Continuous,
                Height = 22
            };

            panelTarjetas = new Panel
            {
                BackColor = Color.White,
                AutoScroll = true
            };
            panelTarjetas.Paint += (s, e) => DrawCardBorder(e.Graphics, panelTarjetas, 18);

            pnlLeft.Controls.Add(barraProgreso);
            pnlLeft.Controls.Add(panelTarjetas);

            // =========================
            // DERECHA: CONTENEDORES
            // =========================
            panelNaturaleza = CrearContenedor("Naturaleza", Color.FromArgb(46, 204, 113));
            panelCultura = CrearContenedor("Cultura", Color.FromArgb(52, 152, 219));
            panelGastronomia = CrearContenedor("Gastronomía", Color.FromArgb(243, 156, 18));

            pnlRightContent.Controls.Add(panelNaturaleza);
            pnlRightContent.Controls.Add(panelCultura);
            pnlRightContent.Controls.Add(panelGastronomia);

            // =========================
            // PANEL FINAL (OVERLAY)
            // =========================
            panelFinal = new Panel
            {
                Size = new Size(520, 320),
                BackColor = Color.White,
                Visible = false
            };
            panelFinal.Paint += (s, e) => DrawCardBorder(e.Graphics, panelFinal, 18);

            lblMensajeFinal = new Label
            {
                Text = "🎉 Nivel completado",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 90
            };

            lblPuntajeFinal = new Label
            {
                Text = "Puntaje: 0",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 160, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70
            };

            btnReintentar = MakeButton("Reintentar", colorGrisBoton, Color.White);
            btnFinalizar = MakeButton("Volver", colorAccent, Color.White);

            btnReintentar.Size = new Size(160, 46);
            btnFinalizar.Size = new Size(160, 46);

            btnReintentar.Click += (s, e) => ReiniciarJuegoCompleto();
            btnFinalizar.Click += (s, e) =>
            {
                GuardarRecordSiAplica();
                var f = new FrmActividadesTuristicas();
                f.Show();
                Dispose();
            };

            panelFinal.Controls.Add(btnFinalizar);
            panelFinal.Controls.Add(btnReintentar);
            panelFinal.Controls.Add(lblPuntajeFinal);
            panelFinal.Controls.Add(lblMensajeFinal);
            Controls.Add(panelFinal);
            panelFinal.BringToFront();

            // =========================
            // TIMER
            // =========================
            timerNivel = new Timer { Interval = 1000 };
            timerNivel.Tick += TimerNivel_Tick;
        }


        private void ApplyLayout()
        {
            // =========================
            // Header stats (derecha)
            // =========================
            int rightPad = 22;
            int yStats = 28;

            lblNivel.Location = new Point(ClientSize.Width - rightPad - lblNivel.PreferredWidth, yStats);
            lblTiempo.Location = new Point(lblNivel.Left - 22 - lblTiempo.PreferredWidth, yStats);
            lblErrores.Location = new Point(lblTiempo.Left - 22 - lblErrores.PreferredWidth, yStats);
            lblPuntaje.Location = new Point(lblErrores.Left - 22 - lblPuntaje.PreferredWidth, yStats);

            // =========================
            // Main 2 columnas
            // =========================
            int gap = 18;
            int leftW = 360;

            int mainW = pnlMain.ClientSize.Width;
            int mainH = pnlMain.ClientSize.Height;

            pnlLeft.SetBounds(0, 0, leftW, mainH);
            pnlRight.SetBounds(leftW + gap, 0, mainW - leftW - gap, mainH);

            // =========================
            // Left: progreso + tarjetas
            // =========================
            barraProgreso.SetBounds(0, 0, pnlLeft.Width, 22);

            int topPadLeft = 12;
            int yCards = barraProgreso.Bottom + topPadLeft;
            panelTarjetas.SetBounds(0, yCards, pnlLeft.Width, pnlLeft.Height - yCards);

            OrdenarTarjetasEnPanel(panelTarjetas);

            // =========================
            // Right: contenedores PEQUEÑOS + centrados
            // =========================
            int topPadRight = 12;
            int bottomPadRight = 12;
            int contGap = 16;

            // ancho real visible del panel derecho (sin inventos raros)
            int containerW = pnlRight.ClientSize.Width;
            if (containerW < 200) containerW = pnlRight.Width;

            // ✅ tamaño fijo (ajústalo aquí)
            int containerH = 170;   // prueba 150, 160, 170

            int groupH = containerH * 3 + contGap * 2;
            int availableH = pnlRight.ClientSize.Height - topPadRight - bottomPadRight;

            // centrado vertical solo si CABE
            int startY = topPadRight;
            if (availableH > groupH)
                startY = topPadRight + (availableH - groupH) / 2;

            // pnlRightContent se adapta al contenido (AutoSize=true)
            pnlRightContent.Width = containerW;

            panelNaturaleza.SetBounds(0, startY, containerW, containerH);
            panelCultura.SetBounds(0, panelNaturaleza.Bottom + contGap, containerW, containerH);
            panelGastronomia.SetBounds(0, panelCultura.Bottom + contGap, containerW, containerH);

            // =========================
            // Footer botón
            // =========================
            btnRegresar.Location = new Point(18, (pnlFooter.Height - btnRegresar.Height) / 2);

            // =========================
            // Panel final centrado
            // =========================
            panelFinal.Location = new Point(
                (ClientSize.Width - panelFinal.Width) / 2,
                (ClientSize.Height - panelFinal.Height) / 2
            );

            btnReintentar.Location = new Point(90, 240);
            btnFinalizar.Location = new Point(270, 240);
        }


        // =========================================================
        // Juego
        // =========================================================
        private void ResetAndStartGame()
        {
            juegoTerminado = false;
            errores = 0;
            puntaje = 0;
            tarjetasCorrectas = 0;
            totalTarjetas = 0;

            tiempoRestante = (nivelActual == 1) ? 60 : 45;

            CargarRecord();
            UpdateStats();

            // limpiar tarjetas y contenedores (pero no reconstruir UI)
            panelTarjetas.Controls.Clear();
            LimpiarContenedor(panelNaturaleza);
            LimpiarContenedor(panelCultura);
            LimpiarContenedor(panelGastronomia);

            // tarjetas por nivel (misma idea)
            List<(string Texto, string Categoria)> tarjetasNivel =
                (nivelActual == 1)
                ? new List<(string, string)>
                {
                    ("Caminata guiada", "Naturaleza"),
                    ("Visita cultural", "Cultura"),
                    ("Plato típico", "Gastronomía")
                }
                : new List<(string, string)>
                {
                    ("Senderismo", "Naturaleza"),
                    ("Festival local", "Cultura"),
                    ("Mercado gastronómico", "Gastronomía"),
                    ("Ruta ecológica", "Naturaleza")
                };

            tarjetasNivel = MezclarLista(tarjetasNivel);

            foreach (var t in tarjetasNivel)
                CrearTarjeta(t.Texto, t.Categoria);

            // progreso
            barraProgreso.Minimum = 0;
            barraProgreso.Maximum = totalTarjetas;
            barraProgreso.Value = 0;

            panelFinal.Visible = false;
            panelTarjetas.Enabled = true;
            panelNaturaleza.Enabled = true;
            panelCultura.Enabled = true;
            panelGastronomia.Enabled = true;

            timerNivel.Stop();
            timerNivel.Start();

            ApplyLayout();
        }

        private void ReiniciarJuegoCompleto()
        {
            timerNivel.Stop();
            nivelActual = 1;
            ResetAndStartGame();
        }

        private void TimerNivel_Tick(object sender, EventArgs e)
        {
            if (juegoTerminado) return;

            tiempoRestante--;
            lblTiempo.Text = $"Tiempo: {tiempoRestante}s";

            if (tiempoRestante <= 0)
            {
                timerNivel.Stop();
                juegoTerminado = true;

                panelTarjetas.Enabled = false;
                panelNaturaleza.Enabled = false;
                panelCultura.Enabled = false;
                panelGastronomia.Enabled = false;

                lblMensajeFinal.Text = $"⏰ Tiempo agotado (Nivel {nivelActual})";
                lblPuntajeFinal.Text = $"Puntaje final: {puntaje}\nRécord: {puntajeMaximo}";
                MostrarPanelFinal();
            }
        }

        private void VerificarFinDelJuego()
        {
            if (juegoTerminado) return;

            if (tarjetasCorrectas == totalTarjetas)
            {
                timerNivel.Stop();

                int bonusTiempo = tiempoRestante * bonusPorSegundo;
                puntaje += bonusTiempo;
                GuardarRecordSiAplica();
                UpdateStats();

                if (nivelActual == 1)
                {
                    nivelActual = 2;
                    MessageBox.Show("🎮 ¡Nivel 1 completado!\nPasas al Nivel 2", "Siguiente nivel",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetAndStartGame();
                }
                else
                {
                    juegoTerminado = true;
                    lblMensajeFinal.Text = $"🎉 ¡Nivel {nivelActual} completado!";
                    lblPuntajeFinal.Text = $"Puntaje final: {puntaje}\nRécord: {puntajeMaximo}";
                    MostrarPanelFinal();
                }
            }
        }

        private void MostrarPanelFinal()
        {
            panelFinal.Visible = true;
            panelFinal.BringToFront();
            ApplyLayout();
        }

        // =========================================================
        // Crear UI del juego (tarjetas/contendores)
        // =========================================================
        private Panel CrearContenedor(string categoria, Color accent)
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                AllowDrop = true,
                Tag = categoria,
                Padding = new Padding(14)
            };

            panel.Paint += (s, e) => DrawCardBorder(e.Graphics, panel, 18);

            // Badge color (barra izquierda)
            var badge = new Panel
            {
                BackColor = accent,
                Width = 10,
                Dock = DockStyle.Left
            };
            panel.Controls.Add(badge);

            // Título
            var titulo = new Label
            {
                Text = categoria,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 80),
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = null
            };
            panel.Controls.Add(titulo);

            // Hint
            var hint = new Label
            {
                Text = "Arrastra aquí las tarjetas correctas",
                Font = fontBody,
                ForeColor = Color.DimGray,
                Dock = DockStyle.Top,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = null
            };
            panel.Controls.Add(hint);

            panel.DragEnter += Panel_DragEnter;
            panel.DragDrop += Panel_DragDrop;

            return panel;
        }

        private void LimpiarContenedor(Panel cont)
        {
            // mantiene badge/titulo/hint (Tag==null)
            var keep = cont.Controls.Cast<Control>().Where(c => c is Label l && l.Tag == null).ToList();
            var badge = cont.Controls.Cast<Control>().FirstOrDefault(c => c is Panel && c.Dock == DockStyle.Left);

            cont.Controls.Clear();

            if (badge != null) cont.Controls.Add(badge);
            foreach (var c in keep) cont.Controls.Add(c);
        }

        private void CrearTarjeta(string texto, string categoria)
        {
            var tarjeta = new Label
            {
                Text = texto,
                Tag = categoria,
                Size = new Size(290, 70),
                BackColor = colorCard,
                ForeColor = Color.FromArgb(25, 25, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = fontCard,
                Cursor = Cursors.Hand,
                Padding = new Padding(6)
            };

            tarjeta.Paint += (s, e) => DrawCardBorder(e.Graphics, tarjeta, 14);

            tarjeta.MouseEnter += (s, e) => tarjeta.BackColor = colorCardHover;
            tarjeta.MouseLeave += (s, e) =>
            {
                if (tarjeta.Enabled) tarjeta.BackColor = colorCard;
            };

            tarjeta.MouseDown += (s, e) =>
            {
                if (!tarjeta.Enabled) return;
                tarjeta.DoDragDrop(tarjeta, DragDropEffects.Move);
            };

            panelTarjetas.Controls.Add(tarjeta);
            totalTarjetas++;

            OrdenarTarjetasEnPanel(panelTarjetas);
        }


        private void OrdenarTarjetasEnPanel(Panel panel)
        {
            int espacio = 12;
            var tarjetas = panel.Controls.OfType<Label>().Where(l => l.Tag != null).ToList();
            if (tarjetas.Count == 0) return;

            // altura total
            int totalH = tarjetas.Sum(t => t.Height) + (espacio * (tarjetas.Count - 1));

            // si sobra espacio, centro vertical
            int y = 16;
            if (panel.ClientSize.Height > totalH + 32)
                y = (panel.ClientSize.Height - totalH) / 2;

            foreach (var lbl in tarjetas)
            {
                lbl.Left = (panel.ClientSize.Width - lbl.Width) / 2;
                lbl.Top = y;
                y += lbl.Height + espacio;
            }
        }


        // =========================================================
        // Drag & Drop
        // =========================================================
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Label)))
                e.Effect = DragDropEffects.Move;

        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (juegoTerminado) return;

            var destino = sender as Panel;
            var tarjeta = e.Data.GetData(typeof(Label)) as Label;
            if (tarjeta == null || destino == null) return;

            string categoriaCorrecta = tarjeta.Tag.ToString();
            string categoriaDestino = destino.Tag.ToString();

            bool correcto = (categoriaCorrecta == categoriaDestino);

            if (correcto)
            {
                puntaje += 10;
                tarjetasCorrectas++;
                UpdateStats();

                tarjeta.BackColor = colorCorrecto;
                tarjeta.Enabled = false;

                panelTarjetas.Controls.Remove(tarjeta);
                destino.Controls.Add(tarjeta);

                OrdenarTarjetasEnPanel(destino);
                OrdenarTarjetasEnPanel(panelTarjetas);

                barraProgreso.Value = Math.Min(tarjetasCorrectas, barraProgreso.Maximum);

                VerificarFinDelJuego();
            }
            else
            {
                errores++;
                puntaje = Math.Max(0, puntaje - 5);
                UpdateStats();

                tarjeta.BackColor = colorIncorrecto;
                var t = new Timer { Interval = 350 };
                t.Tick += (s, ev) =>
                {
                    t.Stop();
                    if (tarjeta.Enabled) tarjeta.BackColor = colorCard;
                };
                t.Start();

                if (errores >= maxErrores)
                {
                    juegoTerminado = true;
                    timerNivel.Stop();

                    panelTarjetas.Enabled = false;
                    panelNaturaleza.Enabled = false;
                    panelCultura.Enabled = false;
                    panelGastronomia.Enabled = false;

                    lblMensajeFinal.Text = $"❌ Nivel {nivelActual} finalizado: demasiados errores";
                    lblPuntajeFinal.Text = $"Puntaje final: 0\nRécord: {puntajeMaximo}";
                    MostrarPanelFinal();
                }
            }

            _ = MostrarFeedbackPanel(destino, correcto);
            OrdenarTarjetasEnPanel(destino);

        }

        private async Task MostrarFeedbackPanel(Panel panel, bool correcto)
        {
            Color original = panel.BackColor;
            panel.BackColor = correcto ? Color.FromArgb(235, 250, 240) : Color.FromArgb(255, 235, 235);
            await Task.Delay(220);
            if (!juegoTerminado) panel.BackColor = original;
        }

        // =========================================================
        // Helpers: stats / record / estilos
        // =========================================================
        private Label MakeStatLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = fontStats,
                ForeColor = Color.FromArgb(45, 45, 45),
                AutoSize = true
            };
        }

        private Button MakeButton(string text, Color back, Color fore)
        {
            var b = new Button
            {
                Text = text,
                BackColor = back,
                ForeColor = fore,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Paint += (s, e) => RoundButtonRegion(b, 16);
            return b;
        }

        private void UpdateStats()
        {
            lblPuntaje.Text = $"Puntaje: {puntaje}";
            lblErrores.Text = $"Errores: {errores}/{maxErrores}";
            lblTiempo.Text = $"Tiempo: {tiempoRestante}s";
            lblNivel.Text = $"Nivel: {nivelActual}";
        }

        private void CargarRecord()
        {
            try
            {
                if (System.IO.File.Exists(rutaArchivoRecord))
                    int.TryParse(System.IO.File.ReadAllText(rutaArchivoRecord), out puntajeMaximo);
                else
                    puntajeMaximo = 0;
            }
            catch
            {
                puntajeMaximo = 0;
            }
        }

        private void GuardarRecordSiAplica()
        {
            try
            {
                if (puntaje > puntajeMaximo)
                {
                    puntajeMaximo = puntaje;
                    System.IO.File.WriteAllText(rutaArchivoRecord, puntajeMaximo.ToString());
                }
            }
            catch { /* no rompas el juego por IO */ }
        }

        private List<T> MezclarLista<T>(List<T> lista)
        {
            return lista.OrderBy(x => rnd.Next()).ToList();
        }

        // =========================================================
        // Helpers: dibujo pro (bordes/redondeos)
        // =========================================================
        private void DrawBottomBorder(Graphics g, Control c)
        {
            using (Pen p = new Pen(colorBorde, 1))
            {
                g.DrawLine(p, 0, c.Height - 1, c.Width, c.Height - 1);
            }
        }

        private void DrawTopBorder(Graphics g, Control c)
        {
            using (Pen p = new Pen(colorBorde, 1))
            {
                g.DrawLine(p, 0, 0, c.Width, 0);
            }
        }

        private void DrawCardBorder(Graphics g, Control c, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (GraphicsPath path = RoundedRectPath(
                new Rectangle(1, 1, c.Width - 3, c.Height - 3), radius))
            using (Pen pen = new Pen(colorBorde, 2))
            {
                g.DrawPath(pen, path);
            }
        }


        private GraphicsPath RoundedRectPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }


        private void RoundButtonRegion(Control c, int radius)
        {
            if (c.Width < 10 || c.Height < 10) return;

            using (GraphicsPath path =
                RoundedRectPath(new Rectangle(0, 0, c.Width, c.Height), radius))
            {
                c.Region = new Region(path);
            }
        }

        private void ClasificaActividadForm_Load(object sender, EventArgs e)
        {

        }
    }
}
