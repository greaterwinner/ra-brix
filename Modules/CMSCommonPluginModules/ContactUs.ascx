<%@ Assembly 
    Name="CMSCommonPluginModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSCommonPluginModules.ContactUs" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    style="padding:5px;"
    id="wrp">
    <table class="spacedTable">
        <tr>
            <td style="text-align:right;">
                <%=Language.Instance["EmailOrPhone", null, "Your Email or Phone:"] %>
            </td>
            <td style="width:450px;">
                <ra:TextBox 
                    runat="server" 
                    style="width:100%;border:1px solid #76B4E0;"
                    id="email" />
            </td>
        </tr>
        <tr>
            <td style="text-align:right;">
                <%=Language.Instance["Your name", null, "Your name:"] %>
            </td>
            <td style="width:450px;">
                <ra:TextBox 
                    runat="server" 
                    style="width:100%;border:1px solid #76B4E0;"
                    id="name" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align:top;text-align:right;">
                <%=Language.Instance["Comment", null, "Comment:"] %>
            </td>
            <td>
                <ra:TextArea 
                    runat="server" 
                    style="width:100%;height:130px;border:1px solid #76B4E0;"
                    id="body" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align:right;">
                <ra:ExtButton 
                    runat="server" 
                    id="submit" 
                    OnClick="submit_Click"
                    Text='<%#Language.Instance["Submit", null, "Submit"] %>' />
            </td>
        </tr>
    </table>
</ra:Panel>
<ra:Panel 
    runat="server" 
    style="display:none;"
    id="wrp2">
    <strong>
        <%= Language.Instance["ThankYouForEmailWeWillComeBack", null, "Thank you for sending us email, we will come back to you as fast as possible."] %>
    </strong>
</ra:Panel>



