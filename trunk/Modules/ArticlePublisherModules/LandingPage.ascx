<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.LandingPage" %>

<div class="articleWrapper">
    <ra:Label 
        runat="server" 
        Visible="true"
        Tag="h2"
        id="header" />
    <ra:Panel 
        runat="server" 
        id="infoWrp">
        <ra:Label 
            runat="server" 
            id="description" 
            Text='<%#LanguageRecords.Language.Instance["NoArticlesFoundYetDescription", null, "No articles found with these criteria."] %>'
            Tag="p" />
    </ra:Panel>
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <ra:Panel 
                runat="server" 
                CssClass="article">
                <a 
                    class="mainLink"
                    runat="server" 
                    href='<%#Eval("[URL].Value") %>'>
                    <ra:Label 
                        runat="server" 
                        Tag="h3"
                        style="display:block;font-size:16px;font-weight:bold;margin-bottom:10px;"
                        Text='<%#Eval("[Header].Value") %>' />
                    <img 
                        runat="server"
                        class="articleImage" 
                        alt='<%#Eval("[Header].Value") + " - " + Eval("[Ingress].Value") %>'
                        src='<%#Eval("[Icon].Value") %>' />
                    <ra:Label 
                        runat="server" 
                        style="display:block;"
                        Tag="summary"
                        Text='<%#Eval("[Ingress].Value") %>' />
                </a>
                <span style="position:absolute;bottom:5px;left:5px;font-size:10px;">
                    <%#Eval("[CommentCount].Value") %> 
                    <%#LanguageRecords.Language.Instance["Comments", null, "Comments"] %>
                </span>
                <a 
                    runat="server" 
                    href='<%#"~/authors/" + Eval("[Author].Value").ToString().Replace(".", "--") + ConfigurationManager.AppSettings["DefaultPageExtension"] %>'
                    style="position:absolute;bottom:5px;right:5px;font-size:10px;">
                    <%#Eval("[Author].Value") %>
                </a>
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>
<br style="clear:both" />
<hr style="margin-top:25px;margin-bottom:25px;" />



