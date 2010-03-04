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
<ra:TabControl 
    runat="server" 
    id="tab">
    <ra:TabView 
        runat="server" 
        Caption="Sample"
        id="tabSample">
        <div style="margin:50px;">
            <ra:Dynamic
                runat="server"
                OnReload="sampleDyn_Reload"
                id="sampleDyn" />
        </div>
    </ra:TabView>
    <ra:TabView 
        runat="server" 
        Caption="C# code"
        id="tabCode">
        <div style="margin:50px;">
            <ra:Label 
                runat="server" 
                id="code" />
        </div>
    </ra:TabView>
    <ra:TabView 
        runat="server" 
        Caption=".ASPX syntax"
        id="tabASPX">
        <div style="margin:50px;">
            <ra:Label 
                runat="server" 
                id="aspx" />
        </div>
    </ra:TabView>
</ra:TabControl>
