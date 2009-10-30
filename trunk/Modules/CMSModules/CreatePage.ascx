<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.CreatePage" %>
<%@ Import Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="submit"
    style="position:relative;width:100%;height:100%;">
    <table style="margin:5px;">
        <tr>
            <td><%#Language.Instance["CMSCreateNewPage", null, "Create new page: "]%></td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    id="cmsPageName" />
            </td>
        </tr>
        <tr>
            <td><%#Language.Instance["CMSParentPage", null, "Choose parent: "]%></td>
            <td>
                <ra:SelectList runat="server" id="parent">
                    <ra:ListItem Text="None" Value="xxx-no-parent" />
                </ra:SelectList>
            </td>
        </tr>
    </table>
    <ra:ExtButton 
        runat="server" 
        id="submit" 
        style="position:absolute;right:5px;bottom:5px;"
        OnClick="submit_Click"
        Text='<%#Language.Instance["Submit", null, "Submit"]%>' />
</ra:Panel>

