using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class UserControls_wucSearchConditionPost : System.Web.UI.UserControl
{
    [Dependency]
    public SearchPageCommon SearchPageCommonIn { get; set; }

    protected SearchPageCommon c;

    protected void Page_Init(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (SearchPageCommonIn == null)
            throw new ArgumentException("SearchPageCommonIn");

        this.c = SearchPageCommonIn;
        c.InitialLoggerOfUI(this.GetType());

        if (!IsPostBack)
        {
            LoadUIData();
        }
    }

    private void LoadUIData()
    {
        txtKeyword.Attributes["placeholder"] = Resources.Lang.SearchResult_txtKeyword_Hint;
        txtKeyword.ToolTip = Resources.Lang.SearchResult_txtKeyword_Hint;
        btnToSearchResult.ToolTip = Resources.Lang.SearchResult_btnSearch_Hint;
    }

    protected void btnToSearchResult_Click(object sender, EventArgs e)
    {
        string keyword = txtKeyword.Text.Trim();

        if (keyword == "")
            return;

        c.GoToSearchResult(keyword);
    }
}