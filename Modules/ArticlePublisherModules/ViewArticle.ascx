<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewArticle" %>

<div style="position:relative;">

    <h1 
        runat="server" 
        id="header" 
        style="margin-bottom:25px;" />

    <img 
        runat="server" 
        id="image" 
        style="float:right;margin-left:15px;" />

    <p 
        runat="server" 
        id="date" 
        style="font-style:italic;color:#666;" />

    <p 
        runat="server" 
        id="ingress" 
        style="font-style:italic;color:#666;" />

    <div 
        runat="server" 
        class="articleMainBody"
        id="content" />

    <a 
        runat="server" 
        id="author" 
        style="position:absolute;top:5px;right:25px;font-style:italic;" />

    <ra:LinkButton 
        runat="server" 
        id="bookmark" 
        OnClick="bookmark_Click"
        CssClass="bookmark" />

    <ra:ExtButton 
        runat="server" 
        id="edit" 
        style="position:absolute;bottom:0px;right:0px;"
        OnClick="edit_Click"
        Text='<%#LanguageRecords.Language.Instance["Edit", null, "Edit"] %>' />

    <br style="clear:both;" />
    <hr style="margin-top:35px;margin-bottom:15px;" />

</div>


