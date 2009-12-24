<%@ Assembly 
    Name="RolesModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RolesModules.AccessControl" %>

<ra:Panel 
    runat="server" 
    id="wholeUnit" 
    style="height:100%;width:100%;">

    <div style="width:50%;float:left;height:100%;overflow:auto;">
        <ra:Tree 
            runat="server" 
            OnSelectedNodeChanged="tree_SelectedNodeChanged"
            UseRichAnimations="true"
            style="margin:15px;"
            id="tree">

            <ra:TreeNodes 
                runat="server" 
                id="root" />

        </ra:Tree>
    </div>
    <div style="width:50%;float:left;height:100%;">
        <ra:Panel 
            runat="server" 
            id="editWrp" 
            style="padding-left:10px;display:none;height:100%;">

            <ra:Label 
                runat="server" 
                Tag="h2"
                id="lblNodeName" />
            <ra:SelectList 
                runat="server" 
                OnSelectedIndexChanged="selectRoles_SelectedIndexChanged"
                id="selectRoles">
                <ra:ListItem />
            </ra:SelectList>
            <ra:Panel 
                runat="server" 
                id="grdWrp">
                <asp:Repeater 
                    runat="server" 
                    id="grd">
                    <HeaderTemplate>
                        <ul class="list">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <ra:LinkButton 
                                runat="server" 
                                Text="X" 
                                Xtra='<%#Eval("Value") %>'
                                OnClick="DeleteRoleMenuMapping" />
                            <%#Eval("Value") %>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ra:Panel>
        </ra:Panel>
    </div>
</ra:Panel>




