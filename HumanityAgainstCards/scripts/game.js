$(function () {
    $.connection.hub.start().done(function () {
        console.log("Connection succeeded...");
    });
});