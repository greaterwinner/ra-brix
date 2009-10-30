<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSCommonPluginModules.ContactUs" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="wrp">
    <table class="spacedTable">
        <tr>
            <td style="text-align:right;">
                <%=Language.Instance["YourEmail", null, "Your Email:"] %>
            </td>
            <td style="width:450px;">
                <ra:TextBox 
                    runat="server" 
                    style="width:100%;border:1px solid #76B4E0;"
                    id="email" />
            </td>
        </tr>
        <tr>
            <td style="text-align:right;"><%=Language.Instance["Header", null, "Header:"] %></td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    style="width:100%;border:1px solid #76B4E0;"
                    id="header" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align:top;text-align:right;">
                <%=Language.Instance["Body", null, "Body:"] %>
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
