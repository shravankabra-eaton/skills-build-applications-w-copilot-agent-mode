using System;
using System.Collections.Generic;
using System.Globalization;

// Window form elements
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections;
using PXR.Resources.Strings;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.IO;
using C1.WPF;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Windows.Documents;
// Array List

namespace PXR
{
    /// <summary>
    /// Filename: ScreenCreator.cs
    /// Created:
    /// Author: Sarah M. Norris
    /// Description:    This class creates the dynamic elements of the UI in order to 
    ///                 meet the needs of the different settings that have been 
    ///                 specified in the xml model file
    /// Modifications:
    ///     4/3/13      Adding connection to the Setting class in order to connect the UI
    ///                 elements to the settings they represent. 
    ///     
    ///     4/9/13      Textbox creator uses a grid in order to have the unit inside the textbox
    /// </summary>
    public static class ScreenCreator
    {
        public static void ShowScreenContent(ref ScrollViewer scrollViewer)
        {
            try
            {
                Grid contentPane = new Grid();
                contentPane.Margin = new Thickness(0, 0, 0, 0);
                contentPane.HorizontalAlignment = HorizontalAlignment.Stretch;

                string s1 = null;
                //Set backup before screen creation
                TripUnit.backupGroups = TripUnit.groups;
                //rowcounter is used for deciding rowindex while adding multiple expanders in contentPane
                int rowcounter = 0;
                foreach (Group group in TripUnit.groups)
                {
                        // Group 4 is added for the sub Group to create the display       by Ashish
                        if (0 != group.subgroups.Length)
                        {

                            Grid contentGrid = new Grid();
                            StackPanel subgroupStackPanel = new StackPanel();
                            StackPanel leftmargin = new StackPanel();
                            StackPanel leftStackPanel = new StackPanel();

                            subgroupStackPanel.Orientation = Orientation.Vertical;
                            leftmargin.Margin = new Thickness(25, 0, 0, 0);

                            leftmargin.SetResourceReference(StackPanel.StyleProperty, "StackPanel_subgroup");

                            subgroupStackPanel.Margin = new Thickness(0, 0, 0, 0);
                            subgroupStackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                            contentGrid.HorizontalAlignment = HorizontalAlignment.Stretch;


                            int subgrp_count = 0;
                            int grpSetpoints_count = 0;
                            for (int j = 0; j < group.sequence.Length; j++)
                            {
                                leftStackPanel.Margin = new Thickness(-5, 0, 0, 2);
                                s1 = group.subgroups[subgrp_count].name;

                                //if (s1 == "ATS")
                                //{
                                //    subgrp_count++;
                                //    continue;
                                //}

                                if (s1 == "CAM")
                                {
                                    subgrp_count++;
                                    continue;
                                }

                                if (group.sequence[j] == group.subgroups[subgrp_count].name)
                                {
                                    Expander expander = createExpanderContent(ref group.subgroups[subgrp_count]);
                                    subgroupStackPanel.Children.Add(expander);
                                    group.subgroups[subgrp_count].expander = expander;
                                    subgrp_count++;
                                    if (subgrp_count == group.subgroups.Length)
                                    {
                                        subgrp_count--;
                                    }
                                }

                                else if ((group.sequence[j] == group.groupSetPoints[grpSetpoints_count].name) || (group.groupSetPoints[grpSetpoints_count].name).Contains(group.sequence[j]))

                                {
                                    //For ACB device type, need to align general group setpoints to left
                                    if (Global.selectedTemplateType == Global.ACBTEMPLATE)
                                        leftStackPanel.Margin = new Thickness(-25, 0, 0, 2);
                                    if (!(Global.device_type == Global.PTM_DEVICE && group.groupSetPoints[grpSetpoints_count].ID == "SYS12P" && Global.deviceFirmware4.CompareTo("01.05.0107") <= 0))
                                    {
                                        leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[grpSetpoints_count]));
                                    }

                                    if (((j + 1) == group.sequence.Length) || group.sequence[j + 1] == group.subgroups[subgrp_count].name)
                                    {
                                        Grid contentGrid_2 = new Grid();
                                        ColumnDefinition column = new ColumnDefinition();
                                        contentGrid_2.ColumnDefinitions.Add(column);
                                        Grid.SetColumn(leftStackPanel, 0);
                                        contentGrid_2.Children.Add(leftStackPanel);
                                        subgroupStackPanel.Children.Add(contentGrid_2);
                                        leftStackPanel = new StackPanel();
                                    }

                                    grpSetpoints_count++;
                                }
                            }



                            ColumnDefinition[] columns = new ColumnDefinition[2];
                            columns[0] = new ColumnDefinition();
                            columns[1] = new ColumnDefinition();
                            columns[0].Width = GridLength.Auto;
                            contentGrid.ColumnDefinitions.Add(columns[0]);
                            contentGrid.ColumnDefinitions.Add(columns[1]);


                            Grid.SetColumn(leftmargin, 0);
                            Grid.SetColumn(subgroupStackPanel, 1);

                            contentGrid.Children.Add(leftmargin);
                            contentGrid.Children.Add(subgroupStackPanel);
                            contentGrid.SetResourceReference(Grid.StyleProperty, "GridBackgroundInExpander");

                            ScrollViewer scrollViewers = new ScrollViewer();

                            scrollViewers.Content = contentGrid;
                            scrollViewers.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                            scrollViewers.HorizontalAlignment = HorizontalAlignment.Stretch;

                            Expander expander_subgrp = new Expander();
                            expander_subgrp.Name = "expItem_" + group.ID;

                            expander_subgrp.Header = group.name;
                            expander_subgrp.Margin = new Thickness(0);
                            expander_subgrp.Content = scrollViewers;

                            expander_subgrp.SetResourceReference(Expander.StyleProperty, "ExpanderStyle_trigger");
                            group.ExpanderItem = expander_subgrp;
                            if (contentPane.Children.Count == 0)
                                expander_subgrp.IsExpanded = true;



                            //For new expander, Added new row inside contentPane i.e. Container grid
                            //rowcounter is used for deciding rowindex
                            RowDefinition row = new RowDefinition();
                            row.Height = GridLength.Auto;// new GridLength(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height * 0.20, GridUnitType.Star);
                            contentPane.RowDefinitions.Add(row);
                            if (contentPane.Children.Count == 0 && rowcounter == 0)
                                expander_subgrp.IsExpanded = true;

                            Grid.SetRow(expander_subgrp, rowcounter);
                            rowcounter++;

                            contentPane.Children.Add(expander_subgrp);
                        }
                        else
                        {
                            Expander item = CreateExpanderContent(group);
                            group.ExpanderItem = item;

                            //For new expander, Added new row inside contentPane i.e. Container grid
                            //rowcounter is used for deciding rowindex
                            RowDefinition row = new RowDefinition();
                            row.Height = GridLength.Auto;
                            contentPane.RowDefinitions.Add(row);
                            Grid.SetRow(item, rowcounter);
                            rowcounter++;

                            contentPane.Children.Add(item);
                        }
                }

                scrollViewer.CanContentScroll = true;
                try
                {
                    contentPane.SetResourceReference(Grid.StyleProperty, "GridBackgroundInExpander");
                    scrollViewer.Content = contentPane;
                }
                catch (Exception ex)
                {
                    LogExceptions.LogExceptionToFile(ex);
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            catch (Exception ex)
            {

                LogExceptions.LogExceptionToFile(ex);
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public static void ShowScreenContentTest(ref ScrollViewer scrollViewer, bool mIsPhaseTest)
        {
            StackPanel contentPane = new StackPanel();

            contentPane.Orientation = Orientation.Vertical;
            contentPane.VerticalAlignment = VerticalAlignment.Top;
            contentPane.Margin = new Thickness(0, 0, 0, 0);

            contentPane.HorizontalAlignment = HorizontalAlignment.Stretch;

            //Set backup before screen creation
            TripUnit.backupGroups = TripUnit.groups;

            Group group = TripUnit.groups[1] as Group;
            contentPane.Children.Add(CreateAccordianContentForTest(mIsPhaseTest));
            scrollViewer.CanContentScroll = true;
            scrollViewer.Content = contentPane;
        }


        private static Grid CreateAccordianContentForTest(bool mIsPhaseTest)
        {
            Grid contentGrid = new Grid();
            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_LeftTest");
            StackPanel rightStackPanel = new StackPanel();
            rightStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_RightTest");



            Settings[] testGroupSetPoints = null;

            //To change sequence of setpoints displayed in Group 0 and Group 1 -Sreejith

            if (mIsPhaseTest)
            {
                testGroupSetPoints = new Settings[11];
            }
            else
            {
                testGroupSetPoints = new Settings[6];
            }
            int testSetpointConuter = 0;
            foreach (Group group in TripUnit.groups)
            {
                for (int i = 0; i < group.groupSetPoints.Length; i++)
                {
                    if (mIsPhaseTest)
                    {
                        //Add current config fields
                        if ((Convert.ToInt32(group.ID) == 01) || (Convert.ToInt32(group.ID) == 0))
                        {
                            if (group.groupSetPoints[i].ID == "CPC004" || group.groupSetPoints[i].ID == "CPC004A" || group.groupSetPoints[i].ID == "CPC006" || group.groupSetPoints[i].ID == "CPC007" || group.groupSetPoints[i].ID == "CPC007A" ||
                                group.groupSetPoints[i].ID == "CPC008" || group.groupSetPoints[i].ID == "CPC008A" || group.groupSetPoints[i].ID == "CPC011" || group.groupSetPoints[i].ID == "CPC012" || group.groupSetPoints[i].ID == "CPC012A" ||
                                group.groupSetPoints[i].ID == "CPC013" || group.groupSetPoints[i].ID == "CPC013A" || group.groupSetPoints[i].ID == "CPC014" || group.groupSetPoints[i].ID == "CPC014A" || group.groupSetPoints[i].ID == "CPC005" ||
                                group.groupSetPoints[i].ID == "SYS005" || group.groupSetPoints[i].ID == "SYS004A" )
                            {
                                testGroupSetPoints[testSetpointConuter] = group.groupSetPoints[i];
                                testGroupSetPoints[testSetpointConuter].readOnly = true;
                                testSetpointConuter++;
                            }
                        }

                        // add ARMS mode
                        //else if (Convert.ToInt32(group.ID) == 0)
                        //{
                        //    //if ((group.groupSetPoints[i].ID == "SYS004A")||(group.groupSetPoints[i].ID == "SYS004B"))
                        //    //{
                        //    //    testGroupSetPoints[testSetpointConuter] = group.groupSetPoints[i].setpoint[0];
                        //    //    testGroupSetPoints[testSetpointConuter].readOnly = true;
                        //    //    testSetpointConuter++;
                        //    //}

                        //    if (group.groupSetPoints[i].ID == "SYS005"|| group.groupSetPoints[i].ID == "SYS004A")
                        //    {
                        //        testGroupSetPoints[testSetpointConuter] = group.groupSetPoints[i];
                        //        testGroupSetPoints[testSetpointConuter].readOnly = true;
                        //        testSetpointConuter++;
                        //    }

                        //}
                    }
                    else
                    {
                        if (group.groupSetPoints[i].ID == "CPC004" || group.groupSetPoints[i].ID == "CPC004A" || group.groupSetPoints[i].ID == "CPC015" || group.groupSetPoints[i].ID == "CPC017" ||
                            group.groupSetPoints[i].ID == "CPC018" || group.groupSetPoints[i].ID == "CPC018A" || group.groupSetPoints[i].ID == "CPC019" || group.groupSetPoints[i].ID == "CPC005")
                        {
                            testGroupSetPoints[testSetpointConuter] = group.groupSetPoints[i];
                            testGroupSetPoints[testSetpointConuter].readOnly = true;
                            testSetpointConuter++;
                        }
                    }
                }
            }


            int halfwaypoint = (int)Math.Ceiling((double)testGroupSetPoints.Length / 2);

            for (int i = 0; i < testGroupSetPoints.Length; i++)
            {
                if (i < halfwaypoint)
                {
                    leftStackPanel.Children.Add(createSettingDisplay(testGroupSetPoints[i]));
                }
                else
                {
                    rightStackPanel.Children.Add(createSettingDisplay(testGroupSetPoints[i]));
                }
            }
            ColumnDefinition[] columns = new ColumnDefinition[2];
            columns[0] = new ColumnDefinition();
            columns[1] = new ColumnDefinition();
            contentGrid.ColumnDefinitions.Add(columns[0]);
            contentGrid.ColumnDefinitions.Add(columns[1]);

            Grid.SetColumn(leftStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);

            contentGrid.Children.Add(leftStackPanel);
            contentGrid.Children.Add(rightStackPanel);
            contentGrid.SetResourceReference(Grid.StyleProperty, "GridBackgroundInExpander");

            return contentGrid;
        }

        private static Expander createExpanderContent( ref Group group)
        {
            Grid contentGrid = new Grid();
            StackPanel leftStackPanel = new StackPanel();
            //Grid setpoint_element = new Grid(); //Need to remove after coverity scan
            contentGrid.SetResourceReference(Grid.StyleProperty, "GridBackgroundInExpander");
            leftStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Left");
            int defaultValueForcounter = 0;//This variable is added to exclude first three setpoints in group1 else they will get redundant on Main Setpoint screen
            if (Convert.ToInt32(group.ID) == 0) /*|| Convert.ToInt32(group.ID) == 1*/
            {
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[3]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[2]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[1]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[0]));
                defaultValueForcounter = 4;
            }
            else if (Convert.ToInt32(group.ID) == 1)
            {
                defaultValueForcounter = 3;
            }
            else
            {
                defaultValueForcounter = 0;
            }

            if (group.subgroups.Length != 0)
            {
                for (int k = 0; k < group.subgroups.Length; k++)
                {
                    Expander expander = createExpanderContent(ref group.subgroups[k]);
                    if (expander != null)
                    {
                        expander.Name = "exp_" + group.subgroups[k].ID;
                        leftStackPanel.Children.Add(expander);
                        group.subgroups[k].expander = expander;
                    }
                }
            }
            else
            {
                for (int i = defaultValueForcounter; i < group.groupSetPoints.Length; i++)
                {
                    Grid setpoint_element = (Grid)createSettingDisplay(group.groupSetPoints[i]);
                    leftStackPanel.Children.Add(setpoint_element);
                    group.groupSetPoints[i].setpoint_stack = setpoint_element;

                    if (group.groupSetPoints[i].ID == "IP002")
                    {
                        leftStackPanel.Children.Add(createGridForIP002CalValue((group.groupSetPoints[i])));
                    }
                }
            }


            ColumnDefinition columns = new ColumnDefinition();//#COVARITY FIX
            contentGrid.ColumnDefinitions.Add(columns);

            Grid.SetColumn(leftStackPanel, 0);

            contentGrid.Children.Add(leftStackPanel);


            //ScrollViewer scrollViewer = new ScrollViewer();
            //scrollViewer.Content = contentGrid;
            //scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //scrollViewer.HorizontalAlignment = HorizontalAlignment.Stretch;

            Expander testExpander = new Expander();
            testExpander.Name = "expanderItem_" + group.ID;
            testExpander.Header = group.name;
            testExpander.Margin = new Thickness(0);

            testExpander.Content = contentGrid;

            testExpander.SetResourceReference(Expander.StyleProperty, "ExpanderStyle_trigger");
            return testExpander;
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

                    foreach (T childOfChild in Global.FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private static Expander CreateExpanderContent(Group group)
        {
            Expander expanderitem = new Expander();
            Grid contentGrid = new Grid();
            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Center");
            int defaultValueForcounter = 0;//This variable is added to exclude first three setpoints in group1 else they will get redundant on Main Setpoint screen

            int halfwaypoint = (int)Math.Ceiling((double)group.groupSetPoints.Length / 2);

            //To change sequence of setpoints displayed in Group 0 and Group 1 -Sreejith
            if (Convert.ToInt32(group.ID) == 1 && Global.selectedTemplateType == Global.ACBTEMPLATE) /*|| Convert.ToInt32(group.ID) == 1*/
            {
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[3]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[2]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[1]));
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[0]));
                defaultValueForcounter = 4;
            }
            else if (Convert.ToInt32(group.ID) == 2 && Global.selectedTemplateType == Global.ACBTEMPLATE)
            {
                defaultValueForcounter = 3;
            }
            else
            {
                defaultValueForcounter = 0;
            }
            int indexForHighLoad1=0;  
            int indexForHighLoadToggle1 = 0;
            int indexForHighLoadToggle2 = 0;
            for (int i = defaultValueForcounter; i < group.groupSetPoints.Length; i++)
            {
               if(group.groupSetPoints[i].ID=="CPC010" && TripUnit.IDTable.Contains("CPC021"))
                {
                    indexForHighLoad1 = i;
                    continue;
                }

                if (group.groupSetPoints[i].ID == "CPC026" && TripUnit.IDTable.Contains("CPC021"))
                {
                    indexForHighLoadToggle1 = i;
                    continue;
                }

                if (group.groupSetPoints[i].ID == "CPC027" && TripUnit.IDTable.Contains("CPC021"))
                {
                    indexForHighLoadToggle2 = i;
                    continue;
                }

                //As per coverity this is copy paste error but this is not the copy paste error 
                // for ACB hiload 1 and 2 setpoints are not in sequance to display those in sequance this change is made.
                //For ACB3.0, high load 1 and 2 have toggles, to place them in sequence indexForHighLoadToggle1 and indexForHighLoadToggle2 variable values used.
                if (group.groupSetPoints[i].ID == "CPC021")
                {
                    leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[indexForHighLoadToggle1]));
                    leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[indexForHighLoad1]));
                    leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[indexForHighLoadToggle2]));
                }
                leftStackPanel.Children.Add(createSettingDisplay(group.groupSetPoints[i]));
                if (group.groupSetPoints[i].ID == "IP002")
                {
                    leftStackPanel.Children.Add(createGridForIP002CalValue((group.groupSetPoints[i])));
                }

            }

            contentGrid.Children.Add(leftStackPanel);
            contentGrid.SetResourceReference(Grid.StyleProperty, "GridBackgroundInExpander");
            if (Global.selectedTemplateType == Global.MCCBTEMPLATE || Global.selectedTemplateType == Global.NZMTEMPLATE)
            {
                expanderitem.Name = "expItem_" + group.ID;
            }
            else
            {
                expanderitem.Name = "expItem_" + group.ID;

            }
            group.setupExpanderItem(ref expanderitem);
            expanderitem.Header = group.name;
            expanderitem.Margin = new Thickness(0);
            expanderitem.Content = contentGrid;
            expanderitem.SetResourceReference(Expander.StyleProperty, "ExpanderStyle_trigger");

            return expanderitem;
        }
        private static UIElement createGridForIP002CalValue(Settings setting)
        {
            Grid panel_setting = new Grid();
            panel_setting.SetResourceReference(Grid.StyleProperty, "GridSetpoint");
            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Left");
            StackPanel rightStackPanel = new StackPanel();
            if (setting.subgrp_index == -1)  // Style to align the UI for setpoints inside subgroup and outside subgroup
            {
                rightStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Right_1");
            }
            else
            {
                rightStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Right");
            }
            rightStackPanel.Orientation = Orientation.Horizontal;
            NameScope.SetNameScope(panel_setting, new NameScope());

            if(setting.ID == "IP002")
            {
                //Label calculated_Value = new Label();

                //calculated_Value.Name = "lbl2_" + setting.ID;
                //calculated_Value.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                //StackPanel spac = new StackPanel();
                //spac.Width = 15;
                //setting.label_calculation = calculated_Value;
                //calculated_Value.Content = "Subnet Mask Calculation";
                //setting.emptyspace = spac;
                //  rightStackPanel.Children.Add(spac);
                rightStackPanel.Children.Add(setting.label_calculation);
            }

            ColumnDefinition[] columns = new ColumnDefinition[2];
            columns[0] = new ColumnDefinition();
            columns[1] = new ColumnDefinition();
            panel_setting.ColumnDefinitions.Add(columns[0]);
            panel_setting.ColumnDefinitions.Add(columns[1]);

            Grid.SetColumn(leftStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);

            panel_setting.Children.Add(leftStackPanel);
            panel_setting.Children.Add(rightStackPanel);
            return panel_setting;
        }

        private static Label createLabel(String text, Boolean visible, ref Label _labelName, ref Settings setting)
        {
            Label label = new Label();
            label.Name = "lbl_" + setting.ID;
            label.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
            label.Content = text;
            if (!visible)
            {
                label.Visibility = Visibility.Collapsed;
            }
            // Connect Label to UI. 
            _labelName = label;
            return label;
        }
        private static TextBlock createLeftLabel(String text, Boolean visible, ref TextBlock _txtblockName, ref Settings setting)
        {
            TextBlock tblock = new TextBlock();
            tblock.Name = "lbl_" + setting.ID;
            tblock.SetResourceReference(Label.StyleProperty, "LeftBlackLabelStyle");
            tblock.Text = text;
            if (!visible)
            {
                tblock.Visibility = Visibility.Collapsed;
            }
            // Connect Label to UI. 
            _txtblockName = tblock;
            return tblock;
        }
        private static TextBlock createRightLabel(String text, Boolean visible, ref TextBlock _txtblockName, ref Settings setting)
        {
            TextBlock tblock = new TextBlock();
            tblock.Name = "lbl_" + setting.ID;
            tblock.SetResourceReference(Label.StyleProperty, "RightBlackLabelStyle");
            tblock.Text = text;
            if (!visible)
            {
                tblock.Visibility = Visibility.Collapsed;
            }
            // Connect Label to UI. 
            _txtblockName = tblock;
            return tblock;
        }
        //updown control to calculate subnetmask 
        private static TextBox createSubnetMaskCal(ref Settings setting, ref TextBox _TextBox_subnetMask)
        {
            TextBox textBox = new TextBox();
            textBox.Name = "lbl_NA_" + setting.ID;
            textBox.SetResourceReference(TextBox.StyleProperty, "FocusTextBox");

            if (setting.type == Settings.Type.type_number)
            {
                textBox.Margin = new Thickness(-259, 0, 0, 0);
            }

            // double _min, double _max, double _stepSize


            else if (setting.type == Settings.Type.type_number)
            {
                if (setting.numberDefault == 1)
                {
                    textBox.Margin = new Thickness(-217, 0, 0, 0);
                }
                else
                {
                    textBox.Margin = new Thickness(-225, 0, 0, 0);
                }

            }
            else
            {
                textBox.Margin = new Thickness(-235, 0, 0, 0);
            }
            _TextBox_subnetMask = textBox;

            //if (!setting.visible)
            //{
            //    textBox.Visibility = Visibility.Collapsed;
            //}
            // Connect Label to UI. 
            return textBox;
        }

        // Label for not available of setpoint 
        private static Label createLabel_NA(ref Settings setting, ref Label _label_notavailable)
        {
            Label label = new Label();
            label.Name = "lbl_NA_" + setting.ID;
            label.SetResourceReference(Label.StyleProperty, "BlackLabelStyle_for_NA");

            if (setting.type == Settings.Type.type_bNumber || setting.type == Settings.Type.type_number)
            {
                label.Margin = new Thickness(-259, 0, 0, 0);
            }
            else if(setting.type == Settings.Type.type_toggle)
            {
                label.Margin = new Thickness(-98, 0, 0, 0);
            }
            else if(setting.type==Settings.Type.type_text )
            {
                if(setting.defaulttextStrValue == "NA")
                {
                    label.Margin = new Thickness(-217, 0, 0, 0);
                }
                else
                {
                    label.Margin = new Thickness(-225, 0, 0, 0);
                }

            }
            else
            {
                label.Margin = new Thickness(-235, 0, 0, 0);
            }
            _label_notavailable = label;
            if (setting.notAvailable)
            {
                label.Content = Resource.NotAvailable;
            }
            else
            {
                label.Content = "";
            }

            if (!setting.visible)
            {
                label.Visibility = Visibility.Collapsed;
            }
            // Connect Label to UI. 
            return label;
        }

        private static ComboBox createComboBox(ref Settings setting, string lblComboContent)
        {
            string[] tagInfo = new string[3];
            List<string> lv_strTempItem = new List<string>();

            ComboBox comboBox = new ComboBox();
            Label label_unit = new Label();
            comboBox.Name = "cmb_" + setting.ID;
            if (!String.IsNullOrEmpty(setting.unit))
            {

                label_unit.Content = setting.unit;
                label_unit.SetResourceReference(Label.StyleProperty, "UnitLabelStyle_text");
            }

            ComboBox comboBoxOffline = new ComboBox();


            if (setting.description != null && setting.description.Trim() != "")
            {
                ToolTip toolTip = createComboItemToolTip(setting.description);
                comboBox.ToolTip = toolTip;
                comboBoxOffline.ToolTip = toolTip;
                ToolTipService.SetToolTip(toolTip, "");
                ToolTipService.SetShowDuration(comboBox, 300000);
                ToolTipService.SetShowDuration(comboBoxOffline, 300000);

            }
            comboBoxOffline.SetResourceReference(ComboBox.StyleProperty, "TriggerComboBoxStyle");
            comboBox.SetResourceReference(ComboBox.StyleProperty, "TriggerComboBoxStyle");
            int selectedIndex = 0;

            foreach (String itemKey in setting.lookupTable.Keys)
            {
                item_ComboBox settingItem = (item_ComboBox)setting.lookupTable[itemKey];
                String selectionString = settingItem.item;
                //if (selectionString.Contains(","))
                //{
                //    string[] values = selectionString.Split(',');
                //    foreach (var item in values)
                //    {
                //        lv_strTempItem.Add(itemKey);
                //    }
                //}
                //else
                //{
                lv_strTempItem.Add(itemKey);
                // }
            }


            lv_strTempItem.Sort();


            //  foreach (String itemKey in setting.lookupTable.Keys)
            foreach (String itemKey in lv_strTempItem)
            {
                // ComboBox Items
                item_ComboBox settingItem = (item_ComboBox)setting.lookupTable[itemKey];
                String selectionString = settingItem.item;
                string[] values = null;

                values = selectionString.Split('|');


                foreach (var val in values)
                {


                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = val;
                    if (settingItem.description != "")
                    {
                        ToolTip toolTip = createComboItemToolTip(settingItem.description);
                        item.ToolTip = toolTip;
                        ToolTipService.SetToolTip(toolTip, "");
                        ToolTipService.SetShowDuration(item, 300000);
                    }
                    item.SetResourceReference(ComboBoxItem.StyleProperty, "TriggerComboBoxItemStyle");

                    if (setting.ID == "SYS11" && Global.IsOffline)
                    {
                        switch (Global.device_type)
                        {
                            case Global.MCCBDEVICE:
                                if (comboBox.Items != null && itemKey == "0001"  || itemKey == "0006" || (Global.IsOpenFile && itemKey == setting.reverseLookupTable[setting.selectionValue].ToString()))
                                {
                                    comboBox.Items?.Add(item);
                                    selectedIndex++;
                                }

                                break;
                            case Global.NZMDEVICE:
                                if (comboBox.Items != null && itemKey == "0001" || itemKey == "0002" || itemKey == "0003" || itemKey == "0005" || itemKey == "0012" || itemKey == "0019" || itemKey == "0008" || itemKey == "0026" || (Global.IsOpenFile && itemKey == setting.reverseLookupTable[setting.selectionValue].ToString()))
                                {
                                    comboBox.Items?.Add(item);
                                    selectedIndex++;
                                }

                                break;
                        }

                    }
                    else if (setting.ID == "SYS11" && Global.isDemoMode)
                    {
                        if (comboBox.Items != null && itemKey == "0001" || itemKey == "0002" || itemKey == "0003" || itemKey == "0004" || itemKey == "0005")
                        {
                            comboBox.Items?.Add(item);
                            selectedIndex++;
                        }
                    }
                    else
                    {
                        if (comboBox.Items != null)
                            comboBox.Items?.Add(item);
                    }

                    if (val == setting.selectionValue)
                    {
                        comboBox.SelectedIndex = setting.ID == "SYS11" && Global.IsOffline ? selectedIndex-1: selectedIndex;
                    }
                    if(setting.ID != "SYS11" ) selectedIndex++;
                }
            }

            //For Motor protection group appending its subgroup name before its setpoint name - For differentiating in Change summary  
            //eg. Over Voltage - Feature
            if (setting.GroupID == "04" || setting.GroupID == "004")
            {
                tagInfo[1] = ((Group)(TripUnit.groups[4])).subgroups[setting.subgrp_index].name + " - " +
                        lblComboContent.Trim();
            }
                else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && setting.GroupID == "01" &&
            (setting.subgrp_index == 5 || setting.subgrp_index == 6 || setting.subgrp_index == 7 || setting.subgrp_index == 8 ||
            setting.subgrp_index == 9 || setting.subgrp_index == 10 || setting.subgrp_index == 11 ||setting.subgrp_index == 12 ||setting.subgrp_index == 13 || setting.subgrp_index == 14 || setting.subgrp_index == 15 || setting.subgrp_index == 16))
            {
                tagInfo[1] = ((Group)(TripUnit.groups[1])).subgroups[setting.subgrp_index].name + " - " +
                    lblComboContent.Trim();
            }
            else
            {
                tagInfo[1] = lblComboContent.Trim(); // stores the label to be displayed in summary
            }

            tagInfo[0] = setting.selectionValue;
            tagInfo[2] = comboBox.Name.Trim();
            comboBox.Tag = tagInfo;


            ToolTipService.SetShowDuration(comboBoxOffline, 300000);
            if (setting.type == Settings.Type.type_bSelection)
            {
                comboBox.IsEnabled = setting.bValue;
                if (setting.bValue)
                {
                    comboBox.IsReadOnly = !setting.bValue;
                }
            }
            if (!setting.visible)
            {
                comboBox.Visibility = Visibility.Collapsed;
                comboBoxOffline.Visibility = Visibility.Collapsed;
            }
            setting.SetupComboBox(ref comboBox, ref label_unit);

            if (setting.readOnly == true && setting.showvalueInBothModes == false)
            {
                comboBox.IsEnabled = !setting.readOnly;
                comboBoxOffline.IsEnabled = !setting.readOnly;

                if (Global.IsOffline && (setting.ID != "SYS004A" && setting.ID != "SYS4A"))
                {
                    setting.UnsubscribeFromComboBoxSelectionChangedEvent(ref comboBox);
                    comboBox.SelectedIndex = -1;
                }
            }
            if (setting.readOnly == true && setting.showvalueInBothModes == true)
            {
                comboBox.IsEnabled = !setting.readOnly;
                comboBoxOffline.IsEnabled = !setting.readOnly;
            }

            //In online mode for control use value of OnlineReadOnly flag not the Readonly
            //(Note - For setpoints those are dependant on any other setpoints value for readonly is getting updated with OnlineRedonly through dependancy call.)
            if (!Global.IsOffline && setting.onlineReadOnly == true)
            {
                setting.readOnly = setting.onlineReadOnly;
                comboBox.IsEnabled = !setting.readOnly;
                comboBoxOffline.IsEnabled = !setting.readOnly;
            }
            else
            {
                comboBox.IsEnabled = !setting.readOnly;
                comboBoxOffline.IsEnabled = !setting.readOnly;
            }

            //if (setting.ID == "GEN01" && Global.IsOffline) // conditon written for PXR10 export in offline
            //{
            //    if (setting.defaultSelectionValue == "PXR 10")
            //    {
            //        Global.device_type_PXR10 = true;
            //    }
            //}

            if (Global.IsUndoLock == false && setting.showvalueInBothModes == false && setting.readOnly && Global.IsOffline && setting.ID != "SYS004A" && setting.ID != "SYS4A" && setting.ID != "SYS132" && setting.ID != "SYS142" && setting.ID != "SYS152")
            {
                setting.SetupComboBox(ref comboBoxOffline, ref label_unit);
                return comboBoxOffline;
            }
            else
            {
                setting.SetupComboBox(ref comboBox, ref label_unit);
                return comboBox;
            }




        }

        //public static ComboBox createComboBoxForMultiDependency(ref Settings setting, string lblComboContent)
        //{
        //    string[] tagInfo = new string[3];
        //    ComboBox comboBox = new ComboBox();
        //    comboBox.Name = "cmb_" + setting.ID;
        //    if (setting.description != null && setting.description.Trim() != "")
        //    {
        //        ToolTip toolTip = createComboItemToolTip(setting.description.Replace("\r\n", "").Replace("  ", "").Trim());
        //        comboBox.ToolTip = toolTip;
        //        ToolTipService.SetToolTip(toolTip, "");
        //        ToolTipService.SetShowDuration(comboBox, 300000);
        //    }
        //    comboBox.SetResourceReference(ComboBox.StyleProperty, "TriggerComboBoxStyle");

        //    if (Convert.ToString(((Group)(TripUnit.groups[1])).groupSetPoints[10].selectionValue) == Resource.strSourceGround)
        //    {
        //        fillComboBox(ref setting, ref comboBox);
        //    }
        //    else
        //    {
        //        comboBox.IsEnabled = false;
        //    }
        //    setting.SetupComboBox(ref comboBox);
        //    if (comboBox.SelectedItem != null)
        //    {
        //        tagInfo[0] = setting.selectionValue;
        //    }
        //    else
        //    {
        //        tagInfo[0] = "";
        //    }
        //    tagInfo[1] = lblComboContent.Trim();
        //    tagInfo[2] = comboBox.Name;
        //    comboBox.Tag = tagInfo;
        //    return comboBox;
        //}



        //Added by Astha to populate dropdown based on combination selected from other dropdowns
        public static void fillComboBox(ref Settings setting, ref ComboBox comboBox)
        {
            try
            {
                int selectedIndex = 0;
                if (comboBox != null)
                {
                    setting.UnsubscribeFromComboBoxSelectionChangedEvent(ref comboBox);
                    if(comboBox.Items != null)
                        comboBox.Items.Clear();
                }
                if (comboBox == null)
                {
                    comboBox = new ComboBox();
                }
                Label label_unit = new Label();
                if (!String.IsNullOrEmpty(setting.unit))
                {

                    label_unit.Content = setting.unit;
                    label_unit.SetResourceReference(Label.StyleProperty, "UnitLabelStyle_text");
                }
                List<string> lv_strTempItem = new List<string>();

                foreach (String itemKey in setting.ItemsToDisplayfromLookupTable.Keys)
                {
                    item_ComboBox settingItem = (item_ComboBox)setting.ItemsToDisplayfromLookupTable[itemKey];
                    String selectionString = settingItem.item;
                    lv_strTempItem.Add(itemKey);
                }
                if ((Global.IsOffline && Global.isnewfile == true) && (comboBox.SelectedIndex == -1)
                      && (setting.ID == "GEN02A" && setting.visible == true && Global.device_type == Global.MCCBDEVICE)
                   )
                {
                    lv_strTempItem.Sort();
                    lv_strTempItem.Reverse();
                }
                else
                    lv_strTempItem.Sort();


                foreach (String itemKey in lv_strTempItem)
                {

                    item_ComboBox settingItem = (item_ComboBox)setting.ItemsToDisplayfromLookupTable[itemKey];
                    String selectionString = settingItem.item;
                    // ComboBox Items
                    string[] values = null;

                    values = selectionString.Split('|');


                    foreach (var val in values)
                    {
                        //if (setting.listofItemsToDisplay == null||setting.listofItemsToDisplay.Contains(val))
                        //{
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = val;
                        if (settingItem.description != "")
                        {
                            ToolTip toolTip = createComboItemToolTip(settingItem.description.Replace("\r\n", "").Replace("  ", "").Trim());
                            item.ToolTip = toolTip;
                            ToolTipService.SetToolTip(toolTip, "");
                            ToolTipService.SetShowDuration(item, 300000);
                        }
                        item.SetResourceReference(ComboBoxItem.StyleProperty, "TriggerComboBoxItemStyle");

                        //Added this condition to add H250 rating only for PD3 (250 and H250 were getting added)
                        string breakerFrame = TripUnit.getBreakerFrame().ID != null ? TripUnit.getBreakerFrame().selectionValue : string.Empty;
                        if (Global.device_type == Global.MCCBDEVICE && setting.ID == "SYS01" && breakerFrame != null &&
                            breakerFrame != Resource.SYS02Item0016 && (val == "H250 A" || val == "H400 A"))
                        {
                            continue;
                        }
                        else
                        {
                            if (comboBox.Items != null)
                                comboBox.Items.Add(item);
                        }
                        if (val == setting.selectionValue)      //Uncommented by Astha to dispay correct rating plug value
                        {
                            //Removed conditon !Global.IsUndoLock to fix dropdown value of rating getting changed on undo.
                            //JIRA ID- PXPM-8832
                            if ((Global.IsOffline && Global.isnewfile == true && !Global.IsUndoLock) && (comboBox.SelectedIndex == -1)
                                 && ((setting.ID == "SYS01" && (Global.device_type == Global.NZMDEVICE || Global.device_type == Global.MCCBDEVICE))
                                      || (setting.ID == "GEN02A" && setting.visible == true && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
                                      || (setting.ID == "SYS001" && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                                      || (setting.ID == "SYS001A" && (Global.device_type == Global.ACB_PXR35_DEVICE))))
                            {
                                //Set default current rating as maximum for ACB,MCCB and NZM in offline

                            }
                            else
                            {
                                setting.UnsubscribeFromComboBoxSelectionChangedEvent(ref comboBox);
                                comboBox.SelectedIndex = selectedIndex;
                            }
                        }
                        selectedIndex++;
                        //  }

                    }
                }

                if (((setting.ID == "SYS01" && (Global.device_type == Global.NZMDEVICE || Global.device_type == Global.MCCBDEVICE))
                      || (setting.ID == "GEN02A" && setting.visible == true && (Global.device_type == Global.MCCBDEVICE || Global.device_type == Global.NZMDEVICE))
                      || (setting.ID == "SYS001" && (Global.device_type == Global.ACBDEVICE || Global.device_type == Global.ACB_02_01_XX_DEVICE  || Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.PTM_DEVICE))
                      /*|| (setting.ID == "SYS001A" && (Global.device_type == Global.ACB_PXR35_DEVICE ))*/)
                  && Global.IsOffline)
                {
                    if (Global.isnewfile == true && (setting.ID != "GEN02A"||Global.device_type==Global.NZMDEVICE) && comboBox.Items != null)
                        setting.selectionDefault = Convert.ToString(comboBox.Items.Count - 1);

                    //string[] tagInfo = (string[])comboBox.Tag;

                    //if (tagInfo == null)
                    //{
                    //    tagInfo = new string[3];
                    //    tagInfo[0] = setting.selectionValue;
                    //    tagInfo[1] = "";
                    //    tagInfo[2] = "";
                    //    comboBox.Tag = null;
                    //    comboBox.Tag = tagInfo;
                    //}
                    //else
                    //{
                    //    tagInfo[0] = setting.selectionValue;
                    //    tagInfo[1] = tagInfo[1].Trim();
                    //    tagInfo[2] = tagInfo[2].Trim();
                    //    comboBox.Tag = null;
                    //    comboBox.Tag = tagInfo;
                    //}
                }
                setting.SetupComboBox(ref comboBox, ref label_unit);

            }
            catch (Exception ex)
            {
                LogExceptions.LogExceptionToFile(ex);
            }
        }

        private static ToolTip createComboItemToolTip(String _description)
        {
            TextBlock description = new TextBlock();
            description.TextWrapping = TextWrapping.Wrap;
            description.Inlines.Add(_description);
            ToolTip tooltip_descripter = new ToolTip();
            tooltip_descripter.Content = description;
            tooltip_descripter.SetResourceReference(ToolTip.StyleProperty, "ToolTip_ComboBoxItem");
            return tooltip_descripter;
        }
        private static Grid createtxt(ref Settings setting, string LabelInfo)
        {
            // text box
            string strNumberFormat = "";
            string[] tagInfo = new string[3];
            TextBox textBox = new TextBox();
            ToolTip toolTip;
            textBox.Name = "txt_" + setting.ID;

            //  string strFormattedOutpit = string.Format(
            textBox.Text = Convert.ToDouble(setting.textvalue.ToString(strNumberFormat)).ToString();

            //Below line changed by sreejith
            textBox.IsEnabled = !setting.readOnly;

            tagInfo[0] = textBox.Text.Trim(); //stores original value of control
            tagInfo[1] = LabelInfo.Trim(); // stores the label to be displayed in summary
            tagInfo[2] = textBox.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow

            //set Valid flag based on below conditions.This will help in displaying red border to all those textboxes which are invalid at the time of screen load

            double displayValue = (double)((decimal)(setting.textvalue * setting.conversion));
            string errorDescription = "";
            bool isError = false;

            if (displayValue.ToString(CultureInfo.InvariantCulture).Trim() == "")
            {
                isError = true;
                errorDescription = Resource.StepSizeNotInRangeError;
            }

            if (isError == true)
            {
                setting.valid = false;
                textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle_text");
                toolTip = createToolTip(errorDescription);
                toolTip.Content = errorDescription;
                textBox.ToolTip = toolTip;
                toolTip.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
            }
            else
            {
                setting.valid = true;

                textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle_text");
                toolTip = createToolTip(setting.description);
                textBox.ToolTip = toolTip;
            }
            // Unit Label
            Label label_unit = new Label();
            label_unit.Content = setting.unit;
            label_unit.SetResourceReference(Label.StyleProperty, "UnitLabelStyle_text");

            ToolTipService.SetToolTip(toolTip, "");
            ToolTipService.SetShowDuration(textBox, 300000);
            ToolTipService.SetShowOnDisabled(textBox, true);

            textBox.Tag = tagInfo;

            // Connect setting TextBox to UI TextBox
            setting.setupTextBox(ref textBox, ref label_unit, ref toolTip);

            Grid grid = new Grid();

            TextBox textBoxOffline = new TextBox();
            textBoxOffline.IsEnabled = !setting.readOnly;

            textBoxOffline.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle_text");
            textBoxOffline.ToolTip = textBox.ToolTip;
            if (setting.readOnly && Global.IsOffline && setting.ID!="CCC007")
            {
                if ((String)label_unit.Content != "%")
                {
                    label_unit.Content = string.Empty;
                }
                setting.setupTextBox(ref textBoxOffline, ref label_unit, ref toolTip);
                if(setting.ID == "SYS03")
                {
                    textBox.Text = setting.textStrValue;
                    grid.Children.Add(textBox);
                }
                else
                {
                    grid.Children.Add(textBoxOffline);
                }
            }
            else
            {
                grid.Children.Add(textBox);
            }

            grid.Children.Add(label_unit);
            grid.SetResourceReference(Grid.StyleProperty, "TextBoxGridStyle_text");

            if (!setting.visible)
            {
                grid.Visibility = Visibility.Collapsed;

            }

            if ((!setting.bValue))
            {
                textBox.IsEnabled = setting.bValue;
                textBox.IsReadOnly = !setting.bValue;
            }

            return grid;
        }

        private  static Grid createcataloglbl(ref Settings setting, string LabelInfo)
        {
            Grid grid = new Grid();
            Label labelname = new Label();
            string[] tagInfo = new string[3];
            labelname.Name = "label_" + setting.ID;
            labelname.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
            labelname.Content = setting.textStrValue;
            tagInfo[0] = labelname.Content.ToString().Trim(); //stores original value of control
            tagInfo[1] = LabelInfo.Trim(); // stores the label to be displayed in summary
            tagInfo[2] = labelname.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow
                                                //commented for bug- Maintenance Mode values are getting blank after UNDO
                                                //if (!setting.visible && setting.ID != "GEN02")
                                                //{
                                                //    labelname.Visibility = Visibility.Collapsed;
                                                //}
            setting.label_catalogNumber = labelname;
            labelname.Tag = tagInfo;
            grid.Children.Add(labelname);
            grid.SetResourceReference(Grid.StyleProperty, "TextBoxGridStyle");
            return grid;
        }

        private static Grid createTextBox(ref Settings setting, string LabelInfo)
        {
            // text box
            string strNumberFormat = "";

            string[] tagInfo = new string[3];
            TextBox textBox = new TextBox();
            ToolTip toolTip;
            textBox.Name = "txt_" + setting.ID;
            string lv_stepSize = Global.updateValueonCultureBasis(setting.strStepSize);
            string originalStepSize = Global.updateValueonCultureBasis(setting.stepsize.ToString());
            string stepSize = lv_stepSize;
            //if (originalStepSize.Length > lv_stepSize.Length)
            //{
            //    stepSize = originalStepSize;
            //}

            for (int i = 0; i < stepSize.Length; i++)
            {
                string currentText = stepSize.Substring(i, 1);
                if (currentText == ".")
                {
                    strNumberFormat = strNumberFormat + ".";
                }
                else
                {
                    strNumberFormat = strNumberFormat + "0";
                }
            }

            //   textBox.Text = Convert.ToDouble(setting.numberValue.ToString(strNumberFormat)).ToString();
            textBox.Text = Global.updateValueonCultureBasis(Convert.ToDouble(setting.numberDefault.ToString()).ToString());

            //Below line changed by sreejith
            textBox.IsEnabled = !setting.readOnly;
            if (!setting.bValue && setting.type == Settings.Type.type_bNumber)
            {
                textBox.IsEnabled = setting.bValue;
            }

            //For Motor protection group appending its subgroup name before its setpoint name - For differentiating in Change summary 
            //eg. Over Voltage - Pickup
            if (setting.GroupID == "04" || setting.GroupID == "004")
            {
                tagInfo[1] = ((Group)(TripUnit.groups[4])).subgroups[setting.subgrp_index].name + " - " +
                        LabelInfo.Trim();
            }
            else if ((Global.device_type == Global.ACB_03_00_XX_DEVICE || Global.device_type == Global.ACB_03_01_XX_DEVICE || Global.device_type == Global.ACB_03_02_XX_DEVICE || Global.device_type == Global.PTM_DEVICE) && setting.GroupID == "01" &&
                (setting.subgrp_index == 5 || setting.subgrp_index == 6 || setting.subgrp_index == 7 || setting.subgrp_index == 8 ||
                setting.subgrp_index == 9 || setting.subgrp_index == 10 || setting.subgrp_index == 11 || setting.subgrp_index == 12 || setting.subgrp_index == 13 || setting.subgrp_index == 14 || setting.subgrp_index == 15 || setting.subgrp_index == 16))
            {
                tagInfo[1] = ((Group)(TripUnit.groups[1])).subgroups[setting.subgrp_index].name + " - " +
                        LabelInfo.Trim();
            }
            else
            {
                tagInfo[1] = LabelInfo.Trim(); // stores the label to be displayed in summary
            }

            tagInfo[0] = textBox.Text.Trim(); //stores original value of control
            tagInfo[2] = textBox.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow

            //set Valid flag based on below conditions.This will help in displaying red border to all those textboxes which are invalid at the time of screen load

            //double displayValue = (double)((decimal)(setting.numberDefault * setting.conversion));
            double displayValue = setting.numberDefault;

            string errorDescription = "";
            bool isError = false;


            if (displayValue.ToString(CultureInfo.InvariantCulture).Trim() == "")
            {
                isError = true;
                errorDescription = Resource.StepSizeNotInRangeError;
            }

            //if ((displayValue.ToString(CultureInfo.InvariantCulture).Trim() != "") && (decimal)(displayValue) % (decimal)(setting.stepsize) != 0)
            //{
            //    isError = true;
            //    errorDescription = Resource.StepSizeIncorrectError;
            //}

            if ((displayValue.ToString(CultureInfo.InvariantCulture).Trim() != "") && (((decimal)(displayValue) % (decimal)(setting.stepsize) != 0)) && (Math.Round((decimal)(displayValue), 2) % (decimal)(setting.stepsize) != 0))
            {
                isError = true;
                errorDescription = Resource.StepSizeIncorrectError;
            }

            if ((displayValue.ToString(CultureInfo.InvariantCulture).Trim() != "") && (displayValue < setting.min))
            {
                isError = true;
                errorDescription = Resource.ValueNotGreaterThanMinError;
            }
            if ((displayValue.ToString(CultureInfo.InvariantCulture).Trim() != "") && (displayValue > setting.max))
            {
                isError = true;
                errorDescription = Resource.ValueNotLesserThanMaxError;
            }

            if (isError == true)
            {
                setting.valid = false;
                textBox.SetResourceReference(TextBox.StyleProperty, "TextBoxErrorStyle");
                toolTip = createToolTip(setting.min, setting.max, setting.stepsize, setting.unit, errorDescription);
                textBox.ToolTip = toolTip;
                toolTip.SetResourceReference(ToolTip.StyleProperty, "ToolTipStyle_TextBoxError");
            }
            else
            {
                setting.valid = true;
                textBox.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");

                //Added by Sarika to fix - PXPM-7287 (show tooltip inline with min,max and unit for below setpoints)

                if (setting.ID != null && (setting.ID == "ADVA012" || setting.ID == "ADVA007" || setting.ID == "ADVA032" || setting.ID == "ADVA033"
                    || setting.ID == "ADVA030" || setting.ID == "ADVA031"  || setting.ID == "ADVA039" || setting.ID == "ADVA041" || setting.ID == "ADVA036"
                    || setting.ID == "ADVA037" || setting.ID == "ADVA034" || setting.ID == "ADVA035"))
                {
                    toolTip = createToolTip(setting.min, setting.max, setting.stepsize, setting.unit, setting.description, true);
                }
                else
                {
                    toolTip = createToolTip(setting.min, setting.max, setting.stepsize, setting.unit, setting.description);
                }

                textBox.ToolTip = toolTip;
            }

            // Unit Label
            Label label_unit = new Label();
            label_unit.Content = setting.unit;
            label_unit.SetResourceReference(Label.StyleProperty, "UnitLabelStyle");

            ToolTipService.SetToolTip(toolTip, "");
            ToolTipService.SetShowDuration(textBox, 300000);
            ToolTipService.SetShowOnDisabled(textBox, true);

            textBox.Tag = tagInfo;

            if (setting.bcalculated && setting.ID != "CPC041" && (Global.device_type==Global.NZMDEVICE ? setting.ID != "CPC043" : setting.ID != "CPC042") && !String.Equals(label_unit.Content,"%"))     //Added by Astha to dispay units in textbox only for settings which do not have calculated value
            {
                label_unit.Content = string.Empty;
            }
            // Connect setting TextBox to UI TextBox
            setting.setupTextBox(ref textBox, ref label_unit, ref toolTip);


            Grid grid = new Grid();

            TextBox textBoxOffline = new TextBox();
            textBoxOffline.IsEnabled = !setting.readOnly;
            textBoxOffline.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
            textBoxOffline.ToolTip = textBox.ToolTip;
            if (!setting.bValue && setting.type == Settings.Type.type_bNumber)
            {
                textBoxOffline.IsEnabled = setting.bValue;
            }
            if (setting.readOnly && Global.IsOffline)
            {
                if (Global.device_type == Global.MCCBDEVICE && setting.ID == "CPC081" && String.IsNullOrWhiteSpace(textBoxOffline.Text) && textBoxOffline.Tag == null)
                {
                    textBoxOffline.Text = textBox.Text;
                    textBoxOffline.Name = "txt_CPC081";
                    textBoxOffline.Tag = tagInfo;
                }
                if (Global.device_type == Global.ACB_PXR35_DEVICE && setting.ID == "CPC151A" && String.IsNullOrWhiteSpace(textBoxOffline.Text) && textBoxOffline.Tag == null)

                {
                    textBoxOffline.Text = textBox.Text;
                    textBoxOffline.Name = "txt_CPC151A";
                    textBoxOffline.Tag = tagInfo;
                }

                if (Global.device_type == Global.ACB_PXR35_DEVICE && setting.ID == "CPC141" && String.IsNullOrWhiteSpace(textBoxOffline.Text) && textBoxOffline.Tag == null)

                {
                    textBoxOffline.Text = textBox.Text;
                    textBoxOffline.Name = "txt_CPC141";
                    textBoxOffline.Tag = tagInfo;
                }

                setting.setupTextBox(ref textBoxOffline, ref label_unit, ref toolTip);
                grid.Children.Add(textBoxOffline);
            }
            else
            {
                grid.Children.Add(textBox);
            }

            grid.Children.Add(label_unit);
            //if (!setting.visible)
            //{
            //    grid.Visibility = Visibility.Collapsed;
            //}
            grid.SetResourceReference(Grid.StyleProperty, "TextBoxGridStyle");

            return grid;
        }
        public static ToolTip createToolTip(String _description)
        {
            ///////////// Tool Tips//////////////////////////////
            StackPanel toolTipContent = new StackPanel();

            if (_description != "" && _description != null)
            {
                TextBlock description = new TextBlock();
                description.TextWrapping = TextWrapping.Wrap;
                description.Inlines.Add(_description);
                description.TextAlignment = TextAlignment.Left;
                toolTipContent.Children.Add(description);
            }
            ToolTip tooltip_descripter = new ToolTip();
            tooltip_descripter.Content = toolTipContent;
            tooltip_descripter.SetResourceReference(ToolTip.StyleProperty, "ToolTip_TextBox");
            tooltip_descripter.StaysOpen = true;
            return tooltip_descripter;
        }
        public static ToolTip createToolTip(double _min, double _max, double _stepSize, String _unit, String _description ,bool isCombineDescription = false)
        {
            ///////////// Tool Tips//////////////////////////////
            StackPanel toolTipContent = new StackPanel();

            // Range
            TextBlock range = new TextBlock();

            //_min = double.Parse(_min.ToString().Replace(",", "."),NumberStyles.AllowDecimalPoint);
            //_max = double.Parse(_max.ToString().Replace(",", "."), NumberStyles.AllowDecimalPoint);
            if (isCombineDescription)
            {
                range.Inlines.Add(Resource.strRange + Global.updateValueonCultureBasis(_min.ToString()) + "-" + Global.updateValueonCultureBasis(_max.ToString()) + " " + _description);
            }
            else
            {
                range.Inlines.Add(Resource.strRange + Global.updateValueonCultureBasis(_min.ToString()) + "-" + Global.updateValueonCultureBasis(_max.ToString()) + " " + _unit);
            }
            //if (CultureInfo.CurrentUICulture.Name == "en-US" || CultureInfo.CurrentUICulture.Name == "zh-CHS")
            //{
            //    range.Inlines.Add(Resource.strRange + _min.ToString().Replace(",", ".") + "-" + _max.ToString().Replace(",", ".") + " " + _unit);
            //}
            //else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES")
            //{
            //    range.Inlines.Add(Resource.strRange + _min.ToString().Replace(".", ",") + "-" + _max.ToString().Replace(".", ",") + " " + _unit);
            //}

            //     range.Inlines.Add(Resource.strRange + _min + "-" + _max + " " + _unit);


            range.TextAlignment = TextAlignment.Left;
            toolTipContent.Children.Add(range);

            // Step Size
            // _stepSize = double.Parse(_stepSize.ToString().Replace(",", "."), NumberStyles.AllowDecimalPoint);

            TextBlock stepSize = new TextBlock();

            stepSize.Inlines.Add(Resource.strStepSize + Global.updateValueonCultureBasis(_stepSize.ToString()));
            //if (CultureInfo.CurrentUICulture.Name == "en-US" || CultureInfo.CurrentUICulture.Name == "zh-CHS")
            //{
            //    stepSize.Inlines.Add(Resource.strStepSize + _stepSize.ToString().Replace(",", "."));
            //}
            //else if (CultureInfo.CurrentUICulture.Name == "de-DE" || CultureInfo.CurrentUICulture.Name == "es-ES")
            //{
            //    stepSize.Inlines.Add(Resource.strStepSize + _stepSize.ToString().Replace(".", ","));
            //}

            stepSize.TextAlignment = TextAlignment.Left;
            toolTipContent.Children.Add(stepSize);

            // Description
            if (_description != "" && null != _description && isCombineDescription == false)
            {
                TextBlock description = new TextBlock();
                description.TextWrapping = TextWrapping.Wrap;
                description.Inlines.Add(_description);
                description.TextAlignment = TextAlignment.Left;
                toolTipContent.Children.Add(description);
            }
            ToolTip tooltip_descripter = new ToolTip();
            tooltip_descripter.Content = toolTipContent;
            tooltip_descripter.SetResourceReference(ToolTip.StyleProperty, "ToolTip_TextBox");
            tooltip_descripter.StaysOpen = true;
            return tooltip_descripter;
        }
        private static CheckBox createCheckBox(ref Settings setting)
        {

            string[] tagInfo = new string[2];
            CheckBox checkbox = new CheckBox();
            checkbox.Content = setting.name;
            checkbox.Name = "chk_" + setting.ID;
            checkbox.SetResourceReference(CheckBox.StyleProperty, "TriggerCheckBox");
            checkbox.IsChecked = setting.bValue;
            tagInfo[0] = setting.bValue.ToString();
            tagInfo[1] = checkbox.Name;
            checkbox.Tag = tagInfo;
            ToolTip toolTip = createToolTip(setting.min, setting.max, setting.stepsize, setting.unit, setting.description);
            checkbox.ToolTip = toolTip;

            ToolTipService.SetToolTip(toolTip, "");
            ToolTipService.SetShowDuration(checkbox, 300000);
            // Connect setting checkbox to UI checkbox
            setting.SetupCheckBox(ref checkbox);
            return checkbox;
        }

        private static ToggleButton createtogglebutton(ref Settings setting)
        {
            string[] tagInfo = new string[2];
            ToggleButton toglebutton = new ToggleButton();
            if (setting.type == Settings.Type.type_toggle)
            {
                toglebutton.SetResourceReference(ToggleButton.StyleProperty, "Togglestyle");
            }
            else
            {
                toglebutton.SetResourceReference(ToggleButton.StyleProperty, "TogglestyleForbNumber");
            }

            toglebutton.Name = "tgl_" + setting.ID;
            toglebutton.IsChecked = setting.bValue;


            if (setting.description != null && setting.description.Trim() != "")
            {

                ToolTip toolTip = createToolTip(setting.description);
                toglebutton.ToolTip = toolTip;
                ToolTipService.SetToolTip(toolTip, "");
                ToolTipService.SetShowDuration(toglebutton, 300000);

            }
            else
            {
                toglebutton.ClearValue(RadioButton.ToolTipProperty);
            }


            if (setting.bValue == false)
            {
                tagInfo[0] = setting.OffLabel.ToString();
            }
            else
            {
                tagInfo[0] = setting.OnLabel.ToString();
            }
            tagInfo[1] = toglebutton.Name;

            toglebutton.Tag = tagInfo;
            if (setting.type == Settings.Type.type_bNumber)
            {
                if (setting.bValue)
                {
                    toglebutton.Content = setting.OnLabel;
                }
                else
                {
                    toglebutton.Content = setting.OffLabel;
                }
            }

            if (setting.ID == "GEN12") // for MP toggle, set global falg. Required in PDF report
            {
                Global.MPsettingOnPageLoad = tagInfo[0];
            }
            //ToolTip toolTip = createToolTip(setting.min, setting.max, setting.stepsize, setting.unit, setting.description);
            //toglebutton.ToolTip = toolTip;

            setting.SetupToggleButton(ref toglebutton);

            return toglebutton;
        }

        private static StackPanel createBNumber(ref Settings setting)
        {
            StackPanel bnum = new StackPanel();
            Grid toggleGrid = new Grid();
            StackPanel labelStack = new StackPanel();
            StackPanel toggleStack = new StackPanel();

            labelStack.SetResourceReference(StackPanel.StyleProperty, "toggleLabelStackPannelInExpander");
            toggleStack.SetResourceReference(StackPanel.StyleProperty, "toggleStackPannelInExpander");

            //creating Lable and toggle
            Label label = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
            setting.label_name = label;
            //label.Width = 50;
            labelStack.Children.Add(setting.label_name);
            ToggleButton togglebutton = createtogglebutton(ref setting);
            toggleStack.Children.Add(togglebutton);

            ColumnDefinition[] columns = new ColumnDefinition[2];
            columns[0] = new ColumnDefinition();
            columns[1] = new ColumnDefinition();
            toggleGrid.ColumnDefinitions.Add(columns[0]);
            toggleGrid.ColumnDefinitions.Add(columns[1]);

            Grid.SetColumn(labelStack, 0);
            Grid.SetColumn(toggleStack, 1);

            toggleGrid.Children.Add(labelStack);
            toggleGrid.Children.Add(toggleStack);
            bnum.Children.Add(toggleGrid);
            // Connect Setting Bnumber to UI Bnumber
            //CheckBox chkBNum = createCheckBox(ref setting);
            //bnum.Children.Add(chkBNum);
            //bnum.Children.Add(createTextBox(ref setting, chkBNum.Content.ToString().Trim() + " Value"));

            return bnum;
        }
        private static ListBox createListBox(ref Settings setting, string lblListBox, int ListBoxCount)
        {
            string[] tagInfo = new string[2];
            ListBox list = new ListBox();

            list.SetResourceReference(ListBox.StyleProperty, "ListBox_whiteStyle");

            for (int i = 0; i < setting.itemList.Count; i++)
            {
                item_ListBox item = (item_ListBox)setting.itemList[i];
                if (!item.isHidden)
                {
                    string chkID = ListBoxCount + "_" + i;

                    ((item_ListBox)setting.itemList[i]).myCheckBox = createListBoxItem(item.name, item.isSelected, setting, setting.ID + "_" + chkID);

                    if(list.Items != null)
                        list.Items.Add(((item_ListBox)setting.itemList[i]).myCheckBox);
                }

            }
            if (!setting.visible)
            {
                list.Visibility = Visibility.Collapsed;
            }

            tagInfo[0] = ""; // setting.selectionValue.ToString();
            tagInfo[1] = lblListBox.Trim();
            list.Tag = tagInfo;

            setting.SetListBox(ref list);


            return list;
        }

        private static CheckBox createListBoxItem(String _name, Boolean _selected, Settings setting, string chkIndex)
        {
            string[] tagInfo = new string[2];
            CheckBox checkbox = new CheckBox();
            checkbox.Content = _name;
            checkbox.Name = "chk_" + chkIndex;
            tagInfo[0] = _selected.ToString();
            tagInfo[1] = checkbox.Name;
            checkbox.Tag = tagInfo;
            checkbox.SetResourceReference(CheckBox.StyleProperty, "TriggerCheckBox");
            checkbox.IsChecked = _selected;
            setting.SetupCheckBox(ref checkbox);
            return checkbox;
        }
        private static UIElement createSettingDisplay(Settings setting)
        {
            Grid panel_setting = new Grid();
            panel_setting.SetResourceReference(Grid.StyleProperty, "GridSetpoint");
            StackPanel leftStackPanel = new StackPanel();
            leftStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Left");
            StackPanel rightStackPanel = new StackPanel();
            if (setting.subgrp_index == -1)  // Style to align the UI for setpoints inside subgroup and outside subgroup
            {
                rightStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Right_1");
            }
            else
            {
                rightStackPanel.SetResourceReference(StackPanel.StyleProperty, "StackPanelInExpander_Right");
            }
            rightStackPanel.Orientation = Orientation.Horizontal;
            NameScope.SetNameScope(panel_setting, new NameScope());
            int listBoxCount = 1;

            // increaser and decreaser button
            var brushUp = new ImageBrush();
            brushUp.ImageSource = new BitmapImage(new Uri(Global.plusimage, UriKind.Relative));
            var brushDown = new ImageBrush();
            brushDown.ImageSource = new BitmapImage(new Uri(Global.minusimage, UriKind.Relative));
            Button decreaser = new Button();
            decreaser.Name = "btn_decrease" + setting.ID;
            decreaser.SetResourceReference(StackPanel.StyleProperty, "TextDecreaserButton");
            decreaser.Background = brushDown;
            setting.decreaseButton = decreaser;
            Button increaser = new Button();
            increaser.Name = "btn_increase" + setting.ID;
            increaser.Background = brushUp;
            setting.increaseButton = increaser;
            increaser.SetResourceReference(StackPanel.StyleProperty, "TextIncreaseButton");
            setting.increaseButton.IsEnabled = !setting.readOnly;
            setting.decreaseButton.IsEnabled = !setting.readOnly;
            // End of increaser and decreaser buttons 

            if (setting.type == Settings.Type.type_selection)
            {
                Label lblCombo = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                // Added to handel the display of the label on Main screen
                if ((setting.ID.StartsWith("IOMC")))
                {
                    lblCombo.Content = "";
                }
                leftStackPanel.Children.Add(lblCombo);
                if (lblCombo.Content != null)
                {
                    ComboBox comboBox = createComboBox(ref setting, lblCombo.Content.ToString().Trim());
                    rightStackPanel.Children.Add(comboBox);
                    panel_setting.RegisterName("cmb_" + setting.ID, comboBox);
                }

                if (setting.bcalculated && setting.calculation.Contains("="))
                {
                    Label calculated_Value = new Label();

                    calculated_Value.Name = "lbl2_" + setting.ID;
                    calculated_Value.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                    calculated_Value.Content = Global.updateValueonCultureBasis(setting.CalculatedValue(setting.calculation).ToString()) + " A";
                    StackPanel spaces = new StackPanel();
                    spaces.Width = 15;
                    setting.emptyspace = spaces;
                    setting.label_calculation = calculated_Value;
                    rightStackPanel.Children.Add(spaces);
                    rightStackPanel.Children.Add(calculated_Value);
                }



            }
            else if (setting.type == Settings.Type.type_number)
            {
                Label label = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                setting.label_name = label;
                leftStackPanel.Children.Add(setting.label_name);
                panel_setting.RegisterName("lbl_" + setting.ID, label);

                Grid grdTextbox = createTextBox(ref setting, label.Content.ToString().Trim());

                setting.setupIncreaseButton(ref increaser);
                setting.setupDecreaseButton(ref decreaser);
                rightStackPanel.Children.Add(decreaser);
                rightStackPanel.Children.Add(grdTextbox);
                rightStackPanel.Children.Add(increaser);


                if (setting.bcalculated && setting.calculation.Contains("="))
                {
                    Label calculated_Value = new Label();

                    calculated_Value.Name = "lbl2_" + setting.ID;
                    calculated_Value.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                    calculated_Value.Content = Global.updateValueonCultureBasis(setting.CalculatedValue(setting.calculation).ToString()) + " A";
                    StackPanel spac = new StackPanel();
                    spac.Width = 15;
                    setting.label_calculation = calculated_Value;
                    setting.emptyspace = spac;
                    rightStackPanel.Children.Add(spac);
                    rightStackPanel.Children.Add(calculated_Value);
                }
                //IP002CalValuLabel
                else if (setting.ID == "IP002")
                {
                    Label calculated_Value = new Label();

                    calculated_Value.Name = "lbl2_" + setting.ID;
                    calculated_Value.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                    StackPanel spac = new StackPanel();
                    spac.Width = 15;
                    setting.label_calculation = calculated_Value;
                    //    setting.emptyspace = spac;
                    //    rightStackPanel.Children.Add(spac);
                    //    rightStackPanel.Children.Add(calculated_Value);
                }
            }
            else if (setting.type == Settings.Type.type_text)
            {
                Label label = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                setting.label_name = label;
                leftStackPanel.Children.Add(setting.label_name);
                panel_setting.RegisterName("lbl_" + setting.ID, label);
                Grid grdTextbox = null;
                if (setting.ID.StartsWith("IP"))
                {
                    switch (setting.ID)
                    {
                        case "IP001":
                            grdTextbox = createIPAddressTextBox(ref setting, label.Content.ToString().Trim());
                            break;
                        case "IP002":
                            grdTextbox = createIPAddressTextBox(ref setting, label.Content.ToString().Trim());
                            //rightStackPanel.Children.Add(createSubnetMaskCal(ref setting, ref setting.textBox));
                            break;
                        case "IP003":
                            grdTextbox = createIPAddressTextBox(ref setting, label.Content.ToString().Trim());
                            break;
                    }
                }
                else if (setting.textStrValue != string.Empty || setting.ID == "SYS03") // Label creation for catalog number 
                {
                    grdTextbox = createcataloglbl(ref setting, label.Content.ToString().Trim());
                    grdTextbox.Visibility = Visibility.Visible;
                }
                else if (setting.textStrValue != string.Empty || setting.ID == "GEN02") //label creation for Firmware Version
                {
                    grdTextbox = createcataloglbl(ref setting, label.Content.ToString().Trim());
                    grdTextbox.Visibility = Visibility.Visible;
                }
                else
                {
                    if (setting.ID == "CC05" || setting.ID == "CC06" || setting.ID == "CCC05A" || setting.ID == "CCC06A" || setting.ID == "SYS18" || setting.ID == "SYS19")
                    {
                        grdTextbox = null;
                    }
                    else
                    {
                        grdTextbox = createtxt(ref setting, label.Content.ToString().Trim());
                        grdTextbox.Visibility = Visibility.Visible;
                    }
                }
                if (grdTextbox != null)
                {
                    rightStackPanel.Children.Add(grdTextbox);
                }
            }
            else if (setting.type == Settings.Type.type_bool)
            {
                leftStackPanel.Children.Add(createCheckBox(ref setting));
            }
            else if (setting.type == Settings.Type.type_bNumber)
            {
                leftStackPanel.Children.Add(createBNumber(ref setting));
                Grid grdTextbox = createTextBox(ref setting, setting.name);
                //rightStackPanel.Children.Add(grdTextbox);
                setting.setupIncreaseButton(ref increaser);
                setting.setupDecreaseButton(ref decreaser);
                rightStackPanel.Children.Add(decreaser);
                rightStackPanel.Children.Add(grdTextbox);
                rightStackPanel.Children.Add(increaser);

                increaser.IsEnabled = setting.bValue;
                decreaser.IsEnabled = setting.bValue;

                if (setting.bcalculated && setting.calculation.Contains("="))
                {
                    Label calculated_Value = new Label();

                    calculated_Value.Name = "lbl2_" + setting.ID;
                    calculated_Value.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                    calculated_Value.Content = Global.updateValueonCultureBasis(setting.CalculatedValue(setting.calculation).ToString()) + " A";
                    StackPanel spac = new StackPanel();
                    spac.Width = 15;
                    setting.label_calculation = calculated_Value;
                    setting.emptyspace = spac;
                    rightStackPanel.Children.Add(spac);
                    rightStackPanel.Children.Add(calculated_Value);
                }
                //setting.SetupToggleButton((ToggleButton)(panel_setting.Children[1]));

            }
            else if (setting.type == Settings.Type.type_toggle)
            {
                Label label = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                setting.label_name = label;
                leftStackPanel.Children.Add(setting.label_name);
                if (!(setting.ID == "SYS131B" || setting.ID == "SYS141B" || setting.ID == "SYS151B"))
                {
                    TextBlock leftlabel = createLeftLabel(setting.OffLabel, setting.visible, ref setting.leftlabel_forToggle, ref setting);
                    setting.leftlabel_forToggle = leftlabel;
                    rightStackPanel.Children.Add(setting.leftlabel_forToggle);

                    ToggleButton togglebutton = createtogglebutton(ref setting);
                    togglebutton.IsEnabled = !setting.readOnly;

                    rightStackPanel.Children.Add(togglebutton);
                    TextBlock rightlabel = createRightLabel(setting.OnLabel, setting.visible, ref setting.rightlabel_forToggle, ref setting);
                    setting.rightlabel_forToggle = rightlabel;
                    rightStackPanel.Children.Add(setting.rightlabel_forToggle);
                }
                // if (togglebutton.Template != null)
                // {
                //     togglebutton.ApplyTemplate();

                //     // var control = togglebutton.Template.FindName("LeftdetailsTextBlock", togglebutton) as TextBlock;
                ////     NameScope.GetNameScope(togglebutton).RegisterName("LeftdetailsTextBlock", togglebutton);

                //     //var child = FindVisualChildren<ToggleButton>(togglebutton);
                //     var lp= LogicalTreeHelper.FindLogicalNode(togglebutton, "LeftdetailsTextBlock");

                //     //var searchBox = VisualTreeHelper.FindAncestor<TextBox>(myDataGridCell, "SeachTextBox");

                //     //var specificChild = VisualTreeHelper.FindChild<Label>(myDataGridCell, "MyCheckBoxLabel");

                //     TextBlock tBlockLeft = togglebutton.Template.FindName("LeftdetailsTextBlock", togglebutton) as TextBlock;
                //     TextBlock tBlockRight = togglebutton.Template.FindName("RightdetailsTextBlock", togglebutton) as TextBlock;

                //     if (setting.bValue)
                //     {

                //         tBlockRight.Text = setting.OnLabel;
                //     }
                //     else
                //     {
                //         tBlockLeft.Text = setting.OffLabel;
                //     }
                // }


            }
            else if (setting.type == Settings.Type.type_bSelection)
            {
                leftStackPanel.Children.Add(createBNumber(ref setting));
                ComboBox comboBox = createComboBox(ref setting, setting.ID.Trim());
                rightStackPanel.Children.Add(comboBox);
                //Label label = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                //setting.label_name = label;
                //panel_setting.Children.Add(setting.label_name);
            }
            else if (setting.type == Settings.Type.type_rPlugStyle)
            {
                panel_setting.Children.Add(createLabel(Resource.strCatalogNumber + TripUnit.userStyle, setting.visible, ref setting.label_name, ref setting));
                panel_setting.Children.Add(createLabel(Resource.strRatingPlug + TripUnit.userRatingPlug, setting.visible, ref setting.label_name, ref setting));
                if (Global.IsOffline)
                    panel_setting.Children.Add(createLabel(Resource.strFirmwareVersion + Global.appFirmware, setting.visible, ref setting.label_name, ref setting));
                else
                    panel_setting.Children.Add(createLabel(Resource.strFirmwareVersion + Global.deviceFirmware, setting.visible, ref setting.label_name, ref setting));
            }
            else if (setting.type == Settings.Type.type_listBox)
            {

                Label lblListBox = createLabel(setting.name, setting.visible, ref setting.label_name, ref setting);
                panel_setting.Children.Add(lblListBox);
                panel_setting.Children.Add(createListBox(ref setting, lblListBox.Content.ToString().Trim(), listBoxCount));
                listBoxCount++;
            }
            else if (setting.type == Settings.Type.type_split)
            {
                for (int i = 0; i < setting.setpoint.Length; i++)
                {
                    Label lblCombo = createLabel(((Settings)setting.setpoint[i]).name, setting.visible, ref setting.label_name, ref setting);
                    panel_setting.Children.Add(lblCombo);
                    panel_setting.Children.Add(createComboBox(ref setting.setpoint[i], lblCombo.Content.ToString().Trim()));
                }
            }
            else if (setting.type == Settings.Type.type_IPAddress)
            {

            }
            else
            {
                panel_setting.Children.Add(createLabel(setting.name, setting.visible, ref setting.label_name, ref setting));
            }

            if (!setting.visible)
            {
                panel_setting.Visibility = Visibility.Collapsed;
            }

            rightStackPanel.Children.Add(createLabel_NA(ref setting, ref setting.label_notavailable));
            // Add Perview Button for LCD Text Orientation 
            if (setting.ID == "SYS12")
            {
                Button alignbtn = new Button();

                alignbtn.Name = "btnl2_" + setting.ID;
                //alignbtn.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                alignbtn.Content = Resource.Preview;
                alignbtn.MinWidth = 50;
                alignbtn.Width = Double.NaN;
                alignbtn.Padding = new Thickness(4);
                alignbtn.Height = 28;
                StackPanel space = new StackPanel();
                space.Width = 30;
                setting.emptyspace = space;
                setting.rotation_button = alignbtn;
                rightStackPanel.Children.Add(space);
                rightStackPanel.Children.Add(alignbtn);
                setting.setupLCDTextOrientationButton(ref alignbtn);
            }
            //if ((setting.ID == "SYS025") && (Global.device_type == Global.ACB_PXR35_DEVICE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE))
            //{
            //    Button ChangeActiveSetbtn = new Button();

            //    ChangeActiveSetbtn.Name = "btnl2_" + setting.ID;
            //    //alignbtn.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
            //    ChangeActiveSetbtn.Content = Resource.ChangeActiveSet;
            //    ChangeActiveSetbtn.MinWidth = 50;
            //    ChangeActiveSetbtn.Width = 200;//Double.NaN;
            //    ChangeActiveSetbtn.Padding = new Thickness(4);
            //    ChangeActiveSetbtn.Height = 28;
            //    ChangeActiveSetbtn.IsEnabled = true;
            //    ChangeActiveSetbtn.Margin = new Thickness(5, 0, 0, 0);

            //    ChangeActiveSetbtn.SetResourceReference(Button.BackgroundProperty, "ButtonNormalBackgroundNew");

            //    ChangeActiveSetbtn.BorderBrush = System.Windows.Media.Brushes.DarkGray;

            //    setting.rotation_button = ChangeActiveSetbtn;

            //    rightStackPanel.Children.Add(ChangeActiveSetbtn);
            //    setting.ShowChangeActiveSetScreen(ref ChangeActiveSetbtn);
            //}

            if ((setting.ID == "SYS131B" || setting.ID == "SYS141B" || setting.ID == "SYS151B") && (Global.device_type == Global.ACB_PXR35_DEVICE || Global.selectedTemplateType == Global.ACB_PXR35_TEMPLATE))
            {
                Button alignbtn = new Button();

                alignbtn.Name = "btnl2_" + setting.ID;
                //alignbtn.SetResourceReference(Label.StyleProperty, "BlackLabelStyle");
                alignbtn.Content = Resource.ViewChangeConfiguration;
                alignbtn.MinWidth = 50;
                alignbtn.Width = 225;//Double.NaN;
                alignbtn.Padding = new Thickness(4);
                alignbtn.Height = 28;
                alignbtn.IsEnabled = true;
                alignbtn.Margin = new Thickness(5, 0, 0, 0);

                alignbtn.SetResourceReference(Button.BackgroundProperty, "ButtonNormalBackgroundNew");

                alignbtn.BorderBrush = System.Windows.Media.Brushes.DarkGray;

                setting.rotation_button = alignbtn;

                rightStackPanel.Children.Add(alignbtn);
                setting.ShowRelayScreen(ref alignbtn);

                //switch (setting.ID)
                //{
                //    case "SYS131B":
                //        if (TripUnit.getRelay1ValuePXR35().relayOriginalValue.Replace('0', ' ').Trim() == string.Empty)
                //        {
                //            setting.bValue = false;
                //        }

                //        setting.toggle.IsChecked = setting.bValue;
                //        alignbtn.IsEnabled = setting.bValue;
                //        break;
                //    case "SYS141B":
                //        if (TripUnit.getRelay2ValuePXR35().relayOriginalValue.Replace('0', ' ').Trim() == string.Empty)
                //        {
                //            setting.bValue = false;
                //        }

                //        setting.toggle.IsChecked = setting.bValue;
                //        alignbtn.IsEnabled = setting.bValue;
                //        break;
                //    case "SYS151B":
                //        if (TripUnit.getRelay3ValuePXR35().relayOriginalValue.Replace('0', ' ').Trim() == string.Empty)
                //        {
                //            setting.bValue = false;
                //        }

                //        setting.toggle.IsChecked = setting.bValue;
                //        alignbtn.IsEnabled = setting.bValue;
                //        break;
                //}
            }

            if (setting.ID == "IP002")
            {
                setting.textBox.SetResourceReference(TextBox.StyleProperty, "TriggerSmallTextBoxStyle");

                //textBox.SetResourceReference(TextBox.StyleProperty, "TriggerSmallTextBoxStyle");


                setting.setupSubnetMask(ref setting.textBox);
                setting.getSubnetAddressFromIPNetMask(setting.textBox, null);
            }
            ColumnDefinition[] columns = new ColumnDefinition[2];
            columns[0] = new ColumnDefinition();
            columns[1] = new ColumnDefinition();
            panel_setting.ColumnDefinitions.Add(columns[0]);
            panel_setting.ColumnDefinitions.Add(columns[1]);

            Grid.SetColumn(leftStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);

            panel_setting.Children.Add(leftStackPanel);
            panel_setting.Children.Add(rightStackPanel);
            return panel_setting;
        }

        private static Grid createIPAddressTextBox(ref Settings setting, string LabelInfo)
        {
            // text box

            string[] tagInfo = new string[3];
            TextBox textBox = new TextBox();
            //textBox.Name = "txt_" + setting.ID;


            C1MaskedTextBox IPAddressBox = new C1MaskedTextBox();
            //IPAddressBox.IncludeLiterals = true;
            //IPAddressBox.Mask = "###.###.###.###";
            IPAddressBox.FontSize = 14;
            //ip_Controler.Height = 28;
            //ip_Controler.Width = 255;
            //ip_Controler.Visibility = Visibility.Visible;
            IPAddressBox.Name = setting.ID;
            IPAddressBox.Margin = new Thickness(5,0,0,0);
            IPAddressBox.PromptChar = ' ';
            IPAddressBox.TextAlignment = TextAlignment.Justify;
            IPAddressBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            IPAddressBox.Height = 30;
            IPAddressBox.Width = 225;


            setting.IPAddressControl = IPAddressBox;

            //textBox.Text = Convert.ToDouble(setting.numberValue.ToString(strNumberFormat)).ToString();

            //Below line changed by sreejith
            //textBox.IsEnabled = !setting.readOnly;

            //tagInfo[0] = textBox.Text.Trim(); //stores original value of control
            tagInfo[1] = LabelInfo.Trim(); // stores the label to be displayed in summary
            tagInfo[2] = IPAddressBox.Name.Trim(); // stores the Control Name. Used to set focus on control when user clcik on datarow
            string[] ipval = null;
            string ip=string.Empty;
            switch (setting.ID)
            {
                case "IP001":
                    //IPAddressBox.Mask = "000.000.000.000";
                    IPAddressBox.Mask = @"000\.000\.000\.000";
                    //ip = setting.IPaddress.Replace(",", ".");  value is getting overwritten    //#COVARITY FIX    240690
                    //ipval = ip.Split('.'); //Commented - Again diff value is getting assigned at next step
                    ipval = setting.IPaddress.Split('.');
                    for (int i = 0; i < 4; i++)
                    {
                        if (ipval.Length != 3)
                        {
                            ipval[i] = ipval[i].PadLeft(3, '0');
                        }
                    }
                    ip = ipval[0] + "." + ipval[1] + "." + ipval[2] + "." + ipval[3];
                    //IPAddressBox.AllowPromptAsInput = true;
                    IPAddressBox.Text = ip;
                    //IPAddressBox.AllowPromptAsInput = false;
                    break;
                case "IP002":
                    //IPAddressBox.Mask = @"255\.255\.255\.###";
                    //ip = "255.255.255.0" + setting.textvalue;
                    ////updated based on review changes  3.0.4 Review start 
                    //UInt32 t = 0xffffffff;
                    //string[] nib = new string[4];
                    //t = t << (32 - Convert.ToInt32(setting.textvalue));
                    //nib[0] =( (t >> 24) & 0xFF).ToString();
                    //nib[1] = ((t >> 16) & 0xFF).ToString();
                    //nib[2] = ((t >> 8) & 0xFF).ToString();
                    //nib[3] = (t & 0xFF).ToString();

                    //for (int i = 0; i < 4; i++)
                    //{
                    //    if (nib.Length != 3)
                    //    {
                    //        nib[i] = nib[i].PadLeft(3, '0');
                    //    }
                    //}

                    //ip = nib[0] + "." + nib[1] + "." + nib[2] + "." + nib[3];
                    //// this.dataGridView3.Rows[12].Cells[1].Value = "/" + ReceivedShortBuffer[16].ToString() + " - " + nib1.ToString() + "." + nib2.ToString() + "." + nib3.ToString() + "." + nib4.ToString();
                    ////===================================== 3.0.4 Review start end

                    //IPAddressBox.AllowPromptAsInput = true;
                    ////IPAddressBox.Mask = "255.255.255.###";
                    //IPAddressBox.Text = ip;
                    //IPAddressBox.AllowPromptAsInput = false;
                    break;
                case "IP003":

                    IPAddressBox.Mask = @"000\.000\.000\.000";
                    ip = setting.IPaddress.Replace(",", ".");
                    ipval = ip.Split('.');
                    //ipval = setting.IPaddress.Split('.');
                    for (int i = 0; i < 4; i++)
                    {
                        if (ipval.Length != 3)
                        {
                            ipval[i] = ipval[i].PadLeft(3, '0');
                        }
                    }
                    ip = ipval[0] + "." + ipval[1] + "." + ipval[2] + "." + ipval[3];
                    //IPAddressBox.AllowPromptAsInput = true;
                    IPAddressBox.Text = ip.ToString();
                    //IPAddressBox.AllowPromptAsInput = false;
                    break;
            }
            setting.IPaddress_default = ip;


            // Connect setting TextBox to UI TextBox
            Grid grid = new Grid();

            TextBox textBoxOffline = new TextBox();
            //textBoxOffline.IsEnabled = !setting.readOnly;
            textBoxOffline.SetResourceReference(TextBox.StyleProperty, "TriggerTextBoxStyle");
            textBoxOffline.ToolTip = textBox.ToolTip;

            //if (setting.readOnly && Global.IsOffline)
            //{
            //    grid.Children.Add(textBoxOffline);
            //}
            //else
            //{
            //    grid.Children.Add(textBox);
            //}
            setting.IPTextBox(ref IPAddressBox);
            IPAddressBox.Tag = tagInfo;
            grid.Children.Add(IPAddressBox);

            //if (!setting.visible)
            //{
            //    grid.Visibility = Visibility.Collapsed;
            //}

            return grid;
        }
    }
}