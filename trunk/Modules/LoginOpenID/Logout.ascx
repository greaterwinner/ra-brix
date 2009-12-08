<%@ Assembly 
    Name="LoginOpenIDModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LoginOpenIDModules.Logout" %>

<div class="logoutLinkButton">
    <ra:LinkButton 
        runat="server" 
        id="logout" 
        OnClick="logout_Click"
        style="opacity:0.3;"
        Text='&nbsp;'>
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unveilerLogin" />
    </ra:LinkButton>
</div>
