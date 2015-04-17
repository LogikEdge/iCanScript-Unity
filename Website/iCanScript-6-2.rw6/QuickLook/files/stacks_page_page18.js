
// 'stacks' is the Stacks global object.
// All of the other Stacks related Javascript will 
// be attatched to it.
var stacks = {};


// this call to jQuery gives us access to the globaal
// jQuery object. 
// 'noConflict' removes the '$' variable.
// 'true' removes the 'jQuery' variable.
// removing these globals reduces conflicts with other 
// jQuery versions that might be running on this page.
stacks.jQuery = jQuery.noConflict(true);

// Javascript for stacks_in_645_page18
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_645_page18 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_645_page18 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	

$(document).ready(function(){var position='browser';var position_class='target_stacks_in_645_page18 fill browser top_px left_px ';var custom_id='#custom_id';var target=$('#stacks_in_645_page18').parent();target.addClass(position_class);if(position=='custom'){var custom_element=$('#custom_id');target.appendTo(custom_element);}
if(position=='body'){target.appendTo('body');}});

	return stack;
})(stacks.stacks_in_645_page18);


// Javascript for stacks_in_477_page18
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_477_page18 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_477_page18 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	

$(document).ready(function(){var position='browser';var position_class='target_stacks_in_477_page18 fill browser top_px left_px ';var custom_id='#custom_id';var target=$('#stacks_in_477_page18').parent();target.addClass(position_class);if(position=='custom'){var custom_element=$('#custom_id');target.appendTo(custom_element);}
if(position=='body'){target.appendTo('body');}});

	return stack;
})(stacks.stacks_in_477_page18);


// Javascript for stacks_in_499_page18
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_499_page18 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_499_page18 = (function(stack) {

/**
 * jQuery meanMenu v2.0
 * Copyright (C) 2012-2013 Chris Wharton (themes@meanthemes.com)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * THIS SOFTWARE AND DOCUMENTATION IS PROVIDED "AS IS," AND COPYRIGHT
 * HOLDERS MAKE NO REPRESENTATIONS OR WARRANTIES, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO, WARRANTIES OF MERCHANTABILITY OR
 * FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF THE SOFTWARE
 * OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY PATENTS,
 * COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.COPYRIGHT HOLDERS WILL NOT
 * BE LIABLE FOR ANY DIRECT, INDIRECT, SPECIAL OR CONSEQUENTIAL
 * DAMAGES ARISING OUT OF ANY USE OF THE SOFTWARE OR DOCUMENTATION.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://gnu.org/licenses/>.
 *
 * Find more information at http://www.meanthemes.com/plugins/meanmenu/
 *
 */
(function ($) {
    $.fn.meanmenu = function (options) {
        var defaults = {
            meanMenuTarget: jQuery(this), // Target the current HTML markup you wish to replace
            meanMenuClose: "X", // single character you want to represent the close menu button
            meanMenuCloseSize: "18px", // set font size of close button
            meanMenuOpen: "<span /><span /><span />", // text/markup you want when menu is closed
            meanRevealPosition: "right", // left right or center positions
            meanRevealPositionDistance: "0", // Tweak the position of the menu
            meanRevealColour: "", // override CSS colours for the reveal background
            meanRevealHoverColour: "", // override CSS colours for the reveal hover
            meanScreenWidth: "640", // set the screen width you want meanmenu to kick in at
            meanNavPush: "", // set a height here in px, em or % if you want to budge your layout now the navigation is missing.
            meanShowChildren: true, // true to show children in the menu, false to hide them
            meanExpandableChildren: true, // true to allow expand/collapse children
            meanExpand: "+", // single character you want to represent the expand for ULs
            meanContract: "—", // single character you want to represent the contract for ULs
            meanRemoveAttrs: false // true to remove classes and IDs, false to keep them
        };
        var options = $.extend(defaults, options);
        
        // get browser width
        currentWidth = window.innerWidth || document.documentElement.clientWidth;

        return this.each(function () {
            var meanMenu = options.meanMenuTarget;
            var meanReveal = options.meanReveal;
            var meanMenuClose = options.meanMenuClose;
            var meanMenuCloseSize = options.meanMenuCloseSize;
            var meanMenuOpen = options.meanMenuOpen;
            var meanRevealPosition = options.meanRevealPosition;
            var meanRevealPositionDistance = options.meanRevealPositionDistance;
            var meanRevealColour = options.meanRevealColour;
            var meanRevealHoverColour = options.meanRevealHoverColour;
            var meanScreenWidth = options.meanScreenWidth;
            var meanNavPush = options.meanNavPush;
            var meanRevealClass = ".meanmenu-reveal";
            meanShowChildren = options.meanShowChildren;
            meanExpandableChildren = options.meanExpandableChildren;
            var meanExpand = options.meanExpand;
            var meanContract = options.meanContract;
            var meanRemoveAttrs = options.meanRemoveAttrs;
                        
            //detect known mobile/tablet usage
            if ( (navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i)) || (navigator.userAgent.match(/iPad/i)) || (navigator.userAgent.match(/Android/i)) || (navigator.userAgent.match(/Blackberry/i)) || (navigator.userAgent.match(/Windows Phone/i)) ) {
                var isMobile = true;
            }
            
            if ( (navigator.userAgent.match(/MSIE 8/i)) || (navigator.userAgent.match(/MSIE 7/i)) ) {
            	// add scrollbar for IE7 & 8 to stop breaking resize function on small content sites
                jQuery('html').css("overflow-y" , "scroll");
            }
            
            function meanCentered() {
            	if (meanRevealPosition == "center") {
	            	var newWidth = window.innerWidth || document.documentElement.clientWidth;
	            	var meanCenter = ( (newWidth/2)-22 )+"px";
	            	meanRevealPos = "left:" + meanCenter + ";right:auto;";
	            	
	            	if (!isMobile) {	            	
	            		jQuery('.meanmenu-reveal').css("left",meanCenter); 
	            	} else {
		            	jQuery('.meanmenu-reveal').animate({
		            	    left: meanCenter
		            	});
	            	}
            	}
            }
            
            menuOn = false;
            meanMenuExist = false;
            
            if (meanRevealPosition == "right") {
                meanRevealPos = "right:" + meanRevealPositionDistance + ";left:auto;";
            }
            if (meanRevealPosition == "left") {
                meanRevealPos = "left:" + meanRevealPositionDistance + ";right:auto;";
            } 
            // run center function	
            meanCentered();
            
            // set all styles for mean-reveal
            meanStyles = "background:"+meanRevealColour+";color:"+meanRevealColour+";"+meanRevealPos;

            function meanInner() {
                // get last class name
                if (jQuery($navreveal).is(".meanmenu-reveal.meanclose")) {
                    $navreveal.html(meanMenuClose);
                } else {
                    $navreveal.html(meanMenuOpen);
                }
            }
            
            //re-instate original nav (and call this on window.width functions)
            function meanOriginal() {
            	jQuery('.mean-bar,.mean-push').remove();
            	jQuery('body').removeClass("mean-container");
            	jQuery(meanMenu).show();
            	menuOn = false;
            	meanMenuExist = false;
            }
            
            //navigation reveal 
            function showMeanMenu() {
                if (currentWidth <= meanScreenWidth) {
                	meanMenuExist = true;
                	// add class to body so we don't need to worry about media queries here, all CSS is wrapped in '.mean-container'
                	jQuery('body').addClass("mean-container");
                	jQuery('.mean-container').prepend('<div class="mean-bar"><a href="#nav" class="meanmenu-reveal" style="'+meanStyles+'">Show Navigation</a><nav class="mean-nav"></nav></div>');
                    
                    //push meanMenu navigation into .mean-nav
                    var meanMenuContents = jQuery(meanMenu).html();
                    jQuery('.mean-nav').html(meanMenuContents);
            		
            		// remove all classes from EVERYTHING inside meanmenu nav
            		if(meanRemoveAttrs) {
            			jQuery('nav.mean-nav *').each(function() {
            				jQuery(this).removeAttr("class");
            				jQuery(this).removeAttr("id");
            			});
            		}
                    
                    // push in a holder div (this can be used if removal of nav is causing layout issues)
                    jQuery(meanMenu).before('<div class="mean-push" />');
                    jQuery('.mean-push').css("margin-top",meanNavPush);
                    
                    // hide current navigation and reveal mean nav link
                    jQuery(meanMenu).hide();
                    jQuery(".meanmenu-reveal").show();
                    
                    // turn 'X' on or off 
                    jQuery(meanRevealClass).html(meanMenuOpen);
                    $navreveal = jQuery(meanRevealClass);
                    
                    //hide mean-nav ul
                    jQuery('.mean-nav ul').hide();
                    
                    // hide sub nav
	                   if(meanShowChildren) {
	                   		// allow expandable sub nav(s)
	                       if(meanExpandableChildren){
		                       jQuery('.mean-nav ul ul').each(function() {
		                           if(jQuery(this).children().length){
		                               jQuery(this,'li:first').parent().append('<a class="mean-expand" href="#" style="font-size: '+ meanMenuCloseSize +'">'+ meanExpand +'</a>');                               
		                           }
		                       });
		                       jQuery('.mean-expand').on("click",function(e){
		                       		e.preventDefault();
		                       	   if (jQuery(this).hasClass("mean-clicked")) {
		                       	   		jQuery(this).text(meanExpand);
		                       	   		console.log("Been clicked");
		                               jQuery(this).prev('ul').slideUp(300, function(){
		                                  
		                               });
		                           } else {
		                           		jQuery(this).text(meanContract);
		                           		jQuery(this).prev('ul').slideDown(300, function(){
		                           		});
		                           }   
		                           jQuery(this).toggleClass("mean-clicked"); 
		                       });     
	                       } else {
	                           jQuery('.mean-nav ul ul').show();   
	                       }
	                   } else {
	                       jQuery('.mean-nav ul ul').hide();
	                   }
	                   
                    // add last class to tidy up borders
                    jQuery('.mean-nav ul li').last().addClass('mean-last');
                
                    $navreveal.removeClass("meanclose");
                    jQuery($navreveal).click(function(e){
                    	e.preventDefault();
	            		if(menuOn == false) {
	                        $navreveal.css("text-align", "center");
	                        $navreveal.css("text-indent", "0");
	                        $navreveal.css("font-size", meanMenuCloseSize);
	                        jQuery('.mean-nav ul:first').slideDown(); 
	                        menuOn = true;
	                    } else {
	                    	jQuery('.mean-nav ul:first').slideUp();
	                    	menuOn = false;
	                    }    
                        $navreveal.toggleClass("meanclose");
                        meanInner();
                    });
                    
                } else {
                	meanOriginal();
                }	
            } 
            
            if (!isMobile) {
                //reset menu on resize above meanScreenWidth
                jQuery(window).resize(function () {
                    currentWidth = window.innerWidth || document.documentElement.clientWidth;
                    if (currentWidth > meanScreenWidth) {
                        meanOriginal();
                    } else {
                    	meanOriginal();
                    }	
                    if (currentWidth <= meanScreenWidth) {
                        showMeanMenu();
                        meanCentered();
                    } else {
                    	meanOriginal();
                    }	
                });
            }

       		// adjust menu positioning on centered navigation     
            window.onorientationchange = function() {
            	meanCentered();
            	// get browser width
            	currentWidth = window.innerWidth || document.documentElement.clientWidth;
            	if (currentWidth >= meanScreenWidth) {
            		meanOriginal();
            	}
            	if (currentWidth <= meanScreenWidth) {
            		if (meanMenuExist == false) {
            			showMeanMenu();
            		}
            	}
            }
           // run main menuMenu function on load
           showMeanMenu(); 
        });
    };
})(jQuery);
	return stack;
})(stacks.stacks_in_499_page18);


// Javascript for stacks_in_860_page18
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_860_page18 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_860_page18 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	

$(document).ready(function(){var position='browser';var position_class='target_stacks_in_860_page18 fill browser top_px left_px ';var custom_id='#custom_id';var target=$('#stacks_in_860_page18').parent();target.addClass(position_class);if(position=='custom'){var custom_element=$('#custom_id');target.appendTo(custom_element);}
if(position=='body'){target.appendTo('body');}});

	return stack;
})(stacks.stacks_in_860_page18);


// Javascript for stacks_in_789_page18
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_789_page18 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_789_page18 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_789_page18 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_789_page18);


