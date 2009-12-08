<%@ Assembly 
    Name="HelpModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="HelpModules.Help" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="height:100%;width:100%;overflow:auto;">
    <div runat="server" id="headers">
        <h1><%#Language.Instance["HelpHeaderText", null, @"Help Files..."]%></h1>
        <p><%#Language.Instance["HelpParagraph", null, @"The application you're now looking at is built as a modular application, which means that you can
            get additional modules for it easily by contacting the vendor that created the application, or
            by developing these modules yourself.<p>Please choose which module you need to read about below.</p>"]%>
        </p>
    </div>
    <asp:Repeater runat="server" ID="rep">
        <HeaderTemplate>
            <ul class="list">
        </HeaderTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
        <ItemTemplate>
            <li>
                <ra:LinkButton 
                    runat="server" 
                    OnClick="ViewContents"
                    Xtra='<%#Eval("Value") %>'
                    Text='<%#Eval("Name") %>' />
                <ra:Panel 
                    runat="server" 
                    style="display:none;background-color:#eee;border:dotted 1px #999;margin-left:15px;">
                    <ra:Label 
                        style="padding:10px;display:block;"
                        runat="server" />
                </ra:Panel>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</div>