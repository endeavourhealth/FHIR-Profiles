using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FhirProfilePublisher.Engine;
using System.IO;

namespace FhirProfilePublisher
{
    public partial class FhirProfilePublisherDialog : Form
    {
        public FhirProfilePublisherDialog()
        {
            InitializeComponent();

            this.textBox1.Text = Properties.Settings.Default.OutputPath;
            
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
                this.textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            this.tbFileList.Text = Properties.Settings.Default.InputFileList;
        }

        private void tbBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Filter = "XML files (*.xml)|*.xml";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    AddFilesToFileList(dialog.FileNames);
            }
        }

        private void tbClear_Click(object sender, EventArgs e)
        {
            tbFileList.Clear();
        }

        private void tbFileList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Link;
        }

        private void tbFileList_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddFilesToFileList(files);
        }

        private void AddFilesToFileList(string[] files)
        {
            foreach (string file in files)
                if ((file ?? string.Empty).ToLower().EndsWith(".xml"))
                    if (!tbFileList.Lines.Contains(file))
                        tbFileList.Text += file + Environment.NewLine;

            if (string.IsNullOrEmpty(textBox1.Text))
                if (tbFileList.Lines.Length > 0)
                    textBox1.Text = Path.GetDirectoryName(tbFileList.Lines.First());
        }

        private void tbGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.Enabled = false;
                this.Refresh();

                TextContent content = new TextContent()
                {
                    HeaderText = tbHeadingText.Text,
                    PageTitleSuffix = tbPageTitlePrefix.Text,
                    FooterText = tbFootingText.Text,
                    IndexPageHtml = tbIndexPageHtml.Text
                };

                HtmlGenerator generator = new HtmlGenerator();
                string htmlFilePath = generator.Generate(tbFileList.Lines, textBox1.Text, content);

                if (cbOpenBrowser.Checked)
                    WebHelper.LaunchBrowser(htmlFilePath);
            }
            catch (ReferenceNotFoundException rnfe)
            {
                MessageBox.Show(this, 
                "Could not publish profiles because of the following error:" + Environment.NewLine + Environment.NewLine + rnfe.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                this.Enabled = true;
            }
        }

        private void tbBrowseOutputPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                    dialog.SelectedPath = textBox1.Text;
                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    textBox1.Text = dialog.SelectedPath;
            }
        }

        private void tbFileList_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.InputFileList = tbFileList.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutputPath = textBox1.Text;
            Properties.Settings.Default.Save();
        }
    }
}
