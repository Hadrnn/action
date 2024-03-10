"""
Game states module
"""
from gamestateid import GameStateID
import spawner


class GameState:
    """
    Base game state
    """
    def update(self, arg):
        """
        Returns next game state
        """
        return GameStateID.QUIT


class MainMenuGameState(GameState):
    """
    Main menu
    """
    def __init__(self, window):
        print("Main menu")
        self.ID = GameStateID.MAINMENU
        self.window = window

    def update(self, arg):
        """
        One game tick
        """
        return GameStateID.QUIT


class OnlineLobbyGameState(GameState):
    """
    Online lobby
    """
    def __init__(self, window):
        print("Online lobby")
        self.ID = GameStateID.ONLINELOBBY
        self.window = window

    def update(self, arg):
        """
        One game tick
        """
        return GameStateID.ONLINELOBBY


class OnlinePlayingGameState(GameState):
    """
    Online playing
    """
    def __init__(self, window):
        print("Online playing")
        self.ID = GameStateID.ONLINEPLAYING
        self.window = window

    def update(self, arg):
        """
        One game tick
        """
        return GameStateID.ONLINEPLAYING


class SingleLobbyGameState(GameState):
    """
    Single-player lobby
    """
    def __init__(self, window):
        print("Single lobby")
        self.ID = GameStateID.SINGLELOBBY
        self.window = window

    def update(self, arg):
        """
        One game tick
        """
        return GameStateID.SINGLELOBBY


class SinglePlayingGameState(GameState):
    """
    Single-player
    """
    def __init__(self):
        print("Single playing")
        self.ID = GameStateID.SINGLEPLAYING

    def update(self, arg):
        """
        One game tick
        """
        return GameStateID.SINGLEPLAYING


def load_map(map_name):
    """
    Loads a game map from a file
    """
    with open("Maps/" + map_name + ".txt") as file:

        width = int(file.readline())
        height = int(file.readline())
        tank_amount = int(file.readline())
        map_spawner = spawner.MapSpawner()

        scene = []

        for i in range(height):
            line = file.readline()
            j = 0
            for block in line:
                scene.append(map_spawner.spawn((i, j), block))

        return scene
