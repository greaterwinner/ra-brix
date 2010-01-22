<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.SearchArticles" %>

<ra:Panel
    runat="server"
    id="pnlWrp" 
    DefaultWidget="btnSearch"
    style="position:relative;height:35px;overflow:hidden;">
    <ra:TextBox 
        runat="server" 
        CssClass="menuSearch filter"
        style="position:absolute;top:5px;right:5px;opacity:0.3;"
        id="search">
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unveilSearch" />
    </ra:TextBox>
    <ra:Button 
        runat="server" 
        style="margin-left:-100px;"
        OnClick="btnSearch_Click"
        id="btnSearch" />
</ra:Panel>

