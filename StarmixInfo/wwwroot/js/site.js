'use strict';

// game loading
var gameInstance = null;
var loadGame = () => {
    gameInstance = UnityLoader.instantiate("gameContainer", "/game/Build/webGL.json", {
        onProgress: UnityProgress
    });
};

// game window hiding
if ($(window).width() < 768) {
    $('#central-hall').addClass('hide-game');
}