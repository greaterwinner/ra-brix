<%@ Assembly 
    Name="WhiteboardModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="WhiteboardModules.CreateNewColumn" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="position:relative;width:100%;height:100%;">
    <ra:Panel 
        runat="server" 
        id="wrp" 
        style="padding:5px;"
        DefaultWidget="submit">
        <table>
            <tr>
                <td><%#Language.Instance["ListCreateColumnLabel", null, "Name of column"] %></td>
                <td>
                    <ra:TextBox 
                        runat="server" 
                        OnEscPressed="name_EscPressed"
                        id="name" />
                </td>
            </tr>
        </table>
        <ra:ExtButton 
            runat="server" 
            id="submit" 
            style="position:absolute;right:5px;bottom:5px;"
            OnClick="submit_Click"
            Text='<%#Language.Instance["WhiteboardCreateSubmitLabel", null, "Submit"] %>' />
    </ra:Panel>
</div>

