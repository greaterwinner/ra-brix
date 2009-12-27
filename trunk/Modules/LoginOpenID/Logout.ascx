<%@ Assembly 
    Name="LoginOpenIDModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LoginOpenIDModules.Logout" %>

<div class="logoutLinkButton">
    <ra:ExtButton 
        runat="server" 
        id="logout" 
        OnClick="logout_Click"
        style="opacity:0.3;"
        Text='<%#LanguageRecords.Language.Instance["Logout", null, "Logout"] %>'>
        <ra:BehaviorUnveiler 
            runat="server" 
            id="unveilerLogin" />
    </ra:ExtButton>
</div>
