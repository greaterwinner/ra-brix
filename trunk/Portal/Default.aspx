<%@ Page 
    Language="C#" 
    AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" 
    ValidateRequest="false"
    Inherits="Ra.Brix.Portal.MainWebPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <base id="baseElement" runat="server" href='<%#GetBaseURL()%>' />
        <title>Ra.Brix - a Modular and Extensible Application Framework</title>
        <link runat="server" id="css1" href='<%# "media/skins/" + GetCssRootFolder() + "/steel/steel.css" %>' rel="stylesheet" type="text/css" />
        <link runat="server" id="css2" href='<%# "media/skins/" + GetCssRootFolder() + "/portalSkins/portalSkins.css" %>' rel="stylesheet" type="text/css" />
        <link runat="server" id="css3" href='<%# "media/skins/" + GetCssRootFolder() + "/main.css" %>' rel="stylesheet" type="text/css" />
        <meta http-equiv="X-UA-Compatible" content="chrome=1" />
    </head>
    <body>
        <form id="form1" runat="server" enctype="multipart/form-data" />

        <!--[if IE]>
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/chrome-frame/1/CFInstall.min.js"></script>
        <div id="placeholder"></div>
        <script>
            CFInstall.check({
                node: "placeholder",
                destination: "<%= GetRedirectUrl() %>"
            });
        </script>
        <![endif]-->
    </body>
</html>
