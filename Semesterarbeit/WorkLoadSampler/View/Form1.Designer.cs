namespace WorkLoadSampler.View
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
			this.components = new System.ComponentModel.Container();
			this.btnAddFeederTask = new Infragistics.Win.Misc.UltraButton();
			this.btnRmvFeederTask = new Infragistics.Win.Misc.UltraButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.button1 = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// btnAddFeederTask
			// 
			this.btnAddFeederTask.Location = new System.Drawing.Point(538, 12);
			this.btnAddFeederTask.Name = "btnAddFeederTask";
			this.btnAddFeederTask.Size = new System.Drawing.Size(118, 23);
			this.btnAddFeederTask.TabIndex = 11;
			this.btnAddFeederTask.Text = "Add new Feedertask";
			this.btnAddFeederTask.Click += new System.EventHandler(this.ultraButton1_Click);
			// 
			// btnRmvFeederTask
			// 
			this.btnRmvFeederTask.Location = new System.Drawing.Point(662, 12);
			this.btnRmvFeederTask.Name = "btnRmvFeederTask";
			this.btnRmvFeederTask.Size = new System.Drawing.Size(118, 23);
			this.btnRmvFeederTask.TabIndex = 12;
			this.btnRmvFeederTask.Text = "Remove Feedertask";
			this.btnRmvFeederTask.Click += new System.EventHandler(this.btnRmvFeederTask_Click);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(449, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(83, 23);
			this.button1.TabIndex = 13;
			this.button1.Text = "Clear Controls";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Location = new System.Drawing.Point(2, 41);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(790, 590);
			this.tabControl1.TabIndex = 14;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 631);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnRmvFeederTask);
			this.Controls.Add(this.btnAddFeederTask);
			this.Name = "Form1";
			this.Text = "Realtime Workload Visualizer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.ResumeLayout(false);

        }

        #endregion
		private Infragistics.Win.Misc.UltraButton btnAddFeederTask;
		private Infragistics.Win.Misc.UltraButton btnRmvFeederTask;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabControl tabControl1;
	}
}

