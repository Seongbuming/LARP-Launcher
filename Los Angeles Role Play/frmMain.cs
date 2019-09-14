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
using System.Management;
using System.Collections.Generic;

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
        bool directstart = false;
        string username = string.Empty;

        public frmMain(string[] args) {
            InitializeComponent();
            // Url Scheme 레지스트리 생성
            CreateUrlSchemeRegistry();
            // 사용자 닉네임 파라미터 전달
            if (args.Length >= 1)
                username = args[0].Replace("larp:", string.Empty).Replace("/", string.Empty);
            else
                LoadUsernameFromRegistry();

            SetStatusMessageToDefault();

            if (username.Length > 0) {
                // Url Scheme으로 실행 시 바로 게임 실행
                if (args.Length >= 1) {
                    directstart = true;
                    StartGame();
                    return;
                }
            }

            StartUpdate();
        }

        private void SetStatusMessageToDefault() {
            if (username.Length > 0) {
                PercentageLabel.Text = username + "님, 안녕하세요!";
            } else {
                PercentageLabel.Text = "닉네임을 설정해 주세요.";
            }
        }

        private string GetUsername() {
            return username;
        }

        private void SetUsername(string name) {
            username = name;
        }

        private void LoadUsernameFromRegistry() {
            try {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\SAMP");
                username = reg.GetValue("PlayerName").ToString();
            } catch {
                username = string.Empty;
            }
        }

        private void SaveUsernameToRegistry() {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(@"Software\SAMP", RegistryKeyPermissionCheck.ReadWriteSubTree);
            reg.SetValue("PlayerName", username);
        }

        private bool GetDirectStart() {
            return directstart;
        }
        #endregion

        #region < 게임 실행 >
        string StartedGamePath;

        private void GameStart_Tick(object sender, EventArgs e) {
            GameStart.Stop();
            KillGameProcess();
            Debug.Print("InitLevel - " + InitLevel + " - ");
            switch (InitLevel) {
                case 1: // 서버 연결 확인
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
                        SetButtonEvent(Button_2_2, ButtonEvent_Exit);
                        ShowButtons(2);
                        this.TopMost = true;
                        this.TopMost = false;
                    } else {
                        GameStart.Start();
                    }

                    break;
                case 2: // 런처 업데이트
                    StartUpdate();
                    break;
                case 3: // 게임 구성 파일 변조 검사 및 패치
                    // 비인가 프로그램 차단 (파일 변조 검사 제외)
                    ResetAuthorizedFiles();
                    LoadAuthorizedFilesFromServer(Program.LauncherURL + "/getfilelist.php?type=allowedfiles");
                    LoadAuthorizedFilesFromServer(Program.LauncherURL + "/getfilelist.php?type=patch");
                    LoadAuthorizedFilesFromServer(Program.LauncherURL + "/getfilelist.php?type=launcher");
                    if (BlockUnauthorizedPrograms())
                        return;
                    // 검사 및 패치
                    if (!CompareMD5OfGameFile(Program.LauncherURL + "/getfilelist.php?type=patch"))
                        StartPatch(Program.LauncherURL + "/patch", GetDissimilarFiles());
                    else
                        // 게임 실행 단계 진행
                        GameStart.Start();
                    break;
                case 4: // 게임 실행
                    // 비인가 프로그램 차단 (파일 변조 검사 포함)
                    if (BlockUnauthorizedPrograms())
                        return;
                    // 닉네임 레지스트리 동기화
                    SaveUsernameToRegistry();

                    if (GetNewLauncherPath() != String.Empty) {
                        // URL Scheme
                        CreateUrlSchemeRegistry();
                        // 최신 런처 실행
                        RunNewLauncher();
                    } else if (string.Compare(GetUsername(), "NULL") == 0) {
                        PercentageLabel.Text = "설치 완료";
                        Button_2_1.Text = "인포웹";
                        Button_2_2.Text = "종료";
                        SetButtonEvent(Button_2_1, ButtonEvent_OpenInfoweb);
                        SetButtonEvent(Button_2_2, ButtonEvent_Exit);
                        ShowButtons(2);
                        this.TopMost = true;
                        this.TopMost = false;
                    } else {
                        PercentageLabel.Text = "게임에 접속중입니다.";
                        // SAMP 실행
                        StartedGamePath = GetGamePath();
                        Process.Start(Path.Combine(StartedGamePath, "samp.exe"), "server.la-rp.co.kr");
                        GameExit.Start();
                    }
                    break;
            }
            InitLevel++;
        }

        private void GameExit_Tick(object sender, EventArgs e) {
            if (IsGameRunning()) {
                if (ExitLevel == 0)
                    this.Hide();
                BlockUnauthorizedPrograms(true);
                ExitLevel = 1;
            } else if (ExitLevel > 0) {
                ExitLevel = 0;
                try {
                    string sampchatlogpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GTA San Andreas User Files", "SAMP");
                    string larpchatlogfile = Path.Combine(Program.Path_ChatLog, "ChatLog" + DateTime.Now.ToString("-yyMMdd-HHmmss") + ".txt");
                    if (!Directory.Exists(Program.Path_ChatLog))
                        Directory.CreateDirectory(Program.Path_ChatLog);
                    File.Copy(Path.Combine(sampchatlogpath, "chatlog.txt"), larpchatlogfile, true);
                    PercentageLabel.Text = "챗로그가 자동 백업되었습니다.";
                } catch {
                    PercentageLabel.Text = "챗로그 백업에 실패했습니다.";
                }
                // 하단 버튼 설정
                Button_2_1.Text = "열기";
                Button_2_2.Text = "종료";
                SetButtonEvent(Button_2_1, ButtonEvent_OpenChatLog);
                SetButtonEvent(Button_2_2, ButtonEvent_Exit);
                ShowButtons(2);
                this.Show();
                this.TopMost = true;
                this.TopMost = false;
            }
        }

        private void StartGame() {
            InitLevel = 1;
            GameStart.Start();
        }

        private int GetInitLevel() {
            return InitLevel;
        }

        private string GetStartedGamePath() {
            return StartedGamePath;
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

        private void SetButtonsToDefault() {
            Button_3_1.Text = "게임실행";
            Button_3_2.Text = "닉네임 설정";
            Button_3_3.Text = "종료";
            SetButtonEvent(Button_3_1, ButtonEvent_GameStart);
            SetButtonEvent(Button_3_2, ButtonEvent_SetNickname);
            SetButtonEvent(Button_3_3, ButtonEvent_Exit);
            ShowButtons(3);
        }

        private void ButtonEvent_Exit() {
            KillGameProcess();
            Application.Exit();
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

        private void ButtonEvent_GameStart() {
            if (GetUsername().Length == 0) {
                MessageBox.Show("닉네임을 설정하셔야 게임을 시작할 수 있습니다.");
            } else {
                StartGame();
            }
        }

        private void ButtonEvent_SetNickname() {
            string name = Microsoft.VisualBasic.Interaction.InputBox("사용하실 닉네임을 입력하세요.", "닉네임 설정", GetUsername());
            SetUsername(name);
            SetStatusMessageToDefault();
            SaveUsernameToRegistry();
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
        /*string sMode;
        string[] sFileList;
        string sHost, sUrlToReadFileFrom, sFilePathToWriteFileTo;
        int sCurrentFileIndex, sFinalIndex;*/
        string PatchFileHost, PatchFileDest;
        string[] PatchFileList;
        int PatchFileIndex;
        string NewLauncherPath = String.Empty;

        private void StartPatch(string host, string[] filelist) {
            // 비인가 프로그램 차단
            if (BlockUnauthorizedPrograms())
                return;

            // Progress Bar 초기화
            SetProgressBar(0, 0);
            SetProgressBar(1, 0);
            // 파일 다운로드
            DownloadPatchFiles(host, GetGamePath(), filelist);
        }

        private void StartUpdate() {
            Uri url;
            HttpWebRequest request;
            HttpWebResponse response;
            HttpStatusCode statuscode;
            string newhash;

            PercentageLabel.Text = "업데이트 확인중";
            Button_1_1.Text = "종료";
            SetButtonEvent(Button_1_1, ButtonEvent_Exit);
            ShowButtons(1);

            try {
                // 최신 런처의 Hash를 가져옴
                url = new Uri(Program.LauncherURL + "/getfilehash.php?name=launcher/" + Program.LauncherFileName);
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
                SetButtonEvent(Button_2_2, ButtonEvent_Exit);
                ShowButtons(2);
                this.TopMost = true;
                this.TopMost = false;
                return;
            }
            
            // 현재 버전의 Hash를 가져옴
            string ehash = GetMD5OfFile(Application.ExecutablePath).ToUpper();

            if ((string.Compare(newhash, ehash, true) == 0 && Program.IsExecutedByLauncher()) || Program.TestMode) { // 런처가 최신 버전인 경우
                if (GetInitLevel() > 0) {
                    GameStart.Start();
                } else {
                    SetStatusMessageToDefault();
                    SetButtonsToDefault();
                }
            } else { // 런처 업데이트가 필요한 경우
                // Progress Bar 초기화
                SetProgressBar(0, 0);
                SetProgressBar(1, 0);
                // 런처 업데이트
                UpdateLauncher();
            }
        }

        private void RunNewLauncher() {
            Process.Start(GetNewLauncherPath(), (GetDirectStart()) ? GetUsername() : string.Empty);
            Application.Exit();
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

        private void DownloadPatchFiles_Completed(object sender, AsyncCompletedEventArgs e) {
            PatchFileIndex++;
            if (e.Cancelled) {
                PercentageLabel.Text = "패치가 취소되었습니다.";
                this.TopMost = true;
                this.TopMost = false;
                SetButtonsToDefault();
            } else if (PatchFileIndex >= PatchFileList.Length) {
                GameStart.Start();
            } else {
                DownloadPatchFiles(PatchFileHost, PatchFileDest, PatchFileList, PatchFileIndex);
            }
        }

        private void DownloadPatchFiles_ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            // Unit Progress Bar 갱신
            SetProgressBar(0, e.ProgressPercentage);
            // Total Progress Bar 갱신
            SetProgressBar(1, (int)((PatchFileIndex + e.ProgressPercentage / 100) * 100 / PatchFileList.Length));
        }

        private void DownloadPatchFiles(string host, string dest, string[] filelist, int index = 0) {
            PatchFileHost = host;
            PatchFileDest = dest;
            PatchFileList = filelist;
            PatchFileIndex = index;

            // PercentageLabel 갱신
            PercentageLabel.Text = "패치중: " + filelist[index];

            using (var wc = new WebClient()) {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadPatchFiles_Completed);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadPatchFiles_ProgressChanged);
                wc.DownloadFileAsync(new Uri(host + "/" + filelist[index]), dest + "/" + filelist[index]);
            }
        }

        private void LauncherUpdate_Completed(object sender, AsyncCompletedEventArgs e) {
            if (e.Cancelled) {
                PercentageLabel.Text = "업데이트가 취소되었습니다.";
                this.TopMost = true;
                this.TopMost = false;
                SetButtonsToDefault();
            } else if (GetInitLevel() > 0) {
                GameStart.Start();
            } else {
                RunNewLauncher();
            }
        }

        private void LauncherUpdate_ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            // Unit Progress Bar 갱신
            SetProgressBar(0, e.ProgressPercentage);
            // Total Progress Bar 갱신
            SetProgressBar(1, e.ProgressPercentage);
        }

        private void UpdateLauncher() {
            string host = Program.LauncherURL + "/launcher/" + Program.LauncherFileName;
            string dest = Path.Combine(Program.Path_Setup,
                (Program.IsExecutedByLauncher()) ? Program.UpdaterFileName : Program.LauncherFileName);
            NewLauncherPath = dest;

            // PercentageLabel 갱신
            PercentageLabel.Text = "최신 런처 설치중";
            
            // 경로 생성
            try {
                if (!Directory.Exists(Program.Path_Setup))
                    Directory.CreateDirectory(Program.Path_Setup);
            } catch { }

            // 파일이 이미 존재할 시 삭제
            try {
                if (File.Exists(dest))
                    File.Delete(dest);
            } catch { }

            using (var wc = new WebClient()) {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(LauncherUpdate_Completed);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(LauncherUpdate_ProgressChanged);
                wc.DownloadFileAsync(new Uri(host), dest);
            }
        }
        #endregion

        #region  < 게임 구성 파일 변조 검사 > 
        private List<string> DissimilarFiles;

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
            DissimilarFiles = new List<string>();

            // 서버에 저장된 파일과 동일한 것인지 검사
            int dcount = 0;
            for (int i = 0; i < fdata.Length; i++) {
                string[] sdata = fdata[i].Split(',');
                if (GetMD5OfFile(Path.Combine(GetGamePath(), sdata[0])).ToUpper() != sdata[1].ToUpper()) {
                    Debug.Print("<DF> " + sdata[0] + ": " + GetMD5OfFile(Path.Combine(GetGamePath(), sdata[0])).ToUpper());
                    DissimilarFiles.Add(sdata[0]);
                    dcount++;
                }
            }

            // 서버의 것과 동일하면 true
            if (dcount == 0)
                return true;
            return false;
        }

        private string[] GetDissimilarFiles() {
            return DissimilarFiles.ToArray();
        }
        #endregion

        #region  < 비인가 프로그램 차단 > 
        string[] AllowedExtension = new string[] { ".cleo", ".cs", ".asi", ".exe", ".dll" };
        string AuthorizedFiles = String.Empty;

        private bool BlockUnauthorizedPrograms(bool blockdissimilarfiles = false) {
            bool blocked = false;
            if (AntiCheat() || AntiAUF())
                blocked = true;
            if (!CompareMD5OfGameFile(Program.LauncherURL + "/getfilelist.php?type=patch") && blockdissimilarfiles) {
                blocked = true;
                // 게임 강제종료
                KillGameProcess();
                // 하단 버튼 설정
                Button_1_1.Text = "종료";
                SetButtonEvent(Button_1_1, ButtonEvent_Exit);
                ShowButtons(1);
                // 오류 메시지 출력
                alert("게임 구성 파일이 변조되었습니다.", true);
            }
            return blocked;
        }

        private bool AntiCheat() {
            FileInfo gtasa = new FileInfo(Path.Combine(GetGamePath(), "gta_sa.exe"));
            double filesize = Math.Round((double)(gtasa.Length / 1000000));
            if (filesize < 14 || filesize > 16) {
                KillGameProcess();
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
                string aufcontainer = Path.Combine(Program.Path_UAF, DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
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
                SetButtonEvent(Button_2_2, ButtonEvent_Exit);
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

        private void ResetAuthorizedFiles() {
            AuthorizedFiles = string.Empty;
        }

        private string LoadAuthorizedFilesFromServer(string listurl) {
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
                    alert("클레오 화이트리스트 정보를 받아오지 못했습니다.", false);
                } else {
                    response.Close();
                }

                if (AuthorizedFiles.Length == 0) {
                    AuthorizedFiles = allowlist;
                } else {
                    AuthorizedFiles += "|" + allowlist;
                }
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
                    if (string.Compare(finfo.Extension.ToUpper(), cextension.ToUpper()) == 0) {
                        string hash = GetMD5OfFile(finfo.FullName).ToUpper();
                        if (!allowlist.ToUpper().Contains("," + hash) && string.Compare(finfo.Name.ToUpper(), "GTA_SA.EXE") != 0) {
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

        #region < 하드 디스크 일련번호 >
        private string[] GetHDSerials() {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            string[] serials = new string[searcher.Get().Count];
            int i = 0;
            foreach (ManagementObject wmi in searcher.Get()) {
                serials[i] = wmi["SerialNumber"].ToString();
                i++;
            }

            return serials;
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
            handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out pid);
            return Process.GetProcessById((int)pid).ProcessName;
        }

        private bool IsGameRunning() {
            foreach (Process proc in Process.GetProcessesByName("gta_sa")) {
                if (string.Compare(proc.MainModule.FileName.ToUpper(), Path.Combine(GetStartedGamePath(), "gta_sa.exe").ToUpper()) == 0)
                    return true;
            }
            return false;
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
            return ".oKd_WTPKbJLmm0;";
        }

        private string AESEncrypt256(string InputText, string Key) {
            string TextToEncrypt = "9[9I$0le" + InputText + "[rDr8l-6";
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
