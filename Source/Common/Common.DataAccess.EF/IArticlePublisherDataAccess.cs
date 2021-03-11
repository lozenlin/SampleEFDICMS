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
using Common.Data.Domain.QueryParam;
using System;
using System.Collections.Generic;

namespace Common.DataAccess.EF
{
    public interface IArticlePublisherDataAccess : IDataAccessBase
    {
        bool BuildSearchDataSource(string mainLinkUrl);
        bool CallSqlServerAgentJob(string jobName);
        bool DecreaseArticleSortNo(Guid articleId, string mdfAccount);
        bool DecreaseAttachFileSortNo(Guid attId, string mdfAccount);
        bool DeleteArticleData(Guid articleId);
        bool DeleteArticlePictureData(Guid picId);
        bool DeleteArticleVideoData(Guid vidId);
        bool DeleteAttachFileData(Guid attId);
        ArticleForBackend GetArticleDataForBackend(Guid articleId);
        ArticleForFrontend GetArticleDataForFrontend(Guid articleId, string cultureName);
        List<ArticleDescendant> GetArticleDescendants(Guid articleId);
        Guid? GetArticleIdByAlias(string articleAlias);
        Guid? GetArticleIdByLinkUrl(string linkUrl);
        int GetArticleMaxSortNo(Guid parentId);
        List<ArticleMultiLangLevelInfo> GetArticleMultiLangLevelInfoList(Guid articleId, string cultureName);
        List<ArticleForBEList> GetArticleMultiLangListForBackend(ArticleListQueryParams param);
        List<ArticleMultiLangForOpMenu> GetArticleMultiLangListForOpMenu(Guid parentId, string cultureName);
        List<ArticlePictureForFrontend> GetArticlePictureListForFrontend(Guid articleId, string cultureName);
        int GetArticlePictureMaxSortNo(Guid? articleId);
        List<ArticlePictureForBEList> GetArticlePictureMultiLangListForBackend(ArticlePictureListQueryParams param);
        ArticleTopLevelIds GetArticleTopLevelIds(Guid articleId);
        List<ArticleForFEList> GetArticleValidListForFrontend(ArticleValidListQueryParams param);
        List<ArticleForFESideSection> GetArticleValidListForSideSection(Guid parentId, string cultureName);
        List<ArticleForFESitemap> GetArticleValidListForSitemap(Guid parentId, string cultureName);
        List<ArticleForFEUnitArea> GetArticleValidListForUnitArea(Guid parentId, string cultureName, bool isShowInUnitArea);
        List<ArticleVideoForFrontend> GetArticleVideoListForFrontend(Guid articleId, string cultureName);
        int GetArticleVideoMaxSortNo(Guid articleId);
        List<ArticleVideoForBEList> GetArticleVideoMultiLangListForBackend(ArticleVideoListQueryParams param);
        List<AttachFileForFrontend> GetAttachFileListForFrontend(Guid articleId, string cultureName);
        int GetAttachFileMaxSortNo(Guid? articleId);
        List<AttachFileForBEList> GetAttachFileMultiLangListForBackend(AttachFileListQueryParams param);
        List<Keyword> GetKeywordListForFrontend(string cultureName, string kw, int topCount);
        List<SearchDataSourceForFrontend> GetSearchDataSourceList(SearchResultListQueryParams param);
        bool IncreaseArticleSortNo(Guid articleId, string mdfAccount);
        bool IncreaseAttachFileSortNo(Guid attId, string mdfAccount);
        InsertResult InsertArticleData(Article entity);
        bool IsSQLInjectionExpr(string expr);
        bool SaveKeywordData(string cultureName, string kw);
        bool UpdateArticleData(Article entity);
        bool UpdateArticleIsAreaShowInFrontStage(ArticleUpdateIsAreaShowInFrontStageParams param);
        bool UpdateArticleSortFieldOfFrontStage(ArticleUpdateSortFieldOfFrontStageParams param);
    }
}