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
using System.Web.UI.WebControls;
using Unity.Attributes;

public partial class Article_Video : System.Web.UI.Page
{
    [Dependency]
    public ArticleVideoCommonOfBackend ArticleVideoCommonOfBackendIn { get; set; }
    [Dependency]
    public ArticlePublisherLogic ArticlePublisherLogicIn { get; set; }
    [Dependency]
    public EmployeeAuthorityLogic EmployeeAuthorityLogicIn { get; set; }

    protected ArticleVideoCommonOfBackend c;
    protected ArticlePublisherLogic artPub;
    protected EmployeeAuthorityLogic empAuth;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (ArticleVideoCommonOfBackendIn == null)
            throw new ArgumentException("ArticleVideoCommonOfBackendIn");

        if (ArticlePublisherLogicIn == null)
            throw new ArgumentException("ArticlePublisherLogicIn");

        if (EmployeeAuthorityLogicIn == null)
            throw new ArgumentException("EmployeeAuthorityLogicIn");

        this.c = ArticleVideoCommonOfBackendIn;
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

            LoadUIData();
            DisplayArticleVideoData();
            txtSortNo.Focus();
        }

        LoadTitle();
    }

    private void LoadUIData()
    {
        rfvSortNo.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        covSortNo.ErrorMessage = "*" + Resources.Lang.ErrMsg_IntegerOnly;
        rfvVidSubjectZhTw.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        rfvVidSubjectEn.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        rfvSourceVideoId.ErrorMessage = "*" + Resources.Lang.ErrMsg_Required;
        btnGetYoutubeId.OnClientClick = "return confirm('" + Resources.Lang.ArticleVideo_btnGetYoutubeId_Confirm + "');";

        SetupLangRelatedFields();
    }

    /// <summary>
    /// 設定語系相關欄位
    /// </summary>
    private void SetupLangRelatedFields()
    {
        if (!LangManager.IsEnableEditLangZHTW())
        {
            VidSubjectZhTwArea.Visible = false;
            chkIsShowInLangZhTw.Checked = false;
            chkIsShowInLangZhTw.Visible = false;
            VidDescZhTwArea.Visible = false;
        }

        if (!LangManager.IsEnableEditLangEN())
        {
            VidSubjectEnArea.Visible = false;
            chkIsShowInLangEn.Checked = false;
            chkIsShowInLangEn.Visible = false;
            VidDescEnArea.Visible = false;
        }
    }

    private void LoadTitle()
    {
        if (c.qsAct == ConfigFormAction.add)
            Title = string.Format(Resources.Lang.ArticleVideo_Title_AddNew_Format, c.qsArtId);
        else if (c.qsAct == ConfigFormAction.edit)
            Title = string.Format(Resources.Lang.ArticleVideo_Title_Edit_Format, c.qsVidId);
    }

    private void DisplayArticleVideoData()
    {
        if (c.qsAct == ConfigFormAction.edit)
        {
            ArticleVideo video = artPub.GetArticleVideoDataForBackend(c.qsVidId);

            if (video != null)
            {
                txtSortNo.Text = video.SortNo.ToString();
                txtVidLinkUrl.Text = video.VidLinkUrl;
                txtSourceVideoId.Text = video.SourceVideoId;
                ltrPostAccount.Text = video.PostAccount;
                ltrPostDate.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", video.PostDate);
                string mdfAccount = video.MdfAccount;
                DateTime mdfDate = DateTime.MinValue;

                if (video.MdfDate.HasValue)
                {
                    mdfDate = video.MdfDate.Value;
                }

                //zh-TW
                if (LangManager.IsEnableEditLangZHTW())
                {
                    ArticleVideoMultiLang vidZhTw = artPub.GetArticleVideoMultiLangDataForBackend(c.qsVidId, LangManager.CultureNameZHTW);

                    if (vidZhTw != null)
                    {
                        txtVidSubjectZhTw.Text = vidZhTw.VidSubject;
                        chkIsShowInLangZhTw.Checked = vidZhTw.IsShowInLang;
                        txtVidDescZhTw.Text = vidZhTw.VidDesc;

                        if (vidZhTw.MdfDate.HasValue && vidZhTw.MdfDate.Value > mdfDate)
                        {
                            mdfAccount = vidZhTw.MdfAccount;
                            mdfDate = vidZhTw.MdfDate.Value;
                        }
                    }
                }

                //en
                if (LangManager.IsEnableEditLangEN())
                {
                    ArticleVideoMultiLang vidEn = artPub.GetArticleVideoMultiLangDataForBackend(c.qsVidId, LangManager.CultureNameEN);

                    if (vidEn != null)
                    {
                        txtVidSubjectEn.Text = vidEn.VidSubject;
                        chkIsShowInLangEn.Checked = vidEn.IsShowInLang;
                        txtVidDescEn.Text = vidEn.VidDesc;

                        if (vidEn.MdfDate.HasValue && vidEn.MdfDate.Value > mdfDate)
                        {
                            mdfAccount = vidEn.MdfAccount;
                            mdfDate = vidEn.MdfDate.Value;
                        }
                    }
                }

                if (mdfDate != DateTime.MinValue)
                {
                    ltrMdfAccount.Text = mdfAccount;
                    ltrMdfDate.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", mdfDate);
                }

                btnSave.Visible = true;
            }
        }
        else if (c.qsAct == ConfigFormAction.add)
        {
            int newSortNo = artPub.GetArticleVideoMaxSortNo(c.qsArtId) + 10;
            txtSortNo.Text = newSortNo.ToString();

            btnSave.Visible = true;
        }
    }

    protected void btnGetYoutubeId_Click(object sender, EventArgs e)
    {
        txtVidLinkUrl.Text = txtVidLinkUrl.Text.Trim();
        txtSourceVideoId.Text = StringUtility.GetYoutubeIdFromUrl(txtVidLinkUrl.Text);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Master.ShowErrorMsg("");

        if (!IsValid)
            return;

        try
        {
            txtVidLinkUrl.Text = txtVidLinkUrl.Text.Trim();
            txtSourceVideoId.Text = txtSourceVideoId.Text.Trim();

            ArticleVideoParams param = new ArticleVideoParams()
            {
                SortNo = Convert.ToInt32(txtSortNo.Text),
                VidLinkUrl = txtVidLinkUrl.Text,
                SourceVideoId = txtSourceVideoId.Text,
                PostAccount = c.GetEmpAccount()
            };

            txtVidSubjectZhTw.Text = txtVidSubjectZhTw.Text.Trim();
            txtVidDescZhTw.Text = txtVidDescZhTw.Text.Trim();

            ArticleVideoMultiLangParams paramZhTw = new ArticleVideoMultiLangParams()
            {
                CultureName = LangManager.CultureNameZHTW,
                VidSubject = txtVidSubjectZhTw.Text,
                IsShowInLang = chkIsShowInLangZhTw.Checked,
                VidDesc = txtVidDescZhTw.Text,
                PostAccount = c.GetEmpAccount()
            };

            txtVidSubjectEn.Text = txtVidSubjectEn.Text.Trim();
            txtVidDescEn.Text = txtVidDescEn.Text.Trim();

            ArticleVideoMultiLangParams paramEn = new ArticleVideoMultiLangParams()
            {
                CultureName = LangManager.CultureNameEN,
                VidSubject = txtVidSubjectEn.Text,
                IsShowInLang = chkIsShowInLangEn.Checked,
                VidDesc = txtVidDescEn.Text,
                PostAccount = c.GetEmpAccount()
            };

            bool result = false;

            if (c.qsAct == ConfigFormAction.add)
            {
                Guid newVidId = Guid.NewGuid();
                param.VidId = newVidId;
                param.ArticleId = c.qsArtId;

                result = artPub.InsertArticleVideoData(param);

                if (result)
                {
                    //zh-TW
                    if (result && LangManager.IsEnableEditLangZHTW())
                    {
                        paramZhTw.VidId = param.VidId;
                        result = artPub.InsertArticleVideoMultiLangData(paramZhTw);
                    }

                    //en
                    if (result && LangManager.IsEnableEditLangEN())
                    {
                        paramEn.VidId = param.VidId;
                        result = artPub.InsertArticleVideoMultiLangData(paramEn);
                    }

                    if (!result)
                    {
                        Master.ShowErrorMsg(Resources.Lang.ErrMsg_AddMultiLangFailed);
                    }
                }
                else
                {
                    Master.ShowErrorMsg(Resources.Lang.ErrMsg_AddFailed);
                }
            }
            else if (c.qsAct == ConfigFormAction.edit)
            {
                param.VidId = c.qsVidId;

                result = artPub.UpdateArticleVideoData(param);

                if (result)
                {
                    //zh-TW
                    if (result && LangManager.IsEnableEditLangZHTW())
                    {
                        paramZhTw.VidId = param.VidId;
                        result = artPub.UpdateArticleVideoMultiLangData(paramZhTw);
                    }

                    //en
                    if (result && LangManager.IsEnableEditLangEN())
                    {
                        paramEn.VidId = param.VidId;
                        result = artPub.UpdateArticleVideoMultiLangData(paramEn);
                    }

                    if (!result)
                    {
                        Master.ShowErrorMsg(Resources.Lang.ErrMsg_UpdateMultiLangFailed);
                    }
                }
                else
                {
                    Master.ShowErrorMsg(Resources.Lang.ErrMsg_UpdateFailed);
                }
            }

            if (result)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", StringUtility.GetNoticeOpenerJs("Video"), true);
            }

            //新增後端操作記錄
            string description = string.Format("．{0}　．儲存網頁影片/Save article video[{1}][{2}]　VidId[{3}]　結果/result[{4}]",
                Title, txtVidSubjectZhTw.Text, txtVidSubjectEn.Text, param.VidId, result);

            empAuth.InsertBackEndLogData(new BackEndLogData()
            {
                EmpAccount = c.GetEmpAccount(),
                Description = description,
                IP = c.GetClientIP()
            });
        }
        catch (Exception ex)
        {
            c.LoggerOfUI.Error("", ex);
            Master.ShowErrorMsg(ex.Message);
        }
    }
}