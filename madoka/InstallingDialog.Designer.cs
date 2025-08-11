namespace madoka
{
	partial class InstallingDialog
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelProgress = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxNoNotify = new System.Windows.Forms.GroupBox();
			this.radioButtonRequireNotify = new System.Windows.Forms.RadioButton();
			this.radioButtonNoAction = new System.Windows.Forms.RadioButton();
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.groupBoxNoNotify.SuspendLayout();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(12, 34);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(448, 16);
			this.progressBar1.TabIndex = 0;
			this.progressBar1.UseWaitCursor = true;
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(10, 15);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(335, 16);
			this.labelMessage.TabIndex = 1;
			this.labelMessage.Text = "Installing...";
			this.labelMessage.UseWaitCursor = true;
			// 
			// labelProgress
			// 
			this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelProgress.Location = new System.Drawing.Point(351, 15);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(109, 16);
			this.labelProgress.TabIndex = 2;
			this.labelProgress.Text = "9/42";
			this.labelProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelProgress.UseWaitCursor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(199, 56);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.UseWaitCursor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBoxNoNotify
			// 
			this.groupBoxNoNotify.Controls.Add(this.radioButtonRequireNotify);
			this.groupBoxNoNotify.Controls.Add(this.radioButtonNoAction);
			this.groupBoxNoNotify.Location = new System.Drawing.Point(12, 96);
			this.groupBoxNoNotify.Name = "groupBoxNoNotify";
			this.groupBoxNoNotify.Size = new System.Drawing.Size(448, 65);
			this.groupBoxNoNotify.TabIndex = 4;
			this.groupBoxNoNotify.TabStop = false;
			this.groupBoxNoNotify.Text = "一時 Install したフォントは…";
			this.groupBoxNoNotify.UseWaitCursor = true;
			// 
			// radioButtonRequireNotify
			// 
			this.radioButtonRequireNotify.AutoSize = true;
			this.radioButtonRequireNotify.Checked = true;
			this.radioButtonRequireNotify.Location = new System.Drawing.Point(6, 40);
			this.radioButtonRequireNotify.Name = "radioButtonRequireNotify";
			this.radioButtonRequireNotify.Size = new System.Drawing.Size(274, 16);
			this.radioButtonRequireNotify.TabIndex = 1;
			this.radioButtonRequireNotify.TabStop = true;
			this.radioButtonRequireNotify.Text = "起動済みのアプリケーションでも利用したいです（重い）";
			this.radioButtonRequireNotify.UseVisualStyleBackColor = true;
			this.radioButtonRequireNotify.UseWaitCursor = true;
			// 
			// radioButtonNoAction
			// 
			this.radioButtonNoAction.AutoSize = true;
			this.radioButtonNoAction.Location = new System.Drawing.Point(6, 18);
			this.radioButtonNoAction.Name = "radioButtonNoAction";
			this.radioButtonNoAction.Size = new System.Drawing.Size(280, 16);
			this.radioButtonNoAction.TabIndex = 0;
			this.radioButtonNoAction.Text = "今後起動するアプリケーションで利用出来れば良いです";
			this.radioButtonNoAction.UseVisualStyleBackColor = true;
			this.radioButtonNoAction.UseWaitCursor = true;
			// 
			// timerUpdate
			// 
			this.timerUpdate.Enabled = true;
			this.timerUpdate.Interval = 200;
			this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
			// 
			// InstallingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(472, 173);
			this.ControlBox = false;
			this.Controls.Add(this.groupBoxNoNotify);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelProgress);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.progressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InstallingDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = " ";
			this.UseWaitCursor = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstallingDialog_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InstallingDialog_FormClosed);
			this.groupBoxNoNotify.ResumeLayout(false);
			this.groupBoxNoNotify.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelProgress;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxNoNotify;
		private System.Windows.Forms.RadioButton radioButtonRequireNotify;
		private System.Windows.Forms.RadioButton radioButtonNoAction;
		private System.Windows.Forms.Timer timerUpdate;
	}
}