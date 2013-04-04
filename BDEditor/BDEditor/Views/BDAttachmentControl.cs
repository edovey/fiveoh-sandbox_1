using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using BDEditor.DataModel;
using BDEditor.Classes;

namespace BDEditor.Views
{
    public partial class BDAttachmentControl : BDNodeControl
    {
        //private System.Windows.Forms.PictureBox attachmentPictureBox;

        public BDAttachmentControl()
        {
            InitializeComponent();
            LayoutControls();
        }

        public BDAttachmentControl(Entities pDataContext, IBDNode pNode): base(pDataContext, pNode)
        {
            InitializeComponent();
            LayoutControls();
        }
        private void LayoutControls()
        {
            //this.attachmentPictureBox = new System.Windows.Forms.PictureBox();
            //this.attachmentPictureBox.Image = global::BDEditor.Properties.Resources.bdy_090s;
            //this.attachmentPictureBox.Location = new System.Drawing.Point(7, 69);
            //this.attachmentPictureBox.Name = "attachemtnPictureBox";
            //this.attachmentPictureBox.Size = new System.Drawing.Size(863, 77);
            //this.attachmentPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            //this.attachmentPictureBox.TabIndex = 4;
            //this.attachmentPictureBox.TabStop = false;
            //this.attachmentPictureBox.Padding = new System.Windows.Forms.Padding(3);
            //this.attachmentPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.attachmentPictureBox.MouseDown += new MouseEventHandler(attachmentSurface_MouseDown);

            //base.pnlOverview.Size = new System.Drawing.Size(870, 225);
            //base.pnlOverview.Controls.Add(this.attachmentPictureBox);
            //base.pnlOverview.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
        }

        public override void RefreshLayout(bool pShowChildren)
        {
            Boolean origState = BDCommon.Settings.IsUpdating;
            BDCommon.Settings.IsUpdating = true;

            base.RefreshLayout(pShowChildren);
            ControlHelper.SuspendDrawing(this);

            if (null != this.currentNode)
            {
                BDAttachment attachment = this.currentNode as BDAttachment;
                if (null != attachment)
                {
                    lblFilename.Text = attachment.filename;

                    if (null == attachment.attachmentData)
                    {
                        attachmentPictureBox.Image = null;
                    }
                    else
                    {
                        BDConstants.BDAttachmentMimeType mimeType = BDConstants.BDAttachmentMimeType.unsupported;
                        if(null != attachment.attachmentMimeType)
                        {
                            mimeType = (BDConstants.BDAttachmentMimeType)Enum.Parse(typeof(BDConstants.BDAttachmentMimeType), attachment.attachmentMimeType.ToString(), true);
                        }
                        switch (mimeType)
                        {
                            case BDConstants.BDAttachmentMimeType.unsupported:
                            case BDConstants.BDAttachmentMimeType.unknown:
                                break;
                            case BDConstants.BDAttachmentMimeType.pdf:
                                attachmentPictureBox.Image = global::BDEditor.Properties.Resources.document;
                                break;
                            default:
                                Image attachmentImage = System.Drawing.Image.FromStream(new System.IO.MemoryStream(attachment.attachmentData));
                                attachmentPictureBox.Image = attachmentImage;
                                break;
                        }

                    }
                }
            }
            ControlHelper.ResumeDrawing(this);
            BDCommon.Settings.IsUpdating = origState;
        }

        private void attachmentSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.attachmentSurfaceContextMenuStrip.Show(attachmentPictureBox, e.Location);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != this.currentNode)
            {
                BDAttachment attachment = this.currentNode as BDAttachment;
                if (null != attachment)
                {
                    attachment.attachmentMimeType = null;
                    attachment.filename = null;
                    attachment.attachmentData = null;
                    attachment.filesize = 0;
                    Console.WriteLine("filesize = {0}", attachment.filesize);
                    BDAttachment.Save(dataContext, attachment);

                    lblFilename.Text = string.Empty;
                    attachmentPictureBox.Image = null;
                }
            }
        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png|JPEG (*.jpg)|*.jpg|Portable Document Format (*.pdf)|*.pdf|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.FileName = string.Empty;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(openFileDialog1.FileName);
                if (null != this.currentNode)
                {
                    BDAttachment attachment = this.currentNode as BDAttachment;
                    if (null != attachment)
                    {
                        string mimeType = BDUtilities.GetMIMEType(fi);
                        Console.WriteLine("Mime Type = {0}", mimeType);
                        
                        BDConstants.BDAttachmentMimeType attachmentMimeType = BDConstants.BDAttachmentMimeType.unsupported;

                        var values = Enum.GetValues(typeof(BDConstants.BDAttachmentMimeType));
                        foreach (var value in values)
                        {
                            BDConstants.BDAttachmentMimeType entry = (BDConstants.BDAttachmentMimeType)Enum.ToObject(typeof(BDConstants.BDAttachmentMimeType), value);
                            if (BDUtilities.GetEnumDescription(entry) == mimeType)
                            {
                                attachmentMimeType = entry;
                                break;
                            }
                        }

                        if ((attachmentMimeType != BDConstants.BDAttachmentMimeType.unsupported) && (attachmentMimeType != BDConstants.BDAttachmentMimeType.unknown))
                        {
                            attachment.attachmentMimeType = (int)attachmentMimeType;
                            attachment.filename = fi.Name;
                            lblFilename.Text = fi.Name;

                            byte[] attachmentData = BDUtilities.ReadFileAsByteArray(fi.FullName);
                            attachment.attachmentData = attachmentData;
                            attachment.filesize = attachmentData.Length;
                            Console.WriteLine("filesize = {0}", attachment.filesize);
                            BDAttachment.Save(dataContext, attachment);

                            if (attachmentMimeType == BDConstants.BDAttachmentMimeType.pdf)
                            {
                                attachmentPictureBox.Image = global::BDEditor.Properties.Resources.document;
                            }
                            else
                            {
                                Image attachmentImage = System.Drawing.Image.FromStream(new System.IO.MemoryStream(attachment.attachmentData));

                                attachmentPictureBox.Image = attachmentImage;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unable to attach", "Invalid file type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != this.currentNode)
            {
                BDAttachment attachment = this.currentNode as BDAttachment;
                if ((null != attachment) && (null != attachment.attachmentData))
                {
                    saveFileDialog1.FileName = attachment.filename;
                    saveFileDialog1.OverwritePrompt = true;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        byte[] attachmentData = attachment.attachmentData;
                        string filename = saveFileDialog1.FileName;
                        BDUtilities.WriteByteArrayAsFile(filename, attachmentData);
                    }
                }
            }
        }

        private void editIndexStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (null != menuItem)
            {
                BDNodeWrapper nodeWrapper = menuItem.Tag as BDNodeWrapper;
                if (null != nodeWrapper)
                {
                    BDSearchEntryEditView indexEditView = new BDSearchEntryEditView();
                    indexEditView.AssignDataContext(dataContext);
                    indexEditView.AssignCurrentNode(nodeWrapper.Node);
                    string contextString = BDUtilities.BuildHierarchyString(dataContext, nodeWrapper.Node, " : ");
                    indexEditView.DisplayContext = contextString;
                    indexEditView.ShowDialog(this);

                }
            }

        }
    }
}
