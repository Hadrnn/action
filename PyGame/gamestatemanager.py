"""
Game state manager module
"""

from gamestateid import GameStateID
from gamestate import (MainMenuGameState, OnlineLobbyGameState,
                       OnlinePlayingGameState)


class StateManager:
    """
    Game state manager
    Manages transitions between game states
    """
    def __init__(self, window):
        self.window = window
        self.state = None

    def change_state(self, state_id):
        """
        Changes game state
        """
        if state_id == GameStateID.MAINMENU:
            self.state = MainMenuGameState(self.window)
        elif state_id == GameStateID.ONLINELOBBY:
            self.state = OnlineLobbyGameState(self.window)
        elif state_id == GameStateID.ONLINEPLAYING:
            self.state = OnlinePlayingGameState(self.window)
        elif state_id == GameStateID.SINGLELOBBY:
            pass
        elif state_id == GameStateID.SINGLEPLAYING:
            pass
        else:
            raise RuntimeError("Non-existant game state")

    def run(self):
        """
        Game state run
        """
        if self.state is None:
            raise RuntimeError("Did not initialize state manager")

        next_game_state_id = self.state.update(None)

        if next_game_state_id == GameStateID.QUIT:
            return False

        if next_game_state_id != self.state.ID:
            self.change_state(next_game_state_id)

        return True
