namespace WindowsFormsApplication1
{
    partial class frmMain
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
            this.room1 = new WindowsFormsApplication1.Room();
            this.room2 = new WindowsFormsApplication1.Room();
            this.room3 = new WindowsFormsApplication1.Room();
            this.room4 = new WindowsFormsApplication1.Room();
            this.room5 = new WindowsFormsApplication1.Room();
            this.room6 = new WindowsFormsApplication1.Room();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxUsers = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // room1
            // 
            this.room1.Location = new System.Drawing.Point(3, 5);
            this.room1.Name = "room1";
            this.room1.Size = new System.Drawing.Size(197, 121);
            this.room1.TabIndex = 0;
            this.room1.Tag = "1";
            // 
            // room2
            // 
            this.room2.Location = new System.Drawing.Point(206, 5);
            this.room2.Name = "room2";
            this.room2.Size = new System.Drawing.Size(197, 121);
            this.room2.TabIndex = 1;
            this.room2.Tag = "2";
            // 
            // room3
            // 
            this.room3.Location = new System.Drawing.Point(3, 132);
            this.room3.Name = "room3";
            this.room3.Size = new System.Drawing.Size(197, 121);
            this.room3.TabIndex = 2;
            this.room3.Tag = "3";
            // 
            // room4
            // 
            this.room4.Location = new System.Drawing.Point(206, 132);
            this.room4.Name = "room4";
            this.room4.Size = new System.Drawing.Size(197, 121);
            this.room4.TabIndex = 3;
            this.room4.Tag = "4";
            // 
            // room5
            // 
            this.room5.Location = new System.Drawing.Point(3, 259);
            this.room5.Name = "room5";
            this.room5.Size = new System.Drawing.Size(197, 121);
            this.room5.TabIndex = 4;
            this.room5.Tag = "5";
            // 
            // room6
            // 
            this.room6.Location = new System.Drawing.Point(206, 259);
            this.room6.Name = "room6";
            this.room6.Size = new System.Drawing.Size(197, 121);
            this.room6.TabIndex = 5;
            this.room6.Tag = "6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(466, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Users";
            // 
            // listBoxUsers
            // 
            this.listBoxUsers.FormattingEnabled = true;
            this.listBoxUsers.Location = new System.Drawing.Point(429, 33);
            this.listBoxUsers.Name = "listBoxUsers";
            this.listBoxUsers.Size = new System.Drawing.Size(120, 355);
            this.listBoxUsers.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.room1);
            this.panel1.Controls.Add(this.room2);
            this.panel1.Controls.Add(this.room3);
            this.panel1.Controls.Add(this.room6);
            this.panel1.Controls.Add(this.room4);
            this.panel1.Controls.Add(this.room5);
            this.panel1.Location = new System.Drawing.Point(555, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(405, 382);
            this.panel1.TabIndex = 8;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 33);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(411, 329);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 368);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(330, 20);
            this.textBox1.TabIndex = 10;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(348, 366);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 11;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Chat Lobby";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 494);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listBoxUsers);
            this.Controls.Add(this.label1);
            this.Name = "frmMain";
            this.Text = "Main";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Room room1;
        private Room room2;
        private Room room3;
        private Room room4;
        private Room room5;
        private Room room6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxUsers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Label label2;
    }
}