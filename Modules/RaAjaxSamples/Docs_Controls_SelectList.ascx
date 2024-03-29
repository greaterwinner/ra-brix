﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_SelectList" %>

<ra:Label 
    runat="server" 
    ID="lbl" 
    Text="Watch me as you select radio buttons" />

<br />

<ra:SelectList 
    runat="server" 
    ID="sel" 
    OnSelectedIndexChanged="SelectChanged">
    <ra:ListItem Text="Highlight" Value="Highlight" />
    <ra:ListItem Text="Fade In" Value="FadeIn" />
</ra:SelectList>