<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ChangeSkinModules.ChangeSkin" %>
<%@ Import Namespace="LanguageRecords"%>

<div style="padding:15px;">
    <h2>
        <%= Language.Instance["PleaseChooseNewSkin", null, "Please choose new skin below"] %>
    </h2>
    <asp:Repeater runat="server" ID="rep">
        <ItemTemplate>
            <ra:ImageButton 
                runat="server" 
                ImageUrl='<%#"media/skins/" + Eval("Folder") + ".png" %>'
                OnClick="ChangeSkinMethod"
                style="float:left;padding:10px;opacity:0.3;"
                Xtra='<%#Eval("Folder") %>'
                AlternateText='<%#Eval("Folder") %>'>
            
                <ra:BehaviorUnveiler 
                    runat="server" />
            
            </ra:ImageButton>
        </ItemTemplate>
    </asp:Repeater>
</div>

