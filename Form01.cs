﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

namespace vSCOPE
{
    public partial class Form01 : Form
    {
        #region Win32 API
        [FlagsAttribute]
        public enum ExecutionState : uint
        {
            // 関数が失敗した時の戻り値
            Null = 0,
            // スタンバイを抑止(Vista以降は効かない？)
            SystemRequired = 1,
            // 画面OFFを抑止
            DisplayRequired = 2,
            // 効果を永続させる。ほかオプションと併用する。
            Continuous = 0x80000000,
        }

        [DllImport("user32.dll")]
        extern static uint SendInput(
            uint nInputs,   // INPUT 構造体の数(イベント数)
            INPUT[] pInputs,   // INPUT 構造体
            int cbSize     // INPUT 構造体のサイズ
            );

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct INPUT
        {
            public int type;  // 0 = INPUT_MOUSE(デフォルト), 1 = INPUT_KEYBOARD
            public MOUSEINPUT mi;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;  // amount of wheel movement
            public int dwFlags;
            public int time;  // time stamp for the event
            public IntPtr dwExtraInfo;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        // dwFlags
        const int MOUSEEVENTF_MOVED = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;  // 右ボタン Down
        const int MOUSEEVENTF_RIGHTUP = 0x0010;  // 右ボタン Up
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;  // 中ボタン Up
        const int MOUSEEVENTF_WHEEL = 0x0080;
        const int MOUSEEVENTF_XDOWN = 0x0100;
        const int MOUSEEVENTF_XUP = 0x0200;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        const int screen_length = 0x10000;  // for MOUSEEVENTF_ABSOLUTE
        [DllImport("kernel32.dll")]
        static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
        #endregion

        public Form01()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
#if true//2019.05.12(縦型対応)
			string[] args = Environment.GetCommandLineArgs();
			foreach (string tmp in args) {
				if (tmp.ToUpper().Contains("-TATE")) {
					G.bTATE_MODE = true;
					this.Text = "縦:" + this.Text;
				}
			}
#endif
            this.timer1.Enabled = true;
			//C:\Users\araya320\AppData\Roaming\KOP\vSCOPE (<-セットアップにてコピーされる)
			//から
			//C:\Users\araya320\Documents\KOP\vSCOPE
			//へコピーし、元ファイルを削除する
			G.COPY_SETTINGS("settings.vSCOPE.xml");
 			//---
			G.AS.load(ref G.AS);
			G.SS.load(ref G.SS);
			//---
			G.SS.AUT_BEF_PATH = G.AS.AUT_BEF_PATH;
			G.SS.BEFORE_PATH  = G.AS.BEFORE_PATH;
			G.SS.PLM_AUT_FOLD = G.AS.PLM_AUT_FOLD;
			G.SS.MOZ_CND_FOLD = G.AS.MOZ_CND_FOLD;
			//---

			if (G.SS.ETC_SPE_CD01 == 0) {
				G.SS.ETC_SPE_CD01 = 1;
				G.SS.PLM_POSWT[0] = G.SS.PLM_POSFT[0] = G.SS.PLM_POSZT[0] = "メモ１";
				G.SS.PLM_POSWT[1] = G.SS.PLM_POSFT[1] = G.SS.PLM_POSZT[1] = "メモ２";
				G.SS.PLM_POSWT[2] = G.SS.PLM_POSFT[2] = G.SS.PLM_POSZT[2] = "メモ３";
			}
			if (G.SS.ETC_SPE_CD02 == 0) {
				G.SS.ETC_SPE_CD02 = 1;
				switch (/*位置検出*/G.SS.MOZ_CND_PDFL) {
				case  0:/*透過*/ G.SS.MOZ_CND_PDFL = 0; break;//カラー
				case  1:/*反射*/ G.SS.MOZ_CND_PDFL = 0; break;//カラー
				default:/*赤外*/ G.SS.MOZ_CND_PDFL = 1; break;//赤外
				}
			}
			//G.SS.ETC_BAK_COLOR = Color.FromArgb(198, 3, 85);
			//---
			this.Left = G.AS.APP_F01_LFT;
			this.Top = G.AS.APP_F01_TOP;
			this.BackColor = G.SS.ETC_BAK_COLOR;
			G.FORM01 = this;
			//---
			G.FORM10 = new Form10();
			G.FORM10.TopLevel = false;
			G.FORM11 = new Form11();
			G.FORM11.TopLevel = false;
			G.FORM12 = new Form12();
			G.FORM12.TopLevel = false;
			G.FORM13 = new Form13();
			G.FORM13.TopLevel = false;
			//---
			G.FORM10.BackColor = G.SS.ETC_BAK_COLOR;
			G.FORM11.BackColor = G.SS.ETC_BAK_COLOR;
			G.FORM12.BackColor = G.SS.ETC_BAK_COLOR;
			G.FORM13.BackColor = G.SS.ETC_BAK_COLOR;
			//---
			if (true) {
				//プログラム起動時のUIFレベルとして記憶
				G.UIF_LEVL = G.SS.ETC_UIF_LEVL;
			}
			/*
				0:ユーザ用(暫定版)
				1:ユーザ用
				2:開発者用(一度)
				3:開発者用(常に)
			 */
			if (G.UIF_LEVL == 0 || G.UIF_LEVL == 1) {
				this.button3.Visible = false;//設定
			}
			if (G.UIF_LEVL == 0) {
				G.FORM11.SET_UIF_USER();
				G.FORM12.SET_UIF_USER();
			}
			//---
			this.groupBox1.Controls.Add(G.FORM10);
			G.FORM10.Location = new Point(3, 12);
			G.FORM10.Show();
			G.FORM10.BringToFront();
			//---
			this.groupBox2.Controls.Add(G.FORM11);
			G.FORM11.Location = new Point(3, 12);
			G.FORM11.Show();
			G.FORM11.BringToFront();
			//---
			this.groupBox3.Controls.Add(G.FORM12);
			G.FORM12.Location = new Point(3, 12);
			G.FORM12.Show();
			G.FORM12.BringToFront();
			//---
			G.FORM10.UPDSTS();
			G.FORM11.UPDSTS();
			G.FORM12.UPDSTS();
			//---
			UPDSTS();
			//---
			if (G.UIF_LEVL == 1/*ユーザ用*/) {
				this.Text = "vSCOPE Application";
				BeginInvoke(new G.DLG_VOID_VOID(this.UIF_LEVL1_INIT));
			}
			else {
				this.Text = this.Text + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			if (G.UIF_LEVL == 2/*開発者用(一度)*/) {
				G.SS.ETC_UIF_LEVL = G.SS.ETC_UIF_BACK;
			}
#if true//2019.01.15(パスワード画面)
			if (G.SS.ETC_CPH_CHK1) {
				BeginInvoke(new G.DLG_VOID_VOID(this.PASSWORD_INPUT));
			}
#endif
#if true//2019.06.03(バンドパス・コントラスト値対応)
			DIGITI.calc_filter_coeff();
#endif
		}
#if true//2019.01.15(パスワード画面)
		private void PASSWORD_INPUT()
		{
			frmPassword frm = new frmPassword();
			if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
				this.Close();
				return;
			}
		}
#endif
		private void UIF_LEVL1_INIT()
		{
			//---
			this.button1.Visible = false;
			this.button2.Visible = false;
			this.button3.Visible = false;
			this.groupBox1.Visible = false;
			this.groupBox2.Visible = false;
			this.groupBox3.Visible = false;
			this.Controls.Add(G.FORM13);
			int w0 = this.Width;
			int w1 = G.FORM13.Width;
			this.Height = G.FORM13.Height+(w0-w1)+12;
			G.FORM13.Location = new Point(3, 3);
			G.FORM13.Show();
			G.FORM13.BringToFront();
			G.FORM13.Dock = DockStyle.Fill;
			//CONNECT
			OnClicks(this.button1, null);
			if (!D.isCONNECTED()) {
				BeginInvoke(new G.DLG_VOID_VOID(this.Close));
				return;
			}
			//OPEN(CAMERA)
			G.FORM02 = new Form02();
			G.FORM02.Show();
			if (!G.FORM02.isCONNECTED()) {
				BeginInvoke(new G.DLG_VOID_VOID(this.Close));
				return;
			}
			G.FORM13.START_TIMER();
		}
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (G.FORM02 != null) {
				G.FORM02.cam_close();
				//G.FORM02.m_bcontinuous = false;
				//G.FORM02.Stop();
				//G.FORM02.DestroyCamera();
				G.FORM02.Close();
				//G.FORM02.Dispose();
				G.FORM02 = null;
			}
			if (true) {
				G.FORM10.LED_SET(0, false);
				G.FORM10.LED_SET(1, false);
				G.FORM10.LED_SET(2, false);
			}
			//---
			if (this.Left <= -32000 || this.Top <= -32000) {
				//最小化時は更新しない
			}
			else {
			G.AS.APP_F01_LFT = this.Left;
			G.AS.APP_F01_TOP = this.Top; 
			}
			//---
			G.AS.AUT_BEF_PATH = G.SS.AUT_BEF_PATH;
			G.AS.BEFORE_PATH  = G.SS.BEFORE_PATH ;
			G.AS.PLM_AUT_FOLD = G.SS.PLM_AUT_FOLD;
			G.AS.MOZ_CND_FOLD = G.SS.MOZ_CND_FOLD;
			//---
			if (G.SS.ETC_UIF_LEVL == 0 || G.SS.ETC_UIF_LEVL == 1) {
				G.SS.ETC_UIF_BACK = G.SS.ETC_UIF_LEVL;
			}
			//---
			G.AS.save(G.AS);
			G.SS.save(G.SS);
			//---
			if (G.AS.DEBUG_MODE != 0) {
				DBGMODE.TERM();
			}
		}
		private void UPDSTS()
		{
			if (!D.isCONNECTED())
			{
				this.button1.Enabled = true;
				this.button2.Enabled = false;
				//this.groupBox1.Enabled = false;
				//this.groupBox2.Enabled = false;
				//this.tabControl1.Enabled = false;
				//this.groupBox6.Enabled = false;
				return;
			}
			this.button1.Enabled = false;
			this.button2.Enabled = true;
//			this.groupBox1.Enabled = true;
//			this.groupBox2.Enabled = true;
//			this.groupBox3.Enabled = true;
			//---
			//---
		}
        private void OnClicks(object sender, EventArgs e)
        {
			if (false) {
			}
			else if (sender == this.button1) {
				if (D.INIT()) {
					G.FORM11.INIT();
					G.FORM10.UPDSTS();
					G.FORM11.UPDSTS();
					G.FORM12.UPDSTS();
				}
			}
			else if (sender == this.button2) {
				G.FORM10.LED_SET(0, false);
				G.FORM10.LED_SET(1, false);
				G.FORM10.LED_SET(2, false);
				D.TERM();
				G.FORM10.UPDSTS();
				G.FORM11.UPDSTS();
				G.FORM12.UPDSTS();
			}
			else if (sender == this.button3) {
				frmSettings frm = new frmSettings();
				frm.m_ss = (G.SYSSET)G.SS.Clone();
				if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) {
					G.SS = (G.SYSSET)frm.m_ss.Clone();
					G.SS.save(G.SS);
				}

				//G.LOAD();
				//G.mlog("#iINIファイルを再読み込みしました.");
			}
			//---------------------------
			UPDSTS();
		}

		private void Form01_Resize(object sender, EventArgs e)
		{
			if (G.FORM02 != null) {
				if (this.WindowState == FormWindowState.Minimized) {
					G.FORM02.WindowState = FormWindowState.Minimized;
				}
				else if (this.WindowState == FormWindowState.Normal) {
					G.FORM02.WindowState = FormWindowState.Normal;
				}
			}
		}

        static bool ctrlKeyFlg = false;
        static bool AKeyFlg = false;
        static bool BKeyFlg = false;
        static bool CKeyFlg = false;
        bool flag = false;

		private void Form01_KeyDown(object sender, KeyEventArgs e)
		{
#if false
			bool flag = true;
			if (false) {
			}
			else if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.LeftCtrl) & System.Windows.Input.KeyStates.Down)== 0) {
				flag = false;
			}
			else if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.A) & System.Windows.Input.KeyStates.Down)== 0) {
				flag = false;
			}
			else if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.B) & System.Windows.Input.KeyStates.Down)== 0) {
				flag = false;
			}
			else if ((System.Windows.Input.Keyboard.GetKeyStates(System.Windows.Input.Key.C) & System.Windows.Input.KeyStates.Down)== 0) {
				flag = false;
			}
			if (flag) {
			//this.KeyPreview = false;
			//G.mlog("#iソフトウェアは次回起動時にユーザモードで起動します。");
			var frm = new frmMessage();
			frm.ShowDialog(this);
            }
#else
			if(e.KeyCode == Keys.ControlKey)
			{
				ctrlKeyFlg = true;
			}
#if true//2018.08.13(CTRL+ABCからCTRL+IOPに変更)
			if(e.KeyCode == Keys.I) {
				AKeyFlg = true;
			}
			if(e.KeyCode == Keys.O) {
				BKeyFlg = true;
			}
			if(e.KeyCode == Keys.P) {
				CKeyFlg = true;
			}
#else
			if(e.KeyCode == Keys.A)
			{
				AKeyFlg = true;
			}
			if(e.KeyCode == Keys.B)
			{
				BKeyFlg = true;
			}
			if(e.KeyCode == Keys.C)
			{
				CKeyFlg = true;
			}
#endif

            if(ctrlKeyFlg && AKeyFlg && BKeyFlg && CKeyFlg)
            {
                flag = true;
                ctrlKeyFlg = false;
                AKeyFlg = false;
                BKeyFlg = false;
                CKeyFlg = false;
            }
            if (flag) {
			//this.KeyPreview = false;
			//G.mlog("#iソフトウェアは次回起動時にユーザモードで起動します。");
			var frm = new frmMessage();
			frm.ShowDialog(this);
            flag = false;
            }
#endif
        }

        private void Form01_KeyUp(object sender, KeyEventArgs e)
        {
			if(e.KeyCode == Keys.ControlKey)
			{
				ctrlKeyFlg = false;
                flag = false;
			}
#if true//2018.08.13(CTRL+ABCからCTRL+IOPに変更)
			if(e.KeyCode == Keys.I) {
				AKeyFlg = false;
                flag = false;
			}
			if(e.KeyCode == Keys.O) {
				BKeyFlg = false;
                flag = false;
			}
			if(e.KeyCode == Keys.P) {
				CKeyFlg = false;
                flag = false;
			}
#else
			if(e.KeyCode == Keys.A)
			{
				AKeyFlg = false;
                flag = false;
			}
			if(e.KeyCode == Keys.B)
			{
				BKeyFlg = false;
                flag = false;
			}
			if(e.KeyCode == Keys.C)
			{
				CKeyFlg = false;
                flag = false;
			}
#endif
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
 //画面暗転阻止
            SetThreadExecutionState(ExecutionState.DisplayRequired);

            // ドラッグ操作の準備 (struct 配列の宣言)
            INPUT[] input = new INPUT[1];  // イベントを格納

            // ドラッグ操作の準備 (イベントの定義 = 相対座標へ移動)
            input[0].mi.dx = 0;  // 相対座標で0　つまり動かさない
            input[0].mi.dy = 0;  // 相対座標で0 つまり動かさない
            input[0].mi.dwFlags = MOUSEEVENTF_MOVED;

            // ドラッグ操作の実行 (イベントの生成)
            SendInput(1, input, Marshal.SizeOf(input[0]));
        }
    }
}
