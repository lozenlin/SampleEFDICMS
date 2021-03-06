-- Article Publisher tables
-- use SampleEFDICMS
go

----------------------------------------------------------------------------
-- dbo.Article 網頁內容	
----------------------------------------------------------------------------
create table dbo.Article(
	ArticleId	uniqueidentifier	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,ParentId	uniqueidentifier		
	,ArticleLevelNo	int		
	,ArticleAlias	varchar(50)		
	,BannerPicFileName	nvarchar(255)		
	,LayoutModeId	int
	,ShowTypeId	int
	,LinkUrl	nvarchar(2048)		
	,LinkTarget	varchar(10)		
	,ControlName	varchar(100)		
	,SubItemControlName	varchar(100)		
	,IsHideSelf	bit	Not Null	Default(0)
	,IsHideChild	bit	Not Null	Default(0)
	,StartDate	datetime		
	,EndDate	datetime		
	,SortNo	int		
	,DontDelete	bit	Not Null	Default(0)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_Article primary key nonclustered (ArticleId)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_Article on dbo.Article (SeqnoForCluster)
go
--	2017/12/19, lozen_lin, 增加額外設定用的欄位
alter table dbo.Article add SubjectAtBannerArea	bit	Not Null	Default(1)
alter table dbo.Article add PublishDate	datetime		
alter table dbo.Article add IsShowInUnitArea	bit	Not Null	Default(0)
alter table dbo.Article add IsShowInSitemap	bit	Not Null	Default(1)
alter table dbo.Article add SortFieldOfFrontStage	varchar(50)		
alter table dbo.Article add IsSortDescOfFrontStage	bit	Not Null	Default(0)
alter table dbo.Article add IsListAreaShowInFrontStage	bit	Not Null	Default(1)
alter table dbo.Article add IsAttAreaShowInFrontStage	bit	Not Null	Default(0)
alter table dbo.Article add IsPicAreaShowInFrontStage	bit	Not Null	Default(0)
alter table dbo.Article add IsVideoAreaShowInFrontStage	bit	Not Null	Default(0)
alter table dbo.Article add SubItemLinkUrl	nvarchar(2048)		
go

CREATE NONCLUSTERED INDEX [IX_Article_GetList] ON [dbo].[Article]
(
	[ArticleId] ASC,
	[ParentId] ASC,
	[IsHideSelf] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)
INCLUDE ( 	[ArticleLevelNo],
	[ArticleAlias],
	[BannerPicFileName],
	[LayoutModeId],
	[ShowTypeId],
	[LinkUrl],
	[LinkTarget],
	[ControlName],
	[IsHideChild],
	[SortNo],
	[PostAccount],
	[PostDate],
	[MdfAccount],
	[MdfDate],
	[SubjectAtBannerArea],
	[PublishDate],
	[IsShowInUnitArea],
	[IsShowInSitemap],
	[SortFieldOfFrontStage],
	[IsSortDescOfFrontStage],
	[IsListAreaShowInFrontStage],
	[IsAttAreaShowInFrontStage],
	[IsPicAreaShowInFrontStage],
	[IsVideoAreaShowInFrontStage]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [IX_Article_ShowTypeId_LinkUrl] ON [dbo].[Article]
(
	[ShowTypeId] ASC
)
INCLUDE ( 	[LinkUrl],
	[ArticleId],
	[PostDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [IX_Article_ForSideSection] ON [dbo].[Article]
(
	[ParentId] ASC,
	[IsHideSelf] ASC,
	[StartDate] ASC,
	[EndDate] ASC,
	[IsShowInUnitArea] ASC,
	[IsShowInSitemap] ASC
)
INCLUDE ( 	[ArticleId],
	[ArticleAlias],
	[ShowTypeId],
	[LinkUrl],
	[LinkTarget],
	[IsHideChild],
	[ArticleLevelNo],
	[SortNo]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go


-- 預設內容
declare @today datetime = convert(varchar(10), getdate(), 111)

set identity_insert dbo.Article on

insert into dbo.Article(
	ArticleId, SeqnoForCluster, ArticleLevelNo, 
	ArticleAlias, LayoutModeId, ShowTypeId, 
	LinkUrl, StartDate, EndDate, 
	SortNo, PostAccount, PostDate, 
	PublishDate
	)
values(
	'00000000-0000-0000-0000-000000000000', 1, 0, 
	'root', 1, 3, 
	'http://SampleEFCMS.dev.com/', @today, dateadd(year,100,@today),
	0, 'admin', getdate(), 
	@today
	)

INSERT dbo.Article (ArticleId, SeqnoForCluster, ParentId, ArticleLevelNo, ArticleAlias, BannerPicFileName, LayoutModeId, ShowTypeId, LinkUrl, LinkTarget, ControlName, SubItemControlName, IsHideSelf, IsHideChild, StartDate, EndDate, SortNo, DontDelete, PostAccount, PostDate, MdfAccount, MdfDate, SubjectAtBannerArea, PublishDate, IsShowInUnitArea, IsShowInSitemap, SortFieldOfFrontStage, IsSortDescOfFrontStage, IsListAreaShowInFrontStage, IsAttAreaShowInFrontStage, IsPicAreaShowInFrontStage, IsVideoAreaShowInFrontStage, SubItemLinkUrl)
 VALUES (N'031C4F54-6BFC-4EE1-96AF-95044CF80DC5', 2, N'00000000-0000-0000-0000-000000000000', 1, '031c4f54-6bfc-4ee1-96af-95044cf80dc5', N'', 1, 3, N'~/Sitemap.aspx', '', '', '', 0, 0, @today, dateadd(year,100,@today), 10, 1, 'admin', getdate(), null, null, 1, @today, 1, 0, '', 0, 1, 0, 0, 0, N'')
INSERT dbo.Article (ArticleId, SeqnoForCluster, ParentId, ArticleLevelNo, ArticleAlias, BannerPicFileName, LayoutModeId, ShowTypeId, LinkUrl, LinkTarget, ControlName, SubItemControlName, IsHideSelf, IsHideChild, StartDate, EndDate, SortNo, DontDelete, PostAccount, PostDate, MdfAccount, MdfDate, SubjectAtBannerArea, PublishDate, IsShowInUnitArea, IsShowInSitemap, SortFieldOfFrontStage, IsSortDescOfFrontStage, IsListAreaShowInFrontStage, IsAttAreaShowInFrontStage, IsPicAreaShowInFrontStage, IsVideoAreaShowInFrontStage, SubItemLinkUrl)
 VALUES (N'5CD4677A-080D-4587-936B-2F3D6D924746', 3, N'00000000-0000-0000-0000-000000000000', 1, '5cd4677a-080d-4587-936b-2f3d6d924746', N'', 1, 3, N'~/Search-Result.aspx', '', '', '', 0, 0, @today, dateadd(year,100,@today), 20, 1, 'admin', getdate(), null, null, 1, @today, 0, 0, '', 0, 1, 0, 0, 0, N'')

set identity_insert dbo.Article off
go

----------------------------------------------------------------------------
-- dbo.ArticleMultiLang 網頁內容的多國語系資料	
----------------------------------------------------------------------------
create table dbo.ArticleMultiLang(
	ArticleId	uniqueidentifier	Not Null
	,CultureName	varchar(10)	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,ArticleSubject	nvarchar(200)		
	,ArticleContext	nvarchar(max)		
	,ReadCount	int	Not Null	Default(0)
	,IsShowInLang	bit	Not Null	Default(1)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_ArticleMultiLang primary key nonclustered (ArticleId, CultureName)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_ArticleMultiLang on dbo.ArticleMultiLang (SeqnoForCluster)
go
--	2017/12/19, lozen_lin, 增加額外設定用的欄位
alter table dbo.ArticleMultiLang add Subtitle	nvarchar(500)
alter table dbo.ArticleMultiLang add PublisherName	nvarchar(50)
--	2017/12/26, lozen_lin, 增加欄位「純文字的網頁內容」
alter table dbo.ArticleMultiLang add TextContext	nvarchar(max)
go

CREATE NONCLUSTERED INDEX [IX_ArticleMultiLang_ForSitemap] ON [dbo].[ArticleMultiLang]
(
	[ArticleId] ASC,
	[CultureName] ASC
)
INCLUDE ( 	[ArticleSubject],
	[IsShowInLang]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go


-- 預設內容
set identity_insert dbo.ArticleMultiLang on

insert into dbo.ArticleMultiLang(
	ArticleId, CultureName, SeqnoForCluster, 
	ArticleSubject, IsShowInLang, PostAccount,
	PostDate, PublisherName
	)
values(
	'00000000-0000-0000-0000-000000000000', 'zh-TW', 1, 
	N'網站架構管理', 1, 'admin',
	getdate(), 'admin'
	)

insert into dbo.ArticleMultiLang(
	ArticleId, CultureName, SeqnoForCluster, 
	ArticleSubject, IsShowInLang, PostAccount,
	PostDate, PublisherName
	)
values(
	'00000000-0000-0000-0000-000000000000', 'en', 2, 
	N'Site Architecture Mgmt.', 1, 'admin',
	getdate(), 'admin'
	)

INSERT dbo.ArticleMultiLang (ArticleId, CultureName, SeqnoForCluster, ArticleSubject, ArticleContext, ReadCount, IsShowInLang, PostAccount, PostDate, MdfAccount, MdfDate, Subtitle, PublisherName, TextContext)
 VALUES (N'031C4F54-6BFC-4EE1-96AF-95044CF80DC5', 'zh-TW', 3, N'網站導覽', N'', 0, 1, 'admin', getdate(), null, null, N'', N'admin', N'')
INSERT dbo.ArticleMultiLang (ArticleId, CultureName, SeqnoForCluster, ArticleSubject, ArticleContext, ReadCount, IsShowInLang, PostAccount, PostDate, MdfAccount, MdfDate, Subtitle, PublisherName, TextContext)
 VALUES (N'031C4F54-6BFC-4EE1-96AF-95044CF80DC5', 'en', 4, N'Sitemap', N'', 0, 1, 'admin', getdate(), null, null, N'', N'admin', N'')
INSERT dbo.ArticleMultiLang (ArticleId, CultureName, SeqnoForCluster, ArticleSubject, ArticleContext, ReadCount, IsShowInLang, PostAccount, PostDate, MdfAccount, MdfDate, Subtitle, PublisherName, TextContext)
 VALUES (N'5CD4677A-080D-4587-936B-2F3D6D924746', 'zh-TW', 5, N'搜尋結果', N'', 0, 1, 'admin', getdate(), null, null, N'', N'admin', N'')
INSERT dbo.ArticleMultiLang (ArticleId, CultureName, SeqnoForCluster, ArticleSubject, ArticleContext, ReadCount, IsShowInLang, PostAccount, PostDate, MdfAccount, MdfDate, Subtitle, PublisherName, TextContext)
 VALUES (N'5CD4677A-080D-4587-936B-2F3D6D924746', 'en', 6, N'Search Result', N'', 0, 1, 'admin', getdate(), null, null, N'', N'admin', N'')

set identity_insert dbo.ArticleMultiLang off
go

----------------------------------------------------------------------------
-- dbo.AttachFile 附件檔案	
----------------------------------------------------------------------------
create table dbo.AttachFile(
	AttId	uniqueidentifier	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,ArticleId	uniqueidentifier
	,FilePath	nvarchar(150)
	,FileSavedName	nvarchar(500)		
	,FileSize	int	Not Null	Default(0)
	,SortNo	int		
	,FileMIME	varchar(255)		
	,DontDelete	bit	Not Null	Default(0)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_AttachFile primary key nonclustered(AttId)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_AttachFile on dbo.AttachFile (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_AttachFile_ArticleId] ON [dbo].[AttachFile]
(
	[ArticleId] ASC
)
INCLUDE ( 	[AttId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.AttachFileMultiLang 附件檔案的多國語系資料	
----------------------------------------------------------------------------
create table dbo.AttachFileMultiLang(
	AttId	uniqueidentifier	Not Null
	,CultureName	varchar(10)	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,AttSubject	nvarchar(200)		
	,ReadCount	int	Not Null	Default(0)
	,IsShowInLang	bit	Not Null	Default(1)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_AttachFileMultiLang primary key nonclustered(AttId, CultureName)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_AttachFileMultiLang on dbo.AttachFileMultiLang (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_AttachFileMultiLang_GetList] ON [dbo].[AttachFileMultiLang]
(
	[AttId] ASC, 
	[CultureName] ASC,
	[IsShowInLang] ASC
)
INCLUDE ([AttSubject],
	[ReadCount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.ArticlePicture 網頁照片	
----------------------------------------------------------------------------
create table dbo.ArticlePicture(
	PicId	uniqueidentifier	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,ArticleId	uniqueidentifier
	,FileSavedName	nvarchar(500)		
	,FileSize	int	Not Null	Default(0)
	,SortNo	int		
	,FileMIME	varchar(255)		
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_ArticlePicture primary key nonclustered(PicId)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_ArticlePicture on dbo.ArticlePicture (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_ArticlePicture_ArticleId] ON [dbo].[ArticlePicture]
(
	[ArticleId] ASC
)
INCLUDE ( 	[PicId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.ArticlePictureMultiLang 網頁照片的多國語系資料	
----------------------------------------------------------------------------
create table dbo.ArticlePictureMultiLang(
	PicId	uniqueidentifier	Not Null	
	,CultureName	varchar(10)	Not Null	
	,SeqnoForCluster	int	Not Null	identity
	,PicSubject	nvarchar(200)		
	,IsShowInLang	bit	Not Null	Default(1)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_ArticlePictureMultiLang primary key nonclustered(PicId, CultureName)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_ArticlePictureMultiLang on dbo.ArticlePictureMultiLang (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_ArticlePictureMultiLang_GetList] ON [dbo].[ArticlePictureMultiLang]
(
	[PicId] ASC, 
	[CultureName] ASC,
	[IsShowInLang] ASC
)
INCLUDE ([PicSubject]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.ArticleVideo 網頁影片	
----------------------------------------------------------------------------
create table dbo.ArticleVideo(
	VidId	uniqueidentifier	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,ArticleId	uniqueidentifier
	,SortNo	int		
	,VidLinkUrl	nvarchar(2048)		
	,SourceVideoId	varchar(100)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime	
	,constraint PK_ArticleVideo primary key nonclustered (VidId)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_ArticleVideo on dbo.ArticleVideo (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_ArticleVideo_ArticleId] ON [dbo].[ArticleVideo]
(
	[ArticleId] ASC
)
INCLUDE ( 	[VidId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.ArticleVideoMultiLang 網頁影片的多國語系資料	
----------------------------------------------------------------------------
create table dbo.ArticleVideoMultiLang(
	VidId	uniqueidentifier	Not Null
	,CultureName	varchar(10)	Not Null
	,SeqnoForCluster	int	Not Null	identity
	,VidSubject	nvarchar(200)		
	,VidDesc	nvarchar(500)		
	,IsShowInLang	bit	Not Null	Default(1)
	,PostAccount	varchar(20)		
	,PostDate	datetime		
	,MdfAccount	varchar(20)		
	,MdfDate	datetime		
	,constraint PK_ArticleVideoMultiLang primary key nonclustered (VidId, CultureName)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_ArticleVideoMultiLang on dbo.ArticleVideoMultiLang (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_ArticleVideoMultiLang_GetList] ON [dbo].[ArticleVideoMultiLang]
(
	[VidId] ASC,
	[CultureName] ASC,
	[IsShowInLang] ASC
)
INCLUDE ( 	[VidSubject],
	[VidDesc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.SearchDataSource 搜尋用資料來源	
----------------------------------------------------------------------------
create table dbo.SearchDataSource(
	ArticleId	uniqueidentifier	Not Null	
	,SubId	uniqueidentifier	Not Null	default('00000000-0000-0000-0000-000000000000')
	,CultureName	varchar(10)	Not Null	
	,SeqnoForCluster	int	Not Null	identity
	,ArticleSubject	nvarchar(200)		
	,ArticleContext	nvarchar(max)		
	,ReadCount	int	Not Null	Default(0)
	,LinkUrl	nvarchar(2048)		
	,PublishDate	datetime
	,BreadcrumbData	nvarchar(4000)		
	,Lv1ArticleId	uniqueidentifier		
	,PostDate	datetime		
	,MdfDate	datetime		
	,constraint PK_SearchDataSource primary key nonclustered (ArticleId, SubId, CultureName)
)
go
-- 為避免 GUID 造成的索引破碎帶來的效能影響，叢集索引使用自動編號並且與主鍵分開
create clustered index IX_SearchDataSource on dbo.SearchDataSource (SeqnoForCluster)
go

CREATE NONCLUSTERED INDEX [IX_SearchDataSource_ForInnerSearch] ON [dbo].[SearchDataSource]
(
	[ArticleId] ASC,
	[SubId] ASC,
	[CultureName] ASC
)
INCLUDE ( 	[ArticleSubject],
	[ArticleContext]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go

----------------------------------------------------------------------------
-- dbo.Keyword 搜尋關鍵字	
----------------------------------------------------------------------------
create table dbo.Keyword(
	Seqno	int	Not Null	identity primary key
	,CultureName	varchar(10)	Not Null	
	,Kw	nvarchar(100)	Not Null	
	,UsedCount	int	Not Null	Default(1)
)
go
create unique index IX_Keyword_Kw on dbo.Keyword (CultureName, Kw)
go

CREATE NONCLUSTERED INDEX [IX_Keyword_GetList] ON [dbo].[Keyword]
(
	[CultureName] ASC
)
INCLUDE ( 	[Kw],
	[UsedCount]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
go



go
----------------------------------------------------------------------------
-- dbo.TableName 資料表名稱
----------------------------------------------------------------------------
go
