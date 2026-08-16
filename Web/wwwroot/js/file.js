window.Download = (options) => {
    let fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;
    fetch(fileUrl)
        .then(response => response.blob())
        .then(blob => {
            let link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, {type: options.mimeType});
            link.download = options.fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}
function  DeleteLoadingAnimation(){
    $('#app').remove();
}

function ScrollToObject(selector){
    $([document.documentElement, document.body]).animate({
        scrollTop: $(`#${selector}`).offset().top
    }, 1000);
}
function  AddStyleToScrollMenu(){
    document.addEventListener('DOMContentLoaded', function () {
        let header = document.getElementById("main-header");
        let sticky = header.offsetTop;
        window.onscroll = function () {
            myFunction();
        };

        function myFunction() {
            if (window.scrollY > sticky) {
                header.classList.add("sticky");
            } else {
                header.classList.remove("sticky");
            }
        }
    })
}

$(document).ready((e)=>{
        CreateImageSlider();
        CreateOrganizerSlider();
    CreateProductImageSlider();
})

function  CreateImageSlider(){
    if($('#gallery-slider')) {
        $('#gallery-slider').not('.slick-initialized').slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 4,
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
                        slidesToShow: 3,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
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


function  CreateProductImageSlider(){
    if($('#product-gallery-slider2')) {
        $('#product-gallery-slider2').not('.slick-initialized').slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 3,
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
                        slidesToShow: 2,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
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
function CreateImageProjectSlider() {
    if ($('#gallery-project-slider').hasClass('slick-initialized')) {
        $('#gallery-project-slider').slick('unslick');
    }

   
        $('#gallery-project-slider').not('.slick-initialized').slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 4,
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
                        slidesToShow: 3,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
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
function CreatePhotographicImageSlider(checkNot = false) {
   /* $('.add-remove').slick('slickRemove', slideIndex - 1);*/
    //console.log('ddd')

    if ($('#photographic-project-slider').hasClass('slick-initialized')) {
        $('#photographic-project-slider').slick('unslick');
    }


    //$('#photographic-project-slider').slick('unslick');

    //if (checkNot) {
        $('#photographic-project-slider')
            .not('.slick-initialized')
            .slick({
                dots: false,
                infinite: false,
                speed: 300,
                slidesToShow: 1,
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
                            slidesToShow: 1,
                        }
                    },
                    {
                        breakpoint: 768,
                        settings: {
                            slidesToShow: 1,
                        }
                    },
                    {
                        breakpoint: 480,
                        settings: {
                            slidesToShow: 1,
                        }
                    }
                ]
            });
    //} else {
    //    $('#photographic-project-slider')
    //        .not('.slick-initialized')
    //        .slick({
    //            dots: false,
    //            infinite: false,
    //            speed: 300,
    //            slidesToShow: 1,
    //            autoplay: true,
    //            rtl: false,
    //            arrows: true,
    //            prevArrow: '<span class="slick-prev slick-arrow"><span><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.335" viewBox="0 0 7.667 13.335">\n' +
    //                '  <path id="Path_490" data-name="Path 490" d="M5.253,10.506,0,5.253,5.253,0" transform="translate(1 1.414)" fill="none" stroke="#051622" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
    //                '</svg>\n</span></span>',
    //            nextArrow: '<span class="slick-next slick-arrow"><span><svg xmlns="http://www.w3.org/2000/svg" width="7.667" height="13.334" viewBox="0 0 7.667 13.334">\n' +
    //                '  <path id="Path_489" data-name="Path 489" d="M-13023.766-9424.151l5.252,5.252-5.252,5.254" transform="translate(13025.18 9425.565)" fill="none" stroke="#051622" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"/>\n' +
    //                '</svg>\n</span></span>',
    //            responsive: [
    //                {
    //                    breakpoint: 992,
    //                    settings: {
    //                        slidesToShow: 1,
    //                    }
    //                },
    //                {
    //                    breakpoint: 768,
    //                    settings: {
    //                        slidesToShow: 1,
    //                    }
    //                },
    //                {
    //                    breakpoint: 480,
    //                    settings: {
    //                        slidesToShow: 1,
    //                    }
    //                }
    //            ]
    //        });
    //}
  
   
}
function  createAwardProjectSlider(){
    
    $('#award-project-slider').not('.slick-initialized').slick({
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 4,
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
                    slidesToShow: 4,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                }
            }
        ]
    });
}
function  CreateOrganizerSlider(){
    if($('#organizer-slider')) {
        $('#organizer-slider').not('.slick-initialized').slick({
            dots: false,
            infinite: false,
            speed: 300,
            slidesToShow: 2,
            autoplay: false,
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
                        slidesToShow: 3,
                    }
                },
                {
                    breakpoint: 768,
                    settings: {
                        slidesToShow: 2,
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                    }
                }
            ]
        });
    }
}


function  ScrollToElement(querySelector){
    console.log(querySelector);
    console.log($(querySelector))
    $("#fes-from").scroll();
    // let element= document.getElementById(querySelector);
    // console.log(element)
    // if(element){
    //    // element.scrollTop = element.scrollHeight - element.clientHeight;
    //     $(element).scroll();
    // }
}

function FindExtra() {
    // This function is diagnostic only. Removing wide elements here used to
    // delete overlays, tabs and dialog content and left the page unresponsive.
    const viewportWidth = document.documentElement.clientWidth;
    document.querySelectorAll('body *').forEach(el => {
        if (el.offsetWidth > viewportWidth + 2) {
            el.dataset.layoutOverflow = 'true';
        }
    });
}
