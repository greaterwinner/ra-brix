﻿/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the GPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */


(function(){

// Creating class
Ra.RichEdit = Ra.klass();


// Inheriting from Ra.Control
Ra.extend(Ra.RichEdit.prototype, Ra.Control.prototype);


// Creating IMPLEMENTATION of class
Ra.extend(Ra.RichEdit.prototype, {

  init: function(el, opt) {
    this.initControl(el, opt);
    this.initRichEdit();
  },

  getValueElement: function() {
    return Ra.$(this.element.id + '__VALUE');
  },

  getSelectedElement: function() {
    return Ra.$(this.element.id + '__SELTEXT');
  },

  initRichEdit: function() {
    var lbl = Ra.$(this.element.id + '_LBL');
    
    lbl.contentEditable = true;
    this.options.label = lbl;
    this.options.ctrl = lbl;
    this.isDesign = true;
    this.modifyHeight();

    // We must have a preSerializer handler to make sure we serialize 
    // the value back to the server
    Ra.Form.preSerializers.push({handler: this.onPreSerializeForm, context: this});

    var clid = this.element.id;
    var T = this;

    Ra.$(clid + 'bold').observe('click', function(){
      return this._execCommand('bold');
    }, this);

    Ra.$(clid + 'italic').observe('click', function(){
      return this._execCommand('italic');
    }, this);

    Ra.$(clid + 'underline').observe('click', function(){
      return this._execCommand('underline');
    }, this);

    Ra.$(clid + 'justifyleft').observe('click', function(){
      return this._execCommand('justifyleft');
    }, this);

    Ra.$(clid + 'justifycenter').observe('click', function(){
      return this._execCommand('justifycenter');
    }, this);

    Ra.$(clid + 'justifyright').observe('click', function(){
      return this._execCommand('justifyright');
    }, this);

    Ra.$(clid + 'justifyfull').observe('click', function(){
      return this._execCommand('justifyfull');
    }, this);

    Ra.$(clid + 'insertunorderedlist').observe('click', function(){
      return this._execCommand('insertunorderedlist');
    }, this);

    Ra.$(clid + 'insertorderedlist').observe('click', function(){
      return this._execCommand('insertorderedlist');
    }, this);

    Ra.$(clid + 'indent').observe('click', function(){
      return this._execCommand('indent');
    }, this);

    Ra.$(clid + 'outdent').observe('click', function(){
      return this._execCommand('outdent');
    }, this);
    
    var image = Ra.$(clid + 'image');
    if(image) {
      image.observe('click', function(){
        this.callback('getImage');
      }, this);
    }

    var hpl = Ra.$(clid + 'hyperlink');
    if(hpl) {
      hpl.observe('click', function(){
        this.callback('getHyperLink');
      }, this);
    }

    Ra.$(clid + 'selectTextType').observe('change', function(){
      var opt = Ra.$(clid + 'selectTextType');
      var val = opt.options[opt.selectedIndex].value;
      this._execCommand('formatblock', '<' + val + '>');
      opt.selectedIndex = 0;
      return false;
    }, this);

    Ra.$(clid + 'selectFontName').observe('change', function(){
      var opt = Ra.$(clid + 'selectFontName');
      var val = opt.options[opt.selectedIndex].value;
      this._execCommand('fontname', val);
      opt.selectedIndex = 0;
      return false;
    }, this);

    Ra.$(clid + 'selectFontSize').observe('change', function(){
      var opt = Ra.$(clid + 'selectFontSize');
      var val = opt.options[opt.selectedIndex].value;
      this._execCommand('fontsize', val);
      opt.selectedIndex = 0;
      return false;
    }, this);

    Ra.$(clid + 'design').observe('click', function(){
      this.options.ctrl.innerHTML = this.getValueElement().value;
      this.getValueElement().style.display = 'none';
      this.options.ctrl.style.display = '';
      Ra.$(clid + 'design').className = 'designButton activeTypeButton';
      Ra.$(clid + 'html').className = 'htmlButton';
      this.enableControls(true);
      this.isDesign = true;
      return false;
    }, this);

    Ra.$(clid + 'html').observe('click', function(){
      this.getValueElement().value = this.options.ctrl.innerHTML;
      this.getValueElement().style.display = '';
      this.options.ctrl.style.display = 'none';
      Ra.$(clid + 'design').className = 'designButton';
      Ra.$(clid + 'html').className = 'htmlButton activeTypeButton';
      this.enableControls(false);
      this.isDesign = false;
      return false;
    }, this);

    // We must handle focus and blur to STORE the old range of the editable element
    this.options.ctrl.observe('focus', this.onFocus, this);
    this.options.ctrl.observe('blur', this.onBlur, this);
  },

  onBlur: function() {
    this._selection = this._getSelection();
  },

  onFocus: function() {
    this._restoreSelection(this._selection);
    this._selection = null;
  },

  _getSelection: function() {
    if (window.getSelection) {
      var selection = window.getSelection();
      if (selection.rangeCount > 0) {
        var selectedRange = selection.getRangeAt(0);
        return selectedRange.cloneRange();
      } else {
        return null;
      }
    } else if (document.selection) {
      var selection = document.selection;
      if (selection.type.toLowerCase() == 'text') {
        return selection.createRange().getBookmark();
      } else {
        return null;
      }
    } else {
      return null;
    }
  },

  _restoreSelection: function(sl) {
    if (sl) {
      if (window.getSelection) {
        var selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(sl);
      } else if (document.selection && document.body.createTextRange) {
        var range = document.body.createTextRange();
        range.moveToBookmark(sl);
        range.select();
      }
    }
  },

  Paste: function(value) {
    var sel = this._selection;
    if( sel ) {
      this.options.label.focus();

      // document.execCommand will insert HTML as HTML and NOT as XHTML so
      // we can't use that one for our purposes...!
      // Therefor the "funny hack" here for inserting HTML text...
      var newContent = sel.createContextualFragment(value); // User has a selection
      sel.deleteContents();
      sel.insertNode(newContent);
      this._selection = null;
    } else {
      this.options.label.focus();
      this._execCommand('insertHTML', value);
    }
  },

  modifyHeight: function() {
    var parHeight = this.options.ctrl.parentNode.clientHeight;
    var fHeight = this.options.ctrl.parentNode.childNodes[0].clientHeight;
    var tHeight = this.options.ctrl.parentNode.childNodes[4].clientHeight;
    var nHeight = (parHeight - (fHeight + tHeight + 5));
    this.options.ctrl.style.height = nHeight + 'px';
    this.getValueElement().style.height = nHeight + 'px';
  },

  Text: function(val) {
    this.options.ctrl.setContent(val);
    this.getValueElement().value = val;
  },

  enableControls: function(enable) {
    var clid = this.element.id;
    var arr = ['bold', 'italic', 'underline', 'justifyleft', 
      'justifycenter', 'justifyright', 'justifyfull', 
      'insertunorderedlist', 'insertorderedlist', 'selectTextType',
      'selectFontName', 'selectFontSize',
      'indent', 'outdent', 'image', 'hyperlink'];
    for (var idx = 0; idx < arr.length; idx++) {
      Ra.$(clid + arr[idx]).disabled = enable ? false : true;
      Ra.$(clid + arr[idx]).setOpacity(enable ? 1 : 0.2);
    }
  },

  _execCommand: function(cmd, params) {
      document.execCommand(cmd, false, params);
      this.getValueElement().value = this.options.ctrl.innerHTML;
      this.options.ctrl.focus();
      return false;
  },

  onPreSerializeForm: function() {
    // This function will be called just before the form is serialized 
    // before pushing back a request to the server so here we set the 
    // value of our hidden value field(s)

    var toRemove = [{from:'<br>', to:'<br/>'}];
    var selection = this._selection;
    if( selection ) {

      // Creating a "dummy HTML DOM element" which we append the
      // range contents into for then later retrieve the XHTML by 
      // using the innerHTML on that "dummy element"
      var rng = selection.cloneContents();
      var el = document.createElement('div');
      el.appendChild(rng);
      val = el.innerHTML;

      // Looping through swapping non-XHTML-conforming HTML with CONFORMING HTML
      for( var idx = 0; idx < toRemove.length; idx++ ) {
        while(true) {
          if( val.indexOf(toRemove[idx].from) == -1 )
            break;
          val = val.replace(toRemove[idx].from, toRemove[idx].to);
        }
      }

      // Now we have the "selected HTML string" of the RichEdit and we can set the 
      // hidden fields value to that value to make sure it goes back to the server...
      this.getSelectedElement().value = val;
    } else {
      // No current selection, setting the hidden field value to EMPTY!
      this.getSelectedElement().value = '';
    }

    // Looping through swapping non-XHTML-conforming HTML with CONFORMING HTML
    var val = this.isDesign ? this.options.label.getContent() : this.getValueElement().value;
    for( var idx = 0; idx < toRemove.length; idx++ ) {
      while(true) {
        if( val.indexOf(toRemove[idx].from) == -1 )
          break;
        val = val.replace(toRemove[idx].from, toRemove[idx].to);
      }
    }

    // Setting the hidden field value to get it back to the server
    this.getValueElement().value = val;
  },

  getValue: function() {
    return this.options.ctrl.getContent();
  },

  destroyThis: function() {

    // Removing the Form Pre-Serialization handler
    var idxOfRemove = 0;
    for( var idx = 0; idx < Ra.Form.preSerializers.length; idx++) {
      if( Ra.Form.preSerializers[idx].handler == this.onPreSerializeForm )
        break;
      idxOfRemove += 1;
    }
    Ra.Form.preSerializers.splice(idxOfRemove, 1);

    // Calling base
    this._destroyThisControl();
  }
});