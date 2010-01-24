<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.Favorites" %>

<div class="bookmarks">
    <ra:LinkButton 
        runat="server" 
        Text="&nbsp;"
        OnClick="bookmarks_Click"
        CssClass="bookmarks"
        id="bookmarks" />
    <ra:Panel 
        runat="server" 
        style="position:absolute;left:0px;"
        Visible="false"
        OnMouseOut="wndWrp_MouseOut"
        id="wndWrp">
        <ra:Window 
            runat="server" 
            CssClass="light-window"
            id="bookmarksPanel">
            <div style="padding:15px;">
                <ext:Grid 
                    runat="server" 
                    CssClass="grid"
                    PageSize="10"
                    style="margin-top:0;"
                    id="grd" />
            </div>
        </ra:Window>
    </ra:Panel>
</div>