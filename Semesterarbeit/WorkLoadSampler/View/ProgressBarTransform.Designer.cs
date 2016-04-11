namespace WorkLoadSampler.View
{
    partial class ProgressBarTransform
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            SpPerfChart.ChartPen chartPen9 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen10 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen11 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen12 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen13 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen14 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen15 = new SpPerfChart.ChartPen();
            SpPerfChart.ChartPen chartPen16 = new SpPerfChart.ChartPen();
            this.upgNms = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upgOutMsg = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upgIncMsg = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.lblNumMsgSeen = new Infragistics.Win.Misc.UltraLabel();
            this.lblIncMsg = new Infragistics.Win.Misc.UltraLabel();
            this.lblOutMsgCount = new Infragistics.Win.Misc.UltraLabel();
            this.ulblNMS = new Infragistics.Win.Misc.UltraLabel();
            this.ulblinc = new Infragistics.Win.Misc.UltraLabel();
            this.ulblOut = new Infragistics.Win.Misc.UltraLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.ulblTitle = new Infragistics.Win.Misc.UltraLabel();
            this.ulblnmsmax = new Infragistics.Win.Misc.UltraLabel();
            this.ulblincmax = new Infragistics.Win.Misc.UltraLabel();
            this.ulbloutmax = new Infragistics.Win.Misc.UltraLabel();
            this.perfChart1 = new SpPerfChart.PerfChart();
            this.perfChart = new SpPerfChart.PerfChart();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // upgNms
            // 
            this.upgNms.Location = new System.Drawing.Point(89, 25);
            this.upgNms.Name = "upgNms";
            this.upgNms.Size = new System.Drawing.Size(178, 23);
            this.upgNms.TabIndex = 0;
            this.upgNms.Text = "[Formatted]";
            // 
            // upgOutMsg
            // 
            this.upgOutMsg.Location = new System.Drawing.Point(89, 83);
            this.upgOutMsg.Name = "upgOutMsg";
            this.upgOutMsg.Size = new System.Drawing.Size(178, 23);
            this.upgOutMsg.TabIndex = 1;
            this.upgOutMsg.Text = "[Formatted]";
            // 
            // upgIncMsg
            // 
            this.upgIncMsg.Location = new System.Drawing.Point(89, 54);
            this.upgIncMsg.Name = "upgIncMsg";
            this.upgIncMsg.Size = new System.Drawing.Size(178, 23);
            this.upgIncMsg.TabIndex = 2;
            this.upgIncMsg.Text = "[Formatted]";
            // 
            // lblNumMsgSeen
            // 
            this.lblNumMsgSeen.Location = new System.Drawing.Point(3, 31);
            this.lblNumMsgSeen.Name = "lblNumMsgSeen";
            this.lblNumMsgSeen.Size = new System.Drawing.Size(80, 23);
            this.lblNumMsgSeen.TabIndex = 3;
            this.lblNumMsgSeen.Text = "NMS:";
            // 
            // lblIncMsg
            // 
            this.lblIncMsg.Location = new System.Drawing.Point(3, 60);
            this.lblIncMsg.Name = "lblIncMsg";
            this.lblIncMsg.Size = new System.Drawing.Size(80, 23);
            this.lblIncMsg.TabIndex = 4;
            this.lblIncMsg.Text = "IncMsgCnt:";
            // 
            // lblOutMsgCount
            // 
            this.lblOutMsgCount.Location = new System.Drawing.Point(3, 89);
            this.lblOutMsgCount.Name = "lblOutMsgCount";
            this.lblOutMsgCount.Size = new System.Drawing.Size(80, 29);
            this.lblOutMsgCount.TabIndex = 5;
            this.lblOutMsgCount.Text = "OutMsgCnt:";
            // 
            // ulblNMS
            // 
            this.ulblNMS.Location = new System.Drawing.Point(273, 31);
            this.ulblNMS.Name = "ulblNMS";
            this.ulblNMS.Size = new System.Drawing.Size(61, 23);
            this.ulblNMS.TabIndex = 6;
            this.ulblNMS.Text = "ultraLabel1";
            // 
            // ulblinc
            // 
            this.ulblinc.Location = new System.Drawing.Point(273, 60);
            this.ulblinc.Name = "ulblinc";
            this.ulblinc.Size = new System.Drawing.Size(61, 23);
            this.ulblinc.TabIndex = 7;
            this.ulblinc.Text = "ultraLabel2";
            // 
            // ulblOut
            // 
            this.ulblOut.Location = new System.Drawing.Point(273, 89);
            this.ulblOut.Name = "ulblOut";
            this.ulblOut.Size = new System.Drawing.Size(61, 23);
            this.ulblOut.TabIndex = 8;
            this.ulblOut.Text = "ultraLabel3";
            // 
            // timer
            // 
            this.timer.Interval = 1;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // ulblTitle
            // 
            this.ulblTitle.Location = new System.Drawing.Point(4, 4);
            this.ulblTitle.Name = "ulblTitle";
            this.ulblTitle.Size = new System.Drawing.Size(263, 21);
            this.ulblTitle.TabIndex = 9;
            this.ulblTitle.Text = "Title";
            // 
            // ulblnmsmax
            // 
            this.ulblnmsmax.Location = new System.Drawing.Point(340, 31);
            this.ulblnmsmax.Name = "ulblnmsmax";
            this.ulblnmsmax.Size = new System.Drawing.Size(61, 23);
            this.ulblnmsmax.TabIndex = 10;
            this.ulblnmsmax.Text = "ultraLabel1";
            // 
            // ulblincmax
            // 
            this.ulblincmax.Location = new System.Drawing.Point(340, 60);
            this.ulblincmax.Name = "ulblincmax";
            this.ulblincmax.Size = new System.Drawing.Size(61, 23);
            this.ulblincmax.TabIndex = 11;
            this.ulblincmax.Text = "ultraLabel1";
            // 
            // ulbloutmax
            // 
            this.ulbloutmax.Location = new System.Drawing.Point(340, 89);
            this.ulbloutmax.Name = "ulbloutmax";
            this.ulbloutmax.Size = new System.Drawing.Size(61, 23);
            this.ulbloutmax.TabIndex = 12;
            this.ulbloutmax.Text = "ultraLabel1";
            // 
            // perfChart1
            // 
            this.perfChart1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.perfChart1.Location = new System.Drawing.Point(407, 55);
            this.perfChart1.Name = "perfChart1";
            this.perfChart1.PerfChartStyle.AntiAliasing = true;
            chartPen9.Color = System.Drawing.Color.LightGreen;
            chartPen9.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            chartPen9.Width = 1F;
            this.perfChart1.PerfChartStyle.AvgLinePen = chartPen9;
            this.perfChart1.PerfChartStyle.BackgroundColorBottom = System.Drawing.Color.YellowGreen;
            this.perfChart1.PerfChartStyle.BackgroundColorTop = System.Drawing.Color.DeepPink;
            chartPen10.Color = System.Drawing.Color.Gold;
            chartPen10.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen10.Width = 1F;
            this.perfChart1.PerfChartStyle.ChartLinePen = chartPen10;
            chartPen11.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen11.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            chartPen11.Width = 1F;
            this.perfChart1.PerfChartStyle.HorizontalGridPen = chartPen11;
            this.perfChart1.PerfChartStyle.ShowAverageLine = true;
            this.perfChart1.PerfChartStyle.ShowHorizontalGridLines = true;
            this.perfChart1.PerfChartStyle.ShowVerticalGridLines = true;
            chartPen12.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen12.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            chartPen12.Width = 1F;
            this.perfChart1.PerfChartStyle.VerticalGridPen = chartPen12;
            this.perfChart1.ScaleMode = SpPerfChart.ScaleMode.Absolute;
            this.perfChart1.Size = new System.Drawing.Size(347, 51);
            this.perfChart1.TabIndex = 14;
            this.perfChart1.TimerInterval = 150;
            this.perfChart1.TimerMode = SpPerfChart.TimerMode.SynchronizedAverage;
            // 
            // perfChart
            // 
            this.perfChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.perfChart.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.perfChart.Location = new System.Drawing.Point(407, 3);
            this.perfChart.Name = "perfChart";
            this.perfChart.PerfChartStyle.AntiAliasing = true;
            chartPen13.Color = System.Drawing.Color.LightGreen;
            chartPen13.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            chartPen13.Width = 1F;
            this.perfChart.PerfChartStyle.AvgLinePen = chartPen13;
            this.perfChart.PerfChartStyle.BackgroundColorBottom = System.Drawing.Color.DeepPink;
            this.perfChart.PerfChartStyle.BackgroundColorTop = System.Drawing.Color.YellowGreen;
            chartPen14.Color = System.Drawing.Color.Gold;
            chartPen14.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen14.Width = 1F;
            this.perfChart.PerfChartStyle.ChartLinePen = chartPen14;
            chartPen15.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen15.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            chartPen15.Width = 1F;
            this.perfChart.PerfChartStyle.HorizontalGridPen = chartPen15;
            this.perfChart.PerfChartStyle.ShowAverageLine = true;
            this.perfChart.PerfChartStyle.ShowHorizontalGridLines = true;
            this.perfChart.PerfChartStyle.ShowVerticalGridLines = true;
            chartPen16.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen16.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            chartPen16.Width = 1F;
            this.perfChart.PerfChartStyle.VerticalGridPen = chartPen16;
            this.perfChart.ScaleMode = SpPerfChart.ScaleMode.Absolute;
            this.perfChart.Size = new System.Drawing.Size(347, 51);
            this.perfChart.TabIndex = 13;
            this.perfChart.TimerInterval = 150;
            this.perfChart.TimerMode = SpPerfChart.TimerMode.SynchronizedAverage;
            // 
            // ultraButton1
            // 
            this.ultraButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraButton1.Location = new System.Drawing.Point(273, 2);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(61, 13);
            this.ultraButton1.TabIndex = 15;
            this.ultraButton1.Text = "START SUB";
            this.ultraButton1.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraButton2
            // 
            this.ultraButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F);
            this.ultraButton2.Location = new System.Drawing.Point(273, 16);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(61, 13);
            this.ultraButton2.TabIndex = 16;
            this.ultraButton2.Text = "STOP SUB";
            this.ultraButton2.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.ultraButton2.Click += new System.EventHandler(this.ultraButton2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(341, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 9);
            this.label1.TabIndex = 17;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(341, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 9);
            this.label2.TabIndex = 18;
            this.label2.Text = "label2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "label3";
            // 
            // ProgressBarTransform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ultraButton2);
            this.Controls.Add(this.ultraButton1);
            this.Controls.Add(this.perfChart1);
            this.Controls.Add(this.perfChart);
            this.Controls.Add(this.ulbloutmax);
            this.Controls.Add(this.ulblincmax);
            this.Controls.Add(this.ulblnmsmax);
            this.Controls.Add(this.ulblTitle);
            this.Controls.Add(this.ulblOut);
            this.Controls.Add(this.ulblinc);
            this.Controls.Add(this.ulblNMS);
            this.Controls.Add(this.lblOutMsgCount);
            this.Controls.Add(this.lblIncMsg);
            this.Controls.Add(this.lblNumMsgSeen);
            this.Controls.Add(this.upgIncMsg);
            this.Controls.Add(this.upgOutMsg);
            this.Controls.Add(this.upgNms);
            this.Name = "ProgressBarTransform";
            this.Size = new System.Drawing.Size(757, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar upgNms;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar upgOutMsg;
        private Infragistics.Win.UltraWinProgressBar.UltraProgressBar upgIncMsg;
        private Infragistics.Win.Misc.UltraLabel lblNumMsgSeen;
        private Infragistics.Win.Misc.UltraLabel lblIncMsg;
        private Infragistics.Win.Misc.UltraLabel lblOutMsgCount;
        private Infragistics.Win.Misc.UltraLabel ulblNMS;
        private Infragistics.Win.Misc.UltraLabel ulblinc;
        private Infragistics.Win.Misc.UltraLabel ulblOut;
        private System.Windows.Forms.Timer timer;
        private Infragistics.Win.Misc.UltraLabel ulblTitle;
        private Infragistics.Win.Misc.UltraLabel ulblnmsmax;
        private Infragistics.Win.Misc.UltraLabel ulblincmax;
        private Infragistics.Win.Misc.UltraLabel ulbloutmax;
        private SpPerfChart.PerfChart perfChart1;
        private SpPerfChart.PerfChart perfChart;
		private Infragistics.Win.Misc.UltraButton ultraButton1;
		private Infragistics.Win.Misc.UltraButton ultraButton2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
