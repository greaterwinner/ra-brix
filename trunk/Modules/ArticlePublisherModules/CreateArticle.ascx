<%@ Assembly 
    Name="ArticlePublisherModules" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="ArticlePublisherModules.CreateArticle" %>

<div class="createArticle">
    <ra:InPlaceEdit 
        runat="server" 
        id="header" 
        Tag="h3"
        CssClass="edit"
        Text="Header of article" />

    <div style="margin:5px;">
        <h3>Tags</h3>
        <ra:TextBox 
            runat="server" 
            style="width:100%;"
            id="tags" />
    </div>

    <div style="margin:5px;">
        <h3>Ingress</h3>
        <ra:TextArea 
            runat="server" 
            id="ingress" />
    </div>
    <h3>Image</h3>
    <ra:Image 
        runat="server" 
        AlternateText="Image for article, will be scaled to fit"
        ImageUrl="media/empty-article-image.png"
        style="cursor:pointer;width:150px;"
        OnClick="img_Click"
        id="img" />

    <h3 style="margin-bottom:-20px;z-index:200;">Content</h3>
    <ra:RichEdit
        ID="editor" 
        CssClass="editor"
        OnGetImageDialog="editor_GetImageDialog"
        OnGetResourceDialog="editor_GetResourceDialog"
        OnGetHyperLinkDialog="editor_GetHyperLinkDialog"
        CtrlKeys="s,g"
        OnCtrlKey="editor_CtrlKeys"
        OnGetExtraToolbarControls="editor_GetExtraToolbarControls"
        runat="server" />
</div>