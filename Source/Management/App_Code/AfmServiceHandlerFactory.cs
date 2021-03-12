using Common.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AfmService
{
    /// <summary>
    /// angular-FileManager service handler factory
    /// </summary>
    public static class AfmServiceHandlerFactory
    {
        public static IAfmServiceHandler GetHandler(HttpContext context, AfmRequest afmRequest, AfmServicePageCommon c, EmployeeAuthorityLogic empAuth)
        {
            IAfmServiceHandler handler = null;

            if (afmRequest == null)
                return handler;

            switch (afmRequest.action)
            {
                case "list":
                    handler = new AfmGetList(context, afmRequest, c, empAuth);
                    break;
                case "upload":
                    handler = new AfmUploadFiles(context, afmRequest, c, empAuth);
                    break;
                case "remove":
                    handler = new AfmRemoveFoldersOrFiles(context, afmRequest, c, empAuth);
                    break;
                case "createFolder":
                    handler = new AfmCreateFolder(context, afmRequest, c, empAuth);
                    break;
                case "rename":
                    handler = new AfmRenameFolderOrFile(context, afmRequest, c, empAuth);
                    break;
            }

            return handler;
        }
    }
}