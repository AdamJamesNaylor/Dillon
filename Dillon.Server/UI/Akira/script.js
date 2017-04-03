var updatesQueue = [];

function calculateValue(element, pageX, pageY) {
    //todo check class for orientation, assume vertical for now
    element = $(element);

    var height = element.outerHeight();
    var y = height - (pageY - element.offset().top);
    y = y.clamp(0, height);

    var width = element.outerWidth();
    var x = width - (pageX - element.offset().left);
    x = x.clamp(0, width);

    return { x: x, y: y }
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

function updateElement(element, x, y) {
    //todo check element type and class for orientation, assume vertical UL for now
    switch (element.tagName) {
        case "BUTTON":

            break;
        case "UL":
            updateVerticalUL(element, y);
            break;
    }
}

function updateVerticalUL(ul, y) {
    ul = $(ul);
    var height = ul.outerHeight();
    var percent = Math.floor((y / height) * 100);
    var tenth = Math.ceil(percent / 10);

    var items = ul.children("li");

    for (var i = 0; i < 10; ++i) {
        $(items[i]).removeClass("disabled");
    }

    for (var i = 0; i < 10 - tenth; ++i) {
        $(items[i]).addClass("disabled");
    }

    var id = ul.data("id");
    $("h1[data-id='" + id + "'").text(percent);
}

function enqueueUpdate(id, x, y) {
    var lastUpdate = updatesQueue[updatesQueue.length - 1];
    if (lastUpdate && lastUpdate.id === id && lastUpdate.x === x && lastUpdate.y === y) {
        console.warn("skipping update due to duplication. Id: " + id + " " + x + "," + y + " queue length: " + updatesQueue.length);
        return;
    }

    console.log("Queued id: " + id + " x: " + x + " y: " + y);
    updatesQueue.push({ id: id, x: x, y: y });
}

function flushUpdatesQueue(queue) {
    if (queue.length <= 0)
        return;

    console.groupCollapsed("Flushing queue");

    var MAX_QUEUE_LENGTH = 20;
    var batchSize = queue.length;
    if (batchSize > MAX_QUEUE_LENGTH)
        console.warn("queue length == " + batchSize + ", should be less than " + MAX_QUEUE_LENGTH);

    var updates = "";
    for (var i = 0; i < batchSize; ++i) {
        var item = "updates[" + i + "]";
        updates += item + "[id]=" + queue[i].id + "&" + item + "[x]=" + queue[i].x + "&" + item + "[y]=" + queue[i].y + "&";
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
    console.groupEnd();
}

function respondToEvent(element, x, y) {
    var coords = calculateValue(element, x, y);
    var id = $(element).data("id");
    enqueueUpdate(id, coords.x, coords.y); //todo pull id from data attribute
    updateElement(element, coords.x, coords.y);

    return coords;
}

var ongoingTouches = [];

function initEvents() {
    var sliders = document.getElementsByTagName("ul");
    var buttons = document.getElementsByTagName("button");
    for (var i = 0; i < sliders.length; ++i) {
        sliders[i].addEventListener("touchstart", handleStart, false);
        sliders[i].addEventListener("touchmove", handleMove, false);
        sliders[i].addEventListener("touchcancel", handleCancel, false);
        sliders[i].addEventListener("touchend", handleEnd, false);
        
        sliders[i].addEventListener("mousedown", handleMouseDown, false);
        sliders[i].addEventListener("mousemove", handleMouseMove, false);
    }

    for (var i = 0; i < buttons.length; ++i) {
        buttons[i].addEventListener("touchstart", handleStart, false);
        buttons[i].addEventListener("mousedown", handleMouseDown, false);
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
    console.groupCollapsed("mousedown on element " + this + " with id: " + $(this).data("id"));
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

function printDiagnostics() {
    console.log("    _____  _ _ _             ");
    console.log("   |  __ \\(_) | |            ");
    console.log("   | |  | |_| | | ___  _ __  ");
    console.log("   | |  | | | | |/ _ \\| '_ \\ ");
    console.log("   | |__| | | | | (_) | | | |");
    console.log("   |_____/|_|_|_|\\___/|_| |_|");
    console.log("---------------------------------");
    console.log(navigator.appVersion);
}

$(document).ready(function () {
    printDiagnostics();

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