using Common.Data.Domain.Model;
using Common.Data.Domain.QueryParam;
using Common.LogicObject;
using Common.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Article_Node : BasePage
{
    [Dependency]
    public ArticleCommonOfBackend ArticleCommonOfBackendIn { get; set; }
    [Dependency]
    public ArticlePublisherLogic ArticlePublisherLogicIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    protected ArticleCommonOfBackend c;
    protected ArticlePublisherLogic artPub;
    protected EmployeeAuthorityLogic empAuth;
    private IHeadUpDisplay hud = null;
    private int totalSubitems = 0;
    private int totalAttachFiles = 0;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (ArticleCommonOfBackendIn == null)
            throw new ArgumentException("ArticleCommonOfBackendIn");

        if (ArticlePublisherLogicIn == null)
            throw new ArgumentException("ArticlePublisherLogicIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        this.c = ArticleCommonOfBackendIn;
        c.InitialLoggerOfUI(this.GetType());
        c.SelectMenuItemToThisPage();

        this.artPub = ArticlePublisherLogicIn;
        artPub.SetAuthenticationConditionProvider(c);

        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);
        empAuth.SetCustomEmployeeAuthorizationResult(artPub);
        empAuth.InitialAuthorizationResultOfTopPage();

        hud = Master.GetHeadUpDisplay();
        isBackendPage = true;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        ucDataPager.MaxItemCountOfPage = 20;
        ucDataPager.MaxDisplayCountInPageCodeArea = 5;
        ucDataPager.LinkUrlToReload = string.Concat(Request.AppRelativeCurrentExecutionFilePath, "?", Request.QueryString);
        ucDataPager.Initialize(0, 0);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RebuildBreadcrumbAndHeadOfHUD();
        Title = hud.GetHeadText() + " - " + Title;

        if (!IsPostBack)
        {
            if (!empAuth.CanOpenThisPage())
            {
                Response.Redirect(c.BACK_END_HOME);
            }

            LoadUIData();
            DisplayArticle();
            DisplayAttachFiles();
            DisplayPictures();
            DisplayVideos();
        }
        else
        {
            //PostBack
            DisplayArticleData();

            if (Master.FlagValue != "")
            {
                // message from config-form

                if (Master.FlagValue == "Config")
                {
                    DisplaySubitems();
                    Master.RefreshOpMenu(empAuth, artPub, c);
                }
                else if (Master.FlagValue == "Attach")
                {
                    DisplayAttachFiles();
                }
                else if (Master.FlagValue == "Picture")
                {
                    DisplayPictures();
                }
                else if (Master.FlagValue == "Video")
                {
                    DisplayVideos();
                }

                Master.FlagValue = "";
            }
        }

        LoadSwitchButtonsUIData();
    }

    private void RebuildBreadcrumbAndHeadOfHUD()
    {
        StringBuilder sbBreadcrumbWoHome = new StringBuilder(100);

        List<ArticleMultiLangLevelInfo> levelInfos = artPub.GetArticleMultiLangLevelInfo(c.qsArtId, c.seCultureNameOfBackend);

        if (levelInfos != null)
        {
            int total = levelInfos.Count    ;

            for (int itemNum = total; itemNum >= 1; itemNum--)
            {
                ArticleMultiLangLevelInfo levelInfo = levelInfos[itemNum - 1];

                Guid articleId = levelInfo.ArticleId;
                string articleSubject = levelInfo.ArticleSubject;
                int articleLevelNo = levelInfo.ArticleLevelNo.Value;
                string url = string.Format("Article-Node.aspx?artid={0}", articleId);

                if (itemNum == 1)
                {
                    sbBreadcrumbWoHome.Append(hud.GetBreadcrumbTextItemHtml(articleSubject));
                    // update head of HUD
                    hud.SetHeadText(articleSubject);

                    // get icon of operation
                    OperationHtmlAnchorData anchorData = empAuth.GetOperationHtmlAnchorData(c.GetOpIdOfPage(), false);

                    if (anchorData != null && !string.IsNullOrEmpty(anchorData.IconImageFileUrl))
                    {
                        string iconImageFile = "~/BPImages/icon/" + anchorData.IconImageFileUrl;
                        hud.SetHeadIconImageUrl(iconImageFile);
                    }
                }
                else
                {
                    sbBreadcrumbWoHome.Append(hud.GetBreadcrumbLinkItemHtml(articleSubject, articleSubject, url));

                    if (itemNum == 2)
                    {
                        // set url of BackToParent button
                        string backToParentUrl = "~/" + c.BuildUrlOfListPage(articleId,
                            c.qsKwOfParent, c.qsSortField, c.qsIsSortDesc,
                            StringUtility.GetLastNumOfParents(c.qsPageCodeOfParents), StringUtility.GetNumOfParentsForParent(c.qsPageCodeOfParents), "");

                        hud.SetButtonAttribute(HudButtonNameEnum.BackToParent, HudButtonAttributeEnum.NavigateUrl, backToParentUrl);
                    }
                }
            }
        }

        hud.RebuildBreadcrumb(sbBreadcrumbWoHome.ToString(), true);
    }

    private void LoadUIData()
    {
        btnShowTypeLinkUrl.InnerHtml = Resources.Lang.PageShowTypeOption_URL;
        btnSearch.ToolTip = Resources.Lang.SearchPanel_btnSearch_Hint;
        btnClear.ToolTip = Resources.Lang.SearchPanel_btnClear_Hint;

        //HUD
        if (empAuth.CanEditThisPage())
        {
            hud.SetButtonVisible(HudButtonNameEnum.Edit, true);
            hud.SetButtonAttribute(HudButtonNameEnum.Edit, HudButtonAttributeEnum.JsInNavigateUrl,
                string.Format("popWin('Article-Config.aspx?act={0}&artid={1}', 700, 600);", ConfigFormAction.edit, c.qsArtId));
        }

        if (empAuth.CanAddSubItemInThisPage())
        {
            hud.SetButtonVisible(HudButtonNameEnum.AddNew, true);
            hud.SetButtonAttribute(HudButtonNameEnum.AddNew, HudButtonAttributeEnum.JsInNavigateUrl,
                string.Format("popWin('Article-Config.aspx?act={0}&artid={1}', 700, 600);", ConfigFormAction.add, c.qsArtId));
        }

        //conditions UI

        //condition vlaues
        txtKw.Text = c.qsKw;

        //columns of list
        btnSortArticleSubject.Text = Resources.Lang.Col_Subject;
        hidSortArticleSubject.Text = btnSortArticleSubject.Text;
        btnSortSortNo.Text = Resources.Lang.Col_SortNo;
        hidSortSortNo.Text = btnSortSortNo.Text;
        btnSortStartDate.Text = Resources.Lang.Col_ValidationDate;
        hidSortStartDate.Text = btnSortStartDate.Text;
        btnSortPostDeptName.Text = Resources.Lang.Col_DeptName;
        hidSortPostDeptName.Text = btnSortPostDeptName.Text;

        c.DisplaySortableCols(new string[] { 
            "ArticleSubject", "SortNo", "StartDate", 
            "PostDeptName"
        });

        LoadSortFieldOfFrontStageUIData();
        SetupLangRelatedFields();
    }

    private void LoadSwitchButtonsUIData()
    {
        btnSwitchIsListAreaShowInFrontStage.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        btnSwitchIsAttAreaShowInFrontStage.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        btnSwitchIsPicAreaShowInFrontStage.ClientIDMode = System.Web.UI.ClientIDMode.Static;
        btnSwitchIsVideoAreaShowInFrontStage.ClientIDMode = System.Web.UI.ClientIDMode.Static;

        if (!empAuth.CanEditThisPage())
        {
            btnSwitchIsListAreaShowInFrontStage.Visible = false;
            btnSwitchIsAttAreaShowInFrontStage.Visible = false;
            btnSwitchIsPicAreaShowInFrontStage.Visible = false;
            btnSwitchIsVideoAreaShowInFrontStage.Visible = false;
        }
    }

    private void LoadSortFieldOfFrontStageUIData()
    {
        ddlSortFieldOfFrontStage.Items.Clear();
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Option_Default, ""));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_ValidationDate, "StartDate"));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_SortNo, "SortNo"));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_CreateDate, "PostDate"));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_ModifyDate, "MdfDate"));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_PublishDate, "PublishDate"));
        ddlSortFieldOfFrontStage.Items.Add(new ListItem(Resources.Lang.Col_Subject, "ArticleSubject"));

        ddlIsSortDescOfFrontStage.Items.Clear();
        ddlIsSortDescOfFrontStage.Items.Add(new ListItem(Resources.Lang.Option_Default, ""));
        ddlIsSortDescOfFrontStage.Items.Add(new ListItem(Resources.Lang.IsSortDescOption_asc, "False"));
        ddlIsSortDescOfFrontStage.Items.Add(new ListItem(Resources.Lang.IsSortDescOption_desc, "True"));

        if (!empAuth.CanEditThisPage())
        {
            ddlSortFieldOfFrontStage.Visible = false;
            ddlIsSortDescOfFrontStage.Visible = false;
            ltrSortInfoOfFrontStage.Visible = true;
        }
    }

    /// <summary>
    /// 設定語系相關欄位
    /// </summary>
    private void SetupLangRelatedFields()
    {
        if (!LangManager.IsEnableEditLangZHTW())
        {
            ContextTabZhTwArea.Visible = false;
            ContextPnlZhTwArea.Visible = false;
        }

        if (!LangManager.IsEnableEditLangEN())
        {
            ContextTabEnArea.Visible = false;
            ContextPnlEnArea.Visible = false;
        }
    }

    private void DisplayArticle()
    {
        DisplayArticleData();
        DisplaySubitems();
    }

    private void SetupStatusHtmlOfArticleContentSetting(HtmlGenericControl ctlStatus, bool show)
    {
        if (show)
        {
            ctlStatus.Attributes["class"] = "status text-success";
            ctlStatus.InnerHtml = "ON";
        }
        else
        {
            ctlStatus.Attributes["class"] = "status text-muted";
            ctlStatus.InnerHtml = "OFF";
        }
    }

    private void DisplayArticleData()
    {
        btnEditContext.Title = Resources.Lang.Article_btnEditContext_Hint;
        btnEditContext.Visible = empAuth.CanEditThisPage();
        btnEditContext.Attributes["onclick"] =
            string.Format("popWin('Article-Config.aspx?act={0}&artid={1}', 700, 600);", ConfigFormAction.edit, c.qsArtId);

        ArticleForBackend article = artPub.GetArticleDataForBackend(c.qsArtId);

        if (article != null)
        {
            hidParentId.Text = article.ParentId.ToString();
            hidArticleLevelNo.Text = article.ArticleLevelNo.ToString();

            ltrValidDateRange.Text = string.Format("{0:yyyy-MM-dd} ~ {1:yyyy-MM-dd}", article.StartDate, article.EndDate);

            DateTime startDate = article.StartDate.Value;
            DateTime endDate = article.EndDate.Value;
            bool isHideSelf = article.IsHideSelf;
            int showTypeId = article.ShowTypeId.Value;
            string linkUrl = article.LinkUrl;

            switch (showTypeId)
            {
                case 1:
                    // page
                    ltrShowTypeName.Text = Resources.Lang.PageShowTypeOption_Page;
                    break;
                case 2:
                    // to sub-page
                    ltrShowTypeName.Text = Resources.Lang.PageShowTypeOption_ToSubPage;
                    break;
                case 3:
                    // URL
                    ltrShowTypeName.Text = Resources.Lang.PageShowTypeOption_URL;
                    string showTypeLinkUrl = linkUrl;

                    if (showTypeLinkUrl.StartsWith("~/"))
                    {
                        showTypeLinkUrl = showTypeLinkUrl.Replace("~", "..");
                    }

                    btnShowTypeLinkUrl.HRef = showTypeLinkUrl;
                    btnShowTypeLinkUrl.Visible = true;
                    ltrShowTypeName.Visible = false;
                    break;
                case 4:
                    // use control
                    ltrShowTypeName.Text = Resources.Lang.PageShowTypeOption_UseControl;
                    break;
            }

            // article-content-setting
            string sortFieldOfFrontStage = article.SortFieldOfFrontStage ?? "";
            bool isSortDescOfFrontStage = article.IsSortDescOfFrontStage;

            if (!string.IsNullOrEmpty(sortFieldOfFrontStage))
            {
                ddlSortFieldOfFrontStage.SelectedValue = sortFieldOfFrontStage;
                ddlIsSortDescOfFrontStage.SelectedValue = isSortDescOfFrontStage.ToString();
            }

            if (ddlSortFieldOfFrontStage.SelectedItem != null)
            {
                ltrSortInfoOfFrontStage.Text = ddlSortFieldOfFrontStage.SelectedItem.Text;
            }

            if (ddlIsSortDescOfFrontStage.SelectedItem != null)
            {
                ltrSortInfoOfFrontStage.Text += " - " + ddlIsSortDescOfFrontStage.SelectedItem.Text;
            }

            bool isListAreaShowInFrontStage = article.IsListAreaShowInFrontStage;
            hidIsListAreaShowInFrontStage.Value = isListAreaShowInFrontStage.ToString();
            SetupStatusHtmlOfArticleContentSetting(ctlIsListAreaShowInFrontStageStatus, isListAreaShowInFrontStage);

            bool isAttAreaShowInFrontStage = article.IsAttAreaShowInFrontStage;
            hidIsAttAreaShowInFrontStage.Value = isAttAreaShowInFrontStage.ToString();
            SetupStatusHtmlOfArticleContentSetting(ctlIsAttAreaShowInFrontStageStatus, isAttAreaShowInFrontStage);

            bool isPicAreaShowInFrontStage = article.IsPicAreaShowInFrontStage;
            hidIsPicAreaShowInFrontStage.Value = isPicAreaShowInFrontStage.ToString();
            SetupStatusHtmlOfArticleContentSetting(ctlIsPicAreaShowInFrontStageStatus, isPicAreaShowInFrontStage);

            bool isVideoAreaShowInFrontStage = article.IsVideoAreaShowInFrontStage;
            hidIsVideoAreaShowInFrontStage.Value = isVideoAreaShowInFrontStage.ToString();
            SetupStatusHtmlOfArticleContentSetting(ctlIsVideoAreaShowInFrontStageStatus, isVideoAreaShowInFrontStage);

            ArticleMultiLang articleMultiLang = artPub.GetArticleMultiLangDataForBackend(c.qsArtId, c.seCultureNameOfBackend);

            if (articleMultiLang != null)
            {
                string mdfAccount;
                DateTime mdfDate;

                if (!articleMultiLang.MdfDate.HasValue)
                {
                    mdfAccount = articleMultiLang.PostAccount;
                    mdfDate = articleMultiLang.PostDate.Value;
                }
                else
                {
                    mdfAccount = articleMultiLang.MdfAccount;
                    mdfDate = articleMultiLang.MdfDate.Value;
                }

                ltrMdfName.Text = mdfAccount;
                ltrMdfDate.Text = mdfDate.ToString("yyyy-MM-dd");
            }

            //zh-TW
            if (LangManager.IsEnableEditLangZHTW())
            {
                ArticleMultiLang articleZhTw = artPub.GetArticleMultiLangDataForBackend(c.qsArtId, LangManager.CultureNameZHTW);

                if (articleZhTw != null)
                {
                    ltrContextZhTw.Text = Microsoft.Security.Application.AntiXss.GetSafeHtmlFragment(articleZhTw.ArticleContext);

                    bool isShowInLang = articleZhTw.IsShowInLang;
                    string url = "";
                    string websiteUrl = ConfigurationManager.AppSettings["WebsiteUrl"];

                    if (startDate <= DateTime.Today && DateTime.Today <= endDate
                        && !isHideSelf
                        && isShowInLang)
                    {
                        // view
                        url = StringUtility.GetLinkUrlOfShowType(c.qsArtId, 1, showTypeId, linkUrl);

                        if (!url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase)
                            && !url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = websiteUrl + "/" + url;
                        }

                        hud.SetButtonAttribute(HudButtonNameEnum.ViewZhTw, HudButtonAttributeEnum.NavigateUrl, url);
                        hud.SetButtonVisible(HudButtonNameEnum.ViewZhTw, true);

                        hud.SetButtonVisible(HudButtonNameEnum.PreviewZhTw, false);
                    }
                    else
                    {
                        // preview
                        url = StringUtility.GetLinkUrlOfShowType(c.qsArtId, 1, showTypeId, linkUrl) + "&preview=1";

                        if (!url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase)
                            && !url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = websiteUrl + "/" + url;
                        }

                        hud.SetButtonAttribute(HudButtonNameEnum.PreviewZhTw, HudButtonAttributeEnum.NavigateUrl, url);
                        hud.SetButtonVisible(HudButtonNameEnum.PreviewZhTw, true);

                        hud.SetButtonVisible(HudButtonNameEnum.ViewZhTw, false);
                    }
                }
            }

            //en
            if (LangManager.IsEnableEditLangEN())
            {
                ArticleMultiLang articleEn = artPub.GetArticleMultiLangDataForBackend(c.qsArtId, LangManager.CultureNameEN);

                if (articleEn != null)
                {
                    ltrContextEn.Text = Microsoft.Security.Application.AntiXss.GetSafeHtmlFragment(articleEn.ArticleContext);

                    bool isShowInLang = articleEn.IsShowInLang;
                    string url = "";
                    string websiteUrl = ConfigurationManager.AppSettings["WebsiteUrl"];

                    if (startDate <= DateTime.Today && DateTime.Today <= endDate
                        && !isHideSelf
                        && isShowInLang)
                    {
                        // view
                        url = StringUtility.GetLinkUrlOfShowType(c.qsArtId, 2, showTypeId, linkUrl);

                        if (!url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase)
                            && !url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = websiteUrl + "/" + url;
                        }

                        hud.SetButtonAttribute(HudButtonNameEnum.ViewEn, HudButtonAttributeEnum.NavigateUrl, url);
                        hud.SetButtonVisible(HudButtonNameEnum.ViewEn, true);

                        hud.SetButtonVisible(HudButtonNameEnum.PreviewEn, false);
                    }
                    else
                    {
                        // preview
                        url = StringUtility.GetLinkUrlOfShowType(c.qsArtId, 2, showTypeId, linkUrl) + "&preview=1";

                        if (!url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase)
                            && !url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase))
                        {
                            url = websiteUrl + "/" + url;
                        }

                        hud.SetButtonAttribute(HudButtonNameEnum.PreviewEn, HudButtonAttributeEnum.NavigateUrl, url);
                        hud.SetButtonVisible(HudButtonNameEnum.PreviewEn, true);

                        hud.SetButtonVisible(HudButtonNameEnum.ViewEn, false);
                    }
                }
            }
        }
    }

    private void DisplaySubitems()
    {
        ArticleListQueryParams param = new ArticleListQueryParams()
        {
            ParentId = c.qsArtId,
            CultureName = c.seCultureNameOfBackend,
            Kw = c.qsKw
        };

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = 0,
            EndNum = 0,
            SortField = c.qsSortField,
            IsSortDesc = c.qsIsSortDesc
        };

        param.AuthParams = new AuthenticationQueryParams()
        {
            CanReadSubItemOfOthers = empAuth.CanReadSubItemOfOthers(),
            CanReadSubItemOfCrew = empAuth.CanReadSubItemOfCrew(),
            CanReadSubItemOfSelf = empAuth.CanReadSubItemOfSelf(),
            MyAccount = c.GetEmpAccount(),
            MyDeptId = c.GetDeptId()
        };

        // get total of items
        artPub.GetArticleMultiLangListForBackend(param);

        // update pager and get begin end of item numbers
        int itemTotalCount = param.PagedParams.RowCount;
        ucDataPager.Initialize(itemTotalCount, c.qsPageCode);
        if (IsPostBack)
            ucDataPager.RefreshPagerAfterPostBack();

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = ucDataPager.BeginItemNumberOfPage,
            EndNum = ucDataPager.EndItemNumberOfPage,
            SortField = c.qsSortField,
            IsSortDesc = c.qsIsSortDesc
        };

        List<ArticleForBEList> subitems = artPub.GetArticleMultiLangListForBackend(param);

        if (subitems != null)
        {
            totalSubitems = subitems.Count;
            rptSubitems.DataSource = subitems;
            rptSubitems.DataBind();
        }

        if (c.qsPageCode > 1 || c.qsSortField != "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "isSearchPanelCollapsingAtBeginning", "isSearchPanelCollapsingAtBeginning = true;", true);
        }
    }

    protected void rptSubitems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ArticleForBEList artData = (ArticleForBEList)e.Item.DataItem;

        Guid articleId = artData.ArticleId;
        string articleSubject = artData.ArticleSubject;
        bool isShowInLangZhTw = artData.IsShowInLangZhTw;
        bool isShowInLangEn = artData.IsShowInLangEn;
        bool isHideSelf = artData.IsHideSelf;
        DateTime startDate = artData.StartDate.Value;
        DateTime endDate = artData.EndDate.Value;
        bool dontDelete = artData.DontDelete;

        LinkButton btnMoveDown = (LinkButton)e.Item.FindControl("btnMoveDown");
        btnMoveDown.ToolTip = Resources.Lang.btnMoveDown;

        LinkButton btnMoveUp = (LinkButton)e.Item.FindControl("btnMoveUp");
        btnMoveUp.ToolTip = Resources.Lang.btnMoveUp;

        int itemNum = e.Item.ItemIndex + 1;

        if (itemNum == 1)
        {
            btnMoveUp.Visible = false;
        }

        if (itemNum == totalSubitems)
        {
            btnMoveDown.Visible = false;
        }

        if (c.qsSortField != "")
        {
            btnMoveUp.Visible = false;
            btnMoveDown.Visible = false;
        }

        HtmlAnchor btnItem = (HtmlAnchor)e.Item.FindControl("btnItem");
        btnItem.InnerHtml = articleSubject;
        btnItem.Title = articleSubject;
        btnItem.HRef = c.BuildUrlOfListPage(articleId,
            "", c.qsSortField, c.qsIsSortDesc,
            1, StringUtility.GetNumOfParentsForChild(c.qsPageCode, c.qsPageCodeOfParents), c.qsKw);

        HtmlGenericControl ctlIsShowInLangZhTw = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangZhTw");
        ctlIsShowInLangZhTw.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangZhTw);
        ctlIsShowInLangZhTw.Visible = LangManager.IsEnableEditLangZHTW();

        HtmlGenericControl ctlIsShowInLangEn = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangEn");
        ctlIsShowInLangEn.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangEn);
        ctlIsShowInLangEn.Visible = LangManager.IsEnableEditLangEN();

        HtmlTableRow ItemArea = (HtmlTableRow)e.Item.FindControl("ItemArea");

        if (isHideSelf)
        {
            ItemArea.Attributes["class"] = "table-danger";
        }

        HtmlGenericControl ctlArticleState = (HtmlGenericControl)e.Item.FindControl("ctlArticleState");

        if (DateTime.Today < startDate)
        {
            // on schedule
            ctlArticleState.Attributes["class"] = "fa fa-hourglass-start fa-lg text-info";
            ctlArticleState.Attributes["title"] = Resources.Lang.Status_OnSchedule;
        }
        else if (endDate < DateTime.Today)
        {
            // offline
            ctlArticleState.Attributes["class"] = "fa fa-ban fa-lg text-danger";
            ctlArticleState.Attributes["title"] = Resources.Lang.Status_AccessDeniedOrExpired;
            ItemArea.Attributes["class"] = "table-danger";
        }
        else
        {
            // online
            ctlArticleState.Attributes["title"] = Resources.Lang.Status_Normal;
        }

        Literal ltrValidDateRange = (Literal)e.Item.FindControl("ltrValidDateRange");
        ltrValidDateRange.Text = string.Format("{0:yyyy-MM-dd} ~ {1:yyyy-MM-dd}", startDate, endDate);

        HtmlAnchor btnEdit = (HtmlAnchor)e.Item.FindControl("btnEdit");
        btnEdit.Attributes["onclick"] = string.Format("popWin('Article-Config.aspx?act={0}&artid={1}', 700, 600); return false;", ConfigFormAction.edit, articleId);
        btnEdit.Title = Resources.Lang.Main_btnEdit_Hint;

        Literal ltrEdit = (Literal)e.Item.FindControl("ltrEdit");
        ltrEdit.Text = Resources.Lang.Main_btnEdit;

        LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDelete");
        btnDelete.CommandArgument = string.Join(",", articleId.ToString(), articleSubject);
        btnDelete.Text = "<i class='fa fa-trash-o'></i> " + Resources.Lang.Main_btnDelete;
        btnDelete.ToolTip = Resources.Lang.Main_btnDelete_Hint;
        btnDelete.OnClientClick = string.Format("return confirm('" + Resources.Lang.Article_ConfirmDelete_Format + "');",
            articleSubject, articleId);

        if (dontDelete)
        {
            HtmlGenericControl ctlDontDelete = (HtmlGenericControl)e.Item.FindControl("ctlDontDelete");
            ctlDontDelete.Attributes["title"] = Resources.Lang.Status_DontDelete;
            ctlDontDelete.Visible = true;
            btnDelete.Visible = false;
        }

        string ownerAccount = artData.PostAccount;
        int ownerDeptId = artData.PostDeptId;

        if (!empAuth.CanEditThisPage(false, ownerAccount, ownerDeptId))
        {
            btnMoveDown.Visible = false;
            btnMoveUp.Visible = false;
            btnEdit.Visible = false;
        }

        if (!empAuth.CanDelThisPage(ownerAccount, ownerDeptId))
        {
            btnDelete.Visible = false;
        }

    }

    protected void rptSubitems_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        bool result = false;
        Guid articleId;

        switch (e.CommandName)
        {
            case "Del":
                string[] args = e.CommandArgument.ToString().Split(',');
                articleId = new Guid(args[0]);
                string articleSubject = args[1];

                //取得所有子項目id, 階層越高排越前面
                List<ArticleDescendant> descsAndSelf = artPub.GetArticleDescendants(articleId);

                if (descsAndSelf != null && descsAndSelf.Count > 1)
                {
                    // delete descendants
                    foreach (ArticleDescendant desc in descsAndSelf)
                    {
                        Guid descId = desc.ArticleId;

                        if (descId == articleId)
                            continue;

                        bool descResult = artPub.DeleteArticleData(descId);

                        //新增後端操作記錄
                        empAuth.InsertBackEndLogData(new BackEndLogData()
                        {
                            EmpAccount = c.GetEmpAccount(),
                            Description = string.Format("．刪除子網頁內容/Delete descendant of article　．代碼/id[{0}]　根代碼/root id[{1}]　結果/result[{2}]", descId, articleId, descResult),
                            IP = c.GetClientIP()
                        });

                        // log to file
                        c.LoggerOfUI.InfoFormat("{0} deletes descendant-[{1}] of article-[{2}], result: {3}",
                            c.GetEmpAccount(), descId.ToString(), articleId.ToString(), descResult);

                        if (!descResult)
                        {
                            Master.ShowErrorMsg(Resources.Lang.ErrMsg_DeleteArticleFailed);
                            return;
                        }
                    }
                }

                result = artPub.DeleteArticleData(articleId);

                //新增後端操作記錄
                empAuth.InsertBackEndLogData(new BackEndLogData()
                {
                    EmpAccount = c.GetEmpAccount(),
                    Description = string.Format("．刪除網頁內容/Delete article　．代碼/id[{0}]　標題/subject[{1}]　結果/result[{2}]", articleId, articleSubject, result),
                    IP = c.GetClientIP()
                });

                // log to file
                c.LoggerOfUI.InfoFormat("{0} deletes {1}, result: {2}", c.GetEmpAccount(), "article-[" + articleId.ToString() + "]-" + articleSubject, result);

                if (!result)
                {
                    Master.ShowErrorMsg(Resources.Lang.ErrMsg_DeleteArticleFailed);
                }

                break;
            case "MoveUp":
                articleId = new Guid(e.CommandArgument.ToString());
                result = artPub.DecreaseArticleSortNo(articleId, c.GetEmpAccount());
                break;
            case "MoveDown":
                articleId = new Guid(e.CommandArgument.ToString());
                result = artPub.IncreaseArticleSortNo(articleId, c.GetEmpAccount());
                break;
        }

        if (result)
        {
            DisplayArticle();
            Master.RefreshOpMenu(empAuth, artPub, c);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        txtKw.Text = txtKw.Text.Trim();

        Response.Redirect(c.BuildUrlOfListPage(c.qsArtId,
            txtKw.Text, "", false,
            1, c.qsPageCodeOfParents, c.qsKwOfParent));
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        Response.Redirect(c.BuildUrlOfListPage(c.qsArtId,
            "", "", false,
            1, c.qsPageCodeOfParents, c.qsKwOfParent));
    }

    protected void btnSort_Click(object sender, EventArgs e)
    {
        LinkButton btnSort = (LinkButton)sender;
        string sortField = btnSort.CommandArgument;
        bool isSortDesc = false;
        c.ChangeSortStateToNext(ref sortField, out isSortDesc);

        //重新載入頁面
        Response.Redirect(c.BuildUrlOfListPage(c.qsArtId,
            c.qsKw, sortField, isSortDesc,
            c.qsPageCode, c.qsPageCodeOfParents, c.qsKwOfParent));
    }

    private void DisplayAttachFiles()
    {
        ltrUploadAttachFile.Text = Resources.Lang.Article_btnUploadAttachFile;
        btnUploadAttachFile.Title = Resources.Lang.Article_btnUploadAttachFile_Hint;
        btnUploadAttachFile.Attributes["onclick"] = 
            string.Format("popWin('Article-Attach.aspx?act={0}&artid={1}', 700, 600); return false;", 
                ConfigFormAction.add, c.qsArtId);
        btnUploadAttachFile.Visible = empAuth.CanEditThisPage();

        AttachFileListQueryParams param = new AttachFileListQueryParams()
        {
            ArticleId = c.qsArtId,
            CultureName = c.seCultureNameOfBackend,
            Kw = ""
        };

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = 1,
            EndNum = 999999999,
            SortField = "",
            IsSortDesc = false
        };

        param.AuthParams = new AuthenticationQueryParams()
        {
            CanReadSubItemOfOthers = empAuth.CanReadSubItemOfOthers(),
            CanReadSubItemOfCrew = empAuth.CanReadSubItemOfCrew(),
            CanReadSubItemOfSelf = empAuth.CanReadSubItemOfSelf(),
            MyAccount = c.GetEmpAccount(),
            MyDeptId = c.GetDeptId()
        };

        List<AttachFileForBEList> attachFiles = artPub.GetAttachFileMultiLangListForBackend(param);

        if (attachFiles != null)
        {
            totalAttachFiles = attachFiles.Count;
            rptAttachFiles.DataSource = attachFiles;
            rptAttachFiles.DataBind();
        }
    }

    protected void rptAttachFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        AttachFileForBEList attData = (AttachFileForBEList)e.Item.DataItem;

        Guid attId = attData.AttId;
        string attSubject = attData.AttSubject;
        bool isShowInLangZhTw = attData.IsShowInLangZhTw;
        bool isShowInLangEn = attData.IsShowInLangEn;
        bool dontDelete = attData.DontDelete;
        string mdfAccount = "";
        DateTime mdfDate = DateTime.MinValue;

        if (!attData.MdfDate.HasValue)
        {
            mdfAccount = attData.PostAccount;
            mdfDate = attData.PostDate.Value;
        }
        else
        {
            mdfAccount = attData.MdfAccount;
            mdfDate = attData.MdfDate.Value;
        }
        
        LinkButton btnMoveDown = (LinkButton)e.Item.FindControl("btnMoveDown");
        btnMoveDown.ToolTip = Resources.Lang.btnMoveDown;

        LinkButton btnMoveUp = (LinkButton)e.Item.FindControl("btnMoveUp");
        btnMoveUp.ToolTip = Resources.Lang.btnMoveUp;

        int itemNum = e.Item.ItemIndex + 1;

        if (itemNum == 1)
        {
            btnMoveUp.Visible = false;
        }

        if (itemNum == totalAttachFiles)
        {
            btnMoveDown.Visible = false;
        }

        HtmlGenericControl ctlIsShowInLangZhTw = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangZhTw");
        ctlIsShowInLangZhTw.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangZhTw);
        ctlIsShowInLangZhTw.Visible = LangManager.IsEnableEditLangZHTW();

        HtmlGenericControl ctlIsShowInLangEn = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangEn");
        ctlIsShowInLangEn.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangEn);
        ctlIsShowInLangEn.Visible = LangManager.IsEnableEditLangEN();

        Literal ltrMdfDate = (Literal)e.Item.FindControl("ltrMdfDate");
        ltrMdfDate.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", mdfDate);

        string fileSavedName = attData.FileSavedName;
        HtmlImage imgExt = (HtmlImage)e.Item.FindControl("imgExt");
        imgExt.Src = "BPimages/FileExtIcon/" + ResUtility.GetExtIconFileName(fileSavedName);
        imgExt.Alt = ResUtility.GetExtIconText(fileSavedName);
        imgExt.Attributes["title"] = imgExt.Alt;

        HtmlAnchor btnEdit = (HtmlAnchor)e.Item.FindControl("btnEdit");
        btnEdit.Attributes["onclick"] = string.Format("popWin('Article-Attach.aspx?act={0}&attid={1}', 700, 600); return false;", ConfigFormAction.edit, attId);
        btnEdit.Title = Resources.Lang.Main_btnEdit_Hint;

        Literal ltrEdit = (Literal)e.Item.FindControl("ltrEdit");
        ltrEdit.Text = Resources.Lang.Main_btnEdit;

        LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDelete");
        btnDelete.CommandArgument = string.Join(",", attId.ToString(), attSubject);
        btnDelete.Text = "<i class='fa fa-trash-o'></i> " + Resources.Lang.Main_btnDelete;
        btnDelete.ToolTip = Resources.Lang.Main_btnDelete_Hint;
        btnDelete.OnClientClick = string.Format("return confirm('" + Resources.Lang.Attachment_ConfirmDelete_Format + "');",
            attSubject, attId);

        if (dontDelete)
        {
            HtmlGenericControl ctlDontDelete = (HtmlGenericControl)e.Item.FindControl("ctlDontDelete");
            ctlDontDelete.Attributes["title"] = Resources.Lang.Status_DontDelete;
            ctlDontDelete.Visible = true;
            btnDelete.Visible = false;
        }

        HtmlAnchor btnDownloadAtt = (HtmlAnchor)e.Item.FindControl("btnDownloadAtt");
        btnDownloadAtt.HRef = string.Format("~/FileAtt.ashx?attid={0}", attId);
        btnDownloadAtt.Title = Resources.Lang.Article_btnDownloadAtt_Hint;

        Literal ltrDownloadAtt = (Literal)e.Item.FindControl("ltrDownloadAtt");
        ltrDownloadAtt.Text = Resources.Lang.Article_btnDownloadAtt;

        string ownerAccount = attData.PostAccount;
        int ownerDeptId = attData.PostDeptId;

        if (!empAuth.CanEditThisPage(false, ownerAccount, ownerDeptId))
        {
            btnMoveDown.Visible = false;
            btnMoveUp.Visible = false;
            btnEdit.Visible = false;
        }

        if (!empAuth.CanDelThisPage(ownerAccount, ownerDeptId))
        {
            btnDelete.Visible = false;
        }

    }

    protected void rptAttachFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        bool result = false;
        Guid attId;

        switch (e.CommandName)
        {
            case "Del":
                string[] args = e.CommandArgument.ToString().Split(',');
                attId = new Guid(args[0]);
                string attSubject = args[1];

                AttachFileManagerLogic attFileMgr = new AttachFileManagerLogic(this.Context, artPub);
                result = attFileMgr.Initialize(attId, c.qsArtId);

                if (result)
                {
                    result = attFileMgr.DeleteData();

                    //新增後端操作記錄
                    empAuth.InsertBackEndLogData(new BackEndLogData()
                    {
                        EmpAccount = c.GetEmpAccount(),
                        Description = string.Format("．刪除附件/Delete attach file　．代碼/id[{0}]　標題/subject[{1}]　結果/result[{2}]", attId, attSubject, result),
                        IP = c.GetClientIP()
                    });

                    // log to file
                    c.LoggerOfUI.InfoFormat("{0} deletes {1}, result: {2}", c.GetEmpAccount(), "attach file-[" + attId.ToString() + "]-" + attSubject, result);
                }

                if (!result)
                {
                    string errMsg = ResUtility.GetErrMsgOfAttFileErrState(attFileMgr.GetErrState());

                    if (errMsg == "")
                    {
                        errMsg = Resources.Lang.ErrMsg_DeleteAttachmentFailed;
                    }

                    Master.ShowErrorMsg(errMsg);
                }

                break;
            case "MoveUp":
                attId = new Guid(e.CommandArgument.ToString());
                result = artPub.DecreaseAttachFileSortNo(attId, c.GetEmpAccount());
                break;
            case "MoveDown":
                attId = new Guid(e.CommandArgument.ToString());
                result = artPub.IncreaseAttachFileSortNo(attId, c.GetEmpAccount());
                break;
        }

        if (result)
        {
            DisplayAttachFiles();
        }
    }

    private void DisplayPictures()
    {
        ltrUploadPicture.Text = Resources.Lang.Article_btnUploadPicture;
        btnUploadPicture.Title = Resources.Lang.Article_btnUploadPicture_Hint;
        btnUploadPicture.Attributes["onclick"] =
            string.Format("popWin('Article-Picture.aspx?act={0}&artid={1}', 700, 600); return false;",
                ConfigFormAction.add, c.qsArtId);
        btnUploadPicture.Visible = empAuth.CanEditThisPage();

        ArticlePictureListQueryParams param = new ArticlePictureListQueryParams()
        {
            ArticleId = c.qsArtId,
            CultureName = c.seCultureNameOfBackend,
            Kw = ""
        };

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = 1,
            EndNum = 999999999,
            SortField = "",
            IsSortDesc = false
        };

        param.AuthParams = new AuthenticationQueryParams()
        {
            CanReadSubItemOfOthers = empAuth.CanReadSubItemOfOthers(),
            CanReadSubItemOfCrew = empAuth.CanReadSubItemOfCrew(),
            CanReadSubItemOfSelf = empAuth.CanReadSubItemOfSelf(),
            MyAccount = c.GetEmpAccount(),
            MyDeptId = c.GetDeptId()
        };

        List<ArticlePictureForBEList> articlePictures = artPub.GetArticlePictureMultiLangListForBackend(param);

        if (articlePictures != null)
        {
            rptArticlePictures.DataSource = articlePictures;
            rptArticlePictures.DataBind();
        }
    }
    protected void rptArticlePictures_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ArticlePictureForBEList artPic = (ArticlePictureForBEList)e.Item.DataItem;

        Guid picId = artPic.PicId;
        string picSubject = artPic.PicSubject;
        bool isShowInLangZhTw = artPic.IsShowInLangZhTw;
        bool isShowInLangEn = artPic.IsShowInLangEn;
        string mdfAccount = "";
        DateTime mdfDate = DateTime.MinValue;

        if (!artPic.MdfDate.HasValue)
        {
            mdfAccount = artPic.PostAccount;
            mdfDate = artPic.PostDate.Value;
        }
        else
        {
            mdfAccount = artPic.MdfAccount;
            mdfDate = artPic.MdfDate.Value;
        }

        HtmlAnchor btnView = (HtmlAnchor)e.Item.FindControl("btnView");
        btnView.HRef = string.Format("FileArtPic.ashx?attid={0}", picId);
        btnView.Title = Resources.Lang.Main_btnClickToOpenInNewWin_Hint;

        HtmlImage imgPic = (HtmlImage)e.Item.FindControl("imgPic");
        imgPic.Src = string.Format("FileArtPic.ashx?attid={0}&w=320&h=240", picId);
        imgPic.Alt = picSubject;

        HtmlGenericControl ctlIsShowInLangZhTw = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangZhTw");
        ctlIsShowInLangZhTw.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangZhTw);
        ctlIsShowInLangZhTw.Visible = LangManager.IsEnableEditLangZHTW();

        HtmlGenericControl ctlIsShowInLangEn = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangEn");
        ctlIsShowInLangEn.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangEn);
        ctlIsShowInLangEn.Visible = LangManager.IsEnableEditLangEN();

        HtmlAnchor btnEdit = (HtmlAnchor)e.Item.FindControl("btnEdit");
        btnEdit.Attributes["onclick"] = string.Format("popWin('Article-Picture.aspx?act={0}&picid={1}', 700, 600); return false;", ConfigFormAction.edit, picId);
        btnEdit.Title = Resources.Lang.Main_btnEdit_Hint;

        Literal ltrEdit = (Literal)e.Item.FindControl("ltrEdit");
        ltrEdit.Text = Resources.Lang.Main_btnEdit;

        LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDelete");
        btnDelete.CommandArgument = string.Join(",", picId.ToString(), picSubject);
        btnDelete.Text = "<i class='fa fa-trash-o'></i> " + Resources.Lang.Main_btnDelete;
        btnDelete.ToolTip = Resources.Lang.Main_btnDelete_Hint;
        btnDelete.OnClientClick = string.Format("return confirm('" + Resources.Lang.ArticlePicture_ConfirmDelete_Format + "');",
            picSubject, picId);

        string ownerAccount = artPic.PostAccount;
        int ownerDeptId = artPic.PostDeptId;

        btnEdit.Visible = empAuth.CanEditThisPage(false, ownerAccount, ownerDeptId);

        if (!empAuth.CanDelThisPage(ownerAccount, ownerDeptId))
        {
            btnDelete.Visible = false;
        }

    }

    protected void rptArticlePictures_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        bool result = false;
        Guid picId;

        switch (e.CommandName)
        {
            case "Del":
                string[] args = e.CommandArgument.ToString().Split(',');
                picId = new Guid(args[0]);
                string picSubject = args[1];

                ArticlePictureManagerLogic artPicMgr = new ArticlePictureManagerLogic(this.Context, artPub);
                result = artPicMgr.Initialize(picId, c.qsArtId);

                if (result)
                {
                    result = artPicMgr.DeleteData();

                    //新增後端操作記錄
                    empAuth.InsertBackEndLogData(new BackEndLogData()
                    {
                        EmpAccount = c.GetEmpAccount(),
                        Description = string.Format("．刪除網頁照片/Delete article picture　．代碼/id[{0}]　標題/subject[{1}]　結果/result[{2}]", picId, picSubject, result),
                        IP = c.GetClientIP()
                    });

                    // log to file
                    c.LoggerOfUI.InfoFormat("{0} deletes {1}, result: {2}", c.GetEmpAccount(), "article picture-[" + picId.ToString() + "]-" + picSubject, result);
                }

                if (!result)
                {
                    string errMsg = ResUtility.GetErrMsgOfAttFileErrState(artPicMgr.GetErrState());

                    if (errMsg == "")
                    {
                        errMsg = Resources.Lang.ErrMsg_DeleteArticlePictureFailed;
                    }

                    Master.ShowErrorMsg(errMsg);
                }

                break;
        }

        if (result)
        {
            DisplayPictures();
        }
    }

    private void DisplayVideos()
    {
        ltrAddVideo.Text = Resources.Lang.Article_btnAddVideo;
        btnAddVideo.Title = Resources.Lang.Article_btnAddVideo_Hint;
        btnAddVideo.Attributes["onclick"] =
            string.Format("popWin('Article-Video.aspx?act={0}&artid={1}', 700, 600); return false;",
                ConfigFormAction.add, c.qsArtId);
        btnAddVideo.Visible = empAuth.CanEditThisPage();

        ArticleVideoListQueryParams param = new ArticleVideoListQueryParams()
        {
            ArticleId = c.qsArtId,
            CultureName = c.seCultureNameOfBackend,
            Kw = ""
        };

        param.PagedParams = new PagedListQueryParams()
        {
            BeginNum = 1,
            EndNum = 999999999,
            SortField = "",
            IsSortDesc = false
        };

        param.AuthParams = new AuthenticationQueryParams()
        {
            CanReadSubItemOfOthers = empAuth.CanReadSubItemOfOthers(),
            CanReadSubItemOfCrew = empAuth.CanReadSubItemOfCrew(),
            CanReadSubItemOfSelf = empAuth.CanReadSubItemOfSelf(),
            MyAccount = c.GetEmpAccount(),
            MyDeptId = c.GetDeptId()
        };

        List<ArticleVideoForBEList> articleVideos = artPub.GetArticleVideoMultiLangListForBackend(param);

        if (articleVideos != null)
        {
            rptArticleVideos.DataSource = articleVideos;
            rptArticleVideos.DataBind();
        }
    }

    protected void rptArticleVideos_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ArticleVideoForBEList artVid = (ArticleVideoForBEList)e.Item.DataItem;

        Guid vidId = artVid.VidId;
        string vidSubject = artVid.VidSubject;
        string sourceVideoId = artVid.SourceVideoId;
        bool isShowInLangZhTw = artVid.IsShowInLangZhTw;
        bool isShowInLangEn = artVid.IsShowInLangEn;
        string mdfAccount = "";
        DateTime mdfDate = DateTime.MinValue;

        if (!artVid.MdfDate.HasValue)
        {
            mdfAccount = artVid.PostAccount;
            mdfDate = artVid.PostDate.Value;
        }
        else
        {
            mdfAccount = artVid.MdfAccount;
            mdfDate = artVid.MdfDate.Value;
        }

        HtmlAnchor btnView = (HtmlAnchor)e.Item.FindControl("btnView");
        btnView.HRef = string.Format("https://www.youtube.com/watch?v={0}", sourceVideoId);
        btnView.Title = Resources.Lang.Main_btnClickToOpenInNewWin_Hint;

        HtmlImage imgPic = (HtmlImage)e.Item.FindControl("imgPic");
        imgPic.Src = string.Format("http://i.ytimg.com/vi/{0}/default.jpg", sourceVideoId);
        imgPic.Alt = vidSubject;

        HtmlGenericControl ctlIsShowInLangZhTw = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangZhTw");
        ctlIsShowInLangZhTw.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangZhTw);
        ctlIsShowInLangZhTw.Visible = LangManager.IsEnableEditLangZHTW();

        HtmlGenericControl ctlIsShowInLangEn = (HtmlGenericControl)e.Item.FindControl("ctlIsShowInLangEn");
        ctlIsShowInLangEn.Attributes["class"] = StringUtility.GetCssClassOfIconIsShowInLang(isShowInLangEn);
        ctlIsShowInLangEn.Visible = LangManager.IsEnableEditLangEN();

        HtmlAnchor btnEdit = (HtmlAnchor)e.Item.FindControl("btnEdit");
        btnEdit.Attributes["onclick"] = string.Format("popWin('Article-Video.aspx?act={0}&vidid={1}', 700, 600); return false;", ConfigFormAction.edit, vidId);
        btnEdit.Title = Resources.Lang.Main_btnEdit_Hint;

        Literal ltrEdit = (Literal)e.Item.FindControl("ltrEdit");
        ltrEdit.Text = Resources.Lang.Main_btnEdit;

        LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDelete");
        btnDelete.CommandArgument = string.Join(",", vidId.ToString(), vidSubject);
        btnDelete.Text = "<i class='fa fa-trash-o'></i> " + Resources.Lang.Main_btnDelete;
        btnDelete.ToolTip = Resources.Lang.Main_btnDelete_Hint;
        btnDelete.OnClientClick = string.Format("return confirm('" + Resources.Lang.ArticleVideo_ConfirmDelete_Format + "');",
            vidSubject, vidId);

        string ownerAccount = artVid.PostAccount;
        int ownerDeptId = artVid.PostDeptId;

        btnEdit.Visible = empAuth.CanEditThisPage(false, ownerAccount, ownerDeptId);

        if (!empAuth.CanDelThisPage(ownerAccount, ownerDeptId))
        {
            btnDelete.Visible = false;
        }
    }

    protected void rptArticleVideos_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        bool result = false;
        Guid vidId;

        switch (e.CommandName)
        {
            case "Del":
                string[] args = e.CommandArgument.ToString().Split(',');
                vidId = new Guid(args[0]);
                string vidSubject = args[1];

                result = artPub.DeleteArticleVideoData(vidId);

                //新增後端操作記錄
                empAuth.InsertBackEndLogData(new BackEndLogData()
                {
                    EmpAccount = c.GetEmpAccount(),
                    Description = string.Format("．刪除網頁影片/Delete article video　．代碼/id[{0}]　標題/subject[{1}]　結果/result[{2}]", vidId, vidSubject, result),
                    IP = c.GetClientIP()
                });

                // log to file
                c.LoggerOfUI.InfoFormat("{0} deletes {1}, result: {2}", c.GetEmpAccount(), "article video-[" + vidId.ToString() + "]-" + vidSubject, result);

                if (!result)
                {
                    Master.ShowErrorMsg(Resources.Lang.ErrMsg_DeleteArticleVideoFailed);
                }
                
                break;
        }

        if (result)
        {
            DisplayVideos();
        }
    }

    protected string GetClientAjaxAuthToken()
    {
        if (!empAuth.CanEditThisPage())
            return "";

        ArticleAjaxAuthData authData = new ArticleAjaxAuthData()
        {
            EmpAccount = c.GetEmpAccount(),
            CanEditSubItemOfOthers = empAuth.CanEditSubItemOfOthers(),
            CanEditSubItemOfCrew = empAuth.CanEditSubItemOfCrew(),
            CanEditSubItemOfSelf = empAuth.CanEditSubItemOfSelf(),
            PostDate = DateTime.Now
        };

        string result = "";

        try
        {
            string authJson = JsonConvert.SerializeObject(authData);
            string aesKeyOfBP = ConfigurationManager.AppSettings["AesKeyOfBP"];
            string basicIV = ConfigurationManager.AppSettings["AesIV"];
            result = AesUtility.Encrypt(authJson, aesKeyOfBP, basicIV);
        }
        catch (Exception ex)
        {
            c.LoggerOfUI.Error("", ex);
        }

        return result;
    }
}