<%@ Assembly 
    Name="SitemapModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="SitemapModules.ShowSitemap" %>

<div style="padding:5px;">
    <h1><%=LanguageRecords.Language.Instance["Sitemap", null, "Sitemap"] %></h1>
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <div>
                <a href='<%#Eval("[URL].Value") %>'>
                    <%#Eval("[Name].Value") %>
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>