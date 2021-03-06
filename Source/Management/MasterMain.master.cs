using Common.Data.Domain.Model;
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

public partial class MasterMain : System.Web.UI.MasterPage
{
    [Dependency]
    public BackendPageCommon BackendPageCommonIn { get; set; }
    [Dependency]
    public ArticlePublisherLogic ArticlePublisherLogicIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    protected BackendPageCommon c;
    protected ArticlePublisherLogic artPub;
    protected EmployeeAuthorityLogic empAuth;

    private bool useEnglishSubject = false;
    private int opIdOfArticleMgmt = 0;

    #region Public properties

    public string FlagValue
    {
        get { return txtFlag.Value; }
        set { txtFlag.Value = value; }
    }

    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        Page.Title = Resources.Lang.BackStageName;
        Page.MaintainScrollPositionOnPostBack = true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (BackendPageCommonIn == null)
            throw new ArgumentException("BackendPageCommonIn");

        if (ArticlePublisherLogicIn == null)
            throw new ArgumentException("ArticlePublisherLogicIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        this.c = BackendPageCommonIn;
        c.InitialLoggerOfUI(this.GetType());
        this.artPub = ArticlePublisherLogicIn;
        artPub.SetAuthenticationConditionProvider(c);
        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);

        if (c.seLoginEmpData.EmpAccount == null)
        {
            ShowErrorMsg(Resources.Lang.ErrMsg_LostSessionState);
            c.LogOutWhenSessionMissed(Resources.Lang.ErrMsg_LostSessionState);
        }
        
        if (!IsPostBack)
        {
            LoadUIData();
        }

        DisplayOpMenu();
    }

    private void LoadUIData()
    {
        if (c.seLoginEmpData.EmpAccount != null)
        {
            LoginEmployeeData d = c.seLoginEmpData;
            ltrRoleDisplayName.Text = string.Format("{0}({1})", d.RoleDisplayName, d.RoleName);
            ltrDeptName.Text = d.DeptName;
            ltrAccountInfo.Text = string.Format("Hi, {0}({1})", d.EmpName, d.EmpAccount);
            btnAccountSettings.Title = Resources.Lang.Main_btnAccountSettings;
            btnAccountSettings.HRef = "Account-List.aspx";
            btnLogout.Title = Resources.Lang.Main_btnLogout;
            btnLogout.HRef = "Logout.ashx";

            btnEditOperations.Title = Resources.Lang.btnEditOperations_Hint;
        }

        //只有管理者能編輯後端作業選項, guest 可看
        if (c.IsInRole("admin") || c.IsInRole("guest"))
        {
            btnEditOperations.Visible = true;
            LineOfCtrl.Visible = btnEditOperations.Visible;
        }
    }

    private List<ArticleMultiLangForOpMenu> GetSubitemsOfArticle(Guid articleId)
    {
        List<ArticleMultiLangForOpMenu> subitems = artPub.GetArticleMultiLangListForOpMenu(articleId, c.seCultureNameOfBackend);

        return subitems;
    }

    private void DisplayOpMenu()
    {
        // get opId of article management
        OperationOpInfo opInfo = empAuth.GetOperationOpInfoByCommonClass("ArticleCommonOfBackend");

        if (opInfo != null)
        {
            opIdOfArticleMgmt = opInfo.OpId;
        }

        List<OperationWithRoleAuth> topList = empAuth.GetOperationWithRoleAuthNestedList(c.GetRoleName());

        if (c.IsInRole("admin"))
        {
            //管理者可以看到全部
            foreach (OperationWithRoleAuth op in topList)
            {
                op.CanRead = true;

                foreach(OperationWithRoleAuth subOp in op.SubItems)
                {
                    subOp.CanRead = true;
                }
            }
        }

        if (c.seCultureNameOfBackend == "en")
        {
            useEnglishSubject = true;
        }

        rptOpMenu.DataSource = topList;
        rptOpMenu.DataBind();
    }

    protected void rptOpMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        OperationWithRoleAuth opAuth = (OperationWithRoleAuth)e.Item.DataItem;

        int opId = opAuth.OpId;
        string opSubject = opAuth.OpSubject;
        string englishSubject = opAuth.EnglishSubject;
        bool isNewWindow = opAuth.IsNewWindow;
        string encodedUrl = opAuth.LinkUrl;
        string linkUrl = c.DecodeUrlOfMenu(encodedUrl);

        if (useEnglishSubject && !string.IsNullOrEmpty(englishSubject))
        {
            opSubject = englishSubject;
        }

        HtmlGenericControl OpHeaderArea = (HtmlGenericControl)e.Item.FindControl("OpHeaderArea");
        OpHeaderArea.Attributes.Add("opId", opId.ToString());

        HtmlAnchor btnOpHeader = (HtmlAnchor)e.Item.FindControl("btnOpHeader");
        btnOpHeader.Title = opSubject;

        if (isNewWindow)
        {
            btnOpHeader.Target = "_blank";
            btnOpHeader.Title += Resources.Lang.HintTail_OpenNewWindow;
        }

        if (linkUrl != "")
        {
            if (linkUrl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                    || linkUrl.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            {
                //外部網址
                if (!isNewWindow)
                    linkUrl = "~/Embedded-Content.aspx?url=" + Server.UrlEncode(encodedUrl);
            }
            else
            {
                linkUrl = "~/" + linkUrl;
            }

            btnOpHeader.HRef = linkUrl;
        }

        HtmlImage imgOpHeader = (HtmlImage)e.Item.FindControl("imgOpHeader");
        imgOpHeader.Alt = opSubject;
        imgOpHeader.Src = "~/BPimages/icon/data.gif";
        string iconImageFile = opAuth.IconImageFile;
        if (!string.IsNullOrEmpty(iconImageFile))
            imgOpHeader.Src = string.Format("~/BPimages/icon/{0}", iconImageFile);

        Literal ltrOpHeaderSubject = (Literal)e.Item.FindControl("ltrOpHeaderSubject");
        ltrOpHeaderSubject.Text = opSubject;

        if (opIdOfArticleMgmt != 0 && opId == opIdOfArticleMgmt)
        {
            string noticeIconOfHoverToExpand = "<span class='hover-intent-notice float-right' title='hover to expand' style='display:none;'><i class='fa fa-hand-o-up'></i><i class='fa fa-hourglass-start'></i></span>";
            ltrOpHeaderSubject.Text += noticeIconOfHoverToExpand;
        }

        //檢查授權
        bool canRead = opAuth.CanRead;

        OpHeaderArea.Visible = canRead;
        Repeater rptOpItems = (Repeater)e.Item.FindControl("rptOpItems");
        Repeater rptArticles = (Repeater)e.Item.FindControl("rptArticles");

        if (opIdOfArticleMgmt != 0 && opId == opIdOfArticleMgmt)
        {
            // articles
            rptOpItems.Visible = false;

            List<ArticleMultiLangForOpMenu> subitems = GetSubitemsOfArticle(Guid.Empty);  //Guid.Empty: root articleId

            if (subitems != null)
            {
                rptArticles.Visible = true;
                rptArticles.DataSource = subitems;
                rptArticles.DataBind();
            }
        }
        else
        {
            // sub-operations
            rptOpItems.DataSource = opAuth.SubItems;
            rptOpItems.DataBind();
        }
    }

    protected void rptOpItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        OperationWithRoleAuth opAuth = (OperationWithRoleAuth)e.Item.DataItem;

        int opId = opAuth.OpId;
        string opSubject = opAuth.OpSubject;
        string englishSubject = opAuth.EnglishSubject;
        bool isNewWindow = opAuth.IsNewWindow;
        string encodedUrl = opAuth.LinkUrl;
        string linkUrl = c.DecodeUrlOfMenu(encodedUrl);

        if (useEnglishSubject && !string.IsNullOrEmpty(englishSubject))
        {
            opSubject = englishSubject;
        }

        HtmlGenericControl OpItemArea = (HtmlGenericControl)e.Item.FindControl("OpItemArea");
        OpItemArea.Attributes.Add("opId", opId.ToString());

        HtmlAnchor btnOpItem = (HtmlAnchor)e.Item.FindControl("btnOpItem");
        btnOpItem.Title = opSubject;

        if (isNewWindow)
        {
            btnOpItem.Target = "_blank";
            btnOpItem.Title += Resources.Lang.HintTail_OpenNewWindow;
        }

        if (linkUrl != "")
        {
            if (linkUrl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                    || linkUrl.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
            {
                //外部網址
                if (!isNewWindow)
                    linkUrl = "~/Embedded-Content.aspx?url=" + Server.UrlEncode(encodedUrl);
            }
            else
            {
                linkUrl = "~/" + linkUrl;
            }

            btnOpItem.HRef = linkUrl;
        }

        HtmlImage imgOpItem = (HtmlImage)e.Item.FindControl("imgOpItem");
        imgOpItem.Alt = opSubject;
        imgOpItem.Src = "~/BPimages/icon/data.gif";
        string iconImageFile = opAuth.IconImageFile;
        if (!string.IsNullOrEmpty(iconImageFile))
            imgOpItem.Src = string.Format("~/BPimages/icon/{0}", iconImageFile);

        Literal ltrOpItemSubject = (Literal)e.Item.FindControl("ltrOpItemSubject");
        ltrOpItemSubject.Text = opSubject;

        //檢查授權
        bool canRead = opAuth.CanRead;

        OpItemArea.Visible = canRead;
    }

    protected void rptArticles_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

        ArticleMultiLangForOpMenu articleData = (ArticleMultiLangForOpMenu)e.Item.DataItem;

        Guid articleId = articleData.ArticleId;
        string articleSubject = articleData.ArticleSubject;
        bool isHideSelf = articleData.IsHideSelf;

        HtmlAnchor btnItem = (HtmlAnchor)e.Item.FindControl("btnItem");
        btnItem.HRef = string.Format("~/Article-Node.aspx?artid={0}", articleId);

        Literal ltrArticleSubject = (Literal)e.Item.FindControl("ltrArticleSubject");
        ltrArticleSubject.Text = articleSubject;

        HtmlGenericControl BranchArea = (HtmlGenericControl)e.Item.FindControl("BranchArea");
        BranchArea.Attributes.Add("articleId", articleId.ToString());
        BranchArea.Visible = !isHideSelf;

        // sub-items
        Control ctlSubitems = e.Item.FindControl("rptSubitems");

        if (ctlSubitems != null)
        {
            Repeater rptSubitems = (Repeater)ctlSubitems;

            List<ArticleMultiLangForOpMenu> subitems = GetSubitemsOfArticle(articleId);

            if (subitems != null)
            {
                rptSubitems.DataSource = subitems;
                rptSubitems.DataBind();
            }
        }
    }

    #region Public Methods

    public void SetHeadUpDisplayVisible(bool visible)
    {
        ucHeadUpDisplay.Visible = visible;
    }

    public IHeadUpDisplay GetHeadUpDisplay()
    {
        return ucHeadUpDisplay;
    }

    /// <summary>
    /// 顯示錯誤訊息
    /// </summary>
    public void ShowErrorMsg(string value)
    {
        ltrErrMsg.Text = value;
        ErrorMsgArea.Visible = (value != "");

        if (ErrorMsgArea.Visible)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorMsg", "smoothUp();", true);
        }
    }

    public void RefreshOpMenu(EmployeeAuthorityLogic _empAuth, ArticlePublisherLogic _artPub, BackendPageCommon _c)
    {
        if (_empAuth == null)
            throw new ArgumentException("_empAuth");

        if (_artPub == null)
            throw new ArgumentException("_artPub");

        if (_c == null)
            throw new ArgumentException("_c");

        if (this.empAuth == null)
        {
            this.empAuth = _empAuth;
        }

        if(this.artPub == null)
        {
            this.artPub = _artPub;
        }

        if (this.c == null)
        {
            this.c = _c;
        }

        DisplayOpMenu();
    }

    #endregion
}
