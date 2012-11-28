using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;
using System.Windows.Forms;
using BDEditor.DataModel;

namespace BDEditor.Classes
{
    public class RepositoryHandler
    {
        public const string AWS_PROD_ARCHIVE = @"bdProdArchive";
        public const string AWS_DEV_ARCHIVE = @"bdDevArchive";
        public const string AWS_PROD_BACKUP = @"bdProdRestoreBackup";
        public const string AWS_DEV_BACKUP = @"bdDevRestoreBackup";
#if DEBUG
        public const string AWS_ARCHIVE = AWS_DEV_ARCHIVE;
        public const string AWS_BACKUP = AWS_DEV_BACKUP;
#else
        public const string AWS_ARCHIVE = AWS_PROD_ARCHIVE;
        public const string AWS_BACKUP = AWS_PROD_BACKUP;
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
        public const string MIMETYPE_METADATA = @"x-amz-meta-mimetype";

        private const string BD_ACCESS_KEY = @"AKIAJ6SRLQLH2ALT7ZBQ";
        private const string BD_SECRET_KEY = @"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";

        static private readonly RepositoryHandler aws = new RepositoryHandler();

        private AmazonSimpleDB simpleDb = null;
        private AmazonS3 s3 = null;

        private List<SyncInfo> syncInfoList = new List<SyncInfo>();

        private RepositoryHandler() { }

        static public RepositoryHandler Aws
        {
            get { return aws; }
        }

        public AmazonSimpleDB SimpleDb
        {
            get
            { 
                AmazonSimpleDBConfig sdbConfig = new AmazonSimpleDBConfig();
                sdbConfig.MaxErrorRetry = 15;

                if (null == simpleDb) { simpleDb = AWSClientFactory.CreateAmazonSimpleDBClient(BD_ACCESS_KEY, BD_SECRET_KEY, sdbConfig); }
                
                return simpleDb;
            }
        }

        public AmazonS3 S3
        {
            get
            {
                AmazonS3Config s3Config = new AmazonS3Config();
                s3Config.MaxErrorRetry = 15;

                if (null == s3) { s3 = Amazon.AWSClientFactory.CreateAmazonS3Client(BD_ACCESS_KEY, BD_SECRET_KEY, s3Config); }
                return s3;
            }
        }

        private SyncInfoDictionary InitializeSyncDictionary(Entities pDataContext, Boolean pCreateMissing, BDConstants.SyncType pSyncType)
        {
            SyncInfoDictionary syncDictionary = new SyncInfoDictionary();

            if (pSyncType == BDConstants.SyncType.Publish)
            {
                syncDictionary.Add(BDHtmlPage.SyncInfo(pDataContext));
                syncDictionary.Add(BDSearchEntry.SyncInfo(pDataContext));
                syncDictionary.Add(BDSearchEntryAssociation.SyncInfo(pDataContext));
                syncDictionary.Add(BDAttachment.SyncInfo(pDataContext));
            }

            // List the remote domains
            ListDomainsResponse sdbListDomainsResponse = SimpleDb.ListDomains(new ListDomainsRequest());
            if (sdbListDomainsResponse.IsSetListDomainsResult())
            {
                ListDomainsResult listDomainsResult = sdbListDomainsResponse.ListDomainsResult;

                foreach (SyncInfo sInfo in syncDictionary.Values)
                {
                    if (listDomainsResult.DomainName.Contains(sInfo.RemoteDevelopmentEntityName))
                        sInfo.ExistsOnRemoteDevelopment = true;
                    if(listDomainsResult.DomainName.Contains(sInfo.RemoteProductionEntityName))
                        sInfo.ExistsOnRemoteProduction = true;
                    if (listDomainsResult.DomainName.Contains(sInfo.RemoteEntityName))
                        sInfo.ExistsOnRemote = true;
                }
            }

            if (pCreateMissing)
            {
                // Create missing domains
                foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
                {
                    if (!syncInfoEntry.ExistsOnRemote)
                    {
                        try
                        {
                            CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(syncInfoEntry.RemoteEntityName);
                            CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
                            syncInfoEntry.ExistsOnRemote = true;
                            System.Diagnostics.Debug.WriteLine(string.Format("Created domain for {0}", syncInfoEntry.RemoteEntityName));
                        }
                        catch (AmazonSimpleDBException ex)
                        {
                            syncInfoEntry.Exception = ex;
                            System.Diagnostics.Debug.WriteLine(string.Format("Failed to created domain for {0}", syncInfoEntry.RemoteEntityName));

                            // Try a second time
                            try
                            {
                                CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(syncInfoEntry.RemoteEntityName);
                                CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
                                syncInfoEntry.ExistsOnRemote = true;
                                System.Diagnostics.Debug.WriteLine(string.Format("Created domain for {0}", syncInfoEntry.RemoteEntityName));
                            }
                            catch (AmazonSimpleDBException ex2)
                            {
                                syncInfoEntry.Exception = ex2;
                                System.Diagnostics.Debug.WriteLine(string.Format("Failed (2nd) to created domain for {0}", syncInfoEntry.RemoteEntityName));
                            }
                        }
                    }
                }
            }


            return syncDictionary;
        }

        /// <summary>
        /// Synchronize with the Amazon SimpleDb
        /// </summary>
        /// <param name="pDataContext"></param>
        /// <param name="pLastSyncDate"></param>
        /// <returns></returns>
        public SyncInfoDictionary Sync(Entities pDataContext, DateTime? pLastSyncDate, BDConstants.SyncType pSyncType)
        {
            DateTime? currentSyncDate = DateTime.Now;

            // Create the SyncInfo instance and update the modified date of all the changed records to be the currentSyncDate
            SyncInfoDictionary syncDictionary = InitializeSyncDictionary(pDataContext,true, pSyncType);

            if (BDCommon.Settings.SyncPushEnabled)
            {
                #region Purge Remote

                // clear the remote tables / entities; clear all pages from S3
                DeleteRemoteHTMLPages();
                DeleteRemoteSearch();
                DeleteRemoteAttachments();
                #endregion

                foreach (SyncInfo syncInfoEntry in syncDictionary.Values)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Push {0}", syncInfoEntry.RemoteEntityName));
                    foreach (IBDObject changeEntry in syncInfoEntry.PushList)
                    {
                        SimpleDb.PutAttributes(changeEntry.PutAttributes());
                        syncInfoEntry.RowsPushed++;

                        if (changeEntry is BDHtmlPage)
                        {
                            BDHtmlPage htmlPage = changeEntry as BDHtmlPage;
                            PutObjectRequest putObjectRequest = new PutObjectRequest()
                                        .WithContentType(@"text/html")
                                        .WithContentBody(htmlPage.documentText)
                                        .WithBucketName(BDHtmlPage.AWS_BUCKET)
                                        .WithKey(htmlPage.storageKey);

                            S3Response s3Response = S3.PutObject(putObjectRequest);
                            s3Response.Dispose();
                        }
                        else if (changeEntry is BDAttachment)
                        {
                            BDAttachment attachmentEntry = (changeEntry as BDAttachment);
                            if (null != attachmentEntry.attachmentData)
                            {
                                byte[] attachmentData = attachmentEntry.attachmentData;
                                MemoryStream ms = new MemoryStream(attachmentData);
                                using (var memoryStream = new MemoryStream(attachmentData))
                                {
                                    NameValueCollection metadata = new NameValueCollection();
                                    metadata.Add(SOURCE_METADATA, BDAttachment.AWS_BUCKET);
                                    metadata.Add(MACHINENAME_METADATA, Environment.MachineName);
                                    metadata.Add(FILENAME_METADATA, attachmentEntry.filename);
                                    metadata.Add(MIMETYPE_METADATA, attachmentEntry.MimeType());
                                    metadata.Add(COMMENT_METADATA, (null != attachmentEntry.name) ? attachmentEntry.name : "" );
                                    metadata.Add(TAG_METADATA, @"");
                                    metadata.Add(CREATEDATE_METADATA, attachmentEntry.modifiedDate.Value.ToString("s"));
                                    metadata.Add(APPVERSION_METADATA, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                                    PutObjectRequest putObjectRequest = new PutObjectRequest();
                                    putObjectRequest
                                        .WithBucketName(BDAttachment.AWS_BUCKET)
                                        .WithKey(attachmentEntry.storageKey)
                                        .WithInputStream(memoryStream)
                                        .AddHeaders(metadata);

                                    S3Response s3Response = S3.PutObject(putObjectRequest);
                                    s3Response.Dispose();
                                }
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Pushed {0} Records for {1}", syncInfoEntry.RowsPushed, syncInfoEntry.RemoteEntityName);
                }
                pLastSyncDate = currentSyncDate;

                BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                systemSetting.settingDateTimeValue = currentSyncDate;
                pDataContext.SaveChanges();
            }

            return syncDictionary;
        }

        #region Helper Methods

        private AttributeDictionary ItemAttributesToDictionary(Amazon.SimpleDB.Model.Item pItem)
        {
            AttributeDictionary attributeDictionary = new AttributeDictionary();

            if (null != pItem)
            {
                foreach (Amazon.SimpleDB.Model.Attribute attribute in pItem.Attribute)
                {
                    attributeDictionary.Add(attribute);
                }
            }

            return attributeDictionary;
        }
        #endregion

        public void DeleteRemoteSearch()
        {
            try
            {
                DeleteDomainRequest saRequest = new DeleteDomainRequest().WithDomainName(BDSearchEntry.AWS_DOMAIN);
               
                SimpleDb.DeleteDomain(saRequest);
                DeleteDomainRequest seRequest = new DeleteDomainRequest().WithDomainName(BDSearchEntryAssociation.AWS_DOMAIN);
                SimpleDb.DeleteDomain(seRequest);

                CreateDomainRequest createDomainRequest1 = (new CreateDomainRequest()).WithDomainName(BDSearchEntry.AWS_DOMAIN);
                CreateDomainRequest createDomainRequest2 = (new CreateDomainRequest()).WithDomainName(BDSearchEntryAssociation.AWS_DOMAIN);
                CreateDomainResponse createResponse1 = simpleDb.CreateDomain(createDomainRequest1);
                CreateDomainResponse createResponse2 = simpleDb.CreateDomain(createDomainRequest2);
                System.Diagnostics.Debug.WriteLine("Remote Search Deleted");
            }
            catch (AmazonSimpleDBException amazonSdbException)
            {
                if (amazonSdbException.ErrorCode != null &&
                                    (amazonSdbException.ErrorCode.Equals("InvalidAccessKeyId")
                                    ||
                                    amazonSdbException.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine(
                    "To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                     "DeleteRemoteSearch Error occurred. Message:'{0}' ",
                     amazonSdbException.Message);
                }
            }

        }

        public void DeleteRemoteHTMLPages()
        {
            try
            {
                DeleteObjectsRequest s3DeleteHtmlPages = new DeleteObjectsRequest()
                    .WithBucketName(BDHtmlPage.AWS_BUCKET);
               ListObjectsRequest request = new ListObjectsRequest().WithBucketName(BDHtmlPage.AWS_BUCKET).WithPrefix(@"bdhp~");

                do
                {
                    ListObjectsResponse response = S3.ListObjects(request);

                    foreach (S3Object entry in response.S3Objects)
                    {
                        s3DeleteHtmlPages.AddKey(entry.Key);
                    }

                    if (s3DeleteHtmlPages.Keys.Count > 0)
                    {
                        try
                        {
                            S3.DeleteObjects(s3DeleteHtmlPages);
                        }
                        catch (DeleteObjectsException e)
                        {
                            var errorResponse = e.ErrorResponse;
                            Console.WriteLine("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
                            Console.WriteLine("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);
                            Console.WriteLine("Printing error data...");
                            foreach (DeleteError deleteError in errorResponse.DeleteErrors)
                            {
                                Console.WriteLine("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
                            }
                        }
                    }
                    DeleteDomainRequest htDomain = new DeleteDomainRequest().WithDomainName(BDHtmlPage.AWS_DOMAIN);
                    SimpleDb.DeleteDomain(htDomain);

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
                System.Diagnostics.Debug.WriteLine("Remote HTML Deleted");
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

            try
            {
                CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(BDHtmlPage.AWS_DOMAIN);
                CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
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

        public void DeleteRemoteAttachments()
        {
            try
            {
                DeleteObjectsRequest s3DeleteAttachments = new DeleteObjectsRequest()
                    .WithBucketName(BDAttachment.AWS_BUCKET);
                ListObjectsRequest request = new ListObjectsRequest().WithBucketName(BDAttachment.AWS_BUCKET).WithPrefix(@"bd~");

                do
                {
                    ListObjectsResponse response = S3.ListObjects(request);

                    foreach (S3Object entry in response.S3Objects)
                    {
                        s3DeleteAttachments.AddKey(entry.Key);
                    }

                    if (s3DeleteAttachments.Keys.Count > 0)
                    {
                        try
                        {
                            S3.DeleteObjects(s3DeleteAttachments);
                        }
                        catch (DeleteObjectsException e)
                        {
                            var errorResponse = e.ErrorResponse;
                            Console.WriteLine("No. of objects successfully deleted = {0}", errorResponse.DeletedObjects.Count);
                            Console.WriteLine("No. of objects failed to delete = {0}", errorResponse.DeleteErrors.Count);
                            Console.WriteLine("Printing error data...");
                            foreach (DeleteError deleteError in errorResponse.DeleteErrors)
                            {
                                Console.WriteLine("Object Key: {0}\t{1}\t{2}", deleteError.Key, deleteError.Code, deleteError.Message);
                            }
                        }
                    }
                    DeleteDomainRequest htDomain = new DeleteDomainRequest().WithDomainName(BDAttachment.AWS_DOMAIN);
                    SimpleDb.DeleteDomain(htDomain);

                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
                System.Diagnostics.Debug.WriteLine("Remote Attachments Deleted");
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

            try
            {
                CreateDomainRequest createDomainRequest = (new CreateDomainRequest()).WithDomainName(BDAttachment.AWS_DOMAIN);
                CreateDomainResponse createResponse = simpleDb.CreateDomain(createDomainRequest);
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

        public void Archive(Entities pDataContext, string pUserName, string pComment)
        {
            Archive(pDataContext, AWS_ARCHIVE, pUserName, pComment, false);
        }

        public void Archive(Entities pDataContext, string pBucketName, string pUserName, string pComment, Boolean pIsBackup)
        {
            DateTime archiveDateTime = DateTime.Now;

            Uri uri = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
            DirectoryInfo di = new DirectoryInfo(uri.AbsolutePath);
            string path = uri.AbsolutePath.Replace("%20", " ");
            FileInfo sourceFi = new FileInfo(Path.Combine(path, BDConstants.DB_FILENAME));

            if (sourceFi.Exists)
            {
                string filename = sourceFi.Name.Replace(sourceFi.Extension, "");
                string context = "prod";
#if DEBUG
                context = "DEBUG";
#endif
                if (pIsBackup) context = string.Format("{0}.backup", context);

                filename = string.Format("{0}.{1}.{2}{3}.gz", filename, context, archiveDateTime.ToString("yyyMMdd-HHmmss"), sourceFi.Extension);
                pDataContext.SaveChanges();
                pDataContext.Connection.Close();

                try
                {
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
                                metadata.Add(TAG_METADATA, @"");
                                metadata.Add(CREATEDATE_METADATA, archiveDateTime.ToString("s"));
                                metadata.Add(APPVERSION_METADATA, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                                try
                                {
                                    PutObjectRequest putObjectRequest = new PutObjectRequest();
                                    putObjectRequest
                                        .WithBucketName(pBucketName)
                                        .WithKey(filename)
                                        .WithInputStream(memoryStream)
                                        .AddHeaders(metadata);

                                    S3Response s3Response = S3.PutObject(putObjectRequest);
                                    s3Response.Dispose();
                                }
                                catch (AmazonS3Exception amazonS3Exception)             
                                {                 
                                    if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))                 
                                    {
                                        string errorMessage = string.Format("Please check the provided AWS Credentials.");
                                        MessageBox.Show(errorMessage);
                                    }                
                                    else                 
                                    {                 
                                        string errorMessage = string.Format("AWS message '{0}' when writing an object", amazonS3Exception.Message);
                                        MessageBox.Show(errorMessage);
                                    }             
                                } 
                            }

                        }
                        else
                        {
                            MessageBox.Show("Cannot archive previously archived files");
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format("General Message:{0}", ex.Message);
                    MessageBox.Show(errorMessage);
                }
                finally
                {
                    pDataContext.Connection.Open();
                }
                
                if (!pIsBackup)
                {
                    try
                    {
                        BDSystemSetting systemSetting = BDSystemSetting.RetrieveSetting(pDataContext, BDSystemSetting.LASTSYNC_TIMESTAMP);
                        systemSetting.settingDateTimeValue = archiveDateTime;
                        pDataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = string.Format("Notification on updating event date [{0}]", ex.Message);
                    }
                }
                
            }
        }

        public void Restore(Entities pDataContext, BDArchiveRecord pArchiveRecord)
        {
            Boolean error = false;

            if (null != pArchiveRecord)
            {
                Uri uri = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));

                DirectoryInfo di = new DirectoryInfo(uri.AbsolutePath);
                string path = uri.AbsolutePath.Replace("%20", " ");
                FileInfo targetFi = new FileInfo(Path.Combine(path, BDConstants.DB_FILENAME));

                if (MessageBox.Show(string.Format("Overwrite local data with file dated:\n\n{0}\n\"{1}\"\n\nfrom the repository?\n\n{2}", pArchiveRecord.CreateDate.ToString("u"), pArchiveRecord.Comment, pArchiveRecord.Key), "Restore Database", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    Archive(pDataContext, AWS_BACKUP, "", "Backup prior to restore", true);

                    GetObjectRequest getObjectRequest = new GetObjectRequest().WithBucketName(AWS_ARCHIVE).WithKey(pArchiveRecord.Key);

                    using (GetObjectResponse response = S3.GetObject(getObjectRequest))
                    {
                        //response.WriteResponseStreamToFile(targetFi.FullName);
                        using (Stream s = response.ResponseStream)
                        {
                            //Create the decompressed file.
                            try
                            {
                                pDataContext.Connection.Close();

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
                                MessageBox.Show("Destination file is in use.\nPlease close the content editor and use the standalone Archive utility.", "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            finally
                            {
                                pDataContext = new DataModel.Entities();
                            }
                        }
                    }
                    if (!error)
                        MessageBox.Show("Restore complete", "Overwrite from Repository",  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public List<BDArchiveRecord> ListArchives()
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
                            switch (key)
                            {
                                case FILENAME_METADATA:
                                    archiveRecord.Filename = metadataResponse.Metadata[key];
                                    break;
                                case CREATEDATE_METADATA:
                                    archiveRecord.CreateDate = DateTime.Parse(metadataResponse.Metadata[key]);
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

            return archiveList;
        }

    }
}
