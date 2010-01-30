<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewUser" %>

<a href="http://gravatar.com">
    <img 
        runat="server" 
        id="gravatar" 
        style="float:right;margin-left:15px;width:80px;height:80px;overflow:hidden;" />
</a>

<h1 
    runat="server" 
    id="header" />

<ra:Label 
    runat="server" 
    Tag="div"
    CssClass="biography"
    id="biography" />

<em 
    runat="server" 
    id="summaryView" />

<hr 
    style="margin-top:25px;margin-bottom:25px;clear:both;" />

