<%@ Assembly 
    Name="Viewport" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Viewport.Main" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<ra:ResizeHandler 
    runat="server" 
    OnResized="res_Resized"
    ID="res" />

<ra:GlobalUpdater 
    runat="server" 
    MaxOpacity="0.7" 
    Delay="500"
    CssClass="wait-gif"
    ID="updater">
    &nbsp;
</ra:GlobalUpdater>

<ra:Timer 
    runat="server" 
    id="timer" 
    Duration="600000"
    OnTick="timer_Tick" />

<div style="background-color: #6DD0FF;width:100%;height:100%;overflow:auto;">
    <ra:Window 
        runat="server" 
        style="z-index:1000;position:absolute;left:7px;top:6px;"
        Visible="false"
        CssClass="light-window"
        OnClosed="maxiWindow_Closed"
        id="maxiWindow">
        
        <ra:LinkButton
            runat="server"
            id="close"
            style="position:absolute;right:45px;bottom:20px;"
            OnClick="close_Click"
            Text='<%# Language.Instance["Close", null, "Close"] %>' />
        
        <ra:Dynamic 
            runat="server" 
            style="height:350px;"
            CssClass="dynamic-content dynamic-content-no-overflow"
            OnReload="dynamic_LoadControls"
            id="dynMaxi" />

        <ra:BehaviorObscurable 
            runat="server" 
            Opacity="0.7"
            Color="Blue"
            id="obscurerMaxi" />

    </ra:Window>

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

    <ra:Window 
        runat="server" 
        style="z-index:1000;position:absolute;right:10px;top:10px;overflow:hidden;"
        Visible="false"
        CssClass="window"
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
                OnMouseOver="zoomImage_MouseOver"
                AlternateText="Zoom here"
                ImageUrl='<%# "media/skins/" + GetCssRootFolder() + "/Images/zoom.png" %>'
                id="zoomImage" />
        </div>
    </ra:Window>

    <!-- Top panel... -->
    <ra:Panel 
        runat="server" 
        ID="pnlTop" 
        Closable="false" 
        Movable="false" 
        CssClass="topHeader"
        Caption="Top parts">
        <a href="~/" runat="server" class="noHoverLink">
            <span class="logo">&nbsp;</span>
        </a>
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
        <ra:Dynamic 
            runat="server" 
            CssClass="dynamic-content"
            OnReload="dynamic_LoadControls"
            ID="dynTop" />  

    </ra:Panel>

    <!-- bread crumb -->
    <div 
        style="width:100%;overflow:hidden;" 
        id="something">
        <ra:Panel 
           runat="server" 
           ID="customBreadParent" 
           CssClass="bread-crumb-parent" />
     </div>

    <!-- Left window... -->
    <ra:Window 
        runat="server" 
        ID="wndLeft" 
        Closable="false" 
        CssClass="light-window"
        Movable="false" 
        style="position:absolute;left:10px;top:130px;"
        Caption="Left parts">
        <ra:Dynamic 
            runat="server" 
            style="width:160px;"
            CssClass="dynamic-content dynamic-content-no-overflow"
            OnReload="dynamic_LoadControls"
            ID="dynLeft" />
    </ra:Window>

    <!-- Center window... -->
    <ra:Window 
        runat="server" 
        ID="wndMid" 
        Closable="false" 
        CssClass="light-window no-background"
        Movable="false" 
        style="position:absolute;left:215px;top:130px;"
        Caption="Center parts">
        
        <ra:Panel runat="server" id="midWrp">

            <ra:TabControl runat="server" id="tab" CssClass="tab-light">

                <ra:TabView Caption="Main" runat="server" id="tab1">
                    <ra:Dynamic 
                        runat="server" 
                        CssClass="dynamic-content"
                        OnReload="dynamic_LoadControls" />
                </ra:TabView>

                <ra:TabView Caption="" Visible="false" runat="server" id="tab2">
                    <ra:Dynamic 
                        runat="server" 
                        CssClass="dynamic-content"
                        OnReload="dynamic_LoadControls" />
                </ra:TabView>

                <ra:TabView Caption="" Visible="false" runat="server" id="tab3">
                    <ra:Dynamic 
                        runat="server" 
                        CssClass="dynamic-content"
                        OnReload="dynamic_LoadControls" />
                </ra:TabView>

                <ra:TabView Caption="" Visible="false" runat="server" id="tab4">
                    <ra:Dynamic 
                        runat="server" 
                        CssClass="dynamic-content"
                        OnReload="dynamic_LoadControls" />
                </ra:TabView>

                <ra:TabView Caption="" Visible="false" runat="server" id="tab5">
                    <ra:Dynamic 
                        runat="server" 
                        CssClass="dynamic-content"
                        OnReload="dynamic_LoadControls" />
                </ra:TabView>

            </ra:TabControl>

        </ra:Panel>
    </ra:Window>
</div>
