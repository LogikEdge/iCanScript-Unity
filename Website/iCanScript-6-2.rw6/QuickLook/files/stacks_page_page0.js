
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

// Javascript for com_joeworkman_stacks_tabulous2
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.com_joeworkman_stacks_tabulous2 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.com_joeworkman_stacks_tabulous2 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	

(function(e){var o="left",n="right",d="up",v="down",c="in",w="out",l="none",r="auto",k="swipe",s="pinch",x="tap",i="doubletap",b="longtap",A="horizontal",t="vertical",h="all",q=10,f="start",j="move",g="end",p="cancel",a="ontouchstart"in window,y="TouchSwipe";var m={fingers:1,threshold:75,cancelThreshold:null,pinchThreshold:20,maxTimeThreshold:null,fingerReleaseThreshold:250,longTapThreshold:500,doubleTapThreshold:200,swipe:null,swipeLeft:null,swipeRight:null,swipeUp:null,swipeDown:null,swipeStatus:null,pinchIn:null,pinchOut:null,pinchStatus:null,click:null,tap:null,doubleTap:null,longTap:null,triggerOnTouchEnd:true,triggerOnTouchLeave:false,allowPageScroll:"auto",fallbackToMouseEvents:true,excludedElements:"button, input, select, textarea, a, .noSwipe"};e.fn.swipe=function(D){var C=e(this),B=C.data(y);if(B&&typeof D==="string"){if(B[D]){return B[D].apply(this,Array.prototype.slice.call(arguments,1))}else{e.error("Method "+D+" does not exist on jQuery.swipe")}}else{if(!B&&(typeof D==="object"||!D)){return u.apply(this,arguments)}}return C};e.fn.swipe.defaults=m;e.fn.swipe.phases={PHASE_START:f,PHASE_MOVE:j,PHASE_END:g,PHASE_CANCEL:p};e.fn.swipe.directions={LEFT:o,RIGHT:n,UP:d,DOWN:v,IN:c,OUT:w};e.fn.swipe.pageScroll={NONE:l,HORIZONTAL:A,VERTICAL:t,AUTO:r};e.fn.swipe.fingers={ONE:1,TWO:2,THREE:3,ALL:h};function u(B){if(B&&(B.allowPageScroll===undefined&&(B.swipe!==undefined||B.swipeStatus!==undefined))){B.allowPageScroll=l}if(B.click!==undefined&&B.tap===undefined){B.tap=B.click}if(!B){B={}}B=e.extend({},e.fn.swipe.defaults,B);return this.each(function(){var D=e(this);var C=D.data(y);if(!C){C=new z(this,B);D.data(y,C)}})}function z(a0,aq){var av=(a||!aq.fallbackToMouseEvents),G=av?"touchstart":"mousedown",au=av?"touchmove":"mousemove",R=av?"touchend":"mouseup",P=av?null:"mouseleave",az="touchcancel";var ac=0,aL=null,Y=0,aX=0,aV=0,D=1,am=0,aF=0,J=null;var aN=e(a0);var W="start";var T=0;var aM=null;var Q=0,aY=0,a1=0,aa=0,K=0;var aS=null;try{aN.bind(G,aJ);aN.bind(az,a5)}catch(ag){e.error("events not supported "+G+","+az+" on jQuery.swipe")}this.enable=function(){aN.bind(G,aJ);aN.bind(az,a5);return aN};this.disable=function(){aG();return aN};this.destroy=function(){aG();aN.data(y,null);return aN};this.option=function(a8,a7){if(aq[a8]!==undefined){if(a7===undefined){return aq[a8]}else{aq[a8]=a7}}else{e.error("Option "+a8+" does not exist on jQuery.swipe.options")}};function aJ(a9){if(ax()){return}if(e(a9.target).closest(aq.excludedElements,aN).length>0){return}var ba=a9.originalEvent?a9.originalEvent:a9;var a8,a7=a?ba.touches[0]:ba;W=f;if(a){T=ba.touches.length}else{a9.preventDefault()}ac=0;aL=null;aF=null;Y=0;aX=0;aV=0;D=1;am=0;aM=af();J=X();O();if(!a||(T===aq.fingers||aq.fingers===h)||aT()){ae(0,a7);Q=ao();if(T==2){ae(1,ba.touches[1]);aX=aV=ap(aM[0].start,aM[1].start)}if(aq.swipeStatus||aq.pinchStatus){a8=L(ba,W)}}else{a8=false}if(a8===false){W=p;L(ba,W);return a8}else{ak(true)}}function aZ(ba){var bd=ba.originalEvent?ba.originalEvent:ba;if(W===g||W===p||ai()){return}var a9,a8=a?bd.touches[0]:bd;var bb=aD(a8);aY=ao();if(a){T=bd.touches.length}W=j;if(T==2){if(aX==0){ae(1,bd.touches[1]);aX=aV=ap(aM[0].start,aM[1].start)}else{aD(bd.touches[1]);aV=ap(aM[0].end,aM[1].end);aF=an(aM[0].end,aM[1].end)}D=a3(aX,aV);am=Math.abs(aX-aV)}if((T===aq.fingers||aq.fingers===h)||!a||aT()){aL=aH(bb.start,bb.end);ah(ba,aL);ac=aO(bb.start,bb.end);Y=aI();aE(aL,ac);if(aq.swipeStatus||aq.pinchStatus){a9=L(bd,W)}if(!aq.triggerOnTouchEnd||aq.triggerOnTouchLeave){var a7=true;if(aq.triggerOnTouchLeave){var bc=aU(this);a7=B(bb.end,bc)}if(!aq.triggerOnTouchEnd&&a7){W=ay(j)}else{if(aq.triggerOnTouchLeave&&!a7){W=ay(g)}}if(W==p||W==g){L(bd,W)}}}else{W=p;L(bd,W)}if(a9===false){W=p;L(bd,W)}}function I(a7){var a8=a7.originalEvent;if(a){if(a8.touches.length>0){C();return true}}if(ai()){T=aa}a7.preventDefault();aY=ao();Y=aI();if(a6()){W=p;L(a8,W)}else{if(aq.triggerOnTouchEnd||(aq.triggerOnTouchEnd==false&&W===j)){W=g;L(a8,W)}else{if(!aq.triggerOnTouchEnd&&a2()){W=g;aB(a8,W,x)}else{if(W===j){W=p;L(a8,W)}}}}ak(false)}function a5(){T=0;aY=0;Q=0;aX=0;aV=0;D=1;O();ak(false)}function H(a7){var a8=a7.originalEvent;if(aq.triggerOnTouchLeave){W=ay(g);L(a8,W)}}function aG(){aN.unbind(G,aJ);aN.unbind(az,a5);aN.unbind(au,aZ);aN.unbind(R,I);if(P){aN.unbind(P,H)}ak(false)}function ay(bb){var ba=bb;var a9=aw();var a8=aj();var a7=a6();if(!a9||a7){ba=p}else{if(a8&&bb==j&&(!aq.triggerOnTouchEnd||aq.triggerOnTouchLeave)){ba=g}else{if(!a8&&bb==g&&aq.triggerOnTouchLeave){ba=p}}}return ba}function L(a9,a7){var a8=undefined;if(F()||S()){a8=aB(a9,a7,k)}else{if((M()||aT())&&a8!==false){a8=aB(a9,a7,s)}}if(aC()&&a8!==false){a8=aB(a9,a7,i)}else{if(al()&&a8!==false){a8=aB(a9,a7,b)}else{if(ad()&&a8!==false){a8=aB(a9,a7,x)}}}if(a7===p){a5(a9)}if(a7===g){if(a){if(a9.touches.length==0){a5(a9)}}else{a5(a9)}}return a8}function aB(ba,a7,a9){var a8=undefined;if(a9==k){aN.trigger("swipeStatus",[a7,aL||null,ac||0,Y||0,T]);if(aq.swipeStatus){a8=aq.swipeStatus.call(aN,ba,a7,aL||null,ac||0,Y||0,T);if(a8===false){return false}}if(a7==g&&aR()){aN.trigger("swipe",[aL,ac,Y,T]);if(aq.swipe){a8=aq.swipe.call(aN,ba,aL,ac,Y,T);if(a8===false){return false}}switch(aL){case o:aN.trigger("swipeLeft",[aL,ac,Y,T]);if(aq.swipeLeft){a8=aq.swipeLeft.call(aN,ba,aL,ac,Y,T)}break;case n:aN.trigger("swipeRight",[aL,ac,Y,T]);if(aq.swipeRight){a8=aq.swipeRight.call(aN,ba,aL,ac,Y,T)}break;case d:aN.trigger("swipeUp",[aL,ac,Y,T]);if(aq.swipeUp){a8=aq.swipeUp.call(aN,ba,aL,ac,Y,T)}break;case v:aN.trigger("swipeDown",[aL,ac,Y,T]);if(aq.swipeDown){a8=aq.swipeDown.call(aN,ba,aL,ac,Y,T)}break}}}if(a9==s){aN.trigger("pinchStatus",[a7,aF||null,am||0,Y||0,T,D]);if(aq.pinchStatus){a8=aq.pinchStatus.call(aN,ba,a7,aF||null,am||0,Y||0,T,D);if(a8===false){return false}}if(a7==g&&a4()){switch(aF){case c:aN.trigger("pinchIn",[aF||null,am||0,Y||0,T,D]);if(aq.pinchIn){a8=aq.pinchIn.call(aN,ba,aF||null,am||0,Y||0,T,D)}break;case w:aN.trigger("pinchOut",[aF||null,am||0,Y||0,T,D]);if(aq.pinchOut){a8=aq.pinchOut.call(aN,ba,aF||null,am||0,Y||0,T,D)}break}}}if(a9==x){if(a7===p||a7===g){clearTimeout(aS);if(V()&&!E()){K=ao();aS=setTimeout(e.proxy(function(){K=null;aN.trigger("tap",[ba.target]);if(aq.tap){a8=aq.tap.call(aN,ba,ba.target)}},this),aq.doubleTapThreshold)}else{K=null;aN.trigger("tap",[ba.target]);if(aq.tap){a8=aq.tap.call(aN,ba,ba.target)}}}}else{if(a9==i){if(a7===p||a7===g){clearTimeout(aS);K=null;aN.trigger("doubletap",[ba.target]);if(aq.doubleTap){a8=aq.doubleTap.call(aN,ba,ba.target)}}}else{if(a9==b){if(a7===p||a7===g){clearTimeout(aS);K=null;aN.trigger("longtap",[ba.target]);if(aq.longTap){a8=aq.longTap.call(aN,ba,ba.target)}}}}}return a8}function aj(){var a7=true;if(aq.threshold!==null){a7=ac>=aq.threshold}return a7}function a6(){var a7=false;if(aq.cancelThreshold!==null&&aL!==null){a7=(aP(aL)-ac)>=aq.cancelThreshold}return a7}function ab(){if(aq.pinchThreshold!==null){return am>=aq.pinchThreshold}return true}function aw(){var a7;if(aq.maxTimeThreshold){if(Y>=aq.maxTimeThreshold){a7=false}else{a7=true}}else{a7=true}return a7}function ah(a7,a8){if(aq.allowPageScroll===l||aT()){a7.preventDefault()}else{var a9=aq.allowPageScroll===r;switch(a8){case o:if((aq.swipeLeft&&a9)||(!a9&&aq.allowPageScroll!=A)){a7.preventDefault()}break;case n:if((aq.swipeRight&&a9)||(!a9&&aq.allowPageScroll!=A)){a7.preventDefault()}break;case d:if((aq.swipeUp&&a9)||(!a9&&aq.allowPageScroll!=t)){a7.preventDefault()}break;case v:if((aq.swipeDown&&a9)||(!a9&&aq.allowPageScroll!=t)){a7.preventDefault()}break}}}function a4(){var a8=aK();var a7=U();var a9=ab();return a8&&a7&&a9}function aT(){return!!(aq.pinchStatus||aq.pinchIn||aq.pinchOut)}function M(){return!!(a4()&&aT())}function aR(){var ba=aw();var bc=aj();var a9=aK();var a7=U();var a8=a6();var bb=!a8&&a7&&a9&&bc&&ba;return bb}function S(){return!!(aq.swipe||aq.swipeStatus||aq.swipeLeft||aq.swipeRight||aq.swipeUp||aq.swipeDown)}function F(){return!!(aR()&&S())}function aK(){return((T===aq.fingers||aq.fingers===h)||!a)}function U(){return aM[0].end.x!==0}function a2(){return!!(aq.tap)}function V(){return!!(aq.doubleTap)}function aQ(){return!!(aq.longTap)}function N(){if(K==null){return false}var a7=ao();return(V()&&((a7-K)<=aq.doubleTapThreshold))}function E(){return N()}function at(){return((T===1||!a)&&(isNaN(ac)||ac===0))}function aW(){return((Y>aq.longTapThreshold)&&(ac<q))}function ad(){return!!(at()&&a2())}function aC(){return!!(N()&&V())}function al(){return!!(aW()&&aQ())}function C(){a1=ao();aa=event.touches.length+1}function O(){a1=0;aa=0}function ai(){var a7=false;if(a1){var a8=ao()-a1;if(a8<=aq.fingerReleaseThreshold){a7=true}}return a7}function ax(){return!!(aN.data(y+"_intouch")===true)}function ak(a7){if(a7===true){aN.bind(au,aZ);aN.bind(R,I);if(P){aN.bind(P,H)}}else{aN.unbind(au,aZ,false);aN.unbind(R,I,false);if(P){aN.unbind(P,H,false)}}aN.data(y+"_intouch",a7===true)}function ae(a8,a7){var a9=a7.identifier!==undefined?a7.identifier:0;aM[a8].identifier=a9;aM[a8].start.x=aM[a8].end.x=a7.pageX||a7.clientX;aM[a8].start.y=aM[a8].end.y=a7.pageY||a7.clientY;return aM[a8]}function aD(a7){var a9=a7.identifier!==undefined?a7.identifier:0;var a8=Z(a9);a8.end.x=a7.pageX||a7.clientX;a8.end.y=a7.pageY||a7.clientY;return a8}function Z(a8){for(var a7=0;a7<aM.length;a7++){if(aM[a7].identifier==a8){return aM[a7]}}}function af(){var a7=[];for(var a8=0;a8<=5;a8++){a7.push({start:{x:0,y:0},end:{x:0,y:0},identifier:0})}return a7}function aE(a7,a8){a8=Math.max(a8,aP(a7));J[a7].distance=a8}function aP(a7){return J[a7].distance}function X(){var a7={};a7[o]=ar(o);a7[n]=ar(n);a7[d]=ar(d);a7[v]=ar(v);return a7}function ar(a7){return{direction:a7,distance:0}}function aI(){return aY-Q}function ap(ba,a9){var a8=Math.abs(ba.x-a9.x);var a7=Math.abs(ba.y-a9.y);return Math.round(Math.sqrt(a8*a8+a7*a7))}function a3(a7,a8){var a9=(a8/a7)*1;return a9.toFixed(2)}function an(){if(D<1){return w}else{return c}}function aO(a8,a7){return Math.round(Math.sqrt(Math.pow(a7.x-a8.x,2)+Math.pow(a7.y-a8.y,2)))}function aA(ba,a8){var a7=ba.x-a8.x;var bc=a8.y-ba.y;var a9=Math.atan2(bc,a7);var bb=Math.round(a9*180/Math.PI);if(bb<0){bb=360-Math.abs(bb)}return bb}function aH(a8,a7){var a9=aA(a8,a7);if((a9<=45)&&(a9>=0)){return o}else{if((a9<=360)&&(a9>=315)){return o}else{if((a9>=135)&&(a9<=225)){return n}else{if((a9>45)&&(a9<135)){return v}else{return d}}}}}function ao(){var a7=new Date();return a7.getTime()}function aU(a7){a7=e(a7);var a9=a7.offset();var a8={left:a9.left,right:a9.left+a7.outerWidth(),top:a9.top,bottom:a9.top+a7.outerHeight()};return a8}function B(a7,a8){return(a7.x>a8.left&&a7.x<a8.right&&a7.y>a8.top&&a7.y<a8.bottom)}}})(jQuery);jQuery.easing['jswing']=jQuery.easing['swing'];jQuery.extend(jQuery.easing,{def:'easeOutQuad',swing:function(x,t,b,c,d){return jQuery.easing[jQuery.easing.def](x,t,b,c,d)},easeInQuad:function(x,t,b,c,d){return c*(t/=d)*t+b},easeOutQuad:function(x,t,b,c,d){return-c*(t/=d)*(t-2)+b},easeInOutQuad:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t+b;return-c/2*((--t)*(t-2)-1)+b},easeInCubic:function(x,t,b,c,d){return c*(t/=d)*t*t+b},easeOutCubic:function(x,t,b,c,d){return c*((t=t/d-1)*t*t+1)+b},easeInOutCubic:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t+b;return c/2*((t-=2)*t*t+2)+b},easeInQuart:function(x,t,b,c,d){return c*(t/=d)*t*t*t+b},easeOutQuart:function(x,t,b,c,d){return-c*((t=t/d-1)*t*t*t-1)+b},easeInOutQuart:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t*t+b;return-c/2*((t-=2)*t*t*t-2)+b},easeInQuint:function(x,t,b,c,d){return c*(t/=d)*t*t*t*t+b},easeOutQuint:function(x,t,b,c,d){return c*((t=t/d-1)*t*t*t*t+1)+b},easeInOutQuint:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t*t*t+b;return c/2*((t-=2)*t*t*t*t+2)+b},easeInSine:function(x,t,b,c,d){return-c*Math.cos(t/d*(Math.PI/2))+c+b},easeOutSine:function(x,t,b,c,d){return c*Math.sin(t/d*(Math.PI/2))+b},easeInOutSine:function(x,t,b,c,d){return-c/2*(Math.cos(Math.PI*t/d)-1)+b},easeInExpo:function(x,t,b,c,d){return(t==0)?b:c*Math.pow(2,10*(t/d-1))+b},easeOutExpo:function(x,t,b,c,d){return(t==d)?b+c:c*(-Math.pow(2,-10*t/d)+1)+b},easeInOutExpo:function(x,t,b,c,d){if(t==0)return b;if(t==d)return b+c;if((t/=d/2)<1)return c/2*Math.pow(2,10*(t-1))+b;return c/2*(-Math.pow(2,-10*--t)+2)+b},easeInCirc:function(x,t,b,c,d){return-c*(Math.sqrt(1-(t/=d)*t)-1)+b},easeOutCirc:function(x,t,b,c,d){return c*Math.sqrt(1-(t=t/d-1)*t)+b},easeInOutCirc:function(x,t,b,c,d){if((t/=d/2)<1)return-c/2*(Math.sqrt(1-t*t)-1)+b;return c/2*(Math.sqrt(1-(t-=2)*t)+1)+b},easeInElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d)==1)return b+c;if(!p)p=d*.3;if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return-(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b},easeOutElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d)==1)return b+c;if(!p)p=d*.3;if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return a*Math.pow(2,-10*t)*Math.sin((t*d-s)*(2*Math.PI)/p)+c+b},easeInOutElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d/2)==2)return b+c;if(!p)p=d*(.3*1.5);if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);if(t<1)return-.5*(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b;return a*Math.pow(2,-10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p)*.5+c+b},easeInBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;return c*(t/=d)*t*((s+1)*t-s)+b},easeOutBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;return c*((t=t/d-1)*t*((s+1)*t+s)+1)+b},easeInOutBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;if((t/=d/2)<1)return c/2*(t*t*(((s*=(1.525))+1)*t-s))+b;return c/2*((t-=2)*t*(((s*=(1.525))+1)*t+s)+2)+b},easeInBounce:function(x,t,b,c,d){return c-jQuery.easing.easeOutBounce(x,d-t,0,c,d)+b},easeOutBounce:function(x,t,b,c,d){if((t/=d)<(1/2.75)){return c*(7.5625*t*t)+b}else if(t<(2/2.75)){return c*(7.5625*(t-=(1.5/2.75))*t+.75)+b}else if(t<(2.5/2.75)){return c*(7.5625*(t-=(2.25/2.75))*t+.9375)+b}else{return c*(7.5625*(t-=(2.625/2.75))*t+.984375)+b}},easeInOutBounce:function(x,t,b,c,d){if(t<d/2)return jQuery.easing.easeInBounce(x,t*2,0,c,d)*.5+b;return jQuery.easing.easeOutBounce(x,t*2-d,0,c,d)*.5+c*.5+b}});if(typeof Object.create!=='function'){Object.create=function(obj){"use strict";function F(){}F.prototype=obj;return new F()}}(function($,window,document,undefined){"use strict";var Slider={makeResponsive:function(){var self=this;$(self.sliderId+'-wrapper').addClass('ls-responsive').css({'max-width':$(self.sliderId+' .panel:first-child').width(),'width':'100%'});$(self.sliderId+' .panel-container').css('width',100*self.panelCountTotal+self.pSign);$(self.sliderId+' .panel').css('width',100/self.panelCountTotal+self.pSign);if(self.options.hideArrowsWhenMobile){self.leftWrapperPadding=$(self.sliderId+'-wrapper').css('padding-left');self.rightWrapperPadding=(self.$sliderWrap).css('padding-right')}self.responsiveEvents();$(window).bind('resize',function(){self.responsiveEvents();clearTimeout(self.resizingTimeout);self.resizingTimeout=setTimeout(function(){var height=(self.options.autoHeight)?self.getHeight():self.getHeighestPanel(self.nextPanel);self.adjustHeight(false,height)},500)})},responsiveEvents:function(){var self=this,mobileNavChangeOver=(self.options.hideArrowsThreshold||self.options.mobileUIThreshold||(self.totalNavWidth+10));self.setNavTallest();if((self.$sliderId).outerWidth()<mobileNavChangeOver){if(self.options.mobileNavigation){(self.navigation).css('display','none');(self.dropdown).css('display','block');(self.dropdownSelect).css('display','block');$(self.sliderId+'-nav-select').val(self.options.mobileNavDefaultText)}if(self.options.dynamicArrows){if(self.options.hideArrowsWhenMobile){(self.leftArrow).remove().length=0;(self.rightArrow).remove().length=0}else if(!self.options.dynamicArrowsGraphical){(self.leftArrow).css('margin-'+self.options.dynamicTabsPosition,'0');(self.rightArrow).css('margin-'+self.options.dynamicTabsPosition,'0')}}}else{if(self.options.mobileNavigation){(self.navigation).css('display','block');(self.dropdown).css('display','none');(self.dropdownSelect).css('display','none')}if(self.options.dynamicArrows){if(self.options.hideArrowsWhenMobile&&(!(self.leftArrow).length||!(self.rightArrow).length)){self.addArrows();self.registerArrows()}else if(!self.options.dynamicArrowsGraphical){(self.leftArrow).css('margin-'+self.options.dynamicTabsPosition,(self.navigation).css('height'));(self.rightArrow).css('margin-'+self.options.dynamicTabsPosition,(self.navigation).css('height'))}}}$(self.sliderId+'-wrapper').css('width','100%');if(self.options.mobileNavigation){(self.dropdownSelect).change(function(){self.setNextPanel(parseInt($(this).val().split('tab')[1],10)-1)})}},addNavigation:function(navClass){var self=this,dynamicTabsElm='<'+self.options.navElementTag+' class="ls-nav"><ul id="'+(self.$elem).attr('id')+'-nav-ul"></ul></'+self.options.navElementTag+'>';if(self.options.dynamicTabsPosition==='bottom'){(self.$sliderId).after(dynamicTabsElm)}else{(self.$sliderId).before(dynamicTabsElm)}if(self.options.mobileNavigation){var selectBoxDefault=(self.options.mobileNavDefaultText)?'<option disabled="disabled" selected="selected">'+self.options.mobileNavDefaultText+'</option>':null,dropDownList='<div class="ls-select-box"><select id="'+(self.$elem).attr('id')+'-nav-select" name="navigation">'+selectBoxDefault+'</select></div>';self.navigation=$(self.sliderId+'-nav-ul').before(dropDownList);self.dropdown=$(self.sliderId+'-wrapper .ls-select-box');self.dropdownSelect=$(self.sliderId+'-nav-select');$.each((self.$elem).find(self.options.panelTitleSelector),function(n){$((self.$sliderWrap)).find('.ls-select-box select').append('<option value="tab'+(n+1)+'">'+$(this).text()+'</option>')})}$.each((self.$elem).find(self.options.panelTitleSelector),function(n){$((self.$sliderWrap)).find('.ls-nav ul').append('<li class="tab'+(n+1)+'"><a class="'+(navClass||'')+'" href="#'+(n+1)+'">'+self.getNavInsides(this)+'</a></li>');if(!self.options.includeTitle)$(this).remove()});self.setNavTallest()},setNavTallest:function(){var self=this,maxHeight=0;var tabs=$((self.$sliderWrap)).find('.ls-nav ul li a');tabs.removeAttr('style').each(function(){maxHeight=maxHeight>$(this).height()?maxHeight:$(this).height()});tabs.each(function(){$(this).height(maxHeight)})},getNavInsides:function(input){return(this.options.dynamicTabsHtml)?$(input).html():$(input).text()},alignNavigation:function(){var self=this,arrow=(self.options.dynamicArrowsGraphical)?'-arrow':'';if(self.options.dynamicTabsAlign!=='center'){if(!self.options.responsive){$((self.$sliderWrap)).find('.ls-nav ul').css('margin-'+self.options.dynamicTabsAlign,$((self.$sliderWrap)).find('.ls-nav-'+self.options.dynamicTabsAlign+arrow).outerWidth(true)+parseInt((self.$sliderId).css('margin-'+self.options.dynamicTabsAlign),10))}$((self.$sliderWrap)).find('.ls-nav ul').css('float',self.options.dynamicTabsAlign)}self.totalNavWidth=$((self.$sliderWrap)).find('.ls-nav ul').outerWidth(true);if(self.options.dynamicTabsAlign==='center'){self.totalNavWidth=0;$((self.$sliderWrap)).find('.ls-nav li a').each(function(){self.totalNavWidth+=$(this).outerWidth(true)});$((self.$sliderWrap)).find('.ls-nav ul').css('width',self.totalNavWidth+1)}},registerNav:function(){var self=this;(self.$sliderWrap).find('[class^=ls-nav] li').on('click',function(){self.setNextPanel(parseInt($(this).attr('class').split('tab')[1],10)-1);return false})},addArrows:function(arrowClass){var self=this,arrow=(self.options.dynamicArrowsGraphical)?"-arrow ":' ';(self.$sliderWrap).addClass("arrows");if(self.options.dynamicArrowsGraphical){self.options.dynamicArrowLeftText='';self.options.dynamicArrowRightText=''}(self.$sliderId).before('<div class="ls-nav-left'+arrow+(arrowClass||'')+'"><a href="#">'+self.options.dynamicArrowLeftText+'</a></div>');(self.$sliderId).after('<div class="ls-nav-right'+arrow+(arrowClass||'')+'"><a href="#">'+self.options.dynamicArrowRightText+'</a></div>');self.leftArrow=$(self.sliderId+'-wrapper [class^=ls-nav-left]').css('visibility',"hidden").addClass('ls-hidden');self.rightArrow=$(self.sliderId+'-wrapper [class^=ls-nav-right]').css('visibility',"hidden").addClass('ls-hidden');if(!self.options.hoverArrows)self.hideShowArrows(undefined,true,true,false)},hideShowArrows:function(speed,forceVisibility,showBoth,hideBoth){var self=this,fadeOut=(typeof speed!=='undefined')?speed:self.options.fadeOutDuration,fadeIn=(typeof speed!=='undefined')?speed:self.options.fadeInDuration,visibility=forceVisibility?"visible":"hidden";if(!showBoth&&(hideBoth||(self.sanatizeNumber(self.nextPanel)===1))){self.leftArrow.stop().fadeTo(fadeOut,0,function(){$(this).css('visibility',visibility).addClass('ls-hidden')})}else if(showBoth||self.leftArrow.hasClass('ls-hidden')){self.leftArrow.stop().css('visibility',"visible").fadeTo(fadeIn,1).removeClass('ls-hidden')}if(!showBoth&&(hideBoth||(self.sanatizeNumber(self.nextPanel)===self.panelCount))){self.rightArrow.stop().fadeTo(fadeOut,0,function(){$(this).css('visibility',visibility).addClass('ls-hidden')})}else if(showBoth||self.rightArrow.hasClass('ls-hidden')){self.rightArrow.stop().css('visibility',"visible").fadeTo(fadeIn,1).removeClass('ls-hidden')}},registerArrows:function(){var self=this;$((self.$sliderWrap).find('[class^=ls-nav-]')).on('click',function(){self.setNextPanel($(this).attr('class').split(' ')[0].split('-')[2])})},registerCrossLinks:function(){var self=this;self.crosslinks=$('[data-liquidslider-ref*='+(self.sliderId).split('#')[1]+']');(self.crosslinks).on('click',function(e){if(self.options.autoSlide===true)self.startAutoSlide(true);self.setNextPanel(self.getPanelNumber(($(this).attr('href').split('#')[1]),self.options.panelTitleSelector));e.preventDefault()});self.updateClass()},registerTouch:function(){var self=this,args=self.options.swipeArgs||{fallbackToMouseEvents:false,allowPageScroll:"vertical",swipe:function(e,dir){if(dir==='up'||dir==='down')return false;self.swipeDir=(dir==='left')?'right':'left';self.setNextPanel(self.swipeDir)}};$(self.sliderId+' .panel').swipe(args)},registerKeyboard:function(){var self=this;$(document).keydown(function(event){var key=event.keyCode||event.which;if(event.target.type!=='textarea'&&event.target.type!=='textbox'){if(!self.options.forceAutoSlide)$(this).trigger('click');if(key===self.options.leftKey)self.setNextPanel('right');if(key===self.options.rightKey)self.setNextPanel('left');$.each(self.options.panelKeys,function(index,value){if(key===value){self.setNextPanel(index-1)}})}})},autoSlide:function(){var self=this;if(self.options.autoSlideInterval<self.options.slideEaseDuration){self.options.autoSlideInterval=(self.options.slideEaseDuration>self.options.heightEaseDuration)?self.options.slideEaseDuration:self.options.heightEaseDuration}self.autoSlideTimeout=setTimeout(function(){self.setNextPanel(self.options.autoSlideDirection);self.autoSlide()},self.options.autoSlideInterval)},stopAutoSlide:function(){var self=this;self.options.autoSlide=false;clearTimeout(self.autoSlideTimeout)},startAutoSlide:function(reset){var self=this;self.options.autoSlide=true;if(!reset)self.setNextPanel(self.options.autoSlideDirection);self.autoSlide(clearTimeout(self.autoSlideTimeout))},adjustHeight:function(noAnimation,height,easing,duration){var self=this;if(noAnimation||self.useCSS){if(noAnimation)self.configureCSSTransitions('0','0');(self.$sliderId).height(height);if(noAnimation)self.configureCSSTransitions();return}(self.$sliderId).animate({'height':height+'px'},{easing:easing||self.options.heightEaseFunction,duration:duration||self.options.heightEaseDuration,queue:false})},getHeight:function(height){var self=this;height=height||self.$panelClass.eq(self.sanatizeNumber(self.nextPanel)-1).outerHeight(true);height=(height<self.options.minHeight)?self.options.minHeight:height;return height},addPreloader:function(){var self=this;$(self.sliderId+'-wrapper').append('<div class="ls-preloader"></div>')},removePreloader:function(){var self=this;$(self.sliderId+'-wrapper .ls-preloader').fadeTo('slow',0,function(){$(this).remove()})},init:function(options,elem){var self=this;self.elem=elem;self.$elem=$(elem);$('body').removeClass('no-js');self.sliderId='#'+(self.$elem).attr('id');self.$sliderId=$(self.sliderId);self.options=$.extend({},$.fn.liquidSlider.options,options);self.pSign=(self.options.responsive)?'%':'px';if(self.options.responsive){self.determineAnimationType()}else{self.options.mobileNavigation=false;self.options.hideArrowsWhenMobile=false}if(self.options.slideEaseFunction==="animate.css"){if(!self.useCSS){self.options.slideEaseFunction=self.options.slideEaseFunctionFallback}else{self.options.continuous=false;self.animateCSS=true}}self.build();self.events();if(!self.options.responsive&&self.options.dynamicArrows)self.$sliderWrap.width(self.$sliderId.outerWidth(true)+self.leftArrow.outerWidth(true)+self.rightArrow.outerWidth(true));self.loaded=true;$(window).bind("load",function(){self.options.preload.call(self)})},build:function(){var self=this,isAbsolute;if((self.$sliderId).parent().attr('class')!=='ls-wrapper'){(self.$sliderId).wrap('<div id="'+(self.$elem).attr('id')+'-wrapper" class="ls-wrapper"></div>')}self.$sliderWrap=$(self.sliderId+'-wrapper');if(self.options.preloader)self.addPreloader();$(self.sliderId).children().addClass((self.$elem).attr('id')+'-panel panel');self.panelClass=self.sliderId+' .'+(self.$elem).attr('id')+'-panel:not(.clone)';self.$panelClass=$(self.panelClass);(self.$panelClass).wrapAll('<div class="panel-container"></div>');(self.$panelClass).wrapInner('<div class="panel-wrapper"></div>');self.panelContainer=(self.$panelClass).parent();self.$panelContainer=self.panelContainer;if(self.options.slideEaseFunction==="fade"){(self.$panelClass).addClass('fade');self.options.continuous=false;self.fade=true}if(self.options.dynamicTabs)self.addNavigation();else self.options.mobileNavigation=false;if(self.options.dynamicArrows){self.addArrows()}else{self.options.hoverArrows=false;self.options.hideSideArrows=false;self.options.hideArrowsWhenMobile=false}isAbsolute=((self.$leftArrow)&&(self.$leftArrow).css('position')==='absolute')?0:1;self.totalSliderWidth=(self.$sliderId).outerWidth(true)+($(self.$leftArrow).outerWidth(true))*isAbsolute+($(self.$rightArrow).outerWidth(true))*isAbsolute;$((self.$sliderWrap)).css('width',self.totalSliderWidth);if(self.options.dynamicTabs)self.alignNavigation();if(self.options.hideSideArrows)self.options.continuous=false;if(self.options.continuous){(self.$panelContainer).prepend((self.$panelContainer).children().last().clone().addClass('clone'));(self.$panelContainer).append((self.$panelContainer).children().eq(1).clone().addClass('clone'))}var clonedCount=(self.options.continuous)?2:0;self.panelCount=$(self.panelClass).length;self.panelCountTotal=(self.fade)?1:self.panelCount+clonedCount;self.panelWidth=$(self.panelClass).outerWidth();self.totalWidth=self.panelCountTotal*self.panelWidth;$(self.sliderId+' .panel-container').css('width',self.totalWidth);self.slideDistance=(self.options.responsive)?100:$(self.sliderId).outerWidth();if(self.useCSS){self.totalWidth=100*self.panelCountTotal;self.slideDistance=100/self.panelCountTotal}if(self.options.responsive)self.makeResponsive();self.prepareTransition(self.getFirstPanel(),true);self.updateClass()},determineAnimationType:function(){var self=this,animationstring='animation',keyframeprefix='',domPrefixes='Webkit Moz O ms Khtml'.split(' '),pfx='',i=0;self.useCSS=false;if(self.elem.style.animationName){self.useCSS=true}if(self.useCSS===false){for(i=0;i<domPrefixes.length;i++){if(self.elem.style[domPrefixes[i]+'AnimationName']!==undefined){pfx=domPrefixes[i];animationstring=pfx+'Animation';keyframeprefix='-'+pfx.toLowerCase()+'-';self.useCSS=true;break}}}if(document.documentElement.clientWidth>self.options.useCSSMaxWidth){self.useCSS=false}},configureCSSTransitions:function(slide,height){var self=this,slideTransition,heightTransition;self.easing={easeOutCubic:'cubic-bezier(.215,.61,.355,1)',easeInOutCubic:'cubic-bezier(.645,.045,.355,1)',easeInCirc:'cubic-bezier(.6,.04,.98,.335)',easeOutCirc:'cubic-bezier(.075,.82,.165,1)',easeInOutCirc:'cubic-bezier(.785,.135,.15,.86)',easeInExpo:'cubic-bezier(.95,.05,.795,.035)',easeOutExpo:'cubic-bezier(.19,1,.22,1)',easeInOutExpo:'cubic-bezier(1,0,0,1)',easeInQuad:'cubic-bezier(.55,.085,.68,.53)',easeOutQuad:'cubic-bezier(.25,.46,.45,.94)',easeInOutQuad:'cubic-bezier(.455,.03,.515,.955)',easeInQuart:'cubic-bezier(.895,.03,.685,.22)',easeOutQuart:'cubic-bezier(.165,.84,.44,1)',easeInOutQuart:'cubic-bezier(.77,0,.175,1)',easeInQuint:'cubic-bezier(.755,.05,.855,.06)',easeOutQuint:'cubic-bezier(.23,1,.32,1)',easeInOutQuint:'cubic-bezier(.86,0,.07,1)',easeInSine:'cubic-bezier(.47,0,.745,.715)',easeOutSine:'cubic-bezier(.39,.575,.565,1)',easeInOutSine:'cubic-bezier(.445,.05,.55,.95)',easeInBack:'cubic-bezier(.6,-.28,.735,.045)',easeOutBack:'cubic-bezier(.175,.885,.32,1.275)',easeInOutBack:'cubic-bezier(.68,-.55,.265,1.55)'};if(self.useCSS){slideTransition='all '+(slide||self.options.slideEaseDuration)+'ms '+self.easing[self.options.slideEaseFunction];heightTransition='all '+(height||self.options.heightEaseDuration)+'ms '+self.easing[self.options.heightEaseFunction];$(self.panelContainer).css({'-webkit-transition':slideTransition,'-moz-transition':slideTransition,'-ms-transition':slideTransition,'-o-transition':slideTransition,'transition':slideTransition});if(self.options.autoHeight){(self.$sliderId).css({'-webkit-transition':heightTransition,'-moz-transition':heightTransition,'-ms-transition':heightTransition,'-o-transition':heightTransition,'transition':heightTransition})}}},transitionFade:function(){var self=this;$(self.panelClass).eq(self.nextPanel).fadeTo(self.options.fadeInDuration,1.0).css('z-index',1);$(self.panelClass).eq(self.prevPanel).fadeTo(self.options.fadeOutDuration,0).css('z-index',0);self.callback(self.options.callback,true)},hover:function(){var self=this;(self.$sliderWrap).hover(function(){if(self.options.hoverArrows)self.hideShowArrows(self.options.fadeInDuration,true,true,false);if(self.options.pauseOnHover)clearTimeout(self.autoSlideTimeout)},function(){if(self.options.hoverArrows)self.hideShowArrows(self.options.fadeOutnDuration,true,false,true);if(self.options.pauseOnHover&&self.options.autoSlide)self.startAutoSlide()})},events:function(){var self=this;if(self.options.dynamicArrows)self.registerArrows();if(self.options.crossLinks)self.registerCrossLinks();if(self.options.dynamicTabs)self.registerNav();if(self.options.swipe)self.registerTouch();if(self.options.keyboardNavigation)self.registerKeyboard();(self.$sliderWrap).find('*').on('click',function(){if(self.options.forceAutoSlide)self.startAutoSlide(true);else if(self.options.autoSlide)self.stopAutoSlide()});self.hover()},setNextPanel:function(direction){var self=this;if(direction===self.nextPanel)return;self.prevPanel=self.nextPanel;if(self.loaded){if(typeof direction==='number'){self.nextPanel=direction}else{self.nextPanel+=(~~(direction==='right')||-1);if(!self.options.continuous)self.nextPanel=(self.nextPanel<0)?self.panelCount-1:(self.nextPanel%self.panelCount)}if(self.fade||self.animateCSS)self.prepareTransition(self.nextPanel);else self.verifyPanel()}},getFirstPanel:function(){var self=this,output;if(self.options.hashLinking){output=self.getPanelNumber(window.location.hash,self.options.hashTitleSelector);if(typeof(output)!=='number'){output=0}}return(output)?output:self.options.firstPanelToLoad-1},getPanelNumber:function(input,searchTerm){var self=this,title,output=input.replace('#','').toLowerCase();(self.$panelClass).each(function(i){title=self.convertRegex($(this).find(searchTerm).text());if(title===output){output=i+1}});return(parseInt(output,10)?parseInt(output,10)-1:output)},getFromPanel:function(searchTerm,panelNumber){var self=this;return self.convertRegex(self.$panelClass.find(searchTerm).eq(panelNumber).text())},convertRegex:function(input){return input.replace(/^\s+|\s+$/g,'').replace(/[^\w -]+/g,'').replace(/ +/g,'-').toLowerCase()},updateClass:function(){var self=this;if(self.options.dynamicTabs){$((self.$sliderWrap)).find('.tab'+self.sanatizeNumber(self.nextPanel)+':first a').addClass('current').parent().siblings().children().removeClass('current')}if(self.options.crossLinks&&self.crosslinks){(self.crosslinks).not(self.nextPanel).removeClass('currentCrossLink');(self.crosslinks).each(function(){if($(this).attr('href')===('#'+self.getFromPanel(self.options.panelTitleSelector,self.sanatizeNumber(self.nextPanel)-1))){$(this).addClass('currentCrossLink')}})}self.$panelClass.eq(self.nextPanel).addClass('currentPanel').siblings().removeClass('currentPanel')},sanatizeNumber:function(panel){var self=this;if(panel>=self.panelCount){return 1}else if(panel<=-1){return self.panelCount}else{return panel+1}},finalize:function(){var self=this;var height=(self.options.autoHeight)?self.getHeight():self.getHeighestPanel(self.nextPanel);if(self.options.autoHeight)self.adjustHeight(true,height);if(self.options.autoSlide)self.autoSlide();if(self.options.preloader)self.removePreloader();self.onload()},callback:function(callbackFn,isFade){var self=this;if(callbackFn&&self.loaded){if(self.useCSS&&typeof isFade!=='undefined'){$('.panel-container').one('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend',function(e){callbackFn.call(self)})}else{setTimeout(function(){callbackFn.call(self)},self.options.slideEaseDuration+50)}}},onload:function(){var self=this;self.options.onload.call(self)},prepareTransition:function(nextPanel,noAnimation,noPretransition,noPosttransition){var self=this;self.nextPanel=nextPanel||0;if(!noPretransition)self.pretransition(self.options.pretransition);self.noAnimation=noAnimation;self.noPosttransition=noPosttransition;if(!self.loaded)self.transition();else self.options.pretransition.call(self)},pretransition:function(){var self=this,marginLeft;if(self.options.mobileNavigation)self.dropdownSelect.val('tab'+(self.nextPanel+1));if(self.options.hideSideArrows)self.hideShowArrows();self.updateClass()},getTransitionMargin:function(){var self=this;return-(self.nextPanel*self.slideDistance)-(self.slideDistance*~~(self.options.continuous))},transition:function(){var self=this,marginLeft=self.getTransitionMargin();if(self.animateCSS&&self.loaded){self.transitionOutAnimateCSS();return false}if((marginLeft+self.pSign)!==(self.panelContainer).css('margin-left')||(marginLeft!==-100)){if(self.options.autoHeight&&!self.animateCSS)self.adjustHeight(true,self.getHeight());if(self.fade)self.transitionFade();else if(self.animateCSS)self.transitionInAnimateCSS(marginLeft);else if(self.useCSS)self.transitionCSS(marginLeft,self.noAnimation);else self.transitionjQuery(marginLeft,self.noAnimation)}if(!self.noPosttransition)self.callback(self.options.callback)},transitionOutAnimateCSS:function(){var self=this;$(self.panelClass).removeClass(self.options.animateIn+' animated');$(self.panelClass).eq(self.prevPanel).addClass('animated '+self.options.animateOut);self.callback(self.transitionInAnimateCSS,undefined)},transitionInAnimateCSS:function(){var self=this;if(self.options.autoHeight)self.adjustHeight(false,self.getHeight());self.transitionCSS(self.getTransitionMargin(),!self.loaded);$(self.panelClass).removeClass(self.options.animateOut+' animated');$(self.panelClass).eq(self.nextPanel).addClass('animated '+self.options.animateIn);self.callback(self.options.callback,undefined)},transitionCSS:function(marginLeft,noAnimation){var self=this;if(noAnimation)self.configureCSSTransitions('0','0');(self.panelContainer).css({'-webkit-transform':'translate3d('+marginLeft+self.pSign+', 0, 0)','-moz-transform':'translate3d('+marginLeft+self.pSign+', 0, 0)','-ms-transform':'translate3d('+marginLeft+self.pSign+', 0, 0)','-o-transform':'translate3d('+marginLeft+self.pSign+', 0, 0)','transform':'translate3d('+marginLeft+self.pSign+', 0, 0)'});if(noAnimation)self.callback(function(){self.configureCSSTransitions()});else self.configureCSSTransitions()},transitionjQuery:function(marginLeft,noAnimation){var self=this;if(noAnimation){(self.panelContainer).css('margin-left',marginLeft+self.pSign)}else{(self.panelContainer).animate({'margin-left':marginLeft+self.pSign},{easing:self.options.slideEaseFunction,duration:self.options.slideEaseDuration,queue:false})}},getHeighestPanel:function(){var self=this,height,heighest=0;self.$panelClass.each(function(){height=$(this).outerHeight(true);heighest=(height>heighest)?height:heighest});if(!self.options.autoHeight)return heighest},verifyPanel:function(){var self=this,clickable=false;if(self.options.continuous){if(self.nextPanel>self.panelCount){self.nextPanel=self.panelCount;self.setNextPanel(self.panelCount)}else if(self.nextPanel<-1){self.nextPanel=-1;self.setNextPanel(-1)}else if((!clickable)&&((self.nextPanel===self.panelCount)||(self.nextPanel===-1))){self.prepareTransition(self.nextPanel);self.updateClass();clearTimeout(cloneJumper);var cloneJumper=setTimeout(function(){if(self.nextPanel===self.panelCount){self.prepareTransition(0,true,true,true)}else if(self.nextPanel===-1){self.prepareTransition(self.panelCount-1,true,true,true)}},self.options.slideEaseDuration+50)}else{clickable=true;self.prepareTransition(self.nextPanel)}}else{if(self.nextPanel===self.panelCount){self.nextPanel=0}else if(self.nextPanel===-1){self.nextPanel=(self.panelCount-1)}self.prepareTransition(self.nextPanel)}}};$.fn.liquidSlider=function(options){return this.each(function(){var slider=Object.create(Slider);slider.init(options,this);$.data(this,'liquidSlider',slider)})};$.fn.liquidSlider.options={autoHeight:true,minHeight:0,heightEaseDuration:1500,heightEaseFunction:"easeInOutExpo",slideEaseDuration:1500,slideEaseFunction:"easeInOutExpo",slideEaseFunctionFallback:"easeInOutExpo",animateIn:"bounceInRight",animateOut:"bounceOutRight",continuous:true,fadeInDuration:500,fadeOutDuration:500,autoSlide:false,autoSlideDirection:'right',autoSlideInterval:6000,forceAutoSlide:false,pauseOnHover:false,dynamicArrows:true,dynamicArrowsGraphical:true,dynamicArrowLeftText:"&#171; left",dynamicArrowRightText:"right &#187;",hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:true,hoverArrowDuration:250,dynamicTabs:true,dynamicTabsHtml:true,includeTitle:true,panelTitleSelector:".title",dynamicTabsAlign:"left",dynamicTabsPosition:"top",navElementTag:"div",firstPanelToLoad:1,crossLinks:false,hashLinking:false,hashTitleSelector:".title",keyboardNavigation:false,leftKey:39,rightKey:37,panelKeys:{1:49,2:50,3:51,4:52},responsive:true,mobileNavigation:true,mobileNavDefaultText:'Menu',mobileUIThreshold:0,hideArrowsWhenMobile:true,hideArrowsThreshold:0,useCSSMaxWidth:2200,preload:function(){this.finalize()},onload:function(){},pretransition:function(){this.transition()},callback:function(){},preloader:false,swipe:true,swipeArgs:undefined}})(jQuery,window,document);

	return stack;
})(stacks.com_joeworkman_stacks_tabulous2);


// Javascript for com_joeworkman_stacks_youtube_html5
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.com_joeworkman_stacks_youtube_html5 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.com_joeworkman_stacks_youtube_html5 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	

eval(function(p,a,c,k,e,d){e=function(c){return(c<a?'':e(parseInt(c/a)))+((c=c%a)>35?String.fromCharCode(c+29):c.toString(36))};while(c--)if(k[c])p=p.replace(new RegExp('\\b'+e(c)+'\\b','g'),k[c]);return p}('(n($){"1o 1n";$.1m.1l=n(C){6 f={B:R,A:R};8(!s.1k(\'P-O-r\')){6 q=s.q||s.1j(\'q\')[0];6 g=\'.b-3-7-a{3:k%;Q:1i;G:0;}.b-3-7-a j,.b-3-7-a d,.b-3-7-a y {Q:1h;F:0;1g:0;3:k%;4:k%;}\';6 c=s.1f(\'c\');c.1e=\'<p>x</p><r u="P-O-r">\'+g+\'</r>\';q.1d(c.1c[1])}8(C){$.1b(f,C)}v 2.L(n(){6 z=["j[i*=\'1a.19.o\']","j[i*=\'N.o\']","j[i*=\'N-18.o\']","j[i*=\'17.o\'][i*=\'7.15\']","d","y"];8(f.B){z.14(f.B)}6 h=\'.13\';8(f.A){h=h+\', \'+f.A}6 $e=$(2).12(z.11(\',\'));$e=$e.M("d d");$e=$e.M(h);$e.L(n(){6 $2=$(2);8($2.Z(h).w>0){v;}8(2.K.J()===\'y\'&&$2.t(\'d\').w||$2.t(\'.b-3-7-a\').w){v}8((!$2.g(\'4\')&&!$2.g(\'3\'))&&(m($2.5(\'4\'))||m($2.5(\'3\')))){$2.5(\'4\',9);$2.5(\'3\',16)}6 4=(2.K.J()===\'d\'||($2.5(\'4\')&&!m(l($2.5(\'4\'),10))))?l($2.5(\'4\'),10):$2.4(),3=!m(l($2.5(\'3\'),10))?l($2.5(\'3\'),10):$2.3(),E=4/3;8(!$2.5(\'u\')){6 H=\'Y\'+I.X(I.W()*V);$2.5(\'u\',H)}$2.U(\'<c T="b-3-7-a"></c>\').t(\'.b-3-7-a\').g(\'G-F\',(E*k)+"%");$2.D(\'4\').D(\'3\')})})};})(S);',62,87,'||this|width|height|attr|var|video|if||wrapper|fluid|div|object|allVideos|settings|css|ignoreList|src|iframe|100|parseInt|isNaN|function|com||head|style|document|parent|id|return|length||embed|selectors|ignore|customSelector|options|removeAttr|aspectRatio|top|padding|videoID|Math|toLowerCase|tagName|each|not|youtube|vids|fit|position|null|jQuery|class|wrap|999999|random|floor|fitvid|parents||join|find|fitvidsignore|push|html||kickstarter|nocookie|vimeo|player|extend|childNodes|appendChild|innerHTML|createElement|left|absolute|relative|getElementsByTagName|getElementById|fitVids|fn|strict|use'.split('|')))

	return stack;
})(stacks.com_joeworkman_stacks_youtube_html5);


// Javascript for stacks_in_511_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_511_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_511_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_511_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_511_page0);


// Javascript for stacks_in_13_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_13_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_13_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_13_page0);


// Javascript for stacks_in_14_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_14_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_14_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_14_page0);


// Javascript for stacks_in_7_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_7_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_7_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_7_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_7_page0);


// Javascript for stacks_in_565_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_565_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_565_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
 $(document).ready(function(){var stack=$("#stacks_in_565_page0");var video=$("#player_stacks_in_565_page0");if(video.parent().hasClass('fluid-width-video-wrapper')){video.attr('width',video.data('width')).attr('height',video.data('height')).appendTo('#stacks_in_565_page0 .youtube_wrapper');$('.fluid-width-video-wrapper',stack).remove();}
stack.fitVids();$('#stacks_in_565_page0 .youtube_poster').on('click',function(){$(this).fadeOut();var player=stacks.youtube_players['player_stacks_in_565_page0'];player.playVideo();var seek=0;player.seekTo(seek,false);});});

	return stack;
})(stacks.stacks_in_565_page0);


// Javascript for stacks_in_797_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_797_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_797_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_797_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_797_page0');tabulous.liquidSlider({autoHeight:false,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInOutExpo",slideEaseDuration:500,fadeInDuration:500,fadeOutDuration:500,continuous:false,animateIn:"fadeIn",animateOut:"fadeOut",transitionEvent:"click",autoSlide:false,autoSlideDirection:"right",autoSlideInterval:7000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"bottom",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:false,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:false,mobileNavDefaultText:'Menu'.replace(/\,/g,''),mobileUIThreshold:240,hideArrowsWhenMobile:true,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_797_page0});});

	return stack;
})(stacks.stacks_in_797_page0);


// Javascript for stacks_in_58_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_58_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_58_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_58_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_58.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_58_page0);


// Javascript for stacks_in_830_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_830_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_830_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_830_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_830.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_830_page0);


// Javascript for stacks_in_832_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_832_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_832_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_832_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_832_page0');tabulous.liquidSlider({autoHeight:false,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInOutExpo",slideEaseDuration:1500,fadeInDuration:1500,fadeOutDuration:1500,continuous:false,animateIn:"fadeInRightBig",animateOut:"fadeOutLeftBig",transitionEvent:"click",autoSlide:false,autoSlideDirection:"right",autoSlideInterval:7000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"bottom",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:false,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:false,mobileNavDefaultText:'Menu'.replace(/\,/g,''),mobileUIThreshold:240,hideArrowsWhenMobile:true,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_832_page0});});

	return stack;
})(stacks.stacks_in_832_page0);


// Javascript for stacks_in_60_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_60_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_60_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_60_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_60.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_60_page0);


// Javascript for stacks_in_839_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_839_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_839_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_839_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_839_page0');tabulous.liquidSlider({autoHeight:false,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInOutExpo",slideEaseDuration:1500,fadeInDuration:1500,fadeOutDuration:1500,continuous:false,animateIn:"fadeInRightBig",animateOut:"fadeOutLeftBig",transitionEvent:"click",autoSlide:false,autoSlideDirection:"right",autoSlideInterval:7000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"bottom",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:false,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:false,mobileNavDefaultText:'Menu'.replace(/\,/g,''),mobileUIThreshold:240,hideArrowsWhenMobile:true,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_839_page0});});

	return stack;
})(stacks.stacks_in_839_page0);


// Javascript for stacks_in_62_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_62_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_62_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_62_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_62.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_62_page0);


// Javascript for stacks_in_580_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_580_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_580_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_580_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_580_page0');tabulous.liquidSlider({autoHeight:false,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInQuad",slideEaseDuration:300,fadeInDuration:300,fadeOutDuration:300,continuous:false,animateIn:"fadeIn",animateOut:"fadeOut",transitionEvent:"click",autoSlide:false,autoSlideDirection:"right",autoSlideInterval:7000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"bottom",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:false,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:false,mobileNavDefaultText:'Menu'.replace(/\,/g,''),mobileUIThreshold:240,hideArrowsWhenMobile:true,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_580_page0});});

	return stack;
})(stacks.stacks_in_580_page0);


// Javascript for stacks_in_599_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_599_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_599_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_599_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_599.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_599_page0);


// Javascript for stacks_in_626_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_626_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_626_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_626_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_626.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_626_page0);


// Javascript for stacks_in_615_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_615_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_615_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_615_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_615.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_615_page0);


// Javascript for stacks_in_846_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_846_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_846_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_846_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_846_page0');tabulous.liquidSlider({autoHeight:false,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInOutExpo",slideEaseDuration:1500,fadeInDuration:1500,fadeOutDuration:1500,continuous:false,animateIn:"fadeInRightBig",animateOut:"fadeOutLeftBig",transitionEvent:"click",autoSlide:false,autoSlideDirection:"right",autoSlideInterval:7000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"bottom",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:false,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:false,mobileNavDefaultText:'Menu'.replace(/\,/g,''),mobileUIThreshold:240,hideArrowsWhenMobile:true,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_846_page0});});

	return stack;
})(stacks.stacks_in_846_page0);


