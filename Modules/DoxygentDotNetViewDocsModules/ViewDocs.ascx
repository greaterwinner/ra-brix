<%@ Assembly 
    Name="DoxygentDotNetViewDocsModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="DoxygentDotNetViewDocsModules.ViewDocs" %>

<div style="overflow:auto;height:100%;">
    <ra:Panel 
        runat="server"
        DefaultWidget="filterSearch"
        id="filterWrp">
        <ra:TextBox 
            runat="server"
            CssClass="filter"
            AccessKey="F"
            style="opacity:0.3;position:absolute;top:0px;right:20px;z-index:100;"
            id="filter">
            <ra:BehaviorUnveiler 
                runat="server" 
                id="unveil" />
        </ra:TextBox>
        <div style="height:10px;">&nbsp;</div>
        <ra:Button
            runat="server"
            id="filterSearch"
            style="display:none;"
            OnClick="filterSearch_Click" />
    </ra:Panel>
    <ra:Tree 
        runat="server" 
        OnSelectedNodeChanged="tree_SelectedNodeChanged"
        UseRichAnimations="true"
        ID="tree">
        <ra:TreeNodes 
            runat="server" 
            ID="topNodes">
            <ra:TreeNode 
                runat="server" 
                ID="tutorials" 
                Text="Tutorials">
                <ra:TreeNodes 
                    runat="server" 
                    ID="rootTutorials" />
            </ra:TreeNode>
            <ra:TreeNode 
                runat="server" 
                ID="rootNode" 
                Text="Classes Reference Documentation">
                <ra:TreeNodes 
                    runat="server" 
                    ID="root" />
            </ra:TreeNode>
        </ra:TreeNodes>
    </ra:Tree>
</div>