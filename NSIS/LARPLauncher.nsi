;##########################################################
; define Settings
;##########################################################
;----------------------------------------------------------
; 배포 프로그램 이름, 버전 및 기타 변수
;----------------------------------------------------------
!define PRODUCT_NAME "Los Angeles Role Play Launcher"
!define PRODUCT_SHORT_NAME "LARPLauncher"
!define PRODUCT_GROUP "Los Angeles Role Play"
!define PRODUCT_VERSION "1.4"
!define PRODUCT_PUBLISHER "Los Angeles Role Play"
!define PRODUCT_WEBSITE "http://la-rp.co.kr"
!define EXEFILE_NAME "LARPLauncher"
!define EXELINK_NAME "LARP"
!define EXEFULL_NAME "${EXEFILE_NAME}.exe"
!define EXEFILE_DIR "$PROGRAMFILES\${PRODUCT_GROUP}"
!define OUTFILE_NAME "${PRODUCT_SHORT_NAME}_${PRODUCT_VERSION}_Installer.exe"
BrandingText ":: ${PRODUCT_PUBLISHER} - [${PRODUCT_WEBSITE}]"
;----------------------------------------------------------
; 레지스트리 키 지정
;----------------------------------------------------------
!define REG_ROOT_KEY "HKLM"
!define REG_UNROOT_KEY "HKLM"
!define REG_APPDIR_KEY "Software\Microsoft\Windows\CurrentVersion\App Path\${EXEFULL_NAME}"
!define REG_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_GROUP}"
;##########################################################
; MUI Settings
;##########################################################
;----------------------------------------------------------
; 관리자 권한 사용
;----------------------------------------------------------
RequestExecutionLevel admin
;----------------------------------------------------------
; include
;----------------------------------------------------------
!include "MUI.nsh"
;----------------------------------------------------------
; 인스톨러 & 언인스톨러 아이콘 설정
;----------------------------------------------------------
!define MUI_ICON "icon\install.ico"
!define MUI_UNICON "icon\uninstall.ico"
;----------------------------------------------------------
; 인스톨러 & 언인스톨러 닫을 경우 경고 메시지 상자를 출력
;----------------------------------------------------------
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
;----------------------------------------------------------
; 인스톨 & 언인스톨 완료 시 자동으로 닫지 않음
;----------------------------------------------------------
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE
;##########################################################
; MUI Pages
;##########################################################
;----------------------------------------------------------
; 페이지 디자인
;----------------------------------------------------------
; 인스톨러 & 언인스톨러 헤더 이미지 (150x57)
;!define MUI_HEADERIMAGE
;!define MUI_HEADERIMAGE_BITMAP_NOSTRETCH
;!define MUI_HEADERIMAGE_BITMAP "img\header_inst.bmp" ; 150x57
;!define MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH
;!define MUI_HEADERIMAGE_UNBITMAP "img\header_uninst.bmp" ; 150x57
!define MUI_BGCOLOR FFFFFF ; Default: FFFFFF
; 인스톨러 첫 페이지 및 마지막 페이지 이미지 (191x290)
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_WELCOMEFINISHPAGE_BITMAP "img\welcome_inst.bmp"
; 언인스톨러 첫 페이지 및 마지막 페이지 이미지 (191x290)
!define MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "img\welcome_uninst.bmp"
;----------------------------------------------------------
; 인스톨러 페이지
;----------------------------------------------------------
!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_COMPONENTS
;!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
;----------------------------------------------------------
; 언인스톨러 페이지
;----------------------------------------------------------
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;----------------------------------------------------------
; 언어
;----------------------------------------------------------
!insertmacro MUI_LANGUAGE "Korean"
;##########################################################
; NSIS Settings
;##########################################################
;----------------------------------------------------------
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${OUTFILE_NAME}"
InstallDir "${EXEFILE_DIR}"
ShowInstDetails show
ShowUninstDetails show
SetCompress off
;SetCompressor lzma
SetOverWrite on ; on|off|try|ifnewer|ifdiff|lastused
;----------------------------------------------------------
InstallDirRegKey ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "Install_Dir"
;##########################################################
; SECTION
;##########################################################
;----------------------------------------------------------
Section "!Base (required)"
    DetailPrint "설치 준비중..."
    SetDetailsPrint listonly
    SetOutPath "$INSTDIR"
    File "..\Los Angeles Role Play\bin\Release\Confused\LARPLauncher.exe"
    ; 바탕화면에 바로가기 등록
    CreateShortCut "$DESKTOP\${EXELINK_NAME}.lnk" "$INSTDIR\${EXEFULL_NAME}"
    ; 시작-프로그램에 바로가기 등록
    CreateDirectory "$SMPROGRAMS\${PRODUCT_GROUP}"
    CreateShortCut "$SMPROGRAMS\${PRODUCT_GROUP}\${EXELINK_NAME}.lnk" "$INSTDIR\${EXEFULL_NAME}"
    CreateShortCut "$SMPROGRAMS\${PRODUCT_GROUP}\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
    ; 레지스트리 - 설치 경로
    WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "Install_Dir" "$INSTDIR"
    WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "" "$INSTDIR\${EXEFULL_NAME}"
    ; 레지스트리 - 삭제 정보
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayName" "$(^Name)"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${EXEFULL_NAME}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEBSITE}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
    ; 언인스톨러 생성
    WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd
;----------------------------------------------------------
Section Uninstall
    RMDir /r "$INSTDIR"
    Delete "$DESKTOP\${EXELINK_NAME}.lnk"
    RMDir /r "$SMPROGRAMS\${PRODUCT_GROUP}"
    DeleteRegKey ${REG_ROOT_KEY} "${REG_APPDIR_KEY}"
    DeleteRegKey ${REG_UNROOT_KEY} "${REG_UNINST_KEY}"
SectionEnd
