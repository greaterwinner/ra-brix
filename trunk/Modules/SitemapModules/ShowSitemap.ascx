<%@ Assembly 
    Name="SitemapModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="SitemapModules.ShowSitemap" %>

<style type="text/css">
.siteMapLink
{
	padding:8px;
}
.siteMapLink:hover
{
	background-color:#d6f4ff;
}
</style>

<ra:Panel 
    runat="server" 
    id="pnl" 
    DefaultWidget="search"
    style="padding:5px;position:relative;">
    <ra:TextBox 
        runat="server" 
        id="filter" 
        style="position:absolute;top:5px;right:5px;opacity:0.3;"
        CssClass="filter">
        <ra:BehaviorUnveiler 
            id="unveil"
            runat="server" />
    </ra:TextBox>
    <h1><%=LanguageRecords.Language.Instance["Sitemap", null, "Sitemap"] %></h1>
    <p>
        <%=LanguageRecords.Language.Instance["SitemapTextInfo", null, @"
This are all the links you've got access to within this system.
Filter in the top right corner to search for some specific page. 
If you are logged in, you will probably have access to more items than if you're not logged in."] %></h1>
    </p>
    <ra:Panel 
        runat="server" 
        id="pnlWrp">
        <asp:Repeater 
            runat="server" 
            ID="rep">
            <ItemTemplate>
                <ra:Panel 
                    runat="server" 
                    style="margin:5px;border:dashed 1px #aaa;float:left;opacity:0.3;">
                    <a style="display:block;" class="siteMapLink" href='<%#Eval("[URL].Value") %>'>
                        <%#Eval("[Name].Value") %>
                    </a>
                    <ra:BehaviorUnveiler 
                        runat="server" />
                </ra:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </ra:Panel>
    <ra:Button 
        runat="server" 
        id="search" 
        OnClick="search_Click"
        style="margin-left:-4000px;" />
</ra:Panel>