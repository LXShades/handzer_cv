using System;

namespace ImageAnalysis.GUI
{
    partial class MainWindow
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
            this.BtnLoadImage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.MoveFilterDown = new System.Windows.Forms.Button();
            this.MoveFilterUp = new System.Windows.Forms.Button();
            this.DeleteFilter = new System.Windows.Forms.Button();
            this.FilterStack = new System.Windows.Forms.ListBox();
            this.AddFilterList = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnCaptureCam = new System.Windows.Forms.Button();
            this.ImageBox = new System.Windows.Forms.GroupBox();
            this.FilterInfoBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnLoadImage
            // 
            this.BtnLoadImage.Location = new System.Drawing.Point(135, 19);
            this.BtnLoadImage.Name = "BtnLoadImage";
            this.BtnLoadImage.Size = new System.Drawing.Size(109, 23);
            this.BtnLoadImage.TabIndex = 0;
            this.BtnLoadImage.Text = "Use Test Image";
            this.BtnLoadImage.UseVisualStyleBackColor = true;
            this.BtnLoadImage.Click += new System.EventHandler(this.BtnLoadImage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 345);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Add Filter:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.MoveFilterDown);
            this.groupBox1.Controls.Add(this.MoveFilterUp);
            this.groupBox1.Controls.Add(this.DeleteFilter);
            this.groupBox1.Controls.Add(this.FilterStack);
            this.groupBox1.Controls.Add(this.AddFilterList);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 369);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filters";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.FilterInfoBox);
            this.groupBox4.Location = new System.Drawing.Point(9, 135);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(242, 201);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Filter Properties, Maybe, When They\'re Added";
            // 
            // MoveFilterDown
            // 
            this.MoveFilterDown.Location = new System.Drawing.Point(90, 106);
            this.MoveFilterDown.Name = "MoveFilterDown";
            this.MoveFilterDown.Size = new System.Drawing.Size(80, 23);
            this.MoveFilterDown.TabIndex = 4;
            this.MoveFilterDown.Text = "Move Down";
            this.MoveFilterDown.UseVisualStyleBackColor = true;
            this.MoveFilterDown.Click += new System.EventHandler(this.MoveFilterDown_Click);
            // 
            // MoveFilterUp
            // 
            this.MoveFilterUp.Location = new System.Drawing.Point(9, 106);
            this.MoveFilterUp.Name = "MoveFilterUp";
            this.MoveFilterUp.Size = new System.Drawing.Size(75, 23);
            this.MoveFilterUp.TabIndex = 4;
            this.MoveFilterUp.Text = "Move Up";
            this.MoveFilterUp.UseVisualStyleBackColor = true;
            this.MoveFilterUp.Click += new System.EventHandler(this.MoveFilterUp_Click);
            // 
            // DeleteFilter
            // 
            this.DeleteFilter.Location = new System.Drawing.Point(176, 106);
            this.DeleteFilter.Name = "DeleteFilter";
            this.DeleteFilter.Size = new System.Drawing.Size(75, 23);
            this.DeleteFilter.TabIndex = 4;
            this.DeleteFilter.Text = "Delete";
            this.DeleteFilter.UseVisualStyleBackColor = true;
            this.DeleteFilter.Click += new System.EventHandler(this.DeleteFilter_Click);
            // 
            // FilterStack
            // 
            this.FilterStack.FormattingEnabled = true;
            this.FilterStack.Location = new System.Drawing.Point(9, 19);
            this.FilterStack.Name = "FilterStack";
            this.FilterStack.Size = new System.Drawing.Size(242, 82);
            this.FilterStack.TabIndex = 4;
            // 
            // AddFilterList
            // 
            this.AddFilterList.FormattingEnabled = true;
            this.AddFilterList.Items.AddRange(new object[] {
            "Select New Filter..."});
            this.AddFilterList.Location = new System.Drawing.Point(79, 342);
            this.AddFilterList.Name = "AddFilterList";
            this.AddFilterList.Size = new System.Drawing.Size(172, 21);
            this.AddFilterList.TabIndex = 2;
            this.AddFilterList.Text = "Select New Filter...";
            this.AddFilterList.SelectionChangeCommitted += new System.EventHandler(this.AddFilterList_SelectionChangeCommitted);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BtnCaptureCam);
            this.groupBox2.Controls.Add(this.BtnLoadImage);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 51);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image";
            // 
            // BtnCaptureCam
            // 
            this.BtnCaptureCam.Location = new System.Drawing.Point(9, 19);
            this.BtnCaptureCam.Name = "BtnCaptureCam";
            this.BtnCaptureCam.Size = new System.Drawing.Size(120, 23);
            this.BtnCaptureCam.TabIndex = 5;
            this.BtnCaptureCam.Text = "Capture DroidCam";
            this.BtnCaptureCam.UseVisualStyleBackColor = true;
            this.BtnCaptureCam.Click += new System.EventHandler(this.BtnCaptureCam_Click);
            // 
            // ImageBox
            // 
            this.ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageBox.AutoSize = true;
            this.ImageBox.Location = new System.Drawing.Point(275, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(470, 426);
            this.ImageBox.TabIndex = 4;
            this.ImageBox.TabStop = false;
            this.ImageBox.Text = "Image Output";
            this.ImageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ImageBox_MouseClick);
            // 
            // FilterInfoBox
            // 
            this.FilterInfoBox.Location = new System.Drawing.Point(7, 20);
            this.FilterInfoBox.Multiline = true;
            this.FilterInfoBox.Name = "FilterInfoBox";
            this.FilterInfoBox.ReadOnly = true;
            this.FilterInfoBox.Size = new System.Drawing.Size(228, 175);
            this.FilterInfoBox.TabIndex = 0;
            this.FilterInfoBox.Text = "Debug info is printed here";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 450);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnLoadImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox AddFilterList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox FilterStack;
        private System.Windows.Forms.Button DeleteFilter;
        private System.Windows.Forms.Button MoveFilterDown;
        private System.Windows.Forms.Button MoveFilterUp;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button BtnCaptureCam;
        private System.Windows.Forms.GroupBox ImageBox;
        private System.Windows.Forms.TextBox FilterInfoBox;
    }
}

