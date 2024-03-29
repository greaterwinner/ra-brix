﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_HiddenField" %>

<ra:Button 
    runat="server" 
    ID="btn" 
    Text="Set HiddenField to DateTime.Now" 
    OnClick="btn_Click" />

<ra:HiddenField 
    runat="server" 
    Value="Not set yet...."
    ID="hid" />

<ra:Button 
    runat="server" 
    ID="btn2" 
    Text="Retrieve value of HiddenField" 
    OnClick="btn2_Click" />

