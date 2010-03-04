<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_CheckBox" %>

<ra:CheckBox 
    runat="server" 
    ID="chk" 
    Text="Check me up..." 
    OnCheckedChanged="chk_CheckedChanged" />

