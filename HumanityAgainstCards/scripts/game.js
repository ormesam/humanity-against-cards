$(function () {
    var gameHub = $.connection.gameHub;

    $.connection.hub.start().done(function () {
        console.log("Connection succeeded...");

        $("#create-game").click(function () {
            gameHub.server.createGame().done(function (roomCode) {
                alert("Your room code is: " + roomCode);
            });
        })
    });
});