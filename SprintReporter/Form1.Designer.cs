namespace SprintReporter
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
            LoginTextInput = new TextBox();
            PasswordJiraTextInput = new TextBox();
            JiraLoginButton = new Button();
            PrintReportButton = new Button();
            dateTimeIssuesStartInput = new DateTimePicker();
            dateTimeIssuesEndInput = new DateTimePicker();
            JiraFilterTextInput = new TextBox();
            label1 = new Label();
            label2 = new Label();
            Пароль = new Label();
            jiraloginlabel = new Label();
            StatusTextOutputLabel = new Label();
            ErrorOutputTextBox = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            fileSystemWatcher1 = new FileSystemWatcher();
            FileExplorerButton = new Button();
            FileDirectoryName = new TextBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
            SuspendLayout();
            // 
            // LoginTextInput
            // 
            LoginTextInput.Location = new Point(30, 51);
            LoginTextInput.Name = "LoginTextInput";
            LoginTextInput.Size = new Size(189, 23);
            LoginTextInput.TabIndex = 0;
            LoginTextInput.TextChanged += LoginTextInput_TextChanged;
            // 
            // PasswordJiraTextInput
            // 
            PasswordJiraTextInput.Location = new Point(30, 101);
            PasswordJiraTextInput.Name = "PasswordJiraTextInput";
            PasswordJiraTextInput.Size = new Size(189, 23);
            PasswordJiraTextInput.TabIndex = 1;
            PasswordJiraTextInput.UseSystemPasswordChar = true;
            // 
            // JiraLoginButton
            // 
            JiraLoginButton.Location = new Point(30, 140);
            JiraLoginButton.Name = "JiraLoginButton";
            JiraLoginButton.Size = new Size(189, 23);
            JiraLoginButton.TabIndex = 2;
            JiraLoginButton.Text = "Войти в Jira";
            JiraLoginButton.UseVisualStyleBackColor = true;
            JiraLoginButton.Click += JiraLoginButton_Click;
            // 
            // PrintReportButton
            // 
            PrintReportButton.Location = new Point(30, 337);
            PrintReportButton.Name = "PrintReportButton";
            PrintReportButton.Size = new Size(200, 23);
            PrintReportButton.TabIndex = 3;
            PrintReportButton.Text = "Печать отчета";
            PrintReportButton.UseVisualStyleBackColor = true;
            PrintReportButton.Click += PrintReportButton_Click;
            // 
            // dateTimeIssuesStartInput
            // 
            dateTimeIssuesStartInput.Location = new Point(30, 251);
            dateTimeIssuesStartInput.Name = "dateTimeIssuesStartInput";
            dateTimeIssuesStartInput.Size = new Size(200, 23);
            dateTimeIssuesStartInput.TabIndex = 4;
            // 
            // dateTimeIssuesEndInput
            // 
            dateTimeIssuesEndInput.Location = new Point(30, 290);
            dateTimeIssuesEndInput.Name = "dateTimeIssuesEndInput";
            dateTimeIssuesEndInput.Size = new Size(200, 23);
            dateTimeIssuesEndInput.TabIndex = 5;
            // 
            // JiraFilterTextInput
            // 
            JiraFilterTextInput.Location = new Point(350, 251);
            JiraFilterTextInput.Name = "JiraFilterTextInput";
            JiraFilterTextInput.Size = new Size(377, 23);
            JiraFilterTextInput.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 214);
            label1.Name = "label1";
            label1.Size = new Size(260, 15);
            label1.TabIndex = 7;
            label1.Text = "Временной промежуток для получения задач";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(350, 214);
            label2.Name = "label2";
            label2.Size = new Size(175, 15);
            label2.TabIndex = 8;
            label2.Text = "Дополнительные фильтры jira";
            // 
            // Пароль
            // 
            Пароль.AutoSize = true;
            Пароль.Location = new Point(30, 83);
            Пароль.Name = "Пароль";
            Пароль.Size = new Size(49, 15);
            Пароль.TabIndex = 9;
            Пароль.Text = "Пароль";
            Пароль.UseWaitCursor = true;
            Пароль.Click += label3_Click;
            // 
            // jiraloginlabel
            // 
            jiraloginlabel.AutoSize = true;
            jiraloginlabel.Location = new Point(30, 24);
            jiraloginlabel.Name = "jiraloginlabel";
            jiraloginlabel.Size = new Size(41, 15);
            jiraloginlabel.TabIndex = 10;
            jiraloginlabel.Text = "Логин";
            // 
            // StatusTextOutputLabel
            // 
            StatusTextOutputLabel.AutoSize = true;
            StatusTextOutputLabel.Location = new Point(30, 392);
            StatusTextOutputLabel.Name = "StatusTextOutputLabel";
            StatusTextOutputLabel.Size = new Size(214, 15);
            StatusTextOutputLabel.TabIndex = 11;
            StatusTextOutputLabel.Text = "Вывод дополнительной информации";
            StatusTextOutputLabel.Click += label_errors;
            // 
            // ErrorOutputTextBox
            // 
            ErrorOutputTextBox.Location = new Point(30, 410);
            ErrorOutputTextBox.Name = "ErrorOutputTextBox";
            ErrorOutputTextBox.Size = new Size(214, 23);
            ErrorOutputTextBox.TabIndex = 12;
            ErrorOutputTextBox.TextChanged += ErrorOutputTextBox_TextChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            // 
            // fileSystemWatcher1
            // 
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.SynchronizingObject = this;
            fileSystemWatcher1.Changed += fileSystemWatcher1_Changed;
            // 
            // FileExplorerButton
            // 
            FileExplorerButton.Location = new Point(565, 318);
            FileExplorerButton.Name = "FileExplorerButton";
            FileExplorerButton.Size = new Size(162, 23);
            FileExplorerButton.TabIndex = 14;
            FileExplorerButton.Text = "Выбор папки";
            FileExplorerButton.UseVisualStyleBackColor = true;
            FileExplorerButton.Click += FileExplorerButton_Click;
            // 
            // FileDirectoryName
            // 
            FileDirectoryName.Location = new Point(350, 318);
            FileDirectoryName.Name = "FileDirectoryName";
            FileDirectoryName.Size = new Size(199, 23);
            FileDirectoryName.TabIndex = 16;
            FileDirectoryName.Text = "C:\\";
            FileDirectoryName.TextChanged += textBox3_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(350, 296);
            label3.Name = "label3";
            label3.Size = new Size(191, 15);
            label3.TabIndex = 17;
            label3.Text = "Выбор папки для загрузки отчета";
            label3.Click += label3_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label3);
            Controls.Add(FileDirectoryName);
            Controls.Add(FileExplorerButton);
            Controls.Add(ErrorOutputTextBox);
            Controls.Add(StatusTextOutputLabel);
            Controls.Add(jiraloginlabel);
            Controls.Add(Пароль);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(JiraFilterTextInput);
            Controls.Add(dateTimeIssuesEndInput);
            Controls.Add(dateTimeIssuesStartInput);
            Controls.Add(PrintReportButton);
            Controls.Add(JiraLoginButton);
            Controls.Add(PasswordJiraTextInput);
            Controls.Add(LoginTextInput);
            Name = "Form1";
            Text = "Выгрузка задач Jira";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox LoginTextInput;
        private TextBox PasswordJiraTextInput;
        private Button JiraLoginButton;
        private Button PrintReportButton;
        private DateTimePicker dateTimeIssuesStartInput;
        private DateTimePicker dateTimeIssuesEndInput;
        private TextBox JiraFilterTextInput;
        private Label label1;
        private Label label2;
        private Label Пароль;
        private Label jiraloginlabel;
        private Label StatusTextOutputLabel;
        private TextBox ErrorOutputTextBox;
        private OpenFileDialog openFileDialog1;
        private FileSystemWatcher fileSystemWatcher1;
        private Button FileExplorerButton;
        private Label label3;
        private TextBox FileDirectoryName;
    }
}
