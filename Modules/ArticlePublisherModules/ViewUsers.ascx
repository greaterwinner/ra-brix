<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ViewUsers" %>

<ra:Label 
    runat="server" 
    Tag="h1"
    id="header" />
<asp:Repeater runat="server" ID="rep">
    <ItemTemplate>
        <div class="userDiv">
            <img 
                style="width:64px;height:64px;float:left;margin-right:5px;overflow:hidden;"
                src='<%#Eval("[ImageSrc].Value") %>' 
                alt='<%#Eval("[Name].Value") %>' />
            <strong style="display:block;">
                <%#Eval("[Name].Value") %>
            </strong>
            <em style="display:block;">
                <%#LanguageRecords.Language.Instance["Score", null, "Score"] %>: 
                <%#Eval("[Score].Value") %>
            </em>
            <a 
                href='<%#Eval("[URL].Value") %>'>
                <%#LanguageRecords.Language.Instance["ReadArticles", null, "Read articles..."] %>
            </a>
        </div>
    </ItemTemplate>
</asp:Repeater>
<br style="clear:both;" />