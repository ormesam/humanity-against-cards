$(function () {
    var gameHub = $.connection.gameHub;
    var room;

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
        room = roomCode;
    };

    gameHub.client.newQuestion = function (question) {
        log("New Question: " + question.Value + " (" + question.BlankCount + " blanks)");
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

        $("#start").click(function () {
            gameHub.server.start(room);
        });
    })
});