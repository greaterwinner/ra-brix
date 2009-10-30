<%@ Application Language="C#" %>

<script runat="server">
    /*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

    void Application_Start(object sender, EventArgs e) 
    {
    }
    
    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Application_EndRequest(object sender, EventArgs e)
    {
        //if (Request.Path.ToLowerInvariant().EndsWith("webresource.axd") &&
        //    HttpContext.Current.Response.ContentType.ToLowerInvariant() == "text/javascript")
        //{
        //    HttpContext.Current.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
        //    HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddYears(3));
        //    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
        //    HttpContext.Current.Response.Cache.SetValidUntilExpires(false);

        //    HttpContext.Current.Response.AppendHeader("Content-Encoding", "gzip");
        //}
    }
    
    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
