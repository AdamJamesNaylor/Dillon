$.fn.random = function () {
    var ret = $();

    if (this.length > 0)
        ret = ret.add(this[Math.floor((Math.random() * this.length))]);

    return ret;
};

function flashGrids() {
    $("table.grid td").random().toggleClass("on");
}

$(document).ready(function () {
    var gridTimer = setInterval(flashGrids, 3000);
});