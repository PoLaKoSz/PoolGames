namespace CSharpSnookerUI
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
            this.timerBallOn = new System.Windows.Forms.Timer(this.components);
            this.timerInBox = new System.Windows.Forms.Timer(this.components);
            this.lblPlayerName1 = new System.Windows.Forms.Label();
            this.rbt2Player = new System.Windows.Forms.Label();
            this.picTable = new System.Windows.Forms.PictureBox();
            this.lblPlayer1 = new System.Windows.Forms.Label();
            this.lblPlayer2 = new System.Windows.Forms.Label();
            this.lblStrenght = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.picBallOn = new System.Windows.Forms.PictureBox();
            this.timerComputer = new System.Windows.Forms.Timer(this.components);
            this.timerSplash = new System.Windows.Forms.Timer(this.components);
            this.lblCSharpSnooker = new System.Windows.Forms.Label();
            this.lblWin = new System.Windows.Forms.Label();
            this.pnlSplash = new CSharpSnookerUI.AntiFlickerPanel();
            this.pnlPlayers = new CSharpSnookerUI.AntiFlickerPanel();
            this.btnOkPlayers = new System.Windows.Forms.Button();
            this.rbt2Machine = new System.Windows.Forms.RadioButton();
            this.rbt1Player = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.picTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBallOn)).BeginInit();
            this.pnlSplash.SuspendLayout();
            this.pnlPlayers.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerBallOn
            // 
            this.timerBallOn.Enabled = true;
            this.timerBallOn.Interval = 500;
            this.timerBallOn.Tick += new System.EventHandler(this.timerBallOn_Tick);
            // 
            // timerInBox
            // 
            this.timerInBox.Enabled = true;
            // 
            // lblPlayerName1
            // 
            this.lblPlayerName1.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayerName1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerName1.ForeColor = System.Drawing.Color.Black;
            this.lblPlayerName1.Location = new System.Drawing.Point(60, 94);
            this.lblPlayerName1.Name = "lblPlayerName1";
            this.lblPlayerName1.Size = new System.Drawing.Size(75, 21);
            this.lblPlayerName1.TabIndex = 3;
            this.lblPlayerName1.Text = "lblPlayerName1";
            // 
            // rbt2Player
            // 
            this.rbt2Player.BackColor = System.Drawing.Color.Transparent;
            this.rbt2Player.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbt2Player.ForeColor = System.Drawing.Color.Black;
            this.rbt2Player.Location = new System.Drawing.Point(60, 150);
            this.rbt2Player.Name = "rbt2Player";
            this.rbt2Player.Size = new System.Drawing.Size(75, 18);
            this.rbt2Player.TabIndex = 5;
            this.rbt2Player.Text = "lblPlayerName2";
            // 
            // picTable
            // 
            this.picTable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picTable.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.picTable.Image = global::CSharpSnookerUI.Properties.Resources.tableBlue1;
            this.picTable.Location = new System.Drawing.Point(240, 26);
            this.picTable.Name = "picTable";
            this.picTable.Size = new System.Drawing.Size(603, 342);
            this.picTable.TabIndex = 1;
            this.picTable.TabStop = false;
            this.picTable.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTable_MouseMove);
            this.picTable.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTable_MouseDown);
            this.picTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTable_MouseUp);
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
            this.picBallOn.BackgroundImage = global::CSharpSnookerUI.Properties.Resources.RedBall;
            this.picBallOn.Location = new System.Drawing.Point(41, 93);
            this.picBallOn.Name = "picBallOn";
            this.picBallOn.Size = new System.Drawing.Size(16, 16);
            this.picBallOn.TabIndex = 12;
            this.picBallOn.TabStop = false;
            this.picBallOn.Visible = false;
            // 
            // timerComputer
            // 
            this.timerComputer.Interval = 1000;
            this.timerComputer.Tick += new System.EventHandler(this.timerComputer_Tick);
            // 
            // lblCSharpSnooker
            // 
            this.lblCSharpSnooker.AutoSize = true;
            this.lblCSharpSnooker.BackColor = System.Drawing.Color.Transparent;
            this.lblCSharpSnooker.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCSharpSnooker.ForeColor = System.Drawing.Color.White;
            this.lblCSharpSnooker.Location = new System.Drawing.Point(437, 162);
            this.lblCSharpSnooker.Name = "lblCSharpSnooker";
            this.lblCSharpSnooker.Size = new System.Drawing.Size(209, 49);
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
            // pnlSplash
            // 
            this.pnlSplash.BackgroundImage = global::CSharpSnookerUI.Properties.Resources.metal5;
            this.pnlSplash.Controls.Add(this.pnlPlayers);
            this.pnlSplash.Location = new System.Drawing.Point(12, 386);
            this.pnlSplash.Name = "pnlSplash";
            this.pnlSplash.Size = new System.Drawing.Size(302, 254);
            this.pnlSplash.TabIndex = 15;
            // 
            // pnlPlayers
            // 
            this.pnlPlayers.BackColor = System.Drawing.Color.Transparent;
            this.pnlPlayers.Controls.Add(this.btnOkPlayers);
            this.pnlPlayers.Controls.Add(this.rbt2Machine);
            this.pnlPlayers.Controls.Add(this.rbt1Player);
            this.pnlPlayers.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold);
            this.pnlPlayers.Location = new System.Drawing.Point(12, 13);
            this.pnlPlayers.Name = "pnlPlayers";
            this.pnlPlayers.Size = new System.Drawing.Size(105, 80);
            this.pnlPlayers.TabIndex = 0;
            // 
            // btnOkPlayers
            // 
            this.btnOkPlayers.Location = new System.Drawing.Point(24, 52);
            this.btnOkPlayers.Name = "btnOkPlayers";
            this.btnOkPlayers.Size = new System.Drawing.Size(62, 23);
            this.btnOkPlayers.TabIndex = 2;
            this.btnOkPlayers.Text = "&Ok";
            this.btnOkPlayers.UseVisualStyleBackColor = true;
            this.btnOkPlayers.Click += new System.EventHandler(this.btnOkPlayers_Click);
            // 
            // rbt2Machine
            // 
            this.rbt2Machine.AutoSize = true;
            this.rbt2Machine.Location = new System.Drawing.Point(5, 25);
            this.rbt2Machine.Name = "rbt2Machine";
            this.rbt2Machine.Size = new System.Drawing.Size(81, 22);
            this.rbt2Machine.TabIndex = 1;
            this.rbt2Machine.Text = "&2 Players";
            this.rbt2Machine.UseVisualStyleBackColor = true;
            this.rbt2Machine.CheckedChanged += new System.EventHandler(this.rbt2Machine_CheckedChanged);
            // 
            // rbt1Player
            // 
            this.rbt1Player.AutoSize = true;
            this.rbt1Player.Checked = true;
            this.rbt1Player.Location = new System.Drawing.Point(5, 5);
            this.rbt1Player.Name = "rbt1Player";
            this.rbt1Player.Size = new System.Drawing.Size(75, 22);
            this.rbt1Player.TabIndex = 0;
            this.rbt1Player.TabStop = true;
            this.rbt1Player.Text = "&1 Player";
            this.rbt1Player.UseVisualStyleBackColor = true;
            // 
            // frmTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::CSharpSnookerUI.Properties.Resources.score_wallpaper;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(868, 394);
            this.Controls.Add(this.lblWin);
            this.Controls.Add(this.pnlSplash);
            this.Controls.Add(this.lblCSharpSnooker);
            this.Controls.Add(this.picBallOn);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblStrenght);
            this.Controls.Add(this.lblPlayer2);
            this.Controls.Add(this.lblPlayer1);
            this.Controls.Add(this.picTable);
            this.Controls.Add(this.rbt2Player);
            this.Controls.Add(this.lblPlayerName1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTable";
            this.Text = "C# Snooker";
            this.Deactivate += new System.EventHandler(this.frmTable_Deactivate);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmTable_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmTable_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.picTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBallOn)).EndInit();
            this.pnlSplash.ResumeLayout(false);
            this.pnlPlayers.ResumeLayout(false);
            this.pnlPlayers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerBallOn;
        private System.Windows.Forms.Timer timerInBox;
        private System.Windows.Forms.Label lblPlayerName1;
        private System.Windows.Forms.Label rbt2Player;
        private System.Windows.Forms.PictureBox picTable;
        private System.Windows.Forms.Label lblPlayer1;
        private System.Windows.Forms.Label lblPlayer2;
        private System.Windows.Forms.Label lblStrenght;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.PictureBox picBallOn;
        private AntiFlickerPanel pnlSplash;
        private AntiFlickerPanel pnlPlayers;
        private System.Windows.Forms.RadioButton rbt2Machine;
        private System.Windows.Forms.RadioButton rbt1Player;
        private System.Windows.Forms.Button btnOkPlayers;
        private System.Windows.Forms.Timer timerComputer;
        private System.Windows.Forms.Timer timerSplash;
        private System.Windows.Forms.Label lblCSharpSnooker;
        private System.Windows.Forms.Label lblWin;
    }
}

