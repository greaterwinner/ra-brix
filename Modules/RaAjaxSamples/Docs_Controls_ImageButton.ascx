<%@ Assembly 
    Name="RaAjaxSamples" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="RaAjaxSamples.Docs_Controls_ImageButton" %>

<p>
    Click button to change image...
</p>
<ra:ImageButton 
    runat="server" 
    ImageUrl="media/Kariem.jpg" 
    AlternateText="Image"
    OnClick="btn_Click"
    ID="img" />


