/**
 * Small utilities, not
 */
//define(['jquery', 'enquire'], function ($) {
  /**
   * Search mode with the transparent overlay over the whole site
   * @return {void}
   */
  (function () {
    $('.js--toggle-search-mode').on('click', function (ev) {
      ev.preventDefault();
      $('body').toggleClass('search-mode');
    });
  })();


  /**
   * Special helpers when we have a touch events
   */
  (function () {
    if ( 'ontouchstart' in document.documentElement ) {
      // add .touch to the <html>
      $( 'html' ).addClass( 'touch' );

      // mobile dropdowns
      $('.js--mobile-dropdown').on('click', function (ev) {
        ev.preventDefault();

        // remove all exiting visible dropdowns
        $(this)
          .siblings('.js--mobile-dropdown')
          .find('.show-menu')
          .removeClass('show-menu');


        // show the current dropdown
        $(this)
          .find('.dropdown-menu')
          .toggleClass('show-menu');
      });
    } else {
      // add .no-touch to the <html>
      $( 'html' ).addClass( 'no-touch' );
    }
  })();

  /**
   * Delete item from cart
   */
  (function () {
    $( '.js--remove-item' ).on( 'click', function( ev ) {
      var $this = $( this );
      $this.parents( $this.data('target') ).animate({
        opacity: 0
      }, 'swing', function() {
        $(this).slideUp();
      });
      return false;
    });
  })();

  /**
   * Thumbnail selector for the product
   */
  (function () {
    $( '.js--preview-thumbs a' ).click( function( ev ) {
      ev.preventDefault();
      $( $( this ).attr( 'href' ) ).attr( 'src', $( this ).data( 'src' ) );
      $( this ).parent().addClass( 'active' ).siblings( '.active' ).removeClass( 'active' );
    } );
  })();

  /**
   * When click on +/- the number should change
   */
  (function () {
    $( '.js--quantity > .js--clickable' ).click( function( ev ) {
      ev.preventDefault();
      var $input = $(this).siblings( 'input[type="text"]' );
      var number = parseInt( $input.val(), 10 );
      if ( isNaN( number ) ) {
        number = 1;
      }
      if ( $( this ).hasClass( 'js--plus-one' ) ) {
        $input.val( number + 1 );
      } else {
        number = number < 2 ? 2 : number;
        $input.val( number - 1 );
      }
    } );
  })();

  //  ==========
  //  = Highlight current date =
  //  ==========
  (function() {
    var
      timeTable = $('.js--timetable'),
      date = new Date();
    if( timeTable.length > 0 ) {
      date = date.getDay();
      timeTable.children('[data-day="'+date+'"]').addClass('today');
    }
  })();


  /**
   * Add the gradient to the jumbotron after the page is loaded
   */
  (function () {
    $( '.js--add-gradient' ).addClass( 'jumbotron--gradient' );
  })();
//});