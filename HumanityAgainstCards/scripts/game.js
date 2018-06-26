$(function () {
    var gameHub = $.connection.gameHub;
    var roomCode;
    var currentQuestion;
    var answersSelected;
    var canVote;

    function createCard(id, text) {
        var card = $("<div>");
        var cardText = $("<p>");

        card.attr("data-id", id);
        card.addClass("card");
        cardText.html(text);

        card.append(cardText);

        return card;
    }

    function createVotingCard(id, textValues) {
        var card = $("<div>");
        card.attr("data-id", id);
        card.addClass("card");

        for (var i = 0; i < textValues.length; i++) {
            var text = textValues[i];
            var cardText = $("<p>");
            cardText.html(text);
            card.append(cardText);
        }

        return card;
    }

    gameHub.client.playerJoined = function (name) {
        console.log("Player joined: " + name);
    };

    gameHub.client.playerLeft = function (name) {
        console.log("Player left: " + name);
    };

    gameHub.client.roomCodeChanged = function (code) {
        console.log("Room code: " + code);
        $("#room-code").text(code);
        roomCode = code;
    };

    gameHub.client.newQuestion = function (question) {
        console.log("New Question: " + question.Value + " (" + question.BlankCount + " blanks)");

        currentQuestion = question;
        answersSelected = 0;
        canVote = true;

        $("#question").html(question.Value);
    };

    gameHub.client.showHand = function (hand) {
        $("#cards").html("");
        $("#card-header").text("Hand");

        for (var i = 0; i < hand.length; i++) {
            var card = createCard(hand[i].Id, hand[i].Value);
            card.addClass("card-hand");
            $("#cards").append(card);
        }
    };

    gameHub.client.showVotingCards = function (cards) {
        $("#cards").html("");
        $("#card-header").text("Vote for your favourite card!");

        for (var i = 0; i < cards.length; i++) {
            var card = createVotingCard(cards[i].Id, cards[i].Values);
            card.addClass("card-vote");
            $("#cards").append(card);
        }
    };

    gameHub.client.showWinningCard = function (player, cardId, votes) {
        canVote = false;

        $(".card-vote[data-id='" + cardId + "']").addClass("winning-card");
    };

    gameHub.client.updateLeaderboard = function (players) {
        $("#leaderboard").html("");

        for (var i = 0; i < players.length; i++) {
            var row = $("<tr>");
            var name = $("<td>");
            var points = $("<td>");

            name.text(players[i].Name);
            points.text(players[i].Points);

            row.append(name);
            row.append(points);
            $("#leaderboard").append(row);
        }
    };

    gameHub.client.setTimer = function (seconds) {
        $("#timer").text(seconds);
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

            var cardId = $(this).attr("data-id");

            gameHub.server.submitCard(roomCode, cardId);

            $(this).addClass("selected");

            console.log("Submitted card " + $(this).text());
        })

        $(document).on("click", ".card-vote", function () {
            if (!canVote) {
                return;
            }

            canVote = false;

            var cardId = $(this).attr("data-id");

            gameHub.server.submitVote(roomCode, cardId);

            $(this).addClass("selected");

            console.log("Voted for: " + $(this).text());
        })
    })
});