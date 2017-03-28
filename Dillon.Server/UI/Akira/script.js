function update(ul, e) {
    var height = ul.outerHeight();
    var pos = e.pageY - ul.offset().top;
    var percent = Math.floor(((height - pos) / height) * 100);
    var tenth = Math.ceil(percent / 10);


    var items = ul.children("li");

    for (var i = 0; i < 10; ++i) {
        var item = items[i];
        $(item).removeClass('disabled');
    }

    for (var i = 0; i < 10 - tenth; ++i) {
        var item = items[i];
        $(item).addClass('disabled');
    }

    $("h1").text(percent);

    $.ajax({
        method: "GET",
        url: "/?id=1&value=" + percent,
        //data: { name: "John", location: "Boston" }
    })
    .done(function (msg) {

    })
    .fail(function(msg) {
        console.error(msg);
    });
}

$(document).ready(function () {

    update($('ul'), { pageY: 1120});

    var down = false;

    $('ul').mousedown(function (e) {
        down = true;
        var ul = $(this);
        update(ul, e);
    });

    $('body').mouseup(function (e) {
        down = false;
    });


    $('ul').mousemove(function (e) { //Offset mouse Position
        if (!down) return;
        var ul = $(this);
        update(ul, e);
    });

    $("button").bind("tap", function() {
        alert('test');
    });
});