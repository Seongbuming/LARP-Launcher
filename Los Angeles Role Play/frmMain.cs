/*
 * 
 *  Los Angeles Role Play Launcher
 *  Copyright (c)2015 서성범. All rights reserved.
 *  
 */

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net;
using Microsoft.Win32;
using System.Reflection;

namespace Los_Angeles_Role_Play
{
    public partial class frmMain : Form
    {
        #region < DllImport >
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        #endregion

        #region  < Initialize > 
        int InitLevel = 0;
        int ExitLevel = 0;
        string username;

        public frmMain(string[] args) {
            InitializeComponent();
            // Url Scheme 레지스트리 생성
            CreateUrlSchemeRegistry();
            // 사용자 닉네임 파라미터 전달
            if (args.Length >= 1)
                username = args[0].Replace("larp:", string.Empty).Replace("/", string.Empty);
            else if (Program.TestMode)
                username = "Larp_Tester";
            else
                username = string.Empty;
            // 환영 문구
            if (username.Length > 0)
                PercentageLabel.Text = username + "님, 안녕하세요!";
            // 게임 실행
            GameStart.Start();
        }

        public string GetUsername() {
            return username;
        }
        #endregion

        #region < 게임 실행 >
        private void GameStart_Tick(object sender, EventArgs e) {
            GameStart.Stop();
            KillGameProcess();
            Debug.Print("InitLevel - " + InitLevel + " - ");
            switch (InitLevel) {
                case 0: // 서버 연결 확인
                    HttpWebRequest request;
                    HttpWebResponse response;
                    HttpStatusCode[] statuscode = new HttpStatusCode[2];
                    try {
                        request = (HttpWebRequest)WebRequest.Create(new Uri(Program.InfowebURL));
                        response = (HttpWebResponse)request.GetResponse();
                        statuscode[0] = response.StatusCode;
                        response.Close();

                        request = (HttpWebRequest)WebRequest.Create(new Uri(Program.LauncherURL));
                        response = (HttpWebResponse)request.GetResponse();
                        statuscode[1] = response.StatusCode;
                        response.Close();
                    } catch {
                        statuscode[0] = statuscode[1] = HttpStatusCode.NotFound;
                    }

                    if (statuscode[0] != HttpStatusCode.OK && statuscode[1] != HttpStatusCode.OK) {
                        PercentageLabel.Text = "서버에 연결할 수 없습니다.";
                        Button_2_1.Text = "인포웹";
                        Button_2_2.Text = "종료";
                        SetButtonEvent(Button_2_1, ButtonEvent_OpenInfoweb);
                        SetButtonEvent(Button_2_2, Application.Exit);
                        ShowButtons(2);
                        this.TopMost = true;
                        this.TopMost = false;
                    } else {
                        GameStart.Start();
                    }

                    break;
                case 1: // 런처 업데이트
                    StartUpdate();
                    break;
                case 2: // 게임 구성 파일 변조 검사 및 패치
                    // 비인가 프로그램 차단 (파일 변조 검사 제외)
                    GetAuthorizedFilesFromServer(Program.LauncherURL + "/getfilelist.php?type=AllowedFiles");
                    if (BlockUnauthorizedPrograms())
                        return;
                    // 검사 및 패치
                    if (!CompareMD5OfGameFile(Program.LauncherURL + "/getfilelist.php?type=Patch"))
                        StartPatch(Program.LauncherURL + "/Patch", GetDissimilarFiles(), GetNumberOfDissimilarFiles() - 1);
                    else
                        // 게임 실행 단계 진행
                        GameStart.Start();
                    break;
                case 3: // 게임 실행
                    // 비인가 프로그램 차단 (파일 변조 검사 포함)
                    if (BlockUnauthorizedPrograms())
                        return;
                    // 닉네임 레지스트리 동기화
                    RegistryKey reg = Registry.CurrentUser.CreateSubKey(@"Software\SAMP", RegistryKeyPermissionCheck.ReadWriteSubTree);
                    reg.SetValue("PlayerName", GetUsername());

                    if (GetNewLauncherPath() != String.Empty) {
                        // URL Scheme
                        CreateUrlSchemeRegistry();
                        // 최신 런처 실행
                        Process.Start(GetNewLauncherPath(), GetUsername());
                        Application.Exit();
                    }
                    else if (string.Compare(GetUsername(), "NULL") == 0) {
                        PercentageLabel.Text = "설치 완료";
                        Button_2_1.Text = "인포웹";
                        Button_2_2.Text = "종료";
                        SetButtonEvent(Button_2_1, ButtonEvent_OpenInfoweb);
                        SetButtonEvent(Button_2_2, Application.Exit);
                        ShowButtons(2);
                        this.TopMost = true;
                        this.TopMost = false;
                    }
                    else {
                        PercentageLabel.Text = "게임에 접속중입니다.";
                        // SAMP 실행
                        Process.Start(Path.Combine(GetGamePath(), "samp.exe"), "server.la-rp.co.kr");
                        GameExit.Start();
                    }
                    break;
            }
            InitLevel++;
        }

        private void GameExit_Tick(object sender, EventArgs e) {
            if (IsProcessRunning("gta_sa")) {
                if (ExitLevel == 0)
                    this.Hide();
                BlockUnauthorizedPrograms(true);
                ExitLevel = 1;
            }
            else if (ExitLevel > 0) {
                ExitLevel = 0;
                try {
                    string sampchatlogpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA San Andreas User Files", "SAMP");
                    string larpchatlogfile = Path.Combine(Program.Path_ChatLog, "ChatLog" + DateTime.Now.ToString("-yyMMdd-HHmmss") + ".txt");
                    if (!Directory.Exists(Program.Path_ChatLog))
                        Directory.CreateDirectory(Program.Path_ChatLog);
                    File.Copy(Path.Combine(sampchatlogpath, "chatlog.txt"), larpchatlogfile, true);
                    PercentageLabel.Text = "챗로그가 자동 백업되었습니다.";
                } catch {
                    PercentageLabel.Text = "챗로그 백업에 실패했습니다!";
                }
                // 하단 버튼 설정
                Button_2_1.Text = "열기";
                Button_2_2.Text = "종료";
                SetButtonEvent(Button_2_1, ButtonEvent_OpenChatLog);
                SetButtonEvent(Button_2_2, Application.Exit);
                ShowButtons(2);
                this.Show();
                this.TopMost = true;
                this.TopMost = false;
            }
        }
        #endregion

        #region  < 폼 이동 > 
        Point MouseLocation;

        private void HeadCaption_MouseDown(object sender, MouseEventArgs e) {
            // HeadCaption 클릭 시 마우스 위치 기억
            MouseLocation = e.Location;
        }

        private void HeadCaption_MouseMove(object sender, MouseEventArgs e) {
            // 마우스 왼쪽 버튼이 눌려 있다면
            if (e.Button == MouseButtons.Left) {
                // 폼 위치 동기화
                int x = this.Location.X + e.Location.X - MouseLocation.X;
                int y = this.Location.Y + e.Location.Y - MouseLocation.Y;
                this.Location = new Point(x, y);
            }
        }
        #endregion

        #region < 하단 버튼 >
        object focusedButton = null;
        private delegate void ButtonEvent();

        private void ShowButtons(int count) {
            switch (count) {
                case 0:
                    ButtonContainer1.Visible = false;
                    ButtonContainer2.Visible = false;
                    ButtonContainer3.Visible = false;
                    break;
                case 1:
                    ButtonContainer1.Visible = true;
                    ButtonContainer2.Visible = false;
                    ButtonContainer3.Visible = false;
                    break;
                case 2:
                    ButtonContainer1.Visible = false;
                    ButtonContainer2.Visible = true;
                    ButtonContainer3.Visible = false;
                    break;
                case 3:
                    ButtonContainer1.Visible = false;
                    ButtonContainer2.Visible = false;
                    ButtonContainer3.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void SetButtonEvent(Label button, ButtonEvent func) {
            // 이벤트 해제
            FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(button);
            PropertyInfo pi = button.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(button, null);
            list.RemoveHandler(obj, list[obj]);

            // 이벤트 설정
            button.Click += (object sender, EventArgs e) => {
                func();
            };
        }

        private void ButtonEvent_OpenInfoweb() {
            Process.Start(Program.InfowebURL);
        }

        private void ButtonEvent_OpenForum() {
            Process.Start(Program.ForumURL);
        }

        private void ButtonEvent_OpenChatLog() {
            Process.Start(Program.Path_ChatLog);
        }

        private void ButtonEvent_OpenPath(string path) {
            Process.Start(path);
        }

        private void Button_MouseEnter(object sender, EventArgs e) {
            focusedButton = sender;
            ((Label)sender).BackColor = Color.SkyBlue;
            ((Label)sender).ForeColor = Color.White;
        }

        private void Button_MouseLeave(object sender, EventArgs e) {
            focusedButton = null;
            ((Label)sender).BackColor = Color.AliceBlue;
            ((Label)sender).ForeColor = Color.SteelBlue;
        }

        private void Button_MouseDown(object sender, MouseEventArgs e) {
            ((Label)sender).BackColor = Color.LightBlue;
            ((Label)sender).ForeColor = Color.White;
        }

        private void Button_MouseUp(object sender, MouseEventArgs e) {
            if (focusedButton == sender) {
                ((Label)sender).BackColor = Color.SkyBlue;
                ((Label)sender).ForeColor = Color.White;
            }
        }
        #endregion

        #region  < 패치 및 업데이트 > 
        string sMode;
        string[] sFileList;
        string sHost, sUrlToReadFileFrom, sFilePathToWriteFileTo;
        int sCurrentFileIndex, sFinalIndex;
        string NewLauncherPath = String.Empty;

        private void StartPatch(string host, string[] filelist, int finalindex = -1) {
            // 비인가 프로그램 차단
            if (BlockUnauthorizedPrograms())
                return;

            // Progress Bar 초기화
            SetProgressBar(0, 0);
            SetProgressBar(1, 0);

            // 파일을 다운로드할 서버 URL
            sHost = host;
            // 다운로드할 모든 파일의 URL
            sFileList = filelist;
            // 인덱스 초기화
            sCurrentFileIndex = 0;
            // 마지막 파일의 인덱스
            if (finalindex == -1)
                for (int i = 0; i < filelist.Length; i++)
                    if (filelist[i].Length > 0)
                        finalindex++;
            sFinalIndex = finalindex;
            Debug.Print("finalindex = " + finalindex.ToString());
            // 다운로드할 파일의 URL
            sUrlToReadFileFrom = sHost + "/" + sFileList[sCurrentFileIndex].Replace('\\', '/');
            // 파일을 저장할 경로 + 이름
            sFilePathToWriteFileTo = Path.Combine(GetGamePath(), sFileList[sCurrentFileIndex]);

            // PercentageLabel 동기화
            PercentageLabel.Text = "패치중: " + sFileList[sCurrentFileIndex];
            // DownloadWorker 실행
            sMode = "Patch";
            DownloadWorker.RunWorkerAsync();
        }

        private void StartUpdate() {
            Uri url;
            HttpWebRequest request;
            HttpWebResponse response;
            HttpStatusCode statuscode;
            string newhash;

            try {
                // 최신 런처의 Hash를 가져옴
                url = new Uri(Program.LauncherURL + "/getfilehash.php?name=" + Program.LauncherFileName);
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                statuscode = response.StatusCode;
                newhash = (new StreamReader(response.GetResponseStream(), Encoding.UTF8)).ReadToEnd().ToUpper();
                response.Close();
            } catch {
                alert("런처 서버에 연결할 수 없습니다.", false);
                return;
            }
            
            if (statuscode != HttpStatusCode.OK || newhash == string.Empty) { // 최신 런처를 확인할 수 없는 경우
                PercentageLabel.Text = "최신 버전의 런처를 내려받으세요.";
                Button_2_1.Text = "인포웹";
                Button_2_2.Text = "종료";
                SetButtonEvent(Button_2_1, ButtonEvent_OpenInfoweb);
                SetButtonEvent(Button_2_2, Application.Exit);
                ShowButtons(2);
                this.TopMost = true;
                this.TopMost = false;
                return;
            }
            
            // 현재 버전의 Hash를 가져옴
            string ehash = GetMD5OfFile(Application.ExecutablePath).ToUpper();
            
            // 런처로 실행되었는지 여부
            int ismm = string.Compare(Application.ExecutablePath, Path.Combine(Program.Path_Setup, Program.LauncherFileName), true);
            
            if (string.Compare(newhash, ehash, true) == 0 && ismm == 0) // 런처가 최신 버전인 경우
                GameStart.Start();
            else { // 런처 업데이트가 필요한 경우
                // Progress Bar 설정
                SetProgressBar(0, 0);
                SetProgressBar(1, 50);
                // 경로 생성
                try {
                    if (!Directory.Exists(Program.Path_Setup))
                        Directory.CreateDirectory(Program.Path_Setup);
                } catch { }
                // 대상 지정
                sUrlToReadFileFrom = Program.LauncherURL + "/" + Program.LauncherFileName;
                sFilePathToWriteFileTo = Path.Combine(Program.Path_Setup,
                    (ismm == 0)? Program.UpdaterFileName : Program.LauncherFileName);
                NewLauncherPath = sFilePathToWriteFileTo;
                // 파일이 이미 존재할 시 삭제
                try {
                    if (File.Exists(sFilePathToWriteFileTo))
                        File.Delete(sFilePathToWriteFileTo);
                } catch { }
                // PercentageLabel 동기화
                PercentageLabel.Text = "최신 런처 설치중";
                // DownloadWorker 실행
                sMode = "Update";
                DownloadWorker.RunWorkerAsync();
            }
        }

        private string GetNewLauncherPath() {
            return NewLauncherPath;
        }

        private bool SetProgressBar(int type, int percentage) {
            Panel bar, background;

            // 패널 오브젝트를 지정
            switch (type) {
                case 0: // Unit
                    bar = this.UnitProgressBar;
                    background = this.UnitProgressBar_Background;
                    break;
                case 1: // Total
                    bar = this.TotalProgressBar;
                    background = this.TotalProgressBar_Background;
                    break;
                default:
                    return false;
            }

            // Percentage에 맞게 Bar의 너비를 조정
            bar.Width = (background.Width - 2) / 100 * percentage;
            // Bar 가운데 정렬
            bar.Left = (background.Width / 2) - (bar.Width / 2);

            return true;
        }

        private void DownloadWorker_DoWork(object sender, DoWorkEventArgs e) {
            try {
                if(string.Compare(sMode, "Patch") == 0)
                    Debug.Print("패치[" + sCurrentFileIndex.ToString() + "] " + sUrlToReadFileFrom);

                // 다운로드할 파일의 정확한 크기(bytes)를 구함
                Uri url = new Uri(sUrlToReadFileFrom);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Int64 iSize = response.ContentLength;
                response.Close();

                // Progress Bar를 동기화하기 위해 이미 다운로드된 총 크기를 측정
                Int64 iRunningByteTotal = 0;

                // 파일을 다운로드하기 위한 webclient 오브젝트
                using (WebClient client = new WebClient()) {
                    // 원격 URL의 파일을 읽기 모드로 염
                    using (System.IO.Stream streamRemote = client.OpenRead(new Uri(sUrlToReadFileFrom))) {
                        // FileStream 오브젝트를 사용하여 다운로드된 데이터를 파일에 저장할 수 있음
                        using (Stream streamLocal = new FileStream(sFilePathToWriteFileTo, FileMode.Create, FileAccess.Write, FileShare.None)) {
                            // 스트림을 반복하며 버퍼에 파일을 받음
                            int iByteSize = 0;
                            byte[] byteBuffer = new byte[iSize];
                            while ((iByteSize = streamRemote.Read(byteBuffer, 0, byteBuffer.Length)) > 0) {
                                // 지정된 경로에 파일을 작성
                                streamLocal.Write(byteBuffer, 0, iByteSize);
                                iRunningByteTotal += iByteSize;

                                // Unit 진행도를 최대 100으로 계산
                                double dIndex = (double)(iRunningByteTotal);
                                double dTotal = (double)byteBuffer.Length;
                                double dProgressPercentage = (dIndex / dTotal);
                                int iProgressPercentage = (int)(dProgressPercentage * 100);

                                // Unit Progress Bar 동기화
                                DownloadWorker.ReportProgress(iProgressPercentage);
                            }
                            // 파일 스트림 종료
                            streamLocal.Close();
                        }
                        // 원격 서버와의 연결 종료
                        streamRemote.Close();
                    }
                }
            } catch {
                DownloadWorker.CancelAsync();
                if (string.Compare(sMode, "Patch") == 0)
                    MessageBox.Show(sFileList[sCurrentFileIndex] + " 파일을 다운로드할 수 없습니다.");
                else if (string.Compare(sMode, "Update") == 0)
                    MessageBox.Show("런처 업데이트에 실패했습니다.");
                Application.Exit();
            }
        }

        private void DownloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            // Unit Progress Bar 동기화
            SetProgressBar(0, e.ProgressPercentage);
        }

        private void DownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (string.Compare(sMode, "Patch") == 0) {
                // 작업이 취소되지 않았고, 다운로드할 파일이 남아 있으면
                if (!e.Cancelled && sCurrentFileIndex < sFinalIndex) {
                    // 인덱스 증가
                    sCurrentFileIndex++;
                    // 다운로드할 파일의 URL
                    sUrlToReadFileFrom = sHost + "/" + sFileList[sCurrentFileIndex].Replace('\\', '/');
                    // 파일을 저장할 경로 + 이름
                    sFilePathToWriteFileTo = Path.Combine(GetGamePath(), sFileList[sCurrentFileIndex]);

                    // Total Progress Bar 동기화
                    SetProgressBar(1, (int)((sCurrentFileIndex - 1) * 100 / sFinalIndex));
                    // PercentageLabel 동기화
                    PercentageLabel.Text = "패치중: " + sFileList[sCurrentFileIndex];

                    // DownloadWorker 실행
                    DownloadWorker.RunWorkerAsync();
                }
                else if (sCurrentFileIndex >= sFinalIndex)
                    GameStart.Start();
                else {
                    PercentageLabel.Text = "패치가 취소되었습니다.";
                    this.TopMost = true;
                    this.TopMost = false;
                }
            }
            else if (string.Compare(sMode, "Update") == 0) {
                if (e.Cancelled) {
                    PercentageLabel.Text = "업데이트가 취소되었습니다.";
                    this.TopMost = true;
                    this.TopMost = false;
                }
                else
                    GameStart.Start();
            }
        }
        #endregion

        #region  < 게임 구성 파일 변조 검사 > 
        private string[] DissimilarFiles = new string[30];

        private bool CompareMD5OfGameFile(string listurl) {
            Uri url;
            HttpWebRequest request;
            HttpWebResponse response;
            string[] fdata;

            try {
                // 서버에 저장된 게임 파일의 목록(Name,Hash)을 가져옴
                // ex) samp.dll,64add3449fa874e17071c5149892ce07|SAMP\custom.img,8fc7f2ec79402a952d5b896b710b3a41|...
                url = new Uri(listurl);
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                fdata = (new StreamReader(response.GetResponseStream(), Encoding.UTF8)).ReadToEnd().Split('|');
                response.Close();
            } catch {
                alert("런처 서버에 연결할 수 없습니다.", false);
                return false;
            }

            // DissimilarFiles 초기화
            for (int i = 0; i < DissimilarFiles.Length; i++)
                DissimilarFiles[i] = String.Empty;

            // 서버에 저장된 파일과 동일한 것인지 검사
            int dcount = 0;
            for (int i = 0; i < fdata.Length; i++) {
                string[] sdata = fdata[i].Split(',');
                if (GetMD5OfFile(Path.Combine(GetGamePath(), sdata[0])).ToUpper() != sdata[1].ToUpper()) {
                    Debug.Print("<DF> " + sdata[0] + ": " + GetMD5OfFile(Path.Combine(GetGamePath(), sdata[0])).ToUpper());
                    DissimilarFiles[dcount] = sdata[0];
                    dcount++;
                }
            }

            // 서버의 것과 동일하면 true
            if (dcount == 0)
                return true;
            return false;
        }

        private string[] GetDissimilarFiles() {
            return DissimilarFiles;
        }

        private int GetNumberOfDissimilarFiles() {
            int dcount = 0;
            for (int i = 0; i < DissimilarFiles.Length; i++)
                if (DissimilarFiles[i] != null && DissimilarFiles[i].Length > 0)
                    dcount++;
            return dcount;
        }
        #endregion

        #region  < 비인가 프로그램 차단 > 
        string[] AllowedExtension = new string[] { ".cleo", ".cs", ".asi", ".dll" };
        string AuthorizedFiles = String.Empty;

        private bool BlockUnauthorizedPrograms(bool blockdissimilarfiles = false) {
            bool blocked = false;
            if (AntiCheat() || AntiAUF())
                blocked = true;
            if (GetNumberOfDissimilarFiles() > 0 && blockdissimilarfiles) {
                blocked = true;
                // 오류 메시지 출력
                alert("게임 구성 파일이 변조되었습니다.", true);
                // 디버그
                for (int i = 0; i < GetNumberOfDissimilarFiles(); i++)
                    Debug.Print("DissimilarFiles[" + i + "]: " + GetDissimilarFiles()[i]);
            }
            if (blocked) {
                // 게임 강제종료
                KillGameProcess();
                // 하단 버튼 설정
                Button_1_1.Text = "종료";
                SetButtonEvent(Button_1_1, Application.Exit);
                ShowButtons(1);
                return true;
            }
            return false;
        }

        private bool AntiCheat() {
            FileInfo gtasa = new FileInfo(Path.Combine(GetGamePath(), "gta_sa.exe"));
            double filesize = Math.Round((double)(gtasa.Length / 1000000));
            if (filesize < 13 || filesize > 15) {
                alert("GTA:SA 실행 파일이 비정상적입니다.", false);
                return true;
            }
            return false;
        }

        private bool AntiAUF() {
            string gamepath = GetGamePath();
            string[] carr = GetUnauthorizedFiles(gamepath);
            Debug.Print("AUF: " + carr[0]);
            Debug.Print("DIR: " + carr[1]);
            string[] auflist = carr[0].Split('|');
            string[] dirlist = carr[1].Split('|');

            if (carr[0].Length > 0 && auflist.Length > 0) {
                // 게임 강제종료
                KillGameProcess();
                // 안내 메시지 작성
                string msg = "다음 파일들이 이동되었습니다.\n";
                string aufcontainer = Path.Combine(Program.Path_UAF, DateTime.Now.ToString("yyMMdd HHmmss"));
                for (int i = 0; i < auflist.Length; i++) {
                    string aufname = auflist[i].Split(',')[0]; // 파일 이름
                    string orgpath = Path.Combine(dirlist[i], aufname); // 원본 경로
                    string relativepath = Regex.Replace(dirlist[i].Substring(gamepath.Length), @"^\\", ""); // 상대 경로
                    string movepath = Path.Combine(aufcontainer, relativepath, aufname); // 이동 경로
                    msg += orgpath + "\n";

                    try {
                        // 폴더 생성
                        if (!Directory.Exists(Path.GetDirectoryName(movepath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(movepath));
                        // 파일 이동
                        File.Move(orgpath, movepath);
                    } catch { }
                }
                // 상태 표시
                PercentageLabel.Text = "비인가 파일 검출. 다시 실행하세요.";
                // 하단 버튼 설정
                Button_2_1.Text = "보기";
                Button_2_2.Text = "종료";
                SetButtonEvent(Button_2_1, () => { ButtonEvent_OpenPath(aufcontainer); });
                SetButtonEvent(Button_2_2, Application.Exit);
                ShowButtons(2);
                // 최상단
                this.TopMost = true;
                this.TopMost = false;
                // 안내 메시지 출력
                MessageBox.Show(msg, "비인가 파일 이동 안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private string GetAuthorizedFilesFromServer(string listurl) {
            Uri url;
            HttpWebRequest request;
            HttpWebResponse response = null;
            string allowlist = "";

            try {
                // 서버에 저장된 클레오 파일의 화이트리스트(Name,Hash)을 가져옴
                url = new Uri(listurl);
                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                allowlist = (new StreamReader(response.GetResponseStream(), Encoding.UTF8)).ReadToEnd();

                if (response.StatusCode != HttpStatusCode.OK) {
                    alert("ACL 정보를 받아오지 못했습니다.", false);
                } else {
                    response.Close();
                }
                AuthorizedFiles = allowlist;
                Debug.Print("허용된 클레오: " + allowlist);
            } catch {
                alert("런처 서버에 연결할 수 없습니다.", false);
            }
            return allowlist;
        }

        private string GetAuthorizedFiles() {
            return AuthorizedFiles;
        }

        private string[] GetUnauthorizedFiles(string path) {
            string uaflist = string.Empty;
            string dirlist = string.Empty;
            string allowlist = GetAuthorizedFiles();

            DirectoryInfo dinfo = new DirectoryInfo(path);
            foreach (DirectoryInfo dsub in dinfo.GetDirectories()) {
                string[] list = GetUnauthorizedFiles(dsub.FullName);
                if (list[0].Length > 0)
                    uaflist += list[0] + "|";
                if (list[1].Length > 0)
                    dirlist += list[1] + "|";
            }
            foreach (FileInfo finfo in dinfo.GetFiles())
                foreach (string cextension in AllowedExtension)
                    if (finfo.Extension.ToUpper() == cextension.ToUpper()) {
                        string hash = GetMD5OfFile(finfo.FullName).ToUpper();
                        if (!allowlist.ToUpper().Contains("," + hash)) {
                            uaflist += finfo.Name + "," + hash + "|";
                            dirlist += finfo.Directory + "|";
                        }
                    }

            if (uaflist.Length > 0)
                uaflist = uaflist.Substring(0, uaflist.Length - 1);
            if (dirlist.Length > 0)
                dirlist = dirlist.Substring(0, dirlist.Length - 1);
            return new string[] { uaflist, dirlist };
        }
        #endregion

        #region  < 게임 경로 탐색 > 
        private string GetGamePath() {
            string file = string.Empty;
            string path = string.Empty;

            try {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\SAMP");
                // C:\...\gta_sa.exe 형식의 문자열 중 경로만 반환
                file = reg.GetValue("gta_sa_exe").ToString();
                path = Path.GetDirectoryName(file);
                if (!File.Exists(file))
                    path = ShowGamePathDialog();
            } catch {
                path = ShowGamePathDialog();
            }

            if (path.Length == 0) {
                KillGameProcess();
                Application.Exit();
            }

            return path;
        }

        private string ShowGamePathDialog() {
            // 파일 열기 Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "GTA San Andreas 실행 파일을 지정해 주세요.";
            dialog.Filter = "GTA San Andreas (gta_sa.exe)|gta_sa.exe";

            // 다이아로그를 출력하며 취소 버튼 클릭 시 프로그램 종료
            if (dialog.ShowDialog() == DialogResult.Cancel) {
                KillGameProcess();
                Application.Exit();
                return string.Empty;
            }

            // gta_sa.exe 경로를 레지스트리에 입력
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(@"Software\SAMP", RegistryKeyPermissionCheck.ReadWriteSubTree);
            reg.SetValue("gta_sa_exe", dialog.FileName);

            return Regex.Split(dialog.FileName, "gta_sa.exe")[0];
        }
        #endregion

        #region  < 파일 해시 > 
        private string GetMD5OfFile(string filepath) {
            if (File.Exists(filepath)) {
                /*try {
                    StringBuilder strMD5 = new StringBuilder();
                    FileStream fs = new FileStream(filepath, FileMode.Open);
                    byte[] byteResult = (new MD5CryptoServiceProvider()).ComputeHash(fs);
                    fs.Close();
                    for (int i = 0; i < byteResult.Length; i++)
                        strMD5.Append(byteResult[i].ToString("X2"));
                    return strMD5.ToString();
                } catch {
                    MessageBox.Show(filepath + " 파일이 실행중입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
                using (var fs = File.OpenRead(filepath))
                using (var md5 = new MD5CryptoServiceProvider())
                    return string.Join("", md5.ComputeHash(fs).ToArray().Select(i => i.ToString("X2")));
            }
            return string.Empty;
        }
        #endregion

        #region  < 프로세스 > 
        private void KillGameProcess() {
            KillProcessesByName("samp");
            KillProcessesByName("gta_sa");
        }

        private string GetCurrentProcessName() {
            IntPtr handle = IntPtr.Zero;
            uint pid = 0;
            for(;;) {
                handle = GetForegroundWindow();
                GetWindowThreadProcessId(handle, out pid);
                return Process.GetProcessById((int)pid).ProcessName;
            }
            return String.Empty;
        }

        private bool IsProcessRunning(string pname) {
            if (Process.GetProcessesByName(pname).Length > 0)
                return true;
            return false;
        }

        private void KillProcessesByName(string pname) {
            foreach (Process process in Process.GetProcessesByName(pname))
                process.Kill();
        }
        #endregion

        #region  < 경고문 출력 >
        private void alert(String message, bool showMessageBox) {
            // 게임 강제종료
            KillGameProcess();
            // 출력
            if (showMessageBox) {
                MessageBox.Show(message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            } else {
                PercentageLabel.Text = message;
            }
            // 창을 최상단으로
            this.TopMost = true;
            this.TopMost = false;
        }
        #endregion

        #region  < Uri Scheme > 
        private void CreateUrlSchemeRegistry()
        {
            // HKEY_CLASSES_ROOT 하위키 생성 및 읽기/쓰기 권한으로 염
            RegistryKey reg;
            reg = Registry.ClassesRoot.CreateSubKey("LARP", RegistryKeyPermissionCheck.ReadWriteSubTree);
            reg.SetValue("", "LARP Launcher");
            reg.SetValue("Url Protocol", "");
            reg = reg.CreateSubKey(@"shell\open\command", RegistryKeyPermissionCheck.ReadWriteSubTree);
            reg.SetValue("", "\"" + Path.Combine(Program.Path_Setup, Program.LauncherFileName) + "\"" + " \"%1\"");
        }
        #endregion

        #region < 암호화 >
        private string GetEncryptKey() {
            return "LARP2009";
        }

        private string AESEncrypt256(string InputText, string Key) {
            string TextToEncrypt = "LARP" + InputText + "SEONGBUM";
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            // 입력받은 문자열을 바이트 배열로 변환  
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(TextToEncrypt);

            // 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서 Salt를 사용한다.  
            byte[] Salt = Encoding.ASCII.GetBytes(Key.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Key, Salt);

            // Create a encryptor from the existing SecretKey bytes.  
            // encryptor 객체를 SecretKey로부터 만든다.  
            // Secret Key에는 32바이트  
            // Initialization Vector로 16바이트를 사용  
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();

            // CryptoStream객체를 암호화된 데이터를 쓰기 위한 용도로 선언  
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);

            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(CipherBytes);
        }

        private string AESDecrypt256(string InputText, string Key) {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] EncryptedData = Convert.FromBase64String(InputText);
            byte[] Salt = Encoding.ASCII.GetBytes(Key.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Key, Salt);

            // Decryptor 객체를 만든다.  
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(EncryptedData);

            // 데이터 읽기 용도의 cryptoStream객체  
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            // 복호화된 데이터를 담을 바이트 배열을 선언한다.  
            byte[] PlainText = new byte[EncryptedData.Length];

            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string DecryptedText = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            DecryptedText = DecryptedText.Substring(4, DecryptedText.Length - 12); // LARP + SEONGBUM = 12

            return DecryptedText;
        }
        #endregion
    }
}
