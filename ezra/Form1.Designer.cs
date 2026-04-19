namespace ezra
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pnllogo = new Panel();
            pictureBox1 = new PictureBox();
            flpchat = new FlowLayoutPanel();
            flpbuttons = new FlowLayoutPanel();
            btnplay = new Button();
            btnmuzic = new Button();
            btnclock = new Button();
            pnlinput = new Panel();
            textBox = new TextBox();
            btnsend = new Button();
            pnllogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            flpbuttons.SuspendLayout();
            pnlinput.SuspendLayout();
            SuspendLayout();
            // 
            // pnllogo
            // 
            pnllogo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnllogo.BackColor = Color.FromArgb(25, 25, 35);
            pnllogo.Controls.Add(pictureBox1);
            pnllogo.Location = new Point(0, 0);
            pnllogo.Name = "pnllogo";
            pnllogo.RightToLeft = RightToLeft.Yes;
            pnllogo.Size = new Size(443, 90);
            pnllogo.TabIndex = 3;
            pnllogo.Paint += pnllogo_Paint;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(174, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(95, 80);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // flpchat
            // 
            flpchat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpchat.AutoScroll = true;
            flpchat.BackColor = Color.FromArgb(35, 35, 48);
            flpchat.FlowDirection = FlowDirection.TopDown;
            flpchat.Location = new Point(0, 90);
            flpchat.Name = "flpchat";
            flpchat.Padding = new Padding(10);
            flpchat.RightToLeft = RightToLeft.No;
            flpchat.Size = new Size(443, 330);
            flpchat.TabIndex = 5;
            flpchat.WrapContents = false;
            // 
            // flpbuttons
            // 
            flpbuttons.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpbuttons.BackColor = Color.FromArgb(25, 25, 35);
            flpbuttons.Controls.Add(btnplay);
            flpbuttons.Controls.Add(btnmuzic);
            flpbuttons.Controls.Add(btnclock);
            flpbuttons.Location = new Point(0, 420);
            flpbuttons.Name = "flpbuttons";
            flpbuttons.Padding = new Padding(5);
            flpbuttons.RightToLeft = RightToLeft.Yes;
            flpbuttons.Size = new Size(443, 80);
            flpbuttons.TabIndex = 6;
            // 
            // btnplay
            // 
            btnplay.BackColor = Color.FromArgb(76, 175, 255);
            btnplay.FlatAppearance.BorderSize = 0;
            btnplay.FlatStyle = FlatStyle.Flat;
            btnplay.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnplay.ForeColor = Color.White;
            btnplay.Location = new Point(330, 8);
            btnplay.Name = "btnplay";
            btnplay.Size = new Size(100, 35);
            btnplay.TabIndex = 0;
            btnplay.Text = "🎮 משחקים";
            btnplay.UseVisualStyleBackColor = false;
            btnplay.Click += btnplay_Click;
            btnplay.MouseEnter += Button_MouseEnter;
            btnplay.MouseLeave += Button_MouseLeave;
            // 
            // btnmuzic
            // 
            btnmuzic.BackColor = Color.FromArgb(165, 105, 255);
            btnmuzic.FlatAppearance.BorderSize = 0;
            btnmuzic.FlatStyle = FlatStyle.Flat;
            btnmuzic.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnmuzic.ForeColor = Color.White;
            btnmuzic.Location = new Point(224, 8);
            btnmuzic.Name = "btnmuzic";
            btnmuzic.Size = new Size(100, 35);
            btnmuzic.TabIndex = 1;
            btnmuzic.Text = "🎵 מוזיקה";
            btnmuzic.UseVisualStyleBackColor = false;
            btnmuzic.MouseEnter += Button_MouseEnter;
            btnmuzic.MouseLeave += Button_MouseLeave;
            // 
            // btnclock
            // 
            btnclock.BackColor = Color.FromArgb(76, 220, 110);
            btnclock.FlatAppearance.BorderSize = 0;
            btnclock.FlatStyle = FlatStyle.Flat;
            btnclock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnclock.ForeColor = Color.White;
            btnclock.Location = new Point(118, 8);
            btnclock.Name = "btnclock";
            btnclock.Size = new Size(100, 35);
            btnclock.TabIndex = 2;
            btnclock.Text = "⏰ שעון";
            btnclock.UseVisualStyleBackColor = false;
            btnclock.Click += btnclock_Click;
            btnclock.MouseEnter += Button_MouseEnter;
            btnclock.MouseLeave += Button_MouseLeave;
            // 
            // pnlinput
            // 
            pnlinput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlinput.BackColor = Color.FromArgb(45, 45, 60);
            pnlinput.Controls.Add(textBox);
            pnlinput.Controls.Add(btnsend);
            pnlinput.Location = new Point(0, 500);
            pnlinput.Name = "pnlinput";
            pnlinput.RightToLeft = RightToLeft.Yes;
            pnlinput.Size = new Size(443, 75);
            pnlinput.TabIndex = 7;
            // 
            // textBox
            // 
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox.BackColor = Color.FromArgb(60, 60, 80);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.ForeColor = Color.White;
            textBox.Location = new Point(70, 20);
            textBox.Name = "textBox";
            textBox.Size = new Size(360, 27);
            textBox.TabIndex = 1;
            // 
            // btnsend
            // 
            btnsend.BackColor = Color.FromArgb(255, 107, 107);
            btnsend.FlatAppearance.BorderSize = 0;
            btnsend.FlatStyle = FlatStyle.Flat;
            btnsend.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnsend.ForeColor = Color.White;
            btnsend.Location = new Point(10, 10);
            btnsend.Name = "btnsend";
            btnsend.Size = new Size(50, 50);
            btnsend.TabIndex = 0;
            btnsend.Text = "➤";
            btnsend.UseVisualStyleBackColor = false;
            btnsend.Click += button1_Click;
            btnsend.MouseEnter += Button_MouseEnter;
            btnsend.MouseLeave += Button_MouseLeave;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(25, 25, 35);
            ClientSize = new Size(443, 575);
            Controls.Add(pnlinput);
            Controls.Add(flpbuttons);
            Controls.Add(flpchat);
            Controls.Add(pnllogo);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "עזרא - AI Assistant";
            Load += Form1_Load;
            pnllogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            flpbuttons.ResumeLayout(false);
            pnlinput.ResumeLayout(false);
            pnlinput.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel pnllogo;
        private FlowLayoutPanel flpchat;
        private FlowLayoutPanel flpbuttons;
        private Panel pnlinput;
        private PictureBox pictureBox1;
        private Button btnplay;
        private Button btnmuzic;
        private Button btnclock;
        private TextBox textBox;
        private Button btnsend;
    }
}
