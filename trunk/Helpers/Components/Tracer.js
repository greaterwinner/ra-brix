/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the LGPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */


(function(){

// Creating class
Ra.Tracer = Ra.klass();


// Inheriting from Ra.Control
Ra.extend(Ra.Tracer.prototype, Ra.Control.prototype);

Ra.Tracer._current = null;


// Creating IMPLEMENTATION of class
Ra.extend(Ra.Tracer.prototype, {

  init: function(el, opt) {
    this.initControl(el, opt);
    if( Ra.Tracer._current ) {
      throw "Can't have more than one active GlobalUpdater at the same time on the same page!";
    }
    Ra.Tracer._current = this;
    this._oldCallback = Ra.Form.prototype.callback;
    Ra.Form.prototype.callback = this.callback;
    this.element.innerHTML = Ra.$('__VIEWSTATE').value.length;
  },

  callback: function() {
    var T = Ra.Tracer._current;
    T._widget = this;
    T._oldCallback.apply(this, [T.onSuccess, T.onError]);
  },

  onSuccess: function(response) {
    var T = Ra.Tracer._current;
    var widget = T._widget;
    if( !widget.options.callingContext ) {
      widget.options.onFinished(response);
    } else {
      widget.options.onFinished.call(widget.options.callingContext, response);
    }
    T.element.innerHTML = Ra.$('__VIEWSTATE').value.length;
  },

  onError: function(status, response) {
    var T = Ra.Tracer._current;
    if( !widget.options.callingContext ) {
      widget.options.onError(status, response);
    } else {
      widget.options.onError.call(widget.options.callingContext, status, response);
    }
  },

  destroyThis: function() {

    // Restoring old callback
    Ra.Form.prototype.callback = this._oldCallback;
    Ra.Tracer._current = null;

    // Forward call to allow overriding in inherited classes...
    this._destroyThisControl();
  }
});
})();
