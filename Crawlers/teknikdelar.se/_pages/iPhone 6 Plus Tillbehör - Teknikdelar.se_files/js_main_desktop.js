var topMenuStartTop = 0;

initMainDesktop();

function initMainDesktop()
{
	// Nothing
}

function footerNewsletterFormSubmit()
{
	$("#footerNewsletterForm").submit();
	return false;
}



$(".listmenu-opened .menu-node").click(function(){
	listMenuId = $(this).data('listmenu');
	if($("#listmenu-" + listMenuId + " .listmenu").is(":visible")) {
		$("#listmenu-" + listMenuId + " .listmenu").hide();
		$("#listmenu-" + listMenuId + " .menu-node-minus").removeClass('menu-node-minus').addClass('menu-node-plus');
	} else {
		$("#listmenu-" + listMenuId + " .listmenu").show();
		$("#listmenu-" + listMenuId + " .menu-node-plus").removeClass('menu-node-plus').addClass('menu-node-minus');
	}
	return false;
});

$(".js-menu-node").on('click', function(e) {
	var selectedItem = $(this).closest('li');

	$(".dropdown-mega__colums > ul > li").not(selectedItem).each(function() {
		if($(this).hasClass('is-selected')) {
			$(this).toggleClass('is-selected');
			$(this).find('.js-menu-node').toggleClass($(this).find('.js-menu-node').data('arrows'));
		}
	});

	selectedItem.toggleClass('is-selected');
	$(this).toggleClass($(this).data('arrows'));
	e.preventDefault();
});

// $(".dropdown-mega__colums > ul > li > a").on('mouseover', function(e) {
// 	$(this).closest('li').toggleClass('is-selected');
// 	$(this).find('.js-menu-node').toggleClass($(this).find('.js-menu-node').data('arrows'));
// 	e.preventDefault();
// });

/*$(".has-dropdown--mega").each(function() {
    var $this       = $(this),
        dropdown    = $this.find('ul:first'),
        pos         = $this.position(),
        menuWidth   = (dropdown.outerWidth() / 2) - parseInt($this.width() / 2),
        centerMenu  = parseInt(pos.left) - menuWidth;

    if(pos.left > menuWidth) {
        $(dropdown).css('left', centerMenu);
    } else {
        $(dropdown).css('left', 0);
    }
});*/

$(window).load(function() {
	// Fixed top menu after scrolling a bit
	try {
		$topmenu = $('.js-fixed-bar');
		topmenuStartTop = $topmenu.offset().top;
		topmenuHeight = $topmenu.outerHeight(true);
		topmenuWidth = $topmenu.width();

		topBarHeight = $('.l-menubar').height();
		topmenuStartTop = topmenuStartTop - topBarHeight;

		// Wrapping the element with a fixed height stops it from jumping 
		$topmenu.wrap('<div style="height:' + topmenuHeight +'px" />');

		if (topmenuStartTop <= $(document).scrollTop()) {
		    $topmenu.addClass('is-fixed-top');

		}

		$(document).scroll(function()
		{
	        if (topmenuStartTop <= $(document).scrollTop()) {
	            $topmenu.addClass('is-fixed-top');
	        }
	        else {
		        $topmenu.removeClass('is-fixed-top');
	        }

	    });
   }
   catch(err){}
});

