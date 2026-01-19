/**
 * AdminLTE Demo Menu
 * ------------------
 * You should not use this file in production.
 * This file is for demo purposes only.
 */

/* eslint-disable camelcase */
$( document ).ready(function() {
  // Carousel
  
  $(".carousel").carousel({
      interval: 5000,
      pause: true
  });

  $( ".carousel .carousel-inner" ).swipe( {
  swipeLeft: function ( event, direction, distance, duration, fingerCount ) {
      this.parent( ).carousel( 'next' );
  },
  swipeRight: function ( ) {
      this.parent( ).carousel( 'prev' );
  },
  threshold: 0,
  tap: function(event, target) {
      window.location = $(this).find('.carousel-item.active a').attr('href');
  },
  excludedElements:"label, button, input, select, textarea, .noSwipe"
  } );

  $('.carousel .carousel-inner').on('dragstart', 'a', function () {
      return false;
  });  

});


