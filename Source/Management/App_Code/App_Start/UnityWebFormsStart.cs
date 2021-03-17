using Common.DataAccess.EF;
using Common.LogicObject;
using Common.LogicObject.DataAccessInterfaces;
using System.Web;

using Unity;
using Unity.Injection;
using Unity.Lifetime;
using UnityFive.WebForms;

[assembly: WebActivatorEx.PostApplicationStartMethod( typeof(ASP.App_Start.UnityWebFormsStart), "PostStart" )]
namespace ASP.App_Start
{
	/// <summary>
	///		Startup class for the UnityFive.WebForms NuGet package.
	/// </summary>
	internal static class UnityWebFormsStart
	{
		/// <summary>
		///     Initializes the unity container when the application starts up.
		/// </summary>
		/// <remarks>
		///		Do not edit this method. Perform any modifications in the
		///		<see cref="RegisterDependencies" /> method.
		/// </remarks>
		internal static void PostStart()
		{
			IUnityContainer container = new UnityContainer();
			HttpContext.Current.Application.SetContainer( container );

			RegisterDependencies( container );
		}

		/// <summary>
		///		Registers dependencies in the supplied container.
		/// </summary>
		/// <param name="container">Instance of the container to populate.</param>
		private static void RegisterDependencies( IUnityContainer container )
		{
			// 2021/03/16, lozenlin, add, here is kind of Composition Root

			// TODO: Add any dependencies needed here
			// Common components of:
			// all
			container.RegisterType<IEmployeeAuthorityDataAccess, EmployeeAuthorityDataAccess>(new HierarchicalLifetimeManager());
			container.RegisterType<IArticlePublisherDataAccess, ArticlePublisherDataAccess>(new HierarchicalLifetimeManager());
			container.RegisterType<IAuthenticationConditionProvider, NullAuthenticationConditionProvider>();
			// captcha, Dashboard, angularFileManager/Index, MasterLogin, MasterMain, MasterConfig, wucHeadUpDisplay
			container.RegisterType<HttpContext>(new InjectionFactory(c => HttpContext.Current));
			container.RegisterType<EmployeeAuthorityLogic>(new HierarchicalLifetimeManager());
			container.RegisterType<ArticlePublisherLogic>(new HierarchicalLifetimeManager());
			container.RegisterType<BackendPageCommon>(new HierarchicalLifetimeManager());

			// Login, Psw-Change, Psw-Require, Logout
			container.RegisterType<LoginCommonOfBackend>(new HierarchicalLifetimeManager());

			// Account-List, Account-Config
			container.RegisterType<AccountCommonOfBackend>(new HierarchicalLifetimeManager());

			// Role-LIst, Role-Config, Role-Privilege, jsonService
			container.RegisterType<RoleCommonOfBackend>(new HierarchicalLifetimeManager());

			// Department-List, Department-Config
			container.RegisterType<DepartmentCommonOfBackend>(new HierarchicalLifetimeManager());

			// Back-End-Log
			container.RegisterType<BackEndLogCommonOfBackend>(new HierarchicalLifetimeManager());

			// Article-Node, Article-Config, Pick-CustomWebProgram, Pick-LayoutControl
			container.RegisterType<ArticleCommonOfBackend>(new HierarchicalLifetimeManager());

			// Article-Attach
			container.RegisterType<ArticleAttachCommonOfBackend>(new HierarchicalLifetimeManager());

			// Article-Picture
			container.RegisterType<ArticlePictureCommonOfBackend>(new HierarchicalLifetimeManager());

			// Article-Video
			container.RegisterType<ArticleVideoCommonOfBackend>(new HierarchicalLifetimeManager());

			// Operation-Node, Operation-Config
			container.RegisterType<OperationCommonOfBackend>(new HierarchicalLifetimeManager());

			// Embedded-Content
			container.RegisterType<EmbeddedContentCommonOfBackend>(new HierarchicalLifetimeManager());

			// AfmDownload, afmService
			container.RegisterType<AfmServicePageCommon>(new HierarchicalLifetimeManager());

			// FileAtt
			container.RegisterType<AttDownloadCommon>(new HierarchicalLifetimeManager());

			// FileArtPic
			container.RegisterType<ArtPicDownloadCommon>(new HierarchicalLifetimeManager());

			// FileAttView
			container.RegisterType<AttViewDownloadCommon>(new HierarchicalLifetimeManager());

			// GetCurrentAccount
			container.RegisterType<SsoAuthenticatorCommon>(new HierarchicalLifetimeManager());

			// Global
			container.RegisterType<ParamFilterClient>();

		}
	}
}