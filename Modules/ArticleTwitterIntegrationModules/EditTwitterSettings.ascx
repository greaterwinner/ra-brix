<%@ Assembly 
    Name="ArticleTwitterIntegrationModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticleTwitterIntegrationModules.EditTwitterSettings" %>

<ra:Panel 
    runat="server" 
    id="pnl2" 
    Visible="false"
    style="padding:5px;">
    <h1>
        <%=LanguageRecords.Language.Instance["TwitterHeaderAfterSetting", null, "Your Twitter username and password has been saved"] %>
    </h1>
    <p>
        <%=LanguageRecords.Language.Instance["ThankYouForAddingTwitterIntegration", null, "Thank you for adding twitter integration to your articles :)"] %>
    </p>
</ra:Panel>

<ra:Panel 
    runat="server" 
    id="pnl" 
    DefaultWidget="submit"
    style="padding:5px;">
    <h1>
        <%=LanguageRecords.Language.Instance["TwitterHeader1", null, "Set your Twitter username..."] %>
    </h1>
    <p>
        <%=LanguageRecords.Language.Instance["TwitterHeader2", null, "...and password here, and we'll tweet your articles automatically for you!"]%>
    </p>
    <table style="width:300px;margin-top:25px;">
        <tr>
            <td>
                <%=LanguageRecords.Language.Instance["TwitterUsername", null, "Twitter Username:"] %>
            </td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    id="username" />
            </td>
        </tr>
        <tr>
            <td>
                <%=LanguageRecords.Language.Instance["TwitterPassword", null, "Twitter Password:"] %>
            </td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    TextMode="Password"
                    id="password" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:right;padding-top:5px;">
                <ra:ExtButton 
                    runat="server" 
                    id="submit" 
                    OnClick="submit_Click"
                    Text='<%#LanguageRecords.Language.Instance["Submit", null, "Submit"] %>' />
            </td>
        </tr>
    </table>
</ra:Panel>