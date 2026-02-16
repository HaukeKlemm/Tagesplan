namespace Tagesplan
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnLoginMews = new Button();
            btnDownloadReport = new Button();
            btnGenerateLists = new Button();
            txtStatus = new TextBox();
            lblStatus = new Label();
            btnSelectFile = new Button();
            txtFilePath = new TextBox();
            lblFile = new Label();
            progressBar = new ProgressBar();
            groupBoxActions = new GroupBox();
            groupBoxFile = new GroupBox();
            groupBoxStatus = new GroupBox();

            groupBoxActions.SuspendLayout();
            groupBoxFile.SuspendLayout();
            groupBoxStatus.SuspendLayout();
            SuspendLayout();

            // groupBoxActions
            groupBoxActions.Controls.Add(btnLoginMews);
            groupBoxActions.Controls.Add(btnDownloadReport);
            groupBoxActions.Controls.Add(btnGenerateLists);
            groupBoxActions.Location = new Point(12, 12);
            groupBoxActions.Name = "groupBoxActions";
            groupBoxActions.Size = new Size(760, 80);
            groupBoxActions.TabIndex = 0;
            groupBoxActions.TabStop = false;
            groupBoxActions.Text = "Aktionen";

            // btnLoginMews
            btnLoginMews.Location = new Point(20, 30);
            btnLoginMews.Name = "btnLoginMews";
            btnLoginMews.Size = new Size(200, 35);
            btnLoginMews.TabIndex = 0;
            btnLoginMews.Text = "1. MEWS Login";
            btnLoginMews.UseVisualStyleBackColor = true;
            btnLoginMews.Click += BtnLoginMews_Click;

            // btnDownloadReport
            btnDownloadReport.Enabled = false;
            btnDownloadReport.Location = new Point(240, 30);
            btnDownloadReport.Name = "btnDownloadReport";
            btnDownloadReport.Size = new Size(200, 35);
            btnDownloadReport.TabIndex = 1;
            btnDownloadReport.Text = "2. Bericht herunterladen";
            btnDownloadReport.UseVisualStyleBackColor = true;
            btnDownloadReport.Click += BtnDownloadReport_Click;

            // btnGenerateLists
            btnGenerateLists.Location = new Point(460, 30);
            btnGenerateLists.Name = "btnGenerateLists";
            btnGenerateLists.Size = new Size(200, 35);
            btnGenerateLists.TabIndex = 2;
            btnGenerateLists.Text = "3. Listen generieren";
            btnGenerateLists.UseVisualStyleBackColor = true;
            btnGenerateLists.Click += BtnGenerateLists_Click;

            // groupBoxFile
            groupBoxFile.Controls.Add(lblFile);
            groupBoxFile.Controls.Add(txtFilePath);
            groupBoxFile.Controls.Add(btnSelectFile);
            groupBoxFile.Location = new Point(12, 98);
            groupBoxFile.Name = "groupBoxFile";
            groupBoxFile.Size = new Size(760, 80);
            groupBoxFile.TabIndex = 1;
            groupBoxFile.TabStop = false;
            groupBoxFile.Text = "Excel-Datei";

            // lblFile
            lblFile.AutoSize = true;
            lblFile.Location = new Point(20, 30);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(105, 15);
            lblFile.TabIndex = 0;
            lblFile.Text = "MEWS Bericht-Datei:";

            // txtFilePath
            txtFilePath.Location = new Point(20, 48);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(600, 23);
            txtFilePath.TabIndex = 1;

            // btnSelectFile
            btnSelectFile.Location = new Point(630, 46);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(110, 27);
            btnSelectFile.TabIndex = 2;
            btnSelectFile.Text = "Datei wählen...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += BtnSelectFile_Click;

            // groupBoxStatus
            groupBoxStatus.Controls.Add(lblStatus);
            groupBoxStatus.Controls.Add(txtStatus);
            groupBoxStatus.Controls.Add(progressBar);
            groupBoxStatus.Location = new Point(12, 184);
            groupBoxStatus.Name = "groupBoxStatus";
            groupBoxStatus.Size = new Size(760, 254);
            groupBoxStatus.TabIndex = 2;
            groupBoxStatus.TabStop = false;
            groupBoxStatus.Text = "Status";

            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 25);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(42, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Bereit.";

            // txtStatus
            txtStatus.BackColor = SystemColors.Window;
            txtStatus.Font = new Font("Consolas", 9F);
            txtStatus.Location = new Point(20, 43);
            txtStatus.Multiline = true;
            txtStatus.Name = "txtStatus";
            txtStatus.ReadOnly = true;
            txtStatus.ScrollBars = ScrollBars.Vertical;
            txtStatus.Size = new Size(720, 165);
            txtStatus.TabIndex = 1;

            // progressBar
            progressBar.Location = new Point(20, 214);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(720, 23);
            progressBar.TabIndex = 2;

            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 450);
            Controls.Add(groupBoxStatus);
            Controls.Add(groupBoxFile);
            Controls.Add(groupBoxActions);
            Name = "Form1";
            Text = "MEWS Tagesplan Generator";
            FormClosing += Form1_FormClosing;
            groupBoxActions.ResumeLayout(false);
            groupBoxFile.ResumeLayout(false);
            groupBoxFile.PerformLayout();
            groupBoxStatus.ResumeLayout(false);
            groupBoxStatus.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnLoginMews;
        private Button btnDownloadReport;
        private Button btnGenerateLists;
        private TextBox txtStatus;
        private Label lblStatus;
        private Button btnSelectFile;
        private TextBox txtFilePath;
        private Label lblFile;
        private ProgressBar progressBar;
        private GroupBox groupBoxActions;
        private GroupBox groupBoxFile;
        private GroupBox groupBoxStatus;
    }
}
