$(function () {
    var gameHub = $.connection.gameHub;
    var roomCode;
    var currentQuestion;
    var answersSelected;
    var hasVoted;

    function createCard(text) {
        var card = $("<div>");
        var cardText = $("<p>");

        card.addClass("card");
        cardText.text(text);

        card.append(cardText);

        return card;
    }

    gameHub.client.playerJoined = function (name) {
        console.log("Player joined: " + name);
    };

    gameHub.client.roomCodeChanged = function (code) {
        console.log("Room code: " + code);
        $("#room-code").text(code);
        roomCode = code;
    };

    gameHub.client.newQuestion = function (question) {
        $("#voting-section").addClass("hidden");
        console.log("New Question: " + question.Value + " (" + question.BlankCount + " blanks)");

        currentQuestion = question;
        answersSelected = 0;
        hasVoted = false;

        $("#question").text(question.Value);
    };

    gameHub.client.showHand = function (hand) {
        $("#hand").html("");
        $("#hand-section").removeClass("hidden");

        for (var i = 0; i < hand.length; i++) {
            var card = createCard(hand[i].Value);
            card.addClass("card-hand");
            $("#hand").append(card);
        }
    };

    gameHub.client.showSelectedCards = function (cards) {
        $("#answers").html("");
        $("#hand-section").addClass("hidden");
        $("#voting-section").removeClass("hidden");

        for (var i = 0; i < cards.length; i++) {
            var card = createCard(cards[i]);
            card.addClass("card-vote");
            $("#answers").append(card);
        }
    };

    gameHub.client.showWinningCard = function (player, card, votes) {
        console.log(player + " won! Card: " + card + " (Votes: " + votes + ")");
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

        $(document).on("click", ".card-hand", function () {
            if (answersSelected === currentQuestion.BlankCount) {
                return;
            }

            answersSelected++;

            var cardText = $(this).children("p").first().text();

            gameHub.server.submitCard(roomCode, cardText);

            $(this).addClass("selected");

            console.log("Submitted card: " + cardText);
        })

        $(document).on("click", ".card-vote", function () {
            if (hasVoted) {
                return;
            }

            hasVoted = true;

            gameHub.server.submitVote(roomCode, $(this).text());
            console.log("Voted for: " + $(this).text());
            $(this).addClass("selected");
        })
    })
});