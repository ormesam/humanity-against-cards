﻿$(function () {
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
        cardText.text(text);

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
            cardText.text(text);
            card.append(cardText);
        }

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
        canVote = true;

        $("#question").text(question.Value);
    };

    gameHub.client.showHand = function (hand) {
        $("#hand").html("");
        $("#hand-section").removeClass("hidden");

        for (var i = 0; i < hand.length; i++) {
            var card = createCard(hand[i].Id, hand[i].Value);
            card.addClass("card-hand");
            $("#hand").append(card);
        }
    };

    gameHub.client.showVotingCards = function (cards) {
        $("#answers").html("");
        $("#hand-section").addClass("hidden");
        $("#voting-section").removeClass("hidden");

        for (var i = 0; i < cards.length; i++) {
            var card = createVotingCard(cards[i].Id, cards[i].Values);
            card.addClass("card-vote");
            $("#answers").append(card);
        }
    };

    gameHub.client.showWinningCard = function (player, card, votes) {
        canVote = false;

        console.log(player + " won! Card: " + card + " (Votes: " + votes + ")");
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

            console.log("Submitted card " + cardId);
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