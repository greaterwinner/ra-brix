<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.NormalContent" %>

<div>
    <h1 runat="server" id="header"></h1>
    <ra:Panel 
        runat="server" 
        id="content" />
</div>