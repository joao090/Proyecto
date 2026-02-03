using System;
using System.Media;
using System.Windows.Forms;

namespace QuevedoPlay
{
    public partial class FrmCaminatasGuiadas : Form
    {
        public FrmCaminatasGuiadas()
        {
            InitializeComponent();
        }

        private void FrmCaminatasGuiadas_Load(object sender, EventArgs e)
        {

        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            FrmActividadesTuristicas Opciones = new FrmActividadesTuristicas();
            Opciones.Show();
            this.Close();
            SoundPlayer sonido = new SoundPlayer(Properties.Resources.clicwav);
            sonido.Play();
        }
    }
}
