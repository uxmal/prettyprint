namespace pp_win
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDeviceWidth = new System.Windows.Forms.TextBox();
            this.txtIndentWidth = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSrc
            // 
            this.txtSrc.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSrc.Location = new System.Drawing.Point(30, 125);
            this.txtSrc.Multiline = true;
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(291, 289);
            this.txtSrc.TabIndex = 0;
            this.txtSrc.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtOutput.Location = new System.Drawing.Point(350, 125);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(423, 289);
            this.txtOutput.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Output width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Tab width:";
            // 
            // txtDeviceWidth
            // 
            this.txtDeviceWidth.Location = new System.Drawing.Point(129, 4);
            this.txtDeviceWidth.Name = "txtDeviceWidth";
            this.txtDeviceWidth.Size = new System.Drawing.Size(100, 23);
            this.txtDeviceWidth.TabIndex = 4;
            this.txtDeviceWidth.Text = "12";
            this.txtDeviceWidth.TextChanged += new System.EventHandler(this.txtDeviceWidth_TextChanged);
            // 
            // txtIndentWidth
            // 
            this.txtIndentWidth.Location = new System.Drawing.Point(129, 37);
            this.txtIndentWidth.Name = "txtIndentWidth";
            this.txtIndentWidth.Size = new System.Drawing.Size(100, 23);
            this.txtIndentWidth.TabIndex = 5;
            this.txtIndentWidth.Text = "3";
            this.txtIndentWidth.TextChanged += new System.EventHandler(this.txtIndentWidth_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtIndentWidth);
            this.Controls.Add(this.txtDeviceWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtSrc);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDeviceWidth;
        private System.Windows.Forms.TextBox txtIndentWidth;
    }
}

