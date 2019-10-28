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
	public partial class DlgProgress : Form
	{
		public static bool	m_bAlive;
		private Control m_owner;

		public DlgProgress()
		{
			InitializeComponent();
		}
//		public override void Show(string strTitle, Control owner)
		public void Show(string strTitle, Control owner)
		{
			if (strTitle == "@") {
				strTitle = " ";
				this.Cancel_Button.Visible = false;
				this.Label1.Top = this.Label1.Top + this.Cancel_Button.Height/2;
			}
			this.Text = strTitle;
			this.Label1.Text = "";
			//'If (Not Me.IsHandleCreated) Then
			//'    Me.CreateHandle()
			//'End If
			if (owner != null) {
					base.Show(owner);
					this.Left = owner.Right - this.Width - 5;
					this.Top = owner.Top + 5;
					owner.Enabled = false;
					m_owner = owner;
					Application.DoEvents();
					Application.DoEvents();
					Application.DoEvents();
			}
			else {
				this.StartPosition = FormStartPosition.CenterScreen;
				base.Show();
			}
		}
		public void SetStatus(string str)
		{
			this.Label1.Text = str;
			Application.DoEvents();
		}
		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			G.bCANCEL = true;
			//this.Close();
		}
		public bool CheckCancelButton()
		{
			if (G.bCANCEL) {
				G.bCANCEL = false;
				return (true);
			}
			return (false);
		}

		private void DlgProgress_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (m_owner != null) {
				m_owner.Enabled = true;
				m_owner = null;
			}
		}

		private void DlgProgress_VisibleChanged(object sender, EventArgs e)
		{
			if (this.Visible == false) {
				if (m_owner != null) {
					m_owner.Enabled = true;
					m_owner.BringToFront();
					m_owner = null;
				}
				this.Visible = false;
				m_bAlive = false;
			}
			else {
				m_bAlive = true;
			}
		}
#if true//2018.07.11
		private void DlgProgress_Shown(object sender, EventArgs e)
		{
			//コントロールボックスが非表示だと
			//DPI変化によるダイアログボックスのリサイズ
			//が正しく行われないことが判明
			//コード上で非表示にすることとする
			this.ControlBox = false;
		}
#endif
	}
}
