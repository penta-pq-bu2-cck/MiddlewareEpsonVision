namespace MiddlewareEpsonVision
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Point = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.U = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.V = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.W = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtBox_mmPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBox_mmCommand = new System.Windows.Forms.TextBox();
            this.txtBox_mmIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.changelogDisplay = new System.Windows.Forms.RichTextBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 378);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 38);
            this.button1.TabIndex = 0;
            this.button1.Text = "Manual_Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(776, 366);
            this.textBox1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main Page";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(657, 378);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(125, 38);
            this.button2.TabIndex = 2;
            this.button2.Text = "MechMind_Trigger";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 424);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Points";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Point,
            this.X,
            this.Y,
            this.Z,
            this.U,
            this.V,
            this.W});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(786, 418);
            this.dataGridView1.TabIndex = 0;
            // 
            // Point
            // 
            this.Point.HeaderText = "Point";
            this.Point.Name = "Point";
            this.Point.ReadOnly = true;
            // 
            // X
            // 
            this.X.HeaderText = "X";
            this.X.Name = "X";
            this.X.ReadOnly = true;
            // 
            // Y
            // 
            this.Y.HeaderText = "Y";
            this.Y.Name = "Y";
            this.Y.ReadOnly = true;
            // 
            // Z
            // 
            this.Z.HeaderText = "Z";
            this.Z.Name = "Z";
            this.Z.ReadOnly = true;
            // 
            // U
            // 
            this.U.HeaderText = "U";
            this.U.Name = "U";
            this.U.ReadOnly = true;
            // 
            // V
            // 
            this.V.HeaderText = "V";
            this.V.Name = "V";
            this.V.ReadOnly = true;
            // 
            // W
            // 
            this.W.HeaderText = "W";
            this.W.Name = "W";
            this.W.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.txtBox_mmPort);
            this.tabPage3.Controls.Add(this.label3);
            this.tabPage3.Controls.Add(this.txtBox_mmCommand);
            this.tabPage3.Controls.Add(this.txtBox_mmIP);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(792, 424);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Setting";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtBox_mmPort
            // 
            this.txtBox_mmPort.Location = new System.Drawing.Point(332, 37);
            this.txtBox_mmPort.Name = "txtBox_mmPort";
            this.txtBox_mmPort.Size = new System.Drawing.Size(124, 20);
            this.txtBox_mmPort.TabIndex = 5;
            this.txtBox_mmPort.Text = "50000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Port";
            // 
            // txtBox_mmCommand
            // 
            this.txtBox_mmCommand.Location = new System.Drawing.Point(161, 101);
            this.txtBox_mmCommand.Name = "txtBox_mmCommand";
            this.txtBox_mmCommand.Size = new System.Drawing.Size(256, 20);
            this.txtBox_mmCommand.TabIndex = 3;
            this.txtBox_mmCommand.Text = "200,0,0,1,-12.649762,-8.378935,-36.662,-0.00,-44.95,194.41,0.008,0.430,0.0366,-88" +
    ".25,0.00,-180";
            // 
            // txtBox_mmIP
            // 
            this.txtBox_mmIP.Location = new System.Drawing.Point(161, 37);
            this.txtBox_mmIP.Name = "txtBox_mmIP";
            this.txtBox_mmIP.Size = new System.Drawing.Size(124, 20);
            this.txtBox_mmIP.TabIndex = 2;
            this.txtBox_mmIP.Text = "192.168.0.88";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(89, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Command";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MechMind IP Address";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.changelogDisplay);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(792, 424);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Version";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // changelogDisplay
            // 
            this.changelogDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changelogDisplay.Location = new System.Drawing.Point(3, 3);
            this.changelogDisplay.Name = "changelogDisplay";
            this.changelogDisplay.Size = new System.Drawing.Size(786, 418);
            this.changelogDisplay.TabIndex = 0;
            this.changelogDisplay.Text = "";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Mindmech > Epson";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Point;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn Z;
        private System.Windows.Forms.DataGridViewTextBoxColumn U;
        private System.Windows.Forms.DataGridViewTextBoxColumn V;
        private System.Windows.Forms.DataGridViewTextBoxColumn W;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox txtBox_mmCommand;
        private System.Windows.Forms.TextBox txtBox_mmIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBox_mmPort;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox changelogDisplay;
    }
}

