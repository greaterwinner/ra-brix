<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ActiveDirectoryLogin.Login" %>

<ra:Panel 
    runat="server" 
    DefaultWidget="btnLogin"
    id="pnl">

    <table>
        <tr>
        <td>Username:</td>
            <td>
                <ra:TextBox
                    runat="server"
                    ID="domainAndUserName" />
            </td>
        </tr>
        <tr>
            <td>Password:</td>
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
                    Text="Login"
                    ID="btnLogin"
                    OnClick="btnLogin_Click" />    
            </td>
        </tr>
    </table>
            
</ra:Panel>
