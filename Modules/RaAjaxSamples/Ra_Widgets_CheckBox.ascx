<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Ra_Widgets_CheckBox" %>

<ra:CheckBox 
    runat="server" 
    id="chk" 
    OnCheckedChanged="chk_CheckedChanged"
    Text="Click me..." />