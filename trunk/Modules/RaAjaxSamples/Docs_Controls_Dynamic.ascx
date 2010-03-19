﻿<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_Dynamic" %>

<ra:SelectList 
    runat="server" 
    OnSelectedIndexChanged="sel_SelectedIndexChanged"
    ID="sel">
    <ra:ListItem 
        Text="Select control..." 
        Value="nothing" />
    <ra:ListItem 
        Text="Button" 
        Value="Ra.Widgets.Button.ascx" />
    <ra:ListItem 
        Text="CheckBox" 
        Value="Ra.Widgets.CheckBox.ascx" />
</ra:SelectList>

<ra:Dynamic 
    runat="server" 
    OnReload="dyn_Reload"
    style="padding:15px;border:dashed 2px #999;margin:25px;"
    ID="dyn" />