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
            PrintReportButton = new Button();
            button1 = new Button();
            label1 = new Label();
            dateTimeIssuesStartInput = new DateTimePicker();
            dateTimeIssuesEndInput = new DateTimePicker();
            LoginTextInput = new TextBox();
            PasswordJiraTextInput = new TextBox();
            JiraLoginButton = new Button();
            JiraFilterTextInput = new TextBox();
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
            label4 = new Label();
            label5 = new Label();
            projectKeysList = new CheckedListBox();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
            SuspendLayout();
            // 
            // PrintReportButton
            // 
            PrintReportButton.Location = new Point(582, 68);
            PrintReportButton.Margin = new Padding(3, 4, 3, 4);
            PrintReportButton.Name = "PrintReportButton";
            PrintReportButton.Size = new Size(229, 31);
            PrintReportButton.TabIndex = 3;
            PrintReportButton.Text = "Печать отчета спринта";
            PrintReportButton.UseVisualStyleBackColor = true;
            PrintReportButton.Click += PrintReportButton_Click;
            // 
            // button1
            // 
            button1.Location = new Point(582, 111);
            button1.Name = "button1";
            button1.Size = new Size(229, 29);
            button1.TabIndex = 0;
            button1.Text = "Печать KPI";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button_print_kpi_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 252);
            label1.Name = "label1";
            label1.Size = new Size(330, 20);
            label1.TabIndex = 7;
            label1.Text = "Временной промежуток для получения задач";
            label1.Click += label1_Click;
            // 
            // dateTimeIssuesStartInput
            // 
            dateTimeIssuesStartInput.Location = new Point(34, 319);
            dateTimeIssuesStartInput.Margin = new Padding(3, 4, 3, 4);
            dateTimeIssuesStartInput.Name = "dateTimeIssuesStartInput";
            dateTimeIssuesStartInput.Size = new Size(228, 27);
            dateTimeIssuesStartInput.TabIndex = 4;
            dateTimeIssuesStartInput.ValueChanged += dateTimeIssuesStartInput_ValueChanged;
            // 
            // dateTimeIssuesEndInput
            // 
            dateTimeIssuesEndInput.Location = new Point(34, 393);
            dateTimeIssuesEndInput.Margin = new Padding(3, 4, 3, 4);
            dateTimeIssuesEndInput.Name = "dateTimeIssuesEndInput";
            dateTimeIssuesEndInput.Size = new Size(228, 27);
            dateTimeIssuesEndInput.TabIndex = 5;
            dateTimeIssuesEndInput.ValueChanged += dateTimeIssuesEndInput_ValueChanged;
            // 
            // LoginTextInput
            // 
            LoginTextInput.Location = new Point(34, 68);
            LoginTextInput.Margin = new Padding(3, 4, 3, 4);
            LoginTextInput.Name = "LoginTextInput";
            LoginTextInput.Size = new Size(215, 27);
            LoginTextInput.TabIndex = 0;
            LoginTextInput.TextChanged += LoginTextInput_TextChanged;
            // 
            // PasswordJiraTextInput
            // 
            PasswordJiraTextInput.Location = new Point(34, 135);
            PasswordJiraTextInput.Margin = new Padding(3, 4, 3, 4);
            PasswordJiraTextInput.Name = "PasswordJiraTextInput";
            PasswordJiraTextInput.Size = new Size(215, 27);
            PasswordJiraTextInput.TabIndex = 1;
            PasswordJiraTextInput.UseSystemPasswordChar = true;
            // 
            // JiraLoginButton
            // 
            JiraLoginButton.Location = new Point(34, 187);
            JiraLoginButton.Margin = new Padding(3, 4, 3, 4);
            JiraLoginButton.Name = "JiraLoginButton";
            JiraLoginButton.Size = new Size(216, 31);
            JiraLoginButton.TabIndex = 2;
            JiraLoginButton.Text = "Войти в Jira";
            JiraLoginButton.UseVisualStyleBackColor = true;
            JiraLoginButton.Click += JiraLoginButton_Click;
            // 
            // JiraFilterTextInput
            // 
            JiraFilterTextInput.Location = new Point(430, 288);
            JiraFilterTextInput.Margin = new Padding(3, 4, 3, 4);
            JiraFilterTextInput.Multiline = true;
            JiraFilterTextInput.Name = "JiraFilterTextInput";
            JiraFilterTextInput.Size = new Size(448, 101);
            JiraFilterTextInput.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(430, 252);
            label2.Name = "label2";
            label2.Size = new Size(220, 20);
            label2.TabIndex = 8;
            label2.Text = "Дополнительные фильтры jira";
            // 
            // Пароль
            // 
            Пароль.AutoSize = true;
            Пароль.Location = new Point(34, 111);
            Пароль.Name = "Пароль";
            Пароль.Size = new Size(62, 20);
            Пароль.TabIndex = 9;
            Пароль.Text = "Пароль";
            Пароль.UseWaitCursor = true;
            Пароль.Click += label3_Click;
            // 
            // jiraloginlabel
            // 
            jiraloginlabel.AutoSize = true;
            jiraloginlabel.Location = new Point(34, 32);
            jiraloginlabel.Name = "jiraloginlabel";
            jiraloginlabel.Size = new Size(52, 20);
            jiraloginlabel.TabIndex = 10;
            jiraloginlabel.Text = "Логин";
            // 
            // StatusTextOutputLabel
            // 
            StatusTextOutputLabel.AutoSize = true;
            StatusTextOutputLabel.Location = new Point(34, 451);
            StatusTextOutputLabel.Name = "StatusTextOutputLabel";
            StatusTextOutputLabel.Size = new Size(272, 20);
            StatusTextOutputLabel.TabIndex = 11;
            StatusTextOutputLabel.Text = "Вывод дополнительной информации";
            StatusTextOutputLabel.Click += label_errors;
            // 
            // ErrorOutputTextBox
            // 
            ErrorOutputTextBox.Location = new Point(34, 486);
            ErrorOutputTextBox.Margin = new Padding(3, 4, 3, 4);
            ErrorOutputTextBox.Multiline = true;
            ErrorOutputTextBox.Name = "ErrorOutputTextBox";
            ErrorOutputTextBox.Size = new Size(350, 101);
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
            FileExplorerButton.Location = new Point(755, 515);
            FileExplorerButton.Margin = new Padding(3, 4, 3, 4);
            FileExplorerButton.Name = "FileExplorerButton";
            FileExplorerButton.Size = new Size(123, 31);
            FileExplorerButton.TabIndex = 14;
            FileExplorerButton.Text = "Выбор папки";
            FileExplorerButton.UseVisualStyleBackColor = true;
            FileExplorerButton.Click += FileExplorerButton_Click;
            // 
            // FileDirectoryName
            // 
            FileDirectoryName.Location = new Point(430, 462);
            FileDirectoryName.Margin = new Padding(3, 4, 3, 4);
            FileDirectoryName.Name = "FileDirectoryName";
            FileDirectoryName.Size = new Size(448, 27);
            FileDirectoryName.TabIndex = 16;
            FileDirectoryName.TextChanged += textBox3_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(430, 420);
            label3.Name = "label3";
            label3.Size = new Size(243, 20);
            label3.TabIndex = 17;
            label3.Text = "Выбор папки для загрузки отчета";
            label3.Click += label3_Click_1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(34, 295);
            label4.Name = "label4";
            label4.Size = new Size(94, 20);
            label4.TabIndex = 19;
            label4.Text = "Дата начала";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(34, 369);
            label5.Name = "label5";
            label5.Size = new Size(121, 20);
            label5.TabIndex = 20;
            label5.Text = "Дата окончания";
            label5.Click += label5_Click;
            // 
            // checkedListBox1
            // 
            projectKeysList.FormattingEnabled = true;
            projectKeysList.Items.AddRange(new object[] { "GP", "GPREGION", "GPBOX" });
            projectKeysList.Location = new Point(297, 68);
            projectKeysList.Name = "checkedListBox1";
            projectKeysList.Size = new Size(150, 114);
            projectKeysList.TabIndex = 21;
            projectKeysList.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged_1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(297, 32);
            label6.Name = "label6";
            label6.Size = new Size(193, 20);
            label6.TabIndex = 22;
            label6.Text = "Выбор проектов выгрузки";
            label6.Click += label6_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(label6);
            Controls.Add(projectKeysList);
            Controls.Add(button1);
            Controls.Add(PrintReportButton);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(dateTimeIssuesStartInput);
            Controls.Add(FileDirectoryName);
            Controls.Add(dateTimeIssuesEndInput);
            Controls.Add(FileExplorerButton);
            Controls.Add(ErrorOutputTextBox);
            Controls.Add(StatusTextOutputLabel);
            Controls.Add(jiraloginlabel);
            Controls.Add(Пароль);
            Controls.Add(label2);
            Controls.Add(JiraFilterTextInput);
            Controls.Add(JiraLoginButton);
            Controls.Add(PasswordJiraTextInput);
            Controls.Add(LoginTextInput);
            Margin = new Padding(3, 4, 3, 4);
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
        private Button button1;
        private Label label5;
        private Label label4;
        private Label label6;
        private CheckedListBox projectKeysList;
    }
}
