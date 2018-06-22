$(function () {
    var gameHub = $.connection.gameHub;

    gameHub.client.playerJoined = function (name) {
        console.log("Player joined: " + name);
        $("#feed").append("<p>" + name + " joined!</p>")
    };

    $.connection.hub.start().done(function () {
        console.log("Connection succeeded...");

        $("#create-game").click(function () {
            gameHub.server.createGame().done(function (roomCode) {
                alert("Your room code is: " + roomCode);
            });
        })

        $("#join-game").click(function () {
            var roomCode = prompt("Enter room code:", "");
            var playerName = prompt("Enter a name:", "");

            if (roomCode === "" || playerName == "") {
                return;
            }

            gameHub.server.joinGame(roomCode, playerName);
        });
    })
});