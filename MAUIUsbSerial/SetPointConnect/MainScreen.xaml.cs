using System;
using System.Collections.Generic;
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
using System.Linq;
using PXR.Screens.Scrs_Download_Language;
using PXR.Screens.Scrs_Test;
using System.Diagnostics;
using System.Windows.Shapes;
using C1.WPF.C1Chart.Extended;
using System.Windows.Media.Animation;
using C1.C1Preview;
using System.Text;
using RadioButton = System.Windows.Controls.RadioButton;
using Convert = System.Convert;
using System.Threading.Tasks;
using static CSJ2K.Color.ColorSpace;

namespace PXR.Screens
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : UserControl
    {
        public static MainScreen_ViewModel mainScreen_ViewModel;
        public MainScreen(String source)
        {
            InitializeComponent();
            MainScreen_ViewModel.MainGridReference = MainGrid;
            MainScreen_ViewModel.ScrollViewer_ContentPaneReference = ScrollViewer_ContentPane;
            MainScreen_ViewModel.grdChangeSummaryReference = grdChangeSummary;
            MainScreen_ViewModel.ActiveSetpointControlsReference = ActiveSetpointControls;
            MainScreen_ViewModel.RbSet1Reference = RbSet1;
            MainScreen_ViewModel.RbSet2Reference = RbSet2;
            MainScreen_ViewModel.RbSet3Reference = RbSet3;
            MainScreen_ViewModel.RbSet4Reference = RbSet4;
            MainScreen_ViewModel.LSChartReference = LSChart;
            MainScreen_ViewModel.GIChartReference = GIChart;
            MainScreen_ViewModel.INSTxInChartReference = INSTxInChart;
            MainScreen_ViewModel.MMxInChartReference = MMxInChart;
            MainScreen_ViewModel.LSIrChartReference = LSIrChart;
            MainScreen_ViewModel.grdSidebarReference = grdSidebar;
            var viewmodel = new MainScreen_ViewModel(source,this);
            this.DataContext = viewmodel;
            mainScreen_ViewModel = viewmodel;

            if (Global.device_type == Global.ACB_PXR35_DEVICE)
            {
                switch (Global.PXR35_SelectedSetpointSet)
                {
                    case "A":
                        RbSet1.Checked += RbSet1234_Checked;
                        RbSet1.IsChecked = true;
                        break;
                    case "B":
                        RbSet2.Checked += RbSet1234_Checked;
                        RbSet2.IsChecked = true;

                        break;
                    case "C":
                        RbSet3.Checked += RbSet1234_Checked;
                        RbSet3.IsChecked = true;

                        break;
                    case "D":
                        RbSet4.Checked += RbSet1234_Checked;
                        RbSet4.IsChecked = true;

                        break;
                }
            }
        }

        private void grdChangeSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (grdChangeSummary.SelectedItem == null)
                return;

            ChangeSummary objChangeSummary = (ChangeSummary)(grdChangeSummary.SelectedItem);
            if (objChangeSummary != null)
            {
                string controlName = objChangeSummary.ControlName;

                foreach (Expander expItem in Global.FindVisualChildren<Expander>(MainGrid))
                {
                    bool isItemFound = false;
                    if (controlName.Contains("txt"))
                    {
                        foreach (TextBox tb in Global.FindVisualChildren<TextBox>(expItem))
                        {
                            if (tb.Name == controlName)
                            {
                                expItem.IsExpanded = true;
                                tb.Focus();
                                isItemFound = true;
                                break;
                            }
                        }
                    }

                    else if (controlName.Contains("cmb"))
                    {
                        foreach (ComboBox cmb in Global.FindVisualChildren<ComboBox>(expItem))
                        {
                            if (cmb.Name == controlName)
                            {
                                expItem.IsExpanded = true;
                                cmb.Focus();
                                SendKeys.SendWait("{F4}");
                                isItemFound = true;
                                break;
                            }
                        }
                    }

                    else if (controlName.Contains("chk"))
                    {
                        foreach (CheckBox chk in Global.FindVisualChildren<CheckBox>(expItem))
                        {
                            if (chk.Name == controlName)
                            {
                                expItem.IsExpanded = true;
                                chk.Focus();
                                isItemFound = true;
                                break;
                            }
                        }
                    }

                    if (isItemFound) break;
                }
            }
        }
        private void MainScreenConfig_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                mainScreen_ViewModel.SaveToFile(null);
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer_ContentPane.ScrollToVerticalOffset(ScrollViewer_ContentPane.VerticalOffset - e.Delta / 6);
        }

        private void MainScreenConfig_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != e)
                e.Handled = true;

            var source = PresentationSource.FromVisual(this);

            ScrollViewer_ContentPane.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.60;
            grdChangeSummary.Width = (Screen.PrimaryScreen.WorkingArea.Width) * 0.40;


            if (null != source && null != source.CompositionTarget)
            {
                var matrix = source.CompositionTarget.TransformToDevice;

                var dpiTransform = new ScaleTransform(1 / matrix.M11, 1 / matrix.M22);

                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();

                LayoutTransform = dpiTransform;


            }
            var window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }
        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel == false)
            {
                //AFter closing mainscreen for new session need to reset this flag to false
                Global.export_Successful_ForSetpointReport = false;
                MainScreen_ViewModel.loadcurves = false;
                mainScreen_ViewModel.clearcurves();
            }

        }

        private void RbSet1234_Checked(object sender, RoutedEventArgs e)
        {
            mainScreen_ViewModel.RbSet1234_Checked(sender, e);
        }

        private void ViewActiveSetHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string setpointSetTOBeSlected = string.Empty;
                if (sender != null)
                {
                    string selectedSet = ((ComboBox)sender).Text.ToString();
                    string SelectedSetNameOnScreen = string.Empty;
                    if (!string.IsNullOrEmpty(selectedSet))
                    {
                        setpointSetTOBeSlected = ((ComboBox)sender).SelectedIndex == 0 ? "A" :
                               (((ComboBox)sender).SelectedIndex == 1 ? "B" : (
                               ((ComboBox)sender).SelectedIndex == 2 ? "C" : "D"));
                        if (((ComboBox)sender).SelectedIndex == 0)
                        {
                            setpointSetTOBeSlected = "A";
                            SelectedSetNameOnScreen = Resource.SYS025Item0000;
                        }
                        else if (((ComboBox)sender).SelectedIndex == 1)
                        {
                            setpointSetTOBeSlected = "B";
                            SelectedSetNameOnScreen = Resource.SYS025Item0001;
                        }
                        if (((ComboBox)sender).SelectedIndex == 2)
                        {
                            setpointSetTOBeSlected = "C";
                            SelectedSetNameOnScreen = Resource.SYS025Item0002;
                        }
                        else if ((((ComboBox)sender).SelectedIndex == 3))
                        {
                            setpointSetTOBeSlected = "D";
                            SelectedSetNameOnScreen = Resource.SYS025Item0003;
                        }
                    }
                    if (MainScreen_ViewModel.Changes.Count == 1 && (MainScreen_ViewModel.Changes.ElementAt(0).ItemName == Resource.SYS025Name))
                    {
                        if (setpointSetTOBeSlected != string.Empty)
                            Global.PXR35_SelectedSetpointSet = setpointSetTOBeSlected;
                        mainScreen_ViewModel.LoadSelectedSetOnScreen();
                    }
                    else if (MainScreen_ViewModel.Changes.Count > 0)
                    {
                        Mouse.OverrideCursor = null;
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
                            mainScreen_ViewModel.LoadSelectedSetOnScreen();
                        }
                    }
                    else
                    {
                        if (setpointSetTOBeSlected != string.Empty)
                            Global.PXR35_SelectedSetpointSet = setpointSetTOBeSlected;
                        mainScreen_ViewModel.LoadSelectedSetOnScreen();
                    }
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
    }
}
 


