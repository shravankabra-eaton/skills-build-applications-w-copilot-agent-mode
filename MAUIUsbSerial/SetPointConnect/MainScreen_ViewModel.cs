using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using C1.WPF.C1Chart;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using System.Collections;
using PXR.Resources.Strings;
using System.IO.Ports;
using System.Windows.Threading;
using PXR.Communication;
using System.Threading;
using PXR.Screens.Scrs_Download_Language;
using PXR.Screens.Scrs_Test;
using System.Diagnostics;
using System.Windows.Shapes;
using C1.WPF.C1Chart.Extended;
using System.Windows.Media.Animation;
using C1.C1Preview;
using RadioButton = System.Windows.Controls.RadioButton;
using Convert = System.Convert;
using static CSJ2K.Color.ColorSpace;
using System.ComponentModel;
using C1.Util.DX;
using static C1.Util.Win.Win32;
using C1.C1Excel;

namespace PXR.Screens
{
    public class MainScreen_ViewModel :INotifyPropertyChanged
    {
        public MainScreen mainscreen;
        public event PropertyChangedEventHandler PropertyChanged;
        public string SourceScreen;
        public static ObservableCollection<ChangeSummary> Changes = new ObservableCollection<ChangeSummary>();
        public string chng;
        public string BeforeRefreshAuxStatus = string.Empty;
        ArrayList OriginalsetpointLines = null;
        List<string> MCCBBackUpTripSetPoints = new List<string>();

        int count = 0;
        // private Queue<byte[]> commandQueue1;
        //private static SerialPort serialPort;
        public static List<byte[]> byteList;
        private delegate void UIDelegate();
        private string port_name;
        Stopwatch stopwatchConnErr = new Stopwatch();
        TimeSpan timeoutConnErr = new TimeSpan(0, 0, 8);
        private int bytListCheck = 0;
        double origsidebarWid;
        double origScrollWid;



        Color colorvalue = Colors.White;
        public static string PXset1Filename = string.Empty;
        public static string PXset2Filename = string.Empty;
        public static string PXSet1Filepath = string.Empty;
        public static string PXSet2Filepath = string.Empty;
        public static bool PXSet1Filechecked = false;
        public static bool PXSet2Filechecked = false;
        public static bool loadcurves = false;
        public static string devicetype1 = string.Empty;
        public static string devicetemplate1 = string.Empty;
        public static bool device1_type_PXR25 = false;
        public static bool device1_type_PXR20 = false;
        public static string devicetype2 = string.Empty;
        public static string devicetemplate2 = string.Empty;
        public static bool device2_type_PXR25 = false;
        public static bool device2_type_PXR20 = false;
        public static System.Windows.Controls.DataGrid grdChangeSummaryReference { get; set; }
        public static Grid ActiveSetpointControlsReference { get; set; }
        public static Grid MainGridReference { get; set; }
        public Grid MainGrid = MainGridReference;
        public static Grid grdSidebarReference { get; set; }
        public static RadioButton RbSet1Reference { get; set; }
        public static RadioButton RbSet2Reference { get; set; }
        public static RadioButton RbSet3Reference { get; set; }
        public static RadioButton RbSet4Reference { get; set; }
        public static C1.WPF.C1Chart.C1Chart LSChartReference { get; set; }
        public static C1.WPF.C1Chart.C1Chart GIChartReference { get; set; }
        public static C1.WPF.C1Chart.C1Chart INSTxInChartReference { get; set; }
        public static C1.WPF.C1Chart.C1Chart MMxInChartReference { get; set; }
        public static C1.WPF.C1Chart.C1Chart LSIrChartReference { get; set; }
        private static ScrollViewer _scrollViewerContentPaneReference;
        public static ScrollViewer ScrollViewer_ContentPaneReference
        {
            get { return _scrollViewerContentPaneReference; }
            set { _scrollViewerContentPaneReference = value; }
        }
        public ICommand CancelCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ChangeActiveSetCommand { get; set; }
        public ICommand btnGroundFaultStateCommand { get; set; }
        public ICommand SaveToFileCommand { get; set; }
        public ICommand Button_ClickCommand { get; set; }
        public ICommand ExportSummaryCommand { get; set; }
        public ICommand UndoAllChangesCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand btnToggleSidebarCommand { get; set; }
        public ICommand ButtonCurvesCoordinationCommand { get; set; }
        public ICommand RbSet1234CheckedCommand { get; set; }
        private Visibility _lblDemoModeVisibility = Visibility.Collapsed;
        public Visibility LblDemoModeVisibility
        {
            get { return _lblDemoModeVisibility; }
            set
            {
                if (_lblDemoModeVisibility != value)
                {
                    _lblDemoModeVisibility = value;
                    OnPropertyChanged(nameof(LblDemoModeVisibility));
                }
            }
        }

        private string _lblDemoModeContent = Resource.DemoMode;
        public string LblDemoModeContent
        {
            get { return _lblDemoModeContent; }
            set
            {
                if (_lblDemoModeContent != value)
                {
                    _lblDemoModeContent = value;
                    OnPropertyChanged(nameof(LblDemoModeContent));
                }
            }
        }

        private bool _btnExportIsEnabled;
        public bool BtnExportIsEnabled
        {
            get { return _btnExportIsEnabled; }
            set
            {
                if (_btnExportIsEnabled != value)
                {
                    _btnExportIsEnabled = value;
                    OnPropertyChanged(nameof(BtnExportIsEnabled));
                }
            }
        }

        private double _txtSettingslocationWidth;
        public double TxtSettingslocationWidth
        {
            get { return _txtSettingslocationWidth; }
            set
            {
                if (_txtSettingslocationWidth != value)
                {
                    _txtSettingslocationWidth = value;
                    OnPropertyChanged(nameof(TxtSettingslocationWidth));
                }
            }
        }

        private Visibility _viewEditSetpointSetVisibility = Visibility.Collapsed;
        public Visibility ViewEditSetpointSetVisibility
        {
            get { return _viewEditSetpointSetVisibility; }
            set
            {
                if (_viewEditSetpointSetVisibility != value)
                {
                    _viewEditSetpointSetVisibility = value;
                    OnPropertyChanged(nameof(ViewEditSetpointSetVisibility));
                }
            }
        }

        private Visibility _lblChooseSetpointSetVisibility = Visibility.Collapsed;
        public Visibility LblChooseSetpointSetVisibility
        {
            get { return _lblChooseSetpointSetVisibility; }
            set
            {
                if (_lblChooseSetpointSetVisibility != value)
                {
                    _lblChooseSetpointSetVisibility = value;
                    OnPropertyChanged(nameof(LblChooseSetpointSetVisibility));
                }
            }
        }

        private Visibility _lblChnageActiveSetVisibility = Visibility.Collapsed;
        public Visibility LblChnageActiveSetVisibility
        {
            get { return _lblChnageActiveSetVisibility; }
            set
            {
                if (_lblChnageActiveSetVisibility != value)
                {
                    _lblChnageActiveSetVisibility = value;
                    OnPropertyChanged(nameof(LblChnageActiveSetVisibility));
                }
            }
        }

        private Visibility _btnChangeActiveSetVisibility = Visibility.Collapsed;
        public Visibility BtnChangeActiveSetVisibility
        {
            get { return _btnChangeActiveSetVisibility; }
            set
            {
                if (_btnChangeActiveSetVisibility != value)
                {
                    _btnChangeActiveSetVisibility = value;
                    OnPropertyChanged(nameof(BtnChangeActiveSetVisibility));
                }
            }
        }

        private Visibility _btnGroundFaultStateVisibility = Visibility.Collapsed;
        public Visibility BtnGroundFaultStateVisibility
        {
            get { return _btnGroundFaultStateVisibility; }
            set
            {
                if (_btnGroundFaultStateVisibility != value)
                {
                    _btnGroundFaultStateVisibility = value;
                    OnPropertyChanged(nameof(BtnGroundFaultStateVisibility));
                }
            }
        }

        private Visibility _lblChangeSummaryHeaderVisibility = Visibility.Collapsed;
        public Visibility LblChangeSummaryHeaderVisibility
        {
            get { return _lblChangeSummaryHeaderVisibility; }
            set
            {
                if (_lblChangeSummaryHeaderVisibility != value)
                {
                    _lblChangeSummaryHeaderVisibility = value;
                    OnPropertyChanged(nameof(LblChangeSummaryHeaderVisibility));
                }
            }
        }

        private Visibility _brdSelectSetpointSetVisibility;
        public Visibility BrdSelectSetpointSetVisibility
        {
            get { return _brdSelectSetpointSetVisibility; }
            set
            {
                if (_brdSelectSetpointSetVisibility != value)
                {
                    _brdSelectSetpointSetVisibility = value;
                    OnPropertyChanged(nameof(BrdSelectSetpointSetVisibility));
                }
            }
        }

        private int _viewEditSetpointSetSelectedIndex = 0;
        public int ViewEditSetpointSetSelectedIndex
        {
            get { return _viewEditSetpointSetSelectedIndex; }
            set
            {
                if (_viewEditSetpointSetSelectedIndex != value)
                {
                    _viewEditSetpointSetSelectedIndex = value;
                    OnPropertyChanged(nameof(ViewEditSetpointSetSelectedIndex));
                }
            }
        }

        private Visibility _btnSaveVisibility;
        public Visibility BtnSaveVisibility
        {
            get { return _btnSaveVisibility; }
            set
            {
                if (_btnSaveVisibility != value)
                {
                    _btnSaveVisibility = value;
                    OnPropertyChanged(nameof(BtnSaveVisibility));
                }
            }
        }

        private string _txtSettingslocationText;
        public string TxtSettingslocationText
        {
            get { return _txtSettingslocationText; }
            set
            {
                if (_txtSettingslocationText != value)
                {
                    _txtSettingslocationText = value;
                    OnPropertyChanged(nameof(TxtSettingslocationText));
                }
            }
        }

        private string _txtSettingslocationTooltip;
        public string TxtSettingslocationTooltip
        {
            get { return _txtSettingslocationTooltip; }
            set
            {
                if (_txtSettingslocationTooltip != value)
                {
                    _txtSettingslocationTooltip = value;
                    OnPropertyChanged(nameof(TxtSettingslocationTooltip));
                }
            }
        }

        private Visibility _buttonRefreshVisibility;
        public Visibility ButtonRefreshVisibility
        {
            get { return _buttonRefreshVisibility; }
            set
            {
                if (_buttonRefreshVisibility != value)
                {
                    _buttonRefreshVisibility = value;
                    OnPropertyChanged(nameof(ButtonRefreshVisibility));
                }
            }
        }
        private string _lSTxtBlockText;
        public string LSTxtBlockText
        {
            get { return _lSTxtBlockText; }
            set
            {
                if (_lSTxtBlockText != value)
                {
                    _lSTxtBlockText = value;
                    OnPropertyChanged(nameof(LSTxtBlockText));
                }
            }
        }

        private string _tabIitemi2tHeader = Resource.LongShortDelayCurves;
        public string TabIitemi2tHeader
        {
            get { return _tabIitemi2tHeader; }
            set
            {
                if (_tabIitemi2tHeader != value)
                {
                    _tabIitemi2tHeader = value;
                    OnPropertyChanged(nameof(TabIitemi2tHeader));
                }
            }
        }

        private string _tabIitemLSIrHeader = Resource.LSxIrHeader;
        public string TabIitemLSIrHeader
        {
            get { return _tabIitemLSIrHeader; }
            set
            {
                if (_tabIitemLSIrHeader != value)
                {
                    _tabIitemLSIrHeader = value;
                    OnPropertyChanged(nameof(TabIitemLSIrHeader));
                }
            }
        }

        private string _lSIrTextBlockText;
        public string LSIrTextBlockText
        {
            get { return _lSIrTextBlockText; }
            set
            {
                if (_lSIrTextBlockText != value)
                {
                    _lSIrTextBlockText = value;
                    OnPropertyChanged(nameof(LSIrTextBlockText));
                }
            }
        }

        private Visibility _tabItemGroundInstVisibility = Visibility.Collapsed;
        public Visibility TabItemGroundInstVisibility
        {
            get { return _tabItemGroundInstVisibility; }
            set
            {
                if (_tabItemGroundInstVisibility != value)
                {
                    _tabItemGroundInstVisibility = value;
                    OnPropertyChanged(nameof(TabItemGroundInstVisibility));
                }
            }
        }

        private int _tbCurvesSelectedIndex;
        public int TbCurvesSelectedIndex
        {
            get { return _tbCurvesSelectedIndex; }
            set
            {
                if (_tbCurvesSelectedIndex != value)
                {
                    _tbCurvesSelectedIndex = value;
                    OnPropertyChanged(nameof(TbCurvesSelectedIndex));
                }
            }
        }

        private Visibility _tabIitemINSTInVisibility = Visibility.Collapsed;
        public Visibility TabIitemINSTInVisibility
        {
            get { return _tabIitemINSTInVisibility; }
            set
            {
                if (_tabIitemINSTInVisibility != value)
                {
                    _tabIitemINSTInVisibility = value;
                    OnPropertyChanged(nameof(TabIitemINSTInVisibility));
                }
            }
        }

        private Visibility _tabIitemMMInVisibility;
        public Visibility TabIitemMMInVisibility
        {
            get { return _tabIitemMMInVisibility; }
            set
            {
                if (_tabIitemMMInVisibility != value)
                {
                    _tabIitemMMInVisibility = value;
                    OnPropertyChanged(nameof(TabIitemMMInVisibility));
                }
            }
        }

        private string _gITxtBlockText;
        public string GITxtBlockText
        {
            get { return _gITxtBlockText; }
            set
            {
                if (_gITxtBlockText != value)
                {
                    _gITxtBlockText = value;
                    OnPropertyChanged(nameof(GITxtBlockText));
                }
            }
        }

        private string _iNSTInTextBlockText;
        public string INSTInTextBlockText
        {
            get { return _iNSTInTextBlockText; }
            set
            {
                if (_iNSTInTextBlockText != value)
                {
                    _iNSTInTextBlockText = value;
                    OnPropertyChanged(nameof(INSTInTextBlockText));
                }
            }
        }

        private string _mMInTextBlockText;
        public string MMInTextBlockText
        {
            get { return _mMInTextBlockText; }
            set
            {
                if (_mMInTextBlockText != value)
                {
                    _mMInTextBlockText = value;
                    OnPropertyChanged(nameof(MMInTextBlockText));
                }
            }
        }

        private bool _btnRestoreDefaultIsEnabled = false;
        public bool BtnRestoreDefaultIsEnabled
        {
            get { return _btnRestoreDefaultIsEnabled; }
            set
            {
                if (_btnRestoreDefaultIsEnabled != value)
                {
                    _btnRestoreDefaultIsEnabled = value;
                    OnPropertyChanged(nameof(BtnRestoreDefaultIsEnabled));
                }
            }
        }

        private bool _btnToggleSidebarIsChecked;
        public bool BtnToggleSidebarIsChecked
        {
            get { return _btnToggleSidebarIsChecked; }
            set
            {
                if (_btnToggleSidebarIsChecked != value)
                {
                    _btnToggleSidebarIsChecked = value;
                    OnPropertyChanged(nameof(BtnToggleSidebarIsChecked));
                }
            }
        }

        private string _btnToggleSidebarTooltip = Resource.sidebarToggleButtonHide;
        public string BtnToggleSidebarTooltip
        {
            get { return _btnToggleSidebarTooltip; }
            set
            {
                if (_btnToggleSidebarTooltip != value)
                {
                    _btnToggleSidebarTooltip = value;
                    OnPropertyChanged(nameof(BtnToggleSidebarTooltip));
                }
            }
        }
        public MainScreen_ViewModel(String source,MainScreen _mainscreen)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            Mouse.UpdateCursor();
            mainscreen = _mainscreen;
            CancelCommand = new RelayCommand(Cancel);
            ExportCommand = new RelayCommand(Export);
            ChangeActiveSetCommand = new RelayCommand(ChangeActiveSet1);
            btnGroundFaultStateCommand = new RelayCommand(btnGroundFaultState);
            SaveToFileCommand = new RelayCommand(SaveToFile);
            Button_ClickCommand = new RelayCommand(Button_Click);
            ExportSummaryCommand = new RelayCommand(ExportSummary);
            UndoAllChangesCommand = new RelayCommand(UndoAllChanges);
            SaveCommand = new RelayCommand(Save);
            RefreshCommand = new RelayCommand(Refresh);
            btnToggleSidebarCommand = new RelayCommand(btnToggleSidebar1);
            ButtonCurvesCoordinationCommand = new RelayCommand(ButtonCurvesCoordination);
            //RbSet1234CheckedCommand = new RelayCommand(OnRbSet1234Checked);

            if (Global.isCommunicatingUsingBluetooth)
            {
                LblDemoModeVisibility = Visibility.Visible;
                LblDemoModeContent = Resource.Bluetooth_Mode;
            }
            else
            {
                if (Global.isDemoMode)
                {
                    LblDemoModeVisibility = Visibility.Visible;
                }
            }
            //Export option is not available for demo mode.
            BtnExportIsEnabled = Global.isDemoMode ? false : true;

            float dpiX;
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
            }

            double deskWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 96 * dpiX;
            if (deskWidth < 1600 && ((CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES"
                                      || CultureInfo.CurrentUICulture.Name == "pl-PL" || CultureInfo.CurrentUICulture.Name == "fr-CA")))
            {
                if (Global.IsOffline)
                    TxtSettingslocationWidth = 100;
                else
                    TxtSettingslocationWidth = 170;
            }
            else
                TxtSettingslocationWidth = 500;
            Global.isnewfile = false;
            Global.OldData = null;
            Global.OldOriginalsetpointLines = null;
            try
            {
                byteList = new List<byte[]>();
                grdChangeSummaryReference.ItemsSource = Changes;
                Global.strWorkFlow = source;
                Global.isMCCBExport = false;
                Global.isACB3_0Export = false;

                ViewEditSetpointSetVisibility = Visibility.Collapsed;
                LblChooseSetpointSetVisibility = Visibility.Collapsed;
                LblChnageActiveSetVisibility = Visibility.Collapsed;
                BtnChangeActiveSetVisibility = Visibility.Collapsed;
                BtnGroundFaultStateVisibility = Visibility.Collapsed;
                ActiveSetpointControlsReference.RowDefinitions[0].Height = new GridLength(0);
                LblChangeSummaryHeaderVisibility = Visibility.Collapsed;
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    LblChangeSummaryHeaderVisibility = Visibility.Visible;
                    if (!Global.IsOffline)
                    {
                        LblChooseSetpointSetVisibility = Visibility.Collapsed;
                        LblChnageActiveSetVisibility = Visibility.Visible;
                        BrdSelectSetpointSetVisibility = Visibility.Visible;
                        ViewEditSetpointSetVisibility = Visibility.Collapsed;
                        BtnChangeActiveSetVisibility = Visibility.Visible;
                        BtnGroundFaultStateVisibility = Visibility.Collapsed;
                        ActiveSetpointControlsReference.RowDefinitions[0].Height = new GridLength(40);
                    }


                    //ViewEditSetpointSet.SelectionChanged -= ViewActiveSetHeader_SelectionChanged;


                    switch (Global.PXR35_SelectedSetpointSet)
                    {
                        case "A":
                            ViewEditSetpointSetSelectedIndex = 0;
                            RbSet1Reference.Checked -= RbSet1234_Checked;
                            //RbSet1Reference.IsChecked = true;
                            //RbSet1Reference.Checked += RbSet1234_Checked;
                            break;
                        case "B":
                            ViewEditSetpointSetSelectedIndex = 1;
                            RbSet2Reference.Checked -= RbSet1234_Checked;
                            //RbSet2Reference.IsChecked = true;
                            //RbSet2Reference.Checked += RbSet1234_Checked;
                            break;
                        case "C":
                            ViewEditSetpointSetSelectedIndex = 2;
                            RbSet3Reference.Checked -= RbSet1234_Checked;
                            //RbSet3Reference.IsChecked = true;
                            //RbSet3Reference.Checked += RbSet1234_Checked;
                            break;
                        case "D":
                            ViewEditSetpointSetSelectedIndex = 3;
                            RbSet4Reference.Checked -= RbSet1234_Checked;
                            //RbSet4Reference.IsChecked = true;
                            //RbSet4Reference.Checked += RbSet1234_Checked;
                            break;
                    }
                    //ViewEditSetpointSet.SelectionChanged += ViewActiveSetHeader_SelectionChanged;
                }

                //To set correct values for Realy assignments in offline mode, Aux will be considered as ON(1).
                //Depenedencies for online and offline mode will work same. 
                if (Global.IsOffline && (Global.device_type == Global.MCCBDEVICE ||
                    Global.selectedTemplateType == Global.MCCBTEMPLATE))
                {
                    Global.auxPowerConnected = true;
                    Global.GlbstrAuxConnected = Resource.GEN11Item0001;
                    ((Settings)TripUnit.getAuxStatus()).defaultSelectionValue = Global.GlbstrAuxConnected;
                    ((Settings)TripUnit.getAuxStatus()).selectionValue = Global.GlbstrAuxConnected;
                }
                else
                {
                    Global.auxPowerConnected = false;
                }
                if (!Global.IsOffline && !Global.isDemoMode)
                {
                    Global.SetpoitGrid = grdChangeSummaryReference;
                    Global.SetpoitGridReportTimeStamp = DateTime.Now.ToString();
                    //Global.Last_CompPort = Global.portName;
                    DeviceDetailsAndSerialNumber.setAppDataForReport();
                    if (!Global.isCommunicatingUsingBluetooth)
                    {
                        deviceFirmware objGetDeviceFrimware = new deviceFirmware();
                        objGetDeviceFrimware.getFirmwareVersion();
                        do
                        {
                            Thread.Sleep(300);
                        } while (Global.deviceFirmware == String.Empty);
                    }
                    System.Threading.Thread.Sleep(500);
                }

                if (!Global.IsOffline && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
                {

                    DeviceDetailsAndSerialNumber.setAppDataForReport();

                    if (!Global.device_type_PXR10 && !Global.device_type_PXR20 && !Global.isDemoMode)
                    {
                        deviceLanguage.getDeviceLanguages();
                        System.Threading.Thread.Sleep(500);
                        do
                        {
                            Thread.Sleep(500);
                        } while (!Global.isReadAllLanguages);
                    }
                }
                SetCurveChartsWidth();
                if (source == Global.str_wizard_CONNECT)
                {

                    if (Global.device_type == Global.MCCBDEVICE)
                    {
                        //TripUnit.IDTable.Clear();
                        //TripUnit.groups.Clear();
                        //TripUnit.ID_list.Clear();
                        XMLParser.parseModelFile(Global.filePath_merged_mccb_xmlFile);
                        Global.selectedTemplateType = Global.MCCBTEMPLATE;


                    }
                    else if (Global.device_type == Global.NZMDEVICE)
                    {
                        //TripUnit.IDTable.Clear();
                        //TripUnit.groups.Clear();
                        //TripUnit.ID_list.Clear();
                        XMLParser.parseModelFile(Global.filePath_merged_nzm_xmlFile);
                        Global.selectedTemplateType = Global.NZMTEMPLATE;
                        if (!Global.IsOffline)
                        { // update ID table information for NZM language
                            Global.SetLangaugesFromDevice();
                        }

                    }
                    else if (Global.device_type == Global.PTM_DEVICE)
                    {
                        XMLParser.parseModelFile_PTM(Global.filePath_merged_PTM_xmlfile);
                        Global.selectedTemplateType = Global.PTM_TEMPLATE;
                    }
                    else if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
                    {
                        XMLParser.parseModelFile(Global.filePath_mergedstylesxmlFile_3_0);
                        Global.selectedTemplateType = Global.ACB3_0TEMPLATE;
                    }
                    else if (Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        XMLParser.parseModelFile(Global.filePath_merged_acbPXR35_xmlFile);
                        Global.selectedTemplateType = Global.ACB_PXR35_TEMPLATE;
                    }
                    else if (Global.device_type == Global.PTM_DEVICE)
                    {
                        XMLParser.parseModelFile_PTM(Global.filePath_merged_PTM_xmlfile);
                        Global.selectedTemplateType = Global.PTM_TEMPLATE;
                    }
                    else
                    {
                        XMLParser.parseModelFile(Global.filePath_mergedstylesxmlFile);
                        Global.selectedTemplateType = Global.ACBTEMPLATE;
                    }
                    Global.updateTriUnitValuesAfterParsing();

                    BtnSaveVisibility = Visibility.Collapsed;
                    //  btnBack.Visibility = Visibility.Collapsed;
                    Global.SetpoitGrid = grdChangeSummaryReference;
                    Global.SetpoitGridReportTimeStamp = DateTime.Now.ToString();
                    SourceScreen = source;
                    Global.readInData_ONLINE("Export");

                    ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                    string deviceconnected = string.Empty;
                    if (Global.device_type.Contains("ACB"))
                        deviceconnected = Global.device_type.Substring(0, 3);
                    else
                        deviceconnected = Global.device_type;
                    TxtSettingslocationText = Global.device_type == Global.NZMDEVICE ? Global.mainScreenDeviceSelectionDropdown + "  " + Resource.FirmwareVersion + ": " + Global.NZMFullFW_Version :
                                                Global.mainScreenDeviceSelectionDropdown;
                    if (Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        TxtSettingslocationText = "ACB - Magnum PXR 35";
                    }
                    TxtSettingslocationTooltip = TxtSettingslocationText;

                    //with firmware version
                    // Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Online, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);

                    //without firmware version
                    //  Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Online, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, );
                    // btnBack.Visibility = Visibility.Hidden;
                    clearCurveData();

                        foreach (String settingID in TripUnit.IDTable.Keys)
                        {
                            var set = ((Settings)TripUnit.IDTable[settingID]);
                            set.notifyDependents();
                            set.SettingValueChange -= set_SettingValueChange;
                            set.SettingValueChange += set_SettingValueChange;
                            set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                            set.CurveCalculationChanged += CurveCalculationApplyToChart;

                        }
                    

                    if (Global.selectedTemplateType == Global.NZMTEMPLATE)
                    {
                        NZMCurveCalculations.AddNZMDataToCurve();
                        NZMCurveCalculations.AddSCRNZMDataToCurve();
                    }
                    else
                    {
                        CurvesCalculation.AddDataToCurve();
                        CurvesCalculation.AddScrDataToCurve();
                    }

                    if (Global.show_curve)
                    {
                        DisplayCurve();
                        DisplaySCRCurve();
                        if (Global.device_type != Global.NZMDEVICE)
                        {
                            setScale();
                        }
                    }

                    if (Global.selectedTemplateType != Global.MCCBTEMPLATE && Global.selectedTemplateType != Global.NZMTEMPLATE)
                    {

                        SetOriginalSetpointLines();
                    }
                    else
                    {
                        SetOriginalSetpointLines();
                        Global.BackUpMCCBData = TripParser.GetConvertedSetPoints();
                    }
                    Global.GeneralSetpointsDisable(!Global.IsOffline);


                }
                else if (source == Global.str_wizard_OPENFILE)
                {
                    BtnSaveVisibility = Visibility.Visible;
                    ButtonRefreshVisibility = Visibility.Collapsed;
                    //   btnBack.Visibility = Visibility.Collapsed;
                    //  Global.deviceFirmware = "";
                    SourceScreen = source;
                    Global.readInData_ONLINE("Save");

                    //with firmware version
                    //Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Offline, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);
                    //   TripUnit.tripUnitString  = TripParser.convertDecimalToHex(TripUnit.tripUnitIndexArray);
                    ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                    if (!String.IsNullOrWhiteSpace(Global.fileNameForSaveOption))
                    {
                        string filecreatedFrom = string.Empty;
                        if (Global.device_type == Global.ACB_PXR35_DEVICE)
                        {
                            string SetName = Global.PXR35_SelectedSetpointSet == "A" ? Resource.SetpointSet1 :
                                                                                                             Global.PXR35_SelectedSetpointSet == "B" ? Resource.SetpointSet2 :
                                                                                                             Global.PXR35_SelectedSetpointSet == "C" ? Resource.SetpointSet3 : Resource.SetpointSet4;
                            filecreatedFrom = "  Created from Setpoint" + SetName;
                        }

                        TxtSettingslocationText = Global.fileNameForSaveOption.Split('\\')[Global.fileNameForSaveOption.Split('\\').Count() - 1] + filecreatedFrom;
                        TxtSettingslocationTooltip = TxtSettingslocationText;
                    }
                    //without firmware version
                    // Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Offline, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);
                    //  btnBack.Visibility = Visibility.Hidden;
                    clearCurveData();
                    foreach (String settingID in TripUnit.IDTable.Keys)
                    {
                        var set = ((Settings)TripUnit.IDTable[settingID]);
                        set.notifyDependents();

                        set.SettingValueChange -= set_SettingValueChange;
                        set.SettingValueChange += set_SettingValueChange;
                        set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                        set.CurveCalculationChanged += CurveCalculationApplyToChart;
                    }

                    if (Global.selectedTemplateType == Global.NZMTEMPLATE)
                    {
                        NZMCurveCalculations.AddNZMDataToCurve();
                        NZMCurveCalculations.AddSCRNZMDataToCurve();
                    }
                    else
                    {
                        CurvesCalculation.AddDataToCurve();
                        CurvesCalculation.AddScrDataToCurve();
                    }

                    if (Global.show_curve)
                    {
                        DisplayCurve();
                        DisplaySCRCurve();
                        if (Global.device_type != Global.NZMDEVICE)
                        {
                            setScale();
                        }
                    }
                    Global.updateExpandersVisibility();

                    //if (Global.selectedTemplateType != Global.MCCBTEMPLATE)
                    //{
                    //    DisplayCurve();
                    //}
                    Global.GeneralSetpointsDisable(!Global.IsOffline);
                    Global.isnewfile = true;
                }

                else if (source == Global.str_wizard_NEWFILE)
                {
                    Global.isnewfile = true;
                    // btnBack.Visibility = Visibility.Visible;
                    BtnSaveVisibility = Visibility.Collapsed;
                    ButtonRefreshVisibility = Visibility.Collapsed;

                    //Global.deviceFirmware = "";
                    SourceScreen = source;
                    ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                    TxtSettingslocationText = Resource.NewFile;
                    TxtSettingslocationTooltip = TxtSettingslocationText;
                    //  btnBack.Visibility = Visibility.Visible;
                    clearCurveData();
                    foreach (String settingID in TripUnit.IDTable.Keys)
                    {
                        var set = ((Settings)TripUnit.IDTable[settingID]);
                        set.notifyDependents();

                        set.SettingValueChange -= set_SettingValueChange;
                        set.SettingValueChange += set_SettingValueChange;
                        set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                        set.CurveCalculationChanged += CurveCalculationApplyToChart;
                        //if (Global.selectedTemplateType != Global.MCCBTEMPLATE)
                        //{
                        //set.addDataToCurveCollection -= DisplayCurve;
                        //set.addDataToCurveCollection += DisplayCurve;
                        //set.AddDataToCurve();
                        //  }
                    }
                    if (Global.selectedTemplateType == Global.NZMTEMPLATE)
                    {
                        NZMCurveCalculations.AddNZMDataToCurve();
                        NZMCurveCalculations.AddSCRNZMDataToCurve();
                    }
                    else
                    {
                        CurvesCalculation.AddDataToCurve();
                        CurvesCalculation.AddScrDataToCurve();
                    }

                    if (Global.show_curve)
                    {
                        DisplayCurve();
                        DisplaySCRCurve();
                        if (Global.selectedTemplateType == Global.MCCBTEMPLATE || Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
                        {
                            setScale();
                        }
                    }
                    Global.updateExpandersVisibility();
                    // Export goes back to main screen. 
                    //if (source != Global.str_wizard_EXPORTFILE || source != Global.str_wizard_EXPORTUI)
                    //{
                    //with firmware version
                    //Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Offline, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);

                    //without firmware version
                    //Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Offline, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);
                    //}

                    Global.GeneralSetpointsDisable(!Global.IsOffline);
                    if (TripUnit.userStyle == null && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE))
                    {
                        var setpoint = TripUnit.getTripUnitStyle();
                        TripUnit.userStyle = setpoint.selectionValue.ToString();
                    }

                }
                else if (source == Global.str_wizard_EXPORTFILE)
                {
                    BtnSaveVisibility = Visibility.Collapsed;
                    // btnBack.Visibility = Visibility.Collapsed;

                    ButtonRefreshVisibility = Visibility.Visible;
                    SourceScreen = source;
                    Global.readInData_ONLINE("Export");
                    ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);

                    //with firmware version
                    //Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Online, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.deviceFirmware);

                    //without firmware version
                    //Label_subtitle.Content = string.Format(Resource.MainScreenSubTitle, Resource.Online, TripUnit.userStyle, TripUnit.userRatingPlug, TripUnit.userBreakerInformation, Global.FirmwareFinal);
                    string deviceconnected = string.Empty;
                    if (Global.device_type.Contains("ACB"))
                        deviceconnected = Global.device_type.Substring(0, 3);
                    else
                        deviceconnected = Global.device_type;
                    //txtSettingslocation.Text = Resource.DeviceText + " " + Global.portName + " - " + deviceconnected;
                    //txtSettingslocation.ToolTip = txtSettingslocation.Text;

                    TxtSettingslocationText = Global.device_type == Global.NZMDEVICE ? Global.mainScreenDeviceSelectionDropdown + "  " + Resource.FirmwareVersion + ": " + Global.NZMFullFW_Version :
                                                Global.mainScreenDeviceSelectionDropdown;
                    TxtSettingslocationTooltip = TxtSettingslocationText;

                    clearCurveData();
                    foreach (String settingID in TripUnit.IDTable.Keys)
                    {
                        var set = ((Settings)TripUnit.IDTable[settingID]);
                        set.notifyDependents();

                        set.SettingValueChange -= set_SettingValueChange;
                        set.SettingValueChange += set_SettingValueChange;
                        set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                        set.CurveCalculationChanged += CurveCalculationApplyToChart;
                        //if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                        //{
                        //set.addDataToCurveCollection -= DisplayCurve;
                        //set.addDataToCurveCollection += DisplayCurve;
                        //set.AddDataToCurve();
                        // }
                    }
                    //if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                    //{

                    if (Global.selectedTemplateType == Global.NZMTEMPLATE)
                    {
                        NZMCurveCalculations.AddNZMDataToCurve();
                        NZMCurveCalculations.AddSCRNZMDataToCurve();
                    }
                    else
                    {
                        CurvesCalculation.AddDataToCurve();
                        CurvesCalculation.AddScrDataToCurve();
                    }

                    if (Global.show_curve)
                    {
                        DisplayCurve();
                        DisplaySCRCurve();
                        if (Global.device_type != Global.NZMDEVICE)
                        {
                            setScale();
                        }
                    }

                    //PXPM-7011  Archana
                    //Not showing refresh pop up msg on export button even after we did any changes in device.
                    //Cause of issue - After str_wizard_EXPORTFILE  control fow old setoints data was not getting updated with newly updated trip unit data - hence showing refresh msg
                    SetOriginalSetpointLines();

                }

                //   Global.updateRelaysAndModbus();

                Global.updateExpandersVisibility();
                Global.GeneralSetpointsDisable(!Global.IsOffline);
                //this.Height = System.Windows.SystemParameters.WorkArea.Height;
                if (Global.listGroupsAsFoundSetPoint != null)
                {
                    Global.listGroupsAsFoundSetPoint.Clear();
                }
                PopulateAsFoundData();
            }

            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox("", " : " + ex.Message, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();

            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static void PopulateAsFoundData()
        {
            //Global.listGroupsAsFoundSetPoint.Add(TripUnit.groups);
            //ArrayList tempAsFound = (ArrayList)TripUnit.groups.Clone();
            ArrayList tempAsFound = new ArrayList();
            foreach (Group item in TripUnit.groups)
            {
                //Group tempGroup = new Group(item);
                Group itemGroup = (Group)item;
                int setpointCount = itemGroup.groupSetPoints.Count();
                string groupName = itemGroup.name;
                string groupId = itemGroup.ID;
                Group tempGroup = new Group(setpointCount, groupId, groupName);
                if (itemGroup.subgroups != null)
                {
                    int subgroupConut = itemGroup.subgroups.Count();
                    tempGroup = new Group(groupId, groupName, subgroupConut, setpointCount, null, null);
                    int i = 0;
                    foreach (Group itemSubgroup in itemGroup.subgroups)
                    {
                        Group tempSubgroup = new Group(itemSubgroup.groupSetPoints.Count(), itemSubgroup.ID, itemSubgroup.name);

                        //subgroup having subgroup
                        if (itemSubgroup.subgroups != null)
                        {

                            int j = 0;
                            foreach (var itemSubSubgroup in itemSubgroup.subgroups)
                            {
                                if (j == 0)
                                {
                                    int setpointCount1 = itemSubgroup.groupSetPoints.Count();
                                    string groupName1 = itemSubgroup.name;
                                    string groupId1 = itemSubgroup.ID;
                                    int subgroupConut1 = itemSubgroup.subgroups.Count();
                                    tempSubgroup = new Group(groupId1, groupName1, subgroupConut1, setpointCount1, null, null);
                                }
                                Group tempSubSubgroup = new Group(itemSubSubgroup.groupSetPoints.Count(), itemSubSubgroup.ID, itemSubSubgroup.name);
                                if (itemSubSubgroup.groupSetPoints.Length > 0)
                                {
                                    int index = 0;
                                    foreach (Settings grpSetting in itemSubSubgroup.groupSetPoints)
                                    {
                                        tempSubSubgroup.groupSetPoints[index] = ((Settings)TripUnit.IDTable[grpSetting.ID]).ShallowCopy();
                                        index++;
                                    }
                                }
                                tempSubgroup.subgroups[j] = tempSubSubgroup;
                                j++;
                            }

                        }

                        if (itemSubgroup.groupSetPoints.Length > 0)
                        {
                            int index = 0;
                            foreach (Settings grpSetting in itemSubgroup.groupSetPoints)
                            {
                                tempSubgroup.groupSetPoints[index] = ((Settings)TripUnit.IDTable[grpSetting.ID]).ShallowCopy();
                                index++;
                            }
                        }
                        tempGroup.subgroups[i] = tempSubgroup;
                        i++;
                    }
                }
                if (itemGroup.groupSetPoints != null)
                {
                    if (itemGroup.groupSetPoints.Length > 0)
                    {
                        int index = 0;
                        foreach (Settings grpSetting in itemGroup.groupSetPoints)
                        {
                            tempGroup.groupSetPoints[index] = ((Settings)TripUnit.IDTable[grpSetting.ID]).ShallowCopy();
                            index++;
                        }
                    }
                }

                tempAsFound.Add(tempGroup);
            }
            Global.listGroupsAsFoundSetPoint.Add(tempAsFound);
        }
        private void clearCurveData()
        {
            CurvesCalculation.LDPickUp.Clear();
            CurvesCalculation.LDTime.Clear();
            CurvesCalculation.SDPickUp.Clear();
            CurvesCalculation.SDTimeI2T.Clear();
            CurvesCalculation.SDTimeFlat.Clear();
            CurvesCalculation.IEEE.Clear();
            CurvesCalculation.IEC.Clear();
            CurvesCalculation.GFPickup.Clear();
            CurvesCalculation.InstPickup.Clear();

            CurvesCalculation.SCR_LDPickUp.Clear();
            CurvesCalculation.SCR_SDPickUp.Clear();
            CurvesCalculation.SCR_IEEE.Clear();
            CurvesCalculation.SCR_IEC.Clear();
            CurvesCalculation.SCR_GFPickup.Clear();
            CurvesCalculation.SCR_InstPickup.Clear();
        }
        public void Addcurves()
        {
            if (loadcurves)
            {
                if (!String.IsNullOrWhiteSpace(PXSet1Filepath) && PXSet1Filechecked)
                {
                    if (devicetype1 == Global.NZMDEVICE)
                    {
                        NZMCurveCalculations.AddFile1NZMDataToCurve(devicetype1, devicetemplate1, device1_type_PXR25, device1_type_PXR20, Global.str_pxset1_ID_Table);
                    }
                    else
                    {
                        CurvesCalculation.AddFile1DataToCurve(devicetype1, devicetemplate1, Global.str_pxset1_ID_Table);
                    }
                    if (Global.show_curve)
                    {
                        DisplayFileCurve1(devicetype1);
                        setAdditionalCurvesScale();
                    }
                }
                if (!String.IsNullOrWhiteSpace(PXSet2Filepath) && PXSet2Filechecked)
                {
                    if (devicetype2 == Global.NZMDEVICE)
                    {
                        NZMCurveCalculations.AddFile2NZMDataToCurve(devicetype2, devicetemplate2, device2_type_PXR25, device2_type_PXR20, Global.str_pxset2_ID_Table);
                    }
                    else
                    {
                        CurvesCalculation.AddFile2DataToCurve(devicetype2, devicetemplate2, Global.str_pxset2_ID_Table);
                    }
                    if (Global.show_curve)
                    {
                        DisplayFileCurve2(devicetype2);
                        setAdditionalCurvesScale();
                    }
                }
            }
        }
        /// <summary>
        /// Common function to set properties for different Data Series
        /// </summary>
        private void SetXYSeriesProperties(ref XYDataSeries objSeries, double thickness, Color color, DoubleCollection dashCollection = null)
        {
            objSeries.XValueBinding = new Binding("X");
            objSeries.ValueBinding = new Binding("Y");
            if (thickness != 0)
                objSeries.ConnectionStrokeThickness = thickness;
            objSeries.ConnectionStroke = new SolidColorBrush(color);
            if (dashCollection != null)
                objSeries.ConnectionStrokeDashes = dashCollection;
            objSeries.PlotElementLoaded += DataSeries_PlotElementLoaded;
        }
        public void DisplayCurve()
        {
            CurveData curvedata = new CurveData
            {
                GF_Enabled = CurvesCalculation.GF_Enabled,
                isGFActionoff = CurvesCalculation.isGFActionoff,
                IN_Enabled = CurvesCalculation.IN_Enabled,
                GFXMax = CurvesCalculation.GFXMax,
                GFXMin = CurvesCalculation.GFXMin,
                GFYMax = CurvesCalculation.GFYMax,
                GFYMin = CurvesCalculation.GFYMin,
                GFPU_ToSetAxis = CurvesCalculation.GFPU_ToSetAxis,
                In_ToSetAxis = CurvesCalculation.In_ToSetAxis,
                IsMMStateOn = CurvesCalculation.IsMMStateOn,
                MM_Enabled = CurvesCalculation.MM_Enabled,
                MMXMax = CurvesCalculation.MMXMax,
                MMXmin = CurvesCalculation.MMXmin,
                InstXMax = CurvesCalculation.InstXMax,
                InstXmin = CurvesCalculation.InstXmin,
                LSIAxMax = CurvesCalculation.LSIAxMax,
                LSIAXMin = CurvesCalculation.LSIAXMin,
                LSIrXMax = CurvesCalculation.LSIrXMax,
                LSIrXmin = CurvesCalculation.LSIrXmin
            };
            DisplayTripCurves(true, false, curvedata, CurvesCalculation.LDPickUp, CurvesCalculation.SDPickUp, CurvesCalculation.InstPickup, CurvesCalculation.LD2_TU,
                              CurvesCalculation.GFPickup, CurvesCalculation.MMLevel, CurvesCalculation.LSIr, CurvesCalculation.LSIrSDPickup, CurvesCalculation.LS_LD2_TU,
                              CurvesCalculation.InstxIn, CurvesCalculation.MMxIn);
        }
        public void DisplaySCRCurve()
        {
            CurveData curvedata = new CurveData
            {
                GF_Enabled = CurvesCalculation.GF_Enabled,
                isGFActionoff = CurvesCalculation.isGFActionoff,
                IN_Enabled = CurvesCalculation.IN_Enabled,
                GFXMax = CurvesCalculation.GFXMax,
                GFXMin = CurvesCalculation.GFXMin,
                GFYMax = CurvesCalculation.GFYMax,
                GFYMin = CurvesCalculation.GFYMin,
                GFPU_ToSetAxis = CurvesCalculation.GFPU_ToSetAxis,
                In_ToSetAxis = CurvesCalculation.In_ToSetAxis,
                IsMMStateOn = CurvesCalculation.IsMMStateOn,
                MM_Enabled = CurvesCalculation.MM_Enabled,
                MMXMax = CurvesCalculation.MMXMax,
                MMXmin = CurvesCalculation.MMXmin,
                InstXMax = CurvesCalculation.InstXMax,
                InstXmin = CurvesCalculation.InstXmin,
                LSIAxMax = CurvesCalculation.LSIAxMax,
                LSIAXMin = CurvesCalculation.LSIAXMin,
                LSIrXMax = CurvesCalculation.LSIrXMax,
                LSIrXmin = CurvesCalculation.LSIrXmin
            };
            ObservableCollection<Point> LS_LD2_TU = new ObservableCollection<Point>();
            DisplayTripCurves(false, true, curvedata, CurvesCalculation.SCR_LDPickUp, CurvesCalculation.SCR_SDPickUp, CurvesCalculation.SCR_InstPickup, CurvesCalculation.SCR_LD2_TU,
                              CurvesCalculation.SCR_GFPickup, CurvesCalculation.SCR_MMLevel, CurvesCalculation.SCR_LSIr, CurvesCalculation.SCR_LSIrSDPickup, LS_LD2_TU,
                              CurvesCalculation.SCR_InstxIn, CurvesCalculation.SCR_MMxIn);
        }
        public void DisplayFileCurve1(string device_type)
        {
            CurveData curvedata = new CurveData
            {
                GF_Enabled = CurvesCalculation.File1_GF_Enabled,
                isGFActionoff = CurvesCalculation.File1_isGFActionoff,
                IN_Enabled = CurvesCalculation.File1_IN_Enabled,
                GFXMax = CurvesCalculation.GFXMax,
                GFXMin = CurvesCalculation.GFXMin,
                GFYMax = CurvesCalculation.GFYMax,
                GFYMin = CurvesCalculation.GFYMin,
                GFPU_ToSetAxis = CurvesCalculation.File1_GFPU_ToSetAxis,
                In_ToSetAxis = CurvesCalculation.File1_In_ToSetAxis
            };
            Color colorvalue = Colors.LightGreen;
            string PXsetFilename = PXset1Filename;
            DisplayAdditionalCurves(device_type, colorvalue, PXsetFilename, curvedata, CurvesCalculation.File1_LDPickUp, CurvesCalculation.File1_SDPickUp, CurvesCalculation.File1_InstPickup,
                                    CurvesCalculation.File1_LD2_TU, CurvesCalculation.File1_GFPickup);
        }
        public void DisplayFileCurve2(string device_type)
        {
            CurveData curvedata = new CurveData
            {
                GF_Enabled = CurvesCalculation.File2_GF_Enabled,
                isGFActionoff = CurvesCalculation.File2_isGFActionoff,
                IN_Enabled = CurvesCalculation.File2_IN_Enabled,
                GFXMax = CurvesCalculation.GFXMax,
                GFXMin = CurvesCalculation.GFXMin,
                GFYMax = CurvesCalculation.GFYMax,
                GFYMin = CurvesCalculation.GFYMin,
                GFPU_ToSetAxis = CurvesCalculation.File2_GFPU_ToSetAxis,
                In_ToSetAxis = CurvesCalculation.File2_In_ToSetAxis
            };
            Color colorvalue = Colors.DarkGreen;
            string PXsetFilename = PXset2Filename;
            DisplayAdditionalCurves(device_type, colorvalue, PXsetFilename, curvedata, CurvesCalculation.File2_LDPickUp, CurvesCalculation.File2_SDPickUp, CurvesCalculation.File2_InstPickup,
                                    CurvesCalculation.File2_LD2_TU, CurvesCalculation.File2_GFPickup);
        }

        /// <summary>
        /// Common function for additional curves display
        /// </summary>
        public void DisplayAdditionalCurves(string device_type, Color colorvalue, string PXsetFilename, CurveData curvedata, ObservableCollection<Point> File_LDPickUp, ObservableCollection<Point> File_SDPickUp,
                                            ObservableCollection<Point> File_InstPickup, ObservableCollection<Point> File_LD2_TU, ObservableCollection<Point> File_GFPickup)
        {
            XYDataSeries File_series1 = new XYDataSeries();
            XYDataSeries File_series2 = new XYDataSeries();
            XYDataSeries File_series3 = new XYDataSeries();
            XYDataSeries File_series4 = new XYDataSeries();
            XYDataSeries File_series5 = new XYDataSeries();
            XYDataSeries File_series6 = new XYDataSeries();
            XYDataSeries File_series7 = new XYDataSeries();
            XYDataSeries File_series8 = new XYDataSeries();
            double Curvethickness = 2;
            var dblDash = new double[] { 3, 3 };
            var dashCollection = new DoubleCollection(dblDash);


            if (File_LDPickUp.Count >= 0)
            {
                File_series1.ItemsSource = File_LDPickUp;
                SetXYSeriesProperties(ref File_series1, Curvethickness, colorvalue);
                File_series1.Display = SeriesDisplay.HideLegend;
                LSChartReference.Data.Children.Add(File_series1);
                //set series for Additional curve legends
                if (File_LDPickUp.Count == 0 && File_SDPickUp.Count == 0 && File_InstPickup.Count == 0 && File_LD2_TU.Count == 0)
                {
                    //Hide additional curve legend only when none of the Long,short,Inst data available
                    File_series7.Display = SeriesDisplay.HideLegend;
                }
                File_series7.Label = PXsetFilename;
                SetXYSeriesProperties(ref File_series7, Curvethickness, colorvalue, dashCollection);
                LSChartReference.Data.Children.Add(File_series7);
            }
            if (File_SDPickUp.Count >= 0)
            {
                File_series2.ItemsSource = File_SDPickUp;
                SetXYSeriesProperties(ref File_series2, Curvethickness, colorvalue);
                File_series2.Display = SeriesDisplay.HideLegend;
                LSChartReference.Data.Children.Add(File_series2);
            }
            if (File_InstPickup.Count >= 0)
            {
                File_series3.ItemsSource = File_InstPickup;
                SetXYSeriesProperties(ref File_series3, Curvethickness, colorvalue);
                File_series3.Display = SeriesDisplay.HideLegend;
                if (curvedata.IN_Enabled)
                {
                    LSChartReference.Data.Children.Add(File_series3);
                }
            }
            if (File_LD2_TU.Count >= 0)
            {
                File_series4.ItemsSource = File_LD2_TU;
                SetXYSeriesProperties(ref File_series4, Curvethickness, colorvalue);
                File_series4.Display = SeriesDisplay.HideLegend;
                LSChartReference.Data.Children.Add(File_series4);
            }
            if (File_GFPickup.Count >= 0)
            {
                File_series5.ItemsSource = File_GFPickup;
                SetXYSeriesProperties(ref File_series5, Curvethickness, colorvalue);
                File_series5.Display = SeriesDisplay.HideLegend;
                if (curvedata.GF_Enabled && !curvedata.isGFActionoff)
                {
                    GIChartReference.Data.Children.Add(File_series5);
                }
                //set series for Additional curve legends
                if (File_GFPickup.Count == 0)
                {
                    File_series8.Display = SeriesDisplay.HideLegend;
                }
                File_series8.Label = PXsetFilename;
                SetXYSeriesProperties(ref File_series8, Curvethickness, colorvalue, dashCollection);
                GIChartReference.Data.Children.Add(File_series8);
            }
            if (device_type == Global.NZMDEVICE)
            {
                if (!Double.IsNaN(LSChartReference.View.AxisY.Min))
                    LSChartReference.View.AxisY.Min = Math.Min(LSChartReference.View.AxisY.Min, 0.001);
                else
                    LSChartReference.View.AxisY.Min = 0.001;

                if (!Double.IsNaN(LSChartReference.View.AxisY.Max))
                    LSChartReference.View.AxisY.Max = Math.Max(LSChartReference.View.AxisY.Max, 10000);
                else
                    LSChartReference.View.AxisY.Max = 10000;
            }
            else
            {
                if (!Double.IsNaN(LSChartReference.View.AxisY.Min))
                    LSChartReference.View.AxisY.Min = Math.Min(LSChartReference.View.AxisY.Min, 0.01);
                else
                    LSChartReference.View.AxisY.Min = 0.01;

                if (!Double.IsNaN(LSChartReference.View.AxisY.Max))
                    LSChartReference.View.AxisY.Max = Math.Max(LSChartReference.View.AxisY.Max, 100000);
                else
                    LSChartReference.View.AxisY.Max = 100000;
            }
            if (curvedata.GF_Enabled && !curvedata.isGFActionoff)
            {
                if (device_type == Global.NZMDEVICE)
                {
                    if (!Double.IsNaN(GIChartReference.View.AxisX.Min))
                        GIChartReference.View.AxisX.Min = Math.Min(GIChartReference.View.AxisX.Min, curvedata.GFXMin);
                    else
                        GIChartReference.View.AxisX.Min = curvedata.GFXMin;

                    if (!Double.IsNaN(GIChartReference.View.AxisX.Max))
                        GIChartReference.View.AxisX.Max = Math.Max(GIChartReference.View.AxisX.Max, curvedata.GFXMax);
                    else
                        GIChartReference.View.AxisX.Max = curvedata.GFXMax;


                    if (!Double.IsNaN(GIChartReference.View.AxisY.Min))
                        GIChartReference.View.AxisY.Min = Math.Min(GIChartReference.View.AxisY.Min, curvedata.GFYMin);
                    else
                        GIChartReference.View.AxisY.Min = curvedata.GFYMin;

                    if (!Double.IsNaN(GIChartReference.View.AxisY.Max))
                        GIChartReference.View.AxisY.Max = Math.Max(GIChartReference.View.AxisY.Max, curvedata.GFYMax);
                    else
                        GIChartReference.View.AxisY.Max = curvedata.GFYMax;
                }
                else
                {
                    double GFmin = Math.Pow(10, Math.Floor(Math.Log10((double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))));
                    //set GFmin scale
                    if (Global.isDemoMode && !Global.IsOffline)
                    {
                        if (GFmin == (double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))
                            GFmin = GFmin / 10;

                        if (File_GFPickup.Count > 0)
                        {
                            double GFminval = File_GFPickup.Min(m => m.X);
                            double GFcurvemin = Math.Pow(10, Math.Floor(Math.Log10((double)(GFminval))));
                            if (GFcurvemin == GFminval)
                                GFcurvemin = GFcurvemin / 10;
                            if (GFmin != GFcurvemin && GFminval != 0)
                                GFmin = GFcurvemin;
                        }
                    }
                    if (GIChartReference.View.AxisX.Min != 0 && !Double.IsNaN(GIChartReference.View.AxisX.Min) && !Double.IsNaN(GFmin))
                        GIChartReference.View.AxisX.Min = Math.Min(GIChartReference.View.AxisX.Min, GFmin);
                    else
                        GIChartReference.View.AxisX.Min = GFmin;

                    //set GFmax scale
                    double GFmax = Math.Pow(10, Math.Ceiling(Math.Log10((double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))));
                    if (GFmax == (double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))
                        GFmax = GFmax * 10;

                    if (File_GFPickup.Count > 0)
                    {
                        double GFmaxval = File_GFPickup.Max(m => m.X);
                        double GFcurvemax = Math.Pow(10, Math.Ceiling(Math.Log10((double)(GFmaxval))));
                        if (GFcurvemax == GFmaxval)
                            GFcurvemax = GFcurvemax * 10;
                        if (GFmax != GFcurvemax && GFmaxval != 0)
                            GFmax = GFcurvemax;
                    }
                    if (GIChartReference.View.AxisX.Max != 0 && !Double.IsNaN(GIChartReference.View.AxisX.Max) && !Double.IsNaN(GFmax))
                        GIChartReference.View.AxisX.Max = Math.Max(GIChartReference.View.AxisX.Max, GFmax);
                    else
                        GIChartReference.View.AxisX.Max = GFmax;
                }
            }
        }
        /// <summary>
        /// Common function for Original and Screen trip curves display 
        /// </summary>
        public void DisplayTripCurves(bool isCurve, bool isSCRcurve, CurveData curvedata, ObservableCollection<Point> LDPickUp, ObservableCollection<Point> SDPickUp,
                                      ObservableCollection<Point> InstPickup, ObservableCollection<Point> LD2_TU, ObservableCollection<Point> GFPickup, ObservableCollection<Point> MMLevel,
                                      ObservableCollection<Point> LSIr, ObservableCollection<Point> LSIrSDPickup, ObservableCollection<Point> LS_LD2_TU, ObservableCollection<Point> InstxIn, ObservableCollection<Point> MMxIn)
        {
            bool addcurvedata = true;//Manages curve data visibility
            bool isMCCBPXR20 = false;
            bool showlegend = false;
            double Curvethickness = 0;
            DoubleCollection dashCollection = null;
            FontFamily[] ff = new FontFamily[] { new FontFamily("OpenSansRegular") };
            Settings Mtrsetting = TripUnit.getMotorProtectionGeneralGrp();
            //Exclude 'short' delay text from curves for NZM devices (PXR10 and motor styles), as short delay not available.
            bool excludeShortdelay = false;
            Settings setDeviceType = null;
            string TripUnitType = string.Empty;
            setDeviceType = TripUnit.getTripUnitType();
            if (setDeviceType != null)
                TripUnitType = setDeviceType.selectionValue;
            if (Global.device_type == Global.NZMDEVICE)
            {
                if (!string.IsNullOrWhiteSpace(TripUnitType))
                {
                    if ((TripUnitType == Resource.GEN1Item0000) || (TripUnitType == Resource.GEN1Item0001) || (TripUnitType == Resource.GEN1Item0005))
                        excludeShortdelay = true;
                }
            }
            //For NZM:Below variable used to add 3 dots between 50k and 100k Amp Current for last curve data in LS Chart
            ObservableCollection<Point> Dottedpoints = new ObservableCollection<Point>();
            if (isSCRcurve)
            {
                bool isdevicePXR25 = true;
                if ((Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
                {
                    if (!string.IsNullOrWhiteSpace(TripUnitType))
                    {
                        if (!(TripUnitType == Resource.GEN01Item0003 || TripUnitType == Resource.GEN01Item0002 ||
                              (!Global.IsOffline && Global.device_type == Global.MCCBDEVICE && TripUnitType == Resource.GEN01Item0001) ||
                              TripUnitType == Resource.GEN1Item0004 || TripUnitType == Resource.GEN1Item0005 ||
                              TripUnitType == Resource.GEN1Item0006 || TripUnitType == Resource.GEN1Item0007))
                            isdevicePXR25 = false;
                    }
                }
                if (!Global.IsOffline && TripUnitType == Resource.GEN01Item0001 && (Global.device_type == Global.MCCBDEVICE))
                    isMCCBPXR20 = true;
                //For PXR10,PXR20 and ACB devices Version<3.0, PXR20 ACB 3.0 in offline, Manage curve tabs visibility and scaling, only hide curve data.
                if (isdevicePXR25 == false || ((Global.IsOffline) && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE) || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && TripUnitType == Resource.GEN01Item0001)))
                    addcurvedata = false;

                //For PT 10 and PT 10G do not display offline curves JiraID - PXPM-9773
                if ((Global.IsOffline) && (Global.device_type == Global.PTM_DEVICE) && (TripUnitType == Resource.GEN001TItem0003))
                    addcurvedata = false;

            }

            XYDataSeries series1 = new XYDataSeries();
            XYDataSeries series2 = new XYDataSeries();
            XYDataSeries series3 = new XYDataSeries();
            XYDataSeries series4 = new XYDataSeries();
            XYDataSeries series5 = new XYDataSeries();
            XYDataSeries series6 = new XYDataSeries();
            XYDataSeries series7 = new XYDataSeries();
            XYDataSeries series8 = new XYDataSeries();
            XYDataSeries series9 = new XYDataSeries();
            XYDataSeries series10 = new XYDataSeries();
            XYDataSeries series11 = new XYDataSeries();
            XYDataSeries series12 = new XYDataSeries();
            XYDataSeries series13 = new XYDataSeries();
            XYDataSeries series14 = new XYDataSeries();
            XYDataSeries series15 = new XYDataSeries();
            XYDataSeries series16 = new XYDataSeries();
            XYDataSeries series17 = new XYDataSeries();
            XYDataSeries series18 = new XYDataSeries();
            XYDataSeries series19 = new XYDataSeries();
            XYDataSeries series20 = new XYDataSeries();
            XYDataSeries series21 = new XYDataSeries();
            XYDataSeries series22 = new XYDataSeries();

            if (!Global.IsOffline && (Global.device_type_PXR10 || (Global.device_type == Global.NZMDEVICE && (!string.IsNullOrWhiteSpace(TripUnitType)) && TripUnitType.Contains("PXR20"))))
                showlegend = true;//set flag to show solid legend for curves when setpoint change from screen does not impact curve
            if (isCurve)
            {
                Curvethickness = 2.7;
                dashCollection = null;
                LSChartReference.Reset(true);
                LSChartReference.BeginUpdate();
                LSChartReference.ChartType = ChartType.Line;

                INSTxInChartReference.Reset(true);
                INSTxInChartReference.BeginUpdate();
                INSTxInChartReference.ChartType = ChartType.Line;

                MMxInChartReference.Reset(true);
                MMxInChartReference.BeginUpdate();
                MMxInChartReference.ChartType = ChartType.Line;
            }
            else
            {
                var dblDash = new double[] { 3, 3 };
                dashCollection = new DoubleCollection(dblDash);
                Curvethickness = 2;
                if (isMCCBPXR20 == true)
                {
                    dashCollection = null;
                    Curvethickness = 2.7;
                }
            }
            if (LDPickUp.Count >= 0)
            {
                series1.ItemsSource = LDPickUp;
                SetXYSeriesProperties(ref series1, Curvethickness, Colors.Gold, dashCollection);
                if ((isCurve && showlegend == true && LDPickUp.Count > 0) || isSCRcurve)
                {
                    if (Mtrsetting != null && Mtrsetting.bValue)
                        series1.Label = Resource.Overload;
                    else
                        series1.Label = Resource.LongDelayPickupLegendIr;
                }
                if (isCurve)
                {
                    if (showlegend == false || LDPickUp.Count == 0)
                    {
                        series1.Display = SeriesDisplay.HideLegend;
                    }
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        //show solid legend in case of PXR10,NZM PXR20
                        if ((LDPickUp.Count == 0 && CurvesCalculation.LDPickUp.Count == 0) || (showlegend && CurvesCalculation.LDPickUp.Count != 0)) //for online mode hide dotted legend if showlegend flag true 
                        {
                            series1.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (LDPickUp.Count == 0)
                            series1.Display = SeriesDisplay.HideLegend;
                    }
                }

                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                {
                    LSChartReference.Data.Children.Add(series1);
                    if (Global.device_type == Global.NZMDEVICE && LDPickUp.Count != 0)
                    {
                        double LDpickupmax = LDPickUp.Max(x => x.X);
                        if (LDpickupmax >= 50000)
                        {
                            if (LDpickupmax < 60000)
                                Dottedpoints.Add(new Point(60000, LDPickUp.Min(x => x.Y)));
                            if (LDpickupmax < 70000)
                                Dottedpoints.Add(new Point(70000, LDPickUp.Min(x => x.Y)));
                            if (LDpickupmax < 80000)
                                Dottedpoints.Add(new Point(80000, LDPickUp.Min(x => x.Y)));
                            series22.ItemsSource = Dottedpoints;
                            SetXYSeriesProperties(ref series22, 0, Colors.Transparent);
                            series22.SymbolMarker = Marker.Dot;
                            series22.SymbolSize = new Size(5, 5);
                            series22.SymbolFill = new SolidColorBrush(Colors.Gold);
                            series22.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                            series22.Display = SeriesDisplay.HideLegend;
                            LSChartReference.Data.Children.Add(series22);
                        }
                    }
                }
            }
            if (SDPickUp.Count >= 0)
            {
                series3.ItemsSource = SDPickUp;
                SetXYSeriesProperties(ref series3, Curvethickness, Colors.Red, dashCollection);
                if ((isCurve && showlegend == true && SDPickUp.Count > 0) || isSCRcurve)
                {
                    series3.Label = Resource.ShortDelayPickupLegendIsd;
                }
                if (isCurve)
                {
                    if (showlegend == false || SDPickUp.Count == 0)
                        series3.Display = SeriesDisplay.HideLegend;
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        if (SDPickUp.Count == 0 && CurvesCalculation.SDPickUp.Count == 0 || (showlegend && CurvesCalculation.SDPickUp.Count != 0))
                        {
                            series3.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (SDPickUp.Count == 0)
                            series3.Display = SeriesDisplay.HideLegend;
                    }
                }
                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                {
                    LSChartReference.Data.Children.Add(series3);
                    if (Global.device_type == Global.NZMDEVICE && SDPickUp.Count != 0)
                    {
                        double SDpickupmax = SDPickUp.Max(x => x.X);
                        if (SDpickupmax >= 50000)
                        {
                            if (SDpickupmax < 60000)
                                Dottedpoints.Add(new Point(60000, SDPickUp.Min(x => x.Y)));
                            if (SDpickupmax < 70000)
                                Dottedpoints.Add(new Point(70000, SDPickUp.Min(x => x.Y)));
                            if (SDpickupmax < 80000)
                                Dottedpoints.Add(new Point(80000, SDPickUp.Min(x => x.Y)));
                            series22.ItemsSource = Dottedpoints;
                            SetXYSeriesProperties(ref series22, 0, Colors.Transparent);
                            series22.SymbolMarker = Marker.Dot;
                            series22.SymbolSize = new Size(5, 5);
                            series22.SymbolFill = new SolidColorBrush(Colors.Red);
                            series22.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                            series22.Display = SeriesDisplay.HideLegend;
                            LSChartReference.Data.Children.Add(series22);
                        }
                    }
                }
            }
            if (InstPickup.Count >= 0)
            {
                series11.ItemsSource = InstPickup;
                SetXYSeriesProperties(ref series11, Curvethickness, Colors.DarkRed, dashCollection);
                if ((isCurve && showlegend == true && InstPickup.Count > 0) || isSCRcurve)

                {
                    series11.Label = Resource.GroundInstCurveLegendIi;
                }
                if (isCurve)
                {
                    if (showlegend == false || InstPickup.Count == 0)
                        series11.Display = SeriesDisplay.HideLegend;
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        if (InstPickup.Count == 0 && CurvesCalculation.InstPickup.Count == 0 || (showlegend && CurvesCalculation.InstPickup.Count != 0))
                        {
                            series11.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (InstPickup.Count == 0)
                            series11.Display = SeriesDisplay.HideLegend;
                    }
                }
                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                {
                    LSChartReference.Data.Children.Add(series11);
                    if (Global.device_type == Global.NZMDEVICE && InstPickup.Count != 0)
                    {
                        double Instpickupmax = InstPickup.Max(x => x.X);
                        if (Instpickupmax >= 50000)
                        {
                            if (Instpickupmax < 60000)
                                Dottedpoints.Add(new Point(60000, InstPickup.Min(x => x.Y)));
                            if (Instpickupmax < 70000)
                                Dottedpoints.Add(new Point(70000, InstPickup.Min(x => x.Y)));
                            if (Instpickupmax < 80000)
                                Dottedpoints.Add(new Point(80000, InstPickup.Min(x => x.Y)));
                            series22.ItemsSource = Dottedpoints;
                            SetXYSeriesProperties(ref series22, 0, Colors.Transparent);
                            series22.SymbolMarker = Marker.Dot;
                            series22.SymbolSize = new Size(5, 5);
                            series22.SymbolFill = new SolidColorBrush(Colors.DarkRed);
                            series22.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                            series22.Display = SeriesDisplay.HideLegend;
                            LSChartReference.Data.Children.Add(series22);
                        }
                    }
                }
            }
            if (MMLevel.Count >= 0)
            {
                series15.ItemsSource = MMLevel;
                SetXYSeriesProperties(ref series15, Curvethickness, Colors.Blue, dashCollection);
                if ((isCurve && showlegend == true && MMLevel.Count > 0) || isSCRcurve)
                {
                    series15.Label = Resource.MaintenanceModeLegendMM;
                }
                if (isCurve)
                {
                    if (showlegend == false || MMLevel.Count == 0)
                        series15.Display = SeriesDisplay.HideLegend;
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        if (MMLevel.Count == 0 && CurvesCalculation.MMLevel.Count == 0 || (showlegend && CurvesCalculation.MMLevel.Count != 0))
                        {
                            series15.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (MMLevel.Count == 0)
                            series15.Display = SeriesDisplay.HideLegend;
                    }
                }
                if (!Global.device_type_PXR10 && curvedata.MM_Enabled && curvedata.IsMMStateOn)
                {
                    if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                    {
                        LSChartReference.Data.Children.Add(series15);
                        if (Global.device_type == Global.NZMDEVICE && MMLevel.Count != 0)
                        {
                            double MMpickupmax = MMLevel.Max(x => x.X);
                            if (MMpickupmax >= 50000)
                            {
                                if (MMpickupmax < 60000)
                                    Dottedpoints.Add(new Point(60000, MMLevel.Min(x => x.Y)));
                                if (MMpickupmax < 70000)
                                    Dottedpoints.Add(new Point(70000, MMLevel.Min(x => x.Y)));
                                if (MMpickupmax < 80000)
                                    Dottedpoints.Add(new Point(80000, MMLevel.Min(x => x.Y)));
                                series22.ItemsSource = Dottedpoints;
                                SetXYSeriesProperties(ref series22, 0, Colors.Transparent);
                                series22.SymbolMarker = Marker.Dot;
                                series22.SymbolSize = new Size(5, 5);
                                series22.SymbolFill = new SolidColorBrush(Colors.Blue);
                                series22.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                                series22.Display = SeriesDisplay.HideLegend;
                                LSChartReference.Data.Children.Add(series22);
                            }
                        }
                    }
                }
            }
            if (LD2_TU.Count >= 0)
            {
                series20.ItemsSource = LD2_TU;
                SetXYSeriesProperties(ref series20, Curvethickness, Colors.Gold, dashCollection);
                series20.Display = SeriesDisplay.HideLegend;

                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                {
                    LSChartReference.Data.Children.Add(series20);
                    if (Global.device_type == Global.NZMDEVICE && LD2_TU.Count != 0)
                    {
                        double LD2pickupmax = LD2_TU.Max(x => x.X);
                        if (LD2pickupmax >= 50000)
                        {
                            if (LD2pickupmax < 60000)
                                Dottedpoints.Add(new Point(60000, LD2_TU.Min(x => x.Y)));
                            if (LD2pickupmax < 70000)
                                Dottedpoints.Add(new Point(70000, LD2_TU.Min(x => x.Y)));
                            if (LD2pickupmax < 80000)
                                Dottedpoints.Add(new Point(80000, LD2_TU.Min(x => x.Y)));
                            series22.ItemsSource = Dottedpoints;
                            SetXYSeriesProperties(ref series22, 0, Colors.Transparent);
                            series22.SymbolMarker = Marker.Dot;
                            series22.SymbolSize = new Size(5, 5);
                            series22.SymbolFill = new SolidColorBrush(Colors.Gold);
                            series22.SymbolStroke = new SolidColorBrush(Colors.Transparent);
                            series22.Display = SeriesDisplay.HideLegend;
                            LSChartReference.Data.Children.Add(series22);
                        }
                    }
                }
            }
            if (!curvedata.MM_Enabled && !curvedata.IsMMStateOn)
            {
                if (!curvedata.IN_Enabled)
                {
                    if (excludeShortdelay)
                        TabIitemi2tHeader = Resource.LongDelayCurvesWithoutMMANDINST;
                    else
                        TabIitemi2tHeader = Resource.LongShortDelayCurvesWithoutMMANDINST;
                }
                else
                {
                    if (excludeShortdelay)
                        TabIitemi2tHeader = Resource.LongDelayCurvesWithoutMM;
                    else
                        TabIitemi2tHeader = Resource.LongShortDelayCurvesWithoutMM;
                }
            }
            else
            {
                if (excludeShortdelay)
                    TabIitemi2tHeader = Resource.LongDelayCurves;
                else
                    TabIitemi2tHeader = Resource.LongShortDelayCurves;
            }
            if (isCurve)
            {
                LSChartReference.View.AxisX.LogBase = 10;
                LSChartReference.View.AxisY.LogBase = 10;

                if (Global.device_type == Global.NZMDEVICE)
                {
                    LSChartReference.View.AxisY.Min = 0.001;
                    LSChartReference.View.AxisY.Max = 10000;

                    LSChartReference.View.AxisX.Min = curvedata.LSIAXMin;
                    LSChartReference.View.AxisX.Max = curvedata.LSIAxMax;
                }
                else
                {
                    LSChartReference.View.AxisY.Min = 0.01;
                    LSChartReference.View.AxisY.Max = 100000;
                }

                if (Global.device_type == Global.PTM_DEVICE && Global.IsOffline)
                {
                    LSChartReference.View.AxisX.Min = 10;
                }

                LSChartReference.View.AxisY.AnnoFormat = "0.0##";
                LSChartReference.View.AxisX.AnnoFormat = "######";

                LSChartReference.View.AxisX.MinorUnit = 1;
                LSChartReference.View.AxisX.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSChartReference.View.AxisX.MinorGridStrokeThickness = 0.5;

                LSChartReference.View.AxisY.MinorUnit = 1;
                LSChartReference.View.AxisY.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSChartReference.View.AxisY.MinorGridStrokeThickness = 0.5;

                // major grid
                LSChartReference.View.AxisX.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSChartReference.View.AxisX.MajorGridStrokeDashes = null;

                LSChartReference.View.AxisY.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSChartReference.View.AxisY.MajorGridStrokeDashes = null;

                //Added by sreejith
                LSTxtBlockText = Resource.LSIMCurveHeader;
                LSChartReference.View.AxisX.Title = Resource.CurrentAmps;
                LSChartReference.View.AxisY.Title = Resource.TimeInSeconds;
                LSChartReference.View.AxisX.FontSize = 14;
                LSChartReference.View.AxisY.FontSize = 14;
                LSChartReference.View.AxisX.FontFamily = ff[0];
                LSChartReference.View.AxisY.FontFamily = ff[0];
                LSChartReference.EndUpdate();

                LSIrChartReference.Reset(true);
                LSIrChartReference.BeginUpdate();
                LSIrChartReference.ChartType = ChartType.Line;
            }
            if (LSIr.Count >= 0)
            {
                series16.ItemsSource = LSIr;
                SetXYSeriesProperties(ref series16, Curvethickness, Colors.Gold, dashCollection);
                if ((isCurve && showlegend == true && LSIr.Count > 0) || isSCRcurve)
                {
                    if (Mtrsetting != null && Mtrsetting.bValue)
                        series16.Label = Resource.Overload;
                    else
                        series16.Label = Resource.LongDelayPickupLegendIr;
                }
                if (isCurve)
                {
                    if (showlegend == false || LSIr.Count == 0)
                    {
                        series16.Display = SeriesDisplay.HideLegend;
                    }
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        if (LSIr.Count == 0 && CurvesCalculation.LSIr.Count == 0 || (showlegend && CurvesCalculation.LSIr.Count != 0))
                        {
                            series16.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (LSIr.Count == 0)
                            series16.Display = SeriesDisplay.HideLegend;
                    }
                }

                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                    LSIrChartReference.Data.Children.Add(series16);
            }
            if (LSIrSDPickup.Count >= 0)
            {
                series17.ItemsSource = LSIrSDPickup;
                SetXYSeriesProperties(ref series17, Curvethickness, Colors.Red, dashCollection);
                if ((isCurve && showlegend == true && LSIrSDPickup.Count > 0) || isSCRcurve)
                {
                    series17.Label = Resource.ShortDelayPickupLegendIsd;
                }
                if (isCurve)
                {
                    if (showlegend == false || LSIrSDPickup.Count == 0)
                        series17.Display = SeriesDisplay.HideLegend;
                }
                else if (isSCRcurve)
                {
                    if (!Global.IsOffline)
                    {
                        if (LSIrSDPickup.Count == 0 && CurvesCalculation.LSIrSDPickup.Count == 0 || (showlegend && CurvesCalculation.LSIrSDPickup.Count != 0))
                        {
                            series17.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    else
                    {
                        if (LSIrSDPickup.Count == 0)
                            series17.Display = SeriesDisplay.HideLegend;
                    }
                }

                if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                    LSIrChartReference.Data.Children.Add(series17);
            }

            if (excludeShortdelay)
                TabIitemLSIrHeader = Resource.LIrHeader;
            else
                TabIitemLSIrHeader = Resource.LSxIrHeader;
            if (isCurve)
            {
                if (LS_LD2_TU.Count >= 0)
                {
                    series21.ItemsSource = LS_LD2_TU;
                    SetXYSeriesProperties(ref series21, Curvethickness, Colors.Gold, dashCollection);
                    series21.Display = SeriesDisplay.HideLegend;

                    if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                        LSIrChartReference.Data.Children.Add(series21);
                }

                LSIrChartReference.View.AxisX.LogBase = 10;
                LSIrChartReference.View.AxisY.LogBase = 10;
                LSIrChartReference.View.AxisX.Max = curvedata.LSIrXMax;

                if (Global.device_type == Global.NZMDEVICE)
                {
                    LSIrChartReference.View.AxisX.Min = 0.1;
                    LSIrChartReference.View.AxisY.Min = 0.001;
                    LSIrChartReference.View.AxisY.Max = 10000;
                }
                else
                {
                    LSIrChartReference.View.AxisX.Min = curvedata.LSIrXmin;
                    LSIrChartReference.View.AxisY.Min = 0.01;
                    LSIrChartReference.View.AxisY.Max = 100000;
                }

                LSIrChartReference.View.AxisX.AnnoFormat = "0.##";
                LSIrChartReference.View.AxisY.AnnoFormat = "0.0##";

                LSIrChartReference.View.AxisX.MinorUnit = 1;
                LSIrChartReference.View.AxisX.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSIrChartReference.View.AxisX.MinorGridStrokeThickness = 0.5;

                LSIrChartReference.View.AxisY.MinorUnit = 1;
                LSIrChartReference.View.AxisY.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSIrChartReference.View.AxisY.MinorGridStrokeThickness = 0.5;

                // major grid
                LSIrChartReference.View.AxisX.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSIrChartReference.View.AxisX.MajorGridStrokeDashes = null;

                LSIrChartReference.View.AxisY.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                LSIrChartReference.View.AxisY.MajorGridStrokeDashes = null;

                //Added by sreejith
                if (Mtrsetting != null && Mtrsetting.bValue)
                {
                    if (excludeShortdelay)
                        LSIrTextBlockText = Resource.OverloadCurveHeaderwithoutShort;
                    else
                        LSIrTextBlockText = Resource.OverloadCurveHeader;
                }
                else
                {
                    if (excludeShortdelay)
                        LSIrTextBlockText = Resource.LIrCurveHeader;
                    else
                        LSIrTextBlockText = Resource.LSIrCurveHeader;
                }
                LSIrChartReference.View.AxisX.Title = Resource.CurrentxIr;
                LSIrChartReference.View.AxisY.Title = Resource.TimeInSeconds;

                LSIrChartReference.View.AxisX.FontSize = 14;
                LSIrChartReference.View.AxisY.FontSize = 14;

                LSIrChartReference.View.AxisX.FontFamily = ff[0];
                LSIrChartReference.View.AxisY.FontFamily = ff[0];

                LSIrChartReference.EndUpdate();

                GIChartReference.Reset(true);
                GIChartReference.BeginUpdate();
                GIChartReference.ChartType = ChartType.Line;
            }
            //Hide GF Tab if the device doesn't support Ground protection
            bool HideGFtab = false;
            bool IsGFprotectionON = TripUnit.getGroundProtectionGeneralGrp().bValue;
            string getdevicetype = TripUnit.getTripUnitType().selectionValue;
            if (!IsGFprotectionON || (isSCRcurve && Global.IsOffline && getdevicetype == Resource.GEN01Item0000))
                HideGFtab = true;
            if (GFPickup.Count >= 0)
            {
                if (HideGFtab)
                {
                    TabItemGroundInstVisibility = Visibility.Hidden;
                    TabItemGroundInstVisibility = Visibility.Collapsed;
                    if (GIChartReference.IsVisible)
                        TbCurvesSelectedIndex = 0;
                }
                else // Remoce this Else conditions once NZM SCr curves implamented. ALready present in that code
                {
                    TabItemGroundInstVisibility = Visibility.Visible;
                    series8.ItemsSource = GFPickup;
                    SetXYSeriesProperties(ref series8, Curvethickness, Colors.Black, dashCollection);
                    if ((isCurve && showlegend == true && GFPickup.Count > 0) || isSCRcurve)
                    {
                        series8.Label = Resource.GroundInstCurveLegendlg;
                    }
                    if (isCurve)
                    {
                        if (showlegend == false || GFPickup.Count == 0)
                            series8.Display = SeriesDisplay.HideLegend;
                    }
                    else if (isSCRcurve)
                    {
                        if (!Global.IsOffline)
                        {
                            if (GFPickup.Count == 0 && CurvesCalculation.GFPickup.Count == 0 || (showlegend && CurvesCalculation.GFPickup.Count != 0))
                            {
                                series8.Display = SeriesDisplay.HideLegend;
                            }
                        }
                        else
                        {
                            if (GFPickup.Count == 0)
                                series8.Display = SeriesDisplay.HideLegend;
                        }
                    }
                    if (curvedata.GF_Enabled && !curvedata.isGFActionoff)
                    {
                        if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                            GIChartReference.Data.Children.Add(series8);
                    }
                }
            }
            bool Hideinsttab = false;
            if (Global.device_type == Global.MCCBDEVICE)
            {
                bool isMotorProtEnabled = TripUnit.getMotorProtectionGeneralGrp().bValue;
                if (getdevicetype == Resource.GEN01Item0000 && isMotorProtEnabled == true)
                    Hideinsttab = true;
            }
            if (InstxIn.Count > 0)
            {
                if (Hideinsttab == true)
                {
                    TabIitemINSTInVisibility = Visibility.Hidden;
                    TabIitemINSTInVisibility = Visibility.Collapsed;
                    if (INSTxInChartReference.IsVisible)
                        TbCurvesSelectedIndex = 0;
                }
                else
                {
                    TabIitemINSTInVisibility = Visibility.Visible;
                    series18.ItemsSource = InstxIn;
                    SetXYSeriesProperties(ref series18, Curvethickness, Colors.DarkRed, dashCollection);
                    if ((isCurve && showlegend == true) || isSCRcurve)
                        series18.Label = Resource.GroundInstCurveLegendIi;
                    series18.Display = ((isCurve && showlegend == false) || InstxIn.Count == 0 || (isSCRcurve && !Global.IsOffline && showlegend && CurvesCalculation.InstxIn.Count != 0)) ? SeriesDisplay.HideLegend : SeriesDisplay.SkipNaN;

                    if (curvedata.IN_Enabled && (isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                        INSTxInChartReference.Data.Children.Add(series18);
                }
            }
            else if (Hideinsttab == true)
            {
                TabIitemINSTInVisibility = Visibility.Hidden;
                TabIitemINSTInVisibility = Visibility.Collapsed;
                if (INSTxInChartReference.IsVisible)
                    TbCurvesSelectedIndex = 0;
            }
            if (MMxIn.Count >= 0)
            {
                if (!curvedata.MM_Enabled && !curvedata.IsMMStateOn)
                {
                    TabIitemMMInVisibility = Visibility.Hidden;
                    TabIitemMMInVisibility = Visibility.Collapsed;
                    if (MMxInChartReference.IsVisible)
                        TbCurvesSelectedIndex = 0;
                }
                else
                {
                    TabIitemMMInVisibility = Visibility.Visible;
                    series19.ItemsSource = MMxIn;
                    SetXYSeriesProperties(ref series19, Curvethickness, Colors.Blue, dashCollection);
                    if ((isCurve && (showlegend == true) && MMxIn.Count > 0) || isSCRcurve)
                        series19.Label = Resource.MaintenanceModeLegendMM;
                    series19.Display = ((isCurve && showlegend == false) || MMxIn.Count == 0 || (isSCRcurve && !Global.IsOffline && showlegend && CurvesCalculation.MMxIn.Count != 0)) ? SeriesDisplay.HideLegend : SeriesDisplay.SkipNaN;
                    if (isCurve)
                        series19.Visibility = Visibility.Visible;

                    if ((isCurve && !Global.IsOffline) || (isSCRcurve && addcurvedata))
                        MMxInChartReference.Data.Children.Add(series19);

                    if (curvedata.MM_Enabled && !curvedata.IsMMStateOn)
                    {
                        MMxIn.Clear();
                    }
                }
            }
            if (isCurve)
            {
                GITxtBlockText = Resource.GroundInstNoneCurveHeader;
                GIChartReference.View.AxisX.LogBase = 10;
                GIChartReference.View.AxisY.LogBase = 10;

                if (Global.device_type == Global.NZMDEVICE)
                {
                    GIChartReference.View.AxisX.Min = curvedata.GFXMin;
                    GIChartReference.View.AxisX.Max = curvedata.GFXMax;
                    GIChartReference.View.AxisY.Min = curvedata.GFYMin;
                    GIChartReference.View.AxisY.Max = curvedata.GFYMax;
                }
                else
                {
                    GIChartReference.View.AxisX.Min = Math.Pow(10, Math.Floor(Math.Log10((double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))));
                    if (Global.isDemoMode && !Global.IsOffline)
                    {
                        if (GIChartReference.View.AxisX.Min == (double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))
                            GIChartReference.View.AxisX.Min = GIChartReference.View.AxisX.Min / 10;
                    }

                    GIChartReference.View.AxisX.Max = Math.Pow(10, Math.Ceiling(Math.Log10((double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))));

                    if (GIChartReference.View.AxisX.Max == (double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))
                        GIChartReference.View.AxisX.Max = GIChartReference.View.AxisX.Max * 10;

                    //Adjust GFmin scale
                    if (Global.isDemoMode && !Global.IsOffline)
                    {
                        if (GFPickup.Count > 0)
                        {
                            double GFminval = GFPickup.Min(m => m.X);
                            double GFmin = Math.Pow(10, Math.Floor(Math.Log10((double)(GFminval))));
                            if (GFmin == GFminval)
                                GFmin = GFmin / 10;

                            if (GIChartReference.View.AxisX.Min != GFmin && GFminval != 0)
                                GIChartReference.View.AxisX.Min = GFmin;
                        }
                    }
                    //Adjust GFMax scale
                    if (GFPickup.Count > 0)
                    {
                        double GFmaxval = GFPickup.Max(m => m.X);
                        double GFmax = Math.Pow(10, Math.Ceiling(Math.Log10((double)(GFmaxval))));
                        if (GFmax == GFmaxval)
                            GFmax = GFmax * 10;
                        if (GIChartReference.View.AxisX.Max != GFmax && GFmaxval != 0)
                            GIChartReference.View.AxisX.Max = GFmax;
                    }

                    if (CurvesCalculation.isOrigGFvalue == true && !Global.IsOffline)
                    {
                        CurvesCalculation.isOrigGFvalue = false;
                        CurvesCalculation.Orig_GFmin = GIChartReference.View.AxisX.Min;
                        CurvesCalculation.Orig_GFmax = GIChartReference.View.AxisX.Max;
                    }

                    GIChartReference.View.AxisY.Min = 0.01;
                    GIChartReference.View.AxisY.Max = 100;
                }
                GIChartReference.View.AxisX.MajorTickOverlap = 1;
                GIChartReference.View.AxisX.MajorTickOverlap = 10;

                GIChartReference.View.AxisY.AnnoFormat = "0.##";
                GIChartReference.View.AxisX.AnnoFormat = "0.##";

                GIChartReference.View.AxisX.MinorUnit = 1;
                GIChartReference.View.AxisX.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                GIChartReference.View.AxisX.MinorGridStrokeThickness = 0.5;

                GIChartReference.View.AxisY.MinorUnit = 1;
                GIChartReference.View.AxisY.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                GIChartReference.View.AxisY.MinorGridStrokeThickness = 0.5;

                // major grid
                GIChartReference.View.AxisX.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                GIChartReference.View.AxisX.MajorGridStrokeDashes = null;

                GIChartReference.View.AxisY.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                GIChartReference.View.AxisY.MajorGridStrokeDashes = null;

                GIChartReference.View.AxisX.Title = Resource.CurrentAmps;
                GIChartReference.View.AxisY.Title = Resource.TimeInSeconds;

                GIChartReference.View.AxisX.FontSize = 14;
                GIChartReference.View.AxisY.FontSize = 14;

                GIChartReference.View.AxisX.FontFamily = ff[0];
                GIChartReference.View.AxisY.FontFamily = ff[0];
                GIChartReference.EndUpdate();

                INSTxInChartReference.View.AxisX.LogBase = 10;
                INSTxInChartReference.View.AxisY.LogBase = 10;

                INSTxInChartReference.View.AxisX.MajorTickOverlap = 1;
                INSTxInChartReference.View.AxisX.MajorTickOverlap = 10;

                if (Global.device_type == Global.NZMDEVICE)
                {
                    INSTxInChartReference.View.AxisX.Min = 1;
                    INSTxInChartReference.View.AxisX.Max = curvedata.InstXMax;
                    INSTxInChartReference.View.AxisY.Min = 0.001;
                    INSTxInChartReference.View.AxisY.Max = 100;
                }
                else
                {
                    INSTxInChartReference.View.AxisX.Min = curvedata.InstXmin;
                    INSTxInChartReference.View.AxisX.Max = curvedata.InstXMax;
                    INSTxInChartReference.View.AxisY.Min = 0.01;
                    INSTxInChartReference.View.AxisY.Max = 100;
                }

                INSTxInChartReference.View.AxisY.AnnoFormat = "0.0##";
                INSTxInChartReference.View.AxisX.AnnoFormat = "######";

                INSTxInChartReference.View.AxisX.MinorUnit = 1;
                INSTxInChartReference.View.AxisX.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                INSTxInChartReference.View.AxisX.MinorGridStrokeThickness = 0.5;

                INSTxInChartReference.View.AxisY.MinorUnit = 1;
                INSTxInChartReference.View.AxisY.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                INSTxInChartReference.View.AxisY.MinorGridStrokeThickness = 0.5;

                // major grid
                INSTxInChartReference.View.AxisX.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                INSTxInChartReference.View.AxisX.MajorGridStrokeDashes = null;

                INSTxInChartReference.View.AxisY.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                INSTxInChartReference.View.AxisY.MajorGridStrokeDashes = null;

                INSTInTextBlockText = Resource.InstCurveHeader;

                INSTxInChartReference.View.AxisX.Title = Resource.CurrentxIn;
                INSTxInChartReference.View.AxisY.Title = Resource.TimeInSeconds;

                INSTxInChartReference.View.AxisX.FontFamily = ff[0];
                INSTxInChartReference.View.AxisY.FontFamily = ff[0];
                INSTxInChartReference.EndUpdate();

                MMxInChartReference.View.AxisX.LogBase = 10;
                MMxInChartReference.View.AxisY.LogBase = 10;

                MMxInChartReference.View.AxisX.MajorTickOverlap = 1;
                MMxInChartReference.View.AxisX.MajorTickOverlap = 10;

                if (Global.device_type == Global.NZMDEVICE)
                {
                    MMxInChartReference.View.AxisX.Min = 1;
                    MMxInChartReference.View.AxisX.Max = curvedata.MMXMax;

                    MMxInChartReference.View.AxisY.Min = 0.001;
                    MMxInChartReference.View.AxisY.Max = 100;
                }
                else
                {
                    MMxInChartReference.View.AxisX.Min = curvedata.MMXmin;
                    MMxInChartReference.View.AxisX.Max = curvedata.MMXMax;

                    MMxInChartReference.View.AxisY.Min = 0.01;
                    MMxInChartReference.View.AxisY.Max = 100;
                }

                MMxInChartReference.View.AxisY.AnnoFormat = "0.0##";
                MMxInChartReference.View.AxisX.AnnoFormat = "######";

                MMxInChartReference.View.AxisX.MinorUnit = 1;
                MMxInChartReference.View.AxisX.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                MMxInChartReference.View.AxisX.MinorGridStrokeThickness = 0.5;

                MMxInChartReference.View.AxisY.MinorUnit = 1;
                MMxInChartReference.View.AxisY.MinorGridStroke = new SolidColorBrush(Colors.DarkGray);
                MMxInChartReference.View.AxisY.MinorGridStrokeThickness = 0.5;

                // major grid
                MMxInChartReference.View.AxisX.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                MMxInChartReference.View.AxisX.MajorGridStrokeDashes = null;

                MMxInChartReference.View.AxisY.MajorGridStroke = new SolidColorBrush(Colors.DarkGray);
                MMxInChartReference.View.AxisY.MajorGridStrokeDashes = null;

                MMInTextBlockText = Resource.MMCurveHeader;
                MMxInChartReference.View.AxisX.Title = Resource.CurrentxIn;
                MMxInChartReference.View.AxisY.Title = Resource.TimeInSeconds;

                MMxInChartReference.View.AxisX.FontFamily = ff[0];
                MMxInChartReference.View.AxisY.FontFamily = ff[0];
                MMxInChartReference.EndUpdate();
            }
            else if (isSCRcurve)
            {
                double GFmin = 0;
                double GFmax = 0;
                //Update max and min scale for GF curve for all devices
                if (Global.device_type == Global.NZMDEVICE)
                {
                    GFmin = curvedata.GFXMin;
                    GFmax = curvedata.GFXMax;
                }
                else
                {

                    GFmin = Math.Pow(10, Math.Floor(Math.Log10((double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))));
                    if (Global.isDemoMode && !Global.IsOffline)
                    {
                        if (GFmin == (double)(curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis / 2))
                            GFmin = GFmin / 10;
                    }
                    GFmax = Math.Pow(10, Math.Ceiling(Math.Log10((double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))));
                    if (GFmax == (double)(10 * curvedata.GFPU_ToSetAxis * curvedata.In_ToSetAxis * 2))
                        GFmax = GFmax * 10;
                }
                //Adjust GFmin scale
                
                    if (GFPickup.Count > 0)
                    {
                        double GFminval = GFPickup.Min(m => m.X);
                        double GFcurvemin = Math.Pow(10, Math.Floor(Math.Log10((double)(GFminval))));
                        if (GFcurvemin == GFminval)
                            GFcurvemin = GFcurvemin / 10;
                        if (GFmin != GFcurvemin && GFminval != 0)
                            GFmin = GFcurvemin;
                    }
                
                if (!Global.IsOffline && CurvesCalculation.Orig_GFmin != 0 && !Double.IsNaN(CurvesCalculation.Orig_GFmin) && !Double.IsNaN(GFmin))
                    GIChartReference.View.AxisX.Min = Math.Min(CurvesCalculation.Orig_GFmin, GFmin);
                else
                    GIChartReference.View.AxisX.Min = GFmin;

                //Adjust GFMax scale
                if (GFPickup.Count > 0)
                {
                    double GFmaxval = GFPickup.Max(m => m.X);
                    double GFcurvemax = Math.Pow(10, Math.Ceiling(Math.Log10((double)(GFmaxval))));
                    if (GFcurvemax == GFmaxval)
                        GFcurvemax = GFcurvemax * 10;
                    if (GFmax != GFcurvemax && GFmaxval != 0)
                        GFmax = GFcurvemax;
                }
                if (!Global.IsOffline && CurvesCalculation.Orig_GFmax != 0 && !Double.IsNaN(CurvesCalculation.Orig_GFmax) && !Double.IsNaN(GFmax))
                    GIChartReference.View.AxisX.Max = Math.Max(CurvesCalculation.Orig_GFmax, GFmax);
                else
                    GIChartReference.View.AxisX.Max = GFmax;
            }
        }
        private void setScale()
        {

            CurvesCalculation.setAxesValues();

            //double lsiaXmin = (from kvp in CurvesCalculation.ocMinMax where kvp.Key == "lsiaXmin" select kvp.Value).FirstOrDefault();
            //double lsiaXmax = (from kvp in CurvesCalculation.ocMinMax where kvp.Key == "lsiaXmax" select kvp.Value).FirstOrDefault();

            //LSChart.View.AxisX.Min = lsiaXmin;
            //LSChart.View.AxisX.Max = lsiaXmax;

            LSChartReference.View.AxisX.Min = CurvesCalculation.LSIAXMin;
            LSChartReference.View.AxisX.Max = CurvesCalculation.LSIAxMax;

            LSIrChartReference.View.AxisX.Min = Math.Min(CurvesCalculation.LSIrXmin, CurvesCalculation.LSIrXmin);
            LSIrChartReference.View.AxisX.Max = Math.Max(CurvesCalculation.LSIrXMax, CurvesCalculation.LSIrXMax);

            INSTxInChartReference.View.AxisX.Min = Math.Min(CurvesCalculation.InstXmin, CurvesCalculation.InstXmin);
            INSTxInChartReference.View.AxisX.Max = Math.Max(CurvesCalculation.InstXMax, CurvesCalculation.InstXMax);

            MMxInChartReference.View.AxisX.Min = Math.Min(CurvesCalculation.MMXmin, CurvesCalculation.MMXmin);
            MMxInChartReference.View.AxisX.Max = Math.Max(CurvesCalculation.MMXMax, CurvesCalculation.MMXMax);

            if (Double.IsNaN(GIChartReference.View.AxisX.Max))
            {
                if (GIChartReference.View.AxisX.Min == 0)
                {
                    GIChartReference.View.AxisX.Min = 10;
                }
                GIChartReference.View.AxisX.Max = Math.Pow(GIChartReference.View.AxisX.Min, 5);
            }
        }

        private void setAdditionalCurvesScale()
        {

            CurvesCalculation.setAxesValues();


            LSChartReference.View.AxisX.Min = CurvesCalculation.LSIAXMin;
            LSChartReference.View.AxisX.Max = CurvesCalculation.LSIAxMax;


            if (Double.IsNaN(GIChartReference.View.AxisX.Max))
            {
                if (GIChartReference.View.AxisX.Min == 0)
                {
                    GIChartReference.View.AxisX.Min = 10;
                }
                GIChartReference.View.AxisX.Max = Math.Pow(GIChartReference.View.AxisX.Min, 5);
            }
        }

        void set_SettingValueChange(string propName, string BaseValue, string newVal, string controlName, bool isVisible, string groupId)
        {
            //If General group values are getting changed, Then it should be considered as new start
            //So, Cleared chnage summary and its dependent changes will not get added in change summary.
            //For ACB, few setpoints are available in Group 1 - Added those using control name
            //cmb_SYS003 - Trip Unit Style
            //cmb_SYS000 - Unit Type
            //cmb_SYS001 - Rating
            //cmb_SYS002 - Breaker Frame

            if (groupId == "00" || groupId == "0" || controlName == "cmb_SYS003" || controlName == "cmb_SYS003A" || controlName == "cmb_SYS000"
                || controlName == "cmb_SYS001" || controlName == "cmb_SYS002"
                || controlName == "cmb_SYS001A" || controlName == "cmb_SYS003B" || controlName == "cmb_SYS002A") // ACB PXR35 ID's
            {
                Changes.Clear();
                Global.listGroupsAsFoundSetPoint.Clear();
                PopulateAsFoundData();
                return;
            }

            //Code to update Labels of Curve based on current selections.
            //LSTxtBlock.Text = String.Format(Resource.TimeCurrentCharLabel, Global.longDelaySlopeSelectionValue,
            //    Global.shortDelaySlopeSelectionValue);

            ///********  adding return on these values causing - when SDT is set to any value from off state it is not getting reflected in change summary 
            // For NZM LDT value recevied from device is 3276.7 when LD toggle is disabled, so hardcoded
            //if (newVal == "3276.7" || newVal == "3276,7" || BaseValue == "3276.7" || BaseValue == "3276,7")
            //    return;
            ///********
            if ((newVal == "65535") && Global.device_type == Global.ACB_PXR35_DEVICE)
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "65535") && Global.device_type == Global.ACB_PXR35_DEVICE)
            {
                BaseValue = Resource.NA;
            }

            if ((newVal == "3276.7" || newVal == "3276,7") && Global.device_type == Global.NZMDEVICE)
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "3276.7" || BaseValue == "3276,7") && Global.device_type == Global.NZMDEVICE)
            {
                BaseValue = Resource.NA;
            }

            if ((newVal == "0" || newVal == "0") && controlName == "cmb_CPC025" && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "0" || BaseValue == "0") && controlName == "cmb_CPC025" && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                BaseValue = Resource.NA;
            }


            //if ((newVal == "0" || newVal == "0") && controlName == "cmb_CPC0126" && ( Global.device_type == Global.ACB_PXR35_DEVICE))
            //{
            //    newVal = Resource.NA;
            //}
            //if ((BaseValue == "0" || BaseValue == "0") && controlName == "cmb_CPC0126" && (Global.device_type == Global.ACB_PXR35_DEVICE))
            //{
            //    BaseValue = Resource.NA;
            //}




            if ((newVal == "0" || newVal == "0") && (controlName == "cmb_CPC026" || controlName == "cmb_CPC027") && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "0" || BaseValue == "0") && (controlName == "cmb_CPC026" || controlName == "cmb_CPC027") && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE))
            {
                BaseValue = Resource.NA;
            }
            //added by srk to fix PXPM-6171
            if ((newVal == "127") && (controlName == "txt_SYS020") && (Global.device_type == Global.ACB_03_02_XX_DEVICE))
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "127") && (controlName == "txt_SYS020") && (Global.device_type == Global.ACB_03_02_XX_DEVICE))
            {
                BaseValue = Resource.NA;
            }

            if ((newVal == "Disabled") && (controlName == "tgl_SYS024") && (Global.device_type == Global.ACB_03_02_XX_DEVICE))
            {
                newVal = Resource.NA;
            }
            if ((BaseValue == "Disabled") && (controlName == "tgl_SYS024") && (Global.device_type == Global.ACB_03_02_XX_DEVICE))
            {
                BaseValue = Resource.NA;
            }

            //newVal = Resource.NotAvailable;
            //Change labels for Long Delay and Ground Fault Thermal memory in Change Summary for MCCB and NZM
            if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
            {
                if (controlName == "tgl_CPC01")
                    propName = TripUnit.getMotorProtectionGeneralGrp().bValue ? Resource.OverloadTM : Resource.LDThermalMemory;
                if (controlName == "tgl_CPC16")
                    propName = Resource.GFThermalMemory;
            }
            LSTxtBlockText = Resource.LSIMCurveHeader;

            if (Global.groundFaultSlopeSelectionValue.ToUpper().Trim() == Resource.CPC017Item0001.ToUpper().Trim())
                GITxtBlockText = Resource.GroundInstI2TCurveHeader;
            else if (Global.groundFaultSlopeSelectionValue.ToUpper().Trim() == Resource.CPC017Item0000.ToUpper().Trim())
                GITxtBlockText = Resource.GroundInstFlatCurveHeader;
            else if (string.IsNullOrEmpty(Global.groundFaultSlopeSelectionValue.Trim()))
                GITxtBlockText = Resource.GroundInstNoneCurveHeader;

            Global.changesSaved = false; // value changed in setting. Use this flag to prompt user to save setpoints

            // first remove the item from collection,if found
            for (int i = Changes.Count - 1; i >= 0; i--)
            {
                if (((Changes[i].ItemName.Trim().ToUpper() == propName.Trim().ToUpper()) ||
                    (Changes[i].ItemName.Trim().ToUpper() == propName.Trim().ToUpper() + " *")) &&
                   (Changes[i].ControlName.Trim().ToUpper() == controlName.Trim().ToUpper() || !isVisible)
                   && !Global.isUpdatingSettingBackForRestrictedUser)
                {
                    if (!Changes[i].commitedSetpoint)
                    {
                        Changes.RemoveAt(i);
                        Global.setting_Changed = false;
                    }
                    else
                    {
                        if (!Global.IsUndoLock)
                        {
                            Changes.RemoveAt(i);
                            Global.setting_Changed = false;
                        }
                    }
                }
            }
            bool isNumber;
            bool isNumberBase;
            if (controlName == Global.ipControl1 || controlName == Global.ipControl2 || controlName == Global.ipControl3)
            {
                isNumber = false;
                isNumberBase = false;
            }
            else
            {
                isNumber = IsNumeric(newVal);
                isNumberBase = IsNumeric(BaseValue);
            }

            try
            {
                Settings obj = null;
                //To remove setpoints from change summery those are not visible 
                if (controlName.Contains("GEN"))
                {
                    ((Settings)TripUnit.IDTable[controlName.Split('_')[1]]).notifyDependents();
                }

                string ID;
                for (int i = Changes.Count - 1; i >= 0; i--)
                {

                    ID = Changes[i].ControlName.Split('_').Count() == 1 ? Changes[i].ControlName : Changes[i].ControlName.Split('_')[1];
                    obj = (Settings)TripUnit.IDTable[ID];
                    if (obj.ID == "CPC051" && Global.device_type == Global.NZMDEVICE &&
                        (Changes[i].NewValue == Resource.NA || Changes[i].OrigValue == Resource.NA))
                    { continue; }
                    if (!Changes[i].commitedSetpoint && (!obj.visible || obj.notAvailable))
                    {
                        Changes.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }

            if (!controlName.Contains("SYS025")) // do not add Active setpoint change in change summary as it is not getting changed from setpoint export 
            {
                AddDataToChangeSummaryList(newVal, BaseValue, controlName, propName, isNumber, isNumberBase, isVisible, groupId);
            }


            if (Changes.Count < 1)
            {
                // btnExportSummary.IsEnabled = false;
                BtnRestoreDefaultIsEnabled = false;
            }
            else
            {
                // logic for checking the change summary button enabling and disabling based on the commited setpoints.
                int count_commited_setpoints = 0;
                for (int i = Changes.Count - 1; i >= 0; i--)
                {
                    if (Changes[i].commitedSetpoint)
                    {
                        count_commited_setpoints++;
                    }
                }
                if (Changes.Count == count_commited_setpoints)
                {
                    BtnRestoreDefaultIsEnabled = false;
                }
                else
                {
                    BtnRestoreDefaultIsEnabled = true;
                }
                // btnExportSummary.IsEnabled = true;
                //if (Global.selectedTemplateType !=  Global.MCCBDEVICE )
                //{
                //    btnRestoreDefault.IsEnabled = true;
                //}
            }
        }


        private string GetUpdatedControlName(string propName, string groupID)
        {
            //For PXR35 list of protection groups which are not supported for ristricted user export 
            //groupID == Group 0 , Group 3 ,Group 4 from template  are allowed to write 

            if (Global.device_type == Global.ACB_PXR35_DEVICE)
            {
                if (!(Convert.ToInt32(groupID) == 0 || Convert.ToInt32(groupID) == 1 || Convert.ToInt32(groupID) == 3 || Convert.ToInt32(groupID) == 4))
                {
                    return propName + " *";
                }
            }
            return propName;
        }

        private void AddDataToChangeSummaryList(string newVal, string BaseValue, string controlName, string propName, bool isNumber, bool isNumberBase, bool isVisible, string groupID)
        {
            propName = GetUpdatedControlName(propName, groupID);

            if (newVal != string.Empty && isVisible)
            {
                // add item in collection if new value is different from default value
                if (isNumber == false || isNumberBase == false) // string
                                                                // if (isNumber == false ) // string
                {

                    //For relay settings, Ground Fault option with space is available for Alarms group, to get it reflected in change summary skipping trim option
                    bool isRelaySetting = (controlName.Contains("SYS132") || controlName.Contains("SYS142") || controlName.Contains("SYS152")) ? true : false;
                    if (BaseValue == null || (BaseValue.Trim() != newVal.Trim()) || (isRelaySetting && BaseValue != newVal))
                    {
                        ChangeSummary sum =
                            new ChangeSummary()
                            {
                                ItemName = propName,
                                OrigValue = BaseValue,
                                NewValue = newVal,
                                ControlName = controlName,
                                commitedSetpoint = false
                            };
                        Changes.Add(sum);
                        grdChangeSummary_CommitedChanges();
                        Global.setting_Changed = true;
                    }
                }
                else if (isNumber == true && isNumberBase == true)
                {

                    NumberFormatInfo nFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;

                    if (nFormat.NumberDecimalSeparator == ",")
                    {
                        BaseValue = BaseValue.Replace(".", ",");
                        newVal = newVal.Replace(".", ",");
                    }


                    if (Convert.ToDouble(BaseValue) - Convert.ToDouble(newVal) != 0)
                    {
                        ChangeSummary sum =
                            new ChangeSummary()
                            {
                                ItemName = propName,
                                OrigValue = BaseValue,
                                NewValue = newVal,
                                ControlName = controlName,
                                commitedSetpoint = false
                            };
                        Changes.Add(sum);
                        grdChangeSummary_CommitedChanges();
                        Global.setting_Changed = true;
                    }

                }
            }
            if (controlName == "btnl2_SYS131B" || controlName == "btnl2_SYS141B" || controlName == "btnl2_SYS151B")
            {
                if (!string.IsNullOrEmpty(propName))
                {
                    ChangeSummary sum =
                           new ChangeSummary()
                           {
                               ItemName = propName,
                               OrigValue = BaseValue,
                               NewValue = newVal,
                               ControlName = controlName,
                               commitedSetpoint = false
                           };
                    Changes.Add(sum);
                    grdChangeSummary_CommitedChanges();
                    Global.setting_Changed = true;
                }
                else
                {
                    for (int i = Changes.Count - 1; i >= 0; i--)
                    {
                        if (Changes[i].ControlName.Trim().ToUpper() == controlName.Trim().ToUpper())
                        {
                            if (!Changes[i].commitedSetpoint)
                            {
                                Changes.RemoveAt(i);
                                Global.setting_Changed = false;
                            }
                        }

                    }

                }
            }
        }
        public static bool IsNumeric(object expression)
        {
            double retNum;
            bool isNum;
            if (expression.ToString() == Resource.CC009Default)
            {
                isNum = false;
            }
            else
            {
                isNum = Double.TryParse(Convert.ToString(expression), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out retNum);
            }
            return isNum;
        }

        #region Button Events

        /// <summary>
        /// gives the user the option to close or go back to the start screen. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender)
        {
            //clear the curves in 'windows_Closing' event.
            //clearcurves();
            Window parentWindow = Window.GetWindow(mainscreen);
            parentWindow.Close();
        }

        public void clearcurves()
        {
            int chartSeriesCnt = 0;

            chartSeriesCnt = LSChartReference.Data.Children.Count;

            for (int seriesCounter = 0; seriesCounter < chartSeriesCnt; seriesCounter++)
            {
                LSChartReference.Data.Children.RemoveAt(0);
            }

            chartSeriesCnt = LSIrChartReference.Data.Children.Count;

            for (int seriesCounter = 0; seriesCounter < chartSeriesCnt; seriesCounter++)
            {
                LSIrChartReference.Data.Children.RemoveAt(0);
            }

            chartSeriesCnt = MMxInChartReference.Data.Children.Count;

            for (int seriesCounter = 0; seriesCounter < chartSeriesCnt; seriesCounter++)
            {
                MMxInChartReference.Data.Children.RemoveAt(0);
            }

            chartSeriesCnt = INSTxInChartReference.Data.Children.Count;

            for (int seriesCounter = 0; seriesCounter < chartSeriesCnt; seriesCounter++)
            {
                INSTxInChartReference.Data.Children.RemoveAt(0);
            }

            chartSeriesCnt = GIChartReference.Data.Children.Count;

            for (int seriesCounter = 0; seriesCounter < chartSeriesCnt; seriesCounter++)
            {
                GIChartReference.Data.Children.RemoveAt(0);
            }
        }
        private void TemplateDeviceMismatch()
        {
            Window parent = new System.Windows.Window();
            parent.Height = 500;
            parent.Width = 500;
            string strMessage = string.Format(Resource.DeviceIncorrectMessage, Global.Connected_device_type, Global.selectedTemplateType);
            parent.Content = new Wizard_Scr_Error(Resource.DeviceMismatchTitle, Resource.RelayFeatureMisMatchSubtitle, Resource.DeviceMisMatchHeading, strMessage, "", "");
            parent.ShowDialog();
            //   Global.CloseSerialPortConnection();
        }
        private void Export(object sender)
        {
            if (Global.portName == string.Empty || Global.portName.Trim() == "")
            {
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.Error, Resource.ChooseCOMPort, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
                return;
            }
            Global.export_Successful = false;
            Global.export_Successful_ForSetpointReport = false;
            Global.isMCCBExport = false;
            Global.isExportControlFlow = false;
            port_name = Global.GlobalAutodetectPort();
            if (port_name != null && port_name != "Nothing")
            {
                Global.globalSerialPort.PortName = port_name;
                Global.portName = port_name;
            }
            if (Global.Connected_device_type != Global.device_type)
            {
               System.Windows.Application.Current.Dispatcher.Invoke(new UIDelegate(TemplateDeviceMismatch));
                return;
            }

            TripUnit.tripUnitString = "";

            try
            {
                //  Global.readTUType(); 

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Mouse.UpdateCursor();


                if ((Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                {
                    // For ACB device :--- firmware version 02.01.0008 and below doesn't support writing Baud rate 1200 to modbus communication.
                    //User will be asked to change the baud rate before exporting setpoints
                    //select System.Version to avoid conflict with iTextSharp.text namespace
                    var baseVersion = new System.Version("02.02.01");
                    var curVersion = new System.Version(Global.ACBdeviceFirmwareForBaudRateCheck);

                    if ((curVersion.CompareTo(baseVersion) == -1) && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                    {
                        Settings camConnStatus;
                        Settings baudRateSetting;

                        if (Global.device_type == Global.PTM_DEVICE)
                        {
                            Group commGroup = (Group)TripUnit.groups[2];
                            camConnStatus = commGroup.groupSetPoints[0];
                            baudRateSetting = commGroup.groupSetPoints[2];

                        }
                        else
                        {
                            Group commGroup = (Group)TripUnit.groups[4];
                            camConnStatus = commGroup.groupSetPoints[0];
                            baudRateSetting = commGroup.groupSetPoints[2];
                        }

                        // if ModbusCAM is slected && Baud rate is 1200 bps then check for Baudrate support
                        //1200 bps is not supported in f/w version 02.01.08 and earlier
                        if (camConnStatus.selectionValue == Resource.CCC001Item0001 && baudRateSetting.selectionValue == Resource.CCC003Item0000)
                        {
                            NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle,
                                                                                                                   Resource.InvalidSetPointsSubtitle,
                                                                                                                   Resource.BaudRateNotSupportedExportUI);
                            errorWindow.ShowDialog();
                            return;
                        }
                    }
                }


                Boolean isValid = TripParser.ConvertScreenInfoIntoSettingRequirements();
                if (!isValid)
                {
                    NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle,
                                                                                        Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.InvalidSetPoints);
                    errorWindow.ShowDialog();
                    return;
                }
                if (!IsRelayFunctionConfigurationValid())
                {
                    return;
                }
                if ((Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE) && Global.IsOffline && !Global.device_type_PXR10 && !Global.device_type_PXR20)
                {
                    deviceLanguage.getDeviceLanguages();
                    System.Threading.Thread.Sleep(1000);
                    do
                    {
                        Thread.Sleep(500);
                    } while (!Global.isReadAllLanguages);
                }

                if (!Global.IsOffline)
                {
                    TripUnit.tripUnitString = "";
                    TripUnit.ArmsModeData = "";
                    TripUnit.tripUnitIndexArray.Clear();
                    // Code to prepare groups - to add setpoint related data
                    if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
                    {
                        CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB();

                    }
                    if (Global.device_type == Global.PTM_DEVICE)
                    {
                        CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandACB_PTM();

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
                            Global.isExport_PXR10MCCB = true;
                        }
                        else
                        {
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandMCCB();
                        }
                    }
                    byteList.Clear();
                    bytListCheck = CommunicationHelperReadData.commandQueue.Count;

                    if (Global.isCommunicatingUsingBluetooth)
                    {
                        Thread.Sleep(500);
                        var task = Task.Run(async () => await GetByteListBle());
                        task.Wait();
                    }
                    else
                    {
                        byteList = CommunicationHelperReadData.readTripUnitGroupsDataFromDevice();
                    }

                    processDataFromDevice();
                    BeforeRefreshAuxStatus = string.Empty;
                    if ((Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE) && (Global.GlbstrBreakerFrame == Resource.SYS02Item0015))
                    {
                        BeforeRefreshAuxStatus = Global.GlbstrAuxConnected;
                        AuxPowerStatus objAuxPower = new AuxPowerStatus();

                        bool isAuxStatusReadSucces = objAuxPower.writeAuxPowerStatusToGlobal();
                        Global.updateModbusAndRelaySetpoint();
                    }
                    bool isMisMatch = false;

                    if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                    {


                        //if (OriginalsetpointLines == null)   //PXPM-7011  Archana
                        //{
                        //    SetOriginalSetpointLines();   // check do we need to call again  Archana
                        //}
                        if ((5 == OriginalsetpointLines.Count && (Global.device_type != Global.ACB_03_00_XX_DEVICE || Global.device_type != Global.ACB_03_01_XX_DEVICE || Global.device_type != Global.ACB_03_02_XX_DEVICE || Global.device_type != Global.PTM_DEVICE)) ||
                             ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && 6 == OriginalsetpointLines.Count))

                        {
                            OriginalsetpointLines.RemoveAt(0);
                        }
                        isMisMatch = IsMismatchInDeviceAndUserSetpoints(OriginalsetpointLines, TripUnit.tripUnitString) ? true : false;
                    }
                    else if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE)
                    {

                        // Commented to fix   //PXPM-7011  Archana
                        //Added by PP to fix PXPM-6157   
                        //Need to set original setpoints before export, so it will comapre latest user setpoints with device setpoints.
                        //SetOriginalSetpointLines();

                        if (!string.IsNullOrWhiteSpace(TripUnit.tripUnitString))
                        {
                            if (Global.OldOriginalsetpointLines == null)
                            {
                                Global.OldOriginalsetpointLines = new ArrayList();
                                int count = Global.device_type == Global.ACB_PXR35_DEVICE ? 12 : 7;
                                for (int i = 0; i <= count; i++)
                                {
                                    Global.OldOriginalsetpointLines.Add(string.Empty);
                                }
                                foreach (var setting in Global.ACBBackUpTripSetPoints)
                                {
                                    CommunicationHelperReadData.AppendToGroupString(Global.ACBBackUpTripSetPoints, setting.SetPointValue, setting.GroupId, ref Global.OldOriginalsetpointLines);
                                }

                                for (int i = Global.OldOriginalsetpointLines.Count - 1; i >= 0; i--)
                                {
                                    if (string.IsNullOrEmpty(Global.OldOriginalsetpointLines[i].ToString()))
                                    {
                                        Global.OldOriginalsetpointLines.RemoveAt(i);
                                    }
                                }
                                if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && 6 == Global.OldOriginalsetpointLines.Count)

                                {
                                    Global.OldOriginalsetpointLines.RemoveAt(0);
                                }
                            }
                            OriginalsetpointLines = Global.OldOriginalsetpointLines;
                            isMisMatch = IsMismatchInDeviceAndUserSetpoints(OriginalsetpointLines, TripUnit.tripUnitString) ? true : false;

                        }

                    }
                    else if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                    {

                        if (!string.IsNullOrWhiteSpace(TripUnit.tripUnitString))
                        {
                            if (Global.OldData == null)
                            {
                                Global.OldData = new List<string>();
                                if (Global.device_type_PXR10 && !(Global.device_type == Global.NZMDEVICE))
                                {
                                    int i = 0;
                                    if (MCCBBackUpTripSetPoints.Count > 0)
                                    {
                                        foreach (var setting in Global.grouplist)
                                        {
                                            if (Global.device_type_PXR10 && (setting.GroupId == "01" || setting.GroupId == "02" || (setting.GroupId == "06" /*&& Global.GlbstrMotor == Resource.GEN12Item0001*/) || setting.GroupId == "07"))
                                            {
                                                Global.OldData.Add(MCCBBackUpTripSetPoints.ElementAt(i));

                                            }
                                            i++;
                                        }
                                    }
                                    TripParser.ParseInputStringForConnect(TripUnit.tripUnitString, ' ');
                                    if (Global.grouplist.Count > 0 && !Global.IsOffline)
                                    {
                                        MCCBBackUpTripSetPoints = new List<String>();
                                        foreach (var setting in Global.grouplist)
                                        {
                                            MCCBBackUpTripSetPoints.Add(setting.SetPointValue);
                                        }
                                    }
                                    if (Global.OldData.Count == 0)
                                    {
                                        int k = 0;
                                        foreach (var setting in Global.grouplist)
                                        {
                                            if (Global.device_type_PXR10 && (setting.GroupId == "01" || setting.GroupId == "02" || (setting.GroupId == "06" /*&& Global.GlbstrMotor == Resource.GEN12Item0001*/) || setting.GroupId == "07"))
                                            {
                                                Global.OldData.Add(MCCBBackUpTripSetPoints.ElementAt(k));

                                            }
                                            k++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (MCCBBackUpTripSetPoints.Count > 0)
                                    {
                                        Global.OldData = MCCBBackUpTripSetPoints;
                                    }

                                }
                            }

                            TripParser.ParseInputStringForConnect(TripUnit.tripUnitString, ' ');  //it will get called twice for PXR10
                            Global.updateModbusAndRelaySetpoint();      //for PD2 it will get called twice done already on line number 2206
                            if (!Global.device_type_PXR10 || Global.device_type == Global.NZMDEVICE)
                            {
                                MCCBBackUpTripSetPoints = new List<String>();
                                foreach (var setting in Global.grouplist)
                                {
                                    MCCBBackUpTripSetPoints.Add(setting.SetPointValue);
                                }
                                if (Global.OldData.Count == 0)
                                {
                                    Global.OldData = MCCBBackUpTripSetPoints;


                                }
                            }
                            // List<string> userData = new List<string>();       //#COVARITY FIX  235024
                            List<string> userData = Global.OldData;
                            isMisMatch = IsMismatchInDeviceAndUserSetpoints_MCCB(userData, TripUnit.tripUnitString);

                            if (!BeforeRefreshAuxStatus.Equals(Global.GlbstrAuxConnected) && (Global.GlbstrBreakerFrame == Resource.SYS02Item0015 && !Global.IsOffline))
                            {
                                Wizard_Screen_MsgBox msgBox = new Wizard_Screen_MsgBox(Resource.SetpointMismatch, Resource.AuxPowerStatusChanged, string.Empty, false);
                                msgBox.Width = 480;
                                msgBox.Height = 220;
                                msgBox.Topmost = true;
                                msgBox.ShowDialog();
                            }
                        }
                    }
                    //********** //Archana uncomment afert PXR 35 implimentation 
                    if (!isMisMatch)
                    {
                        if (Errors.GetWizardFinished())
                        {
                            Global.OldData = null;
                            Global.OldOriginalsetpointLines = null;
                            ContinueImportWizard();

                        }
                    }

                    else
                    {
                        Wizard_Screen_MsgBox msgBox = new Wizard_Screen_MsgBox(Resource.SetpointMismatch, Resource.ClickRefresh, string.Empty, false);
                        msgBox.Width = 480;
                        msgBox.Height = 240;
                        msgBox.Topmost = true;
                        msgBox.ShowDialog();
                    }

                    //************  //Archana uncomment afert PXR 35 implimentation 
                }
                else
                {
                    if (port_name != "Nothing")
                    {
                        if (Global.device_type_PXR10) // condition for PXR10 export in offline
                        {
                            Global.isExport_PXR10MCCB = true;
                        }
                        ContinueImportWizard();
                    }
                    else
                    {
                        ImportWizard wizard_import = new ImportWizard(Global.str_wizard_ERROR);
                        wizard_import.ShowDialog();
                    }
                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Global.CloseSerialPortConnection();
                Errors.SetWizardFinished(false);
                ErrorScreen();
            }
            finally
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                Mouse.UpdateCursor();

                if (Global.export_Successful)
                {
                    if (TripUnit.IDTable.Count > 0 /*&& !Global.device_type_PXR10*/)
                    {
                        //#COVERITY FIX  375871,375874 : setpoint1,setpoint2 variables for MM mode removed as it was unused.
                        //Settings setpoint1 = null;
                        //Settings setpoint2 = null;
                        //if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE)
                        //{
                        //setpoint1 = TripUnit.getMaintenanceModeState();
                        //setpoint2 = TripUnit.getMaintenanceModeRemoteControl();
                        //}
                        if (!Global.IsOffline)
                        {
                            //if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                            //{
                            //    Global.updateMaintenanceModeControls(ref setpoint1, ref setpoint2);
                            //}


                            if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                            {

                                TripParser.ParseInputStringForConnect(TripUnit.tripUnitString, ' ');

                            }

                            SetOriginalSetpointLines();
                            // Global.UpdateMaintenanceModeState(ref setpoint1,  setpoint2.bValue);

                            Refresh(null);
                        }
                    }
                    exportedSetpoints();
                    if (Global.choice != Global.windowChoice.STARTSCREEN)
                    {
                        if (Global.show_curve)
                        {

                            if (Global.device_type == Global.NZMDEVICE)
                            {
                                NZMCurveCalculations.AddNZMDataToCurve();
                                NZMCurveCalculations.AddSCRNZMDataToCurve();
                            }
                            else
                            {
                                CurvesCalculation.AddDataToCurve();
                                CurvesCalculation.AddScrDataToCurve();
                            }

                            DisplayCurve();
                            DisplaySCRCurve();
                            if (Global.device_type != Global.NZMDEVICE)
                            {
                                setScale();
                            }
                            Addcurves();
                        }
                    }
                    //Record change in ChangeSummary only if atleast one LSIG test done
                    if ((Changes.Count > 0) && (Global.PrevioustestSummary.Count > 0))
                    {
                        if (Global.TestSetpointChanges.Count != 0)
                            Global.TestSetpointChanges.Clear();
                        foreach (var item in Changes)
                            Global.TestSetpointChanges.Add(item);

                    }
                    grdChangeSummary_CommitedChanges(true);// calling from Export 
                    BtnRestoreDefaultIsEnabled = false;

                }
                Global.isExport_PXR10MCCB = false;
                //Hide Curve if not available
                if (CurvesCalculation.InstxIn.Count == 0)
                {
                    TabIitemINSTInVisibility = Visibility.Collapsed;
                    TbCurvesSelectedIndex = 0;
                }
                if (CurvesCalculation.MMxIn.Count == 0)
                {
                    TabIitemMMInVisibility = Visibility.Collapsed;
                    TbCurvesSelectedIndex = 0;
                }
                if (CurvesCalculation.SCR_GFPickup.Count == 0)
                {
                    TabItemGroundInstVisibility = Visibility.Collapsed;
                    TbCurvesSelectedIndex = 0;
                }
            }
        }
        private async Task<List<byte[]>> GetByteListBle()
        {
            byteList = await CommunicationHelperReadData.readTripUnitGroupsDataFromDeviceBle().ConfigureAwait(true);
            return byteList;
        }
        public void exportedSetpoints()
        {
            foreach (var settingID in TripUnit.ID_list)
            {
                Settings setting = (Settings)TripUnit.IDTable[settingID];

                for (int i = Changes.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        if (Changes[i].ControlName.Contains(setting.ID))
                        {

                            if (!(Global.isUserHasExtPrev || Global.isAdmin))
                            {
                                String[] ItemNameArr = Changes[i].ItemName.Split();
                                if ((ItemNameArr[ItemNameArr.Length - 1]).Trim() == "*")
                                {

                                    if (Global.IsOffline && Global.device_type == Global.ACB_PXR35_DEVICE)
                                        UpdateSettingsAsChangeSummaryAfterExport(Changes[i].ControlName, Changes[i].ItemName, Changes[i].OrigValue, Changes[i].NewValue);

                                    Changes.RemoveAt(i);
                                }
                                else Changes[i].commitedSetpoint = true;

                            }
                            else
                            {
                                Changes[i].commitedSetpoint = true;
                            }



                            //if (((ItemNameArr[ItemNameArr.Length - 1]).Trim() == "*") && !(Global.isUserHasExtPrev || Global.isAdmin)) return true;
                            //else return false;

                            //try
                            //{
                            //    if (Changes[i].NewValue != Changes[i].OrigValue)
                            //    {
                            //        if (setting.type == Settings.Type.type_number)
                            //        {
                            //            Changes[i].commitedSetpoint = (setting.numberValue == Double.Parse(Changes[i].NewValue)) ? true : false;
                            //        }
                            //        else if (setting.type == Settings.Type.type_selection)
                            //        {
                            //            Changes[i].commitedSetpoint = (setting.selectionValue == Changes[i].NewValue) ? true : false;
                            //        }
                            //        else if (setting.type == Settings.Type.type_toggle)
                            //        {
                            //            string settingValue = setting.bValue ? setting.OnLabel : setting.OffLabel;
                            //            Changes[i].commitedSetpoint = (settingValue == Changes[i].NewValue) ? true : false;
                            //        }
                            //        else if (setting.type == Settings.Type.type_text)
                            //        {
                            //            Changes[i].commitedSetpoint = (setting.textvalue == Convert.ToDouble(Changes[i].NewValue)) ? true : false;
                            //        }
                            //        else if (setting.type == Settings.Type.type_bNumber)
                            //        {
                            //        }
                            //        else if (setting.type == Settings.Type.type_bool)
                            //        {
                            //            Changes[i].commitedSetpoint = (setting.bValue == Convert.ToBoolean(Changes[i].NewValue)) ? true : false;
                            //        }
                            //    }
                            //    // Changes[i].commitedSetpoint = true;
                            //}
                            //catch (Exception ex)
                            //{
                            //    LogExceptions.LogExceptionToFile(ex);
                            //}                            
                        }
                    }
                    catch (Exception ex)
                    {
                        LogExceptions.LogExceptionToFile(ex);
                    }

                }
            }
        }
        //private void Button_Click_Apply_SelectedSet(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string selectedSet = ((System.Windows.Controls.HeaderedItemsControl)sender).Header.ToString();

        //        if (!string.IsNullOrEmpty(selectedSet))
        //        {
        //            Global.PXR35_SelectedSetpointSet = selectedSet == Resource.SetpointSet1 ? "A" :
        //                (selectedSet == Resource.SetpointSet2 ? "B" :(
        //                selectedSet == Resource.SetpointSet3 ? "C" : "D"));
        //            ViewActiveSetHeaderName.Header = "  "+selectedSet;
        //        }

        //       // System.Windows.MessageBox.Show("Selected Set = " + Global.PXR35_SelectedSetpointSet);

        //        //btnRefresh(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogExceptions.LogExceptionToFile(ex);
        //    }
        //}

        private void ChangeActiveSet1(object sender)
        {
            try
            {
                ChangeActiveSet ObjChangeActiveSet = new ChangeActiveSet();

                Window ActiveState = new System.Windows.Window();
                ActiveState.Content = ObjChangeActiveSet;
                ActiveState.Height = 500;
                ActiveState.Width = 550;
                //  ActiveState.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                ActiveState.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                ActiveState.ResizeMode = ResizeMode.NoResize;
                ActiveState.ShowDialog();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }
        private void btnGroundFaultState(object sender)
        {
            try
            {
                GroundFaultState ObjGroundFaultState = new GroundFaultState();

                Window GroundFaultState = new System.Windows.Window();
                GroundFaultState.Content = ObjGroundFaultState;
                GroundFaultState.Height = 500;
                GroundFaultState.Width = 550;
                GroundFaultState.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                GroundFaultState.ResizeMode = ResizeMode.NoResize;
                GroundFaultState.ShowDialog();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public void SaveToFile(object sender)
        {
            try
            {
                Global.isSaveFile = true;
                if (TripUnit.MMforExportForSave != null)
                {
                    TripUnit.MMforExport = TripUnit.MMforExportForSave;     //Added by Astha to save maintenance mode selected by user before exporting it 

                }
                else
                {
                    TripUnit.MMforExportForSave = TripUnit.MMforExport;
                }
                Boolean isValid = TripParser.ConvertScreenInfoIntoSettingRequirements();
                if (!isValid)
                {
                    NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle,
                                                                                        Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.InvalidSetPoints);
                    errorWindow.ShowDialog();
                    return;
                }

                if (!IsRelayFunctionConfigurationValid())
                {
                    return;
                }

                ImportWizard wizard_import = new ImportWizard(Global.str_wizard_SAVEFILE);
                wizard_import.ShowDialog();

                if (Global.choice == Global.windowChoice.STARTSCREEN)
                {

                    Window parent = Window.GetWindow(mainscreen);
                    parent.Close(); // Added by AK
                    if (parent.IsActive == false)
                    {
                        Global.resetTrigger();
                    }
                    Global.choice = Global.windowChoice.MAIN;
                }
                if (TxtSettingslocationText == Resource.NewFile && !String.IsNullOrWhiteSpace(Global.fileNameForSaveOption))
                {
                    TxtSettingslocationText = Global.fileNameForSaveOption.Split('\\')[Global.fileNameForSaveOption.Split('\\').Count() - 1];
                    TxtSettingslocationTooltip = TxtSettingslocationText;
                    BtnSaveVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }
            finally
            {
                Global.isSaveFile = false;
            }
        }

        #endregion

        private bool IsRelayFunctionConfigurationValid()
        {
            try
            {
                var iterationGroups = TripUnit.groups;
                foreach (Group group in iterationGroups)
                {
                    if (group.groupSetPoints != null)
                    {
                        foreach (Settings setting in group.groupSetPoints)
                        {
                            if (setting.ID == "SYS131")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS132").FirstOrDefault();
                                if (valiset == null) // in case of ACB 2.1 ID is different
                                    valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS013").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {

                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig1InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                            if (setting.ID == "SYS141")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS142").FirstOrDefault();
                                if (valiset == null) // in case of ACB 2.1 ID is different
                                    valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS014").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {
                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig2InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                            if (setting.ID == "SYS151")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS152").FirstOrDefault();
                                if (valiset == null) // in case of ACB 2.1 ID is different
                                    valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS015").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {
                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig3InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                            if (setting.ID == "SYS131A")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS013A").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {

                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig1InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                            if (setting.ID == "SYS141A")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS014A").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {
                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig2InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                            if (setting.ID == "SYS151A")
                            {
                                var valiset = group.groupSetPoints.ToList().Where(x => x.ID == "SYS015A").FirstOrDefault();
                                if (setting.selectionValue != Resource.Off && !setting.notAvailable)
                                {
                                    if (valiset.comboBox.SelectedIndex == -1)
                                    {
                                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle, Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.RelayFunctionConfig3InvalidationMessage);
                                        errorWindow.ShowDialog();
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }
        }

        private void Button_Click(object sender)
        {

            if (Global.show_curve)
            {
                int lvSelectedTabIndex = TbCurvesSelectedIndex;
                Curves winCurves = new Curves(lvSelectedTabIndex);
                winCurves.ShowActivated = true;
                winCurves.ShowInTaskbar = true;
                winCurves.ShowDialog();
                //winCurves.Show();
                //    winCurves.ShowDialog();
                //winCurves.ShowInTaskbar = true;
            }
        }


        private string _selectedExportOption = "System.Windows.Controls.ComboBoxItem: Select Export Option";
        public string SelectedExportOption
        {
            get { return _selectedExportOption; }
            set
            {
                if (_selectedExportOption != value)
                {
                    if (value == "System.Windows.Controls.ComboBoxItem: Excel")
                    {
                        ButtonEnabeld = true;
                        ImageOpacity = 1;
                        GenerateReportToolTip = Resource.ToolTipGenerateExcel;
                    }
                    else if (value == "System.Windows.Controls.ComboBoxItem: PDF")
                    {

                        ButtonEnabeld = true;
                        ImageOpacity = 1;
                        GenerateReportToolTip = Resource.ToolTipGeneratePDF;
                    }
                    else
                    {

                        ButtonEnabeld = false;
                        ImageOpacity = 0.5;
                        GenerateReportToolTip = Resource.ToolTipSelectOption;
                    }

                    _selectedExportOption = value;
                    OnPropertyChanged(nameof(SelectedExportOption));

                }
            }
        }

        private string _GenerateReportToolTip = Resource.ToolTipSelectOption;
        public string GenerateReportToolTip
        {
            get { return _GenerateReportToolTip; }
            set
            {
                if (_GenerateReportToolTip != value)
                {
                    _GenerateReportToolTip = value;
                    OnPropertyChanged(nameof(GenerateReportToolTip));

                }
            }
        }


        private bool _ButtonEnabeld = false;
        public bool ButtonEnabeld
        {
            get { return _ButtonEnabeld; }
            set
            {
                if (_ButtonEnabeld != value)
                {
                    _ButtonEnabeld = value;
                    OnPropertyChanged(nameof(ButtonEnabeld));

                }
            }
        }

        private double _ImageOpacity = 0.5;
        public double ImageOpacity
        {
            get { return _ImageOpacity; }
            set
            {
                if (_ImageOpacity != value)
                {
                    _ImageOpacity = value;
                    OnPropertyChanged(nameof(ImageOpacity));

                }
            }
        }

        private void ExportSummary(object sender)
        {
            Global.listGroupAsLeftSetPoint.Clear();
            Global.listGroupAsLeftSetPoint = (ArrayList)TripUnit.groups.Clone();

            Global.dtChangeSummary = null;
            Nullable<bool> userInputresult = false;

            Global.dtChangeSummary = new DataTable();

            Global.IsNotFromTestFlowforUserInput = true;
            TestReportUserInput winTestReportUI = new TestReportUserInput();
            userInputresult = winTestReportUI.ShowDialog();
            winTestReportUI.Focus();
            //Setting Global.IsNotFromTestFlowforUserInput to false to invoke report from test feature.
            Global.IsNotFromTestFlowforUserInput = false;

            //if (userInputresult == false)
            //{
            //    return;
            //}
            if (userInputresult.HasValue && userInputresult.Value == true)
            {

                if (SelectedExportOption == "System.Windows.Controls.ComboBoxItem: PDF")
                {

                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.FileName = "Setpoints.pdf";
                    dlg.Filter = "PDF (*.pdf)|*.pdf";
                    dlg.DefaultExt = "*.pdf";
                    dlg.AddExtension = true;
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        string saveSummaryPDFFilePath = dlg.FileName;
                        // Add columns in datatable
                        foreach (DataGridColumn grdCol in grdChangeSummaryReference.Columns)
                        {
                            if (grdCol.Visibility == Visibility.Visible)
                            {

                                Global.dtChangeSummary.Columns.Add(new DataColumn());
                            }
                        }

                        Global.dtChangeSummary.Columns.Add(new DataColumn("SettingID"));
                        //Global.dtChangeSummary.Columns.Add(new DataColumn("Commited"));
                        Global.dtChangeSummary.Columns[0].ColumnName = "ParamName";
                        // Add records in datatable
                        for (int lv_RowCount = 0; lv_RowCount < Changes.Count; lv_RowCount++)
                        {

                            if (Changes[lv_RowCount].commitedSetpoint)
                            {
                                //dr[4] = "True";
                                DataRow dr = Global.dtChangeSummary.NewRow();
                                dr[0] = Changes[lv_RowCount].ItemName.Trim();
                                dr[1] = Changes[lv_RowCount].OrigValue.Trim();
                                dr[2] = Changes[lv_RowCount].NewValue.Trim();

                                int positionofUnderscore = Changes[lv_RowCount].ControlName.IndexOf("_", StringComparison.Ordinal);

                                dr[3] = Changes[lv_RowCount].ControlName.Substring(positionofUnderscore + 1);
                                Global.dtChangeSummary.Rows.Add(dr);
                            }
                            //else
                            //{
                            //    dr[4] = "False";
                            //}

                        }
                        //Curve data to be populated later in  WizardPDFWriter class to avoid duplicacy
                        //For Setpoint Feature specific report, 
                        //pass Document object as null as it will be constructed in  WizardPDFWriter class
                        //Document doc1 = null;
                        C1PrintDocument c1PrintDocument = null;
                        WizardPDFWriter writer = new WizardPDFWriter();
                        writer.CreateSummaryFile(saveSummaryPDFFilePath, ref c1PrintDocument);
                        if (!Global.isReportAlreadyOpen)
                        {
                            c1PrintDocument?.Export(dlg.FileName);
                            Process.Start(saveSummaryPDFFilePath);
                        }

                        Global.isReportAlreadyOpen = false;
                        //writer.createSummaryFile(saveSummaryPDFFilePath, ref doc1);
                    }
                }
                else if (SelectedExportOption == "System.Windows.Controls.ComboBoxItem: Excel")
                {

                    SaveFileDialog dlg1 = new SaveFileDialog();
                    dlg1.FileName = "Setpoints";
                    dlg1.Filter = "Excel (*.xlsx)|*.xlsx";
                    dlg1.DefaultExt = "*.xlsx";
                    dlg1.AddExtension = true;
                    Nullable<bool> result = dlg1.ShowDialog();

                    if (result == true)
                    {
                        string saveSummaryExcelFilePath = dlg1.FileName;
                        if (File.Exists(saveSummaryExcelFilePath) &&  IsExcelFileOpen(saveSummaryExcelFilePath))
                        {
                            Wizard_Screen_MsgBox msgBox = new Wizard_Screen_MsgBox(null, Resource.PXRWizardReportDocumentError, null, false);
                            msgBox.ShowDialog();
                            return;
                        }


                        // Add columns in datatable
                        foreach (DataGridColumn grdCol in grdChangeSummaryReference.Columns)
                        {
                            if (grdCol.Visibility == Visibility.Visible)
                            {

                                Global.dtChangeSummary.Columns.Add(new DataColumn());
                            }
                        }

                        Global.dtChangeSummary.Columns.Add(new DataColumn("SettingID"));
                        //Global.dtChangeSummary.Columns.Add(new DataColumn("Commited"));
                        Global.dtChangeSummary.Columns[0].ColumnName = "ParamName";
                        // Add records in datatable
                        for (int lv_RowCount = 0; lv_RowCount < Changes.Count; lv_RowCount++)
                        {

                            if (Changes[lv_RowCount].commitedSetpoint)
                            {
                                //dr[4] = "True";
                                DataRow dr = Global.dtChangeSummary.NewRow();
                                dr[0] = Changes[lv_RowCount].ItemName.Trim();
                                dr[1] = Changes[lv_RowCount].OrigValue.Trim();
                                dr[2] = Changes[lv_RowCount].NewValue.Trim();

                                int positionofUnderscore = Changes[lv_RowCount].ControlName.IndexOf("_", StringComparison.Ordinal);

                                dr[3] = Changes[lv_RowCount].ControlName.Substring(positionofUnderscore + 1);
                                Global.dtChangeSummary.Rows.Add(dr);
                            }
                            //else
                            //{
                            //    dr[4] = "False";
                            //}

                        }
                        //Curve data to be populated later in  WizardPDFWriter class to avoid duplicacy
                        //For Setpoint Feature specific report, 
                        //pass Document object as null as it will be constructed in  WizardPDFWriter class
                        //Document doc1 = null;
                        C1PrintDocument c1PrintDocument = null;
                        WizardPDFWriter writer = new WizardPDFWriter();
                        writer.CreateSummaryFile(saveSummaryExcelFilePath, ref c1PrintDocument);
                        if (!Global.isReportAlreadyOpen)
                        {
                            c1PrintDocument?.Export(dlg1.FileName);
                        }

                        Global.isReportAlreadyOpen = false;

                        using (C1XLBook wb = new C1XLBook())
                        {
                            wb.Load(saveSummaryExcelFilePath);
                            XLSheet sheet = wb.Sheets["Sheet1"];
                            // Specify the image and assign it to the cell's Value property
                            var img = System.Drawing.Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Icons\\EatonLogo.png");

                            var resizedImg = new System.Drawing.Bitmap(img, new System.Drawing.Size(200, 100));

                            sheet[0, 0].Value = resizedImg;

                            sheet[0, 1].Value = Resource.ChangeSummaryHeader; ;

                            using (var font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Italic))
                            {
                                var style = new C1.C1Excel.XLStyle(sheet.Book)
                                {

                                    AlignHorz = C1.C1Excel.XLAlignHorzEnum.Center, // Center horizontally
                                    AlignVert = C1.C1Excel.XLAlignVertEnum.Center, // Center vertically
                                    Font = font
                                };

                                sheet[0, 1].Style = style;
                            }

                            // Save and open the new book
                            wb.Save(saveSummaryExcelFilePath);
                        }

                        Process.Start(saveSummaryExcelFilePath);

                    }



                }
            }
        }

        private static bool IsExcelFileOpen(string filePath)
        {
            try
            {
                // Try to open the file exclusively
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fs.Close();
                }
                return false; // File is not open
            }
            catch (IOException)
            {
                return true; // File is open
            }
        }

        private bool checkExportStatusForSetpoint(string ItemName)
        {
            if (Global.device_type != Global.ACB_PXR35_DEVICE) return false;
            //For PXR35 list of protection groups which are not supported for ristricted user export 
            //groupID == Group 0 , Group 3 ,Group 4 from template  are allowed to write 

            String[] ItemNameArr = ItemName.Split();
            if (((ItemNameArr[ItemNameArr.Length - 1]).Trim() == "*") && !(Global.isUserHasExtPrev || Global.isAdmin)) return true;
            else return false;
        }
        public void grdChangeSummary_CommitedChanges(bool isCalledFromExportFlow = false)
        {
            try
            {
                if (grdChangeSummaryReference.Items != null)
                {
                    for (int i = 0; i < grdChangeSummaryReference.Items.Count; i++)
                    {
                        DataGridRow row = (DataGridRow)grdChangeSummaryReference.ItemContainerGenerator.ContainerFromIndex(i);

                        if (row != null)
                        {
                            ChangeSummary dr = (ChangeSummary)row.DataContext;
                            // bool isRestrictedSetpoint = checkExportStatusForSetpoint(dr.ItemName);



                            if (dr != null && dr.commitedSetpoint)
                            {
                                row.Background = Brushes.LightGreen;
                            }
                            else
                            {
                                row.Background = Brushes.Orange;
                                //if (isCalledFromExportFlow && Global.device_type == Global.ACB_PXR35_DEVICE) UpdateSettingsAsChangeSummaryAfterExport(ref dr);
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

        private void UpdateSettingsAsChangeSummaryAfterExport(string ControlName, string ItemName, string OrigValue, string NewValue)
        {
            try
            {
                Global.isUpdatingSettingBackForRestrictedUser = true;
                string ID = ControlName.Split('_').Count() == 1 ? ControlName : ControlName.Split('_')[1];
                Settings SettingsObj = (Settings)TripUnit.IDTable[ID];
                if (NewValue != OrigValue)
                {
                    if (SettingsObj.type == Settings.Type.type_number)
                    {
                        //SettingsObj.numberDefault = Double.Parse(dr.NewValue);
                        if (SettingsObj.numberValue.ToString() != OrigValue)
                        {
                            SettingsObj.SettingValueChange -= set_SettingValueChange;
                            SettingsObj.numberValue = Double.Parse(OrigValue);

                            SettingsObj.SettingValueChange += set_SettingValueChange;
                        }
                    }
                    else if (SettingsObj.type == Settings.Type.type_selection)
                    {
                        SettingsObj.selectionValue = OrigValue;
                        //SettingsObj.defaultSelectionValue = dr.NewValue;
                    }
                    else if (SettingsObj.type == Settings.Type.type_toggle)
                    {
                        SettingsObj.bValue = (SettingsObj.OnLabel == OrigValue) ? true : false;
                        SettingsObj.bValueReadFromTripUnit = SettingsObj.bValue;
                        //SettingsObj.toggle.IsChecked = SettingsObj.bValue;
                    }
                    else if (SettingsObj.type == Settings.Type.type_text)
                    {
                        SettingsObj.textvalue = Convert.ToDouble(OrigValue);
                    }
                    else if (SettingsObj.type == Settings.Type.type_bNumber)
                    {
                    }
                    else if (SettingsObj.type == Settings.Type.type_bool)
                    {
                        SettingsObj.bValue = (SettingsObj.OnLabel == OrigValue) ? true : false;
                    }
                    SettingsObj.notifyDependents();
                    SettingsObj.uiUpdate(ref SettingsObj);
                    ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
            finally
            {
                Global.isUpdatingSettingBackForRestrictedUser = false;
            }
        }

        // check if file is already open
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return false;
        }

        private void UndoAllChanges(object sender)
        {
            Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.UndoChanges, Resource.UndoChangesWarning, "", true);
            MsgBoxWindow.Topmost = true;
            MsgBoxWindow.ShowDialog();

            if (MsgBoxWindow.DialogResult.HasValue)
            {
                if (MsgBoxWindow.DialogResult == true)
                {
                    for (int i = Changes.Count - 1; i >= 0; i--)
                    {
                        if (!Changes[i].commitedSetpoint)
                        {
                            Changes.RemoveAt(i);
                        }
                    }
                    //Changes.Clear();
                    //if(!(Changes.Count > 0))
                    //{
                    BtnRestoreDefaultIsEnabled = false;
                    //}
                    Global.IsUndoLock = true;
                    if (SourceScreen == Global.str_wizard_CONNECT || SourceScreen == Global.str_wizard_EXPORTFILE)
                    {

                        if ((Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE) && !Global.IsOffline)
                        {
                            Global.MatchOutputFileToModelFileSettings_online();
                            TripUnit.userUnitType = Global.GlbstrUnitType;
                            TripUnit.userRatingPlug = Global.GlbstrCurrentRating;
                            TripUnit.deviceBreakerInformation = Global.GlbstrBreakerFrame;
                            Global.updateDetailedGroupForMCCBTemplate();
                        }
                        else
                        {
                            Global.MatchOutputFileToModelFileSettings();
                        }
                        ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);

                    }
                    else if (SourceScreen == Global.str_wizard_OPENFILE || SourceScreen == Global.str_wizard_NEWFILE)
                    {
                        try
                        {
                            Global.transferDefaultValues_OFFLINE();
                            ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);

                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                            throw;
                        }
                    }
                    Thread.Sleep(500);
                    // clearCurveData();
                    try
                    {
                        foreach (String settingID in TripUnit.IDTable.Keys)
                        {
                            var set1 = ((Settings)TripUnit.IDTable[settingID]);
                            set1.notifyDependents();

                            set1.SettingValueChange -= set_SettingValueChange;
                            set1.SettingValueChange += set_SettingValueChange;
                            set1.CurveCalculationChanged -= CurveCalculationApplyToChart;
                            set1.CurveCalculationChanged += CurveCalculationApplyToChart;
                            //if (Global.selectedTemplateType != Global.MCCBTEMPLATE)
                            //{
                            //var curveSetting = ((Settings)((Group)(TripUnit.groups[1])).groupSetPoints[0]);
                            //curveSetting.addDataToCurveCollection -= DisplayCurve;
                            //curveSetting.addDataToCurveCollection += DisplayCurve;
                            //curveSetting.AddDataToCurve();
                            //  }
                        }
                        if (Global.show_curve)
                        {

                            if (Global.device_type == Global.NZMDEVICE)
                            {
                                NZMCurveCalculations.AddNZMDataToCurve();
                                NZMCurveCalculations.AddSCRNZMDataToCurve();
                            }
                            else
                            {
                                CurvesCalculation.AddDataToCurve();
                                CurvesCalculation.AddScrDataToCurve();
                            }

                            DisplayCurve();
                            DisplaySCRCurve();
                            if (Global.device_type != Global.NZMDEVICE)
                            {
                                setScale();
                            }
                            Addcurves();

                        }
                    }
                    catch (Exception ex)
                    {
                        LogExceptions.LogExceptionToFile(ex);
                        throw;
                    }


                    //Below 2 lines commented by AK. Curves will not get updated on valu change(As suggested by Todd in 2.9 revire comments)
                    //Hence no need to update curve on Undo
                    //  Global.AddDataToCurve();
                    //  DisplayCurve();
                    Global.GeneralSetpointsDisable(!Global.IsOffline);

                    //Cleared change summary on undo click
                    if (Changes.Count > 0)
                    {
                        Changes.Clear();
                        BtnRestoreDefaultIsEnabled = false;
                    }

                    //Added by PP to fix PXPM-6396
                    Global.export_Successful_ForSetpointReport = false;
                }
            }

            SetOriginalSetpointLines();
            Global.IsUndoLock = false;
        }

        /// <summary>
        /// Code to check if the data read operation is completed or not
        /// </summary>
        /// <returns></returns>
        //private bool CheckByteList()
        //{
        //    if (byteList != null && byteList.Count == 4)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// Continue wizard
        /// </summary>
        private void ContinueImportWizard()
        {
            ImportWizard wizard_import = new ImportWizard(Global.str_wizard_EXPORTUI);

            // this makes the window modal. meaning the user
            // cannot change what is going on here until they 
            // close the import window. 
            wizard_import.ShowDialog();

            if (Global.choice == Global.windowChoice.STARTSCREEN)
            {
                Window parent = Window.GetWindow(mainscreen);
                parent.Close();
                if (parent.IsActive == false)
                {
                    Global.resetTrigger();
                }
                //   Global.choice = Global.windowChoice.MAIN;
            }
            //Global.Last_CompPort = Global.portName;
            DeviceDetailsAndSerialNumber.setAppDataForReport();
        }

        /// <summary>
        /// Converts input string into 4 digit hex value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertToFourDigitHex(string str)
        {
            string strConvertedHexValue = string.Empty;

            for (int k = 0; k < 4; k++)
            {
                if (str.Length < 4)
                {
                    strConvertedHexValue = "0" + str;
                }
            }

            return strConvertedHexValue;
        }

        /// <summary>
        /// Code to get Data from trip unit
        /// </summary>
        private void processDataFromDevice()
        {
            try
            {
                if (CommunicationHelperReadData.commandQueue.Count == 0)
                {
                    // If data of all 4 groups are received then show set points page
                    if (byteList != null && (byteList.Count >= bytListCheck || (byteList.Count >= bytListCheck - 1 && Global.isCommunicatingUsingBluetooth)))
                    {
                        Errors.SetWizardFinished(true);
                        TripUnit.ArmsModeData = UsbCdcDataHandler.GetArmsMode(byteList[0]);
                        TripUnit.tripUnitString = String.Empty;
                        TripUnit.tripUnitString = UsbCdcDataHandler.GetHexData(byteList);
                        if (!Global.isCommunicatingUsingBluetooth)
                        {
                            Global.CloseSerialPortConnection();
                        }
                    }
                    else
                    {
                        Errors.SetWizardFinished(false);
                       System.Windows.Application.Current.Dispatcher.Invoke(new UIDelegate(ErrorScreen));
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Global.CloseSerialPortConnection();
                Errors.SetWizardFinished(false);
                ErrorScreen();
            }
        }

        /// <summary>
        /// Error Screen display with possible solutions
        /// </summary>
        private void ErrorScreen()
        {
            //Height="500" Width="500" WindowStyle="ToolWindow" WindowState="Normal" ResizeMode="NoResize"
            Window window = new Window();
            window.Height = 500;
            window.Width = 500;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.WindowState = WindowState.Normal;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Content = new Wizard_Scr_Error(Resource.ConnectToTripUnitTitle,
                                                              Resource.ConnectToTripUnitSubtitle,
                                                              Resource.ConnectToTripUnitHeading,
                                                              Resource.ConnectToTripUnitError, "", "");
            window.Show();
            return;
        }
        // Added by Astha
        private void InvalidPositionOfMMRotarySwitch()
        {
            Window window = new Window();
            window.Height = 500;
            window.Width = 500;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.WindowState = WindowState.Normal;
            window.ResizeMode = ResizeMode.NoResize;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            string strMessage = string.Format(Resource.InvalidMMRotarySwitchPosition);
            window.Content = new Wizard_Scr_Error(Resource.InvalidMMRotarySwitchPositionTitle,
                                                          "", Resource.InvalidMMRotarySwitchPositionHeading,
                                                          strMessage, "", "");
            window.Show();
            return;

        }
        /// <summary>
        /// Code to check if there is any mismatch between User Setpoints and Device Setpoints
        /// </summary>
        /// <param name="userSetpoints"></param>
        /// <returns></returns>      

        public static bool IsMismatchInDeviceAndUserSetpoints(ArrayList userData, String deviceData)
        {


            bool isDataMisMatch = false;

            String[] devideDataArray = deviceData.Split('\n');
            int CountForSetpointsFromTripunit = 0;

            try
            {
                string strSetPointDataSent = string.Empty;
                string strSetPointWrittenToUnit = string.Empty;
                int numberOfGroups = Global.device_type == Global.ACB_PXR35_DEVICE ? 12 : 4;
                if(Global.device_type == Global.PTM_DEVICE)
                     numberOfGroups = Global.device_type_PXR10 ? 2 : 5;

                for (int i = 0; i < numberOfGroups; i++)
                {
                    strSetPointDataSent = strSetPointDataSent + ' ' + userData[i];
                    strSetPointWrittenToUnit = strSetPointWrittenToUnit + ' ' + devideDataArray[i];

                }
                //strSetPointDataSent.TrimStart(); Commented this to fix coverity scan issue - is only useful for its return value, which is ignored.
                //strSetPointWrittenToUnit.TrimStart(); Commented this to fix coverity scan issue - is only useful for its return value, which is ignored.
                TripParser.ParseDataSentByUser(strSetPointDataSent, ' ');
                TripParser.ParseDataWrittenToUnit(strSetPointWrittenToUnit, ' ');
                TripParser.GetStyleRatingPlugDetails();

                //string missmatchLocations = string.Empty;
                //for (int i = 1; i < TripUnit.setPointDataWrittenToUnit.Count; i++)
                //{

                //    if (!TripUnit.setPointDataWrittenToUnit[i].Equals(TripUnit.setpointsDataSentbyUser[i]))
                //    {
                //        missmatchLocations += "***Seq No ::" + i.ToString() + "DeviceVal  " + TripUnit.setPointDataWrittenToUnit[i] + " :: AppVal " + TripUnit.setpointsDataSentbyUser[i] + "\n";
                //    }
                //}
                //if (missmatchLocations != string.Empty)
                //    System.Windows.MessageBox.Show(missmatchLocations);

                for (int i = 1; i < TripUnit.setPointDataWrittenToUnit.Count; i++)
                {

                    if (!TripUnit.setPointDataWrittenToUnit[CountForSetpointsFromTripunit].Equals(TripUnit.setpointsDataSentbyUser[CountForSetpointsFromTripunit]))
                    {
                        if (CountForSetpointsFromTripunit == 16 && Global.device_type == Global.ACBDEVICE && Global.deviceFirmware == Resource.GEN002Item0007)
                        {
                            CountForSetpointsFromTripunit++;
                            continue;
                        }// For ACB  firmware version 2.0.99 do not consider hliload setpoint as missmatch 
                        //as it is shown as 105 on screen but in trip unit it is having value as 85 , this is done to handle the changes in firware.
                        isDataMisMatch = true;
                        if (Global.PrevioustestSummary.Count > 0)
                        {
                            int[] ACBsetpoints_testtable = { };

                            if (Global.device_type == Global.ACBDEVICE)
                                ACBsetpoints_testtable = new int[] { 3, 4, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
                            else if (Global.device_type == Global.PTM_DEVICE)
                                ACBsetpoints_testtable = new int[] { 4, 5, 6, 21, 22, 23, 24, 25, 27, 28, 29, 30, 31, 32, 33, 34, 35, 37 };

                            else if (Global.device_type == Global.ACB_02_01_XX_DEVICE)
                                ACBsetpoints_testtable = new int[] { 4, 5, 6, 21, 22, 23, 24, 25, 27, 28, 29, 30, 31, 32, 33, 34, 35, 37 };
                            else if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE)
                                ACBsetpoints_testtable = new int[] { 4, 5, 6, 21, 22, 23, 24, 25, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 40 };
                            else if (Global.device_type == Global.ACB_PXR35_DEVICE)
                                ACBsetpoints_testtable = new int[] { 4, 5, 39, 41, 42, 43, 55, 45, 46, 47, 48, 40, 49, 50, 51, 52, 53, 54, 60 };

                            if (ACBsetpoints_testtable.Contains(CountForSetpointsFromTripunit))
                                Global.IsTestTablechanged = true;
                        }
                    }
                    //bool isPresentInChangeSummary = false;
                    //if (!isPresentInChangeSummary)
                    //{
                    //    isDataMisMatch = true;
                    //    //break;
                    //}
                    if (Global.IsTestTablechanged == true)
                        break;

                    CountForSetpointsFromTripunit++;
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
            if (true == isDataMisMatch)
            {
                // first remove the item from collection,if found

            }

            return isDataMisMatch;
        }

        public static bool IsMismatchInDeviceAndUserSetpoints_MCCB(List<string> userData, String deviceData)
        {


            List<string> mccbTripdeviceData = new List<string>();
            string tripUnitString;// = string.Empty;  //#COVARITY FIX   234797
            string[] tripUnitstringArray = null;
            bool isDataMisMatch = false;
            String[] devideDataArray = deviceData.Split('\n');
            // String[] userDataArray = userData.Split('\n');
            // ArrayList tripUnitHexArray = new ArrayList();     //#COVARITY FIX  235026

            tripUnitString = deviceData.Replace("\n", "");
            tripUnitstringArray = tripUnitString.Split(' ').ToArray();
            //this line create a arraylist using array elements.
            ArrayList tripUnitHexArray = new ArrayList(tripUnitstringArray);
            //if (Global.device_type_PXR10)
            //{
            //    tripUnitHexArray.RemoveAt(tripUnitHexArray.Count - 1);
            //    tripUnitHexArray.RemoveAt(tripUnitHexArray.Count - 1);
            //    userData.RemoveAt(userData.Count - 1);
            //}
            //else
            //{
            tripUnitHexArray.RemoveAt(tripUnitHexArray.Count - 1);

            //  }

            foreach (var deviceItem in tripUnitHexArray)
            {
                mccbTripdeviceData.Add(deviceItem.ToString());
            }


            //string missmatchLocations = string.Empty;
            //for (int i = 0; i < userData.Count; i++)
            //{
            //    if (userData[i] != mccbTripdeviceData[i])
            //    {
            //        missmatchLocations += "***Seq No ::" + i.ToString() + "DeviceVal  " + mccbTripdeviceData[i] + " :: AppVal " + userData[i] + "\n";
            //    }
            //}
            //if (missmatchLocations != string.Empty)
            //    System.Windows.MessageBox.Show(missmatchLocations);

            try
            {
                string strSetPointDataSent = string.Empty;
                string strSetPointWrittenToUnit = string.Empty;
                //if (Global.device_type_PXR10)
                //{
                //    for (int i = 0; i < userData.Count; i++)
                //    {
                //        strSetPointDataSent = strSetPointDataSent + ' ' + userData[i];
                //    }
                //    for (int i = 0; i < devideDataArray.Length; i++)
                //    {

                //        //     strSetPointDataSent = strSetPointDataSent + ' ' + userData[i];
                //        strSetPointWrittenToUnit = strSetPointWrittenToUnit + ' ' + devideDataArray[i];
                //    }
                //}
                //else
                //{
                for (int i = 0; i < userData.Count; i++)
                {
                    strSetPointDataSent = strSetPointDataSent + ' ' + userData[i];
                }
                for (int i = 0; i < devideDataArray.Length; i++)
                {

                    //  strSetPointDataSent = strSetPointDataSent + ' ' + userData[i];
                    strSetPointWrittenToUnit = strSetPointWrittenToUnit + ' ' + devideDataArray[i];
                }
                //  }

                //strSetPointDataSent.TrimStart(); Commented this to fix coverity scan issue- is only useful for its return value, which is ignored.
                //strSetPointWrittenToUnit.TrimStart(); Commented this to fix coverity scan issue - is only useful for its return value, which is ignored.

                TripParser.ParseDataSentByUser(strSetPointDataSent, ' ');
                TripParser.ParseDataWrittenToUnit(strSetPointWrittenToUnit, ' ');

                ArrayList diff = new ArrayList();
                if (mccbTripdeviceData.SequenceEqual(userData))
                {

                    isDataMisMatch = false;
                }

                else
                {
                    isDataMisMatch = true;
                }

                for (int i = 0; i < mccbTripdeviceData.Count; i++)
                {
                    if (mccbTripdeviceData[i] != userData[i])
                    {
                        isDataMisMatch = true;
                        if (Global.PrevioustestSummary.Count > 0)
                        {
                            int[] Setpoints_testtable = { };
                            //Added Motor protection setpoints in the list to create new Protection Settings table
                            if (Global.device_type == Global.MCCBDEVICE)
                            {
                                Setpoints_testtable = new int[] { 5, 6, 26, 27, 28, 29, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41 ,92, 93, 94, 95, 96, 97, 98, 99, 100,
                                                                     101, 102, 103, 104, 105, 106, 107, 108, 109, 110};
                                if (Setpoints_testtable.Contains(i))
                                    Global.IsTestTablechanged = true;
                            }
                            else if (Global.device_type == Global.NZMDEVICE)
                            {
                                Setpoints_testtable = new int[] { 5, 6, 26, 27, 28, 29, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41 , 92, 93, 94, 95, 96, 97, 98, 99, 100,
                                                                     101, 102, 103, 104, 105, 106, 107, 108, 109, 110};
                                if (Setpoints_testtable.Contains(i))
                                    Global.IsTestTablechanged = true;
                            }
                        }

                    }
                    if (Global.IsTestTablechanged == true)
                        break;
                }
                if (Global.device_type_PXR10 && Global.GlbstrMotor == Resource.GEN12Item0001 && mccbTripdeviceData[31] == userData[31] && mccbTripdeviceData[32] == userData[32] && mccbTripdeviceData[33] == userData[33] && (mccbTripdeviceData[69] != userData[69] || mccbTripdeviceData[70] != userData[70] || mccbTripdeviceData[71] != userData[71]))
                {
                    isDataMisMatch = false;
                    Global.IsTestTablechanged = false;
                }
                else if (Global.device_type_PXR10 && Global.GlbstrMotor == Resource.GEN12Item0000 && mccbTripdeviceData[31] == userData[31] && mccbTripdeviceData[32] == userData[32] && mccbTripdeviceData[33] == userData[33] && (mccbTripdeviceData[45] != userData[45] || mccbTripdeviceData[46] != userData[46] || mccbTripdeviceData[47] != userData[47]))
                {
                    isDataMisMatch = false;
                    Global.IsTestTablechanged = false;
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex.Message);
            }

            return isDataMisMatch;
        }

        /// <summary>
        /// Converts observable collection
        /// </summary>
        private void ConvertObservableCollectionToDataTable()
        {
            Global.dtChangeSummary = new DataTable();

            foreach (DataGridColumn grdCol in grdChangeSummaryReference.Columns)
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
        }

        /*  Do not delete -- iMPORTANT LOGIC
         * 
                private void btnBack_Original_Click(object sender, RoutedEventArgs e)
                {

                    //Before config window is closed ,capture selected values and save in rawsetpoints array
                    Global.configScreenCloseSource = "BackButton";


                    Boolean isValid = TripParser.ConvertScreenInfoIntoSettingRequirements();
                    if (!isValid)
                    {
                        NotificationWindow_Error errorWindow = new NotificationWindow_Error(Global.msg_InvalidSetPoints_Title,
                                                                                            Global.msg_InvalidSetPoints_Subtitle,
                                                                                            Global.msg_InvalidSetPoints);
                        errorWindow.ShowDialog();
                        return;
                    }

                    TripParser.ConvertScreenInfoIntoSettingRequirements();
                    string strRawString = null;
                    for (int i = 0; i < 11; i++)
                    {
                        strRawString = strRawString + "\n" + TripParser.GetConvertedSetPoints()[i].ToString();
                    }

                    // copy all the rawsetpoint values(selected values) in to "TripUnit.rawSetPoints" arraylist.
                    //Required to retain values while user selected different style
                    TripParser.ParseInputString(strRawString, ' ');

                    //Close the config screen
                    Window thisWin = (Window)(((UserControl)(this)).Parent);
                    thisWin.Close();




                    //create copy of selectedvalues(rawsetpoints). Later use for recreating config screen and match the selected values
                    //System.Collections.ArrayList groupsCopy = new System.Collections.ArrayList();
                    ArrayList rawSetPointsCopy = new ArrayList();
                    //groupsCopy = TripUnit.groups;
                    rawSetPointsCopy = TripUnit.rawSetPoints;

                    if (Global.strDialogResult == "CANCEL" || Global.strDialogResult == "NO") // means user responded as "NO" to close windoe dialog
                    {
                        return;
                    }

                    //clear changesummary collection;
                    Changes.Clear();


                    StartScreen_Connect_Window objStartScreen = new StartScreen_Connect_Window();
                    Errors.SetWizardFinished(false);

                    ImportWizard winWizard = new ImportWizard(Global.str_wizard_BACKTONEW);
                    winWizard.ShowDialog();

                    TripUnit.rawSetPoints = rawSetPointsCopy;

                    objStartScreen.closeStartScreen(Global.str_wizard_BACKTONEW);
                }
        */

        private void Save(object sender)
        {
            try
            {
                Global.isSaveFile = true;

                if (TripUnit.MMforExportForSave != null)
                {
                    TripUnit.MMforExport = TripUnit.MMforExportForSave;     //Added by Astha to save maintenance mode selected by user before exporting it 

                }
                else
                {
                    TripUnit.MMforExportForSave = TripUnit.MMforExport;
                }
                Boolean isValid = TripParser.ConvertScreenInfoIntoSettingRequirements();
                if (!isValid)
                {
                    NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle,
                                                                                        Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.InvalidSetPoints);
                    errorWindow.ShowDialog();
                    return;
                }
                if (!IsRelayFunctionConfigurationValid())
                {
                    return;
                }
                FileCreator create = new FileCreator();
                create.create_etParaFile(Global.fileNameForSaveOption);
                Global.changesSaved = true;
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.SaveToFile, Resource.DataSaved, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }
            finally
            {
                Global.isSaveFile = false;
            }
        }

        private void Refresh(object sender)
        {
            try
            {
                if (Global.portName == string.Empty || Global.portName.Trim() == "")
                {
                    Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.Error, Resource.ChooseCOMPort, "", false);
                    MsgBoxWindow.Topmost = true;
                    MsgBoxWindow.ShowDialog();
                    return;
                }
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Mouse.UpdateCursor();

                Boolean isValid = TripParser.ConvertScreenInfoIntoSettingRequirements();
                if (!isValid && (Global.device_type != Global.MCCBDEVICE && Global.device_type != Global.NZMDEVICE))
                {
                    NotificationWindow_Error errorWindow = new NotificationWindow_Error(Resource.InvalidSetPointsTitle,
                                                                                        Resource.InvalidSetPointsSubtitle,
                                                                                        Resource.InvalidSetPoints);
                    errorWindow.ShowDialog();
                    return;
                }
                if (!IsRelayFunctionConfigurationValid())
                {
                    return;
                }

                Global.parsed_Template_File = string.Empty;// It will parse the template file again while refresh
                Global.parseFor_IDTable = string.Empty;

                var setpointLines = new ArrayList();
                var mccbSetpoints = new List<string>();
                if (Global.device_type != Global.MCCBDEVICE && Global.device_type != Global.NZMDEVICE)
                {
                    setpointLines = TripParser.GetConvertedSetPoints();
                }
                else
                {

                    foreach (var setting in Global.grouplist)
                    {

                        if (Global.device_type == Global.NZMDEVICE)
                        {
                            mccbSetpoints.Add(setting.SetPointValue);
                        }
                        else if (Global.device_type_PXR10 && (setting.GroupId == "01" || setting.GroupId == "02" || (setting.GroupId == "06" /*&& Global.GlbstrMotor == Resource.GEN12Item0001*/) || setting.GroupId == "07"))
                        {
                            mccbSetpoints.Add(setting.SetPointValue);
                        }
                        else if (!Global.device_type_PXR10)
                        {
                            mccbSetpoints.Add(setting.SetPointValue);
                        }
                    }

                }

                //  TripParser.ConvertScreenInfoIntoSettingRequirements();
                //  ArrayList setpointLines = TripParser.GetConvertedSetPoints();
                //if (setpointLines.Count > 0)
                //{
                //    _convertedSetPoints.RemoveAt(0);

                //}

                if (!Global.IsOffline)
                {
                    TripUnit.tripUnitString = string.Empty;
                    TripUnit.ArmsModeData = string.Empty;
                    TripUnit.tripUnitIndexArray.Clear();

                    // Code to prepare groups - to add setpoint related data

                    if (Global.device_type == Global.PTM_DEVICE)
                    {
                        CommunicationHelperReadData.commandQueue = (Global.device_type == Global.PTM_DEVICE) ? CommunicationHelperReadData.fillCommandACB_PTM() : CommunicationHelperReadData.fillCommandMCCB();
                    }
                    else
                    {
                        CommunicationHelperReadData.commandQueue = (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) ? CommunicationHelperReadData.fillCommandACB() : CommunicationHelperReadData.fillCommandMCCB();

                        if (Global.device_type_PXR10)
                        {
                            CommunicationHelperReadData.commandQueue = CommunicationHelperReadData.fillCommandPXR10MCCBWithTripUnit();
                            //  CommunicationHelper.commandQueue = new Queue<byte[]>(CommunicationHelper.commandQueue.Reverse());
                            //   CommunicationHelper.commandQueue.Dequeue();
                            //  CommunicationHelper.commandQueue = new Queue<byte[]>(CommunicationHelper.commandQueue.Reverse());

                        }
                    }
                    if (Global.device_type == Global.ACB_PXR35_DEVICE)
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

                    byteList.Clear();
                    bytListCheck = CommunicationHelperReadData.commandQueue.Count;
                    if (Global.isCommunicatingUsingBluetooth)
                    {

                        // Dispatcher.BeginInvoke(DispatcherPriority.Send, new UIDelegate(readSetpointInformation));
                        Thread.Sleep(1000);
                        var task = Task.Run(async () => await GetByteListBle());
                        task.Wait();
                    }
                    else
                    {
                        byteList = CommunicationHelperReadData.readTripUnitGroupsDataFromDevice();
                    }

                    processDataFromDevice();

                    if (!Global.IsOffline && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE) && (Global.GlbstrBreakerFrame == Resource.SYS02Item0015))
                    {
                        BeforeRefreshAuxStatus = Global.GlbstrAuxConnected;
                        AuxPowerStatus objAuxPower = new AuxPowerStatus();

                        bool isAuxStatusReadSucces = objAuxPower.writeAuxPowerStatusToGlobal();
                        Global.updateModbusAndRelaySetpoint();
                    }
                    bool isMisMatch = false;

                    if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                    {
                        setpointLines.RemoveAt(0);
                        isMisMatch = IsMismatchInDeviceAndUserSetpoints(setpointLines, TripUnit.tripUnitString) ? true : false;
                    }
                    else if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE)
                    {
                        if (!string.IsNullOrWhiteSpace(TripUnit.tripUnitString))
                        {
                            if (Global.OldOriginalsetpointLines == null)
                            {
                                Global.OldOriginalsetpointLines = new ArrayList();
                                int count = Global.device_type == Global.ACB_PXR35_DEVICE ? 12 : 7;
                                for (int i = 0; i <= count; i++)
                                {
                                    Global.OldOriginalsetpointLines.Add(string.Empty);
                                }
                                foreach (var setting in Global.ACBBackUpTripSetPoints)
                                {
                                    CommunicationHelperReadData.AppendToGroupString(Global.ACBBackUpTripSetPoints, setting.SetPointValue, setting.GroupId, ref Global.OldOriginalsetpointLines);
                                }

                                for (int i = Global.OldOriginalsetpointLines.Count - 1; i >= 0; i--)
                                {
                                    if (string.IsNullOrEmpty(Global.OldOriginalsetpointLines[i].ToString()))
                                    {
                                        Global.OldOriginalsetpointLines.RemoveAt(i);
                                    }
                                }
                                if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && 6 == Global.OldOriginalsetpointLines.Count)
                                {
                                    Global.OldOriginalsetpointLines.RemoveAt(0);
                                }
                            }

                            OriginalsetpointLines = Global.OldOriginalsetpointLines;
                            isMisMatch = IsMismatchInDeviceAndUserSetpoints(OriginalsetpointLines, TripUnit.tripUnitString) ? true : false;
                        }

                    }
                    else if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE)
                    {
                        Thread.Sleep(500);

                        if (!string.IsNullOrWhiteSpace(TripUnit.tripUnitString))
                        {
                            isMisMatch = IsMismatchInDeviceAndUserSetpoints_MCCB(mccbSetpoints, TripUnit.tripUnitString);
                            if (!BeforeRefreshAuxStatus.Equals(Global.GlbstrAuxConnected) && (Global.GlbstrBreakerFrame == Resource.SYS02Item0015 && !Global.IsOffline))
                            {
                                isMisMatch = true;
                            }
                        }
                    }

                    if (isMisMatch || Changes.Count > 0)
                    {
                        //   Added by Astha to check if MM swich is at valid position

                        if ((Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && string.Equals(TripUnit.ArmsModeData, Global.SwitchAtValidPosition) && (Global.GlbstrARMS == Resource.GEN003Item0001))
                        {
                           System.Windows.Application.Current.Dispatcher.Invoke(new UIDelegate(InvalidPositionOfMMRotarySwitch));
                            return;
                        }

                        Mouse.OverrideCursor = null;
                        Mouse.UpdateCursor();
                        Wizard_Screen_MsgBox msgBox = new Wizard_Screen_MsgBox(Resource.SetpointMismatch, Resource.ClickRefreshText, string.Empty, true);
                        msgBox.Width = 480;
                        msgBox.Height = 230;
                        msgBox.Topmost = true;
                        if (sender != null) msgBox.ShowDialog();
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                        Mouse.UpdateCursor();

                        if (true == msgBox.DialogResult || sender == null)
                        {
                            msgBox.Close();
                            //if (chng != "")
                            //{
                            //    BackupchangesToFile();
                            //}  
                            Global.readInData_ONLINE("Export");
                            ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                            ReloadDataFromTripUnit(sender);
                            Global.GeneralSetpointsDisable(!Global.IsOffline);
                            //    ReloadScreen();
                            SetOriginalSetpointLines();


                            //Redraw trip curves         
                            clearCurveData();
                            var set = ((Settings)((Group)(TripUnit.groups[1])).groupSetPoints[0]);
                            //if (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE)
                            //{
                            //set.addDataToCurveCollection -= DisplayCurve;
                            //set.addDataToCurveCollection += DisplayCurve;
                            //set.AddDataToCurve();
                            //DisplayCurve();
                            //   }
                            if (Global.show_curve)
                            {

                                if (Global.device_type == Global.NZMDEVICE)
                                {
                                    NZMCurveCalculations.AddNZMDataToCurve();
                                    NZMCurveCalculations.AddSCRNZMDataToCurve();
                                }
                                else
                                {
                                    CurvesCalculation.AddDataToCurve();
                                    CurvesCalculation.AddScrDataToCurve();
                                }

                                DisplayCurve();
                                DisplaySCRCurve();
                                if (Global.device_type != Global.NZMDEVICE)
                                {
                                    setScale();
                                }
                                Addcurves();

                            }

                            if (chng != "")
                            {
                                // Wizard_Screen_MsgBox msgBox1 = new Wizard_Screen_MsgBox("Refresh Setpoints", "Setpoints Refreshed Successfully", string.Empty, true);
                                Mouse.OverrideCursor = null;
                                Mouse.UpdateCursor();
                                Wizard_Screen_MsgBox msgBox1 = new Wizard_Screen_MsgBox(Resource.RefreshSetpoints, Resource.RefreshSuccessful, string.Empty, false);
                                msgBox1.Width = 480;
                                msgBox1.Height = 220;
                                if (4 >= count)
                                    msgBox1.Height = 250;
                                else if (8 >= count)
                                    msgBox1.Height = 350;
                                else if (8 < count)
                                {
                                    msgBox1.Height = 450;
                                    msgBox1.Width = 550;
                                }
                                msgBox1.Topmost = true;
                                if (sender != null) msgBox1.ShowDialog();
                                BtnRestoreDefaultIsEnabled = false; //disable undo changes after refresh as their will be no changes to undo after refresh
                                Global.OldData = null;
                                Global.OldOriginalsetpointLines = null;
                            }
                        }
                    }
                    else
                    {
                        //   Added by Astha to check if MM swich is at valid position
                        if (!(string.Equals(TripUnit.ArmsModeData, Global.SwitchAtValidPosition)) && (Global.GlbstrARMS == Resource.GEN003Item0001))
                        {
                            Mouse.OverrideCursor = null;
                            Mouse.UpdateCursor();
                            Wizard_Screen_MsgBox msgBox1 = new Wizard_Screen_MsgBox(Resource.SetpointMatch, Resource.ClickRefreshNochange, string.Empty, false);
                            msgBox1.Width = 480;
                            msgBox1.Height = 220;
                            msgBox1.Topmost = true;
                            if (sender != null) msgBox1.ShowDialog();
                        }
                        else if (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE
                            || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE)
                        {
                            Mouse.OverrideCursor = null;
                            Mouse.UpdateCursor();
                            Wizard_Screen_MsgBox msgBox1 = new Wizard_Screen_MsgBox(Resource.SetpointMatch, Resource.ClickRefreshNochange, string.Empty, false);
                            msgBox1.Width = 480;
                            msgBox1.Height = 220;
                            msgBox1.Topmost = true;
                            if (sender != null) msgBox1.ShowDialog();
                        }
                    }
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
                Mouse.UpdateCursor();
            }
        }
        private void ReloadDataFromTripUnit(object sender)
        {
            foreach (String settingID in TripUnit.IDTable.Keys)
            {
                var set = ((Settings)TripUnit.IDTable[settingID]);
                set.notifyDependents();

                set.SettingValueChange -= set_SettingValueChange;
                set.SettingValueChange += set_SettingValueChange;
                set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                set.CurveCalculationChanged += CurveCalculationApplyToChart;
            }

            Global.updateExpandersVisibility();
            if (sender != null)
            {
                Changes.Clear();
                //Need to recollect as found data for setpoint report when refresh is clicked
                Global.listGroupsAsFoundSetPoint.Clear();
                Global.export_Successful_ForSetpointReport = false;
                PopulateAsFoundData();
            }
        }
        private void readSetpointInformation()
        {
            var task = Task.Run(async () => await GetByteListBle());
            task.Wait();
        }
        private void SetOriginalSetpointLines()
        {
            MCCBBackUpTripSetPoints = new List<string>();
            Global.ACBBackUpTripSetPoints.Clear();

            TripParser.ConvertScreenInfoIntoSettingRequirements();
            OriginalsetpointLines = TripParser.GetConvertedSetPoints();
            if ((Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE) && Global.grouplist.Count > 0 && !Global.IsOffline)
            {
                foreach (var setting in Global.grouplist)
                {
                    MCCBBackUpTripSetPoints.Add(setting.SetPointValue);
                }
            }
            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && Global.grouplist.Count > 0 && !Global.IsOffline)
            {
                TripParser.ParseInputStringForConnect(TripUnit.tripUnitString, ' ');

                foreach (var setting in Global.grouplist)
                {
                    Global.ACBBackUpTripSetPoints.Add(new TripUnitMapper()
                    {
                        SetPointId = setting.SetPointId,
                        SetPointValue = setting.SetPointValue,
                        TripUnitPos = setting.TripUnitPos,
                        GroupId = setting.GroupId
                    });
                }
            }
        }
        public void CurveCalculationApplyToChart(object sender, EventArgs e)
        {

            DisplayCurve();
            DisplaySCRCurve();
            if (Global.device_type != Global.NZMDEVICE)
            {
                setScale();
            }

            Addcurves();

        }
        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.Button).ContextMenu.IsEnabled = true;
            (sender as System.Windows.Controls.Button).ContextMenu.PlacementTarget = (sender as System.Windows.Controls.Button);
            (sender as System.Windows.Controls.Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as System.Windows.Controls.Button).ContextMenu.IsOpen = true;

        }

        //private void mnuExport_Click(object sender, RoutedEventArgs e)
        //{
        //    Button_Click_Export(null, null);
        //}

        //private void mnuSaveAs_Click(object sender, RoutedEventArgs e)
        //{
        //    Button_Click_SaveToFile(null, null);
        //}

        private void btnToggleSidebar1(object sender)
        {
            try
            {

                if (origsidebarWid == 0)
                {
                    origsidebarWid = grdSidebarReference.ActualWidth;
                }
                if (origScrollWid == 0)
                {
                    origScrollWid = ScrollViewer_ContentPaneReference.ActualWidth; ;
                }


                Storyboard sidebarStoryBoard = new Storyboard();
                DoubleAnimation sidebarWidthAnimation = new DoubleAnimation();


                Storyboard scrollStoryBoard = new Storyboard();
                DoubleAnimation scrollAnimation = new DoubleAnimation();

                scrollAnimation.Completed += new EventHandler(myanim_Completed);

                if (BtnToggleSidebarIsChecked == false)
                {

                    sidebarWidthAnimation.From = 0;
                    sidebarWidthAnimation.To = origsidebarWid;
                    sidebarWidthAnimation.Duration = TimeSpan.FromSeconds(0.3);

                    Storyboard.SetTarget(sidebarWidthAnimation, grdSidebarReference);
                    Storyboard.SetTargetProperty(sidebarWidthAnimation, new PropertyPath(Grid.WidthProperty));
                    sidebarStoryBoard.Children.Add(sidebarWidthAnimation);
                    sidebarStoryBoard.Begin();


                    scrollAnimation.From = origScrollWid + origsidebarWid;
                    scrollAnimation.To = origScrollWid;
                    scrollAnimation.Duration = TimeSpan.FromSeconds(0.4);

                    Storyboard.SetTarget(scrollAnimation, _scrollViewerContentPaneReference);
                    Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(ScrollViewer.WidthProperty));
                    scrollStoryBoard.Children.Add(scrollAnimation);
                    scrollStoryBoard.Begin();

                    BtnToggleSidebarTooltip = Resource.sidebarToggleButtonHide;

                    Grid.SetColumnSpan(_scrollViewerContentPaneReference, 1);

                }
                if (BtnToggleSidebarIsChecked == true)
                {
                    sidebarWidthAnimation.From = origsidebarWid;
                    sidebarWidthAnimation.To = 1;
                    sidebarWidthAnimation.Duration = TimeSpan.FromSeconds(0.3);

                    Storyboard.SetTarget(sidebarWidthAnimation, grdSidebarReference);
                    Storyboard.SetTargetProperty(sidebarWidthAnimation, new PropertyPath(Grid.WidthProperty));
                    sidebarStoryBoard.Children.Add(sidebarWidthAnimation);




                    scrollAnimation.From = origScrollWid;
                    scrollAnimation.To = origScrollWid + origsidebarWid;
                    scrollAnimation.Duration = TimeSpan.FromSeconds(0.05);

                    Storyboard.SetTarget(scrollAnimation, ScrollViewer_ContentPaneReference);
                    Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath(ScrollViewer.WidthProperty));
                    scrollStoryBoard.Children.Add(scrollAnimation);


                    scrollStoryBoard.Begin();
                    sidebarStoryBoard.Begin();

                    BtnToggleSidebarTooltip = Resource.sidebarToggleButtonShow;



                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                //throw;
            }

        }

        private void myanim_Completed(object sender, EventArgs e)
        {
            if (BtnToggleSidebarIsChecked == true)
            {
                Grid.SetColumnSpan(_scrollViewerContentPaneReference, 2);
            }
            //else
            //{
            //    Grid.SetColumnSpan(ScrollViewer_ContentPane, 1);
            //}   
        }
        private void SetCurveChartsWidth()
        {
            LSChartReference.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.35;
            LSIrChartReference.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.35;
            INSTxInChartReference.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.35;
            MMxInChartReference.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.35;
            GIChartReference.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.35;
        }

        private void ButtonCurvesCoordination(object sender)
        {
            try
            {
                Window newWin = new Window();
                newWin.Content = new AddCurves(PXSet1Filepath, PXSet2Filepath, PXset1Filename, PXset2Filename, PXSet1Filechecked, PXSet2Filechecked);
                newWin.ShowActivated = true;
                newWin.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                newWin.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                newWin.ResizeMode = ResizeMode.NoResize;
                newWin.ShowDialog();
                if (loadcurves)
                {
                    if (MainScreen_ViewModel.Changes.Count == 0)
                    {
                        if (Global.device_type == Global.NZMDEVICE)
                        {
                            NZMCurveCalculations.AddNZMDataToCurve();
                            NZMCurveCalculations.AddSCRNZMDataToCurve();
                        }
                        else
                        {
                            CurvesCalculation.AddDataToCurve();
                            CurvesCalculation.AddScrDataToCurve();
                        }
                        if (Global.show_curve)
                        {
                            DisplayCurve();
                            DisplaySCRCurve();
                        }
                    }
                    else
                    {
                        if (Global.device_type == Global.NZMDEVICE)
                        {
                            NZMCurveCalculations.AddSCRNZMDataToCurve();
                        }
                        else
                        {
                            CurvesCalculation.AddScrDataToCurve();
                        }
                        if (Global.show_curve)
                        {
                            DisplayCurve();
                            DisplaySCRCurve();
                        }
                    }
                    if (Global.show_curve && Global.device_type != Global.NZMDEVICE)
                    {
                        setScale();
                    }
                    Addcurves();

                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }
        private void DataSeries_PlotElementLoaded(object sender, EventArgs e)
        {
            PlotElement pe = sender as PlotElement;
            if (pe != null)
            {
                var dataPoint = pe.DataPoint;
                pe.MouseMove += chart_MouseMove;//associate MouseMove event to the datapoints in the curves
            }
        }

        /// <summary>
        /// MouseMove Event handler to show co-ordinates on hovering to the curve data
        /// </summary>
        private void chart_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                C1Chart Chart = new C1Chart();
                if (TbCurvesSelectedIndex == 0)
                    Chart = LSChartReference;
                else if (TbCurvesSelectedIndex == 1)
                    Chart = LSIrChartReference;
                else if (TbCurvesSelectedIndex == 2)
                    Chart = INSTxInChartReference;
                else if (TbCurvesSelectedIndex == 3)
                    Chart = MMxInChartReference;
                else if (TbCurvesSelectedIndex == 4)
                    Chart = GIChartReference;

                string datapoint = string.Empty;
                if (Chart.Data.Children.Count != 0)
                {
                    if (sender is Lines)
                    {
                        var obj = (Lines)sender;
                        string dataValues = string.Empty;
                        string XYValues = string.Empty;
                        if (obj != null)
                        {
                            System.Windows.Point ptdata = Chart.View.PointToData(e.GetPosition(obj));
                            datapoint = "(" + Math.Round(Convert.ToDouble(ptdata.X), 2) + ", " + Math.Round(Convert.ToDouble(ptdata.Y), 2) + ")";
                            double distance = 0;

                            for (int i = 0; i < Chart.Data.Children.Count; i++)
                            {
                                int index = Chart.View.DataIndexFromPoint(ptdata, i, MeasureOption.X, out distance);
                                if (index != -1)
                                {
                                    ToolTipService.SetToolTip(Chart.Data.Children[i], datapoint);
                                    ToolTipService.SetShowDuration(Chart, 500);
                                    ToolTipService.SetBetweenShowDelay(Chart, 1);
                                }
                            }
                        }
                    }
                    else //for test plots, NZM trip curve(dotted Symbol)
                    {
                        var pe = (PlotElement)sender;
                        if (pe.DataPoint.PointIndex >= 0)
                        {
                            XYDataSeries.XYDataPoint ptdata = pe.DataPoint as XYDataSeries.XYDataPoint;
                            datapoint = "(" + Math.Round(Convert.ToDouble(ptdata.X), 2) + ", " + Math.Round(Convert.ToDouble(ptdata.Y), 2) + ")";
                            ToolTipService.SetToolTip(pe, datapoint);
                            ToolTipService.SetShowDuration(pe, 500);
                            ToolTipService.SetBetweenShowDelay(pe, 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }
        public void RbSet1234_Checked(object sender,RoutedEventArgs e)
        {
            try
            {
                string setpointSetTOBeSlected = string.Empty;
                // string selectedSet = ((RadioButton)sender).Text.ToString();
                string SelectedSetNameOnScreen = string.Empty;
                Mouse.OverrideCursor = null;
                Mouse.UpdateCursor();

                if (sender != null)
                {
                    if (((RadioButton)sender).Content.ToString() == Resource.SYS025Item0000 && (bool)((RadioButton)sender).IsChecked)
                    {
                        setpointSetTOBeSlected = "A";
                        SelectedSetNameOnScreen = Resource.SYS025Item0000;
                    }
                    if (((RadioButton)sender).Content.ToString() == Resource.SYS025Item0001 && (bool)((RadioButton)sender).IsChecked)
                    {
                        setpointSetTOBeSlected = "B";
                        SelectedSetNameOnScreen = Resource.SYS025Item0001;
                    }
                    if (((RadioButton)sender).Content.ToString() == Resource.SYS025Item0002 && (bool)((RadioButton)sender).IsChecked)
                    {
                        setpointSetTOBeSlected = "C";
                        SelectedSetNameOnScreen = Resource.SYS025Item0002;
                    }
                    if (((RadioButton)sender).Content.ToString() == Resource.SYS025Item0003 && (bool)((RadioButton)sender).IsChecked)
                    {
                        setpointSetTOBeSlected = "D";
                        SelectedSetNameOnScreen = Resource.SYS025Item0003;
                    }
                }
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Mouse.UpdateCursor();
                if (Changes.Count == 1 && (Changes.ElementAt(0).ItemName == Resource.SYS025Name))
                {
                    if (setpointSetTOBeSlected != string.Empty)
                        Global.PXR35_SelectedSetpointSet = setpointSetTOBeSlected;
                    LoadSelectedSetOnScreen();
                }
                else if (Changes.Count > 0)
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    Mouse.UpdateCursor();

                    string message = string.Format(Resource.LoadSetpointSetConfirmationMsg, SelectedSetNameOnScreen);
                    Wizard_Screen_MsgBox msgBox = new Wizard_Screen_MsgBox(Resource.LoadSetpointSet, message, string.Empty, true);
                    msgBox.Width = 480;
                    msgBox.Height = 230;
                    msgBox.Topmost = true;
                    if (sender != null) msgBox.ShowDialog();


                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Mouse.UpdateCursor();

                    if (true == msgBox.DialogResult || sender == null)
                    {
                        Global.PXR35_SelectedSetpointSet = setpointSetTOBeSlected;
                        LoadSelectedSetOnScreen();
                    }
                    else
                    {
                        switch (Global.PXR35_SelectedSetpointSet)
                        {
                            case "A":
                                RbSet1Reference.Checked -= RbSet1234_Checked;
                                RbSet1Reference.IsChecked = true;
                                RbSet1Reference.Checked += RbSet1234_Checked;
                                break;
                            case "B":
                                RbSet2Reference.Checked -= RbSet1234_Checked;
                                RbSet2Reference.IsChecked = true;
                                RbSet2Reference.Checked += RbSet1234_Checked;
                                break;
                            case "C":
                                RbSet3Reference.Checked -= RbSet1234_Checked;
                                RbSet3Reference.IsChecked = true;
                                RbSet3Reference.Checked += RbSet1234_Checked;
                                break;
                            case "D":
                                RbSet4Reference.Checked -= RbSet1234_Checked;
                                RbSet4Reference.IsChecked = true;
                                RbSet4Reference.Checked += RbSet1234_Checked;
                                break;
                        }

                    }
                }
                else
                {
                    if (setpointSetTOBeSlected != string.Empty)
                        Global.PXR35_SelectedSetpointSet = setpointSetTOBeSlected;
                    LoadSelectedSetOnScreen();
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                Mouse.UpdateCursor();
            }
        }

        public void LoadSelectedSetOnScreen()
        {
            // Global.PXR35_SelectedSetpointSet
            if (Global.IsOffline) return;

            try
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


                if (byteList != null) byteList.Clear();
                bytListCheck = CommunicationHelperReadData.commandQueue.Count;
                if (Global.isCommunicatingUsingBluetooth)
                {
                    var task = Task.Run(async () => await GetByteListBle());
                    task.Wait();
                }
                else
                {
                    byteList = CommunicationHelperReadData.readTripUnitGroupsDataFromDevice();
                }
                processDataFromDevice();


                Global.readInData_ONLINE("Export");

                ScreenCreator.ShowScreenContent(ref _scrollViewerContentPaneReference);
                clearCurveData();

                foreach (String settingID in TripUnit.IDTable.Keys)
                {
                    var set = ((Settings)TripUnit.IDTable[settingID]);
                    set.notifyDependents();
                    set.SettingValueChange -= set_SettingValueChange;
                    set.SettingValueChange += set_SettingValueChange;
                    set.CurveCalculationChanged -= CurveCalculationApplyToChart;
                    set.CurveCalculationChanged += CurveCalculationApplyToChart;

                }

                CurvesCalculation.AddDataToCurve();
                CurvesCalculation.AddScrDataToCurve();

                if (Global.show_curve)
                {
                    DisplayCurve();
                    DisplaySCRCurve();
                    if (Global.device_type != Global.NZMDEVICE)
                    {
                        setScale();
                    }
                }

                SetOriginalSetpointLines();

                Global.GeneralSetpointsDisable(!Global.IsOffline);

                if (Changes != null)
                {
                    Changes.Clear();
                }

                if (Global.listGroupsAsFoundSetPoint != null)
                {
                    Global.listGroupsAsFoundSetPoint.Clear();
                }
                PopulateAsFoundData();
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
        }
    }
    public class CurveData
    {
        public bool GF_Enabled { get; set; }
        public bool isGFActionoff { get; set; }
        public bool IN_Enabled { get; set; }
        public double GFXMax { get; set; }
        public double GFXMin { get; set; }
        public double GFYMax { get; set; }
        public double GFYMin { get; set; }
        public decimal GFPU_ToSetAxis { get; set; }
        public decimal In_ToSetAxis { get; set; }
        public bool IsMMStateOn { get; set; }
        public bool MM_Enabled { get; set; }
        public double MMXMax { get; set; }
        public double MMXmin { get; set; }
        public double InstXMax { get; set; }
        public double InstXmin { get; set; }
        public double LSIAxMax { get; set; }
        public double LSIAXMin { get; set; }
        public double LSIrXMax { get; set; }
        public double LSIrXmin { get; set; }
    }
}

