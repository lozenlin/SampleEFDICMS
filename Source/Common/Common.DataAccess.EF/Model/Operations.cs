//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Common.DataAccess.EF.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Operations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Operations()
        {
            this.EmployeeRoleOperationsDesc = new HashSet<EmployeeRoleOperationsDesc>();
        }
    
        public int OpId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string OpSubject { get; set; }
        public string LinkUrl { get; set; }
        public bool IsNewWindow { get; set; }
        public string IconImageFile { get; set; }
        public Nullable<int> SortNo { get; set; }
        public bool IsHideSelf { get; set; }
        public string CommonClass { get; set; }
        public string PostAccount { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string MdfAccount { get; set; }
        public Nullable<System.DateTime> MdfDate { get; set; }
        public string EnglishSubject { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeRoleOperationsDesc> EmployeeRoleOperationsDesc { get; set; }
    }
}
