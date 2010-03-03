<%@ Assembly 
    Name="DoxygentDotNetViewDocsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="DoxygentDotNetViewDocsModules.ViewClass" %>

<h1 
    runat="server" 
    id="header" />

<p 
    runat="server" 
    style="padding:15px;"
    id="info" />

<h2 style="margin:5px 0px 5px 0px;">Functions, Properties and Events</h2>
<asp:Repeater 
    runat="server" 
    ID="repProperties">
    <HeaderTemplate>
        <ul class="docsList">
    </HeaderTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
    <ItemTemplate>
        <li class='<%#Eval("Kind")%>'>
            <ra:LinkButton 
                runat="server" 
                Xtra='<%#Eval("ID")%>'
                OnClick="PropertyChosen">
                <ra:Label runat="server" ID="returnTypeLabel" Text='<%#Eval("Returns")%>' Visible="false" style="color:#b21;" />
                <ra:Label runat="server" Text='<%#Eval("Name")%>' />
                <ra:Label runat="server" ID="paramsLabel" Text='<%#Eval("Params")%>' Visible="false" style="color:#b21;" />
            </ra:LinkButton>
            <ra:Panel 
                runat="server" 
                CssClass="property"
                Visible="false">
                <ra:Label 
                    runat="server" />
            </ra:Panel>
        </li>
    </ItemTemplate>
</asp:Repeater>
