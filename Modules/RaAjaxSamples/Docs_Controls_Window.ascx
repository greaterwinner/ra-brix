﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_Window" %>

<ra:ExtButton 
    runat="server" 
    ID="btn" 
    Text="Show Window" 
    OnClick="btn_Click" />

<ra:Window 
    runat="server" 
    ID="wnd" 
    Visible="false"
    Caption="Window caption"
    style="position:fixed;top:250px;left:250px;z-index:1500;width:250px;">

    <div style="height:150px;padding:50px;">
        Here's an example of a Window.
    </div>
    <ra:BehaviorObscurable 
        runat="server" 
        ID="obscurr" />

</ra:Window>