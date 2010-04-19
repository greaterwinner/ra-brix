<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Ra_Widgets_Button" %>

<ra:ExtButton 
    runat="server" 
    id="btn" 
    OnClick="btn_Click"
    Text="Click me..." />