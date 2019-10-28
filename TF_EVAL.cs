using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Linq;
//---
using System.IO;


namespace vSCOPE
{
    class TF_EVAL
    {
	//textbox2:
		public const string MODEL_FILE = "retrained_graph.pb";
	//textbox3:retrained_labels.txt
		static  List<double> score = new List<double>();

		static public bool load_txt(string path, out List<string> buf)
		{
			bool ret = false;
			buf = new List<string>();
			try {
				var st = new StreamReader(path, Encoding.Default);
				string tmp;
				while ((tmp = st.ReadLine()) != null) {
					buf.Add(tmp.Trim());
				}
				st.Close();
				ret = true;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK);
			}
			return(ret);
		}
		static public void load_score(string path, out List<double> score)
		{
			List<string> buf;
			score = new List<double>();
			int mscnt = 0;

			load_txt(path, out buf);
			for (int i = 0; i < buf.Count; i++) {
				string tmp;
				int p = buf[i].IndexOf(':');
				if (p < 0) {
					p = 0;
				}
				else {
					p++;
				}
				double f;
				if (buf[i].IndexOf("ms") > 0) {
					tmp = buf[i].Substring(0, buf[i].Length-2);
					mscnt++;
				}
				else {
					tmp = buf[i].Substring(p);
				}
				score.Add(double.Parse(tmp));
			}
			if (mscnt > 1) {
				int cnt = score.Count / 2;
				for (int i = 0; i < cnt; i++) {
					score.RemoveAt(0);
				}
			}
		}
		static public void eval_score(string path, string scor, out List<double> score)
		{
			List<string> buf;
			score = new List<double>();
			string dcur = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			try {
				if (true) {
					//Processオブジェクトを作成
					System.Diagnostics.Process p = new System.Diagnostics.Process();
					p.StartInfo.FileName = System.IO.Path.Combine(dcur, "ConsoleApplication1.exe");
					//出力を読み取れるようにする
					p.StartInfo.UseShellExecute = false;
					p.StartInfo.RedirectStandardOutput = false;
					p.StartInfo.RedirectStandardInput = false;
					//ウィンドウを表示しないようにする
					p.StartInfo.CreateNoWindow = true;
					//コマンドラインを指定（"/c"は実行後閉じるために必要）
					p.StartInfo.Arguments = dcur + "\\" + MODEL_FILE + " " + path + " " + scor;
					//起動
					p.Start();

					//出力を読み取る
					//string results = p.StandardOutput.ReadToEnd();

					//プロセス終了まで待機する
					//WaitForExitはReadToEndの後である必要がある
					//(親プロセス、子プロセスでブロック防止のため)
					p.WaitForExit();
					p.Close();
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK);
			}
			load_score(scor, out score);
		}
	
		static public string get_score_file(string path)
		{
			string name = System.IO.Path.GetFileNameWithoutExtension(path);
			string dirn = System.IO.Path.GetDirectoryName(path);
			string scor = dirn + "\\score\\" + name + ".txt";
			return(scor);
		}
		static private Image load_image(string path)
		{
			try {
				var fs = new FileStream (path, FileMode.Open, FileAccess.Read);
				Image img = Image.FromStream(fs);
				fs.Close();
				string scor = get_score_file(path);
				if (System.IO.File.Exists(scor)) {
					load_score(scor, out TF_EVAL.score);
				}
				else {
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(scor));
					eval_score(path, scor, out TF_EVAL.score);
				}
				return(img);
			}
			catch (Exception ex) {
			}
			return(null);
		}
	}
}
