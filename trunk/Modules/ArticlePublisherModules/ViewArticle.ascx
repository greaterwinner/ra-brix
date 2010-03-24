<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewArticle" %>

<div style="position:relative;margin-top:15px;">

    <h1 
        runat="server" 
        id="header" 
        style="margin-bottom:25px;" />

    <p 
        runat="server" 
        id="ingress" 
        style="font-style:italic;color:#666;" />

    <div 
        runat="server" 
        class='<%#GetArticleCssClass() %>'
        id="content" />

    <p 
        runat="server" 
        id="date" 
        style="font-style:italic;color:#666;" />

    <a 
        runat="server" 
        id="author" 
        style="position:absolute;top:5px;right:25px;font-style:italic;margin-top:-25px;" />

    <ra:LinkButton 
        runat="server" 
        id="bookmark" 
        style="margin-top:-25px;"
        OnClick="bookmark_Click"
        CssClass="bookmark" />

    <ra:ExtButton 
        runat="server" 
        id="follow" 
        style="position:absolute;bottom:0px;right:160px;"
        Tooltip='<%#LanguageRecords.Language.Instance["FollowTooltip", null, "If you follow an article you will get emails when new comments are being written for it. Note you must have registered your email in your profile for this to work."] %>'
        OnClick="follow_Click" />

    <ra:ExtButton 
        runat="server" 
        id="delete" 
        style="position:absolute;bottom:0px;right:80px;"
        OnClick="delete_Click"
        Text='<%#LanguageRecords.Language.Instance["Delete", null, "Delete"] %>' />

    <ra:ExtButton 
        runat="server" 
        id="edit" 
        style="position:absolute;bottom:0px;right:0px;"
        OnClick="edit_Click"
        Text='<%#LanguageRecords.Language.Instance["Edit", null, "Edit"] %>' />

    <br style="clear:both;" />
    <hr style="margin-top:35px;margin-bottom:15px;" />

</div>


