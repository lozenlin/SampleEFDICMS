using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsonService
{
    public static class JsonServiceHandlerFactory
    {
        public static IJsonServiceHandler GetHandler(HttpContext context, string serviceName, RoleCommonOfBackend c, EmployeeAuthorityLogic empAuth, ArticlePublisherLogic artPub)
        {
            IJsonServiceHandler handler = null;

            switch (serviceName)
            {
                case "TempStoreRolePvg":
                    handler = new TemporarilyStoreRolePrivilege(context, c, empAuth);
                    break;
                case "UpdateArticleIsAreaShowInFrontStage":
                    handler = new UpdateArticleIsAreaShowInFrontStage(context, c, artPub);
                    break;
                case "UpdateArticleSortFieldOfFrontStage":
                    handler = new UpdateArticleSortFieldOfFrontStage(context, c, artPub);
                    break;
            }

            return handler;
        }
    }
}