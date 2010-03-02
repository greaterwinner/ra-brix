<%@ Assembly 
    Name="ExternalLinksModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ExternalLinksModules.ViewLinks" %>

<div style="margin:5px;">
    <div>
        <ra:ExtButton 
            runat="server" 
            id="create" 
            OnClick="create_Click"
            Text='<%#LanguageRecords.Language.Instance["CreateNew", null, "Create New..."] %>' />
    </div>
    <ra:Panel 
        runat="server" 
        id="repWrp">
        <asp:Repeater 
            runat="server"
            ID="rep">
            <HeaderTemplate>
                <div style="width:100%;">
                    <div style="float:left;width:49%;">
                        Name
                    </div>
                    <div style="float:left;width:49%;">
                        URL
                    </div>
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <div style="width:100%;">
                    <div style="float:left;width:49%;">
                        <ra:InPlaceEdit 
                            Text='<%#Eval("[Name].Value") %>'
                            Xtra='<%#Eval("[ID].Value") %>'
                            CssClass="edit"
                            OnTextChanged="NameChanged"
                            runat="server" />
                    </div>
                    <div style="float:left;width:49%;">
                        <ra:InPlaceEdit 
                            Text='<%#Eval("[URL].Value") %>'
                            Xtra='<%#Eval("[ID].Value") %>'
                            CssClass="edit"
                            OnTextChanged="URLChanged"
                            runat="server" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </ra:Panel>
</div>


