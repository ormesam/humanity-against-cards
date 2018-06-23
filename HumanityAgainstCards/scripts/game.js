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

    gameHub.client.showHand = function (hand) {
        $("#hand").html("");

        for (var i = 0; i < hand.length; i++) {
            var card = $("<p>");
            card.text(hand[i].Value);
            card.addClass("hand-card");
            $("#hand").append(card);
        }
    };

    gameHub.client.showSelectedCards = function (cards) {
        $("#answers").html("");

        for (var i = 0; i < cards.length; i++) {
            var card = $("<p>");
            card.text(cards[i]);
            card.addClass("vote-card");
            $("#answers").append(card);
        }
    };

    gameHub.client.showWinningCard = function (player, card, votes) {
        log(player + " won! Card: " + card + " (Votes: " + votes + ")");
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

        $(document).on("click", ".hand-card", function () {
            gameHub.server.submitCard(room, $(this).text());
            console.log("Submitted card: " + $(this).text());
            $(this).addClass("hidden");
        })

        $(document).on("click", ".vote-card", function () {
            gameHub.server.submitVote(room, $(this).text());
            console.log("Voted for: " + $(this).text());
            $(this).addClass("hidden");
        })
    })
});