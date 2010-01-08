<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewArticle" %>

<div style="position:relative;">
    <h1 runat="server" id="header" style="margin-bottom:25px;" />
    <img runat="server" id="image" style="float:right;margin-left:15px;" />
    <p runat="server" id="date" style="font-style:italic;color:#666;" />
    <p runat="server" id="ingress" style="font-style:italic;color:#666;" />
    <div runat="server" id="content" />
    <a runat="server" id="author" style="position:absolute;top:5px;right:5px;font-style:italic;" />
</div>


