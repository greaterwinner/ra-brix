<%@ Assembly 
    Name="LoginOpenIDModules" %>

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
                        style="opacity:0.3"
                        ID="openIdURL">
                        <ra:BehaviorUnveiler 
                            runat="server" 
                            id="unveilLogin" />
                    </ra:TextBox>
                    <ra:ExtButton
                        runat="server"
                        ID="logInButton"
                        style="opacity:0.3;"
                        Text='<%#LanguageRecords.Language.Instance["Login", null, "Login"] %>'
                        OnClick="logInButton_Click">
                        <ra:BehaviorUnveiler 
                            runat="server" 
                            id="unveilerLogin" />
                    </ra:ExtButton>
                </div>
            </td>
        </tr>
    </table>
</ra:Panel>

