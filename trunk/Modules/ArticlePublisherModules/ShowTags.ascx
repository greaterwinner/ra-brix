<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.ShowTags" %>

<div style="margin-bottom:25px;">
    <asp:Repeater 
        runat="server" 
        ID="rep">
        <ItemTemplate>
            <a href='<%#Eval("[URL].Value") %>'><%#Eval("[Name].Value") %></a>
        </ItemTemplate>
    </asp:Repeater>
</div>
