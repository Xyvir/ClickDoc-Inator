﻿namespace Better_Steps_Recorder
{
    partial class HelpPopup
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
            button_CloseHelp = new Button();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            pictureBox2 = new PictureBox();
            VersionLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // button_CloseHelp
            // 
            button_CloseHelp.Location = new Point(432, 113);
            button_CloseHelp.Margin = new Padding(3, 4, 3, 4);
            button_CloseHelp.Name = "button_CloseHelp";
            button_CloseHelp.Size = new Size(94, 31);
            button_CloseHelp.TabIndex = 0;
            button_CloseHelp.Text = "Close Form";
            button_CloseHelp.UseVisualStyleBackColor = true;
            button_CloseHelp.Click += button_CloseHelp_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(133, 12);
            label1.MaximumSize = new Size(400, 400);
            label1.Name = "label1";
            label1.Size = new Size(363, 100);
            label1.TabIndex = 1;
            label1.Text = "Welcome to the ClickDocinator help menu.\r\nThis tool helps you record steps and take screenshots efficiently.\r\nFor more details and instructions, visit our GitHub repository.";
            label1.Click += label1_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(133, 72);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(204, 20);
            linkLabel1.TabIndex = 3;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "GitHub ClickDoc-Inator";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.StepsRecorder;
            pictureBox2.InitialImage = Properties.Resources.StepsRecorder;
            pictureBox2.Location = new Point(11, 16);
            pictureBox2.Margin = new Padding(3, 4, 3, 4);
            pictureBox2.MaximumSize = new Size(110, 128);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(110, 128);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // VersionLabel
            // 
            VersionLabel.AutoSize = true;
            VersionLabel.Location = new Point(135, 97);
            VersionLabel.Name = "VersionLabel";
            VersionLabel.Size = new Size(60, 20);
            VersionLabel.TabIndex = 5;
            VersionLabel.Text = "Version:";
            // 
            // HelpPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(539, 160);
            Controls.Add(VersionLabel);
            Controls.Add(pictureBox2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Controls.Add(button_CloseHelp);
            Margin = new Padding(3, 4, 3, 4);
            Name = "HelpPopup";
            ShowIcon = false;
            Text = "Help";
            TopMost = true;
            Load += HelpPopup_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_CloseHelp;
        private Label label1;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox2;
        private Label VersionLabel;
    }
}