<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.LandingPage" %>

<div class="articleWrapper">
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
                        title='<%#Eval("[Ingress].Value") %>'
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
