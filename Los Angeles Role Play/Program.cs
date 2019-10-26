using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Los_Angeles_Role_Play
{
    static class Program
    {
        public static bool TestMode = false;
        public static string InfowebURL = "http://status.la-rp.co.kr";
        public static string ForumURL = "http://la-rp.co.kr";
        public static string LauncherURL = "http://launcher.la-rp.co.kr";
        public static string LauncherFileName = "LARPLauncher.exe";
        public static string UpdaterFileName = "LARPUpdater.exe";
        public static string Path_LARP = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LARP");
        public static string Path_ChatLog = Path.Combine(Path_LARP, "챗로그");
        public static string Path_UAF = Path.Combine(Path_LARP, "비인가 파일");
        public static string Path_Setup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Los Angeles Role Play");

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            // 중복 실행 방지
            bool bnew;
            string mutexname = (IsExecutedByLauncher()) ? "LARPLauncher" : "LARPUpdater";
            Mutex mutex = new Mutex(true, mutexname, out bnew);
            if (bnew) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain(args));
                mutex.ReleaseMutex();
            } else {
                MessageBox.Show("LARP 런처가 이미 실행중입니다.");
                Application.Exit();
            }
        }

        static public bool IsExecutedByLauncher() {
            // 런처로 실행되었는지 여부
            return (string.Compare(Application.ExecutablePath, Path.Combine(Program.Path_Setup, Program.LauncherFileName), true) == 0);
        }
    }
}
