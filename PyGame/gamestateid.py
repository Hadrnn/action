"""
Game state id module
"""

from enum import Enum


class GameStateID(Enum):
    """
    Enum with all game states
    """
    MAINMENU = 0
    ONLINELOBBY = 100
    ONLINEPLAYING = 101
    SINGLELOBBY = 200
    SINGLEPLAYING = 201
    QUIT = 300
