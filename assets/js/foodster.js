

/*============================================
Match height of header carousel to window height
==============================================*/
function matchCarouselHeight() {
    "use strict";
    // Adjust Header carousel .item height to same as window height
    var wH = $(window).height();
    $('#hero-carousel .item').css("height", wH);
}

/*====================================================================================================
Any JS inside $(window).load function is called when the window is ready and all assets are downloaded
======================================================================================================*/
$(window).load(function() {
    "use strict";

    // Remove loading screen when window is loaded after 1.5 seconds
    setTimeout(function() {
        $(window).trigger('resize');
        $('.loading-screen').fadeOut(); // fade out the loading-screen div
        if ( $('#video').length != 0 ) {
            // Play video once the page is fully loaded and loading screen is hidden
            $('#video').get(0).play();
        }
    },1500); // 1.5 second delay so that we avoid the 'flicker' of the loading screen showing for a split second and then hiding immediately when its not needed

    // Call function for Google Maps
    $('.restaurantPopUp').on('show.bs.modal', function (e) {
        // Call function for Google Maps when a modal is opened
        setTimeout(function() {
            loadGoogleMap();
        },300);   
    });

});

/*==================================================
Any JS inside $(window).resize(function() runs when the window is resized
====================================================*/

$(window).resize(function() {
    "use strict";
    // Call the matchCarouselheight() function when the window is resized
    matchCarouselHeight();
});

/*==================================================
Any JS inside $(window).scroll(function() runs when the window is scrolled
====================================================*/

$(window).scroll(function() {
    "use strict";
    if ($(this).scrollTop() > 100) {
        $('.scroll-up').fadeIn();
    } else {
        $('.scroll-up').fadeOut();
    }
});

/*==================================================
Any JS inside $(function() runs when jQuery is ready
====================================================*/
$(function() {
    "use strict";
    // We use strict mode to encounter errors when using JSHint/JSLint

    // Call matchCarouselHeight() function
    matchCarouselHeight();

    //Highlight the top nav as scrolling occurs
    $('body').scrollspy({
        target: '.navbar-shrink',
        offset: 85
    });

    // Smooth scrolling links - requires jQuery Easing plugin
    $('a.page-scroll').bind('click', function(event) {
        var $anchor = $(this);

        if ( $anchor.hasClass('header-scroll') ) {
            $('html, body').stop().animate({
                scrollTop: $($anchor.attr('href')).offset().top
            }, 1500, 'easeInOutExpo');
        }
        else{
            $('html, body').stop().animate({
                scrollTop: $($anchor.attr('href')).offset().top - 75
            }, 1500, 'easeInOutExpo');
        }
        event.preventDefault();
    });

    // Call the matchCarouselHeight() function when the carousel slide.bs event is triggered
    $('#hero-carousel').on('slide.bs.carousel', function () {
        matchCarouselHeight();
    });

    // Initialise WOW.js for section animation triggered when page scrolling
    new WOW().init();
});
