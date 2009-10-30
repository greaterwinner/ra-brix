<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ActiveDirectoryLoginModules.Login" %>
<%@ Import Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    DefaultWidget="btnLogin"
    id="pnl">

    <table runat="server" id="tbl">
        <tr>
            <td colspan="2">
                <ra:Label 
                    runat="server"
                    ID="lbl"
                    Text='<%#Language.Instance["ADLoginLabel", null, "Type in your username and password"]%>'/>
            </td>
        </tr>
        <tr>
            <td><%#Language.Instance["ADLoginUsernameLabel", null, "Username: "]%></td>
            <td>
                <ra:TextBox
                    runat="server"
                    ID="domainAndUserName" />
            </td>
        </tr>
        <tr>
            <td><%#Language.Instance["ADLoginPasswordLabel", null, "Password: "]%></td>
            <td>
                <ra:TextBox
                    runat="server"
                    ID="passWord"
                    TextMode="Password" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ra:ExtButton
                    runat="server"
                    Text='<%#Language.Instance["ADLoginButton", null, "Login"]%>'
                    ID="btnLogin"
                    OnClick="btnLogin_Click" />    
            </td>
        </tr>
    </table>
            
</ra:Panel>
