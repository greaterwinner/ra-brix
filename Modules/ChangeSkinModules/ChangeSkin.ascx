<%@ Assembly 
    Name="ChangeSkinModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChangeSkinModules.ChangeSkin" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="padding:15px;">
    <h1>
        <%= Language.Instance["PleaseChooseNewSkin", null, "Please choose new skin below"] %>
    </h1>
    <asp:Repeater runat="server" ID="rep">
        <ItemTemplate>
            <ra:Panel 
                runat="server" 
                OnClick="ChangeSkinMethod"
                Xtra='<%#Eval("Folder") %>'
                style="float:left;padding:10px;opacity:0.3;">
                <h2>
                    <%#Eval("Folder") %>
                </h2>
                <ra:ImageButton 
                    runat="server" 
                    ImageUrl='<%#"media/skins/" + Eval("Folder") + ".png" %>'
                    AlternateText='<%#Eval("Folder") %>' />
                <ra:BehaviorUnveiler 
                    runat="server" />
            </ra:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>

