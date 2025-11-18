namespace DMXControl
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TrackBar trbChannel1iwaa;
        private System.Windows.Forms.Label lblChannel1Value;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnToggleLight1;
        private System.Windows.Forms.Button btnToggleLight2;
        private System.Windows.Forms.Button btnToggleLight3;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.trbChannel1iwaa = new System.Windows.Forms.TrackBar();
            this.lblChannel1Value = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblChannel2Value = new System.Windows.Forms.Label();
            this.trbChannel2iwaa = new System.Windows.Forms.TrackBar();
            this.lblChannel3Value = new System.Windows.Forms.Label();
            this.trbChannel3iwaa = new System.Windows.Forms.TrackBar();
            this.btnToggleLight1 = new System.Windows.Forms.Button();
            this.btnToggleLight2 = new System.Windows.Forms.Button();
            this.btnToggleLight3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel1iwaa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel2iwaa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel3iwaa)).BeginInit();
            this.SuspendLayout();
            // 
            // trbChannel1iwaa
            // 
            this.trbChannel1iwaa.Location = new System.Drawing.Point(30, 70);
            this.trbChannel1iwaa.Maximum = 255;
            this.trbChannel1iwaa.Name = "trbChannel1iwaa";
            this.trbChannel1iwaa.Size = new System.Drawing.Size(300, 56);
            this.trbChannel1iwaa.TabIndex = 0;
            this.trbChannel1iwaa.TickFrequency = 25;
            this.trbChannel1iwaa.Scroll += new System.EventHandler(this.trbChannel1Iwaa_Scroll);
            // 
            // lblChannel1Value
            // 
            this.lblChannel1Value.AutoSize = true;
            this.lblChannel1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblChannel1Value.Location = new System.Drawing.Point(30, 115);
            this.lblChannel1Value.Name = "lblChannel1Value";
            this.lblChannel1Value.Size = new System.Drawing.Size(90, 18);
            this.lblChannel1Value.TabIndex = 1;
            this.lblChannel1Value.Text = "Channel 1: 0";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(30, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(157, 20);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "DMX Light Control";
            // 
            // btnToggleLight1
            // 
            this.btnToggleLight1.BackColor = System.Drawing.Color.LightGreen;
            this.btnToggleLight1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnToggleLight1.Location = new System.Drawing.Point(30, 40);
            this.btnToggleLight1.Name = "btnToggleLight1";
            this.btnToggleLight1.Size = new System.Drawing.Size(120, 30);
            this.btnToggleLight1.TabIndex = 3;
            this.btnToggleLight1.Text = "Light 1: ON";
            this.btnToggleLight1.UseVisualStyleBackColor = false;
            this.btnToggleLight1.Click += new System.EventHandler(this.btnToggleLight1_Click);
            // 
            // lblChannel2Value
            // 
            this.lblChannel2Value.AutoSize = true;
            this.lblChannel2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblChannel2Value.Location = new System.Drawing.Point(30, 224);
            this.lblChannel2Value.Name = "lblChannel2Value";
            this.lblChannel2Value.Size = new System.Drawing.Size(90, 18);
            this.lblChannel2Value.TabIndex = 5;
            this.lblChannel2Value.Text = "Channel 4: 0";
            // 
            // trbChannel2iwaa
            // 
            this.trbChannel2iwaa.Location = new System.Drawing.Point(30, 179);
            this.trbChannel2iwaa.Maximum = 255;
            this.trbChannel2iwaa.Name = "trbChannel2iwaa";
            this.trbChannel2iwaa.Size = new System.Drawing.Size(300, 56);
            this.trbChannel2iwaa.TabIndex = 4;
            this.trbChannel2iwaa.TickFrequency = 25;
            this.trbChannel2iwaa.Scroll += new System.EventHandler(this.trbChannel2_Scroll);
            // 
            // btnToggleLight2
            // 
            this.btnToggleLight2.BackColor = System.Drawing.Color.LightGreen;
            this.btnToggleLight2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnToggleLight2.Location = new System.Drawing.Point(30, 149);
            this.btnToggleLight2.Name = "btnToggleLight2";
            this.btnToggleLight2.Size = new System.Drawing.Size(120, 30);
            this.btnToggleLight2.TabIndex = 6;
            this.btnToggleLight2.Text = "Light 2: ON";
            this.btnToggleLight2.UseVisualStyleBackColor = false;
            this.btnToggleLight2.Click += new System.EventHandler(this.btnToggleLight2_Click);
            // 
            // lblChannel3Value
            // 
            this.lblChannel3Value.AutoSize = true;
            this.lblChannel3Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.lblChannel3Value.Location = new System.Drawing.Point(34, 329);
            this.lblChannel3Value.Name = "lblChannel3Value";
            this.lblChannel3Value.Size = new System.Drawing.Size(90, 18);
            this.lblChannel3Value.TabIndex = 8;
            this.lblChannel3Value.Text = "Channel 7: 0";
            // 
            // trbChannel3iwaa
            // 
            this.trbChannel3iwaa.Location = new System.Drawing.Point(34, 284);
            this.trbChannel3iwaa.Maximum = 255;
            this.trbChannel3iwaa.Name = "trbChannel3iwaa";
            this.trbChannel3iwaa.Size = new System.Drawing.Size(300, 56);
            this.trbChannel3iwaa.TabIndex = 7;
            this.trbChannel3iwaa.TickFrequency = 25;
            this.trbChannel3iwaa.Scroll += new System.EventHandler(this.trbChannel3iwaa_Scroll);
            // 
            // btnToggleLight3
            // 
            this.btnToggleLight3.BackColor = System.Drawing.Color.LightGreen;
            this.btnToggleLight3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnToggleLight3.Location = new System.Drawing.Point(34, 254);
            this.btnToggleLight3.Name = "btnToggleLight3";
            this.btnToggleLight3.Size = new System.Drawing.Size(120, 30);
            this.btnToggleLight3.TabIndex = 9;
            this.btnToggleLight3.Text = "Light 3: ON";
            this.btnToggleLight3.UseVisualStyleBackColor = false;
            this.btnToggleLight3.Click += new System.EventHandler(this.btnToggleLight3_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(380, 380);
            this.Controls.Add(this.btnToggleLight3);
            this.Controls.Add(this.lblChannel3Value);
            this.Controls.Add(this.trbChannel3iwaa);
            this.Controls.Add(this.btnToggleLight2);
            this.Controls.Add(this.lblChannel2Value);
            this.Controls.Add(this.trbChannel2iwaa);
            this.Controls.Add(this.btnToggleLight1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblChannel1Value);
            this.Controls.Add(this.trbChannel1iwaa);
            this.Name = "Form1";
            this.Text = "DMX Control - COM12";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel1iwaa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel2iwaa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbChannel3iwaa)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblChannel2Value;
        private System.Windows.Forms.TrackBar trbChannel2iwaa;
        private System.Windows.Forms.Label lblChannel3Value;
        private System.Windows.Forms.TrackBar trbChannel3iwaa;
    }
}