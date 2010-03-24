<%@ Page 
    Language="C#" 
    AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" 
    ValidateRequest="false"
    Inherits="Ra.Brix.Portal.MainWebPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <base runat="server" id="baseElement" />
        <title>Ra.Brix - a Modular and Extensible Application Framework</title>
        <link 
            runat="server" 
            id="css1" 
            href='<%# GetCssRootFolder() + "/Gold/Gold.css" %>' 
            rel="stylesheet" 
            type="text/css" />
        <link 
            runat="server" 
            id="css3" 
            href='<%# GetCssRootFolder() + "/Gold.css" %>' 
            rel="stylesheet" 
            type="text/css" />
        <meta http-equiv="X-UA-Compatible" content="chrome=1" />
    </head>
    <body>
        <form 
            id="form1" 
            runat="server" 
            enctype="multipart/form-data" />
    </body>
</html>
