<%@ Assembly 
    Name="RssReaderModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RssReaderModules.ViewRSSItems" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:5px;">
    <h2><%=Language.Instance["RSSNewsFromTheWorld", null, "News from the world..."]%></h2>
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <HeaderTemplate>
            <ul class="list">
        </HeaderTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
        <ItemTemplate>
            <li>
                <h3 style="display:inline;">
                    <ra:LinkButton 
                        runat="server" 
                        OnClick="ExpandRSS"
                        Text='<%#Eval("[Caption].Value") %>' />
                </h3>
                <ra:Panel 
                    runat="server"
                    style="display:none;">
                    <asp:Repeater 
                        runat="server" 
                        DataSource='<%#Eval("[Items]") %>'>
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <ItemTemplate>
                            <li>
                                <h4 style="display:inline;">
                                    <ra:LinkButton 
                                        runat="server" 
                                        OnClick="ExpandRSS"
                                        Text='<%#Eval("[Caption].Value") %>' />
                                </h4>
                                <ra:Panel 
                                    runat="server"
                                    style="display:none;">
                                    <%#Eval("[Body].Value") %>
                                </ra:Panel>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ra:Panel>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</div>


