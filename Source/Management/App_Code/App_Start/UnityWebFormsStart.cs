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
			// captcha, Dashboard
			container.RegisterType<HttpContext>(new InjectionFactory(c => HttpContext.Current));
			container.RegisterType<EmployeeAuthorityLogic>(new HierarchicalLifetimeManager());
			container.RegisterType<ArticlePublisherLogic>(new HierarchicalLifetimeManager());
			container.RegisterType<BackendPageCommon>();

			// Login, Psw-Change, Psw-Require
			container.RegisterType<LoginCommonOfBackend>();

			// Global
			container.RegisterType<ParamFilterClient>();

		}
	}
}