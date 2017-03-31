var updatesQueue = [];

function calculateValue(element, pageX, pageY) {
    //todo check class for orientation, assume vertical for now
    element = $(element);
    var height = element.outerHeight();
    var value = height - (pageY - element.offset().top);
    return value.clamp(0, height);
}

function submitValue(id, value) {
    var url = "/?id=" + id + "&value=" + value;
    console.log("submitting: " + url);

    $.ajax({
        method: "GET",
        url: url,
        cache: false, //don't let the browser fake the response
    })
    .done(function (msg) {
        console.log("submission done");
    })
    .fail(function (msg) {
        console.error(msg);
    });
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
        $(items[i]).removeClass("disabled");
    }

    for (var i = 0; i < 10 - tenth; ++i) {
        $(items[i]).addClass("disabled");
    }

    $("h1").text(percent);
}

function enqueueUpdate(id, value) {
    var lastUpdate = updatesQueue[updatesQueue.length - 1];
    if (lastUpdate && lastUpdate.id === id && lastUpdate.value === value)
        return;

    console.log("Adding value " + value + " to queue.");
    updatesQueue.push({ id: id, value: value });
}

function flushUpdatesQueue(queue) {
    if (queue.length <= 0)
        return;

    console.log("Flushing queue...");

    var MAX_QUEUE_LENGTH = 20;
    var batchSize = queue.length;
    if (batchSize > MAX_QUEUE_LENGTH)
        console.warn("queue length == " + batchSize + ", should be less than " + MAX_QUEUE_LENGTH);

    var updates = "";
    for (var i = 0; i < batchSize; ++i) {
        var item = "updates[" + i + "]";
        updates += item + "[id]=" + queue[i].id + "&" + item + "[value]=" + queue[i].value + "&";
    }
    queue.splice(0, batchSize); //new updates could have been added so only remove the batch we just processed
    updates = updates.slice(0, -1);
    var url = "/update?" + updates; //todo increment and send batch id
    console.log("Submitting " + batchSize + " updates to " + url);

    $.ajax({
        method: "GET",
        url: url,
        cache: false, //don't let the browser fake the response
    })
    .done(function (response) {
        console.log("Submission done for " + response + " updates.");
    })
    .fail(function (msg) {
        console.error(msg);
    });
}

function respondToEvent(element, x, y) {
    var value = calculateValue(element, x, y);
    enqueueUpdate(1, value); //todo pull id from data attribute
    //submitValue(1, value);
    updateElement(element, value);

    return value;
}

var ongoingTouches = [];

function initEvents() {
    var elements = document.getElementsByTagName("ul");
    for (var i = 0; i < elements.length; ++i) {
        elements[i].addEventListener("touchstart", handleStart, false);
        elements[i].addEventListener("touchmove", handleMove, false);
        elements[i].addEventListener("touchcancel", handleCancel, false);
        elements[i].addEventListener("touchend", handleEnd, false);

        elements[i].addEventListener("mousedown", handleMouseDown, false);
        elements[i].addEventListener("mousemove", handleMouseMove, false);
    }
    document.getElementsByTagName("body")[0].addEventListener("mouseup", handleMouseUp, false);
}

function handleStart(e) {
    console.groupCollapsed("touch event");
    e.preventDefault();
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        ongoingTouches.push(copyTouch(touches[i]));
        respondToEvent(this, touches[i].pageX, touches[i].pageY);
    }
}

function handleMove(e) {
    e.preventDefault();
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var idx = ongoingTouchIndexById(touches[i].identifier);

        if (idx < 0) {
            console.error("Couldn't find touch with identifier " + touches[i].identifier);
            return;
        }

        respondToEvent(this, touches[i].pageX, touches[i].pageY);

        ongoingTouches.splice(idx, 1, copyTouch(touches[i]));  // swap in the new touch record
    }
}

function handleCancel(e) {
    e.preventDefault();
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var idx = ongoingTouchIndexById(touches[i].identifier);
        ongoingTouches.splice(idx, 1);  // remove it; we're done
    }
}

function handleEnd(e) {
    e.preventDefault();
    var touches = e.changedTouches;

    for (var i = 0; i < touches.length; i++) {
        var idx = ongoingTouchIndexById(touches[i].identifier);

        if (idx < 0) {
            console.error("Couldn't find touch with identifier " + touches[i].identifier);
            return;
        }

        respondToEvent(this, touches[i].pageX, touches[i].pageY);

        ongoingTouches.splice(idx, 1); // remove it; we're done
    }
    //todo add code to reset batch counter (also add to mouse equivalent)
    console.log("touch ended");
    console.groupEnd();
}

var down = false;

function handleMouseDown(e) {
    console.groupCollapsed("mousedown");
    down = true;
    respondToEvent(this, e.pageX, e.pageY);
}

function handleMouseMove(e) {
    if (!down) return;
    respondToEvent(this, e.pageX, e.pageY);
}

function handleMouseUp(e) {
    down = false;
    console.log("mouse up");
    console.groupEnd();
}

$(document).ready(function () {
    updateVerticalUL($('ul'), 0);
    initEvents();

    var queueTimer = setInterval(flushUpdatesQueue, 100, updatesQueue);
});

function copyTouch(touch) {
    return { identifier: touch.identifier, pageX: touch.pageX, pageY: touch.pageY };
}

function ongoingTouchIndexById(idToFind) {
    for (var i = 0; i < ongoingTouches.length; i++) {
        var id = ongoingTouches[i].identifier;

        if (id == idToFind)
            return i;
    }
    return -1;    // not found
}

Number.prototype.clamp = function(min, max) {
    return Math.min(Math.max(this, min), max);
}