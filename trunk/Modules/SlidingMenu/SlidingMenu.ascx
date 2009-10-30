<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="SlidingMenuModules.SlidingMenu" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="position:relative;height:100%;">

    <ra:Panel 
        runat="server" 
        DefaultWidget="searchBtn"
        id="searchWrp">
        <ra:TextBox 
            runat="server" 
            CssClass="menuSearch filter"
            style="opacity:0.3;"
            AccessKey="F"
            id="search">
            <ra:BehaviorUnveiler 
                runat="server" />
        </ra:TextBox>
        <ra:Button 
            runat="server" 
            OnClick="searchBtn_Click"
            style="margin-left:-1000px;"
            id="searchBtn" />
    </ra:Panel>
    <ra:Window 
        runat="server"
        CssClass="light-window searchResults no-background"
        Visible="false"
        id="pnlSearchResults">
        <asp:Repeater 
            runat="server"
            id="repSearch">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
            <ItemTemplate>
                <li>
                    <ra:ExtButton 
                        runat="server" 
                        CssClass="button menuBtn"
                        OnClick="SearchClicked"
                        Xtra='<%#Eval("DNA") %>'
                        Text='<%# Language.Instance[Eval("Name").ToString()] %>' />
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ra:Window>

    <ra:SlidingMenu 
        runat="server" 
        style="width:100%;height:100%;" 
        OnItemClicked="menu_ItemClicked"
        ID="menu">

        <ra:SlidingMenuLevel 
            runat="server" 
            ID="root" />

    </ra:SlidingMenu>

</div>