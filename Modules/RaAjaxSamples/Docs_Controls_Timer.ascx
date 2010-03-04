<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_Timer" %>

<div style="position:relative;height:270px;border:dotted 1px #999;">
<ra:Label 
    runat="server" 
    Text="Changes..."
    style="display:block;padding:5px;font-size:14px;position:absolute;"
    ID="lbl" />
</div>

<ra:Button 
    runat="server" 
    ID="btn" 
    Text="Click me..."
    OnClick="btn_Click" />

<ra:Timer 
    runat="server" 
    ID="timer" 
    Duration="1000"
    OnTick="timer_Tick" />
