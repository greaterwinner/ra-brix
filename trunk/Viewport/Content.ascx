<%@ Assembly 
    Name="Viewport" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Viewport.Content" %>

<ra:Window 
    runat="server" 
    style="z-index:1000;position:absolute;left:200px;top:50px;width:350px;"
    Visible="false"
    CssClass="window"
    OnClosed="popupWindow_Closed"
    id="popupWindow">

    <ra:Dynamic 
        runat="server" 
        style="height:350px;"
        CssClass="dynamic-content dynamic-content-no-overflow"
        OnReload="dynamic_LoadControls"
        id="dynPopup" />

    <ra:BehaviorObscurable 
        runat="server" 
        id="obscurer" />

</ra:Window>

<!-- Top panel... -->
<ra:Panel 
    runat="server" 
    CssClass="informationPanel"
    id="informationPanel">
    <audio 
        src='<%# "media/skins/" + GetCssRootFolder() + "/Sounds/ping.wav" %>'
        id="pingSound">
    </audio>
    <div class="informationPanelInner">
        <ra:Label 
            runat="server" 
            CssClass="informationLabel"
            id="informationLabel" />
        <ra:ExtButton 
            runat="server" 
            id="handleInformationEvt" 
            style="position:absolute;bottom:2px;right:2px;"
            OnClick="handleInformationEvt_Click" />
    </div>

</ra:Panel>
<div id="paper">
    <div id="header">
        <ra:Panel 
            runat="server" 
            id="pnlLogo" 
            style="opacity:0.3;"
            CssClass="logoRaBrix">
            <a href="~/" runat="server" id="logo" class="logoImg">&nbsp;</a>
            <ra:BehaviorUnveiler 
                runat="server" 
                id="logoUnveiler" />
        </ra:Panel>
        <ra:Dynamic 
            runat="server" 
            CssClass="dynamic-content"
            OnReload="dynamic_LoadControls"
            ID="dynTop" />
    </div>

    <!-- bread crumb -->
    <div 
        style="width:100%;overflow:hidden;margin-top:10px;" 
        id="something">
        <ra:Panel 
           runat="server" 
           ID="customBreadParent" 
           CssClass="bread-crumb-parent" />
     </div>

    <!-- Left window... -->
    <div style="padding:0 15px 0 15px;">
        <ra:Window 
            runat="server" 
            style="float:left;"
            CssClass="light-window"
            id="wndMenu">
            <ra:Dynamic 
                runat="server" 
                CssClass="dynamic-content dynamic-content-no-overflow"
                style="width:160px;height:320px;"
                OnReload="dynamic_LoadControls"
                ID="dynLeft" />
        </ra:Window>

        <!-- Center window... -->
        <div style="float:left;min-height:500px;width:804px;position:relative;">
            <ra:Dynamic 
                runat="server" 
                CssClass="dynamic-content"
                ID="dynMid"
                style="min-height:300px;padding:5px;"
                OnReload="dynamic_LoadControls" />
        </div>
    </div>
    <p class="copyright">
        Copyright 2009 - <a href="mailto:thomas@ra-ajax.org">Thomas Hansen</a>. 
        <br />
        All text is licensed under <a href="http://creativecommons.org/licenses/by-sa/3.0/">CC Share-Alike</a>. 
        <a href="http://ra-brix.org">Ra-Brix</a> is Affero GPL licensed.
    </p>
</div>

