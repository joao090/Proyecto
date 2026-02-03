using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp4.Forms
{
    public partial class Orientacion : Form
    {
        // Referencia al formulario principal (OpcionesTurismo)
        private Form _formOpcionesTurismo;

        // Constructor que recibe el formulario padre
        public Orientacion(Form formOpcionesTurismo)
        {
            InitializeComponent();
            _formOpcionesTurismo = formOpcionesTurismo;
        }

        // Botón para abrir Alimentacion
        private void button1_Click(object sender, EventArgs e)
        {
            Alimentacion alimento = new Alimentacion(this); // pasamos Orientacion como padre
            alimento.Show();
            this.Hide();
        }

        // Botón para abrir Hospedaje
        private void button2_Click(object sender, EventArgs e)
        {
            Hospedaje hotel = new Hospedaje(this);
            hotel.Show();
            this.Hide();
        }

        // Botón para abrir Transporte
        private void button3_Click(object sender, EventArgs e)
        {
            Transporte bus = new Transporte(this);
            bus.Show();
            this.Hide();
        }

        // Botón para abrir Ahorcado
        private void bnJugar_Click(object sender, EventArgs e)
        {
            Ahorcado a = new Ahorcado(this);
            a.Show();
            this.Hide();
        }

        // Botón para regresar al formulario principal
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            _formOpcionesTurismo.Show(); // mostramos OpcionesTurismo
            this.Close();                // cerramos Orientacion
        }
    }
}




