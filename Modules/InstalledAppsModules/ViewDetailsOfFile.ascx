<%@ Assembly 
    Name="InstalledAppsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="InstalledAppsModules.ViewDetailsOfFile" %>

<ra:Label 
    runat="server" 
    id="headerLbl" 
    Tag="h1" />

<ra:Tree 
    runat="server" 
    style="width:30%;height:100%;float:left;min-height:250px;" 
    OnSelectedNodeChanged="menu_SelectedNodeChanged"
    CssClass="tree"
    ID="tree">

    <ra:TreeNodes 
        runat="server" 
        ID="root" />

</ra:Tree>
<ra:Panel 
    runat="server" 
    style="float:left;width:60%;border:1px solid #CCD1FF;margin-left:5px;padding:5px;min-height:250px;overflow:auto;"
    id="info">
    <ra:Panel 
        runat="server" 
        id="infoContentTip">
        <%=LanguageRecords.Language.Instance["SelectNodeFromLeft", null, "Select node from left to view its details..."] %>
    </ra:Panel>
    <ra:Panel 
        runat="server" 
        Visible="false"
        id="infoContent">
        <ra:Label 
            runat="server" 
            Tag="h3"
            id="lblFullName" />
        <ext:Grid 
            runat="server" 
            CssClass="grid smaller"
            PageSize="8"
            id="grd" />
    </ra:Panel>
</ra:Panel>
