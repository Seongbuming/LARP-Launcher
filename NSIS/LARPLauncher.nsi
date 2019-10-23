;##########################################################
; define Settings
;##########################################################
;----------------------------------------------------------
; ���� ���α׷� �̸�, ���� �� ��Ÿ ����
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
; ������Ʈ�� Ű ����
;----------------------------------------------------------
!define REG_ROOT_KEY "HKLM"
!define REG_UNROOT_KEY "HKLM"
!define REG_APPDIR_KEY "Software\Microsoft\Windows\CurrentVersion\App Path\${EXEFULL_NAME}"
!define REG_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_GROUP}"
;##########################################################
; MUI Settings
;##########################################################
;----------------------------------------------------------
; ������ ���� ���
;----------------------------------------------------------
RequestExecutionLevel admin
;----------------------------------------------------------
; include
;----------------------------------------------------------
!include "MUI.nsh"
;----------------------------------------------------------
; �ν��緯 & ���ν��緯 ������ ����
;----------------------------------------------------------
!define MUI_ICON "icon\install.ico"
!define MUI_UNICON "icon\uninstall.ico"
;----------------------------------------------------------
; �ν��緯 & ���ν��緯 ���� ��� ��� �޽��� ���ڸ� ���
;----------------------------------------------------------
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
;----------------------------------------------------------
; �ν��� & ���ν��� �Ϸ� �� �ڵ����� ���� ����
;----------------------------------------------------------
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_UNFINISHPAGE_NOAUTOCLOSE
;##########################################################
; MUI Pages
;##########################################################
;----------------------------------------------------------
; ������ ������
;----------------------------------------------------------
; �ν��緯 & ���ν��緯 ��� �̹��� (150x57)
;!define MUI_HEADERIMAGE
;!define MUI_HEADERIMAGE_BITMAP_NOSTRETCH
;!define MUI_HEADERIMAGE_BITMAP "img\header_inst.bmp" ; 150x57
;!define MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH
;!define MUI_HEADERIMAGE_UNBITMAP "img\header_uninst.bmp" ; 150x57
!define MUI_BGCOLOR FFFFFF ; Default: FFFFFF
; �ν��緯 ù ������ �� ������ ������ �̹��� (191x290)
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_WELCOMEFINISHPAGE_BITMAP "img\welcome_inst.bmp"
; ���ν��緯 ù ������ �� ������ ������ �̹��� (191x290)
!define MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "img\welcome_uninst.bmp"
;----------------------------------------------------------
; �ν��緯 ������
;----------------------------------------------------------
!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_COMPONENTS
;!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
;----------------------------------------------------------
; ���ν��緯 ������
;----------------------------------------------------------
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
;----------------------------------------------------------
; ���
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
    DetailPrint "��ġ �غ���..."
    SetDetailsPrint listonly
    SetOutPath "$INSTDIR"
    File "..\Los Angeles Role Play\bin\Release\Confused\LARPLauncher.exe"
    ; ����ȭ�鿡 �ٷΰ��� ���
    CreateShortCut "$DESKTOP\${EXELINK_NAME}.lnk" "$INSTDIR\${EXEFULL_NAME}"
    ; ����-���α׷��� �ٷΰ��� ���
    CreateDirectory "$SMPROGRAMS\${PRODUCT_GROUP}"
    CreateShortCut "$SMPROGRAMS\${PRODUCT_GROUP}\${EXELINK_NAME}.lnk" "$INSTDIR\${EXEFULL_NAME}"
    CreateShortCut "$SMPROGRAMS\${PRODUCT_GROUP}\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
    ; ������Ʈ�� - ��ġ ���
    WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "Install_Dir" "$INSTDIR"
    WriteRegStr ${REG_ROOT_KEY} "${REG_APPDIR_KEY}" "" "$INSTDIR\${EXEFULL_NAME}"
    ; ������Ʈ�� - ���� ����
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayName" "$(^Name)"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninstall.exe"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${EXEFULL_NAME}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEBSITE}"
    WriteRegStr ${REG_UNROOT_KEY} "${REG_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
    ; ���ν��緯 ����
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
