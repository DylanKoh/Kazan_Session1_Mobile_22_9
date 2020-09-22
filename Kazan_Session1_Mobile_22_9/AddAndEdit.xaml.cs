using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Kazan_Session1_Mobile_22_9.GlobalClass;

namespace Kazan_Session1_Mobile_22_9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAndEdit : ContentPage
    {
        int _assetID = 0;
        List<Department> _departmentList;
        List<DepartmentLocation> _departmentLocationList;
        List<Location> _locationList;
        List<AssetGroup> _assetGroupList;
        List<Employee> _accountableList;
        Asset _asset;
        public AddAndEdit(int AssetID)
        {
            InitializeComponent();
            _assetID = AssetID;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await LoadPickers();
            if (_assetID != 0)
            {
                await LoadData();
                pAssetGroup.IsEnabled = false;
                pDepartment.IsEnabled = false;
                pLocation.IsEnabled = false;
                entryAssetName.IsEnabled = false;
            }
        }

        private async Task LoadData()
        {
            var client = new WebApi();
            var assetResponse = await client.PostAsync(null, $"Assets/Details/{_assetID}");
            _asset = JsonConvert.DeserializeObject<Asset>(assetResponse);
            entryAssetName.Text = _asset.AssetName;
            var getAssetGroup = (from x in _assetGroupList
                                 where x.ID == _asset.AssetGroupID
                                 select x.Name).FirstOrDefault();
            pAssetGroup.SelectedItem = getAssetGroup;
            var getDepartment = (from x in _departmentLocationList
                                 join y in _departmentList on x.DepartmentID equals y.ID
                                 where x.ID == _asset.DepartmentLocationID
                                 select y.Name).FirstOrDefault();
            pDepartment.SelectedItem = getDepartment;
            var getAccountable = (from x in _accountableList
                                  where x.ID == _asset.EmployeeID
                                  select x.FirstName + " " + x.LastName).FirstOrDefault();
            pAccountable.SelectedItem = getAccountable;
            editorAssetDescription.Text = _asset.Description;
            lblAssetSN.Text = _asset.AssetSN;
            var getLocation = (from x in _departmentLocationList
                               join y in _locationList on x.LocationID equals y.ID
                               where x.ID == _asset.DepartmentLocationID
                               select y.Name).FirstOrDefault();
            pLocation.SelectedItem = getLocation;
        }

        private async Task LoadPickers()
        {
            pAssetGroup.Items.Clear();
            pDepartment.Items.Clear();
            pAccountable.Items.Clear();
            pLocation.Items.Clear();
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

            var employeeResponse = await client.PostAsync(null, "Employees");
            _accountableList = JsonConvert.DeserializeObject<List<Employee>>(employeeResponse);
            foreach (var item in _accountableList)
            {
                pAccountable.Items.Add(item.FirstName + " " + item.LastName);
            }

            var departmentLocationResponse = await client.PostAsync(null, "DepartmentLocations");
            _departmentLocationList = JsonConvert.DeserializeObject<List<DepartmentLocation>>(departmentLocationResponse);

            var locationResponse = await client.PostAsync(null, "Locations");
            _locationList = JsonConvert.DeserializeObject<List<Location>>(locationResponse);
        }

        private void pDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            pLocation.Items.Clear();
            pLocation.IsEnabled = true;
            if (pDepartment.SelectedItem != null)
            {
                var getLocation = (from x in _departmentLocationList
                                   where x.EndDate == null
                                   join y in _departmentList on x.DepartmentID equals y.ID
                                   join z in _locationList on x.LocationID equals z.ID
                                   where y.Name == pDepartment.SelectedItem.ToString()
                                   select z).ToList();
                foreach (var item in getLocation)
                {
                    pLocation.Items.Add(item.Name);
                }
            }
            if (pDepartment.SelectedItem != null && pAssetGroup.SelectedItem != null && _assetID == 0)
            {
                CalcuateAssetSN();
            }
        }

        private async void CalcuateAssetSN()
        {
            var getAssetGroupID = (from x in _assetGroupList
                                   where x.Name == pAssetGroup.SelectedItem.ToString()
                                   select x.ID).FirstOrDefault();

            var getDepartmentID = (from x in _departmentList
                                   where x.Name == pDepartment.SelectedItem.ToString()
                                   select x.ID).FirstOrDefault();
            var ddgg = $"{getDepartmentID.ToString().PadLeft(2, '0')}/{getAssetGroupID.ToString().PadLeft(2, '0')}";
            var client = new WebApi();
            var response = JsonConvert.DeserializeObject<List<string>>(await client.PostAsync(null, "Assets/GetAllSN"));
            var getRelated = (from x in response
                              where x.Contains(ddgg)
                              orderby x descending
                              select x).FirstOrDefault();
            if (getRelated == null)
            {
                lblAssetSN.Text = $"{ddgg}/0001";
            }
            else
            {
                var newNNNN = int.Parse(getRelated.Split('/')[2]) + 1;
                lblAssetSN.Text = $"{ddgg}/{newNNNN.ToString().PadLeft(4, '0')}";
            }
        }

        private void pAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pDepartment.SelectedItem != null && pAssetGroup.SelectedItem != null && _assetID == 0)
            {
                CalcuateAssetSN();
            }
        }

        private async void btnSubmit_Clicked(object sender, EventArgs e)
        {
            var client = new WebApi();
            var getAccountable = (from x in _accountableList
                                  where pAccountable.SelectedItem.ToString().Contains(x.FirstName + " " + x.LastName)
                                  select x.ID).FirstOrDefault();
            var getAssetGroupID = (from x in _assetGroupList
                                   where x.Name == pAssetGroup.SelectedItem.ToString()
                                   select x.ID).FirstOrDefault();

            var getDepartmentID = (from x in _departmentList
                                   where x.Name == pDepartment.SelectedItem.ToString()
                                   select x.ID).FirstOrDefault();

            var getLocationID = (from x in _locationList
                                 where x.Name == pLocation.SelectedItem.ToString()
                                 select x.ID).FirstOrDefault();
            var getDepartmentLocationID = (from x in _departmentLocationList
                                           where x.LocationID == getLocationID && x.DepartmentID == getDepartmentID
                                           where x.EndDate == null
                                           select x.ID).FirstOrDefault();
            if (_assetID != 0)
            {
                if (string.IsNullOrWhiteSpace(editorAssetDescription.Text))
                {
                    _asset.Description = " ";
                    _asset.EmployeeID = getAccountable;
                    _asset.WarrantyDate = dpWarranty.Date;
                    
                }
                else
                {
                    _asset.Description = editorAssetDescription.Text;
                    _asset.EmployeeID = getAccountable;
                    _asset.WarrantyDate = dpWarranty.Date;
                }
                var response = await client.PostAsync(JsonConvert.SerializeObject(_asset), "Assets/Edit");
                if (response != "\"Successfully edited Asset!\"")
                {
                    await DisplayAlert("Submit", "An error occured while editing asset!", "Ok");
                }
                else
                {
                    await DisplayAlert("Submit", "Successfully edited Asset!", "Ok");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                var newAsset = new Asset()
                {
                    AssetGroupID = getAssetGroupID,
                    AssetName = entryAssetName.Text,
                    AssetSN = lblAssetSN.Text,
                    DepartmentLocationID = getDepartmentLocationID,
                    EmployeeID = getAccountable,
                    WarrantyDate = dpWarranty.Date
                };
                if (string.IsNullOrWhiteSpace(editorAssetDescription.Text))
                {

                    newAsset.Description = " ";
                }
                else
                {
                    newAsset.Description = editorAssetDescription.Text;
                }
                var response = await client.PostAsync(JsonConvert.SerializeObject(newAsset), "Assets/Create");
                if (response != "\"Successfully created Asset!\"")
                {
                    if (response == "\"Asset already exist in location!\"")
                    {
                        await DisplayAlert("Submit", "Please change your Asset Name and or location!", "Ok");
                    }
                    else
                    {
                        await DisplayAlert("Submit", "An error occured while creating asset!", "Ok");
                    }
                    
                }
                else
                {
                    await DisplayAlert("Submit", "Successfully created Asset!", "Ok");
                    await Navigation.PopAsync();
                }
            }
        }

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}