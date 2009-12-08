<%@ Assembly 
    Name="UsersModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="UsersModules.CreateNewUsers" %>

Number of users

<ra:TextBox 
    runat="server"
    id="noUsers" />
    
<ra:ExtButton 
    runat="server"
    id="save"
    Text="Create" 
    OnClick="save_Click"/>