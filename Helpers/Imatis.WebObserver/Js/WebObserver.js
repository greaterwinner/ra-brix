/*
 * Ra-Ajax - A Managed Ajax Library for ASP.NET and Mono
 * Copyright 2008 - 2009 - Thomas Hansen thomas@ra-ajax.org
 * This code is licensed under the LGPL version 3 which 
 * can be found in the license.txt file on disc.
 *
 */


(function() {
Imatis = {};

Imatis.WebObserver = Ra.klass();

Ra.extend(Imatis.WebObserver.prototype, Ra.Control.prototype);

Ra.extend(Imatis.WebObserver.prototype, {
   init: function(el, opt) {
      this.initControl(el, opt);
      this.options = Ra.extend({
         dataURL:''
      }, this.options || {});
      this.canvas = document.getElementById(el);
      initCanvas(this.canvas, opt);
   },
   
   Width: function(value) {
   },

   Height: function(value) {
   },
   
   DataURL: function(value) {
      this.options.dataURL = value;
      initCanvas(this.canvas, this.options);
   },

   destroyThis: function() {
       // Calling base
       this._destroyThisControl();
   }
});
})();
