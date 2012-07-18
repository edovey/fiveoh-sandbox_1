using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Globalization;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;
using BDEditor.Views;

namespace BDDataArchiver
{
    public partial class BDDataArchiverView : Form
    {
        public const string AWS_PROD_ARCHIVE = @"bdProdArchive";
        public const string AWS_DEV_ARCHIVE = @"bdDevArchive";
#if DEBUG
        public const string AWS_ARCHIVE = AWS_DEV_ARCHIVE;
#else
        public const string AWS_ARCHIVE = AWS_PROD_ARCHIVE;
#endif
        public const string SOURCE_METADATA = @"x-amz-meta-source";
        public const string FILENAME_METADATA = @"x-amz-meta-filename";
        public const string PATH_METADATA = @"x-amz-meta-path";
        public const string MACHINENAME_METADATA = @"x-amz-meta-machinename";
        public const string USER_METADATA = @"x-amz-meta-user";
        public const string COMMENT_METADATA = @"x-amz-meta-comment";
        public const string TAG_METADATA = @"x-amz-meta-tag";
        public const string CREATEDATE_METADATA = @"x-amz-meta-createdate";
        public const string APPVERSION_METADATA = @"x-amz-meta-appversion";

        private const string BD_ACCESS_KEY = @"AKIAJ6SRLQLH2ALT7ZBQ";
        private const string BD_SECRET_KEY = @"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";

        private AmazonS3 _s3 = null;

        private string dataFolder = string.Empty;

        public BDDataArchiverView()
        {
            InitializeComponent();
        }

        public AmazonS3 S3
        {
            get
            {
                if (null == _s3) { _s3 = Amazon.AWSClientFactory.CreateAmazonS3Client(BD_ACCESS_KEY, BD_SECRET_KEY); }
                return _s3;
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
                    {
                        btnArchive.Enabled = true;
                        dataFolder = fi.DirectoryName;
                    }
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
                // BDArchiveDialog archiveDialog = new BDArchiveDialog();
                //if (archiveDialog.ShowDialog(this) == DialogResult.OK)
                //{
                //    this.Cursor = Cursors.WaitCursor;
                //    Application.DoEvents();
                    Archive(lblSource.Text, AWS_ARCHIVE, txtName.Text, txtComment.Text, false);
                    MessageBox.Show("Archive complete", "Achive to Repository", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

                this.Cursor = Cursors.Default;
            }
        }

        public void Archive(string pFilename, string pBucketName, string pUserName, string pComment, Boolean pIsBackup)
        {
            DateTime archiveDateTime = DateTime.Now;

            Uri uri = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
            DirectoryInfo di = new DirectoryInfo(uri.AbsolutePath);
            string path = uri.AbsolutePath.Replace("%20", " ");
            FileInfo sourceFi = new FileInfo(pFilename);

            if (sourceFi.Exists)
            {
                string filename = sourceFi.Name.Replace(sourceFi.Extension, "");
                string context = "A.prod";
#if DEBUG
                context = "A.DEBUG";
#endif
                if (pIsBackup) context = string.Format("{0}.backup", context);

                filename = string.Format("{0}.{1}.{2}{3}.gz", filename, context, archiveDateTime.ToString("yyyMMdd-HHmmss"), sourceFi.Extension);

                using (FileStream inFile = sourceFi.OpenRead())
                {
                    // Prevent compressing hidden and already compressed files.
                    if ((File.GetAttributes(sourceFi.FullName) & FileAttributes.Hidden)
                            != FileAttributes.Hidden & sourceFi.Extension != ".gz")
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

                            NameValueCollection metadata = new NameValueCollection();
                            metadata.Add(SOURCE_METADATA, pBucketName);
                            metadata.Add(MACHINENAME_METADATA, Environment.MachineName);
                            metadata.Add(FILENAME_METADATA, sourceFi.Name);
                            metadata.Add(PATH_METADATA, sourceFi.DirectoryName);
                            metadata.Add(USER_METADATA, pUserName);
                            metadata.Add(COMMENT_METADATA, pComment);
                            metadata.Add(TAG_METADATA, @"Standalone Archiver");
                            metadata.Add(CREATEDATE_METADATA, archiveDateTime.ToString("s"));
                            metadata.Add(APPVERSION_METADATA, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                            PutObjectRequest putObjectRequest = new PutObjectRequest();
                            putObjectRequest
                                .WithBucketName(pBucketName)
                                .WithKey(filename)
                                .WithInputStream(memoryStream)
                                .AddHeaders(metadata);

                            S3Response s3Response = S3.PutObject(putObjectRequest);
                            s3Response.Dispose();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Cannot archive previously archived files");
                    }
                }
            }
        }

        private void BDDataArchiverView_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}", "BD Data Archiver", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
#if DEBUG
            this.Text = string.Format("{0} <DEBUG>", this.Text);
#endif
        }

        private void ListArchives(object sender, EventArgs e)
        {
            List<BDArchiveRecord> archiveList = new List<BDArchiveRecord>();

            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = AWS_ARCHIVE;
                do
                {
                    ListObjectsResponse response = S3.ListObjects(request);

                    foreach (S3Object entry in response.S3Objects)
                    {
                        Console.WriteLine("key = {0} size = {1}", entry.Key, entry.Size);
                        GetObjectMetadataRequest metadataRequest = new GetObjectMetadataRequest();
                        metadataRequest.WithBucketName(AWS_ARCHIVE).WithKey(entry.Key);
                        GetObjectMetadataResponse metadataResponse = S3.GetObjectMetadata(metadataRequest);

                        BDArchiveRecord archiveRecord = new BDArchiveRecord();
                        archiveRecord.Key = entry.Key;
                        
                        foreach (string key in metadataResponse.Metadata.AllKeys)
                        {
                            switch(key)
                            {
                                case FILENAME_METADATA:
                                    archiveRecord.Filename = metadataResponse.Metadata[key];
                                    break;
                                case CREATEDATE_METADATA:
                                    archiveRecord.CreateDate = DateTime.Parse(metadataResponse.Metadata[key], null, DateTimeStyles.RoundtripKind);
                                    break;
                                case COMMENT_METADATA:
                                    archiveRecord.Comment = metadataResponse.Metadata[key];
                                    break;
                                case USER_METADATA:
                                    archiveRecord.Username = metadataResponse.Metadata[key];
                                    break;
                                default:
                                    break;
                            }
                        }

                        archiveList.Add(archiveRecord);
                    }
                    // If response is truncated, set the marker to get the next 
                    // set of keys.
                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);


                archiveList.Sort();

                listBoxArchives.Items.Clear();
                listBoxArchives.Items.AddRange(archiveList.ToArray());


            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                    "To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                     "Error occurred. Message:'{0}' when listing objects",
                     amazonS3Exception.Message);
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (listBoxArchives.SelectedIndex >= 0)
            {
                Boolean error = false;
                BDArchiveRecord archiveRecord = listBoxArchives.SelectedItem as BDArchiveRecord;
                if (null != archiveRecord)
                {
                    if (MessageBox.Show(string.Format("Overwrite local data with file dated {0} from the repository?", archiveRecord.CreateDate.ToString("u")), "Restore Database", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                    {
                        FileInfo targetFi = new FileInfo(lblSource.Text);

                        GetObjectRequest getObjectRequest = new GetObjectRequest().WithBucketName(AWS_ARCHIVE).WithKey(archiveRecord.Key);

                        using (GetObjectResponse response = S3.GetObject(getObjectRequest))
                        {
                            //response.WriteResponseStreamToFile(targetFi.FullName);
                            using (Stream s = response.ResponseStream)
                            {
                                //Create the decompressed file.
                                try
                                {
                                    using (FileStream outFile = File.Create(targetFi.FullName))
                                    {
                                        using (GZipStream Decompress = new GZipStream(s, CompressionMode.Decompress))
                                        {
                                            // Copy the decompression stream 
                                            // into the output file.
                                            Decompress.CopyTo(outFile);

                                            Console.WriteLine("Decompressed: {0}", targetFi.FullName);
                                        }
                                    }
                                }
                                catch (IOException ioex)
                                {
                                    error = true;
                                    MessageBox.Show("Destination file is in use.\nPlease close the content editor and try again", "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                        }
                        if(!error)
                            MessageBox.Show("Restore complete", "Overwrite from Repository");
                    }
                }
            }
        }

        private void listBoxArchives_Click(object sender, EventArgs e)
        {
            btnRestore.Enabled = ((listBoxArchives.SelectedIndex >= 0) && (lblSource.Text.Trim() != string.Empty));
        }

        private void lblSource_Click(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            btnArchive.Enabled = (lblSource.Text.Trim().Length > 0) && (txtName.Text.Trim().Length > 0) && (txtComment.Text.Trim().Length > 0);
        }

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            btnArchive.Enabled = (lblSource.Text.Trim().Length > 0) && (txtName.Text.Trim().Length > 0) && (txtComment.Text.Trim().Length > 0);
        }   

    }

    public class BDArchiveRecord: IComparable
    {
        public string Key { get; set; }
        public DateTime CreateDate { get; set; }
        public string Comment { get; set; }
        public string Filename { get; set; }
        public string Username { get; set; }

        public BDArchiveRecord() { }

        public int CompareTo(object obj)
        {
            if (obj is BDArchiveRecord)
            {
                BDArchiveRecord temp = (BDArchiveRecord)obj;

                return temp.CreateDate.CompareTo(this.CreateDate);
            }

            throw new ArgumentException("object is not a BDArchiveRecord");
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", CreateDate.ToString("ddd yyy MMM dd HH:mm:ss"), Comment);
        }
    }
}

