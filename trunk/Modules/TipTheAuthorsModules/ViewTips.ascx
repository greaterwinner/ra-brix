<%@ Assembly 
    Name="TipTheAuthorsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="TipTheAuthorsModules.ViewTips" %>

<div style="padding:5px;">
    <asp:Repeater runat="server" ID="rep">
        <ItemTemplate>
            <div>
                <a 
                    target="_blank" 
                    rel="nofollow"
                    href='<%#Eval("[URL].Value") %>'>
                    <%#Eval("[URL].Value") %> - 
                    <%#Eval("[Date].Value") %> - 
                    <%#Eval("[User].Value") %>
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>