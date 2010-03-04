﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_TextArea" %>

<ra:TextArea 
    runat="server" 
    ID="txt" 
    OnKeyUp="txt_KeyPress"
    Text="Type into me" />

<br />

<ra:Label 
    runat="server" 
    ID="lbl" 
    Text="Watch me..." />

