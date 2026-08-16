$(document).ready(e => {
    CreateSlider();
    CreateFestivalGallerySlider();
});

function mainInter() {
    $('span.more-text').click(function () {
        $(this).parent().parent().find('p').css('height', 'auto').addClass('show');
        $('span.less-text').show();
        $(this).hide();
    });

    $('span.less-text').click(function () {
        $(this).parent().parent().find('p').css('height', '50px').removeClass('show');
        $('span.more-text').show();
        $(this).hide();
    });

    $('.currency .drop-down span.current').click(function () {
        $(this).parent().find('ul').addClass('show');
        $('.language-switcher .drop-down ul').removeClass('show');
    });

    $('.language-switcher .drop-down span.current').click(function () {
        $(this).parent().find('ul').addClass('show');
        $('.currency .drop-down ul').removeClass('show');
    });

    // site drop down
    $('.drop-down li').click(function () {
        var new_label = $(this).html();
        var old_label = $(this).parent().parent().find('span.current').html();
        $(this).html(old_label);
        $(this).parent().parent().find('.current').html(new_label);

        $(this).parent().removeClass('show');
    });

    $('.detail-box .drop-dwon').click(function () {
        $(this).find('ul').addClass('open');
        $('.detail-box .drop-dwon').not(this).find('ul').removeClass('open');
    });

    $('.detail-box .drop-dwon li').click(function (e) {
        e.stopPropagation();
        var new_label = $(this).html();
        $(this).parent().parent().find('.current').html(new_label);
        $(this).parent().parent().parent().find('p.selected').html(new_label);
        $(this).parent().removeClass('open');
    });

    $('.overal-rating svg path').mouseenter(function () {
        $(this).parent().prevAll().addClass('active1');
        $(this).parent().addClass('active1');
    });

    $('.overal-rating svg').mouseout(function () {
        $(this).prevAll().removeClass('active1');
        $(this).removeClass('active1');
    });

    $('.overal-rating svg').click(function () {
        var data_rate = $(this).attr('data-rate');
        $(this).prevAll().addClass('active');
        $(this).nextAll().removeClass('active');
        $(this).addClass('active');
        $(this).parent().parent().find('.rate-number .rate').text(data_rate)
    });


    // mobile menu
    $('.side-menu-toggle').click(function () {

        $(".side-menu").addClass('show');
        $(".side-menu-bac").addClass('show');
    });

    $('.side-menu-bac').click(function () {
        $(".side-menu").removeClass('show');
        $(this).removeClass('show');
    });

    // video player
    $('.video-player').click(function () {
        $(".modal-video").fadeIn('fast');
        $(".modal-video-bac").fadeIn('fast');
    });

    $('.modal-video-bac').click(function () {
        $(".modal-video").fadeOut('fast');
        $(".modal-video video")[0].pause();
        $(".modal-video video")[0].currentTime = 0;
        $(this).fadeOut('fast');
    });


    // to top link
    var n = $("#to-top");
    n.on("click", function (n) {
        n.preventDefault();
        $("html, body").animate({scrollTop: 0}, "500")
    });

    $(window).scroll(function () {
        var $height = $(window).scrollTop();
        if ($height > 250) {
            $('.to-top').css({'opacity': '1', 'visibility': 'visible'});
        } else {
            $('.to-top').css({'opacity': '0', 'visibility': 'hidden'});
        }
    });
    $(window).on("scroll", function () {
        let scrollHeight = $(document).height();
        let scrollPosition = $(window).height() + $(window).scrollTop();
        if ((scrollHeight - scrollPosition) / scrollHeight === 0) {
            $('.to-top').css('bottom', '70px');
        } else {
            $('.to-top').css('bottom', '20px');
        }
    });


    //slide toggle menu
    $(".side-menu li.has-submenu").on('click', function (event) {
        event.stopPropagation();
        if (event.target !== this) {
            return;
        }
        if ($(this).find("li > ul").is(":visible")) {
            $(this).find("li > ul").slideUp();
            $(this).find("li").removeClass('open-sub-menu');
        }
        $(this).find('ul:first').slideToggle();
        $(this).toggleClass('open-sub-menu');

    });
}

function CreateSlider() {
    if (!($('.festivalMainSlider slick-slider')))
        $('.festivalMainSlider').slick({
            dots: false,
            infinite: true,
            speed: 300,
            slidesToShow: 1,
            autoplay: true,
            rtl: false,
            prevArrow: '<span class="slick-prev slick-arrow"><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.335" viewBox="0 0 7.667 13.335">\n' +
                '  <path id="Path_490" data-name="Path 490" d="M5.253,10.506,0,5.253,5.253,0" transform="translate(1 1.414)" fill="none" stroke="#fefefe" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
                '</svg>\n</span>',
            nextArrow: '<span class="slick-next slick-arrow"><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.334" viewBox="0 0 7.667 13.334">\n' +
                '  <path id="Path_489" data-name="Path 489" d="M-13023.766-9424.151l5.252,5.252-5.252,5.254" transform="translate(13025.18 9425.565)" fill="none" stroke="#fefefe" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
                '</svg>\n</span>',
        });
}

function CreateProjectImageSlider() {
}

function CreateMusicPlayerSlider() {
    if (!($('.slider.audio-player-slider slick-slider')))
        $('.slider.audio-player-slider').slick(
            {
                dots: false,
                infinite: false,
                speed: 300,
                slidesToShow: 1,
                autoplay: false,
                rtl: false,
                prevArrow: `<span color="#ff4d4d" class="slick-next slick-arrow">
                                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#ff4d4d" height="800px" width="800px" version="1.1" id="Capa_1" viewBox="0 0 489.789 489.789" xml:space="preserve">
                                    <g id="SVGRepo_bgCarrier" stroke-width="0"/>
                                    <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"/>
                                    <g id="SVGRepo_iconCarrier"> <g id="XMLID_105_"> <path id="XMLID_109_" d="M208.511,102.62c-6.6-6.623-16.58-8.608-25.222-5.022c-8.674,3.589-14.303,12.03-14.303,21.398v251.797 c0,9.368,5.629,17.809,14.303,21.397c2.853,1.187,5.864,1.762,8.849,1.762c6.025,0,11.958-2.353,16.373-6.784l125.906-125.9 c9.041-9.04,9.041-23.708,0-32.748L208.511,102.62z"/> <path id="XMLID_106_" d="M244.895,0C109.644,0,0,109.645,0,244.894c0,135.251,109.644,244.895,244.895,244.895 s244.894-109.644,244.894-244.895C489.788,109.645,380.146,0,244.895,0z M244.895,440.81c-108.034,0-195.915-87.89-195.915-195.916 S136.861,48.979,244.895,48.979S440.81,136.868,440.81,244.894S352.929,440.81,244.895,440.81z"/> </g> </g>
                                </svg>
                            \n</span>`,
                nextArrow: `<span class="slick-prev slick-arrow">
                                  <svg fill="#ff4d4d" height="200px" width="200px" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 489.789 489.789" xml:space="preserve" transform="rotate(180)">
                                       <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
                                       <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
                                       <g id="SVGRepo_iconCarrier">
                                       <g id="XMLID_105_"> <path id="XMLID_109_" d="M208.511,102.62c-6.6-6.623-16.58-8.608-25.222-5.022c-8.674,3.589-14.303,12.03-14.303,21.398v251.797 c0,9.368,5.629,17.809,14.303,21.397c2.853,1.187,5.864,1.762,8.849,1.762c6.025,0,11.958-2.353,16.373-6.784l125.906-125.9 c9.041-9.04,9.041-23.708,0-32.748L208.511,102.62z"></path> <path id="XMLID_106_" d="M244.895,0C109.644,0,0,109.645,0,244.894c0,135.251,109.644,244.895,244.895,244.895 s244.894-109.644,244.894-244.895C489.788,109.645,380.146,0,244.895,0z M244.895,440.81c-108.034,0-195.915-87.89-195.915-195.916 S136.861,48.979,244.895,48.979S440.81,136.868,440.81,244.894S352.929,440.81,244.895,440.81z"></path> </g> </g>
                                   </svg>
                             \n</span>`,
            });

}

function CreatePhotoSlider() {
    if(!($('.slider.project-photo-slider slick-slider'))){
        $('.slider.project-photo-slider')
            .slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 1,
            autoplay: true,
            rtl: false,
            prevArrow: `<span color="#ff4d4d" class="slick-next slick-arrow">
                             <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" fill="#ff4d4d" height="800px" width="800px" version="1.1" id="Capa_1" viewBox="0 0 489.789 489.789" xml:space="preserve">
                                 <g id="SVGRepo_bgCarrier" stroke-width="0"/>
                                 <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"/>
                                 <g id="SVGRepo_iconCarrier"> <g id="XMLID_105_"> <path id="XMLID_109_" d="M208.511,102.62c-6.6-6.623-16.58-8.608-25.222-5.022c-8.674,3.589-14.303,12.03-14.303,21.398v251.797 c0,9.368,5.629,17.809,14.303,21.397c2.853,1.187,5.864,1.762,8.849,1.762c6.025,0,11.958-2.353,16.373-6.784l125.906-125.9 c9.041-9.04,9.041-23.708,0-32.748L208.511,102.62z"/> <path id="XMLID_106_" d="M244.895,0C109.644,0,0,109.645,0,244.894c0,135.251,109.644,244.895,244.895,244.895 s244.894-109.644,244.894-244.895C489.788,109.645,380.146,0,244.895,0z M244.895,440.81c-108.034,0-195.915-87.89-195.915-195.916 S136.861,48.979,244.895,48.979S440.81,136.868,440.81,244.894S352.929,440.81,244.895,440.81z"/> </g> </g>
                             </svg>
                        \n</span>`,
            nextArrow: `<span class="slick-prev slick-arrow">
                              <svg fill="#ff4d4d" height="200px" width="200px" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 489.789 489.789" xml:space="preserve" transform="rotate(180)">
                                    <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
                                    <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
                                    <g id="SVGRepo_iconCarrier">
                                     <g id="XMLID_105_"> <path id="XMLID_109_" d="M208.511,102.62c-6.6-6.623-16.58-8.608-25.222-5.022c-8.674,3.589-14.303,12.03-14.303,21.398v251.797 c0,9.368,5.629,17.809,14.303,21.397c2.853,1.187,5.864,1.762,8.849,1.762c6.025,0,11.958-2.353,16.373-6.784l125.906-125.9 c9.041-9.04,9.041-23.708,0-32.748L208.511,102.62z"></path> <path id="XMLID_106_" d="M244.895,0C109.644,0,0,109.645,0,244.894c0,135.251,109.644,244.895,244.895,244.895 s244.894-109.644,244.894-244.895C489.788,109.645,380.146,0,244.895,0z M244.895,440.81c-108.034,0-195.915-87.89-195.915-195.916 S136.861,48.979,244.895,48.979S440.81,136.868,440.81,244.894S352.929,440.81,244.895,440.81z"></path> </g> </g>
                              </svg>
                       \n</span>`,
        });
    }
}

function CreateFestivalGallerySlider() {
    if(!($('.gallery-slider slick-slider'))){
        $('.gallery-slider').slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 7,
            autoplay: true,
            rtl: false,
            arrows: true,
            prevArrow: '<span class="slick-prev slick-arrow"><span><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.335" viewBox="0 0 7.667 13.335">\n' +
                '  <path id="Path_490" data-name="Path 490" d="M5.253,10.506,0,5.253,5.253,0" transform="translate(1 1.414)" fill="none" stroke="#051622" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
                '</svg>\n</span></span>',
            nextArrow: '<span class="slick-next slick-arrow"><span><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.334" viewBox="0 0 7.667 13.334">\n' +
                '  <path id="Path_489" data-name="Path 489" d="M-13023.766-9424.151l5.252,5.252-5.252,5.254" transform="translate(13025.18 9425.565)" fill="none" stroke="#051622" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
                '</svg>\n</span></span>',
            responsive: [
                {
                    breakpoint: 992,
                    settings: {
                        slidesToShow: 5,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 3,
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 2,
                    }
                }
            ]
        });
    }
}

async function clipboardCopy(text) {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return true;
        }

        const input = document.createElement('textarea');
        input.value = text;
        input.setAttribute('readonly', '');
        input.style.position = 'fixed';
        input.style.opacity = '0';
        document.body.appendChild(input);
        input.select();
        const copied = document.execCommand('copy');
        input.remove();
        return copied;
    } catch (error) {
        console.error('Clipboard copy failed', error);
        return false;
    }
}

async function shareLink(text, title) {
    if (navigator.share) {
        try {
            await navigator.share({ title: title || document.title, url: text });
            return true;
        } catch (error) {
            if (error?.name === 'AbortError') return false;
        }
    }

    return clipboardCopy(text);
}
