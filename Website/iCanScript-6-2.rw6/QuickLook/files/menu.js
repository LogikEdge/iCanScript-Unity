jQuery(document).ready(function($){
    $('#nav li:has(ul)').addClass('hasChild');

    $('#nav ul li').hover(
        function(){
            $('ul:first',this).show();
        },
        function(){
            $('ul:first',this).hide();
        }
    );
});
