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
    
    public partial class ArticleVideo
    {
        public System.Guid VidId { get; set; }
        public int SeqnoForCluster { get; set; }
        public Nullable<System.Guid> ArticleId { get; set; }
        public Nullable<int> SortNo { get; set; }
        public string VidLinkUrl { get; set; }
        public string SourceVideoId { get; set; }
        public string PostAccount { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string MdfAccount { get; set; }
        public Nullable<System.DateTime> MdfDate { get; set; }
    }
}
