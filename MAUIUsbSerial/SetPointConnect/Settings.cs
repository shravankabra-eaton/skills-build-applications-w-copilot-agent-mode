using System;
using System.Linq;

// Hashtables
using System.Collections;
// Window form elements
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using PXR.Screens;
using System.Xml;
using PXR.Resources.Strings;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using C1.WPF;
using PXR.Screens.RelayConfig;

namespace PXR
{
    /// <summary>
    /// Filename: Settings.cs
    /// Created:
    /// Author: Sarah M. Norris
    /// Description:    Settings.cs contains all of the information about the individual setpoints as
    ///                 described in the model xml file. It also updates the dependencies, UI, and values
    ///                 of the settings as the user changes them in the UI. 
    /// Modifications:  
    ///     4/3/13      Added range checking logic to the class. And therefore had to add a way to 
    ///                 connect the UI elements to their respective settings. 
    ///                 
    ///     4/3/13      Behavior for the bNumber is taken care of in the checkbox function
    /// 
    /// </summary>
    public class Settings
    {
        public Settings ShallowCopy()
        {
            return (Settings)this.MemberwiseClone();
        }

        // Split settings will have 2 settings mashed together
        public Settings[] setpoint;

        public String selectionFrameValue;                           // Value the comboBox currently holds
        public String ID;       // Const. This value is set in the XML file and is used to look up the 
        // setting from the ID table
        public String GroupID;  // Group that this setting is a part of used for looking up the setting. 
        public int index;       // index of setting in group
        public String name;     // Const. Set in the XML file. This is shown on the form and pdf file. 
        public String OnLabel;
        public String OffLabel;
        public int subgrp_index = -1;
        public int subgrp_setpoint_index = -1;
        public int grpsetpoint_index = -1;
        public int subgrp_sub_index = -1;
        public Boolean visible;     // Determines if the element is shown on the form
        public Boolean readOnly;    // Determines if the user can edit the value
        public Boolean isEnabled;
        public Boolean notAvailable;
        public Boolean bnumbervisible;
        public Boolean onlineReadOnly = false;
        public Boolean showvalueInBothModes = false;
        public bool parseInPXPM;
        public bool parseForACB_2_1_XX;
        public bool isLooping = false;
        System.Windows.Threading.DispatcherTimer dispatcherIrTimer = new System.Windows.Threading.DispatcherTimer();

        public String[] listofDependenciesID;//Added by Astha to store id of setpoint values mentioned in Single dependency tag
  
        public List<Settings> listofdependenciesSetpoint = new List<Settings>();//Added by Astha to store setpoint values mentioned in Single dependency tag

        
        public enum Type
        {
            type_number,
            type_bNumber,
            type_bool,
            type_selection,
            type_skip,
            type_rPlugStyle,
            type_listBox,
            type_split,
            type_text,
            type_none,
            type_bSelection,    // by Ashish for binary Selection
            type_IPAddress,
            type_toggle// by Ashish for IPAddress control
        };


        public string relayOriginalValue = "00 00 00 00 00 00 00 00";
        public string relayChosenValue = "00 00 00 00 00 00 00 00";

        public string relayOriginalValuebackup = "00 00 00 00 00 00 00 00";

        public Type type = Type.type_none;      // this value is used to determine which of the many setting 
        // attributes are used for this unique setting. 
        // boolean
        public bool bValue;                     // Current value of the check box. 
        public bool bDefault;                   // Stores the default value for a boolean for resetting to default
        public bool ? bDefaultAfterDependencyApplied=null;
        public bool bValueReadFromTripUnit;
        public bool commitedChange = false;     //to check weather the setpoint is exported to device or not

        //bselection
        public String bselectionValue;                           // Value the comboBox currently holds
        public String selectionDefault;                    // Used for resetting the comboBox back to its original value.
        public String selectionDefaultAfterDependencyApplied;
        public String grouphead;                // for storing the head of the subgoups

        // number
        public double numberValue = -1;         // Stores the current value of the number and is used to validate setting
        public double numberDefault = -1;       // Stores the default value for resetting to default
        public double conversion = 1;           // Multiplied by the raw value in order to convert value to the correct value
        public string conversionOperation = "*";
        public string MinCalculation = "";
        public string MaxCalculation = "";
        public string ExcludedValue = "";
        public double min = -1;                 // Minimum value allowed for value to be valid
        public double max = -1;                 // Maximum value allowed for value to be valid
        public double stepsize = -1;            // Value can only be increments of this step size. Values are rounded down. 
        public String unit;                     // Unit is used for display purposes only. 
        public string strStepSize = "";         // save stepsize as string variable to reatain format. Latet use this format to display value with correct digits after decimal
        // selection
        public Hashtable lookupTable = new Hashtable();         // Options for comboBox and is used to translate values.
        public Hashtable ItemsToDisplayfromLookupTable = new Hashtable();
        public Hashtable reverseLookupTable = new Hashtable();  // Translates the selected value into a hex value.
        public Dictionary<string,string> indexesWithHexValuesMapping = new Dictionary<string,string>() ;
        public Hashtable value_map = new Hashtable();           // to map the value with index.
        public Hashtable reversevalue_map = new Hashtable();    // translate selected hex value to index value

        // selection for H and L polarity for MCCB group 4 setpoint  by Ashish
        public Hashtable H_value_map = new Hashtable();           // to map the value with index.
        public Hashtable H_reversevalue_map = new Hashtable();    // translate selected hex value to index value
        public Hashtable L_value_map = new Hashtable();           // to map the value with index.
        public Hashtable L_reversevalue_map = new Hashtable();    // translate selected hex value to index value
        public int selectionIndex;
        public String selectionValue;                          // Value the comboBox currently holds
        public List<string> listofItemsToDisplay;
        public String defaultSelectionValue;                    // Used for resetting the comboBox back to its original value. 
        // list box
        public ArrayList itemList = new ArrayList();            // Item list for list box- stores listBoxItems
        public String rawValue;                                 // Raw hex value. 
                                                                // text box
        public double textvalue;                                //Added by Astha for textbox value
        public string textStrValue = string.Empty;
        public string defaulttextStrValue = string.Empty;
        public double defaultextvalue;
        public string txtUnit;
        public string IPaddress_default;
        public string IPaddress;                                //Added by Ashish for IP control

        // Calculated value
        public Boolean bcalculated;
        public string calculation;



        // Dependents
        public ArrayList dependents = new ArrayList();          // Stores a list of settings that are dependent on this setting
        // Dependencies
        public Hashtable singleDependencies = new Hashtable();  // Stores a list of dependencies that affect this setting 
        public Hashtable multiDependencies = new Hashtable();   // Single means only one value of a setting affects this one
        // while multi means a combination of different settings affect this           
        // this setting. This requires special helper functions found in 
        // MultiDependency.cs
        public String description;                              // Brief description that helps user identify the setting. This is shown in the tool tip
        public XmlNodeList nodesForSingledependencies;
        public delegate void settingValueChangeHandler(string propName, string OrigVal, string newVal, string ControlName, bool isVisible, string GroupId);
        public event settingValueChangeHandler SettingValueChange;
        public event EventHandler CurveCalculationChanged;

        public delegate void FrameAndLCDOrienatationHandler();
        public event FrameAndLCDOrienatationHandler lcdDataToCurveCollection;
    
        public delegate void AddDataToCurveHandler();
     //   public event AddDataToCurveHandler addDataToCurveCollection;
        public CultureInfo userCulture = Thread.CurrentThread.CurrentUICulture;
        public NumberFormatInfo nFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
        public int TripUnitSequence { get; set; }
        /// <summary>
        /// The default delay before first interval change
        /// </summary>
        public int FirstDelay = 1000;
        int incrementValue = 1, reduceTimer = 0;
        /// <summary>
        /// The interval delay for reducing interval time
        /// </summary>
        public int ReduceTimerDelay = 150;
        /// <summary>
        /// The interval delay after threshold limit
        /// </summary>
        public int HighSpeedTimerDelay = 30;
        //ToolTip newTooltip; // Used to display tooltip at the time of gotFocus while using TAB to navigate

        /// <summary>
        /// Notifies dependents that the setting value has changed. The dependents then 
        /// update their value based on the new value. 
        /// </summary>
        public void notifyDependents()
        {
         
            foreach (String dependentID in dependents)
            {                
                Settings dependent = (Settings)TripUnit.IDTable[dependentID];
                // if their are typos in the xml data file we don't want them crashing the program. 
                if (dependent != null)
                  {
                    foreach (var item in dependent.singleDependencies.Keys)
                    {
                        //for IO subgroups
                        //String[] listOfIDs;
                        //listOfIDs = (item.ToString()).Split(',');

                        //if (listOfIDs.Contains(ID) || listOfIDs.Contains(ID + "_1") || listOfIDs.Contains(ID + "_2"))
                        //{
                           // dependent.dependencyUpdate(item.ToString(), ref dependent);
                            dependent.dependencyUpdate(item.ToString(), dependent);
                      // }
                    }
                    
                }
            }
          
        }


        /// <summary>
        ///Added by Astha
        ///Called by a setting dependency. This function updates the setting value based
        /// on the new dependency value. 
        /// </summary>
        /// <param name="_ID"></param>
        private void dependencyUpdate(String _ID, Settings dependent)
        {
            int position = 0;
            string setpointId;
            Hashtable listOfSettings = new Hashtable();

            SingleDependency sd = (SingleDependency)singleDependencies[_ID];
            // Added this in case XML is wrong. xml should only list the first
            // level dependencies. For example VT000 affects VT001 and VT002
            // but since VT001 also controls VT002 we simply add it to VT001
            // dependents and remove it from VT000
            if (sd != null)
            {
                int groupID = Int32.Parse(GroupID);
                listofDependenciesID = _ID.Split(',').ToArray<String>();
                for (int i = 0; i < listofDependenciesID.Length; i++)
                {
                    if (listofDependenciesID[i].EndsWith("_1") || listofDependenciesID[i].EndsWith("_2"))
                    {
                        position = listofDependenciesID[i].IndexOf("_");
                        setpointId = listofDependenciesID[i].Substring(0, position);
                        listofdependenciesSetpoint.Add((Settings)TripUnit.IDTable[setpointId]);
                        listOfSettings.Add((Settings)TripUnit.IDTable[setpointId], listofDependenciesID[i]);
                    }
                    else
                    {
                        listofdependenciesSetpoint.Add((Settings)TripUnit.IDTable[listofDependenciesID[i]]);
                        listOfSettings.Add((Settings)TripUnit.IDTable[listofDependenciesID[i]], listofDependenciesID[i]);
                    }
                }

                if (dependent.index != -1 && dependent.grpsetpoint_index == -1 && dependent.subgrp_index == -1 && dependent.subgrp_setpoint_index == -1)
                {
                    sd.ApplyDependency(ref ((Group)TripUnit.groups[groupID]).groupSetPoints[index],
                                        listofdependenciesSetpoint, listOfSettings);                  
                    listofdependenciesSetpoint.Clear();
                    listOfSettings.Clear();
                    uiUpdate(ref ((Group)TripUnit.groups[groupID]).groupSetPoints[index]);
                    


                }
                else if (dependent.index != -1 && dependent.grpsetpoint_index != -1 && dependent.subgrp_index == -1 && dependent.subgrp_setpoint_index == -1)
                {
                    sd.ApplyDependency(ref ((Group)TripUnit.groups[groupID]).groupSetPoints[grpsetpoint_index],
                                                          listofdependenciesSetpoint, listOfSettings);                    
                    listofdependenciesSetpoint.Clear();
                    listOfSettings.Clear();
                    uiUpdate(ref ((Group)TripUnit.groups[groupID]).groupSetPoints[grpsetpoint_index]);
                }
                else if (dependent.index != -1 && dependent.subgrp_index != -1 && dependent.subgrp_setpoint_index != -1 && dependent.grpsetpoint_index == -1 && subgrp_sub_index == -1)
                {
                    sd.ApplyDependency(ref ((Group)TripUnit.groups[groupID]).subgroups[subgrp_index].groupSetPoints[subgrp_setpoint_index],
                                       listofdependenciesSetpoint, listOfSettings);                   
                    listofdependenciesSetpoint.Clear();
                    listOfSettings.Clear();
                    uiUpdate(ref ((Group)TripUnit.groups[groupID]).subgroups[subgrp_index].groupSetPoints[subgrp_setpoint_index]);

                }
                else if (dependent.index != -1 && dependent.subgrp_index != -1 && dependent.subgrp_setpoint_index != -1 && dependent.grpsetpoint_index == -1 && subgrp_sub_index != -1)
                {
                    sd.ApplyDependency(ref ((Group)TripUnit.groups[groupID]).subgroups[subgrp_index].subgroups[subgrp_sub_index].groupSetPoints[subgrp_setpoint_index],
                                       listofdependenciesSetpoint, listOfSettings);                    
                    listofdependenciesSetpoint.Clear();
                    listOfSettings.Clear();
                    uiUpdate(ref ((Group)TripUnit.groups[groupID]).subgroups[subgrp_index].subgroups[subgrp_sub_index].groupSetPoints[subgrp_setpoint_index]);
                }
           
            }
        }
        /// <summary>
        /// Update UI to express the change of the dependency. This applies only to readonly and visibility. 
        /// </summary>
        public void uiUpdate(ref Settings setting)
        {
            try
            {
                if (setting.visible && label_notavailable != null)
                {
                    if (setting.notAvailable)
                    {
                        setting.label_name.Visibility = Visibility.Visible;
                        setting.label_notavailable.Visibility = Visibility.Visible;
                        setting.label_notavailable.Content = Resource.NotAvailable;

                        if (setting.label_calculation != null)
                        {
                            setting.label_calculation.Visibility = Visibility.Collapsed;
                        }
                        if (setting.emptyspace != null)
                        {
                            setting.emptyspace.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        setting.label_name.Visibility = Visibility.Visible;
                        setting.label_notavailable.Visibility = Visibility.Collapsed;
                        if (setting.label_calculation != null)
                        {
                            label_calculation.Visibility = Visibility.Visible;
                        }
                        if (emptyspace != null)
                        {
                            setting.emptyspace.Visibility = Visibility.Visible;
                        }
                    }
                }


                if (setting.type == Type.type_number)
                {
                    upDateTextBox(!setting.readOnly);

                }
                else if (setting.type == Type.type_bNumber)
                {
                    //if (setting.bValue == false)
                    //{
                    if (setting.bnumbervisible == true && setting.textBox != null
                        && setting.increaseButton != null && setting.decreaseButton != null)
                    {

                        setting.textBox.IsEnabled = true;
                        setting.increaseButton.IsEnabled = true;
                        setting.decreaseButton.IsEnabled = true;
                        // setting.readOnly = false;
                    }
                    if (setting.toggle != null)
                    {
                        if (setting.notAvailable)
                        {

                            setting.toggle.Visibility = Visibility.Collapsed;
                            if (setting.leftlabel_forToggle != null) setting.leftlabel_forToggle.Visibility = Visibility.Collapsed;
                            if (setting.rightlabel_forToggle != null) setting.rightlabel_forToggle.Visibility = Visibility.Collapsed;
                            //    setting.textBox.IsEnabled = false;
                            //    setting.increaseButton.IsEnabled = false;
                            //    setting.decreaseButton.IsEnabled = false;
                            //    //setting.readOnly = true;
                            //}
                        }
                        else
                        {
                            setting.toggle.Visibility = Visibility.Visible;
                        }
                    }
                    upDateTextBox(!setting.readOnly);
                }
                else if (setting.type == Type.type_bool)
                {
                    upDateCheckBox();
                }
                else if (setting.type == Type.type_selection)
                {
                    updateComboBox(ref setting);
                }
                else if (setting.type == Type.type_toggle)
                {
                    if (setting.toggle != null)
                    {
                        if (setting.notAvailable)
                        {

                            setting.toggle.Visibility = Visibility.Collapsed;
                            setting.leftlabel_forToggle.Visibility = Visibility.Collapsed;
                            setting.rightlabel_forToggle.Visibility = Visibility.Collapsed;
                            //    setting.textBox.IsEnabled = false;
                            //    setting.increaseButton.IsEnabled = false;
                            //    setting.decreaseButton.IsEnabled = false;
                            //    //setting.readOnly = true;
                            //}
                        }
                        else
                        {
                            setting.toggle.Visibility = Visibility.Visible;
                            setting.leftlabel_forToggle.Visibility = Visibility.Visible;
                            setting.rightlabel_forToggle.Visibility = Visibility.Visible;
                        }
                        UpdateToggle(ref setting);

                    }
                }
                else if (setting.type == Type.type_listBox)
                {
                    UpdateListBox();
                }
                else if (setting.type == Type.type_text)
                {
                    upDateTextBoxTxt(!setting.readOnly);
                }
                else if (setting.type == Type.type_bSelection)
                {
                    updateComboBox(ref setting);
                }

                Grid _GridParent = null;
                if (label_notavailable != null)
                {
                    _GridParent = Global.FindParent_Expander<Grid>(label_notavailable);
                }

                if (_GridParent != null)
                {
                    if (Global.IsOffline && Global.device_type == Global.NZMDEVICE &&
                          (setting.ID == "CC05" || setting.ID == "CC06"
                          || setting.ID == "CCC05A" || setting.ID == "CCC06A"))
                    {
                        _GridParent.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _GridParent.Visibility = setting.visible ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (setting.ID == "CPC03" && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
                    {
                        bool isMotorProtEnabled = TripUnit.getMotorProtectionGeneralGrp().bValue;

                        //Added null checks while getting every parent elemnt to fix coverity scan issue
                        if (((System.Windows.FrameworkElement)_GridParent.Parent) != null)
                        {
                            FrameworkElement gridParentElement1 = (System.Windows.FrameworkElement)_GridParent.Parent;
                            if (gridParentElement1 != null && ((System.Windows.FrameworkElement)(gridParentElement1).Parent) != null)
                            {
                                FrameworkElement gridParentElement2 = (System.Windows.FrameworkElement)(gridParentElement1).Parent;
                                if (gridParentElement2 != null)
                                {
                                    Expander exp = (Expander)(gridParentElement2).Parent;
                                    if (exp != null)//Added null for fixing coverity scan issue
                                    {
                                        exp.Header = isMotorProtEnabled ? Resource.MPEnabledSubGroupName : Resource.MPDisabledSubGroupName;
                                    }
                                }
                            }
                        }
                    }
                    //if (setting.ID == "MPC00" && Global.device_type == Global.NZMDEVICE)
                    //{
                    //    bool isMotorProtEnabled = TripUnit.getMotorProtectionGeneralGrp().bValue;

                    //    //Added null checks while getting every parent elemnt to fix coverity scan issue
                    //    if (((System.Windows.FrameworkElement)_GridParent.Parent) != null)
                    //    {
                    //        FrameworkElement gridParentElement1 = (System.Windows.FrameworkElement)_GridParent.Parent;

                    //        if (gridParentElement1 != null && ((System.Windows.FrameworkElement)(gridParentElement1).Parent) != null)
                    //        {
                    //            FrameworkElement gridParentElement2 = (System.Windows.FrameworkElement)(gridParentElement1).Parent;

                    //            if (gridParentElement2 != null && ((System.Windows.FrameworkElement)(gridParentElement2).Parent) != null)
                    //            {
                    //                FrameworkElement gridParentElement3 = (System.Windows.FrameworkElement)(gridParentElement2).Parent;
                    //                if (gridParentElement3 != null)
                    //                {
                    //                    // Expander exp = (Expander)(gridParentElement3).Parent;
                    //                    //if (exp != null)//Added null for fixing coverity scan issue
                    //                    //{
                    //                   // ((Expander)((System.Windows.FrameworkElement)((System.Windows.FrameworkElement)((((System.Windows.FrameworkElement)(gridParentElement3).Parent)).Parent)).Parent).Parent).Header = isMotorProtEnabled ? "Motor enabled " : "Motor disabled ";
                    //                    //exp.Header = isMotorProtEnabled ? "Motor enabled " : "Motor disabled ";
                    //                    //}
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateToggle(ref Settings setting)
        {
            if (setting.toggle == null)
                return;
            setting.toggle.IsChecked = bValue;
            setting.toggle.IsEnabled = !readOnly;
            if (visible)
            {
                setting.toggle.Visibility = Visibility.Visible;
            }
            else
            {
                setting.toggle.Visibility = Visibility.Collapsed;
            }

            if (setting.label_name != null)
            {
                setting.label_name.Content = setting.name;
            }

            if (setting.description != null && setting.description.Trim() != "")
            {
               

                    ToolTip toolTip = ScreenCreator.createToolTip(setting.description);
                    setting.toggle.ToolTip = toolTip;
                    //  setting.toglebutton.ToolTip = toolTip;
                    ToolTipService.SetToolTip(toolTip, "");
                    ToolTipService.SetShowDuration(setting.toggle, 300000);
               
            }
            else
            {
                setting.toggle.ClearValue(RadioButton.ToolTipProperty);
            }

        }

        private void upDateTextBoxTxt(Boolean _isEnabled)
        {
            try
            {
                if (textBox == null && IPAddressControl == null)
                {
                    return;
                }
                else if (textBox != null)
                {
                    textBox.IsEnabled = _isEnabled;
                }
                else if (IPAddressControl != null)
                {
                    IPAddressControl.IsEnabled = _isEnabled;
                }
                if (_isEnabled == false && !notAvailable && IPAddressControl == null)
                {
                    /*Added by Astha
                    Following lines of code is added to handle null exception which occurs in offline mode when Tag value is null*/
                    if (textBox != null)
                    {
                        if (textBox.Tag == null)
                        {
                            textBox.Text = "";
                        }
                        else if ((string[])(textBox.Tag) != null)
                        {
                            string[] getTagInfoasStringOfText = (string[])(textBox.Tag);
                            //Added null for fixing coverity scan issue
                            if (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0)
                            {
                                string defaultValueofText = getTagInfoasStringOfText[0];
                                textBox.Text = Global.updateValueonCultureBasis(defaultValueofText);
                            }
                        }
                    }
                }
                bool isError = false;
                ToolTip toolTip;

                Grid _GridParent = null;
                if (label_notavailable != null)
                {
                    _GridParent = Global.FindParent_Expander<Grid>(label_notavailable);
                }

                if (visible && _GridParent != null)
                {
                    _GridParent.Visibility = Visibility.Visible;
                    label_name.Visibility = Visibility.Visible;
                }
                else if (_GridParent != null)
                {
                    _GridParent.Visibility = Visibility.Collapsed;
                    label_name.Visibility = Visibility.Collapsed;
                }

                if (textBox != null)
                {

                    if ((textBox.Text.Trim() == "") && (!Global.IsOffline))
                    {
                        isError = true;
                        description = Resource.StepSizeNotInRangeError;
                    }

                    if (isError == true)//&& (textBox.IsEnabled == true))
                    {
                        valid = false;
                        textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle_text");
                        toolTip = ScreenCreator.createToolTip(description);
                        textBox.ToolTip = toolTip;
                        toolTip.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
                    }
                    else
                    {
                        valid = true;
                        textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle_text");
                        toolTip = ScreenCreator.createToolTip(description);
                        textBox.ToolTip = toolTip;
                    }
                    ToolTipService.SetToolTip(toolTip, "");
                    ToolTipService.SetShowDuration(textBox, 300000);
                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        public static void SetCultureNumberFormat()
        {
            // NumberFormatInfo nFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;      //#COVARITY FIX   235047
            CultureInfo userCulture = Thread.CurrentThread.CurrentUICulture;
            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);

            NumberFormatInfo nFormat = (NumberFormatInfo)CultureInfo.CurrentUICulture.NumberFormat.Clone();
            if (CultureInfo.CurrentUICulture.Name == "en-US" || CultureInfo.CurrentUICulture.Name == "zh-CHS")
            {
                nFormat.NumberDecimalSeparator = ".";
                userCulture.NumberFormat.NumberDecimalSeparator = ".";
                CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ".";
            }
            else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES"
                     || CultureInfo.CurrentUICulture.Name == "pl-PL" || CultureInfo.CurrentUICulture.Name == "fr-CA")
            {
                nFormat.NumberDecimalSeparator = ",";
                userCulture.NumberFormat.NumberDecimalSeparator = ",";
                CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ",";
            }
        }

        private void upDateTextBox(Boolean _isEnabled)
        {
            SetCultureNumberFormat();
            if (label_name != null)
            {
                label_name.Content = name;
            }
            if (textBox == null)
            {
                return;
            }

            //Added by SRK to fix PXPM-9731
            if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && GroupID == "01" &&
                (subgrp_index == 5 || subgrp_index == 6 || subgrp_index == 7 || subgrp_index == 8 ||
                subgrp_index == 9 || subgrp_index == 10 || subgrp_index == 11 || subgrp_index == 12 || subgrp_index == 13 || subgrp_index == 14 || subgrp_index == 15 || subgrp_index == 16))
            {
                string[] tagInfo = new string[3];
                textBox.Name = "txt_" + ID;
                tagInfo[0] = numberDefault.ToString();
                //stores original value of control
                tagInfo[1] = label_name.Content.ToString().Trim(); // stores the label to be displayed in summary
                tagInfo[2] = textBox.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow
                textBox.Tag = tagInfo;
            }

            if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && (Global.IsOpenFile || Global.IsOffline) && ID == "CPC010")
            {
                string[] tagInfo = new string[3];
                textBox.Name = "txt_" + ID;
                if (ID == "CPC010" && numberValue == -1)
                {
                    numberValue = numberDefault = 95;
                }
                tagInfo[0] = numberDefault.ToString();
                //stores original value of control
                tagInfo[1] = label_name.Content.ToString().Trim(); // stores the label to be displayed in summary
                tagInfo[2] = textBox.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow
                textBox.Tag = tagInfo;
            }


            if (textBox.Tag != null)
            {
                string[] getTagInfoasString = (string[])textBox.Tag;
                //Added null for fixing coverity scan issue
                if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                {
                    string[] tagInfo = new string[3];
                    tagInfo[0] = getTagInfoasString[0];

                    //For Motor protection group appending its subgroup name before its setpoint name - For differentiating in Change summary 
                    //eg. Over Voltage - Pickup
                    string labelText = label_name == null ? string.Empty : label_name.Content.ToString().Trim();
                    if (GroupID == "04" || GroupID == "004")
                    {
                        tagInfo[1] = ((Group)(TripUnit.groups[4])).subgroups[subgrp_index].name + " - " + labelText;
                    }                 
                    else if ((Global.device_type == Global.PTM_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && GroupID == "01" &&
                        (subgrp_index == 5 || subgrp_index == 6 || subgrp_index == 7 || subgrp_index == 8 ||
                        subgrp_index == 9 || subgrp_index == 10 || subgrp_index == 11 || subgrp_index == 12 || subgrp_index == 13 || subgrp_index == 14 || subgrp_index == 15 || subgrp_index == 16))
                    {
                        tagInfo[1] = ((Group)(TripUnit.groups[1])).subgroups[subgrp_index].name + " - " + labelText;
                    }
                    else
                    {
                        tagInfo[1] = labelText; // stores the label to be displayed in summary
                    }

                    //tagInfo[1] = label_name == null ? string.Empty : label_name.Content.ToString().Trim();
                    tagInfo[2] = getTagInfoasString[2];
                    textBox.Tag = tagInfo;
                }
            }

             Grid _GridParent = null;
            Grid _GridParentForIPCalculation = null;
           
            if (label_notavailable != null)
            {
                _GridParent = Global.FindParent_Expander<Grid>(label_notavailable);
            }

            if (visible && _GridParent != null)
            {
                _GridParent.Visibility = Visibility.Visible;
                //Added null for fixing coverity scan issue
                if (label_name != null) label_name.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Visible;
                label_Unit.Visibility = Visibility.Visible;            
            }
            else if (_GridParent != null)
            {               
                _GridParent.Visibility = Visibility.Collapsed;
                //Added null for fixing coverity scan issue
                if (label_name != null) label_name.Visibility = Visibility.Collapsed;
                label_Unit.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Collapsed;

            }

            //============================
            if (label_calculation != null && !notAvailable && _GridParent != null)
            {
                _GridParentForIPCalculation = Global.FindParent_Expander<Grid>(label_calculation);
                if (readOnly && _GridParentForIPCalculation != null)
                {
                    label_calculation.Visibility = Visibility.Collapsed;
                    if (!_GridParentForIPCalculation.Equals(_GridParent))
                    {
                        _GridParentForIPCalculation.Visibility = Visibility.Collapsed;
                    }

                }
                else if (!readOnly  && _GridParentForIPCalculation != null)
                {
                    
                    label_calculation.Visibility = Visibility.Visible;
                    if (!_GridParentForIPCalculation.Equals(_GridParent))
                    {
                        if (visible)
                        {
                            _GridParentForIPCalculation.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _GridParentForIPCalculation.Visibility = Visibility.Collapsed;
                        }
                    }
                    
                }
            }
            if (emptyspace != null && !notAvailable)
            {
                if (readOnly)
                {
                    emptyspace.Visibility = Visibility.Collapsed;
                }
                else if (!readOnly)
                {
                    emptyspace.Visibility = Visibility.Visible;
                }
            }
            //============================


            textBox.IsEnabled = _isEnabled;
            if (increaseButton != null)
            {
                increaseButton.IsEnabled = _isEnabled;
                decreaseButton.IsEnabled = _isEnabled;
            }

            if (label_notavailable != null && notAvailable)
            {
                label_notavailable.Visibility = Visibility.Visible;

                //Need to remove device and id condition after 21.09.1 release.
                if (min == max && min != numberValue && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && (ID == "CPC021" || ID == "CPC010" || ID == "CPC014A" || ID == "CPC0101" || ID == "CPC024"))
                {
                    numberValue = min;
                    valid = true;
                    textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
                }
                if (!valid && (numberValue <= min || numberValue >= max))
                {
                    numberValue = numberDefault;
                    valid = true;
                    textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
                }

                //Setting value to Long Delay Time (tr) i.e. CPC051 when control is not available (Long Delay Protection is set to Off). 
                //Require to correctly export setpoint
                if (textBox.Name == "txt_CPC051")
                {

                    numberValue = numberDefault;
                    valid = true;
                    textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
                    textBox.Text = numberValue.ToString();
                }

                if (type == Type.type_bNumber)
                {
                    toggle.IsEnabled = !notAvailable;
                }
                return;
            }
            else if (label_notavailable != null)
            {
                label_notavailable.Visibility = Visibility.Collapsed;
                if (type == Type.type_bNumber)
                {
                    toggle.IsEnabled = !notAvailable;
                }
            }
           if (!notAvailable)
            {
                //Need to remove device and id condition after 21.09.1 release.
                if (min == max && min != numberValue)
                {
                    numberValue = min;
                    valid = true;
                    textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
                    textBox.Text = numberValue.ToString();
                }

                //Added by PP to fix PXPM-7600 Curve appears to be distorted.
                //If numbervalue is invalid then set current value of textbox to numbervalue
                if ((numberValue < min || numberValue > max) && !Global.IsOffline)
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        var currVal = Convert.ToDouble(textBox.Text);

                        if (currVal < min || currVal > max)
                        {
                            numberValue = numberDefault;
                        }
                        else
                        {
                            numberValue = currVal;
                        }
                    }

                }
            }
            if (Global.IsOffline)
            {
                if ((_isEnabled == true || showvalueInBothModes == true/*|| !Global.IsOffline*/) && !notAvailable)
                {
                    /*Added by Astha
                    Following lines of code is added to handle null exception which occurs in offline mode when Tag value is null*/


                    //Added dynamic min/max calculation for High-load1 and High-load2 to avoid its overlapping
                    if (!string.IsNullOrEmpty(MinCalculation))
                    {
                        string[] calc = MinCalculation.Split(' ');
                        if (calc.Length > 1)
                        {
                            if (Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.device_type == Global.ACB_PXR35_DEVICE)
                            {
                                switch (calc[1].Trim())
                                {
                                    case "+":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "-":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "*":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "/":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / Convert.ToDouble(calc[2].Trim());
                                        break;
                                }
                            }
                            else
                            {
                                switch (calc[1].Trim())
                                {
                                    case "+":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + stepsize;
                                        break;
                                    case "-":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - stepsize;
                                        break;
                                    case "*":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * stepsize;
                                        break;
                                    case "/":
                                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / stepsize;
                                        break;
                                }
                            }

                        }
                        else
                        {
                            min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                        }
                    }

                    if (!string.IsNullOrEmpty(MaxCalculation))
                    {

                        string[] calc = MaxCalculation.Split(' ');
                        if (calc.Length > 1)
                        {
                            if (Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.device_type == Global.ACB_PXR35_DEVICE)
                            {
                                switch (calc[1].Trim())
                                {
                                    case "+":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "-":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "*":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case "/":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / Convert.ToDouble(calc[2].Trim());
                                        break;
                                    case " ":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                                        break;
                                }
                            }
                            else
                            {
                                switch (calc[1].Trim())
                                {
                                    case "+":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + stepsize;
                                        break;
                                    case "-":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - stepsize;
                                        break;
                                    case "*":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * stepsize;
                                        break;
                                    case "/":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / stepsize;
                                        break;
                                    case " ":
                                        max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                                        break;
                                }
                            }

                        }
                        else
                        {
                            max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                        }

                    }

                    //Added by SRK to fix PXPM-6215
                    if (min > max)
                    {
                        min = max;
                    }
                    if (Global.IsOpenFile)
                    {
                        if (numberValue != -1 && numberValue >= min && numberValue <= max)
                        {
                            textBox.Text = Global.updateValueonCultureBasis(numberValue.ToString());
                        }
                    }
                }


                if (textBox.Tag == null)
                {
                    textBox.Text = string.Empty;
                }
                else
                {
                    if (Global.IsOpenFile && (_isEnabled == true || showvalueInBothModes == true) && !notAvailable)
                    {
                        if (numberValue != -1 && numberValue >= min && numberValue <= max)
                        {
                            textBox.Text = Global.updateValueonCultureBasis(numberValue.ToString());
                        }
                    }
                    string[] getTagInfoasStringOfText = (string[])(textBox.Tag);
                    //Added null for fixing coverity scan issue
                    string defaultValueofText = (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0) ? getTagInfoasStringOfText[0] : string.Empty;
                    if (textBox.Text == string.Empty || !((Convert.ToDouble(textBox.Text, CultureInfo.CurrentUICulture) >= min && Convert.ToDouble(textBox.Text, CultureInfo.CurrentUICulture) <= max)))
                    {
                        textBox.Text = Global.updateValueonCultureBasis(defaultValueofText);
                        if ((min >= numberDefault || min >= numberValue) ||
                       (max <= numberDefault || max <= numberValue))
                        {
                            string[] getTagInfo = (string[])(textBox.Tag);
                            bool SDPickuptexttag = false;
                            //handle visibility of short delay pickup
                            if (_isEnabled == false && Global.device_type == Global.MCCBDEVICE && textBox.Name == "txt_CPC081")
                                SDPickuptexttag = true;


                            if (max == numberDefault)
                            {
                                //update text tag value before calling textbox_change to handle item in change summary
                                if (SDPickuptexttag == true)
                                {
                                    //Added null for fixing coverity scan issue
                                    if (getTagInfo != null && getTagInfo[0].Count() > 0)

                                    {
                                        getTagInfo[0] = numberDefault.ToString();
                                        textBox.Tag = getTagInfo;
                                    }
                                }
                                textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                                numberValue = numberDefault;
                            }
                            else
                            {
                                if (SDPickuptexttag == true)
                                {
                                    //Added null for fixing coverity scan issue
                                    if (getTagInfo != null && getTagInfo[0].Count() > 0)
                                    {
                                        getTagInfo[0] = min.ToString();
                                        textBox.Tag = getTagInfo;
                                    }

                                }
                                textBox.Text = Global.updateValueonCultureBasis(min.ToString());
                                numberDefault = min;
                                numberValue = min;
                            }
                            if (SDPickuptexttag == false)
                            {
                                //Added null for fixing coverity scan issue
                                if (getTagInfo != null && getTagInfo[0].Count() > 0)
                                {
                                    getTagInfo[0] = numberValue.ToString();
                                    textBox.Tag = getTagInfo;
                                }
                            }

                        }
                    }



                }
                if (numberDefault == max && !Global.IsOpenFile && type != Type.type_bNumber)
                {
                    numberDefault = max;
                    numberValue = numberDefault;
                    textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                    if (textBox.Tag != null)
                    {
                        string[] getTagInfo = (string[])(textBox.Tag);
                        getTagInfo[0] = numberValue.ToString();
                        textBox.Tag = getTagInfo;
                    }
                }

                //If numbervalue is invalid then set current value of textbox to numbervalue
                if (numberValue < min || numberValue > max)
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        var currVal = Convert.ToDouble(textBox.Text);

                        if (currVal < min || currVal > max)
                        {
                            numberValue = numberDefault;
                        }
                        else
                        {
                            numberValue = currVal;
                        }
                    }

                }
                if (textBox.Name == "txt_CPC014A" && numberValue == 0 && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && Global.IsOpenFile)
                {
                    numberValue = numberDefault;
                }
                if (textBox.Name == "txt_CPC010" && numberValue == -1 && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && (Global.IsOpenFile || Global.IsOffline))
                {
                    numberValue = numberDefault;
                }
                //If other than max value is set as default and textbox is empty then set its default value to as text
                //if (textBox.Text == string.Empty && (numberDefault >= min && numberDefault <= max))
                //{
                //    textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                //}
            }

            if((!Global.IsOffline && _isEnabled && textBox.Tag != null)||showvalueInBothModes==true||(readOnly && bnumbervisible))
            {
                //for online, 'numberValue' for Highload 1 and Highload 2 always set to the value coming from device when respective dependent is changed.
                if ((Global.device_type == Global.PTM_DEVICE  || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE) && (textBox.Name == "txt_CPC021" || textBox.Name == "txt_CPC010"
                     || textBox.Name == "txt_CPC014A"|| textBox.Name == "txt_SYS020" || textBox.Name == "txt_ADVA030" || textBox.Name == "txt_ADVA031" || textBox.Name == "txt_ADVA034" || textBox.Name == "txt_ADVA035"
                    || textBox.Name == "txt_ADVA036" || textBox.Name == "txt_ADVA037" || textBox.Name == "txt_ADVA039" || textBox.Name == "txt_ADVA041" || textBox.Name == "txt_CPC024"))
                {
                    numberValue = Convert.ToDouble(textBox.Text);
                }
                //string defaultValueofText = string.Empty; //Coverity fix 418334
                string[] getTagInfoasStringOfText = (string[])(textBox.Tag);


                //if (min == max)
                //{
                //    if (getTagInfoasStringOfText[2] != "txt_CPC081")
                //    {
                //        numberValue = min;
                //    }
                //}
                //if (((decimal)(Convert.ToDecimal(numberValue)) % (decimal)(stepsize) != 0) && min != max)
                //{

                //    numberValue = min;
                //}               
                //if (numberValue != -1 && numberValue >= min && numberValue <= max)
                //{
                //    textBox.Text = Global.updateValueonCultureBasis(numberValue.ToString());              
                //}
                //else if ((max == numberDefault) && (!(numberValue >= min && numberValue <= max)))
                //{
                //    if (numberValue == 0 && getTagInfoasStringOfText[2] == "txt_CPC081")
                //    {
                //        textBox.Text = Global.updateValueonCultureBasis(getTagInfoasStringOfText[0].ToString());
                //    }
                //    else
                //    {
                //        textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                //    }
                //}
                //else
                //{
                //    textBox.Text = Global.updateValueonCultureBasis(min.ToString());
                //}

                ////getTagInfoasStringOfText[0] = textBox.Text;

                //if (numberDefault != Convert.ToDouble(getTagInfoasStringOfText[0], CultureInfo.CurrentUICulture))
                //{
                //    defaultValueofText = numberDefault.ToString();
                //}
                //else
                //{
                //    defaultValueofText = textBox.Text;  //getTagInfoasStringOfText[0];
                //}
                ////textBox.Tag = getTagInfoasStringOfText;
                // -------------------

                //Added dynamic min/max calculation for High-load1 and High-load2 to avoid its overlapping
                if (!string.IsNullOrEmpty(MinCalculation))
                {
                    string[] calc = MinCalculation.Split(' ');
                    if (calc.Length > 1)
                    {
                        if(Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.device_type == Global.ACB_PXR35_DEVICE)
                        {
                            switch (calc[1].Trim())
                            {
                                case "+":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "-":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "*":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "/":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / Convert.ToDouble(calc[2].Trim());
                                    break;
                            }
                        }
                        else
                        {
                            switch (calc[1].Trim())
                            {
                                case "+":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + stepsize;
                                    break;
                                case "-":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - stepsize;
                                    break;
                                case "*":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * stepsize;
                                    break;
                                case "/":
                                    min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / stepsize;
                                    break;
                            }
                        }
                        
                    }
                    else
                    {
                        min = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                    }
                   
                }
                

                if (!string.IsNullOrEmpty(MaxCalculation))
                {
                    string[] calc = MaxCalculation.Split(' ');
                    if (calc.Length > 1)
                    {
                        if(Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.device_type == Global.ACB_PXR35_DEVICE)
                        {
                            switch (calc[1].Trim())
                            {
                                case "+":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "-":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "*":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * Convert.ToDouble(calc[2].Trim());
                                    break;
                                case "/":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / Convert.ToDouble(calc[2].Trim());
                                    break;
                            }
                        }
                        else
                        {
                            switch (calc[1].Trim())
                            {
                                case "+":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue + stepsize;
                                    break;
                                case "-":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue - stepsize;
                                    break;
                                case "*":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue * stepsize;
                                    break;
                                case "/":
                                    max = ((Settings)TripUnit.IDTable[calc[0]]).numberValue / stepsize;
                                    break;
                            }
                        }
                       
                    }
                    else
                    {
                        max= ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                    }
                }

                //Added by SRK to fix PXPM-6215
                if (min > max)
                {
                    min = max;
                }

                if (min == max)
                {
                    if (getTagInfoasStringOfText != null && getTagInfoasStringOfText[2] != "txt_CPC081")
                    {
                        numberValue = min;
                    }
                }
                if (((decimal)(Convert.ToDecimal(numberValue)) % (decimal)(stepsize) != 0) && min != max)
                {
                    numberValue = min;
                }
                if (numberValue != -1 && numberValue >= min && numberValue <= max)
                {
                    textBox.Text = Global.updateValueonCultureBasis(numberValue.ToString());
                }
                else if ((max == numberDefault) && (!(numberValue >= min && numberValue <= max)))
                {
                    //if (numberValue == 0 && getTagInfoasStringOfText[2] == "txt_CPC081")
                    //{
                    //    textBox.Text = Global.updateValueonCultureBasis(getTagInfoasStringOfText[0].ToString());
                    //}
                    //else
                    //{
                    //    textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                    //}
                    textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                }
                //else if (numberValue != -1 && numberValue <= min && numberValue <= max)   //This condition is added to fix - Highload 2 not setting back to it's original value after undo operation
                //{
                //    textBox.Text = Global.updateValueonCultureBasis(numberDefault.ToString());
                //}
                else
                {
                    textBox.Text = Global.updateValueonCultureBasis(min.ToString());
                }
                if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && getTagInfoasStringOfText != null && getTagInfoasStringOfText[2] == "txt_CPC014A")
                {
                    getTagInfoasStringOfText[0] = textBox.Text;
                    textBox.Tag = getTagInfoasStringOfText;
                }
                //defaultValueofText = numberDefault.ToString();

                //if (getTagInfoasStringOfText != null)
                //{

                //    if (numberDefault != Convert.ToDouble(getTagInfoasStringOfText[0], CultureInfo.CurrentUICulture))
                //        {
                //            defaultValueofText = numberDefault.ToString();
                //        }
                //        else
                //        {
                //            defaultValueofText = getTagInfoasStringOfText[0];
                //        }
                //}
                //else
                //{
                //    defaultValueofText = numberDefault.ToString();
                //}
            }
            bool isError = false;
            ToolTip toolTip;

            if (bcalculated && visible)
            {
                string[] equation_string1 = calculation.Split(',');
                if(label_calculation != null && (ID == "CPC018A" || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "CPC018") || ID == "CPC022"))
                {
                    label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                }
                else if (label_calculation != null && equation_string1[0].Contains("="))
                {
                    label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(equation_string1[0]).ToString()) + " A";
                }
                else
                {
                    string dependent_id;
                    // Settings setpoint1 = new Settings();  //#COVARITY FIX   234917
                    int j;
                    if (equation_string1.Length == 1 || !(equation_string1[0].Contains("=")))
                    {
                        j = 0;
                    }
                    else
                    {
                        j = 1;
                    }
                    for (int i = j; i < equation_string1.Length; i++)
                    {
                        dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                        Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                        if (setpoint1 != null && setpoint1.calculation != null)
                        {
                            setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                        }
                    }
                }
            }

            if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "SYS019")
            {
                numberValue = (min == max && min != numberValue) ? min : numberValue;
                textBox.Text = numberValue.ToString();
            }
            if ((textBox.Text.Trim() == "") && (!Global.IsOffline))
            {
                isError = true;
                description = Resource.StepSizeNotInRangeError;
            }        
           
            if ((textBox.Text.Trim() != "") && ((decimal)(Convert.ToDecimal(textBox.Text.Trim())) % (decimal)(stepsize) != 0) && (Math.Round((decimal)(Convert.ToDecimal(textBox.Text.Trim())), 2) % (decimal)(stepsize) != 0) && min != max)
            {
                isError = true;
                description = Resource.StepSizeIncorrectError;
            }

            if ((textBox.Text.Trim() != "") && (Convert.ToDouble(textBox.Text.Trim(), CultureInfo.CurrentUICulture) < min))
            {
                isError = true;
                description = Resource.ValueNotGreaterThanMinError;
            }
            if ((textBox.Text.Trim() != "") && (Convert.ToDouble(textBox.Text.Trim(), CultureInfo.CurrentUICulture) > max))
            {
                isError = true;
                description = Resource.ValueNotLesserThanMaxError;
            }
            if (isError == true)//&& (textBox.IsEnabled == true))
            {
                valid = false;
                textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
                toolTip = ScreenCreator.createToolTip(min, max, stepsize, unit, description);
                textBox.ToolTip = toolTip;
                toolTip.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
            }
            else
            {
                valid = true;
                textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");

                //Added by Sarika to fix - PXPM-7287 (show tooltip inline with min,max and unit for below setpoints)
                if ( textBox.Name == "txt_ADVA012" || textBox.Name == "txt_ADVA007" || textBox.Name == "txt_ADVA032" || textBox.Name == "txt_ADVA033" 
                    || textBox.Name == "txt_ADVA030" || textBox.Name == "txt_ADVA031" || textBox.Name == "txt_ADVA039" || textBox.Name == "txt_ADVA041" 
                    || textBox.Name == "txt_ADVA036" || textBox.Name == "txt_ADVA037" || textBox.Name == "txt_ADVA034" || textBox.Name == "txt_ADVA035" )
                {
                    toolTip = ScreenCreator.createToolTip(min, max, stepsize, unit, description,true);
                }
                else
                {
                    toolTip = ScreenCreator.createToolTip(min, max, stepsize, unit, description);
                }
                    
                textBox.ToolTip = toolTip;
            }
            if (Global.IsOffline && !_isEnabled && visible & type!=Settings.Type.type_bNumber && showvalueInBothModes==false)
            {
                textBox.TextChanged -= new TextChangedEventHandler(textBox_textChange);
                textBox.Text = string.Empty;
                textBox.TextChanged += new TextChangedEventHandler(textBox_textChange);
            }
            ExcludeNumberFromRange();
            ToolTipService.SetToolTip(toolTip, "");
            ToolTipService.SetShowDuration(textBox, 300000);
        }


        public void setTextBox_text(String _text)
        {
            textBox.Text = Global.updateValueonCultureBasis(_text);
        }
        private void upDateCheckBox()
        {
            if (checkBox == null)
                return;

            checkBox.IsEnabled = !readOnly;
            if (visible)
            {
                checkBox.Visibility = Visibility.Visible;
            }
            else
            {
                checkBox.Visibility = Visibility.Collapsed;
            }
            checkBox.IsChecked = bValue;
        }
        public void updateComboBox(ref Settings setting)
        {
            //Added by Astha to handle the values of readonly combobox in offline mode in both MCCB and ACB .
            //This is required because in ACB when mode is offline Current Rating combobox should be blank and when MCCB type is selected then in offline mode 
            //Current rating readonly value can be false in offline mode and values should be populated in current rating combobox as per the dependency applied.
            //We can get rid of redundant codde of calling fillcombox and specifying SYS001 if in offline mode in both ACB and MCCB types read only controls are supposed to be blank 

            try
            {
                item_ComboBox defaultItem;
                ComboBoxItem defaultComboboxItem;
                string valueOfValuemap;
                if (setting.ItemsToDisplayfromLookupTable.Count > 0 /*&& setting.ID != "SYS001"*//*  (!Global.IsOffline)*/)
                {
                    ScreenCreator.fillComboBox(ref setting, ref setting.comboBox);
                }

                if (comboBox == null)
                {
                    return;
                }

                Grid _GridParent = null;
                if (setting.label_notavailable != null)
                {
                    _GridParent = Global.FindParent_Expander<Grid>(setting.label_notavailable);
                }

                if (setting.visible && _GridParent != null)
                {
                    _GridParent.Visibility = Visibility.Visible;
                    if (label_name != null)
                    {
                        label_name.Visibility = Visibility.Visible;
                    }
                    if (comboBox != null)
                    {
                        comboBox.Visibility = Visibility.Visible;
                    }


                    if (type == Type.type_bSelection)
                    {
                        toggle.Visibility = Visibility.Visible;
                        StackPanel _stackParent = Global.FindParent<StackPanel>(toggle);

                        //Added null for fixing coverity scan issue
                        if (_stackParent != null)
                            _stackParent.Visibility = Visibility.Visible;

                        if (!(toggle.IsChecked == true))
                        {
                            foreach (var cmbi in comboBox.Items.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == selectionValue))
                            {
                                cmbi.IsSelected = true;

                            }
                            comboBox.IsEnabled = false;
                        }

                    }
                }
                else if (_GridParent != null)
                {
                    if (label_name != null)
                    {
                        label_name.Visibility = Visibility.Collapsed;
                    }
                    if (comboBox != null)
                    {
                        comboBox.Visibility = Visibility.Collapsed;
                    }
                    _GridParent.Visibility = Visibility.Collapsed;
                    _GridParent.Visibility = Visibility.Collapsed;
                    if (type == Type.type_bSelection)
                    {
                        toggle.Visibility = Visibility.Collapsed;
                        StackPanel _stackParent = Global.FindParent<StackPanel>(toggle);
                        if (_stackParent != null)
                            _stackParent.Visibility = Visibility.Collapsed;

                    }
                }
                //============================================
                if (setting.label_calculation != null && !setting.notAvailable)
                {
                    if (label_calculation != null)
                    {
                        label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                    }
                    if (setting.readOnly)
                    {
                        setting.label_calculation.Visibility = Visibility.Collapsed;
                    }
                    else if (!setting.readOnly)
                    {
                        setting.label_calculation.Visibility = Visibility.Visible;
                    }

                    if (!Global.IsOffline)
                    {
                        setting.label_calculation.Visibility = Visibility.Visible;
                    }
                }
                if (setting.emptyspace != null && !setting.notAvailable)
                {
                    if (setting.readOnly)
                    {
                        setting.emptyspace.Visibility = Visibility.Collapsed;
                    }
                    else if (!setting.readOnly)
                    {
                        setting.emptyspace.Visibility = Visibility.Visible;
                    }
                }
                //============================================
                setting.comboBox.IsEnabled = !setting.readOnly;
                if (setting.label_notavailable != null && setting.notAvailable)
                {
                    setting.label_notavailable.Visibility = Visibility.Visible;
                    if (setting.ID == "SYS12")
                    {
                        setting.rotation_button.IsEnabled = !setting.notAvailable;
                    }

                    if (setting.type == Type.type_bSelection)
                    {
                        setting.toggle.IsEnabled = !notAvailable;
                    }
                    return;
                }
                else if (setting.label_notavailable != null)
                {
                    setting.label_notavailable.Visibility = Visibility.Collapsed;
                    if (setting.ID == "SYS12")
                    {
                        setting.rotation_button.IsEnabled = !setting.notAvailable;
                    }
                    if (type == Type.type_bSelection)
                    {
                        setting.toggle.IsEnabled = !notAvailable;
                    }
                }
                if (setting.rotation_button != null && setting.readOnly)
                {
                    setting.rotation_button.IsEnabled = !setting.readOnly;
                }
                else if (setting.rotation_button != null)
                {
                    setting.rotation_button.IsEnabled = !setting.readOnly;
                }

                //Original code comented by AK         
                //added by AK to resolve bug 139308
                //Initially select value of Long delay slope =i2t and change short delay slope = i2t.
                // then change  Long delay slope to i4t. short delay slope should be "Flat". In original code shord delay slope was not getting changed
                //despite having the code:-  comboBox.SelectedItem = selectionValue; (Now commented)
                //Following code is added by Astha           

                if ((comboBox.SelectedIndex == -1 && setting.selectionDefault != null && setting.ID != "SYS004A" && setting.ID != "SYS4A") || (setting.selectionDefaultAfterDependencyApplied != null)/*&& comboBox.SelectedIndex == -1*/)/*|| (!String.Equals(setting.selectionDefault, "0")))*/
                {
                    if ((Convert.ToInt32(setting.selectionDefault) == -1) || (Convert.ToInt32(setting.selectionDefaultAfterDependencyApplied) == -1))
                    {
                        comboBox.SelectedIndex = Convert.ToInt32(setting.selectionDefault);
                        if(setting.comboBox.Items != null)
                        {
                            defaultComboboxItem = (ComboBoxItem)setting.comboBox.Items[0];
                        }
                        

                        //setting.defaultSelectionValue = defaultComboboxItem.Content.ToString();

                    }
                    else
                    {
                        valueOfValuemap = setting.value_map[setting.selectionDefault].ToString();

                        if (setting.lookupTable.Count > 0)
                        {
                            defaultItem = (item_ComboBox)setting.lookupTable[valueOfValuemap];

                            setting.defaultSelectionValue = defaultItem.item;

                            //by PP
                            //Reverted this fix which was intially added to fix - PXPM-5518
                            //Because of this fix new issues introduced - PXPM-5693, PXPM-6081

                            ////For relay values, need to set value from file while opening file
                            //if (!Global.IsOpenFile || setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"
                            //    || setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A" || setting.ID == "GEN001")
                            //{
                            //    setting.defaultSelectionValue = defaultItem.item;
                            //}

                            string deviceValue = string.Empty;
                            if (setting.selectionValue != null)
                            {
                                deviceValue = setting.selectionValue.ToString();
                                setting.selectionValue = setting.defaultSelectionValue;
                            }

                            if (/*(Global.IsOffline && (setting.ID == "SYS004A" || setting.ID == "SYS4A") && (setting.selectionValue != Resource.SYS004ADefault)) || */(setting.ID != "SYS004A" && setting.ID != "SYS4A") || !Global.IsOffline)

                            {

                                //this case is added for PXR35 as change summary is having rating change issue.
                                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                                {
                                    for (int i = 0; i < comboBox.Items.Count; i++)
                                    {
                                        ComboBoxItem CItem = (ComboBoxItem)comboBox.Items[i];
                                        if (CItem != null && (string)CItem.Content == setting.defaultSelectionValue)
                                        {
                                            comboBox.SelectedIndex = i;
                                            break;
                                        }
                                    }
                                } 
                                else { comboBox.SelectedIndex = Convert.ToInt32(setting.selectionDefault); }
                                
                                //In online mdoe for rating,standard and frame - If values are incorrectly configured in device then default values will be added to those setting
                                //Identifying such combinations to show warning message.
                                if (!Global.IsOffline &&
                                    (setting.ID == "SYS000" || setting.ID == "SYS002" || setting.ID == "SYS001" || setting.ID == "SYS001A" ||
                                     setting.ID == "SYS01" || setting.ID == "SYS16" || setting.ID == "SYS6"))
                                {
                                    string setId = setting.name;
                                    var Item = Global.validationErrorList.Find(x => x.SettingName == setId);
                                    if (Item == null && setting.selectionValue != deviceValue)
                                    {
                                        Global.validationErrorList.Add(new ValidationMapper
                                        {
                                            SettingName = setting.name,
                                            PxrValue = setting.selectionValue,
                                            TripUnitValue = deviceValue
                                        });
                                    }
                                }
                            }
                        }
                    }

                }


                if (selectionValue != null && (!Global.IsOffline || !readOnly))
                {
                    foreach (var cmbi in comboBox.Items?.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == selectionValue))
                    {
                        cmbi.IsSelected = true;
                    }
                }

                //else if (comboBox.SelectedIndex != -1 && selectionValue != null && (setting.ID == "SYS004A" || setting.ID == "SYS4A"))
                //{
                //    foreach (var cmbi in comboBox.Items.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == selectionValue))
                //    {
                //        cmbi.IsSelected = true;
                //        break;
                //    }
                //}
                else
                {
                    if (setting.showvalueInBothModes == false)
                    {
                        if (null != comboBox && comboBox.SelectedIndex != -1)
                        {
                            comboBox.SelectedIndex = -1;
                        }
                        setting.selectionValue = setting.defaultSelectionValue;

                    }
                }
                if (setting.label_name != null)
                {
                    setting.label_name.Content = setting.name;
                }

                if (comboBox.Tag != null)
                {
                    string[] getTagInfoasString = (string[])comboBox.Tag;
                    //Added null for fixing coverity scan issue
                    if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                    {
                        string[] tagInfo = new string[3];
                        tagInfo[0] = getTagInfoasString[0];

                        //For Motor protection group appending its subgroup name before its setpoint name - For differentiating in Change summary  
                        //eg. Over Voltage - Feature
                        if (setting.GroupID == "04" || setting.GroupID == "004")
                        {
                            tagInfo[1] = ((Group)(TripUnit.groups[4])).subgroups[setting.subgrp_index].name + " - " +
                                    setting.label_name.Content.ToString().Trim();
                        }
                        else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && GroupID == "01" &&
                        (subgrp_index == 5 || subgrp_index == 6 || subgrp_index == 7 || subgrp_index == 8 ||
                        subgrp_index == 9 || subgrp_index == 10 || setting.subgrp_index == 11 || setting.subgrp_index == 12 || setting.subgrp_index == 13 || setting.subgrp_index == 14 || setting.subgrp_index == 15 || setting.subgrp_index == 16))
                        {
                            tagInfo[1] = ((Group)(TripUnit.groups[1])).subgroups[setting.subgrp_index].name + " - " +
                                    setting.label_name.Content.ToString().Trim();
                        }
                        else
                        {
                            tagInfo[1] = setting.label_name.Content.ToString().Trim(); // stores the label to be displayed in summary
                        }

                        //tagInfo[1] = setting.label_name.Content.ToString().Trim();
                        tagInfo[2] = getTagInfoasString[2];
                        comboBox.Tag = tagInfo;
                    }
                }

                if(Global.device_type != Global.PTM_DEVICE)
                      ChangesForRelay(setting);
                if (Global.IsOffline && !Global.IsOpenFile && !Global.isExportControlFlow)
                {
                    if (setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015")
                    {
                        string[] getTagInfoasString1 = (string[])comboBox.Tag;
                        string[] tagInfo1 = new string[3];
                        tagInfo1[0] = setting.defaultSelectionValue;
                        tagInfo1[1] = getTagInfoasString1[1];
                        tagInfo1[2] = getTagInfoasString1[2];
                        comboBox.Tag = tagInfo1;
                    }
                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        private void ChangesForRelay(Settings setting)
        {
            try
            {
                if (Global.IsOffline && !Global.IsOpenFile && !Global.isExportControlFlow)
                {
                    if (setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015")
                    {
                        Settings ground = TripUnit.getGroundProtectionGeneralGrp();
                        Settings MM_Mode = TripUnit.getMM_ModeProtectionGeneralGrp();

                        //Changes For Relay2
                        switch (setting.ID)
                        {
                            case "SYS013":
                                if (MM_Mode.bValue && setting.lookupTable.Contains("0017"))
                                {//Default - MM mode active 
                                    setting.selectionValue = Resource.SYS013Item0017;// ((item_ComboBox)setting.lookupTable["0017"]).item;
                                    setting.defaultSelectionValue = Resource.SYS013Item0017;
                                    setting.selectionDefault = "23";
                                }
                                else if (setting.lookupTable.Contains("0012"))
                                { //Default - breaker health alarm 
                                    setting.selectionValue = Resource.SYS013Item0012;// ((item_ComboBox)setting.lookupTable["0012"]).item;
                                    setting.defaultSelectionValue = Resource.SYS013Item0012;
                                    setting.selectionDefault = "12";
                                }
                                break;
                            case "SYS014":
                                if (ground.bValue && setting.lookupTable.Contains("0006"))
                                {
                                    //Default - GF trip 
                                    setting.selectionValue = Resource.SYS013Item0006;// ((item_ComboBox)setting.lookupTable["0006"]).item;
                                    setting.defaultSelectionValue = Resource.SYS013Item0006;
                                    setting.selectionDefault = "6";
                                }
                                else if (setting.lookupTable.Contains("000A"))
                                {
                                    //Default - HL2
                                    setting.selectionValue = Resource.SYS013Item000A;// ((item_ComboBox)setting.lookupTable["000A"]).item;
                                    setting.defaultSelectionValue = Resource.SYS013Item000A;
                                    setting.selectionDefault = "10";
                                }
                                break;
                            case "SYS015":
                                if (setting.lookupTable.Contains("0008"))
                                {
                                    // All trips
                                    setting.selectionValue = Resource.SYS013Item0008;// ((item_ComboBox)setting.lookupTable["0008"]).item;
                                    setting.defaultSelectionValue = Resource.SYS013Item0008;
                                    setting.selectionDefault = "8";
                                }
                                break;


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
         
        
        }
        private void UpdateListBox()
        {
            if (listBox == null)
                return;

            if (visible)
            {
                label_name.Visibility = Visibility.Visible;
                listBox.Visibility = Visibility.Visible;
            }
            else
            {
                label_name.Visibility = Visibility.Collapsed;
                listBox.Visibility = Visibility.Collapsed;
                if (listBox.Items != null && listBox.Items.Count != 0)
                {
                    for (int i = 0; i < listBox.Items.Count; i++)
                    {
                        CheckBox myChkBox = (CheckBox)(listBox.Items[i]);
                        myChkBox.IsChecked = false;
                    }
                }

            }
        }

        // UI Elements
        public TextBox textBox;
        public Label label_name;
        public TextBlock leftlabel_forToggle;
        public TextBlock rightlabel_forToggle;
        private Label label_Unit;
        public ComboBox comboBox;
        private CheckBox checkBox;
        private ToolTip toolTip;
        private ListBox listBox;
        public ToggleButton toggle;
        public Button increaseButton;
        public Button decreaseButton;
        public Label label_notavailable;
        public Button rotation_button;
        public Label label_calculation;
        public Label label_catalogNumber;
        public C1MaskedTextBox IPAddressControl;
        public Grid setpoint_stack;   // Added to grey out the Not available setpoint
        public StackPanel emptyspace; // added for the calculated value

        //////////////////////////// UI Setup Methods//////////////////////////////////////
        public void SetLabelName(ref Label _labelName)
        {
            label_name = _labelName;
        }

        public void IPTextBox(ref C1MaskedTextBox _IPAddress)
        {
            IPAddressControl = _IPAddress;
            _IPAddress.LostFocus += IPAddress_LostFocus;
        }

        private void IPAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            bool isError = false;
            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            string[] Each_IP = null;
            Each_IP = IPAddressControl.Text.Split('.');
            // string changedIP = string.Empty;    //#COVARITY FIX   234915
            string IPvalue;
            Settings setpoint1;//= new Settings();    //#COVARITY FIX              234796
            int value = 0;
            
            string ip,ip2;
            description = Resource.IpBoxRangeCorrectMsg;

            if (ID == "IP002")
            {
                string changedIP = Each_IP[3];
                try
                {
                    value = Convert.ToInt32(changedIP.Trim());
                    if (!(value < 33 && value > 15))
                    {
                        isError = true;
                        description = Resource.IpBoxRangeIncorrectError;
                        
                    }
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);
                    isError = true;
                    description = Resource.IpBoxRangeIncorrectError;
                 
                }
                finally
                {
                    IPvalue = value.ToString();
                    while (IPvalue.Length != 3)
                    {
                        IPvalue = "0" + IPvalue;
                    }
                    ip = "255.255.255." + IPvalue;               
                    IPAddressControl.Text = ip;
                
                }
                textvalue = value;
            }
            else if (ID == "IP003")
            {
                setpoint1 = TripUnit.getIPAddress();
                if (setpoint1.IPAddressControl == null)
                {
                    return;
                }
                string[] IP1 = null;

                description = Resource.IpBoxCorrectMsg;
                IP1 = setpoint1.IPAddressControl.Text.Split('.');

                for (int i = 2; i < 4; i++)
                {
                    value = 0;
                    try
                    {
                        value = Convert.ToInt32(Each_IP[i].Trim());
                        if (!(value <=255 && value >= 0))
                        {
                            isError = true;                          
                            description = Resource.IpBoxIncorrectError;
                        }

                    }
                    catch (Exception ex)
                    {
                        LogExceptions.LogExceptionToFile(ex);
                        isError = true;                        
                        description = Resource.IpBoxIncorrectError;
                    }             

                    finally
                    {
                        IPvalue = value.ToString();
                        while (IPvalue.Length != 3)
                        {
                            IPvalue = '0' + IPvalue;
                        }
                        Each_IP[i] = IPvalue;
                       
                    }
                }      

                ip = IP1[0] + "." + IP1[1] + "." + Each_IP[2] + "." + Each_IP[3];         
                IPAddressControl.Text = ip.ToString();
         
            }
            else
            {
                setpoint1 = TripUnit.getEthernetCAMDefaultGateway();
                if (setpoint1.IPAddressControl == null)
                {
                    return;
                }
                string[] IP1 = setpoint1.IPAddressControl.Text.Split('.');


                description = Resource.IpBoxCorrectMsg;
                // IP1 = setpoint1.IPAddressControl.Text.Split('.');    //#COVARITY FIX   234870
                for (int i = 0; i < 4; i++)
                {
                    value = 0;
                    try
                    {
                        value = Convert.ToInt32(Each_IP[i].Trim());
                        if (!(value < 255 && value >= 0))
                        {
                            isError = true;
                            description = Resource.IpBoxIncorrectError;
                    
                        }
                    }
                    catch (Exception ex)
                    {
                        LogExceptions.LogExceptionToFile(ex);
                        isError = true;
                        description = Resource.IpBoxIncorrectError;
                   
                    }
                    finally
                    {
                        IPvalue = value.ToString();
                        while (IPvalue.Length != 3)
                        {
                            IPvalue = '0' + IPvalue;
                        }
                        Each_IP[i] = IPvalue; ;
                    }

                }
              
                ip = Each_IP[0] + "." + Each_IP[1] + "." + Each_IP[2] + "." + Each_IP[3];       
                ip2 = Each_IP[0] + "." + Each_IP[1] + "." + IP1[2] + "." + IP1[3];             
                IPAddressControl.Text = ip;
                setpoint1.IPAddressControl.Text = ip2.ToString();
            
            }
            if (isError == true)            {
                valid = false;
               
                IPAddressControl.BorderBrush = Brushes.Red;
                IPAddressControl.BorderThickness = new Thickness(2);
              
                toolTip = ScreenCreator.createToolTip(description);
                IPAddressControl.ToolTip = toolTip;
                toolTip.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
            }
            else
            {
                valid = true;           
          
                IPAddressControl.BorderBrush = Brushes.LightGray;
                IPAddressControl.BorderThickness = new Thickness(1);
                toolTip = ScreenCreator.createToolTip(description);
                IPAddressControl.ToolTip = toolTip;
            }
            IPaddress = IPAddressControl.Text;
            if ((string[])((C1MaskedTextBox)sender).Tag != null)
            {
                string[] getTagInfoasString = (string[])((C1MaskedTextBox)sender).Tag;
                //Added null for fixing coverity scan issue
                if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                {
                    string displayFieldName = getTagInfoasString[1];
                    string ControlName = getTagInfoasString[2];

                    if (SettingValueChange != null && ((C1MaskedTextBox)sender).Visibility == Visibility.Visible)
                    {
                        SettingValueChange(displayFieldName, IPaddress_default, IPaddress, ControlName, ((C1MaskedTextBox)sender).IsVisible, GroupID);
                    }
                }
            }
        }

       
        public void setupTextBox(ref TextBox _textBox, ref Label _labelUnit, ref ToolTip _toolTip)
        {
            textBox = _textBox;

            label_Unit = _labelUnit;
            toolTip = _toolTip;
            textBox.TextChanged += new TextChangedEventHandler(textBox_textChange);
            textBox.KeyDown += textBox_KeyDown;
            //textBox.GotFocus += textBox_GotFocus;
            //textBox.LostFocus += textBox_LostFocus;
        }

        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && (string[])((TextBox)sender).Tag != null)
            {
                string[] getTagInfoasString = (string[])((TextBox)sender).Tag;
                //Added null for fixing coverity scan issue
                if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                {
                    string defaultValue = getTagInfoasString[0];
                    ((TextBox)(sender)).Text = defaultValue;
                }
            }
        }



        private void textBox_textChange(object sender, TextChangedEventArgs e)
        {
            Global.setting_Changed = false;
            formValidation_number();

            string[] getTagInfoasString = (string[])((TextBox)sender).Tag;
            // string defaultValue=string.Empty;   
            if (getTagInfoasString != null)
            {
                string defaultValue = getTagInfoasString[0];
                string displayFieldName = getTagInfoasString[1];
                string controlName = getTagInfoasString[2];

                

                if (valid)
                {
                    notifyDependents();

                    if (SettingValueChange != null && ((TextBox)sender).Visibility == Visibility.Visible)
                    {

                        SettingValueChange(displayFieldName, Global.updateValueonCultureBasis(defaultValue), ((TextBox)sender).Text, controlName, ((TextBox)sender).IsVisible,GroupID);
                        if (Global.setting_Changed)
                        {
                            commitedChange = false;

                        }
                        else
                        {

                        }
                    }

                }
                else
                {
                    //remove the item_ComboBox from summary if entered value is invalid
                    if (SettingValueChange != null)
                    {
                        SettingValueChange(displayFieldName, "", "", controlName, ((TextBox)sender).IsVisible,GroupID);
                        if (Global.setting_Changed)
                        {
                            commitedChange = false;
                        }

                    }
                }

            }
            Expander _ExpndrItem = Global.FindParent_Expander<Expander>((TextBox)sender);


            // find the accordian Item for current textbox and change the style based on error in the current group
            Expander _ExpItem = Global.FindParent<Expander>((TextBox)sender);

            if (_ExpndrItem == null && _ExpItem != null)
            {
                if (TripUnit.isErrorInGroup(_ExpItem.Header.ToString()))
                {
                    // _AccItem.SetResourceReference(AccordionItem.StyleProperty, "AccordionItemErrorStyle");
                 //Commented above line temporarily as the red text for accordion header is not displayed consistently
                }
                else
                {
                    _ExpItem.SetResourceReference(Expander.StyleProperty, "ExpanderStyle_trigger");
                }
            }
            else
            {
                // handel the style change of the expander for the sub groups
            }

            if (bcalculated && visible)

            {
                string[] equation_string1 = calculation.Split(',');
                if (equation_string1[0].Contains("="))
                {
                    if (ID == "CPC018A" || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "CPC018") || ID == "CPC022")
                    {
                        label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                    }
                    else
                    {
                        label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(equation_string1[0]).ToString()) + " A";
                    }

                    if (equation_string1.Length > 1)
                    {
                        string dependent_id;
                        // Settings setpoint1 = new Settings();      //#COVARITY FIX     234803
                        int j = 1;
                        for (int i = j; i < equation_string1.Length; i++)
                        {
                            dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                            Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                            if (setpoint1 != null && setpoint1.calculation != null)
                            {
                                setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                            }
                        }
                    }
                }
                else
                {
                    string dependent_id;
                    Settings setpoint1;// = new Settings();          //#COVARITY FIX  234803
                    int j;
                    if (equation_string1.Length == 1 || !(equation_string1[0].Contains("=")))
                    {
                        j = 0;
                    }
                    else
                    {
                        j = 1;
                    }
                    for (int i = j; i < equation_string1.Length; i++)
                    {
                        dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                        setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                        if (setpoint1 != null && setpoint1.calculation != null)
                        {
                            setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                        }
                    }
                }
            }
            Global.updateExpandersVisibility();
            if (Global.device_type == Global.NZMDEVICE)
            {
                NZMCurveCalculations.AddSCRNZMDataToCurve();
            }
            else
            {
                CurvesCalculation.AddScrDataToCurve();
            }
            
            //Fire the event - notifying all subscribers
            if (CurveCalculationChanged != null)
                CurveCalculationChanged(this, null);

        }


     

        public Boolean valid = true;
        public Boolean isValid()
        {
            return valid;
        }
        /// <summary>
        /// Validates the value the user has typed into the forms
        /// </summary>
        private void formValidation_number()
        {

            double temp = 0.0;
            bool isError = false;
            string errorDescription = "";

            try
            {
                if (notAvailable)
                {
                    valid = true;
                    return;
                }
                if (textBox.Text.Trim() != "")
                {
                    bool isNumeric = MainScreen_ViewModel.IsNumeric(textBox.Text);
                    if (isNumeric == false)
                    {
                        textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
                        SetErrorState(Resource.EnterNumericValue);
                        valid = false;
                        return;

                    }
                    else
                    {
                        temp = Double.Parse(textBox.Text,CultureInfo.CurrentUICulture);
                    }
                }



                if ((((decimal)(temp) % (decimal)(stepsize) != 0))&&(min!=max))
                {
                    isError = true;
                    errorDescription = Resource.StepSizeIncorrectError;
                }

                if (temp < min)
                {
                    isError = true;
                    errorDescription = Resource.ValueNotGreaterThanMinError;
                }
                if (temp > max)
                {
                    isError = true;
                    errorDescription = Resource.ValueNotLesserThanMaxError;
                }

                if (textBox.Text.Trim() == "")
                {
                    isError = true;
                    errorDescription = Resource.StepSizeNotInRangeError;
                }

                if (isError == true)
                {
                    textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
                    SetErrorState(errorDescription);
                    valid = false;
                }
                else
                {
                    textBox.ToolTip = "";
                    textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
                    ToolTip lvTooltip = ScreenCreator.createToolTip(min, max, stepsize, unit, description);
                    textBox.ToolTip = lvTooltip;
                    numberValue = temp;
                    valid = true;
                }
                ExcludeNumberFromRange();
            }
            catch (FormatException)
            {
                // setErrorState("Value must be a number" + description);
                isError = true;
                errorDescription = Resource.InvalidNumber;
                textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
                SetErrorState(errorDescription);
                valid = false;

            }


        }
        private void ExcludeNumberFromRange()
        {


            try
            {
                if (!string.IsNullOrEmpty(ExcludedValue))
                {
                    int count = 0;
                    string[] calc = ExcludedValue.Split(',');
                    double[] excludeValue = new double[calc.Length];                  
                    if (calc.Length == 1)
                    {
                        excludeValue[0] = ((Settings)TripUnit.IDTable[calc[0]]).numberValue;
                    }
                    if(calc.Length > 1)
                    {
                        for(int i = 0; i < calc.Length; i++) { excludeValue[i] = ((Settings)TripUnit.IDTable[calc[i]]).numberValue; }
                    }
                    if (textBox.Text.Trim() != "")
                    {
                       
                        var temp = Double.Parse(textBox.Text, CultureInfo.CurrentUICulture);
                        //PXPM-7337  
                        //For Goose communication No two Transfer Breaker should have same value except 0 as not available value ,
                        //hence skiiping those situations by returning for Goose breaker ID setpoints
                        for (int i = 0; i < excludeValue.Length; i++)
                        {
                            if (excludeValue[i] == 0)
                            {
                                count = count + 1;
                            }
                        }
                        if ((count < 6 && temp == 0 && ((Settings)TripUnit.IDTable["GC00113"]).selectionValue == "Microgrid" && (ID == "GC00115" || ID == "GC00116" || ID == "GC00117" || ID == "GC00118" || ID == "GC00119" || ID == "GC00120" || ID == "GC00121" || ID == "GC00122"))
                            || (temp == 0 && ((Settings)TripUnit.IDTable["GC00114"]).selectionDefault == "0" && (ID == "GC00117" || ID == "GC00118" || ID == "GC00119" || ID == "GC00120" || ID == "GC00121" || ID == "GC00122"))) return;
                        for (int i = 0; i < excludeValue.Length; i++)
                        {
                            if (temp == excludeValue[i])
                            {
                                SetErrorState("Value is not in range, Export will not work");
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {


            }


        }

        //AK added function with same name but diff parameter. Scroll down
        private void SetErrorState(String _description)
        {
            ToolTip tooltipDescripter = new ToolTip();
            textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
            tooltipDescripter.StaysOpen = true;
            tooltipDescripter = ScreenCreator.createToolTip(min, max, stepsize, unit, _description);
            textBox.ToolTip = tooltipDescripter;
            tooltipDescripter.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
            valid = false;
        }




        public void SetupComboBox(ref ComboBox _comboBox, ref Label _labelUnit)
        {
            comboBox = _comboBox;
            label_Unit = _labelUnit;
            comboBox.SelectionChanged += comboBox_SelectionChanged;


        }

        public void UnsubscribeFromComboBoxSelectionChangedEvent(ref ComboBox comboBox)
        {
          
            comboBox.SelectionChanged -= comboBox_SelectionChanged;


        }
        public void SetupToggleButton(ref ToggleButton _togglebutton)
        {
            toggle = _togglebutton;      

            toggle.Checked += ToggleButton_Checked;
            toggle.Unchecked += ToggleButton_Checked;
        }
        public void ResetTimervalues()
        {
            incrementValue = 1;
            reduceTimer = 0;
        }
        // increase button code for continuous increment
        public void setupIncreaseButton(ref Button _incrbutton)
        {
            _incrbutton.Click += IncreaseValue;
            _incrbutton.PreviewMouseUp += _incrbutton_PreviewMouseUp;
            _incrbutton.PreviewMouseDown += _incrbutton_PreviewMouseDown;
        }

        private void _incrbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dispatcherIrTimer.Tick += dispatcherTimerIncIr_Tick;
            dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, FirstDelay);
            dispatcherIrTimer.Start();
        }

        private void _incrbutton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            dispatcherIrTimer.Tick -= dispatcherTimerIncIr_Tick;
            dispatcherIrTimer.Stop();
            ResetTimervalues();
        }

        private void dispatcherTimerIncIr_Tick(object sender, EventArgs e)
        {
            IncreaseValue(null, null);
            System.Threading.Thread.Sleep(100);
            incrementValue++;
            if (incrementValue % 2 == 0) // Increase value of reducetimer after 2 increments, which will lower the dispatcherTimer interval
            {
                reduceTimer = reduceTimer + ReduceTimerDelay;
            }
            if (FirstDelay - reduceTimer > 0)
            {
                dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, FirstDelay - reduceTimer);
            }
            else // Set minimum interval to 30 ms, if it goes below 0
            {
                dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, HighSpeedTimerDelay);
            }
        }
        public void IncreaseValue(object sender, RoutedEventArgs e)
        {
          
            if (stepsize > 0)
            { 
                double Value;
                try
                {
                    Value = Convert.ToDouble(textBox.Text, CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);
                    Value = min;
                }
                int mod = (int)(Value % stepsize);
                if (mod != 0 || Value < min || Value > max)
                {
                    do
                    {
                        if (Value > min && Value < max)
                        {
                            Value++;
                        }
                        if (Value < min)
                        {
                            Value = min;
                        }
                        if (Value > max)
                        {
                            Value=max;
                        }
                    }
                    while ((int)(Value % stepsize) != 0);
                   
                    Value = Value - stepsize;
                }
                //if (Value < min || Value > max || (mod != 0))
                //{
                //    Value = min;
                //}

                if (Value + stepsize <= max)
                {
                    textBox.Text = Global.updateValueonCultureBasis((Value + stepsize).ToString());
                    if (bcalculated)
                    {
                        string[] equation_string1 = calculation.Split(',');
                        if (equation_string1[0].Contains("="))
                        {
                            if (ID == "CPC018A" || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "CPC018") || ID == "CPC022")
                            {
                                label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                            }
                            else
                            {
                                label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(equation_string1[0]).ToString()) + " A";
                            }
                            if (equation_string1.Length > 1)
                            {
                                string dependent_id;
                                //Settings setpoint1 = new Settings();          //#COVARITY FIX    235014
                                int j = 1;
                                for (int i = j; i < equation_string1.Length; i++)
                                {
                                    dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                                    Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                                    if (setpoint1 != null && setpoint1.calculation != null)
                                    {
                                        setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                                    }
                                }
                            }
                        }
                        else
                        {
                            string dependent_id;
                            // Settings setpoint1 = new Settings();      //#COVARITY FIX     235014
                            int j;
                            if (equation_string1.Length == 1 || !(equation_string1[0].Contains("=")))
                            {
                                j = 0;
                            }
                            else
                            {
                                j = 1;
                            }
                            for (int i = j; i < equation_string1.Length; i++)
                            {
                                dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                                Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                                if (setpoint1 != null && setpoint1.calculation != null)
                                {
                                    setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                                }
                            }
                        }
                    }
                  
                }
            }
        }


        //decrease button code for continuous decrement
        public void setupDecreaseButton(ref Button _descbutton)
        {
            _descbutton.Click += DecreaseValue;
            _descbutton.PreviewMouseDown += _descbutton_PreviewMouseDown;
            _descbutton.PreviewMouseUp += _descbutton_PreviewMouseUp;
        }

        private void _descbutton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            dispatcherIrTimer.Tick -= dispatcherTimerDecIr_Tick;
            dispatcherIrTimer.Stop();
            ResetTimervalues();
        }

        private void _descbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dispatcherIrTimer.Tick += dispatcherTimerDecIr_Tick;
            dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, FirstDelay);
            dispatcherIrTimer.Start();
        }
       
        private void dispatcherTimerDecIr_Tick(object sender, EventArgs e)
        {
            DecreaseValue(null, null);
            System.Threading.Thread.Sleep(100);
            incrementValue++;
            if (incrementValue % 2 == 0) // Increase value of reducetimer after 2 increments, which will lower the dispatcherTimer interval
            {
                reduceTimer = reduceTimer + ReduceTimerDelay;
            }
            if (FirstDelay - reduceTimer > 0)
            {
                dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, FirstDelay - reduceTimer);
            }
            else // Set minimum interval to 30 ms, if it goes below 0
            {
                dispatcherIrTimer.Interval = new TimeSpan(0, 0, 0, 0, HighSpeedTimerDelay);
            }
        }


        public void DecreaseValue(object sender, RoutedEventArgs e)
        {
            if (stepsize > 0)
            {
                double Value;
                try
                {
                    Value = Convert.ToDouble(textBox.Text, CultureInfo.CurrentUICulture);
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);
                    Value = min;
                }


                 int mod = (int)(Value % stepsize);
                if (mod != 0||Value<min||Value>max)
                {
                    do
                    {

                        if (Value > min && Value < max)
                        {
                            Value--;
                        }
                        if (Value > max)
                        {
                            Value = max;
                        }
                        if (Value<min)
                        {
                            Value=min;
                        }

                    }
                    while ((int)(Value % stepsize) != 0);
                    
                    Value = Value + stepsize;
                }
                //if (Value < min || Value > max || (mod != 0))
                //{
                //    Value = min;
                //}
                if ((float)(Value - stepsize) >= min)
                {
                    textBox.Text = Global.updateValueonCultureBasis((Value - stepsize).ToString());
                    if (bcalculated)
                    {
                        string[] equation_string1 = calculation.Split(',');
                        if (equation_string1[0].Contains("="))
                        {
                            if (ID == "CPC018A" || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "CPC018") || ID == "CPC022")
                            {
                                label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                            }
                            else
                            {
                                label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(equation_string1[0]).ToString()) + " A";
                            }
                            if (equation_string1.Length > 1)
                            {
                                string dependent_id;
                                // Settings setpoint1 = new Settings();   //#COVARITY FIX  235063
                                int j = 1;
                                for (int i = j; i < equation_string1.Length; i++)
                                {
                                    dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                                    Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                                    if (setpoint1 != null && setpoint1.calculation != null)
                                    {
                                        setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                                    }
                                }
                            }
                        }
                        else
                        {
                            string dependent_id;
                            //Settings setpoint1 = new Settings();      //#COVARITY FIX     235063
                            int j;
                            if (equation_string1.Length == 1 || !(equation_string1[0].Contains("=")))
                            {
                                j = 0;
                            }
                            else
                            {
                                j = 1;
                            }
                            for (int i = j; i < equation_string1.Length; i++)
                            {
                                dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                                Settings setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                                if (setpoint1 != null && setpoint1.calculation != null)
                                {
                                    setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    textBox.Text = Global.updateValueonCultureBasis((Value).ToString());
                }
            }
        }

        public double CalculatedValue(string equation, string DrawFor = Global.str_app_ID_Table)
        {
            try
            {

                switch (DrawFor)
                {
                    case Global.str_app_ID_Table:
                        TripUnit.CurvesIDTable = TripUnit.IDTable;
                        break;
                    case Global.str_pxset1_ID_Table:
                        TripUnit.CurvesIDTable = TripUnit.Pxset1IDTable;
                        break;
                    case Global.str_pxset2_ID_Table:
                        TripUnit.CurvesIDTable = TripUnit.Pxset2IDTable;
                        break;
                }
                double result = 0;
                String[] get_Clamping = equation.Split('L');
                String[] get_equation = equation.Split(',');
                string[] equation_calculation = get_equation[0].Split('=');
                if (equation_calculation.Length > 1)
                {
                    string[] equation_left = equation_calculation[1].Trim().Split(' ');
                    int count = equation_left.Length;
                    if (equation_left[1] == "*")
                    {
                        double setting1, setting2;
                        Settings setpoint1 = new Settings();
                        if (equation_left[0].Contains("_"))
                        {
                            equation_left[0] = equation_left[0].Substring(0, equation_left[0].IndexOf('_'));
                        }
                        setpoint1 = (Settings)TripUnit.CurvesIDTable[equation_left[0]];

                        setting1 = Convert.ToDouble(setpoint1.numberValue, CultureInfo.CurrentUICulture);
                        Settings setpoint2 = new Settings();
                        if (!Double.TryParse(equation_left[2], out setting2))
                        {
                            if (equation_left[2].Contains("||"))
                            {

                                string[] arrayForVisibleSetpoint = equation_left[2].Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var item in arrayForVisibleSetpoint)
                                {
                                    Settings setting = new Settings();
                                    var set = item;
                                    if (set.Contains("_"))
                                    {
                                        set = set.Substring(0, set.IndexOf('_'));
                                    }
                                    setting = (Settings)TripUnit.CurvesIDTable[set];
                                    if (setting.visible)
                                    {
                                        setpoint2 = setting;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (equation_left[2].Contains("_"))
                                {
                                    equation_left[2] = equation_left[2].Substring(0, equation_left[2].IndexOf('_'));
                                }
                                setpoint2 = (Settings)TripUnit.CurvesIDTable[equation_left[2]];
                            }

                            if (setpoint2.type == Settings.Type.type_selection)
                            {
                                if (Regex.Replace(setpoint2.selectionValue, "[A-Za-z]", "").Trim() == "" || !(Regex.IsMatch(setpoint2.selectionValue, @"\d")))
                                {
                                    setting2 = 0;
                                }
                                else
                                {
                                    setting2 = Convert.ToDouble(Regex.Replace(setpoint2.selectionValue, "[A-Za-z]", "").Trim(), CultureInfo.CurrentUICulture);
                                }

                                String value = Regex.Replace(setpoint2.selectionValue, "[A-Za-z]", "").Trim();

                                //if(value == string.Empty)
                                //{
                                //    value = "0";
                                //}
                                //setting2 = Convert.ToDouble(value, CultureInfo.CurrentUICulture);
                            }
                            else
                            {
                                //if (setpoint2.label_calculation != null)
                                //{
                                //    setting2 = Convert.ToDouble(Regex.Replace(setpoint2.label_calculation.Content.ToString(), "[A-Za-z]", "").Trim(), CultureInfo.CurrentUICulture);
                                //}
                                //else
                                //{
                                setting2 = Convert.ToDouble(setpoint2.numberValue, CultureInfo.CurrentUICulture);
                                //  }
                            }
                        }


                        if (setpoint1.type == Settings.Type.type_selection)
                        {
                            if (Regex.Replace(setpoint1.selectionValue, "[A-Za-z]", "").Trim() == "" || !(Regex.IsMatch(setpoint1.selectionValue, @"\d")))
                            {
                                setting1 = 0;
                            }
                            else
                            {
                                setting1 = Convert.ToDouble(Regex.Replace(setpoint1.selectionValue, "[A-Za-z]", "").Trim(), CultureInfo.CurrentUICulture);
                            }
                        }
                        else
                        {
                            setting1 = Convert.ToDouble(setpoint1.numberValue, CultureInfo.CurrentUICulture);
                        }

                        if (equation_left.Length > 3 && equation_left[3] != null && equation_left[3].Contains("/100"))
                        {
                            result = (setting1 * setting2) / 100;
                        }
                        else
                        {
                            result = setting1 * setting2;
                        }
                        //  result = Math.Round(result, 2);
                    }
                    if (equation_left.Length > 3)
                    {
                        if (equation_left[3] == "*")
                        {
                            double setting3;
                            if (!Double.TryParse(equation_left[4], out setting3))
                            {
                                Settings setpoint3 = new Settings();
                                if (equation_left[4].Contains("_"))
                                {
                                    equation_left[4] = equation_left[4].Substring(0, equation_left[4].IndexOf('_'));
                                }
                                setpoint3 = (Settings)TripUnit.CurvesIDTable[equation_left[4]];

                                //setting3 = Convert.ToDouble(setpoint3.numberValue, CultureInfo.CurrentUICulture);

                                if (equation_left[4].Contains("||"))
                                {

                                    string[] arrayForVisibleSetpoint = equation_left[4].Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var item in arrayForVisibleSetpoint)
                                    {
                                        Settings setting = new Settings();
                                        var set = item;
                                        if (set.Contains("_"))
                                        {
                                            set = set.Substring(0, set.IndexOf('_'));
                                        }
                                        setting = (Settings)TripUnit.CurvesIDTable[set];
                                        if (setting.visible)
                                        {
                                            setpoint3 = setting;
                                            break;
                                        }
                                    }
                                }
                                else
                                {

                                    setpoint3 = (Settings)TripUnit.CurvesIDTable[equation_left[4]];
                                }


                                if (setpoint3.type == Settings.Type.type_selection)
                                {
                                    if (Regex.Replace(setpoint3.selectionValue, "[A-Za-z]", "").Trim() == "" || !(Regex.IsMatch(setpoint3.selectionValue, @"\d")))
                                    {
                                        setting3 = 0;
                                    }
                                    else
                                    {
                                        setting3 = Convert.ToDouble(Regex.Replace(setpoint3.selectionValue, "[A-Za-z]", "").Trim(), CultureInfo.CurrentUICulture);
                                    }
                                }
                                else
                                {
                                    setting3 = Convert.ToDouble(setpoint3.numberValue, CultureInfo.CurrentUICulture);
                                }
                            }
                            if (equation_left.Length > 5 && equation_left[5] != null && equation_left[5].Contains("/100"))
                            {
                                result = (result * setting3) / 100;
                            }
                            else
                            {
                                result = result * setting3;
                            }
                        }
                    }
                    if (equation_left.Length > 5)
                    {
                        if (equation_left[5] == "*")
                        {
                            double setting4;
                            Settings setpoint4 = new Settings();
                            if (equation_left[6].Contains("_"))
                            {
                                equation_left[6] = equation_left[6].Substring(0, equation_left[6].IndexOf('_'));
                            }
                            setpoint4 = (Settings)TripUnit.CurvesIDTable[equation_left[6]];

                            //  setting4 = Convert.ToDouble(setpoint4.numberValue, CultureInfo.CurrentUICulture);

                            if (equation_left[6].Contains("||"))
                            {

                                string[] arrayForVisibleSetpoint = equation_left[2].Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var item in arrayForVisibleSetpoint)
                                {
                                    Settings setting = new Settings();
                                    var set = item;
                                    if (set.Contains("_"))
                                    {
                                        set = set.Substring(0, set.IndexOf('_'));
                                    }
                                    setting = (Settings)TripUnit.CurvesIDTable[set];
                                    if (setting.visible)
                                    {
                                        setpoint4 = setting;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                setpoint4 = (Settings)TripUnit.CurvesIDTable[equation_left[6]];
                            }

                            if (setpoint4.type == Settings.Type.type_selection)
                            {
                                if (Regex.Replace(setpoint4.selectionValue, "[A-Za-z]", "").Trim() == "" || !(Regex.IsMatch(setpoint4.selectionValue, @"\d")))
                                {
                                    setting4 = 0;
                                }
                                else
                                {
                                    setting4 = Convert.ToDouble(Regex.Replace(setpoint4.selectionValue, "[A-Za-z]", "").Trim(), CultureInfo.CurrentUICulture);
                                }
                            }
                            else
                            {
                                setting4 = Convert.ToDouble(setpoint4.numberValue, CultureInfo.CurrentUICulture);
                            }

                            if (equation_left.Length > 6 && equation_left[7] != null && equation_left[7].Contains("/100"))
                            {
                                result = (result * setting4) / 100;
                            }
                            else
                            {
                                result = result * setting4;
                            }
                        }
                    }

                    result = Math.Round(result, 2);
                }
                if (emptyspace != null && emptyspace.Visibility == Visibility.Collapsed)
                {
                    emptyspace.Visibility = Visibility.Visible;
                }
                if (get_Clamping.Count() > 1)
                {
                    if (ID == "CPC022")
                    {
                        Settings GroundFaultPickup;
                        if (equation.Contains("CPC018A"))
                            GroundFaultPickup = TripUnit.getGroundFaultPickupNum();  
                        else
                            GroundFaultPickup = TripUnit.getGroundFaultPickupSel();

                        result = numberValue* GroundFaultPickup.CalculatedValue(GroundFaultPickup.calculation)/100;
                    }
                    else if (result > Convert.ToDouble(get_Clamping[1]))
                    {
                        result = Convert.ToDouble(get_Clamping[1]);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                throw;
            }           
        }

        public static string ToBin(int value, int len)
        {
            return (len > 1 ? ToBin(value >> 1, len - 1) : null) + "01"[value & 1];
        }


        public void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            bValue = (Boolean)((ToggleButton)sender).IsChecked;
            
            //string defaultValue = getTagInfoasString[0];
            //string ControlName = getTagInfoasString[1];

            try
            {
                if (type == Type.type_bNumber)
                {
                    ToggleButton button = (sender as ToggleButton);
                    TextBlock tBlock = button.Template?.FindName("detailsTextBlock", button) as TextBlock;
                    if ((Boolean)((ToggleButton)sender).IsChecked)
                    {

                        tBlock.Text = OnLabel;
                        textBox.IsEnabled = true;
                        textBox.IsReadOnly = false;
                    }
                    else
                    {
                        tBlock.Text = OffLabel;
                        string[] getTagInfoasStringOfText = (string[])(textBox.Tag);
                        //Added null for fixing coverity scan issue
                        if (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0)
                        {
                            string defaultValueodText = getTagInfoasStringOfText[0];
                            textBox.Text = Global.updateValueonCultureBasis(defaultValueodText);
                        }
                        textBox.IsEnabled = false;
                        textBox.IsReadOnly = false;
                    }
                    if (decreaseButton != null)
                    {
                        decreaseButton.IsEnabled = textBox.IsEnabled;
                        increaseButton.IsEnabled = textBox.IsEnabled;
                    }
                }
                else if (type == Type.type_toggle)
                {
                    ToggleButton button = (sender as ToggleButton);

                    //  string tgdefaultValue = getTagInfoasString[0];
                    string tgdefaultValue;
                    string tgNewValue;
                    
                    string tgdisplayFieldName = label_name.Content.ToString();
                    string[] getTagInfoasString = (string[])((ToggleButton)sender).Tag;
                    //Added null for fixing coverity scan issue
                    string tgcontrolName = (getTagInfoasString != null && getTagInfoasString.Count() > 0) ? getTagInfoasString[1] : string.Empty;                    var resKeyForOnLabel = ID + "OnLabel".ToString().Trim();
                    var resKeyForOffLabel = ID + "OffLabel".ToString().Trim();
                    if((ID == "GEN004"||ID== "SYS004B") && Global.IsOffline && Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        Global.updateDefaultRelayValuesPXR35();
                    }
                    if (Global.device_type == Global.ACB_PXR35_DEVICE && !Global.IsOffline)
                    {
                        TripUnit.CSRTULIST = ToBin((int)((Settings)TripUnit.IDTable["CC012"]).numberValue, 8);
                        TripUnit.CSTCPLIST = ToBin((int)((Settings)TripUnit.IDTable["CC016"]).numberValue, 8);
                    }
                    else if (Global.device_type == Global.ACB_PXR35_DEVICE && Global.IsOffline && !Global.IsOpenFile)
                    {
                        TripUnit.CSRTULIST = "00000000";
                        TripUnit.CSTCPLIST = "00000000";
                    }
                    else if (Global.device_type == Global.ACB_PXR35_DEVICE && Global.IsOffline && Global.IsOpenFile && Global.RTUTCPcount == 0)
                    {
                        Global.RTUTCPcount = 1;
                        TripUnit.CSRTULIST = string.Format("000{0}{1}{2}{3}{4}", TripUnit.RTU_b4, TripUnit.RTU_b3, TripUnit.RTU_b2, TripUnit.RTU_b1, TripUnit.RTU_b0);
                        TripUnit.CSTCPLIST = string.Format("000{0}{1}{2}{3}{4}", TripUnit.TCP_b4, TripUnit.TCP_b3, TripUnit.TCP_b2, TripUnit.TCP_b1, TripUnit.TCP_b0);
                    }
                    if (ID == "CC012A" || ID == "CC012B" || ID == "CC012C" || ID == "CC012D" || ID == "CC012E" || ID == "CC016A" || ID == "CC016B" || ID == "CC016C" || ID == "CC016D" || ID == "CC016E")
                    {
                        switch (ID)
                        {
                            case "CC012A":
                                bValueReadFromTripUnit = TripUnit.CSRTULIST[3] == '0' ? false : true;
                                break;
                            case "CC012B":
                                bValueReadFromTripUnit = TripUnit.CSRTULIST[4] == '0' ? false : true;
                                break;
                            case "CC012C":
                                bValueReadFromTripUnit = TripUnit.CSRTULIST[5] == '0' ? false : true;
                                break;
                            case "CC012D":
                                bValueReadFromTripUnit = TripUnit.CSRTULIST[6] == '0' ? false : true;
                                break;
                            case "CC012E":
                                bValueReadFromTripUnit = TripUnit.CSRTULIST[7] == '0' ? false : true;
                                break;
                            case "CC016A":
                                bValueReadFromTripUnit = TripUnit.CSTCPLIST[3] == '0' ? false : true; ;
                                break;
                            case "CC016B":
                                bValueReadFromTripUnit = TripUnit.CSTCPLIST[4] == '0' ? false : true; ;
                                break;
                            case "CC016C":
                                bValueReadFromTripUnit = TripUnit.CSTCPLIST[5] == '0' ? false : true; ;
                                break;
                            case "CC016D":
                                bValueReadFromTripUnit = TripUnit.CSTCPLIST[6] == '0' ? false : true; ;
                                break;
                            case "CC016E":
                                bValueReadFromTripUnit = TripUnit.CSTCPLIST[7] == '0' ? false : true; ;
                                break;
                            default:
                                break;
                        }
                    }
                    tgdefaultValue = bValueReadFromTripUnit ? Resources.Strings.Resource.ResourceManager.GetString(resKeyForOnLabel) : Resources.Strings.Resource.ResourceManager.GetString(resKeyForOffLabel);
                    if (((ToggleButton)sender).IsChecked == true)
                    {
                        tgNewValue = Resources.Strings.Resource.ResourceManager.GetString(resKeyForOnLabel);
                    }
                    else
                    {
                        tgNewValue = Resources.Strings.Resource.ResourceManager.GetString(resKeyForOffLabel);
                    }

                    if (SettingValueChange != null && ((ToggleButton)sender).Visibility == Visibility.Visible)
                    {

                        SettingValueChange(tgdisplayFieldName, Global.updateValueonCultureBasis(tgdefaultValue), tgNewValue, tgcontrolName, ((ToggleButton)sender).IsVisible,GroupID);
                        if (Global.setting_Changed)
                        {
                            commitedChange = false;

                        }
                    }

                    //TextBlock tBlockLeft = button.Template.FindName("LeftdetailsTextBlock", button) as TextBlock;
                    //TextBlock tBlockRight = button.Template.FindName("RightdetailsTextBlock", button) as TextBlock;
                    //if ((Boolean)((ToggleButton)sender).IsChecked)
                    //{

                    //    tBlockRight.Text = OnLabel;
                    //}
                    //else
                    //{
                    //    tBlockLeft.Text = OffLabel;
                    //}

                    if (button.Name == "tgl_SYS004B" || button.Name == "tgl_SYS4B")
                    {
                        if (Global.IsOffline)
                        {
                            if (bValue)
                            {
                                TripUnit.MM_b0 = '1';
                                //TripUnit.MM_b8 = '1';
                            }
                            else
                            {
                                TripUnit.MM_b0 = '0';
                                //TripUnit.MM_b8 = '0';
                            }
                        }
                        else
                        {
                            if (bValue)
                            {
                                TripUnit.MM_b0 = '1';
                            }
                            else
                            {
                                TripUnit.MM_b0 = '0';
                            }

                        }
                        //Following lines of code are added by Astha to save maintenance mode values selected by user
                        TripUnit.MM16bitStringForSave = string.Format("0000000{0}{1}{2}0000{2}{3}", TripUnit.MM_b8, TripUnit.MM_b7, TripUnit.MM_b2, TripUnit.MM_b1, TripUnit.MM_b0);

                        TripUnit.MMforExportForSave = Convert.ToInt32(TripUnit.MM16bitStringForSave, 2).ToString("X");

                        while (TripUnit.MMforExportForSave.Length < ("0000").Length)
                        {
                            TripUnit.MMforExportForSave = "0" + TripUnit.MMforExportForSave;
                        }
                    }

                    if(button.Name == "tgl_GC00112A" || button.Name == "tgl_GC00112B"|| button.Name == "tgl_GC00112C" || button.Name == "tgl_GC00112D" || button.Name == "tgl_GC00112E" || button.Name == "tgl_GC00112F" || button.Name == "tgl_GC00112G" || button.Name == "tgl_GC00112H")
                    {
                        switch (button.Name)
                        {
                            case "tgl_GC00112A":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b0 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b0 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b0 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b0 = '0';
                                    }
                                }
                                break;

                            case "tgl_GC00112B":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b1 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b1 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b1 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b1 = '0';
                                    }
                                }
                                break;

                            case "tgl_GC00112C":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b2 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b2 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b2 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b2 = '0';
                                    }
                                }
                                break;
                            case "tgl_GC00112D":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b3 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b3 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b3 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b3 = '0';
                                    }
                                }
                                break;
                            case "tgl_GC00112E":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b4 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b4 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b4 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b4 = '0';
                                    }
                                }
                                break;

                            case "tgl_GC00112F":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b5 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b5 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b5 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b5 = '0';
                                    }
                                }
                                break;

                            case "tgl_GC00112G":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b6 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b6 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b6 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b6 = '0';
                                    }
                                }
                                break;
                            case "tgl_GC00112H":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b7 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b7 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.GC_b7 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.GC_b7 = '0';
                                    }
                                }
                                break;
                        }
                        TripUnit.GC8bitString = string.Format("00000000{0}{1}{2}{3}{4}{5}{6}{7}", TripUnit.GC_b7, TripUnit.GC_b6, TripUnit.GC_b5, TripUnit.GC_b4, TripUnit.GC_b3, TripUnit.GC_b2, TripUnit.GC_b1, TripUnit.GC_b0);
                        //;
                        TripUnit.GCforExport = Convert.ToInt32(TripUnit.GC8bitString, 2).ToString("X").PadLeft(4, '0');
                    }
                    if(button.Name == "tgl_CC016A"|| button.Name == "tgl_CC016B" || button.Name == "tgl_CC016C" || button.Name == "tgl_CC016D" || button.Name == "tgl_CC016E")
                    {
                        switch (button.Name)
                        {
                            case "tgl_CC016E":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b0 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b0 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b0 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b0 = '0';
                                    }
                                }
                                break;

                            case "tgl_CC016D":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b1 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b1 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b1 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b1 = '0';
                                    }
                                }
                                break;

                            case "tgl_CC016C":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b2 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b2 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b2 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b2 = '0';
                                    }
                                }
                                break;
                            case "tgl_CC016B":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b3 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b3 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b3 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b3 = '0';
                                    }
                                }
                                break;
                            case "tgl_CC016A":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b4 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b4 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.TCP_b4 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.TCP_b4 = '0';
                                    }
                                }
                                break;

                           
                        }
                        TripUnit.TCP8bitString = string.Format("000{0}{1}{2}{3}{4}",TripUnit.TCP_b4, TripUnit.TCP_b3, TripUnit.TCP_b2, TripUnit.TCP_b1, TripUnit.TCP_b0);
                        //;
                        TripUnit.TCPforExport = Convert.ToInt32(TripUnit.TCP8bitString, 2).ToString("X").PadLeft(4, '0');
                    }

                    if (button.Name == "tgl_CC012A" || button.Name == "tgl_CC012B" || button.Name == "tgl_CC012C" || button.Name == "tgl_CC012D" || button.Name == "tgl_CC012E")
                    {
                        switch (button.Name)
                        {
                            case "tgl_CC012E":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b0 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b0 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b0 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b0 = '0';
                                    }
                                }
                                break;

                            case "tgl_CC012D":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b1 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b1 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b1 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b1 = '0';
                                    }
                                }
                                break;

                            case "tgl_CC012C":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b2 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b2 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b2 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b2 = '0';
                                    }
                                }
                                break;
                            case "tgl_CC012B":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b3 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b3 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b3 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b3 = '0';
                                    }
                                }
                                break;
                            case "tgl_CC012A":
                                if (Global.IsOffline)
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b4 = '1';
                                        //TripUnit.MM_b8 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b4 = '0';
                                        //TripUnit.MM_b8 = '0';
                                    }
                                }
                                else
                                {
                                    if (bValue)
                                    {
                                        TripUnit.RTU_b4 = '1';
                                    }
                                    else
                                    {
                                        TripUnit.RTU_b4 = '0';
                                    }
                                }
                                break;


                        }
                        TripUnit.RTU8bitString = string.Format("000{0}{1}{2}{3}{4}", TripUnit.RTU_b4, TripUnit.RTU_b3, TripUnit.RTU_b2, TripUnit.RTU_b1, TripUnit.RTU_b0);
                        //;
                        TripUnit.RTUforExport = Convert.ToInt32(TripUnit.RTU8bitString, 2).ToString("X").PadLeft(4, '0');
                    }
                    notifyDependents();

                    //Need to popuplate as found data after notifyDependents
                    if (GroupID == "00" || GroupID == "0")
                    {
                        Global.listGroupsAsFoundSetPoint.Clear();
                        MainScreen_ViewModel.PopulateAsFoundData();
                        //return;
                    }
                    Global.updateExpandersVisibility();

                    //remove all the dependents(if present) of short delay toggle from change summary if toggle is disabled
                    if (Global.device_type == Global.MCCBDEVICE && button.Name == "tgl_CPC080" && bValue == false)
                    {
                        var ShortDelaytimeoption = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "tgl_CPC090");
                        if(ShortDelaytimeoption!=null && ShortDelaytimeoption.Count()!=0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytimeoption.FirstOrDefault());

                        var ShortDelaySlope = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "tgl_CPC07");
                        if (ShortDelaySlope != null && ShortDelaySlope.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaySlope.FirstOrDefault());

                        var ShortDelaytime1 = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC091A");
                        if (ShortDelaytime1 != null && ShortDelaytime1.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytime1.FirstOrDefault());

                        var ShortDelaytime2 = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC091B");
                        if (ShortDelaytime2 != null && ShortDelaytime2.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytime2.FirstOrDefault());
                    }

                    if (Global.device_type == Global.MCCBDEVICE && button.Name == "tgl_CPC07" && bValue == false)
                    {
                        var ShortDelaytime2 = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC091B");
                        if (ShortDelaytime2 != null && ShortDelaytime2.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytime2.FirstOrDefault());

                        var ShortDelaytimeoption = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "tgl_CPC090");
                        if (ShortDelaytimeoption != null && ShortDelaytimeoption.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytimeoption.FirstOrDefault());
                    }

                    if (Global.device_type == Global.MCCBDEVICE && button.Name == "tgl_CPC07" && bValue == true)
                    {
                        var ShortDelaytime1 = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC091A");
                        if (ShortDelaytime1 != null && ShortDelaytime1.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytime1.FirstOrDefault());

                        var ShortDelaytime2 = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC091B");
                        if (ShortDelaytime2 != null && ShortDelaytime2.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytime2.FirstOrDefault());

                        var ShortDelaytimeoption = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "tgl_CPC090");
                        if (ShortDelaytimeoption != null && ShortDelaytimeoption.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(ShortDelaytimeoption.FirstOrDefault());
                    }
                    //Remove dependent GFT (if present) from change Sumary if parent source toggle GF slope is changed
                    if (Global.device_type == Global.MCCBDEVICE && button.Name == "tgl_CPC13" && bValue == false)
                    {
                        var GroundFaulttimeoption = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC151B");
                        if (GroundFaulttimeoption != null && GroundFaulttimeoption.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(GroundFaulttimeoption.FirstOrDefault());
                    }
                    //In case of NZM, since their is same GFT control for both flat and I2T slope, so no need to remove existing GFT item from ChangeSummary
                    if (button.Name == "tgl_CPC13" && ((Global.device_type == Global.MCCBDEVICE && bValue == true)))
                    {
                        var GroundFaulttimeoption = MainScreen_ViewModel.Changes.Where(x => x.ControlName == "txt_CPC151A");
                        if (GroundFaulttimeoption != null && GroundFaulttimeoption.Count() != 0)
                            MainScreen_ViewModel.Changes.Remove(GroundFaulttimeoption.FirstOrDefault());
                    }
                    if (Global.device_type == Global.NZMDEVICE)
                    {
                        NZMCurveCalculations.AddSCRNZMDataToCurve();
                    }
                    else
                    {
                        CurvesCalculation.AddScrDataToCurve();
                    }

                    //Fire the event - notifying all subscribers
                    if (CurveCalculationChanged != null)
                        CurveCalculationChanged(this, null);

                }
                else if (type == Type.type_bSelection)
                {
                    if (comboBox != null && comboBox.Items != null && comboBox.Items.Count != 0)
                    {
                        int selected_index;
                        if ((Boolean)((ToggleButton)sender).IsChecked)
                        {
                            string[] getTagInfoasStringOfText = (string[])(comboBox.Tag);

                            string defaultValueodText = (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0) ? getTagInfoasStringOfText[0] : string.Empty;
                            comboBox.SelectedValue = defaultValueodText;
                            comboBox.IsEnabled = true;
                            comboBox.IsReadOnly = false;
                            foreach (var cmbi in comboBox.Items.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == defaultValueodText))
                            {
                                selected_index = comboBox.SelectedIndex;
                                cmbi.IsSelected = true;
                            }

                        }
                        else
                        {
                            string[] getTagInfoasStringOfText = (string[])(comboBox.Tag);
                            string defaultValueodText = (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0) ? getTagInfoasStringOfText[0] : string.Empty;
                            comboBox.SelectedValue = defaultValueodText;
                            foreach (var cmbi in comboBox.Items.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == defaultValueodText))
                            {
                                selected_index = comboBox.SelectedIndex;
                                cmbi.IsSelected = true;
                            }
                            comboBox.IsEnabled = false;
                            comboBox.IsReadOnly = true;
                        }
                    }
                }
                notifyDependents();

                //Disable relay configuration button if relay config toggle is set to off for PXR35
                if (type == Type.type_toggle)
                {
                    ToggleButton button = (sender as ToggleButton);
                    if (ID == "SYS131B")
                    {
                        Expander _expItem = null;
                        _expItem = Global.FindParent_Expander<Expander>(button);
                        foreach (Control ctrl in Global.FindVisualChildren<Control>(_expItem))
                        {
                            if (ctrl.Name.Trim() == "btnl2_SYS131B")
                            {
                                ctrl.IsEnabled = button.IsChecked == true ? true : false;
                            }
                        }
                    }
                    else if (ID == "SYS141B")
                    {
                        Expander _expItem = null;
                        _expItem = Global.FindParent_Expander<Expander>(button);
                        foreach (Control ctrl in Global.FindVisualChildren<Control>(_expItem))
                        {
                            if (ctrl.Name.Trim() == "btnl2_SYS141B")
                            {
                                ctrl.IsEnabled = button.IsChecked == true ? true : false;
                            }
                        }
                    }
                    else if (ID == "SYS151B")
                    {
                        Expander _expItem = null;
                        _expItem = Global.FindParent_Expander<Expander>(button);
                        foreach (Control ctrl in Global.FindVisualChildren<Control>(_expItem))
                        {
                            if (ctrl.Name.Trim() == "btnl2_SYS151B")
                            {
                                ctrl.IsEnabled = button.IsChecked == true ? true : false;
                            }
                        }
                    }
                    //else if (ID == "SYS033")
                    //{ 
                    //     Expander _expItem = null;
                    //    _expItem = Global.FindParent_Expander<Expander>(button);
                    //    foreach (Control ctrl in Global.FindVisualChildren<Control>(_expItem))
                    //    {
                    //        if (ctrl.Name.Trim() == "btnl2_SYS025")
                    //        {
                    //            ctrl.IsEnabled = button.IsChecked == true ? true : false;
                    //        }
                    //    }
                    //}
                }

                Global.updateExpandersVisibility();
                if (Global.device_type == Global.NZMDEVICE)
                {
                    NZMCurveCalculations.AddSCRNZMDataToCurve();
                }
                else
                {
                    CurvesCalculation.AddScrDataToCurve();
                }

                if (CurveCalculationChanged != null)
                    CurveCalculationChanged(this, null);

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex.Message);
            }


        }
        public void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Global.setting_Changed = false;
                if (((ComboBox)sender) == null)
                    return;

                if (((ComboBoxItem)((ComboBox)sender).SelectedItem) == null)
                    return;

                selectionValue = (String)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content;
                //this fix is for MM mode test report Bug_PXPM_791, it should set default selection while setting selection value.
                if (ID == "SYS4A") defaultSelectionValue = selectionValue;
                selectionIndex = ((System.Windows.Controls.Primitives.Selector)sender).SelectedIndex;
                notifyDependents();
                if (comboBox == null)
                    return;
                if(comboBox.Name =="cmb_GEN1" && Global.IsOffline && Global.device_type == Global.NZMDEVICE )
                {
                    Expander _expItem = null;
                    _expItem = Global.FindParent_Expander<Expander>(comboBox);

                    Global.updateToggleForNZMOffline(_expItem, selectionIndex);
                }


                if (comboBox.Name == "cmb_SYS002" || comboBox.Name == "cmb_GEN002")
                {
                    if (comboBox.Name == "cmb_SYS002")
                    {
                        if (Global.IsOffline)
                        {
                            Global.GlbstrBreakerFrame = selectionValue;
                        }
                    }
                    if (comboBox.Name == "cmb_GEN002")
                    {
                        if (Global.IsOffline)
                        {
                            Global.appFirmware = selectionValue;
                            if (Global.GlbstrBreakerFrame == null)
                            {
                                Settings set = TripUnit.getBreakerFrame();
                                Global.GlbstrBreakerFrame = set.selectionValue;
                            }

                        }
                    }
                }
                if (comboBox.Name == "cmb_GEN01" && Global.IsOffline && Global.device_type == Global.MCCBDEVICE)
                {
                    Settings firmwareVersion;
                    if (selectionValue == Resource.GEN01Item0000)
                    {
                        Global.appFirmware = "NA";
                        firmwareVersion = TripUnit.getFirmwareVersionNum();
                        firmwareVersion.textStrValue = Global.appFirmware;
                    }
                    else
                    {
                         firmwareVersion = TripUnit.getFirmwareVersionSel();
                        Global.appFirmware = firmwareVersion.selectionValue;
                    }
                }

                if (comboBox.Name == "cmb_SYS12" || comboBox.Name == "cmb_SYS02")
                {
                    var LCDPreviewWindow = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.Name.Equals("LCDTextOrientationWin"));
                    if (LCDPreviewWindow != null)
                    {
                        if (lcdDataToCurveCollection != null)
                        {
                            lcdDataToCurveCollection();
                            LCDPreviewWindow.Topmost = true;
                            LCDPreviewWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        }
                        else
                        {
                            LCDPreviewWindow.Close();
                        }
                    }
                }
                if (comboBox.Name == "cmb_SYS001" || comboBox.Name == "cmb_SYS01")
                {
                    TripUnit.userRatingPlug = selectionValue;
                }
                if (comboBox.Name == "cmb_SYS002" || comboBox.Name == "cmb_SYS02")
                {
                    TripUnit.userBreakerInformation = selectionValue;
                }
                if (comboBox.Name == "cmb_GEN002" || comboBox.Name == "cmb_GEN02" || comboBox.Name == "cmb_GEN02A")
                {
                    if ((Global.IsOffline && Global.isnewfile == true) && Global.device_type == Global.MCCBDEVICE)
                    {
                        if (comboBox.IsVisible == true)
                            Global.appFirmware = selectionValue;
                    }
                    else
                        Global.appFirmware = selectionValue;

                }
                if (comboBox.Name == "cmb_SYS003"|| comboBox.Name == "cmb_SYS003A")
                {
                    TripUnit.userStyle = selectionValue;
                }
                if (comboBox.Name == "cmb_GEN002")
                {
                    Global.appFirmware = selectionValue;
                }
                //if (comboBox.Name == "cmb_SYS004B" || comboBox.Name == "cmb_SYS4B")
                //{
                //    if (Global.IsOffline)
                //    {
                //        if (selectionValue == Resource.SYS004BItem81 || selectionValue == Resource.SYS004BItem01 || selectionValue == Resource.SYS4BItem81 || selectionValue == Resource.SYS4BItem01)
                //        {
                //            TripUnit.MM_b0 = '1';
                //            TripUnit.MM_b8 = '1';
                //        }
                //        else
                //        {
                //            TripUnit.MM_b0 = '0';
                //            TripUnit.MM_b8 = '0';
                //        }
                //    }
                //    else
                //    {
                //        if (selectionValue == Resource.SYS004BItem81 || selectionValue == Resource.SYS004BItem01 || selectionValue == Resource.SYS4BItem81 || selectionValue == Resource.SYS4BItem01)
                //        {

                //            TripUnit.MM_b0 = '1';
                //            //      TripUnit.MM_b8 = '1';
                //            // if (comboBox.Name == "cmb_SYS004B" && TripUnit.MM_b8 == '1')
                //            //  {
                //            //Settings set = (Settings)TripUnit.IDTable["SYS004A"];
                //            //set.selectionValue = ((item_ComboBox)(set.lookupTable["01"])).item;
                //            //set.defaultSelectionValue = set.selectionValue;


                //            //((Group)(TripUnit.groups[1])).groupSetPoints[indexForMM].selectionValue = ((item_ComboBox)((Group)(TripUnit.groups[1])).groupSetPoints[indexForMM].lookupTable["01"]).item;
                //            //((Group)(TripUnit.groups[1])).groupSetPoints[indexForMM].defaultSelectionValue = ((Group)(TripUnit.groups[1])).groupSetPoints[indexForMM].selectionValue;
                //            //  }
                //            //  if (comboBox.Name == "cmb_SYS4B" && TripUnit.MM_b8 == '1')
                //            //  {
                //            //((Group)(TripUnit.groups[1])).subgroups[5].groupSetPoints[0].selectionValue = ((item_ComboBox)((Group)(TripUnit.groups[1])).subgroups[5].groupSetPoints[0].lookupTable["01"]).item;
                //            //((Group)(TripUnit.groups[1])).subgroups[5].groupSetPoints[0].selectionValue = ((Group)(TripUnit.groups[1])).subgroups[5].groupSetPoints[0].selectionValue;
                //            // }
                //        }
                //        else
                //        {
                //            TripUnit.MM_b0 = '0';
                //        }

                //    }
                    //Following lines of code are added by Astha to save maintenance mode values selected by user
                    TripUnit.MM16bitStringForSave = string.Format("0000000{0}{1}00000{2}{3}", TripUnit.MM_b8, TripUnit.MM_b7, TripUnit.MM_b1, TripUnit.MM_b0);

                    TripUnit.MMforExportForSave = Convert.ToInt32(TripUnit.MM16bitStringForSave, 2).ToString("X");

                    while (TripUnit.MMforExportForSave.Length < ("0000").Length)
                    {
                        TripUnit.MMforExportForSave = "0" + TripUnit.MMforExportForSave;
                    }


               // }

                //else
                //{
                    if (selectionValue != null && comboBox.Items != null && comboBox.Items.Count != 0)
                    {
                        foreach (var cmbi in comboBox.Items.Cast<ComboBoxItem>().Where(cmbi => (string)cmbi.Content == selectionValue))
                        {

                            cmbi.IsSelected = true;

                        }
                    }
             //   }




                if (bcalculated && visible)
                {
                    string[] equation_string1 = calculation.Split(',');
                    if (equation_string1[0].Contains("="))
                    {
                        if (ID == "CPC018A" || ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && ID == "CPC018") || ID == "CPC022")
                        {
                            label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(calculation).ToString()) + " A";
                        }
                        else if (label_calculation != null)
                        {
                            label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(equation_string1[0]).ToString()) + " A";
                        }
                    }
                    else
                    {
                        string dependent_id;
                        Settings setpoint1 = new Settings();
                        int j;
                        if (equation_string1.Length == 1 || !(equation_string1[0].Contains("=")))
                        {
                            j = 0;
                        }
                        else
                        {
                            j = 1;
                        }
                        for (int i = j; i < equation_string1.Length; i++)
                        {
                            dependent_id = Regex.Replace(equation_string1[i], "[{}]", string.Empty).Trim();
                            setpoint1 = (Settings)TripUnit.IDTable[dependent_id];
                            if (setpoint1 != null && setpoint1.calculation != null && setpoint1.label_calculation != null)
                            {
                                setpoint1.label_calculation.Content = Global.updateValueonCultureBasis(CalculatedValue(setpoint1.calculation).ToString()) + " A";
                            }
                        }
                    }

                }

                if ((string[])((ComboBox)sender).Tag != null)
                {
                    string[] getTagInfoasString = (string[])((ComboBox)sender).Tag;
                    string defaultValue = string.Empty;
                    string displayFieldName = string.Empty;
                    string ControlName = string.Empty;
                    //Added null for fixing coverity scan issue
                    if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                    {
                        defaultValue = getTagInfoasString[0];
                        displayFieldName = getTagInfoasString[1];
                        ControlName = getTagInfoasString[2];
                    }

                    if (SettingValueChange != null && ((ComboBox)sender).Visibility == Visibility.Visible)
                    {
                        if (ControlName != "cmb_SYS131" && ControlName != "cmb_SYS141" && ControlName != "cmb_SYS151" &&
                            ControlName != "cmb_SYS131A" && ControlName != "cmb_SYS141A" && ControlName != "cmb_SYS151A")
                        {
                            if (displayFieldName == String.Empty)
                            {
                                displayFieldName = description;
                            }
                            if (((ComboBox)sender).IsVisible)
                            {

                                SettingValueChange(displayFieldName, defaultValue, selectionValue, ControlName, ((ComboBox)sender).IsVisible,GroupID);
                                if (Global.setting_Changed)
                                {
                                    commitedChange = false;
                                }

                            }
                        }
                    }

                }
                Global.updateExpandersVisibility();              
                if (Global.device_type == Global.NZMDEVICE)
                {
                    NZMCurveCalculations.AddSCRNZMDataToCurve();
                }
                else
                {
                    CurvesCalculation.AddScrDataToCurve();
                }

                //Fire the event - notifying all subscribers
                if (CurveCalculationChanged != null)
                    CurveCalculationChanged(this, null);
            }
            catch(Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        //Todo:make this function generic when we write generic code, Added code to set default value for relay's second combobox by VishalN
       
        public void SetListBox(ref ListBox _listBox)
        {
            listBox = _listBox;
        }
        public ListBox getListBox()
        {
            return listBox;
        }


        public void SetupCheckBox(ref CheckBox _checkBox)
        {
            checkBox = _checkBox;
            checkBox.Checked += checkBox_Checked;
            checkBox.Unchecked += checkBox_Checked;
        }

        void checkBox_Checked(object sender, RoutedEventArgs e)
        {
          

            bValue = (Boolean)((CheckBox)sender).IsChecked;

            if ((string[])((CheckBox)sender).Tag != null)
            {
                string[] getTagInfoasString = (string[])((CheckBox)sender).Tag;
                //Added null for fixing coverity scan issue
                if (getTagInfoasString != null && getTagInfoasString.Count() > 0)
                {
                    string defaultValue = getTagInfoasString[0];
                    string ControlName = getTagInfoasString[1];

                    if (SettingValueChange != null && ((CheckBox)sender).Visibility == Visibility.Visible)
                    {

                        SettingValueChange(((CheckBox)sender).Content.ToString().Trim(), defaultValue, bValue.ToString(), ControlName, ((CheckBox)sender).IsVisible, GroupID);
                    }
                }
            }

            // This is the case of the BNumber because it has both a checkbox
            // and a text box that are part of the same setting. 
            if (type == Type.type_bNumber)
            {
                if ((Boolean)((CheckBox)sender).IsChecked)
                {
                    textBox.IsEnabled = true;
                    textBox.IsReadOnly = false;
                }
                else
                {
                    string[] getTagInfoasStringOfText = (string[])(textBox.Tag);
                    //Added null check to fix coverity scan issue
                    if (getTagInfoasStringOfText != null && getTagInfoasStringOfText.Count() > 0)
                    {
                        string defaultValueodText = getTagInfoasStringOfText[0];
                        textBox.Text = Global.updateValueonCultureBasis(defaultValueodText);
                    }
                    textBox.IsEnabled = false;
                    textBox.IsReadOnly = true;
                }
            }
            notifyDependents();
            Global.updateExpandersVisibility();
        }





        // Global Variables
        public static Settings setLongDelaySlope, setLongDelayPickup, setLongDelayTime, setShortDelaySlope, setShortDelayPickup, setShortDelayTime;
        public static double setLongDelayPickupSelectionValue, setShortDelayTimeSelectionValue, setShortDelayPickupSelectionValue, setLongDelayTimeSelectionValue, setLongDelaySlopeSelectionValue, setShortDelaySlopeSelectionValue, setLongDelayCurve;
        public static double longDelayPickupX1, longDelayPickupY1, longDelayPickupX2, longDelayPickupY2, longDelayTimeX1, longDelayTimeY1, longDelayTimeZ1, longDelayTimeX2, longDelayTimeY2, longDelayTimeZ2, shortDelayTimeY1, shortDelayTimeY2, shortDelayTimeX2, shortDelayTimeX1, shortDelayTimeZ1, shortDelayTimeZ2, shortDelayTimeFlatY1, shortDelayTimeFlatY2, shortDelayTimeFlatX2, shortDelayTimeFlatX1, shortDelayPickupX1, shortDelayPickupX2, shortDelayPickupY1, shortDelayPickupY2;
        public static string LongDelaySlopeSelectionValue = string.Empty;

        public static bool CoordinationType;

        public void setupSubnetMask(ref TextBox _aligntxtIP002)
        {
            _aligntxtIP002.TextChanged += getSubnetAddressFromIPNetMask;
       }
        public void getSubnetAddressFromIPNetMask(object sender, RoutedEventArgs e)
        {
            try
            {
                string netMask = ((System.Windows.Controls.TextBox)(sender)).Text;
                string subNetMask = string.Empty;
                int calSubNet = 0;
                double result = 0;
                if (!string.IsNullOrEmpty(netMask))
                {
                    calSubNet = 32 - Convert.ToInt32(netMask);
                    if (calSubNet >= 0 && calSubNet <= 8)
                    {
                        for (int ipower = 0; ipower < calSubNet; ipower++)
                        {
                            result += Math.Pow(2, ipower);
                        }
                        double finalSubnet = 255 - result;
                        subNetMask = "255.255.255." + Convert.ToString(finalSubnet);
                    }
                    else if (calSubNet > 8 && calSubNet <= 16)
                    {
                        int secOctet = 16 - calSubNet;
                        // if (secOctet >=8)
                        //{
                        secOctet = 8 - secOctet;
                        // }
                        for (int ipower = 0; ipower < secOctet; ipower++)
                        {
                            result += Math.Pow(2, ipower);
                        }
                        double finalSubnet = 255 - result;
                        subNetMask = "255.255." + Convert.ToString(finalSubnet) + ".0";
                    }
                    else if (calSubNet > 16 && calSubNet <= 24)
                    {
                        int thirdOctet = 24 - calSubNet;

                        thirdOctet = 8 - thirdOctet;

                        for (int ipower = 0; ipower < thirdOctet; ipower++)
                        {
                            result += Math.Pow(2, ipower);
                        }
                        double finalSubnet = 255 - result;
                        subNetMask = "255." + Convert.ToString(finalSubnet) + ".0.0";
                    }
                    else if (calSubNet > 24 && calSubNet <= 32)
                    {
                        int fourthOctet = 32 - calSubNet;

                        fourthOctet = 8 - fourthOctet;

                        for (int ipower = 0; ipower < fourthOctet; ipower++)
                        {
                            result += Math.Pow(2, ipower);
                        }
                        double finalSubnet = 255 - result;
                        subNetMask = Convert.ToString(finalSubnet) + ".0.0.0";
                    }
                }

                Settings displaySetting =TripUnit.getEthernetCAMSubnetMask();            
                displaySetting.label_calculation.Visibility = Visibility.Visible;
                displaySetting.label_calculation.Content = subNetMask;
               
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);

                throw;
            }
           



            //displaySetting.IPAddressControl.Text = subNetMask;
            // return subNetMask;
        }
        
       public void ShowChangeActiveSetScreen(ref Button _alignbutton)
        {
            _alignbutton.Click += ShowChangeActiveSetScreenValue;
        }

        public void ShowChangeActiveSetScreenValue(object sender, RoutedEventArgs e)
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

        public void setupLCDTextOrientationButton(ref Button _alignbutton)
        {
            _alignbutton.Click += LCDTextOrientationValue;
        }

        // Event handler for LCD Text Orientation button click
        public void LCDTextOrientationValue(object sender, RoutedEventArgs e)
        {

            Settings setUnitType  = TripUnit.getTripUnitType();
            Settings setFrameType = TripUnit.getBreakerFrame();
            Settings setOrientation = TripUnit.getLCDTextOrientation();

            int UnitTypeSelIndex  = setUnitType.selectionIndex;
            int FrameTypeSelIndex = setFrameType.selectionIndex;
            int LCDOrientation = setOrientation.selectionIndex;

            LCDPreview winLCDPreview = new LCDPreview();
            winLCDPreview.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            winLCDPreview.showPreview(FrameTypeSelIndex, LCDOrientation);
            
        }


        public void ShowRelayScreen(ref Button _alignbutton)
        {
            _alignbutton.Click += ShowRelayScreenValue;
        }
        public void ShowRelayScreenValue(object sender, RoutedEventArgs e)
        {


            Scr_RelayConfig winRelay = new Scr_RelayConfig((sender as Button).Name);

            winRelay.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            winRelay.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            //winRelay.WindowState = System.Windows.WindowState.Maximized;
            winRelay.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            winRelay.ResizeMode = ResizeMode.NoResize;
            winRelay.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            winRelay.ShowActivated = true;
            Nullable<bool> result = winRelay.ShowDialog();
            if(result == true)
            {
                this.RelayChangeSummary(winRelay, sender);
            }
        }

        private void RelayChangeSummary(Scr_RelayConfig winRelay, object sender)
        {
            var relayVmObj = winRelay.DataContext as RelayConfigViewModel;
            var relayName = (sender as Button).Name;
            var relayChange = relayVmObj.set_RelaySettingValueChange();
            string setpointName = string.Empty;

            if (relayChange)
            {
                if (relayName == "btnl2_SYS131B")
                {
                    setpointName = Resource.Relay1ChangeSummary;
                }
                else if (relayName == "btnl2_SYS141B")
                {
                    setpointName = Resource.Relay2ChangeSummary;
                }
                else
                {
                    setpointName = Resource.Relay3ChangeSummary;
                }
            }

            if (SettingValueChange != null && ((Button)sender).Visibility == Visibility.Visible)
            {

                SettingValueChange(setpointName, "", "", relayName, ((Button)sender).IsVisible, GroupID);
                if (Global.setting_Changed)
                {
                    commitedChange = false;

                }
            }
        }
    }
}
