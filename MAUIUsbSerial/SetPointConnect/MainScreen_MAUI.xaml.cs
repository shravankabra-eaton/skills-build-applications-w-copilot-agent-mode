using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MAUIUsbSerial.SetPointConnect
{
    public partial class MainScreen_MAUI : ContentPage
    {
        public ObservableCollection<ChangeSummaryItem> ChangeSummaryItems { get; set; } = new();

        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private readonly MainScreen_ViewModel _viewModel;

        public MainScreen_MAUI()
        {
            InitializeComponent();
            _viewModel = new MainScreen_ViewModel("MAUI", null); // Pass appropriate args as needed

            // Bind the CollectionView to the ViewModel's Changes collection if available
            if (MainScreen_ViewModel.Changes != null)
            {
                ChangeSummaryCollection.ItemsSource = MainScreen_ViewModel.Changes;
            }
            else
            {
                ChangeSummaryItems.Add(new ChangeSummaryItem { ItemName = "Setpoint 1", OrigValue = "10", NewValue = "12" });
                ChangeSummaryItems.Add(new ChangeSummaryItem { ItemName = "Setpoint 2", OrigValue = "20", NewValue = "22" });
                ChangeSummaryCollection.ItemsSource = ChangeSummaryItems;
            }

            SaveCommand = _viewModel.SaveCommand;
            SaveAsCommand = _viewModel.SaveToFileCommand;
            ExportCommand = _viewModel.ExportCommand;
            RefreshCommand = _viewModel.RefreshCommand;
            CancelCommand = _viewModel.CancelCommand;

            BindingContext = _viewModel;
        }

        private void OnSave()
        {
            Application.Current.MainPage.DisplayAlert("Save", "Save logic executed.", "OK");
        }
        private void OnSaveAs()
        {
            Application.Current.MainPage.DisplayAlert("Save As", "Save As logic executed.", "OK");
        }
        private void OnExport()
        {
            Application.Current.MainPage.DisplayAlert("Export", "Export logic executed.", "OK");
        }
        private void OnRefresh()
        {
            Application.Current.MainPage.DisplayAlert("Refresh", "Refresh logic executed.", "OK");
        }
        private void OnCancel()
        {
            Application.Current.MainPage.DisplayAlert("Cancel", "Cancel logic executed.", "OK");
        }
    }

    public class ChangeSummaryItem
    {
        public string ItemName { get; set; }
        public string OrigValue { get; set; }
        public string NewValue { get; set; }
    }
}
