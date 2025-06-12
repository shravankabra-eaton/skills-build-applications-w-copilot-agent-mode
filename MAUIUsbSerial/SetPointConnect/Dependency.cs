using System;
using System.Globalization;
using System.Collections;
using System.Windows;
using PXR.Screens;
using PXR.Resources.Strings;
using System.Collections.Generic;
using PXR;
using System.Linq;
using System.Text.RegularExpressions;
using PXR.Properties;

namespace PXR
{
    /// <summary>
    /// Date: 3-28-13
    /// Author: Sarah M. Norris
    /// Description:
    /// Single Dependencies are for settings that depend on another setting
    /// This class updates the setting based on a set of conditions and
    /// attributes list. 
    /// </summary>
    class SingleDependency
    {
        public ArrayList ConditionList = new ArrayList();
        public String ID = string.Empty;
        /// <summary>
        /// After a dependency update is triggered this method finds all
        /// conditions associated with the dependent settting and updates
        /// the appropriate attribute. 
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="settingDependency"></param>
        public void ApplyDependency(ref Settings setting, List<Settings> settingDependency,Hashtable listOfSettings)
        {
            try
            {
                foreach (Condition condition in ConditionList)
                {
                    if (condition.CheckCondition(settingDependency, listOfSettings))
                    {
                        foreach (Attribute attribute in condition.AttributeList)
                        {
                            attribute.SetSettingAttributesFromAttributeList(ref setting);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                
            } 
          
        }
        
    }
    /// <summary>
    /// Date: 3-28-13
    /// Author: Sarah M. Norris
    /// Description:
    /// MutliDependency is for settings that depend on multiple settings
    /// This is an AND condition. Or conditions can be met using Single Dependencies
    /// This class updates the setting based on a set of conditions and
    /// attributes list. 
    /// </summary>
    //class MultiDependency
    //{
    //    // Stores the single dependencies this multidependency is dependent on. 
    //    public ArrayList Dependencies = new ArrayList();
    //    public ArrayList Attributes = new ArrayList();
    //    /// <summary>
    //    /// AND type dependency. If you need an OR condition you'll have to partner a
    //    /// single depdency with a multidependency.
    //    /// </summary>
    //    /// <returns></returns>
    //    private Boolean CheckDependencies()
    //    {
    //        foreach (SingleDependency dependency in Dependencies)
    //        {
    //            Settings settingDependency = (Settings)TripUnit.IDTable[dependency.ID];

    //            if (!((Condition)dependency.ConditionList[0]).CheckCondition(settingDependency))
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
        /// <summary>
        /// Date: 3-28-13
        /// Author: Sarah M. Norris
        /// Update the setting based on the attribute
        /// </summary>
        /// <param name="setting"></param>
    //    public void ApplyDependency(ref Settings setting)
    //    {
    //        if (CheckDependencies()) // check all conditions are met. 
    //        {
    //            foreach (Attribute attribute in Attributes)
    //            {
    //                attribute.SetSettingAttributesFromAttributeList(ref setting);
    //            }
    //        }
    //    }
    //}
    /// <summary>
    /// Date: 3-22-13
    /// Author: Sarah M. Norris
    /// Description:
    /// This class stores conditions that this dependency is looking for
    /// Many of the setting have multiple conditions and must be able 
    /// to look for the right combination of condition and attributes
    /// by storing the dependent attributes within a condition
    /// the class looks like the XML sheet. 
    /// </summary>
    class Condition
    {
        public ArrayList AttributeList = new ArrayList();       
        public Boolean CondBValue;
        public Double CondNumber;
        public string CondDescription;
        public List<String> CondItem=new List<String>();       
        public string dependency_valuemap;

        /// <summary>
        /// Date: 3-22-13
        /// Author: Sarah M. Norris
        /// Description:
        /// Each condition is checked against the dependency value
        /// Used only for single dependencies
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        /// Modified By Astha to handle dependencies with respect to dropdowns
        /// wherein only specific items of combobox will be visible on certain
        /// combination selection and condition mentioned in single dependency tag of xml
        public Boolean CheckCondition(List<Settings> dependencies, Hashtable listofSettings)
        {
            try
            {
                int value = 1;//This variable is added to check AND operation result   
                Settings dependency;
                string IDNameGivenInDependency;
                List<int> results = new List<int>();
                for (int i = 0; i < dependencies.Count; i++)
                {
                    dependency = dependencies[i];
                    IDNameGivenInDependency = listofSettings[dependency].ToString();                    
                    int flag = 0;
                    //Added by Astha to enable and disbale dropdown on basis of toggle button state ON/OFF
                    if (IDNameGivenInDependency.EndsWith("_1") || (dependency.type == Settings.Type.type_bNumber) /*|| IDNameGivenInDependency.EndsWith("_2")*/)
                    {
                        dependency_valuemap = dependency.bValue.ToString().ToLower();
                    }
                    else if (dependency.type == Settings.Type.type_toggle)
                    {
                        dependency_valuemap = dependency.value_map[Convert.ToString(dependency.bValue).ToLower()].ToString();
                    }
                    else
                    {
                        if (dependency.reverseLookupTable.Count > 0 && dependency.selectionValue != null && dependency.selectionValue != String.Empty)
                        { dependency_valuemap = dependency.reverseLookupTable[dependency.selectionValue].ToString(); }

                    }
                    if (dependency.type == Settings.Type.type_bool)
                    {
                        if (dependency.bValue == CondBValue)
                        {
                            return true;
                        }
                    }
                    else if (dependency.type == Settings.Type.type_selection)
                    {
                        if(CondItem[i].Contains('-'))
                        {
                         //   var conditionItems = CondItem[i].Split('|').ToList();
                            var conditionItems=CondItem[i].Split('|').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                            foreach (var item in conditionItems)
                            {
                                if (item.Contains('-'))
                                {
                                    var key = dependency.indexesWithHexValuesMapping.FirstOrDefault(x => x.Value == dependency.selectionValue).Key;
                                    var hexValueWithKey = (dependency_valuemap + "-" + key).Trim();
                                    if (item.Equals(hexValueWithKey))
                                    {
                                        dependency_valuemap = hexValueWithKey;
                                    }
                                }

                                if (item.Equals(dependency_valuemap))/* || (String.Equals(CondItem[i], "null")) || String.Equals(CondItem[i], "NA"))*/ // this allows us to use the N-frame and R-frame in this case. 
                                {
                                    flag = 1;
                                    break;
                                }

                            }

                        }
                        else if (dependency_valuemap != null && ((!CondItem[i].Contains('-')) && CondItem[i].Contains(dependency_valuemap)) || (String.Equals(CondItem[i], "null")) || String.Equals(CondItem[i], "NA")) // this allows us to use the N-frame and R-frame in this case. 
                        {
                            flag = 1;
                        }
                        else if ((TripUnit.userStyle != null) && (String.Equals(CondItem[i], TripUnit.userStyle)))
                        {
                            flag = 1;
                        }
                        results.Add(flag);

                    }
                    else if (dependency.type == Settings.Type.type_bNumber||dependency.type== Settings.Type.type_toggle)
                    {
                        if (CondItem[i].Contains(dependency_valuemap) || (String.Equals(CondItem[i], "null")) || String.Equals(CondItem[i], "NA") || String.Equals(CondItem[i],dependency.bValue))
                        {
                            flag = 1;
                        }
                        results.Add(flag);
                    }
                    else if (dependency.type == Settings.Type.type_number)
                    {
                        // By AG for PXPM-6845  #Current THD Alarm Pickup, Voltage THD Alarm Pickup, Operation Count off setting is not getting reflected in PXPM.
                        //if (dependency.numberValue == CondNumber)
                        //{
                        //    MessageBox.Show(string.Format(Resource.DependencyNumberValue, dependency.ID));
                        //    return true;
                        //}

                        // fetch the conditions and check set of values in condtions and set the flag to true when the nuber values are included in condition list
                        if (CondItem[i].Contains("|"))
                        {
                            var conditionItems = CondItem[i].Split('|').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            if (conditionItems.Length > 1)
                            {
                                int max = conditionItems.Length;
                                for (i = 0; i < max; i++)
                                {
                                    if (double.Parse(conditionItems[i]) == dependency.numberValue)
                                    {
                                        flag = 1;
                                    }
                                }
                                results.Add(flag);
                            }
                        }
                        // fetch the conditions and set min and max value and set the flag to true when the nuber values falls between range
                        else if (CondItem[i].Contains("-"))
                        {
                            var conditionItems = CondItem[i].Split('-').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            if (conditionItems.Length == 2)
                            {
                                var min = double.Parse(conditionItems[0]);
                                var max = double.Parse(conditionItems[1]);
                                if (min <= dependency.numberValue && dependency.numberValue <= max)
                                {
                                    flag = 1;
                                }
                                results.Add(flag);
                            }
                        }
                        // this will cover the condition of single value
                        else if (CondItem[i] != "null")
                        {
                            var conditionItems = CondItem[i].Split('-').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            if (conditionItems.Length == 1)
                            {
                                if (double.Parse(CondItem[i]) == dependency.numberValue)
                                {
                                    flag = 1;
                                }
                                results.Add(flag);
                            }
                        }

                    }
                    else if (dependency.type == Settings.Type.type_bSelection)
                    {
                        if (String.Equals(CondItem[i], dependency_valuemap) || CondItem[i].Contains(dependency_valuemap) || (String.Equals(CondItem[i], "null")) || String.Equals(CondItem[i], "NA"))
                        {
                            flag = 1;
                        }
                        results.Add(flag);
                    }                  
                }


                if (results != null)
                {
                    bool allTrue = results.All(i => i == value);
                    if (allTrue)
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
                return false;
            }
            

        
    }
    /// <summary>
    /// Date: 3-22-13
    /// Author: Sarah M. Norris
    /// Description:
    /// This class stores the attributes that will be affected when a condition is met. 
    /// Because the attributes only have 3 overall types I have lumped the similar
    /// types into type: String, Bool, and Double to simplify.
    /// 
    /// The attr_type string will pickout whether the attribute is a readonly or a visible
    /// attribute and the same goes for the double for min, max, stepsize
    /// </summary>
    class Attribute
    {
        private String _attrType;
        private Boolean _attrBValue;
        private Double _attrNumber;
        private String _attrItem;
        private String _attrLabel;
        private String _attrMincalculation;
        private String _attrExcludedValue;
        private String _attrMaxcalculation;
        private String _attrCalculatedValue;
        private String _attrDescription;

        /// <summary>
        /// calculated value expression
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bValue"></param>
        public void set_attrCalculatedValue (String type, string CalculatedValue)
        {
            _attrType = type;
            _attrCalculatedValue = CalculatedValue;
        }

        /// <summary>
        /// description value expression
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public void set_attrDescription(String type, string Description)
        {
            _attrType = type;
            _attrDescription = Regex.Replace(Description, @"[^0-9a-zA-Z]+", "");
        }

        /// <summary>
        /// ReadOnly condition, Visible condition
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bValue"></param>
        public void set_bValue(String type, Boolean bValue)
        {
            _attrType = type;
            _attrBValue = bValue;
        }

        /// <summary>
        /// _attr Excludedvalue expression 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        public void set_attrExcludedvalue(String type, String expression)
        {
            _attrType = type;
            _attrExcludedValue = expression;
        }

        /// <summary>
        /// min calculation expression 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        public void set_attrMincalculation(String type, String expression)
        {
            _attrType = type;
            _attrMincalculation = expression;
        }

        /// <summary>
        /// max calculation expression 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        public void set_attrMaxcalculation(String type, String expression)
        {
            _attrType = type;
            _attrMaxcalculation = expression;
        }

        /// <summary>
        /// Min, Max, StepSize Conditions
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        public void set_number(String type, Double number)
        {
            _attrType = type;
            _attrNumber = number;
        }
        /// <summary>
        /// selection conditions
        /// </summary>
        /// <param name="type"></param>
        /// <param name="item"></param>
        public void set_item(String type, String item)
        {
            _attrType = type;
            _attrItem = item;
        }
        public void set_label(String type, String item)
        {
            _attrType = type;            
            _attrLabel = Regex.Replace(item, @"[^0-9a-zA-Z]+", "");


        }
        public void set_SubGroupName(String type, String item)
        {
            _attrType = type;
            _attrLabel = Regex.Replace(item, @"[^0-9a-zA-Z]+", "");

        }
        /// <summary>
        /// Date: 3-22-13
        /// Author: Sarah M. Norris
        /// Determines what setting value will be changed and sets the corresponding
        /// attribute within the setting. 
        /// </summary>
        /// <param name="setting"></param>
        /// Modified by Astha to handle dependencies and dropdown values
        public void SetSettingAttributesFromAttributeList(ref Settings setting)
        {
            Hashtable ForSelectedLookupdata = new Hashtable();
            Hashtable IndexesWithHexValues = new Hashtable();
            List<String> settingvaluemap=new List<String>();
            ArrayList KeysOfLookupTable = new ArrayList();
            //setting.listofItemsToDisplay = null;
            String hexvalueOfAnItemFromValueMap = string.Empty;
            try
            {

                if (_attrType == Global.str_READONLY)
                {
                    if (setting.onlineReadOnly == true && !Global.IsOffline)
                    {
                        setting.readOnly = true;

                    }
                    else
                    {
                        setting.readOnly = _attrBValue;
                    }

                }
                else if (_attrType == Global.str_VISIBLE)
                {
                    setting.visible = _attrBValue;
                }
                else if (_attrType == Global.str_bnumbervisible)
                {
                    setting.bnumbervisible = _attrBValue;
                }
                else if (_attrType == Global.str_ENABLE)
                {
                    setting.isEnabled = _attrBValue;
                }
                else if (_attrType == Global.str_MIN)
                {
                    setting.min = Convert.ToDouble(Global.updateValueonCultureBasis(_attrNumber.ToString()), CultureInfo.CurrentUICulture);

                }
                else if (_attrType == Global.str_MAX)
                {
                    setting.max = Convert.ToDouble(Global.updateValueonCultureBasis(_attrNumber.ToString()), CultureInfo.CurrentUICulture);
                }
                else if (_attrType == Global.str_STEPSIZE)
                {
                    setting.stepsize = Convert.ToDouble(Global.updateValueonCultureBasis(_attrNumber.ToString()), CultureInfo.CurrentUICulture);
                }
                else if (_attrType == Global.str_ITEM)
                //Following code is added by Astha to handle the values shown in combobox for offline
                //and online mode for setpoint SYS004A and SYS004B
                {
                    if (Global.IsOffline)
                    {
                        hexvalueOfAnItemFromValueMap = setting.value_map[_attrItem].ToString();
                        item_ComboBox itemOfCombobox = (item_ComboBox)setting.lookupTable[hexvalueOfAnItemFromValueMap];
                        setting.selectionValue = itemOfCombobox.item;
                    }

                }
                else if (_attrType == Global.str_BOOL)
                {
                    setting.bValue = _attrBValue;
                }
                else if (_attrType == Global.str_LOOKUPDATA)
                {
                    if (setting.ID != "SYS005")
                    {
                        if (setting.listofItemsToDisplay != null && setting.listofItemsToDisplay.Count > 0)
                        {
                            setting.listofItemsToDisplay.Clear();
                        }
                        setting.ItemsToDisplayfromLookupTable.Clear();
                        setting.ItemsToDisplayfromLookupTable = (Hashtable)setting.lookupTable.Clone();
                        setting.listofItemsToDisplay = _attrItem.Split(',').ToList();
                        foreach (var item in setting.listofItemsToDisplay)
                        {
                            IndexesWithHexValues = (Hashtable)Global.valuemap[setting.ID];
                            if (!IndexesWithHexValues.ContainsKey(item))
                            {
                                foreach (var key in IndexesWithHexValues.Keys)
                                {
                                    string[] values = key.ToString().Split(',');
                                    bool isPresent = values.Contains(item);
                                    if (isPresent)
                                    {
                                        settingvaluemap.Add(IndexesWithHexValues[key].ToString());
                                    }

                                }
                            }
                            else
                            {
                                settingvaluemap.Add(IndexesWithHexValues[item].ToString());
                            }
                        }
                        foreach (var item in setting.lookupTable.Keys)
                        {
                            KeysOfLookupTable.Add(item);
                        }
                        foreach (var item in KeysOfLookupTable)
                        {
                            if (!settingvaluemap.Contains(item))
                            {
                                setting.ItemsToDisplayfromLookupTable.Remove(item);
                            }
                        }
                    }
                }
                else if (_attrType == Global.str_CONVERSION)
                {
                    setting.numberValue = setting.numberDefault * _attrNumber;
                    setting.setTextBox_text(setting.numberValue.ToString(CultureInfo.InvariantCulture));
                }
                else if (_attrType == Global.str_DEFAULT)
                {
                    if (_attrNumber != setting.numberDefault && setting.min == setting.max && setting.type == Settings.Type.type_bNumber)
                    {
                        setting.numberDefault = (double)_attrNumber;
                    }
                    if (!(setting.numberDefault >= setting.min && setting.numberDefault <= setting.max))
                    {
                        setting.numberDefault = (double)_attrNumber;
                    }
                    if (setting.type == Settings.Type.type_toggle)
                    {
                        //if (_attrBValue != setting.bDefault)
                        //{
                        setting.bDefaultAfterDependencyApplied = (Boolean)_attrBValue;
                        //setting.bDefaultAfterDependencyApplied = setting.bDefault;
                        // setting.bValue = (Boolean)_attrBValue;
                        // Added this conditon to fix ZSI False Warning issue where we need to execute this code for devices other ACB 3.0 & 3.1
                        // regardless of any other condition.
                        //PXPM-5776 Random test failures
                        //PXPM-5777 False ZSI warning message for Non - ZSI, ZSI disabled devices
                        if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && Global.IsOffline && !Global.IsOpenFile && !Global.isExportControlFlow)
                        {
                            setting.bValue = (bool)setting.bDefaultAfterDependencyApplied;
                            setting.bValueReadFromTripUnit = setting.bValue;
                        }
                        else if (!(Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE))
                        {
                            setting.bValue = (bool)setting.bDefaultAfterDependencyApplied;
                            setting.bValueReadFromTripUnit = setting.bValue;
                        }
                    }
                    if (setting.type == Settings.Type.type_selection)
                    {
                        if (_attrNumber != Int32.Parse(setting.selectionDefault))
                        {
                            setting.selectionDefault = _attrNumber.ToString();
                            setting.selectionDefaultAfterDependencyApplied = setting.selectionDefault;
                            if (setting.selectionDefaultAfterDependencyApplied != null && setting.ID == "SYS4A")
                            {
                                var valueOfValuemap = setting.value_map[setting.selectionDefault].ToString();
                                var defaultItem = (item_ComboBox)setting.lookupTable[valueOfValuemap];

                                setting.defaultSelectionValue = defaultItem.item;
                                setting.selectionValue = setting.defaultSelectionValue;
                            }
                        }
                    }

                }
                else if (_attrType == Global.str_NOTAVAILABLE)
                {
                    if (Global.IsOffline && (setting.ID == "SYS004A"  || setting.ID == "SYS4A"))
                    {
                        setting.notAvailable = _attrBValue;
                        // setting.bValue =false;
                        //setting.bDefault = false;
                        //if (setting.toggle != null)
                        //{
                        //   // setting.un(ref setting.toggle);
                        //    setting.toggle.IsChecked = false;
                        //}
                    }

                    setting.notAvailable = _attrBValue;
                    if (setting.notAvailable)
                        setting.numberValue = setting.numberDefault;
                    //if (!(setting.ID == "CPC018A" && !Global.IsOffline)) setting.numberValue = setting.numberDefault;
                    setting.readOnly = true;
                }
                else if (_attrType == Global.str_LABEL)
                {
                    var groupID = setting.GroupID;
                    var settingName = setting.name;
                    var settingID = setting.ID;
                    var labelNameAfterDependency = Resources.Strings.Resource.ResourceManager.GetString(_attrLabel);
                    var query = from Group gp in TripUnit.groups
                                where (gp.ID == groupID && gp.sequence != null)
                                select gp;

                    foreach (Group gp in query)
                    {
                        // Chnging names in gp.sequence is applicable for only group setpoints not the setpoints present under subgroup . 
                        //SO aaditional check is added to identify whether that setpint is group setpoint (using -setpointIndex)
                        var setpointIndex = Array.FindIndex<Settings>(gp.groupSetPoints, setpoint => setpoint.ID == settingID);

                        var index = Array.FindIndex<string>(gp.sequence, row => row.Contains(settingName));

                        if (index != -1 && setpointIndex!= -1)
                        {
                           gp.sequence[index] = labelNameAfterDependency;
                        }

                    }
                    setting.name = labelNameAfterDependency;
                }
                else if (_attrType == Global.str_DESCRIPTION)
                {
                    setting.description = Resources.Strings.Resource.ResourceManager.GetString(_attrDescription); 
                }
                else if (_attrType == Global.str_mincalculation)
                {
                    setting.MinCalculation = _attrMincalculation;
                }
                else if (_attrType == Global.str_maxcalculation)
                {
                    setting.MaxCalculation = _attrMaxcalculation;
                }
                else if (_attrType == Global.str_CalculatedValue)
                {
                    setting.calculation = _attrCalculatedValue;
                }
                else if (_attrType == Global.str_ExcludedValue)
                {
                    setting.ExcludedValue = _attrExcludedValue;
                }
            }
            catch (Exception ex)
            {
                string temp = setting.name;
                LogExceptions.LogExceptionToFile(ex);

            }
           
        }
    
    }
}
