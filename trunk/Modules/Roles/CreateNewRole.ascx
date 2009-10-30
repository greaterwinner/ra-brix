<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RolesModules.CreateNewRole" %>
<%@ Import Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="btnSubmit"
    style="position:relative;width:100%;height:100%;">
    <div style="padding:15px;">
        <%#Language.Instance["RolesRoleName", null, "Role name..."]%>
        <br />
        <ra:TextBox 
            runat="server" 
            id="roleTxt" />
        <br />
        <ra:ExtButton 
            runat="server" 
            id="btnSubmit" 
            style="position:absolute;right:15px;bottom:10px;"
            OnClick="btnSubmit_Click"/>
    </div>
</ra:Panel>

