$(function () {
    var gameHub = $.connection.gameHub;
    var roomCode;
    var currentQuestion;

    function log(message) {
        $("#feed").append("<p>" + message + "</p>");
        console.log(message);
    }

    gameHub.client.playerJoined = function (name) {
        log("Player joined: " + name);
    };

    gameHub.client.roomCodeChanged = function (code) {
        log("Room code changed: " + code);
        $("#room-code").text(code);
        roomCode = code;
    };

    gameHub.client.newQuestion = function (question) {
        log("New Question: " + question.Value + " (" + question.BlankCount + " blanks)");

        currentQuestion = question;
        $("#question").text(question.Value.replace("_", "_______"));
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

            gameHub.server.createGame(playerName).then(function () {
                $(".cover").addClass("hidden");
            });;
        })

        $("#join-game").click(function () {
            var code = prompt("Enter room code:", "");
            var playerName = prompt("Enter a name:", "");

            if (code === "" || playerName === "") {
                return;
            }

            gameHub.server.joinGame(code, playerName).then(function () {
                $(".cover").addClass("hidden");
            });
        });

        $("#start").click(function () {
            gameHub.server.start(roomCode);
        });

        $(document).on("click", ".hand-card", function () {
            gameHub.server.submitCard(roomCode, $(this).text());
            console.log("Submitted card: " + $(this).text());
            $(this).addClass("hidden");
        })

        $(document).on("click", ".vote-card", function () {
            gameHub.server.submitVote(roomCode, $(this).text());
            console.log("Voted for: " + $(this).text());
            $(this).addClass("hidden");
        })
    })
});