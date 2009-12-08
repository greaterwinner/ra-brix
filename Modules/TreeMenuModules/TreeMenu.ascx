<%@ Assembly 
    Name="TreeMenuModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="TreeMenuModules.TreeMenu" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="position:relative;height:100%;">

    <div class="treeMenuSearchWrapper">
        <ra:TextBox 
            runat="server" 
            CssClass="treeMenuSearch filter"
            style="opacity:0.3;"
            OnKeyUp="search_KeyUp"
            AccessKey="F"
            id="search">
            <ra:BehaviorUnveiler 
                runat="server" />
        </ra:TextBox>
    </div>
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

    <div class="treeMenuWrapper">
        <div class="treeMenuSpacer">&nbsp;</div>
        <ra:Tree 
            runat="server" 
            style="width:100%;height:100%;" 
            ClientSideExpansion="true"
            CssClass="tree treeMenu"
            OnSelectedNodeChanged="menu_SelectedNodeChanged"
            ID="menu">

            <ra:TreeNodes 
                runat="server" 
                ID="root" />

        </ra:Tree>
    </div>

</div>
