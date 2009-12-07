<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.CreateHyperLink" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="wrp" 
    DefaultWidget="submit"
    style="padding:15px;">
    <table class="createLink">
        <tr>
            <td style="white-space:nowrap;">
                <%=Language.Instance["LocalLink", null, "Local link: "] %>
            </td>
            <td>
                <ra:SelectList 
                    runat="server" 
                    OnSelectedIndexChanged="internalPages_SelectedIndexChanged"
                    style="width:170px;"
                    id="internalPages">
                </ra:SelectList>
            </td>
        </tr>
        <tr>
            <td style="white-space:nowrap;">
                <%=Language.Instance["URLToLink", null, "URL to link: "] %>
            </td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    id="urlText" />
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


