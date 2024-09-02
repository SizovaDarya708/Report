using JiraInteraction;
using Reporter;

namespace SprintReporter
{
    public partial class Form1 : Form
    {
        private IJiraService jiraService;
        private ISprintReportService sprintReportService;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void JiraLoginButton_Click(object sender, EventArgs e)
        {
            ErrorOutputTextBox.Text = string.Empty;
            jiraService = new JiraClientService(new JiraInteraction.Dtos.JiraClientInitData
            {
                JiraLogin = LoginTextInput.Text,
                JiraPassword = PasswordJiraTextInput.Text,
            });

            var isSuccessLogTask = jiraService.CheckClientConnection();
            var isSuccessLog = await isSuccessLogTask;
            if (isSuccessLog)
            {
                JiraLoginButton.BackColor = Color.Green;
                JiraLoginButton.Text = "Вы авторизованы";
            }
            else
            {
                JiraLoginButton.BackColor = Color.Red;
            }
        }


        private async void PrintReportButton_Click(object sender, EventArgs e)
        {
            try
            {
                ErrorOutputTextBox.Text = string.Empty;
                if (jiraService == null)
                {
                    ErrorOutputTextBox.Text = "Необходима авторизация в Jira";
                    return;
                }

                sprintReportService = new SprintReportService(jiraService);
                var reportTask = sprintReportService.ExecuteAsync(
                    new JiraInteraction.Dtos.SprintIssuesDataInput
                    {
                        StartDate = dateTimeIssuesStartInput.Value,
                        EndDate = dateTimeIssuesEndInput.Value,
                        AdditionalJiraFilter = JiraFilterTextInput.Text,
                        FilePath = FileDirectoryName.Text,
                    });
                await reportTask;

                ErrorOutputTextBox.Text = "Печать завершена";
            }
            catch (Exception ex)
            {
                ErrorOutputTextBox.Text = ex.Message;
            }
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label_errors(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ErrorOutputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void LoginTextInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void FileExplorerButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                FileDirectoryName.Text = fdlg.SelectedPath;
            }
        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
