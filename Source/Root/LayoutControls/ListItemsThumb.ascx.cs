using Common.Data.Domain.Model;
using Common.Data.Domain.QueryParam;
using Common.LogicObject;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class LayoutControls_ListItemsThumb : System.Web.UI.UserControl
{
    [Dependency]
    public ArticlePublisherLogic ArticlePublisherLogicIn { get; set; }
    [Dependency]
    public FrontendPageCommon FrontendPageCommonIn { get; set; }

    protected FrontendPageCommon c;
    protected ArticlePublisherLogic artPub;
    protected FrontendBasePage basePage;
    protected ArticleData articleData;
    protected IMasterArticleSettings masterSettings;

    private bool isLazyLoadingMode = true;   //滾動加載模式

    protected void Page_Init(object sender, EventArgs e)
    {
        basePage = (FrontendBasePage)this.Page;
        articleData = basePage.GetArticleData();
        masterSettings = (IMasterArticleSettings)this.Page.Master;

        ucDataPager.MaxItemCountOfPage = 10;
        ucDataPager.MaxDisplayCountInPageCodeArea = 5;
        ucDataPager.LinkUrlToReload = string.Concat(Request.AppRelativeCurrentExecutionFilePath, "?", Request.QueryString);
        ucDataPager.Initialize(0, 0);

        if (articleData.IsPreviewMode)
        {
            isLazyLoadingMode = false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ArticlePublisherLogicIn == null)
            throw new ArgumentException("ArticlePublisherLogicIn");

        if (FrontendPageCommonIn == null)
            throw new ArgumentException("FrontendPageCommonIn");

        this.artPub = ArticlePublisherLogicIn;
        this.c = FrontendPageCommonIn;
        c.InitialLoggerOfUI(this.GetType());

        if (!IsPostBack)
        {
            if (isLazyLoadingMode)
            {
                LazyLoadingArea.Visible = true;
                LazyLoadingCtrlArea.Visible = true;
                ucDataPager.Visible = false;
            }
            else
            {
                DisplaySubitems();
            }
        }
    }

    private void DisplaySubitems()
    {
        ArticleValidListQueryParams param = new ArticleValidListQueryParams()
        {
            ParentId = articleData.ArticleId.Value,
            CultureName = c.qsCultureNameOfLangNo,
            Kw = ""
        };

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = 0,
            EndNum = 0,
            SortField = articleData.SortFieldOfFrontStage,
            IsSortDesc = articleData.IsSortDescOfFrontStage
        };

        // get total of items
        artPub.GetArticleValidListForFrontend(param);

        // update pager and get begin end of item numbers
        int itemTotalCount = param.PagedParams.RowCount;
        ucDataPager.Initialize(itemTotalCount, c.qsPageCode);

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = ucDataPager.BeginItemNumberOfPage,
            EndNum = ucDataPager.EndItemNumberOfPage,
            SortField = articleData.SortFieldOfFrontStage,
            IsSortDesc = articleData.IsSortDescOfFrontStage
        };

        List<ArticleForFEList> subitems = artPub.GetArticleValidListForFrontend(param);

        if (subitems != null)
        {
            rptSubitems.DataSource = subitems;
            rptSubitems.DataBind();
        }
    }

    protected void rptSubitems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        ArticleForFEList artData = (ArticleForFEList)e.Item.DataItem;

        Guid articleId = artData.ArticleId;
        string articleSubject = artData.ArticleSubject;
        int showTypeId = artData.ShowTypeId.Value;
        string linkUrl = artData.LinkUrl;
        string linkTarget = artData.LinkTarget ?? "";
        string destUrl = "/" + StringUtility.GetLinkUrlOfShowType(articleId, c.qsLangNo, showTypeId, linkUrl);

        HtmlAnchor btnItem = (HtmlAnchor)e.Item.FindControl("btnItem");
        btnItem.HRef = destUrl;
        btnItem.Title = articleSubject;

        HtmlAnchor btnPic = (HtmlAnchor)e.Item.FindControl("btnPic");
        btnPic.HRef = destUrl;
        btnPic.Title = articleSubject;

        if (!string.IsNullOrEmpty(linkTarget))
        {
            btnItem.Target = linkTarget;
            btnPic.Target = linkTarget;
        }

        // get thumb picture
        List<ArticlePictureForFrontend> pictures = artPub.GetArticlePictureListForFrontend(articleId, c.qsCultureNameOfLangNo);

        if (pictures != null && pictures.Count > 0)
        {
            ArticlePictureForFrontend artPic = pictures[0];

            Guid picId = artPic.PicId;
            string picSubject = artPic.PicSubject;

            HtmlImage imgPic = (HtmlImage)e.Item.FindControl("imgPic");
            imgPic.Src = string.Format("/FileArtPic.ashx?attid={0}&w=640&h=480&l={1}", picId, c.qsLangNo);
            imgPic.Alt = picSubject;
        }
    }
}