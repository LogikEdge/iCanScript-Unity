
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


// Javascript for com_joeworkman_stacks_cycler3
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.com_joeworkman_stacks_cycler3 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.com_joeworkman_stacks_cycler3 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
 if(!$.fn.cycle){(function($){var ver="2.99";if($.support==undefined){$.support={opacity:!($.browser.msie)};}function debug(s){$.fn.cycle.debug&&log(s);}function log(){window.console&&console.log&&console.log("[cycle] "+Array.prototype.join.call(arguments," "));}$.expr[":"].paused=function(el){return el.cyclePause;};$.fn.cycle=function(options,arg2){var o={s:this.selector,c:this.context};if(this.length===0&&options!="stop"){if(!$.isReady&&o.s){log("DOM not ready, queuing slideshow");$(function(){$(o.s,o.c).cycle(options,arg2);});return this;}log("terminating; zero elements found by selector"+($.isReady?"":" (DOM not ready)"));return this;}return this.each(function(){var opts=handleArguments(this,options,arg2);if(opts===false){return;}opts.updateActivePagerLink=opts.updateActivePagerLink||$.fn.cycle.updateActivePagerLink;if(this.cycleTimeout){clearTimeout(this.cycleTimeout);}this.cycleTimeout=this.cyclePause=0;var $cont=$(this);var $slides=opts.slideExpr?$(opts.slideExpr,this):$cont.children();var els=$slides.get();if(els.length<2){log("terminating; too few slides: "+els.length);return;}var opts2=buildOptions($cont,$slides,els,opts,o);if(opts2===false){return;}var startTime=opts2.continuous?10:getTimeout(els[opts2.currSlide],els[opts2.nextSlide],opts2,!opts2.backwards);if(startTime){startTime+=(opts2.delay||0);if(startTime<10){startTime=10;}debug("first timeout: "+startTime);this.cycleTimeout=setTimeout(function(){go(els,opts2,0,!opts.backwards);},startTime);}});};function handleArguments(cont,options,arg2){if(cont.cycleStop==undefined){cont.cycleStop=0;}if(options===undefined||options===null){options={};}if(options.constructor==String){switch(options){case"destroy":case"stop":var opts=$(cont).data("cycle.opts");if(!opts){return false;}cont.cycleStop++;if(cont.cycleTimeout){clearTimeout(cont.cycleTimeout);}cont.cycleTimeout=0;$(cont).removeData("cycle.opts");if(options=="destroy"){destroy(opts);}return false;case"toggle":cont.cyclePause=(cont.cyclePause===1)?0:1;checkInstantResume(cont.cyclePause,arg2,cont);return false;case"pause":cont.cyclePause=1;return false;case"resume":cont.cyclePause=0;checkInstantResume(false,arg2,cont);return false;case"prev":case"next":var opts=$(cont).data("cycle.opts");if(!opts){log('options not found, "prev/next" ignored');return false;}$.fn.cycle[options](opts);return false;default:options={fx:options};}return options;}else{if(options.constructor==Number){var num=options;options=$(cont).data("cycle.opts");if(!options){log("options not found, can not advance slide");return false;}if(num<0||num>=options.elements.length){log("invalid slide index: "+num);return false;}options.nextSlide=num;if(cont.cycleTimeout){clearTimeout(cont.cycleTimeout);cont.cycleTimeout=0;}if(typeof arg2=="string"){options.oneTimeFx=arg2;}go(options.elements,options,1,num>=options.currSlide);return false;}}return options;function checkInstantResume(isPaused,arg2,cont){if(!isPaused&&arg2===true){var options=$(cont).data("cycle.opts");if(!options){log("options not found, can not resume");return false;}if(cont.cycleTimeout){clearTimeout(cont.cycleTimeout);cont.cycleTimeout=0;}go(options.elements,options,1,!options.backwards);}}}function removeFilter(el,opts){if(!$.support.opacity&&opts.cleartype&&el.style.filter){try{el.style.removeAttribute("filter");}catch(smother){}}}function destroy(opts){if(opts.next){$(opts.next).unbind(opts.prevNextEvent);}if(opts.prev){$(opts.prev).unbind(opts.prevNextEvent);}if(opts.pager||opts.pagerAnchorBuilder){$.each(opts.pagerAnchors||[],function(){this.unbind().remove();});}opts.pagerAnchors=null;if(opts.destroy){opts.destroy(opts);}}function buildOptions($cont,$slides,els,options,o){var opts=$.extend({},$.fn.cycle.defaults,options||{},$.metadata?$cont.metadata():$.meta?$cont.data():{});if(opts.autostop){opts.countdown=opts.autostopCount||els.length;}var cont=$cont[0];$cont.data("cycle.opts",opts);opts.$cont=$cont;opts.stopCount=cont.cycleStop;opts.elements=els;opts.before=opts.before?[opts.before]:[];opts.after=opts.after?[opts.after]:[];if(!$.support.opacity&&opts.cleartype){opts.after.push(function(){removeFilter(this,opts);});}if(opts.continuous){opts.after.push(function(){go(els,opts,0,!opts.backwards);});}saveOriginalOpts(opts);if(!$.support.opacity&&opts.cleartype&&!opts.cleartypeNoBg){clearTypeFix($slides);}if($cont.css("position")=="static"){$cont.css("position","relative");}if(opts.width){$cont.width(opts.width);}if(opts.height&&opts.height!="auto"){$cont.height(opts.height);}if(opts.startingSlide){opts.startingSlide=parseInt(opts.startingSlide);}else{if(opts.backwards){opts.startingSlide=els.length-1;}}if(opts.random){opts.randomMap=[];for(var i=0;i<els.length;i++){opts.randomMap.push(i);}opts.randomMap.sort(function(a,b){return Math.random()-0.5;});opts.randomIndex=1;opts.startingSlide=opts.randomMap[1];}else{if(opts.startingSlide>=els.length){opts.startingSlide=0;}}opts.currSlide=opts.startingSlide||0;var first=opts.startingSlide;$slides.css({position:"absolute",top:0,left:0}).hide().each(function(i){var z;if(opts.backwards){z=first?i<=first?els.length+(i-first):first-i:els.length-i;}else{z=first?i>=first?els.length-(i-first):first-i:els.length-i;}$(this).css("z-index",z);});$(els[first]).css("opacity",1).show();removeFilter(els[first],opts);if(opts.fit&&opts.width){$slides.width(opts.width);}if(opts.fit&&opts.height&&opts.height!="auto"){$slides.height(opts.height);}var reshape=opts.containerResize&&!$cont.innerHeight();if(reshape){var maxw=0,maxh=0;for(var j=0;j<els.length;j++){var $e=$(els[j]),e=$e[0],w=$e.outerWidth(),h=$e.outerHeight();if(!w){w=e.offsetWidth||e.width||$e.attr("width");}if(!h){h=e.offsetHeight||e.height||$e.attr("height");}maxw=w>maxw?w:maxw;maxh=h>maxh?h:maxh;}if(maxw>0&&maxh>0){$cont.css({width:maxw+"px",height:maxh+"px"});}}if(opts.pause){$cont.hover(function(){this.cyclePause++;},function(){this.cyclePause--;});}if(supportMultiTransitions(opts)===false){return false;}var requeue=false;options.requeueAttempts=options.requeueAttempts||0;$slides.each(function(){var $el=$(this);this.cycleH=(opts.fit&&opts.height)?opts.height:($el.height()||this.offsetHeight||this.height||$el.attr("height")||0);this.cycleW=(opts.fit&&opts.width)?opts.width:($el.width()||this.offsetWidth||this.width||$el.attr("width")||0);if($el.is("img")){var loadingIE=($.browser.msie&&this.cycleW==28&&this.cycleH==30&&!this.complete);var loadingFF=($.browser.mozilla&&this.cycleW==34&&this.cycleH==19&&!this.complete);var loadingOp=($.browser.opera&&((this.cycleW==42&&this.cycleH==19)||(this.cycleW==37&&this.cycleH==17))&&!this.complete);var loadingOther=(this.cycleH==0&&this.cycleW==0&&!this.complete);if(loadingIE||loadingFF||loadingOp||loadingOther){if(o.s&&opts.requeueOnImageNotLoaded&&++options.requeueAttempts<100){log(options.requeueAttempts," - img slide not loaded, requeuing slideshow: ",this.src,this.cycleW,this.cycleH);setTimeout(function(){$(o.s,o.c).cycle(options);},opts.requeueTimeout);requeue=true;return false;}else{log("could not determine size of image: "+this.src,this.cycleW,this.cycleH);}}}return true;});if(requeue){return false;}opts.cssBefore=opts.cssBefore||{};opts.cssAfter=opts.cssAfter||{};opts.cssFirst=opts.cssFirst||{};opts.animIn=opts.animIn||{};opts.animOut=opts.animOut||{};$slides.not(":eq("+first+")").css(opts.cssBefore);$($slides[first]).css(opts.cssFirst);if(opts.timeout){opts.timeout=parseInt(opts.timeout);if(opts.speed.constructor==String){opts.speed=$.fx.speeds[opts.speed]||parseInt(opts.speed);}if(!opts.sync){opts.speed=opts.speed/2;}var buffer=opts.fx=="none"?0:opts.fx=="shuffle"?500:250;while((opts.timeout-opts.speed)<buffer){opts.timeout+=opts.speed;}}if(opts.easing){opts.easeIn=opts.easeOut=opts.easing;}if(!opts.speedIn){opts.speedIn=opts.speed;}if(!opts.speedOut){opts.speedOut=opts.speed;}opts.slideCount=els.length;opts.currSlide=opts.lastSlide=first;if(opts.random){if(++opts.randomIndex==els.length){opts.randomIndex=0;}opts.nextSlide=opts.randomMap[opts.randomIndex];}else{if(opts.backwards){opts.nextSlide=opts.startingSlide==0?(els.length-1):opts.startingSlide-1;}else{opts.nextSlide=opts.startingSlide>=(els.length-1)?0:opts.startingSlide+1;}}if(!opts.multiFx){var init=$.fn.cycle.transitions[opts.fx];if($.isFunction(init)){init($cont,$slides,opts);}else{if(opts.fx!="custom"&&!opts.multiFx){log("unknown transition: "+opts.fx,"; slideshow terminating");return false;}}}var e0=$slides[first];if(opts.before.length){opts.before[0].apply(e0,[e0,e0,opts,true]);}if(opts.after.length){opts.after[0].apply(e0,[e0,e0,opts,true]);}if(opts.next){$(opts.next).bind(opts.prevNextEvent,function(){return advance(opts,1);});}if(opts.prev){$(opts.prev).bind(opts.prevNextEvent,function(){return advance(opts,0);});}if(opts.pager||opts.pagerAnchorBuilder){buildPager(els,opts);}exposeAddSlide(opts,els);return opts;}function saveOriginalOpts(opts){opts.original={before:[],after:[]};opts.original.cssBefore=$.extend({},opts.cssBefore);opts.original.cssAfter=$.extend({},opts.cssAfter);opts.original.animIn=$.extend({},opts.animIn);opts.original.animOut=$.extend({},opts.animOut);$.each(opts.before,function(){opts.original.before.push(this);});$.each(opts.after,function(){opts.original.after.push(this);});}function supportMultiTransitions(opts){var i,tx,txs=$.fn.cycle.transitions;if(opts.fx.indexOf(",")>0){opts.multiFx=true;opts.fxs=opts.fx.replace(/\s*/g,"").split(",");for(i=0;i<opts.fxs.length;i++){var fx=opts.fxs[i];tx=txs[fx];if(!tx||!txs.hasOwnProperty(fx)||!$.isFunction(tx)){log("discarding unknown transition: ",fx);opts.fxs.splice(i,1);i--;}}if(!opts.fxs.length){log("No valid transitions named; slideshow terminating.");return false;}}else{if(opts.fx=="all"){opts.multiFx=true;opts.fxs=[];for(p in txs){tx=txs[p];if(txs.hasOwnProperty(p)&&$.isFunction(tx)){opts.fxs.push(p);}}}}if(opts.multiFx&&opts.randomizeEffects){var r1=Math.floor(Math.random()*20)+30;for(i=0;i<r1;i++){var r2=Math.floor(Math.random()*opts.fxs.length);opts.fxs.push(opts.fxs.splice(r2,1)[0]);}debug("randomized fx sequence: ",opts.fxs);}return true;}function exposeAddSlide(opts,els){opts.addSlide=function(newSlide,prepend){var $s=$(newSlide),s=$s[0];if(!opts.autostopCount){opts.countdown++;}els[prepend?"unshift":"push"](s);if(opts.els){opts.els[prepend?"unshift":"push"](s);}opts.slideCount=els.length;$s.css("position","absolute");$s[prepend?"prependTo":"appendTo"](opts.$cont);if(prepend){opts.currSlide++;opts.nextSlide++;}if(!$.support.opacity&&opts.cleartype&&!opts.cleartypeNoBg){clearTypeFix($s);}if(opts.fit&&opts.width){$s.width(opts.width);}if(opts.fit&&opts.height&&opts.height!="auto"){$s.height(opts.height);}s.cycleH=(opts.fit&&opts.height)?opts.height:$s.height();s.cycleW=(opts.fit&&opts.width)?opts.width:$s.width();$s.css(opts.cssBefore);if(opts.pager||opts.pagerAnchorBuilder){$.fn.cycle.createPagerAnchor(els.length-1,s,$(opts.pager),els,opts);}if($.isFunction(opts.onAddSlide)){opts.onAddSlide($s);}else{$s.hide();}};}$.fn.cycle.resetState=function(opts,fx){fx=fx||opts.fx;opts.before=[];opts.after=[];opts.cssBefore=$.extend({},opts.original.cssBefore);opts.cssAfter=$.extend({},opts.original.cssAfter);opts.animIn=$.extend({},opts.original.animIn);opts.animOut=$.extend({},opts.original.animOut);opts.fxFn=null;$.each(opts.original.before,function(){opts.before.push(this);});$.each(opts.original.after,function(){opts.after.push(this);});var init=$.fn.cycle.transitions[fx];if($.isFunction(init)){init(opts.$cont,$(opts.elements),opts);}};function go(els,opts,manual,fwd){if(manual&&opts.busy&&opts.manualTrump){debug("manualTrump in go(), stopping active transition");$(els).stop(true,true);opts.busy=0;}if(opts.busy){debug("transition active, ignoring new tx request");return;}var p=opts.$cont[0],curr=els[opts.currSlide],next=els[opts.nextSlide];if(p.cycleStop!=opts.stopCount||p.cycleTimeout===0&&!manual){return;}if(!manual&&!p.cyclePause&&!opts.bounce&&((opts.autostop&&(--opts.countdown<=0))||(opts.nowrap&&!opts.random&&opts.nextSlide<opts.currSlide))){if(opts.end){opts.end(opts);}return;}var changed=false;if((manual||!p.cyclePause)&&(opts.nextSlide!=opts.currSlide)){changed=true;var fx=opts.fx;curr.cycleH=curr.cycleH||$(curr).height();curr.cycleW=curr.cycleW||$(curr).width();next.cycleH=next.cycleH||$(next).height();next.cycleW=next.cycleW||$(next).width();if(opts.multiFx){if(opts.lastFx==undefined||++opts.lastFx>=opts.fxs.length){opts.lastFx=0;}fx=opts.fxs[opts.lastFx];opts.currFx=fx;}if(opts.oneTimeFx){fx=opts.oneTimeFx;opts.oneTimeFx=null;}$.fn.cycle.resetState(opts,fx);if(opts.before.length){$.each(opts.before,function(i,o){if(p.cycleStop!=opts.stopCount){return;}o.apply(next,[curr,next,opts,fwd]);});}var after=function(){opts.busy=0;$.each(opts.after,function(i,o){if(p.cycleStop!=opts.stopCount){return;}o.apply(next,[curr,next,opts,fwd]);});};debug("tx firing("+fx+"); currSlide: "+opts.currSlide+"; nextSlide: "+opts.nextSlide);opts.busy=1;if(opts.fxFn){opts.fxFn(curr,next,opts,after,fwd,manual&&opts.fastOnEvent);}else{if($.isFunction($.fn.cycle[opts.fx])){$.fn.cycle[opts.fx](curr,next,opts,after,fwd,manual&&opts.fastOnEvent);}else{$.fn.cycle.custom(curr,next,opts,after,fwd,manual&&opts.fastOnEvent);}}}if(changed||opts.nextSlide==opts.currSlide){opts.lastSlide=opts.currSlide;if(opts.random){opts.currSlide=opts.nextSlide;if(++opts.randomIndex==els.length){opts.randomIndex=0;}opts.nextSlide=opts.randomMap[opts.randomIndex];if(opts.nextSlide==opts.currSlide){opts.nextSlide=(opts.currSlide==opts.slideCount-1)?0:opts.currSlide+1;}}else{if(opts.backwards){var roll=(opts.nextSlide-1)<0;if(roll&&opts.bounce){opts.backwards=!opts.backwards;opts.nextSlide=1;opts.currSlide=0;}else{opts.nextSlide=roll?(els.length-1):opts.nextSlide-1;opts.currSlide=roll?0:opts.nextSlide+1;}}else{var roll=(opts.nextSlide+1)==els.length;if(roll&&opts.bounce){opts.backwards=!opts.backwards;opts.nextSlide=els.length-2;opts.currSlide=els.length-1;}else{opts.nextSlide=roll?0:opts.nextSlide+1;opts.currSlide=roll?els.length-1:opts.nextSlide-1;}}}}if(changed&&opts.pager){opts.updateActivePagerLink(opts.pager,opts.currSlide,opts.activePagerClass);}var ms=0;if(opts.timeout&&!opts.continuous){ms=getTimeout(els[opts.currSlide],els[opts.nextSlide],opts,fwd);}else{if(opts.continuous&&p.cyclePause){ms=10;}}if(ms>0){p.cycleTimeout=setTimeout(function(){go(els,opts,0,!opts.backwards);},ms);}}$.fn.cycle.updateActivePagerLink=function(pager,currSlide,clsName){$(pager).each(function(){$(this).children().removeClass(clsName).eq(currSlide).addClass(clsName);});};function getTimeout(curr,next,opts,fwd){if(opts.timeoutFn){var t=opts.timeoutFn.call(curr,curr,next,opts,fwd);while(opts.fx!="none"&&(t-opts.speed)<250){t+=opts.speed;}debug("calculated timeout: "+t+"; speed: "+opts.speed);if(t!==false){return t;}}return opts.timeout;}$.fn.cycle.next=function(opts){advance(opts,1);};$.fn.cycle.prev=function(opts){advance(opts,0);};function advance(opts,moveForward){var val=moveForward?1:-1;var els=opts.elements;var p=opts.$cont[0],timeout=p.cycleTimeout;if(timeout){clearTimeout(timeout);p.cycleTimeout=0;}if(opts.random&&val<0){opts.randomIndex--;if(--opts.randomIndex==-2){opts.randomIndex=els.length-2;}else{if(opts.randomIndex==-1){opts.randomIndex=els.length-1;}}opts.nextSlide=opts.randomMap[opts.randomIndex];}else{if(opts.random){opts.nextSlide=opts.randomMap[opts.randomIndex];}else{opts.nextSlide=opts.currSlide+val;if(opts.nextSlide<0){if(opts.nowrap){return false;}opts.nextSlide=els.length-1;}else{if(opts.nextSlide>=els.length){if(opts.nowrap){return false;}opts.nextSlide=0;}}}}var cb=opts.onPrevNextEvent||opts.prevNextClick;if($.isFunction(cb)){cb(val>0,opts.nextSlide,els[opts.nextSlide]);}go(els,opts,1,moveForward);return false;}function buildPager(els,opts){var $p=$(opts.pager);$.each(els,function(i,o){$.fn.cycle.createPagerAnchor(i,o,$p,els,opts);});opts.updateActivePagerLink(opts.pager,opts.startingSlide,opts.activePagerClass);}$.fn.cycle.createPagerAnchor=function(i,el,$p,els,opts){var a;if($.isFunction(opts.pagerAnchorBuilder)){a=opts.pagerAnchorBuilder(i,el);debug("pagerAnchorBuilder("+i+", el) returned: "+a);}else{a='<a href="#"><span>'+(i+1)+"</span></a>";}if(!a){return;}var $a=$(a);if($a.parents("body").length===0){var arr=[];if($p.length>1){$p.each(function(){var $clone=$a.clone(true);$(this).append($clone);arr.push($clone[0]);});$a=$(arr);}else{$a.appendTo($p);}}opts.pagerAnchors=opts.pagerAnchors||[];opts.pagerAnchors.push($a);$a.bind(opts.pagerEvent,function(e){e.preventDefault();opts.nextSlide=i;var p=opts.$cont[0],timeout=p.cycleTimeout;if(timeout){clearTimeout(timeout);p.cycleTimeout=0;}var cb=opts.onPagerEvent||opts.pagerClick;if($.isFunction(cb)){cb(opts.nextSlide,els[opts.nextSlide]);}go(els,opts,1,opts.currSlide<i);});if(!/^click/.test(opts.pagerEvent)&&!opts.allowPagerClickBubble){$a.bind("click.cycle",function(){return false;});}if(opts.pauseOnPagerHover){$a.hover(function(){opts.$cont[0].cyclePause++;},function(){opts.$cont[0].cyclePause--;});}};$.fn.cycle.hopsFromLast=function(opts,fwd){var hops,l=opts.lastSlide,c=opts.currSlide;if(fwd){hops=c>l?c-l:opts.slideCount-l;}else{hops=c<l?l-c:l+opts.slideCount-c;}return hops;};function clearTypeFix($slides){debug("applying clearType background-color hack");function hex(s){s=parseInt(s).toString(16);return s.length<2?"0"+s:s;}function getBg(e){for(;e&&e.nodeName.toLowerCase()!="html";e=e.parentNode){var v=$.css(e,"background-color");if(v&&v.indexOf("rgb")>=0){var rgb=v.match(/\d+/g);return"#"+hex(rgb[0])+hex(rgb[1])+hex(rgb[2]);}if(v&&v!="transparent"){return v;}}return"#ffffff";}$slides.each(function(){$(this).css("background-color",getBg(this));});}$.fn.cycle.commonReset=function(curr,next,opts,w,h,rev){$(opts.elements).not(curr).hide();if(typeof opts.cssBefore.opacity=="undefined"){opts.cssBefore.opacity=1;}opts.cssBefore.display="block";if(opts.slideResize&&w!==false&&next.cycleW>0){opts.cssBefore.width=next.cycleW;}if(opts.slideResize&&h!==false&&next.cycleH>0){opts.cssBefore.height=next.cycleH;}opts.cssAfter=opts.cssAfter||{};opts.cssAfter.display="none";$(curr).css("zIndex",opts.slideCount+(rev===true?1:0));$(next).css("zIndex",opts.slideCount+(rev===true?0:1));};$.fn.cycle.custom=function(curr,next,opts,cb,fwd,speedOverride){var $l=$(curr),$n=$(next);var speedIn=opts.speedIn,speedOut=opts.speedOut,easeIn=opts.easeIn,easeOut=opts.easeOut;$n.css(opts.cssBefore);if(speedOverride){if(typeof speedOverride=="number"){speedIn=speedOut=speedOverride;}else{speedIn=speedOut=1;}easeIn=easeOut=null;}var fn=function(){$n.animate(opts.animIn,speedIn,easeIn,function(){cb();});};$l.animate(opts.animOut,speedOut,easeOut,function(){$l.css(opts.cssAfter);if(!opts.sync){fn();}});if(opts.sync){fn();}};$.fn.cycle.transitions={fade:function($cont,$slides,opts){$slides.not(":eq("+opts.currSlide+")").css("opacity",0);opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts);opts.cssBefore.opacity=0;});opts.animIn={opacity:1};opts.animOut={opacity:0};opts.cssBefore={top:0,left:0};}};$.fn.cycle.ver=function(){return ver;};$.fn.cycle.defaults={activePagerClass:"activeSlide",after:null,allowPagerClickBubble:false,animIn:null,animOut:null,autostop:0,autostopCount:0,backwards:false,before:null,cleartype:!$.support.opacity,cleartypeNoBg:false,containerResize:1,continuous:0,cssAfter:null,cssBefore:null,delay:0,easeIn:null,easeOut:null,easing:null,end:null,fastOnEvent:0,fit:0,fx:"fade",fxFn:null,height:"auto",manualTrump:true,next:null,nowrap:0,onPagerEvent:null,onPrevNextEvent:null,pager:null,pagerAnchorBuilder:null,pagerEvent:"click.cycle",pause:0,pauseOnPagerHover:0,prev:null,prevNextEvent:"click.cycle",random:0,randomizeEffects:1,requeueOnImageNotLoaded:true,requeueTimeout:250,rev:0,shuffle:null,slideExpr:null,slideResize:1,speed:1000,speedIn:null,speedOut:null,startingSlide:0,sync:1,timeout:4000,timeoutFn:null,updateActivePagerLink:null};})(jQuery);(function($){$.fn.cycle.transitions.none=function($cont,$slides,opts){opts.fxFn=function(curr,next,opts,after){$(next).show();$(curr).hide();after();};};$.fn.cycle.transitions.fadeout=function($cont,$slides,opts){$slides.not(":eq("+opts.currSlide+")").css({display:"block",opacity:1});opts.before.push(function(curr,next,opts,w,h,rev){$(curr).css("zIndex",opts.slideCount+(!rev===true?1:0));$(next).css("zIndex",opts.slideCount+(!rev===true?0:1));});opts.animIn.opacity=1;opts.animOut.opacity=0;opts.cssBefore.opacity=1;opts.cssBefore.display="block";opts.cssAfter.zIndex=0;};$.fn.cycle.transitions.scrollUp=function($cont,$slides,opts){$cont.css("overflow","hidden");opts.before.push($.fn.cycle.commonReset);var h=$cont.height();opts.cssBefore.top=h;opts.cssBefore.left=0;opts.cssFirst.top=0;opts.animIn.top=0;opts.animOut.top=-h;};$.fn.cycle.transitions.scrollDown=function($cont,$slides,opts){$cont.css("overflow","hidden");opts.before.push($.fn.cycle.commonReset);var h=$cont.height();opts.cssFirst.top=0;opts.cssBefore.top=-h;opts.cssBefore.left=0;opts.animIn.top=0;opts.animOut.top=h;};$.fn.cycle.transitions.scrollLeft=function($cont,$slides,opts){$cont.css("overflow","hidden");opts.before.push($.fn.cycle.commonReset);var w=$cont.width();opts.cssFirst.left=0;opts.cssBefore.left=w;opts.cssBefore.top=0;opts.animIn.left=0;opts.animOut.left=0-w;};$.fn.cycle.transitions.scrollRight=function($cont,$slides,opts){$cont.css("overflow","hidden");opts.before.push($.fn.cycle.commonReset);var w=$cont.width();opts.cssFirst.left=0;opts.cssBefore.left=-w;opts.cssBefore.top=0;opts.animIn.left=0;opts.animOut.left=w;};$.fn.cycle.transitions.scrollHorz=function($cont,$slides,opts){$cont.css("overflow","hidden").width();opts.before.push(function(curr,next,opts,fwd){if(opts.rev){fwd=!fwd;}$.fn.cycle.commonReset(curr,next,opts);opts.cssBefore.left=fwd?(next.cycleW-1):(1-next.cycleW);opts.animOut.left=fwd?-curr.cycleW:curr.cycleW;});opts.cssFirst.left=0;opts.cssBefore.top=0;opts.animIn.left=0;opts.animOut.top=0;};$.fn.cycle.transitions.scrollVert=function($cont,$slides,opts){$cont.css("overflow","hidden");opts.before.push(function(curr,next,opts,fwd){if(opts.rev){fwd=!fwd;}$.fn.cycle.commonReset(curr,next,opts);opts.cssBefore.top=fwd?(1-next.cycleH):(next.cycleH-1);opts.animOut.top=fwd?curr.cycleH:-curr.cycleH;});opts.cssFirst.top=0;opts.cssBefore.left=0;opts.animIn.top=0;opts.animOut.left=0;};$.fn.cycle.transitions.slideX=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$(opts.elements).not(curr).hide();$.fn.cycle.commonReset(curr,next,opts,false,true);opts.animIn.width=next.cycleW;});opts.cssBefore.left=0;opts.cssBefore.top=0;opts.cssBefore.width=0;opts.animIn.width="show";opts.animOut.width=0;};$.fn.cycle.transitions.slideY=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$(opts.elements).not(curr).hide();$.fn.cycle.commonReset(curr,next,opts,true,false);opts.animIn.height=next.cycleH;});opts.cssBefore.left=0;opts.cssBefore.top=0;opts.cssBefore.height=0;opts.animIn.height="show";opts.animOut.height=0;};$.fn.cycle.transitions.shuffle=function($cont,$slides,opts){var i,w=$cont.css("overflow","visible").width();$slides.css({left:0,top:0});opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,true,true);});if(!opts.speedAdjusted){opts.speed=opts.speed/2;opts.speedAdjusted=true;}opts.random=0;opts.shuffle=opts.shuffle||{left:-w,top:15};opts.els=[];for(i=0;i<$slides.length;i++){opts.els.push($slides[i]);}for(i=0;i<opts.currSlide;i++){opts.els.push(opts.els.shift());}opts.fxFn=function(curr,next,opts,cb,fwd){if(opts.rev){fwd=!fwd;}var $el=fwd?$(curr):$(next);$(next).css(opts.cssBefore);var count=opts.slideCount;$el.animate(opts.shuffle,opts.speedIn,opts.easeIn,function(){var hops=$.fn.cycle.hopsFromLast(opts,fwd);for(var k=0;k<hops;k++){fwd?opts.els.push(opts.els.shift()):opts.els.unshift(opts.els.pop());}if(fwd){for(var i=0,len=opts.els.length;i<len;i++){$(opts.els[i]).css("z-index",len-i+count);}}else{var z=$(curr).css("z-index");$el.css("z-index",parseInt(z)+1+count);}$el.animate({left:0,top:0},opts.speedOut,opts.easeOut,function(){$(fwd?this:curr).hide();if(cb){cb();}});});};$.extend(opts.cssBefore,{display:"block",opacity:1,top:0,left:0});};$.fn.cycle.transitions.turnUp=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,false);opts.cssBefore.top=next.cycleH;opts.animIn.height=next.cycleH;opts.animOut.width=next.cycleW;});opts.cssFirst.top=0;opts.cssBefore.left=0;opts.cssBefore.height=0;opts.animIn.top=0;opts.animOut.height=0;};$.fn.cycle.transitions.turnDown=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,false);opts.animIn.height=next.cycleH;opts.animOut.top=curr.cycleH;});opts.cssFirst.top=0;opts.cssBefore.left=0;opts.cssBefore.top=0;opts.cssBefore.height=0;opts.animOut.height=0;};$.fn.cycle.transitions.turnLeft=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,true);opts.cssBefore.left=next.cycleW;opts.animIn.width=next.cycleW;});opts.cssBefore.top=0;opts.cssBefore.width=0;opts.animIn.left=0;opts.animOut.width=0;};$.fn.cycle.transitions.turnRight=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,true);opts.animIn.width=next.cycleW;opts.animOut.left=curr.cycleW;});$.extend(opts.cssBefore,{top:0,left:0,width:0});opts.animIn.left=0;opts.animOut.width=0;};$.fn.cycle.transitions.zoom=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,false,true);opts.cssBefore.top=next.cycleH/2;opts.cssBefore.left=next.cycleW/2;$.extend(opts.animIn,{top:0,left:0,width:next.cycleW,height:next.cycleH});$.extend(opts.animOut,{width:0,height:0,top:curr.cycleH/2,left:curr.cycleW/2});});opts.cssFirst.top=0;opts.cssFirst.left=0;opts.cssBefore.width=0;opts.cssBefore.height=0;};$.fn.cycle.transitions.fadeZoom=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,false);opts.cssBefore.left=next.cycleW/2;opts.cssBefore.top=next.cycleH/2;$.extend(opts.animIn,{top:0,left:0,width:next.cycleW,height:next.cycleH});});opts.cssBefore.width=0;opts.cssBefore.height=0;opts.animOut.opacity=0;};$.fn.cycle.transitions.blindX=function($cont,$slides,opts){var w=$cont.css("overflow","hidden").width();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts);opts.animIn.width=next.cycleW;opts.animOut.left=curr.cycleW;});opts.cssBefore.left=w;opts.cssBefore.top=0;opts.animIn.left=0;opts.animOut.left=w;};$.fn.cycle.transitions.blindY=function($cont,$slides,opts){var h=$cont.css("overflow","hidden").height();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts);opts.animIn.height=next.cycleH;opts.animOut.top=curr.cycleH;});opts.cssBefore.top=h;opts.cssBefore.left=0;opts.animIn.top=0;opts.animOut.top=h;};$.fn.cycle.transitions.blindZ=function($cont,$slides,opts){var h=$cont.css("overflow","hidden").height();var w=$cont.width();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts);opts.animIn.height=next.cycleH;opts.animOut.top=curr.cycleH;});opts.cssBefore.top=h;opts.cssBefore.left=w;opts.animIn.top=0;opts.animIn.left=0;opts.animOut.top=h;opts.animOut.left=w;};$.fn.cycle.transitions.growX=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,true);opts.cssBefore.left=this.cycleW/2;opts.animIn.left=0;opts.animIn.width=this.cycleW;opts.animOut.left=0;});opts.cssBefore.top=0;opts.cssBefore.width=0;};$.fn.cycle.transitions.growY=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,false);opts.cssBefore.top=this.cycleH/2;opts.animIn.top=0;opts.animIn.height=this.cycleH;opts.animOut.top=0;});opts.cssBefore.height=0;opts.cssBefore.left=0;};$.fn.cycle.transitions.curtainX=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,false,true,true);opts.cssBefore.left=next.cycleW/2;opts.animIn.left=0;opts.animIn.width=this.cycleW;opts.animOut.left=curr.cycleW/2;opts.animOut.width=0;});opts.cssBefore.top=0;opts.cssBefore.width=0;};$.fn.cycle.transitions.curtainY=function($cont,$slides,opts){opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,false,true);opts.cssBefore.top=next.cycleH/2;opts.animIn.top=0;opts.animIn.height=next.cycleH;opts.animOut.top=curr.cycleH/2;opts.animOut.height=0;});opts.cssBefore.height=0;opts.cssBefore.left=0;};$.fn.cycle.transitions.cover=function($cont,$slides,opts){var d=opts.direction||"left";var w=$cont.css("overflow","hidden").width();var h=$cont.height();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts);if(d=="right"){opts.cssBefore.left=-w;}else{if(d=="up"){opts.cssBefore.top=h;}else{if(d=="down"){opts.cssBefore.top=-h;}else{opts.cssBefore.left=w;}}}});opts.animIn.left=0;opts.animIn.top=0;opts.cssBefore.top=0;opts.cssBefore.left=0;};$.fn.cycle.transitions.uncover=function($cont,$slides,opts){var d=opts.direction||"left";var w=$cont.css("overflow","hidden").width();var h=$cont.height();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,true,true);if(d=="right"){opts.animOut.left=w;}else{if(d=="up"){opts.animOut.top=-h;}else{if(d=="down"){opts.animOut.top=h;}else{opts.animOut.left=-w;}}}});opts.animIn.left=0;opts.animIn.top=0;opts.cssBefore.top=0;opts.cssBefore.left=0;};$.fn.cycle.transitions.toss=function($cont,$slides,opts){var w=$cont.css("overflow","visible").width();var h=$cont.height();opts.before.push(function(curr,next,opts){$.fn.cycle.commonReset(curr,next,opts,true,true,true);if(!opts.animOut.left&&!opts.animOut.top){$.extend(opts.animOut,{left:w*2,top:-h/2,opacity:0});}else{opts.animOut.opacity=0;}});opts.cssBefore.left=0;opts.cssBefore.top=0;opts.animIn.left=0;};$.fn.cycle.transitions.wipe=function($cont,$slides,opts){var w=$cont.css("overflow","hidden").width();var h=$cont.height();opts.cssBefore=opts.cssBefore||{};var clip;if(opts.clip){if(/l2r/.test(opts.clip)){clip="rect(0px 0px "+h+"px 0px)";}else{if(/r2l/.test(opts.clip)){clip="rect(0px "+w+"px "+h+"px "+w+"px)";}else{if(/t2b/.test(opts.clip)){clip="rect(0px "+w+"px 0px 0px)";}else{if(/b2t/.test(opts.clip)){clip="rect("+h+"px "+w+"px "+h+"px 0px)";}else{if(/zoom/.test(opts.clip)){var top=parseInt(h/2);var left=parseInt(w/2);clip="rect("+top+"px "+left+"px "+top+"px "+left+"px)";}}}}}}opts.cssBefore.clip=opts.cssBefore.clip||clip||"rect(0px 0px 0px 0px)";var d=opts.cssBefore.clip.match(/(\d+)/g);var t=parseInt(d[0]),r=parseInt(d[1]),b=parseInt(d[2]),l=parseInt(d[3]);opts.before.push(function(curr,next,opts){if(curr==next){return;}var $curr=$(curr),$next=$(next);$.fn.cycle.commonReset(curr,next,opts,true,true,false);opts.cssAfter.display="block";var step=1,count=parseInt((opts.speedIn/13))-1;(function f(){var tt=t?t-parseInt(step*(t/count)):0;var ll=l?l-parseInt(step*(l/count)):0;var bb=b<h?b+parseInt(step*((h-b)/count||1)):h;var rr=r<w?r+parseInt(step*((w-r)/count||1)):w;$next.css({clip:"rect("+tt+"px "+rr+"px "+bb+"px "+ll+"px)"});(step++<=count)?setTimeout(f,13):$curr.css("display","none");})();});$.extend(opts.cssBefore,{display:"block",opacity:1,top:0,left:0});opts.animIn={left:0};opts.animOut={left:0};};})(jQuery);jQuery.easing['jswing']=jQuery.easing['swing'];jQuery.extend(jQuery.easing,{def:'easeOutQuad',swing:function(x,t,b,c,d){return jQuery.easing[jQuery.easing.def](x,t,b,c,d)},easeInQuad:function(x,t,b,c,d){return c*(t/=d)*t+b},easeOutQuad:function(x,t,b,c,d){return-c*(t/=d)*(t-2)+b},easeInOutQuad:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t+b;return-c/2*((--t)*(t-2)-1)+b},easeInCubic:function(x,t,b,c,d){return c*(t/=d)*t*t+b},easeOutCubic:function(x,t,b,c,d){return c*((t=t/d-1)*t*t+1)+b},easeInOutCubic:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t+b;return c/2*((t-=2)*t*t+2)+b},easeInQuart:function(x,t,b,c,d){return c*(t/=d)*t*t*t+b},easeOutQuart:function(x,t,b,c,d){return-c*((t=t/d-1)*t*t*t-1)+b},easeInOutQuart:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t*t+b;return-c/2*((t-=2)*t*t*t-2)+b},easeInQuint:function(x,t,b,c,d){return c*(t/=d)*t*t*t*t+b},easeOutQuint:function(x,t,b,c,d){return c*((t=t/d-1)*t*t*t*t+1)+b},easeInOutQuint:function(x,t,b,c,d){if((t/=d/2)<1)return c/2*t*t*t*t*t+b;return c/2*((t-=2)*t*t*t*t+2)+b},easeInSine:function(x,t,b,c,d){return-c*Math.cos(t/d*(Math.PI/2))+c+b},easeOutSine:function(x,t,b,c,d){return c*Math.sin(t/d*(Math.PI/2))+b},easeInOutSine:function(x,t,b,c,d){return-c/2*(Math.cos(Math.PI*t/d)-1)+b},easeInExpo:function(x,t,b,c,d){return(t==0)?b:c*Math.pow(2,10*(t/d-1))+b},easeOutExpo:function(x,t,b,c,d){return(t==d)?b+c:c*(-Math.pow(2,-10*t/d)+1)+b},easeInOutExpo:function(x,t,b,c,d){if(t==0)return b;if(t==d)return b+c;if((t/=d/2)<1)return c/2*Math.pow(2,10*(t-1))+b;return c/2*(-Math.pow(2,-10*--t)+2)+b},easeInCirc:function(x,t,b,c,d){return-c*(Math.sqrt(1-(t/=d)*t)-1)+b},easeOutCirc:function(x,t,b,c,d){return c*Math.sqrt(1-(t=t/d-1)*t)+b},easeInOutCirc:function(x,t,b,c,d){if((t/=d/2)<1)return-c/2*(Math.sqrt(1-t*t)-1)+b;return c/2*(Math.sqrt(1-(t-=2)*t)+1)+b},easeInElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d)==1)return b+c;if(!p)p=d*.3;if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return-(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b},easeOutElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d)==1)return b+c;if(!p)p=d*.3;if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);return a*Math.pow(2,-10*t)*Math.sin((t*d-s)*(2*Math.PI)/p)+c+b},easeInOutElastic:function(x,t,b,c,d){var s=1.70158;var p=0;var a=c;if(t==0)return b;if((t/=d/2)==2)return b+c;if(!p)p=d*(.3*1.5);if(a<Math.abs(c)){a=c;var s=p/4}else var s=p/(2*Math.PI)*Math.asin(c/a);if(t<1)return-.5*(a*Math.pow(2,10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p))+b;return a*Math.pow(2,-10*(t-=1))*Math.sin((t*d-s)*(2*Math.PI)/p)*.5+c+b},easeInBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;return c*(t/=d)*t*((s+1)*t-s)+b},easeOutBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;return c*((t=t/d-1)*t*((s+1)*t+s)+1)+b},easeInOutBack:function(x,t,b,c,d,s){if(s==undefined)s=1.70158;if((t/=d/2)<1)return c/2*(t*t*(((s*=(1.525))+1)*t-s))+b;return c/2*((t-=2)*t*(((s*=(1.525))+1)*t+s)+2)+b},easeInBounce:function(x,t,b,c,d){return c-jQuery.easing.easeOutBounce(x,d-t,0,c,d)+b},easeOutBounce:function(x,t,b,c,d){if((t/=d)<(1/2.75)){return c*(7.5625*t*t)+b}else if(t<(2/2.75)){return c*(7.5625*(t-=(1.5/2.75))*t+.75)+b}else if(t<(2.5/2.75)){return c*(7.5625*(t-=(2.25/2.75))*t+.9375)+b}else{return c*(7.5625*(t-=(2.625/2.75))*t+.984375)+b}},easeInOutBounce:function(x,t,b,c,d){if(t<d/2)return jQuery.easing.easeInBounce(x,t*2,0,c,d)*.5+b;return jQuery.easing.easeOutBounce(x,t*2-d,0,c,d)*.5+c*.5+b}});(function(b){var a=function(d){var c=b(this);if(c.data("resizetimer")){window.clearTimeout(c.data("resizetimer"))}c.data("resizetimer",window.setTimeout(function(){c.trigger("afterresize")},300))};b.event.special.afterresize={add:function(c){b(this).bind("resize",a);var d=c.handler;c.handler=function(e){return d.apply(this,arguments)}},remove:function(c){b(this).unbind("resize",a)}};b.fn.extend({afterresize:function(c){if(b.isFunction(c)){b(this).bind("afterresize",c)}else{b(this).trigger("afterresize")}return this}})})(jQuery);jQuery.fn.exists=function(){return jQuery(this).length>0;}}
(function(a){a.fn.swipe=function(c){if(!this){return false}var k={fingers:1,threshold:75,swipe:null,swipeLeft:null,swipeRight:null,swipeUp:null,swipeDown:null,swipeStatus:null,click:null,triggerOnTouchEnd:true,allowPageScroll:"auto"};var m="left";var l="right";var d="up";var s="down";var j="none";var u="horizontal";var q="vertical";var o="auto";var f="start";var i="move";var h="end";var n="cancel";var t="ontouchstart"in window,b=t?"touchstart":"mousedown",p=t?"touchmove":"mousemove",g=t?"touchend":"mouseup",r="touchcancel";var e="start";if(c.allowPageScroll==undefined&&(c.swipe!=undefined||c.swipeStatus!=undefined)){c.allowPageScroll=j}if(c){a.extend(k,c)}return this.each(function(){var D=this;var H=a(this);var E=null;var I=0;var x={x:0,y:0};var A={x:0,y:0};var K={x:0,y:0};function z(N){var M=t?N.touches[0]:N;e=f;if(t){I=N.touches.length}distance=0;direction=null;if(I==k.fingers||!t){x.x=A.x=M.pageX;x.y=A.y=M.pageY;if(k.swipeStatus){y(N,e)}}else{C(N)}D.addEventListener(p,J,false);D.addEventListener(g,L,false)}function J(N){if(e==h||e==n){return}var M=t?N.touches[0]:N;A.x=M.pageX;A.y=M.pageY;direction=v();if(t){I=N.touches.length}e=i;G(N,direction);if(I==k.fingers||!t){distance=B();if(k.swipeStatus){y(N,e,direction,distance)}if(!k.triggerOnTouchEnd){if(distance>=k.threshold){e=h;y(N,e);C(N)}}}else{e=n;y(N,e);C(N)}}function L(M){M.preventDefault();distance=B();direction=v();if(k.triggerOnTouchEnd){e=h;if((I==k.fingers||!t)&&A.x!=0){if(distance>=k.threshold){y(M,e);C(M)}else{e=n;y(M,e);C(M)}}else{e=n;y(M,e);C(M)}}else{if(e==i){e=n;y(M,e);C(M)}}D.removeEventListener(p,J,false);D.removeEventListener(g,L,false)}function C(M){I=0;x.x=0;x.y=0;A.x=0;A.y=0;K.x=0;K.y=0}function y(N,M){if(k.swipeStatus){k.swipeStatus.call(H,N,M,direction||null,distance||0)}if(M==n){if(k.click&&(I==1||!t)&&(isNaN(distance)||distance==0)){k.click.call(H,N,N.target)}}if(M==h){if(k.swipe){k.swipe.call(H,N,direction,distance)}switch(direction){case m:if(k.swipeLeft){k.swipeLeft.call(H,N,direction,distance)}break;case l:if(k.swipeRight){k.swipeRight.call(H,N,direction,distance)}break;case d:if(k.swipeUp){k.swipeUp.call(H,N,direction,distance)}break;case s:if(k.swipeDown){k.swipeDown.call(H,N,direction,distance)}break}}}function G(M,N){if(k.allowPageScroll==j){M.preventDefault()}else{var O=k.allowPageScroll==o;switch(N){case m:if((k.swipeLeft&&O)||(!O&&k.allowPageScroll!=u)){M.preventDefault()}break;case l:if((k.swipeRight&&O)||(!O&&k.allowPageScroll!=u)){M.preventDefault()}break;case d:if((k.swipeUp&&O)||(!O&&k.allowPageScroll!=q)){M.preventDefault()}break;case s:if((k.swipeDown&&O)||(!O&&k.allowPageScroll!=q)){M.preventDefault()}break}}}function B(){return Math.round(Math.sqrt(Math.pow(A.x-x.x,2)+Math.pow(A.y-x.y,2)))}function w(){var P=x.x-A.x;var O=A.y-x.y;var M=Math.atan2(O,P);var N=Math.round(M*180/Math.PI);if(N<0){N=360-Math.abs(N)}return N}function v(){var M=w();if((M<=45)&&(M>=0)){return m}else{if((M<=360)&&(M>=315)){return m}else{if((M>=135)&&(M<=225)){return l}else{if((M>45)&&(M<135)){return s}else{return d}}}}}try{this.addEventListener(b,z,false);this.addEventListener(r,C)}catch(F){}})}})(jQuery);

	return stack;
})(stacks.com_joeworkman_stacks_cycler3);


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


// Javascript for stacks_in_15_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_15_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_15_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_15_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_15_page0);


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


// Javascript for stacks_in_20_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_20_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_20_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(window).load(function(){var stack=$('#stacks_in_20_page0');$(document).keydown(function(e){var key_next;var key_prev;switch('scrollHorz'){case'scrollHorz':key_next=39;key_prev=37;break;case'scrollVert':key_next=40;key_prev=38;break;default:break;}switch(e.keyCode){case key_next:$('.cycler_reel',stack).cycle('next');break;case key_prev:$('.cycler_reel',stack).cycle('prev');break;default:break;}});function swipe(event,direction){var swipe_next;var swipe_prev;switch('scrollHorz'){case'scrollHorz':swipe_next='right';swipe_prev='left';break;case'scrollVert':swipe_next='down';swipe_prev='up';break;default:break;}switch(direction){case swipe_next:$('.cycler_reel',stack).cycle('next');break;case swipe_prev:$('.cycler_reel',stack).cycle('prev');break;default:break;}}
var swipe_options={allowPageScroll:"auto",threshold:150};swipe_options['fingers']=1;switch('scrollHorz'){case'scrollHorz':swipe_options['swipeLeft']=swipe;swipe_options['swipeRight']=swipe;break;case'scrollVert':swipe_options['swipeUp']=swipe;swipe_options['swipeDown']=swipe;break;default:break;}stack.parent().hover(function(){},function(){});var start_slide_stacks_in_20_page0=0;if(location.hash.match(/^#\d+$/)){start_slide_stacks_in_20_page0=parseInt(location.hash.substring(1));}
updateNavigation_stacks_in_20_page0=function(pager,currSlide,clsName){start_slide_stacks_in_20_page0=currSlide;$(pager).each(function(){$(this).children().removeClass(clsName).eq(currSlide).addClass(clsName);});};function resize_gallery_stacks_in_20_page0(){$('.cycler_reel',stack).cycle('pause');}
function build_gallery_stacks_in_20_page0(){$('.cycler_reel',stack).cycle('destroy').removeAttr('style');$('.cycler_reel > div.stacks_out',stack).removeAttr('style');$('#stacks_in_20_page0 .cycler_reel').cycle({fx:'scrollHorz',timeout:4000,timeoutFn:null,continuous:0,speed:1500,speedIn:1500,speedOut:1500,next:'#stacks_in_20_page0 .cycler_nav.next',prev:'#stacks_in_20_page0 .cycler_nav.prev',onPrevNextEvent:null,prevNextEvent:'click.cycle',pager:'#cycler_pager_stacks_in_20_page0',onPagerEvent:null,pagerEvent:'click.cycle',allowPagerClickBubble:false,pagerAnchorBuilder:null,before:null,after:null,end:null,easing:null,easeIn:null,easeOut:null,shuffle:null,animIn:null,animOut:null,cssBefore:null,cssAfter:null,fxFn:null,height:'auto',width:'100%',startingSlide:start_slide_stacks_in_20_page0,sync:1,random:0,fit:0,containerResize:1,pause:1,pauseOnPagerHover:true,autostop:0,autostopCount:0,delay:0,slideExpr:null,cleartype:!$.support.opacity,cleartypeNoBg:true,nowrap:0,fastOnEvent:0,randomizeEffects:0,rev:true,manualTrump:true,requeueOnImageNotLoaded:true,requeueTimeout:250,activePagerClass:'activeSlide',updateActivePagerLink:updateNavigation_stacks_in_20_page0,backwards:0}).swipe(swipe_options);var maxHeight=0;$('.cycler_reel > div.stacks_out',stack).each(function(){maxHeight=maxHeight>$(this).height()?maxHeight:$(this).height();});$('.cycler_reel,.cycler_reel > div.stacks_out',stack).height(maxHeight);}
stack.animate({opacity:100},500,function(){build_gallery_stacks_in_20_page0();$('.cycler_reel > div.stacks_out',stack).css('visibility','visible');stack.css('visibility','visible');});if(typeof(orientationEvent)==undefined){var orientationEvent=0;}
$(window).bind(orientationEvent,build_gallery_stacks_in_20_page0).bind('resize',resize_gallery_stacks_in_20_page0).afterresize(build_gallery_stacks_in_20_page0);});

	return stack;
})(stacks.stacks_in_20_page0);


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


// Javascript for stacks_in_64_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_64_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_64_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_64_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_64.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_64_page0);


// Javascript for stacks_in_406_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_406_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_406_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_406_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_406_page0);


// Javascript for stacks_in_410_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_410_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_410_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_410_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_410.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_410_page0);


// Javascript for stacks_in_422_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_422_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_422_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_422_page0);


// Javascript for stacks_in_408_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_408_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_408_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_408_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_408_page0);


// Javascript for stacks_in_412_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_412_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_412_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_412_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_412.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_412_page0);


// Javascript for stacks_in_423_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_423_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_423_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_423_page0);


// Javascript for stacks_in_492_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_492_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_492_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_492_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_492_page0);


// Javascript for stacks_in_494_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_494_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_494_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_494_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_494.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_494_page0);


// Javascript for stacks_in_496_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_496_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_496_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_496_page0);


// Javascript for stacks_in_457_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_457_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_457_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var jack=$("#stacks_in_457_page0 >.jack-wrapper");var trigger=$('>.jack,>.jack-back',jack);jack.hover(function(){trigger.addClass('hover');},function(){trigger.removeClass('hover');});});

	return stack;
})(stacks.stacks_in_457_page0);


// Javascript for stacks_in_459_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_459_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_459_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
var addEvent=function(){return document.addEventListener?function(a,c,d){if(a&&a.nodeName||a===window)a.addEventListener(c,d,!1);else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}:function(a,c,d){if(a&&a.nodeName||a===window)a.attachEvent("on"+c,function(){return d.call(a,window.event)});else if(a&&a.length)for(var b=0;b<a.length;b++)addEvent(a[b],c,d)}}();var responsiveEnhance=function(img,width,monitor){if(img.length){for(var i=0,len=img.length;i<len;i++){responsiveEnhance(img[i],width,monitor);}}else{if(((' '+img.className+' ').replace(/[\n\t]/g,' ').indexOf(' large ')==-1)&&img.clientWidth>width){var fullimg=new Image();addEvent(fullimg,'load',function(e){img.src=this.src;img.className+=' large';});fullimg.src=img.getAttribute('data-fullsrc');}
if(monitor!=false){addEvent(window,'resize',function(e){responsiveEnhance(img,width,false);});addEvent(img,'load',function(e){responsiveEnhance(img,width,false);});}}};function detectIE789(){var version;if(navigator.appName=='Microsoft Internet Explorer'){var ua=navigator.userAgent;var re=new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");if(re.exec(ua)!=null){version=parseFloat(RegExp.$1)}if(version<=8.0){return true}else{if(version==9.0){if(document.compatMode=="BackCompat"){var mA=document.createElement("meta");mA.content="IE=EmulateIE8";document.getElementsByTagName('head')[0].appendChild(mA);return true}else{return false}}return false}}else{return false}}
$(document).ready(function(){var image=$('#stacks_in_459_page0 img.imageStyle:first');var version=parseInt($.browser.version);if(!($.browser.msie&&version<=8)){var source=image.attr('src');var responsiveWidth=Math.round(image.attr('width')*0.65);responsiveWidth=responsiveWidth>500?500:responsiveWidth;image.attr('data-fullsrc',source);image.attr('src','files/thumb_459.png');responsiveEnhance(image,responsiveWidth);}var width=image.attr('width');image.css('max-width',width+'px');if(detectIE789())image.css('width','auto');});

	return stack;
})(stacks.stacks_in_459_page0);


// Javascript for stacks_in_461_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_461_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_461_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
jQuery.fn.exists=function(){return jQuery(this).length>0;}

	return stack;
})(stacks.stacks_in_461_page0);


// Javascript for stacks_in_74_page0
// ---------------------------------------------------------------------

// Each stack has its own object with its own namespace.  The name of
// that object is the same as the stack's id.
stacks.stacks_in_74_page0 = {};

// A closure is defined and assigned to the stack's object.  The object
// is also passed in as 'stack' which gives you a shorthand for referring
// to this object from elsewhere.
stacks.stacks_in_74_page0 = (function(stack) {

	// When jQuery is used it will be available as $ and jQuery but only
	// inside the closure.
	var jQuery = stacks.jQuery;
	var $ = jQuery;
	
$(document).ready(function(){var onload_stacks_in_74_page0=function(){var self=this;}
var tabulous=$('#tabulous-stacks_in_74_page0');tabulous.liquidSlider({autoHeight:true,minHeight:0,heightEaseFunction:"easeInOutExpo",heightEaseDuration:1500,slideEaseFunction:"animate.css",slideEaseFunctionFallback:"easeInOutExpo",slideEaseDuration:1500,fadeInDuration:1500,fadeOutDuration:1500,continuous:true,animateIn:"fadeInRightBig",animateOut:"fadeOutLeftBig",transitionEvent:"click",autoSlide:true,autoSlideDirection:"right",autoSlideInterval:15000,pauseOnHover:true,forceAutoSlide:false,dynamicArrows:false,hideSideArrows:false,hideSideArrowsDuration:750,hoverArrows:false,hoverArrowDuration:250,dynamicArrowsGraphical:false,dynamicArrowLeftText:"« previous",dynamicArrowRightText:"next »",dynamicTabs:true,dynamicTabsAlign:"center",dynamicTabsPosition:"top",panelTitleSelector:".tab-title",navElementTag:"div",includeTitle:true,dynamicTabsHtml:true,keyboardNavigation:true,leftKey:39,rightKey:37,panelKeys:false,preloader:false,crossLinks:true,firstPanelToLoad:1,hashLinking:true,hashTitleSelector:".tab-title",responsive:true,mobileNavigation:true,mobileNavDefaultText:'Overview'.replace(/\,/g,''),mobileUIThreshold:400,hideArrowsWhenMobile:false,hideArrowsThreshold:481,swipe:true,onload:onload_stacks_in_74_page0});});$(window).load(function(){var api=$.data($('#tabulous-stacks_in_74_page0')[0],'liquidSlider');api.options.autoHeight=true;api.adjustHeight(false,api.getHeight());});

	return stack;
})(stacks.stacks_in_74_page0);


