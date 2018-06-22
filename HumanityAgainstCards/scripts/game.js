$(function () {
    var gameHub = $.connection.gameHub;

    function log(message) {
        $("#feed").append("<p>" + message + "</p>");
        console.log(message);
    }

    gameHub.client.playerJoined = function (name) {
        log("Player joined: " + name);
    };

    gameHub.client.roomCodeChanged = function (roomCode) {
        log("Room code changed: " + roomCode);
        $("#room-code").text(roomCode);
    };

    $.connection.hub.start().done(function () {
        console.log("Connection succeeded...");

        $("#create-game").click(function () {
            var playerName = prompt("Enter a name:", "");

            if (playerName === "") {
                return;
            }

            gameHub.server.createGame(playerName);
        })

        $("#join-game").click(function () {
            var roomCode = prompt("Enter room code:", "");
            var playerName = prompt("Enter a name:", "");

            if (roomCode === "" || playerName === "") {
                return;
            }

            gameHub.server.joinGame(roomCode, playerName);
        });
    })
});