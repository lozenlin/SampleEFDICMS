using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class angularFileManager_Index : System.Web.UI.Page
{
    [Dependency]
    public BackendPageCommon BackendPageCommonIn { get; set; }

    protected BackendPageCommon c;
    protected string afmLang = "en";

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (BackendPageCommonIn == null)
            throw new ArgumentException("BackendPageCommonIn");

        this.c = BackendPageCommonIn;

        afmLang = c.seCultureNameOfBackend;

        if (string.Compare(afmLang, "zh-TW", true) == 0)
        {
            afmLang = "zh_tw";
        }
        else if (afmLang != "en")
        {
            afmLang = "en";
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}