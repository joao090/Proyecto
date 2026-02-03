namespace ConsoleApp4
{
    partial class Ahorcado
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
            this.flFichasDeJuego = new System.Windows.Forms.FlowLayoutPanel();
            this.picAhorcado = new System.Windows.Forms.PictureBox();
            this.flPalabra = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnIniciarJuego = new System.Windows.Forms.PictureBox();
            this.bntVolver2 = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picAhorcado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIniciarJuego)).BeginInit();
            this.SuspendLayout();
            // 
            // flFichasDeJuego
            // 
            this.flFichasDeJuego.Location = new System.Drawing.Point(12, 89);
            this.flFichasDeJuego.Name = "flFichasDeJuego";
            this.flFichasDeJuego.Size = new System.Drawing.Size(445, 300);
            this.flFichasDeJuego.TabIndex = 0;
            // 
            // picAhorcado
            // 
            this.picAhorcado.Location = new System.Drawing.Point(463, 89);
            this.picAhorcado.Name = "picAhorcado";
            this.picAhorcado.Size = new System.Drawing.Size(250, 300);
            this.picAhorcado.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picAhorcado.TabIndex = 1;
            this.picAhorcado.TabStop = false;
            // 
            // flPalabra
            // 
            this.flPalabra.Location = new System.Drawing.Point(121, 400);
            this.flPalabra.Name = "flPalabra";
            this.flPalabra.Size = new System.Drawing.Size(608, 88);
            this.flPalabra.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConsoleApp4.Properties.Resources.llama;
            this.pictureBox1.Location = new System.Drawing.Point(0, 318);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(115, 161);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblMensaje
            // 
            this.lblMensaje.AutoSize = true;
            this.lblMensaje.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.ForeColor = System.Drawing.Color.Red;
            this.lblMensaje.Location = new System.Drawing.Point(504, 50);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(189, 32);
            this.lblMensaje.TabIndex = 3;
            this.lblMensaje.Text = "¡ Incorrecto ! ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(73, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(265, 33);
            this.label2.TabIndex = 4;
            this.label2.Text = "Adivina la palabra ";
            // 
            // btnIniciarJuego
            // 
            this.btnIniciarJuego.Image = global::ConsoleApp4.Properties.Resources.btnStart;
            this.btnIniciarJuego.Location = new System.Drawing.Point(14, 13);
            this.btnIniciarJuego.Name = "btnIniciarJuego";
            this.btnIniciarJuego.Size = new System.Drawing.Size(445, 46);
            this.btnIniciarJuego.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnIniciarJuego.TabIndex = 5;
            this.btnIniciarJuego.TabStop = false;
            this.btnIniciarJuego.Click += new System.EventHandler(this.btnIniciarJuego_Click);
            // 
            // bntVolver2
            // 
            this.bntVolver2.BackColor = System.Drawing.Color.Red;
            this.bntVolver2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntVolver2.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntVolver2.ForeColor = System.Drawing.Color.White;
            this.bntVolver2.Location = new System.Drawing.Point(1, 485);
            this.bntVolver2.Name = "bntVolver2";
            this.bntVolver2.Size = new System.Drawing.Size(115, 35);
            this.bntVolver2.TabIndex = 6;
            this.bntVolver2.Text = "REGRESAR";
            this.bntVolver2.UseVisualStyleBackColor = false;
            this.bntVolver2.Click += new System.EventHandler(this.bntVolver2_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.Red;
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.White;
            this.btnSalir.Location = new System.Drawing.Point(696, 10);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(51, 37);
            this.btnSalir.TabIndex = 14;
            this.btnSalir.Text = "X";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // Ahorcado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(759, 529);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.bntVolver2);
            this.Controls.Add(this.btnIniciarJuego);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.flPalabra);
            this.Controls.Add(this.picAhorcado);
            this.Controls.Add(this.flFichasDeJuego);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Ahorcado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ahorcado";
            this.Load += new System.EventHandler(this.Ahorcado_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picAhorcado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIniciarJuego)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flFichasDeJuego;
        private System.Windows.Forms.PictureBox picAhorcado;
        private System.Windows.Forms.FlowLayoutPanel flPalabra;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox btnIniciarJuego;
        private System.Windows.Forms.Button bntVolver2;
        private System.Windows.Forms.Button btnSalir;
    }
}