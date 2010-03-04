<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_ExtButton" %>

<ra:ExtButton 
    runat="server" 
    ID="btn" 
    Text="Click me" 
    OnClick="btn_Click" />