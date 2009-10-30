<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="LoginOpenIDModules.Login" %>

<ra:Panel 
    runat="server" 
    id="openIdWrp" 
    CssClass="openIDTextBoxWrapper"
    DefaultWidget="logInButton">
    <table>
        <tr>
            <td>
                <div class="loginLinkButton">
                    <ra:TextBox
                        runat="server"
                        CssClass="openIDTextBox"
                        ID="openIdURL" />
                    <ra:Button
                        runat="server"
                        ID="logInButton"
                        Text="&nbsp;"
                        style="border:none 0;background-color:Transparent;opacity:0.3"
                        OnClick="logInButton_Click">
                        <ra:BehaviorUnveiler 
                            runat="server" 
                            id="unveilerLogin" />
                    </ra:Button>
                </div>
            </td>
        </tr>
    </table>
</ra:Panel>

