namespace ZxXpFly_WiFI
{
    partial class passwordName
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
            components = new System.ComponentModel.Container();
            ConnectWifi = new Button();
            labelWifi = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            textBox1 = new TextBox();
            password = new Label();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // ConnectWifi
            // 
            ConnectWifi.Location = new Point(291, 25);
            ConnectWifi.Name = "ConnectWifi";
            ConnectWifi.Size = new Size(75, 23);
            ConnectWifi.TabIndex = 0;
            ConnectWifi.Text = "开始破解";
            ConnectWifi.UseVisualStyleBackColor = true;
            ConnectWifi.Click += ConnectWifi_Click;
            // 
            // labelWifi
            // 
            labelWifi.AutoSize = true;
            labelWifi.Location = new Point(40, 28);
            labelWifi.Name = "labelWifi";
            labelWifi.Size = new Size(73, 17);
            labelWifi.TabIndex = 4;
            labelWifi.Text = "Wi-Fi名称：";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(40, 92);
            label1.Name = "label1";
            label1.Size = new Size(63, 17);
            label1.TabIndex = 5;
            label1.Text = "wifi密码：";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(104, 25);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(181, 23);
            textBox1.TabIndex = 6;
            // 
            // password
            // 
            password.AutoSize = true;
            password.Location = new Point(104, 92);
            password.Name = "password";
            password.Size = new Size(32, 17);
            password.TabIndex = 7;
            password.Text = "密码";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(40, 64);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 8;
            label2.Text = "当前尝试：";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(104, 64);
            label3.Name = "label3";
            label3.Size = new Size(32, 17);
            label3.TabIndex = 9;
            label3.Text = "密码";
            // 
            // passwordName
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 118);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(password);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(labelWifi);
            Controls.Add(ConnectWifi);
            Name = "passwordName";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ConnectWifi;
        private Label labelWifi;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private TextBox textBox1;
        private Label password;
        private Label label2;
        private Label label3;
    }
}
