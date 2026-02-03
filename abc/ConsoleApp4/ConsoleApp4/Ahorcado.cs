using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConsoleApp4.Forms;

namespace ConsoleApp4
{
    public partial class Ahorcado : Form
    {
        char[] PalabrasAdivinadas;
        int Oportunidades;
        char[] PalabraSeleccionada;
        char[] Alfabeto;
        string[] Palabras;

        private Orientacion _formOrientacion;
        public Ahorcado(Orientacion formOrientacion)
        {
            InitializeComponent();
            _formOrientacion = formOrientacion;
        }

        public void IniciarJuego() 
        {
            // Valores iniciales del juego
            flFichasDeJuego.Controls.Clear();
            flFichasDeJuego.Enabled = true;
            picAhorcado.Image = null;
            lblMensaje.Visible = false;
            Oportunidades = 0; // Fallo
            btnIniciarJuego.Image = Properties.Resources.Jugando;

            Palabras = new string[]
            {
                "quevedo", "orientacion", "turismo",
                "hospedaje", "alimentacio   n", "transporte"
            };

            Alfabeto = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ".ToCharArray();

            //Palabra aleatoria - Adivinar
            Random random = new Random();
            int IndicePalabraSeleccionada = random.Next(0, Palabras.Length);
            PalabraSeleccionada = Palabras[IndicePalabraSeleccionada].ToUpper().ToCharArray();
            PalabrasAdivinadas = PalabraSeleccionada;

            // Ciclo que carga el alfabeto en un flowlayout - flFichasDeJuego 

            foreach (char LetrasAlfabeto in Alfabeto)
            {
                Button btnLetra = new Button();
                btnLetra.Tag = "";
                btnLetra.Text = LetrasAlfabeto.ToString();
                btnLetra.Width = 60;
                btnLetra.Height = 40;
                btnLetra.Click += Compara;
                btnLetra.ForeColor = Color.White;
                btnLetra.Font = new Font(btnLetra.Font.Name, 15, FontStyle.Bold);
                btnLetra.BackColor = Color.Black;
                btnLetra.Name = LetrasAlfabeto.ToString();
                flFichasDeJuego.Controls.Add(btnLetra);
            }
            flPalabra.Controls.Clear();

                for (int IndiceValorLetra  = 0; IndiceValorLetra < PalabraSeleccionada.Length; IndiceValorLetra++)
            {
                Button Letra = new Button();
                Letra.Tag = PalabraSeleccionada[IndiceValorLetra].ToString();
                Letra.Width = 46;
                Letra.Height = 80;
                Letra.ForeColor = Color.Blue;
                Letra.Text = "_";
                Letra.Font = new Font(Letra.Font.Name, 32, FontStyle.Bold);
                Letra.BackColor = Color.Black;
                Letra.FlatStyle =System.Windows.Forms.FlatStyle.Flat;
                Letra.Name = "Adivinado" + IndiceValorLetra.ToString();
                Letra.BackgroundImage = ((System.Drawing.Image)(Properties.Resources.acertijo));
                flPalabra.Controls.Add(Letra);


            }
        }

        void Compara(object sender, EventArgs e) {

            //Botón presionado se desactiva
            bool encontrado = false;
            Button btn = (Button)sender;
            btn.BackColor = Color.White;
            btn.ForeColor = Color.Black; 
            btn.Enabled = false;

            //Compara la letra seleccionada con las letras de la palabra 
            for (int indiceRevisar = 0; indiceRevisar < PalabrasAdivinadas.Length; indiceRevisar++)
            {
                if (PalabrasAdivinadas[indiceRevisar] == Char.Parse(btn.Text))
                {
                    //Si eziste la letra actualizada la palabra agregando el valor correspondiente 
                    Button tbx = this.Controls.Find("Adivinado" + indiceRevisar,true).FirstOrDefault() as Button;
                    tbx.Text = PalabrasAdivinadas[indiceRevisar].ToString();
                    encontrado = true;
                }
            
            
            
            }
            
            //Verificar si todas las letras de la palabra estan destapadas
            bool Ganaste = true;

            for (int indiceAnalizadorGanador = 0; indiceAnalizadorGanador < PalabrasAdivinadas .Length; indiceAnalizadorGanador++)
            {

                //Si el usuario tiene letras pemdientes por destapar se cambia el estatus
                if (PalabrasAdivinadas[indiceAnalizadorGanador] != '-')
                {
                    Ganaste = false;
                }

            }

            // Si el estatus de la variable no cambia quiere decir el usuario gano
            if (Ganaste) { MessageBox.Show("Ganaste ok,ok"); btnIniciarJuego.Image = Properties.Resources.btnStart; }

            if (!encontrado)
            {
                Oportunidades = Oportunidades + 1;
                picAhorcado.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("ahorcado" + Oportunidades);

                // Si las oportunidadesya se acabaron (mostrar la palabra)
                if (Oportunidades == 8)
                {
                    lblMensaje.Visible = true;
                    //Muestra la palabra que el usuario intentaba descubrir 
                    for ( int indiceValorLetra = 0; indiceValorLetra < PalabraSeleccionada.Length; indiceValorLetra++)
                    {
                        Button btnLetra = this.Controls.Find("Adivinado" + indiceValorLetra, true).FirstOrDefault() as Button;
                        btnLetra.Text = btnLetra.Tag.ToString();

                    }

                    // Desactiva las fichas y cambia le botón de reinicio 
                    flFichasDeJuego.Enabled = false;
                    btnIniciarJuego.Image = Properties.Resources.btnStart;
                }
            }

        }
        private void Ahorcado_Load(object sender, EventArgs e)
        {
            IniciarJuego();
        }

        private void btnIniciarJuego_Click(object sender, EventArgs e)
        {
            //Iniciar Juego
            IniciarJuego();
        }

        private void bntVolver2_Click(object sender, EventArgs e)
        {
            _formOrientacion.Show();
            Hide();
        }
    }
}
