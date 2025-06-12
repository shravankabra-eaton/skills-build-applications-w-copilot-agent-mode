using System;
using System.Xml;
using System.IO;
using System.Collections;
using PXR.Screens;
using PXR.Resources.Strings;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Vbe.Interop;

namespace PXR
{
    public static class XMLParser
    {
        /// <summary>
        /// Added by Astha
        /// Following method name is renamed as it no longer parses Rating plug xml .
        /// This method gives all the setpoint  values required to generate style in offline mode
        /// because now that xml has been merged into single xml 
        /// </summary>
         public static bool GetStyleandPlugCodes()
        {
            Settings setpoint;
            bool isError = false;
            try
            {
        
                Global.UnitTypeItems.Clear();
                if (Global.device_type == Global.ACB_PXR35_DEVICE)
                {
                    setpoint = TripUnit.getUnittypeGroup1ForPXR35();
                }
                else
                {
                    setpoint = TripUnit.getUnittypeGroup1ForACB();
                }
                foreach (item_ComboBox itemValue in setpoint.lookupTable.Values)
                {
                    Global.UnitTypeItems.Add(itemValue.item);
                }
                TripUnit.lookupTable_unitType = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_unitType.Add(entry.Key, item.item);
                }
                //  setpoint.selectionValue = Global.GlbstrUnitType;


                //Create a new table for plug codes
                setpoint = TripUnit.getRating();
                Global.UI_ratingPlugs.Clear();

                foreach (item_ComboBox itemValue in setpoint.lookupTable.Values)
                {
                    Global.UI_ratingPlugs.Add(itemValue.item);
                }
                Global.UI_ratingPlugs.Sort();
              //  setpoint.selectionValue = Global.GlbstrCurrentRating;
                TripUnit.lookupTable_plugCodes = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_plugCodes.Add(entry.Key, item.item);
                }

               // Create a new table for style code
                setpoint = TripUnit.getTripUnitStyle();
                TripUnit.lookupTable_styleCodes.Clear();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_styleCodes.Add(entry.Key, item.item);

                }

                setpoint = TripUnit.getGroundSensingType();
                TripUnit.lookupTable_GST = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_GST.Add(entry.Key, item.item);

                }
                Global.listofBreakerFrameItems.Clear();
                setpoint = TripUnit.getBreakerFrame();
                TripUnit.lookupTable_BreakerFrame = new Dictionary<String, String>();
                List<String> lv_strTempItem = new List<String>();
                foreach (String itemKey in setpoint.lookupTable.Keys)
                {
                    lv_strTempItem.Add(itemKey);
                }
                lv_strTempItem.Sort();
                foreach (String itemKey in lv_strTempItem)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)setpoint.lookupTable[itemKey];
                    string itemvalue = item.item;
                    TripUnit.lookupTable_BreakerFrame.Add(itemKey, itemvalue);
                    Global.listofBreakerFrameItems.Add(itemvalue);
                }

                setpoint = TripUnit.getLineFrequency();
                TripUnit.lookupTable_LineFrequency = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_LineFrequency.Add(entry.Key, item.item);

                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isError = true;
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, Resource.XMLParserError, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            return isError;

        }



        internal static bool GetStyleandPlugCodesForMCCB()
        {
            Settings setpoint;
            bool isError = false;
            try
            {

                Global.UnitTypeItems.Clear();
                setpoint = TripUnit.getUnittypeANSI_IECGeneralGrp();

                foreach (item_ComboBox itemValue in setpoint.lookupTable.Values)
                {
                    Global.UnitTypeItems.Add(itemValue.item);
                }
                //setpoint.selectionValue = Global.GlbstrUnitType;


                //Create a new table for plug codes
                setpoint = TripUnit.getRating();
                Global.UI_ratingPlugs.Clear();

                foreach (item_ComboBox itemValue in setpoint.lookupTable.Values)
                {
                    Global.UI_ratingPlugs.Add(itemValue.item);
                }
                Global.UI_ratingPlugs.Sort();
                // setpoint.selectionValue = Global.GlbstrCurrentRating;
                TripUnit.lookupTable_plugCodes = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_plugCodes.Add(entry.Key, item.item);
                }

             
                Global.listofBreakerFrameItems.Clear();
                setpoint = TripUnit.getBreakerFrame();               
               
                TripUnit.lookupTable_BreakerFrame = new Dictionary<String, String>();
                List<String> lv_strTempItem = new List<String>();
                foreach (String itemKey in setpoint.lookupTable.Keys)
                {
                    lv_strTempItem.Add(itemKey);
                }
                lv_strTempItem.Sort();
                foreach (String itemKey in lv_strTempItem)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)setpoint.lookupTable[itemKey];
                    string itemvalue = item.item;
                    TripUnit.lookupTable_BreakerFrame.Add(itemKey, itemvalue);
                    Global.listofBreakerFrameItems.Add(itemvalue);
                }

                setpoint = TripUnit.getLineFrequency();
                TripUnit.lookupTable_LineFrequency = new Hashtable();
                foreach (DictionaryEntry entry in setpoint.lookupTable)
                {
                    item_ComboBox item;
                    item = (item_ComboBox)entry.Value;
                    TripUnit.lookupTable_LineFrequency.Add(entry.Key, item.item);
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isError = true;
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, Resource.XMLParserError, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            return isError;

        }

        private static void resetIDTableAndList(String parseFor)
        {
            switch (parseFor)
            {
                case Global.str_app_ID_Table:
                    Global.valuemap.Clear();
                    if (TripUnit.IDTable.Count > 0)
                    {
                        TripUnit.IDTable.Clear();
                        TripUnit.ID_list.Clear();
                    }
                    if (TripUnit.groups.Count > 0)
                    {
                        TripUnit.groups.Clear();
                    }
                    break;
                case Global.str_pxset1_ID_Table:
                    if (TripUnit.Pxset1IDTable.Count > 0)
                    {
                        TripUnit.Pxset1IDTable.Clear();
                        TripUnit.ID_list_Pxset1.Clear();
                    }
                    if (TripUnit.Pxset1groups.Count > 0)
                    {
                        TripUnit.Pxset1groups.Clear();
                    }
                    break;
                case Global.str_pxset2_ID_Table:
                    if (TripUnit.Pxset2IDTable.Count > 0)
                    {
                        TripUnit.Pxset2IDTable.Clear();
                        TripUnit.ID_list_Pxset2.Clear();
                    }
                    if (TripUnit.Pxset2groups.Count > 0)
                    {
                        TripUnit.Pxset2groups.Clear();
                    }
                    break;
            }
        }


        private static void AddToIdTable(string ID, Settings settingsObj, String parseFor)
        {
            switch (parseFor)
            {
                case Global.str_app_ID_Table:

                    TripUnit.IDTable.Add(ID, settingsObj);
                    TripUnit.ID_list.Add(ID);
                  
                    break;
                case Global.str_pxset1_ID_Table:
                    TripUnit.Pxset1IDTable.Add(ID, settingsObj);
                    TripUnit.ID_list_Pxset1.Add(ID);

                    break;
                case Global.str_pxset2_ID_Table:
                    TripUnit.Pxset2IDTable.Add(ID, settingsObj);
                    TripUnit.ID_list_Pxset2.Add(ID);
                    break;
            }
        }

        private static void setDataForGroup(Group newGroup, String parseFor)
        {
            switch (parseFor)
            {
                case Global.str_app_ID_Table:
                    TripUnit.groups.Add(newGroup);

                    break;
                case Global.str_pxset1_ID_Table:
                    TripUnit.Pxset1groups.Add(newGroup);

                    break;
                case Global.str_pxset2_ID_Table:
                    TripUnit.Pxset2groups.Add(newGroup);
                    break;
            }           
        }

        private static void updateTripUnitValues(string parseFor)
        {
            switch (parseFor)
            {
                case Global.str_app_ID_Table:
                    TripUnit.setRating();
                    TripUnit.setBreakerFrame();
                    TripUnit.setTripUnitStyle();
                    break;
                case Global.str_pxset1_ID_Table:
                    TripUnit.setRating(true,false,false);
                    TripUnit.setBreakerFrame(true, false, false);
                    TripUnit.setTripUnitStyle(true, false, false);

                    break;
                case Global.str_pxset2_ID_Table:
                    TripUnit.setRating(false,true,false);
                    TripUnit.setBreakerFrame(false, true, false);
                    TripUnit.setTripUnitStyle(false, true, false);
                    break;
            }
        }

            /////////////////////////////////////////////////////////////////////////////////////
            /// Date:3-22-13
            /// Author: Sarah M. Norris
            /// <summary>
            /// This parses the model file that matches the trip unit style either selected 
            /// by the user or read in from the trip unit.
            /// </summary>
            /// <param name="filename">File and file path of xml model file</param>
            // commented by AK public
            //static void parseModelFile(String filename)
            public static bool parseModelFile(String filename, String parseFor = Global.str_app_ID_Table) // Added by AK. To find if any error while parsing file
        {

            updateTripUnitValues(parseFor);
            if (Global.parsed_Template_File == Global.selectedTemplateType && Global.parseFor_IDTable == parseFor) return true;

            //  TripUnit.IDTable.Clear();
            // TripUnit.groups.Clear();
            // TripUnit.ID_list.Clear();


            Global.selectedTemplateType = filename == Global.filePath_mergedstylesxmlFile ? Global.ACBTEMPLATE :
               (filename == Global.filePath_merged_PTM_xmlfile ? Global.PTM_TEMPLATE :
               (filename == Global.filePath_mergedstylesxmlFile_3_0 ? Global.ACB3_0TEMPLATE:
               (filename == Global.filePath_merged_acbPXR35_xmlFile ? Global.ACB_PXR35_TEMPLATE : 
               (filename == Global.filePath_merged_mccb_xmlFile ? Global.MCCBTEMPLATE : Global.NZMTEMPLATE))));

            if (Global.device_type == Global.PTM_DEVICE)
            {
                Global.selectedTemplateType = filename = Global.filePath_parameterSelectionFile_PTM;

            }
          
            if (parseFor == Global.str_app_ID_Table)
            {
                TripUnit.MMforExport = string.Empty;
                TripUnit.MMforExportForSave = null;
                if (Global.IsOffline && (!Global.IsOpenFile))
                {
                    TripUnit.MM16bitString = "0000000000000000";
                    if (TripUnit.MM16bitString != null)
                    {
                        TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                        TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                        TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                        TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                        TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                    }
                }
            }
            
            bool isParseSuccess = false;
            string idForError = null;
            try
            {
                // following 2 "If block" added by AK .
                // If user clicks "New file" on Main screen second time in same session then existing IDtbale and Groups should be cleard

                resetIDTableAndList(parseFor);              

                // extract model file from zip folder

                string PathForZipFolder = Global.filePath_basePath_ZIP + "DataFiles.zip";

                //FileInfo fileInfo = new FileInfo(Global.filePath_PXRxmlFile);
                //// FileInfo fileInfo = new FileInfo(Global.filePath_mergedstylesxmlFile);
                //string strFileNAme = fileInfo.Name;

                // bool isFileExist = false;

                /* using (var zip = ZipFile.Read(PathForZipFolder))
                 {


                     if (zip != null)
                     {
                         for (int i = 0; i < zip.Count; i++)
                         {
                             isFileExist = zip[i].FileName.Contains(strFileNAme);
                             if (isFileExist == true)
                                 break;
                         }

                         if (isFileExist)
                         {
                             zip.Password = Global.PASSWORD;
                             zip[strFileNAme].Extract(Global.filePath_basePath, ExtractExistingFileAction.OverwriteSilently);
                         }
                         else
                         {
                             isParseSuccess = false;
                             PXR.Screens.Wizard_Screen_MsgBox MsgBoxWindow = new PXR.Screens.Wizard_Screen_MsgBox("Load Model File", "Model file not found.", false);
                             MsgBoxWindow.Topmost = true;
                             MsgBoxWindow.ShowDialog();
                             return isParseSuccess;
                         }
                     }
                     else
                     {
                         isParseSuccess = false;
                         PXR.Screens.Wizard_Screen_MsgBox MsgBoxWindow = new PXR.Screens.Wizard_Screen_MsgBox("Load Model File", "Error loading Model file.", false);
                         MsgBoxWindow.Topmost = true;
                         MsgBoxWindow.ShowDialog();
                         return isParseSuccess;
                     }
                 }*/

                //

                
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
              
                //Commented By Sreejith
                ////Decryption logic -- Uncomment
                //if (!TriggerSecureXML.Decrypt(doc))
                //{
                //    //Some exception happened while decrypting the XML
                //    throw new Exception(TriggerSecureXML.ExceptionData);
                //}
               

                XmlNodeList list = doc.GetElementsByTagName("Group");
                foreach (XmlNode group in list)
                {
                    string resourceKey = "Group" + group.Attributes["ID"].Value + "Name";
                    Group newGroup;
                    if (true == Convert.ToBoolean(group.ChildNodes[1].Attributes["value"].Value))
                    {
                        if (/*"true" == group.Attributes["createSubgroup"].Value */ group.SelectSingleNode("SubGroup") != null)
                        {
                            //continue;
                            int subgroup_counter = group.ChildNodes.Count;
                            int grpsetpoint_counter = 0, subgrpS_counter = 0;
                            int numberOfSetpoints = 0;
                            XmlNodeList groupSetPoints = group.ChildNodes[2].Name == "SubGroup" ? group.ChildNodes[2].ChildNodes[2].ChildNodes : group.ChildNodes[2].ChildNodes;
                            if (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE)
                            {
                                groupSetPoints = group.ChildNodes[2].Name == "SubGroup" ? group.ChildNodes[2].ChildNodes[2].ChildNodes : group.ChildNodes[2].ChildNodes;
                                if(Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE)
                                {
                                    if(group.ChildNodes[2].ChildNodes[2].Name == "SubGroup")
                                    {
                                        groupSetPoints = group.ChildNodes[2].ChildNodes[2].ChildNodes[2].ChildNodes;
                                    }
                                }
                                //XmlNodeList groupSetPoints = group.ChildNodes[2].ChildNodes;
                                foreach (XmlNode setpoint in groupSetPoints)
                                {
                                    var visibleInPXM = setpoint.ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = setpoint.ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = setpoint.ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {
                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }

                                    else
                                    {
                                        numberOfSetpoints++;
                                    }
                                }
                            }

                            for (int cnt = 2; cnt < subgroup_counter; cnt++)
                            {
                                if (group.ChildNodes[cnt].Name == "SubGroup" && group.ChildNodes[cnt].Attributes["createSubgroup"].Value == "true")
                                {
                                    subgrpS_counter++;      // to count the number of subgroups in the group
                                }
                                else
                                {
                                    if (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE/*|| Global.selectedTemplateType == Global.ACB3_0TEMPLATE*/)
                                    {
                                        grpsetpoint_counter = numberOfSetpoints++;
                                    }
                                    else
                                    {
                                        grpsetpoint_counter = group.ChildNodes[cnt].ChildNodes[2].ChildNodes.Count + grpsetpoint_counter;     // to count the number of setpoints in the group
                                    }
                                }
                            }
                            //   subgroup_counter = subgrpS_counter;
                            int counter = 0;
                            int groupsetpoint_counter = 0;
                            int sequence_counter = 0;
                            newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), subgrpS_counter, grpsetpoint_counter, null, null);
                            //newGroup.groupSetPoints[i].GroupID = newGroup.ID;
                            newGroup.sequence = new string[grpsetpoint_counter + subgrpS_counter];

                            //for ACB device type, groupsetpoint are there in general group
                            if ((Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && newGroup.ID == "0")
                            {
                                int countForSetpointsOfGroup = 0;
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {

                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];      //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE && Global.selectedTemplateType != Global.ACB3_0TEMPLATE && Global.selectedTemplateType != Global.ACB_PXR35_TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";

                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                        sequence_counter++;
                                    }
                                }
                            }

                            //For subgroups inside group
                            for (int j = 2; j < subgroup_counter; j++)
                            {
                                if ((group.ChildNodes[j].Name == "SubGroup") && (group.ChildNodes[j].SelectSingleNode("SubGroup") != null) && (group.ChildNodes[j].Attributes["createSubgroup"].Value == "true"))

                                {
                                    // group containing the subgroups

                                    int subgroup_subCounter = group.ChildNodes[j].ChildNodes.Count;
                                    int visibleSubgroupsWithinSubgroupsCount = 0;       //Added by Astha to remove IO module if in xml template subgroup within a subgroup is invisible
                                    int sbgrpCounter = -1;
                                    for (int k = 2; k < subgroup_subCounter; k++)
                                    {
                                        if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[k].ChildNodes[1].Attributes["value"].Value))
                                        {
                                            visibleSubgroupsWithinSubgroupsCount++;
                                        }
                                    }
                                    resourceKey = "SubGroup" + group.ChildNodes[j].Attributes["ID"].Value + "Name";
                                    newGroup.subgroups[counter] = new Group(group.ChildNodes[j].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), visibleSubgroupsWithinSubgroupsCount, 0, null, null);
                                    newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                    for (int k = 2; k < subgroup_subCounter; k++)
                                    {

                                        resourceKey = "SubGroup" + group.ChildNodes[j].ChildNodes[k].Attributes["ID"].Value + "Name";
                                        if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[k].ChildNodes[1].Attributes["value"].Value))
                                        {
                                            // subgroup containing the subgroup i.e. two level group
                                            sbgrpCounter += 1;
                                            newGroup.subgroups[counter].subgroups[sbgrpCounter] = new Group(group.ChildNodes[j].ChildNodes[k].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, group.ChildNodes[j].ChildNodes[k].ChildNodes[2].ChildNodes.Count, null, null);
                                            XmlNodeList subgroupSetPoints = group.ChildNodes[j].ChildNodes[k].ChildNodes[2].ChildNodes;
                                            for (int i = 0; i < subgroupSetPoints.Count; i++)
                                            {

                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i] = new Settings();

                                                // SubGroup ID
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].GroupID = newGroup.ID;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_index = counter;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_sub_index = sbgrpCounter;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_setpoint_index = i;

                                                // ID
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID = subgroupSetPoints[i].Attributes["ID"].Value;
                                                idForError = newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID;

                                                //Trip Unit sequence
                                                var tripUnitsequence = subgroupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }

                                                //Keys are created in below line Ex: Group0Name
                                                resourceKey = newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID + "Name";

                                                //The strings are accessed from Resources for generated keys
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                // Visible (Determines is element is shown)
                                                // newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].visible = Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                                var visibleInPXM = subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].visible = (visibleInPXM != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                                // Readonly (Determines if user can change the value)
                                                // newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].readOnly = Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].readOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].onlineReadOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].showvalueInBothModes = (subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                                SettingsValue(ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i], subgroupSetPoints, i,parseFor);



                                                // Dependents [4]
                                                // Add dependents to setting. On a change in our value we will go through
                                                // this and do a dependency update. 
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].nodesForSingledependencies = subgroupSetPoints[i].ChildNodes[5].ChildNodes;
                                                setupDependents(subgroupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);

                                                // Dependencies [5]
                                                // Keep the depency in lists so that we can trickle down to the attributes
                                                // affected by a dependency change. 
                                                setupDependencies(subgroupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);

                                                // Description [6]
                                                //if ((subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                                //{
                                                //    resourceKey = newGroup.groupSetPoints[i].ID + "Tooltip";
                                                //    string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                //    strDesc = strDesc.Replace("\r\n", "");
                                                //    strDesc = strDesc.Replace("  ", "");
                                                //    strDesc = strDesc.Trim();
                                                //    newGroup.groupSetPoints[i].description = strDesc;
                                                //}
                                                // by adding them to this list we can then access the settings associated with 
                                                // the ID's and therefore trip an event
                                                AddToIdTable(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID, newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i], parseFor);
                                                //TripUnit.IDTable.Add(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID, newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);
                                                //TripUnit.ID_list.Add(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID);
                                            }
                                        }

                                    }
                                    counter++;
                                    sequence_counter++;

                                }
                                else if (group.ChildNodes[j].Name == "SubGroup" && group.ChildNodes[j].SelectSingleNode("SubGroup") == null && group.ChildNodes[j].Attributes["createSubgroup"].Value == "true")
                                {
                                    // subgroup containing the groupsetpoints only

                                    resourceKey = "SubGroup" + group.ChildNodes[j].Attributes["ID"].Value + "Name";
                                    if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[1].Attributes["value"].Value))
                                    {
                                        //counter = j - 2;
                                        newGroup.subgroups[counter] = new Group(group.ChildNodes[j].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, group.ChildNodes[j].ChildNodes[2].ChildNodes.Count, null, null);

                                        newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);


                                        XmlNodeList subgroupSetPoints = group.ChildNodes[j].ChildNodes[2].ChildNodes;
                                        for (int i = 0; i < subgroupSetPoints.Count; i++)
                                        {
                                            newGroup.subgroups[counter].groupSetPoints[i] = new Settings();

                                            // SubGroup ID
                                            newGroup.subgroups[counter].groupSetPoints[i].GroupID = newGroup.ID;
                                            newGroup.subgroups[counter].groupSetPoints[i].subgrp_index = counter;
                                            newGroup.subgroups[counter].groupSetPoints[i].subgrp_setpoint_index = i;

                                            // ID
                                            newGroup.subgroups[counter].groupSetPoints[i].ID = subgroupSetPoints[i].Attributes["ID"].Value;
                                            idForError = subgroupSetPoints[i].Attributes["ID"].Value;
                                            // newGroup.subgroups[counter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(subgroupSetPoints[i].Attributes["TRIPUNITSEQUENCE"].Value);
                                            //Trip Unit Sequence
                                            var tripUnitsequence = subgroupSetPoints[i].ChildNodes[7].InnerText;

                                            if (tripUnitsequence.Length > 2)
                                            {
                                                tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);

                                                newGroup.subgroups[counter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                            }


                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.subgroups[counter].groupSetPoints[i].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.subgroups[counter].groupSetPoints[i].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                            // Visible (Determines is element is shown)
                                            //  newGroup.subgroups[counter].groupSetPoints[i].visible = Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            // newGroup.subgroups[counter].groupSetPoints[i].readOnly = Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            var visibleInPXM = subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.subgroups[counter].groupSetPoints[i].visible = (visibleInPXM != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            //visibleInPXM = subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value;
                                            newGroup.subgroups[counter].groupSetPoints[i].readOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);

                                            //for disable the general group setpoints
                                            newGroup.subgroups[counter].groupSetPoints[i].onlineReadOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.subgroups[counter].groupSetPoints[i].showvalueInBothModes = (subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;

                                            //NotAvailable (Determines weather setpoint is available or not)
                                            if (subgroupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.subgroups[counter].groupSetPoints[i].notAvailable = Boolean.Parse(subgroupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }

                                            SettingsValue(ref newGroup.subgroups[counter].groupSetPoints[i], subgroupSetPoints, i,parseFor);



                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 

                                            newGroup.subgroups[counter].groupSetPoints[i].nodesForSingledependencies = subgroupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(subgroupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.subgroups[counter].groupSetPoints[i]);


                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(subgroupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.subgroups[counter].groupSetPoints[i]);

                                           // Description[6]
                                            if ((subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.subgroups[counter].groupSetPoints[i].ID + "Tooltip";
                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                
                                                if(!string.IsNullOrEmpty(strDesc))
                                                {
                                                    strDesc = strDesc.Replace("\r\n", "");
                                                    strDesc = strDesc.Replace("  ", "");
                                                    strDesc = strDesc.Trim();
                                                    newGroup.subgroups[counter].groupSetPoints[i].description = strDesc;
                                                }
                                               
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.subgroups[counter].groupSetPoints[i].ID, newGroup.subgroups[counter].groupSetPoints[i], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.subgroups[counter].groupSetPoints[i].ID, newGroup.subgroups[counter].groupSetPoints[i]);
                                            //TripUnit.ID_list.Add(newGroup.subgroups[counter].groupSetPoints[i].ID);
                                        }
                                    }
                                    counter++;
                                    sequence_counter++;
                                }
                                else if (group.ChildNodes[j].Name == "SubGroup" && group.ChildNodes[j].SelectSingleNode("SubGroup") == null && group.ChildNodes[j].Attributes["createSubgroup"].Value == "false")
                                {
                                    // setpoints inside group with subgroups

                                    groupSetPoints = group.ChildNodes[j].ChildNodes[2].ChildNodes;
                                    int groupSetPointsCount;
                                    if (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE/* || Global.selectedTemplateType == Global.ACB3_0TEMPLATE*/)
                                    {
                                        groupSetPointsCount = grpsetpoint_counter;
                                    }
                                    else
                                    {
                                        groupSetPointsCount = groupSetPoints.Count;
                                    }
                                    for (int i = 0; i < groupSetPointsCount; i++)
                                    {

                                        //groupsetpoint_counter = groupsetpoint_counter + i;
                                        newGroup.groupSetPoints[groupsetpoint_counter] = new Settings();

                                        // ID
                                        newGroup.groupSetPoints[groupsetpoint_counter].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        //Trip unit sequence
                                        var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;

                                        if (tripUnitsequence.Length > 2)
                                        {
                                            tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                            newGroup.groupSetPoints[groupsetpoint_counter].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                        }


                                        //  tripUnitsequence.Replace("", "");
                                        //   newGroup.groupSetPoints[groupsetpoint_counter].TripUnitSequence = Convert.ToInt32(groupSetPoints[i].Attributes["TRIPUNITSEQUENCE"].Value);                                       


                                        newGroup.groupSetPoints[groupsetpoint_counter].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[groupsetpoint_counter].grpsetpoint_index = groupsetpoint_counter;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith

                                        //Keys are created in below line Ex: Group0Name
                                        resourceKey = newGroup.groupSetPoints[groupsetpoint_counter].ID + "Name";

                                        //The strings are accessed from Resources for generated keys
                                        newGroup.groupSetPoints[groupsetpoint_counter].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                        //to store the sequence of the set points
                                        newGroup.sequence[sequence_counter] = newGroup.groupSetPoints[groupsetpoint_counter].name;

                                        // Visible (Determines is element is shown)
                                        //  newGroup.groupSetPoints[groupsetpoint_counter].visible = Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                        // Readonly (Determines if user can change the value)
                                        //     newGroup.groupSetPoints[groupsetpoint_counter].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                        var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                        newGroup.groupSetPoints[groupsetpoint_counter].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                        // Readonly (Determines if user can change the value)
                                        //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                        // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];   //#COVARITY FIX   234978
                                        newGroup.groupSetPoints[groupsetpoint_counter].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                        newGroup.groupSetPoints[groupsetpoint_counter].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                        newGroup.groupSetPoints[groupsetpoint_counter].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                        //NotAvailable (Determines weather setpoint is available or not)
                                        if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                        {
                                            newGroup.groupSetPoints[groupsetpoint_counter].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                        }

                                        SettingsValue(ref newGroup.groupSetPoints[groupsetpoint_counter], groupSetPoints, i, parseFor);

                                        // Dependents [4]
                                        // Add dependents to setting. On a change in our value we will go through
                                        // this and do a dependency update. 
                                        newGroup.groupSetPoints[groupsetpoint_counter].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                        setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[groupsetpoint_counter]);

                                        // Dependencies [5]
                                        // Keep the depency in lists so that we can trickle down to the attributes
                                        // affected by a dependency change. 
                                        setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[groupsetpoint_counter]);

                                        // Description [6]
                                        if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                        {
                                            resourceKey = newGroup.groupSetPoints[groupsetpoint_counter].ID + "Tooltip";
                                            string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                            if (strDesc != null)
                                            {
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                            }
                                            newGroup.groupSetPoints[groupsetpoint_counter].description = strDesc;
                                        }


                                        // by adding them to this list we can then access the settings associated with 
                                        // the ID's and therefore trip an event
                                        AddToIdTable(newGroup.groupSetPoints[groupsetpoint_counter].ID, newGroup.groupSetPoints[groupsetpoint_counter], parseFor);
                                        //TripUnit.IDTable.Add(newGroup.groupSetPoints[groupsetpoint_counter].ID, newGroup.groupSetPoints[groupsetpoint_counter]);
                                        //TripUnit.ID_list.Add(newGroup.groupSetPoints[groupsetpoint_counter].ID);
                                        groupsetpoint_counter++;
                                        sequence_counter++;
                                    }

                                }
                            }
                        }
                        else
                        {
                            //setpoints inside the group i.e. without subgroup

                            // newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey),0, group.ChildNodes[2].ChildNodes.Count,null,null);
                            XmlNodeList groupSetPoints = group.ChildNodes[2].ChildNodes;
                            int numberOfSetpoints = 0;
                            foreach (XmlNode setpoint in groupSetPoints)
                            {
                                var visibleInPXM = setpoint.ChildNodes[1].Attributes["visibleInPXPM"];
                                var parseInPXPM = setpoint.ChildNodes[1].Attributes["parseInPXPM"];
                                var parseForACB_2_1_XX = setpoint.ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                {
                                    continue;
                                }
                                else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                {
                                    continue;
                                }

                                else
                                {
                                    numberOfSetpoints++;
                                }
                            }
                            newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, numberOfSetpoints, null, null);
                            int countForSetpointsOfGroup = 0;
                            // Create the settings and then add them to the group
                            if (newGroup.ID == "1")
                            {
                                //for (int i = 3; i >=0 ; i--)
                                //{
                                //    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                //    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                //    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE|| Global.selectedTemplateType == Global.ACB3_0TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                //    {
                                //        continue;
                                //    }
                                //    else
                                //    {
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                //        // Group ID
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                //        // ID
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;

                                //        // Name (displayed on form)
                                //        //Modified to Read Group name from Resouce file -Sreejith
                                //        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                //        if (xmlAttributeCollection != null)
                                //        {
                                //            //Keys are created in below line Ex: Group0Name
                                //            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                //            //The strings are accessed from Resources for generated keys
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                //            // Visible (Determines is element is shown)
                                //            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                //            // Readonly (Determines if user can change the value)
                                //            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                //            visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                //            //trip unit sequence
                                //            if (Global.selectedTemplateType != Global.ACBTEMPLATE)
                                //            {
                                //                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                //                if (tripUnitsequence.Length > 2)
                                //                {
                                //                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                //                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                //                }
                                //            }
                                //            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i);


                                //            // Dependents [4]
                                //            // Add dependents to setting. On a change in our value we will go through
                                //            // this and do a dependency update. 
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                //            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                //            // Dependencies [5]
                                //            // Keep the depency in lists so that we can trickle down to the attributes
                                //            // affected by a dependency change. 
                                //            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                //            // Description [6]
                                //            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                //            {
                                //                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";
                                //                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                //                strDesc = strDesc.Replace("\r\n", "");
                                //                strDesc = strDesc.Replace("  ", "");
                                //                strDesc = strDesc.Trim();
                                //                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                //            }
                                //            // by adding them to this list we can then access the settings associated with 
                                //            // the ID's and therefore trip an event
                                //            TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                //            TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                //            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                //            {
                                //                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                //            }
                                //        }
                                //        countForSetpointsOfGroup++;
                                //    }
                                //}
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {
                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" &&( Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)2500 A
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];          //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE &&  Global.selectedTemplateType != Global.ACB3_0TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";
                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {

                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE ||  Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];      //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE &&  Global.selectedTemplateType != Global.ACB3_0TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";

                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                    }
                                }
                            }
                        }
                        setDataForGroup(newGroup, parseFor);                       
                    }
                    isParseSuccess = true;
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isParseSuccess = false;
                //MessageBox.Show("XMLParser:: " + ex.Message.ToString());                

                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, idForError +" : "+ ex.Message, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            Global.parsed_Template_File = Global.selectedTemplateType;
            Global.parseFor_IDTable = parseFor;
            return isParseSuccess;
        }

            public static bool parseModelFile_PTM(String filename, String parseFor = Global.str_app_ID_Table) // Added by AK. To find if any error while parsing file
        {

            updateTripUnitValues(parseFor);
            if (Global.parsed_Template_File == Global.selectedTemplateType && Global.parseFor_IDTable == parseFor) return true;

            //  TripUnit.IDTable.Clear();
            // TripUnit.groups.Clear();
            // TripUnit.ID_list.Clear();

            Global.selectedTemplateType = filename == Global.filePath_mergedstylesxmlFile ? Global.ACBTEMPLATE :
               (filename == Global.filePath_merged_PTM_xmlfile ? Global.PTM_TEMPLATE :
               (filename == Global.filePath_merged_mccb_xmlFile ? Global.MCCBTEMPLATE : Global.NZMTEMPLATE));

            if (parseFor == Global.str_app_ID_Table)
            {
                TripUnit.MMforExport = string.Empty;
                TripUnit.MMforExportForSave = null;
                if (Global.IsOffline && (!Global.IsOpenFile))
                {
                    TripUnit.MM16bitString = "0000000000000000";
                    if (TripUnit.MM16bitString != null)
                    {
                        TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                        TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                        TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                        TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                        TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                    }
                }
            }

            bool isParseSuccess = false;
            string idForError = null;
            try
            {
                // following 2 "If block" added by AK .
                // If user clicks "New file" on Main screen second time in same session then existing IDtbale and Groups should be cleard

                resetIDTableAndList(parseFor);

                // extract model file from zip folder

                string PathForZipFolder = Global.filePath_basePath_ZIP + "DataFiles.zip";

                //FileInfo fileInfo = new FileInfo(Global.filePath_PXRxmlFile);
                //// FileInfo fileInfo = new FileInfo(Global.filePath_mergedstylesxmlFile);
                //string strFileNAme = fileInfo.Name;

                // bool isFileExist = false;

                /* using (var zip = ZipFile.Read(PathForZipFolder))
                 {


                     if (zip != null)
                     {
                         for (int i = 0; i < zip.Count; i++)
                         {
                             isFileExist = zip[i].FileName.Contains(strFileNAme);
                             if (isFileExist == true)
                                 break;
                         }

                         if (isFileExist)
                         {
                             zip.Password = Global.PASSWORD;
                             zip[strFileNAme].Extract(Global.filePath_basePath, ExtractExistingFileAction.OverwriteSilently);
                         }
                         else
                         {
                             isParseSuccess = false;
                             PXR.Screens.Wizard_Screen_MsgBox MsgBoxWindow = new PXR.Screens.Wizard_Screen_MsgBox("Load Model File", "Model file not found.", false);
                             MsgBoxWindow.Topmost = true;
                             MsgBoxWindow.ShowDialog();
                             return isParseSuccess;
                         }
                     }
                     else
                     {
                         isParseSuccess = false;
                         PXR.Screens.Wizard_Screen_MsgBox MsgBoxWindow = new PXR.Screens.Wizard_Screen_MsgBox("Load Model File", "Error loading Model file.", false);
                         MsgBoxWindow.Topmost = true;
                         MsgBoxWindow.ShowDialog();
                         return isParseSuccess;
                     }
                 }*/

                //


                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                //Commented By Sreejith
                ////Decryption logic -- Uncomment
                //if (!TriggerSecureXML.Decrypt(doc))
                //{
                //    //Some exception happened while decrypting the XML
                //    throw new Exception(TriggerSecureXML.ExceptionData);
                //}


                XmlNodeList list = doc.GetElementsByTagName("Group");
                foreach (XmlNode group in list)
                {
                    string resourceKey = "Group" + group.Attributes["ID"].Value + "Name";
                    Group newGroup;
                    if (true == Convert.ToBoolean(group.ChildNodes[1].Attributes["value"].Value))
                    {
                        if (/*"true" == group.Attributes["createSubgroup"].Value */ group.SelectSingleNode("SubGroup") != null)
                        {
                            //continue;
                            int subgroup_counter = group.ChildNodes.Count;
                            int grpsetpoint_counter = 0, subgrpS_counter = 0;
                            int numberOfSetpoints = 0;
                            XmlNodeList groupSetPoints = group.ChildNodes[2].Name == "SubGroup" ? group.ChildNodes[2].ChildNodes[2].ChildNodes : group.ChildNodes[2].ChildNodes;
                            if (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE)
                            {
                                groupSetPoints = group.ChildNodes[2].Name == "SubGroup" ? group.ChildNodes[2].ChildNodes[2].ChildNodes : group.ChildNodes[2].ChildNodes;

                                //XmlNodeList groupSetPoints = group.ChildNodes[2].ChildNodes;
                                foreach (XmlNode setpoint in groupSetPoints)
                                {
                                    var visibleInPXM = setpoint.ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = setpoint.ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = setpoint.ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {
                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }

                                    else
                                    {
                                        numberOfSetpoints++;
                                    }
                                }
                            }

                            for (int cnt = 2; cnt < subgroup_counter; cnt++)
                            {
                                if (group.ChildNodes[cnt].Name == "SubGroup" && group.ChildNodes[cnt].Attributes["createSubgroup"].Value == "true")
                                {
                                    subgrpS_counter++;      // to count the number of subgroups in the group
                                }
                                else
                                {
                                    if (Global.selectedTemplateType == Global.ACBTEMPLATE /*|| Global.selectedTemplateType == Global.ACB3_0TEMPLATE*/)
                                    {
                                        grpsetpoint_counter = numberOfSetpoints++;
                                    }
                                    else
                                    {
                                        grpsetpoint_counter = group.ChildNodes[cnt].ChildNodes[2].ChildNodes.Count + grpsetpoint_counter;     // to count the number of setpoints in the group
                                    }
                                }
                            }
                            //   subgroup_counter = subgrpS_counter;
                            int counter = 0;
                            int groupsetpoint_counter = 0;
                            int sequence_counter = 0;
                            newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), subgrpS_counter, grpsetpoint_counter, null, null);
                            //newGroup.groupSetPoints[i].GroupID = newGroup.ID;
                            newGroup.sequence = new string[grpsetpoint_counter + subgrpS_counter];

                            //for ACB device type, groupsetpoint are there in general group
                            if (Global.selectedTemplateType == Global.ACBTEMPLATE && newGroup.ID == "0")
                            {
                                int countForSetpointsOfGroup = 0;
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {

                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];      //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";

                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                        sequence_counter++;
                                    }
                                }
                            }

                            //For subgroups inside group
                            for (int j = 2; j < subgroup_counter; j++)
                            {
                                if ((group.ChildNodes[j].Name == "SubGroup") && (group.ChildNodes[j].SelectSingleNode("SubGroup") != null) && (group.ChildNodes[j].Attributes["createSubgroup"].Value == "true"))

                                {
                                    // group containing the subgroups

                                    int subgroup_subCounter = group.ChildNodes[j].ChildNodes.Count;
                                    int visibleSubgroupsWithinSubgroupsCount = 0;       //Added by Astha to remove IO module if in xml template subgroup within a subgroup is invisible
                                    int sbgrpCounter = -1;
                                    for (int k = 2; k < subgroup_subCounter; k++)
                                    {
                                        if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[k].ChildNodes[1].Attributes["value"].Value))
                                        {
                                            visibleSubgroupsWithinSubgroupsCount++;
                                        }
                                    }
                                    resourceKey = "SubGroup" + group.ChildNodes[j].Attributes["ID"].Value + "Name";
                                    newGroup.subgroups[counter] = new Group(group.ChildNodes[j].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), visibleSubgroupsWithinSubgroupsCount, 0, null, null);
                                    newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                    for (int k = 2; k < subgroup_subCounter; k++)
                                    {

                                        resourceKey = "SubGroup" + group.ChildNodes[j].ChildNodes[k].Attributes["ID"].Value + "Name";
                                        if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[k].ChildNodes[1].Attributes["value"].Value))
                                        {
                                            // subgroup containing the subgroup i.e. two level group
                                            sbgrpCounter += 1;
                                            newGroup.subgroups[counter].subgroups[sbgrpCounter] = new Group(group.ChildNodes[j].ChildNodes[k].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, group.ChildNodes[j].ChildNodes[k].ChildNodes[2].ChildNodes.Count, null, null);
                                            XmlNodeList subgroupSetPoints = group.ChildNodes[j].ChildNodes[k].ChildNodes[2].ChildNodes;
                                            for (int i = 0; i < subgroupSetPoints.Count; i++)
                                            {

                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i] = new Settings();

                                                // SubGroup ID
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].GroupID = newGroup.ID;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_index = counter;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_sub_index = sbgrpCounter;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].subgrp_setpoint_index = i;

                                                // ID
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID = subgroupSetPoints[i].Attributes["ID"].Value;
                                                idForError = newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID;

                                                //Trip Unit sequence
                                                var tripUnitsequence = subgroupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }

                                                //Keys are created in below line Ex: Group0Name
                                                resourceKey = newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID + "Name";

                                                //The strings are accessed from Resources for generated keys
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                // Visible (Determines is element is shown)
                                                // newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].visible = Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                                var visibleInPXM = subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].visible = (visibleInPXM != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                                // Readonly (Determines if user can change the value)
                                                // newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].readOnly = Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].readOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].onlineReadOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].showvalueInBothModes = (subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                                SettingsValue(ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i], subgroupSetPoints, i, parseFor);



                                                // Dependents [4]
                                                // Add dependents to setting. On a change in our value we will go through
                                                // this and do a dependency update. 
                                                newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].nodesForSingledependencies = subgroupSetPoints[i].ChildNodes[5].ChildNodes;
                                                setupDependents(subgroupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);

                                                // Dependencies [5]
                                                // Keep the depency in lists so that we can trickle down to the attributes
                                                // affected by a dependency change. 
                                                setupDependencies(subgroupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);

                                                // Description [6]
                                                //if ((subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                                //{
                                                //    resourceKey = newGroup.groupSetPoints[i].ID + "Tooltip";
                                                //    string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                //    strDesc = strDesc.Replace("\r\n", "");
                                                //    strDesc = strDesc.Replace("  ", "");
                                                //    strDesc = strDesc.Trim();
                                                //    newGroup.groupSetPoints[i].description = strDesc;
                                                //}
                                                // by adding them to this list we can then access the settings associated with 
                                                // the ID's and therefore trip an event
                                                AddToIdTable(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID, newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i], parseFor);
                                                //TripUnit.IDTable.Add(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID, newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i]);
                                                //TripUnit.ID_list.Add(newGroup.subgroups[counter].subgroups[sbgrpCounter].groupSetPoints[i].ID);
                                            }
                                        }

                                    }
                                    counter++;
                                    sequence_counter++;

                                }
                                else if (group.ChildNodes[j].Name == "SubGroup" && group.ChildNodes[j].SelectSingleNode("SubGroup") == null && group.ChildNodes[j].Attributes["createSubgroup"].Value == "true")
                                {
                                    // subgroup containing the groupsetpoints only

                                    resourceKey = "SubGroup" + group.ChildNodes[j].Attributes["ID"].Value + "Name";
                                    if (true == Convert.ToBoolean(group.ChildNodes[j].ChildNodes[1].Attributes["value"].Value))
                                    {
                                        //counter = j - 2;
                                        newGroup.subgroups[counter] = new Group(group.ChildNodes[j].Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, group.ChildNodes[j].ChildNodes[2].ChildNodes.Count, null, null);

                                        newGroup.sequence[sequence_counter] = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);


                                        XmlNodeList subgroupSetPoints = group.ChildNodes[j].ChildNodes[2].ChildNodes;
                                        for (int i = 0; i < subgroupSetPoints.Count; i++)
                                        {
                                            newGroup.subgroups[counter].groupSetPoints[i] = new Settings();

                                            // SubGroup ID
                                            newGroup.subgroups[counter].groupSetPoints[i].GroupID = newGroup.ID;
                                            newGroup.subgroups[counter].groupSetPoints[i].subgrp_index = counter;
                                            newGroup.subgroups[counter].groupSetPoints[i].subgrp_setpoint_index = i;

                                            // ID
                                            newGroup.subgroups[counter].groupSetPoints[i].ID = subgroupSetPoints[i].Attributes["ID"].Value;
                                            idForError = subgroupSetPoints[i].Attributes["ID"].Value;
                                            // newGroup.subgroups[counter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(subgroupSetPoints[i].Attributes["TRIPUNITSEQUENCE"].Value);
                                            //Trip Unit Sequence
                                            var tripUnitsequence = subgroupSetPoints[i].ChildNodes[7].InnerText;

                                            if (tripUnitsequence.Length > 2)
                                            {
                                                tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);

                                                newGroup.subgroups[counter].groupSetPoints[i].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                            }


                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.subgroups[counter].groupSetPoints[i].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.subgroups[counter].groupSetPoints[i].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                            // Visible (Determines is element is shown)
                                            //  newGroup.subgroups[counter].groupSetPoints[i].visible = Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            // newGroup.subgroups[counter].groupSetPoints[i].readOnly = Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            var visibleInPXM = subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.subgroups[counter].groupSetPoints[i].visible = (visibleInPXM != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            //visibleInPXM = subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value;
                                            newGroup.subgroups[counter].groupSetPoints[i].readOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["value"].Value);

                                            //for disable the general group setpoints
                                            newGroup.subgroups[counter].groupSetPoints[i].onlineReadOnly = (subgroupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.subgroups[counter].groupSetPoints[i].showvalueInBothModes = (subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(subgroupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;

                                            //NotAvailable (Determines weather setpoint is available or not)
                                            if (subgroupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.subgroups[counter].groupSetPoints[i].notAvailable = Boolean.Parse(subgroupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }

                                            SettingsValue(ref newGroup.subgroups[counter].groupSetPoints[i], subgroupSetPoints, i, parseFor);



                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 

                                            newGroup.subgroups[counter].groupSetPoints[i].nodesForSingledependencies = subgroupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(subgroupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.subgroups[counter].groupSetPoints[i]);


                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(subgroupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.subgroups[counter].groupSetPoints[i]);

                                            // Description [6]
                                            //if ((subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (subgroupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            //{
                                            //    resourceKey = newGroup.groupSetPoints[i].ID + "Tooltip";
                                            //    string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                            //    strDesc = strDesc.Replace("\r\n", "");
                                            //    strDesc = strDesc.Replace("  ", "");
                                            //    strDesc = strDesc.Trim();
                                            //    newGroup.groupSetPoints[i].description = strDesc;
                                            //}
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.subgroups[counter].groupSetPoints[i].ID, newGroup.subgroups[counter].groupSetPoints[i], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.subgroups[counter].groupSetPoints[i].ID, newGroup.subgroups[counter].groupSetPoints[i]);
                                            //TripUnit.ID_list.Add(newGroup.subgroups[counter].groupSetPoints[i].ID);
                                        }
                                    }
                                    counter++;
                                    sequence_counter++;
                                }
                                else if (group.ChildNodes[j].Name == "SubGroup" && group.ChildNodes[j].SelectSingleNode("SubGroup") == null && group.ChildNodes[j].Attributes["createSubgroup"].Value == "false")
                                {
                                    // setpoints inside group with subgroups

                                    groupSetPoints = group.ChildNodes[j].ChildNodes[2].ChildNodes;
                                    int groupSetPointsCount;
                                    if (Global.selectedTemplateType == Global.ACBTEMPLATE/* || Global.selectedTemplateType == Global.ACB3_0TEMPLATE*/)
                                    {
                                        groupSetPointsCount = grpsetpoint_counter;
                                    }
                                    else
                                    {
                                        groupSetPointsCount = groupSetPoints.Count;
                                    }
                                    for (int i = 0; i < groupSetPointsCount; i++)
                                    {

                                        //groupsetpoint_counter = groupsetpoint_counter + i;
                                        newGroup.groupSetPoints[groupsetpoint_counter] = new Settings();

                                        // ID
                                        newGroup.groupSetPoints[groupsetpoint_counter].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        //Trip unit sequence
                                        var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;

                                        if (tripUnitsequence.Length > 2)
                                        {
                                            tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                            newGroup.groupSetPoints[groupsetpoint_counter].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                        }


                                        //  tripUnitsequence.Replace("", "");
                                        //   newGroup.groupSetPoints[groupsetpoint_counter].TripUnitSequence = Convert.ToInt32(groupSetPoints[i].Attributes["TRIPUNITSEQUENCE"].Value);                                       


                                        newGroup.groupSetPoints[groupsetpoint_counter].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[groupsetpoint_counter].grpsetpoint_index = groupsetpoint_counter;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith

                                        //Keys are created in below line Ex: Group0Name
                                        resourceKey = newGroup.groupSetPoints[groupsetpoint_counter].ID + "Name";
                                        
                                        if (Global.device_type == Global.PTM_DEVICE && (newGroup.groupSetPoints[groupsetpoint_counter].ID == "GEN002" || 
                                            newGroup.groupSetPoints[groupsetpoint_counter].ID == "GEN002B" || 
                                            newGroup.groupSetPoints[groupsetpoint_counter].ID == "GEN002C"))
                                        {
                                            resourceKey = newGroup.groupSetPoints[groupsetpoint_counter].ID +"00"+ "Name";
                                        }

                                        //The strings are accessed from Resources for generated keys
                                        newGroup.groupSetPoints[groupsetpoint_counter].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                        //to store the sequence of the set points
                                        newGroup.sequence[sequence_counter] = newGroup.groupSetPoints[groupsetpoint_counter].name;

                                        // Visible (Determines is element is shown)
                                        //  newGroup.groupSetPoints[groupsetpoint_counter].visible = Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                        // Readonly (Determines if user can change the value)
                                        //     newGroup.groupSetPoints[groupsetpoint_counter].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                        var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                        newGroup.groupSetPoints[groupsetpoint_counter].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                        // Readonly (Determines if user can change the value)
                                        //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                        // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];   //#COVARITY FIX   234978
                                        newGroup.groupSetPoints[groupsetpoint_counter].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                        newGroup.groupSetPoints[groupsetpoint_counter].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                        newGroup.groupSetPoints[groupsetpoint_counter].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                        //NotAvailable (Determines weather setpoint is available or not)
                                        if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                        {
                                            newGroup.groupSetPoints[groupsetpoint_counter].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                        }

                                        SettingsValue(ref newGroup.groupSetPoints[groupsetpoint_counter], groupSetPoints, i, parseFor);

                                        // Dependents [4]
                                        // Add dependents to setting. On a change in our value we will go through
                                        // this and do a dependency update. 
                                        newGroup.groupSetPoints[groupsetpoint_counter].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                        setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[groupsetpoint_counter]);

                                        // Dependencies [5]
                                        // Keep the depency in lists so that we can trickle down to the attributes
                                        // affected by a dependency change. 
                                        setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[groupsetpoint_counter]);

                                        // Description [6]
                                        if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                        {
                                            //resourceKey = newGroup.groupSetPoints[groupsetpoint_counter].ID + "Tooltip";
                                            //string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                            //strDesc = strDesc.Replace("\r\n", "");
                                            //strDesc = strDesc.Replace("  ", "");
                                            //strDesc = strDesc.Trim();
                                            //newGroup.groupSetPoints[groupsetpoint_counter].description = strDesc;
                                        }


                                        // by adding them to this list we can then access the settings associated with 
                                        // the ID's and therefore trip an event
                                        AddToIdTable(newGroup.groupSetPoints[groupsetpoint_counter].ID, newGroup.groupSetPoints[groupsetpoint_counter], parseFor);
                                        //TripUnit.IDTable.Add(newGroup.groupSetPoints[groupsetpoint_counter].ID, newGroup.groupSetPoints[groupsetpoint_counter]);
                                        //TripUnit.ID_list.Add(newGroup.groupSetPoints[groupsetpoint_counter].ID);
                                        groupsetpoint_counter++;
                                        sequence_counter++;
                                    }

                                }
                            }
                        }
                        else
                        {
                            //setpoints inside the group i.e. without subgroup

                            // newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey),0, group.ChildNodes[2].ChildNodes.Count,null,null);
                            XmlNodeList groupSetPoints = group.ChildNodes[2].ChildNodes;
                            int numberOfSetpoints = 0;
                            foreach (XmlNode setpoint in groupSetPoints)
                            {
                                var visibleInPXM = setpoint.ChildNodes[1].Attributes["visibleInPXPM"];
                                var parseInPXPM = setpoint.ChildNodes[1].Attributes["parseInPXPM"];
                                var parseForACB_2_1_XX = setpoint.ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                {
                                    continue;
                                }
                                else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                {
                                    continue;
                                }

                                else
                                {
                                    numberOfSetpoints++;
                                }
                            }
                            newGroup = new Group(group.Attributes["ID"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey), 0, numberOfSetpoints, null, null);
                            int countForSetpointsOfGroup = 0;
                            // Create the settings and then add them to the group
                            if (newGroup.ID == "1")
                            {
                                //for (int i = 3; i >=0 ; i--)
                                //{
                                //    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                //    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                //    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE|| Global.selectedTemplateType == Global.ACB3_0TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                //    {
                                //        continue;
                                //    }
                                //    else
                                //    {
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                //        // Group ID
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                //        // ID
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                //        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;

                                //        // Name (displayed on form)
                                //        //Modified to Read Group name from Resouce file -Sreejith
                                //        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                //        if (xmlAttributeCollection != null)
                                //        {
                                //            //Keys are created in below line Ex: Group0Name
                                //            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                //            //The strings are accessed from Resources for generated keys
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                //            // Visible (Determines is element is shown)
                                //            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                //            // Readonly (Determines if user can change the value)
                                //            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                //            visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                //            //trip unit sequence
                                //            if (Global.selectedTemplateType != Global.ACBTEMPLATE)
                                //            {
                                //                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                //                if (tripUnitsequence.Length > 2)
                                //                {
                                //                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                //                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                //                }
                                //            }
                                //            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i);


                                //            // Dependents [4]
                                //            // Add dependents to setting. On a change in our value we will go through
                                //            // this and do a dependency update. 
                                //            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                //            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                //            // Dependencies [5]
                                //            // Keep the depency in lists so that we can trickle down to the attributes
                                //            // affected by a dependency change. 
                                //            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                //            // Description [6]
                                //            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                //            {
                                //                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";
                                //                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                //                strDesc = strDesc.Replace("\r\n", "");
                                //                strDesc = strDesc.Replace("  ", "");
                                //                strDesc = strDesc.Trim();
                                //                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                //            }
                                //            // by adding them to this list we can then access the settings associated with 
                                //            // the ID's and therefore trip an event
                                //            TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                //            TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                //            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                //            {
                                //                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                //            }
                                //        }
                                //        countForSetpointsOfGroup++;
                                //    }
                                //}
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {
                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];          //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";
                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < groupSetPoints.Count; i++)
                                {

                                    var visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                    var parseInPXPM = groupSetPoints[i].ChildNodes[1].Attributes["parseInPXPM"];
                                    var parseForACB_2_1_XX = groupSetPoints[i].ChildNodes[1].Attributes["parseForACB_2_1_XX"];
                                    if (visibleInPXM != null && visibleInPXM.Value == "false" && (Global.selectedTemplateType == Global.ACBTEMPLATE || Global.selectedTemplateType == Global.PTM_TEMPLATE) && (parseInPXPM == null || parseInPXPM.Value == "false"))
                                    {

                                        continue;
                                    }
                                    else if ((/*Global.deviceFirmware == Resource.GEN002Item0000 || Global.deviceFirmware == Resource.GEN002Item0001 ||*/ Global.device_type == Global.ACBDEVICE) && parseForACB_2_1_XX != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {

                                        newGroup.groupSetPoints[countForSetpointsOfGroup] = new Settings();

                                        // Group ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].GroupID = newGroup.ID;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].index = countForSetpointsOfGroup;

                                        // ID
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].ID = groupSetPoints[i].Attributes["ID"].Value;
                                        idForError = groupSetPoints[i].Attributes["ID"].Value;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseInPXPM = parseInPXPM != null ? Boolean.Parse(parseInPXPM.Value) : false;
                                        newGroup.groupSetPoints[countForSetpointsOfGroup].parseForACB_2_1_XX = parseForACB_2_1_XX != null ? Boolean.Parse(parseForACB_2_1_XX.Value) : false;
                                        // Name (displayed on form)
                                        //Modified to Read Group name from Resouce file -Sreejith
                                        var xmlAttributeCollection = groupSetPoints[i].Attributes;
                                        if (xmlAttributeCollection != null)
                                        {
                                            //Keys are created in below line Ex: Group0Name
                                            resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Name";

                                            //The strings are accessed from Resources for generated keys
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                                            // Visible (Determines is element is shown)
                                            visibleInPXM = groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"];
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].visible = (visibleInPXM != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["visibleInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[1].Attributes["value"].Value);
                                            // Readonly (Determines if user can change the value)
                                            //newGroup.groupSetPoints[i].readOnly = Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value);
                                            // visibleInPXM = groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"];      //#COVARITY FIX     234978
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].readOnly = (groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["readOnlyInPXPM"].Value) : Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["value"].Value);
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].onlineReadOnly = (groupSetPoints[i].ChildNodes[2].Attributes["device"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["device"].Value) : false;
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].showvalueInBothModes = (groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"] != null) ? Boolean.Parse(groupSetPoints[i].ChildNodes[2].Attributes["showValueInPXPMOfflineOnline"].Value) : false;
                                            //trip unit sequence
                                            if (Global.selectedTemplateType != Global.ACBTEMPLATE && Global.selectedTemplateType != Global.PTM_TEMPLATE)
                                            {
                                                var tripUnitsequence = groupSetPoints[i].ChildNodes[7].InnerText;
                                                if (tripUnitsequence.Length > 2)
                                                {
                                                    tripUnitsequence = tripUnitsequence.Substring(1, tripUnitsequence.Length - 2);
                                                    newGroup.groupSetPoints[countForSetpointsOfGroup].TripUnitSequence = Convert.ToInt32(tripUnitsequence);
                                                }
                                            }
                                            SettingsValue(ref newGroup.groupSetPoints[countForSetpointsOfGroup], groupSetPoints, i, parseFor);


                                            // Dependents [4]
                                            // Add dependents to setting. On a change in our value we will go through
                                            // this and do a dependency update. 
                                            newGroup.groupSetPoints[countForSetpointsOfGroup].nodesForSingledependencies = groupSetPoints[i].ChildNodes[5].ChildNodes;
                                            setupDependents(groupSetPoints[i].ChildNodes[4].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Dependencies [5]
                                            // Keep the depency in lists so that we can trickle down to the attributes
                                            // affected by a dependency change. 
                                            setupDependencies(groupSetPoints[i].ChildNodes[5].ChildNodes, ref newGroup.groupSetPoints[countForSetpointsOfGroup]);

                                            // Description [6]
                                            if ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "split" && (groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower() != "selection")
                                            {
                                                resourceKey = newGroup.groupSetPoints[countForSetpointsOfGroup].ID + "Tooltip";

                                                string strDesc = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                                                strDesc = strDesc.Replace("\r\n", "");
                                                strDesc = strDesc.Replace("  ", "");
                                                strDesc = strDesc.Trim();
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].description = strDesc;
                                            }
                                            // by adding them to this list we can then access the settings associated with 
                                            // the ID's and therefore trip an event
                                            AddToIdTable(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup], parseFor);
                                            //TripUnit.IDTable.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID, newGroup.groupSetPoints[countForSetpointsOfGroup]);
                                            //TripUnit.ID_list.Add(newGroup.groupSetPoints[countForSetpointsOfGroup].ID);
                                            if (groupSetPoints[i].LastChild.Name == "NotAvailable")
                                            {
                                                newGroup.groupSetPoints[countForSetpointsOfGroup].notAvailable = Boolean.Parse(groupSetPoints[i].LastChild.Attributes["value"].Value);
                                            }
                                        }
                                        countForSetpointsOfGroup++;
                                    }
                                }
                            }
                        }
                        setDataForGroup(newGroup, parseFor);
                    }
                    isParseSuccess = true;
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isParseSuccess = false;
                //MessageBox.Show("XMLParser:: " + ex.Message.ToString());                

                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, idForError + " : " + ex.Message, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            Global.parsed_Template_File = Global.selectedTemplateType;
            Global.parseFor_IDTable = parseFor;
            return isParseSuccess;
        }


        public static bool parseParameterSelectionFile(string filename)
        {
            bool isParseSuccess = false;
            try
            {  
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                TripUnit.parameterSelectionGroups.Clear();
                string ID;
                string[] ACBselectionItem = null;
                string[] MCCBselectionItem =null;
                string[] NZMselectionItem = null;
                string[] ACBPXR35selectionItem = null;
                string[] PTMselectionItem = null;

                XmlNodeList list = doc.GetElementsByTagName("ComboBoxEntry");
                foreach (XmlNode group in list)
                {                   
                    parameterSelectionGroup newGroup;


                    XmlAttribute idAttribute = group.Attributes["ID"];
                    if (idAttribute != null)
                    {
                        ID = idAttribute.Value;

                        foreach (XmlNode node in group.ChildNodes)
                        {
                            int i = 0;
                            if (node.Name == "ACB")
                            {
                                ACBselectionItem = new string[node.ChildNodes.Count];
                                foreach (XmlNode acbChildNode in node.ChildNodes)
                                {
                                    ACBselectionItem[i] = acbChildNode.InnerText;
                                    i++;
                                }
                            }
                            else if (node.Name == "MCCB")
                            {
                                MCCBselectionItem = new string[node.ChildNodes.Count];
                                foreach (XmlNode mccbChildNode in node.ChildNodes)
                                {
                                    MCCBselectionItem[i] = mccbChildNode.InnerText;
                                    i++;
                                }
                            }
                            else if (node.Name == "NZM")
                            {
                                NZMselectionItem = new string[node.ChildNodes.Count];
                                foreach (XmlNode nzmChildNode in node.ChildNodes)
                                {
                                    NZMselectionItem[i] = nzmChildNode.InnerText;
                                    i++;
                                }
                            }
                            else if (node.Name == "ACB_PXR35")
                            {
                                ACBPXR35selectionItem = new string[node.ChildNodes.Count];
                                foreach (XmlNode acbPXR35ChildNode in node.ChildNodes)
                                {
                                    ACBPXR35selectionItem[i] = acbPXR35ChildNode.InnerText;
                                    i++;
                                }
                            }
                            else if (node.Name == "PTM")
                            {
                                PTMselectionItem = new string[node.ChildNodes.Count];
                                foreach (XmlNode ptmSelectionItem in node.ChildNodes)
                                {
                                    PTMselectionItem[i] = ptmSelectionItem.InnerText;
                                    i++;
                                }
                            }
                        }
                        newGroup = new parameterSelectionGroup(ID, ACBselectionItem, MCCBselectionItem, NZMselectionItem, ACBPXR35selectionItem, PTMselectionItem);
                        TripUnit.parameterSelectionGroups.Add(newGroup);
                        isParseSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
                isParseSuccess = false;
                Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, ex.Message, "", false);
                MsgBoxWindow.Topmost = true;
                MsgBoxWindow.ShowDialog();
            }
            return isParseSuccess;
        }


        private static void SettingsValue( ref Settings setPoint, XmlNodeList groupSetPoints, int i, String parseFor = Global.str_app_ID_Table)
        {
            string resourceKey;
            string valueMap_data;
            string[] valueMap_list;
            string strDescSel;
            double conversion;
            string resourceKeyForsetpointOnLabel;
            string resourceKeyForsetpointOffLabel;
            System.Globalization.CultureInfo EnglishCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo GermanCulture = new System.Globalization.CultureInfo("de-DE");
            // Type
            try
            {
                switch ((groupSetPoints[i].ChildNodes[3].Attributes["value"].Value).ToLower())
                {
                    case "skip":
                        setPoint.type = Settings.Type.type_skip;
                        //  setPoint.rawValue = ""; 
                        setPoint.numberDefault = Int32.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        break;
                    case "rplugstyle":
                        setPoint.type = Settings.Type.type_rPlugStyle;
                        setPoint.setpoint = new Settings[groupSetPoints[i].ChildNodes[3].ChildNodes.Count];

                        XmlNode _ratingPlugNode = groupSetPoints[i].ChildNodes[3].ChildNodes[0];
                        Settings setpoint_ratingPlug = new Settings();
                        setpoint_ratingPlug.ID = _ratingPlugNode.Attributes["ID"].Value;
                        setpoint_ratingPlug.name = _ratingPlugNode.ChildNodes[0].InnerText;
                        setpoint_ratingPlug.type = Settings.Type.type_selection; // we want dependencies to look at the selection value. 
                        setpoint_ratingPlug.selectionValue = TripUnit.userRatingPlug;
                        setupDependents(_ratingPlugNode.ChildNodes[1].ChildNodes, ref setpoint_ratingPlug);
                        TripUnit.IDTable.Add(setpoint_ratingPlug.ID, setpoint_ratingPlug);
                        break;
                    case "bool":
                        setPoint.type = Settings.Type.type_bool;
                        setPoint.bDefault = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.bValueReadFromTripUnit = setPoint.bDefault;
                        break;
                    case "number":
                        setPoint.type = Settings.Type.type_number;
                        setPoint.numberDefault = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);                        
                        setPoint.conversion = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText.Trim().Split(' ')[0], CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.conversionOperation = groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText.Trim().Split(' ').Count() ==  1 ? "*" : groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText.Trim().Split(' ')[1];
                        setPoint.min = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.max = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.strStepSize = groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText;
                        setPoint.stepsize = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        //setPoint.stepsize = Convert.ToDouble(setPoint.stepsize, CultureInfo.CurrentUICulture.NumberFormat);
                        if (Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[5].Attributes["runCalc"].Value))
                        {
                            setPoint.bcalculated = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[5].Attributes["runCalc"].Value);
                            setPoint.calculation = groupSetPoints[i].ChildNodes[3].ChildNodes[5].InnerText;
                        }
                        setPoint.MinCalculation = groupSetPoints[i].ChildNodes[3].ChildNodes[7] == null ? "" : groupSetPoints[i].ChildNodes[3].ChildNodes[7].InnerText;
                        setPoint.MaxCalculation = groupSetPoints[i].ChildNodes[3].ChildNodes[8] == null ? "" : groupSetPoints[i].ChildNodes[3].ChildNodes[8].InnerText;
                        setPoint.ExcludedValue = groupSetPoints[i].ChildNodes[3].ChildNodes[9] == null ? "" : groupSetPoints[i].ChildNodes[3].ChildNodes[9].InnerText;
                        resourceKey = setPoint.ID + "Unit";
                        setPoint.unit = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        if (parseFor == Global.str_app_ID_Table)
                            Global.valuemap.Add(setPoint.ID, setPoint.value_map);
                        break;
                    case "bnumber":
                        setPoint.type = Settings.Type.type_bNumber;
                        setPoint.bDefault = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.bValueReadFromTripUnit = setPoint.bDefault;
                        setPoint.bValue = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.numberDefault = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.numberValue = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.conversion = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.min = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.max = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        setPoint.stepsize = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[5].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        if (Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[6].Attributes["runCalc"].Value))
                        {
                            setPoint.bcalculated = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[6].Attributes["runCalc"].Value);
                            setPoint.calculation = groupSetPoints[i].ChildNodes[3].ChildNodes[6].InnerText;
                        }
                        setPoint.unit = groupSetPoints[i].ChildNodes[3].ChildNodes[7].InnerText;
                        resourceKeyForsetpointOnLabel = setPoint.ID + "OnLabel".ToString().Trim();
                        resourceKeyForsetpointOffLabel = setPoint.ID + "OffLabel".ToString().Trim(); ;
                        setPoint.OnLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOnLabel);
                        setPoint.OffLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOffLabel);
                        //setPoint.OnLabel= groupSetPoints[i].ChildNodes[3].ChildNodes[8].InnerText;
                        //setPoint.OffLabel = groupSetPoints[i].ChildNodes[3].ChildNodes[9].InnerText;
                        //        break;
                        setPoint.value_map.Add("true", "true");
                        setPoint.value_map.Add("false", "false");
                        if (parseFor == Global.str_app_ID_Table)
                            Global.valuemap.Add(setPoint.ID, setPoint.value_map);
                        //setPoint.type = Settings.Type.type_bNumber;
                        //setPoint.bDefault = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        //setPoint.numberDefault = Int32.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText);
                        //setPoint.conversion = Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText);
                        //setPoint.min = Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].InnerText);
                        //setPoint.max = Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText);
                        //setPoint.stepsize = Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[5].InnerText);
                        //setPoint.unit = groupSetPoints[i].ChildNodes[3].ChildNodes[6].InnerText;
                        //setPoint.strStepSize = groupSetPoints[i].ChildNodes[3].ChildNodes[5].InnerText;
                        break;
                    case "toggle":
                        setPoint.type = Settings.Type.type_toggle;
                        setPoint.bDefault = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.bValueReadFromTripUnit = setPoint.bDefault;
                        setPoint.bValue = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        resourceKeyForsetpointOnLabel = (setPoint.ID + "OnLabel".ToString()).Trim();
                        resourceKeyForsetpointOffLabel = setPoint.ID + "OffLabel".ToString().Trim();
                        setPoint.OnLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOnLabel);
                        setPoint.OffLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOffLabel);
                        //setPoint.OnLabel = groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText;
                        //setPoint.OffLabel = groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText;
                        // setPoint.conversion = Convert.ToDouble(Global.updateValueonCultureBasis(Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].InnerText, CultureInfo.InvariantCulture).ToString()), CultureInfo.CurrentUICulture);
                        if (Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].Attributes["runCalc"].Value))
                        {
                            setPoint.bcalculated = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[3].Attributes["runCalc"].Value);
                            setPoint.calculation = groupSetPoints[i].ChildNodes[3].ChildNodes[3].InnerText;
                        }
                        setPoint.unit = groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText;
                        valueMap_data = groupSetPoints[i].ChildNodes[8].InnerText;
                        valueMap_data = Regex.Replace(valueMap_data, "[^(0-9)(A-F)(a-z)/,/:]", "");
                        valueMap_list = valueMap_data.Split(',');
                        foreach (string a in valueMap_list)
                        {
                            string[] indx = a.Split(':');
                            if (setPoint.value_map.Contains(indx[1]))
                            {
                                string value = setPoint.value_map[indx[1]].ToString();
                                value = value + "|" + indx[0];
                                setPoint.value_map.Remove(indx[1]);
                                setPoint.value_map.Add(indx[1].Trim(), value);
                                setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());
                            }
                            else
                            {
                                setPoint.value_map.Add(indx[1].Trim(), indx[0].Trim());
                                setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());
                            }


                        }

                        // for tooltip
                        resourceKey = setPoint.ID + "Tooltip";
                        strDescSel = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        setPoint.description = strDescSel;

                        //setPoint.value_map.Add("true", "true");
                        //setPoint.value_map.Add("false", "false");
                        if (parseFor == Global.str_app_ID_Table)
                            Global.valuemap.Add(setPoint.ID, setPoint.value_map);
                        break;
                    case "bselection":
                        setPoint.type = Settings.Type.type_bSelection;
                        setPoint.bDefault = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.bValueReadFromTripUnit = setPoint.bDefault;
                        setPoint.bValue = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText);
                        setPoint.selectionDefault = (groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText);
                        //setPoint.conversion = Double.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText);
                        setPoint.unit = groupSetPoints[i].ChildNodes[3].ChildNodes[4].InnerText;
                        resourceKeyForsetpointOnLabel = setPoint.ID + "OnLabel";
                        resourceKeyForsetpointOffLabel = setPoint.ID + "OffLabel";
                        setPoint.OnLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOnLabel);
                        setPoint.OffLabel = Resources.Strings.Resource.ResourceManager.GetString(resourceKeyForsetpointOffLabel);
                        valueMap_data = groupSetPoints[i].ChildNodes[8].InnerText;
                        valueMap_data = Regex.Replace(valueMap_data, "[^(0-9)(A-F)/,/:]", "");
                        valueMap_list = valueMap_data.Split(',');
                        foreach (string a in valueMap_list)
                        {
                            string[] indx = a.Split(':');
                            setPoint.value_map.Add(indx[1].Trim(), indx[0].Trim());
                            setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());

                        }
                        if (parseFor == Global.str_app_ID_Table)
                            Global.valuemap.Add(setPoint.ID, setPoint.value_map);


                        resourceKey = setPoint.ID + "Default";
                        setPoint.defaultSelectionValue = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        setPoint.selectionValue = setPoint.defaultSelectionValue;
                        if (groupSetPoints[i].ChildNodes[3].ChildNodes[4] != null && (groupSetPoints[i].ChildNodes[3].ChildNodes[4]).Name == "Unit")
                        {
                            //Read from Resource
                            resourceKey = setPoint.ID + "Unit";
                            setPoint.unit = (resourceKey);
                        }

                        // for tooltip
                        resourceKey = setPoint.ID + "Tooltip";
                        strDescSel = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        setPoint.description = strDescSel;


                        // for lookuptable values storing
                        foreach (XmlNode item in groupSetPoints[i].ChildNodes[3].ChildNodes[2].ChildNodes)
                        {
                            resourceKey = setPoint.ID + "Item" + setPoint.value_map[item.Attributes["value"].Value];
                            setPoint.lookupTable.Add(setPoint.value_map[item.Attributes["value"].Value],
                            new item_ComboBox(setPoint.value_map[item.Attributes["value"].Value].ToString(), Resources.Strings.Resource.ResourceManager.GetString(resourceKey), item.ChildNodes[1].InnerText));
                            // this hashtable is used to reverse the entry. 
                            setPoint.reverseLookupTable.Add(Resources.Strings.Resource.ResourceManager.GetString(resourceKey), setPoint.value_map[item.Attributes["value"].Value]);

                        }

                        break;
                    case "selection":
                        // According to the new template file for value map                   
                        string data = groupSetPoints[i].ChildNodes[8].InnerText;
                        data = Regex.Replace(data, "[^(0-9)(A-F)|/,/:]", " ");
                        //var pattern = @"\d{1-4}\D{1-2}|:\d{1,2}/,/";
                        var pattern = @",\s";
                        string[] list1 = Regex.Split(data, pattern);
                        list1 = (from e in list1
                                 select e.Replace(" ", "")).ToArray();
                        //  string[] list1 = data.Split(',');
                        foreach (string a in list1)
                        {
                            string[] indx = a.Split(':');

                            if (setPoint.value_map.Contains(indx[1]))
                            {
                                string value = setPoint.value_map[indx[1]].ToString();
                                value = value + "|" + indx[0];
                                setPoint.value_map.Remove(indx[1]);
                                setPoint.value_map.Add(indx[1].Trim(), value);
                                setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());
                            }
                            else
                            {
                                setPoint.value_map.Add(indx[1].Trim(), indx[0].Trim());
                                setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());
                            }

                        }
                        if (parseFor == Global.str_app_ID_Table)
                            Global.valuemap.Add(setPoint.ID, setPoint.value_map);
                        setPoint.type = Settings.Type.type_selection;
                        resourceKey = setPoint.ID + "Default";

                        //As Default values for PTM are different.
                        if (Global.device_type == Global.PTM_DEVICE && Global.IsOffline && (resourceKey == "CPC003ADefault" || resourceKey == "SYS022Default" || resourceKey == "SYS021Default"))
                        {
                            resourceKey = resourceKey + "PTM";
                        }

                        setPoint.defaultSelectionValue = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        setPoint.selectionValue = setPoint.defaultSelectionValue;
                        setPoint.selectionDefault = groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText;
                        if (groupSetPoints[i].ChildNodes[3].ChildNodes[2] != null && (groupSetPoints[i].ChildNodes[3].ChildNodes[2]).Name == "Unit")
                        {
                            //Read from Resource
                            resourceKey = setPoint.ID + "Unit";
                            setPoint.unit = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        }

                        if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE || Global.selectedTemplateType == Global.PTM_TEMPLATE || Global.selectedTemplateType == Global.ACB3_0TEMPLATE || Global.device_type == Global.ACB_PXR35_DEVICE) &&
                            (setPoint.ID == "SYS013" || setPoint.ID == "SYS014" || setPoint.ID == "SYS015"))
                        {
                            resourceKey = Regex.Replace(groupSetPoints[i].ChildNodes[6].ChildNodes[0].InnerText, @"[^0-9a-zA-Z]+", "");
                            //groupSetPoints[i].ChildNodes[6].ChildNodes[0].InnerText
                        }
                        else
                        {
                            resourceKey = setPoint.ID + "Tooltip";
                        }

                        strDescSel = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        setPoint.description = strDescSel;

                        if (setPoint.ID == "SYS6" && Global.IsOffline && !Global.isExportControlFlow)
                        {
                            setPoint.name = Resources.Strings.Resource.ResourceManager.GetString(resourceKey) + " ( For IEC NZM select IEC, for NZM-NA select IEC/UL )";
                        }
                        //  string[] listOfMultipleValueMaps=null;
                        foreach (XmlNode item in groupSetPoints[i].ChildNodes[3].ChildNodes[1].ChildNodes)
                        {
                            string[] listOfMultipleValueMaps = null;
                            if (setPoint.value_map[item.Attributes["value"].Value] == null && Global.device_type ==Global.NZMDEVICE) continue;
                            if (!setPoint.value_map.ContainsKey(item.Attributes["value"].Value))
                            {
                                foreach (var key in setPoint.value_map.Keys)
                                {
                                    string[] values = key.ToString().Split(',');
                                    bool isPresent = values.Contains(item.Attributes["value"].Value);
                                    if (isPresent)
                                    {
                                        resourceKey = setPoint.ID + "Item" + setPoint.value_map[key];
                                        listOfMultipleValueMaps = setPoint.value_map[key].ToString().Split('|');
                                    }

                                }
                            }
                            else
                            {
                                resourceKey = setPoint.ID + "Item" + setPoint.value_map[item.Attributes["value"].Value].ToString();
                            }
                            if (resourceKey.Contains('|'))
                            {
                                resourceKey = resourceKey.Substring(0, resourceKey.IndexOf('|'));
                            }
                            if (resourceKey.Contains(','))
                            {
                                resourceKey = resourceKey.Replace(",", "");
                            }
                            if (listOfMultipleValueMaps == null)
                            {
                                listOfMultipleValueMaps = setPoint.value_map[item.Attributes["value"].Value].ToString().Split('|');
                            }

                            //Following line is added by Astha  to avoid items with same name to be added more than once
                            //if (setPoint.reverseLookupTable.Count == 2 && (setPoint.ID == "SYS004B" || setPoint.ID == "SYS4B"))
                            //{
                            //    continue;
                            //}

                            //else
                            //{

                            //Added for PTM Relay Configuration.
                            if (Global.device_type == Global.PTM_DEVICE && ((setPoint.ID == "SYS013" || setPoint.ID == "SYS014" || setPoint.ID == "SYS015") || resourceKey == "SYS002Item0006"))
                            {
                                resourceKey = resourceKey + "PTM";
                            }                           
                            string resourceKeyValue = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);

                            if (setPoint.reverseLookupTable.Contains(resourceKeyValue))
                            {
                                var KeyValue = "H" + resourceKeyValue;
                                ////  setPoint.lookupTable.Add(listOfMultipleValueMaps[0],
                                //                               new item_ComboBox(listOfMultipleValueMaps[0], resourceKeyValue, item.ChildNodes[1].InnerText));
                                setPoint.lookupTable[listOfMultipleValueMaps[0]] = new item_ComboBox(listOfMultipleValueMaps[0], resourceKeyValue + "|" + KeyValue, item.ChildNodes[1].InnerText);
                                setPoint.reverseLookupTable.Add(KeyValue, listOfMultipleValueMaps[0]);
                                setPoint.indexesWithHexValuesMapping.Add(item.Attributes["value"].Value, KeyValue);
                            }
                            else
                            {

                                setPoint.lookupTable.Add(listOfMultipleValueMaps[0],
                                        new item_ComboBox(listOfMultipleValueMaps[0], Resources.Strings.Resource.ResourceManager.GetString(resourceKey), item.ChildNodes[1].InnerText));
                                // this hashtable is used to reverse the entry. 

                                setPoint.reverseLookupTable.Add(Resources.Strings.Resource.ResourceManager.GetString(resourceKey), listOfMultipleValueMaps[0]);
                                setPoint.indexesWithHexValuesMapping.Add(item.Attributes["value"].Value, Resources.Strings.Resource.ResourceManager.GetString(resourceKey));
                            }
                           
                          

                            if ( setPoint.ID.ToUpper().Trim() == "SYS004A".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS004B".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4B".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4A".ToUpper().Trim()
                                || setPoint.ID.ToUpper().Trim() == "SYS004C".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS004D".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4C".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4D".ToUpper().Trim()
                                || setPoint.ID.ToUpper().Trim() == "SYS004E".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS004F".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4E".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "SYS4F".ToUpper().Trim())
                            {
                                string valueMapValue;
                                if (resourceKeyValue == setPoint.selectionValue)
                                {
                                    //Added by Astha 
                                    //If selection is "blank space" for SYS004A pass "Off" as the selection else pass the original hex value
                                    if ((item.Attributes["value"].Value == "0") && (setPoint.ID == "SYS004A" || setPoint.ID == "SYS0004A" || setPoint.ID == "SYS4A"))
                                    {
                                        TripUnit.MMforExport += "00";
                                    }
                                    else
                                    {
                                        if (setPoint.ID == "SYS004B" || setPoint.ID == "SYS4B" || setPoint.ID == "SYS004C" || setPoint.ID == "SYS4C"||
                                            setPoint.ID == "SYS004D" || setPoint.ID == "SYS4D" || setPoint.ID == "SYS004E" || setPoint.ID == "SYS4E" ||
                                            setPoint.ID == "SYS004F" || setPoint.ID == "SYS4F" )

                                        {
                                            valueMapValue = listOfMultipleValueMaps[0];
                                        }
                                        else
                                        {
                                            valueMapValue = setPoint.value_map[item.Attributes["value"].Value].ToString();
                                        }
                                        TripUnit.MMforExport += valueMapValue;
                                    }
                                }

                            }                           
                        }

                        if (Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].Attributes["runCalc"].Value))
                        {
                            setPoint.bcalculated = Boolean.Parse(groupSetPoints[i].ChildNodes[3].ChildNodes[2].Attributes["runCalc"].Value);
                            setPoint.calculation = groupSetPoints[i].ChildNodes[3].ChildNodes[2].InnerText;
                        }

                        if (Global.IsOffline && (setPoint.ID.ToUpper().Trim() == "SYS002".ToUpper().Trim() || setPoint.ID.ToUpper().Trim() == "CPC002".ToUpper().Trim()))
                        {
                            if (TripUnit.userBreakerInformation != null)        //Added by Archana to avoid reset in Chamge Trip Unit
                            {
                                Global.GlbstrBreakerFrame = TripUnit.userBreakerInformation;
                            }
                        }

                        //if (setPoint.ID.ToUpper().Trim() == "SYS005".ToUpper().Trim())
                        //{
                        //    setPoint.lookupTable.Clear();
                        //    setPoint.reverseLookupTable.Clear();
                        //    foreach (XmlNode item in groupSetPoints[i].ChildNodes[3].ChildNodes[1].ChildNodes)
                        //    {
                        //        setPoint.lookupTable.Remove(setPoint.value_map[item.Attributes["value"].Value]);
                        //        setPoint.reverseLookupTable.Remove(setPoint.value_map[item.Attributes["value"].Value]);
                        //    }
                        //    Configuration config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);
                        //    string releaseType = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["APAC"]);
                        //    //System.Threading.Thread.Sleep(1000);
                        //    //Global.UpdateMaintenanceModeTripLevelItems(setPoint);
                        //    //if (Global.IsOffline)
                        //    //    Global.FirmwareFinal = Global.PCToolFirmware;
                        //    //else
                        //    //    Global.FirmwareFinal = Global.deviceFirmware;

                        //    //if (Global.FirmwareFinal == "00.01.XX") // APAC Trip units
                        //    //{
                        //    //    if (Global.GlbstrBreakerFrame.Contains("NF"))
                        //    //    {
                        //    //        setPoint.lookupTable.Add("0001", new item_ComboBox("0001", "1000 A", ""));
                        //    //        setPoint.lookupTable.Add("0002", new item_ComboBox("0002", "2000 A", ""));
                        //    //        setPoint.lookupTable.Add("0003", new item_ComboBox("0003", "3000 A", ""));
                        //    //        setPoint.lookupTable.Add("0004", new item_ComboBox("0004", "4000 A", ""));
                        //    //        setPoint.lookupTable.Add("0005", new item_ComboBox("0005", "5000 A", ""));

                        //    //        setPoint.reverseLookupTable.Add("1000 A", "0001");
                        //    //        setPoint.reverseLookupTable.Add("2000 A", "0002");
                        //    //        setPoint.reverseLookupTable.Add("3000 A", "0003");
                        //    //        setPoint.reverseLookupTable.Add("4000 A", "0004");
                        //    //        setPoint.reverseLookupTable.Add("5000 A", "0005");

                        //    //        setPoint.defaultSelectionValue = "1000 A";
                        //    //    }
                        //    //    if (Global.GlbstrBreakerFrame.Contains("RF"))
                        //    //    {
                        //    //        setPoint.lookupTable.Add("0001", new item_ComboBox("0001", "4000 A", ""));
                        //    //        setPoint.lookupTable.Add("0002", new item_ComboBox("0002", "8000 A", ""));
                        //    //        setPoint.lookupTable.Add("0003", new item_ComboBox("0003", "12000 A", ""));
                        //    //        setPoint.lookupTable.Add("0004", new item_ComboBox("0004", "16000 A", ""));
                        //    //        setPoint.lookupTable.Add("0005", new item_ComboBox("0005", "20000 A", ""));

                        //    //        setPoint.reverseLookupTable.Add("4000 A", "0001");
                        //    //        setPoint.reverseLookupTable.Add("8000 A", "0002");
                        //    //        setPoint.reverseLookupTable.Add("12000 A", "0003");
                        //    //        setPoint.reverseLookupTable.Add("16000 A", "0004");
                        //    //        setPoint.reverseLookupTable.Add("20000 A", "0005");

                        //    //        setPoint.defaultSelectionValue = "4000 A";
                        //    //    }
                        //    //}
                        //    //else// Global Trip units
                        //    //{
                        //    //    setPoint.lookupTable.Add("0001", new item_ComboBox("0001", Resource.MMLevelVal01, ""));
                        //    //    setPoint.lookupTable.Add("0002", new item_ComboBox("0002", Resource.MMLevelVal02, ""));
                        //    //    setPoint.lookupTable.Add("0003", new item_ComboBox("0003", Resource.MMLevelVal03, ""));
                        //    //    setPoint.lookupTable.Add("0004", new item_ComboBox("0004", Resource.MMLevelVal04, ""));
                        //    //    setPoint.lookupTable.Add("0005", new item_ComboBox("0005", Resource.MMLevelVal05, ""));

                        //    //    setPoint.reverseLookupTable.Add(Resource.MMLevelVal01, "0001");
                        //    //    setPoint.reverseLookupTable.Add(Resource.MMLevelVal02, "0002");
                        //    //    setPoint.reverseLookupTable.Add(Resource.MMLevelVal03, "0003");
                        //    //    setPoint.reverseLookupTable.Add(Resource.MMLevelVal04, "0004");
                        //    //    setPoint.reverseLookupTable.Add(Resource.MMLevelVal05, "0005");
                        //    //    string culture = Convert.ToString(ConfigurationManager.AppSettings["Culture"]);
                        //    //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);       // By Ashish
                        //    //    if (CultureInfo.CurrentCulture.Name == "en-US" || CultureInfo.CurrentCulture.Name == "zh-CHS")
                        //    //    {
                        //    //        setPoint.defaultSelectionValue = "2.5 x In";
                        //    //    }
                        //    //    else if (CultureInfo.CurrentCulture.Name == "de-DE")
                        //    //    {
                        //    //        setPoint.defaultSelectionValue = "2,5 x In";
                        //    //    }
                        //    //}
                        //}

                        if (/*(!Global.IsOffline) &&*/ (setPoint.ID == "SYS001A" || setPoint.ID == "SYS001" || setPoint.ID == "CPC001" || setPoint.ID == "SYS01"))
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
                        if (/*(!Global.IsOffline) && */setPoint.ID == "SYS000" || setPoint.ID == "SYS16"|| setPoint.ID == "SYS6") /*|| setPoint.ID == "CPC001"*/
                        {
                            if (TripUnit.userUnitType != null)
                            {
                                setPoint.selectionValue = TripUnit.userUnitType;
                                setPoint.defaultSelectionValue = TripUnit.userUnitType;
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
                        break;
                    case "split":
                        TripUnit.MMforExport = string.Empty;
                        setPoint.type = Settings.Type.type_split;
                        setPoint.setpoint = new Settings[groupSetPoints[i].ChildNodes[3].ChildNodes.Count];
                        int count = 0;

                        if (Global.IsOffline)
                        {
                            TripUnit.MM16bitString = "0000000000000000";
                            if (TripUnit.MM16bitString != null)
                            {
                                TripUnit.MM_b0 = TripUnit.MM16bitString[15];
                                TripUnit.MM_b1 = TripUnit.MM16bitString[14];
                                TripUnit.MM_b2 = TripUnit.MM16bitString[13];
                                TripUnit.MM_b7 = TripUnit.MM16bitString[8];
                                TripUnit.MM_b8 = TripUnit.MM16bitString[7];
                            }
                        }
                        foreach (XmlNode _setpoint in groupSetPoints[i].ChildNodes[3].ChildNodes)
                        {
                            Settings setpoint = new Settings();
                            setpoint.ID = _setpoint.Attributes["ID"].Value;

                            setpoint.name = Resources.Strings.Resource.ResourceManager.GetString(setpoint.ID + "Name");
                            setpoint.visible = Boolean.Parse(_setpoint.ChildNodes[1].Attributes["value"].Value);
                            setpoint.readOnly = Boolean.Parse(_setpoint.ChildNodes[2].Attributes["value"].Value);
                            string Nodetype = (_setpoint.ChildNodes[3].Attributes["value"].Value);

                            if (Nodetype.Trim().ToLower() != "skip".Trim().ToLower())
                            {
                                setpoint.type = Settings.Type.type_selection;   // this is hardcoded. If this is eventually needed for other another 
                                                                                // type it will need to be reworked
                                setpoint.defaultSelectionValue = Resources.Strings.Resource.ResourceManager.GetString(setpoint.ID + "Default");
                                setpoint.selectionValue = Resources.Strings.Resource.ResourceManager.GetString(setpoint.ID + "Default");

                                if (_setpoint.ChildNodes[6] != null)
                                {
                                    string strDescSplit = Resources.Strings.Resource.ResourceManager.GetString(setpoint.ID + "Tooltip");
                                    setPoint.description = strDescSplit;
                                    setpoint.description = strDescSplit;
                                }
                                foreach (XmlNode item in _setpoint.ChildNodes[3].ChildNodes[1].ChildNodes)
                                {
                                    string key = setpoint.ID + "Item00" + item.Attributes["value"].Value;
                                    resourceKey = Resources.Strings.Resource.ResourceManager.GetString(key);

                                    setpoint.lookupTable.Add(item.Attributes["value"].Value,
                                    new item_ComboBox(item.Attributes["value"].Value, resourceKey, item.ChildNodes[1].InnerText));
                                    // this hashtable is used to reverse the entry. 
                                    setpoint.reverseLookupTable.Add(resourceKey, item.Attributes["value"].Value);
                                    if (resourceKey == setpoint.selectionValue)
                                    {
                                        //If selection is "blank space" for SYS004A pass "Off" as the selection else pass the original hex value

                                        if ((item.Attributes["value"].Value == "02") && (setpoint.ID == "SYS004A"  || setpoint.ID == "SYS4A"))
                                        {
                                            TripUnit.MMforExport += "00";
                                        }
                                        else
                                        {
                                            TripUnit.MMforExport += item.Attributes["value"].Value;
                                        }

                                    }
                                }
                                setupDependents(_setpoint.ChildNodes[4].ChildNodes, ref setpoint);
                            }
                            else if (Nodetype.Trim().ToLower() == "skip".Trim().ToLower())
                            {
                                setPoint.type = Settings.Type.type_skip;
                                //  setPoint.rawValue = ""; 
                                setPoint.numberDefault = Int32.Parse((_setpoint.ChildNodes[3].ChildNodes[0].InnerText));
                            }

                            setPoint.setpoint[count] = setpoint;
                            TripUnit.IDTable.Add(setpoint.ID, setpoint);
                            count++;
                        }

                        break;
                    case "listbox":
                        setPoint.type = Settings.Type.type_listBox;
                        foreach (XmlNode item in groupSetPoints[i].ChildNodes[3].ChildNodes)
                        {
                            setPoint.itemList.Add(new item_ListBox(item.ChildNodes[0].InnerText, Boolean.Parse(item.Attributes["Hidden"].Value), item.Attributes["HighOrLow"].Value));
                        }
                        break;
                    case "text":
                        // According to the new template file for value map
                        if (groupSetPoints[i].ChildNodes[8].InnerText != string.Empty)
                        {
                            string dataValueMap = groupSetPoints[i].ChildNodes[8].InnerText;

                            dataValueMap = Regex.Replace(dataValueMap, "[^(0-9)(A-F)|/,/:]", " ");
                            var patternForSplit = @",\s";                          
                            string[] listForValueMapValues = Regex.Split(dataValueMap, patternForSplit);
                            foreach (string a in listForValueMapValues)
                            {
                                string[] indx = a.Split(':');
                                setPoint.value_map.Add(indx[1].Trim(), indx[0].Trim());
                                setPoint.reversevalue_map.Add(indx[0].Trim(), indx[1].Trim());
                            }
                            if (parseFor == Global.str_app_ID_Table)
                                Global.valuemap.Add(setPoint.ID, setPoint.value_map);
                        }
                        setPoint.type = Settings.Type.type_text;
                        if (setPoint.ID.StartsWith("IP") && setPoint.ID != "IP002")
                        {
                            setPoint.IPaddress = groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText;
                            setPoint.IPaddress_default = groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText;
                        }
                        else
                        {
                            if (double.TryParse(groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText, out setPoint.textvalue))
                            {
                                setPoint.defaultextvalue = setPoint.textvalue;
                            }
                            else
                            {
                                setPoint.textStrValue = groupSetPoints[i].ChildNodes[3].ChildNodes[0].InnerText;
                                setPoint.defaulttextStrValue = setPoint.textStrValue;
                            }

                        }
                        if (Double.TryParse((groupSetPoints[i].ChildNodes[3].ChildNodes[1].InnerText), out conversion))
                        {
                            setPoint.conversion = conversion;
                        }
                        resourceKey = setPoint.ID + "Unit";
                        setPoint.unit = Resources.Strings.Resource.ResourceManager.GetString(resourceKey);
                        break;
                    default:
                        //MessageBox.Show("Error parsing type :: XMLParser :: parseModelFile()");
                        Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, Resource.XMLParserError, "", false);
                        MsgBoxWindow.Topmost = true;
                        MsgBoxWindow.ShowDialog();
                        break;
                }

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
             
            }
        }

        //Modified by Astha
        //Changes are made to parse the single dependency only when dependents are available and parsed
        private static void setupDependents(XmlNodeList nodeList, ref Settings setPoint)
        {
            foreach (XmlNode dependents in nodeList)
            {
                setPoint.dependents.Add(dependents.Attributes["ID"].Value);
                String settingId = dependents.Attributes["ID"].Value;
                Settings set = null;
                try
                {
                    set = (Settings)TripUnit.IDTable[settingId];
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);


                }
                finally
                {
                    if (set != null && set.nodesForSingledependencies.Count != set.singleDependencies.Count)//Modified by Astha to parse all the single dependencies 
                    {
                        setupDependencies(set.nodesForSingledependencies, ref set);
                    }
                }

            }
        }

        //Modified by Astha
        private static void setupDependencies(XmlNodeList nodeList, ref Settings setPoint)
        {
            foreach (XmlNode dependencies in nodeList)
            {
                if (dependencies.Name == "SingleDependency")
                {
                    setupSingleDependency(dependencies, ref setPoint);
                    //Following condition is added to handle the situation wherein dependency logic should work 

                }
                else if (dependencies.Name == "MultiDependency")
                {
                    //setupMultiDependency(dependencies, ref newGroup.groupSetPoints[i]);
                }
            }
       
            //if order of dependents and dependencies are changed in xml which means dependency can be applied later when the dependent is available
            if (setPoint.singleDependencies.Count !=nodeList.Count)
            {
                return;
            }
        }

        /// 
        /// Date: 3-22-13
        /// Author: Sarah M. Norris
        /// <summary>
        /// This structure mimics the xml structure. We first need to determine what condition needs to be met
        /// and then we associate the proper attribute changes with the the condition. 
        /// 
        /// At the end we have created 2 lists: 1 to hold all of the conditions the setting
        /// to listen for and 1 to hold the list of attributes that need to change if the condition is met. 
        /// </summary>
        /// Modified By Astha Agarwal 
        /// To implement multiple dependencies with respect to dropdowns
        /// <param name="dependency"></param>
        /// <param name="setting"></param>
        private static void setupSingleDependency(XmlNode dependency, ref Settings setting)
        {    
            SingleDependency sd = new SingleDependency();
            Hashtable change = null;                        //#COVARITY FIX     234836
            Hashtable TableForDependencies = new Hashtable();
            Hashtable valuesForSetpoint;
            int position = 0;
            String settingId;
           // ArrayList dropdownIndexesWithHexValues;
            // This structure mimics the xml structure. 
            // we first need to determine what condition needs to be met
            // and then we associate the proper attribute changes with
            // the the condition. 

            Hashtable dependenciesID = new Hashtable();

            List<String> listofID;//= new List<String>();         //#COVARITY FIX   234836
            listofID = dependency.Attributes["ID"].Value.ToString().Split(',').ToList<String>();
            foreach (string dependencyid in listofID)
            {
                if (dependencyid.EndsWith("_1") || dependencyid.EndsWith("_2")) //Added by Astha
                {
                    position = dependencyid.IndexOf("_");
                    settingId = dependencyid.Substring(0, position);
                    change = (Hashtable)Global.valuemap[settingId];                   
                }
                else
                {
                    change = (Hashtable)Global.valuemap[dependencyid];
                }
                if(dependencyid.EndsWith("_1"))/*||*/ /*change == null)*/
                {
                    TableForDependencies.Add(dependencyid, new Hashtable());
                }
                     
                if (change != null && (!dependencyid.EndsWith("_1")))
                {
                    TableForDependencies.Add(dependencyid, change);
                }
                else if (change == null && (!dependencyid.EndsWith("_1")) && (!dependencyid.EndsWith("_2")))
                {                    
                    return;
                }

            }

            foreach (XmlNode xmlCondition in dependency.ChildNodes)
            {              
                Condition condition = new Condition();
                String cond_type = xmlCondition.Attributes["type"].Value.ToLower();
                String cvalue = xmlCondition.Attributes["value"].Value;
                //List<String> listOfConditionValues = new List<String>();                  //#COVARITY FIX     234836
                List<String> listOfConditionValues = cvalue.Split(',').ToList<String>();
                for (int i = 0; i < listofID.Count; i++)
                {
                    dependenciesID.Add(listofID[i], listOfConditionValues[i]);

                    if (i < TableForDependencies.Count)
                    {
                        valuesForSetpoint = new Hashtable(TableForDependencies[listofID[i]] as Hashtable);
                        string[] splt = { "|","-" };
                        string[] list = ((String)dependenciesID[listofID[i]]).Split(splt, StringSplitOptions.RemoveEmptyEntries);
                        string temp = "";
                        int count = 0;
                        foreach (string a in list)
                        {
                            bool isNumberValue = a.Any(char.IsDigit);
                            //isNumberValue condition is added to execute the code if 'a' contains null or NA 
                            if (count + 1 == list.Length && (!isNumberValue))
                            {
                                if ((string.Equals(a, "null") || valuesForSetpoint.Count == 0 || string.Equals(a, "NA"))) //Added by Astha
                                {
                                    temp = temp + a;        //Added by Astha to handle null value of a setpoint
                                }
                                else
                                {
                                    if (!valuesForSetpoint.ContainsKey(a))
                                    {
                                        foreach (var key in valuesForSetpoint.Keys)
                                        {
                                            string[] values = key.ToString().Split(',');
                                            bool isPresent = values.Contains(a);
                                            if (isPresent)
                                            {
                                                temp = temp + valuesForSetpoint[key] + "-" + a;
                                                break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        temp = temp + valuesForSetpoint[a];
                                    }
                                }
                            }
                            // this will cover the condition of set of values
                            else if ((string.Equals(a, "null") || valuesForSetpoint.Count == 0 || string.Equals(a, "NA")) && listOfConditionValues[i].Contains("|")) 
                            {
                                if (count  == 0)
                                {
                                    temp = temp + a;                        
                                }
                                else
                                {
                                    temp = temp + "||" + a;
                                }
                            }
                            // this will cover the condition of set of range
                            else if ((string.Equals(a, "null") || valuesForSetpoint.Count == 0 || string.Equals(a, "NA")) && listOfConditionValues[i].Contains("-"))
                            {
                                if (count + 1 == list.Length)
                                {
                                    temp = temp + "-" + a;
                                }
                                else
                                {
                                    temp = temp + a;
                                }
                            }
                            // this will cover the condition of single value
                            else if ((string.Equals(a, "null") || valuesForSetpoint.Count == 0 || string.Equals(a, "NA")))
                            {
                                if (count == 0)
                                {
                                    temp = temp + a;
                                }
                            }
                            else
                            {
                                if (!valuesForSetpoint.ContainsKey(a))
                                {
                                    foreach (var key in valuesForSetpoint.Keys)
                                    {
                                        string[] values = key.ToString().Split(',');
                                        bool isPresent = values.Contains(a);
                                        if (isPresent)
                                        {
                                            temp = temp + valuesForSetpoint[key] + "-" + a + "||";
                                            break;
                                        }

                                    }
                                }
                                else
                                {
                                    temp = temp + valuesForSetpoint[a] + "||";
                                }

                            }
                            count++;
                        }
                        if ("" != temp)
                        {
                            cvalue = temp;
                        }
                        if (cond_type == Global.str_BOOL)
                        {
                            condition.CondBValue = Boolean.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_ENABLE)
                        {
                            condition.CondBValue = Boolean.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_ITEM)
                        {
                            condition.CondItem.Add(cvalue);
                        }
                        else if (cond_type == Global.str_NUMBER)
                        {
                            condition.CondNumber = Double.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_SELECTEDITEM)
                        {
                            condition.CondItem.Add(cvalue);
                        }
                        else if (cond_type == Global.str_READONLY)
                        {
                            condition.CondBValue = Boolean.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_NOTAVAILABLE)
                        {
                            condition.CondBValue = Boolean.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_DESCRIPTION)
                        {
                            condition.CondDescription = cvalue;
                        }
                        else
                        {
                            // MessageBox.Show("XMLParser::setupSingleDependency: Invalid condition type. \nID:" + setting.ID);

                            Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, Resource.XMLParserDependencyError + setting.ID, "", false);
                            MsgBoxWindow.Topmost = true;
                            MsgBoxWindow.ShowDialog();
                        }
                    }
                }
                foreach (XmlNode xmlAttribute in xmlCondition.ChildNodes)
                {
                    Attribute attribute = new Attribute();
                    String attr_type = xmlAttribute.Attributes["type"].Value.ToLower();
                    String avalue = xmlAttribute.InnerText;
                    if(attr_type==Global.str_emptyorna|| attr_type == Global.onlabel|| attr_type == Global.offlabel)
                    {
                        continue;
                    }
                    if (attr_type == Global.str_READONLY || attr_type == Global.str_VISIBLE || attr_type == Global.str_BOOL || attr_type == Global.str_NOTAVAILABLE || attr_type == Global.str_bnumbervisible)
                    {
                        attribute.set_bValue(attr_type, Boolean.Parse(avalue));
                    }
                    else if (attr_type == Global.str_MIN || attr_type == Global.str_MAX || attr_type == Global.str_STEPSIZE ||
                             attr_type == Global.str_CONVERSION || attr_type == Global.str_DEFAULT)
                    {

                        if (setting.type == Settings.Type.type_toggle)
                        {
                            attribute.set_bValue(attr_type, Boolean.Parse(avalue));
                        }
                        else
                        {
                            attribute.set_number(attr_type, Double.Parse(avalue, CultureInfo.InvariantCulture));
                        }
                    }
                    else if (attr_type == Global.str_LOOKUPDATA || attr_type == Global.str_ITEM || attr_type == Global.str_mccbtripstyle)
                    {
                        attribute.set_item(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_LABEL)
                    {
                        attribute.set_label(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_SubGroupName)
                    {
                        attribute.set_SubGroupName(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_mincalculation)
                    {
                        attribute.set_attrMincalculation(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_maxcalculation)
                    {
                        attribute.set_attrMaxcalculation(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_CalculatedValue)
                    {
                        attribute.set_attrCalculatedValue(attr_type, avalue);
                    }
                    else if (attr_type == Global.str_DESCRIPTION)
                    {
                        attribute.set_attrDescription(attr_type, avalue);
                    }
                    else if(attr_type == Global.str_ExcludedValue)
                    {
                        attribute.set_attrExcludedvalue(attr_type, avalue);
                    }
                    else
                    {
                        //MessageBox.Show("XMLParser::setupSingleDependency: Invalue Attribute type.\nID:" + setting.ID);

                        Wizard_Screen_MsgBox MsgBoxWindow = new Wizard_Screen_MsgBox(Resource.XMLParser, Resource.XMLParserDependencyError + setting.ID, "", false);
                        MsgBoxWindow.Topmost = true;
                        MsgBoxWindow.ShowDialog();
                    }
                    condition.AttributeList.Add(attribute);
                }


                sd.ConditionList.Add(condition);
                dependenciesID.Clear();


            }
            // this is a hashtable. We will use the dependency ID 
            // to match the proper dependency when we are called to update. 


            if (!(setting.singleDependencies.Contains(dependency.Attributes["ID"].Value)))
            {
                setting.singleDependencies.Add(dependency.Attributes["ID"].Value, sd);
            }
        }

        /*
                private static void setupMultiDependency(XmlNode dependency, ref Settings setting)
                {
                    SingleDependency sd = new SingleDependency();
                    // This structure mimics the xml structure. 
                    // we first need to determine what condition needs to be met
                    // and then we associate the proper attribute changes with
                    // the the condition. 
                    foreach (XmlNode xmlCondition in dependency.ChildNodes)
                    {
                        Condition condition = new Condition();
                        String cond_type = xmlCondition.Attributes["type"].Value.ToLower();
                        String cvalue = xmlCondition.Attributes["value"].Value;
                        if (cond_type == Global.str_BOOL)
                        {
                            condition.cond_bValue = Boolean.Parse(cvalue);
                        }
                        else if (cond_type == Global.str_ITEM)
                        {
                            condition.cond_item = cvalue;
                        }
                        else if (cond_type == Global.str_NUMBER)
                        {
                            condition.cond_number = Double.Parse(cvalue);
                        }
                        else
                        {
                            MessageBox.Show("XMLParser::setupMultiDependency: Invalid condition type. \nID:" + setting.ID);
                        }

                        foreach (XmlNode xmlAttribute in xmlCondition.ChildNodes)
                        {
                            Attribute attribute = new Attribute();
                            String attr_type = xmlAttribute.Attributes["type"].Value.ToLower();
                            String avalue = xmlAttribute.InnerText;
                            if (attr_type == Global.str_READONLY || attr_type == Global.str_VISIBLE || attr_type == Global.str_BOOL)
                            {
                                attribute.set_bValue(attr_type, Boolean.Parse(avalue));
                            }
                            else if (attr_type == Global.str_MIN || attr_type == Global.str_MAX || attr_type == Global.str_STEPSIZE)
                            {
                                attribute.set_number(attr_type, Double.Parse(avalue));
                            }
                            else if (attr_type == Global.str_ITEM)
                            {
                                attribute.set_item(attr_type, avalue);
                            }
                            else
                            {
                                MessageBox.Show("XMLParser::setupMultiDependency: Invalue Attribute type.\nID:" + setting.ID);
                            }
                            condition.attributeList.Add(attribute);
                        }
                        sd.conditionList.Add(condition);
                    }
                    // this is a hashtable. We will use the dependency ID 
                    // to match the proper dependency when we are called to update. 
                    setting.singleDependencies.Add(dependency.Attributes["ID"].Value, sd);
                }
        */
    }
}
