﻿namespace SpiralLab.Sirius
{
    partial class MotfMarkerForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pgbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.manualOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualOnToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panReady = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panBusy = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panError = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtEncSpeed = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDistance = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chbExternalTrigger = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.panReady.SuspendLayout();
            this.panBusy.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pgbProgress,
            this.lblTime,
            this.toolStripDropDownButton2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 622);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(507, 28);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pgbProgress
            // 
            this.pgbProgress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pgbProgress.Name = "pgbProgress";
            this.pgbProgress.Size = new System.Drawing.Size(100, 22);
            this.pgbProgress.Step = 1;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = false;
            this.lblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(100, 23);
            this.lblTime.Text = "0.000";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualOffToolStripMenuItem,
            this.manualOnToolStripMenuItem1});
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(29, 26);
            this.toolStripDropDownButton2.Text = "toolStripDropDownButton2";
            // 
            // manualOffToolStripMenuItem
            // 
            this.manualOffToolStripMenuItem.Name = "manualOffToolStripMenuItem";
            this.manualOffToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.manualOffToolStripMenuItem.Text = "Manual Off";
            this.manualOffToolStripMenuItem.Click += new System.EventHandler(this.manualOffToolStripMenuItem_Click_1);
            // 
            // manualOnToolStripMenuItem1
            // 
            this.manualOnToolStripMenuItem1.Name = "manualOnToolStripMenuItem1";
            this.manualOnToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.manualOnToolStripMenuItem1.Text = "Manual On";
            this.manualOnToolStripMenuItem1.Click += new System.EventHandler(this.manualOnToolStripMenuItem1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(7, 51);
            this.listBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(496, 349);
            this.listBox1.TabIndex = 12;
            // 
            // panReady
            // 
            this.panReady.BackColor = System.Drawing.Color.Green;
            this.panReady.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panReady.Controls.Add(this.label1);
            this.panReady.Location = new System.Drawing.Point(7, 5);
            this.panReady.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panReady.Name = "panReady";
            this.panReady.Size = new System.Drawing.Size(100, 39);
            this.panReady.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "READY";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(327, 566);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(172, 47);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(327, 519);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(172, 46);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "S&top";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(327, 413);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(172, 62);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panBusy
            // 
            this.panBusy.BackColor = System.Drawing.Color.Green;
            this.panBusy.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panBusy.Controls.Add(this.label2);
            this.panBusy.Location = new System.Drawing.Point(113, 5);
            this.panBusy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panBusy.Name = "panBusy";
            this.panBusy.Size = new System.Drawing.Size(100, 39);
            this.panBusy.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Maroon;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "BUSY";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Green;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.panError);
            this.panel3.Location = new System.Drawing.Point(217, 5);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(100, 39);
            this.panel3.TabIndex = 19;
            // 
            // panError
            // 
            this.panError.BackColor = System.Drawing.Color.Maroon;
            this.panError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panError.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panError.ForeColor = System.Drawing.Color.White;
            this.panError.Location = new System.Drawing.Point(0, 0);
            this.panError.Name = "panError";
            this.panError.Size = new System.Drawing.Size(96, 35);
            this.panError.TabIndex = 0;
            this.panError.Text = "ERROR";
            this.panError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtEncSpeed
            // 
            this.txtEncSpeed.Location = new System.Drawing.Point(148, 566);
            this.txtEncSpeed.Name = "txtEncSpeed";
            this.txtEncSpeed.Size = new System.Drawing.Size(80, 21);
            this.txtEncSpeed.TabIndex = 21;
            this.txtEncSpeed.Text = "100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(90, 590);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 15);
            this.label3.TabIndex = 22;
            this.label3.Text = "Encoder Speed (mm/s):";
            // 
            // txtDistance
            // 
            this.txtDistance.Location = new System.Drawing.Point(148, 420);
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.Size = new System.Drawing.Size(80, 21);
            this.txtDistance.TabIndex = 23;
            this.txtDistance.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(50, 420);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 15);
            this.label4.TabIndex = 24;
            this.label4.Text = "Distance (mm):";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(7, 413);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(288, 200);
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // chbExternalTrigger
            // 
            this.chbExternalTrigger.AutoSize = true;
            this.chbExternalTrigger.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbExternalTrigger.Location = new System.Drawing.Point(327, 482);
            this.chbExternalTrigger.Name = "chbExternalTrigger";
            this.chbExternalTrigger.Size = new System.Drawing.Size(116, 19);
            this.chbExternalTrigger.TabIndex = 26;
            this.chbExternalTrigger.Text = "External Trigger";
            this.chbExternalTrigger.UseVisualStyleBackColor = true;
            this.chbExternalTrigger.CheckedChanged += new System.EventHandler(this.chbExternalTrigger_CheckedChanged);
            // 
            // MotfMarkerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 650);
            this.Controls.Add(this.chbExternalTrigger);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDistance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEncSpeed);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.panReady);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.panBusy);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MotfMarkerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Marker (c)SpiralLab";
            this.VisibleChanged += new System.EventHandler(this.YourMarkerForm_VisibleChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panReady.ResumeLayout(false);
            this.panBusy.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar pgbProgress;
        private System.Windows.Forms.ToolStripStatusLabel lblTime;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panReady;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panBusy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label panError;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtEncSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDistance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem manualOnToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem manualOffToolStripMenuItem;
        private System.Windows.Forms.CheckBox chbExternalTrigger;
    }
}