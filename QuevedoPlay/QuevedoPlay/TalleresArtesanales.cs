using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace QuevedoPlay
{
    public partial class FrmActividadDetalle : Form
    {
        // =========================
        // CONFIG / ESTADO
        // =========================
        private readonly Color Danger = Color.Firebrick;
        private readonly Color Primary = Color.SteelBlue;

        private ActividadData _data;

        // Layout (espaciados)
        private const int SIDE_MARGIN = 40;
        private const int GAP_CARDS = 30;
        private const int GAP_AFTER_DESC = 16;
        private const int GAP_AFTER_CARDS = 28;
        private const int GAP_SUBTEXT = 12;
        private const int GAP_BOTTOM = 25;

        // Anti re-open modal (por click fantasma)
        private int _lastModalCloseTick = 0;
        private const int MODAL_GUARD_MS = 250;

        // Para no spamear re-layout en Resize
        private bool _layoutQueued = false;

        // =========================
        // CONSTRUCTORES
        // =========================
        public FrmActividadDetalle()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Load += FrmActividadDetalle_Load;
            Shown += (s, e) => ApplyAllLayout();
            Resize += (s, e) => ApplyAllLayout();
        }

        public FrmActividadDetalle(ActividadData data) : this()
        {
            _data = data;
        }

        // =========================
        // LOAD
        // =========================
        private void FrmActividadDetalle_Load(object sender, EventArgs e)
        {
            ApplyTheme();

            // Si NO te pasan data, por defecto usa Talleres
            if (_data == null)
                _data = ActividadBuilders.BuildDefaultTalleres();


            ApplyDataToUI(_data);

            // Hover
            WireHover(panel1);
            WireHover(panel2);
            WireHover(panel3);

            // Modal click
            BindCardClick(panel1, _data.ModalTitulo1, _data.ModalQueEs1, _data.ModalAprendes1, _data.ModalTip1);
            BindCardClick(panel2, _data.ModalTitulo2, _data.ModalQueEs2, _data.ModalAprendes2, _data.ModalTip2);
            BindCardClick(panel3, _data.ModalTitulo3, _data.ModalQueEs3, _data.ModalAprendes3, _data.ModalTip3);

            ApplyAllLayout();
            RoundAll();
        }

        // =========================
        // DATA -> UI
        // =========================
        private void ApplyDataToUI(ActividadData d)
        {
            if (d == null) return;

            label1.Text = d.Titulo ?? "";
            lblDescripcion.Text = d.Descripcion ?? "";

            label3.Text = d.Card1Titulo ?? "";
            label4.Text = d.Card2Titulo ?? "";
            label5.Text = d.Card3Titulo ?? "";

            lblSub1.Text = d.Sub1 ?? "";
            lblSub2.Text = d.Sub2 ?? "";
            lblSub3.Text = d.Sub3 ?? "";

            lblTipTitulo.Text = string.IsNullOrWhiteSpace(d.TipTitulo) ? "✅ Recomendación rápida" : d.TipTitulo;
            lblTipTexto.Text = d.TipTexto ?? "";

            if (d.Img1 != null) pic1.Image = d.Img1;
            if (d.Img2 != null) pic2.Image = d.Img2;
            if (d.Img3 != null) pic3.Image = d.Img3;

            pic1.SizeMode = PictureBoxSizeMode.Zoom;
            pic2.SizeMode = PictureBoxSizeMode.Zoom;
            pic3.SizeMode = PictureBoxSizeMode.Zoom;

            lblSub1.BringToFront();
            lblSub2.BringToFront();
            lblSub3.BringToFront();
        }

        // =========================
        // BUILDERS (3 actividades)
        // =========================
        public static ActividadData BuildTalleres()
        {
            // OJO: esta función no puede llamar LoadImg (es static).
            // Por eso se usa BuildDefaultTalleres() que sí carga imágenes.
            return null;
        }


        // ✅ Llama estas funciones desde tu form principal
        public ActividadData BuildCaminatas()
        {
            return new ActividadData
            {
                Titulo = "Caminatas Guiadas",
                Descripcion =
                    "Las caminatas guiadas permiten recorrer zonas naturales o representativas con un guía,\n" +
                    "aprendiendo sobre la historia, naturaleza y cultura del lugar.",

                Card1Titulo = "Ruta Natural",
                Card2Titulo = "Avistamiento",
                Card3Titulo = "Fotografía",

                Sub1 = "Explora senderos y paisajes.",
                Sub2 = "Observa fauna y flora local.",
                Sub3 = "Captura momentos del recorrido.",

                TipTitulo = "✅ Recomendación rápida",
                TipTexto = "Lleva agua, usa ropa cómoda y respeta las señales del guía.",

                Img1 = LoadImg("RutaNatural.png"),
                Img2 = LoadImg("Avistamiento.png"),
                Img3 = LoadImg("Fotografia.png"),

                ModalTitulo1 = "Ruta Natural",
                ModalQueEs1 = "Recorridos por senderos naturales para conocer paisajes y ecosistemas.",
                ModalAprendes1 = "Aprendes: orientación básica, cuidado del entorno y puntos de interés.",
                ModalTip1 = "Tip: no dejes basura y respeta la flora.",

                ModalTitulo2 = "Avistamiento",
                ModalQueEs2 = "Actividad para observar aves y animales en su hábitat.",
                ModalAprendes2 = "Aprendes: paciencia, identificación básica y respeto por la fauna.",
                ModalTip2 = "Tip: evita ruidos fuertes y no alimentes animales.",

                ModalTitulo3 = "Fotografía",
                ModalQueEs3 = "Captura imágenes del recorrido: paisajes, detalles y cultura.",
                ModalAprendes3 = "Aprendes: encuadre, luz y contar historias con fotos.",
                ModalTip3 = "Tip: pide permiso antes de fotografiar personas."
            };
        }

        public ActividadData BuildVisitas()
        {
            return new ActividadData
            {
                Titulo = "Visitas Guiadas",
                Descripcion =
                    "Las visitas guiadas recorren lugares históricos o culturales, explicando su significado\n" +
                    "y ayudando al visitante a entender la identidad del sitio.",

                Card1Titulo = "Museo",
                Card2Titulo = "Centro Histórico",
                Card3Titulo = "Patrimonio",

                Sub1 = "Conoce objetos e historia.",
                Sub2 = "Recorre sitios representativos.",
                Sub3 = "Aprende sobre tradiciones.",

                TipTitulo = "✅ Recomendación rápida",
                TipTexto = "Respeta las normas del lugar y escucha al guía para aprovechar la visita.",

                Img1 = LoadImg("Museo.png"),
                Img2 = LoadImg("CentroHistorico.png"),
                Img3 = LoadImg("Patrimonio.png"),

                ModalTitulo1 = "Museo",
                ModalQueEs1 = "Espacio donde se conservan piezas y relatos de la historia local.",
                ModalAprendes1 = "Aprendes: contexto histórico y valor cultural.",
                ModalTip1 = "Tip: no tocar piezas y no usar flash si está prohibido.",

                ModalTitulo2 = "Centro Histórico",
                ModalQueEs2 = "Recorrido por calles y lugares que representan la memoria de la ciudad.",
                ModalAprendes2 = "Aprendes: arquitectura, historias y cambios del lugar.",
                ModalTip2 = "Tip: mantente con el grupo para no perderte.",

                ModalTitulo3 = "Patrimonio",
                ModalQueEs3 = "Conjunto de tradiciones, edificios y símbolos que identifican a una comunidad.",
                ModalAprendes3 = "Aprendes: respeto por costumbres y significado cultural.",
                ModalTip3 = "Tip: pregunta antes de grabar o entrevistar."
            };
        }

        // =========================
        // IMG LOADER
        // =========================
        private Image LoadImg(string fileName)
        {
            // Busca en: bin\Debug\Resources\Imagenes\
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDir, "Resources", "Imagenes", fileName);

            if (!File.Exists(path)) return null;

            using (var temp = Image.FromFile(path))
            {
                return new Bitmap(temp);
            }
        }

        // =========================
        // THEME
        // =========================
        private void ApplyTheme()
        {
            BackColor = Color.White;

            label1.ForeColor = Color.Black;
            label1.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            label1.AutoSize = true;

            lblDescripcion.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblDescripcion.ForeColor = Color.DimGray;
            lblDescripcion.TextAlign = ContentAlignment.MiddleCenter;
            lblDescripcion.AutoSize = true;

            pnlTip.BackColor = Color.FromArgb(245, 245, 245);
            pnlTip.Padding = new Padding(18);

            lblTipTitulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTipTitulo.ForeColor = Color.Black;

            lblTipTexto.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblTipTexto.ForeColor = Color.DimGray;
            lblTipTexto.AutoSize = true;

            btnRegresar.BackColor = Danger;
            btnRegresar.ForeColor = Color.White;
            btnRegresar.FlatStyle = FlatStyle.Flat;
            btnRegresar.FlatAppearance.BorderSize = 0;
            btnRegresar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegresar.Cursor = Cursors.Hand;

            StyleSub(lblSub1);
            StyleSub(lblSub2);
            StyleSub(lblSub3);
        }

        private void StyleSub(Label lbl)
        {
            if (lbl == null) return;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lbl.ForeColor = Color.DimGray;
            lbl.AutoSize = true;
        }

        // =========================
        // LAYOUT
        // =========================
        private void ApplyAllLayout()
        {
            if (!IsHandleCreated) return;
            if (_layoutQueued) return;

            _layoutQueued = true;

            BeginInvoke((Action)(() =>
            {
                _layoutQueued = false;
                ArrangeLayout();
                RoundAll();
            }));
        }

        private void ArrangeLayout()
        {
            SuspendLayout();

            CenterLabelTop(label1, 28);

            int maxTextWidth = 900;
            int descWidth = Math.Min(maxTextWidth, ClientSize.Width - SIDE_MARGIN * 2);

            lblDescripcion.MaximumSize = new Size(descWidth, 0);
            lblDescripcion.AutoSize = true;
            CenterLabelUnder(label1, lblDescripcion, 10);

            int totalCardsWidth = panel1.Width + panel2.Width + panel3.Width + GAP_CARDS * 2;
            int startX = (ClientSize.Width - totalCardsWidth) / 2;

            int yCards = lblDescripcion.Bottom + GAP_AFTER_DESC;

            panel1.Left = startX;
            panel2.Left = panel1.Right + GAP_CARDS;
            panel3.Left = panel2.Right + GAP_CARDS;

            panel1.Top = yCards;
            panel2.Top = yCards;
            panel3.Top = yCards;

            PlaceSubUnderPanelTitle(panel1, lblSub1, GAP_SUBTEXT);
            PlaceSubUnderPanelTitle(panel2, lblSub2, GAP_SUBTEXT);
            PlaceSubUnderPanelTitle(panel3, lblSub3, GAP_SUBTEXT);

            int bottomSub = Math.Max(lblSub1.Bottom, Math.Max(lblSub2.Bottom, lblSub3.Bottom));

            btnRegresar.Left = 30;
            btnRegresar.Top = ClientSize.Height - btnRegresar.Height - GAP_BOTTOM;

            int tipMaxWidth = 980;
            pnlTip.Width = Math.Min(tipMaxWidth, ClientSize.Width - SIDE_MARGIN * 2);
            pnlTip.Left = (ClientSize.Width - pnlTip.Width) / 2;

            ArrangeTipInner();

            int idealTop = bottomSub + GAP_AFTER_CARDS;
            int maxTop = btnRegresar.Top - pnlTip.Height - 16;
            pnlTip.Top = Math.Min(idealTop, maxTop);

            ResumeLayout();
        }

        private void ArrangeTipInner()
        {
            pnlTip.SuspendLayout();

            int pad = pnlTip.Padding.Left;

            lblTipTitulo.Left = pad;
            lblTipTitulo.Top = pad;
            lblTipTitulo.AutoSize = true;

            int textWidth = pnlTip.ClientSize.Width - pad * 2;
            if (textWidth < 200) textWidth = 200;

            lblTipTexto.MaximumSize = new Size(textWidth, 0);
            lblTipTexto.Left = pad;
            lblTipTexto.Top = lblTipTitulo.Bottom + 10;
            lblTipTexto.AutoSize = true;

            pnlTip.Height = lblTipTexto.Bottom + pad;

            pnlTip.ResumeLayout();
        }

        private void CenterLabelTop(Label lbl, int top)
        {
            if (lbl == null) return;
            lbl.Left = (ClientSize.Width - lbl.PreferredWidth) / 2;
            lbl.Top = top;
        }

        private void CenterLabelUnder(Control above, Label lbl, int gap)
        {
            if (above == null || lbl == null) return;
            lbl.Left = (ClientSize.Width - lbl.PreferredWidth) / 2;
            lbl.Top = above.Bottom + gap;
        }

        private void PlaceSubUnderPanelTitle(Panel panel, Label sub, int gap)
        {
            if (panel == null || sub == null) return;

            Label title = null;

            foreach (Control c in panel.Controls)
            {
                if (c is Label l)
                {
                    if (title == null) title = l;
                    if (l.Font != null && l.Font.Bold) { title = l; break; }
                    if (l.Width > title.Width) title = l;
                }
            }

            if (title == null)
            {
                sub.Left = panel.Left + (panel.Width - sub.PreferredWidth) / 2;
                sub.Top = panel.Bottom + gap;
                return;
            }

            int titleScreenX = panel.Left + title.Left;
            int titleBottomY = panel.Top + title.Bottom;

            sub.Left = titleScreenX + (title.Width - sub.PreferredWidth) / 2;
            sub.Top = titleBottomY + gap;
        }
        private void WireHover(Panel p)
        {
            if (p == null) return;

            Color normal = Color.White;
            Color hover = Color.WhiteSmoke;

            p.BackColor = normal;
            p.Cursor = Cursors.Hand;

            p.MouseEnter += (s, e) => p.BackColor = hover;
            p.MouseLeave += (s, e) => p.BackColor = normal;
        }

        private void BindCardClick(Control card, string titulo, string queEs, string queAprendes, string tip)
        {
            if (card == null) return;

            void handler()
            {
                int now = Environment.TickCount;
                if (now - _lastModalCloseTick < MODAL_GUARD_MS) return;

                ShowInfoModal(titulo ?? "Información", queEs ?? "", queAprendes ?? "", tip ?? "");
            }

            card.MouseClick += (s, e) => handler();

            foreach (Control c in card.Controls)
                c.MouseClick += (s, e) => handler();
        }

        private void ShowInfoModal(string titulo, string queEs, string queAprendes, string tip)
        {
            using (Form dlg = new Form())
            {
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.BackColor = Color.White;
                dlg.ClientSize = new Size(700, 420);

                // ===== TÍTULO =====
                Label lblTitulo = new Label
                {
                    Text = titulo,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.Black,
                    AutoSize = false,
                    Left = 30,
                    Top = 25,
                    Width = dlg.ClientSize.Width - 60,
                    Height = 40
                };

                // ===== CONTENEDOR =====
                Panel cont = new Panel
                {
                    Left = 30,
                    Top = lblTitulo.Bottom + 10,
                    Width = dlg.ClientSize.Width - 60,
                    Height = 270,
                    AutoScroll = true
                };

                // helper bloque
                Panel CrearBloque(string subtitulo, string texto)
                {
                    Panel p = new Panel
                    {
                        Width = cont.Width - 20,
                        BackColor = Color.FromArgb(245, 246, 248),
                        Padding = new Padding(14)
                    };

                    Label t = new Label
                    {
                        Text = subtitulo,
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        Left = 0,
                        Top = 0
                    };

                    Label b = new Label
                    {
                        Text = texto,
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.DimGray,
                        AutoSize = true,
                        MaximumSize = new Size(p.Width - 10, 0),
                        Left = 0,
                        Top = t.Bottom + 8
                    };

                    p.Controls.Add(t);
                    p.Controls.Add(b);
                    p.Height = b.Bottom + 10;

                    return p;
                }

                // ===== BLOQUES =====
                Panel b1 = CrearBloque("¿Qué es esta actividad?", queEs);
                Panel b2 = CrearBloque("¿Qué aprende el visitante?", queAprendes);
                Panel b3 = CrearBloque("Recomendaciones para el visitante", tip);

                int y = 0;
                foreach (var b in new[] { b1, b2, b3 })
                {
                    b.Left = 0;
                    b.Top = y;
                    cont.Controls.Add(b);
                    y = b.Bottom + 14;
                }

                // ===== BOTÓN =====
                Button btnOk = new Button
                {
                    Text = "Entendido",
                    Width = 150,
                    Height = 40,
                    BackColor = Primary,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Left = dlg.ClientSize.Width - 180,
                    Top = dlg.ClientSize.Height - 60
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s, e) => dlg.Close();

                dlg.Controls.Add(lblTitulo);
                dlg.Controls.Add(cont);
                dlg.Controls.Add(btnOk);

                dlg.ShowDialog(this);
            }
        }

        private void RoundAll()
        {
            Redondear(panel1, 25);
            Redondear(panel2, 25);
            Redondear(panel3, 25);
            Redondear(btnRegresar, 25);
            Redondear(pnlTip, 20);
        }

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

        // =========================
        // NAV / BOTONES
        // =========================
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            new SoundPlayer(Properties.Resources.clicwav).Play();
            FrmActividadesTuristicas opciones = new FrmActividadesTuristicas();
            opciones.Show();
            Close();
        }

        // Tu botón X
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmActividadDetalle_Load_1(object sender, EventArgs e)
        {

        }
    }
}
