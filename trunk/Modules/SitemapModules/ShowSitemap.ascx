<%@ Assembly 
    Name="SitemapModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="SitemapModules.ShowSitemap" %>

<style type="text/css">
.siteMapWrp
{
    margin:5px;
    border:solid 1px #aaa;
    float:left;
	-webkit-border-radius: 10px;
	-moz-border-radius: 10px;
	-webkit-box-shadow: 2px 2px 2px #777;
	-moz-box-shadow: 2px 2px 2px #777;
	-webkit-transition-property:text-shadow, background-color, opacity;
	-moz-transition-property:text-shadow, background-color, opacity;
	-webkit-transition-duration: 0.5s, 0.5s;
	-moz-transition-duration: 0.5s, 0.5s;
	opacity:0.3;
}

.siteMapWrp:hover
{
	-webkit-box-shadow: 5px 5px 5px #777;
	-moz-box-shadow: 5px 5px 5px #777;
	background-color:#b6d4ef;
	opacity:1.0;
	background-color:#b6d4ef;
}

.siteMapLink
{
	padding:8px;
}

.siteMapLink:hover
{
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
                    CssClass="siteMapWrp">
                    <a style="display:block;" class="siteMapLink" href='<%#Eval("[URL].Value") %>'>
                        <%#Eval("[Name].Value") %>
                    </a>
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