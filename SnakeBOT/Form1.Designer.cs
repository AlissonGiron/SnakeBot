namespace SnakeBOT
{
    partial class Form1
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
            this.btnGerarGrafo = new System.Windows.Forms.Button();
            this.wbSnake = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            this.txtPos = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnGerarGrafo
            // 
            this.btnGerarGrafo.Location = new System.Drawing.Point(12, 12);
            this.btnGerarGrafo.Name = "btnGerarGrafo";
            this.btnGerarGrafo.Size = new System.Drawing.Size(155, 63);
            this.btnGerarGrafo.TabIndex = 1;
            this.btnGerarGrafo.Text = "Gerar Grafo";
            this.btnGerarGrafo.UseVisualStyleBackColor = true;
            this.btnGerarGrafo.Click += new System.EventHandler(this.btnGerarGrafo_Click);
            // 
            // wbSnake
            // 
            this.wbSnake.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSnake.Location = new System.Drawing.Point(0, 0);
            this.wbSnake.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSnake.Name = "wbSnake";
            this.wbSnake.Size = new System.Drawing.Size(801, 534);
            this.wbSnake.TabIndex = 2;
            this.wbSnake.Url = new System.Uri("https://www.google.com.br/search?q=snake&oq=snake&aqs=chrome..69i57j69i59j0l4.906" +
        "j0j7&sourceid=chrome&ie=UTF-8", System.UriKind.Absolute);
            this.wbSnake.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.wbSnake_PreviewKeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(173, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 63);
            this.button1.TabIndex = 3;
            this.button1.Text = "Fechar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPos
            // 
            this.txtPos.Location = new System.Drawing.Point(287, 13);
            this.txtPos.Name = "txtPos";
            this.txtPos.Size = new System.Drawing.Size(482, 20);
            this.txtPos.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(801, 534);
            this.Controls.Add(this.txtPos);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGerarGrafo);
            this.Controls.Add(this.wbSnake);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGerarGrafo;
        private System.Windows.Forms.WebBrowser wbSnake;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtPos;
    }
}

