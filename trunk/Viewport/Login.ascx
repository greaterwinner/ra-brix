<%@ Assembly 
    Name="Viewport" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Viewport.Login" %>

<div style="height:50px;">&nbsp;</div>

<!-- Center window... -->
<ra:Window 
    runat="server" 
    ID="wndMid" 
    Closable="false" 
    Movable="false" 
    style="width:450px;margin-left:auto;margin-right:auto;"
    CssClass="light-window">
    <ra:Panel 
        runat="server" 
        CssClass="panePanel"
        ID="pnlMid">
        <ra:Dynamic 
            runat="server" 
            CssClass="dynamic-content login"
            OnReload="dynamic_LoadControls"
            ID="dynMid" />
 
    </ra:Panel>
</ra:Window>
