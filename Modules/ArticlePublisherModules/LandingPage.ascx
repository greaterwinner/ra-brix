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
                    runat="server" 
                    href='<%#Eval("[URL].Value") %>'>
                    <ra:Label 
                        runat="server" 
                        style="display:block;font-size:16px;font-weight:bold;"
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
                <ra:BehaviorUnveiler 
                    MinOpacity="0.5"
                    runat="server" />
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>
