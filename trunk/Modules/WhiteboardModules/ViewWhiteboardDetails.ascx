<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="WhiteboardModules.ViewWhiteboardDetails" %>

<div style="overflow:auto;height:100%;">
    <ra:Label 
        runat="server" 
        id="headerLbl" 
        style="margin-left:25px;"
        Tag="h1" />
    <ra:Panel 
        runat="server" 
        style="padding:8px 45px 45px 45px;"
        id="wrpPnl" />
</div>


