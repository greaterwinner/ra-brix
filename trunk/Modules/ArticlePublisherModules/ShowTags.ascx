<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ShowTags" %>

<ra:Panel 
    runat="server" 
    id="pnl" 
    style="margin-bottom:25px;">
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <ra:Panel 
                runat="server"
                CssClass="tagLink"
                style="opacity:0.3;">
                <span class="button">
                    <span class="bRight">
                        <span class="bLeft">
                            <span class="bCenter">
                                <a 
                                    href='<%#Eval("[URL].Value") %>'>
                                    <%#Eval("[Name].Value") %>
                                </a>
                            </span>
                        </span>
                    </span>
                </span>
                <ra:BehaviorUnveiler 
                    runat="server" />
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</ra:Panel>
<br style="clear:both;" />


