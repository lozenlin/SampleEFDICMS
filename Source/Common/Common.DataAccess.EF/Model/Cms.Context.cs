﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CmsContext : DbContext
    {
        public CmsContext()
            : base("name=CmsContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<ArticleMultiLang> ArticleMultiLang { get; set; }
        public virtual DbSet<ArticlePicture> ArticlePicture { get; set; }
        public virtual DbSet<ArticlePictureMultiLang> ArticlePictureMultiLang { get; set; }
        public virtual DbSet<ArticleVideo> ArticleVideo { get; set; }
        public virtual DbSet<ArticleVideoMultiLang> ArticleVideoMultiLang { get; set; }
        public virtual DbSet<AttachFile> AttachFile { get; set; }
        public virtual DbSet<AttachFileMultiLang> AttachFileMultiLang { get; set; }
        public virtual DbSet<BackEndLog> BackEndLog { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeRole> EmployeeRole { get; set; }
        public virtual DbSet<EmployeeRoleOperationsDesc> EmployeeRoleOperationsDesc { get; set; }
        public virtual DbSet<Keyword> Keyword { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<SearchDataSource> SearchDataSource { get; set; }
    
        public virtual int spSearchDataSource_Build(string mainLinkUrl)
        {
            var mainLinkUrlParameter = mainLinkUrl != null ?
                new ObjectParameter("MainLinkUrl", mainLinkUrl) :
                new ObjectParameter("MainLinkUrl", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spSearchDataSource_Build", mainLinkUrlParameter);
        }
    }
}
