//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Common.Data.Domain.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeRoleOperationsDesc
    {
        public string RoleName { get; set; }
        public int OpId { get; set; }
        public bool CanRead { get; set; }
        public bool CanEdit { get; set; }
        public bool CanReadSubItemOfSelf { get; set; }
        public bool CanEditSubItemOfSelf { get; set; }
        public bool CanAddSubItemOfSelf { get; set; }
        public bool CanDelSubItemOfSelf { get; set; }
        public bool CanReadSubItemOfCrew { get; set; }
        public bool CanEditSubItemOfCrew { get; set; }
        public bool CanDelSubItemOfCrew { get; set; }
        public bool CanReadSubItemOfOthers { get; set; }
        public bool CanEditSubItemOfOthers { get; set; }
        public bool CanDelSubItemOfOthers { get; set; }
        public string PostAccount { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string MdfAccount { get; set; }
        public Nullable<System.DateTime> MdfDate { get; set; }
    
        public virtual Operations Operations { get; set; }
    }
}
