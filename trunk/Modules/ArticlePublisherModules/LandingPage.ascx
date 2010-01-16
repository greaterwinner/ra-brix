<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.LandingPage" %>

<div class="articleWrapper">
    <ra:Panel 
        runat="server" 
        id="infoWrp">
        <ra:Label 
            runat="server" 
            id="lblInfo" 
            Tag="h1" 
            Text='<%#LanguageRecords.Language.Instance["NoArticlesYet", null, "No articles yet"] %>' />
        <ra:Label 
            runat="server" 
            id="description" 
            Text='<%#LanguageRecords.Language.Instance["NoArticlesYetDescriptionText", null, "There are no articles here yet for this part or user. If this is your first visit here, you may login and start writing articles to publish here."] %>'
            Tag="p" />
    </ra:Panel>
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <ra:Panel 
                runat="server" 
                style="opacity:0.5;"
                CssClass="article">
                <a 
                    class="mainLink"
                    runat="server" 
                    href='<%#Eval("[URL].Value") %>'>
                    <ra:Label 
                        runat="server" 
                        style="display:block;font-size:16px;font-weight:bold;margin-bottom:10px;"
                        Text='<%#Eval("[Header].Value") %>' />
                    <img 
                        runat="server"
                        style="float:right;" 
                        alt='<%#Eval("[Header].Value") + " - " + Eval("[Ingress].Value") %>'
                        src='<%#Eval("[Icon].Value") %>' />
                    <ra:Label 
                        runat="server" 
                        style="display:block;"
                        Text='<%#Eval("[Ingress].Value") %>' />
                </a>
                <a 
                    runat="server" 
                    href='<%#"~/authors/" + Eval("[Author].Value") + ".aspx" %>'
                    style="position:absolute;bottom:5px;right:5px;">
                    <%#Eval("[Author].Value") %>
                </a>
                <ra:BehaviorUnveiler 
                    MinOpacity="0.5"
                    runat="server" />
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>
<br style="clear:both" />
<hr style="margin-top:25px;margin-bottom:25px;" />



