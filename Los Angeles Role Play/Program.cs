using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Los_Angeles_Role_Play
{
    static class Program
    {
        public static bool TestMode = false;
        public static string InfowebURL = "http://la-rp.co.kr";
        public static string LauncherURL = "http://la-rp.seongbum.com";
        public static string LauncherFileName = "larp.exe";
        public static string UpdaterFileName = "larpupdater.exe";
        public static string Path_LARP = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LARP");
        public static string Path_ChatLog = Path.Combine(Path_LARP, "챗로그");
        public static string Path_UAF = Path.Combine(Path_LARP, "비인가 파일");
        public static string Path_Setup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LARP");

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(args));
        }
    }
}
