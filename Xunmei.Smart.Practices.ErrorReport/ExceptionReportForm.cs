using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Xunmei.Smart.Practices.ErrorReport
{
    internal class ExceptionReportForm : Form, IExceptionReportView
    {
        #region 成员变量

        private Button btnCancel;
        private Button btnOk;
        private CheckBox chkAutoRestart;
        private int closeSeconds;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Timer tmrClose;
        private TextBox txtMessage;

        #endregion

        #region 构造函数

        public ExceptionReportForm()
        {
            this.InitializeComponent();
        }

        #endregion

        #region 私有方法

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.CloseForm();
        }

        private void CloseForm()
        {
            if (this.chkAutoRestart.Checked)
            {
                base.DialogResult = DialogResult.OK;
            }
            else
            {
                base.DialogResult = DialogResult.Cancel;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            this.chkAutoRestart = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(291, 262);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(71, 35);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Image = global::Xunmei.Smart.Practices.ErrorReport.Properties.Resources.Alert_12;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "　　   系统运行时出现了异常情况，建议您将系统重新启动。";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(12, 47);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(440, 168);
            this.txtMessage.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(10, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 41);
            this.label2.TabIndex = 1;
            this.label2.Text = "　　上面是异常的详细信息，建议您将这些信息提供给产品供应商，这将有助于修复您所遇到的问题，并提高您所使用的产品可靠性。";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(378, 262);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tmrClose
            // 
            this.tmrClose.Interval = 1000;
            this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
            // 
            // chkAutoRestart
            // 
            this.chkAutoRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAutoRestart.AutoSize = true;
            this.chkAutoRestart.Checked = true;
            this.chkAutoRestart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRestart.Location = new System.Drawing.Point(12, 272);
            this.chkAutoRestart.Name = "chkAutoRestart";
            this.chkAutoRestart.Size = new System.Drawing.Size(120, 16);
            this.chkAutoRestart.TabIndex = 3;
            this.chkAutoRestart.Text = "自动重新启动软件";
            this.chkAutoRestart.UseVisualStyleBackColor = true;
            // 
            // ExceptionReportForm
            // 
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(469, 309);
            this.Controls.Add(this.chkAutoRestart);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(485, 348);
            this.MinimizeBox = false;
            this.Name = "ExceptionReportForm";
            this.ShowInTaskbar = false;
            this.Text = "系统异常";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.tmrClose.Stop();
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.chkAutoRestart.Checked = false;
            }

            base.OnFormClosed(e);
            EventHandler<EventArgs> handler = Closed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void StartTimer()
        {
            if (this.closeSeconds > 0)
            {
                this.tmrClose.Start();
            }
            else if (this.closeSeconds == 0)
            {
                this.tmrClose.Stop();
            }
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            if (this.closeSeconds > 0)
            {
                this.closeSeconds--;
            }
            this.Text = string.Format("系统异常({0})", this.closeSeconds);
            if (this.closeSeconds == 0)
            {
                this.CloseForm();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.chkAutoRestart.Checked = false;
            CloseForm();
        }

        #endregion

        #region IExceptionReportView 接口成员

        public new event EventHandler<EventArgs> Closed;

        public bool AutoRestart
        {
            get
            {
                return this.chkAutoRestart.Checked;
            }
        }

        public bool ModalDialog
        {
            get { return true; }
        }

        public void ShowReport(string errorMessage, int closeSeconds, bool autoRestart)
        {
            this.txtMessage.Text = errorMessage;
            this.closeSeconds = closeSeconds;
            this.StartTimer();
            ShowDialog();
            this.chkAutoRestart.Visible = autoRestart;
            this.chkAutoRestart.Checked = autoRestart;
        }

        public void CloseReport()
        {
            CloseForm();
        }

        #endregion
    }
}

