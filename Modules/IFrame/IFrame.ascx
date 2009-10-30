<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="IFrameModules.IFrame" %>

<ra:Panel 
    runat="server" 
    id="iframeWrp" 
    style="width:100%;height:100%;">
    <iframe 
        id="iframe"
        style="width:100%;height:100%;" 
        frameborder="0" 
        runat="server">
    </iframe>
</ra:Panel>