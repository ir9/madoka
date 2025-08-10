namespace madoka_test
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonInstallingDialog = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonInstallingDialog
			// 
			this.buttonInstallingDialog.Location = new System.Drawing.Point(12, 12);
			this.buttonInstallingDialog.Name = "buttonInstallingDialog";
			this.buttonInstallingDialog.Size = new System.Drawing.Size(151, 23);
			this.buttonInstallingDialog.TabIndex = 0;
			this.buttonInstallingDialog.Text = "InstallingDialog";
			this.buttonInstallingDialog.UseVisualStyleBackColor = true;
			this.buttonInstallingDialog.Click += new System.EventHandler(this.buttonInstallingDialog_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(444, 279);
			this.Controls.Add(this.buttonInstallingDialog);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonInstallingDialog;
	}
}

