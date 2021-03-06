// ===============================================================================
// ArticlePublisherDataAccess of SampleEFDICMS
// https://github.com/lozenlin/SampleEFDICMS
//
// ArticlePublisherDataAccess.cs
//
// ===============================================================================
// Copyright (c) 2019 lozenlin
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// ===============================================================================

using Common.Data.Domain.Model;
using Common.DataAccess.EF.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data.Domain.QueryParam;
using Common.LogicObject.DataAccessInterfaces;

namespace Common.DataAccess.EF
{
    public class ArticlePublisherDataAccess : DataAccessBase, IArticlePublisherDataAccess
    {
        public ArticlePublisherDataAccess() : base()
        {
        }

        #region 網頁內容

        /// <summary>
        /// 取得作業選單用指定語系的網頁內容清單
        /// </summary>
        public List<ArticleMultiLangForOpMenu> GetArticleMultiLangListForOpMenu(Guid parentId, string cultureName)
        {
            Logger.Debug("GetArticleMultiLangListForOpMenu(parentId)");

            List<ArticleMultiLangForOpMenu> entities = null;

            try
            {
                entities = (from am in cmsCtx.ArticleMultiLang
                            from a in cmsCtx.Article
                            where am.ArticleId == a.ArticleId
                             && am.CultureName == cultureName
                             && a.ParentId == parentId
                            orderby a.SortNo
                            select new ArticleMultiLangForOpMenu()
                            {
                                ArticleId = am.ArticleId,
                                ArticleSubject = am.ArticleSubject,
                                IsHideSelf = a.IsHideSelf
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得後台用網頁內容資料
        /// </summary>
        public ArticleForBackend GetArticleDataForBackend(Guid articleId)
        {
            Logger.Debug("GetArticleDataForBackend(articleId)");
            ArticleForBackend entity = null;

            try
            {
                entity = (from a in cmsCtx.Article
                          from e in cmsCtx.Employee
                          where a.PostAccount == e.EmpAccount
                             && a.ArticleId == articleId
                          select new ArticleForBackend()
                          {
                              ArticleId = a.ArticleId,
                              ParentId = a.ParentId,
                              ArticleLevelNo = a.ArticleLevelNo,
                              ArticleAlias = a.ArticleAlias,
                              BannerPicFileName = a.BannerPicFileName,
                              LayoutModeId = a.LayoutModeId,
                              ShowTypeId = a.ShowTypeId,
                              LinkUrl = a.LinkUrl,
                              LinkTarget = a.LinkTarget,
                              ControlName = a.ControlName,
                              SubItemControlName = a.SubItemControlName,
                              IsHideSelf = a.IsHideSelf,
                              IsHideChild = a.IsHideChild,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              SortNo = a.SortNo,
                              DontDelete = a.DontDelete,
                              PostAccount = a.PostAccount,
                              PostDate = a.PostDate,
                              MdfAccount = a.MdfAccount,
                              MdfDate = a.MdfDate,
                              SubjectAtBannerArea = a.SubjectAtBannerArea,
                              PublishDate = a.PublishDate,
                              IsShowInUnitArea = a.IsShowInUnitArea,
                              IsShowInSitemap = a.IsShowInSitemap,
                              SortFieldOfFrontStage = a.SortFieldOfFrontStage,
                              IsSortDescOfFrontStage = a.IsSortDescOfFrontStage,
                              IsListAreaShowInFrontStage = a.IsListAreaShowInFrontStage,
                              IsAttAreaShowInFrontStage = a.IsAttAreaShowInFrontStage,
                              IsPicAreaShowInFrontStage = a.IsPicAreaShowInFrontStage,
                              IsVideoAreaShowInFrontStage = a.IsVideoAreaShowInFrontStage,
                              SubItemLinkUrl = a.SubItemLinkUrl,
                              PostDeptId = e.DeptId ?? 0
                          }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entity;
        }

        /// <summary>
        /// 取得指定語系的網頁內容階層資料
        /// </summary>
        public List<ArticleMultiLangLevelInfo> GetArticleMultiLangLevelInfoList(Guid articleId, string cultureName)
        {
            Logger.Debug("GetArticleMultiLangLevelInfoList(articleId, cultureName)");
            List<ArticleMultiLangLevelInfo> entities = null;

            try
            {
                entities = new List<ArticleMultiLangLevelInfo>();
                ArticleMultiLangLevelInfo entity = null;
                Guid curArticleId = articleId;

                do
                {
                    entity = (from am in cmsCtx.ArticleMultiLang
                              from a in cmsCtx.Article
                              where am.ArticleId == a.ArticleId
                               && am.ArticleId == curArticleId
                               && am.CultureName == cultureName
                              select new ArticleMultiLangLevelInfo()
                              {
                                  ArticleId = am.ArticleId,
                                  ParentId = a.ParentId,
                                  ArticleSubject = am.ArticleSubject,
                                  ArticleLevelNo = a.ArticleLevelNo,
                                  ShowTypeId = a.ShowTypeId,
                                  LinkUrl = a.LinkUrl,
                                  LinkTarget = a.LinkTarget,
                                  IsHideSelf = a.IsHideSelf,
                                  IsShowInLang = am.IsShowInLang,
                                  StartDate = a.StartDate,
                                  EndDate = a.EndDate
                              }).FirstOrDefault();

                    if (entity == null)
                    {
                        throw new Exception(string.Format("there is no data of curArticleId[{0}].", curArticleId));
                    }

                    entities.Add(entity);

                    if (entity.ParentId.HasValue)
                        curArticleId = entity.ParentId.Value;

                } while (entity.ParentId.HasValue);

            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得後台用指定語系的網頁內容清單
        /// </summary>
        public List<ArticleForBEList> GetArticleMultiLangListForBackend(ArticleListQueryParams param)
        {
            Logger.Debug("GetArticleMultiLangListForBackend(param)");
            List<ArticleForBEList> entities = null;

            try
            {
                var tempQuery = from am in cmsCtx.ArticleMultiLang
                                join a in cmsCtx.Article on am.ArticleId equals a.ArticleId
                                join e in cmsCtx.Employee.Include(emp => emp.Department) on am.PostAccount equals e.EmpAccount
                                into amGroup
                                from e in amGroup.DefaultIfEmpty()
                                where a.ParentId == param.ParentId
                                    && am.CultureName == param.CultureName
                                select new ArticleForBEList()
                                {
                                    ArticleId = am.ArticleId,
                                    ArticleSubject = am.ArticleSubject,
                                    ReadCount = am.ReadCount,
                                    PostAccount = am.PostAccount,
                                    PostDate = am.PostDate,
                                    MdfAccount = am.MdfAccount,
                                    MdfDate = am.MdfDate,

                                    IsHideSelf = a.IsHideSelf,
                                    IsHideChild = a.IsHideChild,
                                    StartDate = a.StartDate,
                                    EndDate = a.EndDate,
                                    SortNo = a.SortNo,
                                    DontDelete = a.DontDelete,

                                    IsShowInLangZhTw = fnArticle_IsShowInLang(am.ArticleId, "zh-TW"),
                                    IsShowInLangEn = fnArticle_IsShowInLang(am.ArticleId, "en"),
                                    PostDeptId = e.DeptId ?? 0,
                                    PostDeptName = e.Department.DeptName
                                };

                // Query conditions

                if (!param.AuthParams.CanReadSubItemOfOthers)
                {
                    tempQuery = tempQuery.Where(obj =>
                        param.AuthParams.CanReadSubItemOfCrew && obj.PostDeptId == param.AuthParams.MyDeptId
                        || param.AuthParams.CanReadSubItemOfSelf && obj.PostAccount == param.AuthParams.MyAccount);
                }

                if (param.Kw != "")
                {
                    tempQuery = tempQuery.Where(obj => obj.ArticleSubject.Contains(param.Kw));
                }

                // total
                param.PagedParams.RowCount = tempQuery.Count();

                // sorting
                if (param.PagedParams.SortField != "")
                {
                    tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                }
                else
                {
                    // default
                    tempQuery = tempQuery.OrderBy(obj => obj.SortNo);
                }

                // paging
                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (skipCount > 0)
                {
                    tempQuery = tempQuery.Skip(skipCount);
                }

                if (takeCount >= 0)
                {
                    tempQuery = tempQuery.Take(takeCount);
                }

                // result
                entities = tempQuery.ToList();
                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得網頁的所有子網頁資訊
        /// </summary>
        public List<ArticleDescendant> GetArticleDescendants(Guid articleId)
        {
            Logger.Debug("GetArticleDescendants(articleId)");
            List<ArticleDescendant> entities = null;
            List<ArticleDescendant> descendants = null;
            Guid curArticleId = articleId;
            int curLevelNo = 0;

            try
            {
                entities = new List<ArticleDescendant>();

                // get current info
                ArticleDescendant entity = cmsCtx.Article.Where(obj => obj.ArticleId == articleId)
                    .Select(obj => new ArticleDescendant()
                    {
                        ArticleId = obj.ArticleId,
                        ArticleLevelNo = obj.ArticleLevelNo
                    }).FirstOrDefault();

                if (entity != null)
                {
                    curLevelNo = entity.ArticleLevelNo.Value;
                    entities.Add(entity);

                    do
                    {
                        List<Guid?> parentIds = entities.Where(obj => obj.ArticleLevelNo == curLevelNo)
                            .Select(obj => (Guid?)obj.ArticleId).ToList();

                        descendants = (from a in cmsCtx.Article
                                       where parentIds.Contains(a.ParentId)
                                       select new ArticleDescendant()
                                       {
                                           ArticleId = a.ArticleId,
                                           ArticleLevelNo = a.ArticleLevelNo
                                       }).ToList();

                        if (descendants.Count > 0)
                        {
                            entities.InsertRange(0, descendants);
                        }

                        curLevelNo++;

                    } while (descendants.Count > 0);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 刪除網頁內容
        /// </summary>
        public bool DeleteArticleData(Guid articleId)
        {
            Logger.Debug("DeleteArticleData(articleId)");
            DbContextTransaction tran = null;
            MyConfiguration.SuspendExecutionStrategy = true;

            try
            {
                tran = cmsCtx.Database.BeginTransaction();

                // delete attachment
                cmsCtx.Database.ExecuteSqlCommand(@"
delete from dbo.AttachFileMultiLang
where exists(
	select *
	from dbo.AttachFile af
	where af.AttId=AttachFileMultiLang.AttId and af.ArticleId=@p0
)", articleId);

                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.AttachFile where ArticleId = @p0", articleId);

                // delete picture
                cmsCtx.Database.ExecuteSqlCommand(@"
delete from dbo.ArticlePictureMultiLang
where exists(
	select *
	from dbo.ArticlePicture ap
	where ap.PicId=ArticlePictureMultiLang.PicId and ap.ArticleId=@p0
)", articleId);

                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.ArticlePicture where ArticleId = @p0", articleId);

                // delete video
                cmsCtx.Database.ExecuteSqlCommand(@"
delete from dbo.ArticleVideoMultiLang
where exists(
	select *
	from dbo.ArticleVideo av
	where av.VidId=ArticleVideoMultiLang.VidId and av.ArticleId=@p0
)", articleId);

                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.ArticleVideo where ArticleId = @p0", articleId);

                // delete multi language data
                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.ArticleMultiLang where ArticleId = @p0", articleId);

                // delete main data
                Article entity = new Article() { ArticleId = articleId };
                cmsCtx.Entry<Article>(entity).State = EntityState.Deleted;

                cmsCtx.SaveChanges();

                tran.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;

                if (tran != null)
                    tran.Rollback();

                return false;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();

                MyConfiguration.SuspendExecutionStrategy = false;
            }

            return true;
        }

        /// <summary>
        /// 加大網頁內容的排序編號
        /// </summary>
        public bool IncreaseArticleSortNo(Guid articleId, string mdfAccount)
        {
            Logger.Debug("IncreaseArticleSortNo(articleId, mdfAccount)");

            try
            {
                Article entity = cmsCtx.Article.Find(articleId);

                if (entity == null)
                {
                    throw new Exception("there is no data of articleId.");
                }

                // get bigger one
                Article biggerOne = cmsCtx.Article.Where(obj =>
                    obj.ParentId == entity.ParentId
                    && obj.ArticleId != entity.ArticleId
                    && obj.SortNo >= entity.SortNo)
                    .OrderBy(obj => obj.SortNo)
                    .FirstOrDefault();

                // there is no bigger one, exit
                if (biggerOne == null)
                {
                    return true;
                }

                int sortNo = entity.SortNo ?? 0;
                int biggerSortNo = biggerOne.SortNo ?? 0;

                // when the values are the same
                if (biggerSortNo == sortNo)
                {
                    biggerSortNo = sortNo + 1;
                }

                // swap
                entity.SortNo = biggerSortNo;
                entity.MdfAccount = mdfAccount;
                entity.MdfDate = DateTime.Now;

                biggerOne.SortNo = sortNo;
                biggerOne.MdfAccount = mdfAccount;
                biggerOne.MdfDate = DateTime.Now;

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 減小網頁內容的排序編號
        /// </summary>
        public bool DecreaseArticleSortNo(Guid articleId, string mdfAccount)
        {
            Logger.Debug("DecreaseArticleSortNo(articleId, mdfAccount)");

            try
            {
                Article entity = cmsCtx.Article.Find(articleId);

                if (entity == null)
                {
                    throw new Exception("there is no data of articleId.");
                }

                // get smaller one
                Article smallerOne = cmsCtx.Article.Where(obj =>
                    obj.ParentId == entity.ParentId
                    && obj.ArticleId != entity.ArticleId
                    && obj.SortNo <= entity.SortNo)
                    .OrderByDescending(obj => obj.SortNo)
                    .FirstOrDefault();

                // there is no smaller one, exit
                if (smallerOne == null)
                {
                    return true;
                }

                int sortNo = entity.SortNo ?? 0;
                int smallerSortNo = smallerOne.SortNo ?? 0;

                // when the values are the same
                if (smallerSortNo == sortNo)
                {
                    sortNo = smallerSortNo + 1;
                }

                // swap
                entity.SortNo = smallerSortNo;
                entity.MdfAccount = mdfAccount;
                entity.MdfDate = DateTime.Now;

                smallerOne.SortNo = sortNo;
                smallerOne.MdfAccount = mdfAccount;
                smallerOne.MdfDate = DateTime.Now;

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新網頁內容的指定區域是否在前台顯示
        /// </summary>
        public bool UpdateArticleIsAreaShowInFrontStage(ArticleUpdateIsAreaShowInFrontStageParams param)
        {
            Logger.Debug("UpdateArticleIsAreaShowInFrontStage(param)");

            try
            {
                // get valid entity
                var tempQuery = from a in cmsCtx.Article
                                join e in cmsCtx.Employee on a.PostAccount equals e.EmpAccount
                                into articleGroup
                                from e in articleGroup.DefaultIfEmpty()
                                where a.ArticleId == param.ArticleId
                                    && (param.AuthUpdateParams.CanEditSubItemOfOthers
                                        || param.AuthUpdateParams.CanEditSubItemOfCrew && e.DeptId == param.AuthUpdateParams.MyDeptId
                                        || param.AuthUpdateParams.CanEditSubItemOfSelf && a.PostAccount == param.AuthUpdateParams.MyAccount)
                                select a;

                Article entity = tempQuery.FirstOrDefault();

                if (entity == null)
                {
                    throw new Exception("update failed");
                }

                switch (param.AreaName)
                {
                    case "ListArea":
                        entity.IsListAreaShowInFrontStage = param.IsShowInFrontStage;
                        break;
                    case "AttArea":
                        entity.IsAttAreaShowInFrontStage = param.IsShowInFrontStage;
                        break;
                    case "PicArea":
                        entity.IsPicAreaShowInFrontStage = param.IsShowInFrontStage;
                        break;
                    case "VideoArea":
                        entity.IsVideoAreaShowInFrontStage = param.IsShowInFrontStage;
                        break;
                }

                if (cmsCtx.Entry<Article>(entity).State == EntityState.Modified)
                {
                    entity.MdfAccount = param.MdfAccount;
                    entity.MdfDate = DateTime.Now;
                }

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新網頁內容的前台子項目排序欄位
        /// </summary>
        public bool UpdateArticleSortFieldOfFrontStage(ArticleUpdateSortFieldOfFrontStageParams param)
        {
            Logger.Debug("UpdateArticleSortFieldOfFrontStage(param)");

            try
            {
                // get valid entity
                var tempQuery = from a in cmsCtx.Article
                                join e in cmsCtx.Employee on a.PostAccount equals e.EmpAccount
                                into articleGroup
                                from e in articleGroup.DefaultIfEmpty()
                                where a.ArticleId == param.ArticleId
                                    && (param.AuthUpdateParams.CanEditSubItemOfOthers
                                        || param.AuthUpdateParams.CanEditSubItemOfCrew && e.DeptId == param.AuthUpdateParams.MyDeptId
                                        || param.AuthUpdateParams.CanEditSubItemOfSelf && a.PostAccount == param.AuthUpdateParams.MyAccount)
                                select a;

                Article entity = tempQuery.FirstOrDefault();

                if (entity == null)
                {
                    throw new Exception("update failed");
                }

                entity.SortFieldOfFrontStage = param.SortFieldOfFrontStage;
                entity.IsSortDescOfFrontStage = param.IsSortDescOfFrontStage;
                entity.MdfAccount = param.MdfAccount;
                entity.MdfDate = DateTime.Now;

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得網頁內容最大排序編號
        /// </summary>
        public int GetArticleMaxSortNo(Guid parentId)
        {
            Logger.Debug("GetArticleMaxSortNo(parentId)");
            int result = 0;

            try
            {
                result = cmsCtx.Article.Where(obj => obj.ParentId == parentId).Max(obj => obj.SortNo) ?? 0;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return -1;
            }

            return result;
        }

        /// <summary>
        /// 新增網頁內容
        /// </summary>
        public InsertResult InsertArticleData(Article entity)
        {
            Logger.Debug("InsertArticleData(entity)");
            InsertResult insResult = new InsertResult() { IsSuccess = false };

            try
            {
                // check id
                if (cmsCtx.Article.Any(obj => obj.ArticleId == entity.ArticleId))
                {
                    sqlErrNumber = 50000;
                    sqlErrState = 2;
                    return insResult;
                }

                // check alias
                if (cmsCtx.Article.Any(obj => obj.ArticleAlias == entity.ArticleAlias))
                {
                    sqlErrNumber = 50000;
                    sqlErrState = 3;
                    return insResult;
                }

                // get parent info
                var parent = cmsCtx.Article.Where(obj => obj.ArticleId == entity.ParentId)
                    .Select(obj => new
                    {
                        obj.ArticleLevelNo
                    }).FirstOrDefault();

                entity.ArticleLevelNo = parent.ArticleLevelNo.Value + 1;
                entity.PostDate = DateTime.Now;

                cmsCtx.Article.Add(entity);
                cmsCtx.SaveChanges();

                insResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return insResult;
            }

            return insResult;
        }

        /// <summary>
        /// 更新網頁內容
        /// </summary>
        public bool UpdateArticleData(Article entity)
        {
            Logger.Debug("UpdateArticleData(entity)");

            try
            {
                // check alias
                if (cmsCtx.Article.Any(obj => obj.ArticleAlias == entity.ArticleAlias && obj.ArticleId != entity.ArticleId))
                {
                    sqlErrNumber = 50000;
                    sqlErrState = 3;
                    return false;
                }

                entity.MdfDate = DateTime.Now;
                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得使用在單元區的有效網頁清單
        /// </summary>
        public List<ArticleForFEUnitArea> GetArticleValidListForUnitArea(Guid parentId, string cultureName, bool isShowInUnitArea)
        {
            Logger.Debug("GetArticleValidListForUnitArea(parentId, cultureName, isShowInUnitArea)");
            List<ArticleForFEUnitArea> entities = null;

            try
            {
                entities = (from am in cmsCtx.ArticleMultiLang
                            from a in cmsCtx.Article
                            where am.ArticleId == a.ArticleId
                             && a.ParentId == parentId
                             && am.CultureName == cultureName
                             && !a.IsHideSelf
                             && a.StartDate <= DateTime.Now && DateTime.Now < DbFunctions.AddDays(a.EndDate, 1)
                             && am.IsShowInLang
                             && a.IsShowInUnitArea == isShowInUnitArea
                            orderby a.SortNo
                            select new ArticleForFEUnitArea()
                            {
                                ArticleId = am.ArticleId,
                                ArticleSubject = am.ArticleSubject,
                                ArticleAlias = a.ArticleAlias,
                                ShowTypeId = a.ShowTypeId,
                                LinkUrl = a.LinkUrl,
                                LinkTarget = a.LinkTarget,
                                IsHideChild = a.IsHideChild
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得使用在側邊區塊的有效網頁清單
        /// </summary>
        public List<ArticleForFESideSection> GetArticleValidListForSideSection(Guid parentId, string cultureName)
        {
            Logger.Debug("GetArticleValidListForSideSection(parentId, cultureName)");
            List<ArticleForFESideSection> entities = null;

            try
            {
                entities = (from am in cmsCtx.ArticleMultiLang
                            from a in cmsCtx.Article
                            where am.ArticleId == a.ArticleId
                             && a.ParentId == parentId
                             && am.CultureName == cultureName
                             && !a.IsHideSelf
                             && a.StartDate <= DateTime.Now && DateTime.Now < DbFunctions.AddDays(a.EndDate, 1)
                             && am.IsShowInLang
                            orderby a.SortNo
                            select new ArticleForFESideSection()
                            {
                                ArticleId = am.ArticleId,
                                ArticleSubject = am.ArticleSubject,
                                ArticleAlias = a.ArticleAlias,
                                ShowTypeId = a.ShowTypeId,
                                LinkUrl = a.LinkUrl,
                                LinkTarget = a.LinkTarget,
                                IsHideChild = a.IsHideChild
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得使用在網站導覽的有效網頁清單
        /// </summary>
        public List<ArticleForFESitemap> GetArticleValidListForSitemap(Guid parentId, string cultureName)
        {
            Logger.Debug("GetArticleValidListForSitemap(parentId, cultureName)");
            List<ArticleForFESitemap> entities = null;

            try
            {
                entities = (from am in cmsCtx.ArticleMultiLang
                            from a in cmsCtx.Article
                            where am.ArticleId == a.ArticleId
                             && a.ParentId == parentId
                             && am.CultureName == cultureName
                             && !a.IsHideSelf
                             && a.StartDate <= DateTime.Now && DateTime.Now < DbFunctions.AddDays(a.EndDate, 1)
                             && am.IsShowInLang
                             && a.IsShowInSitemap
                            orderby a.SortNo
                            select new ArticleForFESitemap()
                            {
                                ArticleId = am.ArticleId,
                                ArticleSubject = am.ArticleSubject,
                                ArticleAlias = a.ArticleAlias,
                                ShowTypeId = a.ShowTypeId,
                                LinkUrl = a.LinkUrl,
                                LinkTarget = a.LinkTarget,
                                IsHideChild = a.IsHideChild,
                                ArticleLevelNo = a.ArticleLevelNo
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 取得前台用網頁內容資料
        /// </summary>
        public ArticleForFrontend GetArticleDataForFrontend(Guid articleId, string cultureName)
        {
            Logger.Debug("GetArticleDataForFrontend(articleId, cultureName)");
            ArticleForFrontend entity = null;

            try
            {
                entity = (from a in cmsCtx.Article
                          from am in cmsCtx.ArticleMultiLang
                          where a.ArticleId == am.ArticleId
                           && a.ArticleId == articleId
                           && am.CultureName == cultureName
                          select new ArticleForFrontend()
                          {
                              ParentId = a.ParentId,
                              ArticleLevelNo = a.ArticleLevelNo,
                              ArticleAlias = a.ArticleAlias,
                              BannerPicFileName = a.BannerPicFileName,
                              LayoutModeId = a.LayoutModeId,
                              ShowTypeId = a.ShowTypeId,
                              LinkUrl = a.LinkUrl,
                              LinkTarget = a.LinkTarget,
                              ControlName = a.ControlName,
                              IsHideSelf = a.IsHideSelf,
                              IsHideChild = a.IsHideChild,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              SortNo = a.SortNo,
                              PostAccount = a.PostAccount,
                              PostDate = a.PostDate,
                              MdfAccount = a.MdfAccount,
                              MdfDate = a.MdfDate,
                              SubjectAtBannerArea = a.SubjectAtBannerArea,
                              PublishDate = a.PublishDate,
                              IsShowInUnitArea = a.IsShowInUnitArea,
                              IsShowInSitemap = a.IsShowInSitemap,
                              SortFieldOfFrontStage = a.SortFieldOfFrontStage,
                              IsSortDescOfFrontStage = a.IsSortDescOfFrontStage,
                              IsListAreaShowInFrontStage = a.IsListAreaShowInFrontStage,
                              IsAttAreaShowInFrontStage = a.IsAttAreaShowInFrontStage,
                              IsPicAreaShowInFrontStage = a.IsPicAreaShowInFrontStage,
                              IsVideoAreaShowInFrontStage = a.IsVideoAreaShowInFrontStage,
                              ArticleSubject = am.ArticleSubject,
                              ArticleContext = am.ArticleContext,
                              ReadCount = am.ReadCount,
                              IsShowInLang = am.IsShowInLang,
                              Subtitle = am.Subtitle,
                              PublisherName = am.PublisherName
                          }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entity;
        }

        /// <summary>
        /// 取得前台用的有效網頁內容清單
        /// </summary>
        public List<ArticleForFEList> GetArticleValidListForFrontend(ArticleValidListQueryParams param)
        {
            Logger.Debug("GetArticleValidListForFrontend(param)");
            List<ArticleForFEList> entities = null;

            try
            {
                var tempQuery = from am in cmsCtx.ArticleMultiLang
                                from a in cmsCtx.Article
                                where am.ArticleId == a.ArticleId
                                    && a.ParentId == param.ParentId
                                    && am.CultureName == param.CultureName
                                    && !a.IsHideSelf
                                    && a.StartDate <= DateTime.Now && DateTime.Now < DbFunctions.AddDays(a.EndDate, 1)
                                    && am.IsShowInLang
                                select new ArticleForFEList()
                                {
                                    ArticleId = am.ArticleId,
                                    ArticleSubject = am.ArticleSubject,
                                    PublisherName = am.PublisherName,
                                    TextContext = am.TextContext,
                                    ArticleAlias = a.ArticleAlias,
                                    ShowTypeId = a.ShowTypeId,
                                    LinkUrl = a.LinkUrl,
                                    LinkTarget = a.LinkTarget,
                                    StartDate = a.StartDate,
                                    SortNo = a.SortNo,
                                    PostDate = a.PostDate,
                                    MdfDate = a.MdfDate,
                                    PublishDate = a.PublishDate
                                };

                // Query conditions

                if (param.Kw != "")
                {
                    tempQuery = tempQuery.Where(obj => obj.ArticleSubject.Contains(param.Kw));
                }

                // total
                param.PagedParams.RowCount = tempQuery.Count();

                // sorting
                if (param.PagedParams.SortField != "")
                {
                    tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                }
                else
                {
                    // default
                    tempQuery = tempQuery.OrderBy(obj => obj.SortNo);
                }

                // paging
                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (skipCount > 0)
                {
                    tempQuery = tempQuery.Skip(skipCount);
                }

                if (takeCount >= 0)
                {
                    tempQuery = tempQuery.Take(takeCount);
                }

                // result
                entities = tempQuery.ToList();
                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 依網址別名取得網頁代碼
        /// </summary>
        public Guid? GetArticleIdByAlias(string articleAlias)
        {
            Logger.Debug("GetArticleIdByAlias(articleAlias)");
            Guid? result = null;

            try
            {
                result = cmsCtx.Article.Where(obj => obj.ArticleAlias == articleAlias)
                    .OrderBy(obj => obj.PostDate)
                    .Select(obj => obj.ArticleId)
                    .First();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return result;
        }

        /// <summary>
        /// 依超連結網址取得網頁代碼
        /// </summary>
        public Guid? GetArticleIdByLinkUrl(string linkUrl)
        {
            Logger.Debug("GetArticleIdByLinkUrl(linkUrl)");
            Guid? result = null;

            try
            {
                result = cmsCtx.Article.Where(obj => obj.ShowTypeId == 3 /* URL */ && obj.LinkUrl == linkUrl)
                    .OrderBy(obj => obj.PostDate)
                    .Select(obj => obj.ArticleId)
                    .First();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return result;
        }

        /// <summary>
        /// 取得指定網頁內容的前幾層網頁代碼
        /// </summary>
        public ArticleTopLevelIds GetArticleTopLevelIds(Guid articleId)
        {
            Logger.Debug("GetArticleTopLevelIds(Guid articleId)");
            ArticleTopLevelIds result = null;

            try
            {
                result = new ArticleTopLevelIds();
                Guid? curArticleId = articleId;

                do
                {
                    var entity = (from a in cmsCtx.Article
                                  where a.ArticleId == curArticleId
                                  select new
                                  {
                                      a.ArticleId,
                                      a.ParentId,
                                      a.ArticleLevelNo
                                  }).FirstOrDefault();

                    curArticleId = null;

                    if (entity != null && entity.ArticleLevelNo >= 1)
                    {
                        switch (entity.ArticleLevelNo.Value)
                        {
                            case 1:
                                result.Lv1Id = entity.ArticleId;
                                break;
                            case 2:
                                result.Lv2Id = entity.ArticleId;
                                break;
                            case 3:
                                result.Lv3Id = entity.ArticleId;
                                break;
                        }

                        if (entity.ParentId.HasValue && entity.ArticleLevelNo > 1)
                        {
                            curArticleId = entity.ParentId;
                        }
                    }

                } while (curArticleId.HasValue);
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return result;
        }

        #endregion

        #region 附件檔案

        /// <summary>
        /// 取得後台用指定語系的附件檔案清單
        /// </summary>
        public List<AttachFileForBEList> GetAttachFileMultiLangListForBackend(AttachFileListQueryParams param)
        {
            Logger.Debug("GetAttachFileMultiLangListForBackend(param)");
            List<AttachFileForBEList> entities = null;

            try
            {
                var tempQuery = from afm in cmsCtx.AttachFileMultiLang
                                join af in cmsCtx.AttachFile on afm.AttId equals af.AttId
                                join e in cmsCtx.Employee.Include(emp => emp.Department) on afm.PostAccount equals e.EmpAccount
                                into afmGroup
                                from e in afmGroup.DefaultIfEmpty()
                                where af.ArticleId == param.ArticleId
                                    && afm.CultureName == param.CultureName
                                select new AttachFileForBEList()
                                {
                                    AttId = afm.AttId,
                                    AttSubject = afm.AttSubject,
                                    ReadCount = afm.ReadCount,
                                    PostAccount = afm.PostAccount,
                                    PostDate = afm.PostDate,
                                    MdfAccount = afm.MdfAccount,
                                    MdfDate = afm.MdfDate,

                                    FilePath = af.FilePath,
                                    FileSavedName = af.FileSavedName,
                                    FileSize = af.FileSize,
                                    SortNo = af.SortNo,
                                    FileMIME = af.FileMIME,
                                    DontDelete = af.DontDelete,

                                    IsShowInLangZhTw = fnAttachFile_IsShowInLang(afm.AttId, "zh-TW"),
                                    IsShowInLangEn = fnAttachFile_IsShowInLang(afm.AttId, "en"),
                                    PostDeptId = e.DeptId ?? 0,
                                    PostDeptName = e.Department.DeptName
                                };

                // Query conditions

                if (!param.AuthParams.CanReadSubItemOfOthers)
                {
                    tempQuery = tempQuery.Where(obj =>
                        param.AuthParams.CanReadSubItemOfCrew && obj.PostDeptId == param.AuthParams.MyDeptId
                        || param.AuthParams.CanReadSubItemOfSelf && obj.PostAccount == param.AuthParams.MyAccount);
                }

                if (param.Kw != "")
                {
                    tempQuery = tempQuery.Where(obj =>
                        obj.AttSubject.Contains(param.Kw));
                }

                // total
                param.PagedParams.RowCount = tempQuery.Count();

                // sorting
                if (param.PagedParams.SortField != "")
                {
                    tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                }
                else
                {
                    // default
                    tempQuery = tempQuery.OrderBy(obj => obj.SortNo);
                }

                // paging
                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (skipCount > 0)
                {
                    tempQuery = tempQuery.Skip(skipCount);
                }

                if (takeCount >= 0)
                {
                    tempQuery = tempQuery.Take(takeCount);
                }

                // result
                entities = tempQuery.ToList();
                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 加大附件檔案的排序編號
        /// </summary>
        public bool IncreaseAttachFileSortNo(Guid attId, string mdfAccount)
        {
            Logger.Debug("IncreaseAttachFileSortNo(attId, mdfAccount)");

            try
            {
                AttachFile entity = cmsCtx.AttachFile.Find(attId);

                if (entity == null)
                {
                    throw new Exception("there is no data of attId.");
                }

                // get bigger one
                AttachFile biggerOne = cmsCtx.AttachFile.Where(obj =>
                    obj.ArticleId == entity.ArticleId
                    && obj.AttId != attId
                    && obj.SortNo >= entity.SortNo)
                    .OrderBy(obj => obj.SortNo)
                    .FirstOrDefault();

                // there is no bigger one, exit
                if (biggerOne == null)
                {
                    return true;
                }

                int sortNo = entity.SortNo ?? 0;
                int biggerSortNo = biggerOne.SortNo ?? 0;

                // when the values are the same
                if (biggerSortNo == sortNo)
                {
                    biggerSortNo++;
                }

                // swap
                entity.SortNo = biggerSortNo;
                entity.MdfAccount = mdfAccount;
                entity.MdfDate = DateTime.Now;

                biggerOne.SortNo = sortNo;
                biggerOne.MdfAccount = mdfAccount;
                biggerOne.MdfDate = DateTime.Now;

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 減小附件檔案的排序編號
        /// </summary>
        public bool DecreaseAttachFileSortNo(Guid attId, string mdfAccount)
        {
            Logger.Debug("DecreaseAttachFileSortNo(attId, mdfAccount)");

            try
            {
                AttachFile entity = cmsCtx.AttachFile.Find(attId);

                if (entity == null)
                {
                    throw new Exception("there is no data of attId");
                }

                // get smaller one
                AttachFile smallerOne = cmsCtx.AttachFile.Where(obj =>
                    obj.ArticleId == entity.ArticleId
                    && obj.AttId != attId
                    && obj.SortNo <= entity.SortNo)
                    .OrderByDescending(obj => obj.SortNo)
                    .FirstOrDefault();

                // there is no smaller one, exit
                if (smallerOne == null)
                {
                    return true;
                }

                int sortNo = entity.SortNo ?? 0;
                int smallerSortNo = smallerOne.SortNo ?? 0;

                // when the values are the same
                if (smallerSortNo == sortNo)
                {
                    sortNo++;
                }

                // swap
                entity.SortNo = smallerSortNo;
                entity.MdfAccount = mdfAccount;
                entity.MdfDate = DateTime.Now;

                smallerOne.SortNo = sortNo;
                smallerOne.MdfAccount = mdfAccount;
                smallerOne.MdfDate = DateTime.Now;

                cmsCtx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 刪除附件檔案資料
        /// </summary>
        public bool DeleteAttachFileData(Guid attId)
        {
            Logger.Debug("DeleteAttachFileData(attId)");
            DbContextTransaction tran = null;
            MyConfiguration.SuspendExecutionStrategy = true;

            try
            {
                tran = cmsCtx.Database.BeginTransaction();

                // delete multi language data
                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.AttachFileMultiLang where AttId=@p0", attId);

                // delete main data
                AttachFile entity = Get<AttachFile>(attId);
                cmsCtx.Entry<AttachFile>(entity).State = EntityState.Deleted;
                cmsCtx.SaveChanges();

                tran.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;

                if (tran != null)
                    tran.Rollback();

                return false;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();

                MyConfiguration.SuspendExecutionStrategy = false;
            }

            return true;
        }

        /// <summary>
        /// 取得附件檔案的最大排序編號
        /// </summary>
        public int GetAttachFileMaxSortNo(Guid? articleId)
        {
            Logger.Debug("GetAttachFileMaxSortNo(articleId)");
            int result = 0;

            try
            {
                result = cmsCtx.AttachFile.Where(obj => obj.ArticleId == articleId).Max(obj => obj.SortNo) ?? 0;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return -1;
            }

            return result;
        }

        /// <summary>
        /// 取得前台用附件檔案清單
        /// </summary>
        public List<AttachFileForFrontend> GetAttachFileListForFrontend(Guid articleId, string cultureName)
        {
            Logger.Debug("GetAttachFileListForFrontend(articleId, cultureName)");
            List<AttachFileForFrontend> entities = null;

            try
            {
                entities = (from afm in cmsCtx.AttachFileMultiLang
                            from af in cmsCtx.AttachFile
                            where afm.AttId == af.AttId
                             && af.ArticleId == articleId
                             && afm.CultureName == cultureName
                             && afm.IsShowInLang
                            orderby af.SortNo
                            select new AttachFileForFrontend()
                            {
                                AttId = afm.AttId,
                                AttSubject = afm.AttSubject,
                                ReadCount = afm.ReadCount,
                                FileSavedName = af.FileSavedName,
                                FileSize = af.FileSize,
                                SortNo = af.SortNo,
                                PostDate = af.PostDate,
                                MdfDate = af.MdfDate
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        #endregion

        #region 網頁照片

        /// <summary>
        /// 取得後台用指定語系的網頁照片清單
        /// </summary>
        /// <history>
        /// 2019/02/28, lozenlin, modify, typo fixes "Picutre" to "Picture"
        /// </history>
        public List<ArticlePictureForBEList> GetArticlePictureMultiLangListForBackend(ArticlePictureListQueryParams param)
        {
            Logger.Debug("GetArticlePictureMultiLangListForBackend(param)");
            List<ArticlePictureForBEList> entities = null;

            try
            {
                var tempQuery = from apm in cmsCtx.ArticlePictureMultiLang
                                join ap in cmsCtx.ArticlePicture on apm.PicId equals ap.PicId
                                join e in cmsCtx.Employee.Include(emp => emp.Department) on apm.PostAccount equals e.EmpAccount
                                into apmGroup
                                from e in apmGroup.DefaultIfEmpty()
                                where ap.ArticleId == param.ArticleId
                                    && apm.CultureName == param.CultureName
                                select new ArticlePictureForBEList()
                                {
                                    PicId = apm.PicId,
                                    PicSubject = apm.PicSubject,
                                    PostAccount = apm.PostAccount,
                                    PostDate = apm.PostDate,
                                    MdfAccount = apm.MdfAccount,
                                    MdfDate = apm.MdfDate,
                                    FileSavedName = ap.FileSavedName,
                                    FileSize = ap.FileSize,
                                    SortNo = ap.SortNo,
                                    FileMIME = ap.FileMIME,
                                    IsShowInLangZhTw = fnArticlePicture_IsShowInLang(apm.PicId, "zh-TW"),
                                    IsShowInLangEn = fnArticlePicture_IsShowInLang(apm.PicId, "en"),
                                    PostDeptId = e.DeptId ?? 0,
                                    PostDeptName = e.Department.DeptName
                                };

                // Query conditions

                if (!param.AuthParams.CanReadSubItemOfOthers)
                {
                    tempQuery = tempQuery.Where(obj =>
                       param.AuthParams.CanReadSubItemOfCrew && obj.PostDeptId == param.AuthParams.MyDeptId
                       || param.AuthParams.CanReadSubItemOfSelf && obj.PostAccount == param.AuthParams.MyAccount);
                }

                if (param.Kw != "")
                {
                    tempQuery = tempQuery.Where(obj =>
                        obj.PicSubject.Contains(param.Kw));
                }

                // total
                param.PagedParams.RowCount = tempQuery.Count();

                // sorting
                if (param.PagedParams.SortField != "")
                {
                    tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                }
                else
                {
                    tempQuery = tempQuery.OrderByDescending(obj => obj.SortNo);
                }

                // paging
                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (skipCount > 0)
                {
                    tempQuery = tempQuery.Skip(skipCount);
                }

                if (takeCount >= 0)
                {
                    tempQuery = tempQuery.Take(takeCount);
                }

                // result
                entities = tempQuery.ToList();
                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 刪除網頁照片資料
        /// </summary>
        public bool DeleteArticlePictureData(Guid picId)
        {
            Logger.Debug("DeleteArticlePictureData(picId)");
            DbContextTransaction tran = null;
            MyConfiguration.SuspendExecutionStrategy = true;

            try
            {
                tran = cmsCtx.Database.BeginTransaction();

                // delete multi language data
                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.ArticlePictureMultiLang where PicId=@p0", picId);

                // delete main data
                ArticlePicture entity = Get<ArticlePicture>(picId);
                cmsCtx.Entry<ArticlePicture>(entity).State = EntityState.Deleted;
                cmsCtx.SaveChanges();

                tran.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;

                if (tran != null)
                    tran.Rollback();

                return false;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();

                MyConfiguration.SuspendExecutionStrategy = false;
            }

            return true;
        }

        /// <summary>
        /// 取得網頁照片的最大排序編號
        /// </summary>
        public int GetArticlePictureMaxSortNo(Guid? articleId)
        {
            Logger.Debug("GetArticlePictureMaxSortNo(articleId)");
            int result = 0;

            try
            {
                result = cmsCtx.ArticlePicture.Where(obj => obj.ArticleId == articleId).Max(obj => obj.SortNo) ?? 0;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return -1;
            }

            return result;
        }

        /// <summary>
        /// 取得前台用網頁照片清單
        /// </summary>
        public List<ArticlePictureForFrontend> GetArticlePictureListForFrontend(Guid articleId, string cultureName)
        {
            Logger.Debug("GetArticlePictureListForFrontend(articleId, cultureName)");
            List<ArticlePictureForFrontend> entities = null;

            try
            {
                entities = (from apm in cmsCtx.ArticlePictureMultiLang
                            from ap in cmsCtx.ArticlePicture
                            where apm.PicId == ap.PicId
                             && ap.ArticleId == articleId
                             && apm.CultureName == cultureName
                             && apm.IsShowInLang
                            orderby ap.SortNo descending
                            select new ArticlePictureForFrontend()
                            {
                                PicId = apm.PicId,
                                PicSubject = apm.PicSubject,
                                FileSavedName = ap.FileSavedName,
                                SortNo = ap.SortNo
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        #endregion

        #region 網頁影片

        /// <summary>
        /// 取得後台用指定語系的網頁影片清單
        /// </summary>
        public List<ArticleVideoForBEList> GetArticleVideoMultiLangListForBackend(ArticleVideoListQueryParams param)
        {
            Logger.Debug("GetArticleVideoMultiLangListForBackend(param)");
            List<ArticleVideoForBEList> entities = null;

            try
            {
                var tempQuery = from avm in cmsCtx.ArticleVideoMultiLang
                                join av in cmsCtx.ArticleVideo on avm.VidId equals av.VidId
                                join e in cmsCtx.Employee.Include(emp => emp.Department) on avm.PostAccount equals e.EmpAccount
                                into avmGroup
                                from e in avmGroup.DefaultIfEmpty()
                                where av.ArticleId == param.ArticleId
                                    && avm.CultureName == param.CultureName
                                select new ArticleVideoForBEList()
                                {
                                    VidId = avm.VidId,
                                    VidSubject = avm.VidSubject,
                                    VidDesc = avm.VidDesc,
                                    PostAccount = avm.PostAccount,
                                    PostDate = avm.PostDate,
                                    MdfAccount = avm.MdfAccount,
                                    MdfDate = avm.MdfDate,
                                    SortNo = av.SortNo,
                                    VidLinkUrl = av.VidLinkUrl,
                                    SourceVideoId = av.SourceVideoId,
                                    IsShowInLangZhTw = fnArticleVideo_IsShowInLang(avm.VidId, "zh-TW"),
                                    IsShowInLangEn = fnArticleVideo_IsShowInLang(avm.VidId, "en"),
                                    PostDeptId = e.DeptId ?? 0,
                                    PostDeptName = e.Department.DeptName
                                };

                // Query conditions

                if (!param.AuthParams.CanReadSubItemOfOthers)
                {
                    tempQuery = tempQuery.Where(obj =>
                        param.AuthParams.CanReadSubItemOfCrew && obj.PostDeptId == param.AuthParams.MyDeptId
                        || param.AuthParams.CanReadSubItemOfSelf && obj.PostAccount == param.AuthParams.MyAccount);
                }

                if (param.Kw != "")
                {
                    tempQuery = tempQuery.Where(obj => obj.VidSubject.Contains(param.Kw));
                }

                // total
                param.PagedParams.RowCount = tempQuery.Count();

                // sorting
                if (param.PagedParams.SortField != "")
                {
                    tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                }
                else
                {
                    // default
                    tempQuery = tempQuery.OrderByDescending(obj => obj.SortNo);
                }

                // paging
                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (skipCount > 0)
                {
                    tempQuery = tempQuery.Skip(skipCount);
                }

                if (takeCount >= 0)
                {
                    tempQuery = tempQuery.Take(takeCount);
                }

                // result
                entities = tempQuery.ToList();
                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        /// <summary>
        /// 刪除網頁影片資料
        /// </summary>
        public bool DeleteArticleVideoData(Guid vidId)
        {
            Logger.Debug("DeleteArticleVideoData(vidId)");
            DbContextTransaction tran = null;
            MyConfiguration.SuspendExecutionStrategy = true;

            try
            {
                tran = cmsCtx.Database.BeginTransaction();

                // delete multi language data
                cmsCtx.Database.ExecuteSqlCommand("delete from dbo.ArticleVideoMultiLang where VidId=@p0", vidId);

                // delete main data
                ArticleVideo entity = new ArticleVideo() { VidId = vidId };
                cmsCtx.Entry<ArticleVideo>(entity).State = EntityState.Deleted;
                cmsCtx.SaveChanges();

                tran.Commit();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;

                if (tran != null)
                    tran.Rollback();

                return false;
            }
            finally
            {
                if (tran != null)
                    tran.Dispose();

                MyConfiguration.SuspendExecutionStrategy = false;
            }

            return true;
        }

        /// <summary>
        /// 取得網頁影片的最大排序編號
        /// </summary>
        public int GetArticleVideoMaxSortNo(Guid articleId)
        {
            Logger.Debug("GetArticleVideoMaxSortNo(articleId)");
            int result = 0;

            try
            {
                result = cmsCtx.ArticleVideo.Where(obj => obj.ArticleId == articleId).Max(obj => obj.SortNo) ?? 0;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return -1;
            }

            return result;
        }

        /// <summary>
        /// 取得前台用網頁影片清單
        /// </summary>
        public List<ArticleVideoForFrontend> GetArticleVideoListForFrontend(Guid articleId, string cultureName)
        {
            Logger.Debug("GetArticleVideoListForFrontend(articleId, cultureName)");
            List<ArticleVideoForFrontend> entities = null;

            try
            {
                entities = (from avm in cmsCtx.ArticleVideoMultiLang
                            from av in cmsCtx.ArticleVideo
                            where avm.VidId == av.VidId
                             && av.ArticleId == articleId
                             && avm.CultureName == cultureName
                             && avm.IsShowInLang
                            orderby av.SortNo descending
                            select new ArticleVideoForFrontend()
                            {
                                VidId = avm.VidId,
                                VidSubject = avm.VidSubject,
                                VidDesc = avm.VidDesc,
                                SortNo = av.SortNo,
                                SourceVideoId = av.SourceVideoId
                            }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        #endregion

        #region 搜尋關鍵字

        /// <summary>
        /// 儲存搜尋關鍵字
        /// </summary>
        public bool SaveKeywordData(string cultureName, string kw)
        {
            Logger.Debug("SaveKeywordData(cultureName, kw)");

            try
            {
                if (cmsCtx.Keyword.Any(obj => obj.CultureName == cultureName && obj.Kw == kw))
                {
                    // increase used count
                    Keyword entity = cmsCtx.Keyword.Where(obj =>
                        obj.CultureName == cultureName
                        && obj.Kw == kw
                        && obj.UsedCount >= 0)  // UsedCount >= 0 : enabled
                        .FirstOrDefault();

                    if (entity != null)
                    {
                        entity.UsedCount++;
                        cmsCtx.SaveChanges();
                    }
                }
                else
                {
                    // new one
                    Keyword entity = new Keyword()
                    {
                        CultureName = cultureName,
                        Kw = kw,
                        UsedCount = 1
                    };

                    cmsCtx.Keyword.Add(entity);
                    cmsCtx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得前台用搜尋關鍵字
        /// </summary>
        public List<Keyword> GetKeywordListForFrontend(string cultureName, string kw, int topCount)
        {
            Logger.Debug("GetKeywordListForFrontend(cultureName, kw, topCount)");
            List<Keyword> entities = null;

            try
            {
                entities = (from k in cmsCtx.Keyword
                            where k.CultureName == cultureName
                                && k.Kw.Contains(kw)
                                && k.UsedCount > 0
                            orderby k.UsedCount descending
                            select k)
                            .Take(topCount)
                            .ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        #endregion

        #region 搜尋用資料來源

        /// <summary>
        /// 建立搜尋用資料來源
        /// </summary>
        public bool BuildSearchDataSource(string mainLinkUrl)
        {
            Logger.Debug("BuildSearchDataSource(mainLinkUrl)");

            try
            {
                cmsCtx.spSearchDataSource_Build(mainLinkUrl);
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 取得搜尋用資料來源清單
        /// </summary>
        /// <returns></returns>
        public List<SearchDataSourceForFrontend> GetSearchDataSourceList(SearchResultListQueryParams param)
        {
            Logger.Debug("GetSearchDataSourceList(param)");
            List<SearchDataSourceForFrontend> entities = null;

            try
            {
                var tempQuery = from sds in cmsCtx.SearchDataSource
                                select new SearchDataSourceForFrontend()
                                {
                                    ArticleId = sds.ArticleId,
                                    SubId = sds.SubId,
                                    CultureName = sds.CultureName,
                                    ArticleSubject = sds.ArticleSubject,
                                    ArticleContext = sds.ArticleContext,
                                    ReadCount = sds.ReadCount,
                                    LinkUrl = sds.LinkUrl,
                                    PublishDate = sds.PublishDate,
                                    BreadcrumbData = sds.BreadcrumbData,
                                    Lv1ArticleId = sds.Lv1ArticleId,
                                    PostDate = sds.PostDate,
                                    MdfDate = sds.MdfDate,
                                    MatchesTotal = 0
                                };

                // Query conditions
                tempQuery = tempQuery.Where(obj => obj.CultureName == param.CultureName);

                int skipCount = param.PagedParams.GetSkipCount();
                int takeCount = param.PagedParams.GetTakeCount();

                if (param.Keywords.IndexOf(',') < 0)
                {
                    // 單一關鍵字查詢

                    // Query conditions
                    if (param.Keywords != "")
                    {
                        tempQuery = tempQuery.Where(obj =>
                            obj.ArticleSubject.Contains(param.Keywords)
                            || obj.ArticleContext.Contains(param.Keywords));
                    }

                    // total
                    param.PagedParams.RowCount = tempQuery.Count();

                    // sorting
                    if (param.PagedParams.SortField != "")
                    {
                        tempQuery = tempQuery.OrderBy(param.PagedParams.SortField, param.PagedParams.IsSortDesc);
                    }
                    else
                    {
                        // default
                        tempQuery = tempQuery.OrderByDescending(obj => obj.PublishDate);
                    }

                    // paging
                    if (skipCount > 0)
                    {
                        tempQuery = tempQuery.Skip(skipCount);
                    }

                    if (takeCount >= 0)
                    {
                        tempQuery = tempQuery.Take(takeCount);
                    }

                    // result
                    entities = tempQuery.ToList();
                }
                else
                {
                    // 多項關聯字查詢

                    List<string> kws = param.Keywords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim()).ToList();

                    // Query conditions
                    System.Linq.Expressions.Expression<Func<SearchDataSourceForFrontend, bool>> whereClause = null;

                    foreach (string kw in kws)
                    {
                        if (kw != "")
                        {
                            System.Linq.Expressions.Expression<Func<SearchDataSourceForFrontend, bool>> tempWhere = obj =>
                                obj.ArticleSubject.Contains(kw)
                                || obj.ArticleContext.Contains(kw);

                            if (whereClause == null)
                            {
                                whereClause = tempWhere;
                            }
                            else
                            {
                                whereClause = whereClause.Or(tempWhere);
                            }
                        }
                    }

                    if (whereClause != null)
                    {
                        tempQuery = tempQuery.Where(whereClause);
                    }

                    // get data
                    entities = tempQuery.ToList();

                    // total
                    param.PagedParams.RowCount = entities.Count;

                    // 記錄符合的關鍵字數量
                    foreach (string kw in kws)
                    {
                        foreach (var entity in entities)
                        {
                            if (entity.ArticleSubject.IndexOf(kw, StringComparison.CurrentCultureIgnoreCase) >= 0
                                || entity.ArticleContext.IndexOf(kw, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                entity.MatchesTotal++;
                            }
                        }
                    }

                    // sorting
                    entities = entities.OrderByDescending(obj => obj.MatchesTotal)
                        .ThenByDescending(obj => obj.PublishDate)
                        .ToList();

                    // paging
                    if (skipCount > 0)
                    {
                        entities = entities.Skip(skipCount).ToList();
                    }

                    if (takeCount >= 0)
                    {
                        entities = entities.Take(takeCount).ToList();
                    }
                }

                int rowNum = 1;

                foreach (var entity in entities)
                {
                    entity.RowNum = skipCount + rowNum;
                    rowNum++;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return null;
            }

            return entities;
        }

        #endregion

        #region SQLInjectionFilter

        /// <summary>
        /// 測試運算式是否成立,能否被用來做 SQL Injection
        /// </summary>
        public bool IsSQLInjectionExpr(string expr)
        {
            Logger.Debug("IsSQLInjectionExpr(expr)");
            bool result = false;

            try
            {
                System.Data.Entity.Core.Objects.ObjectResult<bool?> objResults = cmsCtx.spIsSQLInjectionExpr(expr);
                result = objResults.First().Value;
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return result;
        }

        #endregion

        #region msdb

        /// <summary>
        /// 指示 SQL Server Agent 立即執行作業
        /// </summary>
        public bool CallSqlServerAgentJob(string jobName)
        {
            Logger.Debug("CallSqlServerAgentJob(jobName)");

            try
            {
                int rc = cmsCtx.spStartJob(jobName);
                Logger.InfoFormat("executed sp_start_job '{0}', return:{1} ", jobName, rc);
            }
            catch (Exception ex)
            {
                Logger.Error("", ex);
                errMsg = ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Custom database function

        // reference: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/how-to-call-custom-database-functions

        [DbFunction("CmsModel.Store", "fnArticle_IsShowInLang")]
        private bool fnArticle_IsShowInLang(Guid ArticleId, string CultureName)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        [DbFunction("CmsModel.Store", "fnAttachFile_IsShowInLang")]
        private bool fnAttachFile_IsShowInLang(Guid AttId, string CultureName)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        [DbFunction("CmsModel.Store", "fnArticlePicture_IsShowInLang")]
        private bool fnArticlePicture_IsShowInLang(Guid PicId, string CultureName)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        [DbFunction("CmsModel.Store", "fnArticleVideo_IsShowInLang")]
        private bool fnArticleVideo_IsShowInLang(Guid VidId, string CultureName)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        #endregion
    }
}
