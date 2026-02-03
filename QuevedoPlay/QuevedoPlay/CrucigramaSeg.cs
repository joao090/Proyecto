using QuevedoPlay.Estilos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QuevedoPlay
{
    public partial class CrucigramaSeg : Form
    {
        bool procesandoCarga = false;
        bool juegoIniciado = false;
        int totalIntentos = 0;

        int segundosAcumulados = 0;
        DateTime momentoInicioTurno;

        private Timer timerReloj;

        List<Palabra> palabras = new List<Palabra>();
        HashSet<string> palabrasYaContadas = new HashSet<string>();
        Dictionary<(int fila, int columna), TextBox> matrizCrucigrama = new Dictionary<(int, int), TextBox>();
        Dictionary<(int fila, int columna), char> respuestasCorrectas = new Dictionary<(int, int), char>();
        Dictionary<(int fila, int columna), char> respuestasUsuario = new Dictionary<(int, int), char>();
        Dictionary<(int fila, int columna), int> estadoValidacion = new Dictionary<(int, int), int>();

        public CrucigramaSeg()
        {
            InitializeComponent();
            ConfigurarTimer();
        }

        private void ConfigurarTimer()
        {
            timerReloj = new Timer();
            timerReloj.Interval = 500;
            timerReloj.Tick += (s, e) => ActualizarEstadisticas();
        }

        class Palabra
        {
            public string Nombre;
            public List<(int fila, int col)> Celdas;
        }

        private void CrucigramaSeg_Load(object sender, EventArgs e)
        {
            label4.Visible = true;
            RedondeoHelper.Aplicar(btnRegresar, 20);
            RedondeoHelper.Aplicar(btnSalir, 20);
            RedondeoHelper.Aplicar(label3, 15);
            RedondeoHelper.Aplicar(label5, 15);
            RedondeoHelper.Aplicar(panel2, 15);
            RedondeoHelper.Aplicar(label4, 15);
            RedondeoHelper.Aplicar(label6, 15);

            ConfigurarTextBoxesCrucigrama(this);
            CrearMatrizCrucigrama(this);
            CargarRespuestas();
            CargarPalabras();
            CargarProgreso();
        }

        private void ValidarPalabras()
        {
            foreach (var txt in matrizCrucigrama.Values)
                txt.BackColor = Color.White;

            foreach (var palabra in palabras)
            {
                bool palabraCompleta = true;
                bool palabraCorrecta = true;

                foreach (var (fila, col) in palabra.Celdas)
                {
                    if (!matrizCrucigrama.TryGetValue((fila, col), out TextBox txt)) continue;
                    if (string.IsNullOrWhiteSpace(txt.Text)) { palabraCompleta = false; break; }
                    if (respuestasCorrectas.TryGetValue((fila, col), out char correcta))
                    {
                        if (txt.Text.ToUpper()[0] != correcta) palabraCorrecta = false;
                    }
                }

                int estado = 0;
                if (palabraCompleta)
                {
                    if (palabraCorrecta)
                    {
                        estado = 1;
                        palabrasYaContadas.Remove(palabra.Nombre);
                    }
                    else
                    {
                        estado = 2;
                        if (!palabrasYaContadas.Contains(palabra.Nombre))
                        {
                            totalIntentos++;
                            palabrasYaContadas.Add(palabra.Nombre);
                        }
                    }
                }

                foreach (var (fila, col) in palabra.Celdas)
                {
                    if (!matrizCrucigrama.TryGetValue((fila, col), out TextBox txt)) continue;
                    if (estado == 1) txt.BackColor = Color.LightGreen;
                    else if (estado == 2 && txt.BackColor != Color.LightGreen) txt.BackColor = Color.LightCoral;
                    estadoValidacion[(fila, col)] = estado;
                }
            }
        }

        private void GuardarProgreso()
        {
            if (procesandoCarga) return;
            string ruta = $"datos/{Sesion.UsuarioActual}_crucigrama.txt";
            Directory.CreateDirectory("datos");

            int segundosSesion = juegoIniciado ? (int)(DateTime.Now - momentoInicioTurno).TotalSeconds : 0;
            int segundosActuales = segundosAcumulados + segundosSesion;

            List<string> lineas = new List<string>();
            lineas.Add($"#INTENTOS={totalIntentos}");
            lineas.Add($"#TIEMPO={segundosActuales}");

            foreach (var kv in respuestasUsuario)
            {
                int estado = estadoValidacion.ContainsKey(kv.Key) ? estadoValidacion[kv.Key] : 0;
                lineas.Add($"{kv.Key.fila},{kv.Key.columna}={kv.Value},{estado}");
            }
            File.WriteAllLines(ruta, lineas);
        }

        private void CargarProgreso()
        {
            string ruta = $"datos/{Sesion.UsuarioActual}_crucigrama.txt";
            if (!File.Exists(ruta)) return;

            procesandoCarga = true;
            foreach (string linea in File.ReadAllLines(ruta))
            {
                if (linea.StartsWith("#INTENTOS="))
                {
                    totalIntentos = int.Parse(linea.Split('=')[1]);
                    continue;
                }
                if (linea.StartsWith("#TIEMPO="))
                {
                    segundosAcumulados = int.Parse(linea.Split('=')[1]);
                    continue;
                }
                if (linea.StartsWith("#") || !linea.Contains("=")) continue;

                var partes = linea.Split('=');
                var pos = partes[0].Split(',');
                var datos = partes[1].Split(',');

                if (matrizCrucigrama.TryGetValue((int.Parse(pos[0]), int.Parse(pos[1])), out TextBox txt))
                {
                    txt.Text = datos[0];
                    respuestasUsuario[(int.Parse(pos[0]), int.Parse(pos[1]))] = datos[0][0];
                }
            }
            procesandoCarga = false;

            momentoInicioTurno = DateTime.Now;

            ValidarPalabras();
            ActualizarEstadisticas();
        }

        private void ActualizarEstadisticas()
        {
            int palabrasCorrectasCount = 0;
            foreach (var palabra in palabras)
            {
                bool palabraOk = true;
                foreach (var (fila, col) in palabra.Celdas)
                {
                    if (!matrizCrucigrama.TryGetValue((fila, col), out TextBox txt) || txt.BackColor != Color.LightGreen)
                    {
                        palabraOk = false;
                        break;
                    }
                }
                if (palabraOk) palabrasCorrectasCount++;
            }

            if (palabrasCorrectasCount == 10) timerReloj.Stop();

            int segundosSesion = juegoIniciado ? (int)(DateTime.Now - momentoInicioTurno).TotalSeconds : 0;
            TimeSpan t = TimeSpan.FromSeconds(segundosAcumulados + segundosSesion);
            string reloj = string.Format("{0:00}:{1:00}", (int)t.TotalMinutes, t.Seconds);

            double totalAcciones = palabrasCorrectasCount + totalIntentos;
            double precisionVal = totalAcciones > 0 ? ((double)palabrasCorrectasCount / totalAcciones) * 100 : 100;

            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (lblAciertos != null) lblAciertos.Text = palabrasCorrectasCount + "/10";
                    if (lblIntentos != null) lblIntentos.Text = totalIntentos.ToString();

                    if (lblTiempo != null)
                    {
                        lblTiempo.Text = reloj;
                        lblTiempo.Update();
                    }

                    if (lblPrecision != null) lblPrecision.Text = Math.Round(precisionVal) + "%";
                });
            }
        }

        private void Txt_TextChanged(object sender, EventArgs e)
        {
            if (procesandoCarga) return;

            if (!juegoIniciado)
            {
                juegoIniciado = true;
                momentoInicioTurno = DateTime.Now;
                timerReloj.Start();
            }

            TextBox txt = sender as TextBox;
            if (txt == null) return;

            string[] partes = txt.Name.Substring(3).Split('_');
            int columna = int.Parse(partes[0]);
            int fila = int.Parse(partes[1]);

            procesandoCarga = true;
            txt.Text = txt.Text.ToUpper();
            txt.SelectionStart = txt.Text.Length;
            procesandoCarga = false;

            if (!string.IsNullOrEmpty(txt.Text))
                respuestasUsuario[(fila, columna)] = txt.Text[0];
            else
                respuestasUsuario.Remove((fila, columna));

            ValidarPalabras();
            GuardarProgreso();
            ActualizarEstadisticas();
        }

        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!char.IsLetter(e.KeyChar)) { e.Handled = true; return; }
            TextBox txt = sender as TextBox;
            txt.Text = e.KeyChar.ToString().ToUpper();
            e.Handled = true;
        }

        private void CargarRespuestas()
        {
            respuestasCorrectas.Clear();
            char[] saca = "SACAPINTAS".ToCharArray();
            for (int i = 0; i < saca.Length; i++) respuestasCorrectas[(2, 8 + i)] = saca[i];
            char[] abi = "ABIGEATO".ToCharArray();
            for (int i = 0; i < abi.Length; i++) respuestasCorrectas[(4, 16 + i)] = abi[i];
            char[] info = "INFORMATIVA".ToCharArray();
            for (int i = 0; i < info.Length; i++) respuestasCorrectas[(12, 2 + i)] = info[i];
            char[] ciu = "CIUDADANA".ToCharArray();
            for (int i = 0; i < ciu.Length; i++) respuestasCorrectas[(16, 4 + i)] = ciu[i];
            char[] eco = "ECONOMICA".ToCharArray();
            for (int i = 0; i < eco.Length; i++) respuestasCorrectas[(12, 17 + i)] = eco[i];
            char[] psi = "PSICOLOGICA".ToCharArray();
            for (int i = 0; i < psi.Length; i++) respuestasCorrectas[(2 + i, 12)] = psi[i];
            char[] asa = "ASALTOS".ToCharArray();
            for (int i = 0; i < asa.Length; i++) respuestasCorrectas[(2 + i, 16)] = asa[i];
            char[] ext = "EXTORSION".ToCharArray();
            for (int i = 0; i < ext.Length; i++) respuestasCorrectas[(4 + i, 20)] = ext[i];
            char[] mov = "MOVILIZARSE".ToCharArray();
            for (int i = 0; i < mov.Length; i++) respuestasCorrectas[(11 + i, 5)] = mov[i];
            char[] san = "SANITARIA".ToCharArray();
            for (int i = 0; i < san.Length; i++) respuestasCorrectas[(11 + i, 25)] = san[i];
        }

        private void CargarPalabras()
        {
            palabras.Clear();
            palabras.Add(new Palabra { Nombre = "SACAPINTAS", Celdas = new List<(int, int)> { (2, 8), (2, 9), (2, 10), (2, 11), (2, 12), (2, 13), (2, 14), (2, 15), (2, 16), (2, 17) } });
            palabras.Add(new Palabra { Nombre = "ABIGEATO", Celdas = new List<(int, int)> { (4, 16), (4, 17), (4, 18), (4, 19), (4, 20), (4, 21), (4, 22), (4, 23) } });
            palabras.Add(new Palabra { Nombre = "INFORMATIVA", Celdas = new List<(int, int)> { (12, 2), (12, 3), (12, 4), (12, 5), (12, 6), (12, 7), (12, 8), (12, 9), (12, 10), (12, 11), (12, 12) } });
            palabras.Add(new Palabra { Nombre = "CIUDADANA", Celdas = new List<(int, int)> { (16, 4), (16, 5), (16, 6), (16, 7), (16, 8), (16, 9), (16, 10), (16, 11), (16, 12) } });
            palabras.Add(new Palabra { Nombre = "ECONOMICA", Celdas = new List<(int, int)> { (12, 17), (12, 18), (12, 19), (12, 20), (12, 21), (12, 22), (12, 23), (12, 24), (12, 25) } });
            palabras.Add(new Palabra { Nombre = "PSICOLOGICA", Celdas = new List<(int, int)> { (2, 12), (3, 12), (4, 12), (5, 12), (6, 12), (7, 12), (8, 12), (9, 12), (10, 12), (11, 12), (12, 12) } });
            palabras.Add(new Palabra { Nombre = "ASALTOS", Celdas = new List<(int, int)> { (2, 16), (3, 16), (4, 16), (5, 16), (6, 16), (7, 16), (8, 16) } });
            palabras.Add(new Palabra { Nombre = "EXTORSION", Celdas = new List<(int, int)> { (4, 20), (5, 20), (6, 20), (7, 20), (8, 20), (9, 20), (10, 20), (11, 20), (12, 20) } });
            palabras.Add(new Palabra { Nombre = "MOVILIZARSE", Celdas = new List<(int, int)> { (11, 5), (12, 5), (13, 5), (14, 5), (15, 5), (16, 5), (17, 5), (18, 5), (19, 5), (20, 5), (21, 5) } });
            palabras.Add(new Palabra { Nombre = "SANITARIA", Celdas = new List<(int, int)> { (11, 25), (12, 25), (13, 25), (14, 25), (15, 25), (16, 25), (17, 25), (18, 25), (19, 25) } });
        }

        private void ConfigurarTextBoxesCrucigrama(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox txt && txt.Name.StartsWith("txt"))
                {
                    txt.MaxLength = 1;
                    txt.KeyPress += SoloLetras_KeyPress;
                    txt.TextChanged += Txt_TextChanged;
                    txt.KeyDown += MoverConFlechas_KeyDown;
                }
                if (c.HasChildren) ConfigurarTextBoxesCrucigrama(c);
            }
        }

        private void CrearMatrizCrucigrama(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox txt && txt.Name.StartsWith("txt"))
                {
                    string[] partes = txt.Name.Substring(3).Split('_');
                    matrizCrucigrama[(int.Parse(partes[1]), int.Parse(partes[0]))] = txt;
                }
                if (c.HasChildren) CrearMatrizCrucigrama(c);
            }
        }

        private void MoverConFlechas_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txtActual = sender as TextBox;
            string[] partes = txtActual.Name.Substring(3).Split('_');
            int col = int.Parse(partes[0]);
            int fila = int.Parse(partes[1]);
            int nF = fila, nC = col;

            if (e.KeyCode == Keys.Left) nC--;
            else if (e.KeyCode == Keys.Right) nC++;
            else if (e.KeyCode == Keys.Up) nF--;
            else if (e.KeyCode == Keys.Down) nF++;
            else return;

            if (matrizCrucigrama.TryGetValue((nF, nC), out TextBox txtDestino))
            {
                txtDestino.Focus();
                e.Handled = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            GuardarProgreso();
            GuardarRecordFinal();
            Application.Exit();
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            GuardarProgreso();
            GuardarRecordFinal();
            new OpcionesTurismoSeguro().Show();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Borrar todo el progreso?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                procesandoCarga = true;
                juegoIniciado = false;
                timerReloj.Stop();

                totalIntentos = 0;
                segundosAcumulados = 0;
                foreach (var txt in matrizCrucigrama.Values) { txt.Clear(); txt.BackColor = Color.White; }
                respuestasUsuario.Clear();
                estadoValidacion.Clear();
                procesandoCarga = false;
                GuardarProgreso();
                ActualizarEstadisticas();
            }
        }

        private void GuardarRecordFinal()
        {
            string ruta = "datos/Récord_Crucigrama.txt";
            string linea = $"{DateTime.Now} | Usuario: {Sesion.UsuarioActual} | Aciertos: {lblAciertos.Text} | Intentos: {totalIntentos} | Tiempo: {lblTiempo.Text}\n";
            File.AppendAllText(ruta, linea);
        }

        private void label3_Click(object sender, EventArgs e) { label4.Visible = true; label6.Visible = false; panel2.Visible = false; }
        private void label5_Click(object sender, EventArgs e) { panel2.Visible = false; label4.Visible = false; label6.Visible = true; }
        private void pictureBox2_Click(object sender, EventArgs e) { panel2.Visible = true; label4.Visible = false; label6.Visible = false; }
    }
}