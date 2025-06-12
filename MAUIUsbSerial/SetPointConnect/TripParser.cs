using System;

// ArrayList
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

// Regex
using System.Text.RegularExpressions;

// ListBox
using System.Windows.Controls;
using PXR.Resources.Strings;

using System.Globalization;
using System.Configuration;
using System.Threading;
using System.Text;
using System.Windows;
using PXR.Screens;

namespace PXR
{
    public static class TripParser
    {
        /// <summary>
        /// Used to parse the input file. Stores the settings in setPoint.
        /// </summary>
        public static void parseOutputFile(String filePath_inputFile)
        {
            // Read in trip output
            StreamReader reader = new StreamReader(filePath_inputFile);
            try
            {

                while (reader.Peek() != -1)
                {
                    // Match match = Regex.Match(input, pattern);
                    MatchCollection matches = Regex.Matches(reader.ReadLine(), @"(?<=x)\w{4}");
                    for (int i = 0; i < matches.Count; i++)
                    {
                        TripUnit.rawSetPoints.Add(matches[i].Value);
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                reader.Close();                         //#COVARITY FIX    234992
                reader.Dispose();
            }
        }

        /// <summary>
        /// Parses input from the relay. Application reads in all of the group setpoint
        /// in the form of 0x00XX 0x00XX and we need them as 00XX so this function removes
        /// ' ', '>', and '0x' from the raw input string
        /// </summary>
        /// <param name="rawInputString">String of form '> 0x#### 0x####</param>
        /// <param name="strDelimiter"></param>
        public static void ParseInputString(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            TripUnit.rawSetPoints.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            string mappingString = rawInputString;
            // string replaceNewLineInputString = rawInputString.Replace('\n', ' ');    //#COVARITY FIX   234955
            string replaceNewLineInputString = rawInputString.Replace('\n', strDelimiter);
            replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointList = replaceNewLineInputString.Split(strDelimiter);

            for (int i = 0; i < setpointList.Length; i++)
            {
                if (setpointList[i].Trim().Length > 1)
                {
                    if (Global.IsOffline)
                    {
                        TripUnit.rawSetPoints.Add(setpointList[i].Trim());
                    }
                    else
                    {
                        var setPointval = setpointList[i].Trim().Substring(1);
                        TripUnit.rawSetPoints.Add(setPointval.Trim());
                    }
                }
            }

        }

        public static void ParseInputStringForConnect(String rawInputString, char strDelimiter)
        {
            try
            {
                string[] listofStrings = rawInputString.Split('\n');

                if (Global.device_type_PXR10 && !(Global.device_type == Global.NZMDEVICE) && !Global.isDemoMode)
                {
                    rawInputString = listofStrings[0] + "\n" + listofStrings[1];
                    rawInputString = rawInputString + "\n 0000 0000 0000 0000\n 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000";
                    rawInputString = rawInputString + "\n 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000";


                    if (Global.GlbstrMotor == Resource.GEN12Item0001 && TripUnit.deviceShortDelay == Resource.GEN12Item0001)
                    {
                        //  rawInputString = rawInputString + "\n 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000";
                        rawInputString = rawInputString + "\n" + listofStrings[2];
                        if ((listofStrings.Count() - 1) >= 3)
                        {
                            rawInputString = rawInputString + "\n" + listofStrings[3];
                        }
                    }
                    else if (Global.GlbstrMotor == Resource.GEN12Item0000 && TripUnit.deviceShortDelay == Resource.GEN12Item0001)
                    {
                        rawInputString = rawInputString + "\n" + listofStrings[2];
                        if ((listofStrings.Count() - 1) >= 3) // when short delay is enabled 
                        {
                            rawInputString = rawInputString + "\n" + listofStrings[3];

                        }
                    }
                    else if (Global.GlbstrMotor == Resource.GEN12Item0001 && TripUnit.deviceShortDelay == Resource.GEN12Item0000)
                    {
                        rawInputString = rawInputString + "\n" + listofStrings[2];
                        if ((listofStrings.Count() - 1) >= 3) // when short delay is enabled 
                        {
                            rawInputString = rawInputString + "\n" + listofStrings[3];

                        }
                    }
                    else if (Global.GlbstrMotor == Resource.GEN12Item0000 && TripUnit.deviceShortDelay == Resource.GEN12Item0000)
                    {
                        rawInputString = rawInputString + "\n" + listofStrings[2];
                        rawInputString = rawInputString + "\n 0000 0000 0000 0000";
                    }
                }


                TripUnit.rawSetPoints.Clear();
                Global.tripUnitData = string.Empty;
                MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
                string replaceNewLineInputString = rawInputString.Replace('\r', ' ');
                string mappingString = replaceNewLineInputString;
                Global.tripUnitData = mappingString;
                replaceNewLineInputString = rawInputString.Replace('\n', ' ');
                replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
                string[] setpointList = replaceNewLineInputString.Split(strDelimiter);             
                foreach (string t1 in setpointList)
                {
                    if (t1.Trim().Length > 0)
                    {
                        var setPointval = t1;
                        TripUnit.rawSetPoints.Add(setPointval);
                    }
                }
                if ((Global.device_type == Global.MCCBDEVICE && Global.selectedTemplateType == Global.MCCBTEMPLATE) ||
                    (Global.device_type == Global.NZMDEVICE && Global.selectedTemplateType == Global.NZMTEMPLATE) ||
                    ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE) && Global.selectedTemplateType == Global.ACB3_0TEMPLATE)||
                    (Global.device_type == Global.ACB_PXR35_DEVICE && Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) ||
                     (Global.device_type == Global.PTM_DEVICE && Global.selectedTemplateType == Global.PTM_TEMPLATE))
                {
                    CreateGroupMap(mappingString, strDelimiter);
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);

              //  throw;
            }           
        }
        /// <summary>
        /// MCCB trip unit mapping
        /// </summary>
        /// <param name="replaceNewLineInputString"></param>
        /// <param name="strDelimiter"></param>
        private static void CreateGroupMap(string replaceNewLineInputString, char strDelimiter)
        {
            Char delimiter = '\n';
            int setPointCounter = 0;
            int groupCounter = 0;
            var GroupCnt = 1;
            string[] setpointNameStartsWith = { "SYS", "CPC", "CCC", "CAM", "IO", "MPC","PXR10","CPC0","Relay","CC" ,"GOS","ATS","GOS1"};
            Global.grouplist.Clear();
            String[] substrings = replaceNewLineInputString.Split(delimiter);
            List<string[]> grpArray = new List<string[]>();
            try
            {
                foreach (var item in substrings)
                {
                    string[] grp = item.Split(strDelimiter);
                    List<string> group = grp.ToList<string>();
                    group.RemoveAll(string.IsNullOrEmpty);
                    grp = group.ToArray();
                    grpArray.Add(grp);
                }

                foreach (var Group in grpArray)
                {
                    groupCounter = 0;
                    foreach (string setpoint in Group)
                    {
                        string sys = setpointNameStartsWith[GroupCnt - 1];

                        Global.grouplist.Add(new TripUnitMapper()
                        {
                            SetPointId = sys + groupCounter,
                            SetPointValue = Group[groupCounter],
                            TripUnitPos = setPointCounter,
                            GroupId = "0" + GroupCnt.ToString()
                        });
                        groupCounter++;
                        setPointCounter++;

                    }
                    GroupCnt++;
                }
            }
            catch(Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        } 
         
        public static void ParseDataSentByUser(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            TripUnit.setpointsDataSentbyUser.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            // string replaceNewLineInputString = rawInputString.Replace('\r', ' ');              //#COVARITY FIX   234911 
            string replaceNewLineInputString = rawInputString.Replace('\r', ' ').Replace('\n', ' ');

            // replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointList = replaceNewLineInputString.Split(strDelimiter);

            //string setPointval = "";  //#COVARITY FIX   234889,234920

            for (int i = 0; i < setpointList.Length; i++)
            {
                if (setpointList[i].Trim().Length > 0)
                {
                    string setPointval = string.Empty;
                    if ((i == 194 || i == 199 || i == 203) && Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        setPointval = setpointList[i].Trim();
                    }
                    else
                    {
                        setPointval = setpointList[i].Trim().Substring(1);
                    }
                    TripUnit.setpointsDataSentbyUser.Add(setPointval);
                }
            }
        }

        public static void ParseDataWrittenToUnit(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            TripUnit.setPointDataWrittenToUnit.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            string replaceNewLineInputString = rawInputString.Replace('\r', ' ').Replace('\n', ' '); //#COVARITY FIX        235019
            //replaceNewLineInputString = rawInputString.Replace('\n', ' ');

            // replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointList = replaceNewLineInputString.Split(strDelimiter);

            // string setPointval = "";//#COVARITY FIX  235029

            for (int i = 0; i < setpointList.Length; i++)
            {
                if (setpointList[i].Trim().Length > 0)
                {
                    string setPointval = string.Empty;
                    if ((i == 194 || i == 199 || i == 203) && Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        setPointval = setpointList[i].Trim();
                    }
                    else
                    {
                        setPointval = setpointList[i].Trim().Substring(1);
                    }
                    TripUnit.setPointDataWrittenToUnit.Add(setPointval);
                }
            }
        }

        public static void ParseInputStringForBackupData(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            TripUnit.rawSetPointsBackupData.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            string replaceNewLineInputString = rawInputString.Replace('\r', ' ').Replace('\n', ' ');        ////#COVARITY FIX  
             // replaceNewLineInputString = rawInputString.Replace('\n', ' ');

            replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointList = replaceNewLineInputString.Split(strDelimiter);
            /*  for (int i = 0; i < matches.Count; i++)
               {
                   TripUnit.rawSetPoints.Add(matches[i].Value);
               }*/

            // string setPointval = "";          //#COVARITY FIX     235029

            for (int i = 0; i < setpointList.Length; i++)
            {
                if (setpointList[i].Trim().Length > 0)
                {
                    string setPointval = setpointList[i].Trim().Substring(1);
                    TripUnit.rawSetPointsBackupData.Add(setPointval);
                }
            }
        }

        public static void ParseInputStringForTestReportAsFound(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            //  TripUnit.TestReportrawSetPointsAsFound.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            string replaceNewLineInputString = rawInputString.Replace('\n', strDelimiter);
            //  replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointListAsFound = replaceNewLineInputString.Split(strDelimiter);

            string setPointval = "";

            for (int i = 0; i < setpointListAsFound.Length; i++)
            {
                if (setpointListAsFound[i].Trim().Length > 1)
                {
                    setPointval = setpointListAsFound[i].Trim().Substring(1);
                    //       TripUnit.TestReportrawSetPointsAsFound.Add(setPointval);
                }
            }
        }

        public static void ParseInputStringForTestReportAsLeft(String rawInputString, char strDelimiter)
        {
            //  Match match = Regex.Match(input, pattern);
            //  TripUnit.TestReportrawSetPointsAsLeft.Clear();
            MatchCollection matches = Regex.Matches(rawInputString, @"(?<=x)\w{4}");
            string replaceNewLineInputString = rawInputString.Replace('\n', strDelimiter);
            //  replaceNewLineInputString = replaceNewLineInputString.Remove(replaceNewLineInputString.LastIndexOf(strDelimiter));
            string[] setpointListAsLeft = replaceNewLineInputString.Split(strDelimiter);

            string setPointval = "";

            for (int i = 0; i < setpointListAsLeft.Length; i++)
            {
                if (setpointListAsLeft[i].Trim().Length > 1)
                {
                    setPointval = setpointListAsLeft[i].Trim().Substring(1);
                    //        TripUnit.TestReportrawSetPointsAsLeft.Add(setPointval);
                }
            }
        }

        public static void GetStyleRatingPlugDetails()
        {
            var styleResults = (from row in (Global.dtPXRStyle).AsEnumerable()
                                where
                                row.Field<string>("StyleName") == TripUnit.userStyle
                                select row);
            if (!Global.IsOffline && ! Global.isDemoMode)
            {
                BreakerFramInfo objBreakerFram = new BreakerFramInfo();
                objBreakerFram.ReadBreakerCurrent();
                objBreakerFram.ReadBreakerCatlogNumber();
            }
            if (styleResults.Count() == 1)
            {
                if (TripUnit.ResponseForUnitType != null)
                {
                    switch (TripUnit.ResponseForUnitType)
                    {
                        case "1"://IEC
                            Global.GlbstrUnitType = Resource.SYS000Item0001;
                            break;
                        case "2"://ANSI
                            Global.GlbstrUnitType = Resource.SYS000Item0002;
                            break;
                        case "3"://UL489
                            Global.GlbstrUnitType = Resource.SYS000Item0003;
                            break;
                        case "4"://CCC
                            Global.GlbstrUnitType = Resource.SYS000Item0004;
                            break;
                        default:
                            Global.GlbstrUnitType = Resource.SYS000Item0001;
                            break;
                    }
                }
               // Global.GlbstrUnitType = TripUnit.ResponseForUnitType != null && TripUnit.ResponseForUnitType == "1" ? Resource.SYS000Item0001 : Resource.SYS000Item0002;

              //  Global.GlbstrUnitType = Resource.ResourceManager.GetString((styleResults.ElementAt(0)).Field<string>(1));
                Global.GlbstrSensing = Resource.ResourceManager.GetString((styleResults.ElementAt(0)).Field<string>(2));
                Global.GlbstrGFP = Resource.ResourceManager.GetString((styleResults.ElementAt(0)).Field<string>(3));
                if(Global.device_type != Global.PTM_DEVICE)
                    Global.GlbstrARMS = Resource.ResourceManager.GetString((styleResults.ElementAt(0)).Field<string>(4));
                Global.GlbstrModBus = Resource.ResourceManager.GetString((styleResults.ElementAt(0)).Field<string>(5));
                Global.unitTypeSelectionValue = Global.GlbstrUnitType;
            }
            //Commented by Astha as there is no Ratingplug.xml now .It has been merged and dependencies are added in the xml itself
            //var ratingPlugresults = (from row in (Global.dtRatingPlug).AsEnumerable()
            //                         where
            //                         row.Field<string>("RatingPlug") == TripUnit.userRatingPlug
            //                         && row.Field<string>("UnitType") == (styleResults.ElementAt(0)).Field<string>(1)
            //                         select row);

            //if (ratingPlugresults.Count() == 1)
            //{
            //    Global.FrameTypeSelectionValue = (ratingPlugresults.ElementAt(0)).Field<string>(0);
            //    Global.RPlugSelectionValue = (ratingPlugresults.ElementAt(0)).Field<string>(2);

            //Global.GlbstrCurrentRating = Global.RPlugSelectionValue;
            Global.GlbstrCurrentRating = TripUnit.userRatingPlug;
            Global.GlbstrBreakerFrame = TripUnit.userBreakerInformation;
            //}
        }

        private static ArrayList _convertedSetPoints;    // Stores the setpoint that have been converted from Decimal to Hex
        private static ArrayList _convertedSetPoint_List;
        private static ArrayList _backUpconvertedSetPoints;

        /// <summary>
        /// Get the setpoints in Hex format.
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetConvertedSetPoints()
        {

            return _convertedSetPoints;
        }

        /// <summary>
        /// Get the setpoints in Hex format.
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetBackUpConvertedSetPoints()
        {

            return _backUpconvertedSetPoints;
        }

        public static ArrayList GetConvertedPointstoIndex()
        {
            return _convertedSetPoint_List;
        }

        public static Boolean ConvertScreenInfoIntoSettingRequirements()
        {
            try
            {
                string hexMaintMode = string.Empty;
                bool isSave = true;
                bool isACBRead = !Global.IsOffline && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE);
                bool isACB3_0Read = !Global.IsOffline && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE);
                bool isMCCBRead = !Global.IsOffline && Global.device_type == Global.MCCBDEVICE;
                bool isNZMRead = !Global.IsOffline && Global.device_type == Global.NZMDEVICE;
                _convertedSetPoints = new ArrayList();
                _convertedSetPoint_List = new ArrayList();  //for saving the data in the csv file  by Ashish
                _backUpconvertedSetPoints = new ArrayList();
                StringBuilder commandString = new StringBuilder();
                StringBuilder setpointIDString = new StringBuilder();
                StringBuilder backUpComandString = new StringBuilder();
                String hex_Value = null;  // hex value of set points from screen by Ashish
                String index_Value = null; // index value from selected hex value by Ashish
                string strReturnVal = string.Empty; ;                        //int i = 0;
                String valueForMaintenanceMode = string.Empty;
                String valueForMaintenanceModeRemote = string.Empty;
                String CombinedValueForMaintenanceMode = string.Empty;
                var iterationGroups = TripUnit.groups;
                // read through each group
                Int16 setPointCounter = 0;
                Global.IterationList = new List<Settings>();
                foreach (var settingID in TripUnit.ID_list)
                {
                    Settings setting = (Settings)TripUnit.IDTable[settingID];
                    Global.IterationList.Add(setting);
                }
                foreach (Group group in iterationGroups)
                {
                    // Condition for PXR 10 device to bypass general and unit group             
                    //if (!Global.isSaveFile)
                    //{
                    //    //if (Global.device_type == Global.ACB_PXR35_DEVICE && (!(group.ID == "1" || group.ID == "002"))) continue;   
                    //     if (Global.device_type == Global.ACB_PXR35_DEVICE && (group.ID == "003")) continue; //Communication group is yet to be impilimented
                    //}
                var groupIterator = Global.IterationList.Where(x => x.GroupID == group.ID).ToList();
                    foreach (Settings setting in groupIterator)
                    {
                        try
                        {                            
                            if (!setting.isValid())
                            {
                                if (!setting.visible && setting.type == Settings.Type.type_number)
                                {
                                    setting.valid = true;
                                    setting.numberValue = setting.min;
                                    setting.numberDefault = setting.min ;
                                }
                                else if (!setting.visible && setting.type == Settings.Type.type_bNumber)
                                {
                                    setting.valid = true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            
                                if ((( Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && !Global.isSaveFile) && (setting.ID == "SYS131C" || setting.ID == "SYS141C" || setting.ID == "SYS151C" || setting.ID == "SYS131B" || setting.ID == "SYS141B" || setting.ID == "SYS151B" || setting.ID == "SYS13" || setting.ID == "SYS14"))
                            { continue; }

                            if (((isACBRead || Global.selectedTemplateType == Global.ACBTEMPLATE) && !Global.isSaveFile) && (setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT"))
                            { continue; }


                            if (setting.ID == "CPC041")
                            {
                                string temp = setting.numberValue.ToString();
                            }
                            if (setting.type == Settings.Type.type_selection)
                            {
                                strReturnVal = string.Empty;

                               
                                if (setting.ID == "SYS000" || setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"
                                    || setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A")   //for MCCB Save Open

                                {
                                    strReturnVal = ConvertSelectionToHex(setting);
                                    if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                                    {
                                        hex_Value = strReturnVal.Trim();
                                    }

                                    if (setting.reversevalue_map[hex_Value] != null)
                                    {
                                        index_Value = setting.reversevalue_map[hex_Value].ToString();
                                        _convertedSetPoint_List.Add(index_Value);
                                    }

                                    continue;
                                }
                                else
                                {
                                    strReturnVal = ConvertSelectionToHex(setting);

                                                                          

                                    if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                                    {
                                        hex_Value = strReturnVal.Trim();
                                        if (strReturnVal.Contains(','))
                                        {
                                            var values = strReturnVal.Split(',');
                                            switch (setting.ID)
                                            {
                                                case "SYS02":
                                                    //if ((Global.IsOffline && !Global.isExportControlFlow && !Global.IsOpenFile)
                                                    //    || ((Global.isExportControlFlow || Global.IsOpenFile) && TripUnit.PD3PD3HHexValue == null)) 
                                                    if (Global.IsOffline && TripUnit.PD3PD3HHexValue == null)
                                                    {   // in offline no need to check value form device get value for PD3 based on rating 
                                                        //value only and then compare with device settings 
                                                        if (TripUnit.getRating().selectionValue.Contains('H'))
                                                        {
                                                            strReturnVal = "0017";
                                                        }
                                                        else
                                                        {
                                                            strReturnVal = "0016";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var items = from m in values where m == TripUnit.PD3PD3HHexValue select m;
                                                        strReturnVal = items.FirstOrDefault();
                                                    }
                                                    break;
                                                case "GEN10":
                                                    if (Global.IsOffline/* && TripUnit.tripUnitPoleHexValue == null*/)
                                                    {
                                                        strReturnVal = "0003";
                                                    }
                                                    else if (Global.isDemoMode && !Global.IsOffline)
                                                    {// in demo mode TripUnit.tripUnitPoleHexValue value of pole is not set henc assignng 0 value for 0 and 3 pole , it is not applicable for offline mode 

                                                        strReturnVal = values[0];
                                                    }
                                                    else
                                                    {
                                                        var items = from m in values where m == TripUnit.tripUnitPoleHexValue select m;
                                                        strReturnVal = items.FirstOrDefault();
                                                    }
                                                    break;
                                            }
                                        }

                                        //For ACB 3.0 multiple relay assignment , 2 relay values are taken from high byte and low byte of
                                        //single value henc integrating those to values again for write 

                                         if (setting.ID == "SYS013A" || setting.ID == "SYS014A" || setting.ID == "SYS015A")
                                        {                                          
                                           
                                            char[] relay1 = new char[4];
                                            commandString.CopyTo(commandString.Length - 5, relay1,0 , 4);
                                            commandString.Remove(commandString.Length - 5, 5);
                                            char[] relay2 = strReturnVal.ToCharArray();
                                            char[] integratedrelay = new char[4];
                                            integratedrelay[2] = relay1[2];
                                            integratedrelay[3] = relay1[3];
                                            integratedrelay[0] = relay2[2];
                                            integratedrelay[1] = relay2[3];

                                            strReturnVal = new string(integratedrelay);

                                            if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null)
                                                Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = strReturnVal;
                                        }

                                        commandString.Append(strReturnVal.Trim() + " "); //Comma removed AK
                                        setpointIDString.Append(setting.ID + " ");
                                    }
                                }
                            }
                            else if (setting.type == Settings.Type.type_toggle)
                            {
                                if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                                {
                                    commandString.Append(ConvertSplit(setting) + " ");//Comma removed AK
                                    setpointIDString.Append(setting.ID + " ");
                                    //if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                    //{
                                    //    backUpComandString.Append(ConvertSplit(setting) + " ");//Comma removed AK
                                    //if (TripUnit.MMforExport.Trim() == "0000" && setting.bValue)
                                    //    CombinedValueForMaintenanceMode = "0101";
                                    //else
                                    CombinedValueForMaintenanceMode = TripUnit.MMforExport.Trim() == string.Empty ? "0000" : TripUnit.MMforExport;

                                    valueForMaintenanceMode = CombinedValueForMaintenanceMode.Substring(0, 2);
                                    valueForMaintenanceModeRemote = CombinedValueForMaintenanceMode.Substring(2);
                                   
                                    Global.valueForMaintenanceModeRemote(ref valueForMaintenanceModeRemote, valueForMaintenanceMode);

                                    hex_Value = valueForMaintenanceMode;
                                }
                                //added by srk to fix PXPM-6171
                                else if (setting.ID == "SYS004B" || setting.ID == "SYS4B" ||
                                    setting.ID == "SYS004C" || setting.ID == "SYS4C" ||
                                    setting.ID == "SYS004D" || setting.ID == "SYS4D" ||
                                    setting.ID == "SYS004E" || setting.ID == "SYS4E" ||
                                setting.ID == "SYS004F" || setting.ID == "SYS4F" ||
                                 setting.ID == "CPC025"|| setting.ID == "CPC026" || setting.ID == "CPC027"||setting.ID=="SYS024"|| setting.ID == "GC00112A" || setting.ID == "GC00112B" || setting.ID == "GC00112C" ||
                                setting.ID == "GC00112D" || setting.ID == "GC00112E" || setting.ID == "GC00112F" || setting.ID == "GC00112G" || setting.ID == "GC00112H" || setting.ID == "CC012A" || setting.ID == "CC012B" || setting.ID == "CC012C" || setting.ID == "CC012D" || setting.ID == "CC012E"
                                || setting.ID == "CC016A" || setting.ID == "CC016B" || setting.ID == "CC016C" || setting.ID == "CC016D" || setting.ID == "CC016E")

                                {

                                    //hex_Value = valueForMaintenanceModeRemote;
                                    //while (hex_Value.Length < ("0000").Length)
                                    //{
                                    //    hex_Value = "0" + hex_Value;
                                    //}
                                    //  index_Value = (setting.reversevalue_map.Contains(hex_Value)) ? setting.reversevalue_map[hex_Value].ToString() : "0";
                                    index_Value = (setting.bValue) ? "true" : "false";
                                    _convertedSetPoint_List.Add(index_Value);
                                    continue;
                                }
                                else
                                {
                                    commandString.Append(ConvertToggleSelectionToHex(setting) + " ");//Comma removed AK
                                    setpointIDString.Append(setting.ID + " ");
                                    hex_Value = ConvertToggleSelectionToHex(setting);
                                    index_Value = setting.bValue.ToString().ToLower();
                                    //var bValue = index_Value == "0.067" || index_Value == "0" ? true : false;
                                    //index_Value = bValue + "," + index_Value;
                                }
                            }
                            // if number use the conversion factor to convert
                            // then convert to hex
                            // make sure it's in format 0x00
                            else if (setting.type == Settings.Type.type_number)
                            {
                                string[] valueseperator = null;

                                if (setting.ID == "GC00112")
                                {
                                    commandString.Append(ConvertSplit_GC(setting));
                                    hex_Value = ConvertSplit_GC(setting);
                                }
                                if (setting.ID == "CC012")
                                {
                                    commandString.Append(ConvertSplit_RTU(setting));
                                    hex_Value = ConvertSplit_RTU(setting);
                                }
                                if (setting.ID == "CC016")
                                {
                                    commandString.Append(ConvertSplit_TCP(setting));
                                    hex_Value = ConvertSplit_TCP(setting);
                                }
                                else if (setting.ID == "SYS15")
                                {
                                    commandString.Append(ConvertSplit_PD(setting) + " ");
                                    hex_Value = ConvertSplit_PD(setting);
                                    //ConvertSplit_PD(setting);
                                    //continue;
                                }
                                else
                                {
                                    commandString.Append(ConvertNumberToHex(setting) + " ");
                                    hex_Value = ConvertNumberToHex(setting);
                                }
                                     
                                    setpointIDString.Append(setting.ID + " ");//hex_Value = setting.numberValue.ToString();


                                if (setting.ID == Global.ipControl2)
                                {
                                    SaveIpAddress(ref commandString, out hex_Value, out index_Value, setting, ref valueseperator, isSave, ref backUpComandString);

                                }
                                else if (!setting.visible && (decimal)(Convert.ToDouble(setting.numberValue)) % (decimal)setting.stepsize != 0)
                                {
                                    index_Value = setting.min.ToString(CultureInfo.InvariantCulture);
                                }

                                else if (setting.ID == "GC00112" || setting.ID == "CC012" || setting.ID == "CC016")
                                {
                                    if(hex_Value == null) hex_Value = "0000";

                                    index_Value = int.Parse(hex_Value, System.Globalization.NumberStyles.HexNumber).ToString();
                                }                                
                                else
                                {
                                    index_Value = setting.numberValue.ToString(CultureInfo.InvariantCulture);
                                }
                                
                            }
                            else if (setting.type == Settings.Type.type_text)
                            {

                                string[] valueseperator = null;
                                if (setting.ID == "SYS03")
                                {
                                    commandString.Append(ConvertNumberToHex(setting) + " ");
                                    setpointIDString.Append(setting.ID + " ");
                                    index_Value = Global.MCCB_TripUnitStyle;
                                }

                                else if (setting.ID == Global.ipControl1 || setting.ID == Global.ipControl3)
                                {
                                    SaveIpAddress(ref commandString, out hex_Value, out index_Value, setting, ref valueseperator, isSave, ref backUpComandString);
                                }
                                else
                                {
                                    if (setting.value_map.Count > 0)
                                    {
                                        int convertedNumber = (int)((decimal)(setting.textvalue * setting.conversion));
                                        string strNumber = convertedNumber.ToString();
                                        while (strNumber.Length < 4)
                                        {
                                            strNumber = "0" + strNumber;
                                        }
                                        strReturnVal = string.Empty;
                                        //strReturnVal = (setting.value_map[strNumber]).ToString();
                                        //commandString.Append(strReturnVal.Trim() + " ");
                                        //hex_Value = strReturnVal.Trim();
                                        foreach (DictionaryEntry value in setting.value_map)
                                        {
                                            string[] lst = value.Key.ToString().Split(',');
                                            if (lst.Contains(strNumber))
                                            {
                                                strReturnVal = value.Value.ToString();// (setting.reversevalue_map[strNumber]).ToString();
                                                commandString.Append(strReturnVal.Trim() + " ");
                                                setpointIDString.Append(setting.ID + " ");
                                                hex_Value = strReturnVal.Trim();
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        commandString.Append(ConvertNumberToHex(setting) + " ");
                                        setpointIDString.Append(setting.ID + " ");
                                        //  hex_Value = setting.textvalue.ToString();
                                        hex_Value = ConvertNumberToHex(setting);
                                    }
                                    if (String.IsNullOrEmpty(setting.textStrValue))
                                    {
                                        index_Value = setting.textvalue.ToString();
                                    }
                                    else
                                    {
                                        index_Value = setting.textStrValue.ToString();
                                    }
                                }
                            }
                            // if bnumber bval = true  :: 0x00
                            // else follow the number conversion
                            else if (setting.type == Settings.Type.type_bNumber)
                            {
                                commandString.Append(ConvertNumberToHex(setting) + " ");//Comma removed AK
                                setpointIDString.Append(setting.ID + " ");
                                hex_Value = ConvertNumberToHex(setting);
                                index_Value = setting.numberValue.ToString(CultureInfo.InvariantCulture);
                                var bValue = index_Value == "0.067" || index_Value == "0" ? false : true;
                                index_Value = bValue + "," + index_Value;

                            }
                            else if (setting.type == Settings.Type.type_bool)
                            {
                                if (setting.bValue)
                                {
                                    commandString.Append(Global.str_True + " ");
                                    setpointIDString.Append(setting.ID + " ");
                                    hex_Value = Global.str_True;
                                }
                                else
                                {
                                    commandString.Append(Global.str_False + " ");
                                    setpointIDString.Append(setting.ID + " ");
                                    hex_Value = Global.str_False;
                                }
                            }

                            setPointCounter++;
                            if (hex_Value == null)
                            {
                                _convertedSetPoint_List.Add(string.Empty);
                            }
                            else
                            {
                                if (setting.type == Settings.Type.type_selection && setting.reversevalue_map.Count > 0)
                                {
                                    index_Value = setting.reversevalue_map[hex_Value].ToString();     //maping hexa decimal value to index value
                                    if (index_Value.Contains(','))
                                    {
                                        // index_Value = setting.listofItemsToDisplay[setting.comboBox.SelectedIndex];
                                        var value = setting.indexesWithHexValuesMapping.FirstOrDefault(kvp => kvp.Value.Contains(setting.selectionValue)).Key;
                                        index_Value = value;
                                    }
                                }
                                else if (setting.type == Settings.Type.type_toggle && setting.reversevalue_map.Count > 0 && (setting.ID == "SYS4A" || setting.ID == "SYS004A" ))
                                {
                                    index_Value = hex_Value == "01" ? "true" : "false";
                                }
                                
                                _convertedSetPoint_List.Add(index_Value);
                            }

                            if (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE)
                            {
                                string[] setpointId = { "SYS011A","SYS10", "SYS12", "SYS17", "RPC19", "RPC20" , "CPC0031A" };
                                if (Global.device_type != Global.ACB_PXR35_DEVICE) setpointId.Append("SYS09");

                                if (setpointId.Contains(setting.ID)) continue;
                                //SYS011,SYS09, SYS10, SYS12, SYS17,RPC19, RPC20

                                if((setting.ID == "CPC018" || setting.ID == "CPC019") && Global.device_type_PXR25)
                                {
                                    commandString.Remove(commandString.Length - 5, 5);
                                    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                    {
                                        backUpComandString.Remove(backUpComandString.Length - 5, 5);
                                    }
                                    continue;
                                }

                                //if ((setting.ID == "CPC018" || setting.ID == "CPC019") && Global.device_type_PXR25)
                                //{                                   
                                //    continue;
                                //}

                                //if ((setting.ID == "CPC018A" || setting.ID == "CPC019A") && Global.device_type_PXR20)
                                //{

                                //    continue;
                                //}
                                //Added by Nishant for bug id -> PXPM-9757
                                if (Global.device_type != Global.PTM_DEVICE)
                                {
                                    if ((setting.ID == "CPC018A" || setting.ID == "CPC019A") && Global.device_type_PXR20)
                                    {
                                        commandString.Remove(commandString.Length - 5, 5);
                                        if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                        {
                                            backUpComandString.Remove(backUpComandString.Length - 5, 5);
                                        }
                                        continue;
                                    }
                                }
                                //if ((setting.ID == "CPC018" || setting.ID == "CPC018A" || setting.ID == "CPC019" || setting.ID == "CPC019A") && !setting.visible)
                                //{  
                                //    //commandString.Remove(commandString.Length - 5, 5);
                                //    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                //    {
                                //        backUpComandString.Remove(backUpComandString.Length - 5, 5);
                                //    }
                                //    continue;
                                //}
                            }

                            if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && setting.ID == "SYS005B" && !setting.visible && (!setting.parseInPXPM) && setting.GroupID != "0" && !TripUnit.getMaintenanceModeTripLevel().visible && !TripUnit.getMaintenanceModeTripLevelACB2().visible)
                            {
                                continue;
                            }

                            else if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || (Global.device_type == Global.ACB_02_01_XX_DEVICE)) && !setting.parseForACB_2_1_XX && !setting.visible && !setting.parseInPXPM && setting.GroupID != "0"/*(setting.ID == "SYS005B" || setting.ID == "SYS005A" || setting.ID == "SYS005")*/)
                            {
                                commandString.Remove(commandString.Length - 5, 5);
                                if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                {
                                    backUpComandString.Remove(backUpComandString.Length - 5, 5);
                                }
                                continue;
                            }
                            //   else if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE) && !setting.visible && (!setting.parseInPXPM) && setting.GroupID != "0"/*(setting.ID == "SYS005B" || setting.ID == "SYS005A" || setting.ID == "SYS005")*/)
                            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && !setting.parseForACB_2_1_XX && !setting.visible && !setting.parseInPXPM  && setting.GroupID != "0")
                           {
                                if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE || Global.device_type == Global.PTM_DEVICE) && (setting.ID != "CPC022"))
                                {

                                    commandString.Remove(commandString.Length - 5, 5);
                                    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                                    {
                                        backUpComandString.Remove(backUpComandString.Length - 5, 5);
                                    }
                                }
                                continue;
                            }

                            //Need to use values from user screen for SDS and SDT while exporting even if those are not available, else dependency of LDS I4T => SDS I2t, SDT range will fail.
                            //So conditio for CPC07 and CPC092 are added.
                            //FOr ACB3.0 , need to use value of CPC014 and CPC014A even if it is NA, 0 value should get exported to device
                            if ((isMCCBRead || Global.isMCCBExport || isNZMRead || Global.isNZMExport ||  isACB3_0Read || Global.isACB3_0Export) && setting.visible && !setting.ID.StartsWith("GEN") && !setting.ID.StartsWith("IP")
                            && Global.grouplist.Count > 0 && setting.TripUnitSequence != 0 && (!setting.notAvailable || setting.ID == "CPC014" || setting.ID == "CPC014A" || setting.ID == "CPC051" || setting.ID == "CPC07" || setting.ID == "CPC092"
                            || setting.ID == "CPC010" || setting.ID == "CPC021"||setting.ID=="SYS020" || setting.ID == "CPC0101" || setting.ID == "CPC024" || setting.ID == "CPC032"
                            || setting.ID == "CPC0126" || setting.ID == "SYS10B" || setting.ID == "CPC0129" || setting.ID == "CPC0132" || setting.ID == "CPC0134"
                            || setting.ID == "ADVA012" || setting.ID == "ADVA023" || setting.ID == "ADVA026" || setting.ID == "ADVA030" || setting.ID == "ADVA031"
                            || setting.ID == "ADVA034" || setting.ID == "ADVA035" || setting.ID == "ADVA036" || setting.ID == "ADVA037" || setting.ID == "ADVA039" || setting.ID == "ADVA041"))
                            {
                                if (setting.ID == "SYS4A")
                                {
                                    if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null)
                                        Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = CombinedValueForMaintenanceMode;
                                }
                                else if (setting.ID == "SYS02" || setting.ID == "SYS2")
                                {
                                    if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null)
                                        Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = strReturnVal;
                                }
                                else if (setting.ID != "SYS03" && setting.ID != "SYS013A" && setting.ID != "SYS014A" && setting.ID != "SYS015A"/* && (setting.ID != "SYS16" || setting.ID != "SYS6")*/)
                                {
                                    if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null)
                                        Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = hex_Value;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(null, setting.ID.ToString());
                            LogExceptions.LogExceptionToFile(ex);
                        }

                    }

                    string strSetPoint = commandString.ToString().Trim();
                    // we need to remove the last comma in order to fit the format of the command strings.
                    //   strSetPoint = commandString.Substring(0, commandString.Length - 1);
                    //   strSetPoint = strSetPoint;
                    _convertedSetPoints.Add(strSetPoint);
                   // _convertedSetPoints.Add(setpointIDString.ToString().Trim());
                    commandString.Clear();
                    setpointIDString.Clear();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                return true;

            }
        }
        /// <summary>
        /// Sunny : Function to Save IP address for Ethernet cam
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="hex_Value"></param>
        /// <param name="index_Value"></param>
        /// <param name="setting"></param>
        /// <param name="valueseperator"></param>
        /// <param name="isSave"></param>
        public static void SaveIpAddress(ref StringBuilder commandString, out string hex_Value, out string index_Value, Settings setting, ref string[] valueseperator, bool isSave, ref StringBuilder backupComandString)
        {
            index_Value = string.Empty;
            hex_Value = string.Empty;
            bool isACBRead = !Global.IsOffline && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE);
            bool isACB3_0Read = !Global.IsOffline && ( Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE);
            bool isMCCBRead = !Global.IsOffline && Global.device_type == Global.MCCBDEVICE;
            bool isNZMRead = !Global.IsOffline && Global.device_type == Global.NZMDEVICE;
            switch (setting.ID)
            {
                case Global.ipControl1:

                    setting.IPaddress = setting.IPaddress.Replace(",", ".");
                    valueseperator = setting.IPaddress.Split('.');

                    string ip1Val = string.Empty;
                    int counter = 0;
                    foreach (var ipVal in valueseperator)
                    {
                        if (ip1Val == string.Empty)
                        {
                            ip1Val = ipVal;
                        }
                        else
                        {
                            ip1Val = ip1Val + "," + ipVal;
                        }
                        commandString.Append(ConvertStringToHex(ipVal) + " ");
                        hex_Value = ConvertStringToHex(ipVal);
                        if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                        {
                            backupComandString.Append(ConvertStringToHex(ipVal) + " ");
                        }
                        if ((Global.device_type == Global.MCCBDEVICE && Global.grouplist.Count > 0 && isSave && (isMCCBRead || Global.isMCCBExport)) ||
                            (Global.device_type == Global.NZMDEVICE  && Global.grouplist.Count > 0 && isSave && (isNZMRead || Global.isNZMExport)) ||
                             ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE) && Global.grouplist.Count > 0 && isSave && (isACB3_0Read || Global.isACB3_0Export)))
                        {
                            int seqCounter = setting.TripUnitSequence + counter;
                            if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter) != null) Global.grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter).SetPointValue = hex_Value;
                            counter++;
                        }
                    }
                    index_Value = setting.IPaddress.Replace(",", ".");
                    break;
                case Global.ipControl2:
                  //   setting.IPaddress = "255.255.255." + setting.textvalue.ToString();
                   // setting.IPaddress_default = "255.255.255." + setting.textvalue.ToString();
                  //  index_Value = setting.IPaddress_default.Replace(",", ".").ToString();
                    index_Value = setting.numberValue.ToString(CultureInfo.InvariantCulture);
                    //   commandString.Append(ConvertStringToHex(setting.textvalue.ToString()) + " ");
                  
                    hex_Value = ConvertNumberToHex(setting);
                    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                    {
                        // backupComandString.Append(ConvertStringToHex(setting.textvalue.ToString()) + " ");
                      
                         backupComandString.Append(hex_Value + " ");
                    }
                    // hex_Value = ConvertStringToHex(setting.textvalue.ToString());

                    if (Global.device_type == Global.MCCBDEVICE && Global.grouplist.Count > 0 && isSave && (isMCCBRead || Global.isMCCBExport || isACB3_0Read || Global.isACB3_0Export))
                    {
                       if(Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null) Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = hex_Value;
                    }
                    break;
                case Global.ipControl3:

                    setting.IPaddress = setting.IPaddress.Replace(",", ".");
                    valueseperator = setting.IPaddress.Split('.');

                    string ipVal3 = valueseperator[2];
                    commandString.Append(ConvertStringToHex(ipVal3) + " ");
                   
                    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                    {
                        backupComandString.Append(ConvertStringToHex(ipVal3) + " ");
                    }
                    if ((Global.device_type == Global.MCCBDEVICE && Global.grouplist.Count > 0 && isSave && (isMCCBRead || Global.isMCCBExport)) ||
                            (Global.device_type == Global.NZMDEVICE && Global.grouplist.Count > 0 && isSave && (isNZMRead || Global.isNZMExport)) ||
                            ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE) && Global.grouplist.Count > 0 && isSave && (isACB3_0Read || Global.isACB3_0Export)))
                    {

                        if (Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence) != null)
                            Global.grouplist.SingleOrDefault(x => x.TripUnitPos == setting.TripUnitSequence).SetPointValue = ConvertStringToHex(ipVal3);
                    }
                    string ip2Val = valueseperator[3];
                    commandString.Append(ConvertStringToHex(ip2Val) + " ");
                    if (Global.isMCCBBackUp && Global.grouplist.Count > 0)
                    {
                        backupComandString.Append(ConvertStringToHex(ip2Val) + " ");
                    }

                    if ((Global.device_type == Global.MCCBDEVICE && Global.grouplist.Count > 0 && isSave && (isMCCBRead || Global.isMCCBExport)) ||
                            (Global.device_type == Global.NZMDEVICE && Global.grouplist.Count > 0 && isSave && (isNZMRead || Global.isNZMExport)) ||
                            ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE) && Global.grouplist.Count > 0 && isSave && (isACB3_0Read || Global.isACB3_0Export)))
                    {
                        int seqCounter = setting.TripUnitSequence+1;
                        if(Global.grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter) != null) Global.grouplist.SingleOrDefault(x => x.TripUnitPos == seqCounter).SetPointValue = ConvertStringToHex(ip2Val);
                    }
                    index_Value = setting.IPaddress.Replace(",", ".");
                    break;

            }
        }

        
        public static void LookupStyleAndPlug()
        {
            try
            {                
                // Split the rating plug and style code
                String rawStyle = ((String)TripUnit.rawSetPoints[2]);
                rawStyle = rawStyle.PadLeft(4, '0');
                TripUnit.userStyle = TripUnit.lookupTable_styleCodes[rawStyle].ToString();

                //string rawGST = ((String)TripUnit.rawSetPoints[21]).Substring(2, 2);
                // Get the rating plug while we have it decoded
                String rawRatingPlug = ((String)TripUnit.rawSetPoints[0]);
                rawRatingPlug = rawRatingPlug.PadLeft(4, '0');
                TripUnit.userRatingPlug = TripUnit.lookupTable_plugCodes[rawRatingPlug].ToString();
                string plug = Regex.Replace(TripUnit.userRatingPlug, "[^0-9.]", "");
                TripUnit.ratingPlug = Convert.ToInt32(plug);
                // TripUnit.GSTUserSelectedValue = TripUnit.lookupTable_GST[rawGST].ToString();
                String breaker = ((String)TripUnit.rawSetPoints[1]);
                breaker = breaker.PadLeft(4, '0');
                //TripUnit.userBreakerInformation = Resource.ResourceManager.GetString(TripUnit.lookupTable_BreakerFrame[breaker].ToString());
                TripUnit.userBreakerInformation = TripUnit.lookupTable_BreakerFrame[breaker].ToString();
                //TripUnit.lookupTable_BreakerFrame[breaker].ToString()


            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);
                //MessageBox.Show("Error looking up style and plug codes: " + ex.Message);
                //Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox("Parse File", "Error looking up style and plug codes: " + ex.Message, false);
                //MsgBoxWindow.Topmost = true;
                //MsgBoxWindow.ShowDialog();
            }
        }

        public static void LookupStyleAndPlug_Offline()
        {
            try
            {
                String rawStyle = "";
                String rawunitType = "";

                if (Global.device_type == Global.PTM_DEVICE)
                {
                    LookupStyleAndPlug_OfflinePTM();
                    return;


                }

                // Split the rating plug and style code
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    rawStyle = ((String)TripUnit.rawSetPoints[10]);
                }
                else
                {
                    rawStyle = ((String)TripUnit.rawSetPoints[12]);
                }
                rawStyle = rawStyle.PadLeft(4, '0');
                TripUnit.userStyle = TripUnit.lookupTable_styleCodes[rawStyle].ToString();
                String rawRatingPlug = "";
                //string rawGST = ((String)TripUnit.rawSetPoints[21]).Substring(2, 2);
                // Get the rating plug while we have it decoded
                if (Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
                {
                    rawRatingPlug = ((String)TripUnit.rawSetPoints[12]);
                }
                else
                {
                    rawRatingPlug = Global.selectedTemplateType == Global.ACB3_0TEMPLATE ? ((String)TripUnit.rawSetPoints[4]) : ((String)TripUnit.rawSetPoints[10]);
                }
                rawRatingPlug = rawRatingPlug.PadLeft(4, '0');
                TripUnit.userRatingPlug = TripUnit.lookupTable_plugCodes[rawRatingPlug].ToString();
                string plug = Regex.Replace(TripUnit.userRatingPlug, "[^0-9.]", "");
                TripUnit.ratingPlug = Convert.ToInt32(plug);
                // TripUnit.GSTUserSelectedValue = TripUnit.lookupTable_GST[rawGST].ToString();
                String breaker = Global.selectedTemplateType == Global.ACB3_0TEMPLATE ? ((String)TripUnit.rawSetPoints[5]) : ((String)TripUnit.rawSetPoints[11]);
                breaker = breaker.PadLeft(4, '0');
                //TripUnit.userBreakerInformation = Resource.ResourceManager.GetString(TripUnit.lookupTable_BreakerFrame[breaker].ToString());
                TripUnit.userBreakerInformation = TripUnit.lookupTable_BreakerFrame[breaker].ToString();
                //Global.GlbstrBreakerFrame = TripUnit.userBreakerInformation;
                if(Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
                {
                    rawunitType =  ((String)TripUnit.rawSetPoints[0]);
                }
                else
                {
                    rawunitType = Global.selectedTemplateType == Global.ACB3_0TEMPLATE ? ((String)TripUnit.rawSetPoints[3]) : ((String)TripUnit.rawSetPoints[9]);
                }
                
                rawunitType = rawunitType.PadLeft(4, '0');
                Global.GlbstrUnitType = TripUnit.lookupTable_unitType[rawunitType].ToString();

                //TripUnit.lookupTable_BreakerFrame[breaker].ToString() 
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);              
            }
        }


        public static void LookupStyleAndPlug_OfflinePTM()
        {
            try
            {
                // Split the rating plug and style code
                String rawStyle = ((String)TripUnit.rawSetPoints[6]);
                rawStyle = rawStyle.PadLeft(4, '0');
                TripUnit.userStyle = TripUnit.lookupTable_styleCodes[rawStyle].ToString();

                //string rawGST = ((String)TripUnit.rawSetPoints[21]).Substring(2, 2);
                // Get the rating plug while we have it decoded
                String rawRatingPlug = Global.selectedTemplateType == Global.PTM_TEMPLATE ? ((String)TripUnit.rawSetPoints[4]) : ((String)TripUnit.rawSetPoints[10]);
                rawRatingPlug = rawRatingPlug.PadLeft(4, '0');
                TripUnit.userRatingPlug = TripUnit.lookupTable_plugCodes[rawRatingPlug].ToString();
                string plug = Regex.Replace(TripUnit.userRatingPlug, "[^0-9.]", "");
                TripUnit.ratingPlug = Convert.ToInt32(plug);
                // TripUnit.GSTUserSelectedValue = TripUnit.lookupTable_GST[rawGST].ToString();
                String breaker = Global.selectedTemplateType == Global.PTM_TEMPLATE ? ((String)TripUnit.rawSetPoints[5]) : ((String)TripUnit.rawSetPoints[11]);
                breaker = breaker.PadLeft(4, '0');
                //TripUnit.userBreakerInformation = Resource.ResourceManager.GetString(TripUnit.lookupTable_BreakerFrame[breaker].ToString());
                TripUnit.userBreakerInformation = TripUnit.lookupTable_BreakerFrame[breaker].ToString();
                //Global.GlbstrBreakerFrame = TripUnit.userBreakerInformation;
                String rawunitType = Global.selectedTemplateType == Global.PTM_TEMPLATE ? ((String)TripUnit.rawSetPoints[3]) : ((String)TripUnit.rawSetPoints[9]);
                rawunitType = rawunitType.PadLeft(4, '0');
                Global.GlbstrUnitType = TripUnit.lookupTable_unitType[rawunitType].ToString();

                //TripUnit.lookupTable_BreakerFrame[breaker].ToString() 
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex);
            }
        }







        /// <summary>
        /// Added by Sunny :Process and Open a CSV file
        /// </summary>
        /// <param name="setpointIndex"></param>
        /// <returns></returns>
        public static string convertDecimalToHex(ArrayList setpointIndex,string setFor = Global.str_app_ID_Table)
        {
            string tripUnitString = null;           //#COVARITY FIX       234835, 235001
            try
            {
                ArrayList tripunitStrings = new ArrayList();
                string combinedValueofMaintenancemodeAndRemote = string.Empty;
                int setPointCounter = 0;
                Global.IterationList = new List<Settings>();
                var iterationGroups = new ArrayList();

                switch (setFor)
                {
                    case Global.str_app_ID_Table:
                        iterationGroups = new ArrayList(TripUnit.groups);
                        foreach (var settingID in TripUnit.ID_list)
                        {
                            Settings setting1 = (Settings)TripUnit.IDTable[settingID];
                            Global.IterationList.Add(setting1);
                        }
                        break;

                    case Global.str_pxset1_ID_Table:
                        iterationGroups = new ArrayList(TripUnit.Pxset1groups);
                        foreach (var settingID in TripUnit.ID_list_Pxset1)
                        {
                            Settings setting1 = (Settings)TripUnit.Pxset1IDTable[settingID];
                            Global.IterationList.Add(setting1);
                        }
                        break;
                    case Global.str_pxset2_ID_Table:
                        iterationGroups = new ArrayList(TripUnit.Pxset2groups);
                        foreach (var settingID in TripUnit.ID_list_Pxset2)
                        {
                            Settings setting1 = (Settings)TripUnit.Pxset2IDTable[settingID];
                            Global.IterationList.Add(setting1);
                        }
                        break;
                }


                foreach (Group group in iterationGroups)
                {
                    tripunitStrings.Add(string.Empty);
                }
                //foreach (var settingID in TripUnit.ID_list)
                //{
                //    Settings setting = (Settings)TripUnit.IDTable[settingID];
                //    Global.IterationList.Add(setting);
                //}
                foreach (Group group in iterationGroups)//TripUnit.groups)
                {

                    var groupIterator = Global.IterationList.Where(x => x.GroupID == group.ID).ToList();
                    foreach (Settings setting in groupIterator)
                    {
                        try
                        {
                            if (Global.selectedTemplateType == Global.ACBTEMPLATE && !(Global.IsOpenFile || Global.isExportControlFlow) && (setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT"))
                            { continue; }
                            if (setting.type == Settings.Type.type_selection)
                            {
                                string selectedValue = string.Empty;


                                selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                if (!setting.reversevalue_map.ContainsValue((string)setpointIndex[setPointCounter]))
                                {
                                    foreach (DictionaryEntry item in setting.reversevalue_map)
                                    {
                                        string[] values = item.Value.ToString().Split(',');
                                        bool isPresent = values.Contains((string)setpointIndex[setPointCounter]);
                                        if (isPresent)
                                        {
                                            selectedValue = item.Key.ToString();
                                            break;
                                        }

                                    }
                                }

                                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                            }

                            if (setting.type == Settings.Type.type_bNumber)
                            {
                                string selectedValue = "";
                                string selection = (string)setpointIndex[setPointCounter];
                                string[] bSelection = selection.Split(',');
                                setting.bValue = Convert.ToBoolean(bSelection[0]);
                                //setting.bDefault = Convert.ToBoolean(bSelection[0]);
                                setting.bValueReadFromTripUnit = setting.bValue;
                                setting.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(bSelection[1].ToString()), CultureInfo.CurrentUICulture);
                                selectedValue = ConvertNumberToHex(setting);
                                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                            }

                            if (setting.type == Settings.Type.type_number)
                            {
                                setting.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(setpointIndex[setPointCounter].ToString()), CultureInfo.CurrentUICulture);
                                var selectedValue = ConvertNumberToHex(setting);
                                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                            }
                            if (setting.type == Settings.Type.type_bool)
                            {
                                var selectedValue = (string)setpointIndex[setPointCounter];
                                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                            }
                            if (setting.type == Settings.Type.type_text)
                            {

                                if (setting.ID == Global.ipControl1 || setting.ID == Global.ipControl2 || setting.ID == Global.ipControl3)
                                {
                                    ExtractIpAddress(setpointIndex, tripunitStrings, setPointCounter, group, setting);
                                }
                                else if (setting.ID == "SYS03")
                                {
                                    var selectedValue = ConvertNumberToHex(setting);
                                    AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                                    setting.numberValue = setting.numberDefault * setting.conversion;
                                }
                                else if (setting.ID == "SYS131C" || setting.ID == "SYS141C" || setting.ID == "SYS151C")
                                {
                                    setting.textStrValue = (String)(setpointIndex[setPointCounter]);
                                }
                                else
                                {
                                    //if (String.Equals((String)(setpointIndex[setPointCounter]), "NA")) 
                                    //{
                                    //    setting.textvalue = 0;
                                    //}
                                    //else
                                    //{
                                    bool isNumber = Double.TryParse(setpointIndex[setPointCounter].ToString(), out setting.textvalue);
                                    if (!isNumber)
                                    {
                                        setting.textvalue = 0;
                                        setting.textStrValue = setpointIndex[setPointCounter].ToString();
                                    }
                                    //  setting.textvalue = Convert.ToDouble(setpointIndex[setPointCounter]);
                                    //  }
                                    var selectedValue = ConvertNumberToHex(setting);
                                    AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                                    setting.numberValue = setting.numberDefault * setting.conversion;
                                }
                            }
                            if (setting.type == Settings.Type.type_toggle)
                            {
                                string selectedValue = string.Empty;
                                if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                                {
                                    // Added by Astha 
                                    selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                    combinedValueofMaintenancemodeAndRemote += selectedValue;
                                    // setPointCounter++;
                                    //continue;
                                }

                                else if (setting.ID == "SYS004B" || setting.ID == "SYS4B" ||
                                        setting.ID == "SYS004C" || setting.ID == "SYS4C" ||
                                       setting.ID == "SYS004D" || setting.ID == "SYS4D" ||
                                       setting.ID == "SYS004E" || setting.ID == "SYS4E" ||
                                   setting.ID == "SYS004F" || setting.ID == "SYS4F")

                                {
                                    //Added by astha to combine maintenance mode and maintenance mode remote values
                                    //in order to match with count of setpoints(48) returned from tripunit and count
                                    //of setpoints in ACB xml(50)
                                    selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                    combinedValueofMaintenancemodeAndRemote += selectedValue;
                                    // selectedValue = combinedValueofMaintenancemodeAndRemote;
                                    //setPointCounter++;
                                }
                                else
                                {
                                    if ((setPointCounter == 32 || setPointCounter == 34 || setPointCounter == 36) && Global.device_type == Global.ACB_PXR35_DEVICE)
                                    {
                                        string value = (string)setpointIndex[setPointCounter];
                                        value.ToLower(CultureInfo.InvariantCulture);
                                        if (value == "TRUE")
                                        {
                                            value = "true";
                                        }
                                        else if (value == "FALSE")
                                        {
                                            value = "false";
                                        }
                                        selectedValue = FindKey(value, setting.reversevalue_map);
                                    }
                                    else
                                    {
                                        selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
                                    }
                                }
                                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                            }
                            if (setting.type == Settings.Type.type_bSelection)
                            {
                                string selectedValue = string.Empty;
                                string selection = (string)setpointIndex[setPointCounter];
                                string[] bSelection = selection.Split(',');
                                setting.bValue = Convert.ToBoolean(bSelection[0]);
                                //  setting.bDefault = Convert.ToBoolean(bSelection[0]);
                                setting.bValueReadFromTripUnit = setting.bValue;
                                if (!setting.ID.EndsWith("B"))
                                {
                                    if (setting.bValue)
                                    {
                                        if (setting.ID.EndsWith("A"))
                                        {
                                            string setPointBId = "";
                                            setPointBId = setting.ID.Remove(setting.ID.Length - 1) + "B";
                                            selectedValue = FindKey(bSelection[1], setting.reversevalue_map);
                                            int setPointbCounter = setPointCounter + 1;
                                            Settings dependent = (Settings)TripUnit.IDTable[setPointBId];
                                            selectedValue += FindKey((string)setpointIndex[setPointbCounter], dependent.reversevalue_map);
                                            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);

                                        }
                                        else
                                        {
                                            selectedValue += FindKey(bSelection[1], setting.reversevalue_map);
                                            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                                        }
                                    }
                                    else
                                    {
                                        selectedValue = "0000";
                                        AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                                    }
                                }
                            }
                            setPointCounter++;


                        }
                        catch (Exception ex)
                        {
                            LogExceptions.LogExceptionToFile(ex);
                        }
                    }
                }
                string concatenatedSettings = "";
                foreach (var tripUnitHexString in tripunitStrings)
                {
                    concatenatedSettings += tripUnitHexString + "\n";
                }
                tripUnitString = concatenatedSettings;
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
            }
            return tripUnitString;

        }
        /// <summary>
        /// Sunny: IP address extraction from saved file
        /// </summary>
        /// <param name="setpointIndex"></param>
        /// <param name="tripunitStrings"></param>
        /// <param name="setPointCounter"></param>
        /// <param name="group"></param>
        /// <param name="setting"></param>
        private static void ExtractIpAddress(ArrayList setpointIndex, ArrayList tripunitStrings, int setPointCounter, Group group, Settings setting)
        {
            string[] valueseperator = null;
            switch (setting.ID)
            {
                case Global.ipControl1:
                    setting.IPaddress = (setpointIndex[setPointCounter]).ToString();

                    //setting.IPaddress = Global.updateValueonCultureBasis(setting.IPaddress);
                    valueseperator = setting.IPaddress.Split('.');

                    for (int i = 0; i < 4; i++)
                    {
                        var selectedValue1 = ConvertStringToHex(valueseperator[i]);
                        AppendToGroupString(tripunitStrings, setting, selectedValue1, group.ID);
                    }
                    break;
                case Global.ipControl2:
                    setting.IPaddress = (setpointIndex[setPointCounter]).ToString();
                    var selectedValue = ConvertStringToHex(setting.textvalue.ToString());
                    AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
                    break;
                case Global.ipControl3:
                    setting.IPaddress = (setpointIndex[setPointCounter]).ToString();

                    //setting.IPaddress = Global.updateValueonCultureBasis(setting.IPaddress);

                    valueseperator = setting.IPaddress.Split('.');

                    string ipVal1 = valueseperator[2];
                    var ipVal = ConvertStringToHex(ipVal1);
                    AppendToGroupString(tripunitStrings, setting, ipVal, group.ID);
                    ipVal1 = valueseperator[3];
                    ipVal = ConvertStringToHex(ipVal1);
                    AppendToGroupString(tripunitStrings, setting, ipVal, group.ID);
                    break;
            }
        }

        //private static void ConstructSubGroupHexFromCSV(ArrayList setpointIndex, ArrayList tripunitStrings, ref string combinedValueofMaintenancemodeAndRemote, ref int setPointCounter, Group group, Settings[] subgroupSetPoints)
        //{
        //    foreach (Settings setting in subgroupSetPoints)
        //    {


        //        if (setting.type == Settings.Type.type_selection)
        //        {
        //            string selectedValue = string.Empty;
        //            if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
        //            {
        //                selectedValue = string.Empty;
        //                selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
        //                combinedValueofMaintenancemodeAndRemote += selectedValue;
        //                setPointCounter++;
        //                continue;
        //            }
        //            else if (setting.ID == "SYS004B" || setting.ID == "SYS4B")
        //            {
        //                //Added by astha to combine maintenance mode and maintenance mode remote values
        //                //in order to match with count of setpoints(48) returned from tripunit and count
        //                //of setpoints in ACB xml(50)
        //                selectedValue = string.Empty;
        //                selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);
        //                combinedValueofMaintenancemodeAndRemote += selectedValue;
        //                selectedValue = string.Empty;
        //                selectedValue = combinedValueofMaintenancemodeAndRemote;
        //            }
        //            else if (!setting.ID.EndsWith("B") || (setting.ID == "CPC04B" || setting.ID == "SYS4B"))
        //            {
        //                selectedValue = string.Empty;
        //                selectedValue = FindKey((string)setpointIndex[setPointCounter], setting.reversevalue_map);

        //            }
        //            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //        }

        //        if (setting.type == Settings.Type.type_bSelection)
        //        {
        //            string selectedValue = string.Empty;
        //            string selection = (string)setpointIndex[setPointCounter];
        //            string[] bSelection = selection.Split(',');
        //            setting.bValue = Convert.ToBoolean(bSelection[0]);
        //            setting.bDefault = Convert.ToBoolean(bSelection[0]);
        //            if (!setting.ID.EndsWith("B"))
        //            {
        //                if (setting.bValue)
        //                {
        //                    if (setting.ID.EndsWith("A"))
        //                    {
        //                        string setPointBId = "";
        //                        setPointBId = setting.ID.Remove(setting.ID.Length - 1) + "B";
        //                        selectedValue = FindKey(bSelection[1], setting.reversevalue_map);
        //                        int setPointbCounter = setPointCounter + 1;
        //                        Settings dependent = (Settings)TripUnit.IDTable[setPointBId];
        //                        selectedValue += FindKey((string)setpointIndex[setPointbCounter], dependent.reversevalue_map);
        //                        //tripUnitString4 += selectedValue + " ";
        //                        AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);

        //                    }
        //                    else
        //                    {
        //                        selectedValue += FindKey(bSelection[1], setting.reversevalue_map);
        //                        // tripUnitString4 += selectedValue + " ";
        //                        AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //                    }
        //                }
        //                else
        //                {
        //                    selectedValue = "0000";
        //                    AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //                }
        //            }
        //        }
        //        if (setting.type == Settings.Type.type_bNumber)
        //        {
        //            string selectedValue = "";
        //            string selection = (string)setpointIndex[setPointCounter];
        //            string[] bSelection = selection.Split(',');
        //            setting.bValue = Convert.ToBoolean(bSelection[0]);
        //            setting.bDefault = Convert.ToBoolean(bSelection[0]);
        //            setting.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(bSelection[1].ToString()), CultureInfo.CurrentUICulture);
        //            //if (setting.bValue)
        //            //{
        //            selectedValue = ConvertNumberToHex(setting);
        //            //  }
        //            //   else
        //            //    {
        //            //        selectedValue = "0000";
        //            //    }
        //            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //        }
        //        if (setting.type == Settings.Type.type_number)
        //        {
        //            setting.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(setpointIndex[setPointCounter].ToString()), CultureInfo.CurrentUICulture);
        //            var selectedValue = ConvertNumberToHex(setting);
        //            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //            setting.numberValue = setting.numberDefault * setting.conversion;
        //            setting.numberDefault = setting.numberValue;
        //        }
        //        if (setting.type == Settings.Type.type_bool)
        //        {
        //            var selectedValue = (string)setpointIndex[setPointCounter];
        //            AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //        }
        //        if (setting.type == Settings.Type.type_text)
        //        {
        //            if (setting.ID == Global.ipControl1 || setting.ID == Global.ipControl2 || setting.ID == Global.ipControl3)
        //            {
        //                ExtractIpAddress(setpointIndex, tripunitStrings, setPointCounter, group, setting);
        //            }
        //            else
        //            {
        //                setting.textvalue = Convert.ToDouble(setpointIndex[setPointCounter]);
        //                var selectedValue = ConvertNumberToHex(setting);
        //                AppendToGroupString(tripunitStrings, setting, selectedValue, group.ID);
        //            }
        //        }
        //        setPointCounter++;
        //    }
        //}

        /// <summary>
        /// Added By Sunny: This function recreates tripunit data format 
        /// </summary>
        /// <param name="tripunitStrings"></param>
        /// <param name="setting"></param>
        /// <param name="selectedValue"></param>
        /// <param name="groupID"></param>
        private static void AppendToGroupString(ArrayList tripunitStrings, Settings setting, string selectedValue, string groupID)
        {
            tripunitStrings[int.Parse(groupID)] += selectedValue + " ";
        }

        /// <summary>
        /// Uses the reverse lookup table to lookup the hex equivalent for the selected value
        /// </summary>
        /// <param name="setting">Selection Setting to be converted</param>
        /// <returns>Converted setting in command format of 0x00##</returns>
        public static String ConvertSelectionToHex(Settings setting)
        {
            string temp = String.Empty;
            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
            try
            {
                if (CultureInfo.CurrentCulture.Name == "en-US" || CultureInfo.CurrentCulture.Name == "zh-CHS")
                {
                    if (setting.comboBox != null)
                    {
                        if ((setting.ID == "SYS132" || setting.ID == "SYS142" || setting.ID == "SYS152") && setting.comboBox.SelectedIndex == -1)
                        {
                            setting.selectionValue = setting.defaultSelectionValue;
                        }
                        else if ((setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015" ||
                            setting.ID == "SYS013A" || setting.ID == "SYS014A" || setting.ID == "SYS015A") && setting.comboBox.SelectedIndex == -1)
                        {
                            setting.selectionValue = setting.defaultSelectionValue;
                        }
                    }
                    if(setting.selectionValue != null)
                    setting.selectionValue = setting.selectionValue.Replace(",", ".");
                }
                else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES"
                          || CultureInfo.CurrentUICulture.Name == "pl-PL" || CultureInfo.CurrentUICulture.Name == "fr-CA")

                {
                    bool isNumeric = MainScreen_ViewModel.IsNumeric(setting.selectionValue);
                    if (isNumeric)
                    {
                        if ((setting.ID != "GEN002") && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                        {
                            setting.selectionValue = setting.selectionValue.Replace(".", ",");
                        }
                        if (setting.ID != "GEN002E" && Global.device_type == Global.ACB_PXR35_DEVICE)
                        {
                            setting.selectionValue = setting.selectionValue.Replace(".", ",");
                        }
                        if (setting.ID != "GEN02A" && Global.device_type == Global.MCCBDEVICE)
                        {
                            setting.selectionValue = setting.selectionValue.Replace(".", ",");
                        }
                    }
                }
                if (setting.reverseLookupTable.Count > 0)
                {

                    if (setting.ID == "SYS132" || setting.ID == "SYS142" || setting.ID == "SYS152") //|| setting.ID == "SYS013" || setting.ID == "SYS014" || setting.ID == "SYS015"
                    {
                        temp = (setting.reverseLookupTable[setting.selectionValue]).ToString();
                    }
                    else
                    {
                        if(setting.selectionValue != null && setting.reverseLookupTable[setting.selectionValue.Trim()] != null)
                        temp = (setting.reverseLookupTable[setting.selectionValue.Trim()]).ToString();
                    }

                }
                //  }
                //Following code is commented by Astha
                //if ((setting.ID == "SYS004A") || (setting.ID == "SYS004B"))
                //{
                //    Group grp = (Group)(TripUnit.groups[0]);
                //    string strSelValueMMRemote = grp.groupSetPoints[5].selectionValue;
                //    if (string.Equals(strSelValueMMRemote, Resources.Strings.Resource.SYS009Item0000))
                //    {
                //        temp = "0000";
                //    }
                //    else if (string.Equals(strSelValueMMRemote, Resources.Strings.Resource.SYS009Item0001))
                //    {
                //        temp = "0001";
                //    }
                //}

                for (int i = 0; i < 4; i++)
                {
                    if (temp.Length < 4)
                    {
                        temp = "0" + temp;
                    }
                }
                // }
            }
            catch (Exception ex)
            {                
                LogExceptions.LogExceptionToFile(null, ex.Message +"Setting :"+setting.ID + " setting.selectionValue : "+ setting.selectionValue);
            }           

            return temp.Trim();
        }

        public static String ConvertToggleSelectionToHex(Settings setting)
        {
            string temp = String.Empty;
            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);


           
            if (setting.value_map.Count > 0)
            {
                temp = (setting.value_map[setting.bValue.ToString().ToLower()]).ToString();
            }
      
          

            for (int i = 0; i < 4; i++)
            {
                if (temp.Length < 4)
                {
                    temp = "0" + temp;
                }
            }
            // }

            return temp.Trim();
        }
        /// <summary>
        /// Uses the reverse lookup table to lookup the hex equivalent for the selected value
        /// </summary>
        /// <param name="setting">Selection Setting to be converted</param>
        /// <returns>Converted setting in command format of 0x00##</returns>
        private static String ConvertDefaultSelectionToHex(Settings setting)
        {
            string temp = String.Empty;
            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);

            if (CultureInfo.CurrentCulture.Name == "en-US" || CultureInfo.CurrentCulture.Name == "zh-CHS")
            {
                setting.defaultSelectionValue = setting.defaultSelectionValue.Replace(",", ".");
            }
            else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES"
                     || CultureInfo.CurrentUICulture.Name == "pl-PL" || CultureInfo.CurrentUICulture.Name == "fr-CA")
            {
                bool isNumeric = MainScreen_ViewModel.IsNumeric(setting.selectionValue);
                if (isNumeric)

                {
                    if (setting.ID != "GEN002" && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                    {
                        setting.selectionValue = setting.selectionValue.Replace(".", ",");
                    }
                    if (setting.ID != "GEN002E" && Global.device_type == Global.ACB_PXR35_DEVICE)
                    {
                        setting.selectionValue = setting.selectionValue.Replace(".", ",");
                    }
                    if (setting.ID != "GEN02A" && Global.device_type == Global.MCCBDEVICE)
                    {
                        setting.selectionValue = setting.selectionValue.Replace(".", ",");
                    }
                }
            }
            if (setting.reverseLookupTable.Count > 0)
            {
                temp = (setting.reverseLookupTable[setting.defaultSelectionValue.Trim()]).ToString();
            }

            for (int i = 0; i < 4; i++)
            {
                if (temp.Length < 4)
                {
                    temp = "0" + temp;
                }
            }
            // }

            return temp.Trim();
        }

        private static String ConvertbSelectionToHex(Settings setting)
        {
            string temp = String.Empty;

            //if (setting.ID != "SYS009")
            //{
            // group.groupSetPoints
            //if ((setting.ID != "SYS004A")&& (setting.ID != "SYS004B"))
            //{

            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);

            setting.selectionValue = Global.updateValueonCultureBasis(setting.selectionValue);
            
            if (setting.reverseLookupTable.Count > 0)
            {
                temp = (setting.reverseLookupTable[setting.selectionValue.Trim()]).ToString();
            }
            for (int i = 0; i < 2; i++)
            {
                if (temp.Length < 2)
                {
                    temp = "0" + temp;
                }
            }
            // }

            return temp.Trim();
        }

        private static String ConvertbDefaultSelectionToHex(Settings setting)
        {
            string temp = String.Empty;

            //if (setting.ID != "SYS009")
            //{
            // group.groupSetPoints
            //if ((setting.ID != "SYS004A")&& (setting.ID != "SYS004B"))
            //{

            string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);

            setting.defaultSelectionValue = Global.updateValueonCultureBasis(setting.defaultSelectionValue);

            
            if (setting.reverseLookupTable.Count > 0)
            {
                temp = (setting.reverseLookupTable[setting.defaultSelectionValue.Trim()]).ToString();
            }
            for (int i = 0; i < 2; i++)
            {
                if (temp.Length < 2)
                {
                    temp = "0" + temp;
                }
            }
            // }

            return temp.Trim();
        }

        /// <summary>
        /// Converts the setting value from a decimal number to
        /// a 2 digit hex number using the conversion number
        /// </summary>
        /// <param name="setting">Setting to be converted</param>
        /// <returns>Converted setting in command format of 0x00##</returns>
        ///
        public static String ConvertNumberToHex(Settings setting)
        {
            int convertedNum = 0;
           // String temp = "";

            if (setting.type == Settings.Type.type_number || setting.type == Settings.Type.type_bNumber)
            {

                if (setting.conversionOperation == "-")
                {
                    convertedNum = (int)((decimal)(setting.conversion - setting.numberValue));
                }
                else
                {
                    convertedNum = (int)((decimal)(setting.numberValue * setting.conversion));
                }
            }
            else if (setting.type == Settings.Type.type_text)
            {
                //if (setting.value_map!=)
                bool isNumber = int.TryParse(((setting.textvalue * setting.conversion).ToString()), out convertedNum);
                if(!isNumber)
                {
                    convertedNum = 0;
                }
                //convertedNum = (int)((decimal)(setting.textvalue * setting.conversion));
            }
            else if (setting.type == Settings.Type.type_skip)
            {
                convertedNum = (int)((decimal)(setting.numberDefault));
            }
            String temp = convertedNum.ToString("X");    //#COVARITY FIX    234815

            for (int i = 0; i < 4; i++)
            {
                if (temp.Length < 4)
                {
                    temp = "0" + temp;
                }
            }
            return temp;
        }

        public static String ConvertStringToHex(string Ipval)
        {
            int convertedNum = 0;
            // String temp = "";            //#COVARITY FIX   234913
            //if (setting.value_map!=)
            convertedNum = (int)((decimal)(Convert.ToInt32(Ipval) * 1));
            String temp = convertedNum.ToString("X");

            for (int i = 0; i < 4; i++)
            {
                if (temp.Length < 4)
                {
                    temp = "0" + temp;
                }
            }
            return temp;
        }

        /// <summary>
        /// Converts the Rating plug and Trip Unit style back into their Hex equivalents and combines
        /// them into a single value.
        /// </summary>
        /// <returns>Converted Rating plug and Style in command format of 0x00##</returns>
        private static String ConvertRPlugStyleToHex()
        {
            // style
            String rawStyle = "";
            foreach (String styleKey in TripUnit.lookupTable_styleCodes.Keys)
            {
                if (TripUnit.userStyle == TripUnit.lookupTable_styleCodes[styleKey].ToString())
                {
                    rawStyle = styleKey;
                    break;
                }
            }
            // rating plug
            String rawRatingPlug = "";
            foreach (String ratingPlugKey in TripUnit.lookupTable_plugCodes.Keys)
            {
                //Below If condition changed by AK.This will save correct rating plug in settings file.Avoid crash in Open file module
                // if (TripUnit.ratingPlug.ToString() == TripUnit.lookupTable_plugCodes[ratingPlugKey].ToString())
                if (TripUnit.userRatingPlug.ToLower() == TripUnit.lookupTable_plugCodes[ratingPlugKey].ToString().ToLower())
                {
                    rawRatingPlug = ratingPlugKey;
                    break;
                }
            }
            return "x" + rawRatingPlug + rawStyle;
        }

        /// <summary>
        /// List boxes are a bitwise selection. Each of the checkboxes represents a bit for a
        /// total of 16 bits. The reserved spaces are used as fillers in order to have 16 bits.
        /// </summary>
        /// <param name="setting">Setting to be converted</param>
        /// <param name="highOrLow"></param>
        /// <returns>Converted setting in command format of 0x####</returns>
        private static String ConvertListBox(Settings setting, String highOrLow)
        {
            ListBox listBox = setting.getListBox();

            String binaryString = "";
            for (int i = setting.itemList.Count - 1; i >= 0; i--)
            {
                if (((item_ListBox)(setting.itemList[i])).HighOrLow == highOrLow)
                {
                    if (((item_ListBox)(setting.itemList[i])).isHidden == true)
                    {
                        binaryString += "0";
                    }
                    else
                    {
                        //  CheckBox checkBox = (CheckBox)listBox.Items[i];

                        //  if ((bool)checkBox.IsChecked)
                        if (((item_ListBox)(setting.itemList[i])).myCheckBox.IsChecked == true)
                        {
                            binaryString += "1";
                        }
                        else
                        {
                            binaryString += "0";
                        }
                    }
                }
            }

            String hexString = Convert.ToInt32(binaryString, 2).ToString("X");

            if (hexString.Length < (Global.str_False).Length)
            {
                while (hexString.Length < (Global.str_False).Length)
                {
                    hexString = "0" + hexString;
                }
            }

            // final format must be '0x####'
            return ("x" + hexString);
        }

        /// <summary>
        /// Relay Configuration has 2 setpoints combined into one. This is a special case.
        /// Relay 1 configuration is the first 8 bits and
        /// Relay 2 configuration is the 2nd 8 bits.
        /// </summary>
        /// <param name="setting">Setting to be converted</param>
        /// <returns>Converted Relay Configuration</returns>
        private static String ConvertSplit(Settings setting)
        {
            return TripUnit.MMforExport;
        }

        private static String ConvertSplit_GC(Settings setting)
        {
            TripUnit.GCforExport = Convert.ToInt32(setting.numberValue.ToString(), 2).ToString("X4");
            return TripUnit.GCforExport;
        }

        private static void resetBitValues()
        {
            TripUnit.PD_b0 = '0';
            TripUnit.PD_b1 = '0';
            TripUnit.PD_b2 = '0';
            TripUnit.PD_b3 = '0';
            TripUnit.PD_b4 = '0';
            TripUnit.PD_b5 = '0';
            TripUnit.PD_b6 = '0';
            TripUnit.PD_b7 = '0';
        }
        private static String ConvertSplit_PD(Settings setting)
        {
            //Reset bit values 
            resetBitValues();

            Settings PDMode_setting = TripUnit.getPowerDemandMode();
            TripUnit.PD_b0 = PDMode_setting.bValue ? '1' : '0';
           


            Settings PDModePrecision_setting = TripUnit.getPowerDemandPrecision();

            var PowerDemandPrecisionValue = string.Empty;

            string SYS14Value = Convert.ToString(TripUnit.PD_b5 + TripUnit.PD_b4);

            if (PDModePrecision_setting.selectionValue == Resource.SYS14Item0000)
            {
                TripUnit.PD_b5 = '0';
                TripUnit.PD_b4 = '0';
            }
            else if (PDModePrecision_setting.selectionValue == Resource.SYS14Item0001)
            {
                TripUnit.PD_b5 = '0';
                TripUnit.PD_b4 = '1';
            }
            else if (PDModePrecision_setting.selectionValue == Resource.SYS14Item0010)
            {
                TripUnit.PD_b5 = '1';
                TripUnit.PD_b4 = '0';
            }
            //else if (PDModePrecision_setting.selectionValue == Resource.SYS14Item0011)
            //{
            //    TripUnit.PD_b5 = '1';
            //    TripUnit.PD_b4 = '1';
            //}

          string PD8Value=  string.Concat(TripUnit.PD_b7, TripUnit.PD_b6, TripUnit.PD_b5, TripUnit.PD_b4, TripUnit.PD_b3, TripUnit.PD_b2, TripUnit.PD_b1, TripUnit.PD_b0);
          

          TripUnit.PDforExport = Convert.ToInt32(PD8Value, 2).ToString("X4");

            return TripUnit.PDforExport;
        }

        private static String ConvertSplit_TCP(Settings setting)
        {
            
            string PD8Value = string.Concat(0, 0, 0, TripUnit.TCP_b4, TripUnit.TCP_b3, TripUnit.TCP_b2, TripUnit.TCP_b1, TripUnit.TCP_b0);

            TripUnit.TCPforExport = Convert.ToInt32(PD8Value, 2).ToString("X4");

            return TripUnit.TCPforExport;
        }

        private static String ConvertSplit_RTU(Settings setting)
        {

            string PD8Value = string.Concat(0, 0, 0, TripUnit.RTU_b4, TripUnit.RTU_b3, TripUnit.RTU_b2, TripUnit.RTU_b1, TripUnit.RTU_b0);

            TripUnit.RTUforExport = Convert.ToInt32(PD8Value, 2).ToString("X4");

            return TripUnit.RTUforExport;
        }

        //private static String getUserRatingPlug(string indexValue)
        //{
        //    foreach (Group group in TripUnit.groups)
        //    {
        //        foreach (Settings setting in group.groupSetPoints)
        //        {
        //            if (setting.type == Settings.Type.type_selection)
        //            {
        //                var selectedValue = FindKey(indexValue, setting.reversevalue_map);
        //            }
        //        }
        //}


        /// <summary>
        /// Sunny: For creating Backup file
        /// </summary>
        /// <param name="HexValues"></param>
        /// <returns></returns>
        public static ArrayList convertHexToIndex(ArrayList HexValues)
        {
            bool isACBRead = !Global.IsOffline && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE);
            bool isACB3_0Read = !Global.IsOffline && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE);
            bool isMCCBRead = !Global.IsOffline && Global.device_type == Global.MCCBDEVICE;
            bool isNZMRead = !Global.IsOffline && Global.device_type == Global.NZMDEVICE;
            ArrayList index_Value = new ArrayList();
            // String valueForMaintenanceMode = string.Empty;
            String valueForMaintenanceModeRemote = string.Empty;
           // String CombinedValueForMaintenanceMode;// = string.Empty;//#COVERITY FIX   235042
            int setPointCounter = 0;
            var hex_Value = string.Empty;
            var iterationGroups = TripUnit.groups;
            Global.IterationList = new List<Settings>();
            char MM_b1 = '0', MM_b2 = '0', MM_b7 = '0', MM_b0 = '0', MM_b8='0'; //#COVERITY FIX  376679

            foreach (var settingID in TripUnit.ID_list)
            {
                Settings setting = (Settings)TripUnit.IDTable[settingID];
                Global.IterationList.Add(setting);
            }
            foreach (Group group in TripUnit.groups)
            {
                 
        var groupIterator = Global.IterationList.Where(x => x.GroupID == group.ID).ToList();
                foreach (Settings setting in groupIterator)
                {
                    //if (Global.selectedTemplateType == Global.ACBTEMPLATE && (setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT"))
                    //{
                    //    continue;
                    //}

                    if (setting.type == Settings.Type.type_selection)
                    {
                        if (setting.ID == "SYS000" || setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151")
                        {
                            string strReturnVal = ConvertSelectionToHex(setting);

                            if (setting.ID == "SYS131" && !string.IsNullOrEmpty(Global.relay1Fun))
                            {
                                strReturnVal = Global.relay1Fun;
                            }
                            if (setting.ID == "SYS141" && !string.IsNullOrEmpty(Global.relay2Fun))
                            {
                                strReturnVal = Global.relay2Fun;
                            }
                            if (setting.ID == "SYS151" && !string.IsNullOrEmpty(Global.relay3Fun))
                            {
                                strReturnVal = Global.relay3Fun;
                            }

                            if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                            {
                                index_Value.Add(setting.reversevalue_map[strReturnVal.Trim()].ToString());
                            }
                            continue;
                        }

                        if (setting.visible)
                        {
                            var hexVal = HexValues[setPointCounter];
                            //restrict exception for offline backup
                            if (!Global.IsOffline)
                            {
                                if ((setting.ID == "SYS132" || setting.ID == "SYS013") && !string.IsNullOrEmpty(Global.relay1Set))
                                {
                                    HexValues[setPointCounter] = Global.relay1Set;
                                }
                                if ((setting.ID == "SYS142" || setting.ID == "SYS014") && !string.IsNullOrEmpty(Global.relay2Set))
                                {
                                    HexValues[setPointCounter] = Global.relay2Set;
                                }
                                if ((setting.ID == "SYS152" || setting.ID == "SYS015") && !string.IsNullOrEmpty(Global.relay3Set))
                                {
                                    HexValues[setPointCounter] = Global.relay3Set;
                                }
                            }

                            if (!setting.reversevalue_map.ContainsKey(hexVal))
                            {
                                foreach (var key in setting.reversevalue_map.Keys)
                                {
                                    string[] values = key.ToString().Split(',');
                                    bool isPresent = values.Contains(hexVal);
                                    if (isPresent)
                                    {
                                        index_Value.Add(setting.reversevalue_map[key]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var index = setting.reversevalue_map[HexValues[setPointCounter]].ToString();
                                if (index.Contains(','))
                                {
                                    // index_Value = setting.listofItemsToDisplay[setting.comboBox.SelectedIndex];
                                    var value = setting.indexesWithHexValuesMapping.FirstOrDefault(kvp => kvp.Value.Contains(setting.selectionValue)).Key;
                                    index = value;
                                }
                                index_Value.Add(index);
                            }
                        }

                        else if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE) && setting.ID == "SYS005B" && !setting.visible && (!setting.parseInPXPM) && setting.GroupID != "0" && !TripUnit.getMaintenanceModeTripLevel().visible && !TripUnit.getMaintenanceModeTripLevelACB2().visible)

                        {
                            index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString());
                        }
                        else
                        {
                            string strReturnVal = ConvertDefaultSelectionToHex(setting);
                            if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                            {
                                hex_Value = strReturnVal.Trim();
                            }
                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString());
                            if (!setting.parseForACB_2_1_XX && !setting.visible && !setting.parseInPXPM && setting.GroupID != "0" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE))
                            {
                                continue;
                            }
                        }

                    }
                    if (setting.type == Settings.Type.type_toggle)
                    {
                        if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                        {
                            string CombinedValueForMaintenanceMode = HexValues[setPointCounter].ToString();
                            //string valueForMaintenanceMode = CombinedValueForMaintenanceMode.Substring(0, 2);
                            //valueForMaintenanceModeRemote = CombinedValueForMaintenanceMode.Substring(2);
                            //index_Value.Add(setting.reversevalue_map[valueForMaintenanceMode]);


                            string MM16bitString = String.Join(String.Empty, CombinedValueForMaintenanceMode.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                            if (MM16bitString != null)
                            {
                                //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                //b8: ARMs mode enable / disable channel 
                                //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)


                                MM_b0 = MM16bitString[15];
                                MM_b1 = MM16bitString[14];
                                MM_b2 = MM16bitString[13];
                                MM_b7 = MM16bitString[8];
                                MM_b8 = MM16bitString[7];//#COVERITY FIX  376679, MM_B8 is used only in one if scope so decalre it in that scope

                                index_Value.Add(setting.reversevalue_map["0" + MM_b8]);
                            }
                            
                            //setPointCounter--;
                        }
                        else if (setting.ID == "SYS004B" || setting.ID == "SYS004C" || setting.ID == "SYS004D" || setting.ID == "SYS004E" ||
                                setting.ID == "SYS4B" || setting.ID == "SYS4C" || setting.ID == "SYS4D" || setting.ID == "SYS4E" || setting.ID == "SYS4F")

                        {
                            switch (setting.ID)
                            {

                                //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                case "SYS004B":
                                case "SYS4B":
                                    index_Value.Add(setting.reversevalue_map["0" + MM_b0]);
                                    break;

                                case "SYS004C":
                                case "SYS4C":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b0]);
                                    break;

                                case "SYS004D":
                                case "SYS4D":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b7]);
                                    break;

                                case "SYS004E":
                                case "SYS4E":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b1]);
                                    break;

                                case "SYS004F":
                                case "SYS4F":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b2]);
                                    break;

                            }

                            //string hexVal = HexValues[setPointCounter].ToString();
                            //if(setting.ID == "SYS004B" || setting.ID == "SYS4B")
                            //{
                            //    string valforSYS4B = hexVal.Substring(2);
                            //    index_Value.Add(setting.reversevalue_map[valforSYS4B].ToString().ToLower());
                            //}
                            //if (setting.reversevalue_map.Contains(hexVal))
                            //{
                            //    index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString().ToLower());
                            //}
                            if (Global.device_type == Global.NZMDEVICE || Global.device_type == Global.MCCBDEVICE) setPointCounter++;
                            continue;
                        }
                        
                        else if (setting.ID == "CPC080")
                        {
                            Settings ShortDelayPickup = new Settings();
                            String hexValue_SDPickup = String.Empty;

                            var SDPickupitem = groupIterator.Where(x => x.ID == "CPC081");
                            if (SDPickupitem != null && SDPickupitem.Count() != 0)
                            {
                                ShortDelayPickup = SDPickupitem.FirstOrDefault();
                                hexValue_SDPickup = Global.convertHexToNum(HexValues[setPointCounter + 1].ToString(), ShortDelayPickup.conversion, setting.conversionOperation).ToString();
                            }
                            if ((ShortDelayPickup.visible) && (!String.IsNullOrWhiteSpace(hexValue_SDPickup) && Convert.ToDouble(hexValue_SDPickup) > 0))

                                hex_Value = "0001";
                            else
                                hex_Value = "0000";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else if (setting.ID == "CPC090")
                        {
                            var SDTime = groupIterator.Where(x => x.ID == "CPC091B").FirstOrDefault();

                            //var hexValue_SDTime = string.Empty; //#COVERITY FIX   235042
                            var hexValue_SDTime = HexValues[setPointCounter + 2].ToString();
                            hexValue_SDTime = Global.convertHexToNum(hexValue_SDTime, SDTime.conversion, setting.conversionOperation).ToString();


                            if ((hexValue_SDTime == "0.067"))
                                hex_Value = "0000";
                            else
                                hex_Value = "0001";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else if (setting.ID == "CPC040" && Global.device_type == Global.NZMDEVICE)
                        {
                            //var LongDelayTime_NZM = string.Empty;    //#COVARITY FIX 235042
                            var LongDelayTime_NZM = HexValues[setPointCounter + 3].ToString();

                            if ((LongDelayTime_NZM == "7FFF"))
                                hex_Value = "0000";
                            else
                                hex_Value = "0001";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else
                        {
                            var hexVal = HexValues[setPointCounter];
                            if (setting.reversevalue_map.Contains(hexVal))
                            {
                                index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString().ToLower());
                            }
                        }

                    }
                    if (setting.type == Settings.Type.type_bNumber)
                    {
                        hex_Value = HexValues[setPointCounter].ToString();
                        hex_Value = (Global.convertHexToNum(hex_Value, setting.conversion, ref setting.numberValue,"*",false)).ToString();
                       var bValue = (hex_Value == "0.067") ? false : true;
                        hex_Value = bValue + "," + hex_Value;
                        index_Value.Add(hex_Value);
                        //hex_Value = setting.numberValue.ToString(CultureInfo.InvariantCulture);

                        //index_Value.Add(hex_Value);
                    }


                    if (setting.type == Settings.Type.type_number)
                    {
                        //if (setting.visible)
                        //{
                        //    hex_Value = HexValues[setPointCounter].ToString();
                        //    hex_Value = Global.convertHexToNum(hex_Value, setting.conversion).ToString();
                        //    if (setting.ID == "SYS22")
                        //        hex_Value = (setting.conversion - Convert.ToDouble(hex_Value) * setting.conversion).ToString() ;
                        //    index_Value.Add(hex_Value);
                        //}
                        //else if(setting.ID == "CPC091A" || setting.ID == "CPC091B")
                        //{
                        //    hex_Value = HexValues[setPointCounter].ToString();
                        //    hex_Value = Global.convertHexToNum(hex_Value, setting.conversion).ToString();
                        //    index_Value.Add(hex_Value);
                        //}
                        //else
                        //{
                        //    if ((decimal)setting.numberValue % (decimal)setting.stepsize != 0)
                        //    {
                        //        hex_Value = setting.min.ToString();
                        //    }
                        //    else
                        //    {
                        //        hex_Value = setting.numberDefault.ToString();
                        //    }
                        //    index_Value.Add(hex_Value);
                        //}
                       
                            hex_Value = HexValues[setPointCounter].ToString();
                            hex_Value = Global.convertHexToNum(hex_Value, setting.conversion, setting.conversionOperation).ToString();
                       
                        double numberValue = Convert.ToDouble(hex_Value);
                        if (!Global.isMCCBBackUp && !((decimal)numberValue % (decimal)setting.stepsize == 0 && numberValue >= setting.min && numberValue <= setting.max))
                        {
                            hex_Value = setting.numberDefault.ToString();                           
                        }
                        index_Value.Add(hex_Value);
                    }

                    else if (setting.type == Settings.Type.type_text)
                    {

                        string[] valueseperator = null;

                        if (setting.ID == Global.ipControl1)
                        {
                            string str = string.Empty;
                            for (int ip1 = 0; ip1 <= 3; ip1++)
                            {
                                hex_Value = (String)HexValues[setPointCounter];
                                if (str == string.Empty)
                                {
                                    str = Global.convertHexToString(hex_Value, setting.conversion); ;
                                }
                                else
                                {
                                    str = str + "." + Global.convertHexToString(hex_Value, setting.conversion);
                                }
                                setPointCounter++;
                            }
                            index_Value.Add(str);
                            continue;
                        }

                        else if (setting.ID == Global.ipControl2)
                        {
                            //string str = string.Empty;    //#COVARITY FIX     235042
                            hex_Value = (String)HexValues[setPointCounter];
                            string str = Global.convertHexToString(hex_Value, setting.conversion);
                            string Ipvalue2 = Global.ipMask + str;
                            index_Value.Add(Ipvalue2);
                        }

                        else if (setting.ID == Global.ipControl3)
                        {
                            Settings settingIp1 = (Settings)TripUnit.IDTable[Global.ipControl1];
                            valueseperator = settingIp1.IPaddress.Split('.');
                            string ip1ControlVal = valueseperator[0] + "." + valueseperator[1];
                            string str = string.Empty;

                            for (var ip3 = 0; ip3 <= 1; ip3++)
                            {
                                hex_Value = (String)HexValues[setPointCounter];
                                if (str == string.Empty)
                                {
                                    str = Global.convertHexToString(hex_Value, setting.conversion); ;
                                }
                                else
                                {
                                    str = str + "." + Global.convertHexToString(hex_Value, setting.conversion);
                                }
                                setPointCounter++;
                            }
                            string ipcontrol3 = ip1ControlVal + "." + str;
                            index_Value.Add(ipcontrol3);
                            continue;
                        }
                        else if (setting.ID == "GEN02B" || setting.ID == "GEN02C")
                        {
                            hex_Value = (String)HexValues[setPointCounter];
                            index_Value.Add(hex_Value);
                            setPointCounter++;
                            continue;
                        }
                        else if (setting.ID == "SYS03")
                        {

                            index_Value.Add(Global.MCCB_TripUnitStyle);
                        }
                        else if (setting.ID == "SYS004AT" || setting.ID == "SYS004BT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET")

                        {
                            switch (setting.ID)
                            {

                                //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                case "SYS004AT":
                                    index_Value.Add(MM_b8 == '1' ? "Yes" : "No");
                                    break;

                                case "SYS004BT":
                                    index_Value.Add(MM_b0 == '1' ? "Yes" : "No");
                                    break;

                                case "SYS004CT":
                                    index_Value.Add(MM_b0 == '1' ? "Yes" : "No");
                                    break;

                                case "SYS004DT":
                                    index_Value.Add(MM_b7 == '1' ? "Yes" : "No");
                                    break;

                                case "SYS004ET":
                                    index_Value.Add(MM_b1 == '1' ? "Yes" : "No");
                                    break;

                                case "SYS004FT":
                                    index_Value.Add(MM_b2 == '1' ? "Yes" : "No");
                                    break;

                            }
                            continue;
                        }
                        else
                        {
                            if (setting.value_map.Count > 0)
                            {
                                hex_Value = HexValues[setPointCounter].ToString();
                                if (setting.reversevalue_map[hex_Value] != null)
                                {
                                    index_Value.Add(setting.reversevalue_map[hex_Value]);
                                }
                                else
                                {
                                    hex_Value = Global.convertHexToString(hex_Value, setting.conversion);
                                    index_Value.Add(hex_Value);
                                }
                            }
                            else
                            {
                                hex_Value = HexValues[setPointCounter].ToString();
                                hex_Value = Global.convertHexToString(hex_Value, setting.conversion);
                                index_Value.Add(hex_Value);
                            }
                        }
                    }
                    setPointCounter++;

                }
            }
            return index_Value;
        }


        /// <summary>
        /// Sunny: For creating Backup file
        /// </summary>
        /// <param name="HexValues"></param>
        /// <returns></returns>
        public static ArrayList convertHexToIndex_ACB3_0(ArrayList HexValues)
        {           
            bool isACB3_0Read = !Global.IsOffline && (Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_PXR35_DEVICE);
            bool isMCCBRead = !Global.IsOffline && Global.device_type == Global.MCCBDEVICE;
            bool isNZMRead = !Global.IsOffline && Global.device_type == Global.NZMDEVICE;
            ArrayList index_Value = new ArrayList();
            // String valueForMaintenanceMode = string.Empty;
            String valueForMaintenanceModeRemote = string.Empty;
            // String CombinedValueForMaintenanceMode;// = string.Empty;//#COVERITY FIX   235042
            int setPointCounter = 0;
            var hex_Value = string.Empty;
            var iterationGroups = TripUnit.groups;
            Global.IterationList = new List<Settings>();
            char MM_b1 = '0', MM_b2 = '0', MM_b7 = '0', MM_b0 = '0'; //#COVERITY FIX  376679

            foreach (var settingID in TripUnit.ID_list)
            {
                Settings setting = (Settings)TripUnit.IDTable[settingID];
                Global.IterationList.Add(setting);
            }
            foreach (Group group in TripUnit.groups)
            {

                var groupIterator = Global.IterationList.Where(x => x.GroupID == group.ID).ToList();
                foreach (Settings setting in groupIterator)
                {
                    //if (Global.selectedTemplateType == Global.ACBTEMPLATE && (setting.ID == "SYS004AT" || setting.ID == "SYS004CT" || setting.ID == "SYS004DT" || setting.ID == "SYS004ET" || setting.ID == "SYS004FT"))
                    //{
                    //    continue;
                    //}

                    if (setting.type == Settings.Type.type_selection)
                    {
                        if (/*setting.ID == "SYS000" || */setting.ID == "SYS131" || setting.ID == "SYS141" || setting.ID == "SYS151"
                            || setting.ID == "SYS131A" || setting.ID == "SYS141A" || setting.ID == "SYS151A")
                        {
                            //////string strReturnVal = ConvertSelectionToHex(setting);

                            //////if (setting.ID == "SYS131" && !string.IsNullOrEmpty(Global.relay1Fun))
                            //////{
                            //////    strReturnVal = Global.relay1Fun;
                            //////}
                            //////if (setting.ID == "SYS141" && !string.IsNullOrEmpty(Global.relay2Fun))
                            //////{
                            //////    strReturnVal = Global.relay2Fun;
                            //////}
                            //////if (setting.ID == "SYS151" && !string.IsNullOrEmpty(Global.relay3Fun))
                            //////{
                            //////    strReturnVal = Global.relay3Fun;
                            //////}

                            //if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                            //{
                            //    index_Value.Add(setting.reversevalue_map[strReturnVal.Trim()].ToString());
                            //}
                            //  continue;
                        }

                        if (setting.visible)
                        {
                            var hexVal = HexValues[setPointCounter];
                            //restrict exception for offline backup
                            //if (!Global.IsOffline)
                            //{
                            //    if ((setting.ID == "SYS132" || setting.ID == "SYS013") && !string.IsNullOrEmpty(Global.relay1Set))
                            //    {
                            //        HexValues[setPointCounter] = Global.relay1Set;
                            //    }
                            //    if ((setting.ID == "SYS142" || setting.ID == "SYS014") && !string.IsNullOrEmpty(Global.relay2Set))
                            //    {
                            //        HexValues[setPointCounter] = Global.relay2Set;
                            //    }
                            //    if ((setting.ID == "SYS152" || setting.ID == "SYS015") && !string.IsNullOrEmpty(Global.relay3Set))
                            //    {
                            //        HexValues[setPointCounter] = Global.relay3Set;
                            //    }
                            //}

                            if (!setting.reversevalue_map.ContainsKey(hexVal))
                            {
                                foreach (var key in setting.reversevalue_map.Keys)
                                {
                                    string[] values = key.ToString().Split(',');
                                    bool isPresent = values.Contains(hexVal);
                                    if (isPresent)
                                    {
                                        index_Value.Add(setting.reversevalue_map[key]);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var index = setting.reversevalue_map[HexValues[setPointCounter]].ToString();
                                if (index.Contains(','))
                                {
                                    // index_Value = setting.listofItemsToDisplay[setting.comboBox.SelectedIndex];
                                    var value = setting.indexesWithHexValuesMapping.FirstOrDefault(kvp => kvp.Value.Contains(setting.selectionValue)).Key;
                                    index = value;
                                }
                                index_Value.Add(index);
                            }
                        }

                        else if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) 
                            && setting.ID == "SYS005B" && !setting.visible && (!setting.parseInPXPM) && setting.GroupID != "0" && !TripUnit.getMaintenanceModeTripLevel().visible && !TripUnit.getMaintenanceModeTripLevelACB2().visible)

                        {
                            index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString());
                        }
                        else
                        {
                            string strReturnVal = ConvertDefaultSelectionToHex(setting);
                            if (!String.IsNullOrEmpty(strReturnVal.Trim()))
                            {
                                hex_Value = strReturnVal.Trim();
                            }
                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString());
                            //if (!setting.parseForACB_2_1_XX && !setting.visible && !setting.parseInPXPM && setting.GroupID != "0" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE))
                            //{
                            //    continue;
                            //}
                        }

                    }
                    if (setting.type == Settings.Type.type_toggle)
                    {
                        if (setting.ID == "SYS004A" || setting.ID == "SYS4A")
                        {
                            string CombinedValueForMaintenanceMode = HexValues[setPointCounter].ToString();
                            //string valueForMaintenanceMode = CombinedValueForMaintenanceMode.Substring(0, 2);
                            //valueForMaintenanceModeRemote = CombinedValueForMaintenanceMode.Substring(2);
                            //index_Value.Add(setting.reversevalue_map[valueForMaintenanceMode]);


                            string MM16bitString = String.Join(String.Empty, CombinedValueForMaintenanceMode.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                            if (MM16bitString != null)
                            {
                                //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                //b8: ARMs mode enable / disable channel 
                                //b2: "Enabled via LCD Display" with a toggle switch (YES / NO). (bit 2)(NZM Only)


                                MM_b0 = MM16bitString[15];
                                MM_b1 = MM16bitString[14];
                                MM_b2 = MM16bitString[13];
                                MM_b7 = MM16bitString[8];
                                char MM_b8 = MM16bitString[7];//#COVERITY FIX  376679, MM_B8 is used only in one if scope so decalre it in that scope
                                index_Value.Add(setting.reversevalue_map["0" + MM_b8]);
                            }

                            //setPointCounter--;
                        }
                        else if (setting.ID == "SYS004B" || setting.ID == "SYS004C" || setting.ID == "SYS004D" || setting.ID == "SYS004E" ||
                                setting.ID == "SYS4B" || setting.ID == "SYS4C" || setting.ID == "SYS4D" || setting.ID == "SYS4E" || setting.ID == "SYS4F")

                        {
                            switch (setting.ID)
                            {

                                //b0: communication channel   ==  "Enabled via PXPM or Communications"  "SYS004C"
                                //b1: ARMs Pin channel              "Enabled via Remote Switch"   "SYS004E"
                                //b7: Rotary switch channel         "Enabled via Trip Unit Switch"(ACB / PD Only)   "SYS004D"
                                case "SYS004B":
                                case "SYS4B":
                                    index_Value.Add(setting.reversevalue_map["0" + MM_b0]);
                                    break;

                                case "SYS004C":
                                case "SYS4C":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b0]);
                                    break;

                                case "SYS004D":
                                case "SYS4D":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b7]);
                                    break;

                                case "SYS004E":
                                case "SYS4E":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b1]);
                                    break;

                                case "SYS004F":
                                case "SYS4F":
                                    index_Value.Add(setting.reversevalue_map["000" + MM_b2]);
                                    break;

                            }

                            //string hexVal = HexValues[setPointCounter].ToString();
                            //if(setting.ID == "SYS004B" || setting.ID == "SYS4B")
                            //{
                            //    string valforSYS4B = hexVal.Substring(2);
                            //    index_Value.Add(setting.reversevalue_map[valforSYS4B].ToString().ToLower());
                            //}
                            //if (setting.reversevalue_map.Contains(hexVal))
                            //{
                            //    index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString().ToLower());
                            //}
                            /*if (Global.device_type == Global.NZMDEVICE || Global.device_type == Global.MCCBDEVICE)*/ setPointCounter++;
                            continue;
                        }
                        else if (setting.ID == "CPC080")
                        {
                            Settings ShortDelayPickup = new Settings();
                            String hexValue_SDPickup = String.Empty;

                            var SDPickupitem = groupIterator.Where(x => x.ID == "CPC081");
                            if (SDPickupitem != null && SDPickupitem.Count() != 0)
                            {
                                ShortDelayPickup = SDPickupitem.FirstOrDefault();
                                hexValue_SDPickup = Global.convertHexToNum(HexValues[setPointCounter + 1].ToString(), ShortDelayPickup.conversion, setting.conversionOperation).ToString();
                            }
                            if ((ShortDelayPickup.visible) && (!String.IsNullOrWhiteSpace(hexValue_SDPickup) && Convert.ToDouble(hexValue_SDPickup) > 0))

                                hex_Value = "0001";
                            else
                                hex_Value = "0000";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else if (setting.ID == "CPC090")
                        {
                            var SDTime = groupIterator.Where(x => x.ID == "CPC091B").FirstOrDefault();

                            //var hexValue_SDTime = string.Empty; //#COVERITY FIX   235042
                            var hexValue_SDTime = HexValues[setPointCounter + 2].ToString();
                            hexValue_SDTime = Global.convertHexToNum(hexValue_SDTime, SDTime.conversion, setting.conversionOperation).ToString();


                            if ((hexValue_SDTime == "0.067"))
                                hex_Value = "0000";
                            else
                                hex_Value = "0001";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else if (setting.ID == "CPC040" && Global.device_type == Global.NZMDEVICE)
                        {
                            //var LongDelayTime_NZM = string.Empty;    //#COVARITY FIX 235042
                            var LongDelayTime_NZM = HexValues[setPointCounter + 3].ToString();

                            if ((LongDelayTime_NZM == "7FFF"))
                                hex_Value = "0000";
                            else
                                hex_Value = "0001";

                            index_Value.Add(setting.reversevalue_map[hex_Value].ToString().ToLower());
                        }
                        else
                        {
                            var hexVal = HexValues[setPointCounter];
                            if (setting.reversevalue_map.Contains(hexVal))
                            {
                                index_Value.Add(setting.reversevalue_map[HexValues[setPointCounter]].ToString().ToLower());
                            }
                        }

                    }
                    if (setting.type == Settings.Type.type_bNumber)
                    {
                        hex_Value = HexValues[setPointCounter].ToString();
                        hex_Value = (Global.convertHexToNum(hex_Value, setting.conversion, ref setting.numberValue, "*", false)).ToString();
                        var bValue = (hex_Value == "0.067") ? false : true;
                        hex_Value = bValue + "," + hex_Value;
                        index_Value.Add(hex_Value);
                        //hex_Value = setting.numberValue.ToString(CultureInfo.InvariantCulture);

                        //index_Value.Add(hex_Value);
                    }


                    if (setting.type == Settings.Type.type_number)
                    {
                        //if (setting.visible)
                        //{
                        //    hex_Value = HexValues[setPointCounter].ToString();
                        //    hex_Value = Global.convertHexToNum(hex_Value, setting.conversion).ToString();
                        //    if (setting.ID == "SYS22")
                        //        hex_Value = (setting.conversion - Convert.ToDouble(hex_Value) * setting.conversion).ToString() ;
                        //    index_Value.Add(hex_Value);
                        //}
                        //else if(setting.ID == "CPC091A" || setting.ID == "CPC091B")
                        //{
                        //    hex_Value = HexValues[setPointCounter].ToString();
                        //    hex_Value = Global.convertHexToNum(hex_Value, setting.conversion).ToString();
                        //    index_Value.Add(hex_Value);
                        //}
                        //else
                        //{
                        //    if ((decimal)setting.numberValue % (decimal)setting.stepsize != 0)
                        //    {
                        //        hex_Value = setting.min.ToString();
                        //    }
                        //    else
                        //    {
                        //        hex_Value = setting.numberDefault.ToString();
                        //    }
                        //    index_Value.Add(hex_Value);
                        //}

                        hex_Value = HexValues[setPointCounter].ToString();
                        hex_Value = Global.convertHexToNum(hex_Value, setting.conversion, setting.conversionOperation).ToString();

                        double numberValue = Convert.ToDouble(hex_Value);
                        if (!Global.isMCCBBackUp && !((decimal)numberValue % (decimal)setting.stepsize == 0 && numberValue >= setting.min && numberValue <= setting.max))
                        {
                            hex_Value = setting.numberDefault.ToString();
                        }
                        index_Value.Add(hex_Value);
                    }

                    else if (setting.type == Settings.Type.type_text)
                    {

                        string[] valueseperator = null;

                        if (setting.ID == Global.ipControl1)
                        {
                            string str = string.Empty;
                            for (int ip1 = 0; ip1 <= 3; ip1++)
                            {
                                hex_Value = (String)HexValues[setPointCounter];
                                if (str == string.Empty)
                                {
                                    str = Global.convertHexToString(hex_Value, setting.conversion); ;
                                }
                                else
                                {
                                    str = str + "." + Global.convertHexToString(hex_Value, setting.conversion);
                                }
                                setPointCounter++;
                            }
                            index_Value.Add(str);
                            continue;
                        }

                        else if (setting.ID == Global.ipControl2)
                        {
                            //string str = string.Empty;    //#COVARITY FIX     235042
                            hex_Value = (String)HexValues[setPointCounter];
                            string str = Global.convertHexToString(hex_Value, setting.conversion);
                            string Ipvalue2 = Global.ipMask + str;
                            index_Value.Add(Ipvalue2);
                        }

                        else if (setting.ID == Global.ipControl3)
                        {
                            Settings settingIp1 = (Settings)TripUnit.IDTable[Global.ipControl1];
                            valueseperator = settingIp1.IPaddress.Split('.');
                            string ip1ControlVal = valueseperator[0] + "." + valueseperator[1];
                            string str = string.Empty;

                            for (var ip3 = 0; ip3 <= 1; ip3++)
                            {
                                hex_Value = (String)HexValues[setPointCounter];
                                if (str == string.Empty)
                                {
                                    str = Global.convertHexToString(hex_Value, setting.conversion); ;
                                }
                                else
                                {
                                    str = str + "." + Global.convertHexToString(hex_Value, setting.conversion);
                                }
                                setPointCounter++;
                            }
                            string ipcontrol3 = ip1ControlVal + "." + str;
                            index_Value.Add(ipcontrol3);
                            continue;
                        }
                        else if (setting.ID == "SYS03")
                        {

                            index_Value.Add(Global.MCCB_TripUnitStyle);
                        }
                        else if (setting.ID == "SYS131C" || setting.ID == "SYS141C" || setting.ID == "SYS151C")
                        {
                            if (Global.IsOffline)
                            {
                                Settings Relay1 = (Settings)TripUnit.IDTable["SYS131C"];
                                Settings Relay2 = (Settings)TripUnit.IDTable["SYS141C"];
                                Settings Relay3 = (Settings)TripUnit.IDTable["SYS151C"];
                                Relay1.relayOriginalValuebackup = TripUnit.rawSetPoints[184].ToString() + " " + TripUnit.rawSetPoints[185].ToString() + " " + TripUnit.rawSetPoints[186] + " " + TripUnit.rawSetPoints[187];
                                Relay2.relayOriginalValuebackup = TripUnit.rawSetPoints[188].ToString() + " " + TripUnit.rawSetPoints[189].ToString() + " " + TripUnit.rawSetPoints[190] + " " + TripUnit.rawSetPoints[191];
                                Relay3.relayOriginalValuebackup = TripUnit.rawSetPoints[192].ToString() + " " + TripUnit.rawSetPoints[193].ToString() + " " + TripUnit.rawSetPoints[194] + " " + TripUnit.rawSetPoints[195];
                                index_Value.Add(((Settings)TripUnit.IDTable[setting.ID]).relayOriginalValuebackup);
                            }
                            else
                            {
                                index_Value.Add(((Settings)TripUnit.IDTable[setting.ID]).relayOriginalValue);
                            }
                        }
                        else
                        {
                            if (setting.value_map.Count > 0)
                            {
                                hex_Value = HexValues[setPointCounter].ToString();
                                if (setting.reversevalue_map[hex_Value] != null)
                                {
                                    index_Value.Add(setting.reversevalue_map[hex_Value]);
                                }
                                else
                                {
                                    hex_Value = Global.convertHexToString(hex_Value, setting.conversion);
                                    index_Value.Add(hex_Value);
                                }
                            }
                            else
                            {
                                hex_Value = HexValues[setPointCounter].ToString();
                                hex_Value = Global.convertHexToString(hex_Value, setting.conversion);
                                index_Value.Add(hex_Value);
                            }
                        }
                    }
                    setPointCounter++;

                }
            }
            return index_Value;
        }

        /// <summary>
        /// Fetch the corresponding key from value of hashtable
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="HT"></param>
        /// <returns></returns>
        public static string FindKey(string Value, Hashtable HT)
        {
            string Key = "";
            IDictionaryEnumerator e = HT.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Value.ToString().Equals(Value))
                {
                    Key = e.Key.ToString();
                }
            }
            return Key;
        }
    }

}
