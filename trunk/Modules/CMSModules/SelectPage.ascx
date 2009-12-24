<%@ Assembly 
    Name="CMSModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="CMSModules.SelectPage" %>

<%@ Import 
    Namespace="LanguageRecords"%>

<div style="height:100%;overflow:auto;">
    <ra:Tree 
        runat="server" 
        CssClass="tree"
        UseRichAnimations="true"
        OnSelectedNodeChanged="menu_SelectedNodeChanged"
        ID="tree">

        <ra:TreeNodes 
            runat="server" 
            ID="root" />

    </ra:Tree>
</div>
