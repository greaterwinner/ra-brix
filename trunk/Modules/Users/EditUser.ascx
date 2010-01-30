<%@ Assembly 
    Name="UsersModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="UsersModules.EditUser" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:5px;position:relative;">
    <ra:Label 
        runat="server" 
        style="font-style:italic;color:#999;position:absolute;top:5px;right:5px;"
        id="lblLastLoggedIn" />
    <div style="float:left;width:100px;height:100px;">
        <ra:Image
            runat="server" 
            AlternateText="User image" 
            Tooltip='<%#Language.Instance["EditUserImageTooltip", null, "Image fetched from Gravatar.com by email"] %>'
            style="margin-top:15px;margin-left:10px;"
            id="imgGravatar" />
    </div>
    <div  runat="server" id="userData" style="float:left;">
        <ra:Label 
            runat="server" 
            id="lblUsername" 
            Tag="h1" />
        <div style="float:left;width:200px;">
            <div>
                <ra:Label 
                    runat="server" 
                    id="lblEmail"
                    Text='<%#Language.Instance["EditUserEmailLabel", null, "Email: "] %>' />
                <ra:InPlaceEdit 
                    runat="server" 
                    style="color:Blue;cursor:pointer;"
                    Tooltip='<%#Language.Instance["EditUserEmailTooltip", null, "Click to edit..."] %>'
                    OnTextChanged="email_TextChanged"
                    id="email" />
            </div>
        </div>
        <div style="float:left;width:150px;">
            <ra:Label 
                runat="server" 
                id="lblPhone"
                Text='<%#Language.Instance["EditUserPhoneLabel", null, "Phone; "] %>' />
            <ra:InPlaceEdit 
                runat="server" 
                style="color:Blue;cursor:pointer;"
                Tooltip='<%#Language.Instance["EditUserPhoneTooltip", null, "Click to edit..."] %>'
                OnTextChanged="phone_TextChanged"
                id="phone" />
        </div>
    </div>
    <div style="clear:both;">
        <ra:Panel 
            runat="server" 
            id="roleEditing">
            <ra:SelectList 
                runat="server" 
                id="selectRole" 
                OnSelectedIndexChanged="selectRole_SelectedIndexChanged">
            </ra:SelectList>
            <ra:Panel runat="server" id="repWrp">
                <asp:Repeater runat="server" ID="rep">
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
                                OnClick="RoleDeleted" 
                                Xtra='<%#Eval("Value") %>' />
                            <%#Eval("Value") %>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ra:Panel>
        </ra:Panel>
    </div>
    <h2><%=LanguageRecords.Language.Instance["Biography", null, "Biography"]%></h2>
    <ra:RichEdit
        ID="editor" 
        CssClass="editor"
        OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
        CtrlKeys="s"
        OnCtrlKey="editor_CtrlKeys"
        OnGetExtraToolbarControls="editor_GetExtraToolbarControls"
        runat="server" />
</div>