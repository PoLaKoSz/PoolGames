namespace SkypeSnookerUI
{
    partial class frmTable
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTable));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPlayer1 = new System.Windows.Forms.TextBox();
            this.txtPlayer2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pnlTable = new SkypeSnookerUI.AntiFlickerPanel();
            this.pnlBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackgroundImage = global::SkypeSnookerUI.Properties.Resources.snookertable;
            this.pnlBorder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlBorder.Controls.Add(this.pnlTable);
            this.pnlBorder.Location = new System.Drawing.Point(15, 18);
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size(640, 377);
            this.pnlBorder.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Black;
            this.progressBar1.Location = new System.Drawing.Point(15, 401);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(640, 27);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Value = 50;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(663, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 43);
            this.label1.TabIndex = 3;
            this.label1.Text = "Player 1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtPlayer1
            // 
            this.txtPlayer1.BackColor = System.Drawing.Color.Black;
            this.txtPlayer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPlayer1.Enabled = false;
            this.txtPlayer1.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayer1.ForeColor = System.Drawing.Color.Yellow;
            this.txtPlayer1.Location = new System.Drawing.Point(666, 75);
            this.txtPlayer1.Name = "txtPlayer1";
            this.txtPlayer1.Size = new System.Drawing.Size(118, 44);
            this.txtPlayer1.TabIndex = 4;
            this.txtPlayer1.Text = "0";
            this.txtPlayer1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtPlayer2
            // 
            this.txtPlayer2.BackColor = System.Drawing.Color.Black;
            this.txtPlayer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPlayer2.Enabled = false;
            this.txtPlayer2.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayer2.ForeColor = System.Drawing.Color.Yellow;
            this.txtPlayer2.Location = new System.Drawing.Point(666, 168);
            this.txtPlayer2.Name = "txtPlayer2";
            this.txtPlayer2.Size = new System.Drawing.Size(118, 44);
            this.txtPlayer2.TabIndex = 6;
            this.txtPlayer2.Text = "0";
            this.txtPlayer2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(663, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 43);
            this.label2.TabIndex = 5;
            this.label2.Text = "Player 2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(666, 219);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(118, 199);
            this.listBox1.TabIndex = 7;
            // 
            // pnlTable
            // 
            this.pnlTable.BackColor = System.Drawing.Color.Green;
            this.pnlTable.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlTable.BackgroundImage")));
            this.pnlTable.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.pnlTable.Location = new System.Drawing.Point(16, 16);
            this.pnlTable.Name = "pnlTable";
            this.pnlTable.Size = new System.Drawing.Size(606, 344);
            this.pnlTable.TabIndex = 0;
            this.pnlTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTable_MouseDown);
            this.pnlTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlTable_MouseUp);
            // 
            // frmTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(792, 434);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txtPlayer2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPlayer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pnlBorder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "frmTable";
            this.Text = "Skype Snooker";
            this.Deactivate += new System.EventHandler(this.frmTable_Deactivate);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmTable_Paint);
            this.Activated += new System.EventHandler(this.frmTable_Activated);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTable_KeyDown);
            this.pnlBorder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AntiFlickerPanel pnlTable;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlBorder;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPlayer1;
        private System.Windows.Forms.TextBox txtPlayer2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox1;
    }
}

