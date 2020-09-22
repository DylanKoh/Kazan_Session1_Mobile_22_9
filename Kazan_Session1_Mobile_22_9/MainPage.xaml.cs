using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Kazan_Session1_Mobile_22_9.GlobalClass;

namespace Kazan_Session1_Mobile_22_9
{
    public partial class MainPage : ContentPage
    {
        List<Department> _departmentList;
        List<AssetGroup> _assetGroupList;
        List<CustomView> _originalList;
        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await LoadPickers();
            await LoadData();
            dpStart.Date = DateTime.Parse("1 Jan 2000");
            dpEnd.Date = DateTime.Parse("31 Dec 2100");
        }

        private async Task LoadData()
        {
            lvAsset.ItemsSource = null;
            var client = new WebApi();
            var customListResponse = await client.PostAsync(null, "Assets/GetCustomView");
            _originalList = JsonConvert.DeserializeObject<List<CustomView>>(customListResponse);
            lvAsset.ItemsSource = _originalList;
        }

        private async Task LoadPickers()
        {
            pAssetGroup.Items.Clear();
            pDepartment.Items.Clear();
            pAssetGroup.Items.Add("No Filter");
            pDepartment.Items.Add("No Filter");
            var client = new WebApi();
            var assetGroupResponse = await client.PostAsync(null, "AssetGroups");
            _assetGroupList = JsonConvert.DeserializeObject<List<AssetGroup>>(assetGroupResponse);
            foreach (var item in _assetGroupList)
            {
                pAssetGroup.Items.Add(item.Name);
            }

            var departmentResponse = await client.PostAsync(null, "Departments");
            _departmentList = JsonConvert.DeserializeObject<List<Department>>(departmentResponse);
            foreach (var item in _departmentList)
            {
                pDepartment.Items.Add(item.Name);
            }
        }

        private void pDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterResults();
        }

        private void pAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterResults();
        }

        private void dpStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            Task.WaitAny(LoadData());
            FilterResults();
        }

        private void dpEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            Task.WaitAny(LoadData());
            FilterResults();
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterResults();
        }

        private void FilterResults()
        {
            lblAssetCount.Text = " assets from ";
            lvAsset.ItemsSource = null;
            if ((pDepartment.SelectedItem == null || pDepartment.SelectedItem.ToString() == "No Filter") && (pAssetGroup.SelectedItem == null || pAssetGroup.SelectedItem.ToString() == "No Filter"))
            {
                if (string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
                else
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetName.ToLower().Contains(searchBar.Text.ToLower()) || x.AssetSN.ToLower().Contains(searchBar.Text.ToLower())
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
            }
            else if (pDepartment.SelectedItem.ToString() != "No Filter" && (pAssetGroup.SelectedItem == null || pAssetGroup.SelectedItem.ToString() == "No Filter"))
            {
                if (string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.DepartmentName == pDepartment.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
                else
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetName.ToLower().Contains(searchBar.Text.ToLower()) || x.AssetSN.ToLower().Contains(searchBar.Text.ToLower())
                                    where x.DepartmentName == pDepartment.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
            }
            else if ((pDepartment.SelectedItem == null || pDepartment.SelectedItem.ToString() == "No Filter") && pAssetGroup.SelectedItem.ToString() != "No Filter")
            {
                if (string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
                else
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetName.ToLower().Contains(searchBar.Text.ToLower()) || x.AssetSN.ToLower().Contains(searchBar.Text.ToLower())
                                    where x.AssetGroup == pAssetGroup.SelectedItem.ToString() && x.DepartmentName == pDepartment.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchBar.Text))
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetGroup == pAssetGroup.SelectedItem.ToString() && x.DepartmentName == pDepartment.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
                else
                {
                    var filtered = (from x in _originalList
                                    where x.WarrantyDate <= dpEnd.Date && x.WarrantyDate >= dpStart.Date
                                    where x.AssetName.ToLower().Contains(searchBar.Text.ToLower()) || x.AssetSN.ToLower().Contains(searchBar.Text.ToLower())
                                    where x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                    select x).ToList();
                    lvAsset.ItemsSource = filtered;
                    lblAssetCount.Text = $"{filtered.Count} assets from {_originalList.Count}";
                }
            }
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            var editBtn = (Button)sender;
            var stackParent = (StackLayout)editBtn.Parent;
            var childToTake = (StackLayout)stackParent.Children[0];
            var lblAssetID = (Label)childToTake.Children[0];
            await Navigation.PushAsync(new AddAndEdit(int.Parse(lblAssetID.Text)));
        }

        private void btnMove_Clicked(object sender, EventArgs e)
        {
            var editBtn = (Button)sender;
            var stackParent = (StackLayout)editBtn.Parent;
            var childToTake = (StackLayout)stackParent.Children[0];
            var lblAssetID = (Label)childToTake.Children[0];
        }

        private void btnHistory_Clicked(object sender, EventArgs e)
        {
            var editBtn = (Button)sender;
            var stackParent = (StackLayout)editBtn.Parent;
            var childToTake = (StackLayout)stackParent.Children[0];
            var lblAssetID = (Label)childToTake.Children[0];
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAndEdit(0));
        }
    }
}
