﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_EffectBorder" %>

<ra:Label 
    runat="server" 
    ID="lbl" 
    style="padding:15px;margin:15px;"
    Tag="div"
    Text="Watch me as you click button" />

<ra:Button 
    runat="server" 
    ID="btn" 
    Text="Click me..." 
    OnClick="btn_Click" />
