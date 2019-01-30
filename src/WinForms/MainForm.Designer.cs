namespace CSharpSnooker.WinForms
{
    partial class MainForm
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
            this.timerBallOn = new System.Windows.Forms.Timer(this.components);
            this.timerInBox = new System.Windows.Forms.Timer(this.components);
            this.lblPlayer1Name = new System.Windows.Forms.Label();
            this.lblPlayer2Name = new System.Windows.Forms.Label();
            this.picTable = new System.Windows.Forms.PictureBox();
            this.lblPlayer1 = new System.Windows.Forms.Label();
            this.lblPlayer2 = new System.Windows.Forms.Label();
            this.lblStrenght = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.picBallOn = new System.Windows.Forms.PictureBox();
            this.timerSplash = new System.Windows.Forms.Timer(this.components);
            this.lblCSharpSnooker = new System.Windows.Forms.Label();
            this.lblWin = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBallOn)).BeginInit();
            this.SuspendLayout();
            // 
            // timerBallOn
            // 
            this.timerBallOn.Enabled = true;
            this.timerBallOn.Interval = 500;
            this.timerBallOn.Tick += new System.EventHandler(this.BallOnTimer_Tick);
            // 
            // timerInBox
            // 
            this.timerInBox.Enabled = true;
            // 
            // lblPlayer1Name
            // 
            this.lblPlayer1Name.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayer1Name.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayer1Name.ForeColor = System.Drawing.Color.Black;
            this.lblPlayer1Name.Location = new System.Drawing.Point(60, 94);
            this.lblPlayer1Name.Name = "lblPlayer1Name";
            this.lblPlayer1Name.Size = new System.Drawing.Size(75, 21);
            this.lblPlayer1Name.TabIndex = 3;
            this.lblPlayer1Name.Text = "lblPlayerName1";
            // 
            // lblPlayer2Name
            // 
            this.lblPlayer2Name.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayer2Name.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayer2Name.ForeColor = System.Drawing.Color.Black;
            this.lblPlayer2Name.Location = new System.Drawing.Point(60, 150);
            this.lblPlayer2Name.Name = "lblPlayer2Name";
            this.lblPlayer2Name.Size = new System.Drawing.Size(75, 18);
            this.lblPlayer2Name.TabIndex = 5;
            this.lblPlayer2Name.Text = "lblPlayerName2";
            // 
            // picTable
            // 
            this.picTable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picTable.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picTable.Image = global::CSharpSnooker.WinForms.Properties.Resources.tableBlue1;
            this.picTable.Location = new System.Drawing.Point(240, 26);
            this.picTable.Name = "picTable";
            this.picTable.Size = new System.Drawing.Size(603, 342);
            this.picTable.TabIndex = 1;
            this.picTable.TabStop = false;
            this.picTable.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PoolTable_MouseMove);
            this.picTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PoolTable_MouseUp);
            // 
            // lblPlayer1
            // 
            this.lblPlayer1.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayer1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayer1.ForeColor = System.Drawing.Color.Black;
            this.lblPlayer1.Location = new System.Drawing.Point(134, 94);
            this.lblPlayer1.Name = "lblPlayer1";
            this.lblPlayer1.Size = new System.Drawing.Size(41, 17);
            this.lblPlayer1.TabIndex = 8;
            this.lblPlayer1.Text = "0";
            this.lblPlayer1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblPlayer2
            // 
            this.lblPlayer2.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayer2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayer2.ForeColor = System.Drawing.Color.Black;
            this.lblPlayer2.Location = new System.Drawing.Point(134, 150);
            this.lblPlayer2.Name = "lblPlayer2";
            this.lblPlayer2.Size = new System.Drawing.Size(41, 14);
            this.lblPlayer2.TabIndex = 9;
            this.lblPlayer2.Text = "0";
            this.lblPlayer2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblStrenght
            // 
            this.lblStrenght.BackColor = System.Drawing.Color.Red;
            this.lblStrenght.ForeColor = System.Drawing.Color.Red;
            this.lblStrenght.Location = new System.Drawing.Point(37, 253);
            this.lblStrenght.Name = "lblStrenght";
            this.lblStrenght.Size = new System.Drawing.Size(140, 3);
            this.lblStrenght.TabIndex = 10;
            this.lblStrenght.Text = "label3";
            this.lblStrenght.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblStrenght_MouseDown);
            // 
            // lblTarget
            // 
            this.lblTarget.BackColor = System.Drawing.Color.Red;
            this.lblTarget.ForeColor = System.Drawing.Color.Red;
            this.lblTarget.Location = new System.Drawing.Point(100, 301);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(7, 7);
            this.lblTarget.TabIndex = 11;
            this.lblTarget.Text = "label3";
            // 
            // picBallOn
            // 
            this.picBallOn.BackgroundImage = global::CSharpSnooker.WinForms.Properties.Resources.RedBall;
            this.picBallOn.Location = new System.Drawing.Point(41, 93);
            this.picBallOn.Name = "picBallOn";
            this.picBallOn.Size = new System.Drawing.Size(16, 16);
            this.picBallOn.TabIndex = 12;
            this.picBallOn.TabStop = false;
            this.picBallOn.Visible = false;
            // 
            // lblCSharpSnooker
            // 
            this.lblCSharpSnooker.AutoSize = true;
            this.lblCSharpSnooker.BackColor = System.Drawing.Color.Transparent;
            this.lblCSharpSnooker.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCSharpSnooker.ForeColor = System.Drawing.Color.White;
            this.lblCSharpSnooker.Location = new System.Drawing.Point(437, 162);
            this.lblCSharpSnooker.Name = "lblCSharpSnooker";
            this.lblCSharpSnooker.Size = new System.Drawing.Size(208, 49);
            this.lblCSharpSnooker.TabIndex = 16;
            this.lblCSharpSnooker.Text = "C# Snooker";
            this.lblCSharpSnooker.Visible = false;
            // 
            // lblWin
            // 
            this.lblWin.BackColor = System.Drawing.Color.Transparent;
            this.lblWin.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWin.ForeColor = System.Drawing.Color.White;
            this.lblWin.Location = new System.Drawing.Point(307, 175);
            this.lblWin.Name = "lblWin";
            this.lblWin.Size = new System.Drawing.Size(472, 41);
            this.lblWin.TabIndex = 18;
            this.lblWin.Text = "Player 1 Wins !!!";
            this.lblWin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblWin.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::CSharpSnooker.WinForms.Properties.Resources.score_wallpaper;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(868, 394);
            this.Controls.Add(this.lblWin);
            this.Controls.Add(this.lblCSharpSnooker);
            this.Controls.Add(this.picBallOn);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblStrenght);
            this.Controls.Add(this.lblPlayer2);
            this.Controls.Add(this.lblPlayer1);
            this.Controls.Add(this.picTable);
            this.Controls.Add(this.lblPlayer2Name);
            this.Controls.Add(this.lblPlayer1Name);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "C# Snooker";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Window_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Window_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.picTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBallOn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerBallOn;
        private System.Windows.Forms.Timer timerInBox;
        private System.Windows.Forms.Label lblPlayer1Name;
        private System.Windows.Forms.Label lblPlayer2Name;
        public System.Windows.Forms.PictureBox picTable;
        private System.Windows.Forms.Label lblPlayer1;
        private System.Windows.Forms.Label lblPlayer2;
        private System.Windows.Forms.Label lblStrenght;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.PictureBox picBallOn;
        private System.Windows.Forms.Timer timerSplash;
        private System.Windows.Forms.Label lblCSharpSnooker;
        private System.Windows.Forms.Label lblWin;
    }
}

