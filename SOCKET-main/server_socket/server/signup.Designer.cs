namespace server
{
	partial class signup
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
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.signbtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(350, 233);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(100, 21);
			this.textBox3.TabIndex = 18;
			this.textBox3.Text = "PW";
		
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(350, 194);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 21);
			this.textBox2.TabIndex = 17;
			this.textBox2.Text = "ID";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(350, 159);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 21);
			this.textBox1.TabIndex = 16;
			this.textBox1.Text = "이름";
			// 
			// signbtn
			// 
			this.signbtn.Location = new System.Drawing.Point(350, 270);
			this.signbtn.Name = "signbtn";
			this.signbtn.Size = new System.Drawing.Size(100, 73);
			this.signbtn.TabIndex = 20;
			this.signbtn.Text = "가입하기";
			this.signbtn.UseVisualStyleBackColor = true;
			this.signbtn.Click += new System.EventHandler(this.signbtn_Click);
			// 
			// signup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.signbtn);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Name = "signup";
			this.Text = "회원가입";
			this.Load += new System.EventHandler(this.signup_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button signbtn;
	}
}