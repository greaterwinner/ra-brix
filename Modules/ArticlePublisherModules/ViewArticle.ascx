<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewArticle" %>

<h1 runat="server" id="header" />
<img runat="server" id="image" style="float:right;margin-left:15px;" />
<p runat="server" id="date" style="font-style:italic;color:#666;" />
<p runat="server" id="ingress" style="font-style:italic;color:#666;" />
<div runat="server" id="content" />
