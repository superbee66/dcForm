/*	
	Watermark plugin for jQuery
	Version: 3.2.0
	http://jquery-watermark.googlecode.com/

	Copyright (c) 2009-2014 Todd Northrop
	http://www.speednet.biz/
	
	August 16, 2014

	Requires:  jQuery 1.2.3+
	
	Dual licensed under the MIT or GPL Version 2 licenses.
	See mit-license.txt and gpl2-license.txt in the project root for details.
------------------------------------------------------*/

( function ( jQuery, window, undefined ) {

var
	// String constants for data names
	dataFlag = "watermark",
	dataClass = "watermarkClass",
	dataFocus = "watermarkFocus",
	dataFormSubmit = "watermarkSubmit",
	dataMaxLen = "watermarkMaxLength",
	dataPassword = "watermarkPassword",
	dataText = "watermarkText",
	
	// Copy of native jQuery regex use to strip return characters from element value
	rreturn = /\r/g,
	
	// Used to determine if type attribute of input element is a non-text type (invalid)
	rInvalidType = /^(button|checkbox|hidden|image|radio|range|reset|submit)jQuery/i,

	// Includes only elements with watermark defined
	selWatermarkDefined = "input:data(" + dataFlag + "),textarea:data(" + dataFlag + ")",

	// Includes only elements capable of having watermark
	selWatermarkAble = ":watermarkable",
	
	// triggerFns:
	// Array of function names to look for in the global namespace.
	// Any such functions found will be hijacked to trigger a call to
	// hideAll() any time they are called.  The default value is the
	// ASP.NET function that validates the controls on the page
	// prior to a postback.
	// 
	// Am I missing other important trigger function(s) to look for?
	// Please leave me feedback:
	// http://code.google.com/p/jquery-watermark/issues/list
	triggerFns = [
		"Page_ClientValidate"
	],
	
	// Holds a value of true if a watermark was displayed since the last
	// hideAll() was executed. Avoids repeatedly calling hideAll().
	pageDirty = false,
	
	// Detects if the browser can handle native placeholders
	hasNativePlaceholder = ( "placeholder" in document.createElement( "input" ) );

// Best practice: this plugin adds only one method to the jQuery object.
// Also ensures that the watermark code is only added once.
jQuery.watermark = jQuery.watermark || {

	// Current version number of the plugin
	version: "3.2.0",
		
	runOnce: true,
	
	// Default options used when watermarks are instantiated.
	// Can be changed to affect the default behavior for all
	// new or updated watermarks.
	options: {
		
		// Default class name for all watermarks
		className: "watermark",
		
		// The default functionality is to clear the watermarks only from
		// the form being submitted.  Changing this option to true will
		// clear the watermarks from all forms on the page regardless
		// of which form is submitted.
		clearAllFormsOnSubmit: false,
		
		// If true, all watermarks will be hidden during the window's
		// beforeunload event. This is done mainly because WebKit
		// browsers remember the watermark text during navigation
		// and try to restore the watermark text after the user clicks
		// the Back button. We can avoid this by hiding the text before
		// the browser has a chance to save it. The regular unload event
		// was tried, but it seems the browser saves the text before
		// that event kicks off, because it didn't work.
		hideBeforeUnload: true,
		
		// Set to the name of an attribute in order to automatically
		// use the attribute's content as the watermark text.  For
		// example, set to "placeholder" to automatically use the
		// placeholder attribute content as the watermark text.
		// If the attribute specified does not have a value or is empty
		// no watermark will be set.  If textAttr is set to "placeholder"
		// and useNative is set to true, it has the effect of simply
		// adding support for the placeholder attribute to old browsers.
		textAttr: "",
		
		// If true, plugin will detect and use native browser support for
		// watermarks, if available. (i.e., placeholder attribute.)
		useNative: true
	},
	
	// Hide one or more watermarks by specifying any selector type
	// i.e., DOM element, string selector, jQuery matched set, etc.
	hide: function ( selector ) {
		jQuery( selector ).filter( selWatermarkDefined ).each(
			function () {
				jQuery.watermark._hide( jQuery( this ) );
			}
		);
	},
	
	// Internal use only.
	_hide: function ( jQueryinput, focus ) {
		var elem = jQueryinput[ 0 ],
			inputVal = ( elem.value || "" ).replace( rreturn, "" ),
			inputWm = jQueryinput.data( dataText ) || "",
			maxLen = jQueryinput.data( dataMaxLen ) || 0,
			className = jQueryinput.data( dataClass );
	
		if ( ( inputWm.length ) && ( inputVal == inputWm ) ) {
			elem.value = "";
			
			// Password type?
			if ( jQueryinput.data( dataPassword ) ) {
				
				if ( ( jQueryinput.attr( "type" ) || "" ) === "text" ) {
					var jQuerypwd = jQueryinput.data( dataPassword ) || [], 
						jQuerywrap = jQueryinput.parent() || [];
						
					if ( ( jQuerypwd.length ) && ( jQuerywrap.length ) ) {
						jQuerywrap[ 0 ].removeChild( jQueryinput[ 0 ] ); // Can't use jQuery methods, because they destroy data
						jQuerywrap[ 0 ].appendChild( jQuerypwd[ 0 ] );
						jQueryinput = jQuerypwd;
					}
				}
			}
			
			if ( maxLen ) {
				jQueryinput.attr( "maxLength", maxLen );
				jQueryinput.removeData( dataMaxLen );
			}
		
			if ( focus ) {
				jQueryinput.attr( "autocomplete", "off" );  // Avoid NS_ERROR_XPC_JS_THREW_STRING error in Firefox
				
				window.setTimeout(
					function () {
						jQueryinput.select();  // Fix missing cursor in IE
					}
				, 1 );
			}
		}
		
		className && jQueryinput.removeClass( className );
	},
	
	// Display one or more watermarks by specifying any selector type
	// i.e., DOM element, string selector, jQuery matched set, etc.
	// If conditions are not right for displaying a watermark, ensures that watermark is not shown.
	show: function ( selector ) {
		jQuery( selector ).filter( selWatermarkDefined ).each(
			function () {
				jQuery.watermark._show( jQuery( this ) );
			}
		);
	},
	
	// Internal use only.
	_show: function ( jQueryinput ) {
		var elem = jQueryinput[ 0 ],
			val = ( elem.value || "" ).replace( rreturn, "" ),
			text = jQueryinput.data( dataText ) || "",
			type = jQueryinput.attr( "type" ) || "",
			className = jQueryinput.data( dataClass );

		if ( ( ( val.length == 0 ) || ( val == text ) ) && ( !jQueryinput.data( dataFocus ) ) ) {
			pageDirty = true;
		
			// Password type?
			if ( jQueryinput.data( dataPassword ) ) {
				
				if ( type === "password" ) {
					var jQuerypwd = jQueryinput.data( dataPassword ) || [],
						jQuerywrap = jQueryinput.parent() || [];
						
					if ( ( jQuerypwd.length ) && ( jQuerywrap.length ) ) {
						jQuerywrap[ 0 ].removeChild( jQueryinput[ 0 ] ); // Can't use jQuery methods, because they destroy data
						jQuerywrap[ 0 ].appendChild( jQuerypwd[ 0 ] );
						jQueryinput = jQuerypwd;
						jQueryinput.attr( "maxLength", text.length );
						elem = jQueryinput[ 0 ];
					}
				}
			}
		
			// Ensure maxLength big enough to hold watermark (input of type="text" or type="search" only)
			if ( ( type === "text" ) || ( type === "search" ) ) {
				var maxLen = jQueryinput.attr( "maxLength" ) || 0;
				
				if ( ( maxLen > 0 ) && ( text.length > maxLen ) ) {
					jQueryinput.data( dataMaxLen, maxLen );
					jQueryinput.attr( "maxLength", text.length );
				}
			}
            
			className && jQueryinput.addClass( className );
			elem.value = text;
		}
		else {
			jQuery.watermark._hide( jQueryinput );
		}
	},
	
	// Hides all watermarks on the current page.
	// scope argument is optional; used by the submit handler to only
	// clear watermarks in the submitted form.
	hideAll: function ( scope ) {
		if ( pageDirty ) {
			jQuery.watermark.hide( jQuery( selWatermarkAble, scope ) );
			pageDirty = false;
		}
	},
	
	// Displays all watermarks on the current page.
	showAll: function () {
		jQuery.watermark.show( selWatermarkAble );
	}
};

jQuery.fn.watermark = jQuery.fn.watermark || function ( text, options ) {
	///	<summary>
	///		Set watermark text and class name on all input elements of type="text/password/search" and
	/// 	textareas within the matched set. If className is not specified in options, the default is
	/// 	"watermark". Within the matched set, only input elements with type="text/password/search"
	/// 	and textareas are affected; all other elements are ignored.
	///	</summary>
	///	<returns type="jQuery">
	///		Returns the original jQuery matched set (not just the input and texarea elements).
	/// </returns>
	///	<param name="text" type="String">
	///		Text to display as a watermark when the input or textarea element has an empty value and does not
	/// 	have focus. The first time watermark() is called on an element, if this argument is empty (or not
	/// 	a String type), then the watermark will have the net effect of only changing the class name when
	/// 	the input or textarea element's value is empty and it does not have focus.
	///	</param>
	///	<param name="options" type="Object" optional="true">
	///		Provides the ability to override the default watermark options (jQuery.watermark.options). For backward
	/// 	compatibility, if a string value is supplied, it is used as the class name that overrides the class
	/// 	name in jQuery.watermark.options.className. Properties include:
	/// 		className: When the watermark is visible, the element will be styled using this class name.
	/// 		useNative (Boolean or Function): Specifies if native browser support for watermarks will supersede
	/// 			plugin functionality. If useNative is a function, the return value from the function will
	/// 			determine if native support is used. The function is passed one argument -- a jQuery object
	/// 			containing the element being tested as the only element in its matched set -- and the DOM
	/// 			element being tested is the object on which the function is invoked (the value of "this").
	///	</param>
	/// <remarks>
	///		The effect of changing the text and class name on an input element is called a watermark because
	///		typically light gray text is used to provide a hint as to what type of input is required. However,
	///		the appearance of the watermark can be something completely different: simply change the CSS style
	///		pertaining to the supplied class name.
	///		
	///		The first time watermark() is called on an element, the watermark text and class name are initialized,
	///		and the focus and blur events are hooked in order to control the display of the watermark.  Also, as
	/// 	of version 3.0, drag and drop events are hooked to guard against dropped text being appended to the
	/// 	watermark.  If native watermark support is provided by the browser, it is detected and used, unless
	/// 	the useNative option is set to false.
	///		
	///		Subsequently, watermark() can be called again on an element in order to change the watermark text
	///		and/or class name, and it can also be called without any arguments in order to refresh the display.
	///		
	///		For example, after changing the value of the input or textarea element programmatically, watermark()
	/// 	should be called without any arguments to refresh the display, because the change event is only
	/// 	triggered by user actions, not by programmatic changes to an input or textarea element's value.
	/// 	
	/// 	The one exception to programmatic updates is for password input elements:  you are strongly cautioned
	/// 	against changing the value of a password input element programmatically (after the page loads).
	/// 	The reason is that some fairly hairy code is required behind the scenes to make the watermarks bypass
	/// 	IE security and switch back and forth between clear text (for watermarks) and obscured text (for
	/// 	passwords).  It is *possible* to make programmatic changes, but it must be done in a certain way, and
	/// 	overall it is not recommended.
	/// </remarks>
	
	if ( !this.length ) {
		return this;
	}
	
	var hasClass = false,
		hasText = ( typeof( text ) === "string" );
	
	if ( hasText ) {
		text = text.replace( rreturn, "" );
	}
	
	if ( typeof( options ) === "object" ) {
		hasClass = ( typeof( options.className ) === "string" );
		options = jQuery.extend( {}, jQuery.watermark.options, options );
	}
	else if ( typeof( options ) === "string" ) {
		hasClass = true;
		options = jQuery.extend( {}, jQuery.watermark.options, { className: options } );
	}
	else if ( typeof( text ) === "object" ) {
		options = jQuery.extend( {}, jQuery.watermark.options, text );
		text = "";
	}
	else {
		options = jQuery.watermark.options;
	}
	
	if ( typeof( options.useNative ) !== "function" ) {
		options.useNative = options.useNative? function () { return true; } : function () { return false; };
	}
	
	return this.each(
		function () {
			var jQueryinput = jQuery( this );
			
			if ( !jQueryinput.is( selWatermarkAble ) ) {
				return;
			}
			
			if ( options.textAttr ) {
				text = ( jQueryinput.attr( options.textAttr ) || "" ).replace( rreturn, "" );
				hasText = !!text;
			}
			
			// Watermark already initialized?
			if ( jQueryinput.data( dataFlag ) ) {
			
				// If re-defining text or class, first remove existing watermark, then make changes
				if ( hasText || hasClass ) {
					jQuery.watermark._hide( jQueryinput );
			
					if ( hasText ) {
						jQueryinput.data( dataText, text );
					}
					
					if ( hasClass ) {
						jQueryinput.data( dataClass, options.className );
					}
				}
			}
			else {
			
				// Detect and use native browser support, if enabled in options
				if (
					( hasNativePlaceholder )
					&& ( options.useNative.call( this, jQueryinput ) )
					&& ( ( jQueryinput.attr( "tagName" ) || "" ) !== "TEXTAREA" )
				) {
					// className is not set because current placeholder standard doesn't
					// have a separate class name property for placeholders (watermarks).
					if ( ( hasText ) && ( options.textAttr !== "placeholder" ) ) {
						jQueryinput.attr( "placeholder", text );
					}
					
					// Only set data flag for non-native watermarks
					// [purposely commented-out] -> jQueryinput.data(dataFlag, 1);
					return;
				}
				
				jQueryinput.data( dataText, hasText? text : "" );
				jQueryinput.data( dataClass, options.className );
				jQueryinput.data( dataFlag, 1 ); // Flag indicates watermark was initialized
				
				// Special processing for password type
				if ( ( jQueryinput.attr( "type" ) || "" ) === "password" ) {
					var jQuerywrap = jQueryinput.wrap( "<span>" ).parent(),
						jQuerywm = jQuery( jQuerywrap.html().replace( /type=["']?password["']?/i, 'type="text"' ) );
					
					jQuerywm.data( dataText, jQueryinput.data( dataText ) );
					jQuerywm.data( dataClass, jQueryinput.data( dataClass ) );
					jQuerywm.data( dataFlag, 1 );
					jQuerywm.attr( "maxLength", text.length );
					
					jQuerywm.focus(
						function () {
							jQuery.watermark._hide( jQuerywm, true );
						}
					).bind( "dragenter",
						function () {
							jQuery.watermark._hide( jQuerywm );
						}
					).bind( "dragend",
						function () {
							window.setTimeout( function () { jQuerywm.blur(); }, 1 );
						}
					);
					
					jQueryinput.blur(
						function () {
							jQuery.watermark._show( jQueryinput );
						}
					).bind( "dragleave",
						function () {
							jQuery.watermark._show( jQueryinput );
						}
					);
					
					jQuerywm.data( dataPassword, jQueryinput );
					jQueryinput.data( dataPassword, jQuerywm );
				}
				else {
					
					jQueryinput.focus(
						function () {
							jQueryinput.data( dataFocus, 1 );
							jQuery.watermark._hide( jQueryinput, true );
						}
					).blur(
						function () {
							jQueryinput.data( dataFocus, 0 );
							jQuery.watermark._show( jQueryinput );
						}
					).bind( "dragenter",
						function () {
							jQuery.watermark._hide( jQueryinput );
						}
					).bind( "dragleave",
						function () {
							jQuery.watermark._show( jQueryinput );
						}
					).bind( "dragend",
						function () {
							window.setTimeout( function () { jQuery.watermark._show(jQueryinput); }, 1 );
						}
					).bind( "drop",
						// Firefox makes this lovely function necessary because the dropped text
						// is merged with the watermark before the drop event is called.
						function ( evt ) {
							var elem = jQueryinput[ 0 ],
								dropText = evt.originalEvent.dataTransfer.getData( "Text" );
							
							if ( ( elem.value || "" ).replace( rreturn, "" ).replace( dropText, "" ) === jQueryinput.data( dataText ) ) {
								elem.value = dropText;
							}
							
							jQueryinput.focus();
						}
					);
				}
				
				// In order to reliably clear all watermarks before form submission,
				// we need to replace the form's submit function with our own
				// function.  Otherwise watermarks won't be cleared when the form
				// is submitted programmatically.
				if ( this.form ) {
					var form = this.form,
						jQueryform = jQuery( form );
					
					if ( !jQueryform.data( dataFormSubmit ) ) {
						jQueryform.submit( function () { return jQuery.watermark.hideAll.apply( this, options.clearAllFormsOnSubmit? [] : [ form ] ); } );
						
						// form.submit exists for all browsers except Google Chrome
						// (see "else" below for explanation)
						if ( form.submit ) {
							jQueryform.data( dataFormSubmit, form.submit );
							
							form.submit = ( function ( f, jQueryf ) {
								return function () {
									var nativeSubmit = jQueryf.data( dataFormSubmit );
									
									jQuery.watermark.hideAll( options.clearAllFormsOnSubmit? null : f );
									
									if ( nativeSubmit.apply ) {
										nativeSubmit.apply( f, Array.prototype.slice.call( arguments ) );
									}
									else {
										nativeSubmit();
									}
								};
							})( form, jQueryform );
						}
						else {
							jQueryform.data( dataFormSubmit, 1 );
							
							// This strangeness is due to the fact that Google Chrome's
							// form.submit function is not visible to JavaScript (identifies
							// as "undefined").  I had to invent a solution here because hours
							// of Googling (ironically) for an answer did not turn up anything
							// useful.  Within my own form.submit function I delete the form's
							// submit function, and then call the non-existent function --
							// which, in the world of Google Chrome, still exists.
							form.submit = ( function ( f ) {
								return function () {
									jQuery.watermark.hideAll( options.clearAllFormsOnSubmit? null : f );
									delete f.submit;
									f.submit();
								};
							})( form );
						}
					}
				}
			}
			
			jQuery.watermark._show( jQueryinput );
		}
	);
};

// The code included within the following if structure is guaranteed to only run once,
// even if the watermark script file is included multiple times in the page.
if ( jQuery.watermark.runOnce ) {
	jQuery.watermark.runOnce = false;

	jQuery.extend( jQuery.expr[ ":" ], {

		// Extends jQuery with a custom selector - ":data(...)"
		// :data(<name>)  Includes elements that have a specific name defined in the jQuery data
		// collection. (Only the existence of the name is checked; the value is ignored.)
		// A more sophisticated version of the :data() custom selector originally part of this plugin
		// was removed for compatibility with jQuery UI. The original code can be found in the SVN
		// source listing in the file, "jquery.data.js".
		data: jQuery.expr.createPseudo ?
			jQuery.expr.createPseudo( function( dataName ) {
				return function( elem ) {
					return !!jQuery.data( elem, dataName );
				};
			}) :
			// support: jQuery <1.8
			function( elem, i, match ) {
				return !!jQuery.data( elem, match[ 3 ] );
			},

		// Extends jQuery with a custom selector - ":watermarkable"
		// Includes elements that can be watermarked, including textareas and most input elements
		// that accept text input.  It uses a "negative" test (i.e., testing for input types that DON'T
		// work) because the HTML spec states that you can basically use any type, and if it doesn't
		// recognize the type it will default to type=text.  So if we only looked for certain type attributes
		// we would fail to recognize non-standard types, which are still valid and watermarkable.
		watermarkable: function ( elem ) {
			var type,
				name = elem.nodeName;
			
			if ( name === "TEXTAREA" ) {
				return true;
			}
			
			if ( name !== "INPUT" ) {
				return false;
			}
			
			type = elem.getAttribute( "type" );
			
			return ( ( !type ) || ( !rInvalidType.test( type ) ) );
		}
	});

	// Overloads the jQuery .val() function to return the underlying input value on
	// watermarked input elements.  When .val() is being used to set values, this
	// function ensures watermarks are properly set/removed after the values are set.
	// Uses self-executing function to override the default jQuery function.
	( function ( valOld ) {

		jQuery.fn.val = function () {
			var args = Array.prototype.slice.call( arguments );
			
			// Best practice: return immediately if empty matched set
			if ( !this.length ) {
				return args.length? this : undefined;
			}

			// If no args, then we're getting the value of the first element;
			// else we're setting values for all elements in matched set
			if ( !args.length ) {

				// If element is watermarked, get the underlying value;
				// else use native jQuery .val()
				if ( this.data( dataFlag ) ) {
					var v = ( this[ 0 ].value || "" ).replace( rreturn, "" );
					return ( v === ( this.data( dataText ) || "" ) )? "" : v;
				}
				else {
					return valOld.apply( this );
				}
			}
			else {
				valOld.apply( this, args );
				jQuery.watermark.show( this );
				return this;
			}
		};

	})( jQuery.fn.val );
	
	// Hijack any functions found in the triggerFns list
	if ( triggerFns.length ) {

		// Wait until DOM is ready before searching
		jQuery( function () {
			var i, name, fn;
		
			for ( i = triggerFns.length - 1; i >= 0; i-- ) {
				name = triggerFns[ i ];
				fn = window[ name ];
				
				if ( typeof( fn ) === "function" ) {
					window[ name ] = ( function ( origFn ) {
						return function () {
							jQuery.watermark.hideAll();
							return origFn.apply( null, Array.prototype.slice.call( arguments ) );
						};
					})( fn );
				}
			}
		});
	}

	jQuery( window ).bind( "beforeunload", function () {
		if ( jQuery.watermark.options.hideBeforeUnload ) {
			jQuery.watermark.hideAll();
		}
	});
}

})( jQuery, window );
