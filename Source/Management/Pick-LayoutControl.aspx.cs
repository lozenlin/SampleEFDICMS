using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Pick_LayoutControl : System.Web.UI.Page
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

    private List<ControlInfo> infoList;

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

        this.artPub = ArticlePublisherLogicIn;
        artPub.SetAuthenticationConditionProvider(c);

        this.empAuth = EmployeeAuthorityLogicIn;
        empAuth.SetAuthenticationConditionProvider(c);
        empAuth.SetCustomEmployeeAuthorizationResult(artPub);
        empAuth.InitialAuthorizationResultOfSubPages();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Authenticate
            if (!empAuth.CanEditThisPage())
            {
                string jsClose = "closeThisForm();";
                ClientScript.RegisterStartupScript(this.GetType(), "invalid", jsClose, true);
                return;
            }

            InitialInfoList();
            DisplayList();
        }

        LoadTitle();
    }

    private void LoadTitle()
    {
        Title = Resources.Lang.GroupLabel_LayoutControl;
    }

    private void InitialInfoList()
    {
        infoList = new List<ControlInfo>();

        infoList.Add(new ControlInfo()
        {
            Name = "",
            PicFileName = "default.png"
        });

        infoList.Add(new ControlInfo()
        {
            Name = "ListItemsThumb",
            PicFileName = "ListItemsThumb.png"
        });

        infoList.Add(new ControlInfo()
        {
            Name = "ListBlocks",
            PicFileName = "ListBlocks.png"
        });
    }

    private void DisplayList()
    {
        rptList.DataSource = infoList;
        rptList.DataBind();
    }

    protected void rptList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ControlInfo info = (ControlInfo)e.Item.DataItem;

        string displayText = info.Name;

        if (info.Name == "")
        {
            displayText = "(" + Resources.Lang.Pick_btnClear + ")";
        }


        HtmlAnchor btnPic = (HtmlAnchor)e.Item.FindControl("btnPic");
        btnPic.HRef = "BPimages/LayoutControl/" + info.PicFileName;
        btnPic.Title = info.Name;

        HtmlImage imgPic = (HtmlImage)e.Item.FindControl("imgPic");
        imgPic.Src = "BPimages/LayoutControl/" + info.PicFileName;

        HtmlGenericControl ctlNameArea = (HtmlGenericControl)e.Item.FindControl("ctlNameArea");
        ctlNameArea.InnerHtml = displayText;
        ctlNameArea.Attributes["title"] = displayText;

        LinkButton btnSelect = (LinkButton)e.Item.FindControl("btnSelect");
        btnSelect.CommandArgument = info.Name;
        btnSelect.OnClientClick = string.Format("return confirm('" + Resources.Lang.Pick_ConfirmSelect_Format + "');", displayText);
    }

    protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "sel":
                string controlName = e.CommandArgument.ToString();
                string js = Common.Utility.StringUtility.GetWriteValueOfOpenerJs(c.qsCtlText, controlName);
                js += " closeThisForm();";
                ClientScript.RegisterStartupScript(this.GetType(), "sel", js, true);
                break;
        }
    }

    private class ControlInfo
    {
        public string Name;
        public string PicFileName;
    }
}