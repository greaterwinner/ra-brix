<%@ Assembly 
    Name="ADGroups2RolesModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ADGroups2RolesModules.CreateNew" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:Panel 
    runat="server" 
    id="pnlWrp" 
    DefaultWidget="submit"
    style="position:relative;width:100%;height:100%;">
    <table style="margin:5px;">
        <tr>
            <td><%#Language.Instance["ADGroups2RolesCreateNewLabel", null, "AD-Group Name: "]%></td>
            <td>
                <ra:TextBox 
                    runat="server" 
                    id="adGroupName" />
            </td>
        </tr>
    </table>
    <ra:ExtButton 
        runat="server" 
        id="submit" 
        style="position:absolute;right:5px;bottom:5px;"
        OnClick="submit_Click"
        Text='<%#Language.Instance["ADGroups2RolesCreateNewButton", null, "Submit"]%>' />
</ra:Panel>

