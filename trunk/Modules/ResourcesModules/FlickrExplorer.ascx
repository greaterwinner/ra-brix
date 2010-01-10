<%@ Assembly 
    Name="ResourcesModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ResourcesModules.FlickrExplorer" %>

<ra:Panel 
    runat="server" 
    DefaultWidget="btnSearch"
    id="pnlWrp">
    <ra:TextBox 
        runat="server" 
        Text='<%#LanguageRecords.Language.Instance["SearchQuery", null, "Search query"] %>'
        id="search" />

    <ra:ExtButton 
        runat="server" 
        Text='<%#LanguageRecords.Language.Instance["Search", null, "Search"] %>'
        OnClick="btnSearch_Click"
        id="btnSearch" />
</ra:Panel>

<ra:Panel 
    runat="server" 
    style="height:440px;overflow:auto;"
    id="pnlRepWrp">
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <ra:Panel 
                style="float:left;margin:5px;cursor:pointer;height:120px;"
                runat="server">
                <ra:Image 
                    runat="server" 
                    AlternateText='<%#Eval("[Title].Value") %>' 
                    OnClick="ImagePreview"
                    ImageUrl='<%#Eval("[Thumb].Value")%>' />
                <ra:Image 
                    runat="server" 
                    style="position:fixed;top:0;left:0;z-index:100;display:none;"
                    Visible="false"
                    AlternateText='<%#Eval("[Title].Value") %>' 
                    OnMouseOut="MouseOutOverPreview"
                    OnClick="ImageChosen"
                    ImageUrl='<%#Eval("[Medium].Value")%>' />
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</ra:Panel>

