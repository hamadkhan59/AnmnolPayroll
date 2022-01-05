$('.dropdown-submenu > a').click(function () {
    $(this).parent().children('.dropdown-menu').toggleClass('shown');
});