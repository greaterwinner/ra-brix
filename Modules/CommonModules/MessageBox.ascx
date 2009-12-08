<%@ Assembly 
    Name="CommonModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CommonModules.MessageBox" %>

<div style="position:relative;height:100%;">
    <div style="padding:5px;">
        <ra:Label 
            runat="server" 
            id="txt" />
        <div style="position:absolute;bottom:5px;right:5px;">
            <ra:ExtButton 
                runat="server" 
                id="submit" 
                style="float:left;"
                Text='<%#LanguageRecords.Language.Instance["OK", null, "OK"] %>' 
                OnClick="submit_Click" />
            <ra:ExtButton 
                runat="server" 
                id="cancel" 
                style="float:left;"
                Text='<%#LanguageRecords.Language.Instance["Cancel", null, "Cancel"] %>' 
                OnClick="cancel_Click" />
        </div>
    </div>
</div>

