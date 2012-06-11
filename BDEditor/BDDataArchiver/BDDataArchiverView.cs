using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace BDDataArchiver
{
    public partial class BDDataArchiverView : Form
    {
        private const string BD_ACCESS_KEY = @"AKIAJ6SRLQLH2ALT7ZBQ";
        private const string BD_SECRET_KEY = @"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";

        private AmazonS3 s3 = null;

        public BDDataArchiverView()
        {
            InitializeComponent();
        }

        public AmazonS3 S3
        {
            get
            {
                if (null == s3) { s3 = Amazon.AWSClientFactory.CreateAmazonS3Client(BD_ACCESS_KEY, BD_SECRET_KEY); }
                return s3;
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "BD Data files (*.sdf)|*.sdf";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.FileName = "BDDataStore.sdf";
            openFileDialog1.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Saint Street");

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileInfo fi = new FileInfo(openFileDialog1.FileName);
                    lblSource.Text = openFileDialog1.FileName;
                    if (lblSource.Text.Trim() != string.Empty)
                        btnArchive.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            if (lblSource.Text.Trim() != string.Empty)
            {
                this.Cursor = Cursors.WaitCursor;
                FileInfo fi = new FileInfo(lblSource.Text);
                if (fi.Exists)
                {
                    DateTime date = DateTime.Now;
                    string filename = fi.Name.Replace(fi.Extension, "");
                    string context = "A";
#if DEBUG
                context = "A.DEBUG";
#endif
                    filename = string.Format("{0}.{1}.{2}{3}.gz", filename, context, date.ToString("yyyMMdd-HHmmss"), fi.Extension);
                    lblOutput.Text = filename;

                    using (FileStream inFile = fi.OpenRead())
                    {
                        // Prevent compressing hidden and already compressed files.
                        if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden)
                                != FileAttributes.Hidden & fi.Extension != ".gz")
                        {
                            // Create the compressed file.

                            using (var memoryStream = new MemoryStream())
                            {
                                using (GZipStream Compress = new GZipStream(memoryStream, CompressionMode.Compress, true))
                                {
                                    // Copy the source file into the compression stream.
                                    byte[] buffer = new byte[4096];
                                    int numRead;
                                    while ((numRead = inFile.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        Compress.Write(buffer, 0, numRead);
                                    }
                                    //Console.WriteLine("Compressed {0} from {1} to {2} bytes.", fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                                }

                                PutObjectRequest putObjectRequest = new PutObjectRequest();
                                putObjectRequest
                                    .WithBucketName("bdArchive")
                                    .WithKey(filename)
                                    .WithInputStream(memoryStream);

                                S3Response s3Response = S3.PutObject(putObjectRequest);
                                s3Response.Dispose();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cannot archive previously archived files");
                        }
                    }
                    MessageBox.Show("Complete", "Archive to Repository");
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void BDDataArchiverView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}", "BD Data Archiver", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
#if DEBUG
            this.Text = string.Format("{0} <DEBUG>", this.Text);
#endif
        }   

    }
}

