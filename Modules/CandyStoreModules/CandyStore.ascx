<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CandyStoreModules.CandyStore" %>

<asp:Repeater runat="server" ID="rep">

    <ItemTemplate>
        <ra:Panel 
            runat="server" 
            OnClick="SelectModule"
            Xtra='<%#Eval("[\"CandyName\"].Value") %>'
            CssClass="candy"
            style="opacity:0.3;">
            <h3 style="margin-bottom:10px;"><%#Eval("[\"CandyName\"].Value").ToString().Replace(".zip", "") %></h3>
            <img src='<%#Eval("[\"CandyUrl\"].Value") %>' alt='<%#Eval("[\"CandyName\"].Value") %>' />
            <ra:BehaviorUnveiler 
                runat="server" />
        </ra:Panel>
    </ItemTemplate>

</asp:Repeater>