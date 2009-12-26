<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.NormalContent" %>

<div style="padding-left:15px;">
    <h1 runat="server" id="header"></h1>
    <ra:Panel 
        runat="server" 
        id="content" />
</div>