<%@ Assembly 
    Name="UsersModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="UsersModules.CreateNewUser" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="btnSubmit"
    style="position:relative;width:100%;height:100%;">
    <div style="padding:15px;">
         <%#Language.Instance["UsersCreateNewUserLabel", null, "Username..."]%>
        <br />
        <ra:TextBox 
            runat="server" 
            EscPressed="usernameTxt_EscPressed"
            id="usernameTxt" />
        <br />
        <ra:ExtButton 
            runat="server" 
            id="btnSubmit" 
            style="position:absolute;right:15px;bottom:10px;"
            OnClick="btnSubmit_Click" />
    </div>
</ra:Panel>

