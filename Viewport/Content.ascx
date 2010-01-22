<%@ Assembly 
    Name="Viewport" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Viewport.Content" %>

<ra:Timer 
    runat="server" 
    id="timer" 
    Duration="600000"
    OnTick="timer_Tick" />

<ra:Window 
    runat="server" 
    style="z-index:2000;position:fixed;left:200px;top:50px;width:350px;"
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

<ra:Window 
    runat="server" 
    style="z-index:1000;position:absolute;right:50px;top:10px;overflow:hidden;"
    Visible="false"
    CssClass="window"
    Caption="&nbsp;"
    OnMouseOut="popupWindow2_MouseOut"
    OnClosed="popupWindow2_Closed"
    id="popupWindow2">
    <div style="position:relative;">
        <ra:Dynamic 
            runat="server" 
            CssClass="dynamic-content dynamic-content-no-overflow"
            OnReload="dynamic_LoadControls"
            id="dynPopup2" />
        <ra:Image 
            runat="server" 
            OnClick="zoomImage_MouseOver"
            AlternateText="Zoom here"
            CssClass="zoomImage"
            Text="&nbsp;"
            ImageUrl='<%# "media/skins/" + GetCssRootFolder() + "/Images/zoom.png" %>'
            id="zoomImage" />
    </div>
</ra:Window>

<div id="paper">
    <!-- Top panel... -->
    <ra:Window 
        runat="server" 
        CssClass="light-window informationPanel"
        Visible="false"
        id="informationPanel">
        <audio 
            src='<%# "media/skins/" + GetCssRootFolder() + "/Sounds/ping.wav" %>'
            id="pingSound">
        </audio>
        <ra:Label 
            runat="server" 
            CssClass="informationLabel"
            id="informationLabel" />
        <ra:ExtButton 
            runat="server" 
            id="handleInformationEvt" 
            style="position:absolute;bottom:2px;right:2px;"
            OnClick="handleInformationEvt_Click" />
    </ra:Window>

    <div id="header">
        <ra:Panel 
            runat="server" 
            id="pnlLogo" 
            CssClass="logoRaBrix">
            <a 
                href='<%=GetMostWantedResponseUrl() %>' 
                title='<%=GetMostWantedResponseTooltip() %>' 
                class="logoImg">&nbsp;</a>
        </ra:Panel>
        <ra:Dynamic 
            runat="server" 
            CssClass="dynamic-content"
            OnReload="dynamic_LoadControls"
            ID="dynTop" />
    </div>

    <!-- bread crumb -->
    <div 
        style="width:100%;overflow:hidden;margin-top:-10px;position:relative;z-index:100;" 
        id="something">
        <ra:Panel 
           runat="server" 
           ID="customBreadParent" 
           CssClass="bread-crumb-parent" />
     </div>

    <!-- Left window... -->
    <div style="padding:0 15px 0 15px;overflow:auto;">
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
        <div style="float:left;width:796px;position:relative;margin-left:8px;">
            <ra:Window 
                runat="server" 
                CssClass="light-window"
                id="wndCenter">
                <ra:Dynamic 
                    runat="server" 
                    CssClass="dynamic-content"
                    ID="dynMid"
                    style="min-height:320px;"
                    OnReload="dynamic_LoadControls" />
            </ra:Window>
        </div>
    </div>
    <p class="copyright">
        Copyright 2009 - <a href="mailto:thomas@ra-ajax.org">Ra-Software, Inc.</a>
        <br />
        <a href="http://ra-brix.org">Ra-Brix</a> is <a href="http://www.gnu.org/licenses/gpl.html">Free and Open Source Software</a>.
    </p>
</div>

