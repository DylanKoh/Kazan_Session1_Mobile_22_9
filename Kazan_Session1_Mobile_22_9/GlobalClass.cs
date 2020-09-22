using System;
using System.Collections.Generic;
using System.Text;

namespace Kazan_Session1_Mobile_22_9
{
    public class GlobalClass
    {
        public class CustomView
        {
            public int AssetID { get; set; }
            public string AssetName { get; set; }
            public string AssetGroup { get; set; }
            public string DepartmentName { get; set; }
            public string AssetSN { get; set; }
            public DateTime? WarrantyDate { get; set; }
        }

        public partial class AssetGroup
        {
            public long ID { get; set; }
            public string Name { get; set; }

        }

        public class Department
        {

            public long ID { get; set; }
            public string Name { get; set; }
        }

        public class DepartmentLocation
        {
            public long ID { get; set; }
            public long DepartmentID { get; set; }
            public long LocationID { get; set; }
            public System.DateTime StartDate { get; set; }
            public Nullable<System.DateTime> EndDate { get; set; }
        }

        public class Location
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        public class Employee
        {
            public long ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
        }

        public class AssetTransferLog
        {
            public long ID { get; set; }
            public long AssetID { get; set; }
            public System.DateTime TransferDate { get; set; }
            public string FromAssetSN { get; set; }
            public string ToAssetSN { get; set; }
            public long FromDepartmentLocationID { get; set; }
            public long ToDepartmentLocationID { get; set; }
        }

        public class Asset
        {
            public long ID { get; set; }
            public string AssetSN { get; set; }
            public string AssetName { get; set; }
            public long DepartmentLocationID { get; set; }
            public long EmployeeID { get; set; }
            public long AssetGroupID { get; set; }
            public string Description { get; set; }
            public Nullable<System.DateTime> WarrantyDate { get; set; }
        }

    }
}
