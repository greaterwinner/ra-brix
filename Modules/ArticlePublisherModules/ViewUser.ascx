<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewUser" %>

<h1 
    runat="server" 
    id="header" />


<div class="biography">
    <a href="http://gravatar.com">
        <img 
            runat="server" 
            id="gravatar" 
            style="float:left;margin-right:15px;width:80px;height:80px;overflow:hidden;" />
    </a>
    <ra:Label 
        runat="server" 
        Tag="div"
        id="biography" />
</div>

<em 
    runat="server" 
    id="summaryView" />

<hr 
    style="margin-top:25px;margin-bottom:25px;clear:both;" />

