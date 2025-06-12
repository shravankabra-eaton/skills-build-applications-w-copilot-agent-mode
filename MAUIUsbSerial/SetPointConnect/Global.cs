using System;
using System.IO;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using PXR.Screens;
using System.IO.Ports;
using PXR.Resources.Strings;
using System.Collections.Generic;
using System.Globalization;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Media;
using PXR.Screens.Scrs_EventSummaries;
using System.Threading;
using PXR.Screens.Scrs_Reports;
using PXR.Screens.License;
using PXR.Communication;
using static PXR.Screens.Scrs_EventSummaries.Scrs_EventSummaryLoading;
using static PXR.Screens.Scrs_EventSummaries.Scrs_EventSummaryLoading_ViewModel;
using System.Windows.Controls.Primitives;
using PXR.Screens.Scrs_Test;
using System.Diagnostics;
using System.Windows.Threading;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using PXR.Screens.Scrs_Password;
//using Windows.UI.Xaml;

namespace PXR
{
    /// <summary>
    /// Filename: Global.cs
    /// Created:
    /// Author: Sarah M. Norris
    /// Description:    This class contains the functions that are called from multiple locations and the global string library that
    ///                 is used for comparing input from the trip unit. Finally, this class contains the error and warning messages
    ///                 that are used in the wizards
    /// Modifications:
    /// 5/13/13         SMN- Added listbox converter
    ///                 SMN- Added split converter
    ///     
    /// </summary>

    public enum LocalGroundStatus
    {
        Trip,
        Alarm,
        Off,
        Other
    }


    public static class Global
    {
        public static bool CloseWindowEventSummaryLoading;
        public static GattCharacteristic gattCharacteristicRenProperties;
        private static List<byte[]> byteList;
        private static bool isAllDataReceived;
        private static int groupCountACB_MCCB = 0;
        private static Stopwatch stopwatchConnErr = new Stopwatch();
        public static bool Snapshotdataavailiable = false;
        public static bool Disturbancedataavailiable = false;
        public static int RTUTCPcount = 0;


        public static bool isCommunicatingUsingBluetooth = false;
        public static List<FeatureResponse> features = new List<FeatureResponse>();
        public static GattCharacteristic gattCharacteristicProperties;
        public static bool charectersticAccess;
        public static GattDeviceService gattDeviceService;
        //public static BluetoothLEDevice bluetoothLeDevice;
        public static bool NeutralPhaseChecked = false;
        public static bool isFromResetOperation = false;
        public static bool NeutralPhase60 = false;
        public static bool isErrorOccured = false;
        public static double actualcurrentforTolerance = 0;
        private static System.Timers.Timer mTimer = new System.Timers.Timer(2000);
        public static bool verifyAdmin;
        public static bool verifyUser;
        public static bool VerifyForUserOperation;
        public static bool VerifyForAdminOperation;
        //TS DB details - Removed connection string and its db pswd string from config file, as it was getting exposed as plain text and added here.
        public static string databasepath = "\\Resources\\Database\\TroubleShootDB.sdf; Password = 'PXPMTS'";
        //User Type
        public static UserType currentUser;
        // Tag names
        public static ObservableCollection<DeviceInformation> devicelist = null;
        public static ObservableCollection<DeviceInformation> Paireddevicelist = null;
        public const String str_ITEM = "selectionitem";
        public const String str_mccbtripstyle = "mccbtripstyle";
        public const String str_NUMBER = "number";
        public const String str_BOOL = "bool";
        public const String str_READONLY = "readonly";
        public const String str_VISIBLE = "visible";
        public const String str_bnumbervisible = "bnumbervisible";
        public const String str_ENABLE = "enable";
        public const String str_MIN = "min";
        public const String str_MAX = "max";
        public const String str_mincalculation = "mincalculation";
        public const String str_maxcalculation = "maxcalculation";
        public const String str_CalculatedValue = "calculatedvalue";
        public const String str_ExcludedValue = "excludedvalue";
        public const String str_STEPSIZE = "stepsize";
        public const String str_CONVERSION = "conversion";
        public const String str_DEFAULT = "default";
        public const String str_DESCRIPTION = "description";
        public const String str_wizard_ResetStartup = "ResetStartup";
        public const String str_SELECTEDITEM = "selecteditem";
        public const String str_LOOKUPDATA = "lookupdata";
        public const String str_NOTAVAILABLE = "notavailable";
        public const String str_LABEL = "label";
        public const String str_SubGroupName = "subgroupname";
        public const string PTM_DEVICE = "PTM";

        public const String str_wizard_CONNECT = "wizard_Connect";
        public const String str_wizard_TEST_FEATURE_EXPORT_SETPOINT = "wizard_rest_feature_export_setpoint";
        public const String str_wizard_SELECT_COMM_PORT = "wizard_select_comm_port";
        public const String str_wizard_OPENFILE = "wizard_OpenFile";
        public const String str_wizard_NEWFILE = "wizard_NewFile";
        public const String str_wizard_EXPORTFILE = "wizard_Export";
        public const string str_wizard_ADDLANGUAGE = "wizard_AddLanguage";

        public const string str_wizard_AUTOCONNECT = "wizard_autoconnect"; // By Ashish
        public const string str_wizard_ERROR = "wizard_error";              // by Ashish
        public const string str_wizard_PasswordCorrect = "wizard_PasswordCorrect";              // by VishalN
        public const string str_wizard_PasswordIncorrect = "wizard_PasswordIncorrect";              // by VishalN

        public const String str_wizard_EXPORTUI = "wizard_ExportUI";
        public const String str_wizard_IMPORTMODEL = "wizard_ImportModel";
        public const String str_wizard_SAVEFILE = "wizard_SaveFile";
        public const String str_wizard_SAVEFILEONCLOSE = "wizard_SaveFileOnClose";
        public static String CopyrightInformation = filepath + "Resources\\CopyrightInformation.pdf";

        public const String str_setPointFilename = "SetPoints.csv";
        //public const String str_firmwareVersionFilename = "firmwareVersion.json"; //SL Commented to match with config tool
        public const String str_firmwareVersionFilename = "readme.json";
        public const String str_wizard_RESET_Connect = "wizard_Reset_Connect";
        public const String str_wizard_RemoteControlStartup = "wizard_remoteControlStartup";
        public const String str_wizard_AppSettingsStartup = "wizard_AppSettingsStartup";
        public const String str_wizard_RESET = "wizard_Reset";

        public const String str_wizard_RESET_DateTime = "wizard_Reset_DateTime";
        public const String str_wizard_RESET_WaveForm = "wizard_Reset_WaveForm";

        public const String str_wizard_TEST = "wizard_Test";

        public const String str_app_ID_Table = "app_ID_Table";
        public const String str_pxset1_ID_Table = "pxset1_ID_Table";
        public const String str_pxset2_ID_Table = "pxset2_ID_Table";

        // boolean Values
        public const String str_True = "0001";      // By Ashish 
        public const String str_False = "0000";     // By Ashish 

        //IP control constants Added by Sunny
        public const String ipControl1 = "IP001";
        public const String ipControl2 = "IP002";
        public const String ipControl3 = "IP003";
        public const string ipMask = "255.255.255.";
        public const String str_emptyorna = "emptyorna";        //Added by Astha
        public const String onlabel = "onlabel";
        public const String offlabel = "offlabel";
        public static byte Line_Frequency;

        public static string portName = string.Empty;
        public static bool isSaveCalledFromClose = false;

        //  public static String userSelected_ComPort;                  // Stores the com port the user selected if Connect was used. This keeps the user from needing to 
        // to select it again.

        public enum windowChoice { STARTSCREEN, MAIN }              // Where the application goes after the export wizard has exited. 
        public static windowChoice choice = windowChoice.MAIN;
        public enum MATCH { NO_RATINGPLUG, NO_STYLE, YES_MATCH, NO_FIRMWARE, NO_GST }    // Used for responses for uploading to trip unit
        public static String path = AppDomain.CurrentDomain.BaseDirectory;

        public static String filepath = path.Replace(@"\bin\Debug", string.Empty);
        public static String filePath_installedModels = filepath + "Resources\\DataFiles\\installedModels.csv";
        public static String filePath_basePath = filepath + "Resources\\DataFiles\\";
        public static String filePath_basePath_ZIP = filepath + "Resources\\";
        public static String filePath_PXRxmlFile = filepath + "Resources\\Styles\\Model_PXR20V000LGAC.xml";
        //public static String filePath_PXRStyleSelection = filepath + "Resources\\DataFiles\\PXRStyle.xml";
        public static String filePath_ARMSData = filepath + "Resources\\DataFiles\\ARMSData.xml";
        public static String filePath_mergedstylesxmlFile = filepath + "Resources\\DataFiles\\PXR2025ACB.xml";
        public static String filePath_mergedstylesxmlFile_3_0 = filepath + "Resources\\DataFiles\\PXR2025_3.0_ACB.xml";
        public static String filePath_parameterSelectionFile = filepath + "Resources\\DataFiles\\ACB_MCCB_Para_Selection.xml";
        //Added to get PTM file Location
        public static String filePath_parameterSelectionFile_PTM = filepath + "Resources\\DataFiles\\PXR2025PTM.xml";
        public static String parsed_Template_File = string.Empty;
        public static string parseFor_IDTable = string.Empty;
        public static String plusimage = filepath + "Resources\\Graphics\\plus.png";
        public static String minusimage = filepath + "Resources\\Graphics\\minus.png";
        // For parsing of the MCCB Template
        public static String filePath_merged_mccb_xmlFile = filepath + "Resources\\DataFiles\\PXR2025MCCB.xml";
        public static String filePath_merged_nzm_xmlFile = filepath + "Resources\\DataFiles\\PXR2025NZM.xml";
        public static String filePath_merged_acbPXR35_xmlFile = filepath + "Resources\\DataFiles\\PXR35ACB.xml";
        public static String filePath_merged_PTM_xmlfile = filepath + "Resources\\DataFiles\\PXR2025PTM.xml";
        //Sunny :Constants For file creation
        public const String PDEncryption = "12345"; // Encryption /Decryption password
        public const String BASE64 = "data:;base64,";
        public const string ACBDEVICE = "ACB";
        public const string ACB_02_01_XX_DEVICE = "ACB_2.1.X";
        public const string ACB_03_00_XX_DEVICE = "ACB_3.0.X";
        public const string ACB_03_01_XX_DEVICE = "ACB_3.1.X";
        public const string ACB_03_02_XX_DEVICE = "ACB_3.2.X";
        public static string appFirmware4 = Resource.GEN002DItem0000;
        public static String deviceFirmware4 = String.Empty;
        public const string MCCBDEVICE = "MCCB";
        public const string NZMDEVICE = "NZM";
        public const string ACB_PXR35_DEVICE = "ACB_PXR35";
        public const String ACBTEMPLATE = "PXR2025ACB";
        public const String PTM_TEMPLATE = "PXR2025PTM";
        public const String ACB3_0TEMPLATE = "PXR2025ACB3.0";
        public const String ACB_PXR35_TEMPLATE = "PXR35ACB";
        public const String MCCBTEMPLATE = "PXR2025MCCB";
        public const String NZMTEMPLATE = "PXR2025NZM";
        public const string strData_NOTAVAILABLE = "A8";
        public const int NOTAVAILABLE = 0XA8;
        public const int WrongDataType = 0X03;
        public const string MCCBDeviceName = "PXR 20/25 - MCCB";
        public const string ACBDeviceName = "PXR 20/25 - ACB";
        public static SerialPort mSerialPort = new SerialPort();
        public static bool isDemoMode = false;
        public static string mainScreenDeviceSelectionDropdown = string.Empty;
        public static string NZMFullFW_Version = string.Empty;
        public static bool isReportAlreadyOpen = false;
        public static bool isUpdatingSettingBackForRestrictedUser = false;
        //Remote date time variable
        public static String msg_ConnectToTripUnit_Heading = Resource.ConnectToTripUnitHeading;


        public static string DisplayHeaderAllReport;
        public static string DisplayHeaderInteractiveTroubleshooting;

        public static bool AllReportFlag = true;
        public static System.Globalization.CultureInfo UI_culture = System.Globalization.CultureInfo.CurrentUICulture;
        #region SetpointCurves variables
        public static bool show_curve = false;
        #endregion

        #region TestFeature variables
        public static ArrayList setpointIdArrayGroupZero = new ArrayList();
        public static ArrayList setpointIdArrayGroupOne = new ArrayList();
        public static ObservableCollection<TestSummary> PrevioustestSummary = new ObservableCollection<TestSummary>();
        public static ObservableCollection<TestSummary> CurrentdataTestSummary = new ObservableCollection<TestSummary>();
        public static ObservableCollection<ChangeSummary> TestSetpointChanges = new ObservableCollection<ChangeSummary>();
        public static bool IsTestTablechanged = false;
        public static bool IsIncludeCurveplots = false;
        public static bool IsIncludeFailedTest = false;
        public static bool IsIncludeCancelTest = false;
        public static bool IsIncludeInsulationContactTest = false;
        public static bool IsGenerateReportClicked = false;
        public static ArrayList arrCurvedisplay = new ArrayList();
        public static bool isTestFeatureFlow = false;
        public static bool isTestPerformedInCurrentTestCycle = false;
        public static bool isTestLibraryChangeLoaded = false;
        public static bool isTestAllowed;
        public static bool IsContinueTestingClicked = false;
        public static string glbTripTime;
        public static string glbTestX1r = String.Empty;
        public static string glbTestX1n = String.Empty;
        public static string glbTestCurrent;
        public static string glbTripCause;
        public static string glbPhaseTested;
        public static string glbSimulatedCur;
        public static string glbActualCur;
        public static string glbActualCurMultiple;
        public static string glbBreakerFrameCurrent;
        public static string glbBreakerFrameSrNo;
        public static string glbTripUnitSrNo;
        public static string dataTripCause = string.Empty;
        public static string glbPassFail;
        public static string glbTestType;
        public static string glbOpenBkr;
        public static string TestPassFailResult;
        public static bool GFTestNA;
        public static string TestFeatureLast_CompPort;
        public static bool mIsGSTResidual;
        public static double mLongDelayPickup, mShortDelayPickup, mMMLevel, mLongDelayTime, mGroundFaultPickup, mInstantaneousPickup;
        public static string mLongDelaySlope = string.Empty, mThermalMemory = string.Empty, mARMSMode = string.Empty, mZSI = string.Empty, mGF_ZSI = string.Empty;
        public static LocalGroundStatus mlocalGroundStatus;
        public static ArrayList listGroupsAsFound = new ArrayList();
        public static ArrayList listGroupAsLeft = new ArrayList();
        // timestamp for test report
        public static string glbStrAsFoundTimeStamp = string.Empty;
        public static string glbStrAsLeftTimeStamp = string.Empty;
        public static bool gIsTestReportGenerated;
        public static string resultAllGroups = string.Empty;   // trip setpoints info from device  
        public static string resultAllGroupsOnTestSuccess = string.Empty;   // trip setpoints info from device to store for PDF report generation 


        // variables to hold test operation data. This will be used to pre populate the test workflow screens when user continues test operation in the same flow.

        public static ArrayList lstTestUiSelectionState = new ArrayList();
        public static ArrayList lstCoilTestResult = new ArrayList();
        public static ArrayList lstTATestResult = new ArrayList();
        public static ArrayList listTripInfoObj = new ArrayList();
        public static ArrayList listTripInfoObjARMS = new ArrayList();
        public static TestReportUserInputBO objUI = new TestReportUserInputBO();

        public static InsulationAndContactTestInput objInsulationTest = new InsulationAndContactTestInput();


        public static bool GlbIsArmsTest = false;
        public static bool GlbIsInstTest = false;
        public static string TestFailureMessage = string.Empty;
        public static bool isComPortChanged = false;
        public static bool IsNotFromTestFlowforUserInput = false;
        public static bool isFeatureInProgress = false;
        public static bool GlbIsLDPickupTest = false;//Variable is set to true when Long Delay pickup test is performed
        #endregion

        #region RealTimeData variables
        //Capture Waveform variables
        public static bool mIsWaveformGroup1Valid, mIsWaveformGroup2Valid;
        #endregion

        #region EventSummaries variables
        //waveform variables
        public static ObservableCollection<Draw_Waveform_Struct> Trip_Waveform_data_global_List = new ObservableCollection<Draw_Waveform_Struct>();
        public static ObservableCollection<Draw_Waveform_Struct> Alarm_Waveform_data_global_List = new ObservableCollection<Draw_Waveform_Struct>();
        public static bool isIaValid = false;
        public static bool isIbValid = false;
        public static bool isIcValid = false;
        public static bool isInValid = false;
        public static bool isIgValid = false;
        public static bool isVabValid = false;
        public static bool isVbcValid = false;
        public static bool isVcaValid = false;
        public static bool isTAWaveformAllCurrentInvalid = false;
        public static bool isTAWaveformAllVoltageInvalid = false;
        public static bool event_summary01 = false;
        public static bool event_summary02 = false;
        public static bool event_summary03 = false;
        public static bool event_summary04 = false;
        // for events list
        public static List<EventSummary> eventList = new List<EventSummary>();
        public static List<Events> Event_eventList = new List<Events>();
        public static bool IsTripWaveformDataAvailable;
        public static bool IsAlarmWaveformDataAvailable;
        public static List<TripEventsDetails> tripevents = new List<TripEventsDetails>();
        public static List<TripEventsDetails> alarmevents = new List<TripEventsDetails>();
        public static List<Events> Event_tripevents = new List<Events>();
        public static List<Events> Event_alarmevents = new List<Events>();
        public static List<Events> Event_extendedcaptureevents = new List<Events>();
        public static List<Events> Event_disturbancecaptureevents = new List<Events>();
        public static List<Events> Event_timeadjustment = new List<Events>();
        public static List<TimeAdjustmentEvents> timeadjustment = new List<TimeAdjustmentEvents>();
        public static bool isEventSummary = false;
        public static string tripEventWaveformID = string.Empty;
        public static string alarmEventWaveformID = string.Empty;
        public static string tripEventTimestamp = string.Empty;
        public static string alarmEventTimestamp = string.Empty;
        public static Boolean IsESNotFirstTime;
        public static bool IsWaveformDataAvailiable = false;

        #endregion

        #region Setpoint variables
        public static DataTable dtChangeSummary = null;
        public static DataGrid SetpoitGrid;
        public static string SetpoitGridReportTimeStamp;
        public static ObservableCollection<ChangeSummary> Changes = new ObservableCollection<ChangeSummary>();
        public static string RPlugSelectionValue = "";
        public static string longDelaySlopeSelectionValue = "";
        public static string shortDelaySlopeSelectionValue = "";
        public static string groundFaultSlopeSelectionValue = "";
        public static string SwitchAtValidPosition = "false"; //Added by Astha for MMRotarySwitch Position
        public static bool IsOffline = true;
        public static bool IsOpenFile = false;          //Added by Astha to update UI when a file is opened.
        public static bool glbIsGroundSupportedStyle = false;
        public static bool glbIsArmsSupportedStyle = false;
        public static string relay1Fun;
        public static string relay1Set;
        public static string relay2Fun;
        public static string relay2Set;
        public static string relay3Fun;
        public static string relay3Set;
        public static bool glbIsHighLoadAlarmVisible = false;
        public static bool IsUndoLock = false;
        public static string strWorkFlow = "";
        public static string strDialogResult = "";
        public static string configScreenCloseSource = "";
        public static string fileNameForSaveOption = "";
        public static string PXR35_SelectedSetpointSet = "A";
        public static bool changesSaved = false;
        public static bool isSaveFile = false;
        public static List<String> OldData = new List<String>();
        public static ArrayList OldOriginalsetpointLines = null;
        public static Hashtable installedModels = new Hashtable();
        //User selected firmware version in case of offline
        public static string appFirmware = string.Empty;
        public static string appFirmware2 = "NA";
        public static string appFirmware3 = "NA";
        public static bool isExport_PXR10MCCB = false;

        //Firmware version in device
        public static String deviceFirmware = String.Empty;
        public static String deviceFirmware2 = String.Empty;
        public static String deviceFirmware3 = String.Empty;
        public static String ACBdeviceFirmwareForBaudRateCheck = String.Empty;

        public static List<ValidationMapper> validationErrorList = new List<ValidationMapper>();
        public static bool setting_Changed = false;
        public static bool export_Successful = false;
        public static bool export_Successful_ForSetpointReport = false;
        public static bool isnewfile = false;

        public static ArrayList listGroupsAsFoundSetPoint = new ArrayList();
        public static ArrayList listGroupAsLeftSetPoint = new ArrayList();
        #endregion

        #region BreakerInformation variables
        #endregion

        #region License variables
        public static bool boolLicenseInstalled = false;
        static bool isSameSession = false; // for license feature
        public static bool boolDeviceChangedForTestLicense = false;
        public static bool boolDeviceChangedForInReprogrammingLicense = false;
        public static bool boolDeviceChangeForEventSummaryLicense = false;
        public static int TestFTsessionCounter = 0; //used in conjunction with free trial counter so that free trials are reduced only once per session
        public static int TAWaveformFTsessionCounter = 0; //used in conjunction with free trial counter so that free trials are reduced only once per session
        public static int TroubleshootingFTsessionCounter = 0; //used in conjunction with free trial counter so that free trials are reduced only once per session
        private static int DiagFTSessionCounter = 0; // Used for free trial. when user proceeds with any of the diagnostic feature, he should be allowed to use other diag. feature without reducing available free trials
        public static int TestLicenseSessionCounter = 0;
        public static bool WindBackDetected = false;
        private static int TAWaveformSessionCounter = 0; // Used for free trial. when user proceeds with any of the diagnostic feature, he should be allowed to use other diag. feature without reducing available free trials
        private static int ModifyRatingSessionCounter = 0; // when user proceeds with any of the diagnostic feature, he should be allowed to use other diag. feature without reducing available free trials
        private static int TroubleshootingSessionCounter = 0; // Used for free trial. When user proceeds with any of the diagnostic feature, he should be allowed to use other diag. feature without reducing available free trials
        private static LicenseHelper objLicenseHelper = new Screens.License.LicenseHelper();
        public static bool isUserCancleClicked = false;
        #endregion

        #region ReportSection variables
        public static List<Report> checkedReportsItems = new List<Report>();
        public static string MPsettingOnPageLoad = null; // Used to determine if the MP should be printed on setpoint report or not.
        public static bool isAddLanguage = false;
        public static bool isBreakerData = false;
        public static bool isRealTimeData = false;
        public static bool isReport = false;
        public static bool isSetpointCSV = false;
        #endregion

        #region DownloadLanguage variables
        public static bool isReadAllLanguages = false;
        public static string languagePackVersion = string.Empty;
        #endregion

        #region Tripunit types/Dependency/Application Startup variables
        public static string MCCB_TripUnitStyle = string.Empty;
        public static string ACB_TripUnitStyle = string.Empty;
        public static string AppExecutionPhase = string.Empty;
        public static char[] style1;
        public static char[] style2;
        public static List<String> UI_ratingPlugs = new List<String>();
        public static List<String> UnitTypeItems = new List<String>();
        public static List<String> listofBreakerFrameItems = new List<String>();
        public static bool GlbBoolIsReturnFromGenericTemplat;
        public static string GlbstrBreakerType;
        public static string GlbstrUnitType;
        public static string GlbstrLineFrequency;
        public static string GlbstrTripUnitType;
        public static string GlbstrSensing;
        public static string GlbstrBreakerFrame;
        public static string GlbstrBreakerFrameShortName; // store NRX NF/NRX RF
        public static string GlbstrCurrentRating;
        public static string GlbstrGFP;
        public static string GlbstrARMS;
        public static string GlbstrModBus;
        public static string GlbstrPXRStyle;
        public static string GlbstrPole;
        public static string GlbstrShortDelay;
        public static string GlbstrCAMSelection;
        public static string GlbstrRelayFeature;
        public static string GlbstrZSI;
        public static string GlbstrAuxConnected;
        public static string GlbstrMotor;
        public static string GlbstrNeutral;
        public static string GlbstrThermalMemory;
        public static DataTable dtPXRStyle;
        public static DataTable dtRatingPlug;
        public static string strCatNumber;
        //to store the value of selected template type Added by Sunny
        public static string selectedTemplateType = string.Empty;
        public static string device_type = string.Empty;
        public static string Connected_device_type = string.Empty;
        public static bool device_type_PXR10 = false;
        public static bool device_type_PXR20 = false;
        public static bool device_type_PXR20D = false;
        public static bool device_type_PXR25 = false;
        //Added by Sunny for MCCB read
        public static List<TripUnitMapper> grouplist = new List<TripUnitMapper>();
        public static List<TripUnitMapper> ACBBackUpTripSetPoints = new List<TripUnitMapper>();
        public static List<ComPortMapper> comPortList = new List<ComPortMapper>();
        public static List<TripUnitLanguageMapper> listValidMCCBLanguages = new List<TripUnitLanguageMapper>();
        static Settings setpointForStyle;
        public static string tripUnitData = string.Empty;
        public static bool isMCCBExport = false;
        public static bool isNZMExport = false;
        public static bool isACB3_0Export = false;
        public static bool isExportControlFlow = false;
        //Use Below strings to store selection On NRX screen. Use these values to pre-Select dropdown on clcik of "Back"
        public static string unitTypeSelectionValue = "";
        public static double mainScreenWidth;
        public static double mainScreenHeight;
        public static bool resetPwd = false;
        public static bool isFinishedScreenSet;//this variable will set as true when SCR_finished screen is lauched and will get set as false once closed...
        public static List<byte> Data_buffer;
        // for merged xml parsing and storing value map
        public static Hashtable valuemap = new Hashtable();//Added By Ashish
        public static bool readTrupUnit_Style_UniType_Pole = false;
        #endregion

        #region Password Feature variables
        public static string correctPassword;
        public static string connectedComportForStoredPwd;
        public static bool isPasswordVerificationFromExport = false;
        public static bool isPasswordValidatedForExport = false;
        public static bool isPasswordCorrect = false;
        // public static bool isPasswordCorrect_PXR35 = true;
        public static bool isAdmin = false;
        public static bool isUser = false;
        public static bool isUserHasExtPrev = false;
        public static string passwordSetSource = string.Empty;
        public static bool mIsPasswordValidated = false;
        public static int PasswordFailedAttempts { get; set; }
        private static string hexDataFor351 = string.Empty;
        private static int ResponseFor351;
        public static bool Button_yes = false;

        #endregion
        //Aux power connection status
        public static bool auxPowerConnected = false;
        public static bool allowClosing = false;
        //Final Firmware version -> store PCToolFirmware / deviceFrimware in this variable for adding any logic related to firmware

        public static SerialPort globalSerialPort;
        public static List<String> errors = new List<String>();


        static Global()
        {
            globalSerialPort = new SerialPort();
            globalSerialPort.BaudRate = 9600;
            globalSerialPort.Parity = Parity.None;
            globalSerialPort.DataBits = 8;
            globalSerialPort.StopBits = StopBits.One;
            globalSerialPort.ReadTimeout = 10000;

        }

        public static double getNZMFirmwareVersionWave()
        {
            //function will retun 1 or 1.5
            double firmwareVersion = 1;

            string fwVersion = TripUnit.getFirmwareVersionSel().selectionValue;
            if (fwVersion == Resource.GEN02AItem0008)
                firmwareVersion = 1;
            else if (fwVersion == Resource.GEN02AItem0009 || fwVersion == Resource.GEN02AItem0010)
                firmwareVersion = 1.5;
            else if (fwVersion == Resource.GEN02AItem0013)//Added by PP for PXPM 5953
                firmwareVersion = 1.6;
            return firmwareVersion;
        }

        //As multiple parsing is removed values were not getting set to ID table so updating values from here 
        public static void updateTriUnitValuesAfterParsing(string parseFor = Global.str_app_ID_Table)
        {

            if (Global.device_type == Global.MCCBDEVICE)
            {
                IdsToBeUpdated = new string[2] { "SYS01", "SYS16" };
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                IdsToBeUpdated = new string[3] { "SYS01", "SYS6", "SYS11" };
            }

            else if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE)

            {
                IdsToBeUpdated = new string[5] { "SYS001", "CPC001", "SYS002", "CPC002", "SYS000" };
            }
            else if (Global.device_type == Global.ACB_PXR35_DEVICE)

            {
                IdsToBeUpdated = new string[5] { "SYS001A", "CPC001", "SYS002", "CPC002", "SYS000" };
            }
            Settings gen_setpoint;
            foreach (string Id in IdsToBeUpdated)
            {
                if (parseFor == Global.str_pxset1_ID_Table)
                    gen_setpoint = (Settings)TripUnit.Pxset1IDTable[Id];
                else if (parseFor == Global.str_pxset2_ID_Table)
                    gen_setpoint = (Settings)TripUnit.Pxset2IDTable[Id];
                else
                    gen_setpoint = (Settings)TripUnit.IDTable[Id];

                if (gen_setpoint != null) updateVariables(gen_setpoint);
            }

        }

        public static void updateDefaultRelayValuesPXR35()
        {
            try
            {
                if (TripUnit.getMaintenanceModeState().textStrValue == Resource.On || TripUnit.getMaintenanceModeRemoteControl().bValue)
                {
                    ((Settings)TripUnit.IDTable["SYS131C"]).relayOriginalValue = "0040 0000 0000 0000";//0040 0000 0000 0000 //MM
                    ((Settings)TripUnit.IDTable["SYS131C"]).relayChosenValue = "0040 0000 0000 0000";
                    ((Settings)TripUnit.IDTable["SYS131C"]).textStrValue = "0040 0000 0000 0000";
                }
                else
                {
                    ((Settings)TripUnit.IDTable["SYS131C"]).relayOriginalValue = "0000 4000 0000 0000";//0040 0000 0000 0000 //MM
                    ((Settings)TripUnit.IDTable["SYS131C"]).relayChosenValue = "0000 4000 0000 0000";
                    ((Settings)TripUnit.IDTable["SYS131C"]).textStrValue = "0000 4000 0000 0000";
                }


                //if GF style
                if (TripUnit.getGroundProtectionGeneralGrp().bValue)
                {
                    ((Settings)TripUnit.IDTable["SYS141C"]).relayOriginalValue = "0020 0000 0000 0000";//0000 0040 0000 0000 // HL2
                    ((Settings)TripUnit.IDTable["SYS141C"]).relayChosenValue = "0020 0000 0000 0000";//GF
                    ((Settings)TripUnit.IDTable["SYS141C"]).textStrValue = "0020 0000 0000 0000";
                }
                else
                {
                    ((Settings)TripUnit.IDTable["SYS141C"]).relayOriginalValue = "0000 0040 0000 0000";//0000 0040 0000 0000 // HL2
                    ((Settings)TripUnit.IDTable["SYS141C"]).relayChosenValue = "0000 0040 0000 0000";//GF
                    ((Settings)TripUnit.IDTable["SYS141C"]).textStrValue = "0000 0040 0000 0000";
                }

                ((Settings)TripUnit.IDTable["SYS151C"]).relayOriginalValue = "0000 0000 0000 0000";
                ((Settings)TripUnit.IDTable["SYS151C"]).relayChosenValue = "0000 0000 0000 0000";
                ((Settings)TripUnit.IDTable["SYS151C"]).textStrValue = "0000 0000 0000 0000";


            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void updateVariables(Settings setPoint)
        {

            if (/*(!Global.IsOffline) &&*/ (setPoint.ID == "SYS001" || setPoint.ID == "SYS001A" || setPoint.ID == "CPC001" || setPoint.ID == "SYS01"))
            {
                if (TripUnit.userRatingPlug != null)
                {
                    setPoint.selectionValue = TripUnit.userRatingPlug;
                    setPoint.defaultSelectionValue = TripUnit.userRatingPlug;
                }
            }
            if (/*(!Global.IsOffline) && */(setPoint.ID == "SYS002" || setPoint.ID == "CPC002"/* || setPoint.ID == "SYS02"|| setPoint.ID == "SYS2"*/))
            {
                if (TripUnit.userBreakerInformation != null && (!string.IsNullOrEmpty(TripUnit.userBreakerInformation)))

                {
                    setPoint.selectionValue = TripUnit.userBreakerInformation;
                    setPoint.defaultSelectionValue = TripUnit.userBreakerInformation;
                }
            }
            if (/*(!Global.IsOffline) && */setPoint.ID == "SYS000" || setPoint.ID == "SYS16" || setPoint.ID == "SYS6") /*|| setPoint.ID == "CPC001"*/
            {
                if (TripUnit.userUnitType != null)
                {
                    setPoint.selectionValue = TripUnit.userUnitType;
                    setPoint.defaultSelectionValue = TripUnit.userUnitType;
                }

            }
            if (!Global.IsOffline && setPoint.ID == "SYS11")
            {
                if (TripUnit.userLanguage != null && TripUnit.userLanguage == "000A")
                {
                    setPoint.selectionValue = TripUnit.userLanguageName.ToString();
                    setPoint.defaultSelectionValue = TripUnit.userLanguageName.ToString();
                }
            }
            //if (/*(!Global.IsOffline) && */setPoint.ID == "SYS003" || setPoint.ID == "SYS16") /*|| setPoint.ID == "CPC001"*/
            //{
            //    if (TripUnit.userStyle != null)
            //    {
            //        setPoint.selectionValue = TripUnit.userUnitType;
            //        setPoint.defaultSelectionValue = TripUnit.userUnitType;
            //    }

            //}
        }

        // Connection Start

        //<<<<<<< HEAD

        //        public static bool userInputFilled()
        //        {
        //            //bool isUserInputFilled = true;
        //            if (Global.objUI.glbJob == "-" &&
        //            Global.objUI.glbVoltageClass == "-" &&
        //            Global.objUI.glbFrequency == "-" &&
        //            Global.objUI.glbRoom == "-" &&
        //            Global.objUI.glbCell == "-" &&
        //            Global.objUI.glbTemperature == "-" &&
        //            Global.objUI.glbHumidity == "-" &&
        //            Global.objUI.glbCircuitBreaker == "-" &&
        //            Global.objUI.glbETU == "-" &&
        //            Global.objUI.glbEnclosure == "-" &&
        //            Global.objUI.glbCustomerName == "-" &&
        //            Global.objUI.glbPlantLocation == "-")
        //            {
        //                // isUserInputFilled = false;                     //#COVARITY FIX         235020
        //                return false;
        //            }
        //            // isUserInputFilled = true;
        //            //  bool isTestDataPresent = false;
        //            // value of above variables are not getting updated so updated else if condition based on same              //#COVARITY FIX       235032 
        //            if ((Global.lstCoilTestResult.Count > 0 || Global.lstTATestResult.Count > 0 || TripUnit.arrLstTestResults.Count > 0 || TripUnit.arrLstTestResultsARMS.Count > 0) && !DeviceDetailsAndSerialNumber.isDeviceChangedForTest())
        //            {
        //                //isTestDataPresent = true;
        //                return true;
        //            }
        //            //else if (/*(Global.TestFeatureLast_CompPort == Global.portName && isTestDataPresent) || Global.portName != Global.Last_CompPort*/
        //            //    (/*(!(DeviceDetailsAndSerialNumber.isDeviceChangedForTest()) && isTestDataPresent)) ||*/ !DeviceDetailsAndSerialNumber.isDeviceChangedForReports() || (!DeviceDetailsAndSerialNumber.isDeviceChangedForReports() /*&& isUserInputFilled*/)) // if test data is not present or port is changed 
        //            else if (!DeviceDetailsAndSerialNumber.isDeviceChangedForReports() || !DeviceDetailsAndSerialNumber.isDeviceChangedForReports()) // if test data is not present or port is changed 
        //             {
        //                return true;
        //            }
        //            return true;
        //        }

        //=======
        //>>>>>>> ff2cf83f3d9e57db40370c701cb9801ab571af57
        public static void GlobalSetVerifyPassword()
        {
            if (Global.device_type == Global.ACBDEVICE)
            {
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.Password, Resource.PwdNotSupportedInACB, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            else if (null != Global.portName && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE  || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                PXR.Screens.Scrs_Password.Src_Password pwd = new PXR.Screens.Scrs_Password.Src_Password();
                pwd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (isPasswordVerificationFromExport)
                {
                    correctPassword = pwd.PasswordBox.Password;
                }
                pwd.ShowDialog();
            }
        }

        public static void GlobalSetVerifyPassword_ACB3_0()
        {
            PXR.Screens.Scrs_Password.Src_ACB3_0Password pwd = new PXR.Screens.Scrs_Password.Src_ACB3_0Password();
            pwd.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            pwd.ShowDialog();
        }


        public static void CloseSerialPortConnection(SerialDataReceivedEventHandler eventHandler = null)
        {
            if (Global.isCommunicatingUsingBluetooth)
            {
                return;
            }
            if (mSerialPort != null && mSerialPort.IsOpen)
            {
                mSerialPort.DiscardInBuffer();
                mSerialPort.DiscardOutBuffer();
                try
                {
                    if (eventHandler != null)
                    {
                        mSerialPort.DataReceived -= eventHandler;
                    }
                    //Thread.Sleep(30);
                    //mSerialPort.Close();
                    //Thread.Sleep(30);
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);
                    throw;
                }
            }
        }

        private static void mTimeCounter_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            mTimer.Stop();

            Wizard_Scr_Error win = new Wizard_Scr_Error(Resource.ConnectToTripUnitTitle,
                                                               Resource.ConnectToTripUnitSubtitle,
                                                               Resource.ConnectToTripUnitHeading,
                                                               Resource.ConnectToTripUnitError, "", "");

        }


        private static void DataConnectionError()
        {
            Errors.SetWizardFinished(false);
            Window errWin = new Window();
            Wizard_Scr_Error errContent = new Wizard_Scr_Error(Resource.ConnectToTripUnitTitle,
                                                             Resource.ConnectToTripUnitSubtitle,
                                                             Resource.ConnectToTripUnitHeading,
                                                             Resource.ConnectToTripUnitError, "", "");
            errWin.Content = errContent;
            errWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            errWin.SizeToContent = SizeToContent.WidthAndHeight;
            errWin.Topmost = true;
            errWin.ResizeMode = ResizeMode.NoResize;
            errWin.ShowDialog();
            RemoteTestScr2_TestMode.mIsDataConnError = true;
        }

        public static void OpenSerialPortConnection(SerialDataReceivedEventHandler eventHandler = null)
        {
            if (Global.isCommunicatingUsingBluetooth)
            {
                return;
            }
            if (mSerialPort != null && Global.portName != string.Empty || Global.portName != null)
            {
                try
                {
                    mTimer.Elapsed += mTimeCounter_Elapsed;
                    if (!mSerialPort.IsOpen)
                    {
                        mTimer.Start();
                        mSerialPort.Open();
                        mTimer.Stop();
                    }
                    if (eventHandler != null)
                    {
                        mSerialPort.DataReceived += eventHandler;
                    }
                }
                catch (Exception ex)
                {
                    DataConnectionError();
                    LogExceptions.LogExceptionToFile(ex);
                    throw;
                }
            }
        }




        public static void ConnectRealTimeData()
        {
            if (Global.portName == null || Global.portName == string.Empty) // check because it resets the value selected from com port selection screen
            {
                Global.portName = Global.GlobalAutodetectPort();
            }
            if (Global.portName == "Nothing")
            {
                ImportWizard wizard_import = new ImportWizard(Global.str_wizard_ERROR);
                wizard_import.ShowDialog();
            }
            else
            {
                if (Global.portName == null)
                {
                    openWizardWindow(Global.str_wizard_SELECT_COMM_PORT);
                }
                else
                {
                    var breakerinfo = new Screens.Scrs_RealTimeData.Scr_RealTimeDataLoading(CultureInfo.CurrentUICulture);
                    //var breakerinfo = new Screens.Scrs_RealTimeData.Scr_RealTimeData();
                    breakerinfo.ShowDialog();
                    breakerinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    breakerinfo.Topmost = true;

                }
            }
        }
        public static void ConnectReports(List<Report> selectedItems, CultureInfo cultureinfo)
        {

            if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE)
            {
                var breakerinfo = new Src_ReportsLoading(selectedItems, cultureinfo);
                breakerinfo.Show();
                breakerinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                breakerinfo.Topmost = true;
            }
            else if (null != Global.portName && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
            {
                var breakerinfo = new Src_ReportsLoading(selectedItems, cultureinfo);
                breakerinfo.Show();
                breakerinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                breakerinfo.Topmost = true;
            }
            else if (device_type == ACB_PXR35_DEVICE)
            {
                var breakerinfo = new Src_ReportsLoading(selectedItems, cultureinfo);
                breakerinfo.Show();
                breakerinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                breakerinfo.Topmost = true;
            }
        }

        public static void ConnectBreakerInformation()
        {
            if (Global.portName == null || Global.portName == string.Empty) // check because it resets the value selected from com port selection screen
            {
                Global.portName = Global.GlobalAutodetectPort();
            }
            if (Global.portName == "Nothing")
            {
                ImportWizard wizard_import = new ImportWizard(Global.str_wizard_ERROR);
                wizard_import.ShowDialog();
            }
            else
            {
                if (Global.portName == null)
                {
                    openWizardWindow(Global.str_wizard_SELECT_COMM_PORT);
                }
                else
                {
                    var breakerinfo = new Screens.Scrs_BreakerInformation.Scr_BreakerInformationLoading();
                    breakerinfo.ShowDialog();
                    breakerinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    breakerinfo.Topmost = true;

                }
            }
        }


        public static void EventSummaryInformation()
        {


            if (null != Global.portName && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE
            || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))

            {
                var culture = CultureInfo.CurrentUICulture;
                var eventinfo = new Screens.Scrs_EventSummaries.Scrs_EventSummaryLoading(CultureInfo.CurrentUICulture);
                eventinfo.Topmost = false;
                eventinfo.ShowInTaskbar = true;
                eventinfo.ShowDialog();
                eventinfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        public static void addLanguageMCCB()
        {

            if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
            {
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.AddLanguage, Resource.LangNotSupportedInACB, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            else if (null != Global.portName && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
            {
                LogExceptions.LogExceptionToFile(new Exception(), "str_wizard_ADDLANGUAGE clicked"); //#COVARITY FIX   415431
                ImportWizard winWizard = new ImportWizard(Global.str_wizard_ADDLANGUAGE);
                winWizard.ShowDialog();
            }
        }



        public static byte[] Check_Sum_Calculation(byte[] send_message)
        {
            int checksum = 0;
            for (int i = 0; i < send_message.Length - 3; i++)
            {
                checksum += send_message[i] + send_message[i + 1] * 256;
                i++;
            }
            byte[] checksumbyte = new byte[2];
            checksum = checksum % 65536;
            checksumbyte[0] = (byte)(checksum % 256);
            checksum /= 256;
            checksumbyte[1] = (byte)(checksum % 256);

            return checksumbyte;
        }

        private static void openWizardWindow(String typeOfWizard)
        {
            ImportWizard wizard_import = new ImportWizard(typeOfWizard);

            // this makes the window modal. meaning the user
            // cannot change what is going on here until they 
            // close the import window. 
            wizard_import.ShowDialog();
        }

        public static void SetpointReportData()
        {
            Global.dtChangeSummary = null;

            Global.dtChangeSummary = new DataTable();

            // Add columns in datatable
            foreach (DataGridColumn grdCol in SetpoitGrid.Columns)
            {
                if (grdCol.Visibility == Visibility.Visible)
                {

                    Global.dtChangeSummary.Columns.Add(new DataColumn());
                }
            }

            Global.dtChangeSummary.Columns.Add(new DataColumn("SettingID"));

            Global.dtChangeSummary.Columns[0].ColumnName = "ParamName";
            // Add records in datatable
            for (int lv_RowCount = 0; lv_RowCount < Changes.Count; lv_RowCount++)
            {
                DataRow dr = Global.dtChangeSummary.NewRow();
                dr[0] = Changes[lv_RowCount].ItemName.Trim();
                dr[1] = Changes[lv_RowCount].OrigValue.Trim();
                dr[2] = Changes[lv_RowCount].NewValue.Trim();

                int positionofUnderscore = Changes[lv_RowCount].ControlName.IndexOf("_", StringComparison.Ordinal);

                dr[3] = Changes[lv_RowCount].ControlName.Substring(positionofUnderscore + 1);
                Global.dtChangeSummary.Rows.Add(dr);
            }

            //WizardPDFWriter writer = new WizardPDFWriter();
            //writer.createSummaryFile(saveSummaryPDFFilePath);
        }

        public static void resetUnitTypeVariables()
        {
            Global.device_type_PXR10 = false;
            Global.device_type_PXR20 = false;
            Global.device_type_PXR20D = false;
            Global.device_type_PXR25 = false;
        }

        /// <summary>
        /// It will set command for group 0 based on selected device type and return bytelist repsonse of group 0
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static byte[] readGroup0Information(string deviceType)
        {

            CommunicationHelperReadData.commandQueue =
                (deviceType == Global.ACBDEVICE || deviceType == Global.ACB_02_01_XX_DEVICE || device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || device_type == Global.PTM_DEVICE) ? CommunicationHelperReadData.fillCommandACBGroup0() : 
               ( (device_type == Global.ACB_PXR35_DEVICE) ? CommunicationHelperReadData.fillCommandACB_PXR35Group0() : CommunicationHelperReadData.fillCommandReadMCCBGroup0());
            int intbytListCheck = CommunicationHelperReadData.commandQueue.Count;
            List<byte[]> byteList = CommunicationHelperReadData.readTripUnitGroupsDataFromDevice();
            if (byteList != null)
            return byteList[0];
            else
                return new List<byte[]>()[0];
        }

        public static async Task<byte[]> readGroup0InformationBle(string deviceType)
        {

            deviceType = Global.ACB_PXR35_DEVICE;
            CommunicationHelperReadData.commandQueue = (deviceType == Global.ACBDEVICE || deviceType == Global.ACB_02_01_XX_DEVICE || device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) ? CommunicationHelperReadData.fillCommandACBGroup0() : (device_type == Global.ACB_PXR35_DEVICE) ?
                                                       CommunicationHelperReadData.fillCommandACB_PXR35Group0() : CommunicationHelperReadData.fillCommandReadMCCBGroup0();
            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PXR35Group0();
            int intbytListCheck = CommunicationHelperReadData.commandQueue.Count;
            byteList = await CommunicationHelperReadData.readTripUnitGroupsDataFromDeviceBle().ConfigureAwait(true);
            //UpdateUnitTypeForPXR35(byteList);
            return byteList[0];
        }

        public static void setTripUnitInfoForDemoMode()
        {
            if (!Global.isDemoMode) return;
            try
            {
                Global.appFirmware = Resource.GEN02AItem0000;
                Global.device_type = Global.MCCBDEVICE;
                Global.device_type_PXR25 = true;
                Global.selectedTemplateType = Global.MCCBTEMPLATE;
                TripUnit.deviceBreakerInformation = Resource.SYS02Item0015;
                Global.parsed_Template_File = string.Empty;
                string bytelistVal = "128	1	7	176	0	1	1	4	47	55	50	0	233	237	253";

                byte[] byte_buffer = bytelistVal.Split('	').Select(byte.Parse).ToArray();
                // Global.mSerialPort.Read(byte_buffer, 0, bytes);
                style1 = ((Convert.ToString(Convert.ToInt64(byte_buffer[9].ToString("X2") + byte_buffer[8].ToString("X2"), 16), 2)).PadLeft(16, '0')).ToArray();
                style2 = ((Convert.ToString(Convert.ToInt64(byte_buffer[11].ToString("X2") + byte_buffer[10].ToString("X2"), 16), 2)).PadLeft(16, '0')).ToArray();

                Array.Reverse(style1);
                Array.Reverse(style2);

                Global.GlbstrTripUnitType = Resource.GEN01Item0003;
                TripUnit.FrameConstruction = Resource.SYS02Item0015;
                Global.GlbstrPole = Resource.GEN10AItem0003;
                TripUnit.userPoleValue = Resource.GEN10AItem0003;
                TripUnit.breakerPCPoles = Resource.GEN10AItem0003;
                TripUnit.breakerCatalogNumber = "PDG23F0060E2CJ";
                Global.glbBreakerFrameSrNo = "8120200123120628";
                TripUnit.breakerManufactureDate = "23-Jan-20";
                Global.glbTripUnitSrNo = "8120200123120628";
                TripUnit.ETU_ManufactureDate = "23-Jan-20";
                TripUnit.userRatingPlug = "60 A";
                TripUnit.userBreakerInformation = Resource.SYS02Item0015;
                TripUnit.ratingPlug = 60;
                TripUnit.lineFrequency = "60 Hz";
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }
        public static string readTUType()
        {
            try
            {
                string tripUnitType = string.Empty;
                string port_name = Global.GlobalAutodetectPort();
                if (Global.mSerialPort.IsOpen && port_name != null && port_name != "Nothing" && !Global.isDemoMode && !Global.isCommunicatingUsingBluetooth/*port_name != Resource.DemoMode*/)
                {
                    resetUnitTypeVariables();


                    Global.OpenSerialPortConnection();
                    byte[] Send_Command;
                    Data_buffer = new List<byte>();

                    if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                    {
                        Send_Command = new byte[7];
                        Send_Command[0] = 0x80;
                        Send_Command[1] = 0x00;
                        Send_Command[2] = 0x07;
                        Send_Command[3] = 0xC5;
                        byte[] checksumbb = new byte[2];
                        checksumbb = Check_Sum_Calculation(Send_Command);
                        Send_Command[4] = checksumbb[0];
                        Send_Command[5] = checksumbb[1];
                        Send_Command[6] = 0xFD;
                        writeToUnit_TUStyle(Send_Command);
                        int TUType_code = Convert.ToInt32(Data_buffer[9].ToString("X") + Data_buffer[8].ToString("X"), 16);

                        Global.device_type_PXR10 = false;
                        Global.device_type_PXR20 = false;
                        Global.device_type_PXR20D = false;
                        Global.device_type_PXR25 = false;



                        //read trip unit style 
                        string readETUStyle = "80 00 07 B0";
                        byte[] byteArraryFWCommand = UsbCdcDataHandler.convertToByteArrayDecimalFromHexString(readETUStyle);
                        Global.mSerialPort.Write(byteArraryFWCommand, 0, byteArraryFWCommand.Length);
                        Thread.Sleep(50);
                        int bytes = Global.mSerialPort.BytesToRead;
                        byte[] byte_buffer = new byte[bytes];
                        Global.mSerialPort.Read(byte_buffer, 0, bytes);
                        style1 = ((Convert.ToString(Convert.ToInt64(byte_buffer[9].ToString("X2") + byte_buffer[8].ToString("X2"), 16), 2)).PadLeft(16, '0')).ToArray();
                        style2 = ((Convert.ToString(Convert.ToInt64(byte_buffer[11].ToString("X2") + byte_buffer[10].ToString("X2"), 16), 2)).PadLeft(16, '0')).ToArray();

                        Array.Reverse(style1);
                        Array.Reverse(style2);
                        // ===

                        switch (Global.device_type)
                        {
                            case Global.MCCBDEVICE:
                                if (TUType_code == 2 || TUType_code == 10)
                                {
                                    Global.device_type_PXR10 = true;
                                    tripUnitType = Resource.ParaSel_TripUnit_MCCB1;
                                }
                                break;
                            case Global.NZMDEVICE:
                                //LongDelay,Instanttenious,basic metering
                                if ((Global.style1[0] == '1' && Global.style1[2] == '1' && true) &&
                                     //shortdelay , GFP, ARMS ,ZSI Motor, ExtentedMetering, Modbus ,CAM, Relay
                                     (Global.style1[1] == '0' && Global.style1[3] == '0' && Global.style1[4] == '0' && Global.style2[4] == '0' && Global.style1[7] == '0' && Global.style1[12] == '0' && Global.style2[0] == '0' && Global.style2[1] == '0' && Global.style2[3] == '0'))
                                {
                                    Global.device_type_PXR10 = true;
                                    tripUnitType = Resource.ParaSel_TripUnit_MCCB1;
                                }

                                break;
                        }

                        if (!Global.device_type_PXR10)
                        {
                            //Global.device_type_PXR20 = false;
                            //LCDSel(S2-B5) VoltSel(S1-B12)
                            //PXR25   1   1
                            //PXR20D  1   0
                            //PXR20   0   0

                            if (style2[5] == '1' && style1[12] == '1')
                            {
                                Global.device_type_PXR25 = true;
                                tripUnitType = Resource.ParaSel_TripUnit_MCCB4;
                            }
                            else if (style2[5] == '1' && style1[12] == '0')
                            {
                                Global.device_type_PXR20D = true;
                                tripUnitType = Resource.ParaSel_TripUnit_MCCB3;
                            }
                            else if (style2[5] == '0' && style1[12] == '0')
                            {
                                Global.device_type_PXR20 = true;
                                tripUnitType = Resource.ParaSel_TripUnit_MCCB2;
                            }
                        }

                        byte[] byteData = readGroup0Information(Global.device_type);
                        //var byteData0 = readGroup0InformationBle(Global.ACB_PXR35_DEVICE);

                        //Set trip unit features                       
                        TripUnit.deviceZSI = (Convert.ToBoolean(Convert.ToInt32(Global.style2[4].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ZSI_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ZSI_MCCB2");
                        TripUnit.deviceLongDelay = (Convert.ToBoolean(Convert.ToInt32(Global.style1[0].ToString()))) ? Resource.Yes : Resource.No;
                        TripUnit.deviceShortDelay = (Convert.ToBoolean(Convert.ToInt32(Global.style1[1].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ShortDelay_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ShortDelay_MCCB2");
                        TripUnit.deviceInstanttenious = (Convert.ToBoolean(Convert.ToInt32(Global.style1[2].ToString()))) ? Resource.Yes : Resource.No;
                        TripUnit.deviceGFP = (Convert.ToBoolean(Convert.ToInt32(Global.style1[3].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_GroundProtection_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_GroundProtection_MCCB2");
                        TripUnit.deviceARMS = (Convert.ToBoolean(Convert.ToInt32(Global.style1[4].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ARMSProtection_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ARMSProtection_MCCB2");
                        TripUnit.deviceModBus = (Convert.ToBoolean(Convert.ToInt32(Global.style2[0].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ModbusComms_MCCB2") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_ModbusComms_MCCB1");
                        TripUnit.deviceCAMSelection = (Convert.ToBoolean(Convert.ToInt32(Global.style2[1].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_CAMSelection_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_CAMSelection_MCCB2");
                        TripUnit.deviceRelayFeature = (Convert.ToBoolean(Convert.ToInt32(Global.style2[3].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_RelayFeature_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_RelayFeature_MCCB2");
                        Global.GlbstrMotor = (Convert.ToBoolean(Convert.ToInt32(style1[7].ToString()))) ? Resource.GEN12Item0001 : Resource.GEN12Item0000;
                        TripUnit.ExtentedMetering = (Convert.ToBoolean(Convert.ToInt32(style1[12].ToString()))) ? Resource.Yes : Resource.No;
                        TripUnit.BasicMetering = Resource.Yes;

                        //Rating Information  13..12
                        String strRatingHex = string.Format("{0}{1}", byteData[13].ToString("X").PadLeft(2, '0'), byteData[12].ToString("X").PadLeft(2, '0'));
                        TripUnit.userRatingPlug = Convert.ToInt32(strRatingHex, 16).ToString() + " A";

                        //Required to set TripUnit.deviceBreakerInformation - Used in Real time data for comparing connected device's frame
                        if (Global.device_type == Global.MCCBDEVICE)
                        {
                            //Breaker Information  15..14
                            var First = byteData[15].ToString("X").PadLeft(2, '0');
                            var Second = byteData[14].ToString("X").PadLeft(2, '0');
                            string BreakerInformation = string.Format("{0}{1}", First, Second);
                            switch (BreakerInformation)
                            {
                                case "0015":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item0015;
                                    break;
                                case "0016":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item00160017;
                                    break;
                                case "0017":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item00160017;
                                    break;
                                case "0018":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item0018;
                                    break;
                                case "0019":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item0019;
                                    break;
                                case "001A":
                                    TripUnit.deviceBreakerInformation = Resource.SYS02Item001A;
                                    break;
                            }

                            //PXPM-6721  Field issue: PD3 PXR10 - PXPM showing 250 rating instead of H250 for frame 3B
                            if (TripUnit.lookupTable_plugCodes != null)
                            {
                                TripUnit.deviceRatingPlug = TripUnit.lookupTable_plugCodes[strRatingHex].ToString();
                                if (TripUnit.deviceRatingPlug.Contains('|'))
                                {
                                    string[] values = TripUnit.deviceRatingPlug.Split('|').ToArray();
                                    if (TripUnit.PD3PD3HHexValue == "0017")
                                    {
                                        foreach (var val in values)
                                        {
                                            if (val.StartsWith("H"))
                                            {
                                                TripUnit.deviceRatingPlug = val;
                                                TripUnit.userRatingPlug = val;
                                                break;
                                            }
                                        }
                                    }
                                    else /*if (TripUnit.PD3PD3HHexValue == "0016")*/
                                    {
                                        foreach (var val in values)
                                        {
                                            if (!val.StartsWith("H"))
                                            {
                                                TripUnit.deviceRatingPlug = val;
                                                TripUnit.userRatingPlug = val;
                                                break;
                                            }
                                        }
                                    }

                                }
                            }


                        }
                        if (Global.device_type == Global.NZMDEVICE)
                        {
                            if (Global.device_type_PXR20D)
                            {
                                Global.device_type_PXR20 = true;
                                // tripUnitType = Resource.ParaSel_TripUnit_MCCB2;      //#COVARITY FIX  234809
                            }

                            TripUnit.ExtentedMetering = (Convert.ToBoolean(Convert.ToInt32(style1[12].ToString()))) ? Resource.Yes : Resource.No;
                            TripUnit.BasicMetering = Resource.Yes;

                            //Breaker Information  15..14
                            string first = byteData[15].ToString("X").PadLeft(2, '0');
                            string second = byteData[14].ToString("X").PadLeft(2, '0');
                            string breakerInformation = string.Format("{0}{1}", first, second);

                            //"000B":"NZM2","000C":"NZM3","000D":"NZM4"
                            TripUnit.deviceBreakerInformation = (breakerInformation == "000B") ? Resource.SYS2Item000B : ((breakerInformation == "000C") ? Resource.SYS2Item000C : Resource.SYS2Item000D);
                            TripUnitIdentifier.setDeviceTripUnitType();
                            tripUnitType = TripUnit.deviceTripUnit;
                        }
                    }
                    else if (Global.device_type == Global.ACB_PXR35_DEVICE)

                    {

                        byte[] byteData = readGroup0Information(Global.device_type);
                        UpdateUnitTypeForPXR35(byteData);
                    }
                    else if (Global.device_type == Global.PTM_DEVICE)
                    {
                        byte[] byteData = Global.readGroup0Information(Global.device_type);

                        var First = byteData[17].ToString("X").PadLeft(2, '0');
                        var Second = byteData[16].ToString("X").PadLeft(2, '0');
                        tripUnitType = string.Format("{0}{1}", First, Second);

                        Global.device_type_PXR20 = false;
                        Global.device_type_PXR25 = false;
                        Global.device_type_PXR10 = false;

                        switch (tripUnitType)
                        {
                            case "0010":
                                tripUnitType = Resource.SYS003TItem0010;
                                Global.device_type_PXR10 = true;
                                break;
                            case "0011":
                                tripUnitType = Resource.SYS003TItem0011;
                                Global.device_type_PXR10 = true;
                                break;
                            case "0012":
                                tripUnitType = Resource.SYS003TItem0012;
                                Global.device_type_PXR20 = true;
                                break;
                            case "0013":
                                tripUnitType = Resource.SYS003TItem0013;
                                Global.device_type_PXR20 = true;
                                break;
                            case "0014":
                                tripUnitType = Resource.SYS003TItem0014;
                                Global.device_type_PXR20 = true;
                                break;
                            case "0015":
                                tripUnitType = Resource.SYS003TItem0015;
                                Global.device_type_PXR20 = true;
                                break;
                            case "0016":
                                tripUnitType = Resource.SYS003TItem0016;
                                Global.device_type_PXR25 = true;
                                break;
                            case "0017":
                                tripUnitType = Resource.SYS003TItem0017;
                                Global.device_type_PXR25 = true;
                                break;
                        }
                        setReportInformationForPTM();
                    }
                    else
                    {
                        Send_Command = new byte[7];
                        Send_Command[0] = 0x80;
                        Send_Command[1] = 0x04;
                        Send_Command[2] = 0x04;
                        Send_Command[3] = 0x1A;
                        byte[] checksumbb = new byte[2];
                        checksumbb = Check_Sum_Calculation(Send_Command);
                        Send_Command[4] = checksumbb[0];
                        Send_Command[5] = checksumbb[1];
                        Send_Command[6] = 0xFD;
                        writeToUnit_TUStyle(Send_Command);
                        if (0x00 == Data_buffer[4])
                        {
                            Send_Command[0] = 0x80;
                            Send_Command[1] = 0x00;
                            Send_Command[2] = 0x04;
                            Send_Command[3] = 0x1A;
                            checksumbb = new byte[2];
                            checksumbb = Check_Sum_Calculation(Send_Command);
                            Send_Command[4] = checksumbb[0];
                            Send_Command[5] = checksumbb[1];
                            Send_Command[6] = 0xFD;
                            do
                            {
                                writeToUnit_TUStyle(Send_Command);

                                //0x00: action is successful
                                //0x20: action is ongoing, PC need to check result again
                                //0xFF: action is failure

                                if (Data_buffer[4] == 0)
                                {
                                    //action is successful
                                    //isTestSuccess = true;
                                    break;
                                }
                                else if (Data_buffer[4] != 32)
                                {//action is failure
                                    break;
                                }
                            } while (Data_buffer[4] == 32);
                            int TUType_code = Convert.ToInt32(Data_buffer[9].ToString("X") + Data_buffer[8].ToString("X"), 16);

                            //0x0A	PXR20V000L0AM
                            //0x0B    PXR20V000L0AC
                            if (TUType_code <= 5 || TUType_code == 10 || TUType_code == 11) //00,01,02,03,04,05 are PXR20 and 06,07,08 and 09 are PXR25   
                            {
                                Global.device_type_PXR20 = true;
                                tripUnitType = Resource.ParaSel_TripUnit_MCCB2;
                            }
                            else
                            {
                                Global.device_type_PXR20 = false;
                                Global.device_type_PXR25 = true;
                                tripUnitType = Resource.ParaSel_TripUnit_MCCB4;
                            }
                        }
                        setReportInformation();
                    }
                    Global.CloseSerialPortConnection();
                    Thread.Sleep(20);
                    byte[] Receive_Meaage_Byte = Data_buffer.ToArray();
                    //checksumbb = Check_Sum_Calculation(Receive_Meaage_Byte);
                    //if ((checksumbb[0] != Receive_Meaage_Byte[buffer_Num - 3]) || (checksumbb[1] != Receive_Meaage_Byte[buffer_Num - 2]))
                    {
                        //display_Message(Resource.LanguageDownloadError);

                    }
                }


                return tripUnitType;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }

            // {11:NZM2};{12:NZM3};{13:NZM4};{21:SR2};{22:SR3-A};{23:SR3-B};{24:SR4};{25:SR5};{26:SR6}

        }

        public static async Task<string> readTUtype_ble()
        {
            var task = Task.Run(async () => await readGroup0InformationBle(Global.ACB_PXR35_DEVICE));
            task.Wait();
            byte[] bytes = task.Result;
            string tripUnitType = UpdateUnitTypeForPXR35(bytes);
            return tripUnitType;
        }

        public static string UpdateUnitTypeForPXR35(byte[] byteData)
        {
            string tripUnitType = string.Empty;
            try
            {
                // Active setpoint set - Global.PXR35_SelectedSetpointSet
                string first = byteData[63].ToString("X").PadLeft(2, '0');
                string second = byteData[62].ToString("X").PadLeft(2, '0');
                string breakerInformation = string.Format("{0}{1}", first, second);

                switch (breakerInformation)
                {
                    case "0000":
                        Global.PXR35_SelectedSetpointSet = "A";
                        break;
                    case "0001":
                        Global.PXR35_SelectedSetpointSet = "B";
                        break;
                    case "0002":
                        Global.PXR35_SelectedSetpointSet = "C";
                        break;
                    case "0003":
                        Global.PXR35_SelectedSetpointSet = "D";
                        break;

                }

                //byteData = readGroup0Information(Global.device_type);

                style2 = ((Convert.ToString(Convert.ToInt64(byteData[21].ToString("X2") + byteData[20].ToString("X2"), 16), 2)).PadLeft(16, '0')).ToArray();
                Array.Reverse(style2);

                TripUnit.deviceGFP = (Convert.ToBoolean(Convert.ToInt32(Global.style2[0].ToString()))) ? Resources.Strings.Resource.ResourceManager.GetString("ParaSel_GroundProtection_MCCB1") : Resources.Strings.Resource.ResourceManager.GetString("ParaSel_GroundProtection_MCCB2");

                Global.GlbstrGFP = TripUnit.deviceGFP;
                Settings GroundClamping = TripUnit.getGroundClampingOnOff();
                TripUnit.deviceClampingState = (Convert.ToBoolean(Convert.ToInt32(Global.style2[1].ToString()))) ? false : true;
                tripUnitType = "PXR-35";
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
            return tripUnitType;
        }

        public static void FillIDTabledata()
        {
            Global.IsOffline = false;

           if(!Global.isDemoMode) ReadGroupOnline();
            Global.selectedTemplateType = TripUnit.getTripUnitTemplateTypeFromDeviceType(Global.device_type);// == Global.MCCBDEVICE ? Global.MCCBTEMPLATE : Global.device_type == Global.NZMDEVICE ? Global.NZMTEMPLATE : Global.device_type == Global.ACBDEVICE ? Global.ACBTEMPLATE:Global.ACB3_0TEMPLATE;
            if (device_type == Global.PTM_DEVICE)
            {
                XMLParser.parseModelFile_PTM(Global.selectedTemplateType == Global.MCCBTEMPLATE ? Global.filePath_merged_mccb_xmlFile :
                (Global.selectedTemplateType == Global.NZMTEMPLATE ? Global.filePath_merged_nzm_xmlFile :
                (Global.selectedTemplateType == Global.ACBTEMPLATE ? Global.filePath_mergedstylesxmlFile : Global.filePath_merged_PTM_xmlfile)));
            }
            else
            {
            XMLParser.parseModelFile(Global.selectedTemplateType == Global.MCCBTEMPLATE ? Global.filePath_merged_mccb_xmlFile :
                 (Global.selectedTemplateType == Global.NZMTEMPLATE ? Global.filePath_merged_nzm_xmlFile :
                 (Global.selectedTemplateType == Global.ACBTEMPLATE ? Global.filePath_mergedstylesxmlFile : Global.filePath_mergedstylesxmlFile_3_0)));
            }
            Global.updateTriUnitValuesAfterParsing();
            if (!Global.isDemoMode) Global.readInData_ONLINE("Export");
            foreach (String settingID in TripUnit.IDTable.Keys)
            {
                var set = ((Settings)TripUnit.IDTable[settingID]);
                set.notifyDependents();
            }
        }

        public static bool ReadGroupOnline()
        {
            #region Read all Groups in Connect Mode
            isAllDataReceived = false;
            byteList = new List<byte[]>();

            try
            {
                byteList = new List<byte[]>();
                CommunicationHelperReadData.commandQueue = new Queue<byte[]>();

                if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE)
                {
                    if (Global.device_type == Global.PTM_DEVICE)
                    {
                        CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PTM();
                    }
                    else
                    {
                    CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB();
                }
                }
                else if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    switch (Global.PXR35_SelectedSetpointSet)
                    {
                        case "A":
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PXR35_A();
                            break;
                        case "B":
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PXR35_B();
                            break;
                        case "C":
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PXR35_C();
                            break;
                        case "D":
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PXR35_D();
                            break;
                    }

                }
                else if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                {
                    if (Global.device_type_PXR10)
                    {
                        CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandPXR10MCCBWithTripUnit();
                    }
                    else
                    {
                        CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandMCCB();
                    }
                }

                groupCountACB_MCCB = CommunicationHelperReadData.commandQueue.Count;
                stopwatchConnErr.Start(); // Start the timer to read all 4 groups data. It should be done in 8 seconds
                if (Global.isCommunicatingUsingBluetooth)
                {
                    var task = Task.Run(async () => await CommunicationHelperReadData.readTripUnitGroupsDataFromDeviceBle());
                    task.Wait();
                    byteList = task.Result;
                }
                else
                {
                    byteList = CommunicationHelperReadData.readTripUnitGroupsDataFromDevice();
                }
                writeToUnit();

                stopwatchConnErr.Stop();
                stopwatchConnErr.Reset();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isAllDataReceived = false;
                Errors.SetWizardFinished(false);
                //Dispatcher.Invoke(new UIDelegate(ErrorScreen));
            }
            finally
            {
                stopwatchConnErr.Stop();
                stopwatchConnErr.Reset();
            }
            return isAllDataReceived;
            #endregion
        }

        /// <summary>
        /// Write to trip unit
        /// </summary>
        private static void writeToUnit()
        {
            try
            {
                isAllDataReceived = false;

                //System.Threading.Thread.Sleep(500);

                if (CommunicationHelperReadData.commandQueue.Count == 0)
                {
                    // If data of all 4 groups are received then show set points page
                    if (byteList != null && byteList.Count >= groupCountACB_MCCB || (byteList != null && byteList.Count >= groupCountACB_MCCB - 1 && Global.isCommunicatingUsingBluetooth))
                    {
                        isAllDataReceived = true;
                        TripUnit.tripUnitString = UsbCdcDataHandler.GetHexData(byteList);
                        TripUnit.ArmsModeData = UsbCdcDataHandler.GetArmsMode(byteList[0]);
                        // Added by Astha to check if MM swich is at valid position
                        if (string.Equals(TripUnit.ArmsModeData, Global.SwitchAtValidPosition) && (Global.GlbstrARMS == Resource.GEN003Item0001))
                        {
                            //Dispatcher.Invoke(new UIDelegate(InvalidPositionOfMMRotarySwitch));
                            return;
                        }
                        Global.CloseSerialPortConnection();
                    }
                    else
                    {
                        Errors.SetWizardFinished(false);
                        //Dispatcher.Invoke(new UIDelegate(ErrorScreen));
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);

                isAllDataReceived = false;
                // CloseConnection();
                Errors.SetWizardFinished(false);
                //Dispatcher.Invoke(new UIDelegate(ErrorScreen));
            }
        }
        public static void setReportInformationForPTM()
        {
            if (Global.deviceFirmware != null)
            {
                byte[] byteData = readGroup0Information(Global.device_type);

                // TripUnitIdentifier.setPXRIdentifiers(byteList[0], false);
                int deviceStyle1, deviceStyle2 = 0, RatingPlug1, RatingPlug2, BreakerInformation1, BreakerInformation2;


                deviceStyle1 = 17; deviceStyle2 = 16;  //Style 17, 16
                RatingPlug1 = 13; RatingPlug2 = 12; // Rating Plug 13, 12
                BreakerInformation1 = 15; BreakerInformation2 = 14; //Breaker Information  15, 14


                //Device Style
                //string first = string.Empty;          //#COVARITY FIX   234940
                //string second = string.Empty;
                string first = byteData[deviceStyle1].ToString("X").PadLeft(2, '0');
                string second = byteData[deviceStyle2].ToString("X").PadLeft(2, '0');
                string deviceStyleName = string.Format("{0}{1}", first, second);
                string resourceKey = Global.device_type == Global.PTM_DEVICE ? "SYS003AItem" + deviceStyleName : "SYS003Item" + deviceStyleName;
                TripUnit.userStyle = Resource.ResourceManager.GetString(resourceKey);

                ////Rating Information 
                first = byteData[RatingPlug1].ToString("X").PadLeft(2, '0');
                second = byteData[RatingPlug2].ToString("X").PadLeft(2, '0');
                string ratingPlug = string.Format("{0}{1}", first, second);
                TripUnit.userRatingPlug = Convert.ToInt32(ratingPlug, 16).ToString() + " A";

                //Breaker Information
                first = byteData[BreakerInformation1].ToString("X").PadLeft(2, '0');
                second = byteData[BreakerInformation2].ToString("X").PadLeft(2, '0');
                string breakerInformation = string.Format("{0}{1}", first, second);

                switch (breakerInformation)
                {
                    case "0000":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0000;
                        break;
                    case "0001":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0001;
                        break;
                    case "0002":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0002;
                        break;
                    case "0003":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0003;
                        break;
                    case "0004":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0004;
                        break;
                }
            }

        }

        public static void setReportInformation()
        {
            if (Global.deviceFirmware != null)
            {
                byte[] byteData = readGroup0Information(Global.device_type);

                // TripUnitIdentifier.setPXRIdentifiers(byteList[0], false);
                int deviceStyle1, deviceStyle2 = 0, RatingPlug1, RatingPlug2, BreakerInformation1, BreakerInformation2;

                if (Global.deviceFirmware == Resource.GEN002Item0002 || Global.deviceFirmware == Resource.GEN002Item0003 || Global.appFirmware == Resource.GEN002Item0005
                    || Global.deviceFirmware == Resource.GEN002Item0004 || Global.deviceFirmware == Resource.GEN002Item0006 || Global.deviceFirmware == Resource.GEN002Item0008)
                {
                    deviceStyle1 = 17; deviceStyle2 = 16;  //Style 17, 16
                    RatingPlug1 = 13; RatingPlug2 = 12; // Rating Plug 13, 12
                    BreakerInformation1 = 15; BreakerInformation2 = 14; //Breaker Information  15, 14
                }
                else
                {
                    deviceStyle1 = 15; deviceStyle2 = 14;  //Style
                    RatingPlug1 = 11; RatingPlug2 = 10;// Rating Plug
                    BreakerInformation1 = 13; BreakerInformation2 = 12;//Breaker Information
                }

                //Device Style
                //string first = string.Empty;          //#COVARITY FIX   234940
                //string second = string.Empty;
                string first = byteData[deviceStyle1].ToString("X").PadLeft(2, '0');
                string second = byteData[deviceStyle2].ToString("X").PadLeft(2, '0');
                string deviceStyleName = string.Format("{0}{1}", first, second);
                string resourceKey = (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) ? "SYS003AItem" + deviceStyleName : "SYS003Item" + deviceStyleName;
                TripUnit.userStyle = Resource.ResourceManager.GetString(resourceKey);

                ////Rating Information 
                first = byteData[RatingPlug1].ToString("X").PadLeft(2, '0');
                second = byteData[RatingPlug2].ToString("X").PadLeft(2, '0');
                string ratingPlug = string.Format("{0}{1}", first, second);
                TripUnit.userRatingPlug = Convert.ToInt32(ratingPlug, 16).ToString() + " A";

                //Breaker Information
                first = byteData[BreakerInformation1].ToString("X").PadLeft(2, '0');
                second = byteData[BreakerInformation2].ToString("X").PadLeft(2, '0');
                string breakerInformation = string.Format("{0}{1}", first, second);

                switch (breakerInformation)
                {
                    case "0000":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0000;
                        break;
                    case "0001":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0001;
                        break;
                    case "0002":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0002;
                        break;
                    case "0003":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0003;
                        break;
                    case "0004":
                        TripUnit.deviceBreakerInformation = Resource.SYS002Item0004;
                        break;
                }
            }

        }

        public static void writeToUnit_TUStyle(byte[] executeCommand)
        {
            try
            {
                Global.OpenSerialPortConnection();
                byte[] byteData = executeCommand;
                Global.mSerialPort.Write(byteData, 0, byteData.Length);
                Global.alarmevents.Clear();
                Global.tripevents.Clear();
                Global.eventList.Clear();
                Global.timeadjustment.Clear();
                System.Threading.Thread.Sleep(200);
                Data_buffer.Clear();
                //Data_buffer = new List<byte>();
                for (int i = 0; i < 4; i++)
                {
                    Thread.Sleep(50);
                    //Receive data and check it
                    int Receive_Buffer_Num = Global.mSerialPort.BytesToRead;
                    byte[] Event_message_temp = new byte[Receive_Buffer_Num];
                    Global.mSerialPort.Read(Event_message_temp, 0, Receive_Buffer_Num);
                    Data_buffer.AddRange(Event_message_temp);
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Errors.SetWizardFinished(false);
                // Dispatcher.Invoke(new UIDelegate(ErrorScreen));
            }

        }

        public static string GlobalAutodetectPort()
        {
            //if (Global.portName != null)
            //    return Global.portName;

            if (comPortList.Count <= 0 && !Global.isDemoMode && !isCommunicatingUsingBluetooth)
            {
                return "Nothing";
            }
            if (Global.isCommunicatingUsingBluetooth)
            {
                return "ble";
            }
            else return Global.portName;

        }

        public static string updateValueonCultureBasis(string value)
        {
            try
            {
                if (CultureInfo.CurrentUICulture.Name == "en-US" || CultureInfo.CurrentUICulture.Name == "zh-CHS")
                {
                    value = value.ToString().Replace(",", ".");
                }
                else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES"
                         || CultureInfo.CurrentUICulture.Name == "pl-PL" || CultureInfo.CurrentUICulture.Name == "fr-CA")
                {
                    value = value.ToString().Replace(".", ",");
                }
                return value;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }

        }
        public static string ACBStyle()
        {
            string[] styleArray = new string[13];
            Settings settingsUnitType = TripUnit.getUnittypeANSI_IECGeneralGrp();
            Settings settingsTripUnit = TripUnit.getTripUnitType();
            Settings settingsGroundProtection = TripUnit.getGroundProtectionGeneralGrp();
            Settings settingsEmbeddedModbus = TripUnit.getModbusCommsGeneralGrp();
            Settings settingsMaintenanceModeStatus = TripUnit.getMaintenanceModeProtection();

            //TripUnit type 20/25
            styleArray[0] = settingsTripUnit.selectionValue.Replace(" ", "");
            styleArray[5] = "V000L";
            styleArray[10] = "0";
            styleArray[11] = "0";
            styleArray[12] = "C";
            if (settingsGroundProtection.selectionValue == Resource.GEN004Item0001)
            {
                styleArray[10] = "G";
            }
            if (settingsMaintenanceModeStatus.selectionValue == Resource.GEN003Item0001)
            {
                styleArray[10] = "A";
            }
            if (settingsEmbeddedModbus.selectionValue == Resource.GEN005Item0001)
            {
                styleArray[10] = "M";
            }

            return String.Concat(styleArray);
        }



        public static string MCCBStyle()
        {
            string[] styleArray = new string[9];
            Settings settingsBreakerFrame = TripUnit.getBreakerFrame();
            Settings settingsTripUnit = TripUnit.getTripUnitType();
            Settings settingsUnitType = TripUnit.getUnittypeANSI_IECGeneralGrp();
            Settings settingsPole = TripUnit.getPolesGeneralGrp();
            Settings settingsRating = TripUnit.getRating();


            //breaker frame type              
            if (settingsBreakerFrame.selectionValue == Resource.SYS02Item0015 ||
                settingsBreakerFrame.selectionValue == Resource.SYS2Item000B)
            {
                styleArray[0] = "PD";
                styleArray[2] = "2";
            }
            else if (settingsBreakerFrame.selectionValue == Resource.SYS02Item00160017 ||
            settingsBreakerFrame.selectionValue == Resource.SYS02Item00160017 ||
                settingsBreakerFrame.selectionValue == Resource.SYS2Item000C)
            {
                styleArray[0] = "PD";
                styleArray[2] = "3";
            }
            else if (settingsBreakerFrame.selectionValue == Resource.SYS02Item0018 ||
                settingsBreakerFrame.selectionValue == Resource.SYS2Item000D)
            {
                styleArray[0] = "PD";
                styleArray[2] = "4";
            }
            else if (settingsBreakerFrame.selectionValue == Resource.SYS02Item0019 ||
                settingsBreakerFrame.selectionValue == Resource.SYS2Item000E)
            {
                styleArray[0] = "PD";
                styleArray[2] = "5";
            }
            else if (settingsBreakerFrame.selectionValue == Resource.SYS02Item001A ||
                settingsBreakerFrame.selectionValue == Resource.SYS2Item000F)
            {
                styleArray[0] = "PD";
                styleArray[2] = "6";
            }

            ////unit type           
            if (settingsUnitType.selectionValue == Resource.SYS16Item0002)
            {
                styleArray[1] = "G";
            }
            else if (settingsUnitType.selectionValue == Resource.SYS16Item0001)
            {
                styleArray[1] = "C";
            }
            ////hard-coded compulsory string
            styleArray[3] = "XPXR";

            ////poles selected            
            styleArray[4] = settingsPole.selectionValue;

            ////rating plug - Round up to 3 digits
            //var ratingPlug = sourceElementScopeValArray[11].value.slice(0, sourceElementScopeValArray[11].value.length - 1);
            string rateing = settingsRating.selectionValue;

            rateing = Regex.Replace(rateing, "[A-Za-z]", "").Trim();

            if (rateing.Length < 3)
            {
                rateing = rateing.PadLeft(3, '0');
            }

            if (settingsBreakerFrame.selectionValue == Resource.SYS02Item00160017)//PD3H
            {
                int intRateing = Int32.Parse(rateing);
                rateing = (intRateing /*+ 1*/).ToString();
                styleArray[5] = rateing;
            }
            else
            {
                styleArray[5] = rateing;
            }

            ////trip unit type  GEN01
            if (Global.device_type == Global.MCCBDEVICE)
            {
                if (settingsTripUnit.selectionValue == Resource.GEN01Item0000)
                {
                    styleArray[6] = "B";
                }
                else if (settingsTripUnit.selectionValue == Resource.GEN01Item0001)
                {
                    styleArray[6] = "E";
                }
                else if (settingsTripUnit.selectionValue == Resource.GEN01Item0002)
                {
                    styleArray[6] = "D";
                }
                else if (settingsTripUnit.selectionValue == Resource.GEN01Item0003)
                {
                    styleArray[6] = "P";
                }
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                if (settingsTripUnit.selectionValue == Resource.GEN1Item0000)
                {
                    styleArray[6] = "B";
                }
                else if (settingsTripUnit.selectionValue == Resource.GEN1Item0001 ||
                    settingsTripUnit.selectionValue == Resource.GEN1Item0002 ||
                    settingsTripUnit.selectionValue == Resource.GEN1Item0003)
                {
                    styleArray[6] = "E";
                }
                //else if (settingsTripUnit.selectionValue == Resource.GEN1Item0002)
                //{
                //    styleArray[6] = "D";
                //}
                else if (settingsTripUnit.selectionValue == Resource.GEN1Item0004 ||
                    settingsTripUnit.selectionValue == Resource.GEN1Item0005 ||
                    settingsTripUnit.selectionValue == Resource.GEN1Item0006 ||
                    settingsTripUnit.selectionValue == Resource.GEN1Item0007)
                {
                    styleArray[6] = "P";
                }
            }


            ////1-LI, 2-LSI, 3-LSIG, 4- LSI ARMS, 5- LSIG ARMS

            Settings settingsShortDelay = TripUnit.getShortDelay();
            Settings settingsGround = TripUnit.getGroundProtectionGeneralGrp();
            Settings settingsARMS = TripUnit.getMM_ModeProtectionGeneralGrp();

            if (settingsTripUnit.selectionValue == Resource.GEN01Item0000) //PXR10
            {
                //Check only for short delay setting
                if (settingsShortDelay.bValue)
                {
                    styleArray[7] = (2).ToString();
                }
                else
                {
                    styleArray[7] = (1).ToString();
                }
            }
            else
            {
                //check for combination of Ground and ARMS
                if (!settingsGround.bValue && !settingsARMS.bValue)
                {
                    styleArray[7] = (2).ToString();
                }
                else if (settingsGround.bValue && !settingsARMS.bValue)
                {
                    styleArray[7] = (3).ToString();
                }
                else if (!settingsGround.bValue && settingsARMS.bValue)
                {
                    styleArray[7] = (4).ToString();
                }
                else if (settingsGround.bValue && settingsARMS.bValue)
                {
                    styleArray[7] = (5).ToString();
                }
            }

            ////Feature settings
            if (settingsTripUnit.selectionValue == Resource.GEN01Item0000) //PXR10
            {
                styleArray[8] = "N";
            }
            else
            {
                string featureBit = string.Empty;
                Settings settingsZSI = TripUnit.getZSIGeneralGrp();
                Settings settingsRelay = TripUnit.getRelayFeatureGeneralGrp();
                Settings settingsCAM = TripUnit.getCAMSelectionGeneralGrp();
                Settings settingsModbus = TripUnit.getModbusCommsGeneralGrp();

                featureBit += (settingsZSI.bValue) ? 1 : 0; // ZSI setting  GEN09
                featureBit += (settingsRelay.bValue) ? 1 : 0; // Relay setting   GEN08
                featureBit += "0"; // IO setting
                featureBit += (settingsCAM.bValue) ? 1 : 0; // CAM Setting   GEN07
                featureBit += (settingsModbus.bValue) ? 1 : 0; // Modbus setting   GEN06
                string mask = Convert.ToInt32(featureBit, 2).ToString();

                if (mask == "0")
                {
                    styleArray[8] = "N";
                }
                else if (mask == "8")
                {
                    styleArray[8] = "R";
                }
                else if (mask == "9")
                {
                    styleArray[8] = "M";
                }
                else if (mask == "10")
                {
                    styleArray[8] = "C";
                }
                else if (mask == "11")
                {
                    styleArray[8] = "D";
                }
                else if (mask == "24")
                {
                    styleArray[8] = "Z";
                }
                else if (mask == "25")
                {
                    styleArray[8] = "W";
                }
                else if (mask == "26")
                {
                    styleArray[8] = "X";
                }
                else if (mask == "27")
                {
                    styleArray[8] = "Y";
                }
            }
            return String.Concat(styleArray);
        }

        //Following method is Added by Astha to hide groups if no setpoint is visible for the group
        public static void updateExpandersVisibility()
        {
            int countForVisibleSetpointsInGroup = 0;
            int countForVisibleSetpointsInSubGroup = 0;
            int countForVisibleSetpointsInSubGroupWithinSubgroup = 0;
            int countForExpandersVisibleInSubgroupWithinSubgroup = 0;
            int countForExpandersVisibleInSubgroup = 0;

            foreach (Group group in TripUnit.groups)
            {
                countForVisibleSetpointsInGroup = 0;
                countForExpandersVisibleInSubgroup = 0;
                if (group.subgroups != null && group.subgroups.Length != 0)
                {
                    countForExpandersVisibleInSubgroup = 0;
                    foreach (Group subgrp in group.subgroups)
                    {
                        countForVisibleSetpointsInSubGroup = 0;
                        countForExpandersVisibleInSubgroupWithinSubgroup = 0;
                        if (subgrp.subgroups != null && subgrp.subgroups.Length != 0)
                        {
                            foreach (Group subgrpWithinSubgrp in subgrp.subgroups)
                            {
                                countForVisibleSetpointsInSubGroupWithinSubgroup = 0;
                                countForVisibleSetpointsInSubGroupWithinSubgroup = countForVisibleSetpoints(subgrpWithinSubgrp);
                                if (subgrpWithinSubgrp.expander != null)
                                {
                                    if (countForVisibleSetpointsInSubGroupWithinSubgroup == 0)
                                    {
                                        subgrpWithinSubgrp.expander.Visibility = Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        subgrpWithinSubgrp.expander.Visibility = Visibility.Visible;
                                        countForExpandersVisibleInSubgroupWithinSubgroup++;
                                    }
                                }
                            }
                        }
                        if (subgrp.groupSetPoints != null && subgrp.groupSetPoints.Length != 0)
                        {
                            countForVisibleSetpointsInSubGroup = countForVisibleSetpoints(subgrp);
                        }
                        if (subgrp.expander != null)
                        {
                            if (countForVisibleSetpointsInSubGroup == 0 && countForExpandersVisibleInSubgroupWithinSubgroup == 0)
                            {
                                subgrp.expander.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                subgrp.expander.Visibility = Visibility.Visible;
                                countForExpandersVisibleInSubgroup++;
                            }
                        }
                    }
                }
                if (group.groupSetPoints != null && group.groupSetPoints.Length != 0)
                {
                    countForVisibleSetpointsInGroup = countForVisibleSetpoints(group);
                }
                if (group.ExpanderItem != null)
                {
                    if (countForVisibleSetpointsInGroup == 0 && countForExpandersVisibleInSubgroup == 0)
                    {
                        group.ExpanderItem.Visibility = Visibility.Collapsed;
                        group.ExpanderItem.IsExpanded = false;
                    }
                    else
                    {
                        group.ExpanderItem.Visibility = Visibility.Visible;
                    }
                }
            }

            if ((Global.device_type == Global.MCCBDEVICE || Global.selectedTemplateType == Global.MCCBTEMPLATE) ||
                (Global.device_type == Global.NZMDEVICE || Global.selectedTemplateType == Global.NZMTEMPLATE))
            {
                Settings tripUnitStyle = TripUnit.getTripUnitStyle();  // Catalog number ID for MCCB
                tripUnitStyle.textStrValue = MCCBStyle();
                tripUnitStyle.defaulttextStrValue = tripUnitStyle.textStrValue;
                if (tripUnitStyle.label_catalogNumber != null)
                {
                    tripUnitStyle.label_catalogNumber.Content = tripUnitStyle.textStrValue;
                }
                Global.MCCB_TripUnitStyle = tripUnitStyle.textStrValue;

                if (!(TripUnit.getTripUnitType().selectionValue == Resource.GEN01Item0000))
                {
                    tripUnitStyle = TripUnit.getFirmwareVersionSel(); //Firmware version ID for MCCB
                    if (tripUnitStyle.lookupTable.Contains(Global.appFirmware))
                    {
                        tripUnitStyle.selectionValue = Global.appFirmware;
                        tripUnitStyle.defaultSelectionValue = tripUnitStyle.selectionValue;
                    }
                }
                else if ((TripUnit.getTripUnitType().selectionValue == Resource.GEN01Item0000))
                {
                    tripUnitStyle = TripUnit.getFirmwareVersionNum();//Firmware version ID for MCCB
                    tripUnitStyle.textStrValue = Global.appFirmware;
                    tripUnitStyle.defaulttextStrValue = tripUnitStyle.textStrValue;
                    if (tripUnitStyle.label_catalogNumber != null)
                    {
                        tripUnitStyle.label_catalogNumber.Content = tripUnitStyle.textStrValue;
                    }
                }

            }
        }

        //Added by Ashish to Update the General setpoints in online mode for ACB and MCCB
        public static void GeneralSetpointsDisable(bool visible)
        {
            try
            {
                if (visible)
                {
                    foreach (Group group in TripUnit.groups)
                    {
                        if (((group.ID == "0" || group.ID == "1") && (device_type == ACBDEVICE || device_type == ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE)) || ((group.ID == "00" || group.ID == "02") && (device_type == MCCBDEVICE || device_type == NZMDEVICE)))
                        {
                            if (group.subgroups != null /*&& device_type == MCCBDEVICE*/)
                            {
                                foreach (Group grp in group.subgroups)
                                {
                                    GeneralSettingsReadOnly(grp);
                                }
                            }
                            if (group.groupSetPoints != null)
                            {
                                GeneralSettingsReadOnly(group);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        private static void GeneralSettingsReadOnly(Group group)
        {
            foreach (Settings setpoint in group.groupSetPoints)
            {
                if (setpoint.onlineReadOnly)
                {
                    if (setpoint.type == Settings.Type.type_number)
                    {
                        setpoint.textBox.IsEnabled = !setpoint.onlineReadOnly;
                        setpoint.increaseButton.IsEnabled = !setpoint.onlineReadOnly;
                        setpoint.decreaseButton.IsEnabled = !setpoint.onlineReadOnly;
                    }
                    else if (setpoint.type == Settings.Type.type_bNumber)
                    {
                    }
                    else if (setpoint.type == Settings.Type.type_toggle)
                    {
                        if (null != setpoint.toggle)
                            setpoint.toggle.IsEnabled = !setpoint.onlineReadOnly;
                    }
                    else if (setpoint.type == Settings.Type.type_selection)
                    {
                        if (null != setpoint.comboBox)
                            setpoint.comboBox.IsEnabled = !setpoint.onlineReadOnly;
                    }
                    else if (setpoint.type == Settings.Type.type_split)
                    {

                    }
                    else if (setpoint.type == Settings.Type.type_listBox)
                    {
                    }
                    else if (setpoint.type == Settings.Type.type_text)
                    {
                        if (null != setpoint.textBox)
                        {
                            setpoint.textBox.IsEnabled = !setpoint.onlineReadOnly;
                        }
                    }
                    else if (setpoint.type == Settings.Type.type_bSelection)
                    {
                    }
                }
            }
        }
        //Added by Astha to count visible setpoints in a group
        private static int countForVisibleSetpoints(Group group)
        {
            int countVisibleSetpoints = 0;
            foreach (Settings setpoint in group.groupSetPoints)
            {
                if (setpoint.visible == true)
                {
                    countVisibleSetpoints++;
                }
            }
            return countVisibleSetpoints;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        public static T FindParent_Expander<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent_Expander<T>(parentObject);
            }
        }

        public static void updateToggleForNZMOffline(Expander pv_ExpItem, int pv_selectionIndex)
        {
            //pv_selectionIndex = 0 for PXR 10 else for other 

            foreach (Control ctrl in Global.FindVisualChildren<Control>(pv_ExpItem))
            {
                if (ctrl.Name.Trim() == "expanderItem_02")
                {
                    ((Expander)ctrl).IsExpanded = true;
                    ((Expander)ctrl).UpdateLayout();


                    foreach (ToggleButton tglBtn in Global.FindVisualChildren<ToggleButton>(ctrl))
                    {
                        if (tglBtn.Name == "tgl_GEN06" || tglBtn.Name == "tgl_GEN07" || tglBtn.Name == "tgl_GEN08")
                        {
                            tglBtn.IsChecked = pv_selectionIndex == 0 ? false : true;       //    //pv_selectionIndex = 0 for PXR 10 else for other                     
                        }
                    }
                    foreach (Label lblTripUnitFeature in Global.FindVisualChildren<Label>(ctrl))
                    {
                        if (lblTripUnitFeature.Name == "lbl_GEN13")
                        {
                            if (pv_selectionIndex == 1 || pv_selectionIndex == 5)
                            {
                                lblTripUnitFeature.Content = Resource.MPEnabledSubGroupName;
                            }
                            else
                            {
                                lblTripUnitFeature.Content = Resource.MPDisabledSubGroupName;
                            }
                        }
                    }
                      ((Expander)ctrl).IsExpanded = false;
                    ((Expander)ctrl).UpdateLayout();
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    IEnumerable<T> VisualCHild = FindVisualChildren<T>(child);
                    if (VisualCHild != null)
                    {
                        foreach (T childOfChild in VisualCHild)
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        //private static string identifyStyle()
        //{
        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB0, "PD2");
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB1, "PD3");
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB2, "PD3");
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB3, "PD4");
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB4, "PD5");
        //    dict.Add(Resource.ParaSel_BreakerFrame_MCCB5, "PD6");
        //    dict.Add(Resource.ParaSel_UnitType_MCCB0, "G");
        //    dict.Add(Resource.ParaSel_UnitType_MCCB1, "C");
        //    dict.Add(Resource.ParaSel_TripUnit_MCCB1, "B"); //PXR10
        //    dict.Add(Resource.ParaSel_TripUnit_MCCB2, "E"); //PXR20
        //    dict.Add(Resource.ParaSel_TripUnit_MCCB3, "D"); //PXR20D
        //    dict.Add(Resource.ParaSel_TripUnit_MCCB4, "P"); //PXR25

        //    string breakerFrame = dict[Global.GlbstrBreakerFrame];
        //    string unitType = dict[Global.GlbstrUnitType];
        //    string pole = Global.GlbstrPole.ToString();
        //    string rateing = Regex.Replace(Global.GlbstrCurrentRating.ToString(), "[A-Za-z]", "").Trim();

        //    if (rateing.Length < 3)
        //    {
        //        rateing = rateing.ToString().PadLeft(3, '0');
        //    }

        //    string tripUnit = dict[Global.GlbstrTripUnitType];
        //    string port = identifyPort();
        //    string feature = identifyFeature();
        //    if (port == string.Empty || feature == string.Empty)
        //    {
        //        return string.Empty;
        //    }

        //    string tripUnitStyle = breakerFrame + unitType + "X" + "PXR" + pole + rateing + tripUnit + port + feature;
        //    return tripUnitStyle;
        //}

        //private static string identifyPort()
        //{
        //    string port = string.Empty;
        //    //Disabled, Disabled, Disabled 
        //    if (Global.GlbstrShortDelay == Resource.ParaSel_ShortDelay_MCCB2 && Global.GlbstrGFP == Resource.ParaSel_GroundProtection_MCCB2 && Global.GlbstrARMS == Resource.ParaSel_ARMSProtection_MCCB2)
        //    {
        //        port = "1";
        //    }//Enabled, Disabled, Disabled
        //    else if (Global.GlbstrShortDelay == Resource.ParaSel_ShortDelay_MCCB1 && Global.GlbstrGFP == Resource.ParaSel_GroundProtection_MCCB2 && Global.GlbstrARMS == Resource.ParaSel_ARMSProtection_MCCB2)
        //    {
        //        port = "2";
        //    }//Enabled, Enabled, Disabled
        //    else if (Global.GlbstrShortDelay == Resource.ParaSel_ShortDelay_MCCB1 && Global.GlbstrGFP == Resource.ParaSel_GroundProtection_MCCB1 && Global.GlbstrARMS == Resource.ParaSel_ARMSProtection_MCCB2)
        //    {
        //        port = "3";
        //    }//Enabled, Disabled, Disabled
        //    else if (Global.GlbstrShortDelay == Resource.ParaSel_ShortDelay_MCCB1 && Global.GlbstrGFP == Resource.ParaSel_GroundProtection_MCCB2 && Global.GlbstrARMS == Resource.ParaSel_ARMSProtection_MCCB2)
        //    {
        //        port = "4";
        //    }//Enabled, Enabled, Enabled
        //    else if (Global.GlbstrShortDelay == Resource.ParaSel_ShortDelay_MCCB1 && Global.GlbstrGFP == Resource.ParaSel_GroundProtection_MCCB1 && Global.GlbstrARMS == Resource.ParaSel_ARMSProtection_MCCB1)
        //    {
        //        port = "5";
        //    }
        //    else
        //    {
        //        Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox("Invalid selection", "Invalid selection for Short delay, GFP, ARMS", string.Empty, false);
        //        MsgBoxWindow.Height = 200;
        //        MsgBoxWindow.Topmost = true;
        //        MsgBoxWindow.ShowDialog();
        //        return string.Empty;
        //    }
        //    return port;
        //}

        /// <summary>
        /// Author: Sarah M. Norris
        /// Description: 
        /// Reads in the data from the trip unit and parses it to meet the criterial of the model file
        /// </summary>
        public static void readInData_ONLINE(string saveOrExport) //Archana rewrite
        {
            try
            {
                /******************************************************************************
                 * TODO: This needs to be removed from this point further down the road because
                 * there may be look up tables which depend on the style so the style and 
                 * rating plug maybe the only tables we create here. 
                 */
                if (!(Global.portName == null || Global.portName == string.Empty) && !Global.isDemoMode && !Global.isCommunicatingUsingBluetooth)//  Global.portName != Resource.DemoMode)
                {
                    AuxPowerStatus objAuxPower = new AuxPowerStatus();
                    bool isAuxStatusReadSucces = objAuxPower.writeAuxPowerStatusToGlobal();
                }
                //TripParser.parseOutputFile(filePath_inputFile);
                if (saveOrExport == "Export")
                {
                    TripParser.ParseInputStringForConnect(TripUnit.tripUnitString, ' ');

                }
                else if (saveOrExport == "Save")
                {
                    if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && Global.strWorkFlow == Global.str_wizard_EXPORTUI)
                    {
                        var tripUnitStrings = TripUnit.tripUnitString.Split(new[] { '\n' }, 2);
                        TripUnit.tripUnitString = tripUnitStrings[1];
                    }
                    TripParser.ParseInputString(TripUnit.tripUnitString, ' ');

                }

                if (Global.device_type == Global.PTM_DEVICE)
                {
                    XMLParser.parseModelFile_PTM(Global.selectedTemplateType == Global.MCCBTEMPLATE ? Global.filePath_merged_mccb_xmlFile :
                    (Global.selectedTemplateType == Global.NZMTEMPLATE ? Global.filePath_merged_nzm_xmlFile :
                    (Global.selectedTemplateType == Global.ACBTEMPLATE ? Global.filePath_mergedstylesxmlFile : Global.filePath_merged_PTM_xmlfile)));
                }
                else
                {
                XMLParser.parseModelFile(Global.selectedTemplateType == Global.MCCBTEMPLATE ? Global.filePath_merged_mccb_xmlFile :
                    (Global.selectedTemplateType == Global.NZMTEMPLATE ? Global.filePath_merged_nzm_xmlFile :
                    (Global.selectedTemplateType == Global.ACBTEMPLATE ? Global.filePath_mergedstylesxmlFile :
                    Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE ? Global.filePath_merged_acbPXR35_xmlFile : Global.filePath_mergedstylesxmlFile_3_0)));
                }
                Global.updateTriUnitValuesAfterParsing();

                if (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE|| Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE)
                {
                    XMLParser.GetStyleandPlugCodes();
                }
                else
                {
                    XMLParser.GetStyleandPlugCodesForMCCB();
                }
                if (Global.IsOffline && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE))
                {
                    TripParser.LookupStyleAndPlug_Offline();
                }
                else if (!Global.IsOffline && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE))
                {
                    TripParser.LookupStyleAndPlug();
                }

                TripParser.GetStyleRatingPlugDetails();

                if (device_type != Global.MCCBDEVICE && device_type != Global.NZMDEVICE && device_type != Global.ACB_PXR35_DEVICE && !IsOffline)
                {
                    updateGeneralGroupForACBTemplate();
                }
                if (device_type == Global.ACB_PXR35_DEVICE  && !IsOffline)
                {
                    updateGeneralGroupForACBPXR35Template();
                }
                if ((device_type == Global.MCCBDEVICE || device_type == Global.NZMDEVICE) && !IsOffline)
                {
                    updateDetailedGroupForMCCBTemplate();
                    MatchOutputFileToModelFileSettings_online();
                }
                else
                {
                    MatchOutputFileToModelFileSettings();
                }
                updateMMmodeControls();
                updateGCControls();
                updateRTUControls();
                updateTCPControls();

                if (device_type == Global.MCCBDEVICE || device_type == Global.NZMDEVICE)
                {
                    if (!IsOffline)
                    {
                        TripUnit.userUnitType = Global.GlbstrUnitType;
                        TripUnit.userRatingPlug = Global.GlbstrCurrentRating;
                        TripUnit.deviceBreakerInformation = Global.GlbstrBreakerFrame;
                        SetLangaugesFromDevice();
                        //Update Trip Unit Id table for Additional language in NZM refresh issue
                        if (Global.device_type == Global.NZMDEVICE)
                            Global.updateTriUnitValuesAfterParsing();
                    }
                    if (Global.isExportControlFlow)
                    {
                        updateDependenciesForMCCBExport();
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// Added By Sunny:Update general group dependencies for export file
        /// </summary>

        private static void updateDependenciesForMCCBExport()
        {

            if (Global.device_type == Global.MCCBDEVICE && Global.device_type_PXR10)
            {
                IdsToBeUpdated = new string[16] { "SYS01", "SYS02", "SYS16", "GEN01", "GEN02", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10", "GEN11", "GEN12", "CPC11A" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }
            else if (Global.device_type == Global.MCCBDEVICE && !Global.device_type_PXR10)
            {
                IdsToBeUpdated = new string[17] { "SYS01", "SYS02", "SYS16", "GEN01", "GEN02A", "GEN02B", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10", "GEN11", "GEN12", "CPC11A" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                IdsToBeUpdated = new string[20] { "SYS01", "SYS2", "SYS6", "GEN1", "GEN02", "GEN02A", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10A", "GEN11", "GEN12", "GEN13", "GEN14", "GEN15", "GEN16" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }

            Settings gen_setpoint;
            foreach (string Id in IdsToBeUpdated)
            {
                gen_setpoint = (Settings)TripUnit.IDTable[Id];
                gen_setpoint.notifyDependents();
            }
        }

        public static void updateMMmodeControls()
        {
            string[] MM_IdsToBeUpdated = null;
            dictMCCBSetpoints = new Dictionary<string, string>();
            try
            {
                if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                {
                    MM_IdsToBeUpdated = new string[5] { "SYS4AT", "SYS4CT", "SYS4DT", "SYS4ET", "SYS4FT" };
                }
                else if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.PTM_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    MM_IdsToBeUpdated = new string[5] { "SYS004AT", "SYS004CT", "SYS004DT", "SYS004ET", "SYS004FT" };
                }


                if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                {
                    dictMCCBSetpoints.Add("SYS4AT", (TripUnit.MM_b8 == '1') ? Resource.On : Resource.Off);
                    dictMCCBSetpoints.Add("SYS4CT", (TripUnit.MM_b0 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS4DT", (TripUnit.MM_b7 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS4ET", (TripUnit.MM_b1 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS4FT", (TripUnit.MM_b2 == '1') ? Resource.Yes : Resource.No);
                }
                else if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.PTM_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    dictMCCBSetpoints.Add("SYS004AT", (TripUnit.MM_b8 == '1') ? Resource.On : Resource.Off);
                    dictMCCBSetpoints.Add("SYS004CT", (TripUnit.MM_b0 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS004DT", (TripUnit.MM_b7 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS004ET", (TripUnit.MM_b1 == '1') ? Resource.Yes : Resource.No);
                    dictMCCBSetpoints.Add("SYS004FT", (TripUnit.MM_b2 == '1') ? Resource.Yes : Resource.No);
                }
                Settings gen_setpoint;
                if (MM_IdsToBeUpdated != null && MM_IdsToBeUpdated.Count() != 0)
                {
                    foreach (string Id in MM_IdsToBeUpdated)
                    {
                        if (dictMCCBSetpoints[Id] == null)
                        { continue; }
                        gen_setpoint = (Settings)TripUnit.IDTable[Id];
                        if (gen_setpoint == null) continue;
                        if (gen_setpoint.type == Settings.Type.type_text)
                        {
                            gen_setpoint.textStrValue = dictMCCBSetpoints[Id];
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void updateTCPControls()
        {
            string[] GC_IdsToBeUpdated = null;
            dictMCCBSetpoints = new Dictionary<string, string>();
            try
            {
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    GC_IdsToBeUpdated = new string[5] { "CC016A", "CC016B", "CC016C", "CC016D", "CC016E" };
                }

                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    dictMCCBSetpoints.Add("CC016A", (TripUnit.TCP_b4 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC016B", (TripUnit.TCP_b3 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC016C", (TripUnit.TCP_b2 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC016D", (TripUnit.TCP_b1 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC016E", (TripUnit.TCP_b0 == '1') ? "true" : "false");
                }

                Settings gen_setpoint;
                if (GC_IdsToBeUpdated != null && GC_IdsToBeUpdated.Count() != 0)
                {
                    foreach (string Id in GC_IdsToBeUpdated)
                    {
                        if (dictMCCBSetpoints[Id] == null)
                        { continue; }
                        gen_setpoint = (Settings)TripUnit.IDTable[Id];
                        if (gen_setpoint == null) continue;
                        if (gen_setpoint.type == Settings.Type.type_toggle)
                        {
                            gen_setpoint.bValue = bool.Parse(dictMCCBSetpoints[Id]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }


        public static void updateRTUControls()
        {
            string[] GC_IdsToBeUpdated = null;
            dictMCCBSetpoints = new Dictionary<string, string>();
            try
            {
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    GC_IdsToBeUpdated = new string[5] { "CC012A", "CC012B", "CC012C", "CC012D", "CC012E" };
                }

                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    dictMCCBSetpoints.Add("CC012A", (TripUnit.RTU_b4 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC012B", (TripUnit.RTU_b3 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC012C", (TripUnit.RTU_b2 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC012D", (TripUnit.RTU_b1 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("CC012E", (TripUnit.RTU_b0 == '1') ? "true" : "false");
                }

                Settings gen_setpoint;
                if (GC_IdsToBeUpdated != null && GC_IdsToBeUpdated.Count() != 0)
                {
                    foreach (string Id in GC_IdsToBeUpdated)
                    {
                        if (dictMCCBSetpoints[Id] == null)
                        { continue; }
                        gen_setpoint = (Settings)TripUnit.IDTable[Id];
                        if (gen_setpoint == null) continue;
                        if (gen_setpoint.type == Settings.Type.type_toggle)
                        {
                            gen_setpoint.bValue = bool.Parse(dictMCCBSetpoints[Id]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void updateGCControls()
        {
            string[] GC_IdsToBeUpdated = null;
            dictMCCBSetpoints = new Dictionary<string, string>();
            try
            {
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    GC_IdsToBeUpdated = new string[8] { "GC00112A", "GC00112B", "GC00112C", "GC00112D", "GC00112E", "GC00112F", "GC00112G", "GC00112H" };
                }

                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    dictMCCBSetpoints.Add("GC00112A", (TripUnit.GC_b0 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112B", (TripUnit.GC_b1 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112C", (TripUnit.GC_b2 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112D", (TripUnit.GC_b3 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112E", (TripUnit.GC_b4 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112F", (TripUnit.GC_b5 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112G", (TripUnit.GC_b6 == '1') ? "true" : "false");
                    dictMCCBSetpoints.Add("GC00112H", (TripUnit.GC_b7 == '1') ? "true" : "false");
                }

                Settings gen_setpoint;
                if (GC_IdsToBeUpdated != null && GC_IdsToBeUpdated.Count() != 0)
                {
                    foreach (string Id in GC_IdsToBeUpdated)
                    {
                        if (dictMCCBSetpoints[Id] == null)
                        { continue; }
                        gen_setpoint = (Settings)TripUnit.IDTable[Id];
                        if (gen_setpoint == null) continue;
                        if (gen_setpoint.type == Settings.Type.type_toggle)
                        {
                            gen_setpoint.bValue = bool.Parse(dictMCCBSetpoints[Id]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void updatePowerDemandControls()
        {
            try
            {

                Settings PDMode_setting = TripUnit.getPowerDemandMode();
                PDMode_setting.bValue = TripUnit.PD_b0 == '0' ? false : true;
                PDMode_setting.bDefault = PDMode_setting.bValue;
                PDMode_setting.bValueReadFromTripUnit = PDMode_setting.bValue;


                Settings PDModePrecision_setting = TripUnit.getPowerDemandPrecision();
                string SYS14Value = Convert.ToString(TripUnit.PD_b5 + TripUnit.PD_b4);
                switch (SYS14Value)
                { /*
                                           *   00 = Precise, 01 = Normal, 10 = Relaxed, 11 = Not used
                                           * */
                    case "00":
                        PDModePrecision_setting.selectionIndex = 0;
                        PDModePrecision_setting.selectionValue = Resource.SYS14Item0000;
                        break;
                    case "01":
                        PDModePrecision_setting.selectionIndex = 1;
                        PDModePrecision_setting.selectionValue = Resource.SYS14Item0001;
                        break;
                    case "10":
                        PDModePrecision_setting.selectionIndex = 2;
                        PDModePrecision_setting.selectionValue = Resource.SYS14Item0010;
                        break;

                }

            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }
        public static void updateDetailedGroupForMCCBTemplate()
        {
            // Code refacturing needs to be done by adding it to collection ==============
            updateGeneralGroupForMCCBTemplate(); // first set all the values and then call notify dependants
            Settings gen_setpoint;
            foreach (string Id in IdsToBeUpdated)
            {
                gen_setpoint = (Settings)TripUnit.IDTable[Id];
                gen_setpoint.defaultSelectionValue = dictMCCBSetpoints[Id];
                gen_setpoint.selectionValue = dictMCCBSetpoints[Id];
                gen_setpoint.notifyDependents();
            }
        }
        static string[] IdsToBeUpdated;
        static Dictionary<string, string> dictMCCBSetpoints;
        public static void updateModbusAndRelaySetpoint()
        {
            IdsToBeUpdated = new string[3] { "GEN06", "GEN08", "GEN11" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            dictMCCBSetpoints = new Dictionary<string, string>();

            if (!Global.IsOffline && grouplist != null && grouplist.Count != 0)
            {
                Global.GlbstrModBus = (Convert.ToBoolean(Convert.ToInt32(style2[0].ToString()))) ? Resource.ParaSel_ModbusComms_MCCB2 : Resource.ParaSel_ModbusComms_MCCB1;
                Global.GlbstrRelayFeature = (Convert.ToBoolean(Convert.ToInt32(style2[3].ToString()))) ? Resource.ParaSel_RelayFeature_MCCB1 : Resource.ParaSel_RelayFeature_MCCB2;
                TripUnit.userModBus = Global.GlbstrModBus;
                TripUnit.userRelayFeature = Global.GlbstrRelayFeature;
            }
            if (Global.IsOffline)
            {
                Global.GlbstrModBus = TripUnit.deviceModBus;
                Global.GlbstrRelayFeature = TripUnit.deviceRelayFeature;
            }

            Global.GlbstrAuxConnected = Global.auxPowerConnected == true ? Resource.GEN11Item0001 : Resource.GEN11Item0000;
            if (!Global.IsOffline)
            {
                dictMCCBSetpoints.Add("GEN11", Global.GlbstrAuxConnected);
                dictMCCBSetpoints.Add("GEN06", Global.GlbstrModBus);
                dictMCCBSetpoints.Add("GEN08", Global.GlbstrRelayFeature);
                Settings gen_setpoint;
                foreach (string Id in IdsToBeUpdated)
                {
                    gen_setpoint = (Settings)TripUnit.IDTable[Id];
                    if (gen_setpoint.type == Settings.Type.type_toggle)
                    {
                        var resKey = Id + "Item0001";
                        var bValue = dictMCCBSetpoints[Id] == Resources.Strings.Resource.ResourceManager.GetString(resKey) ? true : false;

                        // gen_setpoint.bDefault = bValue;
                        if (gen_setpoint.bValue != bValue || gen_setpoint.bValueReadFromTripUnit != bValue)
                        {
                            gen_setpoint.bValue = bValue;
                            gen_setpoint.bValueReadFromTripUnit = bValue;
                            gen_setpoint.notifyDependents();
                        }
                    }
                    else
                    {
                        if (gen_setpoint.defaultSelectionValue != dictMCCBSetpoints[Id] || gen_setpoint.selectionValue != dictMCCBSetpoints[Id])
                        {
                            gen_setpoint.defaultSelectionValue = dictMCCBSetpoints[Id];
                            gen_setpoint.selectionValue = dictMCCBSetpoints[Id];
                            gen_setpoint.notifyDependents();
                        }
                    }
                }
            }
        }
        private static void updateGeneralGroupForMCCBTemplate()
        {
            if (Global.device_type == Global.MCCBDEVICE && Global.device_type_PXR10)
            {
                IdsToBeUpdated = new string[18] { "SYS01", "SYS02", "SYS16", "GEN01", "GEN02", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10", "GEN11", "GEN12", "GEN17", "GEN18", "CPC11A" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }
            else if (Global.device_type == Global.MCCBDEVICE && !Global.device_type_PXR10)
            {
                IdsToBeUpdated = new string[20] { "SYS01", "SYS02", "SYS16", "GEN01", "GEN02A", "GEN02B", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10", "GEN11", "GEN12", "GEN17", "GEN18", "CPC11A", "SYS4AT" }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                IdsToBeUpdated = new string[23] { "SYS01", "SYS2", "SYS6", "GEN1", "GEN02A", "GEN02B", "GEN02C", "GEN03", "GEN04", "GEN05", "GEN06", "GEN07", "GEN08", "GEN09", "GEN10A", "GEN11", "GEN12", "GEN13", "GEN14", "GEN15", "GEN16", "GEN17", "GEN18", }; // Rateing, Frame, Unit Type, Trip Unit, Firmware Version
            }

            dictMCCBSetpoints = new Dictionary<string, string>();

            Global.GlbstrCurrentRating = TripUnit.getRating().selectionValue;
            Global.GlbstrBreakerFrame = TripUnit.deviceBreakerInformation;
            Global.appFirmware = Global.deviceFirmware;

            TripUnit.userRatingPlug = Global.GlbstrCurrentRating;
            TripUnit.deviceBreakerInformation = Global.GlbstrBreakerFrame;

            TripUnit.deviceInstanttenious = (Convert.ToBoolean(Convert.ToInt32(Global.style1[2].ToString()))) ? Resource.Yes : Resource.No;
            TripUnit.deviceLongDelay = (Convert.ToBoolean(Convert.ToInt32(Global.style1[0].ToString()))) ? Resource.Yes : Resource.No;
            Global.GlbstrZSI = (Convert.ToBoolean(Convert.ToInt32(style2[4].ToString()))) ? Resource.ParaSel_ZSI_MCCB1 : Resource.ParaSel_ZSI_MCCB2;
            Global.GlbstrShortDelay = (Convert.ToBoolean(Convert.ToInt32(style1[1].ToString()))) ? Resource.ParaSel_ShortDelay_MCCB1 : Resource.ParaSel_ShortDelay_MCCB2;
            Global.GlbstrGFP = (Convert.ToBoolean(Convert.ToInt32(style1[3].ToString()))) ? Resource.ParaSel_GroundProtection_MCCB1 : Resource.ParaSel_GroundProtection_MCCB2;
            Global.GlbstrARMS = (Convert.ToBoolean(Convert.ToInt32(style1[4].ToString()))) ? Resource.ParaSel_ARMSProtection_MCCB1 : Resource.ParaSel_ARMSProtection_MCCB2;
            Global.GlbstrModBus = (Convert.ToBoolean(Convert.ToInt32(style2[0].ToString()))) ? Resource.ParaSel_ModbusComms_MCCB2 : Resource.ParaSel_ModbusComms_MCCB1;
            Global.GlbstrCAMSelection = (Convert.ToBoolean(Convert.ToInt32(style2[1].ToString()))) ? Resource.ParaSel_CAMSelection_MCCB1 : Resource.ParaSel_CAMSelection_MCCB2;
            Global.GlbstrRelayFeature = (Convert.ToBoolean(Convert.ToInt32(style2[3].ToString()))) ? Resource.ParaSel_RelayFeature_MCCB1 : Resource.ParaSel_RelayFeature_MCCB2;
            //  Global.GlbstrUnitType = (Convert.ToBoolean(Convert.ToInt32(style2[13].ToString()))) ? Resource.ParaSel_UnitType_MCCB1 : Resource.ParaSel_UnitType_MCCB0;
            Global.GlbstrMotor = (Convert.ToBoolean(Convert.ToInt32(style1[7].ToString()))) ? Resource.GEN12Item0001 : Resource.GEN12Item0000;
            Global.GlbstrNeutral = (Convert.ToBoolean(Convert.ToInt32(style1[8].ToString()))) ? Resource.GEN12Item0001 : Resource.GEN12Item0000;
            Global.GlbstrThermalMemory = (Convert.ToBoolean(Convert.ToInt32(style1[9].ToString()))) ? Resource.GEN12Item0001 : Resource.GEN12Item0000;
            //if (Global.device_type_PXR25 || Global.device_type_PXR20D)
            //{
            //    TripUnit.ExtentedMetering = Resource.Yes;
            //    TripUnit.BasicMetering = Resource.Yes;
            //}
            //else
            //{
            TripUnit.ExtentedMetering = (Convert.ToBoolean(Convert.ToInt32(style1[12].ToString()))) ? Resource.Yes : Resource.No;
            TripUnit.BasicMetering = Resource.Yes;
            //}
            if (Global.device_type == Global.MCCBDEVICE)
            {
                if (TripUnit.ResponseForUnitType == "1")
                {
                    Global.GlbstrUnitType = Resource.ParaSel_UnitType_MCCB0;
                }
                else if (TripUnit.ResponseForUnitType == "2")
                {
                    Global.GlbstrUnitType = Resource.ParaSel_UnitType_MCCB1;
                }
                else
                {
                    Global.GlbstrUnitType = Resource.ParaSel_UnitType_MCCB2;
                }
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                if (TripUnit.ResponseForUnitType == "1")
                {
                    Global.GlbstrUnitType = Resource.SYS6Item0001;
                }
                else if (TripUnit.ResponseForUnitType == "3")
                {
                    Global.GlbstrUnitType = Resource.SYS6Item0003;
                }
            }
            //       Global.GlbstrUnitType = TripUnit.ResponseForUnitType == "1" ? Resource.ParaSel_UnitType_MCCB0: Resource.ParaSel_UnitType_MCCB0;
            Global.GlbstrAuxConnected = Global.auxPowerConnected == true ? Resource.GEN11Item0001 : Resource.GEN11Item0000;
            //TODO Fix Me : Quick fix ,update after data received ==== START ==============
            //   Global.GlbstrTripUnitType = Resource.ParaSel_TripUnit_MCCB4;
            Global.GlbstrPole = TripUnit.userPoleValue;
            // IdentifyMccbStyle obj = new IdentifyMccbStyle();
            //  bool result = obj.writeMCCBUnitTypeToGlobal();
            TripUnit.userUnitType = Global.GlbstrUnitType;

            //===================== END ===================================================           

            ////Update Device data ===============================
            //TripUnit.deviceZSI = Global.GlbstrZSI;
            //TripUnit.deviceShortDelay = Global.GlbstrShortDelay;
            //TripUnit.deviceGFP = Global.GlbstrGFP;
            //TripUnit.deviceARMS = Global.GlbstrARMS;
            //TripUnit.deviceModBus = Global.GlbstrModBus;
            //TripUnit.deviceCAMSelection = Global.GlbstrCAMSelection;
            //TripUnit.deviceRelayFeature = Global.GlbstrRelayFeature;
            ////==============================================

            dictMCCBSetpoints.Add("SYS01", Global.GlbstrCurrentRating);
            if (Global.device_type == Global.MCCBDEVICE)
            {
                dictMCCBSetpoints.Add("SYS02", Global.GlbstrBreakerFrame);
                dictMCCBSetpoints.Add("SYS4AT", (TripUnit.MM_b8 == '1') ? Resource.On : Resource.Off);
                dictMCCBSetpoints.Add("GEN01", Global.GlbstrTripUnitType);
                dictMCCBSetpoints.Add("SYS16", Global.GlbstrUnitType);
                dictMCCBSetpoints.Add("CPC11A", TripUnit.FrameConstruction);
                dictMCCBSetpoints.Add("GEN10", Global.GlbstrPole);

                if (Global.device_type_PXR10)
                {
                    dictMCCBSetpoints.Add("GEN02", Global.appFirmware);
                }
                else
                {
                    dictMCCBSetpoints.Add("GEN02A", Global.appFirmware);
                    dictMCCBSetpoints.Add("GEN02B", Global.appFirmware2);
                }

                if (Global.isDemoMode)
                {
                    Global.appFirmware = "";
                    Global.appFirmware2 = "";
                }
            }
            else if (Global.device_type == Global.NZMDEVICE)
            {
                dictMCCBSetpoints.Add("SYS2", Global.GlbstrBreakerFrame);
                dictMCCBSetpoints.Add("GEN1", Global.GlbstrTripUnitType);
                dictMCCBSetpoints.Add("SYS6", Global.GlbstrUnitType);
                dictMCCBSetpoints.Add("GEN16", TripUnit.ExtentedMetering);
                dictMCCBSetpoints.Add("GEN15", TripUnit.BasicMetering);
                dictMCCBSetpoints.Add("GEN14", TripUnit.deviceInstanttenious);
                dictMCCBSetpoints.Add("GEN13", TripUnit.deviceLongDelay);
                dictMCCBSetpoints.Add("GEN02A", Global.appFirmware);
                dictMCCBSetpoints.Add("GEN10A", Global.GlbstrPole);
                dictMCCBSetpoints.Add("GEN02B", Global.appFirmware2);
                dictMCCBSetpoints.Add("GEN02C", Global.appFirmware3);
            }



            dictMCCBSetpoints.Add("GEN09", Global.GlbstrZSI);
            dictMCCBSetpoints.Add("GEN03", Global.GlbstrShortDelay);
            dictMCCBSetpoints.Add("GEN04", Global.GlbstrGFP);
            dictMCCBSetpoints.Add("GEN05", Global.GlbstrARMS);
            dictMCCBSetpoints.Add("GEN06", Global.GlbstrModBus);
            dictMCCBSetpoints.Add("GEN07", Global.GlbstrCAMSelection);
            dictMCCBSetpoints.Add("GEN08", Global.GlbstrRelayFeature);
            dictMCCBSetpoints.Add("GEN11", Global.GlbstrAuxConnected);
            dictMCCBSetpoints.Add("GEN12", Global.GlbstrMotor);
            dictMCCBSetpoints.Add("GEN17", Global.GlbstrNeutral);
            dictMCCBSetpoints.Add("GEN18", Global.GlbstrThermalMemory);

            ////13Long Delay Protection
            //14 Short Circuit Protection       



            Settings gen_setpoint;
            foreach (string Id in IdsToBeUpdated)
            {
                if (dictMCCBSetpoints[Id] == null)
                { continue; }
                gen_setpoint = (Settings)TripUnit.IDTable[Id];
                if (gen_setpoint.type == Settings.Type.type_toggle)
                {
                    var resKey = Id + "Item0001";
                    var bValue = dictMCCBSetpoints[Id] == Resources.Strings.Resource.ResourceManager.GetString(resKey) ? true : false;

                    //  gen_setpoint.bDefault = bValue;
                    gen_setpoint.bValue = bValue;
                    gen_setpoint.bValueReadFromTripUnit = bValue;
                }
                else if (gen_setpoint.type == Settings.Type.type_text)
                {
                    gen_setpoint.textStrValue = dictMCCBSetpoints[Id];
                }
                else if (gen_setpoint.type == Settings.Type.type_selection)
                {
                    gen_setpoint.defaultSelectionValue = dictMCCBSetpoints[Id];
                    gen_setpoint.selectionValue = dictMCCBSetpoints[Id];
                }

            }
        }

        private static void updateReversLookupTableForLanguage(Settings languages)
        {// on change of lookup table update revers lookup table for language

            if (languages.lookupTable.Count != languages.reverseLookupTable.Count)
            {
                List<string> values = new List<string>();
                foreach (var ele in languages.reverseLookupTable.Values)
                {
                    if (!languages.lookupTable.ContainsKey(ele))
                    {
                        values.Add(ele.ToString());
                    }
                }
                if (values.Count > 0)
                {
                    foreach (string ele in values)
                    {
                        string key = languages.reverseLookupTable.Keys.OfType<String>().FirstOrDefault(a => (string)languages.reverseLookupTable[a] == ele);
                        languages.reverseLookupTable.Remove(key);
                    }
                }
            }
        }
        public static void SetLangaugesFromDevice()
        {
            Settings languages = TripUnit.getLanguage();
            Hashtable deviceLanguagelookupTable = new Hashtable();
            //languages.lookupTable.Clear();
            deviceLanguagelookupTable.Clear();
            foreach (var language in listValidMCCBLanguages)
            {
                var convertDecimal = (int)((decimal)Convert.ToDecimal(language.LanguageCode));
                //  (int)(decimal)(language.LanguageCode);
                var hexNumber = convertDecimal.ToString("X");
                if (hexNumber.Length != 4)
                {
                    switch (hexNumber.Length)
                    {
                        case 1:
                            hexNumber = "000" + hexNumber;
                            break;
                        case 2:
                            hexNumber = "00" + hexNumber;
                            break;
                        case 3:
                            hexNumber = "0" + hexNumber;
                            break;
                        default:
                            break;
                    }
                }
                //   string hexLanguageCode = convertDecimal.ToString("X");

                var languageName = string.Empty;
                if (languages.lookupTable.Contains(hexNumber))
                {
                    languageName = ((PXR.item_ComboBox)(languages.lookupTable[hexNumber])).item;

                    //set 'Additional Language' name on Setpoint screen for Languages combobox instead of newly downloaded language name for NZM
                    if (hexNumber == "000A")
                    {
                        languageName = language.LanguageName;
                        var removeEntry = string.Empty;
                        foreach (var key in languages.reverseLookupTable.Keys)
                        {
                            if ((languages.reverseLookupTable[key]).ToString() == "000A")
                            {
                                removeEntry = key.ToString();
                                break;
                            }
                        }
                        languages.reverseLookupTable.Remove(removeEntry);
                        languages.reverseLookupTable.Add(languageName, "000A");
                    }
                }

                deviceLanguagelookupTable.Add(hexNumber, new item_ComboBox(hexNumber, languageName, ""));
            }

            if (!Global.IsOffline && deviceLanguagelookupTable.Count > 0)
            {
                languages.lookupTable.Clear();
                languages.lookupTable = deviceLanguagelookupTable;

                updateReversLookupTableForLanguage(languages);
            }
            else if (!Global.IsOffline && deviceLanguagelookupTable.Count == 0)
            {
                foreach (DictionaryEntry de in languages.lookupTable)
                {

                    switch (Global.device_type)
                    {
                        case Global.MCCBDEVICE:
                            if ((de.Key.ToString() == "0001" || de.Key.ToString() == "0002" || de.Key.ToString() == "0004" || de.Key.ToString() == "0006"))
                                deviceLanguagelookupTable.Add(de.Key, de.Value);
                            break;
                        //by SRK to fix PXPM-6391
                        case Global.NZMDEVICE:
                            if (de.Key.ToString() == "0001" || de.Key.ToString() == "0002" || de.Key.ToString() == "0003" || de.Key.ToString() == "0005" || de.Key.ToString() == "0019" || de.Key.ToString() == "0008" || de.Key.ToString() == "0026" || de.Key.ToString() == "0012")
                                deviceLanguagelookupTable.Add(de.Key, de.Value);
                            break;
                    }
                }
                languages.lookupTable.Clear();
                languages.lookupTable = deviceLanguagelookupTable;

                updateReversLookupTableForLanguage(languages);
            }


        }
        public static void updateGeneralGroupForACBPXR35Template()
        {
            setpointForStyle = TripUnit.getTripUnitStyle();
            setpointForStyle.selectionValue = TripUnit.userStyle;
            setpointForStyle.defaultSelectionValue = TripUnit.userStyle;
            // code for setting the general setpoints

            //Quick fix update later from data received from trip uni === START===
            if (TripUnit.userStyle.Contains("PXR35"))
            {
                Global.GlbstrTripUnitType = "PXR 35";
            }
            //========================================== END =======
            Settings gen_setpoint;
            gen_setpoint = TripUnit.getUnittypeANSI_IECGeneralGrp();
            gen_setpoint.defaultSelectionValue = Resource.SYS000Item0001; // Global.GlbstrUnitType;
            gen_setpoint.selectionValue = Resource.SYS000Item0001; //Global.GlbstrUnitType;

            gen_setpoint = TripUnit.getUnittypeGroup1ForACB();
            gen_setpoint.defaultSelectionValue = Resource.SYS000Item0001; //Global.GlbstrUnitType;
            gen_setpoint.selectionValue = Resource.SYS000Item0001; //Global.GlbstrUnitType;

            gen_setpoint = TripUnit.getTripUnitType();
            gen_setpoint.defaultSelectionValue = Global.GlbstrTripUnitType;
            gen_setpoint.selectionValue = Global.GlbstrTripUnitType;

            gen_setpoint = TripUnit.getFirmwareVersionSel();
            gen_setpoint.defaultSelectionValue = Global.appFirmware;
            gen_setpoint.selectionValue = Global.appFirmware;

            gen_setpoint = TripUnit.getFirmwareVersion2();
            gen_setpoint.textStrValue = Global.appFirmware2;
            gen_setpoint.defaulttextStrValue = Global.appFirmware2;

            gen_setpoint = TripUnit.getFirmwareVersion3();
            gen_setpoint.textStrValue = Global.appFirmware3;
            gen_setpoint.defaulttextStrValue = Global.appFirmware3;


            gen_setpoint = TripUnit.getMM_ModeProtectionGeneralGrp();
            gen_setpoint.bValue = true;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;


            gen_setpoint = TripUnit.getTripUnitStyle();
            string style = gen_setpoint.selectionValue.ToString();

            gen_setpoint = TripUnit.getGroundProtectionGeneralGrp();
            gen_setpoint.bValue = Global.GlbstrGFP == Resource.GEN004Item0001 ? true : false;//style.Contains('G') ? true : false;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;

            gen_setpoint = TripUnit.getBluetoothGeneralGrp();
            gen_setpoint.bValue = style.Contains('B') ? true : false;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;


            gen_setpoint = TripUnit.getGroundClampingOnOff();
            gen_setpoint.bValue = TripUnit.deviceClampingState;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;


            if (Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
            {
                String rawLineFrequency = ((String)TripUnit.rawSetPoints[6]);
                rawLineFrequency = rawLineFrequency.PadLeft(4, '0');
                Global.GlbstrLineFrequency = TripUnit.lookupTable_LineFrequency[rawLineFrequency].ToString();

                gen_setpoint = TripUnit.getLineFrequency();
                gen_setpoint.selectionValue = Global.GlbstrLineFrequency;
                gen_setpoint.defaultSelectionValue = Global.GlbstrLineFrequency;

                //set value to pole 
                gen_setpoint = TripUnit.getPole();
                gen_setpoint.selectionValue = TripUnit.userPoleValue == "8" ? "8" : TripUnit.userPoleValue == "4" ? "4" : "2";
                gen_setpoint.defaultSelectionValue = gen_setpoint.selectionValue;

            }

            if (Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
            {
                gen_setpoint = TripUnit.getPoleforPXR35();
                gen_setpoint.selectionValue = TripUnit.userPoleValue == "4" ? "4" : TripUnit.userPoleValue == "3" ? "3" : "3";
                gen_setpoint.defaultSelectionValue = gen_setpoint.selectionValue;
            }

            setpointForStyle.notifyDependents();

            Settings setpointForDuplicateStyle = TripUnit.getTripUnitStyleACBGrp1();
            setpointForDuplicateStyle.defaultSelectionValue = setpointForStyle.defaultSelectionValue;
            setpointForDuplicateStyle.selectionValue = setpointForStyle.selectionValue;

            Settings setpointForCurrentRating = TripUnit.getRating();
            setpointForCurrentRating.defaultSelectionValue = Global.GlbstrCurrentRating;
            setpointForCurrentRating.selectionValue = Global.GlbstrCurrentRating;

            setpointForCurrentRating = TripUnit.getRatingACBGrp1();
            setpointForCurrentRating.defaultSelectionValue = Global.GlbstrCurrentRating;
            setpointForCurrentRating.selectionValue = Global.GlbstrCurrentRating;

            Settings setpointForBreakerFrame = TripUnit.getBreakerFrameACBGrp1();
            setpointForBreakerFrame.defaultSelectionValue = Global.GlbstrBreakerFrame;
            setpointForBreakerFrame.selectionValue = Global.GlbstrBreakerFrame;


        }

        public static void updateGeneralGroupForACBTemplate()
        {
            setpointForStyle = TripUnit.getTripUnitStyle();
            setpointForStyle.selectionValue = TripUnit.userStyle;
            setpointForStyle.defaultSelectionValue = TripUnit.userStyle;

            if (Global.device_type == Global.PTM_DEVICE)
            {
                //Quick fix update later from data received from trip uni === START===
                if (TripUnit.userStyle.Contains("PT20"))
                {
                    Global.GlbstrTripUnitType = "PT 20";
                }
                else if (TripUnit.userStyle.Contains("PT10"))
                {
                    Global.GlbstrTripUnitType = "PT 10";
                }
                else if (TripUnit.userStyle.Contains("PT25"))
                {
                    Global.GlbstrTripUnitType = "PT 25";
                }
                //========================================== END =======
            }
            else
            {

                // code for setting the general setpoints

                //Quick fix update later from data received from trip uni === START===
                if (TripUnit.userStyle.Contains("PXR20"))
                {
                    Global.GlbstrTripUnitType = Resource.ParaSel_TripUnit_ACB1;
                }
                else if (TripUnit.userStyle.Contains("PXR25"))
                {
                    Global.GlbstrTripUnitType = Resource.ParaSel_TripUnit_ACB2;
                }
            }
            //========================================== END =======
            Settings gen_setpoint;
            gen_setpoint = TripUnit.getUnittypeANSI_IECGeneralGrp();
            gen_setpoint.defaultSelectionValue = Global.GlbstrUnitType;
            gen_setpoint.selectionValue = Global.GlbstrUnitType;

            gen_setpoint = TripUnit.getUnittypeGroup1ForACB();
            gen_setpoint.defaultSelectionValue = Global.GlbstrUnitType;
            gen_setpoint.selectionValue = Global.GlbstrUnitType;

            gen_setpoint = TripUnit.getTripUnitType();
            gen_setpoint.defaultSelectionValue = Global.GlbstrTripUnitType;
            gen_setpoint.selectionValue = Global.GlbstrTripUnitType;

            gen_setpoint = TripUnit.getFirmwareVersionSel();
            if (Global.device_type == Global.PTM_DEVICE)
            {             
                gen_setpoint.textStrValue = Global.appFirmware;
                gen_setpoint.defaulttextStrValue = Global.appFirmware;
            }
            else
            {
                gen_setpoint.defaultSelectionValue = Global.appFirmware;
                gen_setpoint.selectionValue = Global.appFirmware;
            }

            gen_setpoint = TripUnit.getFirmwareVersion2();
            gen_setpoint.textStrValue = Global.appFirmware2;
            gen_setpoint.defaulttextStrValue = Global.appFirmware2;

            gen_setpoint = TripUnit.getFirmwareVersion3();
            gen_setpoint.textStrValue = Global.appFirmware3;
            gen_setpoint.defaulttextStrValue = Global.appFirmware3;

            //Added by SRK for ACB breaker Health
            //if (Global.device_type == Global.PTM_DEVICE)
            //{
            //    gen_setpoint = TripUnit.getFirmwareVersion4();
            //    gen_setpoint.selectionValue = Global.appFirmware4;
            //    gen_setpoint.defaultSelectionValue = Global.appFirmware4;
            //    gen_setpoint.notifyDependents();
            //}

            if (Global.device_type == Global.PTM_DEVICE)
            {
                gen_setpoint = TripUnit.getFirmwareVersion4();
                gen_setpoint.selectionValue = Global.appFirmware4;
                gen_setpoint.defaultSelectionValue = Global.appFirmware4;
                gen_setpoint.notifyDependents();
            }


            gen_setpoint = TripUnit.getMM_ModeProtectionGeneralGrp();
            gen_setpoint.bValue = Global.GlbstrARMS == Resource.GEN003Item0001 ? true : false;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;

            gen_setpoint = TripUnit.getGroundProtectionGeneralGrp();
            gen_setpoint.bValue = Global.GlbstrGFP == Resource.GEN004Item0001 ? true : false;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;

            gen_setpoint = TripUnit.getModbusCommsGeneralGrp();
            gen_setpoint.bValue = Global.GlbstrModBus == Resource.GEN005Item0001 ? true : false;
            gen_setpoint.bValueReadFromTripUnit = gen_setpoint.bValue;

            if (Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.device_type == Global.PTM_DEVICE)
            {
                String rawLineFrequency = ((String)TripUnit.rawSetPoints[6]);
                rawLineFrequency = rawLineFrequency.PadLeft(4, '0');
                Global.GlbstrLineFrequency = TripUnit.lookupTable_LineFrequency[rawLineFrequency].ToString();

                gen_setpoint = TripUnit.getLineFrequency();
                gen_setpoint.selectionValue = Global.GlbstrLineFrequency;
                gen_setpoint.defaultSelectionValue = Global.GlbstrLineFrequency;

                //set value to pole 
                gen_setpoint = TripUnit.getPole();
                gen_setpoint.selectionValue = TripUnit.userPoleValue == "8" ? "8" : TripUnit.userPoleValue == "4" ? "4" : "2";
                gen_setpoint.defaultSelectionValue = gen_setpoint.selectionValue;

            }

            setpointForStyle.notifyDependents();

            Settings setpointForDuplicateStyle = TripUnit.getTripUnitStyleACBGrp1();
            setpointForDuplicateStyle.defaultSelectionValue = setpointForStyle.defaultSelectionValue;
            setpointForDuplicateStyle.selectionValue = setpointForStyle.selectionValue;

            Settings setpointForCurrentRating = TripUnit.getRating();
            setpointForCurrentRating.defaultSelectionValue = Global.GlbstrCurrentRating;
            setpointForCurrentRating.selectionValue = Global.GlbstrCurrentRating;

            setpointForCurrentRating = TripUnit.getRatingACBGrp1();
            setpointForCurrentRating.defaultSelectionValue = Global.GlbstrCurrentRating;
            setpointForCurrentRating.selectionValue = Global.GlbstrCurrentRating;

            Settings setpointForBreakerFrame = TripUnit.getBreakerFrameACBGrp1();
            setpointForBreakerFrame.defaultSelectionValue = Global.GlbstrBreakerFrame;
            setpointForBreakerFrame.selectionValue = Global.GlbstrBreakerFrame;
        }

        public static string temp;


        public static ArrayList BackUpMCCBData { get; internal set; }
        public static bool isMCCBBackUp { get; internal set; }
        public static bool CamCommunicationDisabled { get; internal set; }
        public static bool CommunicationGroupDisabled { get; internal set; }

        public static List<Settings> IterationList = new List<Settings>();
        internal static string str_wizard_SetPoint;
        internal static Dictionary<MemoryStream, String> dictMemStrCurveData;


        public static void GetRelayValues()
        {// Assign Values to PXR35 relays
         // Relay values starts form 169 location is rawsetPoints 
         // Each relay will have 4 valus mapped 
         //TripUnit.rawSetPoints

            Settings Relay1 = (Settings)TripUnit.IDTable["SYS131C"];
            Settings Relay2 = (Settings)TripUnit.IDTable["SYS141C"];
            Settings Relay3 = (Settings)TripUnit.IDTable["SYS151C"];

            if (Relay1 == null || Relay2 == null || Relay3 == null)
            {
                return;
            }
            if (Global.IsOpenFile || Global.isExportControlFlow)
            {
                Relay1.relayOriginalValue = Relay1.textStrValue;
                Relay2.relayOriginalValue = Relay2.textStrValue;
                Relay3.relayOriginalValue = Relay3.textStrValue;
            }
            else
            {
                Relay1.relayOriginalValue = TripUnit.rawSetPoints[184].ToString() + " " + TripUnit.rawSetPoints[185].ToString() + " " + TripUnit.rawSetPoints[186] + " " + TripUnit.rawSetPoints[187];
                Relay2.relayOriginalValue = TripUnit.rawSetPoints[188].ToString() + " " + TripUnit.rawSetPoints[189].ToString() + " " + TripUnit.rawSetPoints[190] + " " + TripUnit.rawSetPoints[191];
                Relay3.relayOriginalValue = TripUnit.rawSetPoints[192].ToString() + " " + TripUnit.rawSetPoints[193].ToString() + " " + TripUnit.rawSetPoints[194] + " " + TripUnit.rawSetPoints[195];

                Relay1.textStrValue = Relay1.relayOriginalValue;
                Relay2.textStrValue = Relay2.relayOriginalValue;
                Relay3.textStrValue = Relay3.relayOriginalValue;
            }

            Relay1.relayChosenValue = Relay1.relayOriginalValue;
            Relay2.relayChosenValue = Relay2.relayOriginalValue;
            Relay3.relayChosenValue = Relay3.relayOriginalValue;
        }
        /// <summary>
        /// Matches the settings file variables to the settings coming from the trip unit
        /// </summary>
        public static void MatchOutputFileToModelFileSettings(string setFor = str_app_ID_Table)
        {
            int setPointIndex = 0;
            int iterationCount = 0;
            Settings setting;
            String rawSetting;
            // string valueForMaintenancemode = string.Empty;        //#COVARITY FIX     234901
            string valueForMaintenanceModeRemote; //#COVARITY FIX     234901
            bool isACBRead = !IsOffline && (device_type == ACBDEVICE || device_type == ACB_02_01_XX_DEVICE );
            bool isACB3_0Read = !IsOffline && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || device_type == ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE);
            bool isMCCBRead = !IsOffline && device_type == MCCBDEVICE;
            bool isNZMRead = !IsOffline && device_type == NZMDEVICE;

            iterationCount = (isMCCBRead || isNZMRead || isACB3_0Read) ? grouplist.Count : TripUnit.rawSetPoints.Count;

            var defaultcount = 0;
            var iterationGroups = new ArrayList();
            Global.IterationList = new List<Settings>();
            Settings GEN01Obj = null;
            Settings IP001Obj = null;
            bool Pxset1IDTableFlag = false;
            bool Pxset2IDTableFlag = false;
            switch (setFor)
            {
                case str_app_ID_Table:
                    iterationGroups = new ArrayList(TripUnit.groups);
                    foreach (var settingID in TripUnit.ID_list)
                    {
                        Settings setting1 = (Settings)TripUnit.IDTable[settingID];
                        Global.IterationList.Add(setting1);
                    }
                    GEN01Obj = TripUnit.getTripUnitType();
                    IP001Obj = TripUnit.getIPAddress();
                    break;

                case str_pxset1_ID_Table:
                    iterationGroups = new ArrayList(TripUnit.Pxset1groups);
                    foreach (var settingID in TripUnit.ID_list_Pxset1)
                    {
                        Settings setting1 = (Settings)TripUnit.Pxset1IDTable[settingID];
                        Global.IterationList.Add(setting1);
                    }
                    Pxset1IDTableFlag = true;
                    GEN01Obj = TripUnit.getTripUnitType(Pxset1IDTableFlag, Pxset2IDTableFlag);
                    IP001Obj = TripUnit.getIPAddress(Pxset1IDTableFlag, Pxset2IDTableFlag);
                    break;
                case str_pxset2_ID_Table:
                    iterationGroups = new ArrayList(TripUnit.Pxset2groups);
                    foreach (var settingID in TripUnit.ID_list_Pxset2)
                    {
                        Settings setting1 = (Settings)TripUnit.Pxset2IDTable[settingID];
                        Global.IterationList.Add(setting1);
                    }
                    Pxset2IDTableFlag = true;
                    GEN01Obj = TripUnit.getTripUnitType(Pxset1IDTableFlag, Pxset2IDTableFlag);
                    IP001Obj = TripUnit.getIPAddress(Pxset1IDTableFlag, Pxset2IDTableFlag);
                    break;
            }

            //For ACB3.0 no need to remove group 0,
            //ex - line frequency will fall under group 0, so that value need to be set from raw setpoints.
            if (!Global.IsOffline && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                iterationGroups.RemoveAt(0);
            }


            foreach (Group group in iterationGroups)
            {
                //if (device_type == ACB_PXR35_DEVICE && (@group.subgroups[1].ID == "59"))
                //{
                //    continue;
                //}
                if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && !Global.IsOffline)
                {
                    defaultcount = @group.ID == "1" ? 1 : 0;
                }

                var groupIterator = Global.IterationList.Where(x => x.GroupID == group.ID).ToList();

                for (int i = defaultcount; i < groupIterator.Count; i++)
                {

                    if (setPointIndex <= iterationCount && setPointIndex < TripUnit.rawSetPoints.Count)
                    {
                        setting = groupIterator[i];



                        //For ACB3.0, need to skip gen setpoints
                        //if (group.ID == "0" && setting.ID.StartsWith("GEN") && !Global.IsOffline && Global.device_type == Global.ACB_03_00_XX_DEVICE)
                        //    continue;


                        //  rawSetting = isMCCBRead ? grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence && setting.visible)?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];
                        rawSetting = (isNZMRead || isMCCBRead || isACB3_0Read) ? grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence && (setting.visible || isACB3_0Read))?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];   //#COVARITY FIX   235049
                        if ((isMCCBRead || isNZMRead || isACB3_0Read || isACBRead) && (rawSetting == null/* || setting.ID == "SYS4B"*/ || setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"
                            || setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A" ||
                            setting.ID == "SYS4AT" || setting.ID == "SYS4CT" || setting.ID == "SYS4DT" || setting.ID == "SYS4ET" || setting.ID == "SYS4FT" ||
                            setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT" || setting.ID == "GC00112A" || setting.ID == "GC00112B" || setting.ID == "GC00112C" ||
    setting.ID == "GC00112D" || setting.ID == "GC00112E" || setting.ID == "GC00112F" || setting.ID == "GC00112G" || setting.ID == "GC00112H"/*|| setting.ID == "SYS152"*/ || setting.ID == "CC012A" || setting.ID == "CC012B" || setting.ID == "CC012C" || setting.ID == "CC012D" || setting.ID == "CC012E"
    || setting.ID == "CC016A" || setting.ID == "CC016B" || setting.ID == "CC016C" || setting.ID == "CC016D" || setting.ID == "CC016E"))
                        {

                            continue;
                        }
                        else if (setting.ID == "SYS131C" || setting.ID == "SYS141C" || setting.ID == "SYS151C")
                        {
                            continue;
                        }
                        if (/*isACBRead*/!IsOffline && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"|| setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A"))
                        {
                            continue;
                        }
                        if (Global.selectedTemplateType == Global.ACBTEMPLATE && !(Global.IsOpenFile || Global.isExportControlFlow) && (setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT"))
                        {
                            continue;
                        }
                        if (setting.commitedChange)
                        {
                            continue;
                        }
                        //added by srk to fix PXPM-6171
                        if (!IsOffline && (Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE))
                        {
                            if (Global.device_type_PXR25 && (setting.ID == "CPC007" || setting.ID == "CPC008" || setting.ID == "CPC012" ||
                                setting.ID == "CPC013" || setting.ID == "CPC014" || setting.ID == "CPC018" || setting.ID == "CPC019" ||
                                setting.ID == "CPC025" || setting.ID == "CPC026" || setting.ID == "CPC027" || setting.ID == "SYS024"))
                            {
                                continue;
                            }
                            else if (Global.device_type_PXR20 && (setting.ID == "CPC007A" || setting.ID == "CPC008A" || setting.ID == "CPC012A" ||
                               setting.ID == "CPC013A" || setting.ID == "CPC014A" || setting.ID == "CPC018A" || setting.ID == "CPC019A" ||
                               setting.ID == "CPC025" || setting.ID == "CPC026" || setting.ID == "CPC027" || setting.ID == "SYS024"))
                            {
                                continue;
                            }
                            else if (Global.device_type_PXR25 && (setting.ID == "RPC19" || setting.ID == "RPC20"))
                            {
                                setPointIndex++;
                                continue;
                            }
                        }
                        //Added by Nishant to Keep the Logic of PTM Seperate
                        if (!IsOffline && Global.selectedTemplateType == Global.PTM_TEMPLATE)
                        {
                            if ((Global.device_type_PXR20 || Global.device_type_PXR25) && (setting.ID == "CPC007" || setting.ID == "CPC008" || setting.ID == "CPC012" ||
                                setting.ID == "CPC013" || setting.ID == "CPC014" || setting.ID == "CPC018" || setting.ID == "CPC019" ||
                                setting.ID == "CPC025" || setting.ID == "CPC026" || setting.ID == "CPC027"))
                            {
                                continue;
                            }
                            else if (Global.device_type_PXR10 && (setting.ID == "CPC007A" || setting.ID == "CPC008A" || setting.ID == "CPC012A" ||
                               setting.ID == "CPC013A" || setting.ID == "CPC014A" || setting.ID == "CPC018A" || setting.ID == "CPC019A" ||
                               setting.ID == "CPC025" || setting.ID == "CPC026" || setting.ID == "CPC027"))
                            {
                                continue;
                            }
                            else if (Global.device_type_PXR25 && (setting.ID == "RPC19" || setting.ID == "RPC20"))
                            {
                                setPointIndex++;
                                continue;
                            }

                        }                      

                        if (!IsOffline && Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
                        {
                            if (setting.ID != "SYS001A" && setting.TripUnitSequence == 0)
                            {
                                //                                CPC025 Instantaneous Protection
                                //CPC039  Thermal Memory
                                //CPC031 Neutral Alarm}
                                continue;
                            }
                        }
                        if (rawSetting.Length != 4)
                        {
                            switch (rawSetting.Length)
                            {
                                case 1:
                                    rawSetting = "000" + rawSetting;
                                    break;
                                case 2:
                                    rawSetting = "00" + rawSetting;
                                    break;
                                case 3:
                                    rawSetting = "0" + rawSetting;
                                    break;
                                default:
                                    break;
                            }
                        }


                        if (setting.type == Settings.Type.type_number)
                        {

                            setting.numberDefault = Convert.ToDouble(convertHexToNum(rawSetting, setting.conversion, ref setting.numberValue, setting.conversionOperation), CultureInfo.CurrentUICulture);
                            setting.numberValue = setting.numberDefault;

                            if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && (setFor == str_app_ID_Table))
                            {
                                Settings setpoint = new Settings();
                                //Instantaneous pickup is 0 then Instantaneous Protection should be marked as Disabled
                                if (setting.ID == "CPC014A" || setting.ID == "CPC0101")
                                {
                                    setpoint = TripUnit.getInstantaneousProtection();
                                }
                                //If Thermal memory alarm is 0 then Thermal memory toggle should be marked as Disabled
                                if (setting.ID == "CPC024" && device_type == ACB_PXR35_DEVICE)
                                {
                                    setpoint = TripUnit.getThermalMemoryToggle();
                                }
                                //Manage Higload1 toggle
                                else if (setting.ID == "CPC010")
                                {
                                    setpoint = TripUnit.getHighLoad1toggle();
                                }
                                //Manage Highload2 toggle
                                else if (setting.ID == "CPC021")
                                {
                                    setpoint = TripUnit.getHighLoad2toggle();
                                }
                                else if (setting.ID == "SYS020")
                                {
                                    setpoint = TripUnit.getBreakerHealth();
                                }
                                else if (setting.ID == "CPC0126")
                                {
                                    setpoint = TripUnit.getCurrentTHDAlarmStatus();
                                }
                                else if (setting.ID == "CPC0129")
                                {
                                    setpoint = TripUnit.getVoltageTHDAlarmStatus();
                                }
                                else if (setting.ID == "CPC0132")
                                {
                                    setpoint = TripUnit.getOperationCountStatus();
                                }
                                else if (setting.ID == "CPC0134")
                                {
                                    setpoint = TripUnit.getHealthMaintenanceStatus();
                                }
                                else if (setting.ID == "SYS10B")
                                {
                                    setpoint = TripUnit.getPowerDemandIntervalStatus();
                                }
                                else if (setting.ID == "CPC032")
                                {
                                    setpoint = TripUnit.getNeutralAlarm();
                                }
                                else if (setting.ID == "ADVA012")
                                {
                                    setpoint = TripUnit.getLoadVoltageDecayStatus();
                                }
                                else if (setting.ID == "ADVA023")
                                {
                                    setpoint = TripUnit.getTDEmergDisconStatus();
                                }
                                else if (setting.ID == "ADVA026")
                                {
                                    setpoint = TripUnit.getTDNormDisconStatus();
                                }
                                else if (setting.ID == "ADVA030")
                                {
                                    setpoint = TripUnit.getOVDropoutStatus();
                                }
                                else if (setting.ID == "ADVA031")
                                {
                                    setpoint = TripUnit.getOVPickupStatus();
                                }
                                else if (setting.ID == "ADVA034")
                                {
                                    setpoint = TripUnit.getOFDropoutStatus();
                                }
                                else if (setting.ID == "ADVA035")
                                {
                                    setpoint = TripUnit.getOFPickupStatus();
                                }
                                else if (setting.ID == "ADVA036")
                                {
                                    setpoint = TripUnit.getUFDropoutStatus();
                                }
                                else if (setting.ID == "ADVA037")
                                {
                                    setpoint = TripUnit.getUFPickupStatus();
                                }
                                else if (setting.ID == "ADVA039")
                                {
                                    setpoint = TripUnit.getVoltageDropoutStatus();
                                }
                                else if (setting.ID == "ADVA041")
                                {
                                    setpoint = TripUnit.getVoltagePickupStatus();
                                }

                                if (setting.ID == "CPC014A" || setting.ID == "CPC010" || setting.ID == "CPC021" || setting.ID == "CPC0101" || setting.ID == "CPC024" || setting.ID == "CPC0126" || setting.ID == "CPC0129" || setting.ID == "CPC0132" || setting.ID == "CPC0134" || setting.ID == "SYS10B"
                                    || setting.ID == "CPC032" || setting.ID == "ADVA012" || setting.ID == "ADVA023"
                                    || setting.ID == "ADVA026" || setting.ID == "ADVA030" || setting.ID == "ADVA031"
                                    || setting.ID == "ADVA034" || setting.ID == "ADVA035" || setting.ID == "ADVA036"
                                    || setting.ID == "ADVA037" || setting.ID == "ADVA039" || setting.ID == "ADVA041")

                                {
                                    if (setting.ID == "ADVA026" || setting.ID == "ADVA023")
                                    {
                                        if (setting.numberValue == 65535)
                                        {
                                            setpoint.bValue = false;
                                            setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                        }
                                        else
                                        {
                                            setpoint.bValue = true;
                                            setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                        }
                                    }
                                    else if (setting.numberValue == 0)
                                    {
                                        setpoint.bValue = false;
                                        setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                    }
                                    else
                                    {
                                        setpoint.bValue = true;
                                        setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                    }
                                    setpoint.notifyDependents();
                                }
                                else if (setting.ID == "SYS020")
                                {
                                    if (setting.numberValue == 127)
                                    {
                                        setpoint.bValue = false;
                                        setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                    }
                                    else
                                    {
                                        setpoint.bValue = true;
                                        setpoint.bValueReadFromTripUnit = setpoint.bValue;
                                    }
                                    setpoint.notifyDependents();
                                }

                                if (setting.ID == "GC00112")
                                {
                                    if (!Global.IsOpenFile)
                                    {
                                        TripUnit.GC8bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 8), 2).PadLeft(4, '0')));
                                    }
                                    if (TripUnit.GC8bitString != null)
                                    {
                                        //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                        //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                        //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                        //b8: ARMs mode enable / disable channel 
                                        //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)


                                        TripUnit.GC_b0 = TripUnit.GC8bitString[15];
                                        TripUnit.GC_b1 = TripUnit.GC8bitString[14];
                                        TripUnit.GC_b2 = TripUnit.GC8bitString[13];
                                        TripUnit.GC_b3 = TripUnit.GC8bitString[12];
                                        TripUnit.GC_b4 = TripUnit.GC8bitString[11];
                                        TripUnit.GC_b5 = TripUnit.GC8bitString[10];
                                        TripUnit.GC_b6 = TripUnit.GC8bitString[9];
                                        TripUnit.GC_b7 = TripUnit.GC8bitString[8];
                                    }
                                    TripUnit.GCforExport = Convert.ToInt32(TripUnit.GC8bitString, 2).ToString("X");

                                    while (TripUnit.GCforExport.Length < ("0000").Length)
                                    {
                                        TripUnit.GCforExport = "0" + TripUnit.GCforExport;
                                    }

                                }
                                if (setting.ID == "CC012")
                                {
                                    if (!Global.IsOpenFile)
                                    {
                                        TripUnit.RTU8bitString = Convert.ToString(Convert.ToInt32(rawSetting, 16), 2).PadLeft(8, '0');
                                    }
                                    if (TripUnit.RTU8bitString != null)
                                    {
                                        //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                        //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                        //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                        //b8: ARMs mode enable / disable channel 
                                        //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)

                                        TripUnit.RTU_b4 = TripUnit.RTU8bitString[3];
                                        TripUnit.RTU_b3 = TripUnit.RTU8bitString[4];
                                        TripUnit.RTU_b2 = TripUnit.RTU8bitString[5];
                                        TripUnit.RTU_b1 = TripUnit.RTU8bitString[6];
                                        TripUnit.RTU_b0 = TripUnit.RTU8bitString[7];

                                        TripUnit.RTU_Backup_b0 = TripUnit.RTU_b0;
                                        TripUnit.RTU_Backup_b1 = TripUnit.RTU_b1;
                                        TripUnit.RTU_Backup_b2 = TripUnit.RTU_b2;
                                        TripUnit.RTU_Backup_b3 = TripUnit.RTU_b3;
                                        TripUnit.RTU_Backup_b4 = TripUnit.RTU_b4;

                                    }
                                    TripUnit.RTUforExport = Convert.ToInt32(TripUnit.RTU8bitString, 2).ToString("X");

                                    while (TripUnit.RTUforExport.Length < ("0000").Length)
                                    {
                                        TripUnit.RTUforExport = "0" + TripUnit.RTUforExport;
                                    }
                                    TripUnit.RTUforBackup = string.Format("000{0}{1}{2}{3}{4}", TripUnit.RTU_b4, TripUnit.RTU_b3, TripUnit.RTU_b2, TripUnit.RTU_b1, TripUnit.RTU_b0);
                                    if (IsUndoLock)
                                    {
                                        Settings PDMode_setting = TripUnit.getModbusSetptChnge();
                                        PDMode_setting.bValue = TripUnit.RTU_b4 == '0' ? false : true;
                                        PDMode_setting.bDefault = PDMode_setting.bValue;
                                        PDMode_setting.bValueReadFromTripUnit = PDMode_setting.bValue;

                                        Settings PDMode_setting1 = TripUnit.getModbusARMS();
                                        PDMode_setting1.bValue = TripUnit.RTU_b3 == '0' ? false : true;
                                        PDMode_setting1.bDefault = PDMode_setting1.bValue;
                                        PDMode_setting1.bValueReadFromTripUnit = PDMode_setting1.bValue;

                                        Settings PDMode_setting2 = TripUnit.getModbusClose();
                                        PDMode_setting2.bValue = TripUnit.RTU_b2 == '0' ? false : true;
                                        PDMode_setting2.bDefault = PDMode_setting2.bValue;
                                        PDMode_setting2.bValueReadFromTripUnit = PDMode_setting2.bValue;

                                        Settings PDMode_setting3 = TripUnit.getModbusOpen();
                                        PDMode_setting3.bValue = TripUnit.RTU_b1 == '0' ? false : true;
                                        PDMode_setting3.bDefault = PDMode_setting3.bValue;
                                        PDMode_setting3.bValueReadFromTripUnit = PDMode_setting3.bValue;

                                        Settings PDMode_setting4 = TripUnit.getModbusEnB();
                                        PDMode_setting4.bValue = TripUnit.RTU_b0 == '0' ? false : true;
                                        PDMode_setting4.bDefault = PDMode_setting4.bValue;
                                        PDMode_setting4.bValueReadFromTripUnit = PDMode_setting4.bValue;
                                    }

                                }

                                if (setting.ID == "CC016")
                                {
                                    if (!Global.IsOpenFile)
                                    {
                                        TripUnit.TCP8bitString = Convert.ToString(Convert.ToInt32(rawSetting, 16), 2).PadLeft(8, '0');
                                    }
                                    if (TripUnit.TCP8bitString != null)
                                    {
                                        //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                        //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                        //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                        //b8: ARMs mode enable / disable channel 
                                        //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)

                                        TripUnit.TCP_b4 = TripUnit.TCP8bitString[3];
                                        TripUnit.TCP_b3 = TripUnit.TCP8bitString[4];
                                        TripUnit.TCP_b2 = TripUnit.TCP8bitString[5];
                                        TripUnit.TCP_b1 = TripUnit.TCP8bitString[6];
                                        TripUnit.TCP_b0 = TripUnit.TCP8bitString[7];

                                        TripUnit.TCP_Backup_b0 = TripUnit.TCP_b0;
                                        TripUnit.TCP_Backup_b1 = TripUnit.TCP_b1;
                                        TripUnit.TCP_Backup_b2 = TripUnit.TCP_b2;
                                        TripUnit.TCP_Backup_b3 = TripUnit.TCP_b3;
                                        TripUnit.TCP_Backup_b4 = TripUnit.TCP_b4;
                                    }
                                    TripUnit.TCPforExport = Convert.ToInt32(TripUnit.TCP8bitString, 2).ToString("X");

                                    while (TripUnit.TCPforExport.Length < ("0000").Length)
                                    {
                                        TripUnit.TCPforExport = "0" + TripUnit.TCPforExport;
                                    }
                                    TripUnit.TCPforBackup = string.Format("000{0}{1}{2}{3}{4}", TripUnit.TCP_Backup_b4, TripUnit.TCP_Backup_b3, TripUnit.TCP_Backup_b2, TripUnit.TCP_Backup_b1, TripUnit.TCP_Backup_b0);

                                    if (IsUndoLock)
                                    {
                                        Settings PDMode_setting = TripUnit.getModbusSetptChngeTCP();
                                        PDMode_setting.bValue = TripUnit.TCP_b4 == '0' ? false : true;
                                        PDMode_setting.bDefault = PDMode_setting.bValue;
                                        PDMode_setting.bValueReadFromTripUnit = PDMode_setting.bValue;

                                        Settings PDMode_setting1 = TripUnit.getModbusARMSTCP();
                                        PDMode_setting1.bValue = TripUnit.TCP_b3 == '0' ? false : true;
                                        PDMode_setting1.bDefault = PDMode_setting1.bValue;
                                        PDMode_setting1.bValueReadFromTripUnit = PDMode_setting1.bValue;

                                        Settings PDMode_setting2 = TripUnit.getModbusCloseTCP();
                                        PDMode_setting2.bValue = TripUnit.TCP_b2 == '0' ? false : true;
                                        PDMode_setting2.bDefault = PDMode_setting2.bValue;
                                        PDMode_setting2.bValueReadFromTripUnit = PDMode_setting2.bValue;

                                        Settings PDMode_setting3 = TripUnit.getModbusOpenTCP();
                                        PDMode_setting3.bValue = TripUnit.TCP_b1 == '0' ? false : true;
                                        PDMode_setting3.bDefault = PDMode_setting3.bValue;
                                        PDMode_setting3.bValueReadFromTripUnit = PDMode_setting3.bValue;

                                        Settings PDMode_setting4 = TripUnit.getModbusEnBTCP();
                                        PDMode_setting4.bValue = TripUnit.TCP_b0 == '0' ? false : true;
                                        PDMode_setting4.bDefault = PDMode_setting4.bValue;
                                        PDMode_setting4.bValueReadFromTripUnit = PDMode_setting4.bValue;
                                    }

                                }
                                if (setting.ID == "SYS15")
                                {
                                    if (!Global.IsOpenFile)
                                    {
                                        TripUnit.PD8bitString = Convert.ToString(Convert.ToInt32(rawSetting, 16), 2).PadLeft(8, '0');
                                        // TripUnit.PD8bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 8), 2).PadLeft(4, '0')));
                                    }
                                    if (TripUnit.PD8bitString != null)
                                    {

                                        TripUnit.PD_b0 = TripUnit.PD8bitString[7];
                                        TripUnit.PD_b1 = TripUnit.PD8bitString[6];
                                        TripUnit.PD_b2 = TripUnit.PD8bitString[5];
                                        TripUnit.PD_b3 = TripUnit.PD8bitString[4];
                                        TripUnit.PD_b4 = TripUnit.PD8bitString[3];
                                        TripUnit.PD_b5 = TripUnit.PD8bitString[2];
                                        TripUnit.PD_b6 = TripUnit.PD8bitString[1];
                                        TripUnit.PD_b7 = TripUnit.PD8bitString[0];

                                        //for Backup
                                        TripUnit.PD_Backup_b0 = TripUnit.PD_b0;
                                        TripUnit.PD_Backup_b4 = TripUnit.PD_b4;
                                        TripUnit.PD_Backup_b5 = TripUnit.PD_b5;
                                    }
                                    TripUnit.PDforExport = Convert.ToInt32(TripUnit.PD8bitString, 2).ToString("X");

                                    while (TripUnit.PDforExport.Length < ("0000").Length)
                                    {
                                        TripUnit.PDforExport = "0" + TripUnit.PDforExport;
                                    }
                                    TripUnit.PDforBackup = TripUnit.PDforExport;

                                    Settings PDMode_setting = TripUnit.getPowerDemandMode();
                                    PDMode_setting.bValue = TripUnit.PD_b0 == '0' ? false : true;
                                    PDMode_setting.bDefault = PDMode_setting.bValue;
                                    PDMode_setting.bValueReadFromTripUnit = PDMode_setting.bValue;


                                    Settings PDModePrecision_setting = TripUnit.getPowerDemandPrecision();
                                    string SYS14Value = String.Concat(TripUnit.PD_b5, TripUnit.PD_b4);
                                    switch (SYS14Value)
                                    { /*
                                           *   00 = Precise, 01 = Normal, 10 = Relaxed, 11 = Not used
                                           * */
                                        case "00":
                                            PDModePrecision_setting.selectionIndex = 0;
                                            PDModePrecision_setting.selectionValue = Resource.SYS14Item0000;
                                            PDModePrecision_setting.defaultSelectionValue = Resource.SYS14Item0000;
                                            break;
                                        case "01":
                                            PDModePrecision_setting.selectionIndex = 1;
                                            PDModePrecision_setting.selectionValue = Resource.SYS14Item0001;
                                            PDModePrecision_setting.defaultSelectionValue = Resource.SYS14Item0001;
                                            break;
                                        case "10":
                                            PDModePrecision_setting.selectionIndex = 2;
                                            PDModePrecision_setting.selectionValue = Resource.SYS14Item0010;
                                            PDModePrecision_setting.defaultSelectionValue = Resource.SYS14Item0010;
                                            break;
                                    }

                                }
                            }
                        }


                        if (setting.type == Settings.Type.type_text)
                        {
                            if (setting.ID == ipControl1)
                            {
                                string str = string.Empty;
                                for (int ip1 = 0; ip1 <= 3; ip1++)
                                {
                                    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                    if (str == string.Empty)
                                    {
                                        str = convertHexToString(rawSetting, setting.conversion); ;
                                    }
                                    else
                                    {
                                        str = str + "." + convertHexToString(rawSetting, setting.conversion);
                                    }

                                    setPointIndex++;
                                }
                                setting.IPaddress = str;
                                continue;
                            }
                            else if (setting.ID == ipControl2)
                            {
                                rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                string str = convertHexToString(rawSetting, setting.conversion);
                                setting.textvalue = Convert.ToDouble(str);
                                setting.defaultextvalue = Convert.ToDouble(str);
                                setting.IPaddress = ipMask + str;
                                setting.IPaddress_default = ipMask + str;
                            }
                            else if (setting.ID == ipControl3)
                            {
                                string str = string.Empty;
                                // setting.IPaddress = (setpointIndex[setPointCounter]).ToString();
                                // var valueseperator = setting.IPaddress.Split(',');
                                for (var ip3 = 0; ip3 <= 1; ip3++)
                                {
                                    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                    if (str == string.Empty)
                                    {
                                        str = convertHexToString(rawSetting, setting.conversion); ;
                                    }
                                    else
                                    {
                                        str = str + "." + convertHexToString(rawSetting, setting.conversion);
                                    }
                                    setPointIndex++;
                                }
                                Settings ipAddress1 = IP001Obj;

                                String[] ip = ipAddress1.IPaddress.Split('.');
                                setting.IPaddress = ip[0] + "." + ip[1] + "." + str;
                                continue;
                            }
                            else
                            {
                                // string str = string.Empty;            //#COVARITY FIX     234901
                                rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                if (setting.value_map.Count > 0 && setting.reversevalue_map.Contains(rawSetting))
                                {
                                    setting.textvalue = Convert.ToDouble(setting.reversevalue_map[rawSetting]);
                                    setting.defaultextvalue = setting.textvalue;
                                }
                                else
                                {
                                    string str = convertHexToString(rawSetting, setting.conversion);
                                    setting.textvalue = Convert.ToDouble(str);
                                    setting.defaultextvalue = Convert.ToDouble(str);
                                }
                            }
                        }

                        else if (setting.type == Settings.Type.type_selection)
                        {
                            if (!IsOffline && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (setting.ID == "SYS005" || setting.ID == "SYS005A" || setting.ID == "SYS005B"))
                            {
                                if (setting.lookupTable.Contains(rawSetting))
                                {
                                    setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                    setting.defaultSelectionValue = setting.selectionValue;
                                }
                                if (setting.ID == "SYS005B") setPointIndex++;

                                continue;
                            }

                            if (setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015")
                            {

                                while (rawSetting.Length < ("0000").Length)
                                {
                                    rawSetting = "0" + rawSetting;
                                }

                                String[] parts = { "00" + rawSetting.Substring(0, 2), "00" + rawSetting.Substring(2) };

                                if (setting.lookupTable.Contains(parts[1]))
                                {
                                    setting.selectionValue = ((item_ComboBox)setting.lookupTable[parts[1]]).item;
                                    setting.defaultSelectionValue = setting.selectionValue;
                                    rawSetting = parts[1];

                                }
                                switch (setting.ID)
                                {
                                    case "SYS013":
                                        TripUnit.relay1 = parts[0];
                                        break;

                                    case "SYS014":
                                        TripUnit.relay2 = parts[0];
                                        break;

                                    case "SYS015":
                                        TripUnit.relay3 = parts[0];
                                        break;
                                }

                            }
                            if ((setting.ID == "SYS013A" || setting.ID == "SYS014A" || setting.ID == "SYS015A") && isACB3_0Read)
                            {
                                switch (setting.ID)
                                {
                                    case "SYS013A":

                                        if (setting.lookupTable.Contains(TripUnit.relay1))
                                        {
                                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[TripUnit.relay1]).item;
                                            setting.defaultSelectionValue = setting.selectionValue;
                                            rawSetting = TripUnit.relay1;
                                        }
                                        break;

                                    case "SYS014A":
                                        if (setting.lookupTable.Contains(TripUnit.relay2))
                                        {
                                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[TripUnit.relay2]).item;
                                            setting.defaultSelectionValue = setting.selectionValue;
                                            rawSetting = TripUnit.relay2;
                                        }
                                        break;

                                    case "SYS015A":
                                        if (setting.lookupTable.Contains(TripUnit.relay3))
                                        {
                                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[TripUnit.relay3]).item;
                                            setting.defaultSelectionValue = setting.selectionValue;
                                            rawSetting = TripUnit.relay3;
                                        }
                                        break;
                                }
                                //continue;
                            }
                            if (/*(setting.ID != "SYS004A") && (setting.ID != "SYS4A") && (setting.ID != "SYS4B") && (setting.ID != "SYS004B") && */(setting.ID != "SYS002"))

                            {
                                if (!setting.visible && !setting.parseInPXPM && /*!setting.parseForACB_2_1_XX &&*/ setting.GroupID != "0" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.device_type == Global.PTM_DEVICE || Global.selectedTemplateType == Global.PTM_TEMPLATE))
                                {
                                    if (setting.lookupTable.Contains(rawSetting))
                                    {
                                        setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                        setting.defaultSelectionValue = setting.selectionValue;

                                    }
                                    else
                                    {
                                        var value = (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) ? rawSetting : TripUnit.rawSetPoints[setPointIndex - 1];
                                        if (setting.lookupTable.Contains(value))
                                        {
                                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[value]).item;
                                            setting.defaultSelectionValue = setting.selectionValue;

                                        }
                                    }
                                    setPointIndex++;
                                    continue;
                                }
                                if (setting.lookupTable.Contains(rawSetting))
                                {
                                    setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;

                                    if (setting.indexesWithHexValuesMapping != null && setting.indexesWithHexValuesMapping.Count > 0 && setting.reverseLookupTable.Count != setting.lookupTable.Count)
                                    {
                                        // selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                        setting.selectionValue = setting.indexesWithHexValuesMapping.FirstOrDefault(x => x.Key == TripUnit.tripUnitIndexArray[setPointIndex].ToString()).Value;
                                    }

                                    setting.defaultSelectionValue = setting.selectionValue;

                                    //Removed changes form the release 22.9.1 
                                    //string setpointValue = (setting.reverseLookupTable[setting.selectionValue]).ToString();
                                    //setting.selectionDefault = (setting.reversevalue_map[setpointValue]).ToString();                                  

                                    if (setting.ID == "SYS132" || setting.ID == "SYS142" || setting.ID == "SYS152" ||
                                        setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015" ||
                                        setting.ID == "SYS013A" || setting.ID == "SYS014A" || setting.ID == "SYS015A")
                                    {
                                        if (setting.selectionValue == Resource.SYS131Default)
                                        {
                                            if (setting.ID == "SYS132" || setting.ID == "SYS013")
                                            {
                                                ((Settings)TripUnit.getRelay1(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay1(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }
                                            else if (setting.ID == "SYS142" || setting.ID == "SYS014")
                                            {
                                                ((Settings)TripUnit.getRelay2(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay2(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }
                                            else if (setting.ID == "SYS152" || setting.ID == "SYS015")
                                            {
                                                ((Settings)TripUnit.getRelay3(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay3(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }

                                            else if (setting.ID == "SYS013A")
                                            {
                                                ((Settings)TripUnit.getRelay1A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay1A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }
                                            else if (setting.ID == "SYS014A")
                                            {
                                                ((Settings)TripUnit.getRelay2A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay2A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }
                                            else if (setting.ID == "SYS015A")
                                            {
                                                ((Settings)TripUnit.getRelay3A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionValue = Resource.SYS131Default;
                                                ((Settings)TripUnit.getRelay3A(Pxset1IDTableFlag, Pxset2IDTableFlag)).selectionDefault = "0";
                                            }
                                        }

                                        // fix done for bakup file export to unit when relay is set to off from device side 
                                        //   setting.selectionIndex = Convert.ToInt32((setting.reverseLookupTable[setting.selectionValue]).ToString(), 16);

                                        //condition for setFor is added by PP to fix PXPM-6087
                                        //Relay setting related changes are implemented using IDTable and not using Pxset1IDTable, so we can skip this code for additional curve

                                        if ((Global.isExportControlFlow || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && setting.selectionValue != Resource.Off
                                            && setFor == str_app_ID_Table)
                                        {// while exporting file backup file dont have information about relay SYS131, SYS141 and SYS151 settings so based on the actual relay steeing updating  values for same 
                                         //"0000":"0","0001":"1","0002":"2","0003":"3","0004":"4","0005":"5","0006":"6","0007":"7","0008":"8" "0009":"30","000A":"31","000B":"32","000C":"33","000D":"34","000E":"35","000F":"36"
                                         //    ,"0040":"9","0041":"10","0042":"11","0043":"12","0044":"13","0045":"14","0046":"15","0047":"16","0048":"17","0049":"18","004A":"19","004B":"20",,"004C":"37","004D":"38","004E":"39","004F":"40","0050":"41","0051":"42","0052":"43"
                                         //"0020":"21","0021":"22","0022":"23","0023":"24","0024":"25","0025":"26","0026":"27","0027":"28","0028":"29",


                                            string[] Relay1 = new string[15] { "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "0009", "000A", "000B", "000C", "000D", "000E", "000F" };
                                            string[] Relay2 = new string[19] { "0040", "0041", "0042", "0043", "0044", "0045", "0046", "0047", "0048", "0049", "004A", "004B", "004C", "004D", "004E", "004F", "0050", "0051", "0052" };
                                            string[] Relay3 = new string[9] { "0020", "0021", "0022", "0023", "0024", "0025", "0026", "0027", "0028" };
                                            string[] Relay4;
                                            if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
                                            {
                                                /*
                                                 * Trip - 1-8, 30
                                                    Alarm - 9 -20
                                                    Other - 21 -29, 38   
                                                    Other Protection - 31-37
                                                 */
                                                Relay1 = new string[10] { "0000", "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "001E", }; //Trip
                                                Relay2 = new string[12] { "0009", "000A", "000B", "000C", "000D", "000E", "000F", "0010", "0011", "0012", "0013", "0014" };//Alarm
                                                Relay3 = new string[10] { "0015", "0016", "0017", "0018", "0019", "001A", "001B", "001C", "001D", "0026" }; //Other
                                                Relay4 = new string[7] { "001F", "0020", "0021", "0022", "0023", "0024", "0025" };//Other Protection

                                            }

                                            if (((Settings)TripUnit.IDTable["SYS131"]).selectionValue == Resource.Off && (setting.ID == "SYS132" || setting.ID == "SYS013"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS131"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS131"]).selectionIndex == 1 ? "0001" :
                                                                          (((Settings)TripUnit.IDTable["SYS131"]).selectionIndex == 2 ? "0002" :
                                                                           (((Settings)TripUnit.IDTable["SYS131"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS131"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS131"]).lookupTable[lookupTableIndex]).item;
                                            }
                                            else if (((Settings)TripUnit.IDTable["SYS141"]).selectionValue == Resource.Off && (setting.ID == "SYS142" || setting.ID == "SYS014"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS141"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS141"]).selectionIndex == 1 ? "0001" :
                                                                          (((Settings)TripUnit.IDTable["SYS141"]).selectionIndex == 2 ? "0002" :
                                                                          (((Settings)TripUnit.IDTable["SYS141"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS141"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS141"]).lookupTable[lookupTableIndex]).item;
                                            }
                                            else if (((Settings)TripUnit.IDTable["SYS151"]).selectionValue == Resource.Off && (setting.ID == "SYS152" || setting.ID == "SYS015"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS151"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS151"]).selectionIndex == 1 ? "0001" :
                                                                          (((Settings)TripUnit.IDTable["SYS151"]).selectionIndex == 2 ? "0002" :
                                                                          (((Settings)TripUnit.IDTable["SYS151"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS151"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS151"]).lookupTable[lookupTableIndex]).item;
                                            }
                                            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && ((Settings)TripUnit.IDTable["SYS131A"]).selectionValue == Resource.Off && (setting.ID == "SYS013A"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS131A"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS131A"]).selectionIndex == 1 ? "0001" :
                                                                          (((Settings)TripUnit.IDTable["SYS131A"]).selectionIndex == 2 ? "0002" :
                                                                           (((Settings)TripUnit.IDTable["SYS131A"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS131A"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS131A"]).lookupTable[lookupTableIndex]).item;
                                            }
                                            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && ((Settings)TripUnit.IDTable["SYS141A"]).selectionValue == Resource.Off && (setting.ID == "SYS014A"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS141A"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS141A"]).selectionIndex == 1 ? "0001" :
                                                                                                          (((Settings)TripUnit.IDTable["SYS141A"]).selectionIndex == 2 ? "0002" :
                                                                                                          (((Settings)TripUnit.IDTable["SYS141A"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS141A"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS141A"]).lookupTable[lookupTableIndex]).item;
                                            }
                                            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && ((Settings)TripUnit.IDTable["SYS151A"]).selectionValue == Resource.Off && (setting.ID == "SYS015A"))
                                            {
                                                ((Settings)TripUnit.IDTable["SYS151A"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS151A"]).selectionIndex == 1 ? "0001" :
                                                                                                          (((Settings)TripUnit.IDTable["SYS151A"]).selectionIndex == 2 ? "0002" :
                                                                                                          (((Settings)TripUnit.IDTable["SYS151A"]).selectionIndex == 3 ? "0003" : "0004"));
                                                ((Settings)TripUnit.IDTable["SYS151A"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS151A"]).lookupTable[lookupTableIndex]).item;
                                            }

                                        }

                                        //Added by Nishant to keep PTM Logic Seperate
                                        if (Global.device_type == Global.PTM_DEVICE)
                                        {
                                            if (setting.selectionValue != Resource.Off)
                                            {// while exporting file backup file dont have information about relay SYS131, SYS141 and SYS151 settings so based on the actual relay steeing updating  values for same 
                                             //"0000":"0","0001":"1","0002":"2","0003":"3","0004":"4","0005":"5","0006":"6","0007":"7","0008":"8" "0009":"30","000A":"31","000B":"32","000C":"33","000D":"34","000E":"35","000F":"36"
                                             //    ,"0040":"9","0041":"10","0042":"11","0043":"12","0044":"13","0045":"14","0046":"15","0047":"16","0048":"17","0049":"18","004A":"19","004B":"20",,"004C":"37","004D":"38","004E":"39","004F":"40","0050":"41","0051":"42","0052":"43"
                                             //"0020":"21","0021":"22","0022":"23","0023":"24","0024":"25","0025":"26","0026":"27","0027":"28","0028":"29",




                                                string[] Relay1 = new string[20] { "0001", "0002", "0003", "0004", "0005", "0006", "0007", "000A", "001D", "0020", "0022", "0024", "0025", "0028", "002A", "002C", "002E", "0030", "0032", "0035"/*"0000", "0001", "0002", "0003", "0004", "0005", "0006", "0007", "0008", "001E", */}; //Trip
                                                string[] Relay2 = new string[25] { "0008", "0009", "000B", "000C", "000D", "000E", "000F", "0010", "0011", "0012", "0013", "0014", "001E", "001F", "0021", "0023", "0026", "0027", "0029", "002B", "002D", "002F", "0031", "0033", "0036" };//Alarm
                                                string[] Relay3 = new string[9] { "0015", "0016", "0017", "0018", "0019", "001A", "001B", "001C", "0034" }; //Other

                                                if (((Settings)TripUnit.IDTable["SYS131"]).selectionValue == Resource.Off && (setting.ID == "SYS132" || setting.ID == "SYS013"))
                                                {
                                                    ((Settings)TripUnit.IDTable["SYS131"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                    string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS131"]).selectionIndex == 1 ? "0001" :
                                                                              (((Settings)TripUnit.IDTable["SYS131"]).selectionIndex == 2 ? "0002" : "0003");
                                                    ((Settings)TripUnit.IDTable["SYS131"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS131"]).lookupTable[lookupTableIndex]).item;
                                                }
                                                else if (((Settings)TripUnit.IDTable["SYS141"]).selectionValue == Resource.Off && (setting.ID == "SYS142" || setting.ID == "SYS014"))
                                                {
                                                    ((Settings)TripUnit.IDTable["SYS141"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                    string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS141"]).selectionIndex == 1 ? "0001" :
                                                                              (((Settings)TripUnit.IDTable["SYS141"]).selectionIndex == 2 ? "0002" : "0003");
                                                    ((Settings)TripUnit.IDTable["SYS141"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS141"]).lookupTable[lookupTableIndex]).item;
                                                }
                                                else if (((Settings)TripUnit.IDTable["SYS151"]).selectionValue == Resource.Off && (setting.ID == "SYS152" || setting.ID == "SYS015"))
                                                {
                                                    ((Settings)TripUnit.IDTable["SYS151"]).selectionIndex = Relay1.Contains(rawSetting) ? 1 : (Relay2.Contains(rawSetting) ? 2 : (Relay3.Contains(rawSetting) ? 3 : 4));
                                                    string lookupTableIndex = ((Settings)TripUnit.IDTable["SYS151"]).selectionIndex == 1 ? "0001" :
                                                                              (((Settings)TripUnit.IDTable["SYS151"]).selectionIndex == 2 ? "0002" : "0003");
                                                    ((Settings)TripUnit.IDTable["SYS151"]).selectionValue = ((PXR.item_ComboBox)((Settings)TripUnit.IDTable["SYS151"]).lookupTable[lookupTableIndex]).item;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (setting.ID == "GEN002")
                                {
                                    Global.appFirmware = setting.selectionValue;
                                }
                                if (Global.device_type == Global.MCCBDEVICE && setting.ID == "GEN02A" && !(GEN01Obj.selectionValue == Resource.GEN01Item0000))
                                {
                                    Global.appFirmware = setting.selectionValue;
                                }
                                else if (setting.ID == "GEN02" && (GEN01Obj.selectionValue == Resource.GEN01Item0000))
                                {
                                    Global.appFirmware = setting.textStrValue;
                                }
                            }

                            if (setting.ID == "SYS002")
                            {
                                setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                setting.defaultSelectionValue = setting.selectionValue;
                            }


                            if (setting.lookupTable.Contains(rawSetting))
                            {
                                setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                if (setting.indexesWithHexValuesMapping != null && setting.indexesWithHexValuesMapping.Count > 0 && setting.reverseLookupTable.Count != setting.lookupTable.Count)
                                {
                                    // selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                    setting.selectionValue = setting.indexesWithHexValuesMapping.FirstOrDefault(x => x.Key == TripUnit.tripUnitIndexArray[setPointIndex].ToString()).Value;
                                }

                                setting.defaultSelectionValue = setting.selectionValue;
                            }

                        }
                        else if (setting.type == Settings.Type.type_bool)
                        {
                            // format is 0000 or 0001
                            setting.bValue = (rawSetting.Substring(3) == "1");
                            // Set defaults
                            //  setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                        }
                        else if (setting.type == Settings.Type.type_toggle)
                        {
                            if (setting.ID == "SYS004A" || setting.ID == "SYS4A")

                            {
                                #region SYS004 setpoint dependecny
                                TripUnit.MM16bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                                if (TripUnit.MM16bitString != null)
                                {
                                    //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                    //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                    //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                    //b8: ARMs mode enable / disable channel 
                                    //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)


                                    TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                                    TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                                    TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                                    TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                                    TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                                }

                                TripUnit.MMforExport = Convert.ToInt32(TripUnit.MM16bitString, 2).ToString("X");

                                while (TripUnit.MMforExport.Length < ("0000").Length)
                                {
                                    TripUnit.MMforExport = "0" + TripUnit.MMforExport;
                                }
                                //Added by Astha to update UI
                                //with respect to maintenance mode when saved file/Backup file is opened
                                if (Global.IsOpenFile || Global.isExportControlFlow)
                                {
                                    string valueForMaintenancemode = rawSetting.Substring(0, 2);
                                    valueForMaintenanceModeRemote = rawSetting.Substring(2);
                                    //for ACB template, 02 value also comes for maintennace mode state
                                    if (valueForMaintenanceModeRemote == "02")
                                    {
                                        valueForMaintenanceModeRemote = "00";
                                    }
                                    UpdateMMValuesForOpenFile(setting, valueForMaintenanceModeRemote);
                                    setPointIndex++;
                                    continue;
                                }

                                //a)	If b8 - ARMs mode status is 0, display “Off” in “Maintenance Mode State” field.
                                //b)	If b8 - ARMs mode status is 1, display “On” in “Maintenance Mode State” field.

                                setting.bValue = TripUnit.MM_b8 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;

                                setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints(Added SYS004A and SYS004B as a parent setpoint.Now there is no SYS004. ) .
                            }
                            #endregion

                            else if (setting.ID == "SYS004B" || setting.ID == "SYS004C" || setting.ID == "SYS004D" || setting.ID == "SYS004E" ||
                                setting.ID == "SYS4B" || setting.ID == "SYS4C" || setting.ID == "SYS4D" || setting.ID == "SYS4E" || setting.ID == "SYS4F")
                            {
                                //Added by Astha to update UI
                                //with respect to maintenance mode when saved file/Backup file is opened

                                string valueForMaintenancemode = rawSetting.Substring(0, 2);
                                valueForMaintenanceModeRemote = rawSetting.Substring(2);

                                if (Global.IsOpenFile || Global.isExportControlFlow)
                                {
                                    if (setting.ID == "SYS004B" || setting.ID == "SYS4B")
                                    {
                                        if (valueForMaintenanceModeRemote == "80" || valueForMaintenanceModeRemote == "82" ||
                                       valueForMaintenanceModeRemote == "02" || valueForMaintenanceModeRemote == "00")
                                        {
                                            valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "00" : "0000";
                                        }
                                        if (valueForMaintenanceModeRemote == "81" || valueForMaintenanceModeRemote == "03" ||
                                            valueForMaintenanceModeRemote == "83" || valueForMaintenanceModeRemote == "01")
                                        {
                                            valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "01" : "0001";
                                        }
                                    }

                                    UpdateMMValuesForOpenFile(setting, valueForMaintenanceModeRemote);

                                    setPointIndex++;
                                    continue;
                                }

                                switch (setting.ID)

                                {
                                    //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                    //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                    //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                    case "SYS004B":
                                    case "SYS4B":
                                        setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "SYS004C":
                                    case "SYS4C":
                                        setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;

                                        setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                        break;

                                    case "SYS004D":
                                    case "SYS4D":
                                        setting.bValue = TripUnit.MM_b7 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;

                                        setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                        break;

                                    case "SYS004E":
                                    case "SYS4E":
                                        setting.bValue = TripUnit.MM_b1 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                        break;
                                    case "SYS004F":
                                    case "SYS4F":
                                        setting.bValue = TripUnit.MM_b2 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                        break;

                                }
                            }

                            else if (setting.ID == "GC00112A" || setting.ID == "GC00112B" || setting.ID == "GC00112C" ||
                                setting.ID == "GC00112D" || setting.ID == "GC00112E" || setting.ID == "GC00112F" || setting.ID == "GC00112G" || setting.ID == "GC00112H")
                            {
                                switch (setting.ID)

                                {
                                    //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                    //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                    //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                    case "GC00112A":
                                        setting.bValue = TripUnit.GC_b0 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112B":
                                        setting.bValue = TripUnit.GC_b1 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112C":
                                        setting.bValue = TripUnit.GC_b2 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112D":
                                        setting.bValue = TripUnit.GC_b3 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112E":
                                        setting.bValue = TripUnit.GC_b4 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112F":
                                        setting.bValue = TripUnit.GC_b5 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112G":
                                        setting.bValue = TripUnit.GC_b6 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                    case "GC00112H":
                                        setting.bValue = TripUnit.GC_b7 == '0' ? false : true;
                                        setting.bDefault = setting.bValue;
                                        setting.bValueReadFromTripUnit = setting.bValue;
                                        break;

                                }
                            }

                            else if (setting.ID == "CC012A" || setting.ID == "CC012B" || setting.ID == "CC012C" || setting.ID == "CC012D" || setting.ID == "CC012E")
                            {
                                switch (setting.ID)
                                {
                                    case "CC012A":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.RTU_b4 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.RTU_b4 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC012B":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.RTU_b3 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.RTU_b3 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC012C":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.RTU_b2 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.RTU_b2 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC012D":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.RTU_b1 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.RTU_b1 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC012E":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.RTU_b0 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.RTU_b0 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;
                                }
                            }

                            else if (setting.ID == "CC016A" || setting.ID == "CC016B" || setting.ID == "CC016C" || setting.ID == "CC016D" || setting.ID == "CC016E")
                            {
                                switch (setting.ID)
                                {
                                    case "CC016A":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.TCP_b4 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.TCP_b4 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC016B":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.TCP_b3 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.TCP_b3 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC016C":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.TCP_b2 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.TCP_b2 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC016D":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.TCP_b1 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.TCP_b1 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;

                                    case "CC016E":
                                        if (Global.IsOpenFile || Global.isExportControlFlow)
                                        {
                                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(rawSetting));
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            TripUnit.TCP_b0 = setting.bValue ? '1' : '0';
                                        }
                                        else
                                        {
                                            setting.bValue = TripUnit.TCP_b0 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                try
                                {

                                    setting.bValue = Convert.ToBoolean((setting.reversevalue_map[rawSetting]).ToString().ToLower());
                                    setting.bDefault = setting.bValue;
                                    setting.bValueReadFromTripUnit = setting.bValue;
                                }
                                catch (Exception ex)
                                {
                                    string str = ex.Message;
                                    //throw;
                                }

                            }
                        }
                        else if (setting.type == Settings.Type.type_bSelection)
                        {
                            if (rawSetting != "0000")
                            {
                                setting.bValue = true;
                                //  setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                            }
                            else
                            {
                                setting.bValue = false;
                                //  setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                            }

                            if (setting.ID.EndsWith("A"))
                            {
                                string setPtA = rawSetting.Substring(0, 2);
                                setting.selectionValue = ((item_ComboBox)setting.lookupTable[setPtA]).item;
                                setting.defaultSelectionValue = setting.selectionValue;
                                string setPtB = rawSetting.Substring(2);
                                Settings settingB = group.groupSetPoints[i + 1];
                                settingB.selectionValue = ((item_ComboBox)settingB.lookupTable[setPtB]).item;
                                settingB.defaultSelectionValue = setting.selectionValue;

                            }
                            else if (!setting.ID.EndsWith("B") || !setting.ID.EndsWith("A"))
                            {
                                if (setting.lookupTable.Contains(rawSetting))
                                {
                                    setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                    setting.defaultSelectionValue = setting.selectionValue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                        }

                        else if (setting.type == Settings.Type.type_bNumber)
                        {
                            //if (rawSetting != "0000")
                            //{
                            double numberVal = 0;
                            numberVal = (int)Int64.Parse(rawSetting, System.Globalization.NumberStyles.HexNumber);
                            numberVal = numberVal / setting.conversion;
                            //TODO :: reverse dependency
                            setting.bValue = (numberVal == 0.067 || numberVal == 0) ? false : true;
                            setting.bDefault = setting.bValue;
                            //if (!setting.bValue)
                            //{
                            setting.numberValue = numberVal;
                            //}
                            //}
                            //else
                            //{
                            //    setting.bValue = false;
                            //    setting.numberValue = setting.numberDefault / setting.conversion; //-- Use Default from file 
                            //}
                            // Set defaults
                            // setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                        }
                        setPointIndex++;
                    }

                }

                if (group.ID == "1" && Global.device_type != Global.ACB_PXR35_DEVICE)
                {
                    group.groupSetPoints[0].selectionValue = Global.GlbstrUnitType;
                    group.groupSetPoints[0].defaultSelectionValue = group.groupSetPoints[0].selectionValue;

                }
            }

            if (Global.device_type == Global.ACB_PXR35_DEVICE) GetRelayValues();
        }


        public static void UpdateMMValuesForOpenFile(Settings setting, string valueForMaintenanceModeRemote)
        {
            try
            {
                if (Global.IsOpenFile || Global.isExportControlFlow)
                {
                    switch (setting.ID)

                    {
                        //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                        //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                        //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                        case "SYS004A":
                        case "SYS4A":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b8 = setting.bValue ? '1' : '0';
                            break;

                        case "SYS004B":
                        case "SYS4B":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b0 = setting.bValue ? '1' : '0';
                            break;

                        case "SYS004C":
                        case "SYS4C":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b0 = setting.bValue ? '1' : '0';
                            break;

                        case "SYS004D":
                        case "SYS4D":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b7 = setting.bValue ? '1' : '0';
                            break;

                        case "SYS004E":
                        case "SYS4E":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b1 = setting.bValue ? '1' : '0';

                            break;
                        case "SYS004F":
                        case "SYS4F":
                            setting.bValue = Convert.ToBoolean(Convert.ToDouble(valueForMaintenanceModeRemote));
                            setting.bDefault = setting.bValue;
                            setting.bValueReadFromTripUnit = setting.bValue;
                            TripUnit.MM_b2 = setting.bValue ? '1' : '0';
                            break;

                    }
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }


        /// <summary>
        /// Matches the settings file variables to the settings coming from the trip unit
        /// </summary>
        public static void MatchOutputFileToModelFileSettings_online()
        {
            int setPointIndex = 0;
            int iterationCount = 0;
            Settings setting;
            String rawSetting;
            //string valueForMaintenancemode = string.Empty;        //#COVARITY FIX     234997
            //string valueForMaintenanceModeRemote;//#COVERITY FIX     234997
            bool isACBRead = !IsOffline && (device_type == ACBDEVICE || device_type == ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE);
            bool isMCCBRead = !IsOffline && device_type == MCCBDEVICE;
            bool isNZMRead = !IsOffline && device_type == NZMDEVICE;
            iterationCount = (isMCCBRead || isNZMRead) ? grouplist.Count : TripUnit.rawSetPoints.Count;

            var defaultcount = 0;
            var iterationGroups = new ArrayList(TripUnit.groups);               //#COVARITY FIX 235089
            if (!Global.IsOffline && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE))

            {
                iterationGroups.RemoveAt(0);
            }

            foreach (Group group in iterationGroups)
            {
                if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE) && !Global.IsOffline)
                {
                    defaultcount = @group.ID == "1" ? 1 : 0;
                }
                if (group.groupSetPoints != null)
                {
                    for (int i = defaultcount; i < group.groupSetPoints.Length; i++)
                    {
                        if (setPointIndex <= iterationCount)
                        {
                            setting = group.groupSetPoints[i];
                            rawSetting = grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence && /*setting.visible*/ setting.TripUnitSequence != 0)?.SetPointValue;
                            if ((isMCCBRead || isNZMRead) && (rawSetting == null || setting.ID == "SYS4B" || setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151" ||
                                setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A"/*|| setting.ID == "SYS152"*/))
                            {
                                continue;
                            }
                            if (!IsOffline && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE) && (setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"
                                || setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A"))
                            {
                                continue;
                            }
                            if (rawSetting.Length != 4)
                            {
                                switch (rawSetting.Length)
                                {
                                    case 1:
                                        rawSetting = "000" + rawSetting;
                                        break;
                                    case 2:
                                        rawSetting = "00" + rawSetting;
                                        break;
                                    case 3:
                                        rawSetting = "0" + rawSetting;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (setting.type == Settings.Type.type_number)
                            {


                                setting.numberDefault = Convert.ToDouble(convertHexToNum(rawSetting, setting.conversion, ref setting.numberValue, setting.conversionOperation), CultureInfo.CurrentUICulture);
                                setting.numberValue = setting.numberDefault;
                            }

                            if (setting.type == Settings.Type.type_text)
                            {
                                if (setting.ID == ipControl1)
                                {
                                    string str = string.Empty;
                                    // setting.IPaddress = (setpointIndex[setPointCounter]).ToString();
                                    // var valueseperator = setting.IPaddress.Split(',');
                                    for (int ip1 = 0; ip1 <= 3; ip1++)
                                    {
                                        rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                        if (str == string.Empty)
                                        {
                                            str = convertHexToString(rawSetting, setting.conversion); ;
                                        }
                                        else
                                        {
                                            str = str + "." + convertHexToString(rawSetting, setting.conversion);
                                        }
                                        // str += convertHexToString(rawSetting, setting.conversion);
                                        setPointIndex++;
                                    }
                                    setting.IPaddress = str;
                                    continue;
                                }
                                else if (setting.ID == ipControl2)
                                {
                                    // string str = string.Empty;        //#COVARITY FIX    234997
                                    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                    string str = convertHexToString(rawSetting, setting.conversion);
                                    setting.textvalue = Convert.ToDouble(str);
                                    setting.defaultextvalue = Convert.ToDouble(str);
                                }
                                else if (setting.ID == ipControl3)
                                {
                                    string str = string.Empty;
                                    // setting.IPaddress = (setpointIndex[setPointCounter]).ToString();
                                    // var valueseperator = setting.IPaddress.Split(',');
                                    for (var ip3 = 0; ip3 <= 1; ip3++)
                                    {
                                        rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                        if (str == string.Empty)
                                        {
                                            str = convertHexToString(rawSetting, setting.conversion); ;
                                        }
                                        else
                                        {
                                            str = str + "." + convertHexToString(rawSetting, setting.conversion);
                                        }
                                        setPointIndex++;
                                    }
                                    Settings ipAddress1 = TripUnit.getIPAddress();
                                    String[] ip = ipAddress1.IPaddress.Split('.');
                                    setting.IPaddress = ip[0] + "." + ip[1] + "." + str;
                                    continue;
                                }
                                //else
                                //{
                                //    string str = string.Empty;
                                //    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //    str = convertHexToString(rawSetting, setting.conversion);
                                //    setting.textvalue = Convert.ToDouble(str);
                                //    setting.defaultextvalue = Convert.ToDouble(str);
                                //}
                            }

                            else if (setting.type == Settings.Type.type_selection)
                            {
                                //For Non dependency fields, for online mode and on opening file.

                                if ((setting.ID != "SYS4A") && (setting.ID != "SYS4B") && (setting.ID != "SYS004B") && (setting.ID != "SYS002"))
                                {

                                    if (!setting.visible && !setting.parseInPXPM && setting.GroupID != "0" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE))
                                    {
                                        continue;
                                    }
                                    if (setting.lookupTable.Contains(rawSetting))
                                    {

                                        if (setting.indexesWithHexValuesMapping != null && setting.indexesWithHexValuesMapping.Count > 0 && setting.reverseLookupTable.Count != setting.lookupTable.Count)
                                        {
                                            // selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                            //setting.selectionValue = setting.indexesWithHexValuesMapping.FirstOrDefault(x => x.Key == TripUnit.tripUnitIndexArray[setPointIndex].ToString()).Value;
                                        }
                                        else
                                        {
                                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                        }
                                        setting.defaultSelectionValue = setting.selectionValue;
                                        if (setting.ID == "GEN002")
                                        {
                                            Global.appFirmware = setting.selectionValue;
                                        }
                                        if (setting.ID == "GEN02A" && !Global.device_type_PXR10)
                                        {
                                            Global.appFirmware = setting.selectionValue;
                                        }
                                        else if (setting.ID == "GEN02" && Global.device_type_PXR10)
                                        {
                                            Global.appFirmware = setting.textStrValue;
                                        }
                                    }
                                }
                                if (setting.ID == "SYS002")
                                {
                                    setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                                    setting.defaultSelectionValue = setting.selectionValue;
                                }

                                if (setting.ID == "SYS16" || setting.ID == "SYS6")
                                {
                                    setting.selectionValue = Global.GlbstrUnitType;
                                    setting.defaultSelectionValue = Global.GlbstrUnitType;

                                }

                            }
                            else if (setting.type == Settings.Type.type_toggle)
                            {
                                if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                                {
                                    #region SYS004 setpoint dependecny
                                    TripUnit.MM16bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                                    if (TripUnit.MM16bitString != null)
                                    {
                                        TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                                        TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                                        TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                                        TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                                        TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                                    }

                                    TripUnit.MMforExport = Convert.ToInt32(TripUnit.MM16bitString, 2).ToString("X");

                                    while (TripUnit.MMforExport.Length < ("0000").Length)
                                    {
                                        TripUnit.MMforExport = "0" + TripUnit.MMforExport;
                                    }
                                    //Added by Astha to update UI
                                    //with respect to maintenance mode when saved file/Backup file is opened
                                    if (Global.IsOpenFile || Global.isExportControlFlow)
                                    {
                                        string valueForMaintenancemode = rawSetting.Substring(0, 2);
                                        //valueForMaintenanceModeRemote value is not used in this scope, so commented it.
                                        //valueForMaintenanceModeRemote = rawSetting.Substring(2); //#COVERITY FIX  375870

                                        UpdateMMValuesForOpenFile(setting, valueForMaintenancemode);

                                        //setting.bValue = Convert.ToBoolean((setting.reversevalue_map[valueForMaintenancemode]).ToString().ToLower());
                                        //setting.bDefault = setting.bValue;
                                        continue;
                                    }

                                    //a)	If b8 - ARMs mode status is 0, display â€œOffâ€ in â€œMaintenance Mode Stateâ€ field.
                                    //b)	If b8 - ARMs mode status is 1, display â€œOnâ€ in â€œMaintenance Mode Stateâ€ field.

                                    setting.bValue = TripUnit.MM_b8 == '0' ? false : true;
                                    setting.bDefault = setting.bValue;
                                    setting.bValueReadFromTripUnit = setting.bValue;

                                    setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints(Added SYS004A and SYS004B as a parent setpoint.Now there is no SYS004. ) .

                                    #endregion
                                }
                                else if (setting.ID == "SYS004B" || setting.ID == "SYS004C" || setting.ID == "SYS004D" || setting.ID == "SYS004E" ||
                                  setting.ID == "SYS4B" || setting.ID == "SYS4C" || setting.ID == "SYS4D" || setting.ID == "SYS4E" || setting.ID == "SYS4F")

                                {
                                    //Added by Astha to update UI
                                    //with respect to maintenance mode when saved file/Backup file is opened

                                    string valueForMaintenancemode = rawSetting.Substring(0, 2);
                                    string valueForMaintenanceModeRemote = rawSetting.Substring(2);//#COVERITY FIX     234997

                                    if (Global.IsOpenFile || Global.isExportControlFlow)
                                    {
                                        if (valueForMaintenanceModeRemote == "80" || valueForMaintenanceModeRemote == "82" ||
                                            valueForMaintenanceModeRemote == "02" || valueForMaintenanceModeRemote == "00")
                                        {
                                            valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "00" : "0000";
                                        }
                                        if (valueForMaintenanceModeRemote == "81" || valueForMaintenanceModeRemote == "03" ||
                                            valueForMaintenanceModeRemote == "83" || valueForMaintenanceModeRemote == "01")
                                        {
                                            valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "01" : "0001";
                                        }
                                        //setting.bValue = Convert.ToBoolean((setting.reversevalue_map[valueForMaintenanceModeRemote]).ToString().ToLower());
                                        //setting.bValueReadFromTripUnit = setting.bValue;

                                        UpdateMMValuesForOpenFile(setting, valueForMaintenanceModeRemote);

                                        if (setting.ID == "SYS004B" || setting.ID == "SYS4B") setPointIndex++;
                                        continue;
                                    }
                                    switch (setting.ID)
                                    {

                                        //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                        //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                        //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                        case "SYS004B":
                                        case "SYS4B":
                                            setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            break;

                                        case "SYS004C":
                                        case "SYS4C":
                                            setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                            break;

                                        case "SYS004D":
                                        case "SYS4D":
                                            setting.bValue = TripUnit.MM_b7 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                            break;

                                        case "SYS004E":
                                        case "SYS4E":
                                            setting.bValue = TripUnit.MM_b1 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                            break;

                                        case "SYS004F":
                                        case "SYS4F":
                                            setting.bValue = TripUnit.MM_b2 == '0' ? false : true;
                                            setting.bDefault = setting.bValue;
                                            setting.bValueReadFromTripUnit = setting.bValue;
                                            setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                            break;

                                    }
                                }
                                else
                                {
                                    setting.bValue = Convert.ToBoolean((setting.reversevalue_map[rawSetting]).ToString().ToLower());
                                    setting.bValueReadFromTripUnit = setting.bValue;
                                }
                            }
                            else if (setting.type == Settings.Type.type_bool)
                            {
                                // format is 0000 or 0001
                                setting.bValue = (rawSetting.Substring(3) == "1");
                                // Set defaults
                                setting.bValueReadFromTripUnit = setting.bValue;
                            }
                            else if (setting.type == Settings.Type.type_bNumber)
                            {

                                if (rawSetting != "0000")
                                {
                                    setting.bValue = true;
                                    setting.numberDefault = Convert.ToDouble(convertHexToNum(rawSetting, setting.conversion, ref setting.numberValue), CultureInfo.CurrentUICulture);
                                }
                                else
                                {
                                    setting.bValue = false;
                                    setting.numberValue = setting.numberDefault / setting.conversion; //-- Use Default from file 
                                }

                                // Set defaults
                                // setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                            }
                            else if (setting.type == Settings.Type.type_listBox)
                            {
                                //"0xFFFF"
                                // convert hex number to binary
                                // "0b1111 1111 1111 1111
                                // "0b0000 0000 0000 0000 - in this case the converter returns 0
                                // how do we convert a 1 digit number into 16??

                                /// CHEK THIS CODE - REQUIRED OR NOT ================= START ===================
                                //   if (group.groupSetPoints[0].setpoint[0].defaultSelectionValue.Trim().ToLower() == "Alarm".ToLower().Trim())
                                //   {
                                //       if (setting.ID.Trim().ToLower() == "RC002".Trim().ToLower())
                                //       {
                                //           rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //       }
                                //       //if (setting.ID.Trim().ToLower() == "RC003".Trim().ToLower())
                                //       //{
                                //       //    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //       //}
                                //   }

                                //  else  if (group.groupSetPoints[0].setpoint[0].defaultSelectionValue.Trim().ToLower() == "Trip".ToLower().Trim())
                                //   {
                                //       if (setting.ID.Trim().ToLower() == "RC004".Trim().ToLower())
                                //       {
                                //           rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //       }

                                //   }

                                // else   if (group.groupSetPoints[0].setpoint[1].defaultSelectionValue.Trim().ToLower() == "Alarm".ToLower().Trim())
                                //   {
                                //       if (setting.ID.Trim().ToLower() == "RC006".Trim().ToLower())
                                //       {
                                //           rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //       }

                                //   }

                                //else  if (group.groupSetPoints[0].setpoint[1].defaultSelectionValue.Trim().ToLower() == "Trip".ToLower().Trim())
                                //   {
                                //       if (setting.ID.Trim().ToLower() == "RC008".Trim().ToLower())
                                //       {                                        rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                                //       }

                                //   }

                                /// CHEK THIS CODE - REQUIRED OR NOT ================= END ===================

                                for (int k = 0; k < 2; k++)
                                {
                                    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex + k];

                                    String rawBinary = Convert.ToString(Convert.ToInt32(rawSetting, 16), 2);

                                    if (rawBinary.Length < 16)
                                    {
                                        while (rawBinary.Length < 16)
                                        {
                                            rawBinary = "0" + rawBinary;
                                        }
                                    }

                                    string rawBinaryRev = "";

                                    for (int intStrRev = rawBinary.Length - 1; intStrRev >= 0; intStrRev--)
                                    {
                                        rawBinaryRev = rawBinaryRev + rawBinary[intStrRev];
                                    }

                                    rawBinary = rawBinaryRev;

                                    var stringCounter = 0;
                                    for (stringCounter = 16; stringCounter < 32; stringCounter++)
                                    {
                                        ((item_ListBox)setting.itemList[stringCounter]).isSelected = (rawBinary[stringCounter - 16] == '1');
                                    }
                                    if (k == 0)
                                    {
                                        for (stringCounter = 0; stringCounter < rawBinary.Length; stringCounter++)
                                        {
                                            ((item_ListBox)setting.itemList[stringCounter]).isSelected = (rawBinary[stringCounter] == '1');
                                        }
                                    }

                                    if (k == 1)
                                    {
                                        for (stringCounter = 16; stringCounter < 32; stringCounter++)
                                        {
                                            ((item_ListBox)setting.itemList[stringCounter]).isSelected = (rawBinary[stringCounter - 16] == '1');
                                        }
                                    }

                                    for (int j = stringCounter; j < setting.itemList.Count; j++)
                                    {
                                        ((item_ListBox)setting.itemList[j]).isSelected = false;
                                    }

                                }
                                setPointIndex++;
                            }

                            else if (setting.type == Settings.Type.type_split)
                            {
                                #region SYS004 setpoint dependecny

                                TripUnit.MM16bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                                if (TripUnit.MM16bitString != null)
                                {
                                    TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                                    TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                                    TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                                    TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                                    TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                                }

                                TripUnit.MMforExport = Convert.ToInt32(TripUnit.MM16bitString, 2).ToString("X");

                                while (TripUnit.MMforExport.Length < ("0000").Length)
                                {
                                    TripUnit.MMforExport = "0" + TripUnit.MMforExport;
                                }

                                //MM state
                                if (TripUnit.MM_b8 == '0') //a)	If b8 - ARMs mode status is 0, display “Off” in “Maintenance Mode State” field.
                                {
                                    //setting.selectionValue = ((item_ComboBox)setting.lookupTable["0000"]).item;
                                    //setting.defaultSelectionValue = setting.selectionValue;
                                    if (IsOffline)
                                    {
                                        setting.setpoint[0].selectionValue = ((item_ComboBox)setting.setpoint[0].lookupTable["02"]).item;
                                        setting.setpoint[0].defaultSelectionValue = setting.setpoint[0].selectionValue;
                                    }
                                    else
                                    {
                                        setting.setpoint[0].selectionValue = ((item_ComboBox)setting.setpoint[0].lookupTable["00"]).item;
                                        setting.setpoint[0].defaultSelectionValue = setting.setpoint[0].selectionValue;
                                    }
                                }
                                else if (TripUnit.MM_b8 == '1') //b)	If b8 - ARMs mode status is 1, display “On” in “Maintenance Mode State” field.
                                {
                                    //setting.selectionValue = ((item_ComboBox)setting.lookupTable["0001"]).item;
                                    //setting.defaultSelectionValue = setting.selectionValue;
                                    setting.setpoint[0].selectionValue = ((item_ComboBox)setting.setpoint[0].lookupTable["01"]).item;
                                    setting.setpoint[0].defaultSelectionValue = setting.setpoint[0].selectionValue;
                                }


                                //MM Remote Control
                                if (TripUnit.MM_b0 == '0') //a)	If b0 – Remote COM is 0, display “Disabled” in “Maintenance Mode Remote Control” field.
                                {
                                    //group.groupSetPoints[4].selectionValue = ((item_ComboBox)group.groupSetPoints[4].lookupTable["0000"]).item;
                                    //group.groupSetPoints[4].defaultSelectionValue = ((item_ComboBox)group.groupSetPoints[4].lookupTable["0000"]).item;

                                    setting.setpoint[1].selectionValue = ((item_ComboBox)setting.setpoint[1].lookupTable["00"]).item;
                                    setting.setpoint[1].defaultSelectionValue = setting.setpoint[1].selectionValue;
                                }
                                else if (TripUnit.MM_b0 == '1') //b)	If b0 – Remote COM is 1, display “Enabled” in “Maintenance Mode Remote Control” field.
                                {
                                    //group.groupSetPoints[4].selectionValue = ((item_ComboBox)group.groupSetPoints[4].lookupTable["0001"]).item;
                                    //group.groupSetPoints[4].defaultSelectionValue = ((item_ComboBox)group.groupSetPoints[4].lookupTable["0001"]).item;
                                    setting.setpoint[1].selectionValue = ((item_ComboBox)setting.setpoint[1].lookupTable["01"]).item;
                                    setting.setpoint[1].defaultSelectionValue = setting.setpoint[1].selectionValue;
                                }
                                //}


                                #endregion
                            }
                            setPointIndex++;
                        }
                    }
                }
                if (group.subgroups != null)
                {

                    foreach (Group subgroup in group.subgroups)
                    {
                        if (subgroup.groupSetPoints.Length > 0)
                        {
                            Settings[] subgroupSetPoints;

                            subgroupSetPoints = subgroup.groupSetPoints;

                            MatchSubGroupSettingsToModelFileSettings(ref setPointIndex, subgroup, ref subgroupSetPoints);
                        }
                        if (subgroup.subgroups != null)
                        {
                            foreach (Group subgroupLevel2 in subgroup.subgroups)
                            {
                                if (subgroupLevel2.groupSetPoints.Length > 0)
                                {
                                    Settings[] subgroupSetPoints;
                                    setting = null;
                                    rawSetting = null;
                                    subgroupSetPoints = subgroupLevel2.groupSetPoints;

                                    MatchSubGroupSettingsToModelFileSettings(ref setPointIndex, subgroupLevel2, ref subgroupSetPoints);
                                }
                            }
                        }
                    }
                }
                if (group.ID == "1")
                {
                    if (group.groupSetPoints != null)
                    {
                        group.groupSetPoints[0].selectionValue = Global.GlbstrUnitType;
                        group.groupSetPoints[0].defaultSelectionValue = group.groupSetPoints[0].selectionValue;
                    }
                }
            }
            if (!Global.IsOffline && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
            {
                TripUnit.groups = iterationGroups;
            }
        }


        private static void MatchSubGroupSettingsToModelFileSettings(ref int setPointIndex, Group subgroup, ref Settings[] subgroupSetPoints)
        {
            Settings setting;
            String rawSetting;
            // string valueForMaintenancemode = string.Empty;   //#COVARITY FIX   234901, 234906
            //string valueForMaintenanceModeRemote = string.Empty; //#COVERITY FIX  234906
            bool isACBRead = !IsOffline && (device_type == ACBDEVICE || device_type == ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE);
            bool isMCCBRead = !IsOffline && device_type == MCCBDEVICE;
            bool isNZMRead = !IsOffline && device_type == NZMDEVICE;
            for (int i = 0; i < subgroupSetPoints.Length; i++)
            {
                setting = subgroup.groupSetPoints[i];
                rawSetting = grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence /*&& setting.visible*/ && setting.TripUnitSequence != 0)?.SetPointValue;
                if (rawSetting == null)
                {
                    continue;
                }

                if (rawSetting.Length != 4)
                {
                    switch (rawSetting.Length)
                    {
                        case 1:
                            rawSetting = "000" + rawSetting;
                            break;
                        case 2:
                            rawSetting = "00" + rawSetting;
                            break;
                        case 3:
                            rawSetting = "0" + rawSetting;
                            break;
                        default:
                            break;
                    }
                }

                if (setting.type == Settings.Type.type_number)
                {
                    if (setting.ID == ipControl2)
                    {
                        string str = string.Empty;
                        rawSetting = (isMCCBRead || isNZMRead) ? grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence)?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];

                        //str = convertHexToString(rawSetting, setting.conversion);
                        //setting.textvalue = Convert.ToDouble(str);
                        //setting.defaultextvalue = Convert.ToDouble(str);
                    }
                    setting.numberDefault = convertHexToNum(rawSetting, setting.conversion, ref setting.numberValue, setting.conversionOperation);


                    if (setting.ID == "CC01A" && (Global.GlbstrBreakerFrame == Resource.SYS02Item0015) && !(setting.numberDefault >= setting.min && setting.numberDefault <= setting.max))
                    {
                        setting.numberDefault = setting.min;
                        setting.numberValue = setting.numberDefault;
                    }
                    else
                    {

                        setting.numberValue = setting.numberDefault;
                    }
                    if (setting.ID == "CPC081")
                    {
                        Settings setpoint1 = TripUnit.getShortDelayProtection();

                        if ((setting.numberValue == 0))
                        {
                            //setpoint1.selectionValue = ((item_ComboBox)setpoint1.lookupTable["0000"]).item;
                            //  setpoint1.bDefault = false;
                            setpoint1.bValue = false;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                        else
                        {
                            //setpoint1.selectionValue = ((item_ComboBox)setpoint1.lookupTable["0001"]).item;
                            //  setpoint1.bDefault = true;
                            setpoint1.bValue = true;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                    }
                    //Long Delay Time is 3276.7 decimal or 7FFF hex then Long Delay Protection should be marked as Disabled
                    if (setting.ID == "CPC051" && Global.device_type == Global.NZMDEVICE)
                    {
                        Settings setpoint1 = TripUnit.getLongDelayProtection();

                        if (setting.numberValue == 3276.7)
                        {
                            setpoint1.bValue = false;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                        else
                        {
                            setpoint1.bValue = true;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                        setpoint1.notifyDependents();
                    }
                    //Instantaneous pickup is 0 then Instantaneous Protection should be marked as Disabled
                    if ((setting.ID == "CPC014A" || setting.ID == "CPC0101") && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE))
                    {
                        Settings setpoint1 = TripUnit.getInstantaneousProtection();

                        if (setting.numberValue == 0)
                        {
                            setpoint1.bValue = false;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                        else
                        {
                            setpoint1.bValue = true;
                            setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                        }
                        setpoint1.notifyDependents();
                    }

                    //CPC010, CPC021
                    //if (setting.ID == "CPC010" && Global.device_type == Global.ACB_03_00_XX_DEVICE)
                    //{
                    //    Settings setpoint1 = TripUnit.getHighLoad1toggle();

                    //    if (setting.numberValue == 0)
                    //    {
                    //        setpoint1.bValue = false;
                    //        setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                    //    }
                    //    else
                    //    {
                    //        setpoint1.bValue = true;
                    //        setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                    //    }
                    //    setpoint1.notifyDependents();
                    //}

                    //if (setting.ID == "CPC021" && Global.device_type == Global.ACB_03_00_XX_DEVICE)
                    //{
                    //    Settings setpoint1 = TripUnit.getHighLoad2toggle();

                    //    if (setting.numberValue == 0)
                    //    {
                    //        setpoint1.bValue = false;
                    //        setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                    //    }
                    //    else
                    //    {
                    //        setpoint1.bValue = true;
                    //        setpoint1.bValueReadFromTripUnit = setpoint1.bValue;
                    //    }
                    //    setpoint1.notifyDependents();
                    //}

                    if (setting.ID == "CPC091B")
                    {
                        Settings setpoint2 = TripUnit.getShortDelayTimeOption();

                        if ((setting.numberValue == 0.067))
                        {
                            //setpoint2.selectionValue = ((item_ComboBox)setpoint2.lookupTable["0000"]).item;
                            // setpoint2.bDefault = false;
                            setpoint2.bValue = false;
                            setpoint2.bValueReadFromTripUnit = setpoint2.bValue;

                        }
                        else
                        {
                            //setpoint2.selectionValue = ((item_ComboBox)setpoint2.lookupTable["0001"]).item;
                            // setpoint2.bDefault = true;
                            setpoint2.bValue = true;
                            setpoint2.bValueReadFromTripUnit = setpoint2.bValue;
                        }
                    }


                }
                else if (setting.type == Settings.Type.type_bSelection)
                {
                    if (rawSetting != "0000")
                    {
                        setting.bValue = true;
                        // setting.bDefault = setting.bValue;
                        setting.bValueReadFromTripUnit = setting.bValue;
                    }
                    else
                    {
                        setting.bValue = false;
                        // setting.bDefault = setting.bValue;
                        setting.bValueReadFromTripUnit = setting.bValue;
                    }

                    if (setting.ID.EndsWith("A"))
                    {
                        string setPtA = rawSetting.Substring(0, 2);
                        setting.selectionValue = ((item_ComboBox)setting.lookupTable[setPtA]).item;
                        setting.defaultSelectionValue = setting.selectionValue;
                        string setPtB = rawSetting.Substring(2);
                        Settings settingB = subgroup.groupSetPoints[i + 1];
                        settingB.selectionValue = ((item_ComboBox)settingB.lookupTable[setPtB]).item;
                        settingB.defaultSelectionValue = setting.selectionValue;

                    }
                    else if (!setting.ID.EndsWith("B") || !setting.ID.EndsWith("A"))
                    {
                        if (setting.lookupTable.Contains(rawSetting))
                        {
                            setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                            setting.defaultSelectionValue = setting.selectionValue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }

                else if (setting.type == Settings.Type.type_bNumber)
                {

                    //if (rawSetting != "0000")
                    //{
                    double numberVal = 0;
                    numberVal = (int)Int64.Parse(rawSetting, System.Globalization.NumberStyles.HexNumber);
                    numberVal = numberVal / setting.conversion;
                    //TODO :: reverse dependency
                    setting.bValue = (numberVal == 0.067 || numberVal == 0) ? false : true;
                    //if (!setting.bValue)
                    //{
                    setting.numberValue = numberVal;
                    // }
                    //}
                    //   else
                    //   {
                    //       setting.bValue = false;
                    //       setting.numberValue = setting.numberDefault / setting.conversion; //-- Use Default from file 
                    //   }
                    // Set defaults
                    setting.numberDefault = setting.numberValue;
                    //  setting.bDefault = setting.bValue;
                    setting.bValueReadFromTripUnit = setting.bValue;
                }
                else if (setting.type == Settings.Type.type_selection)
                {
                    if (setting.lookupTable.Contains(rawSetting))
                    {
                        setting.selectionValue = ((item_ComboBox)setting.lookupTable[rawSetting]).item;
                        setting.defaultSelectionValue = setting.selectionValue;
                    }

                }
                else if (setting.type == Settings.Type.type_bool)
                {
                    // format is 0000 or 0001
                    setting.bValue = (rawSetting.Substring(3) == "1");
                    // Set defaults
                    //  setting.bDefault = setting.bValue;
                    setting.bValueReadFromTripUnit = setting.bValue;
                }
                else if (setting.type == Settings.Type.type_toggle)
                {

                    if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                    {
                        #region SYS004 setpoint dependecny
                        TripUnit.MM16bitString = String.Join(String.Empty, rawSetting.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                        if (TripUnit.MM16bitString != null)
                        {
                            TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                            TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                            TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                            TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                            TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                        }

                        TripUnit.MMforExport = Convert.ToInt32(TripUnit.MM16bitString, 2).ToString("X");

                        while (TripUnit.MMforExport.Length < ("0000").Length)
                        {
                            TripUnit.MMforExport = "0" + TripUnit.MMforExport;
                        }
                        //Added by Astha to update UI
                        //with respect to maintenance mode when saved file/Backup file is opened
                        if (Global.IsOpenFile || Global.isExportControlFlow)
                        {
                            string valueForMaintenancemode = rawSetting.Substring(0, 2);
                            //'valueForMaintenanceModeRemote' variable is not used in this scope
                            //valueForMaintenanceModeRemote = rawSetting.Substring(2);//#COVERITY FIX  234906

                            UpdateMMValuesForOpenFile(setting, valueForMaintenancemode);
                            //setting.bValue = Convert.ToBoolean((setting.reversevalue_map[valueForMaintenancemode]).ToString().ToLower());
                            //setting.bDefault = setting.bValue;
                            continue;
                        }
                        //a)	If b8 - ARMs mode status is 0, display â€œOffâ€ in â€œMaintenance Mode Stateâ€ field.
                        //b)	If b8 - ARMs mode status is 1, display â€œOnâ€ in â€œMaintenance Mode Stateâ€ field.

                        setting.bValue = TripUnit.MM_b8 == '0' ? false : true;
                        setting.bDefault = setting.bValue;
                        setting.bValueReadFromTripUnit = setting.bValue;

                        setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints(Added SYS004A and SYS004B as a parent setpoint.Now there is no SYS004. ) .

                        #endregion
                    }
                    else if (setting.ID == "SYS004B" || setting.ID == "SYS004C" || setting.ID == "SYS004D" || setting.ID == "SYS004E" ||
                                setting.ID == "SYS4B" || setting.ID == "SYS4C" || setting.ID == "SYS4D" || setting.ID == "SYS4E" || setting.ID == "SYS4F")

                    {
                        //Added by Astha to update UI
                        //with respect to maintenance mode when saved file/Backup file is opened

                        string valueForMaintenancemode = rawSetting.Substring(0, 2);
                        string valueForMaintenanceModeRemote = rawSetting.Substring(2);//#COVERITY FIX  234906

                        if (Global.IsOpenFile || Global.isExportControlFlow)
                        {
                            if (valueForMaintenanceModeRemote == "80" || valueForMaintenanceModeRemote == "82" ||
                                valueForMaintenanceModeRemote == "02" || valueForMaintenanceModeRemote == "00")
                            {
                                valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "00" : "0000";
                            }
                            if (valueForMaintenanceModeRemote == "81" || valueForMaintenanceModeRemote == "03" ||
                                valueForMaintenanceModeRemote == "83" || valueForMaintenanceModeRemote == "01")
                            {
                                valueForMaintenanceModeRemote = (setting.ID == "SYS004B" || setting.ID == "SYS4B") ? "01" : "0001";
                            }
                            //setting.bValue = Convert.ToBoolean((setting.reversevalue_map[valueForMaintenanceModeRemote]).ToString().ToLower());
                            ////setting.bDefault = setting.bValue;
                            //setting.bValueReadFromTripUnit = setting.bValue;

                            UpdateMMValuesForOpenFile(setting, valueForMaintenanceModeRemote);
                            if (setting.ID == "SYS004B" || setting.ID == "SYS4B") setPointIndex++;
                            continue;
                        }
                        switch (setting.ID)
                        {
                            //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                            //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                            //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                            case "SYS004B":
                            case "SYS4B":
                                setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                                break;

                            case "SYS004C":
                            case "SYS4C":
                                setting.bValue = TripUnit.MM_b0 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                                setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                break;

                            case "SYS004D":
                            case "SYS4D":
                                setting.bValue = TripUnit.MM_b7 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                                setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                break;

                            case "SYS004E":
                            case "SYS4E":
                                setting.bValue = TripUnit.MM_b1 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                                setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .
                                break;

                            case "SYS004F":
                            case "SYS4F":
                                setting.bValue = TripUnit.MM_b2 == '0' ? false : true;
                                setting.bDefault = setting.bValue;
                                setting.bValueReadFromTripUnit = setting.bValue;
                                setPointIndex--;//This is decremented because in online mode, trip unit gives 48 setpoints and xml has 49 setpoints .

                                break;
                        }
                    }
                    else
                    {
                        setting.bValue = Convert.ToBoolean((setting.reversevalue_map[rawSetting]).ToString().ToLower());
                        // setting.bDefault = setting.bValue;
                        setting.bValueReadFromTripUnit = setting.bValue;

                    }
                }
                else if (setting.type == Settings.Type.type_bNumber)
                {

                    if (rawSetting != "0000")
                    {
                        setting.bValue = true;
                        setting.numberDefault = convertHexToNum(rawSetting, setting.conversion, ref setting.numberValue);
                    }
                    else
                    {
                        setting.bValue = false;
                        setting.numberValue = setting.numberDefault / setting.conversion; //-- Use Default from file 
                    }

                    // Set defaults
                    //  setting.bDefault = setting.bValue;
                    setting.bValueReadFromTripUnit = setting.bValue;
                }

                if (setting.type == Settings.Type.type_text)
                {
                    if (setting.ID == ipControl1)
                    {
                        string str = string.Empty;
                        int counter = 0;
                        // setting.IPaddress = (setpointIndex[setPointCounter]).ToString();
                        // var valueseperator = setting.IPaddress.Split(',');

                        for (int ip1 = 0; ip1 <= 3; ip1++)
                        {
                            int seqCounter = setting.TripUnitSequence + counter;
                            rawSetting = (isMCCBRead || isNZMRead) ? grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter)?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];
                            //  rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                            if (str == string.Empty)
                            {
                                str = convertHexToString(rawSetting, setting.conversion); ;
                            }
                            else
                            {
                                str = str + "." + convertHexToString(rawSetting, setting.conversion);
                            }
                            // str += convertHexToString(rawSetting, setting.conversion);
                            setPointIndex++;
                            counter++;
                        }
                        setting.IPaddress = str;
                        continue;
                    }
                    else if (setting.ID == ipControl2)
                    {
                        // string str = string.Empty;        //#COVARITY FIX     234906
                        rawSetting = (isMCCBRead || isNZMRead) ? grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence)?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];
                        // rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                        string str = convertHexToString(rawSetting, setting.conversion);
                        setting.textvalue = Convert.ToDouble(str);
                        setting.defaultextvalue = Convert.ToDouble(str);
                    }
                    else if (setting.ID == ipControl3)
                    {
                        string str = string.Empty;
                        int counter = 0;
                        for (var ip3 = 0; ip3 <= 1; ip3++)
                        {
                            int seqCounter = setting.TripUnitSequence + counter;
                            rawSetting = (isMCCBRead || isNZMRead) ? grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter)?.SetPointValue : (String)TripUnit.rawSetPoints[setPointIndex];

                            // rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                            if (str == string.Empty)
                            {
                                str = convertHexToString(rawSetting, setting.conversion); ;
                            }
                            else
                            {
                                str = str + "." + convertHexToString(rawSetting, setting.conversion);
                            }
                            setPointIndex++;
                            counter++;
                        }
                        Settings ipAddress1 = TripUnit.getIPAddress();
                        String[] ip = ipAddress1.IPaddress.Split('.');
                        setting.IPaddress = ip[0] + "." + ip[1] + "." + str;
                        continue;
                    }
                    //else
                    //{
                    //    string str = string.Empty;
                    //    rawSetting = (String)TripUnit.rawSetPoints[setPointIndex];
                    //    str = convertHexToString(rawSetting, setting.conversion);
                    //    setting.textvalue = Convert.ToDouble(str);
                    //    setting.defaultextvalue = Convert.ToDouble(str);
                    //}
                }
                if (!setting.ID.StartsWith("GEN") && (isMCCBRead || isNZMRead))
                {
                    setPointIndex++;
                }
                else if (!(isMCCBRead || isNZMRead))
                {
                    setPointIndex++;
                }
            }
        }

        // This method checks if the default admin password is valid.
        // It returns false if the password is incorrect (response equals 173), otherwise it returns true.
        public static bool CheckDefaultPassword()
        {
            int response = 0;
            ACB3_0_Pwd_CommunicationHelper aCB3_0_Pwd_CommunicationHelper = new ACB3_0_Pwd_CommunicationHelper();

            response = aCB3_0_Pwd_CommunicationHelper.VerifyAdminPassword("0 0 0 0 0 0 ");

            if (response == 173)
                return false;

            return true;
        }

        private static void eventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] byte_buffer;
                string hexData = string.Empty;

                int bytesToRead = Global.mSerialPort.BytesToRead;
                byte_buffer = new byte[bytesToRead];
                Global.mSerialPort.Read(byte_buffer, 0, bytesToRead);
                if (byte_buffer.Length > 0)
                {
                    for (int byteCounter = 0; byteCounter < byte_buffer.Length; byteCounter++)
                    {
                        hexData += (byte_buffer[byteCounter]).ToString("X2") + " ";
                    }
                    ResponseFor351 = byte_buffer[4];
                    hexDataFor351 = hexData;
                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }


        /// <summary>
        /// Converts the setting values sent from the trip unit into their decimal equivalents
        /// </summary>
        public static double convertHexToNum(String hexNum, double conversionRatio, ref double convertedNum, string conversionOperation = "*", bool isassignVlaue = true)
        {
            try
            {
                double unconvertedDecimal = 0;
                unconvertedDecimal = Convert.ToDouble(updateValueonCultureBasis(((int)Int64.Parse(hexNum, System.Globalization.NumberStyles.HexNumber)).ToString()), CultureInfo.CurrentUICulture);

                switch (conversionOperation)
                {
                    case "*":
                        if (isassignVlaue)
                        { convertedNum = unconvertedDecimal / conversionRatio; }
                        if (conversionRatio != 1)
                        {
                            unconvertedDecimal = (unconvertedDecimal / conversionRatio);
                        }
                        break;
                    case "-":
                        unconvertedDecimal = conversionRatio - unconvertedDecimal;
                        break;

                }
                //double unconvertedDecimal = Convert.ToInt32(hexNum, 16);               

                return unconvertedDecimal;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                // MessageBox.Show("Global Function Exception: " + hexNum + " " + ex.ToString());
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.GlobalFunctionException, hexNum + " " + ex, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();

            }
            return -1;
        }
        public static double convertHexToNum(String hexNum, double conversionRatio, string conversionOperation)
        {
            try
            {
                double unconvertedDecimal = 0;
                unconvertedDecimal = Convert.ToDouble(updateValueonCultureBasis(((int)Int64.Parse(hexNum, System.Globalization.NumberStyles.HexNumber)).ToString()), CultureInfo.CurrentUICulture);
                //double unconvertedDecimal = Convert.ToInt32(hexNum, 16);
                // convertedNum = unconvertedDecimal / conversionRatio;
                switch (conversionOperation)
                {
                    case "*":
                        if (conversionRatio != 1)
                        {
                            unconvertedDecimal = (unconvertedDecimal / conversionRatio);
                        }
                        break;
                    case "-":
                        unconvertedDecimal = conversionRatio - unconvertedDecimal;
                        break;

                }
                return unconvertedDecimal;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                // MessageBox.Show("Global Function Exception: " + hexNum + " " + ex.ToString());
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.GlobalFunctionException, hexNum + " " + ex, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();

            }
            return -1;
        }
        /// <summary>
        /// Converts the setting values sent from the trip unit into their decimal equivalents
        /// </summary>
        public static string convertHexToString(String hexNum, double conversionRatio)
        {
            try
            {
                double unconvertedDecimal = 0;
                unconvertedDecimal = (int)Int64.Parse(hexNum, System.Globalization.NumberStyles.HexNumber);
                //double unconvertedDecimal = Convert.ToInt32(hexNum, 16);
                string convertedNum = (unconvertedDecimal / conversionRatio).ToString();
                if (conversionRatio != 1)
                {
                    unconvertedDecimal = (unconvertedDecimal / conversionRatio);
                }
                return unconvertedDecimal.ToString();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                // MessageBox.Show("Global Function Exception: " + hexNum + " " + ex.ToString());
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.GlobalFunctionException, hexNum + " " + ex, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();

            }
            return string.Empty;
        }

        public static bool ReadStyle_OFFLINE(string styleName)
        {
            bool isFileReadSuccess = false;
            TripUnit.userStyle = styleName;
            transferDefaultValues_OFFLINE();
            isFileReadSuccess = true;
            return isFileReadSuccess;
        }

        /// <summary>
        /// Takes the default values from the model file and uses them as the UI values
        /// </summary>
        public static void transferDefaultValues_OFFLINE()
        {
            Settings setting;
            try
            {
                foreach (Group group in TripUnit.groups)
                {
                    if (group.groupSetPoints != null)
                    {
                        for (int i = 0; i < group.groupSetPoints.Length; i++)
                        {
                            setting = group.groupSetPoints[i];



                            //if (setting.ID == "SYS005") //Added by astha to update combobox of maintenance mode trip level based on selected firmware and breaker frame 
                            //{
                            //    UpdateMaintenanceModeTripLevelItems(setting);
                            //}  
                            if (setting.type == Settings.Type.type_number)
                            {
                                setting.numberValue = (selectedTemplateType == MCCBTEMPLATE || selectedTemplateType == NZMTEMPLATE || selectedTemplateType == ACB3_0TEMPLATE || selectedTemplateType == ACB_PXR35_TEMPLATE)
                                    ? setting.numberDefault : setting.numberDefault * setting.conversion;
                            }
                            else if (setting.type == Settings.Type.type_selection)
                            {
                                setting.selectionValue = setting.defaultSelectionValue;
                            }
                            else if (setting.type == Settings.Type.type_bool)
                            {
                                setting.bValue = setting.bDefault;
                            }
                            else if (setting.type == Settings.Type.type_text)
                            {

                                if (setting.ID == "IP001" || setting.ID == "IP003")
                                {
                                    setting.IPaddress = setting.IPaddress_default;
                                }
                                else
                                {
                                    setting.textvalue = setting.defaultextvalue * setting.conversion;
                                }
                            }
                            else if (setting.type == Settings.Type.type_bNumber)
                            {
                                setting.bValue = setting.bDefault;
                                setting.numberValue = setting.numberDefault * setting.conversion;
                            }
                            else if (setting.type == Settings.Type.type_toggle)
                            {
                                setting.bValue = setting.bDefault;
                            }
                            setting.notifyDependents();
                            //else if (setting.type == Settings.Type.type_split)
                            //{
                            //    for (int j = 0; j < setting.setpoint.Length; j++)
                            //    {
                            //        switch (setting.setpoint[j].type.ToString().ToLower())
                            //        {
                            //            case "type_number":
                            //                setting.setpoint[j].numberValue =
                            //                    setting.setpoint[j].numberDefault * setting.setpoint[j].conversion;
                            //                break;
                            //            case "type_selection":
                            //                setting.setpoint[j].selectionValue = setting.setpoint[j].defaultSelectionValue;
                            //                break;
                            //        }
                            //    }
                            //}
                        }
                    }
                    if (group.subgroups != null)
                    {
                        foreach (Group subGroup in group.subgroups)
                        {
                            if (subGroup.groupSetPoints.Length > 0)
                            {
                                foreach (Settings grpSetting in subGroup.groupSetPoints)
                                {
                                    TransferSubgroupValuesOffline(grpSetting);
                                    grpSetting.notifyDependents();
                                }
                            }
                            if (subGroup.subgroups.Length > 0)
                            {
                                foreach (Group subGroupLevel2 in subGroup.subgroups)
                                {
                                    if (subGroupLevel2.groupSetPoints != null)
                                    {
                                        foreach (Settings grpSetting in subGroupLevel2.groupSetPoints)
                                        {
                                            TransferSubgroupValuesOffline(grpSetting);
                                            grpSetting.notifyDependents();
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                // var set = ((Settings)((Group)(TripUnit.groups[1])).groupSetPoints[0]);          
                if (Global.device_type == Global.ACB_PXR35_DEVICE && !IsOpenFile)
                {
                    updateDefaultRelayValuesPXR35();
                }
            }

            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);
            }
        }

        private static void TransferSubgroupValuesOffline(Settings grpSetting)
        {
            var setting = grpSetting;
            try
            {
                //if (setting.ID == "SYS005") //Added by astha to update combobox of maintenance mode trip level based on selected firmware and breaker frame 
                //{
                //    UpdateMaintenanceModeTripLevelItems(setting);
                //}
                if (setting.type == Settings.Type.type_number)
                {
                    setting.numberValue = (selectedTemplateType == PTM_TEMPLATE || selectedTemplateType == MCCBTEMPLATE || selectedTemplateType == NZMTEMPLATE || selectedTemplateType == ACB3_0TEMPLATE || selectedTemplateType == ACB_PXR35_TEMPLATE) ? setting.numberDefault : setting.numberDefault * setting.conversion;
                }
                else if (setting.type == Settings.Type.type_selection)
                {
                    setting.selectionValue = setting.defaultSelectionValue;
                }
                else if (setting.type == Settings.Type.type_bool)
                {
                    setting.bValue = setting.bDefault;
                }
                else if (setting.type == Settings.Type.type_text)
                {
                    if (setting.ID == ipControl1 || setting.ID == ipControl3)
                    {
                        setting.IPaddress = setting.IPaddress_default;
                    }
                    else
                    {
                        setting.textvalue = setting.defaultextvalue * setting.conversion;
                    }
                }
                else if (setting.type == Settings.Type.type_bNumber)
                {
                    setting.bValue = setting.bDefault;
                    setting.numberValue = setting.numberDefault * setting.conversion;
                }
                else if (setting.type == Settings.Type.type_toggle)
                {
                    setting.bValue = setting.bDefault;
                }
                //else if (setting.type == Settings.Type.type_split)
                //{
                //    foreach (Settings splitSetting in setting.setpoint)
                //    {
                //        switch (splitSetting.type.ToString().ToLower())
                //        {
                //            case "type_number":
                //                splitSetting.numberValue =
                //                    splitSetting.numberDefault * splitSetting.conversion;
                //                break;
                //            case "type_selection":
                //                splitSetting.selectionValue = splitSetting.defaultSelectionValue;
                //                break;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);
            }
        }


        /// <summary>
        /// Clears all of the values from the Trigger application so a new action can be taken. 
        /// </summary>
        public static void resetTrigger()
        {
            // TripUnit.groups = new ArrayList();           // Contains the settings in their groups. 
            //  TripUnit.IDTable = new Hashtable();        // Contains the settings and their respective IDs
            // TripUnit.ID_list = new ArrayList();
            TripUnit.rawSetPoints = new ArrayList();     // Stores the set points as they are read in from the trip unit or file. They are in hex. 
            TripUnit.tripUnitString = string.Empty;             // This is used for the file and the trip unit input

            TripUnit.userBreakerInformation = null;
            TripUnit.userUnitType = null;
            TripUnit.userStyle = null;
            TripUnit.userRatingPlug = null;

            Global.selectedTemplateType = string.Empty;
            Errors.SetConnectionStatus(false);
            Errors.SetWizardFinished(false);
            Errors.SetStyleInstalled(false);
            isSaveCalledFromClose = false;
            IsOpenFile = false;                         //Added by Astha to update UI when a file is opened.
            Global.listGroupsAsFoundSetPoint.Clear();                                    //TripUnit.lookupTable_styleCodes = new Hashtable();            // Contain the lookup tables to determine the style of trip unit we are...
                                                                                         //TripUnit.lookupTable_plugCodes = new Hashtable();             // Contain the lookup table for determining what the rating plug is rated for. 

            //delete Datafiles Folder

        }

        public static void CloseWindow(string nameOfWindow)
        {
            foreach (Window win in System.Windows.Application.Current.Windows)
            {
                if (win.Name == "winTestComplete")
                {
                    win.Close();
                }
                if (win.Name == "winSetpointsSelection")
                {
                    win.Close();
                }
                if (win.Name == "winAsFoundAsLeftDisplay")
                {
                    win.Close();
                }
                if (win.Name == "Troubleshooting_DBSearch_Window")
                {
                    win.Close();
                }
            }
        }

        public static void ForbidCloseMessageBox()
        {
            Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.Warning, Resource.StopWindowClosingMsg, " ", false);
            MsgBoxWindow.Topmost = true;
            MsgBoxWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            MsgBoxWindow.ShowDialog();
        }

        public static string getAllNumbersFromString(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static ArrayList populatesetpintData()
        {
            try
            {
                string[] setpointIdArrayACB = null;
                string[] setpointIdArrayMCCB = null;
                setpointIdArrayACB = new string[] { "SYS001","SYS002", "SYS004A", "SYS005", "SYS005A", "SYS005B", "SYS006", "SYS007", "SYS008", "CPC004", "CPC004A", "CPC005", "CPC006", "CPC007", "CPC007A", "CPC008", "CPC008A", "CPC010", "CPC021", "CPC011", "CPC012", "CPC012A", "CPC013", "CPC013A", "CPC014", "CPC014A", "CPC015", "CPC016", "CPC017", "CPC018", "CPC018A", "CPC019", "CPC019A", "CPC020","CPC16","CPC16A",
                "RPC00", "RPC01", "RPC02", "RPC03", "RPC04", "RPC05", "RPC06", "RPC07", "RPC08", "RPC09", "RPC10", "RPC11", "RPC12", "RPC13", "RPC14", "RPC17", "RPC18"};
                // string[] setpointIdArrayMCCB = new string[] { "SYS01", "SYS02", "SYS03", "SYS4A", "SYS4B", "SYS05", "SYS06", "SYS07", "SYS08", "SYS09", "SYS10", "SYS11", "SYS12",  "SYS131", "SYS132", "SYS141", "SYS142", "SYS151", "SYS152", "SYS16","SYS17", "CPC041", "CPC042", "CPC051", "CPC052", "CPC03", "CPC081", "CPC082", "CPC091A", "CPC091B", "CPC092", "CPC07", "CPC0101", "CPC0102", "CPC11", "CPC12", "CPC141", "CPC142", "CPC151A", "CPC151B", "CPC152", "CPC13", "CPC02", "CPC01", "CPC16", "CPC17",  "CPC06", "CPC18", "CPC19A", "CPC19B"};
                //Motor setpoint Ids added to the list for MCCB and NZM
                if (Global.device_type == Global.MCCBDEVICE)
                { setpointIdArrayMCCB = new string[] { "SYS01", "SYS02", "SYS4A", "SYS4B", "SYS05", "SYS06", "CPC01", "CPC02", "CPC03", "SYS16", "CPC041", "CPC042", "CPC051", "CPC052", "CPC07", "CPC081", "CPC082", "CPC091A", "CPC091B", "CPC092", "CPC0101", "CPC0102", "CPC11", "CPC12", "CPC13", "CPC141", "CPC142", "CPC151A", "CPC151B", "CPC152", "CPC17", "CPC16", "CPC06", "CPC18", "CPC19A", "CPC19B", "MPC00", "MPC01", "MPC02", "MPC03", "MPC04", "MPC05", "MPC06", "MPC07", "MPC08", "MPC09", "MPC10", "MPC11", "MPC12", "MPC13", "MPC14", "MPC15", "MPC16", "MPC17", "MPC18" }; }
                else if (Global.device_type == Global.NZMDEVICE)
                { setpointIdArrayMCCB = new string[] { "SYS01", "SYS2", "SYS4A", "SYS4B", "SYS05", "SYS06", "CPC01", "CPC02", "CPC03", "SYS6", "CPC041", "CPC043", "CPC051", "CPC052", "CPC07", "CPC081", "CPC082", "CPC091A", "CPC092", "CPC0101", "CPC0102", "CPC11", "CPC12", "CPC13", "CPC141", "CPC142", "CPC151A", "CPC152", "CPC17", "CPC16", "CPC06", "CPC18", "CPC19A", "CPC19B", "MPC00", "MPC01", "MPC02", "MPC03", "MPC04", "MPC05", "MPC06", "MPC07", "MPC08", "MPC09", "MPC10", "MPC11", "MPC12", "MPC13", "MPC14", "MPC15", "MPC16", "MPC17", "MPC18" }; }

                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                    setpointIdArrayACB = new string[] { "SYS001A","SYS002A", "SYS004A", /*"SYS004AT",*/ "SYS005A", "SYS006", "SYS007A", "SYS008", "CPC028", "CPC16A",
                        "CPC029","CPC030", "CPC006", "CPC007A", "CPC051",  "CPC010", "CPC021", "CPC011","CPC081",  "CPC091A",
                         "CPC0101",  "CPC015", "CPC016", "CPC017", "CPC141",  "CPC151A",  "CPC020A",
                         "CPC078", "CPC079", "CPC083" , "CPC084", "CPC085", "CPC086", "CPC087", "CPC088", "CPC089", "CPC093","CPC094","CPC095", "CPC096",
                         "CPC097", "CPC098",  "CPC0103","CPC0104"

                        };



                Group newGrp1 = null;
                Group newGrp2 = null;
                int setPointCntGrp1 = 0;
                int setPointCntGrp2 = 0;
                int setPointCntGrp = 0;
                setpointIdArrayGroupZero.Clear();
                setpointIdArrayGroupOne.Clear();

                ArrayList temp = new ArrayList();


                if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE/* || Global.device_type == Global.ACB_03_00_XX_DEVICE*/)
                {
                    newGrp1 = new Group(8, "1", "System");
                    newGrp2 = new Group(35, "2", "");
                    foreach (string Id in setpointIdArrayACB)
                    {
                        if (((Settings)TripUnit.IDTable[Id]) != null && (((Settings)TripUnit.IDTable[Id]).visible == true) || (Id == "SYS004A" && ((Settings)TripUnit.IDTable["SYS004AT"]).visible)) //|| ((Settings)TripUnit.IDTable[Id]).parseInPXPM == true)
                        {
                            if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 1)
                            {
                                newGrp1.groupSetPoints[setPointCntGrp1] = ((Settings)TripUnit.IDTable[Id]);
                                setPointCntGrp1++;
                                setPointCntGrp++;
                                setpointIdArrayGroupZero.Add(Id);
                            }
                            if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 2 || Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 5)
                            {
                                newGrp2.groupSetPoints[setPointCntGrp2] = ((Settings)TripUnit.IDTable[Id]);
                                //Set Maintenance mode state to 'On' if maintenance mode is ON either from remote or from device side
                                if (Id == "SYS004A")
                                {
                                    bool MM_Enabled = false;
                                    bool IsMMStateOn = false;
                                    CurvesCalculation.SetMMValues(ref MM_Enabled, ref IsMMStateOn);
                                    newGrp2.groupSetPoints[setPointCntGrp2].bValue = IsMMStateOn;
                                    newGrp2.groupSetPoints[setPointCntGrp2].bDefault = IsMMStateOn;
                                }
                                setPointCntGrp2++;
                                setPointCntGrp++;
                                setpointIdArrayGroupOne.Add(Id);
                            }
                        }

                    }
                    temp.Add(newGrp1);
                    temp.Add(newGrp2);
                }

                if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
                {
                    newGrp1 = new Group(7, "1", "System");
                    newGrp2 = new Group(49, "2", "");//19 motor setpoints added 

                    foreach (string Id in setpointIdArrayACB)
                    {
                        try
                        {
                            if (((Settings)TripUnit.IDTable[Id]) != null && (((Settings)TripUnit.IDTable[Id]).visible == true) || (Id == "SYS004A" && ((Settings)TripUnit.IDTable["SYS004AT"]).visible))
                            {
                                if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 0)
                                {
                                    newGrp1.groupSetPoints[setPointCntGrp1] = ((Settings)TripUnit.IDTable[Id]);
                                    setPointCntGrp1++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupZero.Add(Id);
                                }
                                if ((Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 1 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 2 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 4)
                                {
                                    newGrp2.groupSetPoints[setPointCntGrp2] = ((Settings)TripUnit.IDTable[Id]);
                                    //Set Maintenance mode state to 'On' if maintenance mode is ON either from remote or from device side
                                    if (Id == "SYS004A")
                                    {
                                        bool MM_Enabled = false;
                                        bool IsMMStateOn = false;
                                        CurvesCalculation.SetMMValues(ref MM_Enabled, ref IsMMStateOn);
                                        if (IsMMStateOn)
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem01;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem01;
                                        }
                                        else
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem00;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem00;
                                        }
                                    }
                                    setPointCntGrp2++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupOne.Add(Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                        }

                    }
                    temp.Add(newGrp1);
                    temp.Add(newGrp2);
                }

                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    newGrp1 = new Group(7, "1", "System");
                    newGrp2 = new Group(49, "2", "");//19 motor setpoints added 

                    foreach (string Id in setpointIdArrayACB)
                    {
                        try
                        {

                            if (((Settings)TripUnit.IDTable[Id]) != null && (((Settings)TripUnit.IDTable[Id]).visible == true) || (Id == "SYS004A" && ((Settings)TripUnit.IDTable["SYS004AT"]).visible))
                            {

                                if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 0)
                                {
                                    newGrp1.groupSetPoints[setPointCntGrp1] = ((Settings)TripUnit.IDTable[Id]);
                                    setPointCntGrp1++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupZero.Add(Id);
                                }
                                if ((Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 1 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 2 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 4)
                                {
                                    newGrp2.groupSetPoints[setPointCntGrp2] = ((Settings)TripUnit.IDTable[Id]);
                                    //Set Maintenance mode state to 'On' if maintenance mode is ON either from remote or from device side
                                    if (Id == "SYS004A")
                                    {
                                        bool MM_Enabled = false;
                                        bool IsMMStateOn = false;
                                        CurvesCalculation.SetMMValues(ref MM_Enabled, ref IsMMStateOn);
                                        if (IsMMStateOn)
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem01;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem01;
                                        }
                                        else
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem00;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem00;
                                        }
                                    }
                                    setPointCntGrp2++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupOne.Add(Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                        }

                    }
                    temp.Add(newGrp1);
                    temp.Add(newGrp2);
                }



                if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                {
                    newGrp1 = new Group(7, "1", "System");
                    newGrp2 = new Group(49, "2", "");//19 motor setpoints added 

                    foreach (string Id in setpointIdArrayMCCB)
                    {
                        try
                        {
                            if (((Settings)TripUnit.IDTable[Id]).visible == true || (Id == "SYS4A" && ((Settings)TripUnit.IDTable["SYS4AT"]).visible))
                            {
                                if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 0)
                                {
                                    newGrp1.groupSetPoints[setPointCntGrp1] = ((Settings)TripUnit.IDTable[Id]);
                                    setPointCntGrp1++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupZero.Add(Id);
                                }
                                if ((Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 1 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 2 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 4)
                                {
                                    newGrp2.groupSetPoints[setPointCntGrp2] = ((Settings)TripUnit.IDTable[Id]);
                                    //Set Maintenance mode state to 'On' if maintenance mode is ON either from remote or from device side
                                    if (Id == "SYS4A")
                                    {
                                        bool MM_Enabled = false;
                                        bool IsMMStateOn = false;
                                        CurvesCalculation.SetMMValues(ref MM_Enabled, ref IsMMStateOn);
                                        if (IsMMStateOn)
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem01;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem01;
                                        }
                                        else
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem00;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem00;
                                        }
                                    }
                                    setPointCntGrp2++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupOne.Add(Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                        }

                    }
                    temp.Add(newGrp1);
                    temp.Add(newGrp2);
                }

                if (Global.device_type == Global.PTM_DEVICE)
                {
                    newGrp1 = new Group(7, "1", "System");
                    newGrp2 = new Group(49, "2", "");//19 motor setpoints added 

                    foreach (string Id in setpointIdArrayACB)
                    {
                        try
                        {
                            if (((Settings)TripUnit.IDTable[Id]) != null && (((Settings)TripUnit.IDTable[Id]).visible == true) || (Id == "SYS004A" && ((Settings)TripUnit.IDTable["SYS004AT"]).visible))
                            {
                                if (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID) == 0)
                                {
                                    newGrp1.groupSetPoints[setPointCntGrp1] = ((Settings)TripUnit.IDTable[Id]);
                                    setPointCntGrp1++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupZero.Add(Id);
                                }
                                if ((Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 1 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 2 || (Convert.ToInt32(((Settings)TripUnit.IDTable[Id]).GroupID)) == 4)
                                {
                                    newGrp2.groupSetPoints[setPointCntGrp2] = ((Settings)TripUnit.IDTable[Id]);
                                    //Set Maintenance mode state to 'On' if maintenance mode is ON either from remote or from device side
                                    if (Id == "SYS004A")
                                    {
                                        bool MM_Enabled = false;
                                        bool IsMMStateOn = false;
                                        CurvesCalculation.SetMMValues(ref MM_Enabled, ref IsMMStateOn);
                                        if (IsMMStateOn)
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem01;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem01;
                                        }
                                        else
                                        {
                                            newGrp2.groupSetPoints[setPointCntGrp2].selectionValue = Resource.SYS4AItem00;
                                            newGrp2.groupSetPoints[setPointCntGrp2].defaultSelectionValue = Resource.SYS4AItem00;
                                        }
                                    }
                                    setPointCntGrp2++;
                                    setPointCntGrp++;
                                    setpointIdArrayGroupOne.Add(Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                        }

                    }
                    temp.Add(newGrp1);
                    temp.Add(newGrp2);
                }

                return temp;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }

        }

        public static bool isAnyLegacyLicInstalled()
        {
            KeyValuePair<string, string> liceKeyValWave = objLicenseHelper.IsLicenseInstalled("ValidTripAlarmWaveformLicense");
            KeyValuePair<string, string> liceKeyValTest = objLicenseHelper.IsLicenseInstalled("ValidTestFeatureLicense");
            KeyValuePair<string, string> liceKeyValIN = objLicenseHelper.IsLicenseInstalled("ValidModifyFrameRatingLicense");

            if (liceKeyValTest.Value != null || liceKeyValWave.Value != null)
            {
                return true;
            }
            else if (liceKeyValIN.Value == null && liceKeyValTest.Value == null && liceKeyValWave.Value == null)
            {
                return false;
            }
            else
            {
                return false;
            }
        }


        public static bool isLicenseOrFreeTrialAvailable(string pv_LicenseKey, string pv_featureName, string pv_feaureKey)
        {
            bool isFeatureAllowed = false;
            bool isLicenseInstalled = false;
            int FreeTrialRemaining = 0;
            bool useLecacy = isAnyLegacyLicInstalled();
            KeyValuePair<string, string> liceKeyVal = objLicenseHelper.IsLicenseInstalled(pv_LicenseKey);


            if (liceKeyVal.Key != null)
            {
                isLicenseInstalled = true;
            }

            if (Global.boolDeviceChangedForTestLicense || Global.boolLicenseInstalled == true)
            {
                TestLicenseSessionCounter = 0; // Reset the counter to 0 to indicate device is changed even the session is same
                                               // This will force the licesne counter to get decremented.

                TAWaveformSessionCounter = 0;
                Global.boolDeviceChangedForTestLicense = false; // Set the flag to false so that license counter not decremented in the same session for same device
            }
            if (Global.boolDeviceChangedForInReprogrammingLicense || Global.boolLicenseInstalled == true)
            {
                ModifyRatingSessionCounter = 0;// Reset the counter to 0 to indicate device is changed even the session is same
                                               // This will force the licesne counter to get decremented.
                Global.boolDeviceChangedForInReprogrammingLicense = false; // Set the flag to false so that license counter not decremented in the same session for same device
            }
            //If Perpetual(Infinite) is installed, Use it without showing any message
            if (liceKeyVal.Key != null &&
                      (objLicenseHelper.EncryptText("Perpetual").Trim() == liceKeyVal.Value.Trim()
                      ||
                       objLicenseHelper.CalculateMD5Hash("True").Trim() == liceKeyVal.Value.Trim()
                      )

               )
            {
                isFeatureAllowed = true;
                return isFeatureAllowed;
            }
            //Start= ============If Perpetual is not insatlled, Check for Free trials available 

            if (isFeatureAllowed == false && TestFTsessionCounter == 0 && pv_feaureKey == "TestTrial" && useLecacy)
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    TestFTsessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && TestFTsessionCounter == 1 && pv_feaureKey == "TestTrial")
            {
                isFeatureAllowed = true;
            }

            //===========================
            // for diagnostic free trial
            if (isFeatureAllowed == false && DiagFTSessionCounter == 0 && pv_feaureKey == "DiagnosticTrial" && useLecacy)
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    DiagFTSessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && DiagFTSessionCounter == 1 && pv_feaureKey == "DiagnosticTrial")
            {
                isFeatureAllowed = true;
            }


            //===========================
            // for Trip Alarm Waveform free trial
            if (isFeatureAllowed == false && TAWaveformFTsessionCounter == 0 && pv_feaureKey == "TripAlarmWaveformTrial" && useLecacy)
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    TAWaveformFTsessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && TAWaveformFTsessionCounter == 1 && pv_feaureKey == "TripAlarmWaveformTrial")
            {
                isFeatureAllowed = true;
            }

            // for Modify Frame Rating free trial, there are no free trial, hence checking free trial condition not required.

            //For TS - free trials not applicable
            //// for Troubleshooting free trial
            //if (isFeatureAllowed == false && TroubleshootingFTsessionCounter == 0 && pv_feaureKey == "TroubleshootingTrial")
            //{
            //    isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
            //    if (isFeatureAllowed == true)
            //    {
            //        TroubleshootingFTsessionCounter = 1;
            //    }

            //}
            //else if (isFeatureAllowed == false && TroubleshootingFTsessionCounter == 1 && pv_feaureKey == "TroubleshootingTrial")
            //{
            //    isFeatureAllowed = true;
            //}
            //===============Free Trial check end====================

            //If free trials are not available, Start License check
            //================== Check for License available =============  

            if (!isFeatureAllowed && FreeTrialRemaining == 0)
            {
                //TestLicenseSessionCounter == 0 is added to make sure that License is not being used in same session
                // In this case License utilized message should not be displayed

                if (liceKeyVal.Key != null && objLicenseHelper.DecryptText(liceKeyVal.Value) == "0")
                {
                    if (pv_LicenseKey == "ValidTestFeatureLicense" && TestLicenseSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        //winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }

                    if (pv_LicenseKey == "ValidTripAlarmWaveformLicense" && TAWaveformSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        //winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                    if (pv_LicenseKey == "ValidModifyFrameRatingLicense" && ModifyRatingSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        //winPeruseLicenseUsed.ShowDialog();
                        //Number of License already utilized. Please purchase
                        return isFeatureAllowed;
                    }
                    if (pv_LicenseKey == "ValidTroubleshootingLicense" && TroubleshootingSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, Resource.BreakerTroubleshootingNoLicenseMessage , true);/*string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value)*/
                        //winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                }


                // use TestLicenseSessionCounter ==0 to indicate that this is first update in current session
                if (pv_LicenseKey == "ValidTestFeatureLicense")
                {
                    if (TestLicenseSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim()
                        && liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim()
                        && Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage2 + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.Height = 430;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                string hashedRegistryKey = objLicenseHelper.CalculateMD5Hash(pv_LicenseKey.Trim());
                                objLicenseHelper.UpdateRegistryForLicCounter(hashedRegistryKey);
                                TestLicenseSessionCounter = 1; // Set counter to 1 to indicate 
                                                               // that license count decremented in current session
                                                               //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                                isUserCancleClicked = false;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                isUserCancleClicked = true;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TestLicenseSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "ValidTripAlarmWaveformLicense")
                {
                    if (TAWaveformSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage2 + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.MinHeight = 430;
                        winConfirmLicenseCount.MaxHeight = 480;
                        winConfirmLicenseCount.Width = 460;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with installed session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                string hashedRegistryKey = objLicenseHelper.CalculateMD5Hash(pv_LicenseKey.Trim());
                                objLicenseHelper.UpdateRegistryForLicCounter(hashedRegistryKey);
                                TAWaveformSessionCounter = 1; // Set counter to 1 to indicate 
                                                              // that license count decremented in current session
                                                              //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                                isUserCancleClicked = false;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                isUserCancleClicked = true;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TAWaveformSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "ValidModifyFrameRatingLicense")
                {
                    if (ModifyRatingSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        isFeatureAllowed = true;
                        string hashedRegistryKey = objLicenseHelper.CalculateMD5Hash(pv_LicenseKey.Trim());
                        objLicenseHelper.UpdateRegistryForLicCounter(hashedRegistryKey);
                        ModifyRatingSessionCounter = 1; // Set counter to 1 to indicate 
                                                        // that license count decremented in current session
                                                        //and should not be updated again in this session
                        Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                             // if the feature is opened in the same session when license is installed
                        isSameSession = true;
                    }
                    if (ModifyRatingSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "ValidTroubleshootingLicense")
                {
                    if (TroubleshootingSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt32(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.TSLicenseMessage + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.MinHeight = 430;
                        winConfirmLicenseCount.MaxHeight = 480;
                        winConfirmLicenseCount.Width = 460;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with installed session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                string hashedRegistryKey = objLicenseHelper.CalculateMD5Hash(pv_LicenseKey.Trim());
                                objLicenseHelper.UpdateRegistryForLicCounter(hashedRegistryKey);
                                TroubleshootingSessionCounter = 1; // Set counter to 1 to indicate 
                                                                   // that license count decremented in current session
                                                                   //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                                isUserCancleClicked = false;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                isUserCancleClicked = true;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TroubleshootingSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                    if (TroubleshootingSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, Resource.BreakerTroubleshootingNoLicenseMessage, true);
                        ////  Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, 0, pv_featureName), true);
                        //winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                }
                if ((TestLicenseSessionCounter == 1 || ModifyRatingSessionCounter == 1 || TroubleshootingSessionCounter == 1) && liceKeyVal.Key != null &&
                   liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                   liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                   Convert.ToInt32(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                {
                    isFeatureAllowed = true;
                    return isFeatureAllowed;
                }
            }
            //===============License check end====================
            return isFeatureAllowed;

        }

        public static bool isLicenseOrFreeTrialAvailableForPTM(string pv_LicenseKey, string pv_featureName, string pv_feaureKey)
        {
            bool isFeatureAllowed = false;
            bool isLicenseInstalled = false;
            int FreeTrialRemaining = 0;
            bool useLecacy = isAnyLegacyLicInstalled();

            KeyValuePair<string, string> liceKeyVal = objLicenseHelper.IsLicenseInstalled(pv_LicenseKey);

            if (liceKeyVal.Key != null)
            {
                isLicenseInstalled = true;
            }

            if (Global.boolDeviceChangedForTestLicense || Global.boolLicenseInstalled == true)
            {
                TestLicenseSessionCounter = 0; // Reset the counter to 0 to indicate device is changed even the session is same
                                               // This will force the licesne counter to get decremented.

                TAWaveformSessionCounter = 0;
                Global.boolDeviceChangedForTestLicense = false; // Set the flag to false so that license counter not decremented in the same session for same device
            }
            if (Global.boolDeviceChangedForInReprogrammingLicense || Global.boolLicenseInstalled == true)
            {
                ModifyRatingSessionCounter = 0;// Reset the counter to 0 to indicate device is changed even the session is same
                                               // This will force the licesne counter to get decremented.
                Global.boolDeviceChangedForInReprogrammingLicense = false; // Set the flag to false so that license counter not decremented in the same session for same device
            }
            //If Perpetual(Infinite) is installed, Use it without showing any message
            if (liceKeyVal.Key != null &&
                      (objLicenseHelper.EncryptText("Perpetual").Trim() == liceKeyVal.Value.Trim()
                      ||
                       objLicenseHelper.CalculateMD5Hash("True").Trim() == liceKeyVal.Value.Trim()
                      )

               )
            {
                isFeatureAllowed = true;
                return isFeatureAllowed;
            }
            //Start= ============If Perpetual is not insatlled, Check for Free trials available 

            if (isFeatureAllowed == false && TestFTsessionCounter == 0 && pv_feaureKey == "PTM_TestTrial")
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    TestFTsessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && TestFTsessionCounter == 1 && pv_feaureKey == "PTM_TestTrial")
            {
                isFeatureAllowed = true;
            }

            //===========================
            // for diagnostic free trial
            if (isFeatureAllowed == false && DiagFTSessionCounter == 0 && pv_feaureKey == "PTM_DiagnosticTrial")
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    DiagFTSessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && DiagFTSessionCounter == 1 && pv_feaureKey == "PTM_DiagnosticTrial")
            {
                isFeatureAllowed = true;
            }


            //===========================
            // for Trip Alarm Waveform free trial
            if (isFeatureAllowed == false && TAWaveformFTsessionCounter == 0 && pv_feaureKey == "PTM_TripAlarmWaveformTrial" && useLecacy)
            {
                isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
                if (isFeatureAllowed == true)
                {
                    TAWaveformFTsessionCounter = 1;
                }

            }
            else if (isFeatureAllowed == false && TAWaveformFTsessionCounter == 1 && pv_feaureKey == "PTM_TripAlarmWaveformTrial")
            {
                isFeatureAllowed = true;
            }

            // for Modify Frame Rating free trial, there are no free trial, hence checking free trial condition not required.

            //For TS - free trials not applicable
            //// for Troubleshooting free trial
            //if (isFeatureAllowed == false && TroubleshootingFTsessionCounter == 0 && pv_feaureKey == "TroubleshootingTrial")
            //{
            //    isFeatureAllowed = isFreeTrialAllowed(pv_featureName, pv_feaureKey, isLicenseInstalled, liceKeyVal.Value, out FreeTrialRemaining);
            //    if (isFeatureAllowed == true)
            //    {
            //        TroubleshootingFTsessionCounter = 1;
            //    }

            //}
            //else if (isFeatureAllowed == false && TroubleshootingFTsessionCounter == 1 && pv_feaureKey == "TroubleshootingTrial")
            //{
            //    isFeatureAllowed = true;
            //}
            //===============Free Trial check end====================

            //If free trials are not available, Start License check
            //================== Check for License available =============  

            if (!isFeatureAllowed && FreeTrialRemaining == 0)
            {
                //TestLicenseSessionCounter == 0 is added to make sure that License is not being used in same session
                // In this case License utilized message should not be displayed

                if (liceKeyVal.Key != null && objLicenseHelper.DecryptText(liceKeyVal.Value) == "0")
                {
                    if (pv_LicenseKey == "PTM_ValidTestFeatureLicense" && TestLicenseSessionCounter == 0)
                    {
                        Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }

                    if (pv_LicenseKey == "PTM_ValidTripAlarmWaveformLicense" && TAWaveformSessionCounter == 0)
                    {
                        Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                    if (pv_LicenseKey == "PTM_ValidModifyFrameRatingLicense" && ModifyRatingSessionCounter == 0)
                    {
                        //Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName), true);
                        //winPeruseLicenseUsed.ShowDialog();
                        //Number of License already utilized. Please purchase
                        return isFeatureAllowed;
                    }
                    if (pv_LicenseKey == "PTM_ValidTroubleshootingLicense" && TroubleshootingSessionCounter == 0)
                    {
                        Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, Resource.BreakerTroubleshootingNoLicenseMessage, true);/*string.Format(Resource.LicPerUseMessage1, objLicenseHelper.DecryptText(liceKeyVal.Value)*/
                        winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                }


                // use TestLicenseSessionCounter ==0 to indicate that this is first update in current session
                if (pv_LicenseKey == "PTM_ValidTestFeatureLicense")
                {
                    if (TestLicenseSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim()
                        && liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim()
                        && Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage2 + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.Height = 430;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                objLicenseHelper.UpdateRegistryForLicCounter(pv_LicenseKey);
                                TestLicenseSessionCounter = 1; // Set counter to 1 to indicate 
                                                               // that license count decremented in current session
                                                               //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TestLicenseSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "PTM_ValidTripAlarmWaveformLicense")
                {
                    if (TAWaveformSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage2 + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.MinHeight = 430;
                        winConfirmLicenseCount.MaxHeight = 480;
                        winConfirmLicenseCount.Width = 460;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with installed session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                objLicenseHelper.UpdateRegistryForLicCounter(pv_LicenseKey);
                                TAWaveformSessionCounter = 1; // Set counter to 1 to indicate 
                                                              // that license count decremented in current session
                                                              //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TAWaveformSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "PTM_ValidModifyFrameRatingLicense")
                {
                    if (ModifyRatingSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        isFeatureAllowed = true;
                        objLicenseHelper.UpdateRegistryForLicCounter(pv_LicenseKey);
                        ModifyRatingSessionCounter = 1; // Set counter to 1 to indicate 
                                                        // that license count decremented in current session
                                                        //and should not be updated again in this session
                        Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                             // if the feature is opened in the same session when license is installed
                        isSameSession = true;
                    }
                    if (ModifyRatingSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                }
                if (pv_LicenseKey == "PTM_ValidTroubleshootingLicense")
                {
                    if (TroubleshootingSessionCounter == 0 && liceKeyVal.Key != null &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                        liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                        Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                    {
                        Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.TSLicenseMessage + "\r\n" + "\r\n" + Resource.ContinuePrompt, objLicenseHelper.DecryptText(liceKeyVal.Value), pv_featureName));
                        winConfirmLicenseCount.MinHeight = 430;
                        winConfirmLicenseCount.MaxHeight = 480;
                        winConfirmLicenseCount.Width = 460;
                        winConfirmLicenseCount.ShowDialog();
                        winConfirmLicenseCount.Topmost = true;

                        // Get confirmation from user to continue with installed session
                        if (winConfirmLicenseCount.DialogResult.HasValue)
                        {
                            if (winConfirmLicenseCount.DialogResult == true)
                            {
                                isFeatureAllowed = true;
                                objLicenseHelper.UpdateRegistryForLicCounter(pv_LicenseKey);
                                TroubleshootingSessionCounter = 1; // Set counter to 1 to indicate 
                                                                   // that license count decremented in current session
                                                                   //and should not be updated again in this session
                                Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                                     // if the feature is opened in the same session when license is installed
                                isSameSession = true;
                            }
                            else
                            {
                                isFeatureAllowed = false;
                                return isFeatureAllowed;
                            }
                        }
                    }
                    if (TroubleshootingSessionCounter == 1)
                    {
                        isFeatureAllowed = true;
                    }
                    if (TroubleshootingSessionCounter == 0)
                    {
                        Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, Resource.BreakerTroubleshootingNoLicenseMessage, true);
                        //  Scr_MessageWindow winPeruseLicenseUsed = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage1, 0, pv_featureName), true);
                        winPeruseLicenseUsed.ShowDialog();
                        return isFeatureAllowed;
                    }
                }
                if ((TestLicenseSessionCounter == 1 || ModifyRatingSessionCounter == 1 || TroubleshootingSessionCounter == 1) && liceKeyVal.Key != null &&
                   liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                   liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                   Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                {
                    isFeatureAllowed = true;
                    return isFeatureAllowed;
                }
            }
            //===============License check end====================
            return isFeatureAllowed;

        }

        /// <summary>
        /// It will decrement the session count of Revenera's In tool license by 1
        /// </summary>
        public static void updateReveneraInToolLicenseCount()
        {
            try
            {
                //create hashed feature name to be stored in registry
                string hashedFeatureName = objLicenseHelper.CalcaulateSHA256Hash("Chg_Continous_Current_Rating");

                //Check if the same feature name's registry is available and if available then get its key value pair
                KeyValuePair<string, string> liceKeyVal = objLicenseHelper.IsReveneraLicenseInstalled(hashedFeatureName);


                if (ModifyRatingSessionCounter != 1 && liceKeyVal.Key != null &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                    Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                {

                    objLicenseHelper.UpdateRegistryForLicCounter(hashedFeatureName);
                    ModifyRatingSessionCounter = 1; // Set counter to 1 to indicate 
                                                    // that license count decremented in current session
                                                    //and should not be updated again in this session
                    Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                         // if the feature is opened in the same session when license is installed
                    isSameSession = true;
                }
                //if (ModifyRatingSessionCounter == 1)
                //{
                //    //isFeatureAllowed = true;
                //}

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }


        public static void updateReveneraGFLicenseCount()
        {
            try
            {
                //create hashed feature name to be stored in registry
                string hashedFeatureName = objLicenseHelper.CalcaulateSHA256Hash("Ground_Fault_PXR35");

                //Check if the same feature name's registry is available and if available then get its key value pair
                KeyValuePair<string, string> liceKeyVal = objLicenseHelper.IsReveneraLicenseInstalled(hashedFeatureName);


                if (liceKeyVal.Key != null &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                    Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                {

                    objLicenseHelper.UpdateRegistryForLicCounter(hashedFeatureName);
                    ModifyRatingSessionCounter = 1; // Set counter to 1 to indicate 
                                                    // that license count decremented in current session
                                                    //and should not be updated again in this session
                    Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                         // if the feature is opened in the same session when license is installed
                    isSameSession = true;
                }
                //if (ModifyRatingSessionCounter == 1)
                //{
                //    //isFeatureAllowed = true;
                //}

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void updateReveneraGCLicenseCount()
        {
            try
            {
                //create hashed feature name to be stored in registry
                string hashedFeatureName = objLicenseHelper.CalcaulateSHA256Hash("GF_1200A_Override_PXR35");

                //Check if the same feature name's registry is available and if available then get its key value pair
                KeyValuePair<string, string> liceKeyVal = objLicenseHelper.IsReveneraLicenseInstalled(hashedFeatureName);


                if (liceKeyVal.Key != null &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.EncryptText("Perpetual").Trim() &&
                    liceKeyVal.Value.Trim() != objLicenseHelper.CalculateMD5Hash("True").Trim() &&
                    Convert.ToInt16(objLicenseHelper.DecryptText(liceKeyVal.Value)) > 0)
                {

                    objLicenseHelper.UpdateRegistryForLicCounter(hashedFeatureName);
                    // that license count decremented in current session
                    //and should not be updated again in this session
                    Global.boolLicenseInstalled = false; // Set this flag to avoid license message getting displayed multiple times
                                                         // if the feature is opened in the same session when license is installed
                    isSameSession = true;
                }
                //if (ModifyRatingSessionCounter == 1)
                //{
                //    //isFeatureAllowed = true;
                //}

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }
        public static bool isFreeTrialAllowed(string strFeatureName, string strFeatureNameKey, bool isLicenseInstalled, string licenseValue, out int mFreeTrialRemaining)
        {

            Global.isReport = false;
            bool allowFeature = false;

            //First validation is user should run the application as admin to use free trial.
            //If user is admin then only proceed for use of trial
            //bool isUserAdmin = objLicenseHelper.IsAdministrator();

            //if (!isUserAdmin)
            //{
            //    Scr_MessageWindow winConfirmisAdmin = new Scr_MessageWindow(Resource.LicFTHeader, string.Format(Resource.LicFTIsAdminError, strFeatureName), true,true);
            //    winConfirmisAdmin.ShowDialog();
            //    winConfirmisAdmin.Topmost = true;
            //}
            //else
            //{
            int FreeTrialUsedCnt = objLicenseHelper.ReadregistryValue(strFeatureNameKey, "FreeTrial");

            int FreeTrialRemaining = objLicenseHelper.MaxFreeTrials - FreeTrialUsedCnt;
            mFreeTrialRemaining = FreeTrialRemaining;
            if (FreeTrialRemaining > 0)
            {
                //If License is not installed, Show free trials utilization confirmation and use free trial
                if (!isLicenseInstalled)
                {
                    Scr_MessageWindow winConfirmTrialCont = new Scr_MessageWindow(Resource.LicFTHeader, string.Format(Resource.LicFTTrialsRemaining + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, FreeTrialRemaining, strFeatureName));
                    winConfirmTrialCont.MinHeight = 430;
                    winConfirmTrialCont.MaxHeight = 480;
                    winConfirmTrialCont.Width = 460;
                    winConfirmTrialCont.ShowDialog();
                    winConfirmTrialCont.Topmost = true;


                    // Get confirmation from user to continue with free trial
                    if (winConfirmTrialCont.DialogResult.HasValue)
                    {
                        if (winConfirmTrialCont.DialogResult == true)
                        {
                            objLicenseHelper.WriteRegistryValueForFreeTrial(strFeatureNameKey);
                            allowFeature = true;
                            isUserCancleClicked = false;
                        }
                        else
                        {
                            allowFeature = false;
                            isUserCancleClicked = true;
                        }
                    }
                }
                //If License is installed, Show installed session utilization confirmation and use it
                else
                {
                    int InstalledTestSessions = Convert.ToInt32(objLicenseHelper.DecryptText(licenseValue));
                    int TrailsRemaining = FreeTrialRemaining + InstalledTestSessions;

                    Scr_MessageWindow winConfirmLicenseCount = new Scr_MessageWindow(Resource.LicPerUseMessageHeader, string.Format(Resource.LicPerUseMessage2 + "\r\n" + "\r\n" + Resource.txtSessionInfo + "\r\n" + "\r\n" + Resource.ContinuePrompt, TrailsRemaining, strFeatureName));
                    winConfirmLicenseCount.Height = 430;
                    winConfirmLicenseCount.ShowDialog();
                    winConfirmLicenseCount.Topmost = true;

                    if (winConfirmLicenseCount.DialogResult.HasValue)
                    {
                        if (winConfirmLicenseCount.DialogResult == true)
                        {
                            objLicenseHelper.WriteRegistryValueForFreeTrial(strFeatureNameKey);
                            allowFeature = true;
                            isUserCancleClicked = false;
                        }
                        else
                        {
                            allowFeature = false;
                            isUserCancleClicked = true;
                        }
                    }

                }
            }
            //If free trials are finished then return false
            else
            {
                allowFeature = false;
                //If license is not installed, then only show message of free trials finished
                //No need to display this message as Revenera checks are added after this 
                //if (!isLicenseInstalled)
                //{
                //    Scr_MessageWindow winConfirmTrialContExpired = new Scr_MessageWindow(Resource.LicFTHeader, string.Format(Resource.LicFTAllUsed, strFeatureName), true);
                //    winConfirmTrialContExpired.ShowDialog();
                //    winConfirmTrialContExpired.Topmost = true;
                //}
            }
            //}


            return allowFeature;
        }
        public static void UpdateMaintenanceModeState(ref Settings mmState, bool IsRemoteEnabled)
        {

            bool bValueForState = false;

            if (IsRemoteEnabled || (TripUnit.MM_b8 == '1' && (TripUnit.MM_b7 == '1' || TripUnit.MM_b0 == '1' || TripUnit.MM_b1 == '1' || TripUnit.MM_b2 == '1'))) //b)	If b8 - ARMs mode status is 1, display “On” in “Maintenance Mode State” field.
            {
                bValueForState = true;

            }
            else
            {
                bValueForState = false;
            }
            mmState.toggle.IsChecked = bValueForState;
        }
        public static void updateMaintenanceModeControls(ref Settings mmState, ref Settings mmRemote)
        {
            bool bValueForState = false;
            bool bValueForremote = false;

            if (TripUnit.MM_b8 == '0' || (TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '0')) //a)	If b8 - ARMs mode status is 0, display “Off” in “Maintenance Mode State” field.
            {
                bValueForState = false;
            }
            else if (TripUnit.MM_b8 == '1' && (TripUnit.MM_b7 == '1' || TripUnit.MM_b0 == '1' || TripUnit.MM_b1 == '1' || TripUnit.MM_b2 == '1')) //b)	If b8 - ARMs mode status is 1, display “On” in “Maintenance Mode State” field.
            {
                bValueForState = true;
            }
            if (TripUnit.MM_b0 == '0') //a)	If b0 – Remote COM is 0, display “Disabled” in “Maintenance Mode Remote Control” field.
            {
                bValueForremote = false;

            }
            else
            {

                bValueForremote = true;
            }
            mmState.toggle.IsChecked = bValueForState;
            mmRemote.toggle.IsChecked = bValueForremote;

        }

        public static void valueForMaintenanceModeRemote(ref string valueForMaintenanceModeRemote, string valueForMaintenanceMode)
        {

            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "80";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "81";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "02";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "83";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "82";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "03";
            }
            if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '0')
            {
                valueForMaintenanceModeRemote = "01";
            }

            if (Global.device_type == Global.NZMDEVICE)
            {

                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '1')
                {// "04":"false"
                    valueForMaintenanceModeRemote = "04";
                }
                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '0' && TripUnit.MM_b2 == '1')
                {//"05":"true"
                    valueForMaintenanceModeRemote = "05";
                }
                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '1')
                {//06":"false"
                    valueForMaintenanceModeRemote = "06";
                }
                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "00" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '0' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '1')
                {//"7":"false"
                    valueForMaintenanceModeRemote = "07";
                }
                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '0' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '1')
                {//"86":"true"
                    valueForMaintenanceModeRemote = "86";
                }
                if (valueForMaintenanceMode == "01" && valueForMaintenanceModeRemote == "01" && TripUnit.MM_b8 == '1' && TripUnit.MM_b7 == '1' && TripUnit.MM_b0 == '1' && TripUnit.MM_b1 == '1' && TripUnit.MM_b2 == '1')
                {//"87":"true"
                    valueForMaintenanceModeRemote = "87";
                }
            }
        }
    }

    public static class LogExceptions
    {
        public static void LogExceptionToFile(Exception ex, string appExecutionStage = "")
        {
            try
            {
                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\ErrorLog\" + System.DateTime.Now.ToString("MM/dd/yyyy").Replace("/", "_").Replace("-", "_").Replace(".", "_") + ".txt"))
                {
                    FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + @"\ErrorLog\" + System.DateTime.Now.ToString("MM/dd/yyyy").Replace("/", "_").Replace("-", "_").Replace(".", "_") + ".txt", FileMode.Append, FileAccess.Write);
                    using (StreamWriter sr = new StreamWriter(fs))
                    {
                        if (appExecutionStage != "")
                        {
                            //sr.WriteLine("***\n");
                            sr.WriteLine(appExecutionStage + "\t" + System.DateTime.Now);
                        }
                        else
                        {
                            //MessageBox.Show(ex.Message);
                            sr.WriteLine("***\n");
                            sr.WriteLine(ex.Message);
                            sr.WriteLine(ex.StackTrace + "\t" + System.DateTime.Now);
                            sr.WriteLine("\n\n");
                        }
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }

        }
    }
}


