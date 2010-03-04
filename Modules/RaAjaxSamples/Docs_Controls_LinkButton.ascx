<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_LinkButton" %>

<ra:LinkButton 
    runat="server" 
    ID="lnk" 
    OnClick="lnk_Click"
    Text="Click me..." />

