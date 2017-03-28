function calculateValue(element, pageX, pageY) {
    //todo check class for orientation, assume vertical for now
    element = $(element);
    var height = element.outerHeight();
    var value = height - (pageY - element.offset().top);
    console.log("calculated value: " + value);
    return value;
}

function updateElement(element, value) {
    //todo check element type and class for orientation, assume vertical UL for now
    updateVerticalUL(element, value);
}

function updateVerticalUL(ul, value) {
    ul = $(ul);
    var height = ul.outerHeight();
    var percent = Math.floor((value / height) * 100);
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

}

function submitValue(id, value) {
    var url = "/?id=" + id + "&value=" + value;
    console.log("submitting: " + url);

    $.ajax({
        method: "GET",
        url: url,
        //data: { name: "John", location: "Boston" }
    })
    .done(function (msg) {
        console.log("submission done");
    })
    .fail(function (msg) {
        console.error(msg);
    });
}

var ongoingTouches = [];

function initEvents() {
    var elements = document.getElementsByTagName("ul");
    for (var i = 0; i < elements.length; ++i) {
        elements[i].addEventListener("touchstart", handleStart, false);
        elements[i].addEventListener("touchmove", handleMove, false);
        //elements[i].addEventListener("touchcancel", handleCancel, false);
        //elements[i].addEventListener("touchend", handleEnd, false);

        elements[i].addEventListener("mousedown", handleMouseDown, false);
        elements[i].addEventListener("mousemove", handleMouseMove, false);
    }
    document.getElementsByTagName("body")[0].addEventListener("mouseup", handleMouseUp, false)
}

function handleStart(e) {
    console.groupCollapsed("touch event");
    e.preventDefault();
    var element = this;
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        ongoingTouches.push(copyTouch(touches[i]));
        var value = calculateValue(element, touches[i].pageX, touches[i].pageY);
        submitValue(1, value);
        updateElement(element, value);
    }
    console.log("touch ended"); //todo move this to touch end handler
    console.groupEnd();
}

function handleMove(e) {
    e.preventDefault();
    var element = this;
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var idx = ongoingTouchIndexById(touches[i].identifier);

        if (idx < 0) {
            console.error("Couldn't find touch with identifier " + touches[i].identifier);
            return;
        }

        var value = calculateValue(element, touches[i].pageX, touches[i].pageY);
        submitValue(1, value);
        updateElement(element, value);

        ongoingTouches.splice(idx, 1, copyTouch(touches[i]));  // swap in the new touch record
    }
}

var down = false;

function handleMouseDown(e) {
    console.groupCollapsed("mousedown");
    down = true;
    var value = calculateValue(this, e.pageX, e.pageY);
    updateElement(this, value);
    submitValue(1, value);
}

function handleMouseMove(e) {
    if (!down) return;
    var value = calculateValue(this, e.pageX, e.pageY);
    updateElement(this, value);
    submitValue(1, value);
}

function handleMouseUp(e) {
    down = false;
    console.log("mouse up");
    console.groupEnd();
}

$(document).ready(function () {

    updateVerticalUL($('ul'), 0);

    initEvents();
});

function copyTouch(touch) {
    return { identifier: touch.identifier, pageX: touch.pageX, pageY: touch.pageY };
}

function ongoingTouchIndexById(idToFind) {
    for (var i = 0; i < ongoingTouches.length; i++) {
        var id = ongoingTouches[i].identifier;

        if (id == idToFind) {
            return i;
        }
    }
    return -1;    // not found
}