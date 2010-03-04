<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_Calendar" %>

<ra:Label 
    runat="server" 
    Text="Changes..."
    ID="lbl" />

<ra:Calendar 
    runat="server" 
    style="width:200px;" 
    OnSelectedValueChanged="cal_SelectedValueChanged"
    ID="cal" />