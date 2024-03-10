import pygame
from gamestatemanager import StateManager
from gamestateid import GameStateID

if __name__ == "__main__":

    pygame.init()

    window = pygame.display.set_mode((500, 500))

    window.fill((0, 0, 0))

    manager = StateManager(window)
    manager.change_state(GameStateID.MAINMENU)

    run_app = True

    while run_app:
        run_app = manager.run()

    pygame.quit()
