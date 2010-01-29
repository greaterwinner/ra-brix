<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.Status" %>

<div style="margin-left:auto;margin-right:auto;display:table;margin-top:35px;margin-bottom:5px;font-style:italic;">
    <ra:Label 
        runat="server" 
        id="status" />

    <a 
        runat="server" 
        href="~/authors/all.aspx" 
        id="linkToUsers" />

    <ra:Label 
        runat="server" 
        id="status2" />
</div>

