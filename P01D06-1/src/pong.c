#include <stdio.h>

// Константы
#define WIDTH 80
#define HEIGHT 25
#define WIN_SCORE 21

// Глобальные переменные
int ball_x, ball_y, dir_x, dir_y;
int pos_left_racket, pos_right_racket;
int start_pos_left_racket, start_pos_right_racket;
int player1, player2;
int ball_moving = 0;  // Флаг для отслеживания, нужно ли двигать мяч

// Функции
void draw_screen() {
    printf("\033c");
    for (int y = 0; y < HEIGHT; y++) {
        for (int x = 0; x < WIDTH; x++) {
            if ((x == pos_left_racket && (y == start_pos_left_racket - 1 || y == start_pos_left_racket ||
                                          y == start_pos_left_racket + 1)) ||
                (x == pos_right_racket && (y == start_pos_right_racket - 1 || y == start_pos_right_racket ||
                                           y == start_pos_right_racket + 1))) {
                printf("|");
            } else if (x == ball_x && y == ball_y) {
                printf("o");
            } else if ((y == 0 && x != 0 && x != WIDTH - 1) ||
                       (y == HEIGHT - 1 && x != 0 && x != WIDTH - 1)) {
                printf("-");
            } else if ((x == 0 && y != 0 && y != HEIGHT - 1) ||
                       (x == WIDTH - 1 && y != 0 && y != HEIGHT - 1) || (x == WIDTH / 2 && y != 3)) {
                printf("|");
            } else if (x == WIDTH / 2 && y == 3) {
                printf("|");
            } else if (x == WIDTH / 2 - 4 && y == 3) {
                printf("%02d", player1);
                x++;
            } else if (x == WIDTH / 2 + 3 && y == 3) {
                printf("%02d", player2);
                x++;
            } else {
                printf(" ");
            }
        }
        printf("\n");
    }
}

void update_ball() {
    if (ball_y == 1 || ball_y == HEIGHT - 2) {
        dir_y = -dir_y;
    }
    if (ball_x == pos_left_racket + 1 &&
        (ball_y == start_pos_left_racket - 1 || ball_y == start_pos_left_racket ||
         ball_y == start_pos_left_racket + 1)) {
        dir_x = -dir_x;
    }
    if (ball_x == pos_right_racket - 1 &&
        (ball_y == start_pos_right_racket - 1 || ball_y == start_pos_right_racket ||
         ball_y == start_pos_right_racket + 1)) {
        dir_x = -dir_x;
    }

    ball_x += dir_x;
    ball_y += dir_y;
}

void check_score() {
    if (ball_x < 1) {
        player2++;
        ball_x = WIDTH / 2;
        ball_y = HEIGHT / 2;
    }
    if (ball_x > WIDTH - 2) {
        player1++;
        ball_x = WIDTH / 2;
        ball_y = HEIGHT / 2;
    }
}

void check_win() {
    if (player1 == WIN_SCORE) {
        printf("Победил 1-й игрок!\n");
        return;
    } else if (player2 == WIN_SCORE) {
        printf("Победил 2-й игрок!\n");
        return;
    }
}

void handle_input() {
    char key = getchar();
    if (key == '\n' || key == ' ') {
        ball_moving = 1;  // Разрешить движение мяча
    }
    if (key == 'a' && start_pos_left_racket > 1) {
        start_pos_left_racket--;
    } else if (key == 'z' && start_pos_left_racket < HEIGHT - 2) {
        start_pos_left_racket++;
    }
    if (key == 'k' && start_pos_right_racket > 1) {
        start_pos_right_racket--;
    } else if (key == 'm' && start_pos_right_racket < HEIGHT - 2) {
        start_pos_right_racket++;
    }
}

int main() {
    ball_x = WIDTH / 2;
    ball_y = HEIGHT / 2;
    dir_x = -1;
    dir_y = 1;
    pos_left_racket = 1;
    pos_right_racket = WIDTH - 2;
    start_pos_left_racket = HEIGHT / 2;
    start_pos_right_racket = HEIGHT / 2;
    player1 = 0;
    player2 = 0;

    while (1) {
        draw_screen();
        if (ball_moving) {
            update_ball();
            check_score();
            check_win();
            ball_moving = 0;  // Остановить движение мяча после обновления
        }
        handle_input();
    }

    return 0;
}