/*
Template Name: Velzon - Admin & Dashboard Template
Author: Themesbrand
Version: 4.3.0
Website: https://Themesbrand.com/
Contact: Themesbrand@gmail.com
File: Common Plugins Js File
*/

var scriptEl = document.querySelector('script[src*="plugins.js"]');
var basePath = scriptEl ? scriptEl.src.split('js/plugins.js')[0] : '';

if (document.querySelectorAll("[toast-list]").length > 0 || document.querySelectorAll('[data-choices]').length > 0 || document.querySelectorAll("[data-provider]").length > 0) { 
  document.writeln("<script type='text/javascript' src='https://cdn.jsdelivr.net/npm/toastify-js'></script>");
  document.writeln("<script type='text/javascript' src='" + basePath + "libs/choices.js/public/assets/scripts/choices.min.js'></script>");
  document.writeln("<script type='text/javascript' src='" + basePath + "libs/flatpickr/flatpickr.min.js'></script>");    
}