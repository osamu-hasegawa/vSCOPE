using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vSCOPE
{
	public partial class Form28 : Form
	{
		public G.SYSSET m_ss;

		public Form28()
		{
			InitializeComponent();
		}

		private void Form28_Load(object sender, EventArgs e)
		{
			m_ss = G.SS;
			if (string.IsNullOrEmpty(m_ss.ZMS_AUT_FOLD)) {
				string path;
				path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				path += @"\KOP";
				path += @"\" + Application.ProductName;
				m_ss.ZMS_AUT_FOLD = path;
			}
			DDX(true);
			radioButton1_Click(null, null);
			checkBox2_Click(null, null);
			comboBox3_SelectedIndexChanged(null, null);
			//---
			numericUpDown17_ValueChanged(null, null);
			checkBox4_Click(null, null);
			//---
		}

		private void Form28_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult != DialogResult.OK) {
				return;
			}
			if (DDX(false) == false) {
				e.Cancel = true;
			}
			else if (!System.IO.Directory.Exists(m_ss.ZMS_AUT_FOLD)) {
				G.mlog("指定されたフォルダは存在しません.\r\r" + m_ss.ZMS_AUT_FOLD);
				e.Cancel = true;
			}
			else {
				if (this.comboBox3.Text == "-") {
					G.mlog("有効な測定モードを選択してください.");
					this.comboBox3.Focus();
					e.Cancel = true;
					return;
				}
				G.SS = (G.SYSSET)m_ss.Clone();
			}
		}
		private bool DDX(bool bUpdate)
        {
            bool rc;
			try {
				DDV.DDX(bUpdate, this.textBox1, ref m_ss.ZMS_AUT_TITL);
				DDV.DDX(bUpdate, this.comboBox3, ref m_ss.ZMS_AUT_MODE);
				DDV.DDX(bUpdate, this.comboBox1, ref m_ss.ZMS_AUT_AFMD);
				DDV.DDX(bUpdate, this.numericUpDown1, ref m_ss.ZMS_AUT_SKIP);
				//---
				DDV.DDX(bUpdate, this.checkBox5      , ref m_ss.ZMS_AUT_CNST);
				//---
				DDV.DDX(bUpdate, this.numericUpDown4, ref m_ss.ZMS_AUT_HANI);
				DDV.DDX(bUpdate, this.numericUpDown5, ref m_ss.ZMS_AUT_DISL);
				DDV.DDX(bUpdate, this.numericUpDown6, ref m_ss.ZMS_AUT_DISS);
#if true//2019.03.02(直線近似)
				DDV.DDX(bUpdate, this.numericUpDown20,ref m_ss.ZMS_AUT_DISM);
#endif
				//---
				//---
				DDV.DDX(bUpdate, this.comboBox2      , ref m_ss.ZMS_AUT_FLTP);
				DDV.DDX(bUpdate, this.textBox2       , ref m_ss.ZMS_AUT_FOLD);
				//---
#if true//2019.01.23(GAIN調整&自動測定)
				DDV.DDX(bUpdate, this.checkBox11     , ref m_ss.ZMS_AUT_V_PK);
#endif
				//---
				//---
#if true//2018.07.02
				DDV.DDX(bUpdate, this.textBox3       , ref m_ss.ZMS_AUT_ZPOS, 50, -99, +99);
#endif
				//---
				DDV.DDX(bUpdate, this.checkBox4      , ref m_ss.ZMS_AUT_IRCK);//カラーと同時に赤外測定
#if true//2019.03.02(直線近似)
				DDV.DDX(bUpdate, this.checkBox13,ref m_ss.ZMS_AUT_AF_2);//AF2使用
#endif
				if (bUpdate == false) {
					if (this.textBox2.Text == "") {
						G.mlog("フォルダを指定してください.");
						this.textBox2.Focus();
						return(false);
					}
					char[] fc = {
						'\\', '/', ':', '*', '?', '\"', '<', '>', '|'
					};
					foreach (char c in fc) {
						if (this.textBox1.Text.IndexOf(c) >= 0) {
							this.textBox1.Focus();
							G.mlog("次の文字は使えません.\r\\ / : * ? \" < > |");
							return (false);
						}
					}
#if true//2019.08.08(保存内容変更)
					if (string.IsNullOrEmpty(m_ss.ZMS_AUT_TITL)) {
						G.mlog("タイトルを入力してください.");
						this.textBox1.Focus();
						return(false);
					}
#endif
					//---
#if true//2019.07.27(保存形式変更)
					if (!G.check_zpos(m_ss.ZMS_AUT_ZPOS, true)) {
						this.textBox3.Focus();
						return(false);
					}
#endif
				}
                rc = true;
            }
            catch (Exception e)
            {
                G.mlog(e.Message);
                rc = false;
            }
            return (rc);
		}

		private void OnClicks(object sender, EventArgs e)
		{
			if (sender == this.button3) {
				//FolderBrowserDialogクラスのインスタンスを作成
				FolderBrowserDialog fbd = new FolderBrowserDialog();

				//上部に表示する説明テキストを指定する
				fbd.Description = "フォルダを指定してください。";
				//ルートフォルダを指定する
				//デフォルトでDesktop
				fbd.RootFolder = Environment.SpecialFolder.Desktop;
				//最初に選択するフォルダを指定する
				//RootFolder以下にあるフォルダである必要がある
				fbd.SelectedPath = this.textBox2.Text;
				//ユーザーが新しいフォルダを作成できるようにする
				//デフォルトでTrue
				fbd.ShowNewFolderButton = true;

				//ダイアログを表示する
				if (fbd.ShowDialog(this) == DialogResult.OK)
				{
					this.textBox2.Text = fbd.SelectedPath;
				}
			}
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
		}

		private void radioButton1_Click(object sender, EventArgs e)
		{
#if true//2019.03.02(直線近似)
			if (!this.checkBox13.Checked) {
				check_for_af2();
			}
#endif
#if true//2019.03.02(直線近似)
			if (this.checkBox13.Checked) {
				check_for_af2();
			}
#endif
		}

		private void checkBox2_Click(object sender, EventArgs e){}
		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)	{}

		private void numericUpDown17_ValueChanged(object sender, EventArgs e){}

		private void checkBox4_Click(object sender, EventArgs e)
		{
		}
#if true//2018.12.22(測定抜け対応)

		private void checkBox10_Click(object sender, EventArgs e)
		{
			checkBox2_Click(null, null);
		}
#endif
#if true//2019.03.02(直線近似)
		private void check_for_af2()
		{
			this.numericUpDown5.Enabled = !this.checkBox13.Checked;
			this.numericUpDown6.Enabled = !this.checkBox13.Checked;
			this.numericUpDown20.Enabled = !this.checkBox13.Checked;
		}
#endif
	}
}
