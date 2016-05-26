using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BDEditor.Views
{
    public partial class BDWebPreviewView : Form
    {
        private string innerHtml;

        public BDWebPreviewView()
        {
            InitializeComponent();
        }

        public string previewHtml
        {
            get { return innerHtml; }

            set { innerHtml = value; }
        }

        private void BDWebPreviewView_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("about:blank");
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.Write(string.Empty);
            }
            webBrowser1.DocumentText = innerHtml;
        }

    }
}
