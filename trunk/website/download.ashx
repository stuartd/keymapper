<%@ WebHandler Language="C#" Class="download" %>

using System;
using System.Web;
using KMBlog;

public class download : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string filename = context.Request.QueryString["file"];
        if (String.IsNullOrEmpty(filename))
            return ;
        
        if (filename.ToLower() != "keymapper.application" && filename.ToLower() != "setup.exe")
        {
                context.Response.Write(filename + " can't be downloaded from this server") ;
                return ;
        }

        DataAccess.CreateInstance().LogDownload(filename,
                                                                   context.Request.ServerVariables["remote_addr"],
                                                                   context.Request.ServerVariables["HTTP_REFERER"],
                                                                   context.Request.ServerVariables["HTTP_USER_AGENT"]
                                                                   );
        
        
        context.Response.ContentType = "Application/octet-stream";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
        context.Response.WriteFile(filename);
             
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}