$(document).ready(function () {
    $('.nav-tabs > li a[title]').tooltip();

    //Wizard
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {

        var target = $(e.target);

        if (target.parent().hasClass('disabled')) {
            return false;
        }
    });

    $(".next-step").click(function (e) {

        var active = $('.wizard .nav-tabs li.active');
        active.next().removeClass('disabled');
        nextTab(active);

    });
    $(".prev-step").click(function (e) {

        var active = $('.wizard .nav-tabs li.active');
        prevTab(active);

    });

    $('#btnInstall').click(function () {

        var data = [];

        if ($('#presetModules').val()) {
            

            var modules = $('#presetModules').val();
            modules.forEach((o) => {
                data.push(o);
            });
        }

        $.ajax({
            url: '/System/Install',
            type: 'POST',
            data: JSON.stringify({ "modules": data }),
            contentType: "application/json",
            success: function () {
                window.location = '/Home/Index';
            }
        });
    });

    $('#btnChoosePlugin').click(function () {
        var modules = $('#presetModules').val();

        modules.forEach(function (o) {

            $('#summaryPanel').append("<div class='list-content'><a href='#listone' aria-expanded='false' aria-controls='listone'>" + o + "<i class='fa fa-chevron-down'></i></a></div>");
        });
    });
});

function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}


$('.nav-tabs').on('click', 'li', function () {
    $('.nav-tabs li.active').removeClass('active');
    $(this).addClass('active');
});

