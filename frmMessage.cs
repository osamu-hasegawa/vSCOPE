﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vSCOPE
{
	public partial class frmMessage : Form
	{
		public frmMessage()
		{
			InitializeComponent();
		}

		private void frmMessage_Load(object sender, EventArgs e)
		{
			this.Text = Application.ProductName;
			this.pictureBox1.Image = SystemIcons.Information.ToBitmap();
			if (G.SS.ETC_UIF_LEVL == 0 || G.SS.ETC_UIF_LEVL == 1) {
			G.SS.ETC_UIF_BACK = G.SS.ETC_UIF_LEVL;
			G.SS.ETC_UIF_LEVL = 2;
			this.label1.Text = "ソフトウェアは次回起動時に開発者モードで起動します。";
			}
			else {
			G.SS.ETC_UIF_LEVL = G.SS.ETC_UIF_BACK;
			this.label1.Text = "ソフトウェアは次回起動時にユーザモードで起動します。";
			}
		}
	}
}
