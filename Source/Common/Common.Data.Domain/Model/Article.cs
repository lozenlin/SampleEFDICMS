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
    
    public partial class Article
    {
        public System.Guid ArticleId { get; set; }
        public int SeqnoForCluster { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public Nullable<int> ArticleLevelNo { get; set; }
        public string ArticleAlias { get; set; }
        public string BannerPicFileName { get; set; }
        public Nullable<int> LayoutModeId { get; set; }
        public Nullable<int> ShowTypeId { get; set; }
        public string LinkUrl { get; set; }
        public string LinkTarget { get; set; }
        public string ControlName { get; set; }
        public string SubItemControlName { get; set; }
        public bool IsHideSelf { get; set; }
        public bool IsHideChild { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> SortNo { get; set; }
        public bool DontDelete { get; set; }
        public string PostAccount { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public string MdfAccount { get; set; }
        public Nullable<System.DateTime> MdfDate { get; set; }
        public bool SubjectAtBannerArea { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public bool IsShowInUnitArea { get; set; }
        public bool IsShowInSitemap { get; set; }
        public string SortFieldOfFrontStage { get; set; }
        public bool IsSortDescOfFrontStage { get; set; }
        public bool IsListAreaShowInFrontStage { get; set; }
        public bool IsAttAreaShowInFrontStage { get; set; }
        public bool IsPicAreaShowInFrontStage { get; set; }
        public bool IsVideoAreaShowInFrontStage { get; set; }
        public string SubItemLinkUrl { get; set; }
    }
}