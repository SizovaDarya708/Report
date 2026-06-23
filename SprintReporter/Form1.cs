using JiraInteraction;
using Reporter;
using Reporter.Reports;
using System.Media;

namespace SprintReporter
{
    public partial class Form1 : Form
    {
        private IJiraService jiraService;
        private ISprintReportService sprintReportService;
        private IKpiReportService kpiReportService;

        public Form1()
        {
            InitializeComponent();
            DefaultValuesInitialization();
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
                if (!IsValidForm())
                {
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
                        ProjectKeys = projectKeysList.CheckedItems.Cast<string>().ToList()
                    });
                await reportTask;
                SystemSounds.Beep.Play();
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

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private async void button_print_kpi_1(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidForm())
                {
                    return;
                }

                kpiReportService = new KpiReportService(jiraService);
                var reportTask = kpiReportService.ExecuteAsync(
                    new JiraInteraction.Dtos.KpiReportInput
                    {
                        StartDate = dateTimeIssuesStartInput.Value,
                        EndDate = dateTimeIssuesEndInput.Value,
                        AdditionalJiraFilter = JiraFilterTextInput.Text,
                        FilePath = FileDirectoryName.Text,
                        ProjectKeys = projectKeysList.CheckedItems.Cast<string>().ToList()
                    });
                await reportTask;
                SystemSounds.Beep.Play();
                ErrorOutputTextBox.Text = "Печать завершена";
            }
            catch (Exception ex)
            {
                ErrorOutputTextBox.Text = ex.Message;
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dateTimeIssuesEndInput_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimeIssuesStartInput_ValueChanged(object sender, EventArgs e)
        {

        }

        private bool IsValidForm()
        {
            ErrorOutputTextBox.Text = string.Empty;
            return IsValidAuthorization() && IsValidFileDirectoryName() && IsValidProjectKeysCheckedList();
        }

        private bool IsValidAuthorization()
        {
            if (jiraService == null)
            {
                ErrorOutputTextBox.Text = "Необходима авторизация в Jira";
                return false;
            }
            return true;
        }

        private bool IsValidFileDirectoryName()
        {
            if (FileDirectoryName.Text == string.Empty || FileDirectoryName == null)
            {
                ErrorOutputTextBox.Text = "Выберите папку для загрузки отчета";
                return false;
            }
            return true;
        }

        private bool IsValidProjectKeysCheckedList()
        {
            if (projectKeysList.CheckedItems.Count == 0)
            {
                ErrorOutputTextBox.Text = "Необходимо выбрать хотя бы один проект для выгрузки";
                return false;
            }
            return true;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void DefaultValuesInitialization()
        {
            for (int i = 0; i < projectKeysList.Items.Count; i++)
            {
                projectKeysList.SetItemChecked(i, true);
            }

            var currentDate = DateTime.Now;
            dateTimeIssuesStartInput.Value = new DateTime(day: 1, month: currentDate.Month, year: currentDate.Year);
        }
    }
}
